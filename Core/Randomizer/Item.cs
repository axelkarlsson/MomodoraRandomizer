using LiveSplit.ComponentUtil;
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
        private IntPtr PtrToName;
        private IntPtr PtrToEffect;
        private IntPtr PtrToDescription;
        private IntPtr PtrToGotItem;
        internal DeepPointer DeepPointerToName { get; private set; }
        internal DeepPointer DeepPointerToEffect { get; private set; }
        internal DeepPointer DeepPointerToDescription { get; private set; }
        internal DeepPointer DeepPointerToGotItem { get; private set; }
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

            SetStringDeepPointers(process, gameVersion);
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
        public void SetStringDeepPointers(Process process, GameVersion gameVersion) {
            int[] StringsBase = ItemMetadata.GetStringsBase(gameVersion);

            // Item Name, Effect and Description
            int StringOffset = ItemMetadata.GetStringsOffset(Id);
            if (StringOffset != 0) {
                DeepPointerToName = PointerUtility.CreateDeepPointer(process, StringsBase.Concat(new[] { StringOffset, 0x0, 0x0 }).ToArray());
                DeepPointerToEffect = PointerUtility.CreateDeepPointer(process, StringsBase.Concat(new[] { StringOffset + 0x10, 0x0, 0x0 }).ToArray());
                DeepPointerToDescription = PointerUtility.CreateDeepPointer(process, StringsBase.Concat(new[] { StringOffset + 0x40, 0x0, 0x0 }).ToArray());
            }

            int StringPickupOffset = ItemMetadata.GetStringPickupOffset(Id, DropType);
            if (StringPickupOffset != 0) {
                StringsBase[0] -= 0x3C; // Adjust base pointer to the correct address for the item pickup string
                DeepPointerToGotItem = PointerUtility.CreateDeepPointer(process, StringsBase.Concat(new[] { StringPickupOffset, 0xC, 0x10, 0x0 }).ToArray());
            }

            string[] FakeStrings = ItemMetadata.GetFakeStrings(Id);
            if (FakeStrings.Length > 0) { // Fill any missing string if necessary
                FreeFakeStringPointers(); // Free strings to prevent memory leak

                if (DeepPointerToName == null && FakeStrings[0] != "") {
                    PtrToName = StringInjector.AllocateAnsiString(FakeStrings[0]);
                    DeepPointerToName = new DeepPointer(PtrToName);
                    IsStringFake[0] = true;
                }

                if (DeepPointerToEffect == null && FakeStrings[1] != "") {
                    PtrToEffect = StringInjector.AllocateAnsiString(FakeStrings[1]);
                    DeepPointerToEffect = new DeepPointer(PtrToEffect);
                    IsStringFake[1] = true;
                }

                if (DeepPointerToDescription == null && FakeStrings[2] != "") {
                    PtrToDescription = StringInjector.AllocateAnsiString(FakeStrings[2]);
                    DeepPointerToDescription = new DeepPointer(PtrToDescription);
                    IsStringFake[2] = true;
                }

                if (DeepPointerToGotItem == null && FakeStrings[3] != "") {
                    PtrToGotItem = StringInjector.AllocateAnsiString(FakeStrings[3]);
                    DeepPointerToGotItem = new DeepPointer(PtrToGotItem);
                    IsStringFake[3] = true;
                }
            }
        }

        public void FreeFakeStringPointers() {
            if (IsStringFake[0]) {
                StringInjector.FreeAnsiString(PtrToName);
                IsStringFake[0] = false;
            }

            if (IsStringFake[1]) {
                StringInjector.FreeAnsiString(PtrToEffect);
                IsStringFake[1] = false;
            }

            if (IsStringFake[2]) {
                StringInjector.FreeAnsiString(PtrToDescription);
                IsStringFake[2] = false;
            }

            if (IsStringFake[3]) {
                StringInjector.FreeAnsiString(PtrToGotItem);
                IsStringFake[3] = false;
            }
        }
        #endregion
    }
}