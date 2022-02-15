using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.UI.Components
{
    class MomodoraRandomizerFactory : IComponentFactory
    {
        public string ComponentName => "Momodora RUtM Randomizer";

        public string Description => "A fun and exciting randomizer for Momodora: Reverie Under the Moonlight";

        public ComponentCategory Category => ComponentCategory.Other;

        public string UpdateName => ComponentName;

        public string XMLURL => UpdateURL + "blob/main/update.MomodoraRandomizer.xml";

        public string UpdateURL => "https://github.com/axelkarlsson/MomodoraRandomizer/";

        public Version Version => Version.Parse("1.0.0");

        public IComponent Create(LiveSplitState state)
        {
            return new MomodoraRandomizer(state);
        }
    }
}
