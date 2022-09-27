using System;
using System.Collections.Generic;
using System.Reflection;
using TaiwuModdingLib.Core.Plugin;
using TaiwuModdingLib.Core.Utils;

namespace MynahBaseModBase;

[PluginConfig("MynahBaseModBase", "myna12548", "0")]
public class DummyModEntry: TaiwuRemakePlugin
{
    public override void Initialize()
    {
    }

    public override void Dispose()
    {
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ModSetting : Attribute
{
    protected readonly string Key;
    protected readonly string DisplayName;
    protected readonly string Description;
    protected readonly string SettingType;
    protected readonly string DefaultValue;

    /// <summary>
    /// Mod设置项。如果不使用Lua生成功能的话除了Key其他都不用管
    /// </summary>
    /// <param name="displayName">面板显示名</param>
    /// <param name="key">键。默认为变量名</param>
    /// <param name="description">鼠标放在上面显示的描述，默认为空字符串</param>
    /// <param name="type">类型。默认根据变量类型string还是bool推断。如果用int请用派生类。</param>
    /// <param name="defaultValue">默认值。默认为变量初始值</param>
    public ModSetting(string displayName = "", string key = "", string description = "", string type = "",
        string defaultValue = "")
    {
        this.Key = key;
        this.Description = description;
        this.DisplayName = displayName;
        this.SettingType = type;
        this.DefaultValue = defaultValue;
    }

    public string GetKey(FieldInfo fieldInfo)
    {
        if (this.Key != "") return this.Key;
        return fieldInfo.Name;
    }

    public string GetModSettingType(FieldInfo fieldInfo)
    {
        if (this.SettingType != "") return this.SettingType;
        if (fieldInfo.FieldType == typeof(bool))
        {
            return "Toggle";
        }

        if (fieldInfo.FieldType == typeof(string))
        {
            return "InputField";
        }

        return "Slider";
    }

    public object GetDefaultValue(FieldInfo fieldInfo)
    {
        if (this.DefaultValue != "")
        {
            if (fieldInfo.FieldType == typeof(bool))
            {
                return bool.Parse(DefaultValue);
            }
            
            if (fieldInfo.FieldType == typeof(int))
            {
                return int.Parse(DefaultValue);
            }

            return this.DefaultValue;
        }
        var fieldInfoReflectedType = fieldInfo.ReflectedType;

        if (fieldInfoReflectedType == null || fieldInfo.IsStatic)
        {
            return fieldInfo.GetValue(null);
        }
        else
        {
            return fieldInfo.GetValue(Activator.CreateInstance(fieldInfoReflectedType));
        }
    }

    public virtual Dictionary<object, object> ToDictionary(FieldInfo fieldInfo)
    {
        return new Dictionary<object, object>()
        {
            { "Key", GetKey(fieldInfo) },
            { "SettingType", GetModSettingType(fieldInfo) },
            { "DisplayName", DisplayName },
            { "Description", Description },
            { "DefaultValue", GetDefaultValue(fieldInfo) },
        };
    }
}

public class DropDownModSetting : ModSetting
{
    public string[] Options;

    /// <summary>
    /// 下拉菜单选项。如果不使用lua生成功能的话请用基类ModSetting
    /// </summary>
    /// <param name="displayName">面板显示名</param>
    /// <param name="key">键。默认为变量名</param>
    /// <param name="description">鼠标放在上面显示的描述，默认为空字符串</param>
    /// <param name="defaultValue">默认值。默认为变量初始值</param>
    /// <param name="options">下拉菜单中的选项列表</param>
    public DropDownModSetting(string displayName, string[] options, string key = "", string description = "",
        int defaultValue = 0)
        : base(displayName: displayName, key: key, description: description, type: "Dropdown",
            defaultValue: defaultValue.ToString())
    {
        this.Options = options;
    }

    public override Dictionary<object, object> ToDictionary(FieldInfo fieldInfo)
    {
        var dict = base.ToDictionary(fieldInfo);

        dict["Options"] = Options;

        return dict;
    }
}

public class ToggleGroupModSetting : ModSetting
{
    public string[] Toggles;

    /// <summary>
    /// 单选按钮组选项。如果不使用lua生成功能的话请用基类ModSetting
    /// </summary>
    /// <param name="displayName">面板显示名</param>
    /// <param name="key">键。默认为变量名</param>
    /// <param name="description">鼠标放在上面显示的描述，默认为空字符串</param>
    /// <param name="defaultValue">默认值。默认为变量初始值</param>
    /// <param name="toggles">单选按钮列表</param>
    public ToggleGroupModSetting(string displayName, string[] toggles, string key = "", string description = "",
        int defaultValue = 0)
        : base(displayName: displayName, key: key, description: description, type: "ToggleGroup",
            defaultValue.ToString())
    {
        this.Toggles = toggles;
    }
    
    public override Dictionary<object, object> ToDictionary(FieldInfo fieldInfo)
    {
        var dict = base.ToDictionary(fieldInfo);

        dict["Toggles"] = Toggles;

        return dict;
    }
}

public class SliderModSetting : ModSetting
{
    public int MinValue;
    public int MaxValue;
    public int StepSize;

    /// <summary>
    /// 滑块选项。如果不使用lua生成功能的话请用基类ModSetting
    /// </summary>
    /// <param name="displayName">面板显示名</param>
    /// <param name="key">键。默认为变量名</param>
    /// <param name="description">鼠标放在上面显示的描述，默认为空字符串</param>
    /// <param name="defaultValue">默认值。默认为变量初始值</param>
    /// <param name="minValue">滑动最小值</param>
    /// <param name="maxValue">滑动最大值</param>
    /// <param name="stepSize">滑动步长</param>
    public SliderModSetting(string displayName, string key = "", int minValue = 0, int maxValue = 100, int stepSize = 1,
        string description = "",
        int defaultValue = 0) : base(displayName: displayName, key: key, description: description, type: "Slider",
        defaultValue.ToString())
    {
        this.MaxValue = maxValue;
        this.MinValue = minValue;
        this.StepSize = stepSize;
    }
    
    public override Dictionary<object, object> ToDictionary(FieldInfo fieldInfo)
    {
        var dict = base.ToDictionary(fieldInfo);

        dict["MinValue"] = MinValue;
        dict["MaxValue"] = MaxValue;
        dict["StepSize"] = StepSize;

        return dict;
    }
}