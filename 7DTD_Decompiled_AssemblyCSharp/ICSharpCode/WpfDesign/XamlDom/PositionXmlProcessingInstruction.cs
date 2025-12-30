using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001382 RID: 4994
	public class PositionXmlProcessingInstruction : XmlProcessingInstruction, IXmlLineInfo
	{
		// Token: 0x06009C5D RID: 40029 RVA: 0x003E182F File Offset: 0x003DFA2F
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlProcessingInstruction(string target, string data, XmlDocument doc, IXmlLineInfo lineInfo) : base(target, data, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C5E RID: 40030 RVA: 0x003E185F File Offset: 0x003DFA5F
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x1700105F RID: 4191
		// (get) Token: 0x06009C5F RID: 40031 RVA: 0x003E1867 File Offset: 0x003DFA67
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x17001060 RID: 4192
		// (get) Token: 0x06009C60 RID: 40032 RVA: 0x003E186F File Offset: 0x003DFA6F
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078E7 RID: 30951
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078E8 RID: 30952
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078E9 RID: 30953
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
