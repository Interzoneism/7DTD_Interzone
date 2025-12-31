using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001427 RID: 5159
	public class District
	{
		// Token: 0x0600A058 RID: 41048 RVA: 0x003F6D2C File Offset: 0x003F4F2C
		public District()
		{
		}

		// Token: 0x0600A059 RID: 41049 RVA: 0x003F6D4C File Offset: 0x003F4F4C
		public District(District _other)
		{
			this.name = _other.name;
			this.prefabName = _other.prefabName;
			this.tag = _other.tag;
			this.townships = _other.townships;
			this.weight = _other.weight;
			this.preview_color = _other.preview_color;
			this.counter = _other.counter;
			this.avoidedNeighborDistricts = _other.avoidedNeighborDistricts;
			this.Init();
		}

		// Token: 0x0600A05A RID: 41050 RVA: 0x003F6DDC File Offset: 0x003F4FDC
		public void Init()
		{
			this.type = District.Type.None;
			if (this.name.EndsWith("commercial"))
			{
				this.type = District.Type.Commercial;
				return;
			}
			if (this.name.EndsWith("downtown"))
			{
				this.type = District.Type.Downtown;
				return;
			}
			if (this.name.EndsWith("gateway"))
			{
				this.type = District.Type.Gateway;
				return;
			}
			if (this.name.EndsWith("rural"))
			{
				this.type = District.Type.Rural;
			}
		}

		// Token: 0x04007B32 RID: 31538
		public string name;

		// Token: 0x04007B33 RID: 31539
		public string prefabName;

		// Token: 0x04007B34 RID: 31540
		public District.Type type;

		// Token: 0x04007B35 RID: 31541
		public FastTags<TagGroup.Poi> tag;

		// Token: 0x04007B36 RID: 31542
		public FastTags<TagGroup.Poi> townships;

		// Token: 0x04007B37 RID: 31543
		public float weight = 0.5f;

		// Token: 0x04007B38 RID: 31544
		public Color preview_color;

		// Token: 0x04007B39 RID: 31545
		public int counter;

		// Token: 0x04007B3A RID: 31546
		public bool spawnCustomSizePrefabs;

		// Token: 0x04007B3B RID: 31547
		public List<string> avoidedNeighborDistricts = new List<string>();

		// Token: 0x02001428 RID: 5160
		public enum Type
		{
			// Token: 0x04007B3D RID: 31549
			None,
			// Token: 0x04007B3E RID: 31550
			Commercial,
			// Token: 0x04007B3F RID: 31551
			Downtown,
			// Token: 0x04007B40 RID: 31552
			Gateway,
			// Token: 0x04007B41 RID: 31553
			Rural
		}
	}
}
