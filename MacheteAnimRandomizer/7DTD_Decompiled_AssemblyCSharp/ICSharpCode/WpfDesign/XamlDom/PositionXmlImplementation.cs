using System;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x02001387 RID: 4999
	public class PositionXmlImplementation : XmlImplementation
	{
		// Token: 0x06009C71 RID: 40049 RVA: 0x003E198D File Offset: 0x003DFB8D
		public override XmlDocument CreateDocument()
		{
			return new PositionXmlDocument();
		}
	}
}
