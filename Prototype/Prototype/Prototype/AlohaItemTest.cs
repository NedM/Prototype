using System;

namespace Prototype
{
    public class AlohaItemTest : IComparable
    {
        public AlohaItemTest(string name, int level, bool isValid = true)
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
            if (!(obj is AlohaItemTest))
            {
                throw new InvalidOperationException(string.Format("Cannot compare types {0} and {1}", this.GetType(), obj.GetType()));
            }

            return CompareTo(obj as AlohaItemTest);
        }

        public int CompareTo(AlohaItemTest otherItem)
        {
            if (null == otherItem)
            {
                return -1;
            }

            if (otherItem.Level == this.Level)
            {
                return 0;
            }
            else if (otherItem.Level < this.Level)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
