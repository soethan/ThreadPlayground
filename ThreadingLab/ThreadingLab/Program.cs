using System;
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
            Task t1 = new Task(new Action<object>(printMessage), new Person { Id = 1, Name = "Steven" });
            t1.Start();

            Console.Read();
        }

        private static void printMessage(object obj)
        {
            var person = obj as Person;
            Console.WriteLine(string.Format("Id={0};Name={1};", person.Id.ToString(), person.Name));
        }
    }
}
