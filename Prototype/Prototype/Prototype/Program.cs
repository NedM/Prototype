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
using System.Text;
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
            Console.WriteLine("0: {0}, 1: {1}, 2: {2}, NULL: {3}",
                              Convert.ToBoolean(0) ? "TRUE" : "FALSE",
                              Convert.ToBoolean(1) ? "TRUE" : "FALSE",
                              Convert.ToBoolean(2) ? "TRUE" : "FALSE",
                              Convert.ToBoolean(null) ? "TRUE" : "FALSE");
            //Console.WriteLine("Environment.NewLine does{0} count as empty string.", string.IsNullOrEmpty(Environment.NewLine) ? null : " NOT");
            //Console.WriteLine(null + "This is a test of the null value.");
            Console.WriteLine("True: {0}, False: {1}", Convert.ToInt32(true), Convert.ToInt32(false));

            TestPassByReference();
            //TestRedactMethod();
            //TestTimerBehavior();
            //TestThreadPoolBehavior();
            //TestFlagBehavior();
            //TestJoinBehavior();
            //TestInheritance();
            //TestCollections();
            //TestCollectionsInheritance();
            //TestNumberTruncation();
            //TestRefNumberRollover(true);
            //TestUriBuilder();
            //IterateThrough2dArray();
            //AsyncProgramming();

            //string test = "1a2b3c4d5e6f";
            
            //Console.WriteLine("Original: {0}, Substring(0,6): {1}, Substring(6): {2}", test, test.Substring(0, 6), test.Substring(6));
            //Console.WriteLine("Date YYMMDD format: {0}", DateTime.Now.ToString("yyMMdd", CultureInfo.InvariantCulture));
            Console.WriteLine("Runtime version: " + System.Environment.Version);
        }

        private static void TestPassByReference()
        {
            Blittable blit = new Blittable()
                {
                    Data = 100,
                };

            GCHandle handle = GCHandle.Alloc(blit, GCHandleType.Pinned);
            IntPtr address = handle.AddrOfPinnedObject();
            Console.WriteLine(address.ToString("x") + " <- Before pass by value. Value: " + blit.Data);

            MyPassByValueMethod(blit);

            address = handle.AddrOfPinnedObject();
            Console.WriteLine(address.ToString("x") + " <- After pass by value. Value: " + blit.Data);

            MyPassByReferenceMethod(ref blit);

            address = handle.AddrOfPinnedObject();
            Console.WriteLine(address.ToString("x") + " <- After pass by ref. Value: " + blit.Data);

            handle.Free();
        }

        private static void MyPassByValueMethod(Blittable x)
        {
            //x = null;
            //x.Data = 5;
            x = new Blittable(){ Data = 0 };

            GCHandle handle = GCHandle.Alloc(x, GCHandleType.Pinned);
            IntPtr address = handle.AddrOfPinnedObject();
            Console.WriteLine(address.ToString("x") + " <- MyPassByValueMethod. Value: " + x.Data);
            handle.Free();
        }

        private static void MyPassByReferenceMethod(ref Blittable y)
        {
            //y = null;
            //y.Data = 2;
            y = new Blittable()
                {
                    Data = 345,
                };

            GCHandle handle = GCHandle.Alloc(y, GCHandleType.Pinned);
            IntPtr address = handle.AddrOfPinnedObject();
            Console.WriteLine(address.ToString("x") + " <- MyPassByReferenceMethod. Value: " + y.Data);
            handle.Free();
        }

        private static void TestMultiDimensionalArrays()
        {
            string[,] rectArray = new string[,]
                {
                    {"name1", "param1", "value1"},
                    {"name2", "param2", "value2"},
                };

            for (int i = 0; i < rectArray.GetLongLength(0); i++)
            {
                Console.WriteLine("Name: {0}, Key: {1}, Value: {2}", rectArray[i, 0], rectArray[i, 1], rectArray[i, 2]);
            }

            string[][] jagArray = new string[][]
                {
                    new string[3] {"Alice", "Aaron", "Arthur"},
                    new string[4] {"Bob", "Beth", "Bartholemew", "Buford"}
                };

            for (int i = 0; i < jagArray.GetLongLength(0); i++)
            {
                StringBuilder sb = new StringBuilder(string.Format("Row {0}:", i));

                foreach (string s in jagArray[i])
                {
                    sb.Append(" " + s);
                }

                Console.WriteLine(sb.ToString());
            }
        }

        private static void TestNumberTruncation()
        {
            int random = new Random((int) DateTime.Now.Ticks).Next(100000000);

            string randomAsString = random.ToString();
            string temp = random.ToString("D5");
            string trunc = temp.Substring(temp.Length - 5, 5);

            Console.WriteLine("{0} is {1} characters long.\tTruncated {2} is {3} characters long.", 
                randomAsString, 
                randomAsString.Length,
                trunc,
                trunc.Length);

            int random1 = new Random((int) DateTime.Now.Ticks).Next(100);
            string random1AsString = random1.ToString();
            temp = random1.ToString("D5");
            string trunc1 = temp.Substring(temp.Length - 5, 5);

            Console.WriteLine("{0} is {1} characters long.\tTruncated {2} is {3} characters long.", 
                random1AsString,
                random1AsString.Length,
                trunc1, 
                trunc1.Length);
        }

#if ASYNC
        private static void AsyncProgramming()
        {
            //Best practices: https://msdn.microsoft.com/en-us/magazine/jj991977.aspx

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
#endif

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
            Uri uri = new Uri("https://api.thelevelup.com//v14/access_tokens?t=50&a=45");
            Console.WriteLine("Absolute: {0}\nTo string: {1}", uri.AbsoluteUri, uri);

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
