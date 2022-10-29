using GameData.Domains;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Mod;
using GameData.Utilities;
using MynahBaseModBackend;
using TaiwuModdingLib.Core.Plugin;

namespace MakeItemPreviewBackend;

[PluginConfig("MakeItemPreview", "myna12548", "0")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    public static string StaticModIdStr;

    public override void Initialize()
    {
        base.Initialize();
        StaticModIdStr = ModIdStr;
        AdaptableLog.Info("MakeItemPreview Backend Initialized!");
    }
}