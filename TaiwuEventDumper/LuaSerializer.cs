using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LuaTableSerializer
{

	public class LuaSerializer
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000021E3 File Offset: 0x000003E3
		public static string Serialize(object data)
		{
			return string.Format("{0}", Serializer.ConvertType(data, 1, 0));
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021F7 File Offset: 0x000003F7
		public static Dictionary<object, object> Deserialize(string data)
		{
			return Deserializer.ToDict(data);
		}
	}
}
