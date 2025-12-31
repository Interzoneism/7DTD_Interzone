using System;
using XMLData.Exceptions;

namespace XMLData.Parsers
{
	// Token: 0x020013A5 RID: 5029
	public static class boolParser
	{
		// Token: 0x06009DCB RID: 40395 RVA: 0x003ED653 File Offset: 0x003EB853
		public static bool Parse(string _value)
		{
			if (_value == "true")
			{
				return true;
			}
			if (_value == "false")
			{
				return false;
			}
			throw new InvalidValueException("Expected bool value, found \"" + _value + "\"", -1);
		}

		// Token: 0x06009DCC RID: 40396 RVA: 0x003ED689 File Offset: 0x003EB889
		public static string Unparse(bool _value)
		{
			if (!_value)
			{
				return "false";
			}
			return "true";
		}
	}
}
