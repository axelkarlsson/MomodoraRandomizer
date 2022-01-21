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
        IntPtr[] crestFragmentsRequiredPointers;
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

        public MomodoraRandomizer(LiveSplitState state)
        {
            state.OnStart += onStart;
            this.state = state;
            RandomizerLabel = new SimpleLabel("Randomizer Go!");
            settingsControl = new MomodoraRandomizerSettings();
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


                //Setup magic here!
            }
        }

        private void giveItem(int id)
        {
            //To add an item: Increase total item countery by one, increase the category of item given by 1
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
            //do update stuff here!
            if (invalidator != null)
            {
                invalidator.Invalidate(0, 0, width, height);
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

                        #region setting up IntPtrs
                        karstShopItemsPointers = new IntPtr[3];
                        //Crystal Seed
                        karstShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xcf0}).Deref<Int32>(gameProc), 0x4);
                        //??
                        karstShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xcb0 }).Deref<Int32>(gameProc), 0x4);
                        //??
                        karstShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xd50 }).Deref<Int32>(gameProc), 0x4);

                        graveShopItemsPointers = new IntPtr[2];
                        //Lamp?
                        graveShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xdd0 }).Deref<Int32>(gameProc), 0x4);
                        //Necklace of Sacrifice
                        graveShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xc70 }).Deref<Int32>(gameProc), 0x4);

                        parkShopItemsPointers = new IntPtr[1];
                        //Dull Pearl?
                        parkShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x50 }).Deref<Int32>(gameProc), 0x4);

                        cinderShopItemsPointers = new IntPtr[2];
                        //??
                        cinderShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0x80 }).Deref<Int32>(gameProc), 0x4);
                        //??
                        cinderShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xd50 }).Deref<Int32>(gameProc), 0x4);

                        spiderShopItemsPointers = new IntPtr[3];
                        //??
                        spiderShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xeb0 }).Deref<Int32>(gameProc), 0x4);
                        //??
                        spiderShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xd50 }).Deref<Int32>(gameProc), 0x4);
                        //??
                        spiderShopItemsPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xcd0 }).Deref<Int32>(gameProc), 0x4);

                        monasteryShopItemsPointers = new IntPtr[1];
                        //??
                        monasteryShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xcb0 }).Deref<Int32>(gameProc), 0x4);

                        pinaShopItemsPointers = new IntPtr[3];
                        //??
                        pinaShopItemsPointers[0] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xe00 }).Deref<Int32>(gameProc), 0x4);
                        //??
                        pinaShopItemsPointers[1] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xea0 }).Deref<Int32>(gameProc), 0x4);
                        //??
                        pinaShopItemsPointers[2] = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xb0 }).Deref<Int32>(gameProc), 0x4);

                        bellflowerMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x0230B134, new int[] { 0x1c0,0x0 }).Deref<Int32>(gameProc), 0x14);
                        bellflowerSaveValuePointer = (IntPtr)new DeepPointer(0x0D3B4CF8, new int[] { 0x3f0,0x4,0x4,0x60,0x4 }).Deref<Int32>(gameProc);
                        taintedMissiveMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x6a0, 0x0 }).Deref<Int32>(gameProc), 0x14);
                        taintedMissiveSaveValuePointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x400, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        passifloraMaxValuePointer = IntPtr.Add((IntPtr)new DeepPointer(0x230C440, new int[] { 0x580, 0x0 }).Deref<Int32>(gameProc), 0x14);
                        passifloraSaveValuePointer = (IntPtr)new DeepPointer(0x0D3B4CF8, new int[] { 0x9d0,0x4,0x4,0x60,0x4 }).Deref<Int32>(gameProc);

                        difficultyPointer = (IntPtr)new DeepPointer(0x0230C440, new int[] { 0x630,0x4,0x4,0x60,0x4 }).Deref<Int32>(gameProc);
                        maxHealthPointer = IntPtr.Add((IntPtr)new DeepPointer(0x02304CE8, new int[] { 0xA0 }).Deref<Int32>(gameProc), 0x4);
                        vitalityFragmentCountPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0xAE0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        vitalityFragmentPointers = new IntPtr[17];
                        //0 = Grove Cat Room
                        vitalityFragmentPointers[0] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x940, 0x4,0x4,0x60,0x4 }).Deref<Int32>(gameProc);
                        //1 = Grove behind first hidden wall
                        vitalityFragmentPointers[1] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x150, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //2 = Grove behind second hidden wall
                        vitalityFragmentPointers[2] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x190, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //3 = Frore Ciele
                        vitalityFragmentPointers[3] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x1C0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //4 = Karst City Dog Fragment
                        vitalityFragmentPointers[4] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x170, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //5 = Karst City Boulder Fragment
                        vitalityFragmentPointers[5] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x1B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //6 = Sub Graves
                        vitalityFragmentPointers[6] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x180, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //7 = Memorial Park near Cinder Chambers
                        vitalityFragmentPointers[7] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x180, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //8 = Cinder Chambers bottom text
                        vitalityFragmentPointers[8] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x8D0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //9 = Cinder Chambers Lever Room
                        vitalityFragmentPointers[9] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x1A0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //10 = Monastery near Cat
                        vitalityFragmentPointers[10] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x160, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //11 = Pina Fragment 1
                        vitalityFragmentPointers[11] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x8E0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //12 = Pina Fragment Bellroom
                        vitalityFragmentPointers[12] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x8F0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //13 = Pina Fragment Bakaroom
                        vitalityFragmentPointers[13] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x900, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //14 = Castle after bridge 1
                        vitalityFragmentPointers[14] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x910, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //15 = Castle Far Right
                        vitalityFragmentPointers[15] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x930, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //16 = Castle post Cath
                        vitalityFragmentPointers[16] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x920, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

                        noRequirementPickupPointers = new IntPtr[30]; //not sure if 30 is how many there are, we'll see ^^
                        //0 = Grove Bellflower
                        noRequirementPickupPointers[0] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0xF0, 0x4,0x4,0x60,0x4 }).Deref<Int32>(gameProc);
                        //1 = Astral Charm
                        noRequirementPickupPointers[1] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x100, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //2 = Karst Bellflower
                        noRequirementPickupPointers[2] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x410, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //3 = Magnet Stone
                        noRequirementPickupPointers[3] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x430, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //4 = Garden Key
                        noRequirementPickupPointers[4] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x700, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //5 = Cinder Key
                        noRequirementPickupPointers[5] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x9B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //6 = Monastery Key
                        noRequirementPickupPointers[6] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x260, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //7 = Tainted Missive Pickup
                        noRequirementPickupPointers[7] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x420, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

                        bossItemPointers = new IntPtr[8];
                        //0 = Edea's Pearl
                        bossItemPointers[0] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x140, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //1 = Bakman Patch
                        bossItemPointers[1] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x450, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //2 = Sparse Thread
                        bossItemPointers[2] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x450, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //3 = Torn Branch
                        bossItemPointers[3] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x580, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //4 = Pocket Incensory
                        bossItemPointers[4] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x9F0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //5 = Tainted Missive
                        bossItemPointers[5] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x270, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //6 = Bloodstained Tissue
                        bossItemPointers[6] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x6B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //7 = Heavy Arrows
                        bossItemPointers[7] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x670, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

                        blessedCharmPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x840, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        rottenShroomPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x500, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        greenLeafPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x600, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        dirtyShroomPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x480, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        catSpherePointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x720, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        bellFlowerParkPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x7E0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        softTissuePointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x570, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        blackSachetPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0xA00, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);      
                        sealedWindPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0xA90, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        castleBellflowerPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x6D0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        passifloraCathPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x690, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

                        ivoryBugCountPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x3C0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        ivoryBugPointers = new IntPtr[20];
                        //0 = Grove room 2nd switch
                        ivoryBugPointers[0] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x360, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //1 = Grove 2nd bell
                        ivoryBugPointers[1] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x280, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //2 = Grove 3rd bug
                        ivoryBugPointers[2] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x290, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //3 = Frore Ciele
                        ivoryBugPointers[3] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x340, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //4 = Karst City Bridge
                        ivoryBugPointers[4] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x3A0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //5 = Karst Knight
                        ivoryBugPointers[5] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x2A0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //6 = Sub Graves
                        ivoryBugPointers[6] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x2E0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //7 = After Lubella 2 bug 1
                        ivoryBugPointers[7] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x2C0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //8 = After Lubella 2 bug 2
                        ivoryBugPointers[8] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x2D0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //9 = Park Bug
                        ivoryBugPointers[9] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x3B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //10 = Chambers Top Bug
                        ivoryBugPointers[10] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x300, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //11 = Chambers Cinder Key Room
                        ivoryBugPointers[11] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x310, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //12 = Chambers Hidden Ceiling
                        ivoryBugPointers[12] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x2F0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //13 = Pre Fennel no Cat
                        ivoryBugPointers[13] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x2B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //14 = Pre Fennel Cat
                        ivoryBugPointers[14] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x390, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //15 = Pina Skeleton room
                        ivoryBugPointers[15] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x320, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //16 = Pina Fairy Code room
                        ivoryBugPointers[16] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x350, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //17 = Pina Elevator room
                        ivoryBugPointers[17] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x330, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //18 = Early Castle
                        ivoryBugPointers[18] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x370, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //19 = Yeet Room
                        ivoryBugPointers[19] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x380, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

                        crestFragmentCountPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x5f0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        crestFragmentPointers = new IntPtr[4];
                        //0 = Warp Fragment
                        crestFragmentPointers[0] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x5C0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //1 = Fast Charge
                        crestFragmentPointers[1] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x5B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //2 = Dash
                        crestFragmentPointers[2] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x5D0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        //3 = Charge Level
                        crestFragmentPointers[3] = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x5A0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

                        oneDeliveredPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x780, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        fiveDeliveredPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x790, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        tenDeliveredPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x7A0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        fifteenDeliveredPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x7B0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);
                        twentyDeliveredPointer = (IntPtr)new DeepPointer(0x230C440, new int[] { 0x7C0, 0x4, 0x4, 0x60, 0x4 }).Deref<Int32>(gameProc);

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
