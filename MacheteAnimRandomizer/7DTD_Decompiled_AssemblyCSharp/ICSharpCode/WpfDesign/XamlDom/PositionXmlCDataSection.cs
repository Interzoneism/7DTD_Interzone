using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200137D RID: 4989
	public class PositionXmlCDataSection : XmlCDataSection, IXmlLineInfo
	{
		// Token: 0x06009C49 RID: 40009 RVA: 0x003E16D4 File Offset: 0x003DF8D4
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlCDataSection(string data, XmlDocument doc, IXmlLineInfo lineInfo) : base(data, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C4A RID: 40010 RVA: 0x003E1700 File Offset: 0x003DF900
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001055 RID: 4181
		// (get) Token: 0x06009C4B RID: 40011 RVA: 0x003E1708 File Offset: 0x003DF908
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001056 RID: 4182
		// (get) Token: 0x06009C4C RID: 40012 RVA: 0x003E1710 File Offset: 0x003DF910
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078D8 RID: 30936
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078D9 RID: 30937
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078DA RID: 30938
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
