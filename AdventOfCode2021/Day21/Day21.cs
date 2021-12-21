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
        public string ValidatedPart2 => "";

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
            var diracDice = new DiracDice(_startingPositions);
            var winner = diracDice.Play();
            var loser = diracDice.Players.Single(x => x.PlayerNumber != winner);
            var rolls = diracDice.GetDieRolls();
            var result = rolls * loser.Score;
            return result.ToString();
        }

        public string Part2()
        {
            return "";
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
