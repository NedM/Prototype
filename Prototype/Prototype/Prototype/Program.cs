using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Program.TestFileOrdering();
            //Program.TestTimerBehavior();
            Program.TestStringFormatting();

            //string levelUp = "LU02000HZITLP4UP3VD81FZG020009LU";
            //Console.WriteLine(levelUp);
            //string snip = levelUp.Remove(2, levelUp.Length - 4);
            //Console.WriteLine(snip);
            //Console.WriteLine(snip.Insert(2, "[LevelUp Customer QR Data Omitted]"));
        }

        private static void TestTimerBehavior()
        {
            System.Timers.Timer tmr = new System.Timers.Timer(1000 * 2);  //2 second timer

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
            Console.WriteLine(string.Format("Timer fired at {0}:{1}:{2}", DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString(), DateTime.Now.Second.ToString()));
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
            Console.WriteLine(string.Format("Found {0} files. {1} of them was/were created today. {2} was/were last edited today.", files.Length, filesCreatedToday.Length, filesWrittenToday.Length));
            foreach (FileInfo fi in files)
            {
                Console.WriteLine(string.Format("  {0} created on {1} last edited at {2}", fi.Name, fi.CreationTime.Date.ToShortDateString(), fi.LastWriteTime));
            }
        }

        private static void TestStringFormatting()
        {
            decimal decimalVal = (decimal)0.05;
            int intVal = decimal.ToInt32(decimalVal*100);
            Console.WriteLine("Currency format: {0:C2}", decimalVal);
            Console.WriteLine("Fixed point format: {0:F2}", decimalVal);
        }
    }
}
