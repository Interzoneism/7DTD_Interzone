using System;

namespace XMLData.Exceptions
{
	// Token: 0x020013B2 RID: 5042
	public class InvalidValueException : XmlParserException
	{
		// Token: 0x06009DE5 RID: 40421 RVA: 0x003ED96C File Offset: 0x003EBB6C
		public InvalidValueException(string _msg, int _line) : base(_msg, _line)
		{
		}

		// Token: 0x06009DE6 RID: 40422 RVA: 0x003ED976 File Offset: 0x003EBB76
		public InvalidValueException(string _msg, int _line, Exception _innerException) : base(_msg, _line, _innerException)
		{
		}
	}
}
