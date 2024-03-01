using GameData.Domains;
using GameData.Utilities;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace TaiwuEventDumper;

[PluginConfig("TaiwuEventDumper", "myna12548", "99.99.99.99")]
public class ModEntry : TaiwuRemakePlugin
{

    public static bool ExportJson = true;
    public static bool DumpTwe = true;
    public static bool AutoOpenViewer = true;
    public static string ModDir = "";
    
    public override void Initialize()
    {
    }

    public override void OnLoadedArchiveData()
    {
        DomainManager.Mod.GetSetting(this.ModIdStr, "dumpTwe", ref DumpTwe);
        DomainManager.Mod.GetSetting(this.ModIdStr, "dumpJson", ref ExportJson);
        DomainManager.Mod.GetSetting(this.ModIdStr, "autoOpenViewer", ref AutoOpenViewer);
        
        ModDir = DomainManager.Mod.GetModDirectory(ModIdStr); 

        AdaptableLog.Info($"TaiwuPath: {Program.TaiwuPath}");
        EventDumper.Dump1();
    }

    public override void Dispose()
    {
    }
}