using MomodoraRandomizer.Data.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MomodoraRandomizer.Data.Metadata;

internal static class ItemMetadata {
    public static readonly Dictionary<ItemId, ItemType> Types = new() {
        { ItemId.ADORNED_RING, ItemType.PASSIVE },
        { ItemId.NECKLACE_OF_SACRIFICE, ItemType.PASSIVE },
        { ItemId.BELLFLOWER, ItemType.ACTIVE },
        { ItemId.ASTRAL_CHARM, ItemType.PASSIVE },
        { ItemId.EDEAS_PEARL, ItemType.PASSIVE },
        { ItemId.DULL_PEARL, ItemType.PASSIVE },
        { ItemId.RED_RING, ItemType.PASSIVE },
        { ItemId.MAGNET_STONE, ItemType.PASSIVE },
        { ItemId.ROTTEN_BELLFLOWER, ItemType.ACTIVE },
        { ItemId.FAERIE_TEAR, ItemType.PASSIVE },
        { ItemId.IMPURITY_FLASK, ItemType.PASSIVE },
        { ItemId.PASSIFLORA, ItemType.ACTIVE },
        { ItemId.CRYTAL_SEED, ItemType.ACTIVE },
        { ItemId.MEDAL_OF_EQUIVALENCE, ItemType.PASSIVE },
        { ItemId.TAINTED_MISSIVE, ItemType.ACTIVE },
        { ItemId.BLACK_SACHET, ItemType.PASSIVE },
        { ItemId.RING_OF_CANDOR, ItemType.PASSIVE },
        { ItemId.SMALL_COIN, ItemType.KEY },
        { ItemId.BAKMAN_PATCH, ItemType.ACTIVE },
        { ItemId.CAT_SPHERE, ItemType.ACTIVE },
        { ItemId.HAZLE_BADGE, ItemType.KEY },
        { ItemId.TORN_BRANCH, ItemType.PASSIVE },
        { ItemId.MONASTERY_KEY, ItemType.KEY },
        { ItemId.CLARITY_SHARD, ItemType.ACTIVE },
        { ItemId.DIRTY_SHROOM, ItemType.KEY },
        { ItemId.IVORY_BUG, ItemType.KEY },
        { ItemId.VIOLET_SPRITE, ItemType.ACTIVE },
        { ItemId.SOFT_TISSUE, ItemType.ACTIVE },
        { ItemId.GARDEN_KEY, ItemType.KEY },
        { ItemId.SPARSE_THREAD, ItemType.ACTIVE },
        { ItemId.BLESSING_CHARM, ItemType.ACTIVE },
        { ItemId.HEAVY_ARROWS, ItemType.PASSIVE },
        { ItemId.BLOODSTAINED_TISSUE, ItemType.ACTIVE },
        { ItemId.MAPLE_LEAF, ItemType.KEY },
        { ItemId.FRESH_SPRING_LEAF, ItemType.KEY },
        { ItemId.POCKET_INCENSORY, ItemType.PASSIVE },
        { ItemId.BIRTHSTONE, ItemType.KEY },
        { ItemId.QUICK_ARROWS, ItemType.PASSIVE },
        { ItemId.DRILLING_ARROWS, ItemType.PASSIVE },
        { ItemId.SEALED_WIND, ItemType.ACTIVE },
        { ItemId.CINDER_KEY, ItemType.KEY },
        { ItemId.CF_BOW_LVL, ItemType.KEY },
        { ItemId.CF_BOW_SPEED, ItemType.KEY },
        { ItemId.CF_DASH, ItemType.KEY },
        { ItemId.CF_WARP, ItemType.KEY }
    };

    // TODO: Redo logic for randomization since after placing required items we might not need to go some or other places
    public static readonly Dictionary<ItemId, ItemId[]> ItemRequirements = new() {
        { ItemId.BLESSING_CHARM, CombineRequirements(ItemId.HAZLE_BADGE) },
        { ItemId.BLOODSTAINED_TISSUE, CombineRequirements(ItemId.SOFT_TISSUE) },
        { ItemId.FRESH_SPRING_LEAF, CombineRequirements(ItemId.SEALED_WIND) },
        { ItemId.ROTTEN_BELLFLOWER, CombineRequirements(ItemId.DIRTY_SHROOM) },
        { ItemId.SMALL_COIN, CombineRequirements(ItemId.FRESH_SPRING_LEAF) },
        { ItemId.BIRTHSTONE, CombineRequirements(ItemId.FRESH_SPRING_LEAF) },
        { ItemId.CF_WARP, CombineRequirements(ItemId.CAT_SPHERE) },
        { ItemId.HAZLE_BADGE, CombineRequirements(ItemId.CAT_SPHERE) },
        { ItemId.DIRTY_SHROOM, CombineRequirements(ItemId.CAT_SPHERE) },
        { ItemId.SEALED_WIND, CombineRequirements(ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP, ItemId.CAT_SPHERE) },
        { ItemId.VIOLET_SPRITE, CombineRequirements(ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP, ItemId.CAT_SPHERE) },
        { ItemId.BLACK_SACHET, CombineRequirements(ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP, ItemId.CAT_SPHERE) },
        { ItemId.QUICK_ARROWS, CombineRequirements(ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP, ItemId.CAT_SPHERE) },
        { ItemId.HEAVY_ARROWS, CombineRequirements(ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP, ItemId.CAT_SPHERE) },
        { ItemId.CAT_SPHERE, [ItemId.GARDEN_KEY/*, ItemId.BAKMAN_PATCH*/] },
        { ItemId.SOFT_TISSUE, [ItemId.MONASTERY_KEY] },
        { ItemId.CF_BOW_LVL, [ItemId.MONASTERY_KEY] }
    };

    public static readonly Dictionary<ItemId, int[]> SpawnRooms = new() {
        { ItemId.NECKLACE_OF_SACRIFICE, [127] },
        { ItemId.BELLFLOWER, [25, 64, 70, 162, 163, 269] }, // Individual room 70 is for 1.05b or less, 64 is for 1.07 or more
        { ItemId.ASTRAL_CHARM, [37] },
        { ItemId.EDEAS_PEARL, [53] },
        { ItemId.DULL_PEARL, [160] },
        { ItemId.RED_RING, [181] },
        { ItemId.MAGNET_STONE, [90] },
        { ItemId.ROTTEN_BELLFLOWER, [46] },
        { ItemId.FAERIE_TEAR, [63, 111] }, // Grouped
        { ItemId.IMPURITY_FLASK, [187] },
        { ItemId.PASSIFLORA, [163, 261] }, // Individual
        { ItemId.CRYTAL_SEED, [63] },
        { ItemId.MEDAL_OF_EQUIVALENCE, [] },
        { ItemId.TAINTED_MISSIVE, [104, 117] }, // Individual
        { ItemId.BLACK_SACHET, [204] },
        { ItemId.RING_OF_CANDOR, [63, 181, 187] }, // Grouped
        { ItemId.BAKMAN_PATCH, [73] },
        { ItemId.CAT_SPHERE, [149] },
        { ItemId.HAZLE_BADGE, [163] },
        { ItemId.TORN_BRANCH, [153] },
        { ItemId.MONASTERY_KEY, [113] },
        { ItemId.CLARITY_SHARD, [127] },
        { ItemId.DIRTY_SHROOM, [126] },
        { ItemId.IVORY_BUG, [40, 41, 50, 56, 62, 78, 102, 103, 123, 152, 155, 156, 170, 171, 183, 211, 213, 214, 249, 255] }, // Individual
        { ItemId.VIOLET_SPRITE, [199] },
        { ItemId.SOFT_TISSUE, [103] },
        { ItemId.GARDEN_KEY, [134] },
        { ItemId.SPARSE_THREAD, [141] },
        { ItemId.BLESSING_CHARM, [52] },
        { ItemId.HEAVY_ARROWS, [212] },
        { ItemId.BLOODSTAINED_TISSUE, [97] },
        { ItemId.FRESH_SPRING_LEAF, [83] },
        { ItemId.POCKET_INCENSORY, [193, 199] }, // Grouped
        { ItemId.QUICK_ARROWS, [199] },
        { ItemId.DRILLING_ARROWS, [187] },
        { ItemId.SEALED_WIND, [220] },
        { ItemId.CINDER_KEY, [191] },
        { ItemId.CF_BOW_LVL, [105] },
        { ItemId.CF_BOW_SPEED, [142] },
        { ItemId.CF_DASH, [195] },
        { ItemId.CF_WARP, [60] },
        { ItemId.VITALITY_FRAGMENT, [35, 39, 47, 58, 67, 81, 108, 144, 168, 185, 191, 199, 205, 209, 270, 246, 264] }  // Individual
    };

    public static readonly Dictionary<GameVersion, int[]> StringsBase = new() {
        { GameVersion.VERSION_1_05b, [0x230B134, 0x14, 0x0] },
        { GameVersion.VERSION_1_07, [0x23782F4, 0x14, 0x0] }
    };
    public static readonly Dictionary<ItemId, int> StringsOffsets = new() {
        { ItemId.ADORNED_RING, 0x70 },
        { ItemId.NECKLACE_OF_SACRIFICE, 0xD0 },
        { ItemId.BELLFLOWER, 0x190 },
        { ItemId.ASTRAL_CHARM, 0x1F0 },
        { ItemId.EDEAS_PEARL, 0x250 },
        { ItemId.DULL_PEARL, 0x2B0 },
        { ItemId.RED_RING, 0x310 },
        { ItemId.MAGNET_STONE, 0x370 },
        { ItemId.ROTTEN_BELLFLOWER, 0x3D0 },
        { ItemId.FAERIE_TEAR, 0x430 },
        { ItemId.PH_1, 0x490 },
        { ItemId.IMPURITY_FLASK, 0x4F0 },
        { ItemId.PASSIFLORA, 0x550 },
        { ItemId.CRYTAL_SEED, 0x5B0 },
        { ItemId.MEDAL_OF_EQUIVALENCE, 0x610 },
        { ItemId.TAINTED_MISSIVE, 0x670 },
        { ItemId.BLACK_SACHET, 0x6D0 },
        { ItemId.PH_2, 0x730 },
        { ItemId.PH_3, 0x790 },
        { ItemId.RING_OF_CANDOR, 0x7F0 },
        { ItemId.SMALL_COIN, 0x850 },
        { ItemId.BAKMAN_PATCH, 0x8B0 },
        { ItemId.CAT_SPHERE, 0x910 },
        { ItemId.HAZLE_BADGE, 0x970 },
        { ItemId.TORN_BRANCH, 0x9D0 },
        { ItemId.MONASTERY_KEY, 0xA30 },
        { ItemId.CLARITY_SHARD, 0xBB0 },
        { ItemId.DIRTY_SHROOM, 0xC10 },
        { ItemId.IVORY_BUG, 0xCD0 },
        { ItemId.VIOLET_SPRITE, 0xD30 },
        { ItemId.SOFT_TISSUE, 0xD90 },
        { ItemId.GARDEN_KEY, 0xDF0 },
        { ItemId.SPARSE_THREAD, 0xE50 },
        { ItemId.BLESSING_CHARM, 0xEB0 },
        { ItemId.HEAVY_ARROWS, 0xF10 },
        { ItemId.BLOODSTAINED_TISSUE, 0xF70 },
        { ItemId.MAPLE_LEAF, 0xFD0 },
        { ItemId.FRESH_SPRING_LEAF, 0x1030 },
        { ItemId.POCKET_INCENSORY, 0x1090 },
        { ItemId.BIRTHSTONE, 0x10F0 },
        { ItemId.QUICK_ARROWS, 0x1150 },
        { ItemId.DRILLING_ARROWS, 0x11B0 },
        { ItemId.SEALED_WIND, 0x1210 },
        { ItemId.CINDER_KEY, 0x1270 },
        { ItemId.CF_BOW_LVL, 0x12D0 },
        { ItemId.CF_BOW_SPEED, 0x1230 },
        { ItemId.CF_DASH, 0x1390 },
        { ItemId.CF_WARP, 0x13F0 }
    };
    public static readonly Dictionary<(ItemId, ItemDropType), int> GotItemStringsOffsets = new() {
        { (ItemId.BELLFLOWER, ItemDropType.World), 0x21DC },
        { (ItemId.BELLFLOWER, ItemDropType.Reward), 0x5494 },
        { (ItemId.ASTRAL_CHARM, ItemDropType.World), 0x256C },
        { (ItemId.EDEAS_PEARL, ItemDropType.Boss), 0x28BC },
        { (ItemId.MAGNET_STONE, ItemDropType.World), 0x4434 },
        { (ItemId.ROTTEN_BELLFLOWER, ItemDropType.Reward), 0x160 },
        { (ItemId.PASSIFLORA, ItemDropType.Reward), 0x2338 },
        { (ItemId.PASSIFLORA, ItemDropType.World), 0x1F0C },
        { (ItemId.TAINTED_MISSIVE, ItemDropType.World), 0x4084 },
        { (ItemId.TAINTED_MISSIVE, ItemDropType.Boss), 0x4084 },
        { (ItemId.BLACK_SACHET, ItemDropType.World), 0x9F8 },
        { (ItemId.BAKMAN_PATCH, ItemDropType.Boss), 0x2C0C },
        { (ItemId.CAT_SPHERE, ItemDropType.World), 0x35CC },
        { (ItemId.HAZLE_BADGE, ItemDropType.Reward), 0x5020 },
        { (ItemId.TORN_BRANCH, ItemDropType.Boss), 0x317C },
        { (ItemId.MONASTERY_KEY, ItemDropType.World), 0x27C },
        { (ItemId.DIRTY_SHROOM, ItemDropType.World), 0x11B0 },
        { (ItemId.IVORY_BUG, ItemDropType.World), 0xcc4 },
        { (ItemId.SOFT_TISSUE, ItemDropType.World), 0x4A8C },
        { (ItemId.GARDEN_KEY, ItemDropType.World), 0x4E3C },
        { (ItemId.SPARSE_THREAD, ItemDropType.Boss), 0x3CAC },
        { (ItemId.BLESSING_CHARM, ItemDropType.Reward), 0x5A60 },
        { (ItemId.HEAVY_ARROWS, ItemDropType.Boss), 0x6C10 },
        { (ItemId.BLOODSTAINED_TISSUE, ItemDropType.Boss), 0x2788 },
        { (ItemId.FRESH_SPRING_LEAF, ItemDropType.World), 0x428 },
        { (ItemId.POCKET_INCENSORY, ItemDropType.Boss), 0x2D84 },
        { (ItemId.SEALED_WIND, ItemDropType.World), 0x68A0 },
        { (ItemId.CINDER_KEY, ItemDropType.World), 0x2934 },
        { (ItemId.CF_BOW_LVL, ItemDropType.World), 0x3F4 },
        { (ItemId.CF_BOW_SPEED, ItemDropType.World), 0x2EEC },
        { (ItemId.CF_DASH, ItemDropType.World), 0x2A58 },
        { (ItemId.CF_WARP, ItemDropType.World), 0x740 },
        { (ItemId.VITALITY_FRAGMENT, ItemDropType.World), 0x68A0 }
    };
    public static readonly Dictionary<ItemId, string[]> FakeStrings = new() { // Declaration of strings for items that dont have some of them in-game
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

    public static readonly Dictionary<ItemId, ItemDropType[]> DropTypes = new() { // One for each instance in the game
        { ItemId.ADORNED_RING, [ItemDropType.Starter] },
        { ItemId.NECKLACE_OF_SACRIFICE, [ItemDropType.Shop] },
        { ItemId.BELLFLOWER, [ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.Reward, ItemDropType.World] },
        { ItemId.ASTRAL_CHARM, [ItemDropType.World] },
        { ItemId.EDEAS_PEARL, [ItemDropType.Boss] },
        { ItemId.DULL_PEARL, [ItemDropType.Shop] },
        { ItemId.RED_RING, [ItemDropType.Shop] },
        { ItemId.MAGNET_STONE, [ItemDropType.World] },
        { ItemId.ROTTEN_BELLFLOWER, [ItemDropType.Reward] },
        { ItemId.FAERIE_TEAR, [ItemDropType.Shop] },
        { ItemId.IMPURITY_FLASK, [ItemDropType.Shop] },
        { ItemId.PASSIFLORA, [ItemDropType.Reward, ItemDropType.World] },
        { ItemId.CRYTAL_SEED, [ItemDropType.Shop] },
        { ItemId.MEDAL_OF_EQUIVALENCE, [ItemDropType.Starter] },
        { ItemId.TAINTED_MISSIVE, [ItemDropType.World, ItemDropType.Boss] },
        { ItemId.BLACK_SACHET, [ItemDropType.World] },
        { ItemId.RING_OF_CANDOR, [ItemDropType.Shop] },
        { ItemId.SMALL_COIN, [ItemDropType.Boss]  },
        { ItemId.BAKMAN_PATCH, [ItemDropType.Boss] },
        { ItemId.CAT_SPHERE, [ItemDropType.World] },
        { ItemId.HAZLE_BADGE, [ItemDropType.Reward] },
        { ItemId.TORN_BRANCH, [ItemDropType.Boss] },
        { ItemId.MONASTERY_KEY, [ItemDropType.World] },
        { ItemId.CLARITY_SHARD, [ItemDropType.Shop] },
        { ItemId.DIRTY_SHROOM, [ItemDropType.World] },
        { ItemId.IVORY_BUG, [ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World] },
        { ItemId.VIOLET_SPRITE, [ItemDropType.Shop] },
        { ItemId.SOFT_TISSUE, [ItemDropType.World] },
        { ItemId.GARDEN_KEY, [ItemDropType.World] },
        { ItemId.SPARSE_THREAD, [ItemDropType.Boss] },
        { ItemId.BLESSING_CHARM, [ItemDropType.Reward] },
        { ItemId.HEAVY_ARROWS, [ItemDropType.Boss] },
        { ItemId.BLOODSTAINED_TISSUE, [ItemDropType.Boss] },
        { ItemId.MAPLE_LEAF, [ItemDropType.Starter]  },
        { ItemId.FRESH_SPRING_LEAF, [ItemDropType.World] },
        { ItemId.POCKET_INCENSORY, [ItemDropType.Boss] }, // Treat it as a boss item for randomization purposes
        { ItemId.BIRTHSTONE, [ItemDropType.Boss]  },
        { ItemId.QUICK_ARROWS, [ItemDropType.Shop] },
        { ItemId.DRILLING_ARROWS, [ItemDropType.Shop] },
        { ItemId.SEALED_WIND, [ItemDropType.World] },
        { ItemId.CINDER_KEY, [ItemDropType.World] },
        { ItemId.CF_BOW_LVL, [ItemDropType.World] },
        { ItemId.CF_BOW_SPEED, [ItemDropType.World] },
        { ItemId.CF_DASH, [ItemDropType.World] },
        { ItemId.CF_WARP, [ItemDropType.World] },
        { ItemId.VITALITY_FRAGMENT, [ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World, ItemDropType.World] }
    };

    /// <summary>
    ///     Combines all required Items and their requirements in the given list.
    /// </summary>
    /// <param name="requirements">List of Items to get the depencency of.</param>
    /// <returns>List of distinct ItemId.</returns>
    private static ItemId[] CombineRequirements(params ItemId[] requirements)
        => [.. requirements, .. requirements.SelectMany(item => GetRquirements(item))];

    /// <summary>
    ///     Lists what other Items a user has to acquire to get a specific Item.
    /// </summary>
    /// <param name="itemId">Name of the Item.</param>
    /// <returns>List of distinct ItemId.</returns>
    public static ItemId[] GetRquirements(ItemId itemId)
        => ItemRequirements.TryGetValue(itemId, out var requirements) ? requirements : [];

    /// <summary>
    ///     Gets the Offsets base of the item string references
    /// </summary>
    /// <param name="gameVersion">Version of the game.</param>
    /// <returns>
    ///     Offsets base if the version is valid; otherwise, <c>empty array</c>.
    /// </returns>
    public static int[] GetStringsBase(GameVersion gameVersion)
        => StringsBase.TryGetValue(gameVersion, out var baseOffsets) ? baseOffsets : [];

    /// <summary>
    ///     Get Offset of item name.
    /// </summary>
    /// <param name="itemId">Id of the item</param>
    /// <returns>
    ///     Offset value if ItemId is valid; otherwise, <c>0</c>.
    /// </returns>
    public static int GetStringsOffset(ItemId itemId)
        => StringsOffsets.TryGetValue(itemId, out var stringOffset) ? stringOffset : 0;

    /// <summary>
    ///     Get the Spawn rooms of an item by Id.
    /// </summary>
    /// <param name="itemId">Id of the item.</param>
    /// <param name="spawnIndex">index of item, when multiple of the same can spawn</param>
    /// <returns>
    ///     Rooms where the item spawns; otherwise, <c>empty array</c>.
    /// </returns>
    public static int[] GetItemSpawnRooms(ItemId itemId, int spawnIndex) {
        if (!SpawnRooms.TryGetValue(itemId, out var Rooms) || Rooms.Length == 0 || spawnIndex >= Rooms.Length) {
            return [];
        }

        if (spawnIndex < 0) {
            return Rooms;
        }

        return [Rooms[spawnIndex]];
    }

    /// <summary>
    ///     Get Offset of the item pickup string.
    /// </summary>
    /// <returns>
    ///     Offset value if ItemId and dropType are valid; otherwise, <c>0</c>.
    /// </returns>
    public static int GetStringPickupOffset(ItemId itemId, ItemDropType dropType)
        => GotItemStringsOffsets.TryGetValue((itemId, dropType), out var stringOffset) ? stringOffset : 0;

    /// <summary>
    ///     Get the type of drop an item is.
    /// </summary>
    /// <param name="itemId">Id of the item</param>
    /// <param name="spawnIndex">index of item, when multiple of the same can spawn</param>
    /// <returns>
    ///     <see cref="ItemDropType"/> of that item at that location if it exists; otherwise, <see cref="ItemDropType.None"/>.
    /// </returns>
    internal static ItemDropType GetDropType(ItemId itemId, int spawnIndex) 
        => DropTypes.TryGetValue(itemId, out var Types) && spawnIndex >= 0 && spawnIndex < Types.Length
            ? Types[spawnIndex]
            : ItemDropType.None;
}