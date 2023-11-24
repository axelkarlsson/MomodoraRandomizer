using System.Collections.Generic;

namespace LiveSplit.UI.Components
{
    internal class Items
    {
        public static List<Items> ItemsList;
        private static int CurrentId = 0;

        private ItemValue ItemValue { get; }
        public int Id { get; }

        public string Name { get; }
        public string Effect { get; }
        public string Description { get; }

        public Items ItemReference { get; set; }
    }
}