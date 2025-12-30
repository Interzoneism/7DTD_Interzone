using System;
using System.Globalization;
using XMLData.Exceptions;

namespace XMLData.Parsers
{
	// Token: 0x020013A9 RID: 5033
	public static class floatParser
	{
		// Token: 0x06009DD4 RID: 40404 RVA: 0x003ED8A8 File Offset: 0x003EBAA8
		public static float Parse(string _value)
		{
			float result;
			if (StringParsers.TryParseFloat(_value, out result, 0, -1, NumberStyles.Any))
			{
				return result;
			}
			throw new InvalidValueException("Expected float value, found \"" + _value + "\"", -1);
		}

		// Token: 0x06009DD5 RID: 40405 RVA: 0x003ED8DE File Offset: 0x003EBADE
		public static string Unparse(float _value)
		{
			return _value.ToCultureInvariantString();
		}
	}
}
