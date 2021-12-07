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
        public string ValidatedPart2 => "98119739";

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
            return GetResult(x => x);
        }


        public string Part2()
        {
            return GetResult(GetFuelForDistance);
        }

        private string GetResult(Func<int, long> getFuelForDistance)
        {
            var minPosition = _crabPositions.Min();
            var maxPosition = _crabPositions.Max();
            var count = maxPosition - minPosition;

            var fuelForPositions = Enumerable.Range(minPosition, count)
                .Select(pos => (Position: pos, Fuel: _crabPositions.Sum(cp => getFuelForDistance(Math.Abs(pos - cp)))))
                .ToList();

            var minimalFuel = fuelForPositions
                .OrderBy(x => x.Fuel)
                .First();

            return minimalFuel.Fuel.ToString();
        }

        private Dictionary<int, long> _fuelForDistance = new Dictionary<int, long>();
        private long GetFuelForDistance(int distance)
        {
            if (!_fuelForDistance.ContainsKey(distance))
            {
                if (distance == 0)
                {
                    return 0;
                }

                _fuelForDistance[distance] = GetFuelForDistance(distance - 1) + distance;
            }

            return _fuelForDistance[distance];
        }
    }
}
