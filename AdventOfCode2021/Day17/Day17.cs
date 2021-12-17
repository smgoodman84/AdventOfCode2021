using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2021.Day17
{
    public class Day17 : IDay
    {
        public int DayNumber => 17;
        public string ValidatedPart1 => "";
        public string ValidatedPart2 => "";

        private int _left;
        private int _right;
        private int _top;
        private int _bottom;
        private int _xMiddle;

        public Day17()
        {
            var targetParser = new Regex("target area: x=(?<left>[0-9]*)..(?<right>[0-9]*), y=(?<bottom>-?[0-9]*)..(?<top>-?[0-9]*)");
            var line = File.ReadAllLines("Day17/input.txt").First();

            var matches = targetParser.Match(line);
            _left = int.Parse(matches.Groups["left"].Value);
            _right = int.Parse(matches.Groups["right"].Value);
            _top = int.Parse(matches.Groups["top"].Value);
            _bottom = int.Parse(matches.Groups["bottom"].Value);
            _xMiddle = (_left + _right) / 2;
        }

        private class ProbeState
        {
            public int XPosition { get; set; }
            public int YPosition { get; set; }
            public int XVelocity { get; set; }
            public int YVelocity { get; set; }
            public ProbeState Previous { get; internal set; }

            public ProbeState GetNextState()
            {
                var xVelocity = 0;

                if (XVelocity > 0)
                {
                    xVelocity = XVelocity - 1;
                }

                if (XVelocity < 0)
                {
                    xVelocity = XVelocity + 1;
                }

                return new ProbeState
                {
                    XPosition = XPosition + XVelocity,
                    YPosition = YPosition + YVelocity,
                    XVelocity = xVelocity,
                    YVelocity = YVelocity - 1
                };
            }

            public int GetMaxHeight()
            {
                if (Previous == null)
                {
                    return YPosition;
                }

                var previousMax = Previous.GetMaxHeight();
                return previousMax > YPosition ? previousMax : YPosition;
            }
        }

        public string Part1()
        {
            var yResults = Enumerable.Range(0, 5000)
                .Select(i => (i, GetResultForYVelocity(i)))
                .ToList();

            var hits = yResults
                .Where(x => x.Item2.HitResultType == HitResultType.Hit)
                .ToList();

            var max = hits
                .OrderByDescending(x => x.i)
                .First();

            var maxHeight = max.Item2.FinalState.GetMaxHeight();

            return maxHeight.ToString();
        }


        private HitResult GetResultForYVelocity(int yVelocity)
        {
            var xVelocity = 10;
            bool? increasing = null;

            while (true)
            {
                var result = GetResult(xVelocity, yVelocity);
                if (result.HitResultType == HitResultType.Hit)
                {
                    return result;
                }

                var newxVelocity = (int)( ((double)xVelocity) * ( ((double)_xMiddle) / ((double)result.FinalState.XPosition) ));

                if (newxVelocity == xVelocity)
                {
                    return result;
                }

                if ((increasing.HasValue && increasing.Value && newxVelocity < xVelocity)
                    || (increasing.HasValue && !increasing.Value && newxVelocity > xVelocity))
                {
                    var start = increasing.Value ? newxVelocity : xVelocity;
                    var end = increasing.Value ? xVelocity : newxVelocity;

                    for (var x = start; x <= end; x++)
                    {
                        var result2 = GetResult(x, yVelocity);
                        if (result2.HitResultType == HitResultType.Hit)
                        {
                            return result2;
                        }
                    }

                    return result;
                }

                increasing = newxVelocity > xVelocity;
                xVelocity = newxVelocity;
            }
        }

        public string Part2()
        {
            return "";
        }

        private HitResult GetResult(int xVelocity, int yVelocity)
        {
            var state = new ProbeState
            {
                XPosition = 0,
                YPosition = 0,
                XVelocity = xVelocity,
                YVelocity = yVelocity
            };

            var hitResult = HitResultType.OnCourse;
            while (hitResult == HitResultType.OnCourse)
            {
                var previousState = state;
                state = state.GetNextState();
                state.Previous = previousState;
                hitResult = GetHitResult(state);
            }

            return new HitResult
            {
                HitResultType = hitResult,
                FinalState = state
            };
        }

        private HitResultType GetHitResult(ProbeState probeState)
        {
            if (probeState.YPosition < _bottom)
            {
                if (probeState.XPosition < _left)
                {
                    return HitResultType.MissLeft;
                }

                if (probeState.XPosition > _right)
                {
                    return HitResultType.MissLeft;
                }

                return HitResultType.MissMiddle;
            }

            if (probeState.YPosition > _top)
            {
                return HitResultType.OnCourse;
            }

            if (probeState.XPosition > _right)
            {
                return HitResultType.MissRight;
            }

            if (probeState.XPosition < _left)
            {
                return HitResultType.OnCourse;
            }

            return HitResultType.Hit;
        }

        private class HitResult
        {
            public HitResultType HitResultType { get; set; }
            public ProbeState FinalState { get; set; }
        }

        private enum HitResultType
        {
            OnCourse,
            MissLeft,
            MissMiddle,
            MissRight,
            Hit
        }
    }
}
