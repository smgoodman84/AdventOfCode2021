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
            };

            var invalidCount = 0;
            foreach (var day in days)
            {
                var part1 = day.Part1();
                var part1Invalid = !string.IsNullOrWhiteSpace(day.ValidatedPart1)
                    && part1 != day.ValidatedPart1;
                invalidCount += part1Invalid ? 1 : 0;
                var part1InvalidString = part1Invalid ? " INVALID" : "";
                Console.WriteLine($"Day {day.DayNumber} Part 1: {part1}{part1InvalidString}");

                var part2 = day.Part2();
                var part2Invalid = !string.IsNullOrWhiteSpace(day.ValidatedPart2)
                    && part2 != day.ValidatedPart2;
                invalidCount += part2Invalid ? 1 : 0;
                var part2InvalidString = part2Invalid ? " INVALID" : "";
                Console.WriteLine($"Day {day.DayNumber} Part 2: {part2}{part2InvalidString}");
            }

            Console.WriteLine($"{invalidCount} INVALID Results");
        }

        private bool ExecuteDay(int day, int part, Func<string> func, string validatedResult)
        {
            var result = func();

            var invalid = !string.IsNullOrWhiteSpace(validatedResult)
                && result != validatedResult;

            var invalidString = invalid ? " INVALID" : "";
            Console.WriteLine($"Day {day} Part {part}: {result}{invalidString}");

            return invalid;
        }
    }
}
