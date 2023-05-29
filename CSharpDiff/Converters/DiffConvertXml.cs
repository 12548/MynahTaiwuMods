using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Converters
{
    public static class DiffConvert
    {
        /// <summary>
        /// Convert the changes to an XML structure
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static string ToXml(IList<DiffResult> changes)
        {

            var ret = new List<string>();
            for (var i = 0; i < changes.Count; i++)
            {
                var change = changes.ElementAt(i);
                if (change.added == true)
                {
                    ret.Add("<ins>");
                }
                else if (change.removed == true)
                {
                    ret.Add("<del>");
                }

                ret.Add(HttpUtility.HtmlEncode(change.value));

                if (change.added == true)
                {
                    ret.Add("</ins>");
                }
                else if (change.removed == true)
                {
                    ret.Add("</del>");
                }
            }

            return String.Join("", ret);
        }

        /// <summary>
        /// Convert to diff-match-patch format
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static IList<Tuple<int, string>> ToDmp(IList<DiffResult> changes)
        {
            var ret = new List<Tuple<int, string>>();
            DiffResult change = null;
            var operation = 0;
            for (var i = 0; i < changes.Count; i++)
            {
                change = changes[i];
                if (change.added == true)
                {
                    operation = 1;
                }
                else if (change.removed == true)
                {
                    operation = -1;
                }
                else
                {
                    operation = 0;
                }

                ret.Add(new Tuple<int, string>(operation, change.value));
            }

            return ret;
        }

    }
}