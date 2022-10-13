using GameData.Domains;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Mod;
using GameData.Utilities;
using MynahBaseModBackend;
using TaiwuModdingLib.Core.Plugin;

namespace MakeItemPreviewBackend;

[PluginConfig("MakeItemPreview", "myna12548", "0")]
public class ModEntry : TaiwuRemakePlugin
{
    public override void Initialize()
    {
        Patch.handleMethod(ModIdStr, "GetItemPreview", param =>
        {
            AdaptableLog.Info("Handling method GetItemPreview");
            param.Get("itemType", out int itemType);
            param.Get("templateId", out int templateId);

            DomainManager.Mod.TryGet(ModIdStr, "ItemPreviews", true, out SerializableModData previewData);

            previewData ??= new SerializableModData();

            ItemDisplayData data;
            var key = $"{itemType}_{templateId}";
            var haveData = previewData.Get(key, out ItemKey itemKey);

            if (haveData)
            {
                data = DomainManager.Item.GetItemDisplayData(itemKey);
            }
            else
            {
                data = DomainManager.Item.GetItemDisplayData(new ItemKey((sbyte)itemType, 0, (short)templateId, -1));
                previewData.Set(key, data.Key);
                DomainManager.Mod.SetSerializableModData(
                    DomainManager.TaiwuEvent.MainThreadDataContext, ModIdStr, "ItemPreviews", true, previewData);
            }

            var result = new SerializableModData();
            result.Set("data", data);
            return result;
        });
    }

    public override void Dispose()
    {
    }
}