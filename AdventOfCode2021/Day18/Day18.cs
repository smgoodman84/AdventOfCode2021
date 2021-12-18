using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day18
{
    public class Day18 : IDay
    {
        public int DayNumber => 18;
        public string ValidatedPart1 => "3359";
        public string ValidatedPart2 => "";

        private List<string> _lines;

        public Day18()
        {
            _lines = File.ReadAllLines("Day18/input.txt").ToList();
        }
        
        public string Part1()
        {
            var pairs = _lines.Select(l => Parse(l)).ToList();

            var sum = pairs.First();
            foreach (var pair in pairs.Skip(1))
            {
                sum = Add(sum, pair);
            }

            var magnitude = sum.GetMagnitude();

            return magnitude.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private IPair Add(IPair left, IPair right)
        {
            var sum = new Pair
            {
                Left = left,
                Right = right,
                Depth = 1
            };

            left.Parent = sum;
            right.Parent = sum;

            IncreaseDepth(left);
            IncreaseDepth(right);

            Reduce(sum);

            return sum;
        }

        private void IncreaseDepth(IPair pair)
        {
            pair.Depth += 1;

            var p = pair as Pair;
            if (p != null)
            {
                IncreaseDepth(p.Left);
                IncreaseDepth(p.Right);
            }
        }


        private bool Reduce(IPair pair)
        {
            var somethingHappened = false;
            if (Explode(pair))
            {
                somethingHappened = true;
                Reduce(pair);
            }

            if (Split(pair))
            {
                somethingHappened = true;
                Reduce(pair);
            }

            return somethingHappened;
        }

        private bool Explode(IPair pair)
        {
            var exploder = FindExploder(pair);
            if (exploder == null)
            {
                return false;
            }

            var leftNeighbour = FindLeftNeighbour(exploder);
            var rightNeighbour = FindRightNeighbour(exploder);

            if (leftNeighbour != null)
            {
                leftNeighbour.Value += (exploder.Left as Literal).Value;
            }

            if (rightNeighbour != null)
            {
                rightNeighbour.Value += (exploder.Right as Literal).Value;
            }

            var parent = exploder.Parent;
            var zero = new Literal
            {
                Depth = parent.Depth + 1,
                Parent = parent,
                Value = 0
            };

            if (parent.Left == exploder)
            {
                parent.Left = zero;
            }
            else
            {
                parent.Right = zero;
            }

            return true;
        }

        private Literal FindRightNeighbour(IPair pair)
        {
            if (pair.Parent == null)
            {
                return null;
            }

            var parent = pair.Parent;
            if (parent.Right == pair)
            {
                return FindRightNeighbour(parent);
            }

            return FindLeftmost(parent.Right);
        }

        private Literal FindLeftNeighbour(IPair pair)
        {
            if (pair.Parent == null)
            {
                return null;
            }

            var parent = pair.Parent;
            if (parent.Left == pair)
            {
                return FindLeftNeighbour(parent);
            }

            return FindRightmost(parent.Left);
        }

        private Literal FindRightmost(IPair pair)
        {
            if (pair is Literal)
            {
                return pair as Literal;
            }

            var p = pair as Pair;
            return FindRightmost(p.Right);
        }

        private Literal FindLeftmost(IPair pair)
        {
            if (pair is Literal)
            {
                return pair as Literal;
            }

            var p = pair as Pair;
            return FindLeftmost(p.Left);
        }

        private Pair FindExploder(IPair pair)
        {
            if (pair.Depth == 5)
            {
                return pair as Pair;
            }

            var p = pair as Pair;
            if (p == null)
            {
                return null;
            }

            return FindExploder(p.Left) ?? FindExploder(p.Right);
        }

        private bool Split(IPair pair)
        {
            var splitter = FindSplitter(pair);
            if (splitter == null)
            {
                return false;
            }

            var value = splitter.Value;
            var leftValue = value / 2;
            var rightValue = value / 2;

            if (value % 2 == 1)
            {
                rightValue += 1;
            }

            var left = new Literal
            {
                Depth = splitter.Depth + 1,
                Value = leftValue
            };

            var right = new Literal
            {
                Depth = splitter.Depth + 1,
                Value = rightValue
            };

            var replacement = new Pair
            {
                Depth = splitter.Depth,
                Parent = splitter.Parent,
                Left = left,
                Right = right
            };

            right.Parent = replacement;
            left.Parent = replacement;

            if (splitter.Parent.Left == splitter)
            {
                splitter.Parent.Left = replacement;
            }
            else
            {
                splitter.Parent.Right = replacement;
            }

            return true;
        }

        private Literal FindSplitter(IPair pair)
        {
            var l = pair as Literal;
            if (l != null)
            {
                if (l.Value >= 10)
                {
                    return l;
                }

                return null;
            }

            var p = pair as Pair;
            if (p == null)
            {
                return null;
            }

            return FindSplitter(p.Left) ?? FindSplitter(p.Right);
        }

        private IPair Parse(string expression, int depth = 1)
        {
            if (expression[0] != '[' && int.TryParse(expression, out var intValue))
            {
                return new Literal
                {
                    Value = intValue,
                    Depth = depth
                };
            }

            var leftBracketCount = 0;
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];

                if (c == '[')
                {
                    leftBracketCount += 1;
                }

                if (c == ']')
                {
                    leftBracketCount -= 1;
                }

                if (c == ',' && leftBracketCount == 1)
                {
                    var leftString = expression.Substring(1, i - 1);
                    var rightString = expression.Substring(i + 1, expression.Length - i - 2);

                    var left = Parse(leftString, depth + 1);
                    var right = Parse(rightString, depth + 1);

                    var result = new Pair
                    {
                        Left = left,
                        Right = right,
                        Depth = depth
                    };

                    result.Left.Parent = result;
                    result.Right.Parent = result;

                    return result;
                }
            }

            throw new Exception("Unexpected");
        }

        private interface IPair
        {
            public Pair Parent { get; set; }
            public int Depth { get; set; }
            public int GetMagnitude();
        }

        private class Literal : IPair
        {
            public int Value { get; set; }

            public Pair Parent { get; set; }
            public int Depth { get; set; }

            public int GetMagnitude() => Value;

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private class Pair : IPair
        {
            public IPair Left { get; set; }
            public IPair Right { get; set; }

            public Pair Parent { get; set; }
            public int Depth { get; set; }

            public int GetMagnitude() => Left.GetMagnitude() * 3 + Right.GetMagnitude() * 2;

            public override string ToString()
            {
                return $"[{Left},{Right}]";
            }
        }
    }
}
