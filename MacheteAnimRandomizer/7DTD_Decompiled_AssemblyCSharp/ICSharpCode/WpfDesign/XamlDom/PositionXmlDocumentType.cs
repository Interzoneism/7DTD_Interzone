using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001380 RID: 4992
	public class PositionXmlDocumentType : XmlDocumentType, IXmlLineInfo
	{
		// Token: 0x06009C55 RID: 40021 RVA: 0x003E179F File Offset: 0x003DF99F
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlDocumentType(string name, string publicId, string systemId, string internalSubset, XmlDocument doc, IXmlLineInfo lineInfo) : base(name, publicId, systemId, internalSubset, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C56 RID: 40022 RVA: 0x003E17D3 File Offset: 0x003DF9D3
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x1700105B RID: 4187
		// (get) Token: 0x06009C57 RID: 40023 RVA: 0x003E17DB File Offset: 0x003DF9DB
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x1700105C RID: 4188
		// (get) Token: 0x06009C58 RID: 40024 RVA: 0x003E17E3 File Offset: 0x003DF9E3
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078E1 RID: 30945
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078E2 RID: 30946
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078E3 RID: 30947
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
