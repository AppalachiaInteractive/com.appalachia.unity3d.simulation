using System;
using System.Globalization;
using System.Text;

namespace Appalachia.Simulation.Trees.Extensions
{
    public static class StringExtensions
    {
        public static string ToFriendly(this string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < input.Length; ++index)
            {
                char ch = input[index];
                if ((ch == '_') && ((index + 1) < input.Length))
                {
                    char upper = input[index + 1];
                    if (char.IsLower(upper))
                        upper = char.ToUpper(upper, CultureInfo.InvariantCulture);
                    stringBuilder.Append(upper);
                    ++index;
                }
                else
                    stringBuilder.Append(ch);
            }

            return stringBuilder.ToString();
        }

        public static string ToSafe(this string input)
        {
            var stringBuilder = new StringBuilder();
            
            foreach(var sChar in input)
            {
                if (char.IsLetterOrDigit(sChar) || (sChar == '-') || (sChar == '_'))
                {
                    stringBuilder.Append(sChar);
                }
                else if (char.IsWhiteSpace(sChar))
                {
                    if (stringBuilder.Length == 0)
                    {
                        continue;
                    }
                    
                    if (stringBuilder.Length > 0)
                    {
                        if (stringBuilder[stringBuilder.Length - 1] == '-')
                        {
                        }
                        else
                        {
                            stringBuilder.Append('-');
                        }
                    }
                }
            }
            
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns whether or not the specified string is contained with this string
        /// </summary>
        public static bool Contains(
            this string source,
            string toCheck,
            StringComparison comparisonType)
        {
            return source.IndexOf(toCheck, comparisonType) >= 0;
        }

        public static string SplitPascalCase(this string input)
        {
            switch (input)
            {
                case "":
                case null:
                    return input;
                default:
                    StringBuilder stringBuilder = new StringBuilder(input.Length);
                    if (char.IsLetter(input[0]))
                        stringBuilder.Append(char.ToUpper(input[0]));
                    else
                        stringBuilder.Append(input[0]);
                    for (int index = 1; index < input.Length; ++index)
                    {
                        char c = input[index];
                        if (char.IsUpper(c) && !char.IsUpper(input[index - 1]))
                            stringBuilder.Append(' ');
                        stringBuilder.Append(c);
                    }

                    return stringBuilder.ToString();
            }
        }
    }
}
