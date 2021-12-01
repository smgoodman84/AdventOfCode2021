using System;
namespace AdventOfCode2021
{
    public interface IDay
    {
        public int Day { get; }
        public void ExecutePart1();
        public void ExecutePart2();
    }
}
