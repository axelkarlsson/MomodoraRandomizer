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
            [25] = (int)Items.Bellflower,
            [26] = (int)Items.SoftTissue,
            [27] = (int)Items.FreshSpringLeaf,
            [28] = (int)Items.BlessingCharm,
            [29] = (int)Items.RottenBellflower,
            [30] = (int)Items.EdeaPearl,
            [31] = (int)Items.BackmanPatch,
            [32] = (int)Items.SparseThread,
            [33] = (int)Items.PocketIncensory,
            [34] = (int)Items.TornBranch,
            [35] = (int)Items.TaintedMissive,
            [36] = (int)Items.BloodstainedTissue,
            [37] = (int)Items.HeavyArrows,
        };
        private Dictionary<int, int[]> sourceToLevelMapping = new Dictionary<int, int[]>
        {
            [0] = new int[] { 25 },
            [1] = new int[] { 37 },
            [2] = new int[] { 70 },
            [3] = new int[] { 90 },
            [4] = new int[] { 134},
            [5] = new int[] { 191},
            [6] = new int[] { 113},
            [7] = new int[] { 117},
            [8] = new int[] { 63 },
            [9] = new int[] { 63,111 },
            [10] = new int[] { 63,181,187 },
            [11] = new int[] { 127},
            [12] = new int[] {127 },
            [13] = new int[] { 160},
            [14] = new int[] { 181},
            [15] = new int[] { 187},
            [16] = new int[] { 187},
            [17] = new int[] { 199},
            [18] = new int[] { 199},
            [19] = new int[] { 199},
            [20] = new int[] { 204},
            [21] = new int[] { 220},
            [22] = new int[] { 269},
            [23] = new int[] { 261},
            [24] = new int[] { 126},
            [25] = new int[] { 149},
            [26] = new int[] { 162},
            [27] = new int[] { 103},
            [28] = new int[] { 83},
            [29] = new int[] { 52},
            [30] = new int[] { 46},
            [31] = new int[] { 53},
            [32] = new int[] { 73},
            [33] = new int[] { 141},
            [34] = new int[] { 193},
            [35] = new int[] { 153},
            [36] = new int[] { 104},
            [37] = new int[] { 97},
            [38] = new int[] { 212},
            [39] = new int[] { 39},
            [40] = new int[] { 47},
            [41] = new int[] { 67},
            [42] = new int[] { 81},
            [43] = new int[] { 144},
            [44] = new int[] { 168},
            [45] = new int[] { 191},
            [46] = new int[] { 108},
            [47] = new int[] { 35},
            [48] = new int[] { 58},
            [49] = new int[] { 185},
            [50] = new int[] { 199},
            [51] = new int[] { 205},
            [52] = new int[] { 209},
            [53] = new int[] { 270},
            [54] = new int[] { 246},
            [55] = new int[] { 264},
            [56] = new int[] { 41},
            [57] = new int[] { 50},
            [58] = new int[] { 62},
            [59] = new int[] { 78},
            [60] = new int[] { 170},
            [61] = new int[] { 191},
            [62] = new int[] { 183},
            [63] = new int[] { 40},
            [64] = new int[] { 56},
            [65] = new int[] { 123},
            [66] = new int[] { 152},
            [67] = new int[] { 155},
            [68] = new int[] { 156},
            [69] = new int[] { 102},
            [70] = new int[] { 103},
            [71] = new int[] { 211},
            [72] = new int[] { 249},
            [73] = new int[] { 255},
            [74] = new int[] { 214},
            [75] = new int[] { 213},
            [76] = new int[] { 142},
            [77] = new int[] { 195},
            [78] = new int[] { 105},
            [79] = new int[] { 60},
            [80] = new int[] { 163},
            [81] = new int[] { 163},
            [82] = new int[] { 163},
          };

        const int RANDOMIZER_SOURCE_AMOUNT = 83;

        private SimpleLabel RandomizerLabel;
        private Process gameProc = null;
        private Random randomGenerator = null;
        private int seed = 0;
        private MomodoraRandomizerSettings settingsControl;

        private MemoryWatcherList randoSourceWatchers;
        private List<int> bannedSources;
        private List<int> usedSources;
        private List<int> possibleSources;
        private List<int> impossibleSources;
        private List<List<int>> requirementLists;
        private List<int> requiresCatSphere;
        private List<int> requiresCrestFragments;
        private List<int> requiresGardenKey;
        private List<int> requiresCinderKey;
        private List<int> requiresMonasteryKey;
        private List<int> requiresHazelBadge;
        private List<int> requiresDirtyShroom;
        private List<int> requiresSoftTissue;
        private List<int> requiresIvoryBugs;

        //                                         Crest, Garden, Cinder, Mona, Haze, Soft, Dirty, Bugs
        private bool[,] requirementMatrix = new bool[8,8];
        private bool[] catRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] crestRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] gardenRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] cinderRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] monasteryRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] hazelRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] softTissueRequires = new bool[8] { false, false, false, false, false, false, false, false };
        private bool[] dirtyShroomRequires = new bool[8] { false, false, false, false, false, false, false, false };

        private List<int> vitalityFragments;
        private List<int> ivoryBugs;
        private List<int> bossItems
            ;

        public LiveSplitState state;

        static int[] keyItems = { 
            (int)Items.SmallCoin, 
            (int)Items.HazelBadge,
            (int)Items.MonasteryKey,
            (int)Items.DirtyShroom,
            (int)Items.IvoryBug,
            (int)Items.GardenKey,
            (int)Items.MapleLeaf,
            (int)Items.FreshSpringLeaf,
            (int)Items.Birthstone,
            (int)Items.CinderKey,
            (int)Items.FragmentBowPow,
            (int)Items.FragmentBowQuick,
            (int)Items.FragmentDash,
            (int)Items.FragmentWarp
        };
        static int[] activeItems = { 
            (int)Items.Bellflower,
            (int)Items.RottenBellflower,
            (int)Items.Passiflora,
            (int)Items.CrystalSeed,
            (int)Items.TaintedMissive,
            (int)Items.BackmanPatch,
            (int)Items.CatSphere,
            (int)Items.ClarityShard,
            (int)Items.VioletSprite,
            (int)Items.SoftTissue,
            (int)Items.SparseThread,
            (int)Items.BlessingCharm,
            (int)Items.BloodstainedTissue,
            (int)Items.SealedWind
        };
        static int[] passiveItems = {
            (int)Items.AdornedRing,
            (int)Items.NecklaceOfSacrifice,
            (int)Items.AstralCharm,
            (int)Items.EdeaPearl,
            (int)Items.DullPearl,
            (int)Items.RedRing,
            (int)Items.MagnetStone,
            (int)Items.FaerieTear,
            (int)Items.ImpurityFlask,
            (int)Items.MedalOfEquivalence,
            (int)Items.BlackSachet,
            (int)Items.RingOfCandor,
            (int)Items.TornBranch,
            (int)Items.HeavyArrows,
            (int)Items.PocketIncensory,
            (int)Items.QuickArrows,
            (int)Items.DrillingArrows
        };

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
        IntPtr oneDeliveredPointer;
        IntPtr fiveDeliveredPointer;
        IntPtr tenDeliveredPointer;
        IntPtr fifteenDeliveredPointer;
        IntPtr twentyDeliveredPointer;
        #endregion

        #region vitality fragment pointers
        IntPtr vitalityFragmentCountPointer;
        IntPtr maxHealthPointer;
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

        #region misc pointers
        IntPtr difficultyPointer;
        IntPtr levelIDPointer;
        IntPtr inGamePointer;
        IntPtr saveAmountPointer;
        IntPtr deathAmountPointer;
        #endregion
        #endregion

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
        private MemoryWatcherList specialWatchers;
        private bool randomizerRunning;
        private int itemGiven;

        List<bool> hasChargeItem;
        List<bool> hasSavedChargeItem;
        private bool hasPickedGreenLeaf;
        private bool hasSavedPickedGreenLeaf;
        private double pickedUpLeaf;

        List<List<int>> doorLocations;
        private double unlocked;
        List<int> hasSavedKey;
        List<int> hasKey;

        private bool inGame;


        public MomodoraRandomizer(LiveSplitState state)
        {
            this.state = state;
            RandomizerLabel = new SimpleLabel("Randomizer Go!");
            settingsControl = new MomodoraRandomizerSettings();
            bannedSources = new List<int>();
            usedSources = new List<int>();
            possibleSources = new List<int>();
            impossibleSources = new List<int>();

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
            requirementLists = new List<List<int>>();
            requiresCatSphere = new List<int> { 24, 27, 39, 47, 48, 55, 63, 64, 65, 66, 67, 68, 70, 74, 75, 79 };
            requiresCrestFragments = new List<int> { 0, 2, 17, 18, 19, 20, 21, 22, 23, 38, 39, 47, 50, 51, 52, 53, 54, 55, 71, 72, 73, 74, 75 };
            requirementLists.Add(requiresCrestFragments);
            requiresGardenKey = new List<int> { 66, 67, 68, 35, 26, 25 };
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
            requiresIvoryBugs = new List<int> { 80,81,82 };
            requirementLists.Add(requiresIvoryBugs);
            //GardenKey, MonasteryKey, CinderKey
            doorLocations = new List<List<int>>
            {
                new List<int> { 145, 84},
                new List<int> { 99 },
                new List<int> { 183 }
            };

            state.OnStart += onStart;
            state.OnReset += onReset;
        }

        private void onReset(object sender, TimerPhase value)
        {
            randomizerRunning = false;
        }

        private void onStart(object sender, EventArgs e)
        {
            if (VerifyProcessRunning())
            {
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

                hasSavedChargeItem = new List<bool> { false, false, false };
                hasChargeItem = new List<bool> { false, false, false };
                hasSavedKey = new List<int> { 0, 0, 0 };
                hasKey = new List<int> { 0, 0, 0 };
                hasPickedGreenLeaf = false;

                resetSources();
                updateBannedSources();
                Array.Clear(requirementMatrix, 0, requirementMatrix.Length);
                randoSourceWatchers = new MemoryWatcherList();
                specialWatchers = new MemoryWatcherList();

                //Key items are played in order: Cat Sphere, Crest Fragments, Garden Key, Cinder Key, Monastery Key, (Hazel Badge, Soft Tissue, Dirty Shroom, Ivory Bugs)
                #region item placement
                //1. Place Cat Sphere
                #region cat sphere
                updateImpossibleSources((int)Items.CatSphere);
                updatePossibleSources();
                int index = randomGenerator.Next(possibleSources.Count); 
                createMemoryWatcher((int)Items.CatSphere, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) catRequires[i] = true;
                    if (requirementLists[i].Contains(index)) requirementMatrix[0,i] = true;
                }
                #endregion

                //2: Place Crest Fragments
                #region crest Fragments
                updateImpossibleSources((int)Items.FragmentBowPow);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.FragmentBowPow, possibleSources[index]);
                possibleSources.Remove(index);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) crestRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }

                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.FragmentBowQuick, possibleSources[index]);
                possibleSources.Remove(index);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) crestRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }

                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.FragmentDash, possibleSources[index]);
                possibleSources.Remove(index);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) crestRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }

                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.FragmentWarp, possibleSources[index]);
                possibleSources.Remove(index);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) crestRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[1, j] = requirementMatrix[i, j];
                        requirementMatrix[1, i] = true;
                    }
                }
                #endregion

                //3: Garden Key
                #region garden Key
                updateImpossibleSources((int)Items.GardenKey);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.GardenKey, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) gardenRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[2, j] = requirementMatrix[i, j];
                        requirementMatrix[2, i] = true;
                    }
                }
                #endregion

                //4: Cinder Key
                #region cinder Key
                updateImpossibleSources((int)Items.CinderKey);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.CinderKey, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) cinderRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[3, j] = requirementMatrix[i, j];
                        requirementMatrix[3, i] = true;
                    }
                }
                #endregion

                //5: Monastery Key
                #region monastery Key
                updateImpossibleSources((int)Items.MonasteryKey);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.MonasteryKey, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) monasteryRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[4, j] = requirementMatrix[i, j];
                        requirementMatrix[4, i] = true;
                    }
                }
                #endregion

                //6: Hazel Badge
                #region hazel badge
                updateImpossibleSources((int)Items.HazelBadge);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.HazelBadge, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) hazelRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[5, j] = requirementMatrix[i, j];
                        requirementMatrix[5, i] = true;
                    }
                }
                #endregion

                //7: Soft Tissue
                #region soft tissue
                updateImpossibleSources((int)Items.SoftTissue);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.SoftTissue, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) softTissueRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[6, j] = requirementMatrix[i, j];
                        requirementMatrix[6, i] = true;
                    }
                }
                #endregion

                //8: Dirty Shroom
                #region dirty shroom
                updateImpossibleSources((int)Items.DirtyShroom);
                updatePossibleSources();
                index = randomGenerator.Next(possibleSources.Count);
                createMemoryWatcher((int)Items.DirtyShroom, possibleSources[index]);
                usedSources.Add(index);
                for (int i = 0; i < requirementLists.Count; i++)
                {
                    if (requirementLists[i].Contains(index)) dirtyShroomRequires[i] = true;
                    if (requirementLists[i].Contains(index))
                    {
                        for (int j = 0; j < 8; j++) requirementMatrix[7, j] = requirementMatrix[i, j];
                        requirementMatrix[7, i] = true;
                    }
                }
                #endregion

                //9. Place Ivory Bugs
                #region Ivory Bugs
                updateImpossibleSources((int)Items.IvoryBug);
                for (int i = 56; i < 76; i++)
                {
                    updatePossibleSources();
                    index = randomGenerator.Next(possibleSources.Count);
                    createMemoryWatcher((int)Items.IvoryBug, possibleSources[index]);
                    usedSources.Add(index);
                }
                #endregion

                //10. Place Vitality Fragments
                #region vitality fragments
                for (int i = 39; i < 56; i++)
                {
                    updatePossibleSources();
                    index = randomGenerator.Next(possibleSources.Count);
                    createMemoryWatcher((int)Items.VitalityFragment, possibleSources[index]);
                    usedSources.Add(index);
                }
                #endregion

                //11. Rest of items
                #region rest of items
                for (int i = 0; i < 39; i++)
                {
                    if (!bannedSources.Contains(i))
                    {
                        updatePossibleSources();
                        index = randomGenerator.Next(possibleSources.Count);
                        createMemoryWatcher(sourceIdMapping[i], possibleSources[index]);
                        usedSources.Add(index);
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
                    checkRoom(old, current);
                };

                inGameWatcher = new MemoryWatcher<double>(inGamePointer);
                inGameWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                inGameWatcher.Enabled = true;
                inGameWatcher.OnChanged += (old, current) =>
                {
                    inGame = Convert.ToBoolean(current);
                    if(current == 0)
                    {
                        for(int i = 0; i < hasChargeItem.Count(); i++)
                        {
                            hasChargeItem[i] = hasSavedChargeItem[i];
                            hasKey[i] = hasSavedKey[i];
                            hasPickedGreenLeaf = hasSavedPickedGreenLeaf;
                        }
                    }
                };

                deathWatcher = new MemoryWatcher<double>(deathAmountPointer);
                deathWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                deathWatcher.Enabled = true;
                deathWatcher.OnChanged += (old, current) =>
                {
                    if (current > old)
                    {
                        for (int i = 0; i < hasChargeItem.Count(); i++)
                        {
                            hasChargeItem[i] = hasSavedChargeItem[i];
                            hasKey[i] = hasSavedKey[i];
                            hasPickedGreenLeaf = hasSavedPickedGreenLeaf;
                        }
                    }
                };

                saveWatcher = new MemoryWatcher<double>(saveAmountPointer);
                saveWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                saveWatcher.Enabled = true;
                saveWatcher.OnChanged += (old, current) =>
                {
                    if (current > old)
                    {
                        for (int i = 0; i < hasChargeItem.Count(); i++)
                        {
                            hasSavedChargeItem[i] = hasChargeItem[i];
                            hasSavedKey[i] = hasKey[i];
                            hasSavedPickedGreenLeaf = hasPickedGreenLeaf;
                        }
                    }
                };

                specialWatchers.Add(levelIDWatcher);
                specialWatchers.Add(saveWatcher);
                specialWatchers.Add(deathWatcher);
                specialWatchers.Add(inGameWatcher);

                #endregion

                randomizerRunning = true;
            }
        }

        private void resetSources()
        {
            bannedSources.Clear();
            impossibleSources.Clear();
            usedSources.Clear();
            possibleSources.Clear();
        }

        private void createMemoryWatcher(int giveItemID, int newSourceAddressIndex)
        {
            //Maybe change Debug.WriteLine to write to a file instead?
            Debug.WriteLine("Item " + Enum.GetName(typeof(Items),giveItemID) + " generated at position " + newSourceAddressIndex);
            MemoryWatcher<double> temp = new MemoryWatcher<double>(potentialSourcesPointers[newSourceAddressIndex]);
            temp.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
            if (potentialSourcesPointers[newSourceAddressIndex] != potentialSourcesPointers[28]) {
                temp.OnChanged += (old, current) =>
                {
                    int levelID = gameProc.ReadValue<int>(levelIDPointer);
                    if (current == 1 && sourceToLevelMapping[newSourceAddressIndex].Contains(levelID))
                    {
                        hasPickedGreenLeaf = true;
                        newItem(giveItemID);
                    }
                };
            }
            else {
                temp.OnChanged += (old, current) =>
                {
                    int levelID = gameProc.ReadValue<int>(levelIDPointer);
                    if (current == 1 && sourceToLevelMapping[newSourceAddressIndex].Contains(levelID))
                    {
                        newItem(giveItemID);
                    }
                };
            }
            temp.Enabled = true;
            randoSourceWatchers.Add(temp);
        }

        private void updateImpossibleSources(int itemId)
        {
            impossibleSources.Clear();
            int j = 0;
            //Key items are played in order: Cat Sphere, Crest Fragments, Garden Key, Cinder Key, Monastery Key, (Hazel Badge, Soft Tissue, Dirty Shroom, Ivory Bug) 
            if (itemId == (int)Items.CatSphere)
            { 
                impossibleSources.AddRange(requiresCatSphere);
                return;
            }
            else if (itemId == (int)Items.FragmentBowPow) j = 0;
            else if (itemId == (int)Items.GardenKey) j = 1;
            else if (itemId == (int)Items.CinderKey) j = 2;
            else if (itemId == (int)Items.MonasteryKey) j = 3;
            else if (itemId == (int)Items.HazelBadge) j = 4;
            else if (itemId == (int)Items.SoftTissue) j = 5;
            else if (itemId == (int)Items.DirtyShroom) j = 6;
            else if (itemId == (int)Items.IvoryBug) j = 7;
            for(int i = 0; i < requirementMatrix.GetLength(0); i++)
            {
                impossibleSources.AddRange(requirementLists[j]);
                if (requirementMatrix[i, j]) impossibleSources.AddRange(requirementLists[i]);
            }
        }

        private void updateBannedSources()
        {
            if (!settingsControl.VitalityFragmentsEnabled) bannedSources.AddRange(vitalityFragments);
            if (!settingsControl.IvoryBugsEnabled) bannedSources.AddRange(ivoryBugs);
            if (!settingsControl.HardModeEnabled) bannedSources.AddRange(bossItems);

        }

        private void updatePossibleSources()
        {
            for(int i = 0;  i < RANDOMIZER_SOURCE_AMOUNT; i++)
            {
                if(!bannedSources.Contains(i) && !impossibleSources.Contains(i) && !usedSources.Contains(i))
                {
                    possibleSources.Add(i);
                }
            }
        }

        //Use newItem to give out an item [with charges] and remove the last item acquired
        private void newItem(int id, int addCharges = 0)
        {
            //testMagic
            itemGiven = 3;

            removeItem();
            if (id == (int)Items.IvoryBug)
            {
                addIvoryBug();
            }
            else if ((int)Items.FragmentWarp >= id && id >= (int)Items.FragmentBowPow)
            {
                addCrestFragment(id);
            }
            else if (id == (int)Items.VitalityFragment)
            {
                addVitalityFragment();
            }
            else if (id == (int)Items.Bellflower || id == (int)Items.Passiflora || id == (int)Items.TaintedMissive)
            {
                addChargeItem(id, addCharges);
            }
            else if (id == (int)Items.MonasteryKey || id == (int)Items.GardenKey || id == (int)Items.CinderKey)
            {
                addKey(id);
            }
            else if (id == (int)Items.FreshSpringLeaf)
            {
                addLeaf();
            }
            else
            {
                addItem(id);
            }
        }

        private void addLeaf()
        {
            gameProc.WriteValue<double>(potentialSourcesPointers[28], 1);
            addItem((int)Items.FreshSpringLeaf);
        }

        private void removeItem()
        {
            UpdateItemWatchers();

            if (taintedMissiveWatcher.Changed)
            {
                Debug.WriteLine("Removing missive");
                removeChargeItem((int)Items.TaintedMissive, taintedMissiveWatcher.Current - taintedMissiveWatcher.Old);
            }
            else if (bellflowerWatcher.Changed)
            {
                Debug.WriteLine("Removing bellflower");
                removeChargeItem((int)Items.Bellflower, bellflowerWatcher.Current - bellflowerWatcher.Old);
            }
            else if (passifloraWatcher.Changed)
            {
                Debug.WriteLine("Removing passi");
                removeChargeItem((int)Items.Passiflora, passifloraWatcher.Current - passifloraWatcher.Old);
            }
            else if (ivoryBugWatcher.Changed)
            {
                Debug.WriteLine("Removing IB");
                removeIvoryBug();
            }
            else if (crestFragmentWatcher.Changed)
            {
                Debug.WriteLine("Crest Frag");
                removeCrestFragment();
            }
            else if (vitalityFragmentWatcher.Changed)
            {
                Debug.WriteLine("Removing Vit Frag");
                removeVitalityFragment();
            }
            else
            {
                Debug.WriteLine("Removing General Item");
                removeLastItem();
            }
        }

        private void addChargeItem(int id, double charges)
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
                addItem(id);
            }

            //Update charges if item in inventory, otherwise /shrug
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            for(int i = 0; i > totalItemAmount; i++)
            {
                if (gameProc.ReadValue<double>(IntPtr.Add(inventoryItemsStartPointer,0x10*i)) == (double)id)
                {
                    double currentCharges = gameProc.ReadValue<double>(IntPtr.Add(inventoryItemsChargeStartPointer, 0x10 * i));
                    gameProc.WriteValue<double>(IntPtr.Add(inventoryItemsChargeStartPointer, 0x10 * i), currentCharges + charges);
                }
            }
        }

        private void removeChargeItem(int id, double charges)
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

        private void addItem(int id)
        {
            //To add an item: Increase total item counter by one
            //and set the inventory value for the next item slot to the id of the item given
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            gameProc.WriteValue<int>(totalItemsPointer, (int)totalItemAmount + 1);
            gameProc.WriteValue<double>(IntPtr.Add(inventoryItemsStartPointer, 0x10 * totalItemAmount), id);
        }

        private void removeLastItem()
        {
            //To remove last item, decrease total item counter by one
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            gameProc.WriteValue<int>(totalItemsPointer, (int)totalItemAmount - 1);
            
        }

        private void removeCrestFragment()
        {
            double fragments = gameProc.ReadValue<double>(crestFragmentCountPointer);
            gameProc.WriteValue<double>(crestFragmentCountPointer, fragments - 1);
            removeLastItem();
        }

        private void addCrestFragment(int id)
        {
            double fragments = gameProc.ReadValue<double>(crestFragmentCountPointer);
            gameProc.WriteValue<double>(crestFragmentCountPointer, fragments + 1);
            addItem(id);
        }

        private void removeVitalityFragment()
        {
            double[] healthChange = { 0, 2, 1, 1 };
            double difficulty = gameProc.ReadValue<double>(difficultyPointer);
            double fragments = gameProc.ReadValue<double>(vitalityFragmentCountPointer);
            double health = gameProc.ReadValue<double>(maxHealthPointer);
            gameProc.WriteValue<double>(vitalityFragmentCountPointer, fragments - 1);
            gameProc.WriteValue<double>(maxHealthPointer, health - healthChange[(int)difficulty-1]);
        }

        private void addVitalityFragment()
        {
            double[] healthChange = { 0, 2, 1, 1 };
            double difficulty = gameProc.ReadValue<double>(difficultyPointer);
            double fragments = gameProc.ReadValue<double>(vitalityFragmentCountPointer);
            double health = gameProc.ReadValue<double>(maxHealthPointer);
            gameProc.WriteValue<double>(vitalityFragmentCountPointer, fragments + 1);
            gameProc.WriteValue<double>(maxHealthPointer, health + healthChange[(int)difficulty - 1]);
        }

        private void addIvoryBug()
        {
            double bugs = gameProc.ReadValue<double>(ivoryBugCountPointer);
            gameProc.WriteValue<double>(ivoryBugCountPointer, bugs + 1);
            if(bugs == 0)
            {
                addItem((int)Items.IvoryBug);
            }
        }

        private void removeIvoryBug()
        {
            double bugs = gameProc.ReadValue<double>(ivoryBugCountPointer);
            gameProc.WriteValue<double>(ivoryBugCountPointer, bugs - 1);
            if (bugs == 1)
            {
                removeLastItem();
            }
        }

        private void addKey(int id)
        {
            if(id == (int)Items.MonasteryKey)
            {
                hasKey[1] = 1;
            }
            else if (id == (int)Items.GardenKey)
            {
                hasKey[2] = 1;
            }
            else
            {
                hasKey[3] = 1;
            }

            addItem(id);
        }

        private void checkRoom(int old, int current)
        {
            #region key logic
            int j;

            for (int i = 0; i < 3; i++)
            {
                j = i + 4;//Id for keys is 4-6

                if (doorLocations[i].Contains(current))//If the player is in a room with doors that pertaint to a key
                {
                    unlocked = gameProc.ReadValue<double>(potentialSourcesPointers[j]);//Read value for key acquired

                    if (unlocked != hasKey[i])//If the state of "key acquired" is different than it should be invert value
                    {
                        gameProc.WriteValue<double>(potentialSourcesPointers[j], 1-unlocked);//1-0=1; 1-1=0
                    }
                }
                else if (doorLocations[i].Contains(old) && !doorLocations[i].Contains(current))//If they just left the room revert state of flag
                {
                    gameProc.WriteValue<double>(potentialSourcesPointers[j], unlocked);
                }
            }
            #endregion

            #region green leaf logic
            if(current == 83)
            {
                pickedUpLeaf = gameProc.ReadValue<double>(potentialSourcesPointers[28]);
                //If the state of "has picked green leaf source" is different from "found green leaf" invert
                if (Convert.ToBoolean(pickedUpLeaf) != hasPickedGreenLeaf) gameProc.WriteValue<double>(potentialSourcesPointers[28], 1 - pickedUpLeaf);
            }
            else if (old == 83)
            {
                gameProc.WriteValue<double>(potentialSourcesPointers[28], pickedUpLeaf);
            };
            #endregion
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

            RandomizerLabel.SetActualWidth(g);
            RandomizerLabel.Width = RandomizerLabel.ActualWidth;
            RandomizerLabel.Height = VerticalHeight;
            RandomizerLabel.X = width/2;
            RandomizerLabel.Y = 3f;

            RandomizerLabel.Draw(g);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return settingsControl.GetSettings(document);
        }

        public System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
        {
            return settingsControl;
        }

        public void SetSettings(XmlNode settings)
        {
            settingsControl.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (VerifyProcessRunning() && inGame && randomizerRunning)
            {

                //do update stuff here!
                foreach (var watcher in specialWatchers)
                {
                    watcher.Update(gameProc);
                }
                foreach (var watcher in randoSourceWatchers)
                {
                    watcher.Update(gameProc);
                }
                if (itemGiven > 0) { 
                    UpdateItemWatchers();
                    itemGiven--;
                }

                if (invalidator != null)
                {
                    invalidator.Invalidate(0, 0, width, height);
                }
            }

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

        private bool VerifyProcessRunning()
        {
            if (gameProc != null && !gameProc.HasExited)
            {
                return true;
            }
            Process[] game = Process.GetProcessesByName("MomodoraRUtM");
            if (game.Length > 0)
            {
                switch (game[0].MainModule.ModuleMemorySize)
                {
                    case 39690240:
                        //version 1.05b
                        gameProc = game[0];
                        Debug.WriteLine("Setting up pointers");
                        #region setting up IntPtrs
                        potentialSourcesPointers = new IntPtr[RANDOMIZER_SOURCE_AMOUNT];
                        potentialSourcesPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xF0);
                        potentialSourcesPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x100);
                        potentialSourcesPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x410);
                        potentialSourcesPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x430);
                        potentialSourcesPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x700);
                        potentialSourcesPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9B0);
                        potentialSourcesPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x260);
                        potentialSourcesPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x420);
                        potentialSourcesPointers[8] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4}).Deref<Int32>(gameProc), 0xcf0);
                        potentialSourcesPointers[9] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcb0);
                        potentialSourcesPointers[10] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd50);
                        potentialSourcesPointers[11] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xdd0);
                        potentialSourcesPointers[12] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc70);
                        potentialSourcesPointers[13] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x50);
                        potentialSourcesPointers[14] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x80);
                        potentialSourcesPointers[15] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xeb0);
                        potentialSourcesPointers[16] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcd0);
                        potentialSourcesPointers[17] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xe00);
                        potentialSourcesPointers[18] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xea0);
                        potentialSourcesPointers[19] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xb0);
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
                        potentialSourcesPointers[38] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);
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
                        potentialSourcesPointers[78] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] {0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5A0);
                        potentialSourcesPointers[79] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5C0);
                        potentialSourcesPointers[80] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7A0);
                        potentialSourcesPointers[81] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7B0);
                        potentialSourcesPointers[82] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7C0);

                        oneDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x780);
                        fiveDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x790);

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

                        totalItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230b11c,new int[] { 0x1a4}).Deref<Int32>(gameProc),0x4);

                        activeItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc30);
                        passiveItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                        keyItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1100);
                        inventoryItemsStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4, 0xC }).Deref<int>(gameProc);
                        inventoryItemsChargeStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a8, 0xC }).Deref<int>(gameProc);
                        levelIDPointer = (IntPtr)new DeepPointer(0x230F1A0).Deref<int>(gameProc);
                        deathAmountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4}).Deref<int>(gameProc),0x540);
                        inGamePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0,0x4 }).Deref<int>(gameProc), 0x780);
                        saveAmountPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x0,0x4,0x60,0x4,0x4 }).Deref<int>(gameProc);
                        #endregion
                        RandomizerLabel.Text = "Randomizer ready to go!";
                        return true;
                    default:
                        RandomizerLabel.Text = "Unsupported game version for randomizer";
                        break;
                }   
            }
            return false;
        }
    }
}
