using System.Reflection;

namespace MynahBaseModBase;

public static class BaseUtils
{
    public static object GetFieldValue(this object instance, string key)
    {
        return instance.GetType().GetField(key, (BindingFlags)(-1))!.GetValue(instance);
    }
    
    public static void SetFieldValue(this object instance, string key, object value)
    {
        instance.GetType().GetField(key, (BindingFlags)(-1))!.SetValue(instance, value);
    }

    public static object CallMethod(this object instance, string key, object[] args = null)
    {
        return instance.GetType().GetMethod(key, (BindingFlags)(-1))!.Invoke(instance, args ?? new object[] { });
    }
}