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
            var allPoints = _lines
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
                var dX = 0;
                if (Start.X > End.X)
                {
                    dX = -1;
                }
                if (Start.X < End.X)
                {
                    dX = 1;
                }

                var dY = 0;
                if (Start.Y > End.Y)
                {
                    dY = -1;
                }
                if (Start.Y < End.Y)
                {
                    dY = 1;
                }

                var currentX = Start.X;
                var currentY = Start.Y;

                yield return new Coordinate(currentX, currentY);
                while (currentX != End.X || currentY != End.Y)
                {
                    currentX += dX;
                    currentY += dY;
                    yield return new Coordinate(currentX, currentY);
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
