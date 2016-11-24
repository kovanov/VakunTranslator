using System.Linq;

namespace VakunTranslatorVol2.Extensions
{
    public static class CharExtensions
    {
        public static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }
        public static bool IsFullstop(this char c)
        {
            return c.Equals('.');
        }
        public static bool IsOperator(this char c)
        {
            return operators.Contains(c);
        }
        public static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }
        public static bool IsDelimiter(this char c)
        {
            return delimiters.Contains(c);
        }
        public static bool IsExponent(this char c)
        {
            return c.Equals('e') || c.Equals('E');
        }
        public static bool IsUnderscore(this char c)
        {
            return c.Equals('_');
        }
        public static bool IsSign(this char c)
        {
            return c.Equals('+') || c.Equals('-');
        }
        public static bool IsNewLine(this char c)
        {
            return c.Equals('\n');
        }
        public static bool IsWhitespace(this char c)
        {
            return char.IsWhiteSpace(c);
        }
        public static bool IsEquality(this char c)
        {
            return c.Equals('=');
        }
        private static char[] operators = new[] { '<', '>', '!', '=' };
        private static char[] delimiters = new[] { '{', '}', '(', ')', ';', ',', '*', '/', ':', '&', '|' };
    }
}