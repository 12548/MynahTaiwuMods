using GameData.Utilities;
using TaiwuModdingLib.Core.Plugin;

namespace TaiwuEventDumper;

[PluginConfig("TaiwuEventDumper", "myna12548", "99.99.99.99")]
public class ModEntry : TaiwuRemakePlugin
{
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