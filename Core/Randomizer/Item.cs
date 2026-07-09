using MomodoraRandomizer.Core.Memory;
using MomodoraRandomizer.Data.Enums;
using MomodoraRandomizer.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MomodoraRandomizer.Core.Randomizer;

internal class Item {
    #region Item Data
    internal ItemId Id { get; }
    internal ItemType Type => ItemMetadata.Types[Id];
    internal ItemDropType DropType { get; }
    internal IntPtr PtrToName { get; private set; }
    internal IntPtr PtrToEffect { get; private set; }
    internal IntPtr PtrToDescription { get; private set; }
    internal IntPtr PtrToGotItem { get; private set; }
    internal List<int> SpawnRooms { get; } = [];
    internal List<ItemId> Requirements { get; } = [];
    #endregion

    #region Randomizer Data
    internal Item TransformsFrom { get; set; }
    internal Item TransformsInto { get; set; }
    internal bool[] IsStringFake { get; private set; } = [];
    #endregion

    #region Constructors
    /// <summary>
    ///     Construct Item for the given <see cref="ItemId"/>
    /// </summary>
    /// <param name="itemId">Id if the item, represented as an enum</param>
    /// <param name="process">Process of the game</param>
    /// <param name="gameVersion">Version of the game</param>
    /// <param name="spawnIndex">Index to get which room it spawns in (from all possibilities), leave blank to use all of them</param>
    public Item(ItemId itemId, Process process, GameVersion gameVersion, int spawnIndex = -1) {
        Id = itemId;
        DropType = ItemMetadata.GetDropType(Id, spawnIndex == -1 ? 0 : spawnIndex); // If index is -1 then that means there's only one item instance

        SpawnRooms = [.. ItemMetadata.GetItemSpawnRooms(Id, spawnIndex)];

        IsStringFake = [false, false, false, false];

        SetStringPtrs(process, gameVersion);

        TransformsFrom = this;
        TransformsInto = this;
    }
    #endregion

    #region Instance Methods
    /// <summary>
    ///     Resets values and ItemReferences of the Item.
    /// </summary>
    public void Reset() {
        TransformsFrom = this;
        TransformsInto = this;
        ResetRequirements();
    }

    /// <summary>
    ///     Assign values for the item string pointers, or generate its own pointers if necessary
    /// </summary>
    /// <param name="process">Process of the game</param>
    /// <param name="gameVersion">Version of the game</param>
    public void SetStringPtrs(Process process, GameVersion gameVersion) {
        int[] StringsBase = ItemMetadata.GetStringsBase(gameVersion);

        // Item Name, Effect and Description
        int StringOffset = ItemMetadata.GetStringsOffset(Id);
        if (StringOffset != 0) {
            PtrToName = PointerUtility.CreatePointer(process, [.. StringsBase, StringOffset, 0x0, 0x0]);
            PtrToEffect = PointerUtility.CreatePointer(process, [.. StringsBase, StringOffset + 0x10, 0x0, 0x0]);
            PtrToDescription = PointerUtility.CreatePointer(process, [.. StringsBase, StringOffset + 0x40, 0x0, 0x0]);
        }

        int StringPickupOffset = ItemMetadata.GetStringPickupOffset(Id, DropType);
        if (StringPickupOffset != 0) {
            StringsBase[0] -= 0x3C; // Adjust base pointer to the correct address for the item pickup string
            PtrToGotItem = PointerUtility.CreatePointer(process, [.. StringsBase, StringPickupOffset, 0xC, 0x10, 0x0]);
        }

        string[] FakeStrings = ItemMetadata.FakeStrings[Id] ?? [];
        if (FakeStrings.Length > 0) { // Fill any missing string if necessary
            if (PtrToName == 0) {
                PtrToName = StringInjector.AllocateAnsiString(FakeStrings[0] ?? "");
                IsStringFake[0] = true;
            }

            if (PtrToEffect == 0) {
                PtrToEffect = StringInjector.AllocateAnsiString(FakeStrings[1] ?? "");
                IsStringFake[1] = true;
            }

            if (PtrToDescription == 0) {
                PtrToDescription = StringInjector.AllocateAnsiString(FakeStrings[2] ?? "");
                IsStringFake[2] = true;
            }

            if (PtrToGotItem == 0) {
                PtrToGotItem = StringInjector.AllocateAnsiString(FakeStrings[3] ?? "");
                IsStringFake[3] = true;
            }
        }
    }

    /// <summary>
    ///     Adds items to the requirement lists.
    /// </summary>
    private void AddRequirements(ItemId itemId)
        => Requirements.AddRange(ItemMetadata.GetRquirements(itemId));

    /// <summary>
    ///     Clears requirement lists.
    /// </summary>
    private void ClearRequirements()
        => Requirements.Clear();

    /// <summary>
    ///     Resets the value of the requirement list.
    /// </summary>
    public void ResetRequirements() {
        ClearRequirements();
        AddRequirements(Id);
    }
    #endregion
}