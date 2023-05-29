using System.Collections.Generic;

namespace CSharpDiff.Diffs.Models
{
    public class BestPath
    {
        public BestPath()
        {
            components = new List<DiffResult>();
        }

        public int newPos { get; set; }
        public List<DiffResult> components { get; set; }
    }
}