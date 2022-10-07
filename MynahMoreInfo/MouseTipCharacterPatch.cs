using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TMPro;

namespace MynahMoreInfo;

public partial class ModEntry
{
    [HarmonyPatch]
    public static class MouseTipCharacterPatch
    {
        /// <summary>
        /// 纯为了避免出错，设置一个直接跳过的条件
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="dataPool"></param>
        /// <returns></returns>
        [HarmonyPatch(typeof(MouseTipCharacter), "OnGetCharDisplayData")]
        [HarmonyPrefix]
        static bool MouseTipOnGetDataPrefix(int offset, RawDataPool dataPool)
        {
            List<CharacterDisplayData> list = EasyPool.Get<List<CharacterDisplayData>>();
            list.Clear();
            Serializer.Deserialize(dataPool, offset, ref list);
            if (list.Count != 1)
            {
                EasyPool.Free(list);
                return false;
            }

            EasyPool.Free(list);
            return true;
        }

        // static string GetIdentityText(CharacterItem item, OrganizationInfo organizationInfo)
        // {
        //     OrganizationItem organizationItem = Organization.Instance[organizationInfo.OrgTemplateId];
        //
        //     OrganizationMemberItem organizationMemberItem =
        //         OrganizationMember.Instance[organizationItem.Members[(int)organizationInfo.Grade]];
        //     bool flag4 = item.ActualAge >= 0 && item.ActualAge < organizationMemberItem.IdentityActiveAge;
        //     if (flag4)
        //     {
        //         string text = LocalStringManager.Get((ushort)((AgeGroup.GetAgeGroup(item.ActualAge) == 0)
        //             ? 2027
        //             : ((item.Gender == 0) ? 2029 : 2028)));
        //         text = string.Concat(new string[]
        //         {
        //             "<color=",
        //             Colors.Instance.GradeColors[(int)organizationInfo.Grade].ColorToHexString("#"),
        //             ">",
        //             text,
        //             "</color>"
        //         });
        //         return text;
        //     }
        //
        //     string text2 = organizationInfo.Principal
        //         ? organizationMemberItem.GradeName
        //         : organizationMemberItem.SpouseAnonymousTitles[(int)item.Gender];
        //     text2 = string.Concat(new string[]
        //     {
        //         "<color=",
        //         Colors.Instance.GradeColors[organizationInfo.Grade].ColorToHexString("#"),
        //         ">",
        //         text2,
        //         "</color>"
        //     });
        //     return text2;
        // }

        /// <summary>
        /// 吃下官方mod的内容，显示人物法号真名
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="____charId"></param>
        /// <param name="____displayData"></param>
        [HarmonyPostfix, HarmonyPatch(typeof(MouseTipCharacter), "OnGetCharDisplayData")]
        public static void OnGetCharDisplayDataPostfix(
            MouseTipCharacter __instance,
            int ____charId,
            CharacterDisplayData ____displayData)
        {
            if (!ModEntry.CharacterMouseTipShowRealName) return;
            if (____charId < 0 || ____displayData == null)
                return;
            bool isTaiwu = ____charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
            string nameByDisplayData1 = NameCenter.GetCharMonasticTitleOrNameByDisplayData(____displayData, isTaiwu);
            string nameByDisplayData2 = NameCenter.GetNameByDisplayData(____displayData, isTaiwu, true);
            string color = ____displayData.AliveState == 0 ? "white" : "red";
            if (nameByDisplayData1 != nameByDisplayData2)
                __instance.CGet<TextMeshProUGUI>("Title").text =
                    $"{(object)nameByDisplayData1}/{(object)nameByDisplayData2}({(object)____charId})".SetColor(color);
            else
                __instance.CGet<TextMeshProUGUI>("Title").text =
                    $"{(object)nameByDisplayData1}({(object)____charId})".SetColor(color);
        }
    }
}