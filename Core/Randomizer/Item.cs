using MomodoraRandomizer.Core.Memory;
using MomodoraRandomizer.Data.Enums;
using MomodoraRandomizer.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MomodoraRandomizer.Core.Randomizer {
    internal class Item {
        #region Item Data
        internal ItemId Id { get; }
        internal int InstanceId { get; }
        internal ItemType Type { get; }
        internal ItemDropType DropType { get; }
        internal IntPtr PtrToName { get; private set; }
        internal IntPtr PtrToEffect { get; private set; }
        internal IntPtr PtrToDescription { get; private set; }
        internal IntPtr PtrToGotItem { get; private set; }
        internal List<int> SpawnRooms { get; }
        #endregion

        #region Randomizer Data
        internal Item GivenInsteadOf { get; set; } = null;
        internal Item GivenAs { get; set; } = null;
        internal bool[] IsStringFake { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        ///     Construct Item for the given <see cref="ItemId"/>
        /// </summary>
        /// <param name="itemId">Id if the item, represented as an enum</param>
        /// <param name="process">Process of the game</param>
        /// <param name="gameVersion">Version of the game</param>
        /// <param name="spawnIndex">Index to get which room it spawns in (from all possibilities), leave blank to use all of them</param>
        public Item(ItemId itemId, Process process, GameVersion gameVersion, int instanceId) {
            Id = itemId;
            
            InstanceId = instanceId;

            Type = ItemMetadata.GetItemType(Id);

            DropType = ItemMetadata.GetItemDropType(Id, InstanceId);

            SpawnRooms = ItemMetadata.GetItemSpawnRooms(Id, InstanceId).ToList();

            IsStringFake = new bool[] { false, false, false, false };

            SetStringPtrs(process, gameVersion);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        ///     Resets values and ItemReferences of the Item.
        /// </summary>
        public void Reset() {
            GivenInsteadOf = null;
            GivenAs = null;
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
                PtrToName = PointerUtility.CreatePointer(process, StringsBase.Concat(new[] { StringOffset, 0x0, 0x0 }).ToArray());
                PtrToEffect = PointerUtility.CreatePointer(process, StringsBase.Concat(new[] { StringOffset + 0x10, 0x0, 0x0 }).ToArray());
                PtrToDescription = PointerUtility.CreatePointer(process, StringsBase.Concat(new[] { StringOffset + 0x40, 0x0, 0x0 }).ToArray());
            }

            int StringPickupOffset = ItemMetadata.GetStringPickupOffset(Id, DropType);
            if (StringPickupOffset != 0) {
                StringsBase[0] -= 0x3C; // Adjust base pointer to the correct address for the item pickup string
                PtrToGotItem = PointerUtility.CreatePointer(process, StringsBase.Concat(new[] { StringPickupOffset, 0xC, 0x10, 0x0 }).ToArray());
            }

            string[] FakeStrings = ItemMetadata.GetFakeStrings(Id);
            if (FakeStrings.Length > 0) { // Fill any missing string if necessary
                if (PtrToName == IntPtr.Zero && FakeStrings[0] != "") {
                    PtrToName = StringInjector.AllocateAnsiString(FakeStrings[0]);
                    IsStringFake[0] = true;
                }

                if (PtrToEffect == IntPtr.Zero && FakeStrings[1] != "") {
                    PtrToEffect = StringInjector.AllocateAnsiString(FakeStrings[1]);
                    IsStringFake[1] = true;
                }

                if (PtrToDescription == IntPtr.Zero && FakeStrings[2] != "") {
                    PtrToDescription = StringInjector.AllocateAnsiString(FakeStrings[2]);
                    IsStringFake[2] = true;
                }

                if (PtrToGotItem == IntPtr.Zero && FakeStrings[3] != "") {
                    PtrToGotItem = StringInjector.AllocateAnsiString(FakeStrings[3]);
                    IsStringFake[3] = true;
                }
            }
        }
        #endregion
    }
}