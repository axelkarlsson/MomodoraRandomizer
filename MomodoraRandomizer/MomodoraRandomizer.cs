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
        public enum Items: int
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
        IntPtr levelID;
        #endregion
        #endregion

        MemoryWatcher<double> taintedMissiveWatcher;
        MemoryWatcher<double> bellflowerWatcher;
        MemoryWatcher<double> passifloraWatcher;
        MemoryWatcher<double> ivoryBugWatcher;
        MemoryWatcher<double> crestFragmentWatcher;
        MemoryWatcher<double> vitalityFragmentWatcher;
        MemoryWatcher<int> levelIDWatcher;
        private bool randomizerRunning;
        private int itemGiven;
        private bool hasMissive;
        private bool hasPassiflora;
        private bool hasBellflower;

        List<List<int>> doorLocations;
        private double unlocked;
        List<int> hasKey = new List<int> { 0, 0, 0 };

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

                resetSources();
                updateBannedSources();
                Array.Clear(requirementMatrix, 0, requirementMatrix.Length);
                randoSourceWatchers = new MemoryWatcherList();

                //Key items are played in order: Cat Sphere, Crest Fragments, Garden Key, Cinder Key, Monastery Key, (Hazel Badge, Soft Tissue, Dirty Shroom, Ivory Bugs) 
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
                updateImpossibleSources((int)Items.IvoryBug);
                for (int i = 56; i < 76; i++)
                {
                    updatePossibleSources();
                    index = randomGenerator.Next(possibleSources.Count);
                    createMemoryWatcher((int)Items.IvoryBug, possibleSources[index]);
                    usedSources.Add(index);
                }

                //10. Place Vitality Fragments
                for (int i = 39; i < 56; i++)
                {
                    updatePossibleSources();
                    index = randomGenerator.Next(possibleSources.Count);
                    createMemoryWatcher((int)Items.VitalityFragment, possibleSources[index]);
                    usedSources.Add(index);
                }

                //11. Rest of items
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

                levelIDWatcher = new MemoryWatcher<int>(levelID);
                levelIDWatcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 10);
                levelIDWatcher.Enabled = true;
                levelIDWatcher.OnChanged += (old, current) =>
                {
                    checkRoom(old, current);
                };
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
            temp.OnChanged += (old, current) =>
            {
                if(current == 1)
                {
                    newItem(giveItemID);
                }
            };
            temp.Enabled = true;
            randoSourceWatchers.Add(temp);
        }

        private void updateImpossibleSources(int itemId)
        {
            impossibleSources.Clear();
            //Key items are played in order: Cat Sphere, Crest Fragments, Garden Key, Cinder Key, Monastery Key, (Hazel Badge, Soft Tissue, Dirty Shroom, Ivory Bug) 
            if (itemId == (int)Items.CatSphere)
            { 
                impossibleSources.AddRange(requiresCatSphere);
            }
            else if (itemId == (int)Items.FragmentBowPow)
            {
                impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[0,0]) impossibleSources.AddRange(requiresCatSphere);
            }
            else if (itemId == (int)Items.GardenKey)
            {
                impossibleSources.AddRange(requiresGardenKey);
                if (requirementMatrix[0, 1]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 1]) impossibleSources.AddRange(requiresCrestFragments);
            }
            else if (itemId == (int)Items.CinderKey)
            {
                impossibleSources.AddRange(requiresCinderKey);
                if (requirementMatrix[0, 2]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 2]) impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[2, 2]) impossibleSources.AddRange(requiresGardenKey);
            }
            else if (itemId == (int)Items.MonasteryKey)
            {
                impossibleSources.AddRange(requiresMonasteryKey);
                if (requirementMatrix[0, 3]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 3]) impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[2, 3]) impossibleSources.AddRange(requiresGardenKey);
                if (requirementMatrix[3, 3]) impossibleSources.AddRange(requiresCinderKey);
            }
            else if (itemId == (int)Items.HazelBadge)
            {
                impossibleSources.AddRange(requiresHazelBadge);
                if (requirementMatrix[0, 4]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 4]) impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[2, 4]) impossibleSources.AddRange(requiresGardenKey);
                if (requirementMatrix[3, 4]) impossibleSources.AddRange(requiresCinderKey);
                if (requirementMatrix[4, 4]) impossibleSources.AddRange(requiresMonasteryKey);
            }
            else if (itemId == (int)Items.SoftTissue)
            {
                impossibleSources.AddRange(requiresSoftTissue);
                if (requirementMatrix[0, 5]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 5]) impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[2, 5]) impossibleSources.AddRange(requiresGardenKey);
                if (requirementMatrix[3, 5]) impossibleSources.AddRange(requiresCinderKey);
                if (requirementMatrix[4, 5]) impossibleSources.AddRange(requiresMonasteryKey);
                if (requirementMatrix[5, 5]) impossibleSources.AddRange(requiresHazelBadge);
            }
            else if (itemId == (int)Items.DirtyShroom)
            {
                impossibleSources.AddRange(requiresDirtyShroom);
                if (requirementMatrix[0, 6]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 6]) impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[2, 6]) impossibleSources.AddRange(requiresGardenKey);
                if (requirementMatrix[3, 6]) impossibleSources.AddRange(requiresCinderKey);
                if (requirementMatrix[4, 6]) impossibleSources.AddRange(requiresMonasteryKey);
                if (requirementMatrix[5, 6]) impossibleSources.AddRange(requiresHazelBadge);
                if (requirementMatrix[6, 6]) impossibleSources.AddRange(requiresSoftTissue);
            }
            else if (itemId == (int)Items.IvoryBug)
            {
                impossibleSources.AddRange(requiresIvoryBugs);
                if (requirementMatrix[0, 7]) impossibleSources.AddRange(requiresCatSphere);
                if (requirementMatrix[1, 7]) impossibleSources.AddRange(requiresCrestFragments);
                if (requirementMatrix[2, 7]) impossibleSources.AddRange(requiresGardenKey);
                if (requirementMatrix[3, 7]) impossibleSources.AddRange(requiresCinderKey);
                if (requirementMatrix[4, 7]) impossibleSources.AddRange(requiresMonasteryKey);
                if (requirementMatrix[5, 7]) impossibleSources.AddRange(requiresHazelBadge);
                if (requirementMatrix[6, 7]) impossibleSources.AddRange(requiresSoftTissue);
                if (requirementMatrix[7, 7]) impossibleSources.AddRange(requiresDirtyShroom);
            }
        }

        private void updateBannedSources()
        {
            //If !VitFragments
            bannedSources.AddRange(vitalityFragments);
            //If !Ivory Bugs
            bannedSources.AddRange(ivoryBugs);
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
            else
            {
                addItem(id);
            }
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
            switch (id)
            {
                case (int)Items.Bellflower:
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    hasItem = hasBellflower;
                    break;
                case (int)Items.Passiflora:
                    maxValuePointer = passifloraMaxValuePointer;
                    saveValuePtr = passifloraSaveValuePointer;
                    hasItem = hasPassiflora;
                    break;
                case (int)Items.TaintedMissive:
                    maxValuePointer = taintedMissiveMaxValuePointer;
                    saveValuePtr = taintedMissiveSaveValuePointer;
                    hasItem = hasMissive;
                    break;
                default:
                    return;
            }
            currentMaxCharges = gameProc.ReadValue<double>(maxValuePointer);
            gameProc.WriteValue<double>(maxValuePointer, charges + currentMaxCharges);
            gameProc.WriteValue<double>(saveValuePtr, charges + currentMaxCharges);
            if (currentMaxCharges == 0 && !hasItem)
            {
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
            switch (id)
            {
                case (int)Items.Bellflower:
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    hasBellflower = true;
                    break;
                case (int)Items.Passiflora:
                    maxValuePointer = passifloraMaxValuePointer;
                    saveValuePtr = passifloraSaveValuePointer;
                    hasPassiflora = true;
                    break;
                case (int)Items.TaintedMissive:
                    maxValuePointer = taintedMissiveMaxValuePointer;
                    saveValuePtr = taintedMissiveSaveValuePointer;
                    hasMissive = true;
                    break;
                default:
                    return;
            }
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
            if (VerifyProcessRunning() && randomizerRunning)
            {

                //do update stuff here!
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
                        //0-7 NO REQ PICKUPS
                        //0 = Grove Bellflower
                        potentialSourcesPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xF0);
                        //1 = Astral Charm
                        potentialSourcesPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x100);
                        //2 = Karst Bellflower
                        potentialSourcesPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x410);
                        //3 = Magnet Stone
                        potentialSourcesPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x430);
                        //4 = Garden Key
                        potentialSourcesPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x700);
                        //5 = Cinder Key
                        potentialSourcesPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9B0);
                        //6 = Monastery Key
                        potentialSourcesPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x260);
                        //7 = Tainted Missive Pickup
                        potentialSourcesPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x420);
                        //8-16 NO REQ SHOP ITEMS
                        //Crystal Seed
                        potentialSourcesPointers[8] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4}).Deref<Int32>(gameProc), 0xcf0);
                        //Faerie Tear
                        potentialSourcesPointers[9] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcb0);
                        //Ring of Candor
                        potentialSourcesPointers[10] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd50);
                        //Clarity Shard
                        potentialSourcesPointers[11] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xdd0);
                        //Necklace of Sacrifice
                        potentialSourcesPointers[12] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc70);
                        //Dull Pearl
                        potentialSourcesPointers[13] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x50);
                        //Red Ring
                        potentialSourcesPointers[14] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x80);
                        //Drilling Arrows
                        potentialSourcesPointers[15] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xeb0);
                        //Impurity Flask
                        potentialSourcesPointers[16] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcd0);
                        //17-19 4CF SHOP ITEMS
                        //Violet Sprite
                        potentialSourcesPointers[17] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xe00);
                        //Quick Arrows
                        potentialSourcesPointers[18] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xea0);
                        //Pocket Incensory
                        potentialSourcesPointers[19] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xb0);
                        //PICKUPREQS
                        //20-23 ALL CF REQ
                        //Black Sachet
                        potentialSourcesPointers[20] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA00);
                        //Sealed Wind
                        potentialSourcesPointers[21] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA90);
                        //Castle Bellflower
                        potentialSourcesPointers[22] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6D0);
                        //Passiflora Cath
                        potentialSourcesPointers[23] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x690);
                        //24 CAT SPHERE REQ
                        //Dirty Shroom
                        potentialSourcesPointers[24] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x480);
                        //25-26 GARDEN KEY OR *BACKMAN PATCH REQ
                        //Cat Sphere
                        potentialSourcesPointers[25] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x720);
                        //Bellflower Park
                        potentialSourcesPointers[26] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7E0);
                        //27 CAT SPHERE AND MONASTERY KEY REQ
                        //Soft Tissue
                        potentialSourcesPointers[27] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x570);
                        //28 SEALED WIND AND CAT SPHERE REQ
                        //Green Leaf
                        potentialSourcesPointers[28] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x600);
                        //29 HAZEL BADGE REQ
                        //Blessed Charm
                        potentialSourcesPointers[29] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);
                        //30 DIRTY SHROOM REQ
                        //Rotten Shroom
                        potentialSourcesPointers[30] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x500);
                        //31-34 NO REQ BOSS ITEMS
                        //Edea's Pearl
                        potentialSourcesPointers[31] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x140);
                        //Backman Patch
                        potentialSourcesPointers[32] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x450);
                        //Sparse Thread
                        potentialSourcesPointers[33] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7D0);
                        //Pocket Incensory
                        potentialSourcesPointers[34] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9F0);
                        //35 GARDEN KEY OR *BACKMAN PATCH
                        //Torn Branch
                        potentialSourcesPointers[35] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x580);
                        //36 MONASTERY KEY REQ
                        //Tainted Missive
                        potentialSourcesPointers[36] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x270);
                        //37 SOFT TISSUE REQ
                        //Bloodstained Tissue
                        potentialSourcesPointers[37] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6B0);
                        //38 ALL CF REQ
                        //Heavy Arrows
                        potentialSourcesPointers[38] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);
                        //39-46 NO REQ VIT FRAGMENTS
                        //Grove behind first hidden wall
                        potentialSourcesPointers[39] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x150);
                        //Grove behind second hidden wall
                        potentialSourcesPointers[40] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x190);
                        //Karst City Dog Fragment
                        potentialSourcesPointers[41] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x170);
                        //Karst City Boulder Fragment
                        potentialSourcesPointers[42] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1B0);
                        //Sub Graves
                        potentialSourcesPointers[43] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x180);
                        //Memorial Park near Cinder Chambers
                        potentialSourcesPointers[44] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8C0);
                        //Cinder Chambers bottom text
                        potentialSourcesPointers[45] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8D0);
                        //Monastery near Cat
                        potentialSourcesPointers[46] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x160);
                        //47-48 CAT SPHERE REQ
                        //Grove Cat Room **WITH DAMAGE BOOST**
                        potentialSourcesPointers[47] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x940);
                        //Frore Ciele
                        potentialSourcesPointers[48] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1C0);
                        //49 CINDER KEY REQ
                        //Cinder Chambers Lever Room
                        potentialSourcesPointers[49] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1A0);
                        //50-54 ALL CF REQ
                        //Pina Fragment 1
                        potentialSourcesPointers[50] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8E0);
                        //Pina Fragment Bellroom
                        potentialSourcesPointers[51] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8F0);
                        //Pina Fragment Bakaroom
                        potentialSourcesPointers[52] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x900);
                        //Castle Far Right
                        potentialSourcesPointers[53] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x930);
                        //Castle near Cath
                        potentialSourcesPointers[54] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x920);
                        //55 ALL CF AND CAT SPHERE REQ
                        //Castle after bridge 1
                        potentialSourcesPointers[55] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x910);
                        //56-62 IVORY BUG NO REQ
                        //Grove 2nd bell
                        potentialSourcesPointers[56] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x280);
                        //Grove 3rd bug
                        potentialSourcesPointers[57] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x290);
                        //Karst City Bridge
                        potentialSourcesPointers[58] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3A0);
                        //Karst Knight
                        potentialSourcesPointers[59] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2A0);
                        //Chambers Top Bug
                        potentialSourcesPointers[60] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x300);
                        //Chambers Cinder Key Room
                        potentialSourcesPointers[61] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x310);
                        //Chambers Hidden Ceiling
                        potentialSourcesPointers[62] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2F0);
                        //63-65 CAT SPHERE REQ
                        //Grove room 2nd switch
                        potentialSourcesPointers[63] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x360);
                        //Frore Ciele
                        potentialSourcesPointers[64] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x340);
                        //Sub Graves
                        potentialSourcesPointers[65] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2E0);
                        //66-68 CAT SPHERE AND (GARDEN KEY OR *BACKMAN PATCH) REQ
                        //Park Bug
                        potentialSourcesPointers[66] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3B0);
                        //After Lubella 2 bug 1
                        potentialSourcesPointers[67] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2C0);
                        //After Lubella 2 bug 2
                        potentialSourcesPointers[68] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2D0);
                        //69 MONASTERY KEY REQ
                        //Pre Fennel no Cat
                        potentialSourcesPointers[69] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2B0);
                        //70 MONASTERY KEY AND CAT SPHERE REQ
                        //Pre Fennel Cat
                        potentialSourcesPointers[70] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x390);
                        //71-73 ALL CF REQ
                        //Pina Skeleton room
                        potentialSourcesPointers[71] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x320);
                        //Early Castle
                        potentialSourcesPointers[72] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x370);
                        //Yeet Room
                        potentialSourcesPointers[73] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x380);
                        //74-75 ALL CF AND CAT SPHERE REQ
                        //Pina Fairy Code room
                        potentialSourcesPointers[74] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x350);
                        //Pina Elevator room
                        potentialSourcesPointers[75] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x330);
                        //76-77 NO REQ CF
                        //Fast Charge
                        potentialSourcesPointers[76] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5B0);
                        //Dash
                        potentialSourcesPointers[77] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5D0);
                        //78 MONASTERY KEY
                        //Charge Level
                        potentialSourcesPointers[78] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] {0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5A0);
                        //79 CAT SPHERE REQ
                        //Warp Fragment
                        potentialSourcesPointers[79] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5C0);
                        //80-82 IB REQ
                        //10 del, Bellflower?
                        potentialSourcesPointers[80] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7A0);
                        //15 del, Hazel Badge?
                        potentialSourcesPointers[81] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7B0);
                        //20 del, Passiflora?
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
                        levelID = (IntPtr)new DeepPointer(0x230F1A0).Deref<int>(gameProc);
                        #endregion
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
