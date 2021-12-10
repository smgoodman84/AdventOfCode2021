using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day10
{
    public class Day10 : IDay
    {
        public int DayNumber => 10;
        public string ValidatedPart1 => "268845";
        public string ValidatedPart2 => "";

        private List<NavigationParser> _lines;

        public Day10()
        {
            _lines = File.ReadAllLines("Day10/input.txt")
                .Select(l => new NavigationParser(l))
                .ToList();
        }
        
        public string Part1()
        {
            var result = _lines.Sum(l => l.GetSyntaxErrorScore());

            return result.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class NavigationParser
        {
            private readonly string _input;

            public NavigationParser(string input)
            {
                _input = input;
            }

            public int GetSyntaxErrorScore()
            {
                var stack = new Stack<char>();

                foreach (var c in _input)
                {
                    if (new[] { '(', '[', '{', '<' }.Contains(c))
                    {
                        stack.Push(c);
                        continue;
                    }

                    var previous = stack.Pop();

                    if (c == ')' && previous != '(')
                    {
                        return 3;
                    }
                    if (c == ']' && previous != '[')
                    {
                        return 57;
                    }
                    if (c == '}' && previous != '{')
                    {
                        return 1197;
                    }
                    if (c == '>' && previous != '<')
                    {
                        return 25137;
                    }
                }

                return 0;
            }
        }
    }
}
