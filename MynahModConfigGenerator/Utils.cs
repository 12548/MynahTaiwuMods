using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LuaTableSerializer
{

	public class Utils
	{
		// Token: 0x06000013 RID: 19 RVA: 0x000025E0 File Offset: 0x000007E0
		public static void PrintDict(Dictionary<object, object> dict, int nest = 0)
		{
			foreach (KeyValuePair<object, object> keyValuePair in dict)
			{
				bool flag = keyValuePair.Value is IDictionary;
				if (flag)
				{
					Console.WriteLine(string.Format("{0}K:{1} V: ", Utils.GetNesting(nest), keyValuePair.Key));
					Utils.PrintDict((Dictionary<object, object>)keyValuePair.Value, nest + 1);
				}
				else
				{
					Console.WriteLine(string.Format("{0}K:{1} V:{2}", Utils.GetNesting(nest), keyValuePair.Key, keyValuePair.Value));
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026A0 File Offset: 0x000008A0
		public static string GetNesting(int nesting)
		{
			return new string('\t', nesting);
		}
	}
}
