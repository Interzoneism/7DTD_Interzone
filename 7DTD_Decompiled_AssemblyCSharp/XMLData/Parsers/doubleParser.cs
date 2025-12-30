using System;
using System.Globalization;
using XMLData.Exceptions;

namespace XMLData.Parsers
{
	// Token: 0x020013A7 RID: 5031
	public static class doubleParser
	{
		// Token: 0x06009DD0 RID: 40400 RVA: 0x003ED860 File Offset: 0x003EBA60
		public static double Parse(string _value)
		{
			double result;
			if (StringParsers.TryParseDouble(_value, out result, 0, -1, NumberStyles.Any))
			{
				return result;
			}
			throw new InvalidValueException("Expected double value, found \"" + _value + "\"", -1);
		}

		// Token: 0x06009DD1 RID: 40401 RVA: 0x003ED896 File Offset: 0x003EBA96
		public static string Unparse(double _value)
		{
			return _value.ToCultureInvariantString();
		}
	}
}
