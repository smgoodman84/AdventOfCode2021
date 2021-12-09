using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day09
{
    public class Day09 : IDay
    {
        public int DayNumber => 9;
        public string ValidatedPart1 => "448";
        public string ValidatedPart2 => "";

        private Cave _cave;

        public Day09()
        {
            var heights = File.ReadAllLines("Day09/input.txt")
                .Select(line => line.ToArray().Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();

            _cave = new Cave(heights);
        }

        public string Part1()
        {
            var lowPoints = _cave.FindLowPoints().ToList();
            var totalRiskLevel = lowPoints.Sum(lp => lp + 1);
            return totalRiskLevel.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class Cave
        {
            private readonly int[][] _heights;

            public Cave(int[][] heights)
            {
                _heights = heights;
            }

            public IEnumerable<int> FindLowPoints()
            {
                foreach(var y in Enumerable.Range(0, _heights.Length))
                {
                    foreach (var x in Enumerable.Range(0, _heights[y].Length))
                    {
                        var thisHeight = _heights[y][x];
                        var left = x - 1 >= 0 ? _heights[y][x - 1] : 10;
                        var right = x + 1 < _heights[y].Length ? _heights[y][x + 1] : 10;
                        var up = y - 1 >= 0 ? _heights[y - 1][x] : 10;
                        var down = y + 1 < _heights.Length ? _heights[y + 1][x] : 10;
                        if (!(left <= thisHeight
                            || right <= thisHeight
                            || up <= thisHeight
                            || down <= thisHeight))
                        {
                            yield return thisHeight;
                        }
                    }
                }
            }
        }
    }
}
