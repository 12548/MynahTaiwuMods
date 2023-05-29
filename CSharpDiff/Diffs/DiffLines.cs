using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpDiff.Diffs.Models;
namespace CSharpDiff.Diffs
{
    public class DiffLines : Diff
    {
        public DiffLines() : base()
        {
        }

        public DiffLines(DiffOptions options) : base(options)
        {
        }

        public override string[] tokenize(string value)
        {
            var retLines = new List<string>();
            var regex = new Regex("(\n|\r\n)");
            var linesAndNewlines = regex.Split(value).ToList();

            // Ignore the final empty token that occurs if the string ends with a new line
            if (String.IsNullOrEmpty(linesAndNewlines[linesAndNewlines.Count() - 1]))
            {
                linesAndNewlines.RemoveAt(linesAndNewlines.Count() - 1);
            }

            // Merge the content and line separators into single tokens
            for (var i = 0; i < linesAndNewlines.Count(); i++)
            {
                var line = linesAndNewlines[i];
                if (i % 2 == 1 && Options.NewlineIsToken == false)
                {
                    retLines[retLines.Count() - 1] += line;
                }
                else
                {
                    if (Options.IgnoreWhiteSpace)
                    {
                        line = line.Trim();
                    }
                    retLines.Add(line);
                }
            }

            return retLines.ToArray();
        }
    }

}