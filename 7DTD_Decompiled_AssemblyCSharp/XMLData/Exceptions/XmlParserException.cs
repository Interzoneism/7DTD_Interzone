using System;

namespace XMLData.Exceptions
{
	// Token: 0x020013B6 RID: 5046
	public class XmlParserException : Exception
	{
		// Token: 0x170010F8 RID: 4344
		// (get) Token: 0x06009DEA RID: 40426 RVA: 0x003ED981 File Offset: 0x003EBB81
		// (set) Token: 0x06009DEB RID: 40427 RVA: 0x003ED989 File Offset: 0x003EBB89
		public int Line { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x06009DEC RID: 40428 RVA: 0x003ED992 File Offset: 0x003EBB92
		public XmlParserException(string _msg, int _line) : base(_msg)
		{
			this.Line = _line;
		}

		// Token: 0x06009DED RID: 40429 RVA: 0x003ED9A2 File Offset: 0x003EBBA2
		public XmlParserException(string _msg, int _line, Exception _innerException) : base(_msg, _innerException)
		{
			this.Line = _line;
		}

		// Token: 0x170010F9 RID: 4345
		// (get) Token: 0x06009DEE RID: 40430 RVA: 0x003ED9B3 File Offset: 0x003EBBB3
		public override string Message
		{
			get
			{
				return string.Format("{0} (line {1})", base.Message, this.Line);
			}
		}

		// Token: 0x06009DEF RID: 40431 RVA: 0x003ED9D0 File Offset: 0x003EBBD0
		public override string ToString()
		{
			return this.Message;
		}
	}
}
