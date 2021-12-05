using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day05
{
    public class Day05 : IDay
    {
        private List<Line> _lines;
        public Day05()
        {
            _lines = File.ReadAllLines("Day05/input.txt")
                .Select(l => new Line(l))
                .ToList();
        }

        public int Day => 5;

        public void ExecutePart1()
        {
            var lines = _lines
                .Where(l => l.IsHorizontal || l.IsVertical)
                .ToList();

            var allPoints = lines
                .SelectMany(l => l.GetPoints())
                .ToList();

            var counts = allPoints
                .GroupBy(c => c.ToString())
                .ToList();

            var result = counts
                .Where(g => g.Count() >= 2)
                .Count();

            Console.WriteLine(result);
        }

        public void ExecutePart2()
        {
        }

        private class Line
        {
            public Coordinate Start { get; private set; }
            public Coordinate End { get; private set; }

            public Line(string lineDefinition)
            {
                var coordinates = lineDefinition.Split(" -> ");
                Start = new Coordinate(coordinates[0]);
                End = new Coordinate(coordinates[1]);
            }

            public bool IsHorizontal => Start.X == End.X;
            public bool IsVertical => Start.Y == End.Y;

            public IEnumerable<Coordinate> GetPoints()
            {
                if (IsHorizontal)
                {
                    var startY = Start.Y;
                    var endY = End.Y;

                    if (Start.Y > End.Y)
                    {
                        startY = End.Y;
                        endY = Start.Y;
                    }

                    for (var y = startY; y <= endY; y++)
                    {
                        yield return new Coordinate(Start.X, y);
                    }
                }
                else if (IsVertical)
                {
                    var startX = Start.X;
                    var endX = End.X;

                    if (Start.X > End.X)
                    {
                        startX = End.X;
                        endX = Start.X;
                    }

                    for (var x = startX; x <= endX; x++)
                    {
                        yield return new Coordinate(x, Start.Y);
                    }
                }
            }


            public override string ToString()
            {
                var direction = "Unknown";
                if (IsHorizontal)
                {
                    direction = "Horizontal";
                }
                if (IsVertical)
                {
                    direction = "Vertical";
                }
                return $"{Start} -> {End} [{direction}]";
            }
        }

        private class Coordinate
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public Coordinate(string coordinate)
            {
                var split = coordinate.Split(',');
                X = int.Parse(split[0]);
                Y = int.Parse(split[1]);
            }

            public Coordinate(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"{X},{Y}";
            }
        }
    }
}
