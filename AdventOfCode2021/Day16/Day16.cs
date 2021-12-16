using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AdventOfCode2021.Day16
{
    public class Day16 : IDay
    {
        public int DayNumber => 16;
        public string ValidatedPart1 => "945";
        public string ValidatedPart2 => "10637009915279";

        private string _hexTransmission;

        public Day16()
        {
            _hexTransmission = File.ReadAllLines("Day16/input.txt").First();
        }

        private enum SymbolType
        {
            HeaderVersion,
            HeaderType,
            Literal,
            OperatorLengthType,
            TotalPacketLength,
            SubPacketCount
        }

        private class Symbol
        {
            public Symbol(SymbolType type, IEnumerable<uint> value)
            {
                Type = type;
                RawValue = value.ToArray();
                ConvertToInt();
            }

            public SymbolType Type { get; set; }
            public uint[] RawValue { get; set; }
            public ulong LongValue { get; set; }
            public List<Symbol> Children = new List<Symbol>();

            public void AddBits(IEnumerable<uint> newBits)
            {
                RawValue = RawValue.Concat(newBits).ToArray();
                ConvertToInt();
            }

            private void ConvertToInt()
            {
                ulong result = 0;
                foreach (var i in RawValue)
                {
                    result *= 2;
                    result += i;
                }
                LongValue = result;
            }

            public override string ToString()
            {
                return $"{Type} {LongValue}";
            }
        }

        private enum LengthType
        {
            Bits,
            Packets
        }

        private class Packet
        {
            public Symbol HeaderVersion { get; set; }
            public Symbol HeaderType { get; set; }
            public Symbol Literal { get; set; }
            public Symbol OperatorLengthType { get; set; }
            public Symbol TotalPacketLength { get; set; }
            public Symbol SubPacketCount { get; set; }
            public List<Packet> Packets { get; set; } = new List<Packet>();

            public ulong Evaluate()
            {
                switch(HeaderType.LongValue)
                {
                    case 0:
                        ulong sum = 0;
                        foreach (var p in Packets)
                        {
                            sum += p.Evaluate();
                        }
                        return sum;
                    case 1:
                        ulong product = 1;
                        foreach(var p in Packets)
                        {
                            product *= p.Evaluate();
                        }
                        return product;
                    case 2:
                        return Packets.Min(p => p.Evaluate());
                    case 3:
                        return Packets.Max(p => p.Evaluate());
                    case 4:
                        return Literal.LongValue;
                    case 5:
                        var gta = Packets[0].Evaluate();
                        var gtb = Packets[1].Evaluate();
                        return gta > gtb ? 1ul : 0ul;
                    case 6:
                        var lta = Packets[0].Evaluate();
                        var ltb = Packets[1].Evaluate();
                        return lta < ltb ? 1ul : 0ul;
                    case 7:
                        var ea = Packets[0].Evaluate();
                        var eb = Packets[1].Evaluate();
                        return ea == eb ? 1ul : 0ul;
                }

                throw new Exception($"Unexpected HeaderType {HeaderType.LongValue}");
            }
        }

        private class Parser
        {
            private readonly uint[] _binaryData;

            public Parser(uint[] binaryData)
            {
                _binaryData = binaryData;
            }

            private ulong _index = 0;
            
            private uint[] ReadSymbolData(int bitsToRead)
            {
                var symbolData = new uint[bitsToRead];

                var symbolIndex = 0;
                while (symbolIndex < bitsToRead)
                {
                    symbolData[symbolIndex] = _binaryData[_index];
                    symbolIndex += 1;
                    _index += 1;
                }

                return symbolData;
            }

            private Symbol ReadHeaderVersion()
            {
                var symbolData = ReadSymbolData(3);
                var headerVersion = new Symbol(SymbolType.HeaderVersion, symbolData);
                return headerVersion;
            }

            private Symbol ReadHeaderType()
            {
                var symbolData = ReadSymbolData(3);
                var headerType = new Symbol(SymbolType.HeaderType, symbolData);
                return headerType;
            }

            private Symbol ReadLiteral()
            {
                var literal = new Symbol(SymbolType.Literal, new uint[0]);

                var readMore = true;
                while(readMore)
                {
                    var currentLiteralPart = ReadSymbolData(5);
                    literal.AddBits(currentLiteralPart.Skip(1));

                    readMore = currentLiteralPart[0] == 1;
                }

                return literal;
            }

            private Symbol ReadOperatorTypeLength()
            {
                var symbolData = ReadSymbolData(1);
                var operatorLengthType = new Symbol(SymbolType.OperatorLengthType, symbolData);
                return operatorLengthType;
            }

            private Symbol ReadTotalPacketLength()
            {
                var symbolData = ReadSymbolData(15);
                var totalPacketLength = new Symbol(SymbolType.TotalPacketLength, symbolData);
                return totalPacketLength;
            }

            private Symbol ReadSubPacketCount()
            {
                var symbolData = ReadSymbolData(11);
                var subPacketCount = new Symbol(SymbolType.SubPacketCount, symbolData);
                return subPacketCount;
            }

            private List<Packet> ReadPacketsForCount(ulong count)
            {
                var packets = new List<Packet>();

                while (count > 0)
                {
                    packets.Add(ReadPacket());
                    count -= 1;
                }

                return packets;
            }

            private List<Packet> ReadPacketsForLength(ulong count)
            {
                var endIndex = _index + count;
                var packets = new List<Packet>();

                while (_index < endIndex)
                {
                    packets.Add(ReadPacket());
                }

                return packets;
            }

            private Packet ReadPacket()
            {
                var packet = new Packet();

                packet.HeaderVersion = ReadHeaderVersion();
                packet.HeaderType = ReadHeaderType();

                if (packet.HeaderType.LongValue == 4)
                {
                    packet.Literal = ReadLiteral();
                    return packet;
                }

                packet.OperatorLengthType = ReadOperatorTypeLength();
                if (packet.OperatorLengthType.LongValue == 0)
                {
                    packet.TotalPacketLength = ReadTotalPacketLength();
                    packet.Packets = ReadPacketsForLength(packet.TotalPacketLength.LongValue);
                    return packet;
                }

                packet.SubPacketCount = ReadSubPacketCount();
                packet.Packets = ReadPacketsForCount(packet.SubPacketCount.LongValue);
                return packet;
            }

            public Packet Parse()
            {
                var packet = ReadPacket();
                return packet;
            }
        }

        public string Part1()
        {
            var binaryData = ExpandHex(_hexTransmission).ToArray();

            var parser = new Parser(binaryData);

            var packet = parser.Parse();

            var result = SumAllPacketVersions(packet);

            return result.ToString();
        }

        private ulong SumAllPacketVersions(Packet packet)
        {
            ulong sum = packet.HeaderVersion.LongValue;
            foreach (var p in packet.Packets)
            {
                sum += SumAllPacketVersions(p);
            }

            return sum;
        }

        public string Part2()
        {
            var binaryData = ExpandHex(_hexTransmission).ToArray();

            var parser = new Parser(binaryData);

            var packet = parser.Parse();

            var result = packet.Evaluate();

            return result.ToString();
        }


        private IEnumerable<uint> ExpandHex(string hex)
        {
            return hex.ToArray()
                .SelectMany(ExpandHex);
        }



        private static Dictionary<char, uint[]> HexLookup = new Dictionary<char, uint[]>
        {
            { '0', new []{ 0u, 0u, 0u, 0u} },
            { '1', new []{ 0u, 0u, 0u, 1u} },
            { '2', new []{ 0u, 0u, 1u, 0u} },
            { '3', new []{ 0u, 0u, 1u, 1u} },
            { '4', new []{ 0u, 1u, 0u, 0u} },
            { '5', new []{ 0u, 1u, 0u, 1u} },
            { '6', new []{ 0u, 1u, 1u, 0u} },
            { '7', new []{ 0u, 1u, 1u, 1u} },
            { '8', new []{ 1u, 0u, 0u, 0u} },
            { '9', new []{ 1u, 0u, 0u, 1u} },
            { 'A', new []{ 1u, 0u, 1u, 0u} },
            { 'B', new []{ 1u, 0u, 1u, 1u} },
            { 'C', new []{ 1u, 1u, 0u, 0u} },
            { 'D', new []{ 1u, 1u, 0u, 1u} },
            { 'E', new []{ 1u, 1u, 1u, 0u} },
            { 'F', new []{ 1u, 1u, 1u, 1u} },
        };

        private IEnumerable<uint> ExpandHex(char hex)
        {
            return HexLookup[hex];
        }
    }
}
