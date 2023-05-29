using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpDiff.Diffs;
using CSharpDiff.Diffs.Models;
using CSharpDiff.Patches.Models;

namespace CSharpDiff.Patches
{
    public class Patch : IPatch
    {
        private readonly PatchOptions Options;
        private readonly DiffOptions DiffOptions;

        public Patch()
        {
            Options = new PatchOptions();
            DiffOptions = new DiffOptions();
        }

        public Patch(PatchOptions options)
        {
            Options = options;
            DiffOptions = new DiffOptions();
        }

        public Patch(PatchOptions options, DiffOptions diffOptions)
        {
            Options = options;
            DiffOptions = diffOptions;
        }

        public string[] contextLines(string[] lines)
        {
            return lines.Select((entry) => { return ' ' + entry; }).ToArray();
        }

        public string create(string filename, string oldStr, string newStr)
        {
            return create(filename, filename, oldStr, newStr, null, null);
        }

        public string create(string oldFileName, string newFileName, string oldStr, string newStr)
        {
            return create(oldFileName, newFileName, oldStr, newStr, null, null);
        }

        public string create(string filename, string oldStr, string newStr, string oldHeader, string newHeader)
        {
            return create(filename, filename, oldStr, newStr, oldHeader, newHeader);
        }

        public string create(string oldFileName, string newFileName, string oldStr, string newStr, string oldHeader, string newHeader)
        {
            var result = createPatchResult(oldFileName, newFileName, oldStr, newStr, oldHeader, newHeader);
            return formatPatch(result);
        }

        public PatchResult createPatchResult(string oldFileName, string newFileName, string oldStr, string newStr, string oldHeader, string newHeader)
        {
            var df = new DiffLines(DiffOptions);
            var diff = df.diff(oldStr, newStr);
            diff.Add(new DiffResult
            {
                value = "",
                hasLines = true,
            });

            var hunks = new List<Hunk>();
            var oldRangeStart = 0;
            var newRangeStart = 0;
            var curRange = new List<string>();
            var oldLine = 1;
            var newLine = 1;
            for (var i = 0; i < diff.Count(); i++)
            {
                var current = diff[i];
                var currentLines = new Hunk();
                var lines = current.hasLines ? current.lines : Regex.Replace(current.value, "\n$", "").Split('\n');
                currentLines.lines = lines;
                diff[i].lines = lines;
                diff[i].hasLines = true;

                if (current.added == true || current.removed == true)
                {
                    // If we have previous context, start with that
                    if (oldRangeStart == 0)
                    {
                        var prev = diff.ElementAtOrDefault(i - 1) != null ? diff[i - 1] : null;
                        oldRangeStart = oldLine;
                        newRangeStart = newLine;

                        if (prev != null)
                        {
                            var takenLines = prev.lines.TakeWhile(((_, index) => index >= prev.lines.Length - Options.Context));
                            curRange = Options.Context > 0 ? contextLines(takenLines.ToArray()).ToList() : new List<string>();
                            oldRangeStart -= curRange.Count();
                            newRangeStart -= curRange.Count();
                        }
                    }

                    // Output our changes
                    curRange.AddRange(lines.Select((entry) =>
                    {
                        return (current.added == true ? '+' : '-') + entry;
                    }));

                    // Track the updated file position
                    if (current.added == true)
                    {
                        newLine += lines.Length;
                    }
                    else
                    {
                        oldLine += lines.Length;
                    }

                }
                else
                {
                    // Identical context lines. Track line changes
                    // @todo huh?
                    if (oldRangeStart > 0)
                    {
                        // Close out any changes that have been output (or join overlapping)
                        if (lines.Length <= Options.Context * 2 && i < diff.Count() - 2)
                        {
                            // Overlapping
                            curRange.AddRange(contextLines(lines));
                        }
                        else
                        {
                            // end the range and output
                            var contextSize = Math.Min(lines.Length, Options.Context);
                            var takenLines = lines.Skip(0).Take(contextSize);
                            curRange.AddRange(contextLines(takenLines.ToArray()));

                            var hunk = new Hunk
                            {
                                oldStart = oldRangeStart,
                                oldLines = (oldLine - oldRangeStart + contextSize),
                                newStart = newRangeStart,
                                newLines = (newLine - newRangeStart + contextSize),
                                lines = curRange.ToArray()
                            };

                            if (i >= diff.Count() - 2 && lines.Length <= Options.Context)
                            {
                                // EOF is inside this hunk
                                var oldEOFNewline = Regex.IsMatch(oldStr, "\n$");
                                var newEOFNewline = Regex.IsMatch(newStr, "\n$");
                                var noNlBeforeAdds = lines.Count() == 0 && curRange.Count() > hunk.oldLines;
                                if (!oldEOFNewline && noNlBeforeAdds && oldStr.Count() > 0)
                                {
                                    // special case: old has no eol and no trailing context; no-nl can end up before adds
                                    // however, if the old file is empty, do not output the no-nl line
                                    curRange.Insert(hunk.oldLines, "\\ No newline at end of file");
                                }
                                if ((!oldEOFNewline && !noNlBeforeAdds) || !newEOFNewline)
                                {
                                    curRange.Add("\\ No newline at end of file");
                                }
                            }
                            hunk.lines = curRange.ToArray();
                            hunks.Add(hunk);

                            oldRangeStart = 0;
                            newRangeStart = 0;
                            curRange = new List<string>();
                        }
                    }
                    oldLine += lines.Length;
                    newLine += lines.Length;
                }
            }

            return new PatchResult
            {
                OldFileName = oldFileName,
                NewFileName = newFileName,
                OldHeader = oldHeader,
                NewHeader = newHeader,
                Hunks = hunks
            };
        }

        public string formatPatch(PatchResult diff)
        {
            var ret = new List<string>();
            if (diff.OldFileName == diff.NewFileName)
            {
                ret.Add("Index: " + diff.OldFileName);
            }
            ret.Add("===================================================================");
            ret.Add("--- " + diff.OldFileName + (String.IsNullOrEmpty(diff.OldHeader) ? "" : '\t' + diff.OldHeader));
            ret.Add("+++ " + diff.NewFileName + (String.IsNullOrEmpty(diff.NewHeader) ? "" : '\t' + diff.NewHeader));

            for (var i = 0; i < diff.Hunks.Count(); i++)
            {
                var hunk = diff.Hunks.ElementAt(i);
                // Unified Diff Format quirk: If the chunk size is 0,
                // the first number is one lower than one would expect.
                // https://www.artima.com/weblogs/viewpost.jsp?thread=164293
                if (hunk.oldLines == 0)
                {
                    hunk.oldStart -= 1;
                }
                if (hunk.newLines == 0)
                {
                    hunk.newStart -= 1;
                }
                ret.Add(
                    "@@ -" + hunk.oldStart + ',' + hunk.oldLines
                    + " +" + hunk.newStart + ',' + hunk.newLines
                    + " @@"
                );
                ret.AddRange(hunk.lines);
            }

            return String.Join("\n", ret) + '\n';
        }

    }
}