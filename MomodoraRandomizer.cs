using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using System.Drawing.Drawing2D;
using System.IO;

namespace LiveSplit.UI.Components
{
    public class MomodoraRandomizer : IComponent
    {
        public enum Items : int
        {
            AdornedRing = 1,
            NecklaceOfSacrifice = 2,
            Bellflower = 4,
            AstralCharm = 5,
            EdeaPearl = 6,
            DullPearl = 7,
            RedRing = 8,
            MagnetStone = 9,
            RottenBellflower = 10,
            FaerieTear = 11,
            ImpurityFlask = 13,
            Passiflora = 14,
            CrystalSeed = 15,
            MedalOfEquivalence = 16,
            TaintedMissive = 17,
            BlackSachet = 18,
            RingOfCandor = 21,
            SmallCoin = 22,
            BackmanPatch = 23,
            CatSphere = 24,
            HazelBadge = 25,
            TornBranch = 26,
            MonasteryKey = 27,
            ClarityShard = 31,
            DirtyShroom = 32,
            IvoryBug = 34,
            VioletSprite = 35,
            SoftTissue = 36,
            GardenKey = 37,
            SparseThread = 38,
            BlessingCharm = 39,
            HeavyArrows = 40,
            BloodstainedTissue = 41,
            MapleLeaf = 42,
            FreshSpringLeaf = 43,
            PocketIncensory = 44,
            Birthstone = 45,
            QuickArrows = 46,
            DrillingArrows = 47,
            SealedWind = 48,
            CinderKey = 49,
            FragmentBowPow = 50,
            FragmentBowQuick = 51,
            FragmentDash = 52,
            FragmentWarp = 53,
            VitalityFragment = 54
        };
        public enum shops : int
        {
            KarstCity = 0,
            ForlornMonsatery = 1,
            SubterraneanGrave = 2,
            WhiteleafMemorialPark = 3,
            CinderChambers_1 = 4,
            CinderChambers_2 = 5,
            RoyalPinacotheca = 6,
        }

        const int RANDOMIZER_SOURCE_AMOUNT = 83;

        private SimpleLabel RandomizerLabel;
        private Process gameProc = null;
        private Random randomGenerator = null;
        private int seed = 0;
        private double gameVersion;
        private MomodoraRandomizerSettings settingsControl;
        public LiveSplitState state;

        #region Randomizer logic fields
        private MemoryWatcherList randoSourceWatchers;
        private List<int> bannedSources;
        private List<int> usedSources;
        private List<int> possibleSources;
        private List<int> impossibleSources;
        private List<int> placedItems;
        private List<List<int>> requirementLists;

        //                                         CatSphere, Crest, Garden, Cinder, Mona, Haze, Soft, Dirty, Sealed Wind,Bugs
        private bool[,] requirementMatrix = new bool[9,10];

        private Dictionary<int, int> sourceIdMapping = new Dictionary<int, int>
        {
            [0] = (int)Items.Bellflower,
            [1] = (int)Items.AstralCharm,
            [2] = (int)Items.Bellflower,
            [3] = (int)Items.MagnetStone,
            [4] = (int)Items.GardenKey,
            [5] = (int)Items.CinderKey,
            [6] = (int)Items.MonasteryKey,
            [7] = (int)Items.TaintedMissive,
            [8] = (int)Items.CrystalSeed,
            [9] = (int)Items.FaerieTear,
            [10] = (int)Items.RingOfCandor,
            [11] = (int)Items.ClarityShard,
            [12] = (int)Items.NecklaceOfSacrifice,
            [13] = (int)Items.DullPearl,
            [14] = (int)Items.RedRing,
            [15] = (int)Items.DrillingArrows,
            [16] = (int)Items.ImpurityFlask,
            [17] = (int)Items.VioletSprite,
            [18] = (int)Items.QuickArrows,
            [19] = (int)Items.PocketIncensory,
            [20] = (int)Items.BlackSachet,
            [21] = (int)Items.SealedWind,
            [22] = (int)Items.Bellflower,
            [23] = (int)Items.Passiflora,
            [24] = (int)Items.DirtyShroom,
            [25] = (int)Items.CatSphere,
            [26] = (int)Items.Bellflower,
            [27] = (int)Items.SoftTissue,
            [28] = (int)Items.FreshSpringLeaf,
            [29] = (int)Items.BlessingCharm,
            [30] = (int)Items.RottenBellflower,
            [31] = (int)Items.EdeaPearl,
            [32] = (int)Items.BackmanPatch,
            [33] = (int)Items.SparseThread,
            [34] = (int)Items.PocketIncensory,
            [35] = (int)Items.TornBranch,
            [36] = (int)Items.TaintedMissive,
            [37] = (int)Items.BloodstainedTissue,
            [38] = (int)Items.HeavyArrows,
            [39] = (int)Items.VitalityFragment,
            [40] = (int)Items.VitalityFragment,
            [41] = (int)Items.VitalityFragment,
            [42] = (int)Items.VitalityFragment,
            [43] = (int)Items.VitalityFragment,
            [44] = (int)Items.VitalityFragment,
            [45] = (int)Items.VitalityFragment,
            [46] = (int)Items.VitalityFragment,
            [47] = (int)Items.VitalityFragment,
            [48] = (int)Items.VitalityFragment,
            [49] = (int)Items.VitalityFragment,
            [50] = (int)Items.VitalityFragment,
            [51] = (int)Items.VitalityFragment,
            [52] = (int)Items.VitalityFragment,
            [53] = (int)Items.VitalityFragment,
            [54] = (int)Items.VitalityFragment,
            [55] = (int)Items.VitalityFragment,
            [56] = (int)Items.IvoryBug,
            [57] = (int)Items.IvoryBug,
            [58] = (int)Items.IvoryBug,
            [59] = (int)Items.IvoryBug,
            [60] = (int)Items.IvoryBug,
            [61] = (int)Items.IvoryBug,
            [62] = (int)Items.IvoryBug,
            [63] = (int)Items.IvoryBug,
            [64] = (int)Items.IvoryBug,
            [65] = (int)Items.IvoryBug,
            [66] = (int)Items.IvoryBug,
            [67] = (int)Items.IvoryBug,
            [68] = (int)Items.IvoryBug,
            [69] = (int)Items.IvoryBug,
            [70] = (int)Items.IvoryBug,
            [71] = (int)Items.IvoryBug,
            [72] = (int)Items.IvoryBug,
            [73] = (int)Items.IvoryBug,
            [74] = (int)Items.IvoryBug,
            [75] = (int)Items.IvoryBug,
            [80] = (int)Items.Bellflower,
            [82] = (int)Items.Passiflora
        };
        private Dictionary<int, int[]> sourceToLevelMapping = new Dictionary<int, int[]>
        {
            [0] = new int[] { 25 },
            [1] = new int[] { 37 },
            [2] = new int[] { 70, 64 },
            [3] = new int[] { 90 },
            [4] = new int[] { 134 },
            [5] = new int[] { 191 },
            [6] = new int[] { 113 },
            [7] = new int[] { 117 },
            [8] = new int[] { 63 },
            [9] = new int[] { 63, 111 },
            [10] = new int[] { 63, 181, 187 },
            [11] = new int[] { 127 },
            [12] = new int[] { 127 },
            [13] = new int[] { 160 },
            [14] = new int[] { 181 },
            [15] = new int[] { 187 },
            [16] = new int[] { 187 },
            [17] = new int[] { 199 },
            [18] = new int[] { 199 },
            [19] = new int[] { 199 },
            [20] = new int[] { 204 },
            [21] = new int[] { 220 },
            [22] = new int[] { 269 },
            [23] = new int[] { 261 },
            [24] = new int[] { 126 },
            [25] = new int[] { 149 },
            [26] = new int[] { 162 },
            [27] = new int[] { 103 },
            [28] = new int[] { 83 },
            [29] = new int[] { 52 },
            [30] = new int[] { 46 },
            [31] = new int[] { 53 },
            [32] = new int[] { 73 },
            [33] = new int[] { 141 },
            [34] = new int[] { 193 },
            [35] = new int[] { 153 },
            [36] = new int[] { 104 },
            [37] = new int[] { 97 },
            [38] = new int[] { 212 },
            [39] = new int[] { 39 },
            [40] = new int[] { 47 },
            [41] = new int[] { 67 },
            [42] = new int[] { 81 },
            [43] = new int[] { 144 },
            [44] = new int[] { 168 },
            [45] = new int[] { 191 },
            [46] = new int[] { 108 },
            [47] = new int[] { 35 },
            [48] = new int[] { 58 },
            [49] = new int[] { 185 },
            [50] = new int[] { 199 },
            [51] = new int[] { 205 },
            [52] = new int[] { 209 },
            [53] = new int[] { 270 },
            [54] = new int[] { 246 },
            [55] = new int[] { 264 },
            [56] = new int[] { 41 },
            [57] = new int[] { 50 },
            [58] = new int[] { 62 },
            [59] = new int[] { 78 },
            [60] = new int[] { 170 },
            [61] = new int[] { 191 },
            [62] = new int[] { 183 },
            [63] = new int[] { 40 },
            [64] = new int[] { 56 },
            [65] = new int[] { 123 },
            [66] = new int[] { 152 },
            [67] = new int[] { 155 },
            [68] = new int[] { 156 },
            [69] = new int[] { 102 },
            [70] = new int[] { 103 },
            [71] = new int[] { 211 },
            [72] = new int[] { 249 },
            [73] = new int[] { 255 },
            [74] = new int[] { 214 },
            [75] = new int[] { 213 },
            [76] = new int[] { 142 },
            [77] = new int[] { 195 },
            [78] = new int[] { 105 },
            [79] = new int[] { 60 },
            [80] = new int[] { 163 },
            [81] = new int[] { 163 },
            [82] = new int[] { 163 },
        };
        #endregion

        #region log	
        private List<string> Events;
        #endregion

        private List<int> vitalityFragments;
        private List<int> ivoryBugs;
        private List<int> bossItems;

        #region pointers
        
        IntPtr[] potentialSourcesPointers;

        #region charge item pointers
        IntPtr bellflowerSaveValuePointer;
        IntPtr bellflowerMaxValuePointer;
        IntPtr taintedMissiveSaveValuePointer;
        IntPtr taintedMissiveMaxValuePointer;
        IntPtr passifloraSaveValuePointer;
        IntPtr passifloraMaxValuePointer;
        #endregion

        #region ivory bug pointers
        IntPtr ivoryBugCountPointer;
        #endregion

        #region vitality fragment pointers
        IntPtr vitalityFragmentCountPointer;
        IntPtr maxHealthPointer;
        IntPtr currentHealthPointer;
        #endregion

        #region crest fragment pointers
        IntPtr crestFragmentCountPointer;
        #endregion

        #region inventory pointers
        IntPtr activeItemsPointer;
        IntPtr passiveItemsPointer;
        IntPtr keyItemsPointer;
        IntPtr totalItemsPointer;
        IntPtr inventoryItemsStartPointer;
        IntPtr inventoryItemsChargeStartPointer;
        #endregion

        #region Shop pointers
        IntPtr munnyPointer;
        IntPtr invOpenPointer;
        IntPtr convOpenPointer;
        IntPtr shopPointer;
        IntPtr itemInfoPointer;
        #endregion

        #region misc pointers
        IntPtr difficultyPointer;
        IntPtr levelIDPointer;
        IntPtr inGamePointer;
        IntPtr saveAmountPointer;
        IntPtr deathAmountPointer;
        IntPtr warpStartPointer;
        IntPtr playerFormPointer;
        IntPtr playerXPointer;
        IntPtr playerYPointer;
        private IntPtr lubella2Pointer;
        #endregion
        #endregion

        #region Memory Watchers
        private MemoryWatcher<double> taintedMissiveWatcher;
        private MemoryWatcher<double> bellflowerWatcher;
        private MemoryWatcher<double> passifloraWatcher;
        private MemoryWatcher<double> ivoryBugWatcher;
        private MemoryWatcher<double> crestFragmentWatcher;
        private MemoryWatcher<double> vitalityFragmentWatcher;
        private MemoryWatcher<int> levelIDWatcher;
        private MemoryWatcher<double> deathWatcher;
        private MemoryWatcher<double> saveWatcher;
        private MemoryWatcher<double> inGameWatcher;
        private MemoryWatcher<double> munnyWatcher;
        private MemoryWatcher<double> invOpenWatcher;
        private MemoryWatcher<double> convOpenWatcher;
        private MemoryWatcher<double> playerYWatcher;
        private MemoryWatcher<double> currentHealthWatcher;
        #endregion

        private bool randomizerRunning;
        private int itemGiven;
        private double[] healthChange;

        #region item acquired bools
        List<bool> hasChargeItem;
        List<bool> hasSavedChargeItem;
        private bool hasFoundGreenLeaf;
        private bool hasSavedFoundGreenLeaf;
        private bool hasBathedLeaf;
        private bool hasSavedBathedLeaf;
        private bool hasCatSphere;
        private bool hasSavedCatSphere;

        List<List<int>> doorLocations;
        private double unlocked;
        List<int> hasSavedKey;
        List<int> hasKey;
        private bool hasWarp;
        private bool hasSavedWarp;
        #endregion

        #region shop fields
        List<int> shopLocations;
        List<List<int>> originalShopItems;
        List<List<int>> shopItems;
        List<List<int>> shopOffsets;
        List<List<bool>> hasBoughtItem;
        List<List<bool>> hasSavedBoughtItem;
        List<List<int>> pointerValues;
        List<string> itemNames = new List<string>
        {
            "Small Coin",
            "Karst Crest",
            "Birthstone",
        };
        List<string> itemEffects = new List<string>
        {
            "Key Item.",
            "Key Item. Allows warping when praying.",
            "Key Item.",
        };
        #endregion

        private bool inGame;

        public MomodoraRandomizer(LiveSplitState state)
        {
            this.state = state;
            RandomizerLabel = new SimpleLabel();
            settingsControl = new MomodoraRandomizerSettings();
            bannedSources = new List<int>();
            usedSources = new List<int>();
            possibleSources = new List<int>();
            impossibleSources = new List<int>();
            placedItems = new List<int>();

            InitializeLists();
         
            state.OnStart += OnStart;
            state.OnReset += OnReset;
        }

        private void InitializeLists()
        {
            List<int> requiresCatSphere;
            List<int> requiresCrestFragments;
            List<int> requiresGardenKey;
            List<int> requiresCinderKey;
            List<int> requiresMonasteryKey;
            List<int> requiresHazelBadge;
            List<int> requiresDirtyShroom;
            List<int> requiresSoftTissue;
            List<int> requiresSealedWind;
            List<int> requiresIvoryBugs;

            requirementLists = new List<List<int>>();
            requiresCatSphere = new List<int> { 24, 27, 39, 47, 48, 55, 63, 64, 65, 66, 67, 68, 70, 74, 75, 79 };
            requirementLists.Add(requiresCatSphere);
            requiresCrestFragments = new List<int> { 0, 2, 17, 18, 19, 20, 21, 22, 23, 38, 39, 47, 50, 51, 52, 53, 54, 55, 71, 72, 73, 74, 75 };
            requirementLists.Add(requiresCrestFragments);
            requiresGardenKey = new List<int> { 66, 67, 68, 35, 26, 25, 13 };
            requirementLists.Add(requiresGardenKey);
            requiresCinderKey = new List<int> { 49 };
            requirementLists.Add(requiresCinderKey);
            requiresMonasteryKey = new List<int> { 27, 36, 69, 70, 78 };
            requirementLists.Add(requiresMonasteryKey);
            requiresHazelBadge = new List<int> { 29 };
            requirementLists.Add(requiresHazelBadge);
            requiresSoftTissue = new List<int> { 37 };
            requirementLists.Add(requiresSoftTissue);
            requiresDirtyShroom = new List<int> { 30 };
            requirementLists.Add(requiresDirtyShroom);
            requiresSealedWind = new List<int> { 28 };
            requirementLists.Add(requiresSealedWind);
            requiresIvoryBugs = new List<int> { 80, 81, 82 };
            requirementLists.Add(requiresIvoryBugs);
            vitalityFragments = new List<int>();
            for (int i = 39; i <= 55; i++)
            {
                vitalityFragments.Add(i);
            }
            ivoryBugs = new List<int>();
            for (int i = 56; i <= 75; i++)
            {
                ivoryBugs.Add(i);
            }
            bossItems = new List<int>();
            for (int i = 31; i <= 38; i++)
            {
                bossItems.Add(i);
            }


            //GardenKey, MonasteryKey, CinderKey
            doorLocations = new List<List<int>>
            {
                new List<int> { 145, 84},
                new List<int> { 183 },
                new List<int> { 99 }
            };
            //Karst City, Forlorn Monsatery, Subterranean Grave, Whiteleaf Memorial Park, Cinder Chambers 1, Cinder Chambers 2, Royal Pinacotheca
            shopLocations = new List<int>
            {
                63, 111, 127, 160, 181, 187, 205
            };
            shopLocations = new List<int>
            {
                0,0,0,0,0,0,0
            };
            //Karst City, Forlorn Monsatery, Subterranean Grave, Whiteleaf Memorial Park, Cinder Chambers 1, Cinder Chambers 2, Royal Pinacotheca
            originalShopItems = new List<List<int>>
            {
                new List<int> { 8, 9, 10 },
                new List<int> { 9 },
                new List<int> { 11, 12 },
                new List<int> { 13 },
                new List<int> { 14, 10 },
                new List<int> { 15, 10, 16 },
                new List<int> { 17, 18, 19 }
            };
        }

        private void OnReset(object sender, TimerPhase value)
        {
            randomizerRunning = false;
            if (VerifyProcessRunning())
            {
                foreach (var room in shopLocations)
                {
                    ResetShopItems(room);
                }
            }
            logResults();
        }

        private void OnStart(object sender, EventArgs e)
        {
            if (VerifyProcessRunning())
            {
                SetupVersionDifferences();
                //If set seed ->
                if (!settingsControl.RandomSeed)
                {
                    int.TryParse(settingsControl.seed_get(), out seed);
                    randomGenerator = new Random(seed);
                }
                else
                {
                    randomGenerator = new Random();
                    seed = randomGenerator.Next();
                    settingsControl.seed_set(seed);
                    randomGenerator = new Random(seed);
                }

                Debug.WriteLine("Using seed " + seed);

                hasSavedChargeItem = new List<bool> { false, false, false };
                hasChargeItem = new List<bool> { false, false, false };
                hasSavedKey = new List<int> { 0, 0, 0 };
                hasKey = new List<int> { 0, 0, 0 };
                hasBathedLeaf = false;
                hasSavedBathedLeaf = false;
                hasFoundGreenLeaf = false;
                hasSavedFoundGreenLeaf = false;
                // Place original numbers for now, they get changed later
                shopItems = new List<List<int>>
                {
                    new List<int> { 15, 11, 21 },
                    new List<int> { 11 },
                    new List<int> { 31, 2 },
                    new List<int> { 7 },
                    new List<int> { 8, 21 },
                    new List<int> { 47, 21, 13 },
                    new List<int> { 35, 46, 44 }
                };
                hasBoughtItem = new List<List<bool>>
                {
                    new List<bool> { false, false, false },
                    new List<bool> { false },
                    new List<bool> { false, false },
                    new List<bool> { false },
                    new List<bool> { false, false },
                    new List<bool> { false, false, false },
                    new List<bool> { false, false, false }
                };
                hasSavedBoughtItem = new List<List<bool>>
                {
                    new List<bool> { false, false, false },
                    new List<bool> { false },
                    new List<bool> { false, false },
                    new List<bool> { false },
                    new List<bool> { false, false },
                    new List<bool> { false, false, false },
                    new List<bool> { false, false, false }
                };
                pointerValues = new List<List<int>>
                {
                    new List<int> { },
                    new List<int> { },
                    new List<int> { }
                };
                hasWarp = false;
                hasSavedWarp = false;
                Events = new List<string> { };

                ResetSources();
                UpdateBannedSources();
                Array.Clear(requirementMatrix, 0, requirementMatrix.Length);
                randoSourceWatchers = new MemoryWatcherList();

                //Key items are played in order: Cat Sphere, Crest Fragments, Garden Key, Cinder Key, Monastery Key, (Hazel Badge, Soft Tissue, Dirty Shroom, Ivory Bugs)
                #region item placement
                //0: Dirty Shroom - don't randomize, just update requirement matrix
                #region dirty shroom
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(24))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[7, j] = requirementMatrix[i, j];
                        requirementMatrix[7, i] = true;
                    }
                }
                #endregion

                //1. Place Cat Sphere
                #region cat sphere

                int index = NextIndex((int)Items.CatSphere);
                CreateMemoryWatcher((int)Items.CatSphere, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index])) requirementMatrix[0,i] = true;
                }
                placedItems.Add(25);
                #endregion

                //2: Place Crest Fragments
                #region crest Fragments
                index = NextIndex((int)Items.FragmentBowPow);
                CreateMemoryWatcher((int)Items.FragmentBowPow, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }
                placedItems.Add(78);

                index = NextIndex((int)Items.FragmentBowPow);
                CreateMemoryWatcher((int)Items.FragmentBowQuick, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }
                placedItems.Add(76);

                index = NextIndex((int)Items.FragmentBowPow);
                CreateMemoryWatcher((int)Items.FragmentDash, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }
                placedItems.Add(77);

                index = NextIndex((int)Items.FragmentBowPow);
                CreateMemoryWatcher((int)Items.FragmentWarp, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }
                placedItems.Add(79);
                #endregion

                //3: Garden Key
                #region garden Key
                //If hard mode: decide to place garden key or bakman patch reachable first, and then place the other one "unreachable"
                if (settingsControl.HardModeEnabled)
                {
                    bool gardenKeyFirst = Convert.ToBoolean(randomGenerator.Next(0, 1));

                    index = NextIndex((int)Items.GardenKey);
                    if (gardenKeyFirst) CreateMemoryWatcher((int)Items.GardenKey, possibleSources[index]);
                    else CreateMemoryWatcher((int)Items.BackmanPatch, possibleSources[index]);

                    for (int i = 0; i < requirementLists.Count; i++)
                    {
                        if (requirementLists[i].Contains(possibleSources[index]))
                        {
                            for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[2, j] = requirementMatrix[i, j];
                            requirementMatrix[2, i] = true;
                        }
                    }
                    AlternatePossibleSources();
                    index = randomGenerator.Next(possibleSources.Count());
                    usedSources.Add(possibleSources[index]);

                    if (gardenKeyFirst) CreateMemoryWatcher((int)Items.BackmanPatch, possibleSources[index]);   
                    else CreateMemoryWatcher((int)Items.GardenKey, possibleSources[index]);

                    placedItems.Add(4);
                    placedItems.Add(32);

                }
                else
                {
                    index = NextIndex((int)Items.GardenKey);
                    CreateMemoryWatcher((int)Items.GardenKey, possibleSources[index]);
                    for (int i = 0; i < requirementLists.Count; i++)
                    {
                        if (requirementLists[i].Contains(possibleSources[index]))
                        {
                            for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[2, j] = requirementMatrix[i, j];
                            requirementMatrix[2, i] = true;
                        }
                    }
                    placedItems.Add(4);
                }
                #endregion

                //4: Cinder Key
                #region cinder Key
                index = NextIndex((int)Items.CinderKey);
                CreateMemoryWatcher((int)Items.CinderKey, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[3, j] = requirementMatrix[i, j];
                        requirementMatrix[3, i] = true;
                    }
                }
                placedItems.Add(5);
                #endregion

                //5: Monastery Key
                #region monastery Key
                index = NextIndex((int)Items.MonasteryKey);
                CreateMemoryWatcher((int)Items.MonasteryKey, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[4, j] = requirementMatrix[i, j];
                        requirementMatrix[4, i] = true;
                    }
                }
                placedItems.Add(6);
                #endregion

                //6: Hazel Badge
                #region hazel badge
                index = NextIndex((int)Items.HazelBadge);
                CreateMemoryWatcher((int)Items.HazelBadge, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[5, j] = requirementMatrix[i, j];
                        requirementMatrix[5, i] = true;
                    }
                }
                placedItems.Add(81);
                #endregion

                //7: Soft Tissue
                #region soft tissue
                index = NextIndex((int)Items.SoftTissue);
                CreateMemoryWatcher((int)Items.SoftTissue, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[6, j] = requirementMatrix[i, j];
                        requirementMatrix[6, i] = true;
                    }
                }
                placedItems.Add(27);
                #endregion

                //8: Sealed Wind
                #region sealed wind
                index = NextIndex((int)Items.SealedWind);
                CreateMemoryWatcher((int)Items.SealedWind, possibleSources[index]);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(possibleSources[index]))
                    {
                        for (int j = 0; j < requirementMatrix.GetLength(0); j++) requirementMatrix[8, j] = requirementMatrix[i, j];
                        requirementMatrix[8, i] = true;
                    }
                }
                placedItems.Add(21);
                #endregion

                //9. Place Ivory Bugs
                #region Ivory Bugs
                if (settingsControl.IvoryBugsEnabled)
                {
                    Event("\nIvory Bug randomization enabled\n");
                    for (int i = 56; i < 76; i++)
                    {
                        index = NextIndex((int)Items.IvoryBug);
                        CreateMemoryWatcher((int)Items.IvoryBug, possibleSources[index]);
                        placedItems.Add(i);
                    }
                }
                else Event("\nIvory Bug randomization disabled\n");
                #endregion

                //10. Place vitality fragments
                #region vitality fragments
                if (settingsControl.VitalityFragmentsEnabled)
                {
                    Event("\nVitality Fragment randomization enabled\n");
                    for (int i = 39; i < 56; i++)
                    {
                        index = NextIndex();
                        CreateMemoryWatcher((int)Items.VitalityFragment, possibleSources[index]);
                        placedItems.Add(i);
                    }
                }
                else Event("Vitality Fragment randomization disabled\n");
                #endregion

                //11. Rest of items
                #region rest of items
                for (int i = 0; i < RANDOMIZER_SOURCE_AMOUNT; i++)
                {
                    if (!bannedSources.Contains(i) && !placedItems.Contains(i))
                    {
                        index = NextIndex();
                        CreateMemoryWatcher(sourceIdMapping[i], possibleSources[index]);
                    }
                    
                }
                for(int i = 0; i < shopItems.Count(); i++)
                {
                    Debug.WriteLine("At shop " + Enum.GetName(typeof(shops), i));
                    for (int j = 0; j < shopItems[i].Count(); j++)
                    {
                        Debug.WriteLine("\t-" + Enum.GetName(typeof(Items), shopItems[i][j]) + " generated at " +
                                        Enum.GetName(typeof(Items), sourceIdMapping[originalShopItems[i][j]]));
                    }
                }
                #endregion
                #endregion

                #region Special memory watchers
                taintedMissiveWatcher = new MemoryWatcher<double>(taintedMissiveMaxValuePointer);
                taintedMissiveWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                taintedMissiveWatcher.Enabled = true;

                bellflowerWatcher = new MemoryWatcher<double>(bellflowerMaxValuePointer);
                bellflowerWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                bellflowerWatcher.Enabled = true;

                passifloraWatcher = new MemoryWatcher<double>(passifloraMaxValuePointer);
                passifloraWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                passifloraWatcher.Enabled = true;

                ivoryBugWatcher = new MemoryWatcher<double>(ivoryBugCountPointer);
                ivoryBugWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                ivoryBugWatcher.Enabled = true;

                crestFragmentWatcher = new MemoryWatcher<double>(crestFragmentCountPointer);
                crestFragmentWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                crestFragmentWatcher.Enabled = true;

                vitalityFragmentWatcher = new MemoryWatcher<double>(vitalityFragmentCountPointer);
                vitalityFragmentWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                vitalityFragmentWatcher.Enabled = true;

                levelIDWatcher = new MemoryWatcher<int>(levelIDPointer);
                levelIDWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                levelIDWatcher.Enabled = true;
                levelIDWatcher.OnChanged += (old, current) =>
                {
                    CheckRoom(old, current);
                };

                inGameWatcher = new MemoryWatcher<double>(inGamePointer);
                inGameWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                inGameWatcher.Enabled = true;
                inGameWatcher.OnChanged += (old, current) =>
                {
                    inGame = Convert.ToBoolean(current);
                    if (current == 0)
                    {
                        LoadVariables();
                    }
                };

                deathWatcher = new MemoryWatcher<double>(deathAmountPointer);
                deathWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                deathWatcher.Enabled = true;
                deathWatcher.OnChanged += (old, current) =>
                {
                    if (current > old)
                    {
                        LoadVariables();
                    }
                };

                saveWatcher = new MemoryWatcher<double>(saveAmountPointer);
                saveWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                saveWatcher.Enabled = true;
                saveWatcher.OnChanged += (old, current) =>
                {
                    if (current > old)
                    {
                        SaveVariables();
                    }
                };

                invOpenWatcher = new MemoryWatcher<double>(invOpenPointer);
                invOpenWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                invOpenWatcher.Enabled = true;
                invOpenWatcher.OnChanged += (old, current) =>
                {
                    InvOpen(current);
                };

                munnyWatcher = new MemoryWatcher<double>(munnyPointer);
                munnyWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                munnyWatcher.Enabled = true;
                munnyWatcher.OnChanged += (old, current) =>
                {
                    if (current < old)
                    {
                        ItemBought();
                    }
                };

                convOpenWatcher = new MemoryWatcher<double>(convOpenPointer);
                convOpenWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                convOpenWatcher.Enabled = true;
                convOpenWatcher.OnChanged += (old, current) =>
                {
                    InShop(current);
                };

                //This needs to be setup whenever entering a new room, currently only used for green leaf stuff and only active in levelID 82. 
                //This is set up in the checkRoom function
                playerYWatcher = new MemoryWatcher<double>(playerYPointer);
                playerYWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                playerYWatcher.Enabled = false;
                playerYWatcher.OnChanged += (old, current) =>
                {
                    if (levelIDWatcher.Current == 82)
                    {
                        double pickedUpLeaf = gameProc.ReadValue<double>(potentialSourcesPointers[28]);
                        double playerXPos = gameProc.ReadValue<double>(playerXPointer);
                        if (playerYWatcher.Current > 176 && playerXPos > 410)
                            //If the state of "has picked green leaf source" is different from "found green leaf" invert
                            if (Convert.ToBoolean(pickedUpLeaf) != hasBathedLeaf) gameProc.WriteValue<double>(potentialSourcesPointers[28], 1 - pickedUpLeaf);
                        else { 
                            gameProc.WriteValue<double>(potentialSourcesPointers[28], pickedUpLeaf);
                        }
  
                    };
                };

                currentHealthWatcher = new MemoryWatcher<double>(currentHealthPointer);
                currentHealthWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                currentHealthWatcher.Enabled = true;
                currentHealthWatcher.OnChanged += (old, current) =>
                {
                    if(old == 0 && current != 0)
                    {
                        Debug.WriteLine("Respawning");
                        itemGiven = 3;
                    }
                };

                //Changed from using specialWatchers list to updating each one manually in update, makes it so you can use levelIDWatcher.current and stuff in other places if needed

                #endregion

                AddItem((int)Items.IvoryBug);
                inGame = true;
                randomizerRunning = true;
                itemGiven = 3;
            }
        }

        private void SaveVariables()
        {
            Event("\nSaving variables\n");
            Events.Add("Item Charges & keys:\n");
            for (int i = 0; i < hasChargeItem.Count(); i++)
            {
                Events.Add("SavedChargeItem[" + i+"]: " + hasSavedChargeItem[i] + " = ChargeItem["+i+"]: " + hasChargeItem[i] + "\n");
                Events.Add("SavedKey["+i+"]: " + hasSavedKey[i] + " = Key["+i+"]: " + hasKey[i] + "\n");
                hasSavedChargeItem[i] = hasChargeItem[i];
                hasSavedKey[i] = hasKey[i];
            }
            Events.Add("\nShop items:\n");
            for (int i = 0; i < hasBoughtItem.Count(); i++)
            {
                Events.Add("\n" + Enum.GetName(typeof(shops), i) + " shop:\n");
                for (int j = 0; j < hasBoughtItem[i].Count(); j++)
                {
                    Events.Add("SavedItemBought["+i+"]["+j+"]: " + hasSavedBoughtItem[i][j] +
                               " = ItemBought["+i+"]["+j+"]: " + hasBoughtItem[i][j] + "\n");
                    hasSavedBoughtItem[i][j] = hasBoughtItem[i][j];
                }
            }
            Events.Add("\nSavedLeafBathed: " + hasSavedBathedLeaf + " = LeafBathed: " + hasBathedLeaf + "\n");
            hasSavedBathedLeaf = hasBathedLeaf;
            Events.Add("SavedLeafFound: " + hasSavedFoundGreenLeaf + " = LeafFound: " + hasFoundGreenLeaf + "\n");
            hasSavedFoundGreenLeaf = hasFoundGreenLeaf;
            Events.Add("SavedhasWarp: " + hasSavedWarp + " = hasWarp: " + hasWarp + "\n");
            hasSavedWarp = hasWarp;
            Events.Add("SavedhasCatSphere: " + hasSavedCatSphere + " = hasCatSphere: " + hasCatSphere + "\n");
            hasSavedCatSphere = hasCatSphere;
            Events.Add("\nFinished saving variables\n");
        }

        private void LoadVariables()
        {
            Event("\nLoading saved variables\n");
            Events.Add("Item Charges & keys:\n");
            for (int i = 0; i < hasChargeItem.Count(); i++)
            {
                Events.Add("ChargeItem["+i+"]: " + hasChargeItem[i] + " = SavedChargeItem["+i+"]: " + hasSavedChargeItem[i] + "\n");
                Events.Add("Key["+i+"]: " + hasKey[i] +  " = SavedKey["+i+"]: " + hasSavedKey[i] + "\n");
                hasChargeItem[i] = hasSavedChargeItem[i];
                hasKey[i] = hasSavedKey[i];
            }
            Events.Add("\nShop items:\n");
            for (int i = 0; i < hasBoughtItem.Count(); i++)
            {
                Events.Add("\n" + Enum.GetName(typeof(shops), i) + " shop:\n");
                for (int j = 0; j < hasBoughtItem[i].Count(); j++)
                {
                    Events.Add("hasBoughtItem["+i+"]["+j+"]: " + hasBoughtItem[i][j] + 
                               " = Saved["+i+"]["+j+"]: " + hasSavedBoughtItem[i][j] + "\n");
                    hasBoughtItem[i][j] = hasSavedBoughtItem[i][j];
                }
            }
            Events.Add("LeafBathed: " + hasBathedLeaf + " = SavedLeafBathed: " + hasSavedFoundGreenLeaf + "\n");
            hasBathedLeaf = hasSavedBathedLeaf;
            Events.Add("LeafFound: " + hasFoundGreenLeaf + " = SavedLeafFound: " + hasSavedWarp + "\n");
            hasFoundGreenLeaf = hasSavedFoundGreenLeaf;
            Events.Add("hasWarp: " + hasWarp + " = SavedSavedhasWarp: " + hasSavedBathedLeaf + "\n");
            hasWarp = hasSavedWarp;
            Events.Add("hasCatSphere: " + hasCatSphere + " = SavedhasCatSphere: " + hasSavedCatSphere + "\n");
            hasCatSphere = hasSavedCatSphere;
            Events.Add("\nFinished loading variables\n");
        }

        private int NextIndex(int itemId = 0)
        {
            if (itemId != 0) UpdateImpossibleSources(itemId);
            else impossibleSources.Clear();
            UpdatePossibleSources();
            int index = randomGenerator.Next(possibleSources.Count);
            usedSources.Add(possibleSources[index]);
            return index;
        }

        private void ResetSources()
        {
            bannedSources.Clear();
            impossibleSources.Clear();
            usedSources.Clear();
            possibleSources.Clear();
            placedItems.Clear();
        }

        private void CreateMemoryWatcher(int giveItemID, int newSourceAddressIndex)
        {
            if (8 <= newSourceAddressIndex && newSourceAddressIndex <= 19)// If item is a shop item
            {
                SaveShopItem(newSourceAddressIndex, giveItemID);
            }
            else
            {
                if(sourceIdMapping.ContainsKey(newSourceAddressIndex))
                    Debug.WriteLine("Item " + Enum.GetName(typeof(Items), giveItemID) + " generated at " + Enum.GetName(typeof(Items), sourceIdMapping[newSourceAddressIndex]));
                else
                    Debug.WriteLine("Item " + Enum.GetName(typeof(Items), giveItemID) + " generated at position " + newSourceAddressIndex);
                MemoryWatcher<double> temp = new MemoryWatcher<double>(potentialSourcesPointers[newSourceAddressIndex]);
                temp.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                if (potentialSourcesPointers[newSourceAddressIndex] == potentialSourcesPointers[28])
                {
                    temp.OnChanged += (old, current) =>
                    {
                        int levelID = gameProc.ReadValue<int>(levelIDPointer);
                        if (current == 1 && sourceToLevelMapping[newSourceAddressIndex].Contains(levelID))
                        {
                            hasBathedLeaf = true;
                            NewItem(giveItemID,potentialSourcesPointers[newSourceAddressIndex]);
                        }
                    };
                }
                else
                {
                    temp.OnChanged += (old, current) =>
                    {
                        int levelID = gameProc.ReadValue<int>(levelIDPointer);
                        if (current == 1 && sourceToLevelMapping[newSourceAddressIndex].Contains(levelID))
                        {
                            NewItem(giveItemID, potentialSourcesPointers[newSourceAddressIndex]);
                        }
                    };
                }
                temp.Enabled = true;
                randoSourceWatchers.Add(temp);
            }
        }

        private void UpdateImpossibleSources(int itemId)
        {
            impossibleSources.Clear();
            int j = 0;
            //Key items are played in order: Cat Sphere, Crest Fragments, Garden Key, Cinder Key, Monastery Key, (Hazel Badge, Soft Tissue, Dirty Shroom, Ivory Bug) 
            if (itemId == (int)Items.CatSphere) j = 0;
            else if (itemId == (int)Items.FragmentBowPow) j = 1;
            else if (itemId == (int)Items.GardenKey) j = 2;
            else if (itemId == (int)Items.CinderKey) j = 3;
            else if (itemId == (int)Items.MonasteryKey) j = 4;
            else if (itemId == (int)Items.HazelBadge) j = 5;
            else if (itemId == (int)Items.SoftTissue) j = 6;
            else if (itemId == (int)Items.DirtyShroom) j = 7;
            else if (itemId == (int)Items.SealedWind) j = 8;
            else if (itemId == (int)Items.IvoryBug) j = 9;
            for(int i = 0; i < requirementMatrix.GetLength(0); i++)
            {
                impossibleSources.AddRange(requirementLists[j]);
                if (requirementMatrix[i, j]) impossibleSources.AddRange(requirementLists[i]);
            }
        }

        private void UpdateBannedSources()
        {
            if (!settingsControl.VitalityFragmentsEnabled) bannedSources.AddRange(vitalityFragments);
            if (!settingsControl.IvoryBugsEnabled) bannedSources.AddRange(ivoryBugs);
            if (!settingsControl.HardModeEnabled) bannedSources.AddRange(bossItems);
            //Pocket incensory and Dirty Shroom don't work all the time when randomizing
            bannedSources.Add(34);
            bannedSources.Add(19);
            bannedSources.Add(24);
            //disabled shop items for now
            bannedSources.Add(8);
            bannedSources.Add(9);
            bannedSources.Add(10);
            bannedSources.Add(11);
            bannedSources.Add(12);
            bannedSources.Add(13);
            bannedSources.Add(14);
            bannedSources.Add(15);
            bannedSources.Add(16);
            bannedSources.Add(17);
            bannedSources.Add(18);
        }

        private void UpdatePossibleSources()
        {
            possibleSources.Clear();
            for (int i = 0;  i < RANDOMIZER_SOURCE_AMOUNT; i++)
            {
                if(!bannedSources.Contains(i) && !impossibleSources.Contains(i) && !usedSources.Contains(i))
                {
                    possibleSources.Add(i);
                }
            }
        }

        private void AlternatePossibleSources()
        {
            possibleSources.Clear();
            for (int i = 0; i < RANDOMIZER_SOURCE_AMOUNT; i++)
            {
                if (!bannedSources.Contains(i) && impossibleSources.Contains(i) && !usedSources.Contains(i))
                {
                    possibleSources.Add(i);
                }
            }
        }

        #region add/remove items
        /*
         * Adds a new item to the players inventory and removes the last item acquired
         * id = id of item to give out
         * addr = IntPtr to source of item given out
         */
        private bool NewItem(int id, IntPtr addr, int addCharges = 2)
        {
            bool res = true;
            SetupItemPtrs();
            RandomizerLabel.Text = "New item: " + Enum.GetName(typeof(Items), id);
            RemoveItem();
            Debug.WriteLine("Giving item id: " + id);
            
            int allocatedMemory = gameProc.ReadValue<int>(IntPtr.Subtract(inventoryItemsStartPointer, 0x10));
            Debug.WriteLine("Allocated memory: " + allocatedMemory);
            int totalItems = gameProc.ReadValue<int>(totalItemsPointer);
            Debug.WriteLine("Total items: " + totalItems);

            if (id == (int)Items.IvoryBug)
            {
                AddIvoryBug();
            }
            else if (id == (int)Items.VitalityFragment)
            {
                AddVitalityFragment();
            }
            else if (totalItems * 16 != allocatedMemory)
            {
                if ((int)Items.FragmentWarp >= id && id >= (int)Items.FragmentBowPow)
                {
                    AddCrestFragment(id);
                    if (id == 53)
                    {
                        hasWarp = true;
                    }
                }
                else if (id == (int)Items.Bellflower || id == (int)Items.Passiflora || id == (int)Items.TaintedMissive)
                {
                    AddChargeItem(id, addCharges);
                }
                else if (id == (int)Items.MonasteryKey || id == (int)Items.GardenKey || id == (int)Items.CinderKey)
                {
                    AddKey(id);
                }
                else if (id == (int)Items.FreshSpringLeaf)
                {
                    AddLeaf();
                }
                else
                {
                    AddItem(id);
                }
            }
            else
            {
                if (addr != IntPtr.Zero) gameProc.WriteValue<double>(addr, 0);
                res = false;
            }
            itemGiven = 3;
            if(res == true) Event("\nItem " + id + " given, total items: " + totalItems + "\n");
            else Event("\nUnable to give item " + id + ", total items: " + totalItems + "\n");
            return res;
        }
      
        private void AddLeaf()
        {
            gameProc.WriteValue<double>(potentialSourcesPointers[28], 1);
            hasFoundGreenLeaf = true;
            AddItem((int)Items.FreshSpringLeaf);
        }

        private void RemoveItem()
        {
            UpdateItemWatchers();

            if (taintedMissiveWatcher.Changed)
            {
                Event("Removing missive\n");
                RemoveChargeItem((int)Items.TaintedMissive, taintedMissiveWatcher.Current - taintedMissiveWatcher.Old);
            }
            else if (bellflowerWatcher.Changed)
            {
                Event("\nRemoving bellflower\n");
                RemoveChargeItem((int)Items.Bellflower, bellflowerWatcher.Current - bellflowerWatcher.Old);
            }
            else if (passifloraWatcher.Changed)
            {
                Event("\nRemoving passiflora\n");
                RemoveChargeItem((int)Items.Passiflora, passifloraWatcher.Current - passifloraWatcher.Old);
            }
            else if (ivoryBugWatcher.Changed)
            {
                Event("\nRemoving IB\n");
                RemoveIvoryBug();
            }
            else if (crestFragmentWatcher.Changed)
            {
                Event("\nRemoving Crest Frag\n");
                RemoveCrestFragment();
            }
            else if (vitalityFragmentWatcher.Changed)
            {
                Event("\nRemoving Vit Frag\n");
                RemoveVitalityFragment();
            }
            else if (levelIDWatcher.Current == 83)
            {
                Event("\nGreen leaf upgraded, do not remove stuff\n");
            }
            else
            {
                Event("\nRemoving last item in inventory\n");
                RemoveLastItem();
            }
        }

        private void AddChargeItem(int id, double charges)
        {
            double currentMaxCharges;
            IntPtr maxValuePointer, saveValuePtr;
            bool hasItem;
            int j;
            switch (id)
            {
                case (int)Items.Bellflower:
                    j = 0;
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    hasItem = hasChargeItem[j];
                    break;
                case (int)Items.Passiflora:
                    j = 1;
                    maxValuePointer = passifloraMaxValuePointer;
                    saveValuePtr = passifloraSaveValuePointer;
                    hasItem = hasChargeItem[j];
                    break;
                case (int)Items.TaintedMissive:
                    j = 2;
                    maxValuePointer = taintedMissiveMaxValuePointer;
                    saveValuePtr = taintedMissiveSaveValuePointer;
                    hasItem = hasChargeItem[j];
                    break;
                default:
                    return;
            }
            currentMaxCharges = gameProc.ReadValue<double>(maxValuePointer);
            gameProc.WriteValue<double>(maxValuePointer, charges + currentMaxCharges);
            gameProc.WriteValue<double>(saveValuePtr, charges + currentMaxCharges);
            if (currentMaxCharges == 0 && !hasItem)
            {
                hasChargeItem[j] = true;
                AddItem(id);
            }
        }

        private void RemoveChargeItem(int id, double charges)
        {
            double currentCharges;
            IntPtr maxValuePointer, saveValuePtr;
            int j = 0;
            switch (id)
            {
                case (int)Items.Bellflower:
                    j = 0;
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    break;
                case (int)Items.Passiflora:
                    j = 1;
                    maxValuePointer = passifloraMaxValuePointer;
                    saveValuePtr = passifloraSaveValuePointer;
                    break;
                case (int)Items.TaintedMissive:
                    j = 2;
                    maxValuePointer = taintedMissiveMaxValuePointer;
                    saveValuePtr = taintedMissiveSaveValuePointer;
                    break;
                default:
                    return;
            }
            hasChargeItem[j] = true;
            currentCharges = gameProc.ReadValue<double>(maxValuePointer);
            gameProc.WriteValue<double>(maxValuePointer, currentCharges - charges);
            gameProc.WriteValue<double>(saveValuePtr, currentCharges - charges);
        }

        private void AddItem(int id)
        {
            //To add an item: Increase total item counter by one
            //and set the inventory value for the next item slot to the id of the item given
            SetupItemPtrs();
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            if (id == (int)Items.CatSphere) hasCatSphere = true;
            gameProc.WriteValue<int>(totalItemsPointer, (int)totalItemAmount + 1);
            gameProc.WriteValue<double>(IntPtr.Add(inventoryItemsStartPointer, 0x10 * totalItemAmount), id);
        }

        private void RemoveLastItem()
        {
            //To remove last item, decrease total item counter by one
            SetupItemPtrs();
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            gameProc.WriteValue<int>(totalItemsPointer, (int)totalItemAmount - 1);
        }

        private void RemoveCrestFragment()
        {
            RemoveLastItem();
            CountCrestFragments();
        }

        private void AddCrestFragment(int id)
        {
            AddItem(id);
            CountCrestFragments();
        }

        private void RemoveVitalityFragment()
        {
            double[] healthChange = { 0, 2, 1, 1 };
            double difficulty = gameProc.ReadValue<double>(difficultyPointer);
            double fragments = gameProc.ReadValue<double>(vitalityFragmentCountPointer);
            double health = gameProc.ReadValue<double>(maxHealthPointer);
            gameProc.WriteValue<double>(vitalityFragmentCountPointer, fragments - 1);
            gameProc.WriteValue<double>(maxHealthPointer, health - healthChange[(int)difficulty-1]);
        }

        private void AddVitalityFragment()
        {
            double difficulty = gameProc.ReadValue<double>(difficultyPointer);
            double fragments = gameProc.ReadValue<double>(vitalityFragmentCountPointer);
            double health = gameProc.ReadValue<double>(maxHealthPointer);
            gameProc.WriteValue<double>(vitalityFragmentCountPointer, fragments + 1);
            gameProc.WriteValue<double>(maxHealthPointer, health + healthChange[(int)difficulty - 1]);
        }

        private void AddIvoryBug()
        {
            double bugs = gameProc.ReadValue<double>(ivoryBugCountPointer);
            gameProc.WriteValue<double>(ivoryBugCountPointer, bugs + 1);
        }

        private void RemoveIvoryBug()
        {
            double bugs = gameProc.ReadValue<double>(ivoryBugCountPointer);
            gameProc.WriteValue<double>(ivoryBugCountPointer, bugs - 1);
        }

        private void AddKey(int id)
        {
            if (id == (int)Items.GardenKey)
            {
                hasKey[0] = 1;
            }
            else if (id == (int)Items.CinderKey)
            {
                hasKey[1] = 1;
            }
            else
            {
                hasKey[2] = 1;
            }

            AddItem(id);
        }

        private void CountCrestFragments()
        {
            int totalItems = gameProc.ReadValue<int>(totalItemsPointer);
            int fragments = 0;
            for (int i = 0; i < totalItems; i++)
            {
                double itemId = gameProc.ReadValue<double>(IntPtr.Add(inventoryItemsStartPointer, 0x10 * i));
                if ((int)Items.FragmentWarp >= itemId && itemId >= (int)Items.FragmentBowPow)
                {
                    fragments++;
                }
            }
            Debug.WriteLine("Found a total of " + fragments + " crest fragments");
            fragments = Math.Max(Math.Min(fragments, 4), 0);
            Debug.WriteLine("Writing " + fragments + " crest fragments");
            gameProc.WriteValue<double>(crestFragmentCountPointer, fragments);
        }
        #endregion

        private void CheckRoom(int old, int current)
        {
            #region key logic
            int j;

            //Garden = 4,
            //Cinder = 5,
            //Monastery = 6
            for (int i = 0; i < 3; i++)
            {
                j = i + 4;//Id for keys is 4-6

                if (doorLocations[i].Contains(current))//If the player is in a room with doors that pertaint to a key
                {
                    unlocked = gameProc.ReadValue<double>(potentialSourcesPointers[j]);//Read value for key acquired

                    if (unlocked != hasKey[i])//If the state of "key acquired" is different than it should be invert value
                    {
                        Event("\nApplying door logic\n");
                        Event("Door " + ((unlocked == 1) ? "closed" : "opened") + "\n");
                        gameProc.WriteValue<double>(potentialSourcesPointers[j], 1-unlocked);//1-0=1; 1-1=0
                    }
                }
                else if (doorLocations[i].Contains(old) && !doorLocations[i].Contains(current))//If they just left the room revert state of flag
                {
                    gameProc.WriteValue<double>(potentialSourcesPointers[j], unlocked);
                }
            }
            #endregion

            #region shop logic
            if(shopLocations.Contains(current))// If player is in a shop room
            {
                SetShopItems(current);
            }
            else if (shopLocations.Contains(old) && !shopLocations.Contains(current))// If player just left a shop room
            {
                ResetShopItems(old);
            }
            #endregion

            #region LubellaAntiSoftlock
            bool lubellaDefeated = Convert.ToBoolean(gameProc.ReadValue<double>(lubella2Pointer));
            if (current == 149 && lubellaDefeated && !hasCatSphere && !hasWarp)
            {
                Event("\nApplying antisoftlock logic\n");
                switch (gameVersion)
                {
                    case 1.05:
                        playerXPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0253597C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x120);
                        playerYPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0253597C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x130);
                        break;
                    case 1.07:
                        playerXPointer = IntPtr.Add((IntPtr)new DeepPointer(0x025A2B3C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x120);
                        playerYPointer = IntPtr.Add((IntPtr)new DeepPointer(0x025A2B3C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x130);
                        break;
                }

                playerYWatcher = new MemoryWatcher<double>(playerYPointer);
                playerYWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                playerYWatcher.Enabled = true;
                playerYWatcher.OnChanged += (oldVal, currentVal) =>
                {
                    double playerXPos = gameProc.ReadValue<double>(playerXPointer);
                    Debug.WriteLine(playerXPos + "   " + currentVal);
                    if (currentVal > 195 && playerXPos > 120)
                    {
                        gameProc.WriteValue<double>(playerFormPointer, 1);
                    }
                    else
                    {
                        gameProc.WriteValue<double>(playerFormPointer, 0);
                    }
                };

            }
            else if (old == 149)
            {
                playerYWatcher.Enabled = false;
                Event("\nEnding antisoftlock logic\n");
            }
            #endregion

            #region Green Leaf logic
            //y/x pointers change each room, need to set it up again when entering the correct room
            if (current == 82)
            {
                switch (gameVersion)
                {
                    case 1.05:
                        playerXPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0253597C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x120);
                        playerYPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0253597C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x130);
                        break;
                    case 1.07:
                        playerXPointer = IntPtr.Add((IntPtr)new DeepPointer(0x025A2B3C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x120);
                        playerYPointer = IntPtr.Add((IntPtr)new DeepPointer(0x025A2B3C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x130);
                        break;
                }

                Event("\nApplying green leaf logic\n");
                playerYWatcher = new MemoryWatcher<double>(playerYPointer);
                playerYWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                playerYWatcher.Enabled = true;
                playerYWatcher.OnChanged += (oldVal, currentVal) =>
                {
                    double pickedUpLeaf = gameProc.ReadValue<double>(potentialSourcesPointers[28]);
                    double playerXPos = gameProc.ReadValue<double>(playerXPointer);
                    if (playerYWatcher.Current > 176 && playerXPos > 410)
                    {
                        //If the state of "has picked green leaf source" is different from "found green leaf" invert
                        gameProc.WriteValue<double>(potentialSourcesPointers[28], Convert.ToDouble(hasBathedLeaf));

                    }
                    else
                    {
                        gameProc.WriteValue<double>(potentialSourcesPointers[28], Convert.ToDouble(hasFoundGreenLeaf));
                    }
                };

            }
            else if (old == 82)
            {
                playerYWatcher.Enabled = false;
                Event("\nEnding green leaf logic\n");
            }
            #endregion
        }

        private void InShop(double current)
        {
            int room = gameProc.ReadValue<int>(levelIDPointer);// Get current room

            if (shopLocations.Contains(room))// If player is in a shop room
            {
                Event("\nEntered shop room\n");
                if (current == 1)// If player is in a conversation with an npc (non shop npcs get handled by another function)
                {
                    SetShopItems(room);
                    AddPlaceholders(room);
                }
                else
                {
                    RemovePlaceholders(room);
                }
            }
        }

        private void InvOpen(double current)
        {
            int room = gameProc.ReadValue<int>(levelIDPointer);// Get current room
            int inShop = (int)gameProc.ReadValue<double>(convOpenPointer);// Get if the player has a shop open

            if (shopLocations.Contains(room) && inShop == 1)// If player is in a shop room and is shopping
            {
                Event("\nOpened inventory while in shop\n");
                if (current == 1)// If inventory is open remove all placeholder items for shops
                {
                    RemovePlaceholders(room);
                }
                else// If its closed place items back
                {
                    AddPlaceholders(room);
                }
            }
        }

        private void ItemBought()
        {
            int room = gameProc.ReadValue<int>(levelIDPointer);// Get current room

            if (shopLocations.Contains(room))// If player is in a shop room
            {
                int currentShopLocation = shopLocations.IndexOf(room);
                List<int> shopItemsAux = shopItems[currentShopLocation];// Get list storing what items correspond to the ones in the shop
                int idPos = 0, itemAux;

                int invSize = gameProc.ReadValue<int>(totalItemsPointer);// Get inventory size
                Event("\nCurrent items " + invSize + "\n");
                int placeholderId = (int)gameProc.ReadValue<double>(IntPtr.Add(inventoryItemsStartPointer, 0x10 * (invSize - 1)));// id of last aquired item
                Event("Last acquired item: " + placeholderId + "\n");
                // Index of last aquired item
                if (placeholderId == 22) idPos = 0;
                if (placeholderId == 29) idPos = 1;
                if (placeholderId == 45) idPos = 2;

                Event("Item bought: " + Enum.GetName(typeof(Items), shopItems[currentShopLocation][idPos]) + "\n");
                itemAux = originalShopItems[currentShopLocation][idPos];// Get what is the id that would have been bought
                for (int i = 0; i < originalShopItems.Count(); i++)// Update value of hasBoughtItem in all shops that "sell" itemAux
                {
                    for (int j = 0; j < originalShopItems[i].Count(); j++)
                    {
                        if (originalShopItems[i][j] == itemAux)
                        {
                            Event("Updating shop " + Enum.GetName(typeof(shops), i) + ": position " + j + "\n");
                            hasBoughtItem[i][j] = true;
                        }
                    }
                }

                RemovePlaceholders(room);// remove all placeholders (avoid weird situations)
                AddItem((int)placeholderId);
                NewItem(shopItemsAux[idPos],IntPtr.Zero);
                AddPlaceholders(room);// re-add placeholders
                Event("\nAt shop " + Enum.GetName(typeof(shops), currentShopLocation) + "\n");
                foreach (var item in shopItems[currentShopLocation])
                {
                    Event("Items sold: " + Enum.GetName(typeof(Items), item) +
                        (hasBoughtItem[currentShopLocation][shopItems[currentShopLocation].IndexOf(item)] ? " was bought" : " wasn't bought") + "\n");
                }
            }
        }

        private void AddPlaceholders(int room)
        {
            List<bool> aux = hasBoughtItem[shopLocations.IndexOf(room)];// Get list storing what items where bought in the current shop

            for (int i = 0; i < aux.Count(); i++)
            {
                if (aux[i] == true)// If item was bought add the placeholder
                {
                    if (i == 0)
                    {
                        AddItem(22);
                        Event("Adding placholder id 22\n");
                    }
                    if (i == 1)
                    {
                        AddItem(29);
                        Event("Adding placholder id 29\n");
                    }
                    if (i == 2)
                    {
                        AddItem(45);
                        Event("Adding placholder id 45\n");
                    }
                }
            }
        }

        private void RemovePlaceholders(int room)
        {
            List<bool> aux = hasBoughtItem[shopLocations.IndexOf(room)];// Get list storing what items where bought in the current shop

            foreach (var item in aux)
            {
                if (item == true)// If item was bought remove one placeholder
                {
                    Event("Removing one placeholder item\n");
                    RemoveLastItem();
                }
            }
        }

        private void SaveShopItem(int origin, int swapped)
        {
            int itemPos, listPos;
            foreach (var list in originalShopItems)
            {
                if (list.Contains(origin))
                {
                    listPos = originalShopItems.IndexOf(list);
                    itemPos = list.IndexOf(origin);
                    shopItems[listPos][itemPos] = swapped;
                }
            }
        }

        private void SetShopItems(int room)
        {
            IntPtr pointer, itemPointer;
            int currentShopLocation = shopLocations.IndexOf(room);
            List<int> list = shopOffsets[currentShopLocation];
            List<int> shopItemsAux = shopItems[currentShopLocation];// Get list of items of the current shop
            int id = 22, pValue;
            byte[] bytes;

            Event("\nSetting items for " + Enum.GetName(typeof(shops), currentShopLocation) + " shop\n");
            foreach (var item in shopItemsAux)
                Event("Items sold: " + Enum.GetName(typeof(Items), item) +
                (hasBoughtItem[currentShopLocation][shopItemsAux.IndexOf(item)] ? ": bought" : ": not bought") + "\n");
            for (int i = 0; i < list.Count(); i++)
            {
                if (i == 0) id = 22;
                if (i == 1) id = 29;
                if (i == 2) id = 45;

                pointer = IntPtr.Add((IntPtr)new DeepPointer(shopPointer).Deref<Int32>(gameProc), 0x10 * list[i]);// Get pointer to shop item
                gameProc.WriteValue<double>(pointer, id);// Set shop placeholder to id

                // Vitality Fragment doesnt have a name or effect so we have to write to memory (trying to write as few bytes as possible)
                if (shopItemsAux[i] == 54)
                {
                    double difficulty = gameProc.ReadValue<double>(difficultyPointer);

                    bytes = Encoding.ASCII.GetBytes("Vit. Frag.");
                    pointer = (IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * id) + 0x10), 0x0 }).Deref<Int32>(gameProc);// Get pointer to item name
                    gameProc.WriteBytes(pointer, bytes);// Set name of placeholder to the one that will get added later
                    gameProc.WriteValue<int>(pointer + bytes.Length, 0x0);// Add end of string

                    bytes = Encoding.ASCII.GetBytes("+" + healthChange[(int)difficulty - 1] + " Max Hp");
                    pointer = (IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * id) + 0x20), 0x0 }).Deref<Int32>(gameProc);// Get pointer to item effect
                    gameProc.WriteBytes(pointer, bytes);// Set effect of placeholder to the one that will get added later
                    gameProc.WriteValue<int>(pointer + bytes.Length, 0x0);// Add end of string
                }
                else
                {
                    pointer = IntPtr.Add((IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * id) + 0x10) }).Deref<Int32>(gameProc), 0x0);// Get pointer of placeholder
                    pointerValues[i].Add(gameProc.ReadValue<int>(pointer));// Save value of original pointer
                    itemPointer = IntPtr.Add((IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * shopItemsAux[i]) + 0x10) }).Deref<Int32>(gameProc), 0x0);// Get pointer of real item
                    pValue = gameProc.ReadValue<int>(itemPointer);
                    gameProc.WriteValue(pointer, pValue);// Make placeholder pointer point to the real item's name

                    pointer = IntPtr.Add((IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * id) + 0x20) }).Deref<Int32>(gameProc), 0x0);// Get pointer of placeholder
                    pointerValues[i].Add(gameProc.ReadValue<int>(pointer));// Save value of original pointer
                    itemPointer = IntPtr.Add((IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * shopItemsAux[i]) + 0x20) }).Deref<Int32>(gameProc), 0x0);// Get pointer of real item
                    pValue = gameProc.ReadValue<int>(itemPointer);
                    gameProc.WriteValue(pointer, pValue);// Make placeholder pointer point to the real item's effect
                }
            }
        }

        private void ResetShopItems(int room)
        {
            IntPtr pointer;
            int currentShopLocation = shopLocations.IndexOf(room);
            List<int> list = shopOffsets[currentShopLocation];
            List<int> shopItemsAux = originalShopItems[currentShopLocation];// Get list of original items for the current shop
            List<int> shopItemsAux2 = shopItems[currentShopLocation];// Get list of items of the current shop
            int id, idAux = 22;
            byte[] bytes;

            Event("\nResetting items for " + Enum.GetName(typeof(shops), currentShopLocation) + " shop\n");
            for (int i = 0; i < list.Count(); i++)
            {
                if (i == 0) idAux = 22;
                if (i == 1) idAux = 29;
                if (i == 2) idAux = 45;
                id = shopItemsAux[i];

                pointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x10 * list[i]);// Get pointer to shop item
                gameProc.WriteValue<double>(pointer, (double)sourceIdMapping[id]);// Set shop item id to original one

                // check pointerValues[i] so livesplit doesnt freeze on onReset()
                if (shopItemsAux2[i] == 54 && pointerValues[i].Count == 0)
                {
                    bytes = Encoding.ASCII.GetBytes(itemNames[i]);
                    pointer = (IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * idAux) + 0x10), 0x0 }).Deref<Int32>(gameProc);// Get pointer to item name
                    gameProc.WriteBytes(pointer, bytes);// Restore name
                    gameProc.WriteValue<int>(pointer + bytes.Length, 0x0);// Add end of string

                    bytes = Encoding.ASCII.GetBytes(itemEffects[i]);
                    pointer = (IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * idAux) + 0x20), 0x0 }).Deref<Int32>(gameProc);// Get pointer to item effect
                    gameProc.WriteBytes(pointer, bytes);// Restore effect
                    gameProc.WriteValue<int>(pointer + bytes.Length, 0x0);// Add end of string
                }
                else if (pointerValues[i].Count > 0)
                {
                    pointer = IntPtr.Add((IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * idAux) + 0x10) }).Deref<Int32>(gameProc), 0x0);// Get pointer of placeholder
                    gameProc.WriteValue(pointer, pointerValues[i][0]);// Restore placeholder pointer value

                    pointer = IntPtr.Add((IntPtr)new DeepPointer(itemInfoPointer, new int[] { ((0x60 * idAux) + 0x20) }).Deref<Int32>(gameProc), 0x0);// Get pointer of placeholder
                    gameProc.WriteValue(pointer, pointerValues[i][1]);// Restore placeholder pointer value

                    pointerValues[i].Clear();// Remove placeholder pointer values
                }
            }
        }

        private void Event(string text)
        {
            if(text.StartsWith("\n"))
            {
                Events.Add("\n" + DateTime.Now.ToString("T") + ":" + text.Replace("\n", "") + "\n");
            }
            else Events.Add(DateTime.Now.ToString("T") + ":" + text);
            Debug.WriteLine(text.Replace("\n", ""));
        }

        private void logResults()
        {
            Debug.WriteLine("Creating event log file");
            var path = "Components/MomodoraRandomizer.log";
            File.WriteAllText(path, "Using seed: " + seed + "\n");
            if (File.Exists(path))
            {
                File.AppendAllText(path, "Running on version " + gameVersion + "\n");
                Debug.WriteLine("Writing run events");
                foreach (var Event in Events)
                {
                    File.AppendAllText(path, Event);
                }
            }
            else Debug.WriteLine("Failed to create event log file");
        }

        public string ComponentName => "Momodora Randomizer";

        public float HorizontalWidth {get; set;}

        public float MinimumHeight => 10;

        public float VerticalHeight { get; set; }

        public float MinimumWidth => 200;

        public float PaddingTop => 1;

        public float PaddingBottom => 1;

        public float PaddingLeft => 1;

        public float PaddingRight => 1;

        public IDictionary<string, Action> ContextMenuControls => null;

        public void Dispose()
        {
        }

        public void DrawHorizontal(System.Drawing.Graphics g, LiveSplitState state, float height, System.Drawing.Region clipRegion)
        {
            throw new NotImplementedException();
        }

        public void DrawVertical(System.Drawing.Graphics g, LiveSplitState state, float width, System.Drawing.Region clipRegion)
        {
            var textHeight = g.MeasureString("A", state.LayoutSettings.TextFont).Height;
            VerticalHeight = textHeight * 1.5f;

            prepareDraw(state);
            RandomizerLabel.SetActualWidth(g);
            RandomizerLabel.Width = RandomizerLabel.ActualWidth;
            RandomizerLabel.Height = VerticalHeight;
            RandomizerLabel.X = width-PaddingRight-RandomizerLabel.Width;
            RandomizerLabel.Y = 3f;

            DrawBackground(g, width, VerticalHeight);

            RandomizerLabel.Draw(g);
        }

        public void prepareDraw(LiveSplitState state)
        {
            RandomizerLabel.Font = settingsControl.OverrideTextFont ? settingsControl.TextFont : state.LayoutSettings.TextFont;
            RandomizerLabel.ForeColor = settingsControl.OverrideTextColor ? settingsControl.TextColor : state.LayoutSettings.TextColor;
            RandomizerLabel.OutlineColor = settingsControl.OverrideTextColor ? settingsControl.OutlineColor : state.LayoutSettings.TextOutlineColor;
            RandomizerLabel.ShadowColor = settingsControl.OverrideTextColor ? settingsControl.ShadowColor : state.LayoutSettings.ShadowsColor;

            RandomizerLabel.VerticalAlignment = StringAlignment.Center;
            RandomizerLabel.HorizontalAlignment = StringAlignment.Center;
        }

        private void DrawBackground(Graphics g, float width, float height)
        {
            if (settingsControl.BackgroundColor.A > 0
                || settingsControl.BackgroundGradient != GradientType.Plain
                && settingsControl.BackgroundColor2.A > 0)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            settingsControl.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            settingsControl.BackgroundColor,
                            settingsControl.BackgroundGradient == GradientType.Plain
                            ? settingsControl.BackgroundColor
                            : settingsControl.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return settingsControl.GetSettings(document);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            return settingsControl;
        }

        public void SetSettings(XmlNode settings)
        {
            settingsControl.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (VerifyProcessRunning() && randomizerRunning)
            {
                inGameWatcher.Update(gameProc);
                if (inGame) {
                    UpdateSpecialWatchers();
                    foreach (var watcher in randoSourceWatchers)
                    {
                        watcher.Update(gameProc);
                    }
                    if (itemGiven > 0)
                    {
                        UpdateItemWatchers();
                        itemGiven--;
                    }
                }

                if (invalidator != null)
                {
                    invalidator.Invalidate(0, 0, width, height);
                }
            }
        }

        private void UpdateSpecialWatchers()
        {
            levelIDWatcher.Update(gameProc);
            saveWatcher.Update(gameProc);
            deathWatcher.Update(gameProc);
            munnyWatcher.Update(gameProc);
            invOpenWatcher.Update(gameProc);
            convOpenWatcher.Update(gameProc);
            playerYWatcher.Update(gameProc);
            currentHealthWatcher.Update(gameProc);
        }

        private void UpdateItemWatchers()
        {

            taintedMissiveWatcher.Update(gameProc);
            passifloraWatcher.Update(gameProc);
            bellflowerWatcher.Update(gameProc);
            ivoryBugWatcher.Update(gameProc);
            crestFragmentWatcher.Update(gameProc);
            vitalityFragmentWatcher.Update(gameProc);
        }

        private void SetupVersionDifferences()
        {
            SetupIntPtrs();
            //Karst City, Forlorn Monsatery, Subterranean Grave, Whiteleaf Memorial Park, Cinder Chambers 1, Cinder Chambers 2, Royal Pinacotheca
            switch (gameVersion)
            {
                case 1.05:
                    shopOffsets = new List<List<int>>
                    {
                        new List<int> { 207, 203, 213 },
                        new List<int> { 203 },
                        new List<int> { 221, 199 },
                        new List<int> { 5 },
                        new List<int> { 8, 213 },
                        new List<int> { 235, 213, 205 },
                        new List<int> { 224, 234, 11 }
                    };
                    healthChange = new double[] { 0, 2, 1, 1 };
                    break;
                case 1.07:
                    shopOffsets = new List<List<int>>
                    {
                        new List<int> { 208, 204, 214 },
                        new List<int> { 204 },
                        new List<int> { 222, 200 },
                        new List<int> { 5 },
                        new List<int> { 8, 214 },
                        new List<int> { 236, 214, 206 },
                        new List<int> { 225, 235, 11 }
                    };
                    healthChange = new double[] { 2, 2, 1, 1 };
                    break;
            }
        }
        
        private void SetupIntPtrs()
        {
            switch (gameProc.MainModule.ModuleMemorySize)
            {
                case 39690240:
                    //version 1.05b
                    Debug.WriteLine("Version 1.05b detected");
                    Debug.WriteLine("Setting up pointers");
                    gameVersion = 1.05;
                    #region setting up IntPtrs
                    potentialSourcesPointers = new IntPtr[RANDOMIZER_SOURCE_AMOUNT];
                    potentialSourcesPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xF0);
                    potentialSourcesPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x100);
                    potentialSourcesPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x410);
                    potentialSourcesPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x430);
                    potentialSourcesPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x700);
                    potentialSourcesPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9B0);
                    potentialSourcesPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x260);
                    potentialSourcesPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x420);
                    potentialSourcesPointers[8] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcf0);
                    potentialSourcesPointers[9] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcb0);
                    potentialSourcesPointers[10] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd50);
                    potentialSourcesPointers[11] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xdd0);
                    potentialSourcesPointers[12] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc70);
                    potentialSourcesPointers[13] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x50);
                    potentialSourcesPointers[14] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x80);
                    potentialSourcesPointers[15] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xeb0);
                    potentialSourcesPointers[16] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcd0);
                    potentialSourcesPointers[17] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xe00);
                    potentialSourcesPointers[18] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xea0);
                    potentialSourcesPointers[19] = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xb0);
                    potentialSourcesPointers[20] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA00);
                    potentialSourcesPointers[21] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA90);
                    potentialSourcesPointers[22] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6D0);
                    potentialSourcesPointers[23] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x690);
                    potentialSourcesPointers[24] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x480);
                    potentialSourcesPointers[25] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x720);
                    potentialSourcesPointers[26] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7E0);
                    potentialSourcesPointers[27] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x570);
                    potentialSourcesPointers[28] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x600);
                    potentialSourcesPointers[29] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);
                    potentialSourcesPointers[30] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x500);
                    potentialSourcesPointers[31] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x140);
                    potentialSourcesPointers[32] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x450);
                    potentialSourcesPointers[33] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7D0);
                    potentialSourcesPointers[34] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9F0);
                    potentialSourcesPointers[35] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x580);
                    potentialSourcesPointers[36] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x270);
                    potentialSourcesPointers[37] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6B0);
                    potentialSourcesPointers[38] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x670);
                    potentialSourcesPointers[39] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x150);
                    potentialSourcesPointers[40] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x190);
                    potentialSourcesPointers[41] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x170);
                    potentialSourcesPointers[42] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1B0);
                    potentialSourcesPointers[43] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x180);
                    potentialSourcesPointers[44] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8C0);
                    potentialSourcesPointers[45] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8D0);
                    potentialSourcesPointers[46] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x160);
                    potentialSourcesPointers[47] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x940);
                    potentialSourcesPointers[48] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1C0);
                    potentialSourcesPointers[49] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1A0);
                    potentialSourcesPointers[50] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8E0);
                    potentialSourcesPointers[51] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8F0);
                    potentialSourcesPointers[52] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x900);
                    potentialSourcesPointers[53] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x930);
                    potentialSourcesPointers[54] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x920);
                    potentialSourcesPointers[55] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x910);
                    potentialSourcesPointers[56] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x280);
                    potentialSourcesPointers[57] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x290);
                    potentialSourcesPointers[58] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3A0);
                    potentialSourcesPointers[59] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2A0);
                    potentialSourcesPointers[60] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x300);
                    potentialSourcesPointers[61] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x310);
                    potentialSourcesPointers[62] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2F0);
                    potentialSourcesPointers[63] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x360);
                    potentialSourcesPointers[64] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x340);
                    potentialSourcesPointers[65] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2E0);
                    potentialSourcesPointers[66] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3B0);
                    potentialSourcesPointers[67] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2C0);
                    potentialSourcesPointers[68] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2D0);
                    potentialSourcesPointers[69] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2B0);
                    potentialSourcesPointers[70] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x390);
                    potentialSourcesPointers[71] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x320);
                    potentialSourcesPointers[72] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x370);
                    potentialSourcesPointers[73] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x380);
                    potentialSourcesPointers[74] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x350);
                    potentialSourcesPointers[75] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x330);
                    potentialSourcesPointers[76] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5B0);
                    potentialSourcesPointers[77] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5D0);
                    potentialSourcesPointers[78] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5A0);
                    potentialSourcesPointers[79] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5C0);
                    potentialSourcesPointers[80] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7A0);
                    potentialSourcesPointers[81] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7B0);
                    potentialSourcesPointers[82] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7C0);

                    vitalityFragmentCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xAE0);
                    crestFragmentCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5f0);
                    ivoryBugCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3C0);

                    bellflowerMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x1c0);
                    bellflowerSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3f0);
                    taintedMissiveMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x6a0);
                    taintedMissiveSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x400);
                    passifloraMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x580);
                    passifloraSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9d0);
                    difficultyPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x630);
                    maxHealthPointer = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xa0);
                    currentHealthPointer = (IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc);

                    totalItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4 }).Deref<Int32>(gameProc), 0x4);

                    activeItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc30);
                    passiveItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                    keyItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1100);
                    inventoryItemsStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4, 0xC }).Deref<int>(gameProc);
                    inventoryItemsChargeStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a8, 0xC }).Deref<int>(gameProc);
                    levelIDPointer = IntPtr.Add(gameProc.MainModule.BaseAddress, 0x230F1A0);
                    deathAmountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<int>(gameProc), 0x540);
                    inGamePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0x780);
                    saveAmountPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<int>(gameProc);
                    
                    munnyPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0x550);
                    invOpenPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0xAC0);
                    convOpenPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0x660);
                    shopPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304CE8).Deref<Int32>(gameProc), 0x4);
                    itemInfoPointer = (IntPtr)new DeepPointer(0x230B134, new int[] { 0x14 }).Deref<Int32>(gameProc);

                    warpStartPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<int>(gameProc), 0xB40);
                    playerFormPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4}).Deref<int>(gameProc), 0x760);
                    lubella2Pointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x560);
                    playerXPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0253597C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x120);
                    playerYPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0253597C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x130);

                    #endregion
                    RandomizerLabel.Text = "1.05b randomizer ready to go!";
                    break;
                case 40222720:
                    //version 1.07
                    Debug.WriteLine("Version 1.07 detected");
                    Debug.WriteLine("Setting up pointers");
                    gameVersion = 1.07;
                    #region setting up IntPtrs
                    potentialSourcesPointers = new IntPtr[RANDOMIZER_SOURCE_AMOUNT];
                    potentialSourcesPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xF0);
                    potentialSourcesPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x100);
                    potentialSourcesPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x410);
                    potentialSourcesPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x430);
                    potentialSourcesPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x700);
                    potentialSourcesPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9B0);
                    potentialSourcesPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x260);
                    potentialSourcesPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x02379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x420);
                    potentialSourcesPointers[8] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd00);
                    potentialSourcesPointers[9] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcc0);
                    potentialSourcesPointers[10] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd60);
                    potentialSourcesPointers[11] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xde0);
                    potentialSourcesPointers[12] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc80);
                    potentialSourcesPointers[13] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x60);
                    potentialSourcesPointers[14] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x70);
                    potentialSourcesPointers[15] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xec0);
                    potentialSourcesPointers[16] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xce0);
                    potentialSourcesPointers[17] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xe10);
                    potentialSourcesPointers[18] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xeb0);
                    potentialSourcesPointers[19] = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xb0);
                    potentialSourcesPointers[20] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA00);
                    potentialSourcesPointers[21] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA90);
                    potentialSourcesPointers[22] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6D0);
                    potentialSourcesPointers[23] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x690);
                    potentialSourcesPointers[24] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x480);
                    potentialSourcesPointers[25] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x720);
                    potentialSourcesPointers[26] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7E0);
                    potentialSourcesPointers[27] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x570);
                    potentialSourcesPointers[28] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x600);
                    potentialSourcesPointers[29] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);
                    potentialSourcesPointers[30] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x500);
                    potentialSourcesPointers[31] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x140);
                    potentialSourcesPointers[32] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x450);
                    potentialSourcesPointers[33] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7D0);
                    potentialSourcesPointers[34] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9F0);
                    potentialSourcesPointers[35] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x580);
                    potentialSourcesPointers[36] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x270);
                    potentialSourcesPointers[37] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6B0);
                    potentialSourcesPointers[38] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x670);
                    potentialSourcesPointers[39] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x150);
                    potentialSourcesPointers[40] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x190);
                    potentialSourcesPointers[41] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x170);
                    potentialSourcesPointers[42] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1B0);
                    potentialSourcesPointers[43] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x180);
                    potentialSourcesPointers[44] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8C0);
                    potentialSourcesPointers[45] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8D0);
                    potentialSourcesPointers[46] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x160);
                    potentialSourcesPointers[47] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x940);
                    potentialSourcesPointers[48] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1C0);
                    potentialSourcesPointers[49] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1A0);
                    potentialSourcesPointers[50] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8E0);
                    potentialSourcesPointers[51] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8F0);
                    potentialSourcesPointers[52] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x900);
                    potentialSourcesPointers[53] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x930);
                    potentialSourcesPointers[54] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x920);
                    potentialSourcesPointers[55] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x910);
                    potentialSourcesPointers[56] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x280);
                    potentialSourcesPointers[57] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x290);
                    potentialSourcesPointers[58] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3A0);
                    potentialSourcesPointers[59] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2A0);
                    potentialSourcesPointers[60] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x300);
                    potentialSourcesPointers[61] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x310);
                    potentialSourcesPointers[62] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2F0);
                    potentialSourcesPointers[63] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x360);
                    potentialSourcesPointers[64] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x340);
                    potentialSourcesPointers[65] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2E0);
                    potentialSourcesPointers[66] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3B0);
                    potentialSourcesPointers[67] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2C0);
                    potentialSourcesPointers[68] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2D0);
                    potentialSourcesPointers[69] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2B0);
                    potentialSourcesPointers[70] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x390);
                    potentialSourcesPointers[71] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x320);
                    potentialSourcesPointers[72] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x370);
                    potentialSourcesPointers[73] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x380);
                    potentialSourcesPointers[74] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x350);
                    potentialSourcesPointers[75] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x330);
                    potentialSourcesPointers[76] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5B0);
                    potentialSourcesPointers[77] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5D0);
                    potentialSourcesPointers[78] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5A0);
                    potentialSourcesPointers[79] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5C0);
                    potentialSourcesPointers[80] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7A0);
                    potentialSourcesPointers[81] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7B0);
                    potentialSourcesPointers[82] = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7C0);

                    vitalityFragmentCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xAE0);
                    crestFragmentCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5f0);
                    ivoryBugCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3C0);

                    bellflowerMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x23782F4, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x1c0);
                    bellflowerSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3f0);
                    taintedMissiveMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x23782F4, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x6a0);
                    taintedMissiveSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x400);
                    passifloraMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x23782F4, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x580);
                    passifloraSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9d0);
                    difficultyPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x630);
                    maxHealthPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xa0);
                    currentHealthPointer = (IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc);

                    totalItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x023782DC, new int[] { 0x1ac }).Deref<Int32>(gameProc), 0x4);

                    activeItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                    passiveItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc50);
                    keyItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1110);
                    inventoryItemsStartPointer = (IntPtr)new DeepPointer(0x23782DC, new int[] { 0x1ac, 0xC }).Deref<int>(gameProc);
                    inventoryItemsChargeStartPointer = (IntPtr)new DeepPointer(0x23782DC, new int[] { 0x1b0, 0xC }).Deref<int>(gameProc);
                    levelIDPointer = IntPtr.Add(gameProc.MainModule.BaseAddress, 0x237C360);
                    deathAmountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<int>(gameProc), 0x530);
                    inGamePointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0x790);
                    saveAmountPointer = (IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<int>(gameProc);
                    
                    munnyPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0x540);
                    invOpenPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0xAD0);
                    convOpenPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4 }).Deref<int>(gameProc), 0x670);
                    shopPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8).Deref<Int32>(gameProc), 0x4);
                    itemInfoPointer = (IntPtr)new DeepPointer(0x23782F4, new int[] { 0x14 }).Deref<Int32>(gameProc);

                    warpStartPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<int>(gameProc), 0xB40);
                    playerFormPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<int>(gameProc), 0x760);
                    lubella2Pointer = IntPtr.Add((IntPtr)new DeepPointer(0x2379600, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x560);
                    playerXPointer = IntPtr.Add((IntPtr)new DeepPointer(0x025A2B3C, new int[] { 0xC, 0xBC, 0x8, 0x4}).Deref<int>(gameProc), 0x120);
                    playerYPointer = IntPtr.Add((IntPtr)new DeepPointer(0x025A2B3C, new int[] { 0xC, 0xBC, 0x8, 0x4 }).Deref<int>(gameProc), 0x130);

                    #endregion
                    RandomizerLabel.Text = "1.07 randomizer ready to go!";
                    break;
                default:
                    RandomizerLabel.Text = "Unsupported game version for randomizer";
                    break;
            }
        }

        private void SetupItemPtrs()
        {
            switch (gameVersion)
            {
                case 1.05:
                    totalItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4 }).Deref<Int32>(gameProc), 0x4);
                    activeItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc30);
                    passiveItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                    keyItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1100);
                    inventoryItemsStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4, 0xC }).Deref<int>(gameProc);
                    inventoryItemsChargeStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a8, 0xC }).Deref<int>(gameProc);
                    break;
                case 1.07:
                    totalItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x023782DC, new int[] { 0x1ac }).Deref<Int32>(gameProc), 0x4);
                    activeItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                    passiveItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc50);
                    keyItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2371EA8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1110);
                    inventoryItemsStartPointer = (IntPtr)new DeepPointer(0x23782DC, new int[] { 0x1ac, 0xC }).Deref<int>(gameProc);
                    inventoryItemsChargeStartPointer = (IntPtr)new DeepPointer(0x23782DC, new int[] { 0x1b0, 0xC }).Deref<int>(gameProc);
                    break;
            }
        }

        private bool VerifyProcessRunning()
        {
            if (gameProc != null && !gameProc.HasExited)
            {
                return true;
            }
            Process[] game = Process.GetProcessesByName("MomodoraRUtM");
            if (game.Length > 0)
            {
                gameProc = game[0];
                return true;
            }
            return false;
        }
    }
}
