using MomodoraRandomizer.Data.Enums;
using System.Collections.Generic;

namespace MomodoraRandomizer.Data.Metadata {
    internal static class ItemMetadata {
        internal static readonly Dictionary<ItemId, int> InstancesCount = new Dictionary<ItemId, int> () {
            { ItemId.ADORNED_RING,          1  },
            { ItemId.NECKLACE_OF_SACRIFICE, 1  },
            { ItemId.BELLFLOWER,            5  },
            { ItemId.ASTRAL_CHARM,          1  },
            { ItemId.EDEAS_PEARL,           1  },
            { ItemId.DULL_PEARL,            1  },
            { ItemId.RED_RING,              1  },
            { ItemId.MAGNET_STONE,          1  },
            { ItemId.ROTTEN_BELLFLOWER,     1  },
            { ItemId.FAERIE_TEAR,           1  },
            { ItemId.PH_1,                  1  },
            { ItemId.IMPURITY_FLASK,        1  },
            { ItemId.PASSIFLORA,            2  },
            { ItemId.CRYTAL_SEED,           1  },
            { ItemId.MEDAL_OF_EQUIVALENCE,  1  },
            { ItemId.TAINTED_MISSIVE,       2  },
            { ItemId.BLACK_SACHET,          1  },
            { ItemId.PH_2,                  1  },
            { ItemId.PH_3,                  1  },
            { ItemId.RING_OF_CANDOR,        1  },
            { ItemId.SMALL_COIN,            1  },
            { ItemId.BAKMAN_PATCH,          1  },
            { ItemId.CAT_SPHERE,            1  },
            { ItemId.HAZLE_BADGE,           1  },
            { ItemId.TORN_BRANCH,           1  },
            { ItemId.MONASTERY_KEY,         1  },
            { ItemId.CLARITY_SHARD,         1  },
            { ItemId.DIRTY_SHROOM,          1  },
            { ItemId.IVORY_BUG,             20 },
            { ItemId.VIOLET_SPRITE,         1  },
            { ItemId.SOFT_TISSUE,           1  },
            { ItemId.GARDEN_KEY,            1  },
            { ItemId.SPARSE_THREAD,         1  },
            { ItemId.BLESSING_CHARM,        1  },
            { ItemId.HEAVY_ARROWS,          1  },
            { ItemId.BLOODSTAINED_TISSUE,   1  },
            { ItemId.MAPLE_LEAF,            1  },
            { ItemId.FRESH_SPRING_LEAF,     1  },
            { ItemId.POCKET_INCENSORY,      1  }, // Treat shop and boss as a single boss item for randomization purposes
            { ItemId.BIRTHSTONE,            1  },
            { ItemId.QUICK_ARROWS,          1  },
            { ItemId.DRILLING_ARROWS,       1  },
            { ItemId.SEALED_WIND,           1  },
            { ItemId.CINDER_KEY,            1  },
            { ItemId.CF_BOW_LVL,            1  },
            { ItemId.CF_BOW_SPEED,          1  },
            { ItemId.CF_DASH,               1  },
            { ItemId.CF_WARP,               1  },
            { ItemId.VITALITY_FRAGMENT,     17 }
        };

        internal static readonly Dictionary<ItemId, ItemType> Types = new Dictionary<ItemId, ItemType>() {
            { ItemId.ADORNED_RING,          ItemType.PASSIVE },
            { ItemId.NECKLACE_OF_SACRIFICE, ItemType.PASSIVE },
            { ItemId.BELLFLOWER,            ItemType.ACTIVE  },
            { ItemId.ASTRAL_CHARM,          ItemType.PASSIVE },
            { ItemId.EDEAS_PEARL,           ItemType.PASSIVE },
            { ItemId.DULL_PEARL,            ItemType.PASSIVE },
            { ItemId.RED_RING,              ItemType.PASSIVE },
            { ItemId.MAGNET_STONE,          ItemType.PASSIVE },
            { ItemId.ROTTEN_BELLFLOWER,     ItemType.ACTIVE  },
            { ItemId.FAERIE_TEAR,           ItemType.PASSIVE },
            { ItemId.PH_1,                  ItemType.NONE    },
            { ItemId.IMPURITY_FLASK,        ItemType.PASSIVE },
            { ItemId.PASSIFLORA,            ItemType.ACTIVE  },
            { ItemId.CRYTAL_SEED,           ItemType.ACTIVE  },
            { ItemId.MEDAL_OF_EQUIVALENCE,  ItemType.PASSIVE },
            { ItemId.TAINTED_MISSIVE,       ItemType.ACTIVE  },
            { ItemId.BLACK_SACHET,          ItemType.PASSIVE },
            { ItemId.PH_2,                  ItemType.NONE    },
            { ItemId.PH_3,                  ItemType.NONE    },
            { ItemId.RING_OF_CANDOR,        ItemType.PASSIVE },
            { ItemId.SMALL_COIN,            ItemType.KEY     },
            { ItemId.BAKMAN_PATCH,          ItemType.ACTIVE  },
            { ItemId.CAT_SPHERE,            ItemType.ACTIVE  },
            { ItemId.HAZLE_BADGE,           ItemType.KEY     },
            { ItemId.TORN_BRANCH,           ItemType.PASSIVE },
            { ItemId.MONASTERY_KEY,         ItemType.KEY     },
            { ItemId.CLARITY_SHARD,         ItemType.ACTIVE  },
            { ItemId.DIRTY_SHROOM,          ItemType.KEY     },
            { ItemId.IVORY_BUG,             ItemType.KEY     },
            { ItemId.VIOLET_SPRITE,         ItemType.ACTIVE  },
            { ItemId.SOFT_TISSUE,           ItemType.ACTIVE  },
            { ItemId.GARDEN_KEY,            ItemType.KEY     },
            { ItemId.SPARSE_THREAD,         ItemType.ACTIVE  },
            { ItemId.BLESSING_CHARM,        ItemType.ACTIVE  },
            { ItemId.HEAVY_ARROWS,          ItemType.PASSIVE },
            { ItemId.BLOODSTAINED_TISSUE,   ItemType.ACTIVE  },
            { ItemId.MAPLE_LEAF,            ItemType.KEY     },
            { ItemId.FRESH_SPRING_LEAF,     ItemType.KEY     },
            { ItemId.POCKET_INCENSORY,      ItemType.PASSIVE },
            { ItemId.BIRTHSTONE,            ItemType.KEY     },
            { ItemId.QUICK_ARROWS,          ItemType.PASSIVE },
            { ItemId.DRILLING_ARROWS,       ItemType.PASSIVE },
            { ItemId.SEALED_WIND,           ItemType.ACTIVE  },
            { ItemId.CINDER_KEY,            ItemType.KEY     },
            { ItemId.CF_BOW_LVL,            ItemType.KEY     },
            { ItemId.CF_BOW_SPEED,          ItemType.KEY     },
            { ItemId.CF_DASH,               ItemType.KEY     },
            { ItemId.CF_WARP,               ItemType.KEY     }
        };

        internal static readonly Dictionary<(ItemId, int instanceId), ItemId[]> HardRequirements               = new Dictionary<(ItemId, int), ItemId[]>() {
            { (ItemId.BELLFLOWER,           4), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.ROTTEN_BELLFLOWER,    0), new[] { ItemId.DIRTY_SHROOM }  },
            { (ItemId.TAINTED_MISSIVE,      1), new[] { ItemId.MONASTERY_KEY } },
            { (ItemId.BLACK_SACHET,         0), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.DIRTY_SHROOM,         0), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.IVORY_BUG,            0), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.IVORY_BUG,            6), new[] { ItemId.MONASTERY_KEY } },
            { (ItemId.IVORY_BUG,            7), new[] { ItemId.MONASTERY_KEY,  ItemId.CAT_SPHERE }   },
            { (ItemId.IVORY_BUG,            8), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.IVORY_BUG,           10), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.IVORY_BUG,           11), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.IVORY_BUG,           16), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP,  ItemId.CAT_SPHERE} },
            { (ItemId.IVORY_BUG,           17), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP,  ItemId.CAT_SPHERE} },
            { (ItemId.IVORY_BUG,           18), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.VIOLET_SPRITE,        0), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.SOFT_TISSUE,          0), new[] { ItemId.MONASTERY_KEY,  ItemId.CAT_SPHERE }   },
            { (ItemId.BLESSING_CHARM,       0), new[] { ItemId.HAZLE_BADGE }   },
            { (ItemId.HEAVY_ARROWS,         0), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.BLOODSTAINED_TISSUE,  0), new[] { ItemId.SOFT_TISSUE }   },
            { (ItemId.FRESH_SPRING_LEAF,    0), new[] { ItemId.CAT_SPHERE,     ItemId.SEALED_WIND }  },
            { (ItemId.QUICK_ARROWS,         0), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.SEALED_WIND,          0), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.CF_BOW_LVL,           0), new[] { ItemId.MONASTERY_KEY } },
            { (ItemId.CF_DASH,              0), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.VITALITY_FRAGMENT,    0), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.VITALITY_FRAGMENT,    3), new[] { ItemId.CAT_SPHERE }    },
            { (ItemId.VITALITY_FRAGMENT,    9), new[] { ItemId.CINDER_KEY }    },
            { (ItemId.VITALITY_FRAGMENT,   15), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP } },
            { (ItemId.VITALITY_FRAGMENT,   16), new[] { ItemId.CF_BOW_LVL,     ItemId.CF_BOW_SPEED,  ItemId.CF_DASH, ItemId.CF_WARP,  ItemId.CAT_SPHERE } }
        };
        internal static readonly Dictionary<(ItemId, int instanceId), ItemId[]> SoftRequirements               = new Dictionary<(ItemId, int), ItemId[]>() {
            { (ItemId.DULL_PEARL,        0),  new[] { ItemId.GARDEN_KEY, ItemId.BAKMAN_PATCH } },
            { (ItemId.BELLFLOWER,        3),  new[] { ItemId.GARDEN_KEY, ItemId.BAKMAN_PATCH } },
            { (ItemId.CAT_SPHERE,        0),  new[] { ItemId.GARDEN_KEY, ItemId.BAKMAN_PATCH } },
            { (ItemId.HAZLE_BADGE,       0),  new[] { ItemId.GARDEN_KEY, ItemId.BAKMAN_PATCH } },
            { (ItemId.TORN_BRANCH,       0),  new[] { ItemId.GARDEN_KEY, ItemId.BAKMAN_PATCH } },
            { (ItemId.IVORY_BUG,         9),  new[] { ItemId.CAT_SPHERE, ItemId.CF_DASH }      },
            { (ItemId.IVORY_BUG,         18), new[] { ItemId.CAT_SPHERE, ItemId.CF_DASH }      },
            { (ItemId.VITALITY_FRAGMENT, 15), new[] { ItemId.CAT_SPHERE, ItemId.CF_DASH }      },
        };
        internal static readonly Dictionary<(ItemId, int instanceId), int>      IvoryBugCountRequirements      = new Dictionary<(ItemId, int), int>() {
            { (ItemId.BELLFLOWER,  3), 10},
            { (ItemId.HAZLE_BADGE, 0), 15},
            { (ItemId.PASSIFLORA,  0), 20}
        };
        internal static readonly Dictionary<(ItemId, int instanceId), int>      CrestFragmentCountRequirenents = new Dictionary<(ItemId, int), int>() {
            { (ItemId.BLESSING_CHARM, 0), 3}
        };

        internal static readonly Dictionary<(ItemId, int instanceId), int[]> SpawnRooms = new Dictionary<(ItemId, int), int[]>() { // One for each instance in the game
            { (ItemId.NECKLACE_OF_SACRIFICE, 0),  new[] { 127  }    },
            { (ItemId.BELLFLOWER,            0),  new[] { 25   }    },
            { (ItemId.BELLFLOWER,            1),  new[] { 64,  70   }}, // 70 for <1.07 && 64 for >=1.07
            { (ItemId.BELLFLOWER,            2),  new[] { 162  }    },
            { (ItemId.BELLFLOWER,            3),  new[] { 163  }    },
            { (ItemId.BELLFLOWER,            4),  new[] { 269  }    },
            { (ItemId.ASTRAL_CHARM,          0),  new[] { 37   }    },
            { (ItemId.EDEAS_PEARL,           0),  new[] { 53   }    },
            { (ItemId.DULL_PEARL,            0),  new[] { 160  }    },
            { (ItemId.RED_RING,              0),  new[] { 181  }    },
            { (ItemId.MAGNET_STONE,          0),  new[] { 90   }    },
            { (ItemId.ROTTEN_BELLFLOWER,     0),  new[] { 46   }    },
            { (ItemId.FAERIE_TEAR,           0),  new[] { 63,  111  }}, // Grouped
            { (ItemId.IMPURITY_FLASK,        0),  new[] { 187  }    },
            { (ItemId.PASSIFLORA,            0),  new[] { 163  }    },
            { (ItemId.PASSIFLORA,            1),  new[] { 261  }    },
            { (ItemId.CRYTAL_SEED,           0),  new[] { 63   }    },
            { (ItemId.TAINTED_MISSIVE,       0),  new[] { 104  }    },
            { (ItemId.TAINTED_MISSIVE,       1),  new[] { 117  }    },
            { (ItemId.BLACK_SACHET,          0),  new[] { 204  }    },
            { (ItemId.RING_OF_CANDOR,        0),  new[] { 63,  181, 187 }}, // Grouped
            { (ItemId.BAKMAN_PATCH,          0),  new[] { 73   }    },
            { (ItemId.CAT_SPHERE,            0),  new[] { 149  }    },
            { (ItemId.HAZLE_BADGE,           0),  new[] { 163  }    },
            { (ItemId.TORN_BRANCH,           0),  new[] { 153  }    },
            { (ItemId.MONASTERY_KEY,         0),  new[] { 113  }    },
            { (ItemId.CLARITY_SHARD,         0),  new[] { 127  }    },
            { (ItemId.DIRTY_SHROOM,          0),  new[] { 126  }    },
            { (ItemId.IVORY_BUG,             0),  new[] { 40   }    },
            { (ItemId.IVORY_BUG,             1),  new[] { 41   }    },
            { (ItemId.IVORY_BUG,             2),  new[] { 50   }    },
            { (ItemId.IVORY_BUG,             3),  new[] { 56   }    },
            { (ItemId.IVORY_BUG,             4),  new[] { 62   }    },
            { (ItemId.IVORY_BUG,             5),  new[] { 78   }    },
            { (ItemId.IVORY_BUG,             6),  new[] { 102  }    },
            { (ItemId.IVORY_BUG,             7),  new[] { 103  }    },
            { (ItemId.IVORY_BUG,             8),  new[] { 123  }    },
            { (ItemId.IVORY_BUG,             9),  new[] { 152  }    },
            { (ItemId.IVORY_BUG,             10), new[] { 155  }    },
            { (ItemId.IVORY_BUG,             11), new[] { 156  }    },
            { (ItemId.IVORY_BUG,             12), new[] { 170  }    },
            { (ItemId.IVORY_BUG,             13), new[] { 171  }    },
            { (ItemId.IVORY_BUG,             14), new[] { 183  }    },
            { (ItemId.IVORY_BUG,             15), new[] { 211  }    },
            { (ItemId.IVORY_BUG,             16), new[] { 213  }    },
            { (ItemId.IVORY_BUG,             17), new[] { 214  }    },
            { (ItemId.IVORY_BUG,             18), new[] { 249  }    },
            { (ItemId.IVORY_BUG,             19), new[] { 255  }    },
            { (ItemId.VIOLET_SPRITE,         0),  new[] { 199  }    },
            { (ItemId.SOFT_TISSUE,           0),  new[] { 103  }    },
            { (ItemId.GARDEN_KEY,            0),  new[] { 134  }    },
            { (ItemId.SPARSE_THREAD,         0),  new[] { 141  }    },
            { (ItemId.BLESSING_CHARM,        0),  new[] { 52   }    },
            { (ItemId.HEAVY_ARROWS,          0),  new[] { 212  }    },
            { (ItemId.BLOODSTAINED_TISSUE,   0),  new[] { 97   }    },
            { (ItemId.FRESH_SPRING_LEAF,     0),  new[] { 83   }    },
            { (ItemId.POCKET_INCENSORY,      0),  new[] { 193, 199  }}, // Grouped
            { (ItemId.QUICK_ARROWS,          0),  new[] { 199  }    },
            { (ItemId.DRILLING_ARROWS,       0),  new[] { 187  }    },
            { (ItemId.SEALED_WIND,           0),  new[] { 220  }    },
            { (ItemId.CINDER_KEY,            0),  new[] { 191  }    },
            { (ItemId.CF_BOW_LVL,            0),  new[] { 105  }    },
            { (ItemId.CF_BOW_SPEED,          0),  new[] { 142  }    },
            { (ItemId.CF_DASH,               0),  new[] { 195  }    },
            { (ItemId.CF_WARP,               0),  new[] { 60   }    },
            { (ItemId.VITALITY_FRAGMENT,     0),  new[] { 35   }    },
            { (ItemId.VITALITY_FRAGMENT,     1),  new[] { 39   }    },
            { (ItemId.VITALITY_FRAGMENT,     2),  new[] { 47   }    },
            { (ItemId.VITALITY_FRAGMENT,     3),  new[] { 58   }    },
            { (ItemId.VITALITY_FRAGMENT,     4),  new[] { 67   }    },
            { (ItemId.VITALITY_FRAGMENT,     5),  new[] { 81   }    },
            { (ItemId.VITALITY_FRAGMENT,     6),  new[] { 108  }    },
            { (ItemId.VITALITY_FRAGMENT,     7),  new[] { 144  }    },
            { (ItemId.VITALITY_FRAGMENT,     8),  new[] { 168  }    },
            { (ItemId.VITALITY_FRAGMENT,     9),  new[] { 185  }    },
            { (ItemId.VITALITY_FRAGMENT,     10), new[] { 191  }    },
            { (ItemId.VITALITY_FRAGMENT,     11), new[] { 199  }    },
            { (ItemId.VITALITY_FRAGMENT,     12), new[] { 205  }    },
            { (ItemId.VITALITY_FRAGMENT,     13), new[] { 209  }    },
            { (ItemId.VITALITY_FRAGMENT,     14), new[] { 270  }    },
            { (ItemId.VITALITY_FRAGMENT,     15), new[] { 246  }    },
            { (ItemId.VITALITY_FRAGMENT,     16), new[] { 264  }    }
        };

        internal static readonly Dictionary<GameVersion, int[]>          StringsBase           = new Dictionary<GameVersion, int[]>() {
            { GameVersion.VERSION_1_05b, new[] { 0x230B134, 0x14, 0x0 }},
            { GameVersion.VERSION_1_07,  new[] { 0x23782F4, 0x14, 0x0 }}
        };
        internal static readonly Dictionary<ItemId, int>                 StringsOffsets        = new Dictionary<ItemId, int>() {
            { ItemId.ADORNED_RING,          0x70   },
            { ItemId.NECKLACE_OF_SACRIFICE, 0xD0   },
            { ItemId.BELLFLOWER,            0x190  },
            { ItemId.ASTRAL_CHARM,          0x1F0  },
            { ItemId.EDEAS_PEARL,           0x250  },
            { ItemId.DULL_PEARL,            0x2B0  },
            { ItemId.RED_RING,              0x310  },
            { ItemId.MAGNET_STONE,          0x370  },
            { ItemId.ROTTEN_BELLFLOWER,     0x3D0  },
            { ItemId.FAERIE_TEAR,           0x430  },
            { ItemId.PH_1,                  0x490  },
            { ItemId.IMPURITY_FLASK,        0x4F0  },
            { ItemId.PASSIFLORA,            0x550  },
            { ItemId.CRYTAL_SEED,           0x5B0  },
            { ItemId.MEDAL_OF_EQUIVALENCE,  0x610  },
            { ItemId.TAINTED_MISSIVE,       0x670  },
            { ItemId.BLACK_SACHET,          0x6D0  },
            { ItemId.PH_2,                  0x730  },
            { ItemId.PH_3,                  0x790  },
            { ItemId.RING_OF_CANDOR,        0x7F0  },
            { ItemId.SMALL_COIN,            0x850  },
            { ItemId.BAKMAN_PATCH,          0x8B0  },
            { ItemId.CAT_SPHERE,            0x910  },
            { ItemId.HAZLE_BADGE,           0x970  },
            { ItemId.TORN_BRANCH,           0x9D0  },
            { ItemId.MONASTERY_KEY,         0xA30  },
            { ItemId.CLARITY_SHARD,         0xBB0  },
            { ItemId.DIRTY_SHROOM,          0xC10  },
            { ItemId.IVORY_BUG,             0xCD0  },
            { ItemId.VIOLET_SPRITE,         0xD30  },
            { ItemId.SOFT_TISSUE,           0xD90  },
            { ItemId.GARDEN_KEY,            0xDF0  },
            { ItemId.SPARSE_THREAD,         0xE50  },
            { ItemId.BLESSING_CHARM,        0xEB0  },
            { ItemId.HEAVY_ARROWS,          0xF10  },
            { ItemId.BLOODSTAINED_TISSUE,   0xF70  },
            { ItemId.MAPLE_LEAF,            0xFD0  },
            { ItemId.FRESH_SPRING_LEAF,     0x1030 },
            { ItemId.POCKET_INCENSORY,      0x1090 },
            { ItemId.BIRTHSTONE,            0x10F0 },
            { ItemId.QUICK_ARROWS,          0x1150 },
            { ItemId.DRILLING_ARROWS,       0x11B0 },
            { ItemId.SEALED_WIND,           0x1210 },
            { ItemId.CINDER_KEY,            0x1270 },
            { ItemId.CF_BOW_LVL,            0x12D0 },
            { ItemId.CF_BOW_SPEED,          0x1230 },
            { ItemId.CF_DASH,               0x1390 },
            { ItemId.CF_WARP,               0x13F0 }
        };
        internal static readonly Dictionary<(ItemId, ItemDropType), int> GotItemStringsOffsets = new Dictionary<(ItemId, ItemDropType), int>() {
            { (ItemId.BELLFLOWER,          ItemDropType.WORLD),  0x21DC },
            { (ItemId.BELLFLOWER,          ItemDropType.REWARD), 0x5494 },
            { (ItemId.ASTRAL_CHARM,        ItemDropType.WORLD),  0x256C },
            { (ItemId.EDEAS_PEARL,         ItemDropType.BOSS),   0x28BC },
            { (ItemId.MAGNET_STONE,        ItemDropType.WORLD),  0x4434 },
            { (ItemId.ROTTEN_BELLFLOWER,   ItemDropType.REWARD), 0x160  },
            { (ItemId.PASSIFLORA,          ItemDropType.REWARD), 0x2338 },
            { (ItemId.PASSIFLORA,          ItemDropType.WORLD),  0x1F0C },
            { (ItemId.TAINTED_MISSIVE,     ItemDropType.WORLD),  0x4084 },
            { (ItemId.TAINTED_MISSIVE,     ItemDropType.BOSS),   0x4084 },
            { (ItemId.BLACK_SACHET,        ItemDropType.WORLD),  0x9F8  },
            { (ItemId.BAKMAN_PATCH,        ItemDropType.BOSS),   0x2C0C },
            { (ItemId.CAT_SPHERE,          ItemDropType.WORLD),  0x35CC },
            { (ItemId.HAZLE_BADGE,         ItemDropType.REWARD), 0x5020 },
            { (ItemId.TORN_BRANCH,         ItemDropType.BOSS),   0x317C },
            { (ItemId.MONASTERY_KEY,       ItemDropType.WORLD),  0x27C  },
            { (ItemId.DIRTY_SHROOM,        ItemDropType.WORLD),  0x11B0 },
            { (ItemId.IVORY_BUG,           ItemDropType.WORLD),  0xcc4  },
            { (ItemId.SOFT_TISSUE,         ItemDropType.WORLD),  0x4A8C },
            { (ItemId.GARDEN_KEY,          ItemDropType.WORLD),  0x4E3C },
            { (ItemId.SPARSE_THREAD,       ItemDropType.BOSS),   0x3CAC },
            { (ItemId.BLESSING_CHARM,      ItemDropType.REWARD), 0x5A60 },
            { (ItemId.HEAVY_ARROWS,        ItemDropType.BOSS),   0x6C10 },
            { (ItemId.BLOODSTAINED_TISSUE, ItemDropType.BOSS),   0x2788 },
            { (ItemId.FRESH_SPRING_LEAF,   ItemDropType.WORLD),  0x428  },
            { (ItemId.POCKET_INCENSORY,    ItemDropType.BOSS),   0x2D84 },
            { (ItemId.SEALED_WIND,         ItemDropType.WORLD),  0x68A0 },
            { (ItemId.CINDER_KEY,          ItemDropType.WORLD),  0x2934 },
            { (ItemId.CF_BOW_LVL,          ItemDropType.WORLD),  0x3F4  },
            { (ItemId.CF_BOW_SPEED,        ItemDropType.WORLD),  0x2EEC },
            { (ItemId.CF_DASH,             ItemDropType.WORLD),  0x2A58 },
            { (ItemId.CF_WARP,             ItemDropType.WORLD),  0x740  },
            { (ItemId.VITALITY_FRAGMENT,   ItemDropType.WORLD),  0x68A0 }
        };
        internal static readonly Dictionary<ItemId, string[]>            FakeStrings           = new Dictionary<ItemId, string[]>() { // Declaration of strings for items that dont have some of them in-game.
            { ItemId.VITALITY_FRAGMENT, new[] { // TODO: Not really, update VF to match some in-game text or whatever
                "Vitality Fragment",
                "Increases ones life",
                "Small piece of divinity",
                "Got Vitality Fragment."
            }},
            { ItemId.ADORNED_RING, new[] {
                "",
                "",
                "",
                "Got Adorned Ring.",
            }} // TODO: Add the rest of the items
        };

        internal static readonly Dictionary<(ItemId, int instanceId), ItemDropType> DropType = new Dictionary<(ItemId, int), ItemDropType>() { // One for each instance in the game
            { (ItemId.ADORNED_RING,          0),  ItemDropType.STARTER },
            { (ItemId.NECKLACE_OF_SACRIFICE, 0),  ItemDropType.SHOP    },
            { (ItemId.BELLFLOWER,            0),  ItemDropType.WORLD   },
            { (ItemId.BELLFLOWER,            1),  ItemDropType.WORLD   },
            { (ItemId.BELLFLOWER,            2),  ItemDropType.WORLD   },
            { (ItemId.BELLFLOWER,            3),  ItemDropType.REWARD  },
            { (ItemId.BELLFLOWER,            4),  ItemDropType.WORLD   },
            { (ItemId.ASTRAL_CHARM,          0),  ItemDropType.WORLD   },
            { (ItemId.EDEAS_PEARL,           0),  ItemDropType.BOSS    },
            { (ItemId.DULL_PEARL,            0),  ItemDropType.SHOP    },
            { (ItemId.RED_RING,              0),  ItemDropType.SHOP    },
            { (ItemId.MAGNET_STONE,          0),  ItemDropType.WORLD   },
            { (ItemId.ROTTEN_BELLFLOWER,     0),  ItemDropType.REWARD  },
            { (ItemId.FAERIE_TEAR,           0),  ItemDropType.SHOP    },
            { (ItemId.PH_1,                  0),  ItemDropType.NONE    },
            { (ItemId.IMPURITY_FLASK,        0),  ItemDropType.SHOP    },
            { (ItemId.PASSIFLORA,            0),  ItemDropType.REWARD  },
            { (ItemId.PASSIFLORA,            1),  ItemDropType.WORLD   },
            { (ItemId.CRYTAL_SEED,           0),  ItemDropType.SHOP    },
            { (ItemId.MEDAL_OF_EQUIVALENCE,  0),  ItemDropType.STARTER },
            { (ItemId.TAINTED_MISSIVE,       0),  ItemDropType.WORLD   },
            { (ItemId.TAINTED_MISSIVE,       1),  ItemDropType.BOSS    },
            { (ItemId.BLACK_SACHET,          0),  ItemDropType.WORLD   },
            { (ItemId.PH_2,                  0),  ItemDropType.NONE    },
            { (ItemId.PH_3,                  0),  ItemDropType.NONE    },
            { (ItemId.RING_OF_CANDOR,        0),  ItemDropType.SHOP    },
            { (ItemId.SMALL_COIN,            0),  ItemDropType.BOSS    },
            { (ItemId.BAKMAN_PATCH,          0),  ItemDropType.BOSS    },
            { (ItemId.CAT_SPHERE,            0),  ItemDropType.WORLD   },
            { (ItemId.HAZLE_BADGE,           0),  ItemDropType.REWARD  },
            { (ItemId.TORN_BRANCH,           0),  ItemDropType.BOSS    },
            { (ItemId.MONASTERY_KEY,         0),  ItemDropType.WORLD   },
            { (ItemId.CLARITY_SHARD,         0),  ItemDropType.SHOP    },
            { (ItemId.DIRTY_SHROOM,          0),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             0),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             1),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             2),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             3),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             4),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             5),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             6),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             7),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             8),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             9),  ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             10), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             11), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             12), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             13), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             14), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             15), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             16), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             17), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             18), ItemDropType.WORLD   },
            { (ItemId.IVORY_BUG,             19), ItemDropType.WORLD   },
            { (ItemId.VIOLET_SPRITE,         0),  ItemDropType.SHOP    },
            { (ItemId.SOFT_TISSUE,           0),  ItemDropType.WORLD   },
            { (ItemId.GARDEN_KEY,            0),  ItemDropType.WORLD   },
            { (ItemId.SPARSE_THREAD,         0),  ItemDropType.BOSS    },
            { (ItemId.BLESSING_CHARM,        0),  ItemDropType.REWARD  },
            { (ItemId.HEAVY_ARROWS,          0),  ItemDropType.BOSS    },
            { (ItemId.BLOODSTAINED_TISSUE,   0),  ItemDropType.BOSS    },
            { (ItemId.MAPLE_LEAF,            0),  ItemDropType.STARTER },
            { (ItemId.FRESH_SPRING_LEAF,     0),  ItemDropType.WORLD   },
            { (ItemId.POCKET_INCENSORY,      0),  ItemDropType.BOSS    }, // Treat it as a boss item for randomization purposes
            { (ItemId.BIRTHSTONE,            0),  ItemDropType.BOSS    },
            { (ItemId.QUICK_ARROWS,          0),  ItemDropType.SHOP    },
            { (ItemId.DRILLING_ARROWS,       0),  ItemDropType.SHOP    },
            { (ItemId.SEALED_WIND,           0),  ItemDropType.WORLD   },
            { (ItemId.CINDER_KEY,            0),  ItemDropType.WORLD   },
            { (ItemId.CF_BOW_LVL,            0),  ItemDropType.WORLD   },
            { (ItemId.CF_BOW_SPEED,          0),  ItemDropType.WORLD   },
            { (ItemId.CF_DASH,               0),  ItemDropType.WORLD   },
            { (ItemId.CF_WARP,               0),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     0),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     1),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     2),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     3),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     4),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     5),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     6),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     7),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     8),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     9),  ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     10), ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     11), ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     12), ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     13), ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     14), ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     15), ItemDropType.WORLD   },
            { (ItemId.VITALITY_FRAGMENT,     16), ItemDropType.WORLD   },
        };

        /// <summary>
        ///     Retrieves the amount of instances in the game for a given <seealso cref="ItemId"/>.
        /// </summary>
        /// <param name="itemId">The item Id whose instance count is being requested.</param>
        /// <returns>
        ///     Number of instances for the specified itemId
        /// </returns>
        internal static int GetItemCount(ItemId itemId)
            => InstancesCount.TryGetValue(itemId, out var count) ? count : 0;

        /// <summary>
        ///     Retrieves the type for a specific <seealso cref="ItemId"/>
        /// </summary>
        /// <param name="itemId">The item Id whose instance count is being requested.</param>
        /// <returns>
        ///     <seealso cref="ItemType"/> of that item if it exists<br/>
        ///     <seealso cref="ItemType.NONE"/> otherwise.
        /// </returns>
        internal static ItemType GetItemType(ItemId itemId)
            => Types.TryGetValue(itemId, out var type) ? type : ItemType.NONE;

        /// <summary>
        ///     Lists of all items required to acquire a specific Item.
        /// </summary>
        /// <param name="itemId">Name of the Item.</param>
        /// <param name="instanceId">Instance of the Item, if there are multiple.</param>
        /// <returns>
        ///     List of distinct ItemId.
        /// </returns>
        internal static ItemId[] GetHardRquirements(ItemId itemId, int instanceId)
            => HardRequirements.TryGetValue((itemId, instanceId), out var requirements) ? requirements : new ItemId[] { };

        /// <summary>
        ///     Lists of Items that can beused to acquire a specific Item. One of which is mandatory.
        /// </summary>
        /// <param name="itemId">Name of the Item.</param>
        /// <param name="instanceId">Instance of the Item, if there are multiple.</param>
        /// <returns>
        ///     List of distinct ItemId
        /// </returns>
        internal static ItemId[] GetSoftRquirements(ItemId itemId, int instanceId)
            => SoftRequirements.TryGetValue((itemId, instanceId), out var requirements) ? requirements : new ItemId[] { };

        /// <summary>
        ///     Amount of Ivory Bugs that are required to acquire said item, if any.
        /// </summary>
        /// <param name="itemId">Name of the Item.</param>
        /// <param name="instanceId">Instance of the Item, if there are multiple.</param>
        /// <returns>
        ///     Amount of Ivory Bugs required.
        /// </returns>
        internal static int GetIvoryBugCountRequirements(ItemId itemId, int instanceId)
            => IvoryBugCountRequirements.TryGetValue((itemId, instanceId), out var count) ? count : 0;

        /// <summary>
        ///     Amount of Crest Fragments that are required to acquire said item, if any.
        /// </summary>
        /// <param name="itemId">Name of the Item.</param>
        /// <param name="instanceId">Instance of the Item, if there are multiple.</param>
        /// <returns>
        ///     Amount of Crest Fragments required.
        /// </returns>
        internal static int GetCrestFragmentCountRequirenents(ItemId itemId, int instanceId)
            => CrestFragmentCountRequirenents.TryGetValue((itemId, instanceId), out var count) ? count : 0;

        /// <summary>
        ///     Get the Spawn rooms of an item by Id.
        /// </summary>
        /// <param name="itemId">Id of the item.</param>
        /// <param name="instanceId">Instance of the Item, if there are multiple.</param>
        /// <returns>
        ///     Rooms where the item spawns; otherwise, <c>empty array</c>.
        /// </returns>
        internal static int[] GetItemSpawnRooms(ItemId itemId, int instanceId)
            => SpawnRooms.TryGetValue((itemId, instanceId), out var Rooms) ? Rooms : new int[] { };

        /// <summary>
        ///     Gets the Offsets base of the item string references
        /// </summary>
        /// <param name="gameVersion">Version of the game.</param>
        /// <returns>
        ///     Offsets base if the version is valid; otherwise, <c>empty array</c>.
        /// </returns>
        internal static int[] GetStringsBase(GameVersion gameVersion)
            => StringsBase.TryGetValue(gameVersion, out var baseOffsets) ? baseOffsets : new int[] { };

        /// <summary>
        ///     Get Offset of item name.
        /// </summary>
        /// <param name="itemId">Id of the item</param>
        /// <returns>
        ///     Offset value if ItemId is valid; otherwise, <c>0</c>.
        /// </returns>
        internal static int GetStringsOffset(ItemId itemId)
            => StringsOffsets.TryGetValue(itemId, out var stringOffset) ? stringOffset : 0;

        /// <summary>
        ///     Get Offset of the item pickup string.
        /// </summary>
        /// <param name="itemId">Id of the item.</param>
        /// <param name="dropType">Type of drop of that item instance</param>
        /// <returns>
        ///     Offset value if ItemId and dropType are valid; otherwise, <c>0</c>.
        /// </returns>
        internal static int GetStringPickupOffset(ItemId itemId, ItemDropType dropType)
            => GotItemStringsOffsets.TryGetValue((itemId, dropType), out var stringOffset) ? stringOffset : 0;

        /// <summary>
        ///     Get fake strings for an item that doesnt have them in-game
        /// </summary>
        /// <param name="itemId">Id of the item.</param>
        /// <returns>
        ///     Array of fake strings for that item
        /// </returns>
        internal static string[] GetFakeStrings(ItemId itemId)
            => FakeStrings.TryGetValue(itemId, out var fakeStrings) ? fakeStrings : new string[] { };

        /// <summary>
        ///     Get the type of drop an item is.
        /// </summary>
        /// <param name="itemId">Id of the item</param>
        /// <param name="instanceId">Instance of the Item, if there are multiple.</param>
        /// <returns>
        ///     <seealso cref="ItemDropType"/> of that item if that many instances exist<br/>
        ///     <seealso cref="ItemDropType.NONE"/> otherwise.
        /// </returns>
        internal static ItemDropType GetItemDropType(ItemId itemId, int instanceId) 
            => DropType.TryGetValue((itemId, instanceId), out var type) ? type : ItemDropType.NONE;
    }
}