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

        private SimpleLabel RandomizerLabel;
        private Process gameProc = null;
        private Random randomGenerator = null;
        private int seed = 0;
        private MomodoraRandomizerSettings settingsControl;

        public LiveSplitState state;

        int[] keyItemIDs = { 22,25,27,29,32,34,37,42,43,45,49,50,51,52,53 };
        int[] activeItemIDs = { 4,10,14,15,17,23,24,31,35,36,38,39,41,48 };
        int[] passiveItemIDs = { 1, 2, 5,6,7,8,9,11,13,16,18,21,26,40,44,46,47 };

        #region pointers
        #region charge item pointers
        IntPtr bellflowerSaveValuePointer;
        IntPtr bellflowerMaxValuePointer;
        IntPtr taintedMissiveSaveValuePointer;
        IntPtr taintedMissiveMaxValuePointer;
        IntPtr passifloraSaveValuePointer;
        IntPtr passifloraMaxValuePointer;
        #endregion

        #region shop stock pointers
        IntPtr[] karstShopItemsPointers;
        IntPtr[] graveShopItemsPointers;
        IntPtr[] parkShopItemsPointers;
        IntPtr[] cinderShopItemsPointers;
        IntPtr[] spiderShopItemsPointers;
        IntPtr[] monasteryShopItemsPointers;
        IntPtr[] pinaShopItemsPointers;
        #endregion

        #region ivory bug pointers
        IntPtr ivoryBugCountPointer;
        IntPtr[] ivoryBugPointers;
        IntPtr oneDeliveredPointer;
        IntPtr fiveDeliveredPointer;
        IntPtr tenDeliveredPointer;
        IntPtr fifteenDeliveredPointer;
        IntPtr twentyDeliveredPointer;
        #endregion

        #region vitality fragment pointers
        IntPtr vitalityFragmentCountPointer;
        IntPtr[] vitalityFragmentPointers;
        IntPtr maxHealthPointer;
        #endregion

        #region crest fragment pointers
        IntPtr crestFragmentCountPointer;
        IntPtr[] crestFragmentPointers;
        #endregion

        #region item pickup pointers
        IntPtr[] noRequirementPickupPointers;
        IntPtr blessedCharmPointer;
        IntPtr rottenShroomPointer;
        IntPtr greenLeafPointer;
        IntPtr softTissuePointer;
        IntPtr catSpherePointer;
        IntPtr bellFlowerParkPointer;
        IntPtr dirtyShroomPointer;
        IntPtr blackSachetPointer;
        IntPtr sealedWindPointer;
        IntPtr castleBellflowerPointer;
        IntPtr passifloraCathPointer;
        #endregion

        #region boss items
        IntPtr[] bossItemPointers;
        #endregion

        #region inventory pointers
        IntPtr activeItemsPointer;
        IntPtr passiveItemsPointer;
        IntPtr keyItemsPointer;
        IntPtr totalItemsPointer;
        IntPtr inventoryItemsStartPointer;
        #endregion

        #region misc pointers
        IntPtr difficultyPointer;
        #endregion
        #endregion
        private MemoryWatcher<double>[] removeItemWatchers;
        private MemoryWatcher<double> [] testWatchers;
        private bool randomizerRunning;

        public MomodoraRandomizer(LiveSplitState state)
        {
            state.OnStart += onStart;
            state.OnReset += onReset;
            this.state = state;
            RandomizerLabel = new SimpleLabel("Randomizer Go!");
            settingsControl = new MomodoraRandomizerSettings();
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
                randomGenerator = new Random(0);
                //else, random seed
                //Thought it would be nice to do it this way so that the seed can be stored and displayed so that multiple people can run the same seed etc
                randomGenerator = new Random();
                seed = randomGenerator.Next();
                randomGenerator = new Random(seed);

                //0 = missive, 1 = bellflower, 2 = passiflora, 3 = bugs, 4 = crests, 5 = vitality
                removeItemWatchers = new MemoryWatcher<double>[6];
                removeItemWatchers[0] = new MemoryWatcher<double>(taintedMissiveMaxValuePointer);
                removeItemWatchers[1] = new MemoryWatcher<double>(bellflowerMaxValuePointer);
                removeItemWatchers[2] = new MemoryWatcher<double>(passifloraMaxValuePointer);
                removeItemWatchers[3] = new MemoryWatcher<double>(ivoryBugCountPointer);
                removeItemWatchers[4] = new MemoryWatcher<double>(crestFragmentCountPointer);
                removeItemWatchers[5] = new MemoryWatcher<double>(vitalityFragmentCountPointer);

                foreach(MemoryWatcher watcher in removeItemWatchers)
                {
                    watcher.UpdateInterval = new TimeSpan(0, 0, 0, 0, 100);
                    watcher.Enabled = true;
                }



                testWatchers = new MemoryWatcher<double>[6];
                testWatchers[0] = new MemoryWatcher<double>(noRequirementPickupPointers[1]);
                testWatchers[0].OnChanged += (oldVal, newVal) =>
                {
                    if (newVal == 1)
                    {
                        removeItem();
                        newItem(40);
                    }
                };
                testWatchers[0].UpdateInterval = new TimeSpan(0,0,0,0,100);
                testWatchers[0].Enabled = true;

                testWatchers[1] = new MemoryWatcher<double>(vitalityFragmentPointers[1]);
                testWatchers[1].OnChanged += (oldVal, newVal) =>
                {
                    if (newVal == 1)
                    {
                        removeItem();
                        newItem(52);
                    }
                };
                testWatchers[1].UpdateInterval = new TimeSpan(0, 0, 0, 0, 100);
                testWatchers[1].Enabled = true;

                testWatchers[2] = new MemoryWatcher<double>(ivoryBugPointers[1]);
                testWatchers[2].OnChanged += (oldVal, newVal) =>
                {
                    if (newVal == 1)
                    {
                        removeItem();
                        newItem(54);
                    }
                };
                testWatchers[2].UpdateInterval = new TimeSpan(0, 0, 0, 0, 100);
                testWatchers[2].Enabled = true;


                testWatchers[3] = new MemoryWatcher<double>(vitalityFragmentPointers[2]);
                testWatchers[3].OnChanged += (oldVal, newVal) =>
                {
                    if (newVal == 1)
                    {
                        removeItem();
                        newItem(34);
                    }
                };
                testWatchers[3].UpdateInterval = new TimeSpan(0, 0, 0, 0, 100);
                testWatchers[3].Enabled = true;

                testWatchers[4] = new MemoryWatcher<double>(noRequirementPickupPointers[0]);
                testWatchers[4].OnChanged += (oldVal, newVal) =>
                {
                    if (newVal == 1)
                    {
                        Debug.WriteLine("Picked up bellflower");
                        removeItem();
                        newItem(34);
                    }
                };
                testWatchers[4].UpdateInterval = new TimeSpan(0, 0, 0, 0, 100);
                testWatchers[4].Enabled = true;


                testWatchers[5] = new MemoryWatcher<double>(bossItemPointers[0]);
                testWatchers[5].OnChanged += (oldVal, newVal) =>
                {
                    if (newVal == 1)
                    {
                        removeItem();
                        newItem(4,3);
                    }
                };
                testWatchers[5].UpdateInterval = new TimeSpan(0, 0, 0, 0, 100);
                testWatchers[5].Enabled = true;

                randomizerRunning = true;
                //Setup magic here!
            }
        }

        private void newItem(int id, int addCharges = 0)
        {
            
            if(id == 34)
            {
                //Ivory Bug
                addIvoryBug();
            }
            else if (53 >= id && id >= 50)
            {
                //Crest Fragment
                addCrestFragment(id);
            }
            else if (id == 54)
            {
                //Special id for vitality fragments
                addVitalityFragment();
            }
            else if (id == 4 && id == 14 && id == 17)
            {
                addChargeItem(id, addCharges);
            }
            else
            {
                addItem(id);
            }
        }

        private void removeItem()
        {
            UpdateItemWatchers();

            if (removeItemWatchers[0].Changed)
            {
                removeChargeItem(17, removeItemWatchers[0].Current - removeItemWatchers[0].Old);
            }
            else if (removeItemWatchers[1].Changed)
            {
                removeChargeItem(4, removeItemWatchers[1].Current - removeItemWatchers[1].Old);
            }
            else if (removeItemWatchers[2].Changed)
            {
                removeChargeItem(14, removeItemWatchers[2].Current - removeItemWatchers[2].Old);
            }
            else if (removeItemWatchers[3].Changed)
            {
                removeIvoryBug();
            }
            else if (removeItemWatchers[4].Changed)
            {
                removeCrestFragment();
            }
            else if (removeItemWatchers[5].Changed)
            {
                removeVitalityFragment();
            }
            else
            {
                removeLastItem();
            }
        }

        private void addChargeItem(int id, double charges)
        {
            double currentCharges;
            IntPtr maxValuePointer, saveValuePtr;
            switch (id)
            {
                case 4:
                    //Bellflower
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    break;
                case 14:
                    //Passiflora
                    maxValuePointer = passifloraMaxValuePointer;
                    saveValuePtr = passifloraSaveValuePointer;
                    break;
                case 17:
                    //Tainted Missive
                    maxValuePointer = taintedMissiveMaxValuePointer;
                    saveValuePtr = taintedMissiveSaveValuePointer;
                    break;
                default:
                    //Defalut to bellflower, should probably be an error instead
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    break;
            }
            currentCharges = gameProc.ReadValue<double>(maxValuePointer);
            if (currentCharges == 0)
            {
                addItem(id);
            }
            gameProc.WriteValue<double>(maxValuePointer, charges + currentCharges);
            gameProc.WriteValue<double>(saveValuePtr, charges + currentCharges);
        }

        private void removeChargeItem(int id, double charges)
        {
            double currentCharges;
            IntPtr maxValuePointer, saveValuePtr;
            switch (id)
            {
                case 4:
                    //Bellflower
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    break;
                case 14:
                    //Passiflora
                    maxValuePointer = passifloraMaxValuePointer;
                    saveValuePtr = passifloraSaveValuePointer;
                    break;
                case 17:
                    //Tainted Missive
                    maxValuePointer = taintedMissiveMaxValuePointer;
                    saveValuePtr = taintedMissiveSaveValuePointer;
                    break;
                default:
                    //Default to bellflower, should probably handle this with an error or something
                    maxValuePointer = bellflowerMaxValuePointer;
                    saveValuePtr = bellflowerSaveValuePointer;
                    break;
            }
            currentCharges = gameProc.ReadValue<double>(maxValuePointer);
            if (currentCharges - charges == 0)
            {
                removeLastItem();
            }
            gameProc.WriteValue<double>(maxValuePointer, charges - currentCharges);
            gameProc.WriteValue<double>(saveValuePtr, charges - currentCharges);
        }

        private void addItem(int id)
        {
            //To add an item: Increase total item counter by one, increase the category of item given by 1
            //and set the inventory value for the next item slot to the id of the item given
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            gameProc.WriteValue<int>(totalItemsPointer, (int)totalItemAmount + 1);
            gameProc.WriteValue<double>(IntPtr.Add(inventoryItemsStartPointer, 0x10 * totalItemAmount), id);

            if (activeItemIDs.Contains(id))
            {
                var activeItemAmount = gameProc.ReadValue<double>(activeItemsPointer);
                gameProc.WriteValue<double>(activeItemsPointer, (double)activeItemAmount + 1);
            }
            else if (passiveItemIDs.Contains(id))
            {
                var passiveItemAmount = gameProc.ReadValue<double>(passiveItemsPointer);
                gameProc.WriteValue<double>(passiveItemsPointer, (double)passiveItemAmount + 1);
            }
            else if (keyItemIDs.Contains(id))
            {
                var keyItemAmount = gameProc.ReadValue<double>(keyItemsPointer);
                gameProc.WriteValue<double>(keyItemsPointer, (double)keyItemAmount + 1);
            } 
        }

        private void removeLastItem()
        {
            //To remove last item, decrease total item counter by one and the corresponding 
            var totalItemAmount = gameProc.ReadValue<int>(totalItemsPointer);
            var lastItemID = gameProc.ReadValue<double>(IntPtr.Add(inventoryItemsStartPointer, 0x10 * totalItemAmount));
            if (activeItemIDs.Contains((int)lastItemID))
            {
                var activeItemAmount = gameProc.ReadValue<double>(activeItemsPointer);
                gameProc.WriteValue<double>(activeItemsPointer, (double)activeItemAmount - 1);
            }
            else if (passiveItemIDs.Contains((int)lastItemID))
            {
                var passiveItemAmount = gameProc.ReadValue<double>(passiveItemsPointer);
                gameProc.WriteValue<double>(passiveItemsPointer, (double)passiveItemAmount - 1);
            }
            else if (keyItemIDs.Contains((int)lastItemID))
            {
                var keyItemAmount = gameProc.ReadValue<double>(keyItemsPointer);
                gameProc.WriteValue<double>(keyItemsPointer, (double)keyItemAmount - 1);
            }
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
                addItem(34);
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

                foreach (var watcher in testWatchers)
                {
                    watcher.Update(gameProc);
                }
                UpdateItemWatchers();
                        
            }

            //do update stuff here!
            if (invalidator != null)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        private void UpdateItemWatchers()
        {
            foreach (var watcher in removeItemWatchers)
            {
                watcher.Update(gameProc);
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
                switch (game[0].MainModule.ModuleMemorySize)
                {
                    case 39690240:
                        //version 1.05b
                        gameProc = game[0];
                        Debug.WriteLine("Setting up pointers");
                        #region setting up IntPtrs
                        karstShopItemsPointers = new IntPtr[3];
                        //Crystal Seed
                        karstShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4}).Deref<Int32>(gameProc), 0xcf0);
                        //??
                        karstShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcb0);
                        //??
                        karstShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd50);

                        graveShopItemsPointers = new IntPtr[2];
                        //Lamp?
                        graveShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xdd0);
                        //Necklace of Sacrifice
                        graveShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc70);

                        parkShopItemsPointers = new IntPtr[1];
                        //Dull Pearl?
                        parkShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x50);

                        cinderShopItemsPointers = new IntPtr[2];
                        //??
                        cinderShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x80);
                        //??
                        cinderShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd50);

                        spiderShopItemsPointers = new IntPtr[3];
                        //??
                        spiderShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xeb0);
                        //??
                        spiderShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xd50);
                        //??
                        spiderShopItemsPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcd0);

                        monasteryShopItemsPointers = new IntPtr[1];
                        //??
                        monasteryShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xcb0);

                        pinaShopItemsPointers = new IntPtr[3];
                        //??
                        pinaShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xe00);
                        //??
                        pinaShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xea0);
                        //??
                        pinaShopItemsPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xb0);

                        bellflowerMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x14,0x0 }).Deref<Int32>(gameProc), 0x1c0);
                        bellflowerSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3f0);
                        taintedMissiveMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x6a0);
                        taintedMissiveSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x400);
                        passifloraMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x14, 0x0 }).Deref<Int32>(gameProc), 0x580);
                        passifloraSaveValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0,0x4,0x60,0x4,0x4 }).Deref<Int32>(gameProc), 0x9d0);

                        difficultyPointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230C440, new int[] { 0x0,0x4,0x60,0x4,0x4 }).Deref<Int32>(gameProc),0x630);
                        maxHealthPointer = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xa0);

                        vitalityFragmentCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xAE0);
                        vitalityFragmentPointers = new IntPtr[17];
                        //0 = Grove Cat Room
                        vitalityFragmentPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x940);
                        //1 = Grove behind first hidden wall
                        vitalityFragmentPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x150);
                        //2 = Grove behind second hidden wall
                        vitalityFragmentPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x190);
                        //3 = Frore Ciele
                        vitalityFragmentPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1C0);
                        //4 = Karst City Dog Fragment
                        vitalityFragmentPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x170);
                        //5 = Karst City Boulder Fragment
                        vitalityFragmentPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1B0);
                        //6 = Sub Graves
                        vitalityFragmentPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x180);
                        //7 = Memorial Park near Cinder Chambers
                        vitalityFragmentPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8C0);
                        //8 = Cinder Chambers bottom text
                        vitalityFragmentPointers[8] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc),0x8D0);
                        //9 = Cinder Chambers Lever Room
                        vitalityFragmentPointers[9] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x1A0);
                        //10 = Monastery near Cat
                        vitalityFragmentPointers[10] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x160);
                        //11 = Pina Fragment 1
                        vitalityFragmentPointers[11] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8E0);
                        //12 = Pina Fragment Bellroom
                        vitalityFragmentPointers[12] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x8F0);
                        //13 = Pina Fragment Bakaroom
                        vitalityFragmentPointers[13] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x900);
                        //14 = Castle after bridge 1
                        vitalityFragmentPointers[14] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x910);
                        //15 = Castle Far Right
                        vitalityFragmentPointers[15] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x930);
                        //16 = Castle post Cath
                        vitalityFragmentPointers[16] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x920);

                        noRequirementPickupPointers = new IntPtr[30]; //not sure if 30 is how many there are, we'll see ^^
                        //0 = Grove Bellflower
                        noRequirementPickupPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0,0x4,0x60,0x4,0x4 }).Deref<Int32>(gameProc), 0xF0);
                        //1 = Astral Charm
                        noRequirementPickupPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4,0x4 }).Deref<Int32>(gameProc),0x100);
                        //2 = Karst Bellflower
                        noRequirementPickupPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x410);
                        //3 = Magnet Stone
                        noRequirementPickupPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x430);
                        //4 = Garden Key
                        noRequirementPickupPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x700);
                        //5 = Cinder Key
                        noRequirementPickupPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9B0);
                        //6 = Monastery Key
                        noRequirementPickupPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x260);
                        //7 = Tainted Missive Pickup
                        noRequirementPickupPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x420);

                        bossItemPointers = new IntPtr[8];
                        //0 = Edea's Pearl
                        bossItemPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x140);
                        //1 = Bakman Patch
                        bossItemPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x450);
                        //2 = Sparse Thread
                        bossItemPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7D0);
                        //3 = Torn Branch
                        bossItemPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x580);
                        //4 = Pocket Incensory
                        bossItemPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x9F0);
                        //5 = Tainted Missive
                        bossItemPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x270);
                        //6 = Bloodstained Tissue
                        bossItemPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6B0);
                        //7 = Heavy Arrows
                        bossItemPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);

                        blessedCharmPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x840);
                        rottenShroomPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x500);
                        greenLeafPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x600);
                        dirtyShroomPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x480);
                        catSpherePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x720);
                        bellFlowerParkPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7E0);
                        softTissuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x570);
                        blackSachetPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA00);
                        sealedWindPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0xA90);
                        castleBellflowerPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x6D0);
                        passifloraCathPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x690);

                        ivoryBugCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3C0);
                        ivoryBugPointers = new IntPtr[20];
                        //0 = Grove room 2nd switch
                        ivoryBugPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x360);
                        //1 = Grove 2nd bell
                        ivoryBugPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x280);
                        //2 = Grove 3rd bug
                        ivoryBugPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x290);
                        //3 = Frore Ciele
                        ivoryBugPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x340);
                        //4 = Karst City Bridge
                        ivoryBugPointers[4] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3A0);
                        //5 = Karst Knight
                        ivoryBugPointers[5] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2A0);
                        //6 = Sub Graves
                        ivoryBugPointers[6] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2E0);
                        //7 = After Lubella 2 bug 1
                        ivoryBugPointers[7] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2C0);
                        //8 = After Lubella 2 bug 2
                        ivoryBugPointers[8] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2D0);
                        //9 = Park Bug
                        ivoryBugPointers[9] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x3B0);
                        //10 = Chambers Top Bug
                        ivoryBugPointers[10] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x300);
                        //11 = Chambers Cinder Key Room
                        ivoryBugPointers[11] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x310);
                        //12 = Chambers Hidden Ceiling
                        ivoryBugPointers[12] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2F0);
                        //13 = Pre Fennel no Cat
                        ivoryBugPointers[13] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x2B0);
                        //14 = Pre Fennel Cat
                        ivoryBugPointers[14] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x390);
                        //15 = Pina Skeleton room
                        ivoryBugPointers[15] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x320);
                        //16 = Pina Fairy Code room
                        ivoryBugPointers[16] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x350);
                        //17 = Pina Elevator room
                        ivoryBugPointers[17] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x330);
                        //18 = Early Castle
                        ivoryBugPointers[18] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x370);
                        //19 = Yeet Room
                        ivoryBugPointers[19] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x380);

                        crestFragmentCountPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5f0);
                        crestFragmentPointers = new IntPtr[4];
                        //0 = Warp Fragment
                        crestFragmentPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5C0);
                        //1 = Fast Charge
                        crestFragmentPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5B0);
                        //2 = Dash
                        crestFragmentPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5D0);
                        //3 = Charge Level
                        crestFragmentPointers[3] = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] {0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x5A0);

                        oneDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x780);
                        fiveDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x790);
                        tenDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7A0);
                        fifteenDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7B0);
                        twentyDeliveredPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4 }).Deref<Int32>(gameProc), 0x7C0);

                        totalItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x230b11c,new int[] { 0x1a4}).Deref<Int32>(gameProc),0x4);
                        activeItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc30);
                        passiveItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                        keyItemsPointer = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1100);
                        inventoryItemsStartPointer = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4, 0xC }).Deref<int>(gameProc);
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
