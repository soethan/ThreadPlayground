using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThreadingLab.Models;

namespace ThreadingLab
{
    class Program1
    {
        private static object _ConsoleLock = new object();
        private static object _longRunningTasksLock = new object();

        static void Main1(string[] args)
        {
            #region Long Running Tasks
            
            int N = 10;
            int durationInMins = 0;
            int durationInSecs = 10;

            Welcome(N, durationInMins, durationInSecs);

            // Create 1 per core, and then as they finish, create another:
            int numCores = System.Environment.ProcessorCount;

            List<Task> longRunningTasks = new List<Task>();

            // create initial set of tasks:
            for (int i = 0; i < numCores; i++)
            {
                Task t = CreateOneLongRunningTask(durationInMins, durationInSecs, TaskCreationOptions.None);
                longRunningTasks.Add(t);
            }

            // now, as they finish, create more:
            int done = 0;

            while (done < N)
            {
                int index = Task.WaitAny(longRunningTasks.ToArray());
                done++;

                longRunningTasks.RemoveAt(index);

                if (done + longRunningTasks.Count < N)
                {
                    Console.WriteLine("done=" + done + ";N=" + N);
                    Task t = CreateOneLongRunningTask(durationInMins, durationInSecs, TaskCreationOptions.None);
                    longRunningTasks.Add(t);
                }

            }

            #endregion

            Console.Read();
        }

        static Task CreateOneLongRunningTask(int durationInMins, int durationInSecs, TaskCreationOptions options)
        {
            int durationInMilliSecs = durationInMins * 60 * 1000;
            durationInMilliSecs += (durationInSecs * 1000);

            Task t = Task.Factory.StartNew(() =>
            {
                ConsoleWriteWithColor("starting long-running task...", ConsoleColor.Red);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                long count = 0;

                while (sw.ElapsedMilliseconds < durationInMilliSecs)
                {
                    count++;
                    if (count == 1000000000)
                        count = 0;
                }

                ConsoleWriteWithColor("long-running task finished.", ConsoleColor.Green);
            },
                options
            );

            return t;
        }

        static void Welcome(int N, int durationInMins, int durationInSecs)
        {
            Console.WriteLine("** Long-running Tasks App -- One per core **");
            Console.WriteLine("   Number of tasks: {0:#,##0}", N);
            Console.WriteLine("   Number of cores: {0:#,##0}", System.Environment.ProcessorCount);
            Console.WriteLine("   Task duration:   {0:#,##0} mins, {1:#,##0} secs", durationInMins, durationInSecs);
            Console.WriteLine();
        }

        private static void PrintMessage(object obj)
        {
            var person = obj as Person;
            ConsoleWriteWithColor(string.Format("Id={0};Name={1};", person.Id.ToString(), person.Name));
        }

        private static void ConsoleWriteWithColor(string text, ConsoleColor textColor = ConsoleColor.White)
        {
            lock (_ConsoleLock)
            {
                Console.ForegroundColor = textColor;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }
    }
}
