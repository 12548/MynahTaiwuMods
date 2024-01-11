using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Config;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Serializer;
using HarmonyLib;

namespace VillagerIdentityFrontend;

[HarmonyPatch]
public class UI_BuildingManagePatch
{
    /// <summary>
    /// 修正没获取到扩建对应技艺造诣的问题，发现没有则重新获取再调用
    /// </summary>
    [HarmonyPatch(typeof(UI_BuildingManage), "UpdateExpandOperators")]
    [HarmonyPrefix]
    static bool UpdateExpandOperatorsPrefix(
        UI_BuildingManage __instance,
        int[] ____operatorList,
        Dictionary<int, short> ____propertyValueDict,
        BuildingBlockItem ____configData)
    {
        if (____configData.RequireLifeSkillType < 0) return true;

        // 筛掉不存在的人（-1）
        var realOperators = ____operatorList.Where(it => it > -1).ToList();
        foreach (var operatorCharId in realOperators)
        {
            if (!____propertyValueDict.ContainsKey(operatorCharId))
            {
                // 如果有人缺少所需属性（技艺造诣） 那么这里要获取
                __instance.AsyncMethodCall(
                    DomainHelper.DomainName2DomainId["Character"],
                    MynahBaseModFrontend.MynahBaseModFrontend.GetMethodIdByName(
                        typeof(CharacterDomainHelper.MethodIds),
                        "GetCharacterLifeSkillAttainmentList"),
                    realOperators,
                    ____configData.RequireLifeSkillType,
                    (offset, dp) =>
                    {
                        List<short> result = new List<short>();
                        Serializer.Deserialize(dp, offset, ref result);
                        for (int i = 0; i < result.Count; i++)
                        {
                            ____propertyValueDict[realOperators[i]] = result[i];
                        }

                        UpdateExpandOperatorsStub(__instance);
                    });

                // 不能正常调用，要在回调中调用了
                return false;
            }
        }

        // 没人缺就不用管了
        return true;
    }

    [HarmonyPatch(typeof(UI_BuildingManage), "UpdateExpandOperators")]
    [HarmonyReversePatch]
    static void UpdateExpandOperatorsStub(object instance)
    {
        throw new Exception("stub");
    }
}