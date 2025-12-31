using System;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic
{
	// Token: 0x02001771 RID: 6001
	public struct ThreatLevel : IThreatLevel
	{
		// Token: 0x1700141C RID: 5148
		// (get) Token: 0x0600B3E8 RID: 46056 RVA: 0x0045A007 File Offset: 0x00458207
		// (set) Token: 0x0600B3E9 RID: 46057 RVA: 0x0045A00F File Offset: 0x0045820F
		public ThreatLevelType Category { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700141D RID: 5149
		// (get) Token: 0x0600B3EA RID: 46058 RVA: 0x0045A018 File Offset: 0x00458218
		// (set) Token: 0x0600B3EB RID: 46059 RVA: 0x0045A020 File Offset: 0x00458220
		public float Numeric
		{
			get
			{
				return this.numeric;
			}
			set
			{
				if (value < 0.3f && Time.time - this.suspenseTimer <= 30f)
				{
					this.numeric = 0.3f;
					this.Category = ThreatLevelType.Spooked;
					return;
				}
				this.numeric = value;
				this.Category = ((value < 0.3f) ? ThreatLevelType.Safe : ((value < 0.7f) ? ThreatLevelType.Spooked : ThreatLevelType.Panicked));
				if (this.Category == ThreatLevelType.Spooked)
				{
					this.suspenseTimer = Time.time;
					return;
				}
				if (this.Category == ThreatLevelType.Panicked)
				{
					this.suspenseTimer = 0f;
				}
			}
		}

		// Token: 0x04008C82 RID: 35970
		public const float SPOOKED_THRESHOLD = 0.3f;

		// Token: 0x04008C83 RID: 35971
		public const float PANICKED_THRESHOLD = 0.7f;

		// Token: 0x04008C84 RID: 35972
		[PublicizedFrom(EAccessModifier.Private)]
		public const float SPOOKED_DECAY_TIME = 30f;

		// Token: 0x04008C85 RID: 35973
		[PublicizedFrom(EAccessModifier.Private)]
		public float suspenseTimer;

		// Token: 0x04008C87 RID: 35975
		[PublicizedFrom(EAccessModifier.Private)]
		public float numeric;
	}
}
