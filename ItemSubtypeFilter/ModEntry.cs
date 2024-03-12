using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;
using TaiWuRPC;
using UnityEngine;

namespace ItemSubtypeFilter;

[PluginConfig("ItemSubtypeFilter", "myna12548", "")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting(displayName: "用于行囊")] public static readonly bool UseInInventory = true;

    [ModSetting(displayName: "用于仓库")] public static readonly bool UseInWarehouse = true;

    [ModSetting(displayName: "用于商店")] public static readonly bool UseInShop = true;

    [ModSetting(displayName: "用于交换藏书")] public static readonly bool UseInBookEx = true;

    [ModSetting(displayName: "用于茶马帮")] public static readonly bool UseInTeaHorse = true;

    [ModSetting(displayName: "用于事件", description: "事件界面选择物品，如偷窃等")]
    public static readonly bool UseInEvent = true;

    [ModSetting(displayName: "书籍子类型筛选")] public static readonly bool BookThirdFilter = true;

    [ModSetting(displayName: "书籍阅读状态筛选", description: "按是否已阅读筛选书籍，需要【太吾RPC框架】前置")]
    public static readonly bool BookReadingStateFilter = true;

    [ModSetting(displayName: "修习功法排序", description: "允许在修习界面对功法排序")]
    public static readonly bool PracticeCombatSkillSort = true;

    [DropDownModSetting(
        displayName: "功法书子类型筛选方式",
        options: new[] { "按类型", "按门派" },
        description: "开启书籍高级筛选时生效（进入过游戏再切换则大退重进游戏生效）",
        defaultValue: 0
    )]
    public static readonly int CombatSkillFilterType = 0;

    #region RPC方法

    public static TWRPCClient client;

    public static readonly string BackendTypeName =
        "ItemAdvanceFilterBackend.ModEntry, ItemAdvanceFilterBackend, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

    public static List<sbyte> GetTotalReadingProgressList(List<int> bookItemIdList)
    {
        if (client == null || !TWRPCClient.IsReady()) return bookItemIdList.Select(_ => (sbyte)0).ToList();

        return (List<sbyte>)client.CallMethod(BackendTypeName, "GetTotalReadingProgressList", bookItemIdList);
    }

    public static sbyte GetTotalReadingProgress(int bookItemId)
    {
        if (client == null || !TWRPCClient.IsReady()) return 0;

        return (sbyte)client.CallMethod(BackendTypeName, "GetTotalReadingProgressList", bookItemId);
    }

    #endregion
    
    public override void OnModSettingUpdate()
    {
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);

        if (BookReadingStateFilter && client == null)
        {
            new Thread(() =>
            {
                while (!TWRPCClient.WaitUnitlLoaded(3000))
                {
                }

                // 等待后端加载
                Thread.Sleep(2000);
                client = TWRPCClient.CreateStream("ItemAdvanceFilter");

                Debug.Log("RPC Stream Created");
            }).Start();
        }
    }


}