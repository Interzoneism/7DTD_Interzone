using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200137C RID: 4988
	public class PositionXmlAttribute : XmlAttribute, IXmlLineInfo
	{
		// Token: 0x06009C45 RID: 40005 RVA: 0x003E168A File Offset: 0x003DF88A
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo) : base(prefix, localName, namespaceURI, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C46 RID: 40006 RVA: 0x003E16BC File Offset: 0x003DF8BC
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001053 RID: 4179
		// (get) Token: 0x06009C47 RID: 40007 RVA: 0x003E16C4 File Offset: 0x003DF8C4
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001054 RID: 4180
		// (get) Token: 0x06009C48 RID: 40008 RVA: 0x003E16CC File Offset: 0x003DF8CC
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078D5 RID: 30933
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078D6 RID: 30934
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078D7 RID: 30935
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
