using System;
using System.Collections.Generic;

namespace Prototype
{
    public class AlohaCheck
    {
        public static List<ConvertedItem> BuildItemList(Queue<MenuItem> items)
        {
            if (null == items)
            {
                throw new ArgumentException("NULL item list!");
            }

            List<ConvertedItem> convertedItems = new List<ConvertedItem>();
            ConvertedItem current = null;

            while (items.Count > 0)
            {
                //Check to see that the next item is not a higher level than the current Item
                if (IsNextItemHigherLevel(current, items.Peek()))
                {
                    //If the next item it still higher level than current, return the output list
                    return convertedItems;
                }

                current = DequeueNextValidItem(items);
                if (null == current)
                {
                    return convertedItems;
                }

                convertedItems.Add(current);

                MenuItem next = PeekNextValidItem(items);
                if (null == next)
                {
                    //current item is last valid item in input list.
                    return convertedItems;
                }

                //next item is higher level than current.
                //Unwrap the stack and return the output list
                if (current.Level > next.Level)
                {
                    //Return the list with sub items all filled
                    return convertedItems;
                }

                //next item is lower level than current
                //Recusively build the sub item list
                if (current.Level < next.Level)
                {
                    //Add all the sub items to the parent item
                    current.SubItems.AddRange(BuildItemList(items));
                }
            }

            return convertedItems;
        }

        private static ConvertedItem DequeueNextValidItem(Queue<MenuItem> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("NULL or empty items list!");
            }

            MenuItem alohaItem = items.Dequeue();

            //Remove invalid items
            while (!alohaItem.IsValid)
            {
                if (items.Count == 0)
                {
                    return null;
                }

                alohaItem = items.Dequeue(); //remove the invalid item
            }

            return ConvertedItem.FromMenuItem(alohaItem);
        }

        private static MenuItem PeekNextValidItem(Queue<MenuItem> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Count == 0)
            {
                return null;
            }

            MenuItem nextValidItem = items.Peek();

            //Check next item for validity
            while (!nextValidItem.IsValid)
            {
                items.Dequeue(); //remove the invalid item

                if (items.Count > 0)
                {
                    nextValidItem = items.Peek();
                }
                else
                {
                    //current item is last valid item in input list.
                    return null;
                }
            }

            return nextValidItem;
        }

        private static bool IsNextItemHigherLevel(ConvertedItem current, MenuItem next)
        {
            return null != current && current.Level > next.Level;
        }
    }
}
