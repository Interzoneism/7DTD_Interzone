using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001615 RID: 5653
	[Preserve]
	public class RequirementGameStatBool : BaseRequirement
	{
		// Token: 0x0600ADD1 RID: 44497 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADD2 RID: 44498 RVA: 0x0044007B File Offset: 0x0043E27B
		public override bool CanPerform(Entity target)
		{
			if (GameStats.GetBool(this.GameStat))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600ADD3 RID: 44499 RVA: 0x0044009A File Offset: 0x0043E29A
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<EnumGameStats>(RequirementGameStatBool.PropGameStat, ref this.GameStat);
		}

		// Token: 0x0600ADD4 RID: 44500 RVA: 0x004400B4 File Offset: 0x0043E2B4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGameStatBool
			{
				Invert = this.Invert,
				GameStat = this.GameStat
			};
		}

		// Token: 0x040086FC RID: 34556
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumGameStats GameStat = EnumGameStats.AnimalCount;

		// Token: 0x040086FD RID: 34557
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGameStat = "gamestat";
	}
}
