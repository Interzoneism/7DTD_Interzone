using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015ED RID: 5613
	[Preserve]
	public class ChallengeObjectiveEnterBiome : BaseChallengeObjective
	{
		// Token: 0x17001354 RID: 4948
		// (get) Token: 0x0600AC84 RID: 44164 RVA: 0x00076E19 File Offset: 0x00075019
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.EnterBiome;
			}
		}

		// Token: 0x17001355 RID: 4949
		// (get) Token: 0x0600AC85 RID: 44165 RVA: 0x0043ADF1 File Offset: 0x00438FF1
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveEnter", false) + " " + Localization.Get("biome_" + this.biome, false) + ":";
			}
		}

		// Token: 0x0600AC86 RID: 44166 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AC87 RID: 44167 RVA: 0x0043AE23 File Offset: 0x00439023
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.BiomeEnter += this.Current_BiomeEnter;
		}

		// Token: 0x0600AC88 RID: 44168 RVA: 0x0043AE3B File Offset: 0x0043903B
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BiomeEnter -= this.Current_BiomeEnter;
		}

		// Token: 0x0600AC89 RID: 44169 RVA: 0x0043AE54 File Offset: 0x00439054
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BiomeEnter(BiomeDefinition biomeDef)
		{
			if (biomeDef != null && biomeDef.m_sBiomeName == this.biome)
			{
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600AC8A RID: 44170 RVA: 0x0043AEA9 File Offset: 0x004390A9
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("biome"))
			{
				this.biome = e.GetAttribute("biome");
			}
		}

		// Token: 0x0600AC8B RID: 44171 RVA: 0x0043AEDA File Offset: 0x004390DA
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveEnterBiome
			{
				biome = this.biome
			};
		}

		// Token: 0x04008666 RID: 34406
		[PublicizedFrom(EAccessModifier.Private)]
		public string biome;
	}
}
