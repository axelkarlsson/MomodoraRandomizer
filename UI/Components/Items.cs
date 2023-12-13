using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.UI.Components
{
    internal class Items
    {
        private const string V1 = "1.05b", V2 = "1.07";

        public static List<Items> List = new List<Items>();
        private static string Version;
        private static Process process;

        public ItemName ItemName { get; }
        private IntPtr ValuePtr;
        public Items ItemReference { get; set; }

        /*
        private int[] StringBaseOffsetList;
        private int[] StringOffsetList;
        
        public string Name { get; }
        public string Effect { get; }
        public string Description { get; }
        */

        #region Constructor
        public Items(ItemName itemName, string version, Process processRef)
        {
            switch (version)
            {
                case V1:
                    Version = V1;
                    break;
                case V2:
                    Version = V2;
                    break;
                default:
                    Version = "";
                    break;
            }

            this.ItemName = itemName;

            process = processRef;

            this.ValuePtr = PointerUtility.CreatePointer(process, GetValueBase().Concat(GetValueOffsets()).ToArray());
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Reset values and ItemReference in ItemList
        /// </summary>
        static public void Reset()
        {
            foreach (Items item in List)
            {
                item.ItemReference = null;
                item.ResetValue();
            }
        }

        /// <summary>
        /// Set Value of Items in ItemList to their ItemReference's Value
        /// 
        /// </summary>
        static public void SetValues()
        {
            foreach (Items item in List)
            {
                item.SetValue();
            }
        }

        /// <summary>
        /// Set Value of Items in ItemList to their original Value (ItemName)
        /// </summary>
        static public void ResetValues()
        {
            foreach (Items item in List)
            {
                item.ResetValue();
            }
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Get Value Base of item
        /// </summary>
        /// <returns>
        ///     Offsets base if Version is valid<br/>
        ///     <c>null</c> otherwise
        /// </returns>
        private int[] GetValueBase()
        {
            switch (Version)
            {
                case V1: return new int[] { 0x2304CE8, 0x4 };
                case V2: return new int[] { 0x2371EA8, 0x4 };
                default: return null;
            }
        }

        /// <summary>
        /// Get Value Offsets of item
        /// </summary>
        /// <returns>
        ///     Offsets value if Version and ItemName are valid<br/>
        ///     <c>null</c> otherwise
        /// </returns>
        private int[] GetValueOffsets()
        {
            switch(Version)
            {
                case V1:
                case V2:
                    switch (this.ItemName)
                    {
                        case ItemName.ADORNED_RING: return new int[] { 0xC70 };
                        case ItemName.NECKLACE_OF_SACRIFICE: return new int[] { 0xC80 };
                        //case ItemName.BELLFLOWER: return new int[] { 0x };
                        case ItemName.ASTRAL_CHARM: return new int[] { 0x70 };
                        case ItemName.EDEAS_PEARL: return new int[] { 0x40 };
                        case ItemName.DULL_PEARL: return new int[] { 0x50 };
                        case ItemName.RED_RING: return new int[] { 0x80 };
                        case ItemName.MAGNET_STONE: return new int[] { 0xCA0 };
                        case ItemName.ROTTEN_BELLFLOWER: return new int[] { 0xCB0 };
                        case ItemName.FAERIE_TEAR: return new int[] { 0xCC0 };
                        case ItemName.IMPURITY_FLASK: return new int[] { 0xCE0 };
                        case ItemName.PASSIFLORA: return new int[] { 0xCF0 };
                        case ItemName.CRYTAL_SEED: return new int[] { 0xD00 };
                        case ItemName.MEDAL_OF_EQUIVALENCE: return new int[] { 0xD10 };
                        case ItemName.TAINTED_MISSIVE: return new int[] { 0xD20 };
                        case ItemName.BLACK_SACHET: return new int[] { 0xD30 };
                        case ItemName.RING_OF_CANDOR: return new int[] { 0xD60 };
                        case ItemName.SMALL_COIN: return new int[] { 0xD70 };
                        case ItemName.BAKMAN_PATCH: return new int[] { 0xD80 };
                        case ItemName.CAT_SPHERE: return new int[] { 0xD90 };
                        case ItemName.HAZLE_BADGE: return new int[] { 0xDA0 };
                        case ItemName.TORN_BRANCH: return new int[] { 0x90 };
                        case ItemName.MONASTERY_KEY: return new int[] { 0xDB0 };
                        case ItemName.CLARITY_SHARD: return new int[] { 0xDE0 };
                        case ItemName.DIRTY_SHROOM: return new int[] { 0xDF0 };
                        case ItemName.IVORY_BUG: return new int[] { 0xE00 };
                        case ItemName.VIOLET_SPRITE: return new int[] { 0xE10 };
                        case ItemName.SOFT_TISSUE: return new int[] { 0xE20 };
                        case ItemName.GARDEN_KEY: return new int[] { 0xE30 };
                        case ItemName.SPARSE_THREAD: return new int[] { 0xE40 };
                        case ItemName.BLESSING_CHARM: return new int[] { 0xE50 };
                        case ItemName.HEAVY_ARROWS: return new int[] { 0xE60 };
                        case ItemName.BLOODSTAINED_TISSUE: return new int[] { 0xE70 };
                        case ItemName.MAPLE_LEAF: return new int[] { 0xE80 };
                        case ItemName.FRESH_SPRING_LEAF: return new int[] { 0xE90 };
                        case ItemName.POCKET_INCENSORY: return new int[] { 0xB0 };
                        case ItemName.BIRTHSTONE: return new int[] { 0xEA0 };
                        case ItemName.QUICK_ARROWS: return new int[] { 0xEB0 };
                        case ItemName.DRILLING_ARROWS: return new int[] { 0xEC0 };
                        case ItemName.SEALED_WIND: return new int[] { 0xED0 };
                        case ItemName.CINDER_KEY: return new int[] { 0xEE0 };
                        case ItemName.CF_BOW_LVL: return new int[] { 0xEF0 };
                        case ItemName.CF_BOW_SPEED: return new int[] { 0xF00 };
                        case ItemName.CF_DASH: return new int[] { 0xF10 };
                        case ItemName.CF_WARP: return new int[] { 0xF20 };
                        default: return null;
                    }
                default: return null;
            }
        }

        /// <summary>
        /// Set Value of Item to ItemReference's Value
        /// </summary>
        private void SetValue() => PointerUtility.WriteValue(process, ValuePtr, (int)ItemReference.ItemName);

        /// <summary>
        /// Set Value of Item to original Value (ItemName)
        /// </summary>
        private void ResetValue() => PointerUtility.WriteValue(process, ValuePtr, (int)ItemName);
        #endregion
    }

    enum ItemName
    {
        ADORNED_RING = 1,
        NECKLACE_OF_SACRIFICE,
        BELLFLOWER,
        ASTRAL_CHARM = 5,
        EDEAS_PEARL,
        DULL_PEARL,
        RED_RING,
        MAGNET_STONE,
        ROTTEN_BELLFLOWER,
        FAERIE_TEAR,
        IMPURITY_FLASK = 13,
        PASSIFLORA,
        CRYTAL_SEED,
        MEDAL_OF_EQUIVALENCE,
        TAINTED_MISSIVE,
        BLACK_SACHET,
        RING_OF_CANDOR = 21,
        SMALL_COIN,
        BAKMAN_PATCH,
        CAT_SPHERE,
        HAZLE_BADGE,
        TORN_BRANCH,
        MONASTERY_KEY,
        CLARITY_SHARD = 31,
        DIRTY_SHROOM,
        IVORY_BUG = 34,
        VIOLET_SPRITE,
        SOFT_TISSUE,
        GARDEN_KEY,
        SPARSE_THREAD,
        BLESSING_CHARM,
        HEAVY_ARROWS,
        BLOODSTAINED_TISSUE,
        MAPLE_LEAF,
        FRESH_SPRING_LEAF,
        POCKET_INCENSORY,
        BIRTHSTONE,
        QUICK_ARROWS,
        DRILLING_ARROWS,
        SEALED_WIND,
        CINDER_KEY,
        CF_BOW_LVL,
        CF_BOW_SPEED,
        CF_DASH,
        CF_WARP
    }
}