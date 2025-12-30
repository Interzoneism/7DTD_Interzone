using System;
using UnityEngine;

namespace XMLData.Parsers
{
	// Token: 0x020013AE RID: 5038
	public static class Vector3Parser
	{
		// Token: 0x06009DDE RID: 40414 RVA: 0x003ED92D File Offset: 0x003EBB2D
		public static Vector3 Parse(string _value)
		{
			return StringParsers.ParseVector3(_value, 0, -1);
		}

		// Token: 0x06009DDF RID: 40415 RVA: 0x003ED937 File Offset: 0x003EBB37
		public static string Unparse(Vector3 _value)
		{
			return string.Format("{0},{1},{2}", _value.x.ToCultureInvariantString(), _value.y.ToCultureInvariantString(), _value.z.ToCultureInvariantString());
		}
	}
}
