using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day14
{
    public class Day14 : IDay
    {
        public int DayNumber => 14;
        public string ValidatedPart1 => "3342";
        public string ValidatedPart2 => "";

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
        
        public string Part1()
        {
            var polymer = new Polymer(_template);

            foreach(var i in Enumerable.Range(0, 10))
            {
                polymer = polymer.ApplyInsertionRules(_insertionRules);
            }

            var counts = polymer.Chemicals.GroupBy(c => c.Symbol).ToList();

            var mostCommonCount = counts.Max(g => g.Count());
            var leastCommonCount = counts.Min(g => g.Count());

            var result = mostCommonCount - leastCommonCount;
            return result.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class Polymer
        {
            public List<Chemical> Chemicals { get; private set; }

            public Polymer(string chemicals)
            {
                Chemicals = chemicals.Select(c => new Chemical(c)).ToList();
            }

            private Polymer(List<Chemical> chemicals)
            {
                Chemicals = chemicals;
            }

            public Polymer ApplyInsertionRules(List<InsertionRule> insertionRules)
            {
                var previous = Chemicals.First();

                var replacement = new List<Chemical>()
                {
                    previous
                };

                foreach (var c in Chemicals.Skip(1))
                {
                    foreach (var rule in insertionRules)
                    {
                        if (previous.Symbol == rule.Previous.Symbol && c.Symbol == rule.Next.Symbol)
                        {
                            replacement.Add(rule.Insertion);
                        }
                    }

                    replacement.Add(c);
                    previous = c;
                }

                return new Polymer(replacement);
            }

            public override string ToString()
            {
                return string.Join("", Chemicals.Select(c => c.Symbol));
            }
        }

        private class Chemical
        {
            public char Symbol { get; private set; }

            public Chemical(char symbol)
            {
                Symbol = symbol;
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
                Previous = new Chemical(split[0][0]);
                Next = new Chemical(split[0][1]);
                Insertion = new Chemical(split[1].First());
            }
        }
    }
}
