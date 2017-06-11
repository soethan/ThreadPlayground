﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadingLab.Models;

namespace ThreadingLab
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Pass Parameter to Task

            Console.WriteLine("======= Pass Parameter to Task =======");

            //no return parameter
            var t1 = new Task(new Action<object>(PrintMessage), new Person { Id = 1, Name = "Steven" });
            t1.Start();

            var t2 = new Task(
                delegate(object obj)
                {
                    PrintMessage(obj);
                }, 
                new Person { Id = 2, Name = "David" }
            );

            t2.Start();

            var t3 = new Task(
                (obj) => {
                    PrintMessage(obj);
                }, 
                new Person{ Id = 3, Name = "Bob" }
            );

            t3.Start();

            //with return parameter 'int'
            var t4 = new Task<int>(() => {
                int sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            });

            t4.Start();

            //Main thread will wait at t4.Result until t4 finishes execution and its return statement is executed.
            Console.WriteLine("t4.Result = {0};", t4.Result);
            Console.WriteLine("t4 Ends...");

            var t5 = new Task<int>((objParam) =>
            {
                int max = (int)objParam;
                int sum = 0;
                for (int i = 0; i < max; i++)
                {
                    sum += i;
                }
                return sum;
            }, 200);

            t5.Start();

            //Main thread will wait at t4.Result until t4 finishes execution and its return statement is executed.
            Console.WriteLine("t5.Result = {0};", t5.Result);
            Console.WriteLine("t5 Ends...");

            Task.WaitAll(t1, t2, t3);
            Console.WriteLine("==================================");
            
            #endregion

            #region Task Result

            //1. With new Task().Start
                        
            var task1 = new Task<int>(() => {
                int sum = 0;
                for(int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            });
            task1.Start();

            //Main thread will wait at task1.Result until task1 finishes execution and its return statement is executed.
            Console.WriteLine("task1.Result = {0};", task1.Result);
            Console.WriteLine("task1 Ends...");

            var task2 = new Task<int>(
                (obj) => {
                    int max = (int)obj;
                    int sum = 0;
                    for (int i = 0; i < max; i++)
                    {
                        sum += i;
                    }
                    return sum;
                },
                200
            );

            task2.Start();
            Console.WriteLine("task2.Result = {0};", task2.Result);
            Console.WriteLine("task2 Ends...");

            //2. With Task.Factory.StartNew

            var tfTask1 = Task.Factory.StartNew<int>(() => {
                int sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            });

            //Main thread will wait at tfTask1.Result until tfTask1 finishes execution and its return statement is executed.
            Console.WriteLine("Task.Factory task1.Result = {0};", tfTask1.Result);
            Console.WriteLine("Task.Factory task1 Ends...");

            var tfTask2 = Task.Factory.StartNew<int>((objParam) =>
            {
                int max = (int)objParam;
                int sum = 0;
                for (int i = 0; i < max; i++)
                {
                    sum += i;
                }
                return sum;
            }, 200);

            Console.WriteLine("Task.Factory task2.Result = {0};", tfTask2.Result);
            Console.WriteLine("Task.Factory task2 Ends...");

            #endregion

            Console.Read();
        }

        private static void PrintMessage(object obj)
        {
            var person = obj as Person;
            Console.WriteLine(string.Format("Id={0};Name={1};", person.Id.ToString(), person.Name));
        }
    }
}
