using System;
using UnityEngine;

namespace XMLData.Parsers
{
	// Token: 0x020013AD RID: 5037
	public static class Vector2Parser
	{
		// Token: 0x06009DDC RID: 40412 RVA: 0x003ED903 File Offset: 0x003EBB03
		public static Vector2 Parse(string _value)
		{
			return StringParsers.ParseVector2(_value);
		}

		// Token: 0x06009DDD RID: 40413 RVA: 0x003ED90B File Offset: 0x003EBB0B
		public static string Unparse(Vector2 _value)
		{
			return string.Format("{0},{1}", _value.x.ToCultureInvariantString(), _value.y.ToCultureInvariantString());
		}
	}
}
