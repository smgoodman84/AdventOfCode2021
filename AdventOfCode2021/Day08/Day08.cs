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
        public string ValidatedPart2 => "1083859";

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
            return _displays.Sum(d => d.GetOutput()).ToString();
        }

        private class Display
        {
            private List<string> _signals;
            public List<Output> Outputs;

            public Display(string signalOutputs)
            {
                var split = signalOutputs.Split('|');
                _signals = split[0].Trim().Split(' ').ToList();
                Outputs = split[1].Trim().Split(' ').Select(o => new Output(o)).ToList();
            }

            public int GetOutput()
            {
                var mapping = CalculateMapping();
                var result = 0;
                foreach(var output in Outputs)
                {
                    result *= 10;
                    result += GetValue(output, mapping);
                }
                return result;
            }

            private int GetValue(Output output, Dictionary<char, char> mapping)
            {
                var mappedOutput = string.Join("", output.Value.Select(c => mapping[c]).OrderBy(c => c));

                var characterMappings = new Dictionary<string, int>
                {
                    { "abcefg", 0 },
                    { "cf", 1 },
                    { "acdeg", 2 },
                    { "acdfg", 3 },
                    { "bcdf", 4 },
                    { "abdfg", 5 },
                    { "abdefg", 6 },
                    { "acf", 7 },
                    { "abcdefg", 8 },
                    { "abcdfg", 9 },
                };

                return characterMappings[mappedOutput];
            }

            private Dictionary<char,char> CalculateMapping()
            {
                var characterMappings = new Dictionary<int, string>
                {
                    {0, "abcefg" },
                    {1, "cf" },
                    {2, "acdeg" },
                    {3, "acdfg" },
                    {4, "bcdf" },
                    {5, "abdfg" },
                    {6, "abdefg" },
                    {7, "acf" },
                    {8, "abcdefg" },
                    {9, "abcdfg" },
                };

                var segmentIdentifiers = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };
                /*
                 * segment counts
                 * 0: 6
                 * 1: 2
                 * 2: 5
                 * 3: 5
                 * 4: 4
                 * 5: 5
                 * 6: 6
                 * 7: 3
                 * 8: 7
                 * 9: 6
                 * */
                /*
                 * length possibilities
                 * 2: 1
                 * 3: 7
                 * 4: 4
                 * 5: 2,3,5
                 * 6: 0,6,9
                 * 7: 8
                 * */
                var mappingPossibilities = new Dictionary<char, List<char>>();

                foreach (var id in segmentIdentifiers)
                {
                    mappingPossibilities.Add(id, segmentIdentifiers.ToList());
                }

                var one = _signals.Single(s => s.Length == 2);
                var oneChars = one.ToArray();
                foreach(var c in oneChars)
                {
                    mappingPossibilities[c] = mappingPossibilities[c].Where(x => characterMappings[1].Contains(x)).ToList();
                }

                var four = _signals.Single(s => s.Length == 4);
                var fourChars = four.ToArray();
                foreach (var c in fourChars)
                {
                    mappingPossibilities[c] = mappingPossibilities[c].Where(x => characterMappings[4].Contains(x)).ToList();
                }

                var seven = _signals.Single(s => s.Length == 3);
                var sevenChars = seven.ToArray();
                foreach (var c in sevenChars)
                {
                    mappingPossibilities[c] = mappingPossibilities[c].Where(x => characterMappings[7].Contains(x)).ToList();
                }

                /*
                 * times lit up
                 * a: 8
                 * b: 6
                 * c: 8
                 * d: 7
                 * e: 4
                 * f: 9
                 * g: 7
                 * */

                var timesLitUp = string
                    .Join("", _signals)
                    .GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count());

                var f = timesLitUp.Single(x => x.Value == 9).Key;
                var e = timesLitUp.Single(x => x.Value == 4).Key;
                var b = timesLitUp.Single(x => x.Value == 6).Key;

                mappingPossibilities[b] = new List<char> { 'b' };
                mappingPossibilities[e] = new List<char> { 'e' };
                mappingPossibilities[f] = new List<char> { 'f' };


                var removedSomething = true;
                while (removedSomething)
                {
                    removedSomething = false;
                    foreach (var i in "abcdefg")
                    {
                        if (mappingPossibilities[i].Count == 1)
                        {
                            var possibilityToRemove = mappingPossibilities[i].Single();
                            foreach (var j in "abcdefg")
                            {
                                if (j != i && mappingPossibilities[j].Contains(possibilityToRemove))
                                {
                                    mappingPossibilities[j] = mappingPossibilities[j].Where(x => x != possibilityToRemove).ToList();
                                    removedSomething = true;
                                }
                            }
                        }
                    }
                }

                return mappingPossibilities.ToDictionary
                    (
                    mp => mp.Key,
                    mp => mp.Value.Single()
                    );
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
