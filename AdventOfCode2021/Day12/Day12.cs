using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day12
{
    public class Day12 : IDay
    {
        public int DayNumber => 12;
        public string ValidatedPart1 => "";
        public string ValidatedPart2 => "";

        private List<Edge> _edges;

        public Day12()
        {
            _edges = File.ReadAllLines("Day12/input.txt")
                .Select(l => new Edge(l))
                .ToList();
        }
        
        public string Part1()
        {
            var allCaveNames = _edges.Select(e => e.Start)
                .Union(_edges.Select(e => e.End))
                .Distinct();

            var caves = allCaveNames
                .Select(n => new Cave(n))
                .ToDictionary(c => c.Name, c => c);

            foreach (var edge in _edges)
            {
                caves[edge.Start].AddNeighbour(caves[edge.End]);
                caves[edge.End].AddNeighbour(caves[edge.Start]);
            }

            var startPath = new Path(caves["start"]);
            var inCompletePaths = startPath.FindPaths();
            var completePaths = new List<Path>();

            while (inCompletePaths.Any())
            {
                var newPaths = inCompletePaths.SelectMany(p => p.FindPaths()).ToList();
                completePaths.AddRange(newPaths.Where(p => p.IsComplete));
                inCompletePaths = newPaths.Where(p => !p.IsComplete).ToList();
            }

            return completePaths.Count.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class Path
        {
            public List<Cave> Caves { get; private set; }
            public HashSet<string> Visited { get; private set; }
            public Cave Current { get; private set; }
            public bool IsComplete => Current.Name == "end";

            public Path(Cave cave)
            {
                Caves = new List<Cave>
                {
                    cave
                };
                Visited = new HashSet<string>();
                Visited.Add(cave.Name);
                Current = cave;
            }

            public Path(Path path, Cave cave)
            {
                Caves = path.Caves.ToList();
                Caves.Add(cave);
                Visited = new HashSet<string>(path.Visited);
                Visited.Add(cave.Name);
                Current = cave;
            }

            public IEnumerable<Path> FindPaths()
            {
                if (!IsComplete)
                {
                    foreach (var neighbour in Current.Neighbours)
                    {
                        if (neighbour.IsBig || (neighbour.IsSmall && !Visited.Contains(neighbour.Name)))
                        {
                            yield return new Path(this, neighbour);
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
