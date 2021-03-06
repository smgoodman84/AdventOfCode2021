using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day01
{
    public class Day01 : IDay
    {
        public int DayNumber => 1;
        public string ValidatedPart1 => "1215";
        public string ValidatedPart2 => "1150";

        private List<int> _depths;

        public Day01()
        {
            _depths = File.ReadAllLines("Day01/input.txt")
                .Select(int.Parse)
                .ToList();
        }

        public string Part1()
        {
            int increaseCount = 0;
            int? previousValue = null;
            foreach (var depth in _depths)
            {
                if (previousValue.HasValue && previousValue.Value < depth)
                {
                    increaseCount += 1;
                }
                previousValue = depth;
            }

            return increaseCount.ToString();
        }

        public string Part2()
        {
            var increaseCount = 0;
            var depthArray = _depths.ToArray();

            for (var i = 0; i < depthArray.Length - 3; i++)
            {
                if (depthArray[i] < depthArray[i+3])
                {
                    increaseCount += 1;
                }
            }

            return increaseCount.ToString();
        }
    }
}
