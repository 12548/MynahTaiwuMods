using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LuaTableSerializer
{

	public class Deserializer
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002094 File Offset: 0x00000294
		internal static Dictionary<object, object> ToDict(string data)
		{
			string input = Regex.Replace(data, "\\[(.+?)\\] = ", "$1:");
			string input2 = Regex.Replace(input, "(\\d+):", "\"~$1\":");
			string value = Regex.Replace(input2, ",(\\s*})", "$1");
			Dictionary<string, object> data2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
			return Deserializer.ConvertTypes(data2);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020E8 File Offset: 0x000002E8
		private static Dictionary<object, object> ConvertTypes(Dictionary<string, object> data)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			foreach (KeyValuePair<string, object> keyValuePair in data)
			{
				Dictionary<object, object> dictionary2 = dictionary;
				object key = Deserializer.ConvertKey(keyValuePair.Key);
				JObject jobject = keyValuePair.Value as JObject;
				dictionary2[key] = ((jobject != null) ? Deserializer.ConvertTypes(jobject.ToObject<Dictionary<string, object>>()) : keyValuePair.Value);
			}
			return dictionary;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002178 File Offset: 0x00000378
		private static object ConvertKey(string key)
		{
			bool flag = !key.StartsWith("~");
			object result;
			if (flag)
			{
				result = key;
			}
			else
			{
				key = key.Replace("~", "");
				bool flag2 = key.Contains(".");
				if (flag2)
				{
					result = float.Parse(key);
				}
				else
				{
					result = int.Parse(key);
				}
			}
			return result;
		}
	}
}
