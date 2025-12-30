using System;
using System.Collections;
using System.Xml.Linq;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001753 RID: 5971
	public interface IClipAdapter
	{
		// Token: 0x0600B3AB RID: 45995
		float GetSample(int idx, params float[] _params);

		// Token: 0x17001405 RID: 5125
		// (get) Token: 0x0600B3AC RID: 45996
		bool IsLoaded { get; }

		// Token: 0x0600B3AD RID: 45997
		IEnumerator Load();

		// Token: 0x0600B3AE RID: 45998
		void LoadImmediate();

		// Token: 0x0600B3AF RID: 45999
		void Unload();

		// Token: 0x0600B3B0 RID: 46000
		void SetPaths(int _num, PlacementType _placement, SectionType _section, LayerType _layer, string stress = "");

		// Token: 0x0600B3B1 RID: 46001
		void ParseXml(XElement _xmlNode);
	}
}
