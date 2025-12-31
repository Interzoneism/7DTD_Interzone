using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001386 RID: 4998
	public class PositionXmlDeclaration : XmlDeclaration, IXmlLineInfo
	{
		// Token: 0x06009C6D RID: 40045 RVA: 0x003E1943 File Offset: 0x003DFB43
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlDeclaration(string version, string encoding, string standalone, XmlDocument doc, IXmlLineInfo lineInfo) : base(version, encoding, standalone, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C6E RID: 40046 RVA: 0x003E1975 File Offset: 0x003DFB75
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001067 RID: 4199
		// (get) Token: 0x06009C6F RID: 40047 RVA: 0x003E197D File Offset: 0x003DFB7D
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001068 RID: 4200
		// (get) Token: 0x06009C70 RID: 40048 RVA: 0x003E1985 File Offset: 0x003DFB85
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078F3 RID: 30963
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078F4 RID: 30964
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078F5 RID: 30965
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
