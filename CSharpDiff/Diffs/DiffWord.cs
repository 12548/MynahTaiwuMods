using System;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Diffs
{
    public class DiffWord : Diff
    {
        private const string EXTENDEDWORD_CHARS = @"^[A-Za-z\xC0-\u02C6\u02C8-\u02D7\u02DE-\u02FF\u1E00-\u1EFF]+$";
        private const string WHITESPACE = @"\S";

        public DiffWord() : base(new DiffOptions { IgnoreWhiteSpace = true })
        {
        }

        public DiffWord(DiffOptions options) : base(options)
        {
        }

        public override string[] tokenize(string value)
        {
            var regex = new Regex(@"([^\S\r\n]+|[()[\]{}'""\r\n]|\b)");
            var tokens = regex.Split(value).ToList();
            for (var i = 0; i < tokens.Count - 1; i++)
            {

                // If we have an empty string in the next field and we have only word chars before and after, merge
                if (tokens.ElementAtOrDefault(i + 1) == null && tokens.ElementAtOrDefault(i + 2) != null && Regex.IsMatch(tokens[i], EXTENDEDWORD_CHARS) && Regex.IsMatch(tokens[i + 2], EXTENDEDWORD_CHARS))
                {
                    tokens[i] += tokens[i + 2];
                    tokens.RemoveRange(i + 1, 2);
                    i--;
                }

                if (String.IsNullOrEmpty(tokens[i]))
                {
                    tokens.RemoveAt(i);
                }
            }
            return tokens.ToArray();
        }

        public override bool equals(string left, string right)
        {
            if (Options.IgnoreCase)
            {
                left = left.ToLowerInvariant();
                right = right.ToLowerInvariant();
            }
            var isWhiteSpace = Options.IgnoreWhiteSpace && Regex.IsMatch(left, WHITESPACE) == false && Regex.IsMatch(right, WHITESPACE) == false;
            var isMatch = String.Compare(left, right, Options.IgnoreCase) == 0 || isWhiteSpace;
            return isMatch;
        }
    }

}
