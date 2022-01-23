using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class MomodoraRandomizerSettings : UserControl
    {

        public MomodoraRandomizerSettings()
        {
            InitializeComponent();
            seed.Enabled = false;
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;
        }


        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            SettingsHelper.CreateSetting(document, parent, "setting1", 0);
            return parent;
        }

        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            //var = SettingsHelper.ParseBool(element["label"]);
        }

        //Allow or not the user to input a seed
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                seed.Enabled = true;
            }

            else
            {
                seed.Enabled = false;
            }
        }

        //Change status of variable to include Ivory Bugs
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        //Check status of the checkbox2 to include Ivory Bugs
        private Boolean checkBox2_Get()
        {
            return checkBox2.Checked;
        }

        //Change status of variable to include Health Fragments
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
        }

        //Check status of the checkbox3 to include Health Fragments
        private Boolean checkBox3_Get()
        {
            return checkBox3.Checked;
        }

        //Change display value of seed
        public void seed_set(int seed)
        {
            this.seed.Text = seed.ToString();
        }

        //Return current value of seed
        public string seed_get()
        {
            return this.seed.Text;
        }

        //Return if random seed is enabled
        public Boolean seed_enabled()
        {
            return this.checkBox1.Checked;
        }

        //Generate random seed
        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            seed_set(random.Next());
        }
    }
}
