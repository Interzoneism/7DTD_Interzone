using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001620 RID: 5664
	[Preserve]
	public class RequirementInBiome : BaseRequirement
	{
		// Token: 0x0600AE0E RID: 44558 RVA: 0x00440710 File Offset: 0x0043E910
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return false;
			}
			bool flag = this.biomeList.ContainsCaseInsensitive(entityAlive.biomeStandingOn.m_sBiomeName);
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600AE0F RID: 44559 RVA: 0x0044074E File Offset: 0x0043E94E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementInBiome.PropBiome, ref this.biomes);
			if (!string.IsNullOrEmpty(this.biomes))
			{
				this.biomeList = this.biomes.Split(',', StringSplitOptions.None);
			}
		}

		// Token: 0x0600AE10 RID: 44560 RVA: 0x00440789 File Offset: 0x0043E989
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInBiome
			{
				Invert = this.Invert,
				biomeList = this.biomeList
			};
		}

		// Token: 0x04008715 RID: 34581
		[PublicizedFrom(EAccessModifier.Private)]
		public string biomes;

		// Token: 0x04008716 RID: 34582
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] biomeList;

		// Token: 0x04008717 RID: 34583
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBiome = "biomes";
	}
}
