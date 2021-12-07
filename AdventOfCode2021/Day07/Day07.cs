using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day07
{
    public class Day07 : IDay
    {
        public int DayNumber => 7;
        public string ValidatedPart1 => "353800";
        public string ValidatedPart2 => "";

        private List<int> _crabPositions;

        public Day07()
        {
            _crabPositions = File.ReadAllLines("Day07/input.txt")
                .Single()
                .Split(',')
                .Select(int.Parse)
                .ToList();
        }

        public string Part1()
        {
            var minPosition = _crabPositions.Min();
            var maxPosition = _crabPositions.Max();
            var count = maxPosition - minPosition;

            var fuelForPositions = Enumerable.Range(minPosition, count)
                .Select(pos => (Position: pos, Fuel: _crabPositions.Sum(cp => Math.Abs(pos - cp))))
                .ToList();

            var minimalFuel = fuelForPositions
                .OrderBy(x => x.Fuel)
                .First();

            return minimalFuel.Fuel.ToString();
        }

        public string Part2()
        {
            return "";
        }
    }
}
