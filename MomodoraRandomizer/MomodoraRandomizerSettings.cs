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
    }

}
