using System.Text;

namespace Prototype
{
    public static class ArrayUtils
    {
        private const char PLACEHOLDER_CHAR = ' ';

        public static char[,] ConvertTo2DArray(string[] lines)
        {
            if (null == lines || lines.Length == 0)
            {
                return null;
            }

            int longestLine = 0;

            foreach (string line in lines)
            {
                if (line.Length > longestLine)
                {
                    longestLine = line.Length;
                }
            }

            char[,] result = new char[lines.Length, longestLine];

            for (int i = 0; i < lines.Length; i++)
            {
                char[] lineChars = lines[i].ToCharArray();

                for (int j = 0; j < longestLine; j++)
                {
                    result[i, j] = (j < lines[i].Length)
                                       ? lineChars[j]
                                       : PLACEHOLDER_CHAR;
                }
            }

            return result;
        }

        public static string[] ConvertTo1DArray(char[,] characters)
        {
            int numRows = characters.GetLength(0);
            int numColumns = characters.GetLength(1);
            
            if (null == characters || 0 == numRows || 0 == numColumns)
            {
                return null;
            }

            string[] lines = new string[numRows];

            for (int i = 0; i < numRows; i++)
            {
                StringBuilder lineBuilder = new StringBuilder(numColumns);

                for (int j = 0; j < numColumns; j++)
                {
                    lineBuilder.Append(characters[i, j]);
                }

                lines[i] = lineBuilder.ToString();
            }

            return lines;
        }
    }
}