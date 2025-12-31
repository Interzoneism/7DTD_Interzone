using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001384 RID: 4996
	public class PositionXmlText : XmlText, IXmlLineInfo
	{
		// Token: 0x06009C65 RID: 40037 RVA: 0x003E18BB File Offset: 0x003DFABB
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlText(string text, XmlDocument doc, IXmlLineInfo lineInfo) : base(text, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C66 RID: 40038 RVA: 0x003E18E7 File Offset: 0x003DFAE7
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x17001063 RID: 4195
		// (get) Token: 0x06009C67 RID: 40039 RVA: 0x003E18EF File Offset: 0x003DFAEF
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001064 RID: 4196
		// (get) Token: 0x06009C68 RID: 40040 RVA: 0x003E18F7 File Offset: 0x003DFAF7
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078ED RID: 30957
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078EE RID: 30958
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078EF RID: 30959
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
