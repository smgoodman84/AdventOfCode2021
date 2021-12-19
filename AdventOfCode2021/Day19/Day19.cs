using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2021.Day19
{
    public class Day19 : IDay
    {
        public int DayNumber => 19;
        public string ValidatedPart1 => "332";
        public string ValidatedPart2 => "";

        private List<Scanner> _scanners;

        public Day19()
        {
            _scanners = new List<Scanner>();
            var lines = File.ReadAllLines("Day19/input.txt").ToList();

            Scanner scanner = null;
            foreach (var line in lines.Append(""))
            {
                if (line.StartsWith("---"))
                {
                    scanner = new Scanner(line);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (scanner != null)
                    {
                        scanner.GenerateRotations();
                        _scanners.Add(scanner);
                    }
                    continue;
                }

                scanner.AddBeacon(new Coordinate(line));
            }
        }
        
        public string Part1()
        {
            /*
            var allCoordinates = _scanners
                .SelectMany(s => s.BeaconRotations.SelectMany(b => b.Value))
                .Select(c => c.ToString())
                .OrderBy(c => c)
                .ToList();
            */
            var map = new Map();
            map.InitialiseFromScanner(_scanners[0]);

            var unmergedScanners = _scanners.Skip(1).ToList();

            while (unmergedScanners.Any())
            {
                var remaining = new List<Scanner>();
                foreach (var scanner in unmergedScanners)
                {
                    if (!map.TryMerge(scanner))
                    {
                        remaining.Add(scanner);
                    }
                }
                unmergedScanners = remaining;
            }

            return map.BeaconCount.ToString();
        }

        public string Part2()
        {
            return "";
        }

        private class Map
        {
            public int BeaconCount => _beaconCoordinates.Count;

            private List<Coordinate> _beaconCoordinates = new List<Coordinate>();
            private Dictionary<string, List<int>> _beaconLookup = new Dictionary<string, List<int>>();

            public void InitialiseFromScanner(Scanner scanner)
            {
                AddBeacons(scanner.Beacons, scanner.Id);
            }

            private void AddBeacons(IEnumerable<Coordinate> beacons, int scannerId)
            {
                foreach(var beacon in beacons)
                {
                    AddBeacon(beacon, scannerId);
                }
            }

            private void AddBeacon(Coordinate beacon, int scannerId)
            {
                var key = beacon.ToString();
                if (!_beaconLookup.ContainsKey(key))
                {
                    _beaconLookup.Add(key, new List<int>());
                    _beaconCoordinates.Add(beacon);
                }
                _beaconLookup[key].Add(scannerId);
            }

            public bool TryMerge(Scanner scanner)
            {
                foreach (var mapCoordinate in _beaconCoordinates)
                {
                    foreach (var rotation in scanner.BeaconRotations.Keys)
                    {
                        var scannerCoordinates = scanner.BeaconRotations[rotation];
                        foreach (var scannerCoordinate in scannerCoordinates)
                        {
                            var transformX = mapCoordinate.X - scannerCoordinate.X;
                            var transformY = mapCoordinate.Y - scannerCoordinate.Y;
                            var transformZ = mapCoordinate.Z - scannerCoordinate.Z;

                            var transformedCoordinates = scannerCoordinates
                                .Select(c => new Coordinate(c.X + transformX, c.Y + transformY, c.Z + transformZ));

                            var matchCount = new Dictionary<int, int>();
                            foreach (var transformedCoordinate in transformedCoordinates)
                            {
                                var transformedCoordinateString = transformedCoordinate.ToString();
                                if (_beaconLookup.ContainsKey(transformedCoordinateString))
                                {
                                    foreach (var scannerId in _beaconLookup[transformedCoordinateString])
                                    {
                                        if (matchCount.ContainsKey(scannerId))
                                        {
                                            matchCount[scannerId] += 1;
                                            if (matchCount[scannerId] >= 12)
                                            {
                                                AddBeacons(transformedCoordinates, scannerId);
                                                return true;
                                            }
                                        }
                                        else
                                        {
                                            matchCount[scannerId] = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }
        }

        private class Scanner
        {
            public int Id { get; set; }
            public List<Coordinate> Beacons { get; set; } = new List<Coordinate>();
            public Dictionary<string, List<Coordinate>> BeaconRotations { get; set; } = new Dictionary<string, List<Coordinate>>();

            public Scanner(string heading)
            {
                var id = heading.Replace("--- scanner ", "")
                    .Replace(" ---", "");

                Id = int.Parse(id);
            }

            public void AddBeacon(Coordinate beacon)
            {
                Beacons.Add(beacon);
            }

            public void GenerateRotations()
            {
                var rotationA = Beacons.ToList();
                BeaconRotations.Add("rotationA", rotationA);

                var rotationB = rotationA.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rotationB", rotationB);

                var rotationC = rotationB.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rotationC", rotationC);

                var rotationD = rotationC.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rotationD", rotationD);



                var raForward = rotationA.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("raForward", raForward);

                var rbForward = rotationB.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("rbForward", rbForward);

                var rcForward = rotationC.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("rcForward", rcForward);

                var rdForward = rotationD.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("rdForward", rdForward);



                var raDown = raForward.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("raDown", raDown);

                var rbDown = rbForward.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("rbDown", rbDown);

                var rcDown = rcForward.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("rcDown", rcDown);

                var rdDown = rdForward.Select(b => b.UpToForward()).ToList();
                BeaconRotations.Add("rdDown", rdDown);



                var raLeft = raForward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("raLeft", raLeft);

                var rbLeft = rbForward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rbLeft", rbLeft);

                var rcLeft = rcForward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rcLeft", rcLeft);

                var rdLeft = rdForward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rdLeft", rdLeft);



                var raBackward = raLeft.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("raBackward", raBackward);

                var rbBackward = rbLeft.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rbBackward", rbBackward);

                var rcBackward = rcLeft.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rcBackward", rcBackward);

                var rdBackward = rdLeft.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rdBackward", rdBackward);



                var raRight = raBackward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("raRight", raRight);

                var rbRight = rbBackward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rbRight", rbRight);

                var rcRight = rcBackward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rcRight", rcRight);

                var rdRight = rdBackward.Select(b => b.LeftToBackward()).ToList();
                BeaconRotations.Add("rdRight", rdRight);
            }
        }

        private class Coordinate
        {
            // X - Left <-> Right +
            // Y - Down <-> Up +
            // Z - Backward <-> Forward +

            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public Coordinate(string coordinates)
            {
                var split = coordinates.Split(',');
                X = int.Parse(split[0]);
                Y = int.Parse(split[1]);
                Z = int.Parse(split[2]);
            }

            public Coordinate(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Coordinate LeftToBackward()
            {
                // Up/Down Stays Up/Down, but spins around Up/Down Axis
                return new Coordinate(-Z, Y, X);
            }

            public Coordinate UpToForward()
            {
                // Left/Right Stays Left/Right, but spins around Up Axis
                return new Coordinate(X, -Z, Y);
            }

            public override string ToString()
            {
                return $"{X},{Y},{Z}";
            }
        }
    }
}
