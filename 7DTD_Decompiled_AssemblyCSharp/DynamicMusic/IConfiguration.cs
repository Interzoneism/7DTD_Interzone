using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001755 RID: 5973
	public interface IConfiguration
	{
		// Token: 0x17001406 RID: 5126
		// (get) Token: 0x0600B3B3 RID: 46003
		IList<SectionType> Sections { get; }

		// Token: 0x0600B3B4 RID: 46004
		int CountFor(LayerType _layer);

		// Token: 0x0600B3B5 RID: 46005
		void ParseFromXml(XElement _xmlNode);
	}
}
