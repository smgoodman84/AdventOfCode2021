using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AdventOfCode2021.Day16
{
    public class Day16 : IDay
    {
        public int DayNumber => 16;
        public string ValidatedPart1 => "";
        public string ValidatedPart2 => "";

        private string _hexTransmission;

        public Day16()
        {
            _hexTransmission = File.ReadAllLines("Day16/input.txt").First();
        }

        private enum SymbolType
        {
            Parent,
            HeaderVersion,
            HeaderType,
            Literal,
            OperatorLengthType,
            TotalPacketLength,
            SubPacketCount
        }

        private class Symbol
        {
            public Symbol(SymbolType type, IEnumerable<int> value)
            {
                Type = type;
                RawValue = value.ToArray();
                ConvertToInt();
            }

            public SymbolType Type { get; set; }
            public int[] RawValue { get; set; }
            public int IntValue { get; set; }
            public List<Symbol> Children = new List<Symbol>();

            public void AddBits(IEnumerable<int> newBits)
            {
                RawValue = RawValue.Concat(newBits).ToArray();
                ConvertToInt();
            }

            private void ConvertToInt()
            {
                var result = 0;
                foreach (var i in RawValue)
                {
                    result *= 2;
                    result += i;
                }
                IntValue = result;
            }

            public override string ToString()
            {
                return $"{Type} {IntValue}";
            }
        }

        private enum LengthType
        {
            Bits,
            Packets
        }

        private class Packet
        {
            public Packet Parent { get; set; }
            public LengthType LengthType { get; set; }
            public int End { get; set; }
            public List<Symbol> Symbols { get; set; } = new List<Symbol>();
            public List<Packet> Packets { get; set; } = new List<Packet>();
        }

        public string Part1()
        {
            var binaryData = ExpandHex(_hexTransmission).ToArray();

            var packetStack = new Stack<Packet>();
            Packet packet = new Packet()
            {
                LengthType = LengthType.Packets,
                End = 1
            };
            var headPacket = packet;

            var currentLiteral = new Symbol(SymbolType.Literal, new int[0]);

            var symbolType = SymbolType.HeaderVersion;
            var bitsToRead = 3;

            var index = 0;
            while(bitsToRead > 0)
            {
                var symbol = new int[bitsToRead];
                var symbolIndex = 0;
                while (symbolIndex < bitsToRead)
                {
                    symbol[symbolIndex] = binaryData[index];
                    symbolIndex += 1;
                    index += 1;
                }

                /*
                if (packet.LengthType == LengthType.Bits && index == packet.End)
                {
                    symbolType = SymbolType.HeaderVersion;
                    var isFinished = true;
                    while (isFinished && packetStack.Any())
                    {
                        packet = packetStack.Pop();
                        if (packet.LengthType == LengthType.Bits && index < packet.End)
                        {
                            isFinished = false;
                        }
                        if (packet.LengthType == LengthType.Packets
                            && packet.Parent != null
                            && packet.Parent.Packets.Count < packet.End)
                        {
                            isFinished = false;
                        }
                    }

                    bitsToRead = isFinished ? 0 : 3;
                }
                */
                switch (symbolType)
                {
                    case SymbolType.HeaderVersion:
                        packet.Symbols.Add(new Symbol(SymbolType.HeaderVersion, symbol));

                        symbolType = SymbolType.HeaderType;
                        bitsToRead = 3;
                        break;

                    case SymbolType.HeaderType:
                        var headerTypeSymbol = new Symbol(SymbolType.HeaderType, symbol);
                        packet.Symbols.Add(headerTypeSymbol);

                        var packetType = headerTypeSymbol.IntValue;
                        switch (packetType)
                        {
                            case 4:
                                symbolType = SymbolType.Literal;
                                bitsToRead = 5;
                                break;
                            default:
                                symbolType = SymbolType.OperatorLengthType;
                                bitsToRead = 1;
                                break;

                        }
                        break;

                    case SymbolType.Literal:
                        currentLiteral.AddBits(symbol.Skip(1).ToArray());
                        if (symbol[0] == 0)
                        {
                            packet.Symbols.Add(currentLiteral);
                            currentLiteral = new Symbol(SymbolType.Literal, new int[0]);

                            symbolType = SymbolType.HeaderVersion;

                            if (packet.LengthType == LengthType.Bits)
                            {
                                if (index < packet.End)
                                {
                                    packet = new Packet()
                                    {
                                        Parent = packet.Parent,
                                        LengthType = LengthType.Bits,
                                        End = packet.End
                                    };

                                    packet.Parent.Packets.Add(packet);
                                    bitsToRead = 3;
                                }
                                else
                                {
                                    var isFinished = true;
                                    while (isFinished && packetStack.Any())
                                    {
                                        isFinished = true;
                                        packet = packetStack.Pop();
                                        if (packet.LengthType == LengthType.Bits && index < packet.End)
                                        {
                                            isFinished = false;
                                        }
                                        if (packet.LengthType == LengthType.Packets
                                            && packet.Packets.Count < packet.End)
                                        {
                                            isFinished = false;
                                        }
                                    }

                                    bitsToRead = isFinished ? 0 : 3;
                                }

                            }
                            else if (packet.LengthType == LengthType.Packets)
                            {
                                if (packet.Parent.Packets.Count == packet.End)
                                {
                                    var isFinished = true;
                                    while (isFinished && packetStack.Any())
                                    {
                                        isFinished = true;
                                        packet = packetStack.Pop();
                                        if (packet.LengthType == LengthType.Bits && index < packet.End)
                                        {
                                            isFinished = false;
                                        }
                                        if (packet.LengthType == LengthType.Packets
                                            && packet.Packets.Count < packet.End)
                                        {
                                            isFinished = false;
                                        }
                                    }

                                    bitsToRead = isFinished ? 0 : 3;
                                }
                                else if (packet.Parent.Packets.Count < packet.End)
                                {
                                    packet = new Packet()
                                    {
                                        Parent = packet.Parent,
                                        LengthType = LengthType.Packets,
                                        End = packet.End
                                    };

                                    packet.Parent.Packets.Add(packet);
                                    bitsToRead = 3;
                                }
                            }
                        }
                        break;

                    case SymbolType.OperatorLengthType:
                        var operatorLengthType = new Symbol(SymbolType.OperatorLengthType, symbol);
                        packet.Symbols.Add(operatorLengthType);

                        switch(operatorLengthType.IntValue)
                        {
                            case 0:
                                symbolType = SymbolType.TotalPacketLength;
                                bitsToRead = 15;
                                break;
                            case 1:
                                symbolType = SymbolType.SubPacketCount;
                                bitsToRead = 11;
                                break;
                        }
                        break;

                    case SymbolType.TotalPacketLength:
                        var totalPacketLength = new Symbol(SymbolType.TotalPacketLength, symbol);
                        packet.Symbols.Add(totalPacketLength);

                        //packet.LengthType = LengthType.Bits;
                        //packet.End = index + totalPacketLength.IntValue;

                        packetStack.Push(packet);
                        packet = new Packet()
                        {
                            Parent = packet,
                            LengthType = LengthType.Bits,
                            End = index + totalPacketLength.IntValue
                        };
                        packet.Parent.Packets.Add(packet);

                        symbolType = SymbolType.HeaderVersion;
                        bitsToRead = 3;
                        break;

                    case SymbolType.SubPacketCount:
                        var subPacketCount = new Symbol(SymbolType.SubPacketCount, symbol);
                        packet.Symbols.Add(subPacketCount);

                        //packet.LengthType = LengthType.Packets;
                        //packet.End = subPacketCount.IntValue;

                        packetStack.Push(packet);
                        packet = new Packet()
                        {
                            Parent = packet,
                            LengthType = LengthType.Packets,
                            End = subPacketCount.IntValue
                        };
                        packet.Parent.Packets.Add(packet);

                        symbolType = SymbolType.HeaderVersion;
                        bitsToRead = 3;
                        break;


                }

            }

            var result = SumAllPacketVersions(headPacket);

            return result.ToString();
        }

        private int SumAllPacketVersions(Packet packet)
        {
            var thisPacketVersion = packet.Symbols
                .Where(s => s.Type == SymbolType.HeaderVersion)
                .Sum(s => s.IntValue);

            var children = packet.Packets.Sum(SumAllPacketVersions);

            return thisPacketVersion + children;
        }

        public string Part2()
        {
            return "";
        }


        private IEnumerable<int> ExpandHex(string hex)
        {
            return hex.ToArray()
                .SelectMany(ExpandHex);
        }

        private static Dictionary<char, int[]> HexLookup = new Dictionary<char, int[]>
        {
            { '0', new []{ 0, 0, 0, 0} },
            { '1', new []{ 0, 0, 0, 1} },
            { '2', new []{ 0, 0, 1, 0} },
            { '3', new []{ 0, 0, 1, 1} },
            { '4', new []{ 0, 1, 0, 0} },
            { '5', new []{ 0, 1, 0, 1} },
            { '6', new []{ 0, 1, 1, 0} },
            { '7', new []{ 0, 1, 1, 1} },
            { '8', new []{ 1, 0, 0, 0} },
            { '9', new []{ 1, 0, 0, 1} },
            { 'A', new []{ 1, 0, 1, 0} },
            { 'B', new []{ 1, 0, 1, 1} },
            { 'C', new []{ 1, 1, 0, 0} },
            { 'D', new []{ 1, 1, 0, 1} },
            { 'E', new []{ 1, 1, 1, 0} },
            { 'F', new []{ 1, 1, 1, 1} },
        };

        private IEnumerable<int> ExpandHex(char hex)
        {
            return HexLookup[hex];
        }
    }
}
