using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace DynamicMusic.Legacy.ObjectModel
{
	// Token: 0x02001782 RID: 6018
	public class MusicGroup : EnumDictionary<ThreatLevelLegacyType, ThreatLevel>
	{
		// Token: 0x0600B47D RID: 46205 RVA: 0x0045B9D8 File Offset: 0x00459BD8
		public MusicGroup(int _sampleRate, byte _hbLength)
		{
			this.SampleRate = _sampleRate;
			this.HBLength = _hbLength;
			this.ConfigIDs = new List<int>();
		}

		// Token: 0x0600B47E RID: 46206 RVA: 0x0045B9F9 File Offset: 0x00459BF9
		public static void InitStatic()
		{
			MusicGroup.AllGroups = new List<MusicGroup>();
		}

		// Token: 0x0600B47F RID: 46207 RVA: 0x0045BA05 File Offset: 0x00459C05
		public static void Cleanup()
		{
			if (MusicGroup.AllGroups != null)
			{
				MusicGroup.AllGroups.Clear();
			}
		}

		// Token: 0x04008CD9 RID: 36057
		public static List<MusicGroup> AllGroups;

		// Token: 0x04008CDA RID: 36058
		public List<int> ConfigIDs;

		// Token: 0x04008CDB RID: 36059
		public readonly int SampleRate;

		// Token: 0x04008CDC RID: 36060
		public readonly byte HBLength;
	}
}
