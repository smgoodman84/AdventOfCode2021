using System;
using System.Collections.Generic;

namespace AdventOfCode2021
{
    class Program
    {
        static void Main(string[] args)
        {
            var days = new List<IDay>
            {
                new Day01.Day01(),
                new Day02.Day02(),
                new Day03.Day03()
            };

            foreach(var day in days)
            {
                Console.WriteLine($"Day {day.Day} Part 1");
                day.ExecutePart1();
                Console.WriteLine($"Day {day.Day} Part 2");
                day.ExecutePart2();
            }
        }
    }
}
