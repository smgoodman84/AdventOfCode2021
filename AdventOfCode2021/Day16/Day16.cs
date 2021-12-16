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
            public Symbol HeaderVersion { get; set; }
            public Symbol HeaderType { get; set; }
            public Symbol Literal { get; set; }
            public Symbol OperatorLengthType { get; set; }
            public Symbol TotalPacketLength { get; set; }
            public Symbol SubPacketCount { get; set; }
            /*
            public Packet Parent { get; set; }
            public LengthType LengthType { get; set; }
            public int End { get; set; }
            public List<Symbol> Symbols { get; set; } = new List<Symbol>();
            */
            public List<Packet> Packets { get; set; } = new List<Packet>();
        }

        private class Parser
        {
            private readonly int[] _binaryData;

            public Parser(int[] binaryData)
            {
                _binaryData = binaryData;
            }

            private int _index = 0;
            
            private int[] ReadSymbolData(int bitsToRead)
            {
                var symbolData = new int[bitsToRead];

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
                var literal = new Symbol(SymbolType.Literal, new int[0]);

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

            private List<Packet> ReadPacketsForCount(int count)
            {
                var packets = new List<Packet>();

                while (count > 0)
                {
                    packets.Add(ReadPacket());
                    count -= 1;
                }

                return packets;
            }

            private List<Packet> ReadPacketsForLength(int count)
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

                if (packet.HeaderType.IntValue == 4)
                {

                    packet.Literal = ReadLiteral();
                    return packet;
                }

                packet.OperatorLengthType = ReadOperatorTypeLength();
                if (packet.OperatorLengthType.IntValue == 0)
                {
                    packet.TotalPacketLength = ReadTotalPacketLength();
                    packet.Packets = ReadPacketsForLength(packet.TotalPacketLength.IntValue);
                    return packet;
                }

                packet.SubPacketCount = ReadSubPacketCount();
                packet.Packets = ReadPacketsForCount(packet.SubPacketCount.IntValue);
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

        private int SumAllPacketVersions(Packet packet)
        {
            var thisPacketVersion = packet.HeaderVersion.IntValue;

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
