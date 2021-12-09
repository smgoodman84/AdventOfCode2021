using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day09
{
    public class Day09 : IDay
    {
        public int DayNumber => 9;
        public string ValidatedPart1 => "448";
        public string ValidatedPart2 => "1417248";

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
            var totalRiskLevel = lowPoints.Sum(lp => lp.Height + 1);
            return totalRiskLevel.ToString();
        }

        public string Part2()
        {
            var basins = _cave.FindBasins().ToList();
            var largestBasins = basins.OrderByDescending(b => b.Size).Take(3);
            var result = 1;
            foreach (var basin in largestBasins)
            {
                result *= basin.Size;
            }
            return result.ToString();
        }

        private class Cave
        {
            private readonly int[][] _heights;

            public Cave(int[][] heights)
            {
                _heights = heights;
            }

            public IEnumerable<LowPoint> FindLowPoints()
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
                            yield return new LowPoint(x, y, thisHeight);
                        }
                    }
                }
            }

            public IEnumerable<Basin> FindBasins()
            {
                var lowPoints = FindLowPoints().ToList();
                var basins = lowPoints.Select(lp => new Basin(_heights, lp)).ToList();
                return basins;
            }
        }

        private class LowPoint
        {
            public LowPoint(int x, int y, int height)
            {
                X = x;
                Y = y;
                Height = height;
            }

            public int X { get; }
            public int Y { get; }
            public int Height { get; }
        }

        private class Basin
        {
            public int Size { get; }
            private readonly int[][] _heights;
            private readonly LowPoint _lowPoint;
            private bool[][] _visited;

            public Basin(int[][] heights, LowPoint lowPoint)
            {
                _heights = heights;
                _lowPoint = lowPoint;
                _visited = _heights.Select(row => row.Select(x => false).ToArray()).ToArray();
                Size = CalculateSize();
            }

            private int CalculateSize()
            {
                _visited[_lowPoint.Y][_lowPoint.X] = true;
                return 1 + CheckFrom(_lowPoint.X, _lowPoint.Y);
            }

            private int CheckFrom(int x, int y)
            {
                var total = 0;
                if (x - 1 >= 0)
                {
                    var left = x - 1;
                    if (!_visited[y][left])
                    {
                        _visited[y][left] = true;
                        if (_heights[y][left] < 9)
                        {
                            total += 1 + CheckFrom(left, y);
                        }
                    }
                }

                if (x + 1 < _visited[y].Length)
                {
                    var right = x + 1;
                    if (!_visited[y][right])
                    {
                        _visited[y][right] = true;
                        if (_heights[y][right] < 9)
                        {
                            total += 1 + CheckFrom(right, y);
                        }
                    }
                }


                if (y - 1 >= 0)
                {
                    var up = y - 1;
                    if (!_visited[up][x])
                    {
                        _visited[up][x] = true;
                        if (_heights[up][x] < 9)
                        {
                            total += 1 + CheckFrom(x, up);
                        }
                    }
                }

                if (y + 1 < _visited.Length)
                {
                    var down = y + 1;
                    if (!_visited[down][x])
                    {
                        _visited[down][x] = true;
                        if (_heights[down][x] < 9)
                        {
                            total += 1 + CheckFrom(x, down);
                        }
                    }
                }

                return total;
            }
        }
    }
}
