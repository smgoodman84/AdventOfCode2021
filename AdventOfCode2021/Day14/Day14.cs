using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day14
{
    public class Day14 : IDay
    {
        public int DayNumber => 14;
        public string ValidatedPart1 => "3342";
        public string ValidatedPart2 => "3776553567525";

        private string _template;
        private List<InsertionRule> _insertionRules;

        public Day14()
        {
            var lines = File.ReadAllLines("Day14/input.txt")
                .ToList();

            _template = lines.First();

            _insertionRules = lines.Skip(2)
                .Select(l => new InsertionRule(l))
                .ToList();
        }

        public string Part1() => CalculateForIterations(10);
        public string Part2() => CalculateForIterations(40);

        private string CalculateForIterations(int iterations)
        {
            var chemicals = _template.Select(c => Chemical.Create(c)).ToList();

            var insertionDictionary = _insertionRules
                .GroupBy(r => r.Previous)
                .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.Next, x => x.Insertion));

            var previous = chemicals.First();
            var polymerSections = new List<PolymerSection>();

            foreach (var c in chemicals.Skip(1))
            {
                polymerSections.Add(PolymerSection.Create(previous, c, iterations, insertionDictionary));
                previous = c;
            }

            var first = true;
            var counts = new Dictionary<Chemical, long>();
            foreach(var section in polymerSections)
            {
                foreach (var count in section.ChemicalCounts)
                {
                    if (!counts.ContainsKey(count.Key))
                    {
                        counts[count.Key] = count.Value;
                    }
                    else
                    {
                        counts[count.Key] += count.Value;
                    }
                }

                if (!first)
                {
                    counts[section.Previous] -= 1;
                }

                first = false;
            }

            var mostCommonCount = counts.Max(g => g.Value);
            var leastCommonCount = counts.Min(g => g.Value);

            var result = mostCommonCount - leastCommonCount;
            return result.ToString();
        }

        private class PolymerSection
        {
            private static Dictionary<Chemical, Dictionary<Chemical, Dictionary<int, PolymerSection>>> _polymerSections
                = new Dictionary<Chemical, Dictionary<Chemical, Dictionary<int, PolymerSection>>>();

            public static PolymerSection Create(
                Chemical previous,
                Chemical next,
                int iterations,
                Dictionary<Chemical, Dictionary<Chemical, Chemical>> insertionRules
                )
            {
                if (!_polymerSections.ContainsKey(previous))
                {
                    _polymerSections.Add(previous, new Dictionary<Chemical, Dictionary<int, PolymerSection>>());
                }

                if (!_polymerSections[previous].ContainsKey(next))
                {
                    _polymerSections[previous].Add(next, new Dictionary<int, PolymerSection>());
                }

                if (!_polymerSections[previous][next].ContainsKey(iterations))
                {
                    _polymerSections[previous][next][iterations] = new PolymerSection(previous, next, iterations, insertionRules);
                }

                return _polymerSections[previous][next][iterations];
            }

            private PolymerSection(
                Chemical previous,
                Chemical next,
                int iterations,
                Dictionary<Chemical, Dictionary<Chemical, Chemical>> insertionRules)
            {
                Previous = previous;
                Next = next;
                Iterations = iterations;

                if (iterations == 0)
                {
                    Result = $"{previous}{next}";
                    if (previous == next)
                    {
                        ChemicalCounts = new Dictionary<Chemical, long>
                        {
                            { previous, 2 }
                        };
                    }
                    else
                    {
                        ChemicalCounts = new Dictionary<Chemical, long>
                        {
                            { previous, 1 },
                            { next, 1 },
                        };
                    }

                    return;
                }

                if (insertionRules.TryGetValue(previous, out var previousMatches))
                {
                    if (previousMatches.TryGetValue(next, out var insertion))
                    {
                        var section1 = PolymerSection.Create(previous, insertion, iterations - 1, insertionRules);
                        var section2 = PolymerSection.Create(insertion, next, iterations - 1, insertionRules);

                        if (iterations <= 10)
                        {
                            Result = $"{section1.Result}{section2.Result.Substring(1)}";
                        }
                        else
                        {
                            Result = "LARGE";
                        }

                        ChemicalCounts = section1.ChemicalCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                        foreach(var chemicalCount in section2.ChemicalCounts)
                        {
                            if (!ChemicalCounts.ContainsKey(chemicalCount.Key))
                            {
                                ChemicalCounts[chemicalCount.Key] = chemicalCount.Value;
                            }
                            else
                            {
                                ChemicalCounts[chemicalCount.Key] += chemicalCount.Value;
                            }
                        }

                        ChemicalCounts[insertion] -= 1;
                    }
                }
            }

            public Dictionary<Chemical, long> ChemicalCounts;
            public Chemical Previous;
            public Chemical Next;
            public int Iterations;
            public string Result;

            public override string ToString()
            {
                return $"{Previous}{Next}*{Iterations}={Result}";
            }
        }

        private class Chemical
        {
            private static Dictionary<char, Chemical> _chemicals = new Dictionary<char, Chemical>();

            public char Symbol { get; private set; }

            private Chemical(char symbol)
            {
                Symbol = symbol;
            }

            public static Chemical Create(char symbol)
            {
                if (!_chemicals.ContainsKey(symbol))
                {
                    _chemicals.Add(symbol, new Chemical(symbol));
                }

                return _chemicals[symbol];
            }

            public override string ToString()
            {
                return Symbol.ToString();
            }
        }

        private class InsertionRule
        {
            public Chemical Previous;
            public Chemical Next;
            public Chemical Insertion;

            public InsertionRule(string insertionRule)
            {
                var split = insertionRule.Split(" -> ");
                Previous = Chemical.Create(split[0][0]);
                Next = Chemical.Create(split[0][1]);
                Insertion = Chemical.Create(split[1].First());
            }
        }
    }
}
