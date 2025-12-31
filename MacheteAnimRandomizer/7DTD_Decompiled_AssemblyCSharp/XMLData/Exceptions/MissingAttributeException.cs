using System;

namespace XMLData.Exceptions
{
	// Token: 0x020013B3 RID: 5043
	public class MissingAttributeException : XmlParserException
	{
		// Token: 0x06009DE7 RID: 40423 RVA: 0x003ED96C File Offset: 0x003EBB6C
		public MissingAttributeException(string _msg, int _line) : base(_msg, _line)
		{
		}
	}
}
