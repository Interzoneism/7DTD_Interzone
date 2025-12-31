using System;

namespace XMLData.Exceptions
{
	// Token: 0x020013B4 RID: 5044
	public class RedefinedElementException : XmlParserException
	{
		// Token: 0x06009DE8 RID: 40424 RVA: 0x003ED96C File Offset: 0x003EBB6C
		public RedefinedElementException(string _msg, int _line) : base(_msg, _line)
		{
		}
	}
}
