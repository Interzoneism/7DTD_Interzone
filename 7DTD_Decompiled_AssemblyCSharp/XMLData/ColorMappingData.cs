using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace XMLData
{
	// Token: 0x0200138E RID: 5006
	[Preserve]
	public class ColorMappingData : IXMLData
	{
		// Token: 0x17001069 RID: 4201
		// (get) Token: 0x06009C83 RID: 40067 RVA: 0x003E1E4F File Offset: 0x003E004F
		public static ColorMappingData Instance
		{
			get
			{
				ColorMappingData result;
				if ((result = ColorMappingData.instance) == null)
				{
					result = (ColorMappingData.instance = new ColorMappingData());
				}
				return result;
			}
		}

		// Token: 0x06009C84 RID: 40068 RVA: 0x003E1E65 File Offset: 0x003E0065
		[PublicizedFrom(EAccessModifier.Private)]
		public ColorMappingData()
		{
			this.IDFromName = new Dictionary<string, int>();
			this.NameFromID = new Dictionary<int, string>();
			this.ColorFromID = new Dictionary<int, Color>();
		}

		// Token: 0x040078FB RID: 30971
		[PublicizedFrom(EAccessModifier.Private)]
		public static ColorMappingData instance;

		// Token: 0x040078FC RID: 30972
		public Dictionary<string, int> IDFromName;

		// Token: 0x040078FD RID: 30973
		public Dictionary<int, string> NameFromID;

		// Token: 0x040078FE RID: 30974
		public Dictionary<int, Color> ColorFromID;
	}
}
