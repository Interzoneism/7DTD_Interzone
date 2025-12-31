using System;

namespace XMLData.Parsers
{
	// Token: 0x020013AA RID: 5034
	public static class intParser
	{
		// Token: 0x06009DD6 RID: 40406 RVA: 0x003ED8E6 File Offset: 0x003EBAE6
		public static int Parse(string _value)
		{
			return int.Parse(_value);
		}

		// Token: 0x06009DD7 RID: 40407 RVA: 0x00381DEF File Offset: 0x0037FFEF
		public static string Unparse(int _value)
		{
			return _value.ToString();
		}
	}
}
