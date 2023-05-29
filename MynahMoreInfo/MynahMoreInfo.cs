using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Config;
using CSharpDiff.Converters;
using CSharpDiff.Diffs;
using FrameWork;
using GameData.Domains;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using MynahBaseModBase;
using MynahMoreInfo.SpriteSheet;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine;
using CombatSkillType = Config.CombatSkillType;

namespace MynahMoreInfo;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public partial class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting("显示不传之秘", description: "显示门派武学列表中的不传之秘")]
    public static bool ShowNonPublicSkill = true;

    // [ModSetting("左侧人物浮窗", description: "为地图左侧人物列表增加鼠标浮窗")]
    // public static readonly bool ShowMouseTipMapBlockCharList = true;

    [DropDownModSetting("左侧人物浮窗", new[] { "关闭", "原版", "详细文字" }, defaultValue: 2)]
    public static int MouseTipMapBlockCharList = 2;

    [DropDownModSetting("居民人物浮窗",
        new[] { "关闭", "原版", "详细文字" },
        description: "居住、扩建等界面已安排的人物上",
        defaultValue: 2
    )]
    public static int MouseTipResidentView = 2;

    [DropDownModSetting(
        "地块浮窗",
        new[] { "不显示", "按住alt显示", "始终显示（原版）" },
        description: "注意：如果选择按住alt显示，那么需要按住alt再移动鼠标到新的地块来显示浮窗",
        defaultValue: 2
    )]
    public static int MapBlockMouseTipStat = 2;

    [DropDownModSetting(
        "显示人物特性",
        description: "在详细文字式左侧人物浮窗中显示人物特性",
        options: new[] { "关闭", "显示可见特性", "显示全部特性" },
        defaultValue: 2
    )]
    public static int ShowCharFeatures = 2;

    [ModSetting("功法正逆练", description: "显示功法的正逆练效果")]
    public static bool ShowCombatSkillSpecialEffect = true;

    [ModSetting("书籍正逆练", description: "显示书籍所载功法的正逆练效果")]
    public static bool ShowBookSpecialEffect = true;

    [ModSetting("显示打点和施展时间", description: "显示催破功法的打点分布和可施展功法的基本施展时间（施展速度为100%时的施展时间）")]
    public static bool ShowAttackDistribution = true;

    [DropDownModSetting("突出正逆练区别",
        new[] { "始终关闭", "按住alt键时开启", "始终开启" },
        defaultValue: 1,
        description: "标红正练特效与逆练特效之间的区别")]
    public static int HintEffectDiff = 1;

    [ModSetting("显示书籍五行属性", description: "显示功法书对应功法的五行属性")]
    public static bool ShowBookFiveElements = true;

    [ModSetting("学习进度", description: "开启正逆练显示时生效，显示功法、功法书籍的读书、修炼进度，目前显示位置不太好，不喜欢可以关闭")]
    public static bool ShowLearningProgress = true;

    [ModSetting("对话人物浮窗", description: "为事件界面（人物对话互动等）的左右两个人物增加鼠标浮窗")]
    public static bool ShowEventUICharacterMouseTip = true;

    [ModSetting("人物浮窗显示真名", description: "从官方mod拿来的功能，在人物浮窗中显示法号对应的真实姓名")]
    public static bool CharacterMouseTipShowRealName = true;

    [ModSetting("人物浮窗显示可请教技能", description: "在详细人物浮窗中显示可请教的技艺")]
    public static bool ShowLearnableSkill = true;

    [ModSetting("地块浮窗显示人物列表", description: "")]
    public static bool MapBlockMouseTipCharList = true;

    [ModSetting("地块浮窗高亮较多资源", description: "在地块浮窗中将100以上的资源以不同颜色显示")]
    public static bool MapBlockMouseTipHighlightResource = true;

    [ModSetting("浮窗显示坐标和人物ID", description: "")]
    public static bool ShowPosAndId = true;

    // [ModSetting("延迟去除", description: "将官方的tips机制重写为无延迟的旧版本，请务必谨慎使用")]
    // public static bool DelayFix = false;

    [ModSetting("全部详细人物浮窗", description: "将全部的原版人物浮窗替换为详细文字形式")]
    public static bool ReplaceAllCharacterTipToDetail = true;

    [SliderModSetting("显示人物持有物品数",
        minValue: 0,
        maxValue: 15,
        defaultValue: 3,
        description: "在详细人物浮窗中显示人物持有的此数量的最高品级物品，为0则不显示")]
    public static int ShowNpcGoodItemsCount;

    [SliderModSetting("显示人物擅长功法数",
        minValue: 0,
        maxValue: 15,
        defaultValue: 3,
        description: "在详细人物浮窗中显示人物会的此数量的最高品级功法，为0则不显示")]
    public static int ShowNpcGoodBattleSkillsCount;

    public static string StaticModIdStr;

    // private static bool _init;
    // private static IEnumerator _currDelayFix;
    // private static IEnumerator _origDelay;

    public override void OnModSettingUpdate()
    {
        Debug.Log("MynahMoreInfo OnModSettingUpdate");
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);
        InitDelayFix();
    }

    public override void Initialize()
    {
        base.Initialize();
        StaticModIdStr = ModIdStr;
        SpriteAssetManager.Init();
        // _init = true; 
        InitDelayFix();
    }

    private static void InitDelayFix()
    {
        // Debug.Log($"init delay fix: {_init} {DelayFix}");
        // if (_init)
        // {
        //     if (DelayFix && _currDelayFix == null)
        //     {
        //         var yieldHelper = SingletonObject.getInstance<YieldHelper>();
        //         var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        //         var fieldInfo = typeof(MouseTipManager).GetField("_updateMouseOverObjCoroutine", (BindingFlags)(-1));
        //         if (fieldInfo == null) return;
        //         _origDelay = (IEnumerator)fieldInfo.GetValue(mouseTipManager);
        //         yieldHelper.StopCoroutine(_origDelay);
        //         _currDelayFix = MouseTipManagerPatch.UpdateMouseOverObj();
        //         fieldInfo.SetValue(mouseTipManager, _currDelayFix);
        //         yieldHelper.StartYield(_currDelayFix);
        //         Debug.Log("TipsCo Replaced!");
        //     }
        //     else if (!DelayFix && _origDelay != null && _currDelayFix != null)
        //     {
        //         var yieldHelper = SingletonObject.getInstance<YieldHelper>();
        //         var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        //         var fieldInfo = typeof(MouseTipManager).GetField("_updateMouseOverObjCoroutine", (BindingFlags)(-1));
        //         if (fieldInfo == null) return;
        //         yieldHelper.StopCoroutine(_currDelayFix);
        //         _currDelayFix = null;
        //         fieldInfo.SetValue(mouseTipManager, _origDelay);
        //         yieldHelper.StartYield(_origDelay);
        //         Debug.Log("TipsCo Replaced!");
        //     }
        // }
    }
}