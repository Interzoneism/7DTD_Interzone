using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001383 RID: 4995
	public class PositionXmlSignificantWhitespace : XmlSignificantWhitespace, IXmlLineInfo
	{
		// Token: 0x06009C61 RID: 40033 RVA: 0x003E1877 File Offset: 0x003DFA77
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlSignificantWhitespace(string text, XmlDocument doc, IXmlLineInfo lineInfo) : base(text, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C62 RID: 40034 RVA: 0x003E18A3 File Offset: 0x003DFAA3
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001061 RID: 4193
		// (get) Token: 0x06009C63 RID: 40035 RVA: 0x003E18AB File Offset: 0x003DFAAB
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001062 RID: 4194
		// (get) Token: 0x06009C64 RID: 40036 RVA: 0x003E18B3 File Offset: 0x003DFAB3
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078EA RID: 30954
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078EB RID: 30955
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078EC RID: 30956
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
