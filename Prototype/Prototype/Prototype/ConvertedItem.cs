using System;
using System.Collections.Generic;
using System.Text;

namespace Prototype
{
    public class ConvertedItem
    {
        public static bool operator ==(ConvertedItem left, ConvertedItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConvertedItem left, ConvertedItem right)
        {
            return !Equals(left, right);
        }

        public static ConvertedItem FromMenuItem(MenuItem alohaItem)
        {
            return new ConvertedItem(alohaItem.Name, alohaItem.Level);
        }

        public ConvertedItem(string name, int level)
            : this(name, level, new List<ConvertedItem>())
        {
        }

        public ConvertedItem(string name, int level, List<ConvertedItem> subItems)
        {
            this.Name = name;
            this.Level = level;
            this.SubItems = subItems;
        }

        public string Name { get; set; }
        public int Level { get; set; }
        public List<ConvertedItem> SubItems { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("[{0}] Name: {1}, SubItem Count: {2}.", Level, Name, SubItems.Count));

            foreach (var convertedItem in SubItems)
            {
                sb.AppendFormat("{0}{1}{2}", Environment.NewLine, string.Empty.PadLeft(convertedItem.Level, ' '), convertedItem);
            }

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ConvertedItem) obj);
        }

        protected bool Equals(ConvertedItem other)
        {
            return string.Equals(Name, other.Name) &&
                   Level == other.Level &&
                   Equals(SubItems, other.SubItems);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Level;
                hashCode = (hashCode * 397) ^ (SubItems != null ? SubItems.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
