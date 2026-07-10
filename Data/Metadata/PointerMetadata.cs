using MomodoraRandomizer.Data.Enums;
using System.Collections.Generic;

namespace MomodoraRandomizer.Data.Metadata;

internal static class PointerMetadata{
    internal static readonly Dictionary<PointerNames, int[]> Offsets_105b = new() {
        { PointerNames.LEVEL_ID, [0x230F1A0] }
    };

    internal static readonly Dictionary<PointerNames, int[]> Offsets_107 = new() {
        { PointerNames.LEVEL_ID,                [0x237C360] },
        { PointerNames.MAP_X,                   [0x2371EA8, 0x4,   0x7B0] },
        { PointerNames.MAP_Y,                   [0x2371EA8, 0x4,   0x7C0] },
        { PointerNames.INVENTORY_SIZE,          [0x23782DC, 0x1AC, 0x8] },
        { PointerNames.INVENTORY_COUNT,         [0x23782DC, 0x1AC, 0x4] },
        { PointerNames.INVENTORY_START,         [0x23782DC, 0x1AC, 0xC] },
        { PointerNames.INVENTORY_CHARGES_START, [0x23782DC, 0x1B0, 0xC] }
    };

    internal static Dictionary<PointerNames, int[]> GetOffsets(GameVersion version) {
        return version switch {
            GameVersion.VERSION_1_05b => Offsets_105b,
            GameVersion.VERSION_1_07 => Offsets_107,
            _ => []
        };
    }
}