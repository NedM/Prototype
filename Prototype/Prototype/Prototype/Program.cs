using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Braintree;
using Environment = System.Environment;

namespace Prototype
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.TestAlohaItems();

            //var gateway = new Braintree.BraintreeGateway
            //    {
            //        Environment = Braintree.Environment.SANDBOX,
            //        PublicKey = "MIIBCgKCAQEAsWqrT3RkCyH6yF7ir/lphFcc/lKk792OG5sCGW2dZW3A0NDVoNOpoP0Qj6S0uqgT5higISchdJKSaqurE0fBzqpys0n+o3jTShOxsAS+k1urH7kPtW3DSw9HPVZuKkY+C8a3JcfkFb7OLQsolDGmJdI7BLlt/KB52Z7rP2EqsjqrI+HMLgjN8zWnAl6RbAFNyAsHniww8z/z1BcXhen9UTr9LXcbhZjVjsOrGNR7Ylc2uaLbg/NRuEImXhGc53Dd0DSeKocEG4jdrwZSQFVZjf2D+Hoj11bivkhrd1dwa43rik4cr4qEOlRIdq/DbIroq2tTn46nwOmPd8cFSbu81wIDAQAB",
            //        PrivateKey = "",
            //        MerchantId = "",
            //    };

            //var encryptedValue = gateway.CreditCard.Create(new CreditCardRequest
            //    {
            //        CVV = "1234",
            //        ExpirationMonth = "10",
            //        ExpirationYear = "2014",
            //        Number = "546546545456",
            //    });

            //int g = 2;
            //int m = 35;

            //bool testBool = false;
            //int myInt = 123;
            //string str = "test string";
            //object obj = str;

            //try
            //{
            //    try
            //    {
            //        myInt = (int) obj;

            //        Console.WriteLine("Inside the try.");
            //        testBool = true;
            //    }
            //    finally
            //    {
            //        Console.WriteLine("In the finally block! Test bool is {0}", testBool);
            //    }

            //    Console.WriteLine("After the finally. Test bool is {0}", testBool);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Caught Exception.");
            //}

            //for (int o = 1; o < m; o++)
            //{
            //    if (Math.Pow(g, o)%m == 1)
            //    {
            //        Console.WriteLine("{0}^{1} mod {2} = 1", g, o, m);
            //        break;
            //    }
            //}


            //bool foundIt = false;
            //int x = 7;
            //int y = 23;
            //int z = 23;

            //for (int a = 1; a < 100; a++)
            //{
            //    for (int b = -100; b < 100; b++)
            //    {
            //        int result = (x*a + y*b); //%z;
            //        if (result == 1)
            //        {
            //            Console.WriteLine("Found it!");
            //        }

            //        Console.WriteLine("a = {0}, b = {1}, Result = {2}", a, b, result);

            //        if (result == 1)
            //        {
            //            foundIt = true;
            //            break;
            //        }
            //    }

            //    if (foundIt)
            //    {
            //        break;
            //    }
            //}

            //int n = 35;
            //int count = 0;
            //for (int i = 1; i < n; i++)
            //{
            //    if (CryptoUtils.GCD(i, n) == 1)
            //    {
            //        Console.WriteLine("{0} is relatively prime with {1}", i, n);
            //        count++;
            //    }
            //    else
            //    {
            //        Console.WriteLine("{0} has factors in common with {1}", i, n);
            //    }
            //}
            //Console.WriteLine("Set of relative primes to {0} contains {1} elements", n, count);

            //Address address = new Address("Puyao", "850 Mass. ave.", "Apt. 5", "Cambridge", "MA", string.Empty, "02139", coordinates: new GeographicLocation(123, 321));
            //address.GeographicCoordinates.Latitude = 999;
            //Console.WriteLine(address.ToString());
            //Program.TestFileOrdering();
            //Program.TestTimerBehavior();
            //Program.TestStringFormatting();

            //string levelUp = "LU02000HZITLP4UP3VD81FZG020009LU";
            //Console.WriteLine(levelUp);
            //string snip = levelUp.Remove(2, levelUp.Length - 4);
            //Console.WriteLine(snip);
            //Console.WriteLine(snip.Insert(2, "[LevelUp Customer QR Data Omitted]"));
        }

        private static void TestTimerBehavior()
        {
            System.Timers.Timer tmr = new System.Timers.Timer(1000*2); //2 second timer

            tmr.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);

            Console.WriteLine("Timer Auto reset: " + tmr.AutoReset.ToString());
            Console.WriteLine("Timer Auto enabled: " + tmr.Enabled.ToString());

            tmr.Start();

            Console.WriteLine("Timer Auto enabled: " + tmr.Enabled.ToString());
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {

            Console.WriteLine("Timer fired at {0}", e.SignalTime.ToString("T"));
            //Console.WriteLine("Extra Data: ");
        }

        private static void TestFileOrdering(string directory = "", string pattern = "*.*")
        {
            if (string.IsNullOrEmpty(directory))
            {
                directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }


            Console.WriteLine(string.Format("Finding files matching {0} in {1}...", pattern, directory));
            FileInfo[] files = new DirectoryInfo(directory).GetFiles(pattern)
                                                           .OrderByDescending(f => f.LastWriteTime)
                                                           .ToArray();
            FileInfo[] filesCreatedToday = files.Where(f => f.CreationTime.Date.Equals(DateTime.Now.Date)).ToArray();
            FileInfo[] filesWrittenToday = files.Where(f => f.LastWriteTime.Date.Equals(DateTime.Now.Date)).ToArray();
            Console.WriteLine(
                string.Format("Found {0} files. {1} of them was/were created today. {2} was/were last edited today.",
                              files.Length, filesCreatedToday.Length, filesWrittenToday.Length));
            foreach (FileInfo fi in files)
            {
                Console.WriteLine(string.Format("  {0} created on {1} last edited at {2}", fi.Name,
                                                fi.CreationTime.Date.ToShortDateString(), fi.LastWriteTime));
            }
        }

        private static void TestStringFormatting()
        {
            decimal decimalVal = (decimal) 0.05;
            int intVal = decimal.ToInt32(decimalVal*100);
            Console.WriteLine("Integer value: {0}", intVal);
            Console.WriteLine("Currency format: {0:C2}", decimalVal);
            Console.WriteLine("Fixed point format: {0:F2}", decimalVal);
            decimal newDecimal = ((decimal) intVal)/100;
            Console.WriteLine("New Decimal value: {0:F2}", newDecimal);
        }

        private static void TestAlohaItems()
        {
            List<AlohaItemTest> items = new List<AlohaItemTest>()
                {
                    new AlohaItemTest("invalid item 1", 0, false),
                    new AlohaItemTest("Burger", 0),
                    new AlohaItemTest("Fries", 0),
                    new AlohaItemTest("Extra Salt", 1),
                    new AlohaItemTest("invalid item", 0, false),
                    new AlohaItemTest("invalid sub item", 1, false),
                    new AlohaItemTest("Soup", 0),
                    new AlohaItemTest("invalid sub item 2", 1, false),
                    new AlohaItemTest("Chowdah", 1),
                    new AlohaItemTest("Clam", 2),
                    new AlohaItemTest("Extra oyster crackers", 2),
                    new AlohaItemTest("Cold", 1),
                    new AlohaItemTest("Really Cold", 2),
                    new AlohaItemTest("Large", 1),
                    new AlohaItemTest("Bowl", 1),
                    new AlohaItemTest("Bread", 2),
                    new AlohaItemTest("Wheat", 3),
                    new AlohaItemTest("Pepsi Cola", 0),
                    new AlohaItemTest("Large", 1),
                };
            
            List<ConvertedItem> converted = BuildItemList(new Queue<AlohaItemTest>(items));

            foreach (var convertedItem in converted)
            {
                Console.WriteLine(convertedItem.ToString());
            }
        }

        private static List<ConvertedItem> BuildItemList(Queue<AlohaItemTest> items)
        {
            List<ConvertedItem> convertedItems = new List<ConvertedItem>();
            ConvertedItem current = null;

            if (null == items)
            {
                throw new ArgumentException("NULL item list!");
            }

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

                AlohaItemTest next = PeekNextValidItem(items);
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

        private static ConvertedItem DequeueNextValidItem(Queue<AlohaItemTest> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("NULL or empty items list!");
            }

            AlohaItemTest alohaItem = items.Dequeue();

            //Remove invalid items
            while (!alohaItem.IsValid)
            {
                if (items.Count == 0)
                {
                    return null;
                }

                alohaItem = items.Dequeue(); //remove the invalid item
            }

            return ConvertedItem.FromAlohaItem(alohaItem);
        }

        private static AlohaItemTest PeekNextValidItem(Queue<AlohaItemTest> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Count == 0)
            {
                return null;
            }

            AlohaItemTest nextValidItem = items.Peek();

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

        private static bool IsNextItemHigherLevel(ConvertedItem current, AlohaItemTest next)
        {
            return null != current && current.Level > next.Level;
        }
    }
}
