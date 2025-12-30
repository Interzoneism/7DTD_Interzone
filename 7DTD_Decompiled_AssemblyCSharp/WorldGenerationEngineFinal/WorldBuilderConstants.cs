using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001471 RID: 5233
	public static class WorldBuilderConstants
	{
		// Token: 0x04007D0A RID: 32010
		public static readonly Color32 forestCol = new Color32(0, 64, 0, byte.MaxValue);

		// Token: 0x04007D0B RID: 32011
		public static readonly Color32 burntForestCol = new Color32(186, 0, byte.MaxValue, byte.MaxValue);

		// Token: 0x04007D0C RID: 32012
		public static readonly Color32 desertCol = new Color32(byte.MaxValue, 228, 119, byte.MaxValue);

		// Token: 0x04007D0D RID: 32013
		public static readonly Color32 snowCol = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		// Token: 0x04007D0E RID: 32014
		public static readonly Color32 wastelandCol = new Color32(byte.MaxValue, 168, 0, byte.MaxValue);

		// Token: 0x04007D0F RID: 32015
		public static readonly List<Color32> biomeColorList = new List<Color32>
		{
			WorldBuilderConstants.forestCol,
			WorldBuilderConstants.burntForestCol,
			WorldBuilderConstants.desertCol,
			WorldBuilderConstants.snowCol,
			WorldBuilderConstants.wastelandCol
		};

		// Token: 0x04007D10 RID: 32016
		public const int ForestBiomeWeightDefault = 13;

		// Token: 0x04007D11 RID: 32017
		public const int BurntForestBiomeWeightDefault = 18;

		// Token: 0x04007D12 RID: 32018
		public const int DesertBiomeWeightDefault = 22;

		// Token: 0x04007D13 RID: 32019
		public const int SnowBiomeWeightDefault = 23;

		// Token: 0x04007D14 RID: 32020
		public const int WastelandBiomeWeightDefault = 24;

		// Token: 0x04007D15 RID: 32021
		public static readonly int[] BiomeWeightDefaults = new int[]
		{
			13,
			18,
			22,
			23,
			24
		};
	}
}
