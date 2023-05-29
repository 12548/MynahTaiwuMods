using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSharpDiff.Diffs.Models;

namespace CSharpDiff.Diffs
{
    public class Diff : IDiff
    {
        public Diff()
        {
            Options = new DiffOptions();
        }

        public Diff(DiffOptions options)
        {
            Options = options;
        }

        public DiffOptions Options { get; set; }

        public IList<DiffResult> diff(string oldString, string newString)
        {
            var cleanOldString = removeEmpty(tokenize(oldString));
            var cleanNewString = removeEmpty(tokenize(newString));
            return determineDiff(cleanOldString, cleanNewString);
        }

        public IList<DiffResult> determineDiff(string[] cleanOldString, string[] cleanNewString)
        {

            var diffs = new List<DiffResult>();
            var newLen = cleanNewString.Length;
            var oldLen = cleanOldString.Length;
            var editLength = 1;
            var maxEditLength = newLen + oldLen;

            var bestPath = new Dictionary<int, BestPath?>();
            bestPath.Add(0, new BestPath
            {
                newPos = -1
            });

            BestPath oldPosPath = null;
            bestPath.TryGetValue(0, out oldPosPath);
            oldPosPath ??= new BestPath();
            var oldPos = extractCommon(oldPosPath, cleanNewString, cleanOldString, 0);
            if (oldPosPath.newPos + 1 >= newLen && oldPos + 1 >= oldLen)
            {
                diffs.Add(new DiffResult
                {
                    value = join(cleanNewString),
                    count = cleanNewString.Length
                });
            }

            while (editLength <= maxEditLength)
            {
                var dPath = -1 * editLength;
                for (var diagonalPath = dPath; diagonalPath <= editLength; diagonalPath += 2)
                {
                    BestPath basePath;

                    var addPath = bestPath.ContainsKey(diagonalPath - 1) ? bestPath[diagonalPath - 1] : null;
                    var removePath = bestPath.ContainsKey(diagonalPath + 1) ? bestPath[diagonalPath + 1] : null;
                    oldPos = (removePath != null ? removePath.newPos : 0) - diagonalPath;

                    if (addPath != null)
                    {
                        // No one else is going to attempt to use this value, clear it
                        bestPath[diagonalPath - 1] = null;
                    }

                    var canAdd = addPath != null && addPath.newPos + 1 < newLen;
                    var canRemove = removePath != null && 0 <= oldPos && oldPos < oldLen;
                    if (!canAdd && !canRemove)
                    {
                        if (bestPath.ContainsKey(diagonalPath))
                        {
                            bestPath[diagonalPath] = null;
                        }
                        else
                        {
                            bestPath.Add(diagonalPath, null);
                        }

                        continue;
                    }

                    // Select the diagonal that we want to branch from. We select the prior
                    // path whose position in the new string is the farthest from the origin
                    // and does not pass the bounds of the diff graph
                    if (!canAdd && removePath != null || (canRemove && addPath != null && removePath != null && addPath.newPos < removePath.newPos))
                    {
                        basePath = clonePath(removePath);
                        basePath.components = pushComponent(basePath.components, null, true);
                    }
                    else
                    {
                        // No need to clone, we've pulled it from the list
                        // Note that C# complains here if we don't do the if, but it's not the best way of handling.
                        basePath = addPath != null ? addPath : new BestPath();
                        basePath.newPos++;
                        basePath.components = pushComponent(basePath.components, true, null);
                    }

                    oldPos = extractCommon(basePath, cleanNewString, cleanOldString, diagonalPath);

                    // If we have hit the end of both strings, then we are done
                    if (basePath.newPos + 1 >= newLen && oldPos + 1 >= oldLen)
                    {
                        editLength = maxEditLength;
                        return buildValues(basePath.components, cleanNewString, cleanOldString, false);
                        // return done(buildValues(self, basePath.components, newString, oldString, UseLongestToken));
                    }
                    else
                    {

                        if (bestPath.ContainsKey(diagonalPath))
                        {
                            bestPath[diagonalPath] = basePath;
                        }
                        else
                        {
                            bestPath.Add(diagonalPath, basePath);
                        }
                        // Otherwise track this path as a potential candidate and continue.

                    }
                }

                editLength++;
            }

            return diffs;

        }

        public List<DiffResult> buildValues(List<DiffResult> components, string[] newString, string[] oldString, bool useLongestToken)
        {
            var componentPos = 0;
            var componentLen = components.Count();
            var newPos = 0;
            var oldPos = 0;

            for (; componentPos < componentLen; componentPos++)
            {
                var component = components[componentPos];
                var whereToTake = newPos + (int)component.count;
                var whereToTakeOld = oldPos + (int)component.count;
                if (component.removed != true)
                {
                    if (component.added != null && useLongestToken)
                    {
                        var value = newString.Where(((_, i) => i >= newPos && i < whereToTakeOld)).ToArray();
                        value = value.Select((value, i) =>
                        {
                            var oldValue = oldString[oldPos + i];
                            // @todo wtf is this
                            // return oldValue;
                            return oldValue.Length > value.Length ? oldValue : value;
                        }).ToArray();

                        component.value = join(value.ToArray());
                    }
                    else
                    {
                        component.value = join(newString.Where(((_, i) => i >= newPos && i < whereToTake)).ToArray());
                    }

                    newPos += (int)component.count;

                    // Common case
                    if (component.added != true)
                    {
                        oldPos += (int)component.count;
                    }
                }
                else
                {
                    component.value = join(oldString.Where(((_, i) => i >= oldPos && i < whereToTakeOld)).ToArray());
                    oldPos += (int)component.count;

                    // Reverse add and remove so removes are output first to match common convention
                    // The diffing algorithm is tied to add then remove output and this is the simplest
                    // route to get the desired output with minimal overhead.
                    var prev = componentPos > 0 ? components.ElementAt(componentPos - 1) : null;
                    if (prev != null && prev.added == true)
                    {
                        var tmp = components[componentPos];

                        components.RemoveAt(componentPos);
                        components.RemoveAt(componentPos - 1);

                        components.Insert(componentPos - 1, tmp);
                        components.Insert(componentPos, prev);
                    }
                }
            }

            // Special case handle for when one terminal is ignored (i.e. whitespace).
            // For this case we merge the terminal into the prior string and drop the change.
            // This is only available for string mode.
            var lastComponent = components[componentLen - 1];
            if (componentLen > 1
                // && typeof (lastComponent.value) === 'string' (@todo idk what this is)
                && (lastComponent.added == true || lastComponent.removed == true)
                && equals("", lastComponent.value))
            {
                components[componentLen - 2].value += lastComponent.value;
                components.RemoveAt(components.Count() - 1);
            }

            return components;
        }

        public List<DiffResult> pushComponent(List<DiffResult> components, bool? added, bool? removed)
        {

            var newComponents = new List<DiffResult>(components);
            var last = components.Any() ? components.Last() : null;
            if (last != null && last.added == added && last.removed == removed)
            {
                // We need to clone here as the component clone operation is just
                // as shallow array clone
                newComponents[components.Count() - 1] = new DiffResult { count = last.count + 1, added = added, removed = removed };
            }
            else
            {
                newComponents.Add(new DiffResult { count = 1, added = added, removed = removed });
            }
            return newComponents;
        }

        public BestPath clonePath(BestPath path)
        {
            return new BestPath
            {
                newPos = path.newPos,
                components = new List<DiffResult>(path.components)
            };
        }

        public virtual string[] tokenize(string value)
        {
            return value.ToCharArray().Select(c => c.ToString()).ToArray();
        }

        public string join(string[] chars)
        {
            return String.Join("", chars);
        }

        public string[] removeEmpty(string[] array)
        {
            // return array;
            var ret = new List<string>();
            for (var i = 0; i < array.Count(); i++)
            {
                if (!String.IsNullOrEmpty(array.ElementAt(i)))
                {
                    ret.Add(array[i]);
                }
            }
            return ret.ToArray();
        }

        public int extractCommon(BestPath basePath, string[] newString, string[] oldString, int diagonalPath)
        {
            var newLen = newString.Length;
            var oldLen = oldString.Length;
            var newPos = basePath.newPos;
            var oldPos = newPos - diagonalPath;

            var commonCount = 0;
            while (newPos + 1 < newLen && oldPos + 1 < oldLen && equals(newString[newPos + 1], oldString[oldPos + 1]))
            {
                newPos++;
                oldPos++;
                commonCount++;
            }

            if (commonCount > 0)
            {
                basePath.components.Add(new DiffResult
                {
                    count = commonCount
                });
            }

            basePath.newPos = newPos;
            return oldPos;
        }

        public virtual bool equals(string left, string right)
        {
            return String.Compare(left, right, Options.IgnoreCase) == 0;
        }
    }

}