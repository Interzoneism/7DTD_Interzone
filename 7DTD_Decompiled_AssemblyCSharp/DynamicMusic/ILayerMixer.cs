using System;
using System.Collections;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200175D RID: 5981
	public interface ILayerMixer
	{
		// Token: 0x1700140A RID: 5130
		// (get) Token: 0x0600B3BD RID: 46013
		// (set) Token: 0x0600B3BE RID: 46014
		SectionType Sect { get; set; }

		// Token: 0x1700140B RID: 5131
		float this[int _idx]
		{
			get;
		}

		// Token: 0x0600B3C0 RID: 46016
		IEnumerator Load();

		// Token: 0x0600B3C1 RID: 46017
		void Unload();
	}
}
