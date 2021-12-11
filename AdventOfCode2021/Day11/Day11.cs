using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day11
{
    public class Day11 : IDay
    {
        public int DayNumber => 11;
        public string ValidatedPart1 => "1723";
        public string ValidatedPart2 => "327";

        private Octopus[][] _octopuses;

        private void LoadData()
        {
            _octopuses = File.ReadAllLines("Day11/input.txt")
                .Select(l => l.Select(o => new Octopus(int.Parse(o.ToString()))).ToArray())
                .ToArray();
        }
        
        public string Part1()
        {
            LoadData();

            var flashes = 0;
            foreach(var step in Enumerable.Range(1, 100))
            {
                foreach(var y in Enumerable.Range(0, _octopuses.Length))
                {
                    foreach (var x in Enumerable.Range(0, _octopuses[y].Length))
                    {
                        flashes += IncreaseEnergyLevel(x, y, step);
                    }
                }
            }

            return flashes.ToString();
        }

        private int IncreaseEnergyLevel(int x, int y, int step)
        {
            var flashed = _octopuses[y][x].IncreaseEnergyLevel(step);
            if (!flashed)
            {
                return 0;
            }

            var flashes = 1;

            foreach (var neighbourY in Enumerable.Range(y - 1, 3))
            {
                foreach (var neighbourX in Enumerable.Range(x - 1, 3))
                {
                    if (neighbourX >= 0 && neighbourX < _octopuses[y].Length
                        && neighbourY >= 0 && neighbourY < _octopuses.Length
                        && !(neighbourX == x && neighbourY == y))
                    {
                        flashes += IncreaseEnergyLevel(neighbourX, neighbourY, step);
                    }
                }
            }

            return flashes;
        }

        public string Part2()
        {
            LoadData();

            var totalOctopuses = _octopuses.Sum(row => row.Length);

            foreach (var step in Enumerable.Range(1, 10000))
            {
                var flashesThisStep = 0;
                foreach (var y in Enumerable.Range(0, _octopuses.Length))
                {
                    foreach (var x in Enumerable.Range(0, _octopuses[y].Length))
                    {
                        flashesThisStep += IncreaseEnergyLevel(x, y, step);
                    }
                }
                if (flashesThisStep == totalOctopuses)
                {
                    return step.ToString();
                }
            }

            return "NO SYNCHRONISATION";
        }

        private class Octopus
        {
            private int _energyLevel;
            private int _lastFlashedStep = -1;

            public Octopus(int energyLevel)
            {
                _energyLevel = energyLevel;
            }

            public bool IncreaseEnergyLevel(int step)
            {
                if (_lastFlashedStep == step)
                {
                    return false;
                }

                _energyLevel += 1;

                if (_energyLevel == 10)
                {
                    _energyLevel = 0;
                    _lastFlashedStep = step;
                    return true;
                }

                return false;
            }

            public override string ToString()
            {
                return _energyLevel.ToString();
            }
        }
    }
}
