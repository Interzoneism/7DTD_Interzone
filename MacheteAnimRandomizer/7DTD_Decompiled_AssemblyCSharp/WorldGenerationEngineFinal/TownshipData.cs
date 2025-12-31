using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001495 RID: 5269
	public class TownshipData
	{
		// Token: 0x0600A311 RID: 41745 RVA: 0x0040E9A8 File Offset: 0x0040CBA8
		public TownshipData(string _name, int _id)
		{
			this.Name = _name;
			this.Id = _id;
			if (_name.EndsWith("roadside"))
			{
				this.Category = TownshipData.eCategory.Roadside;
			}
			else if (_name.EndsWith("rural"))
			{
				this.Category = TownshipData.eCategory.Rural;
			}
			else if (_name.EndsWith("wilderness"))
			{
				this.Category = TownshipData.eCategory.Wilderness;
			}
			WorldBuilderStatic.idToTownshipData[this.Id] = this;
		}

		// Token: 0x04007E38 RID: 32312
		public string Name;

		// Token: 0x04007E39 RID: 32313
		public int Id;

		// Token: 0x04007E3A RID: 32314
		public List<string> SpawnableTerrain = new List<string>();

		// Token: 0x04007E3B RID: 32315
		public bool SpawnCustomSizes;

		// Token: 0x04007E3C RID: 32316
		public bool SpawnTrader = true;

		// Token: 0x04007E3D RID: 32317
		public bool SpawnGateway = true;

		// Token: 0x04007E3E RID: 32318
		public string OutskirtDistrict;

		// Token: 0x04007E3F RID: 32319
		public float OutskirtDistrictPercent;

		// Token: 0x04007E40 RID: 32320
		public FastTags<TagGroup.Poi> Biomes;

		// Token: 0x04007E41 RID: 32321
		public TownshipData.eCategory Category;

		// Token: 0x02001496 RID: 5270
		public enum eCategory
		{
			// Token: 0x04007E43 RID: 32323
			Normal,
			// Token: 0x04007E44 RID: 32324
			Roadside,
			// Token: 0x04007E45 RID: 32325
			Rural,
			// Token: 0x04007E46 RID: 32326
			Wilderness
		}
	}
}
