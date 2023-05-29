// See https://aka.ms/new-console-template for more information

using CSharpDiff.Converters;
using CSharpDiff.Diffs;
using CSharpDiff.Diffs.Models;

Console.WriteLine("Hello, World!");

var diff = new Diff(
    new DiffOptions()
    {
        UseLongestToken = false
    });

var d1 = diff.diff(
    "运用者的「道术」造诣越高，运用者在战斗中进行「疗伤」时能够治愈的伤势标记就越多",
    "运用者的「道术」造诣越高，运用者在战斗中进行「驱毒」时能够驱除的毒素就越多"
);

Console.WriteLine(DiffConvert.ToXml(d1));