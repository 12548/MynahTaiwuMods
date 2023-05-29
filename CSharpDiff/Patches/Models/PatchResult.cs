using System.Collections.Generic;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Patches.Models
{
    public class PatchResult
    {
        public PatchResult()
        {
            Hunks = new List<Hunk>();
        }

        public string OldFileName { get; set; } = "";
        public string NewFileName { get; set; } = "";
        public string OldHeader { get; set; } = "";
        public string NewHeader { get; set; } = "";
        public IEnumerable<Hunk> Hunks { get; set; }
    }
}