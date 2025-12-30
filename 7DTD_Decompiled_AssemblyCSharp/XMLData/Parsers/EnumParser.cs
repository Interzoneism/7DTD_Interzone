using System;

namespace XMLData.Parsers
{
	// Token: 0x020013A8 RID: 5032
	public static class EnumParser
	{
		// Token: 0x06009DD2 RID: 40402 RVA: 0x0039DA23 File Offset: 0x0039BC23
		public static TEnum Parse<TEnum>(string _value) where TEnum : struct, IConvertible
		{
			return EnumUtils.Parse<TEnum>(_value, false);
		}

		// Token: 0x06009DD3 RID: 40403 RVA: 0x003ED89E File Offset: 0x003EBA9E
		public static string Unparse<TEnum>(TEnum _value) where TEnum : struct, IConvertible
		{
			return _value.ToStringCached<TEnum>();
		}
	}
}
