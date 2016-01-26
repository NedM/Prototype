using System;

namespace Prototype
{
    public class MenuItem : IComparable
    {
        public MenuItem(string name, int level, bool isValid = true)
        {
            this.Name = name;
            this.Level = level;
            this.IsValid = isValid;
        }

        public bool IsValid { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0} Level: {1}", Name, Level);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is MenuItem))
            {
                throw new InvalidOperationException(string.Format("Cannot compare types {0} and {1}", this.GetType(), obj.GetType()));
            }

            return CompareTo(obj as MenuItem);
        }

        public int CompareTo(MenuItem otherItem)
        {
            if (null == otherItem)
            {
                return -1;
            }

            if (otherItem.Level == this.Level)
            {
                return 0;
            }

            return (otherItem.Level < this.Level) ? 1 : -1;
        }
    }
}
