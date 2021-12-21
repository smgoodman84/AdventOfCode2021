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
            return "";/*
            var die = new DeterministicDie();
            var context = GetInitialContext(1000);
            while (context.Winner == null)
            {
                var roll = die.Roll();
                context = GetWinCount(context, roll, _ => { });
                //Console.WriteLine(context);
            }

            var loser = context.Players[1 - context.Winner.Value];
            var rolls = context.RollCount;
            var result = rolls * loser.Score;
            return result.ToString();*/
        }

        public string Part2()
        {
            var winCounts = GetWinCounts(GetInitialContext(21));

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

        private WinCount GetWinCounts(DiracDiceContext currentContext)
        {
            var one = GetWinCount(currentContext, 1);
            var two = GetWinCount(currentContext, 2);
            var three = GetWinCount(currentContext, 3);
            return new WinCount
            {
                Player1 = one.Player1 + two.Player1 + three.Player1,
                Player2 = one.Player2 + two.Player2 + three.Player2,
            };
        }

        private class WinCount
        {
            public long Player1 { get; set; }
            public long Player2 { get; set; }

            public override string ToString()
            {
                return $"{Player1}, {Player2}";
            }
        }

        private Dictionary<string, WinCount> _winCounts = new Dictionary<string, WinCount>();


        private Dictionary<int, WinCount> _winCountForPlayer = new Dictionary<int, WinCount>
        {
            { 0, new WinCount { Player1 = 1, Player2 = 0} },
            { 1, new WinCount { Player1 = 0, Player2 = 1} }
        };

        private WinCount GetWinCount(DiracDiceContext initialContext, int roll)
        {
            var key = $"{initialContext.Key}|{roll}";

            if (_winCounts.ContainsKey(key))
            {
                return _winCounts[key];
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
                    var winCountSingle = _winCountForPlayer[resultingContext.Winner.Value];
                    _winCounts[key] = winCountSingle;
                    if (initialContext.RollCount < 2)
                    {
                        Console.WriteLine($"Wincount Single {key}: {winCountSingle}");
                        /*
                        var ctx = resultingContext;
                        while (ctx != null)
                        {
                            Console.WriteLine(ctx);
                            ctx = ctx.PreviousContext;
                        }
                        */
                    }
                    return winCountSingle;
                }
            }

            var winCountRecursive = GetWinCounts(resultingContext);
            _winCounts[key] = winCountRecursive;
            if (initialContext.RollCount < 2)
            {
                Console.WriteLine($"Wincount {key}: {winCountRecursive}");
            }
            return winCountRecursive;
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


        private class DiracDice
        {
            public List<Player> Players = new List<Player>();
            private Player _currentPlayer;
            private DeterministicDie _die = new DeterministicDie();

            public DiracDice(Dictionary<int, int> initialPositions)
            {
                Player previousPlayer = null;
                Player firstPlayer = null;

                foreach (var position in initialPositions)
                {
                    var player = new Player()
                    {
                        PlayerNumber = position.Key,
                        Score = 0,
                        Position = position.Value
                    };
                    Players.Add(player);

                    if (firstPlayer == null)
                    {
                        firstPlayer = player;
                        _currentPlayer = firstPlayer;
                    }

                    if (previousPlayer != null)
                    {
                        previousPlayer.NextPlayer = player;
                    }

                    previousPlayer = player;
                }

                previousPlayer.NextPlayer = firstPlayer;
            }

            public int Play()
            {
                while (true)
                {
                    var rollOne = _die.Roll();
                    var rollTwo = _die.Roll();
                    var rollThree = _die.Roll();
                    var advanceBy = rollOne + rollTwo + rollThree;
                    _currentPlayer.Position += advanceBy;
                    if (_currentPlayer.Position > 10)
                    {
                        _currentPlayer.Position = _currentPlayer.Position % 10;
                        if (_currentPlayer.Position == 0)
                        {
                            _currentPlayer.Position = 10;
                        }
                    }
                    _currentPlayer.Score += _currentPlayer.Position;

                    Console.WriteLine($"Player {_currentPlayer.PlayerNumber} Rolled {rollOne},{rollTwo},{rollThree}={advanceBy} Moved to {_currentPlayer.Position} Score {_currentPlayer.Score} RollCount {_die.RollCount}");

                    if (_currentPlayer.Score >= 1000)
                    {
                        return _currentPlayer.PlayerNumber;
                    }

                    _currentPlayer = _currentPlayer.NextPlayer;
                }
            }

            public int GetDieRolls() => _die.RollCount;
        }

        private class Player
        {
            public int PlayerNumber { get; set; }
            public int Score { get; set; }
            public int Position { get; set; }
            public Player NextPlayer { get; set; }
        }

        private class DeterministicDie
        {
            private int _lastValue = 100;
            public int RollCount { get; private set; } = 0;

            public int Roll()
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

                return _lastValue;
            }
        }
    }
}
