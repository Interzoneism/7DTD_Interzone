using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001381 RID: 4993
	public class PositionXmlEntityReference : XmlEntityReference, IXmlLineInfo
	{
		// Token: 0x06009C59 RID: 40025 RVA: 0x003E17EB File Offset: 0x003DF9EB
		[PublicizedFrom(EAccessModifier.Internal)]
		public PositionXmlEntityReference(string name, XmlDocument doc, IXmlLineInfo lineInfo) : base(name, doc)
		{
			if (lineInfo != null)
			{
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}

		// Token: 0x06009C5A RID: 40026 RVA: 0x003E1817 File Offset: 0x003DFA17
		public bool HasLineInfo()
		{
			return this.hasLineInfo;
		}

		// Token: 0x1700105D RID: 4189
		// (get) Token: 0x06009C5B RID: 40027 RVA: 0x003E181F File Offset: 0x003DFA1F
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		// Token: 0x1700105E RID: 4190
		// (get) Token: 0x06009C5C RID: 40028 RVA: 0x003E1827 File Offset: 0x003DFA27
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		// Token: 0x040078E4 RID: 30948
		[PublicizedFrom(EAccessModifier.Private)]
		public int lineNumber;

		// Token: 0x040078E5 RID: 30949
		[PublicizedFrom(EAccessModifier.Private)]
		public int linePosition;

		// Token: 0x040078E6 RID: 30950
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasLineInfo;
	}
}
