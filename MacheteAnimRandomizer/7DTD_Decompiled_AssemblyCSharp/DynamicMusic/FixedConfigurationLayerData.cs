using System;
using System.Collections.Generic;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001726 RID: 5926
	[Preserve]
	public class FixedConfigurationLayerData : ICountable
	{
		// Token: 0x0600B2AF RID: 45743 RVA: 0x00456FFE File Offset: 0x004551FE
		public FixedConfigurationLayerData()
		{
			this.LayerInstances = new List<List<PlacementType>>();
		}

		// Token: 0x170013D8 RID: 5080
		// (get) Token: 0x0600B2B0 RID: 45744 RVA: 0x00457011 File Offset: 0x00455211
		public int Count
		{
			get
			{
				return this.LayerInstances.Count;
			}
		}

		// Token: 0x0600B2B1 RID: 45745 RVA: 0x0045701E File Offset: 0x0045521E
		public void Add(List<PlacementType> _list)
		{
			this.LayerInstances.Add(_list);
		}

		// Token: 0x04008BFD RID: 35837
		public List<List<PlacementType>> LayerInstances;
	}
}
