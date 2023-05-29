using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Patches
{
    public class PatchParse
    {

        public ParsedDiff Parse(string uniDiff)
        {

            var diffStr = Regex.Split(uniDiff, "\r\n|[\n\v\f\r\x85]");
            var delimiters = Regex.Matches(uniDiff, "\r\n|[\n\v\f\r\x85]");
            var list = new List<int>();
            var i = 0;

            while (i < diffStr.Length)
            {

            }

            return new ParsedDiff();
        }

    }

    public class ParsedDiff
    {
        public ParsedDiff()
        {
            Hunks = new List<Hunk>();
        }

        public string Index { get; set; }
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string OldHeader { get; set; }
        public string NewHeader { get; set; }
        public IList<Hunk> Hunks { get; set; }
    }

}