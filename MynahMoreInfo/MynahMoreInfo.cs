using System.Diagnostics.CodeAnalysis;
using MynahBaseModBase;
using MynahBaseModFrontend;
using MynahMoreInfo.SpriteSheet;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

namespace MynahMoreInfo;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
// ReSharper disable once ClassNeverInstantiated.Global
public partial class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting("门派武学列表-显示不传之秘", description: "显示门派武学列表中的不传之秘")]
    public static bool ShowNonPublicSkill = true;

    // [DropDownModSetting(
    //     "地块浮窗",
    //     new[] { "不显示", "按住alt显示", "始终显示（原版）" },
    //     description: "注意：如果选择按住alt显示，那么需要按住alt再移动鼠标到新的地块来显示浮窗",
    //     defaultValue: 2
    // )]
    // public static int MapBlockMouseTipStat = 2;

    [ModSetting("地块浮窗-显示人物列表", description: "")]
    public static bool MapBlockMouseTipCharList = true;
    
    [ModSetting("地块浮窗-显示坐标", description: "")]
    public static bool ShowPosAndId = true;

    // [ModSetting("功法浮窗-显示正逆练", description: "显示功法的正逆练效果")]
    // public static bool ShowCombatSkillSpecialEffect = true;
    //
    // [ModSetting("功法书浮窗-显示正逆练", description: "显示书籍所载功法的正逆练效果")]
    // public static bool ShowBookSpecialEffect = true;

    // [ModSetting("功法/功法书浮窗-显示施展时间", description: "显示可施展功法的基本施展时间（施展速度为100%时的施展时间）")]
    // public static bool ShowCastTime = true;
    //
    [DropDownModSetting("功法浮窗-突出正逆练区别",
        new[] { "关闭", "开启" },
        defaultValue: 1,
        description: "标红正练特效与逆练特效之间的区别")]
    public static int HintEffectDiff = 1;

    // [ModSetting("功法书浮窗-显示五行属性", description: "显示功法书对应功法的五行属性")]
    // public static bool ShowBookFiveElements = true;

    // [ModSetting("功法书浮窗-显示学习进度", description: "开启正逆练显示时生效，显示功法书籍的读书、修炼进度，目前显示位置不太好，不喜欢可以关闭")]
    // public static bool ShowLearningProgress = true;
    //
    [DropDownModSetting("人物浮窗样式", new[] { "原版", "Mod版" }, defaultValue: 1, description: "部分新加人物浮窗强制为Mod版")]
    public static int MouseTipCharStyle = 1;

    [ModSetting("左侧人物浮窗")] public static bool MTC_MapBlockCharList = true;

    [ModSetting("居民人物浮窗",
        description: "居住、扩建等界面已安排的人物上")]
    public static bool MTC_ResidentView = true;
    
    [ModSetting("对话人物浮窗", description: "为事件界面（人物对话互动等）的左右两个人物增加鼠标浮窗")]
    public static bool ShowEventUICharacterMouseTip = true;
    
    [ModSetting("经历链接人物浮窗")]
    public static bool MTC_CharacterNameClickLink = true;

    [ModSetting("Mod人物浮窗-显示七元赋性")] public static bool ModMTCShowPersonalities = true;

    [ModSetting("Mod人物浮窗-显示主属性")] public static bool ModMTCShowAttributes = true;

    [ModSetting("Mod人物浮窗-显示人物ID")] public static bool ModMTCShowCharId = true;

    [ModSetting("Mod人物浮窗-显示人物真名", description: "在人物浮窗中显示法号对应的真实姓名")]
    public static bool CharacterMouseTipShowRealName = true;
    
    [DropDownModSetting(
        "Mod人物浮窗-显示人物特性",
        description: "在详细文字式左侧人物浮窗中显示人物特性",
        options: new[] { "关闭", "显示可见特性", "显示全部特性" },
        defaultValue: 2
    )]
    public static int ShowCharFeatures = 2;
    
    [SliderModSetting("Mod人物浮窗-显示人物持有物品",
        minValue: 0,
        maxValue: 15,
        defaultValue: 3,
        description: "在Mod人物浮窗中显示人物持有的此数量的最高品级物品，为0则不显示")]
    public static int ShowNpcGoodItemsCount;

    [SliderModSetting("Mod人物浮窗-显示人物擅长功法",
        minValue: 0,
        maxValue: 15,
        defaultValue: 3,
        description: "在Mod人物浮窗中显示人物会的此数量的最高品级功法，为0则不显示")]
    public static int ShowNpcGoodBattleSkillsCount;

    public static string StaticModIdStr;

    public override void OnModSettingUpdate()
    {
        Debug.Log("MynahMoreInfo OnModSettingUpdate");
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        StaticModIdStr = ModIdStr;
        SpriteAssetManager.Init();
    }
}