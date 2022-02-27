using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    partial class MomodoraRandomizerSettings : UserControl
    {

        public bool VitalityFragmentsEnabled { get; set; }
        public bool IvoryBugsEnabled { get; set; }
        public bool HardModeEnabled { get; set; }
        public bool RandomSeed { get; set; }

        public Color TextColor { get; set; }
        public Color OutlineColor { get; set; }
        public Color ShadowColor { get; set; }
        public bool OverrideTextColor { get; set; }

        public string TextFontString => SettingsHelper.FormatFont(TextFont);
        public Font TextFont { get; set; }
        public bool OverrideTextFont { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public bool logEnabled { get; set; }
        public bool showLogWarning { get; set; }

        public MomodoraRandomizerSettings()
        {
            InitializeComponent();
            VitalityFragmentsEnabled = true;
            IvoryBugsEnabled = true;
            HardModeEnabled = false;
            RandomSeed = true;
            TextFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
            OverrideTextFont = false;
            TextColor = Color.FromArgb(255, 255, 255, 255);
            OutlineColor = Color.FromArgb(255, 255, 255, 255);
            ShadowColor = Color.FromArgb(0, 255, 255, 255);
            OverrideTextColor = false;
            BackgroundColor = Color.FromArgb(0, 255, 255, 255);
            BackgroundColor2 = Color.FromArgb(0, 255, 255, 255);
            BackgroundGradient = GradientType.Plain;
            logEnabled = false;
            showLogWarning = true;

            chkVitality.DataBindings.Add("Checked", this, "VitalityFragmentsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            chkIvoryBugs.DataBindings.Add("Checked", this, "IvoryBugsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            chkHard.DataBindings.Add("Checked", this, "HardModeEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRandom.DataBindings.Add("Checked", this, "RandomSeed", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFont.DataBindings.Add("Checked", this, "OverrideTextFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblFont.DataBindings.Add("Text", this, "TextFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnOutlineColor.DataBindings.Add("BackColor", this, "OutlineColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnShadowColor.DataBindings.Add("BackColor", this, "ShadowColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLog.DataBindings.Add("Checked", this, "logEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void MomodoraRandomizerSettings_Load(object sender, EventArgs e)
        {
            chkColor_CheckedChanged(null, null);
            chkFont_CheckedChanged(null, null);
            UseRandomSeed_CheckedChanged(null, null);

            if (showLogWarning == true)
            {
                chkLog.Checked = true;
            }
        }

        private void chkColor_CheckedChanged(object sender, EventArgs e)
        {
            label4.Enabled = btnOutlineColor.Enabled = label2.Enabled = btnShadowColor.Enabled = label3.Enabled = btnTextColor.Enabled = chkColor.Checked;
        }

        private void chkFont_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = lblFont.Enabled = btnFont.Enabled = chkFont.Checked;
        }

        private void mbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            if (version >= new Version(1, 2))
            {
                TextFont = SettingsHelper.GetFontFromElement(element["TextFont"]);
                if (version >= new Version(1, 3))
                {
                    OverrideTextFont = SettingsHelper.ParseBool(element["OverrideTextFont"]);
                }
                else
                    OverrideTextFont = !SettingsHelper.ParseBool(element["UseLayoutSettingsFont"]);
            }
            else
            {
                TextFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
                OverrideTextFont = false;
            }

            VitalityFragmentsEnabled = SettingsHelper.ParseBool(element["VitalityFragmentsEnabled"], true);
            IvoryBugsEnabled = SettingsHelper.ParseBool(element["IvoryBugsEnabled"], true);
            HardModeEnabled = SettingsHelper.ParseBool(element["HardModeEnabled"], false);
            RandomSeed = SettingsHelper.ParseBool(element["RandomSeed"], true);
            TextColor = SettingsHelper.ParseColor(element["TextColor"], Color.FromArgb(255, 255, 255, 255));
            OutlineColor = SettingsHelper.ParseColor(element["OutlineColor"], Color.FromArgb(255, 255, 255, 255));
            ShadowColor = SettingsHelper.ParseColor(element["ShadowColor"], Color.FromArgb(0, 255, 255, 255));
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"], false);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.FromArgb(0, 0, 0, 0));
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.FromArgb(0, 0, 0, 0));
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], GradientType.Plain.ToString());
            this.textSeed.Text = SettingsHelper.ParseString(element["textSeed"], "");
            logEnabled = SettingsHelper.ParseBool(element["logEnabled"], false);
            showLogWarning = SettingsHelper.ParseBool(element["showLogWarning"], true);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            SettingsHelper.CreateSetting(document, parent, "VitalityFragmentsEnabled", VitalityFragmentsEnabled);
            SettingsHelper.CreateSetting(document, parent, "IvoryBugsEnabled", IvoryBugsEnabled);
            SettingsHelper.CreateSetting(document, parent, "HardModeEnabled", HardModeEnabled);
            SettingsHelper.CreateSetting(document, parent, "RandomSeed", RandomSeed);
            SettingsHelper.CreateSetting(document, parent, "OverrideTextFont", OverrideTextFont);
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor);
            SettingsHelper.CreateSetting(document, parent, "TextFont", TextFont);
            SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor);
            SettingsHelper.CreateSetting(document, parent, "OutlineColor", OutlineColor);
            SettingsHelper.CreateSetting(document, parent, "ShadowColor", ShadowColor);
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor);
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2);
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient);
            SettingsHelper.CreateSetting(document, parent, "textSeed", this.textSeed.Text);
            SettingsHelper.CreateSetting(document, parent, "logEnabled", logEnabled);
            SettingsHelper.CreateSetting(document, parent, "showLogWarning", showLogWarning);
            return parent;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(TextFont, 7, 20);
            dialog.FontChanged += (s, ev) => TextFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblFont.Text = TextFontString;
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void UseRandomSeed_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkRandom.Checked)
            {
                textSeed.Enabled = true;
            }

            else
            {
                textSeed.Enabled = false;
            }
        }

        public void seed_set(int seed)
        {
            this.textSeed.Text = seed.ToString();
        }

        public string seed_get()
        {
            return this.textSeed.Text;
        }

        private void btnSeed_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            seed_set(random.Next());
        }

        private void chkHard_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHard.Checked)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                                "Key Items can drop from boss fights." +
                                "\n\nTip: you can enter Whiteleaf Memorial Park through Cinder Chambers using Backman Patch\n\n" +
                                "Enable?",
                                "Hard Mode",
                                MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    chkHard.Checked = true;
                }
                else
                    chkHard.Checked = false;
            }
        }

        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLog.Checked)
            {
                LogWarning();
            }
        }

        private void LogWarning()
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                    "With this option enabled a log file will be generated upon stoping the timer.\n" +
                    "This file will be located in the \"Components\" folder inside LiveSplit with" +
                    " the name MomodoraRandomizer.log. Each subsequent run will overwrite this file.\n" +
                    "This file will contain important events that occur during a run but are only useful for debugging purposes.\n\n" +
                    "Enable?",
                    "Event logging system",
                    MessageBoxButtons.OKCancel);
            showLogWarning = false;
            if(dialogResult == DialogResult.OK)
                chkLog.Checked = true;
            else if(dialogResult == DialogResult.Cancel)
                chkLog.Checked = false;
        }

        private void btnClipboard_Click(object sender, EventArgs e)
        {
            if(textSeed.TextLength > 0)
            {
                Clipboard.SetText(seed_get());
            }
        }
    }
}
