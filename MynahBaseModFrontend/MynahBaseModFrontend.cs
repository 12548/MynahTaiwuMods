using System;
using System.Reflection;
using GameData.Domains;
using GameData.Domains.Mod;
using GameData.Serializer;
using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace MynahBaseModFrontend;

[PluginConfig("MynahBaseModFrontend", "myna12548", "0")]
public class DummyModEntry : TaiwuRemakePlugin
{
    public override void Initialize()
    {
    }

    public override void Dispose()
    {
    }
}

public class MynahBaseModFrontend
{
    public static ushort GetDomainIdByName(string name)
    {
        return DomainHelper.DomainName2DomainId[name];
    }

    public static ushort GetMethodIdByName(Type domain, string name)
    {
        return (ushort)domain.GetField(name)!.GetValue(null)!;
    }

    /// <summary>
    /// 设置Mod的字符串数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="value">要设置的值</param>
    /// <param name="isArchive">数据是否存入存档</param>
    /// <param name="callback">回调，结果为是否设置成功</param>
    public static void ModSetString(string modIdStr, UIBase uiBase, string key, string value, bool isArchive,
        Action<bool> callback = null)
    {
        uiBase.AsynchMethodCall(GetDomainIdByName("Mod"),
            GetMethodIdByName(typeof(ModDomainHelper.MethodIds), "SetString"), modIdStr, key, isArchive, value,
            (offset, dataPool) =>
            {
                bool result = false;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }

    /// <summary>
    /// 设置Mod的整形数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="value">要设置的值</param>
    /// <param name="isArchive">数据是否存入存档</param>
    /// <param name="callback">回调，结果为是否设置成功</param>
    public static void ModSetInt(string modIdStr, UIBase uiBase, string key, int value, bool isArchive,
        Action<bool> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.SetInt, modIdStr, key, isArchive,
            value, (offset, dataPool) =>
            {
                bool result = false;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }


    /// <summary>
    /// 设置Mod的布尔数据
    /// </summary>
    /// <param name="ModIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="value">要设置的值</param>
    /// <param name="isArchive">数据是否存入存档</param>
    /// <param name="callback">回调，结果为是否设置成功</param>
    public static void ModSetBool(string ModIdStr, UIBase uiBase, string key, bool value, bool isArchive,
        Action<bool> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.SetBool, ModIdStr, key, isArchive,
            value, (offset, dataPool) =>
            {
                bool result = false;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }

    /// <summary>
    /// 设置Mod的自定义数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="value">要设置的值</param>
    /// <param name="isArchive">数据是否存入存档</param>
    /// <param name="callback">回调，结果为是否设置成功</param>
    public static void ModSetCustom(string modIdStr, UIBase uiBase, string key, SerializableModData value,
        bool isArchive, Action<bool> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.SetSerializableModData, modIdStr,
            key, isArchive, value, (offset, dataPool) =>
            {
                bool result = false;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }

    /// <summary>
    /// 获取Mod的字符串数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="isArchive">是否为存入存档的数据</param>
    /// <param name="callback">获取成功回调</param>
    public static void ModGetString(string modIdStr, UIBase uiBase, string key, bool isArchive,
        Action<string> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.GetString, modIdStr, key,
            isArchive, (offset, dataPool) =>
            {
                string result = "";
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }

    /// <summary>
    /// 获取Mod的整形数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="isArchive">是否为存入存档的数据</param>
    /// <param name="callback">获取成功回调</param>
    public static void ModGetInt(string modIdStr, UIBase uiBase, string key, bool isArchive,
        Action<int> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.GetInt, modIdStr, key, isArchive,
            (offset, dataPool) =>
            {
                int result = 0;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }

    /// <summary>
    /// 获取Mod的布尔数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="isArchive">是否为存入存档的数据</param>
    /// <param name="callback">获取成功回调</param>
    public static void ModGetBool(string modIdStr, UIBase uiBase, string key, bool isArchive,
        Action<bool> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.GetBool, modIdStr, key, isArchive,
            (offset, dataPool) =>
            {
                bool result = false;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }


    /// <summary>
    /// 获取Mod的自定义数据
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="key">键</param>
    /// <param name="isArchive">是否为存入存档的数据</param>
    /// <param name="callback">获取成功回调</param>
    public static void ModGetCustom(string modIdStr, UIBase uiBase, string key, bool isArchive,
        Action<SerializableModData> callback = null)
    {
        uiBase.AsynchMethodCall(DomainHelper.DomainIds.Mod, ModDomainHelper.MethodIds.GetSerializableModData, modIdStr,
            key, isArchive, (offset, dataPool) =>
            {
                SerializableModData result = null;
                Serializer.Deserialize(dataPool, offset, ref result);
                callback?.Invoke(result);
            });
    }

    /// <summary>
    /// 调用后端mod的方法，需配合后端mod使用
    /// </summary>
    /// <param name="modIdStr">调用所在的ModId字符串，可通过mod入口TaiwuRemakePlugin实例获取</param>
    /// <param name="uiBase">调用所在UI</param>
    /// <param name="methodName">方法名</param>
    /// <param name="args">参数</param>
    /// <param name="callback">回调</param>
    public static void CallMethodCustom(string modIdStr, UIBase uiBase, string methodName, SerializableModData args,
        Action<SerializableModData> callback = null)
    {
        ModSetCustom(modIdStr, uiBase, $"_{modIdStr}_method_{methodName}_", args, false, handled =>
        {
            // if (!succ)
            // {
            //     callback?.Invoke(null);
            //     return;
            // }

            ModGetCustom(modIdStr, uiBase, $"_{modIdStr}_method_{methodName}_", false, ret => callback?.Invoke(ret));
        });
    }

    /// <summary>
    /// 前端Mod应当在OnModSettingUpdate中调用此方法，使注解的值跟着玩家设置更新
    /// </summary>
    /// <param name="plugin">Mod入口实例</param>
    public static void OnModSettingUpdate(TaiwuRemakePlugin plugin)
    {
        var type = plugin.GetType();
        foreach (var field in type.GetFields((BindingFlags)(-1)))
        {
            var setting = field.GetCustomAttribute(typeof(ModSetting), true);
            if (setting == null) continue;
            var modSetting = (ModSetting)setting;
            switch (modSetting.GetModSettingType(field))
            {
                case "InputField":
                    var strArg = "";
                    ModManager.GetSetting(plugin.ModIdStr, modSetting.GetKey(field), ref strArg);
                    field.SetValue(plugin, strArg);
                    break;

                case "Toggle":
                    var boolArg = false;
                    ModManager.GetSetting(plugin.ModIdStr, modSetting.GetKey(field), ref boolArg);
                    field.SetValue(plugin, boolArg);
                    break;

                default: // ToggleGroup Slider Dropdown 全都是Int
                    var intArg = 0;
                    ModManager.GetSetting(plugin.ModIdStr, modSetting.GetKey(field), ref intArg);
                    field.SetValue(plugin, intArg);
                    break;
            }
        }
    }
}