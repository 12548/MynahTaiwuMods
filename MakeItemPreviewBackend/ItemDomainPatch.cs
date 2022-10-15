using GameData.Domains;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Mod;
using GameData.Utilities;
using HarmonyLib;

namespace MakeItemPreviewBackend;

[HarmonyPatch]
public class ItemDomainPatch
{
    [HarmonyPrefix, HarmonyPatch(typeof(ItemDomain), "GetItemDisplayData")]
    static bool Prefix(ItemKey itemKey, int charId, ref ItemDisplayData __result)
    {
        AdaptableLog.Info($"Prefixing! {itemKey.Id} {charId}");
        if (itemKey.Id != -12548 || charId != -12548)
        {
            return true;
        }

        var itemType = itemKey.ItemType;
        var templateId = itemKey.TemplateId;
        
        DomainManager.Mod.TryGet(ModEntry.StaticModIdStr, "ItemPreviews", true, out SerializableModData previewData);

        previewData ??= new SerializableModData();
 
        ItemDisplayData data;
        var key = $"{itemType}_{templateId}";
        var haveData = previewData.Get(key, out ItemKey ik);

        if (haveData)
        {
            data = DomainManager.Item.GetItemDisplayData(ik);
        }
        else
        {
            data = DomainManager.Item.GetItemDisplayData(new ItemKey(itemType, 0, (short)templateId, -1));
            previewData.Set(key, data.Key);
            DomainManager.Mod.SetSerializableModData(
                DomainManager.TaiwuEvent.MainThreadDataContext, ModEntry.StaticModIdStr, "ItemPreviews", true, previewData);
        }

        __result = data;
        return false;
    }
}