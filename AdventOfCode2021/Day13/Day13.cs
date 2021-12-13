using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day13
{
    public class Day13 : IDay
    {
        public int DayNumber => 13;
        public string ValidatedPart1 => "737";
        public string ValidatedPart2 => "";

        private List<Coordinate> _coordinates;
        private List<Fold> _folds;

        public Day13()
        {
            var lines = File.ReadAllLines("Day13/input.txt")
                .ToList();

            _coordinates = lines.Where(l => l.Contains(','))
                .Select(l => new Coordinate(l))
                .ToList();

            _folds = lines.Where(l => l.StartsWith("fold"))
                .Select(l => new Fold(l))
                .ToList();
        }

        public string Part1()
        {
            var firstFold = _folds.First();

            foreach(var coordinate in _coordinates)
            {
                coordinate.ApplyFold(firstFold);
            }

            var uniqueCoordinates = _coordinates.Select(c => c.ToString()).Distinct();

            return uniqueCoordinates.Count().ToString();
        }

        public string Part2()
        {
            return "";
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

            public void ApplyFold(Fold fold)
            {
                switch(fold.FoldDirection)
                {
                    case FoldDirection.Horizontal:
                        FoldHorizontal(fold.Location);
                        break;
                    case FoldDirection.Vertical:
                        FoldVertical(fold.Location);
                        break;
                }
            }

            private void FoldHorizontal(int y)
            {
                if (Y > y)
                {
                    Y = y - (Y - y);
                }
            }

            private void FoldVertical(int x)
            {
                if (X > x)
                {
                    X = x - (X - x);
                }
            }

            public override string ToString()
            {
                return $"{X},{Y}";
            }
        }

        private class Fold
        {
            public FoldDirection FoldDirection { get; private set; }
            public int Location { get; private set; }

            public Fold(string fold)
            {
                var split = fold.Replace("fold along ", "")
                    .Split('=');

                FoldDirection = split[0] == "x" ? FoldDirection.Vertical : FoldDirection.Horizontal;
                Location = int.Parse(split[1]);
            }
        }

        private enum FoldDirection
        {
            Horizontal,
            Vertical
        }
    }
}
