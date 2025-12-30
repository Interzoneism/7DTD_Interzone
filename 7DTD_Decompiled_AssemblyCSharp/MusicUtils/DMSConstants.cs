using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace MusicUtils
{
	// Token: 0x020016F2 RID: 5874
	public static class DMSConstants
	{
		// Token: 0x04008B2E RID: 35630
		public static EnumDictionary<SectionType, string> SectionAbbrvs = new EnumDictionary<SectionType, string>
		{
			{
				SectionType.Exploration,
				"Exp"
			},
			{
				SectionType.Suspense,
				"Sus"
			},
			{
				SectionType.Combat,
				"Cbt"
			},
			{
				SectionType.Bloodmoon,
				"Bld"
			}
		};

		// Token: 0x04008B2F RID: 35631
		public static EnumDictionary<LayerType, string> LayerAbbrvs = new EnumDictionary<LayerType, string>
		{
			{
				LayerType.Primary,
				"Pri"
			},
			{
				LayerType.PrimaryPairable1,
				"Pp1"
			},
			{
				LayerType.PrimarySupporting,
				"Psp"
			},
			{
				LayerType.Secondary,
				"Sec"
			},
			{
				LayerType.LongEffects,
				"Lfx"
			},
			{
				LayerType.ShortEffects,
				"Sfx"
			}
		};

		// Token: 0x04008B30 RID: 35632
		public static EnumDictionary<PlacementType, string> PlacementAbbrv = new EnumDictionary<PlacementType, string>
		{
			{
				PlacementType.Begin,
				"a"
			},
			{
				PlacementType.Loop,
				"b"
			},
			{
				PlacementType.End,
				"c"
			}
		};

		// Token: 0x04008B31 RID: 35633
		public const int cChannels = 2;

		// Token: 0x04008B32 RID: 35634
		public const int cFrequency = 44100;

		// Token: 0x04008B33 RID: 35635
		public static List<SectionType> LayeredSections = new List<SectionType>
		{
			SectionType.Exploration,
			SectionType.Suspense,
			SectionType.Combat,
			SectionType.Bloodmoon
		};
	}
}
