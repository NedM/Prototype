using System;
using System.Collections.Generic;
using System.Text;

namespace Prototype
{
    public class ConvertedItem
    {
        public static ConvertedItem FromAlohaItem(AlohaItemTest alohaItem)
        {
            return new ConvertedItem(alohaItem.Name, alohaItem.Level);
        }

        public ConvertedItem(string name, int level)
        {
            this.Name = name;
            this.Level = level;
            this.SubItems = new List<ConvertedItem>();
        }

        public string Name { get; set; }
        public int Level { get; set; }
        public List<ConvertedItem> SubItems { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("[{0}] Name: {1}, SubItem Count: {2}.", Level, Name, SubItems.Count));

            foreach (var convertedItem in SubItems)
            {
                sb.AppendFormat("{0}{1}{2}", Environment.NewLine, string.Empty.PadLeft(convertedItem.Level, ' '), convertedItem);
            }

            return sb.ToString();
        }
    }
}
