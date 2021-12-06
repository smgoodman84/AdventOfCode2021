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
                new Day06.Day06()
            };

            var invalidCount = 0;
            foreach (var day in days)
            {
                var part1 = day.Part1();
                var part2 = day.Part2();

                var part1Invalid = !string.IsNullOrWhiteSpace(day.ValidatedPart1)
                    && part1 != day.ValidatedPart1;

                var part2Invalid = !string.IsNullOrWhiteSpace(day.ValidatedPart2)
                    && part2 != day.ValidatedPart2;

                invalidCount += part1Invalid ? 1 : 0;
                invalidCount += part2Invalid ? 1 : 0;

                var part1InvalidString = part1Invalid ? " INVALID" : "";
                var part2InvalidString = part2Invalid ? " INVALID" : "";

                Console.WriteLine($"Day {day.DayNumber} Part 1: {part1}{part1InvalidString}");
                Console.WriteLine($"Day {day.DayNumber} Part 2: {part2}{part2InvalidString}");
            }

            Console.WriteLine($"{invalidCount} INVALID Results");
        }
    }
}
