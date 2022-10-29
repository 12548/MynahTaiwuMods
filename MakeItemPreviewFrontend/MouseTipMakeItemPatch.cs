using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Config;
using FrameWork;
using FrameWork.Linq;
using GameData.Domains;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Material = Config.Material;

namespace MakeItemPreviewFrontend;

[HarmonyPatch]
public class MouseTipMakeItemPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(MouseTipMakeItem), "Init")]
    static void InitPostfix(MouseTipMakeItem __instance, ArgumentBox argsBox, Dictionary<Refers, List<Refers>> ____stageToItemDict)
    {
        argsBox.Get<UI_Make.MakeResult>("MakeResult", out var makeResult);
        foreach (var pair in ____stageToItemDict)
        {
            var refers = pair.Value;
            foreach (var refer in refers)
            {
                var tmp = refer.transform.Find("Name").GetComponent<TextMeshProUGUI>();
                var pattern = @"<.*?>";
                var replacement = "";
                var input = tmp.text;
                var result = Regex.Replace(input, pattern, replacement).Trim();
                var itemType = makeResult.ItemType;
                var templateId = FindItemByName(itemType, result);
                
                if (templateId <= -1) continue;

                void Callback(int offset, RawDataPool datapool)
                {
                    ItemDisplayData data = null;
                    Serializer.Deserialize(datapool, offset, ref data);

                    if (data == null)
                    {
                        Debug.Log("failed!");
                        return;
                    }

                    var mouseTipDisplayer = refer.GetComponent<MouseTipDisplayer>();

                    if (mouseTipDisplayer == null)
                    {
                        mouseTipDisplayer = refer.gameObject.AddComponent<MouseTipDisplayer>();
                    }

                    mouseTipDisplayer.Type = MouseTipManager.ItemTypeToTipType[makeResult.ItemType];
                    mouseTipDisplayer.enabled = true;
                    mouseTipDisplayer.RuntimeParam = new ArgumentBox()
                        // .SetObject("_mip_MakeResult", makeResult)
                        .SetObject("ItemData", data);
                }

                var ik = new ItemKey
                {
                    Id = -12548,
                    ItemType = makeResult.ItemType,
                    TemplateId = templateId
                };

                __instance.AsynchMethodCall(DomainHelper.DomainIds.Item, ItemDomainHelper.MethodIds.GetItemDisplayData, ik, -12548,
                    Callback);
            }
        }
    }

    public static short FindItemByName(sbyte itemType, string name)
    {
        return itemType switch
        {
            0 => Weapon.Instance.First(it => it.Name == name).TemplateId,
            1 => Armor.Instance.First(it => it.Name == name).TemplateId,
            2 => Accessory.Instance.First(it => it.Name == name).TemplateId,
            3 => Clothing.Instance.First(it => it.Name == name).TemplateId,
            4 => Carrier.Instance.First(it => it.Name == name).TemplateId,
            5 => Material.Instance.First(it => it.Name == name).TemplateId,
            6 => CraftTool.Instance.First(it => it.Name == name).TemplateId,
            7 => Food.Instance.First(it => it.Name == name).TemplateId,
            8 => Medicine.Instance.First(it => it.Name == name).TemplateId,
            9 => TeaWine.Instance.First(it => it.Name == name).TemplateId,
            10 => SkillBook.Instance.First(it => it.Name == name).TemplateId,
            11 => Cricket.Instance.First(it => it.Name == name).TemplateId,
            12 => Misc.Instance.First(it => it.Name == name).TemplateId,
            _ => -1
        };
    }
}