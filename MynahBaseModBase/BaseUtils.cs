using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

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

    public static string DumpObj(this object target, int maxDepth = 1, int currDepth = 0)
    {
        if(target == null) return "null";
        
        var type = target.GetType();

        if (type.IsPrimitive)
        {
            return target.ToString();
        }
        
        var sb = new StringBuilder();
        var fieldInfos = type.GetFields((BindingFlags)(-1));
        
        foreach (var fieldInfo in fieldInfos)
        {
            for (var i = 0; i < currDepth; i++)
            {
                sb.Append("  ");
            }

            sb.Append($"{fieldInfo.Name}: ");

            var value = fieldInfo.GetValue(target);

            if (value == null)
            {
                sb.AppendLine("null");
            } else if (value.GetType().IsPrimitive)
            {
                sb.AppendLine(value.ToString());
            }
            else if (currDepth < maxDepth)
            {
                sb.AppendLine();
                sb.Append(value.DumpObj(maxDepth, currDepth + 1));
            }
            else
            {
                sb.AppendLine(value.ToString());
            }
        }

        return sb.ToString();
    }
}