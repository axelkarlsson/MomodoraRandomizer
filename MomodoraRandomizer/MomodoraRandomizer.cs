﻿using System;
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
        DeepPointer bellflowerSaveValue;
        DeepPointer bellflowerMaxValue;
        DeepPointer taintedMissiveSaveValue;
        DeepPointer taintedMissiveMaxValue;
        DeepPointer passifloraSaveValue;
        DeepPointer passifloraMaxValue;
        #endregion

        #region shop stock pointers
        DeepPointer[] karstShopItems;
        DeepPointer[] graveShopItems;
        DeepPointer[] parkShopItems;
        DeepPointer[] cinderShopItems;
        DeepPointer[] spiderShopItems;
        DeepPointer[] monasteryShopItems;
        DeepPointer[] pinaShopItems;
        #endregion

        #region ivory bug pointers
        DeepPointer ivoryBugCount;
        DeepPointer[] ivoryBugs;
        DeepPointer oneDelivered;
        DeepPointer fiveDelivered;
        DeepPointer tenDelivered;
        DeepPointer fifteenDelivered;
        DeepPointer twentyDelivered;
        #endregion

        #region vitality fragment pointers
        DeepPointer vitalityFragmentCount;
        DeepPointer[] vitalityFragments;
        #endregion

        #region crest fragment pointers
        DeepPointer crestFragmentCount;
        DeepPointer[] crestFragments;
        #endregion

        #region item pickup pointers
        DeepPointer[] noRequirementPickups;
        DeepPointer[] crestFragmentsRequired;
        DeepPointer blessedCharm;
        DeepPointer rottenShroom;
        DeepPointer greenLeaf;
        DeepPointer softTissue;
        DeepPointer catSphere;
        DeepPointer bellFlowerPark;
        DeepPointer dirtyShroom;
        #endregion

        #region boss items
        DeepPointer[] bossItems;
        #endregion

        #region inventory pointers
        IntPtr activeItems;
        IntPtr passiveItems;
        IntPtr keyItems;
        IntPtr totalItems;
        IntPtr inventoryItems;
        #endregion

        #region misc pointers
        DeepPointer difficultyPointer;
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
            var totalItemAmount = gameProc.ReadValue<int>(totalItems);
            gameProc.WriteValue<int>(totalItems, (int)totalItemAmount + 1);
            gameProc.WriteValue<double>(IntPtr.Add(inventoryItems, 0x10 * totalItemAmount), id);

            if (activeItemIDs.Contains(id))
            {
                var activeItemAmount = gameProc.ReadValue<double>(activeItems);
                gameProc.WriteValue<double>(activeItems, (double)activeItemAmount + 1);
            }
            else if (passiveItemIDs.Contains(id))
            {
                var passiveItemAmount = gameProc.ReadValue<double>(passiveItems);
                gameProc.WriteValue<double>(passiveItems, (double)passiveItemAmount + 1);
            }
            else if (keyItemIDs.Contains(id))
            {
                var keyItemAmount = gameProc.ReadValue<double>(keyItems);
                gameProc.WriteValue<double>(keyItems, (double)keyItemAmount + 1);
            } 
        }

        private void removeLastItem()
        {
            //To remove last item, decrease total item counter by one and the corresponding 
            var totalItemAmount = gameProc.ReadValue<int>(totalItems);
            var lastItemID = gameProc.ReadValue<double>(IntPtr.Add(inventoryItems, 0x10 * totalItemAmount));
            if (activeItemIDs.Contains((int)lastItemID))
            {
                var activeItemAmount = gameProc.ReadValue<double>(activeItems);
                gameProc.WriteValue<double>(activeItems, (double)activeItemAmount - 1);
            }
            else if (passiveItemIDs.Contains((int)lastItemID))
            {
                var passiveItemAmount = gameProc.ReadValue<double>(passiveItems);
                gameProc.WriteValue<double>(passiveItems, (double)passiveItemAmount - 1);
            }
            else if (keyItemIDs.Contains((int)lastItemID))
            {
                var keyItemAmount = gameProc.ReadValue<double>(keyItems);
                gameProc.WriteValue<double>(keyItems, (double)keyItemAmount - 1);
            }
            gameProc.WriteValue<int>(totalItems, (int)totalItemAmount - 1);
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
                        /* Example of writing to memory
                        difficultyPointer = new DeepPointer(0x230C440,new int[] {0x0,0x4, 0x60,0x4,0x4,0x630 });
                        crestFragmentCount = new DeepPointer(0x230C440, new int[] { 0x0, 0x4, 0x60, 0x4, 0x4});
                        IntPtr test = (IntPtr)crestFragmentCount.Deref<Int32>(gameProc);
                        Debug.WriteLine(test.ToString("X"));
                        test = IntPtr.Add(test,0x5f0);
                        Debug.WriteLine(test.ToString("X"));
                        gameProc.WriteValue(test, (double)10);
                        */
                        //RandomizerLabel.Text = difficultyPointer.Deref<double>(gameProc).ToString();
                        totalItems = IntPtr.Add((IntPtr)new DeepPointer(0x230b11c,new int[] { 0x1a4}).Deref<Int32>(gameProc),0x4);
                        activeItems = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc30);
                        passiveItems = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0xc40);
                        keyItems = IntPtr.Add((IntPtr)new DeepPointer(0x2304ce8, new int[] { 0x4 }).Deref<Int32>(gameProc), 0x1100);
                        inventoryItems = (IntPtr)new DeepPointer(0x230b11c, new int[] { 0x1a4, 0xC }).Deref<int>(gameProc);
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
