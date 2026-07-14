using MomodoraRandomizer.Data.Enums;
using System.Collections.Generic;

namespace MomodoraRandomizer.Data.Metadata {
    internal static class PointerMetadata{
        internal static readonly Dictionary<PointerNames, int[]> Offsets_105b = new Dictionary<PointerNames, int[]>() {
            { PointerNames.LEVEL_ID, new int[] { 0x230F1A0 } }
        };

        internal static readonly Dictionary<PointerNames, int[]> Offsets_107 = new Dictionary<PointerNames, int[]>() {
            { PointerNames.LEVEL_ID,                new int[] { 0x237C360  } },
            { PointerNames.MAP_X,                   new int[] { 0x2371EA8, 0x4,   0x7B0 } },
            { PointerNames.MAP_Y,                   new int[] { 0x2371EA8, 0x4,   0x7C0 } },
            { PointerNames.INVENTORY_SIZE,          new int[] { 0x23782DC, 0x1AC, 0x8   } },
            { PointerNames.INVENTORY_COUNT,         new int[] { 0x23782DC, 0x1AC, 0x4   } },
            { PointerNames.INVENTORY_START,         new int[] { 0x23782DC, 0x1AC, 0xC   } },
            { PointerNames.INVENTORY_CHARGES_START, new int[] { 0x23782DC, 0x1B0, 0xC   } }
        };

        internal static Dictionary<PointerNames, int[]> GetOffsets(GameVersion version) {
            switch (version) {
                case GameVersion.VERSION_1_05b:
                    return Offsets_105b;
                case GameVersion.VERSION_1_07:
                    return Offsets_107;
                default:
                    return new Dictionary<PointerNames, int[]>();
            };
        }
    }
}