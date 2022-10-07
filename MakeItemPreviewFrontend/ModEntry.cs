using System;
using TaiwuModdingLib.Core.Plugin;

namespace MakeItemPreviewFrontend;

[PluginConfig("MakeItemPreview", "myna12548", "0")]
public class ModEntry: TaiwuRemakeHarmonyPlugin
{
    public static string StaticModIdStr;

    public override void Initialize()
    {
        base.Initialize();
        StaticModIdStr = ModIdStr;
    }
}