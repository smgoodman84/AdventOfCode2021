using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day15
{
    public class Day15 : IDay
    {
        public int DayNumber => 15;
        public string ValidatedPart1 => "361";
        public string ValidatedPart2 => "2838";

        public string Part1() => FindShortestPath(1, 1);
        public string Part2() => FindShortestPath(5, 5);

        public string FindShortestPath(int tileWidth, int tileHeight)
        {
            var fileNodes = File.ReadAllLines("Day15/input.txt")
                .SelectMany((l, y) => l.Select((d, x) => new Node(x, y, int.Parse(d.ToString()))).ToArray())
                .ToDictionary(n => n.Identifier, n => n);

            var fileNodeWidth = fileNodes.Values.Max(n => n.Coordinate.X) + 1;
            var fileNodeHeight = fileNodes.Values.Max(n => n.Coordinate.Y) + 1;

            var nodes = new Dictionary<string, Node>();

            for(var tileX = 0; tileX < tileWidth; tileX++)
            {
                for (var tileY = 0; tileY < tileHeight; tileY++)
                {
                    foreach (var node in fileNodes.Values)
                    {
                        var distance = WrapRisk(node.Distance + tileX + tileY);
                        var x = tileX * fileNodeWidth + node.Coordinate.X;
                        var y = tileY * fileNodeHeight + node.Coordinate.Y;

                        var newNode = new Node(x, y, distance);
                        nodes.Add(newNode.Identifier, newNode);
                    }
                }
            }

            foreach (var node in nodes.Values)
            {
                var coordinate = node.Coordinate;
                var leftKey = new Coordinate(coordinate.X - 1, coordinate.Y).ToString();
                var rightKey = new Coordinate(coordinate.X + 1, coordinate.Y).ToString();
                var upKey = new Coordinate(coordinate.X, coordinate.Y - 1).ToString();
                var downKey = new Coordinate(coordinate.X, coordinate.Y + 1).ToString();

                if (nodes.ContainsKey(leftKey))
                {
                    node.Neighbours.Add(nodes[leftKey]);
                }
                if (nodes.ContainsKey(rightKey))
                {
                    node.Neighbours.Add(nodes[rightKey]);
                }
                if (nodes.ContainsKey(upKey))
                {
                    node.Neighbours.Add(nodes[upKey]);
                }
                if (nodes.ContainsKey(downKey))
                {
                    node.Neighbours.Add(nodes[downKey]);
                }
            }

            var shortestPaths = new ShortestPaths(nodes.Values.ToList());

            var startX = nodes.Values.Min(n => n.Coordinate.X);
            var startY = nodes.Values.Min(n => n.Coordinate.Y);
            var endX = nodes.Values.Max(n => n.Coordinate.X);
            var endY = nodes.Values.Max(n => n.Coordinate.Y);

            var start = nodes[new Coordinate(startX, startY).ToString()];
            var end = nodes[new Coordinate(endX, endY).ToString()];

            var result = shortestPaths.GetShortestPath(start, end, 5000);

            return result.ToString();
        }

        private int WrapRisk(int risk)
        {
            if (risk > 9)
            {
                return risk - 9;
            }

            return risk;
        }


        private class SimplePriorityQueue
        {
            private List<string>[] _items;
            private Dictionary<string, int> _itemValues = new Dictionary<string, int>();

            public SimplePriorityQueue(int maxPriority)
            {
                _items = Enumerable.Range(0, maxPriority + 1)
                    .Select(x => new List<string>())
                    .ToArray();
            }

            public void SetPriority(string key, int value)
            {
                if (_itemValues.ContainsKey(key))
                {
                    Remove(key);
                }

                _itemValues.Add(key, value);
                _items[value].Add(key);
            }

            public void Remove(string key)
            {
                var value = _itemValues[key];
                _itemValues.Remove(key);
                _items[value].Remove(key);
            }

            public string Pop()
            {
                foreach(var itemList in _items)
                {
                    if (itemList.Any())
                    {
                        var result = itemList.First();
                        Remove(result);
                        return result;
                    }
                }

                return null;
            }
        }

        private class ShortestPaths
        {
            public ShortestPaths(List<Node> nodes)
            {
                _allNodes = nodes.ToDictionary(n => n.Identifier, n => n);
            }

            public int GetShortestPath(Node start, Node end, int maxDistance)
            {
                var queue = new SimplePriorityQueue(maxDistance);
                foreach (var node in _allNodes)
                {
                    queue.SetPriority(node.Key, maxDistance);
                }

                _incompleteNodes = _allNodes.ToDictionary(n => n.Key, n => n.Value);
                _previousNodes = _allNodes.ToDictionary(n => n.Key, n => (Node)null);
                _distances = _allNodes.ToDictionary(n => n.Key, n => maxDistance);

                _distances[start.Identifier] = 0;
                queue.SetPriority(start.Identifier, 0);

                while (_incompleteNodes.Any())
                {
                    var minimumIncompleteKey = queue.Pop();
                    _incompleteNodes.Remove(minimumIncompleteKey);

                    var minimumIncomplete = _allNodes[minimumIncompleteKey];
                    
                    foreach (var neighbour in minimumIncomplete.Neighbours)
                    {
                        if (_incompleteNodes.ContainsKey(neighbour.Identifier))
                        {
                            var alt = _distances[minimumIncompleteKey] + neighbour.Distance;
                            if (alt < _distances[neighbour.Identifier])
                            {
                                queue.SetPriority(neighbour.Identifier, alt);
                                _distances[neighbour.Identifier] = alt;
                                _previousNodes[neighbour.Identifier] = minimumIncomplete;
                            }
                        }
                    }
                }

                return _distances[end.Identifier];
            }

            private Dictionary<string, Node> _allNodes;
            private Dictionary<string, Node> _incompleteNodes;
            private Dictionary<string, Node> _previousNodes;
            private Dictionary<string, int> _distances;
        }

        private class Node
        {
            public Node(int x, int y, int distance)
            {
                Coordinate = new Coordinate(x, y);
                Distance = distance;
            }

            public List<Node> Neighbours { get; } = new List<Node>();
            public int Distance { get; set; }
            public Coordinate Coordinate { get; set; }
            public string Identifier => Coordinate.ToString();

            public override string ToString()
            {
                return $"{Coordinate} - {Distance}";
            }
        }

        private class Coordinate
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public Coordinate(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return $"{X},{Y}";
            }
        }
    }
}
