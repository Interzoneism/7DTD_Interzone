using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200137B RID: 4987
	public class PositionXmlElement : XmlElement, IXmlLineInfo
	{
		// Token: 0x06009C41 RID: 40001 RVA: 0x003E1640 File Offset: 0x003DF840
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo) : base(prefix, localName, namespaceURI, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C42 RID: 40002 RVA: 0x003E1672 File Offset: 0x003DF872
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001051 RID: 4177
		// (get) Token: 0x06009C43 RID: 40003 RVA: 0x003E167A File Offset: 0x003DF87A
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001052 RID: 4178
		// (get) Token: 0x06009C44 RID: 40004 RVA: 0x003E1682 File Offset: 0x003DF882
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078D2 RID: 30930
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078D3 RID: 30931
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078D4 RID: 30932
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
