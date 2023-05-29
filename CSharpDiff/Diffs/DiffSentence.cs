using System.Text.RegularExpressions;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Diffs
{
    public class DiffSentence : Diff
    {
        public override string[] tokenize(string value)
        {
            var regex = new Regex(@"(\S.+?[.!?])(?=\s+|$)");
            return regex.Split(value);
        }
    }
}