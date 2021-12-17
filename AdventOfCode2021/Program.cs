using System;
using System.Collections.Generic;
using System.Diagnostics;

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
                new Day03.Day03(),
                new Day04.Day04(),
                new Day05.Day05(),
                new Day06.Day06(),
                new Day07.Day07(),
                new Day08.Day08(),
                new Day09.Day09(),
                new Day10.Day10(),
                new Day11.Day11(),
                new Day12.Day12(),
                new Day13.Day13(),
                new Day14.Day14(),
                new Day15.Day15(),
                new Day16.Day16(),
                new Day17.Day17(),
            };

            var invalidCount = 0;
            foreach (var day in days)
            {
                invalidCount += ResultForDay(day.DayNumber, 1, () => day.Part1(), day.ValidatedPart1);
                invalidCount += ResultForDay(day.DayNumber, 2, () => day.Part2(), day.ValidatedPart2);
            }

            Console.WriteLine($"{invalidCount} INVALID Results");
        }

        private static int ResultForDay(int day, int part, Func<string> resultFunc, string validatedResult)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = resultFunc();
            stopwatch.Stop();

            var invalid = !string.IsNullOrWhiteSpace(validatedResult)
                && result != validatedResult;

            var invalidString = invalid ? " INVALID" : "";

            Console.WriteLine($"Day {day} Part {part} ({stopwatch.ElapsedMilliseconds}ms): {result}{invalidString}");

            return invalid ? 1 : 0;
        }
    }
}
