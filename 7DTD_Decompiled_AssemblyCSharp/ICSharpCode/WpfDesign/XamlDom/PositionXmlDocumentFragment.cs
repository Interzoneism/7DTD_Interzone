using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200137F RID: 4991
	public class PositionXmlDocumentFragment : XmlDocumentFragment, IXmlLineInfo
	{
		// Token: 0x06009C51 RID: 40017 RVA: 0x003E175C File Offset: 0x003DF95C
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlDocumentFragment(XmlDocument doc, IXmlLineInfo lineInfo) : base(doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C52 RID: 40018 RVA: 0x003E1787 File Offset: 0x003DF987
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001059 RID: 4185
		// (get) Token: 0x06009C53 RID: 40019 RVA: 0x003E178F File Offset: 0x003DF98F
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x1700105A RID: 4186
		// (get) Token: 0x06009C54 RID: 40020 RVA: 0x003E1797 File Offset: 0x003DF997
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078DE RID: 30942
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078DF RID: 30943
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078E0 RID: 30944
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
