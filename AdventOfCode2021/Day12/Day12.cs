using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day12
{
    public class Day12 : IDay
    {
        public int DayNumber => 12;
        public string ValidatedPart1 => "4659";
        public string ValidatedPart2 => "148962";

        private List<Edge> _edges;

        public Day12()
        {
            _edges = File.ReadAllLines("Day12/input.txt")
                .Select(l => new Edge(l))
                .ToList();
        }

        public string Part1() => GetPathCount(false);
        public string Part2() => GetPathCount(true);

        private string GetPathCount(bool canVisitOneSmallTwice)
        {
            var allCaveNames = _edges.Select(e => e.Start)
                .Union(_edges.Select(e => e.End))
                .Distinct()
                .ToList();

            var caves = allCaveNames
                .Select(n => new Cave(n))
                .ToDictionary(c => c.Name, c => c);

            foreach (var edge in _edges)
            {
                caves[edge.Start].AddNeighbour(caves[edge.End]);
                caves[edge.End].AddNeighbour(caves[edge.Start]);
            }

            var startPath = new Path(caves["start"], allCaveNames);
            var inCompletePaths = startPath.FindPaths(canVisitOneSmallTwice);
            var completePaths = new List<Path>();

            while (inCompletePaths.Any())
            {
                var newPaths = inCompletePaths.SelectMany(p => p.FindPaths(canVisitOneSmallTwice)).ToList();
                completePaths.AddRange(newPaths.Where(p => p.IsComplete));
                inCompletePaths = newPaths.Where(p => !p.IsComplete).ToList();
            }

            var paths = completePaths
                .Select(p => p.ToString())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return paths.Count.ToString();
        }

        private class Path
        {
            public List<Cave> Caves { get; private set; }
            public Dictionary<string, int> Visited { get; private set; }
            public Cave Current { get; private set; }
            public bool IsComplete => Current.Name == "end";
            private bool HasVisitedSmallTwice = false;

            public Path(Cave cave, List<string> allCaveNames)
            {
                Caves = new List<Cave>
                {
                    cave
                };
                Visited = allCaveNames.ToDictionary(c => c, c => 0);
                Current = cave;
            }

            public Path(Path path, Cave cave, bool hasVisitedSmallTwice)
            {
                Caves = path.Caves.ToList();
                Caves.Add(cave);
                Visited = path.Visited.ToDictionary(v => v.Key, v => v.Value);
                Visited[cave.Name] += 1;
                Current = cave;
                HasVisitedSmallTwice = hasVisitedSmallTwice;
            }

            public IEnumerable<Path> FindPaths(bool canVisitOneSmallTwice)
            {
                if (!IsComplete)
                {
                    foreach (var neighbour in Current.Neighbours)
                    {
                        if (neighbour.Name != "start")
                        {
                            if (neighbour.IsBig)
                            {
                                yield return new Path(this, neighbour, HasVisitedSmallTwice);
                            }

                            if (neighbour.IsSmall && Visited[neighbour.Name] < 1)
                            {
                                yield return new Path(this, neighbour, HasVisitedSmallTwice);
                            }

                            if (canVisitOneSmallTwice && !HasVisitedSmallTwice && neighbour.IsSmall && Visited[neighbour.Name] < 2)
                            {
                                yield return new Path(this, neighbour, true);
                            }
                        }
                    }
                }
            }

            public override string ToString()
            {
                return string.Join(",", Caves.Select(c => c.Name));
            }
        }

        private class Cave
        {
            public string Name { get; private set; }
            public bool IsBig => Name == Name.ToUpper();
            public bool IsSmall => Name == Name.ToLower();
            public List<Cave> Neighbours { get; private set; }

            public Cave(string name)
            {
                Name = name;
                Neighbours = new List<Cave>();
            }

            public void AddNeighbour(Cave neighbour)
            {
                if (!Neighbours.Contains(neighbour))
                {
                    Neighbours.Add(neighbour);
                }
            }
        }

        private class Edge
        {
            public string Start { get; private set; }
            public string End { get; private set; }

            public Edge(string edge)
            {
                var split = edge.Split('-');
                Start = split[0];
                End = split[1];
            }
        }
    }
}
