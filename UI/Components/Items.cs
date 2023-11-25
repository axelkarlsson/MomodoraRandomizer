using System.Collections.Generic;

namespace LiveSplit.UI.Components
{
    internal class Items
    {
        public static List<Items> ItemsList = new List<Items>();
        private static int CurrentId = 0;

        private ItemName ItemName { get; }
        public int Id { get; }

        public string Name { get; }
        public string Effect { get; }
        public string Description { get; }

        public Items ItemReference { get; set; }

        public Items() => this.Id =+ CurrentId;

        public Items(ItemName itemName) : this() => this.ItemName = itemName;

        public Items(ItemName itemName, Items itemReference) : this(itemName) => this.ItemReference = itemReference;

        public Items(ItemName itemName, string version) : this(itemName)
        {

        }
    }

    enum ItemName
    {
        ADORNED_RING = 1,
        NECKLACE_OF_SACRIFICE,
        BELLFLOWER,
        ASTRAL_CHARM,
        EDEAS_PEARL,
        DULL_PEARL,
        RED_RING,
        MAGNET_STONE,
        ROTTEN_BELLFLOWER,
        FAERIE_TEAR,
        IMPURITY_FLASK,
        PASSIFLORA,
        CRYTAL_SEED,
        MEDAL_OF_EQUIVALENCE,
        TAINTED_MISSIVE,
        BLACK_SACHET,
        RING_OF_CANDOR,
        SMALL_COIN,
        BAKMAN_PATCH,
        CAT_SPHERE,
        HAZLE_BADGE,
        TORN_BRANCH,
        MONASTERY_KEY,
        CLARITY_SHARD,
        DIRTY_SHROOM,
        IVORY_BUG,
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