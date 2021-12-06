using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day06
{
    public class Day06 : IDay
    {
        public int DayNumber => 6;
        public string ValidatedPart1 => "345387";
        public string ValidatedPart2 => "";

        private List<int> _lanternfishDaysToReproduction;

        public Day06()
        {
            _lanternfishDaysToReproduction = File.ReadAllLines("Day06/input.txt")
                .Single()
                .Split(',')
                .Select(int.Parse)
                .ToList();
        }

        public string Part1()
        {
            var lanternfishes = _lanternfishDaysToReproduction
                .GroupBy(x => x)
                .Select(g => new Lanternfish(g.Key, g.Count()))
                .ToList();

            foreach (var day in Enumerable.Range(0,80))
            {
                var newLanternfish = 0;

                foreach(var lanternfish in lanternfishes)
                {
                    newLanternfish += lanternfish.AdvanceDays();
                }

                lanternfishes.Add(new Lanternfish(8, newLanternfish));
            }

            var total = lanternfishes.Sum(l => l.Quantity);

            return total.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class Lanternfish
        {
            public Lanternfish(int daysUntilReproduction, int quantity)
            {
                DaysUntilReproduction = daysUntilReproduction;
                Quantity = quantity;
            }

            public int DaysUntilReproduction { get; private set; }
            public int Quantity { get; private set; }

            public int AdvanceDays()
            {
                DaysUntilReproduction -= 1;

                if (DaysUntilReproduction < 0)
                {
                    DaysUntilReproduction = 6;
                    return Quantity;

                }

                return 0;
            }
        }
    }
}
