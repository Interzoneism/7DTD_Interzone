using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001385 RID: 4997
	public class PositionXmlWhitespace : XmlWhitespace, IXmlLineInfo
	{
		// Token: 0x06009C69 RID: 40041 RVA: 0x003E18FF File Offset: 0x003DFAFF
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlWhitespace(string text, XmlDocument doc, IXmlLineInfo lineInfo) : base(text, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C6A RID: 40042 RVA: 0x003E192B File Offset: 0x003DFB2B
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001065 RID: 4197
		// (get) Token: 0x06009C6B RID: 40043 RVA: 0x003E1933 File Offset: 0x003DFB33
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001066 RID: 4198
		// (get) Token: 0x06009C6C RID: 40044 RVA: 0x003E193B File Offset: 0x003DFB3B
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078F0 RID: 30960
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078F1 RID: 30961
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078F2 RID: 30962
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
