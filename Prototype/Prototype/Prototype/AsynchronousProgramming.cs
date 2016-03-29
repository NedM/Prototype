#define ASYNC

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prototype
{
    public class AsynchronousProgramming
    {
        public static void Test()
        {
//            AsyncAwaitProgramming();
            BackgroundThreadProgramming(15);
        }

        private static void BackgroundThreadProgramming(int numberOfElementsToCompute)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);

            Thread thread = new Thread(() =>
                {
                    Console.WriteLine("I am Background Thread! My id is {0}. " +
                                      "Fear me and my mighty background processing!",
                                      Thread.CurrentThread.ManagedThreadId);

                    Console.WriteLine("The mighty Background thread sayeth:" + Environment.NewLine +
                                      FibonnaciSequence(numberOfElementsToCompute));

                    Console.WriteLine("Background Thread is done! Background thread awayyyy!");
                    resetEvent.Set();
                })
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Normal,
                    Name = "Background Thread",
                };
            thread.SetApartmentState(ApartmentState.STA);

            Console.WriteLine("I am the Master thread. My id is {0}.", Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("[Master] Starting background thread...");
            thread.Start();

            Console.WriteLine("[Master] Doing some work of my own...");
            Console.WriteLine("[Master] " + FibonnaciSequence(10));
            Console.WriteLine("[Master] Done with my work!");

            Console.WriteLine("[Master] Waiting for Background thread...");
            WaitHandle.WaitAll(new WaitHandle[] {resetEvent});
            Console.WriteLine("[Master] Done waiting!");
        }

        private static string FibonnaciSequence(int numberElements)
        {
            int nMinus1 = 1;
            int n = 1;
            StringBuilder sb = new StringBuilder();

            if (numberElements < 1)
            {
                return string.Empty;
            }
            
            if (numberElements < 2)
            {
                return "1";
            }

            sb.Append("1, 1");

            for (int i = 2; i < numberElements; i++)
            {
                int next = n + nMinus1;
                nMinus1 = n;
                n = next;

                sb.AppendFormat(", {0}", next);
            }
            
            return sb.ToString();
        }

        //Use '#define ASYNC' at top of file to enable this block of code. 
        //!! Requires .NET 4.5 !!
#if ASYNC
        private static void AsyncAwaitProgramming()
        {
            //Best practices: https://msdn.microsoft.com/en-us/magazine/jj991977.aspx

            Stopwatch watch = new Stopwatch();

            Thread workThread = new Thread(() =>
                {
                    Console.WriteLine("[Worker Thread] Hello! Thread={0}", Thread.CurrentThread.ManagedThreadId);

                    watch.Start();
                    for (int i = 0; i <= 5; i++)
                    {
                        Console.WriteLine("[Worker Thread] Waited {0} seconds.", i);
                        Thread.Sleep(1000);
                    }
                    watch.Stop();
                    Console.WriteLine("[Worker Thread] Goodbye. Worked for {0}", watch.Elapsed.ToString());
                })
                {
                    IsBackground = true,
                    Name = "Worker Thread",
                    Priority = ThreadPriority.Normal,
                };
            workThread.SetApartmentState(ApartmentState.STA);

            Task<string> t = new Task<string>(DoLongWork);

            Console.WriteLine("[Main thread] = {0}", Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("[Main thread] Starting Method1Async...");
            Method1Async();
            Console.WriteLine("[Main thread] Control returned from calling Method1Async!");

            Console.WriteLine("[Main thread] Starting DoLongWork task...");
            t.Start();
            Console.WriteLine("[Main thread] Control returned from DoLongWork task start!");

            Console.WriteLine("[Main thread] Starting Worker Thread...");
            workThread.Start();
            Console.WriteLine("[Main thread] Control returned from Worker Thread start operation...");
            //Thread.Sleep(500); //Allow work thread to get processor time

            Console.WriteLine("[Main thread] Starting Method2Async...");
            Method2Async();
            Console.WriteLine("[Main thread] Control returned from calling Method2Async!");

            Console.WriteLine("[Main thread] BLOCKED waiting for DoLongWork task to complete");
            bool completeSuccess = t.Wait(15000);
            Console.WriteLine("[Main thread] UNBLOCKED" +
                              " DoLongWork task {0}completed successfully within the timeout",
                              completeSuccess ? string.Empty : "was NOT ");
            Console.WriteLine("[Main thread] DoLongWork task result: {0}", t.Result);

            Console.WriteLine("[Main thread] BLOCKED waiting for Worder Thread to complete");
            workThread.Join();
            Console.WriteLine("[Main thread] UNBLOCKED Worder Thread completed");

            Console.WriteLine("[Main thread] Exiting. All done.");

            //Threads:
            //Thread 1 -> Main thread, Method1Async, Method2Async
            //Thread 2 -> DoWork
            //Thread 3 -> Counter task
            //Thread 4 -> Worker thread
            //Thread 5 -> DoLongWork task
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
                    Console.WriteLine("[Method2Async - Task] Starting count... Thread={0}",
                                      Thread.CurrentThread.ManagedThreadId);
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

        private static string DoLongWork()
        {
            Console.WriteLine("Hello from DoLongTask! Thread={0}", Thread.CurrentThread.ManagedThreadId);

            int DISCOUNT_REF_COUNT_LIMIT = (int) Math.Pow(10, 8);
            string refNumber = "UNINITIALIZED";

            Random rand = new Random((int) DateTime.Now.Ticks);

            for (int i = 0; i < Math.Pow(10, 8) + 1; i++)
            {
                i += (int) Math.Pow(10, 4) + rand.Next(5000, 50000);
                int refNum = i%DISCOUNT_REF_COUNT_LIMIT;
                refNumber = string.Format("LD{0}", refNum.ToString("D8", CultureInfo.InvariantCulture));
            }

            Console.WriteLine("Goodbye from DoLongTask");

            return refNumber;
        }
#endif
    }
}
