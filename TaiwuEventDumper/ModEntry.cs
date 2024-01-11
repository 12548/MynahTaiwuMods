using GameData.Utilities;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace TaiwuEventDumper;

[PluginConfig("TaiwuEventDumper", "myna12548", "99.99.99.99")]
public class ModEntry : TaiwuRemakePlugin
{

    public static bool ExportJson = true;
    
    public override void Initialize()
    {
    }

    public override void OnLoadedArchiveData()
    {
        AdaptableLog.Info($"TaiwuPath: {Program.TaiwuPath}");
        EventDumper.Dump1();
    }

    public override void Dispose()
    {
    }
}