using GameData.Domains;
using GameData.Utilities;
using TaiwuModdingLib.Core.Plugin;

namespace ItemAdvanceFilterBackend;

[PluginConfig("ItemAdvanceFilter", "myna12548", "1")]
public class ModEntry : TaiwuRemakePlugin
{
    
    public static List<sbyte> GetTotalReadingProgressList(List<int> bookItemIdList)
    {
        return DomainManager.Taiwu.GetTotalReadingProgressList(bookItemIdList);
    }
    
    public static sbyte GetTotalReadingProgress(int bookItemId)
    {
        return DomainManager.Taiwu.GetTotalReadingProgress(bookItemId);
    }

    
    public override void Initialize()
    {
        AdaptableLog.Info($"TypeName: {typeof(ModEntry).AssemblyQualifiedName}");
    }

    public override void Dispose()
    {
    }
} 