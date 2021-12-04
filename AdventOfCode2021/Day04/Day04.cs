using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day04
{
    public class Day04 : IDay
    {
        private List<int> _numbers;
        private List<BingoBoard> _boards = new List<BingoBoard>();

        public Day04()
        {
            var input = File.ReadAllLines("Day04/input.txt").ToArray();

            _numbers = input
                .First()
                .Split(',')
                .Select(int.Parse)
                .ToList();

            var index = 2;
            while (index < input.Length)
            {
                _boards.Add(new BingoBoard(input.Skip(index).Take(5)));
                index += 6;
            }
        }

        public int Day => 4;

        public void ExecutePart1()
        {
            foreach(var number in _numbers)
            {
                foreach(var board in _boards)
                {
                    if (board.Call(number))
                    {
                        var result = board.GetSumOfUncalled() * number;
                        Console.WriteLine(result);
                        return;
                    }
                }
            }
        }

        public void ExecutePart2()
        {
            foreach (var board in _boards)
            {
                board.Reset();
            }

            var remainingBoards = _boards.ToList();

            foreach (var number in _numbers)
            {
                foreach (var board in remainingBoards)
                {
                    board.Call(number);
                }

                if (remainingBoards.Count > 1)
                {
                    remainingBoards = remainingBoards
                        .Where(board => board.Won == false)
                        .ToList();
                }

                if (remainingBoards.Count == 1)
                {
                    var finalBoard = remainingBoards.Single();

                    if (finalBoard.Won)
                    {
                        var result = finalBoard.GetSumOfUncalled() * number;
                        Console.WriteLine(result);
                        return;
                    }
                }
            }
        }

        private class BingoBoard
        {
            public bool Won { get; private set; } = false;
            private BingoNumber[][] _board;
            public BingoBoard(IEnumerable<string> lines)
            {
                _board = lines.Select(l =>
                        l.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .Select(n => new BingoNumber(n))
                        .ToArray()
                    )
                    .ToArray();
            }

            public int GetSumOfUncalled()
            {
                return _board.Sum(row => row.Sum(n => n.Called ? 0 : n.Number));
            }

            public void Reset()
            {
                Won = false;
                foreach(var row in _board)
                {
                    foreach (var number in row)
                    {
                        number.Reset();
                    }
                }
            }

            public bool Call(int number)
            {
                for (var row = 0; row < _board.Length; row++)
                {
                    for (var column = 0; column < _board[row].Length; column++)
                    {
                        if (_board[row][column].Call(number))
                        {
                            if (CheckWin(row, column))
                            {
                                Won = true;
                            }
                        }
                    }
                }

                return Won;
            }

            private bool CheckWin(int row, int column)
            {
                if (_board[row].All(n => n.Called))
                {
                    return true;
                }

                if (_board.Select(row => row[column]).All(n => n.Called))
                {
                    return true;
                }

                return false;
            }

            public override string ToString()
            {
                return Won.ToString();
            }
        }

        private class BingoNumber
        {
            public int Number { get; private set; }
            public bool Called { get; private set; } = false;

            public BingoNumber(int number)
            {
                Number = number;
            }

            public bool Call(int number)
            {
                if (number == Number)
                {
                    Called = true;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                Called = false;
            }
        }
    }
}
