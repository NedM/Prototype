using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Braintree;
using Environment = System.Environment;

namespace Prototype
{
    public class Program
    {
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int TIMER_ELAPSE_MAX_COUNT = 5;
        private static int _timerFiredCount;
        private static AutoResetEvent _autoResetEventGate;
        
        public static void Main(string[] args)
        {
            //Console.WriteLine("Environment.NewLine does{0} count as empty string.", string.IsNullOrEmpty(Environment.NewLine) ? null : " NOT");
            //Console.WriteLine(null + "This is a test of the null value.");
            //Console.WriteLine("True: {0}, False: {1}", Convert.ToInt32(true), Convert.ToInt32(false));
            //TestRedactMethod();
            //TestTimerBehavior();
            //TestThreadPoolBehavior();
            //TestFlagBehavior();
            //TestJoinBehavior();
            //TestInheritance();
            //TestCollections();
            //TestCollectionsInheritance();
            //TestRefNumberRollover(true);
            //TestUriBuilder();
            //IterateThrough2dArray();
            AsyncProgramming();
        }

        private static void AsyncProgramming()
        {
            //Stopwatch watch = new Stopwatch();

            Thread workThread = new Thread(() =>
                {
                    Console.WriteLine("[Worker Thread] Hello! Thread={0}", Thread.CurrentThread.ManagedThreadId);

                    //watch.Start();
                    for (int i = 0; i <= 5; i++)
                    {
                        Console.WriteLine("[Worker Thread] Waited {0} seconds.", i);
                        Thread.Sleep(1000);
                    }
                    //watch.Stop();
                    //Console.WriteLine("[Worker Thread] Goodbye. Worked for {0}", watch.Elapsed.ToString());
                })
                {
                    IsBackground = true,
                    Name = "Worker Thread",
                    Priority = ThreadPriority.Normal,
                };
            workThread.SetApartmentState(ApartmentState.STA);

            Task<string> t = new Task<string>(TestRefNumberRollover);

            Console.WriteLine("[Main thread] = {0}", Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("[Main thread] Starting Method1Async...");
            Method1Async();
            Console.WriteLine("[Main thread] Control returned from calling Method1Async!");

            Console.WriteLine("[Main thread] Starting TestRefNumberRollover task...");
            t.Start();
            Console.WriteLine("[Main thread] Control returned from TestRefNumberRollover task start!");

            Console.WriteLine("[Main thread] Starting Worker Thread...");
            workThread.Start();
            Console.WriteLine("[Main thread] Control returned from Worker Thread start operation...");
            //Thread.Sleep(500); //Allow work thread to get processor time

            Console.WriteLine("[Main thread] Starting Method2Async...");
            Method2Async();
            Console.WriteLine("[Main thread] Control returned from calling Method2Async!");

            Console.WriteLine("[Main thread] BLOCKED waiting for TestRefNumberRollover task to complete");
            t.Wait(15000);
            Console.WriteLine("[Main thread] UNBLOCKED TestRefNumberRollover task completed");
            Console.WriteLine("[Main thread] TestRefNumberRollover task result: {0}", t.Result);

            Console.WriteLine("[Main thread] BLOCKED waiting for Worder Thread to complete");
            workThread.Join();
            Console.WriteLine("[Main thread] UNBLOCKED Worder Thread completed");

            Console.WriteLine("[Main thread] Exiting. All done.");

            //Threads:
            //Thread 1 -> Main thread, Method1Async, Method2Async
            //Thread 2 -> DoWork
            //Thread 3 -> Counter task
            //Thread 4 -> Worker thread
            //Thread 5 -> TestRefNumberRollover task
        }

        private static async void Method1Async()
        {
            Console.WriteLine("[Method1Async] Hello! Thread={0}", Thread.CurrentThread.ManagedThreadId);

            await DoWork();

            Console.WriteLine("[Method1Async] Goodbye!");
        }

        private static async void Method2Async()
        {
            Console.WriteLine("[Method2Async] Hello! Thread={0}", Thread.CurrentThread.ManagedThreadId);

            await Task.Run(() =>
                {
                    Console.WriteLine("[Method2Async - Task] Starting count... Thread={0}", Thread.CurrentThread.ManagedThreadId);
                    for (int i = 0; i < 10000; i += 5)
                    {
                        if (0 == i%1000)
                        {
                            Console.WriteLine("[Method2Async - Task] Done with {0} iterations.", i);
                            Thread.Sleep(250);
                        }
                    }
                });

            Console.WriteLine("[Method2Async] Goodbye!");
        }

        private static Task DoWork()
        {
            return Task.Run(() =>
                {
                    Console.WriteLine("[DoWork] Doing work... Thread={0}", Thread.CurrentThread.ManagedThreadId);
                    //5 second sleep represents computationally intensive task. 
                    //Substitute real work (e.g. DB reads or HTTP calls) here
                    //Task.Delay(7500);
                    Thread.Sleep(5000);

                    Console.WriteLine("[DoWork] Done with work. Let's have a beer!");
                });
        }

        private static void IterateThrough2dArray()
        {
            string[,] testArray = new string[,]
                {
                    {"abc", "123"},
                    {"def", "456"},
                };

            for (int i = 0; i < testArray.GetLength(0); i++)
            {
                Console.WriteLine("{0} - {1}", testArray[i, 0], testArray[i, 1]);
            }
        }

        private static void TestUriBuilder()
        {
            UriBuilder builder = new UriBuilder("https://api.thelevelup.com/v14/");
            Console.WriteLine("Host: {0}, Scheme: {1}\n     Uri: {2}\nToString: {3}",
                              builder.Host,
                              builder.Scheme,
                              builder.Uri,
                              builder);

            UriBuilder builder1 = new UriBuilder("http://192.168.34.56");
            builder1.Path = "ThisIsA?Test";
            builder1.Query = "test=yes";
            builder1.Port = 12345;
            Console.WriteLine("\n     Uri: {0}\nToString: {1}", builder1.Uri, builder1);
        }

        private static string TestRefNumberRollover()
        {
            return TestRefNumberRollover(false);
        }

        private static string TestRefNumberRollover(bool verbose)
        {
            int DISCOUNT_REF_COUNT_LIMIT = (int)Math.Pow(10, 8);
            string refNumber = "UNINITIALIZED";

            Console.WriteLine("Hello from TestRefNumberRollover! Thread={0}", Thread.CurrentThread.ManagedThreadId);

            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < Math.Pow(10, 8) + 1; i++)
            {
                i += (int) Math.Pow(10, 4) + rand.Next(5000, 50000);
                int refNum = i % DISCOUNT_REF_COUNT_LIMIT;
                refNumber = string.Format("LD{0}", refNum.ToString("D8", CultureInfo.InvariantCulture));

                if (verbose)
                {
                    Console.WriteLine("Length: {0}, {1}", refNumber.Length, refNumber);
                }
            }

            Console.WriteLine("Goodbye from TestRefNumberRollover");

            return refNumber;
        }

        private static void TestCollectionsInheritance()
        {
            BaseFoo foo = new BaseFoo();
            SubFoo subFoo = new SubFoo();
            List<BaseFoo> baseList = new List<BaseFoo>();
            List<SubFoo> subList = new List<SubFoo>();
            BaseFooCollection baseCollection = new BaseFooCollection();
            SubFooCollection subCollection = new SubFooCollection();

            Console.WriteLine("subFoo is{0} a sub class of BaseFoo", subFoo is BaseFoo ? string.Empty : " not");
            Console.WriteLine("subList is{0} a subtype of BaseFoo List", subList is List<BaseFoo> ? string.Empty : " not");
            Console.WriteLine("subCollection is{0} a subtype of BaseFooCollection", subCollection is BaseFooCollection ? string.Empty : " not");
            Console.WriteLine("subCollection is{0} an implementation of ICollection<BaseFoo>", subCollection is ICollection<BaseFoo> ? string.Empty : " not");
        }

        private static void TestCollections()
        {
            ICollection<string> myCollection = null;

            myCollection = CreateCollectionOfStrings();

            myCollection.Add("Final");
        }

        private static ICollection<string> CreateCollectionOfStrings()
        {
            //string[] stringCollection = null;
            string[] stringCollection = new string[]{};
            //stringCollection = new string[] { "First", "Second" };
            //return new Collection<string>(stringCollection);
            //return stringCollection;
            return new List<string>(stringCollection);
        }

        private static void TestInheritance()
        {
            BaseFoo foo = new BaseFoo();
            BaseFoo foo1 = new BaseFoo("Fooyao");
            SubFoo soo = new SubFoo();
            SubFoo soo1 = new SubFoo("SubADubDub");
            BaseFoo foo2 = new SubFoo("Wally");

            Console.WriteLine("foo: {0}", foo);
            Console.WriteLine("foo1: {0}", foo1);
            Console.WriteLine("soo: {0}", soo);
            Console.WriteLine("soo1: {0}", soo1);
            Console.WriteLine("foo2: {0}", foo2);
        }

        private static void TestJoinBehavior()
        {
            Dictionary<int, List<int>> testDictionary = new Dictionary<int, List<int>>
                {
                    {1, new List<int>{ 1, 2, 3}},
                    {3, new List<int>()},
                    {2, null},
                };

            foreach (var key in testDictionary.Keys)
            {
                Console.WriteLine("Key: {0}, Value: {1}", key,
                                  testDictionary[key] == null
                                  ? "NULL"
                                  : string.Join(",", testDictionary[key].ToArray()));
            }
        }

        private static void TestFlagBehavior()
        {
            int count = 0;
            TestFlags[] testFlags =
                {
                    TestFlags.NoFlags, 
                    TestFlags.Flag1, 
                    TestFlags.Flag2 | TestFlags.Flag1,
                    TestFlags.Flag4 | TestFlags.Flag2 | TestFlags.Flag1,
                    TestFlags.AllFlags, 
                };

            foreach (var flag in testFlags)
            {
                Console.WriteLine("[{0}] {1} Flag(s): {2}", ++count, CountFlags(flag), flag);
            }
        }

        private static void TestThreadPoolBehavior()
        {
            const int workTime = 5;
            Console.WriteLine("Queing work item.");
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc));
            Console.WriteLine("Main thread working...");

            for (int i = 0; i < workTime; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Still working. Done in {0}", workTime - i);
            }

            Console.WriteLine("Main thread is done. Exiting...");
        }

        private static void ThreadProc(object state)
        {
            Console.WriteLine("Hello from the thread pool!");
            Thread.Sleep(2000);
            Console.WriteLine("Goodbye from the thread pool.");
        }

        private static void TestTimerBehavior()
        {
            const int numWaitLoops = 2;
            System.Timers.Timer tmr = new System.Timers.Timer(1000*2); //2 second timer
            
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);

            Console.WriteLine("Timer Auto reset: " + tmr.AutoReset.ToString());
            Console.WriteLine("Timer Auto enabled: " + tmr.Enabled.ToString());

            tmr.Start();

            Console.WriteLine("Timer Auto enabled: " + tmr.Enabled.ToString());
            //Console.WriteLine("Press enter to exit");
            //Console.ReadLine();
            Console.WriteLine("Waiting...");
            _autoResetEventGate = new AutoResetEvent(false);
            TimeSpan maxWaitTime = new TimeSpan(0, 0, 45);

            for (int i = 0; i < numWaitLoops; i++)
            {
                Console.WriteLine("Setting/Resetting waiter...");
                _timerFiredCount = 0;
                _autoResetEventGate.Reset();
                Console.WriteLine("Waiter reset. Waiting...");
                _autoResetEventGate.WaitOne(maxWaitTime);
            }
            Console.WriteLine("Done!");
        }

        private static void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            _timerFiredCount++;
            Console.WriteLine("Timer fired at {0}", e.SignalTime.ToString("T"));
            //Console.WriteLine("Extra Data: ");
            Console.WriteLine("Timer has fired {0} times", _timerFiredCount);
            if (_timerFiredCount >= TIMER_ELAPSE_MAX_COUNT)
            {
                _autoResetEventGate.Set();
            }
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

        private static void TestRedactMethod()
        {
            string originalQr = "LU02000ROEUCYCZODZJ1ZND4020000LU";
            Console.WriteLine("Original: {0}{1}" +
                              "Redacted: {2}{1}" +
                              "Original Again: {3}",
                              originalQr,
                              Environment.NewLine,
                              RedactQrData(originalQr),
                              originalQr);
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

        private static int CountFlags(TestFlags flags)
        {
            int counter = 0;

            foreach (TestFlags type in Enum.GetValues(typeof(TestFlags)))
            {
                if (type == TestFlags.NoFlags ||
                    type == TestFlags.AllFlags)
                {
                    continue;
                }

                if ((flags & type) == type)
                {
                    counter++;
                }
            }

            return counter;
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

        private static string RedactQrData(string unsanitizedQrCode)
        {
            const int charsOnEachSideToLeave = 7;
            int length = unsanitizedQrCode.Length;
            int sizeOfSectionToRedact = length - (2 * charsOnEachSideToLeave);

            return
                unsanitizedQrCode.Replace(
                    unsanitizedQrCode.Substring(charsOnEachSideToLeave, sizeOfSectionToRedact), "[** Redacted **]");
        }

    }
}
