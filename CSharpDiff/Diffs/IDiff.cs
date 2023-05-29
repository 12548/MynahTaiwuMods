using System.Collections.Generic;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Diffs
{
    public interface IDiff
    {
        List<DiffResult> buildValues(List<DiffResult> components, string[] newString, string[] oldString, bool useLongestToken);

        BestPath clonePath(BestPath path);

        IList<DiffResult> determineDiff(string[] cleanOldString, string[] cleanNewString);

        IList<DiffResult> diff(string oldString, string newString);

        /// <summary>
        /// Check whether left matches right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        bool equals(string left, string right);
        int extractCommon(BestPath basePath, string[] newString, string[] oldString, int diagonalPath);
        string join(string[] strings);
        List<DiffResult> pushComponent(List<DiffResult> components, bool? added, bool? removed);

        /// <summary>
        /// Remove empty values from the array. Differs per diff type.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        string[] removeEmpty(string[] array);

        /// <summary>
        /// Tokenize string, differs per diff type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string[] tokenize(string value);
    }
}