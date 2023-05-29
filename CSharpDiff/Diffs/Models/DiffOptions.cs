namespace CSharpDiff.Diffs.Models
{
    public class DiffOptions
    {
        public bool UseLongestToken { get; set; } = true;
        public bool IgnoreWhiteSpace { get; set; } = false;
        public bool IgnoreCase { get; set; } = false;
        public bool NewlineIsToken { get; set; } = false;
    }
}