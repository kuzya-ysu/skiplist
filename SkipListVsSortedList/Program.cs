using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkipList2020;

namespace SkipListVsSortedList
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int n = 10;
            var rd = new Random(DateTime.Now.Millisecond);
            var set = new HashSet<int>();
            while (set.Count < n)
                set.Add(rd.Next(0, n));

            var sortedList = new SortedList<int, int>();
            var t = new Stopwatch();
            t.Start();
            foreach (var num in set)
                sortedList.Add(num, 1);
            t.Stop();
            Console.WriteLine("Sorted list" + t.Elapsed);

            var skipList = new SkipList<int, int>();
            t = new Stopwatch();
            t.Start();
            foreach (var num in set)
                skipList.Add(num, 1);
            t.Stop();
            Console.WriteLine("Skip list" + t.Elapsed);

            Console.ReadLine();
        }
    }
}