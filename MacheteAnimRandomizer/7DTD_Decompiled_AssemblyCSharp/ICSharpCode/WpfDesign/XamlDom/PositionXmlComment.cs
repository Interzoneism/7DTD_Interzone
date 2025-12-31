using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200137E RID: 4990
	public class PositionXmlComment : XmlComment, IXmlLineInfo
	{
		// Token: 0x06009C4D RID: 40013 RVA: 0x003E1718 File Offset: 0x003DF918
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlComment(string data, XmlDocument doc, IXmlLineInfo lineInfo) : base(data, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C4E RID: 40014 RVA: 0x003E1744 File Offset: 0x003DF944
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001057 RID: 4183
		// (get) Token: 0x06009C4F RID: 40015 RVA: 0x003E174C File Offset: 0x003DF94C
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001058 RID: 4184
		// (get) Token: 0x06009C50 RID: 40016 RVA: 0x003E1754 File Offset: 0x003DF954
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078DB RID: 30939
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078DC RID: 30940
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078DD RID: 30941
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
