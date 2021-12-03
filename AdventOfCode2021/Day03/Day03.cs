using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day03
{
    public class Day03 : IDay
    {
        private List<BinaryString> _numbers;
        public Day03()
        {
            _numbers = File.ReadAllLines("Day03/input.txt")
                .Select(l => new BinaryString(l))
                .ToList();
        }

        public int Day => 3;

        public void ExecutePart1()
        {
            var totalNumbers = _numbers.Count;
            var threshold = totalNumbers / 2;
            var gammaRate = "";
            var epsilonRate = "";

            foreach (var index in Enumerable.Range(0, _numbers.First().Length))
            {
                var totalOnes = _numbers.Sum(n => n.GetBit(index));
                if (totalOnes > threshold)
                {
                    gammaRate = $"{gammaRate}1";
                    epsilonRate = $"{epsilonRate}0";
                }
                else
                {
                    gammaRate = $"{gammaRate}0";
                    epsilonRate = $"{epsilonRate}1";
                }
            }

            var gamma = new BinaryString(gammaRate);
            var epsilon = new BinaryString(epsilonRate);

            var powerConsumption = gamma.GetValue() * epsilon.GetValue();

            Console.WriteLine(powerConsumption);
        }

        public void ExecutePart2()
        {
        }

        private class BinaryString
        {
            private readonly string _binaryString;

            public BinaryString(string binaryString)
            {
                _binaryString = binaryString;
            }

            public int Length => _binaryString.Length;
            public int GetBit(int index) => BitValue(_binaryString[index]);

            public int GetValue()
            {
                int value = 0;
                int exponent = 1;
                for(var i = _binaryString.Length - 1; i >= 0; i--)
                {
                    int bitValue = BitValue(_binaryString[i]);
                    value += bitValue * exponent;
                    exponent *= 2;
                }

                return value;
            }

            private int BitValue(char c) => c == '0' ? 0 : 1;
        }
    }
}
