using System.Collections.Generic;

namespace CSharpDiff.Diffs.Models
{
    public class Hunk
    {
        public Hunk()
        {
            lines = new List<string>().ToArray();
        }

        public int oldStart { get; set; }
        public int oldLines { get; set; }
        public int newStart { get; set; }
        public int newLines { get; set; }
        public string[] lines { get; set; }
    }
}