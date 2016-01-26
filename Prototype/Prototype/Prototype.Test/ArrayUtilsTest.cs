using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Prototype.Test
{
    [TestClass]
    public class ArrayUtilsTest
    {
        private readonly string[] _inputLines = new string[]
            {
                "lkjasdfljhalsdjfuiohvdj",
                "39834589834850343485834",
                "                       ",
                "&$@)*?(%$<>!*[{}$*(@#]%",
                "ABC123def456xXyYzZ01010",
                " ▄▄█▀▄▀▄█▀▄▀ █▄▄█▄ ▀█  ",
                "this line is shorter",
                "THIS LINE IS THE LONGEST LINE",
                "This line is just right",
                "The quick brown fox ",
                "jumps over the lazy dog",
            };

        private readonly string[] _outputLines = new string[]
            {
                "lkjasdfljhalsdjfuiohvdj      ",
                "39834589834850343485834      ",
                "                             ",
                "&$@)*?(%$<>!*[{}$*(@#]%      ",
                "ABC123def456xXyYzZ01010      ",
                " ▄▄█▀▄▀▄█▀▄▀ █▄▄█▄ ▀█        ",
                "this line is shorter         ",
                "THIS LINE IS THE LONGEST LINE",
                "This line is just right      ",
                "The quick brown fox          ",
                "jumps over the lazy dog      ",
            };

        private readonly int _longestLineLength = "THIS LINE IS THE LONGEST LINE".Length;

        private readonly char[,] _chars = new char[,]
            {
                { 'l', 'k', 'j', 'a', 's', 'd', 'f', 'l', 'j', 'h', 'a', 'l', 's', 'd', 'j', 'f', 'u', 'i', 'o', 'h', 'v', 'd', 'j', ' ', ' ', ' ', ' ', ' ', ' ' },
                { '3', '9', '8', '3', '4', '5', '8', '9', '8', '3', '4', '8', '5', '0', '3', '4', '3', '4', '8', '5', '8', '3', '4', ' ', ' ', ' ', ' ', ' ', ' ' },
                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                { '&', '$', '@', ')', '*', '?', '(', '%', '$', '<', '>', '!', '*', '[', '{', '}', '$', '*', '(', '@', '#', ']', '%', ' ', ' ', ' ', ' ', ' ', ' ' },
                { 'A', 'B', 'C', '1', '2', '3', 'd', 'e', 'f', '4', '5', '6', 'x', 'X', 'y', 'Y', 'z', 'Z', '0', '1', '0', '1', '0', ' ', ' ', ' ', ' ', ' ', ' ' },
                { ' ', '▄', '▄', '█', '▀', '▄', '▀', '▄', '█', '▀', '▄', '▀', ' ', '█', '▄', '▄', '█', '▄', ' ', '▀', '█', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                { 't', 'h', 'i', 's', ' ', 'l', 'i', 'n', 'e', ' ', 'i', 's', ' ', 's', 'h', 'o', 'r', 't', 'e', 'r', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                { 'T', 'H', 'I', 'S', ' ', 'L', 'I', 'N', 'E', ' ', 'I', 'S', ' ', 'T', 'H', 'E', ' ', 'L', 'O', 'N', 'G', 'E', 'S', 'T', ' ', 'L', 'I', 'N', 'E' },
                { 'T', 'h', 'i', 's', ' ', 'l', 'i', 'n', 'e', ' ', 'i', 's', ' ', 'j', 'u', 's', 't', ' ', 'r', 'i', 'g', 'h', 't', ' ', ' ', ' ', ' ', ' ', ' ' },
                { 'T', 'h', 'e', ' ', 'q', 'u', 'i', 'c', 'k', ' ', 'b', 'r', 'o', 'w', 'n', ' ', 'f', 'o', 'x', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                { 'j', 'u', 'm', 'p', 's', ' ', 'o', 'v', 'e', 'r', ' ', 't', 'h', 'e', ' ', 'l', 'a', 'z', 'y', ' ', 'd', 'o', 'g', ' ', ' ', ' ', ' ', ' ', ' ' },
            };

        [TestMethod]
        public void ToCharArrayFromStringArray()
        {
            var result = ArrayUtils.ConvertTo2DArray(_inputLines);

            result.Should().NotBeNull();

            int resultRows = result.GetLength(0);
            int resultColumns = result.GetLength(1);

            resultRows.Should().Be(_inputLines.Length);
            resultColumns.Should().Be(_longestLineLength);

            for (int i = 0; i < _inputLines.Length; i++)
            {
                string currentLine = _inputLines[i];

                for (int j = 0; j < _longestLineLength; j++)
                {
                    result[i, j].ShouldBeEquivalentTo(j < currentLine.Length ? currentLine[j] : ' ');
                }
            }
        }

        [TestMethod]
        public void ToStringArrayFromCharArray()
        {
            var result = ArrayUtils.ConvertTo1DArray(_chars);

            result.Should().NotBeNull();

            int resultRows = result.Length;

            resultRows.Should().Be(_chars.GetLength(0));

            for (int i = 0; i < resultRows; i++)
            {
                string line = result[i];
                string target = _outputLines[i];
                
                String.Compare(target, line, System.StringComparison.Ordinal).Should().Be(0);
                line.Should().BeEquivalentTo(target);
            }
        }
    }
}
