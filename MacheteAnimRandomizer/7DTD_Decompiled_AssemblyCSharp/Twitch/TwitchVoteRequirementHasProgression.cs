using System;

namespace Twitch
{
	// Token: 0x02001595 RID: 5525
	public class TwitchVoteRequirementHasProgression : BaseTwitchVoteOperationRequirement
	{
		// Token: 0x0600A9FD RID: 43517 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600A9FE RID: 43518 RVA: 0x00432571 File Offset: 0x00430771
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(EntityPlayer player)
		{
			return (float)player.Progression.GetProgressionValue(this.SkillName).CalculatedLevel(player);
		}

		// Token: 0x0600A9FF RID: 43519 RVA: 0x0043258B File Offset: 0x0043078B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(EntityPlayer player)
		{
			return (float)this.Level;
		}

		// Token: 0x0600AA00 RID: 43520 RVA: 0x00432594 File Offset: 0x00430794
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool CheckPerk(EntityPlayer player, string buffName)
		{
			if (player.Progression.GetProgressionValue(buffName).CalculatedLevel(player) >= this.Level)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600AA01 RID: 43521 RVA: 0x004325C0 File Offset: 0x004307C0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(TwitchVoteRequirementHasProgression.PropSkillName, ref this.SkillName);
			properties.ParseInt(TwitchVoteRequirementHasProgression.PropLevel, ref this.Level);
		}

		// Token: 0x040084E0 RID: 34016
		[PublicizedFrom(EAccessModifier.Protected)]
		public string SkillName = "";

		// Token: 0x040084E1 RID: 34017
		[PublicizedFrom(EAccessModifier.Protected)]
		public int Level = 1;

		// Token: 0x040084E2 RID: 34018
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSkillName = "skill_name";

		// Token: 0x040084E3 RID: 34019
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropLevel = "level";
	}
}
