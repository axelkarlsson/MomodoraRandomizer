using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.UI.Components
{
    internal class Items
    {
        private const string VERSION_1_05b = "1.05b", VERSION_1_07 = "1.07";

        internal static string Version;
        internal static Process Process;
        internal static List<Items> List = new List<Items>();
        internal static List<ItemName> ExcludeList = new List<ItemName>() { // Items to exclude from Randomization
            ItemName.ADORNED_RING,
            ItemName.MEDAL_OF_EQUIVALENCE,
            ItemName.MAPLE_LEAF,
            ItemName.BELLFLOWER,
            ItemName.DIRTY_SHROOM,
            ItemName.TAINTED_MISSIVE,
            ItemName.POCKET_INCENSORY,
            ItemName.IVORY_BUG
        };

        internal ItemName ItemName { get; }
        internal IntPtr PtrToValue { get; }
        internal Items ReferencedBy { get; set; }
        internal Items ItemReference { get; set; }
        internal List<ItemName> DependsOn { get; } = new List<ItemName>();

        /*
        private int[] StringBaseOffsetList;
        private int[] StringOffsetList;
        
        public string Name { get; }
        public string Effect { get; }
        public string Description { get; }
        */

        #region Constructor
        public Items(ItemName itemName)
        {
            this.ItemName = itemName;

            this.PtrToValue = PointerUtility.CreatePointer(Process, GetBase().Concat(GetOffsets()).ToArray());

            AddDependencies();
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Resets values and ItemReferences in the static 'List' variable of the class.
        /// </summary>
        static public void Reset()
        {
            foreach (Items item in List)
            {
                item.ReferencedBy = null;
                item.ItemReference = null;
                item.ResetValue();
                item.ResetDependencies();
            }
        }

        /// <summary>
        /// Sets the value of the items in the static 'List' variable to their corresponding 'ItemReference' values.
        /// </summary>
        static public void SetValues()
        {
            foreach (Items item in List)
            {
                item.SetValue();
            }
        }

        /// <summary>
        /// Sets the value of the items in the static 'List' variable to their original values (ItemName).
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
        /// Gets the Offsets base of the item
        /// </summary>
        /// <returns>
        ///     Offsets base if the version is valid; otherwise, <c>null</c>.
        /// </returns>
        private int[] GetBase()
        {
            switch (Version)
            {
                case VERSION_1_05b: return new int[] { 0x2304CE8, 0x4 };
                case VERSION_1_07: return new int[] { 0x2371EA8, 0x4 };
                default: return null;
            }
        }

        /// <summary>
        /// Get Offsets of item.
        /// </summary>
        /// <returns>
        ///     Offsets value if the version and ItemName are valid; otherwise, <c>null</c>.
        /// </returns>
        private int[] GetOffsets()
        {
            switch (Version)
            {
                case VERSION_1_05b:
                case VERSION_1_07:
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
        /// Sets the value of the item to its ItemReference's value.
        /// </summary>
        private void SetValue() => PointerUtility.WriteValue(Process, PtrToValue, (double)ItemReference.ItemName);

        /// <summary>
        /// Sets the value of the item to its original value (ItemName).
        /// </summary>
        private void ResetValue() => PointerUtility.WriteValue(Process, PtrToValue, (double)ItemName);

        /// <summary>
        /// Adds dependencies to the dependency lists.
        /// </summary>
        private void AddDependencies()
        {
            DependsOn.AddRange(GetDependsOn(this.ItemName));
        }

        /// <summary>
        /// Clears dependency lists.
        /// </summary>
        private void ClearDependencies()
        {
            DependsOn.Clear();
        }

        /// <summary>
        /// Resets the value of the dependency lists.
        /// </summary>
        private void ResetDependencies()
        {
            ClearDependencies();
            AddDependencies();
        }

        /// <summary>
        /// Lists Items a given Item depends on for a user to acquire it.
        /// </summary>
        /// <param name="itemName">Name of the Item.</param>
        /// <returns>List of distinct ItemName.</returns>
        private ItemName[] GetDependsOn(ItemName itemName)
        {
            switch (itemName)
            {
                case ItemName.BLESSING_CHARM:
                    return CombineDependsOn(ItemName.HAZLE_BADGE);

                case ItemName.BLOODSTAINED_TISSUE:
                    return CombineDependsOn(ItemName.SOFT_TISSUE);

                case ItemName.FRESH_SPRING_LEAF:
                    return CombineDependsOn(ItemName.SEALED_WIND);

                case ItemName.ROTTEN_BELLFLOWER:
                    return CombineDependsOn(ItemName.DIRTY_SHROOM);

                case ItemName.SMALL_COIN:
                case ItemName.BIRTHSTONE:
                    return CombineDependsOn(ItemName.FRESH_SPRING_LEAF);

                case ItemName.CF_WARP:
                    return CombineDependsOn(ItemName.CAT_SPHERE);

                case ItemName.HAZLE_BADGE:
                case ItemName.DIRTY_SHROOM:
                    return CombineDependsOn(ItemName.CAT_SPHERE);

                case ItemName.SEALED_WIND:
                case ItemName.VIOLET_SPRITE:
                case ItemName.BLACK_SACHET:
                case ItemName.QUICK_ARROWS:
                case ItemName.HEAVY_ARROWS:
                    return CombineDependsOn(
                        ItemName.CF_BOW_LVL,
                        ItemName.CF_BOW_SPEED,
                        ItemName.CF_DASH,
                        ItemName.CF_WARP,
                        ItemName.CAT_SPHERE
                    );

                case ItemName.CAT_SPHERE:
                    return new[] { ItemName.GARDEN_KEY/*, ItemName.BAKMAN_PATCH*/ };

                case ItemName.SOFT_TISSUE:
                case ItemName.CF_BOW_LVL:
                    return new[] { ItemName.MONASTERY_KEY };

                default:
                    return new ItemName[] { };
            }
        }

        /// <summary>
        /// Combines all required Items and their requirements in the given list.
        /// </summary>
        /// <param name="dependencies">List of Items to get the depencency of.</param>
        /// <returns>List of distinct ItemName.</returns>
        private ItemName[] CombineDependsOn(params ItemName[] dependencies)
        {
            return dependencies.Concat(dependencies.SelectMany(item => GetDependsOn(item))).ToArray();
        }
        #endregion
    }

    public enum ItemName
    {
        ADORNED_RING = 1,
        NECKLACE_OF_SACRIFICE = 2,
        BELLFLOWER = 4,
        ASTRAL_CHARM = 5,
        EDEAS_PEARL = 6,
        DULL_PEARL = 7,
        RED_RING = 8,
        MAGNET_STONE = 9,
        ROTTEN_BELLFLOWER = 10,
        FAERIE_TEAR = 11,
        IMPURITY_FLASK = 13,
        PASSIFLORA = 14,
        CRYTAL_SEED = 15,
        MEDAL_OF_EQUIVALENCE = 16,
        TAINTED_MISSIVE = 17,
        BLACK_SACHET = 18,
        RING_OF_CANDOR = 21,
        SMALL_COIN = 22,
        BAKMAN_PATCH = 23,
        CAT_SPHERE = 24,
        HAZLE_BADGE = 25,
        TORN_BRANCH = 26,
        MONASTERY_KEY = 27,
        CLARITY_SHARD = 31,
        DIRTY_SHROOM = 32,
        IVORY_BUG = 34,
        VIOLET_SPRITE = 35,
        SOFT_TISSUE = 36,
        GARDEN_KEY = 37,
        SPARSE_THREAD = 38,
        BLESSING_CHARM = 39,
        HEAVY_ARROWS = 40,
        BLOODSTAINED_TISSUE = 41,
        MAPLE_LEAF = 42,
        FRESH_SPRING_LEAF = 43,
        POCKET_INCENSORY = 44,
        BIRTHSTONE = 45,
        QUICK_ARROWS = 46,
        DRILLING_ARROWS = 47,
        SEALED_WIND = 48,
        CINDER_KEY = 49,
        CF_BOW_LVL = 50,
        CF_BOW_SPEED = 51,
        CF_DASH = 52,
        CF_WARP = 53
    }
}