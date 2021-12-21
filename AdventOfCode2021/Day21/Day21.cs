using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2021.Day21
{
    public class Day21 : IDay
    {
        public int DayNumber => 21;
        public string ValidatedPart1 => "802452";
        public string ValidatedPart2 => "270005289024391";

        private List<string> _lines;
        private Dictionary<int, int> _startingPositions = new Dictionary<int, int>();

        public Day21()
        {
            _lines = File.ReadAllLines("Day21/input.txt").ToList();

            var positionParser = new Regex("Player (?<player>[0-9]*) starting position: (?<position>[0-9]*)");
            foreach (var line in _lines)
            {
                var matches = positionParser.Match(line);
                var player = int.Parse(matches.Groups["player"].Value);
                var position = int.Parse(matches.Groups["position"].Value);
                _startingPositions.Add(player, position);
            }
        }
        
        public string Part1()
        {
            DiracDiceContext winningContext = null;
            var die = new DeterministicDie();
            var context = GetInitialContext(1000);
            var winCounts = GetWinCounts(context, die, new Cache(), x => winningContext = x);

            var loser = 1 - winningContext.Winner;
            var rolls = winningContext.RollCount;
            var result = rolls * winningContext.Players[loser.Value].Score;
            return result.ToString();
        }

        public string Part2()
        {
            var die = new QuantumDie();
            var context = GetInitialContext(21);
            var winCounts = GetWinCounts(context, die, new Cache(), _ => { });

            var result = winCounts.Player1 > winCounts.Player2 ? winCounts.Player1 : winCounts.Player2;

            return result.ToString();
        }

        private DiracDiceContext GetInitialContext(int winningScore)
        {
            return new DiracDiceContext
            {
                Players = new PlayerContext[2]
                {
                    new PlayerContext
                    {
                        CurrentRollCount = 0,
                        Position = _startingPositions[1],
                        Score = 0
                    },
                    new PlayerContext
                    {
                        CurrentRollCount = 0,
                        Position = _startingPositions[2],
                        Score = 0
                    }
                },
                WinningScore = winningScore,
                CurrentPlayerToMove = 0,
                RollCount = 0,
                Winner = null
            };
        }

        private WinCount GetWinCounts(
            DiracDiceContext currentContext,
            IDie die,
            Cache cache,
            Action<DiracDiceContext> winningContextAction)
        {
            var result = new WinCount();

            foreach (var roll in die.GetNextRolls())
            {
                var winCount = GetWinCount(currentContext, die, cache, winningContextAction, roll);
                result.Player1 += winCount.Player1;
                result.Player2 += winCount.Player2;
            }

            return result;
        }

        private class WinCount
        {
            public long Player1 { get; set; }
            public long Player2 { get; set; }

            public static Dictionary<int, WinCount> SingleWinForPlayer => new Dictionary<int, WinCount>
            {
                { 0, new WinCount { Player1 = 1, Player2 = 0} },
                { 1, new WinCount { Player1 = 0, Player2 = 1} }
            };

            public override string ToString()
            {
                return $"{Player1}, {Player2}";
            }
        }

        private class Cache
        {
            public Dictionary<string, WinCount> WinCounts = new Dictionary<string, WinCount>();
        }

        private WinCount GetWinCount(
            DiracDiceContext initialContext,
            IDie die,
            Cache cache,
            Action<DiracDiceContext> winningContextAction,
            int roll)
        {
            var key = $"{initialContext.Key}|{roll}";

            if (cache.WinCounts.ContainsKey(key))
            {
                return cache.WinCounts[key];
            }

            var resultingContext = new DiracDiceContext
            {
                PreviousContext = initialContext,
                Players = new PlayerContext[2]
                {
                    new PlayerContext
                    {
                        CurrentRollCount = initialContext.Players[0].CurrentRollCount,
                        Position = initialContext.Players[0].Position,
                        Score = initialContext.Players[0].Score
                    },
                    new PlayerContext
                    {
                        CurrentRollCount = initialContext.Players[1].CurrentRollCount,
                        Position = initialContext.Players[1].Position,
                        Score = initialContext.Players[1].Score
                    }
                },
                WinningScore = initialContext.WinningScore,
                CurrentPlayerToMove = initialContext.CurrentPlayerToMove,
                RollCount = initialContext.RollCount,
                Winner = initialContext.Winner
            };

            var position = resultingContext.Players[resultingContext.CurrentPlayerToMove].Position + roll;
            if (position > 10)
            {
                position = position % 10;
                if (position == 0)
                {
                    position = 10;
                }
            }
            resultingContext.Players[resultingContext.CurrentPlayerToMove].Position = position;

            resultingContext.Players[resultingContext.CurrentPlayerToMove].CurrentRollCount += 1;
            resultingContext.RollCount += 1;
            if (resultingContext.Players[resultingContext.CurrentPlayerToMove].CurrentRollCount == 3)
            {
                var playerThatMoved = resultingContext.CurrentPlayerToMove;
                resultingContext.Players[resultingContext.CurrentPlayerToMove].Score += position;
                resultingContext.CurrentPlayerToMove = 1 - resultingContext.CurrentPlayerToMove;
                resultingContext.Players[resultingContext.CurrentPlayerToMove].CurrentRollCount = 0;

                if (resultingContext.Players[playerThatMoved].Score >= resultingContext.WinningScore)
                {
                    resultingContext.Winner = playerThatMoved;
                    winningContextAction(resultingContext);

                    cache.WinCounts[key] = WinCount.SingleWinForPlayer[playerThatMoved];
                    return cache.WinCounts[key];
                }
            }

            var winCountRecursive = GetWinCounts(resultingContext, die, cache, winningContextAction);
            cache.WinCounts[key] = winCountRecursive;
            return cache.WinCounts[key];
        }

        private class PlayerContext
        {
            public int Position { get; set; }
            public int Score { get; set; }
            public int CurrentRollCount { get; set; }

            public string Key => $"{Position},{Score},{CurrentRollCount}";

            public override string ToString()
            {
                return $"Pos: {Position}, Score: {Score}, RollCount: {CurrentRollCount}";
            }
        }

        private class DiracDiceContext
        {
            public DiracDiceContext PreviousContext { get; set; }

            public PlayerContext[] Players { get; set; }

            public int WinningScore { get; set; }

            public int CurrentPlayerToMove { get; set; }

            public int? Winner { get; set; }

            public int RollCount { get; set; }

            public string Key => $"{Players[0].Key}|{Players[1].Key}|{CurrentPlayerToMove}|{Winner}|{RollCount}";

            public override string ToString()
            {
                return $"{Players[0]} | {Players[1]} | Player: {CurrentPlayerToMove} | Winner: {Winner} | RollCount: {RollCount}";
            }
        }

        private interface IDie
        {
            public IEnumerable<int> GetNextRolls();
        }

        private class QuantumDie : IDie
        {
            private List<int> _rolls = new List<int> { 1, 2, 3 };
            public IEnumerable<int> GetNextRolls() => _rolls;
        }

        private class DeterministicDie : IDie
        {
            private int _lastValue = 100;
            public int RollCount { get; private set; } = 0;

            public IEnumerable<int> GetNextRolls()
            {
                if (_lastValue == 100)
                {
                    _lastValue = 1;
                }
                else
                {
                    _lastValue += 1;
                }

                RollCount += 1;

                yield return _lastValue;
            }
        }
    }
}
