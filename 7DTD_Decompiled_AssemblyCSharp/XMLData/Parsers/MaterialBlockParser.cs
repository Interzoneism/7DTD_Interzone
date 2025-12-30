using System;

namespace XMLData.Parsers
{
	// Token: 0x020013AB RID: 5035
	public static class MaterialBlockParser
	{
		// Token: 0x06009DD8 RID: 40408 RVA: 0x003ED8EE File Offset: 0x003EBAEE
		public static MaterialBlock Parse(string _value)
		{
			return MaterialBlock.materials[_value];
		}

		// Token: 0x06009DD9 RID: 40409 RVA: 0x003ED8FB File Offset: 0x003EBAFB
		public static string Unparse(MaterialBlock _value)
		{
			return _value.id;
		}
	}
}
