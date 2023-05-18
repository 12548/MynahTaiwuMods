using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaiwuEventDumper;

namespace LuaTableSerializer
{
	class Serializer
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002208 File Offset: 0x00000408
		internal static object ConvertType(object item, int nesting = 1, int index = 0)
		{
			if (!true)
			{
			}
			object result;
			if (item is bool)
			{
				result = ((bool)item).ToString().ToLower();
			}
			else if (item is int)
			{
				int num = (int)item;
				result = num;
			}
			else if (item is float)
			{
				float num2 = (float)item;
				result = num2;
			}
			else if (item is double)
			{
				double num3 = (double)item;
				result = num3;
			}
			else
			{
				string text = item as string;
				if (text == null)
				{
					IList list = item as IList;
					if (list == null)
					{
						IDictionary dictionary = item as IDictionary;
						if (dictionary == null)
						{
							result = Serializer.TryToLuaString(item, index);
						}
						else
						{
							result = Serializer.DictToLua<IDictionary>(dictionary, nesting);
						}
					}
					else
					{
						result = Serializer.ListToLua<IList>(list, nesting);
					}
				}
				else
				{
					result = Serializer.EscapeString(text);
				}
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022FC File Offset: 0x000004FC
		private static object TryToLuaString(object item, int index)
		{
			object result;
			MethodInfo method = item.GetType().GetMethod("ToLuaString");
			if (method == null)
			{
				return item.ToString();
			}
			
			if (method.GetParameters().Count<ParameterInfo>() > 0)
			{
				result = method.Invoke(item, new object[]
				{
					index
				});
			}
			else
			{
				result = method.Invoke(item, null);
			}
			return result;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000236C File Offset: 0x0000056C
		private static string ConvertKey(object key)
		{
			string result;
			if (key is string)
			{
				if (int.TryParse((string)key, out int intKey))
				{
					// 这里对于数字字符串就直接当数字用了
					result = string.Format("[{0}]", key);
				}
				else
				{
					result = string.Format("[\"{0}\"]", key);
				}
			}
			else
			{
				if (key is int)
					result = string.Format("[{0}]", key);
				else
					throw new ArgumentOutOfRangeException("key",
						string.Format("Not expected key Type value: {0}", key));
			}

			return result;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000023C8 File Offset: 0x000005C8
		private static string DictToLua<T>(T data, int nesting) where T : IDictionary
		{
			string str = "{";
			foreach (object obj in data)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				bool flag = dictionaryEntry.Value == null;
				if (!flag)
				{
					str += string.Format("\n{0}{1} = {2},", Utils.GetNesting(nesting), Serializer.ConvertKey(dictionaryEntry.Key), Serializer.ConvertType(dictionaryEntry.Value, nesting + 1, 0));
				}
			}
			return str + "\n" + Utils.GetNesting(nesting) + "}";
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000248C File Offset: 0x0000068C
		private static string ListToLua<T>(T data, int nesting) where T : IList
		{
			string str = "{";
			int num = 1;
			foreach (object item in data)
			{
				str += string.Format("\n{0}{1} = {2},", Utils.GetNesting(nesting), Serializer.ConvertKey(num), Serializer.ConvertType(item, nesting + 1, num));
				num++;
			}
			return str + "\n" + Utils.GetNesting(nesting) + "}";
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002538 File Offset: 0x00000738
		private static string EscapeString(string data)
		{
			string str = data.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\n", "\\\n").Replace("\r", "\\r").Replace("\"", "\\\"").Replace("'", "\\'");
			return "\"" + str + "\"";
		}
	}
}
