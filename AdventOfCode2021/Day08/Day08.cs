using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day08
{
    public class Day08 : IDay
    {
        public int DayNumber => 8;
        public string ValidatedPart1 => "525";
        public string ValidatedPart2 => "";

        private List<Display> _displays;

        public Day08()
        {
            _displays = File.ReadAllLines("Day08/input.txt")
                .Select(l => new Display(l))
                .ToList();
        }

        public string Part1()
        {
            var valuesToCount = new[] { 1, 4, 7, 8 };

            var result = _displays
                .SelectMany(d => d.Outputs)
                .Where(o => valuesToCount.Contains(o.GetNumericValue()))
                .Count();

            return result.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class Display
        {
            private List<string> _signals;
            public List<Output> Outputs;

            public Display(string signalOutputs)
            {
                var split = signalOutputs.Split('|');
                _signals = split[0].Split(' ').ToList();
                Outputs = split[1].Split(' ').Select(o => new Output(o)).ToList();
            }
        }

        private class Output
        {
            public Output(string output)
            {
                Value = output;
            }

            public string Value { get; }

            public bool IsOne => Value.Length == 2;
            public bool IsFour => Value.Length == 4;
            public bool IsSeven => Value.Length == 3;
            public bool IsEight => Value.Length == 7;

            public int GetNumericValue()
            {
                if (IsOne) return 1;
                if (IsFour) return 4;
                if (IsSeven) return 7;
                if (IsEight) return 8;

                return -1;
            }
        }
    }
}
