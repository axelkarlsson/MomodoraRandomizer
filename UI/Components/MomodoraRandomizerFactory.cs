using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    class MomodoraRandomizerFactory : IComponentFactory
    {
        public string ComponentName => "Momodora RUtM Randomizer";

        public string Description => "A fun and exciting randomizer for Momodora: Reverie Under the Moonlight";

        public ComponentCategory Category => ComponentCategory.Other;

        public string UpdateName => ComponentName;

        public string XMLURL => UpdateURL + "update.MomodoraRandomizer.xml";

        public string UpdateURL => "https://raw.githubusercontent.com/axelkarlsson/MomodoraRandomizer/main/Updates";

        public Version Version => Version.Parse("1.0.10");

        public IComponent Create(LiveSplitState state)
        {
            return new MomodoraRandomizer(state);
        }
    }
}
