using System.Collections.Generic;
using System.Linq;
using Config;
using Config.ConfigCells.Character;
using GameData.Common;
using GameData.Domains;
using GameData.Domains.Building;
using GameData.Domains.Taiwu;
using HarmonyLib;

#pragma warning disable CS8602

namespace VillagerIdentities;

[HarmonyPatch]
public static class TaiwuDomainPatch
{
    /// <summary>
    /// 在停止村民工作之后将村民等级设置为0
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "StopVillagerWork")]
    public static void StopVillagerWorkPostfix(DataContext context, short areaId, short blockId, sbyte workType,
        Dictionary<int, VillagerWorkData> ____villagerWork)
    {
        foreach (var (charId, workData) in ____villagerWork)
        {
            if ((workData.WorkType != workType && workType != -1) || workData.AreaId != areaId ||
                workData.BlockId != blockId) continue;
            DomainManager.Character.GmCmd_ForceChangeGrade(context, charId, 0, true);
            return;
        }
    }
    
    /// <summary>
    /// 在移除村民工作之后将村民等级设置为0
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "RemoveVillagerWork")]
    public static void RemoveVillagerWorkPostfix(DataContext context, int charId) {
        DomainManager.Character.GmCmd_ForceChangeGrade(context, charId, 0, true);
    }

    /// <summary>
    /// 在设置村民工作之后设置对应村民等级
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "SetVillagerWork")]
    public static void SetVillagerWorkPostfix(DataContext context, int charId, VillagerWorkData workData)
    {
        // 0村民 1农户 2匠人 3大夫 4商人 5文人 6护冢 7村长 8太吾传入
        sbyte grade = workData.WorkType switch
        {
            VillagerWorkType.Build => 2,
            VillagerWorkType.CollectResource => 1,
            VillagerWorkType.ShopManage => DomainManager.Building
                    .GetElement_BuildingBlocks(new BuildingBlockKey(workData.AreaId, workData.BlockId,
                        workData.BuildingBlockIndex)).TemplateId switch
                {
                    (>=24 and <= 33) => 1,
                    (>=34 and <= 43) => 2,
                    (>=150 and <= 158) => 3,
                    _ => 4
                },
            VillagerWorkType.CollectTribute => 5,
            VillagerWorkType.KeepGrave => 6,
            VillagerWorkType.Job => 5,
            _ => 0
        };

        DomainManager.Character.GmCmd_ForceChangeGrade(context, charId, grade, true);
    }

    public static OrganizationMemberItem createOrganizationMemberItem(short arg0, int arg1, sbyte arg2, sbyte arg3,
        bool arg4, sbyte arg5, short arg6, sbyte arg7, sbyte arg8, sbyte arg9, sbyte arg10, sbyte arg11, byte arg12,
        int[] arg13, short arg14, sbyte arg15, short arg16, sbyte arg17, sbyte arg18, List<short> arg19,
        List<short> arg20, int[] arg21, bool arg22, List<short> arg23, short[] arg24,
        PresetEquipmentItemWithProb[] arg25, PresetEquipmentItem arg26, List<PresetInventoryItem> arg27,
        List<PresetOrgMemberCombatSkill> arg28, short[] arg29, int arg30, int arg31, int arg32, short[] arg33,
        short[] arg34, short[] arg35, List<sbyte> arg36, short arg37)
    {
        var item = new OrganizationMemberItem();
        typeof(OrganizationMemberItem).GetField("TemplateId").SetValue(item, arg0);
        typeof(OrganizationMemberItem).GetField("GradeName").SetValue(item, "");
        typeof(OrganizationMemberItem).GetField("Grade").SetValue(item, arg2);
        typeof(OrganizationMemberItem).GetField("Amount").SetValue(item, arg3);
        typeof(OrganizationMemberItem).GetField("RestrictPrincipalAmount").SetValue(item, arg4);
        typeof(OrganizationMemberItem).GetField("Gender").SetValue(item, arg5);
        typeof(OrganizationMemberItem).GetField("SurnameId").SetValue(item, arg6);
        typeof(OrganizationMemberItem).GetField("DeputySpouseDowngrade").SetValue(item, arg7);
        typeof(OrganizationMemberItem).GetField("ChildGrade").SetValue(item, arg8);
        typeof(OrganizationMemberItem).GetField("BrotherGrade").SetValue(item, arg9);
        typeof(OrganizationMemberItem).GetField("TeacherGrade").SetValue(item, arg10);
        typeof(OrganizationMemberItem).GetField("ProbOfBecomingMonk").SetValue(item, arg11);
        typeof(OrganizationMemberItem).GetField("MonkType").SetValue(item, arg12);
        typeof(OrganizationMemberItem).GetField("MonasticTitleSuffixes")
            .SetValue(item, arg13.Select(it => "").ToArray());
        typeof(OrganizationMemberItem).GetField("Neili").SetValue(item, arg14);
        typeof(OrganizationMemberItem).GetField("ConsummateLevel").SetValue(item, arg15);
        typeof(OrganizationMemberItem).GetField("ExpPerMonth").SetValue(item, arg16);
        typeof(OrganizationMemberItem).GetField("Fame").SetValue(item, arg17);
        typeof(OrganizationMemberItem).GetField("ApprenticeProbAdjust").SetValue(item, arg18);
        typeof(OrganizationMemberItem).GetField("FavoriteClothingIds").SetValue(item, arg19);
        typeof(OrganizationMemberItem).GetField("HatedClothingIds").SetValue(item, arg20);
        typeof(OrganizationMemberItem).GetField("SpouseAnonymousTitles")
            .SetValue(item, arg21.Select(it => "").ToArray());
        typeof(OrganizationMemberItem).GetField("CanStroll").SetValue(item, arg22);
        typeof(OrganizationMemberItem).GetField("Minions").SetValue(item, arg23);
        typeof(OrganizationMemberItem).GetField("InitialAges").SetValue(item, arg24);
        typeof(OrganizationMemberItem).GetField("Equipment").SetValue(item, arg25);
        typeof(OrganizationMemberItem).GetField("Clothing").SetValue(item, arg26);
        typeof(OrganizationMemberItem).GetField("Inventory").SetValue(item, arg27);
        typeof(OrganizationMemberItem).GetField("CombatSkills").SetValue(item, arg28);
        typeof(OrganizationMemberItem).GetField("ResourcesAdjust").SetValue(item, arg29);
        typeof(OrganizationMemberItem).GetField("ResourceSatisfyingThreshold").SetValue(item, arg30);
        typeof(OrganizationMemberItem).GetField("ItemSatisfyingThreshold").SetValue(item, arg31);
        typeof(OrganizationMemberItem).GetField("ExpectedWagerValue").SetValue(item, arg32);
        typeof(OrganizationMemberItem).GetField("LifeSkillsAdjust").SetValue(item, arg33);
        typeof(OrganizationMemberItem).GetField("CombatSkillsAdjust").SetValue(item, arg34);
        typeof(OrganizationMemberItem).GetField("MainAttributesAdjust").SetValue(item, arg35);
        typeof(OrganizationMemberItem).GetField("IdentityInteractConfig").SetValue(item, arg36);
        typeof(OrganizationMemberItem).GetField("IdentityActiveAge").SetValue(item, arg37);
        return item;
    }
}