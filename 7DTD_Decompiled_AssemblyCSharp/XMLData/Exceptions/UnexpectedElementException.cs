using System;

namespace XMLData.Exceptions
{
	// Token: 0x020013B5 RID: 5045
	public class UnexpectedElementException : XmlParserException
	{
		// Token: 0x06009DE9 RID: 40425 RVA: 0x003ED96C File Offset: 0x003EBB6C
		public UnexpectedElementException(string _msg, int _line) : base(_msg, _line)
		{
		}
	}
}
