using System;
using System.Collections.Generic;

namespace Prototype
{
    public class MicrosCheck
    {
        public struct Offsets
        {
            public const int Name = 0;
            public const int Level = 1;
            public const int IsValid = 2;
            public const int SomeOtherOffset = 3;
        }

        public static List<ConvertedItem> BuildItemList(string[][] items)
        {
            if (null == items)
            {
                throw new ArgumentException("NULL item array!");
            }

            #region Method 1: 2 Pass method (reduce duplication of logic)
            //List<MenuItem> menuItems = new List<MenuItem>(items.Length);

            //for (int i = 0; i < items.Length; i++)
            //{
            //    menuItems.Add(new MenuItem(items[i][Offsets.Name],
            //                               int.Parse(items[i][Offsets.Level])));
            //}

            //return BuildItemList(menuItems);
            #endregion Method 1: 2 Pass method (reduce duplication of logic)

            #region Method 2: 1 Pass method (logic duplicated but no conversion to menu item list)

            List<ConvertedItem> convertedItems = new List<ConvertedItem>();

            ConvertedItem parent = null;

            for (int i = 0; i < items.Length; i++)
            {
                if (!Convert.ToBoolean(int.Parse(items[i][Offsets.IsValid])))
                {
                    continue;
                }

                ConvertedItem item = new ConvertedItem(items[i][Offsets.Name],
                                                       int.Parse(items[i][Offsets.Level]));

                if (null == parent || !IsCondiment(item))
                {
                    if (null != parent)
                    {
                        convertedItems.Add(parent);
                    }

                    parent = item;
                }
                else
                {
                    if (null == parent.SubItems)
                    {
                        parent.SubItems = new List<ConvertedItem>();
                    }

                    item.Level = Math.Min(item.Level, 1);  //Normalize item levels
                    parent.SubItems.Add(item);
                }
            }

            //Get the last parent into the list:
            convertedItems.Add(parent);

            return convertedItems;
            #endregion Method 2: 1 Pass method (logic duplicated but no conversion to menu item list)
        }

        public static List<ConvertedItem> BuildItemList(List<MenuItem> items)
        {
            if (null == items)
            {
                throw new ArgumentException("NULL item list!");
            }

            List<ConvertedItem> convertedItems = new List<ConvertedItem>();

            ConvertedItem parent = null;

            foreach (MenuItem mi in items)
            {
                if (!mi.IsValid)
                {
                    continue;
                }

                ConvertedItem item = ConvertedItem.FromMenuItem(mi);

                if (null == parent || !IsCondiment(item))
                {
                    if (null != parent)
                    {
                        convertedItems.Add(parent);
                    }

                    parent = item;
                }
                else
                {
                    if (null == parent.SubItems)
                    {
                        parent.SubItems = new List<ConvertedItem>();
                    }

                    item.Level = Math.Min(item.Level, 1);  //Normalize item levels
                    parent.SubItems.Add(item);
                }
            }

            //Get the last parent into the list:
            convertedItems.Add(parent);

            return convertedItems;
        } 

        public static string[][] ConvertToItems(string[] data, int startIndex = 0)
        {
            int numItems = int.Parse(data[startIndex]);
            int lengthOfData = data.Length - startIndex;
            int numFieldsPerItem = lengthOfData / (numItems + 1);

            string[][] jagged = new string[numItems][];

            for (int i = 0; i < numItems; i++)
            {
                jagged[i] = new string[numFieldsPerItem];
            }

            int row = 0;
            int column = 0;

            for (int i = startIndex + 1; i < data.Length; i++)
            {
                if (row >= numItems)
                {
                    column++;
                    row = 0;
                    continue;
                }

                jagged[row++][column] = data[i];
            }

            return jagged;
        }

        private static bool IsCondiment(ConvertedItem current)
        {
            return current.Level > 0;
        }
    }
}
