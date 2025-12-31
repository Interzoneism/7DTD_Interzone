using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001668 RID: 5736
	[Preserve]
	public class ActionExplodeTarget : ActionBaseTargetAction
	{
		// Token: 0x0600AF58 RID: 44888 RVA: 0x00447B78 File Offset: 0x00445D78
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			EntityAlive alive = base.Owner.Target as EntityAlive;
			if (entityAlive != null)
			{
				ExplosionData explosionData = new ExplosionData
				{
					BlastPower = GameEventManager.GetIntValue(alive, this.blastPowerText, 75),
					BlockDamage = GameEventManager.GetFloatValue(alive, this.blockDamageText, 1f),
					BlockRadius = GameEventManager.GetFloatValue(alive, this.blockRadiusText, 4f),
					BlockTags = this.blockTags,
					EntityDamage = GameEventManager.GetFloatValue(alive, this.entityDamageText, 5000f),
					EntityRadius = GameEventManager.GetIntValue(alive, this.entityRadiusText, 3),
					ParticleIndex = this.particleIndex,
					IgnoreHeatMap = this.ignoreHeatMap
				};
				GameManager.Instance.ExplosionServer(0, entityAlive.position, entityAlive.GetBlockPosition(), entityAlive.qrotation, explosionData, -1, 0.1f, false, null);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF59 RID: 44889 RVA: 0x00447C74 File Offset: 0x00445E74
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionExplodeTarget.PropBlastPower, ref this.blastPowerText);
			properties.ParseString(ActionExplodeTarget.PropBlockDamage, ref this.blockDamageText);
			properties.ParseString(ActionExplodeTarget.PropBlockRadius, ref this.blockRadiusText);
			properties.ParseString(ActionExplodeTarget.PropEntityDamage, ref this.entityDamageText);
			properties.ParseString(ActionExplodeTarget.PropEntityRadius, ref this.entityRadiusText);
			properties.ParseString(ActionExplodeTarget.PropBlockTags, ref this.blockTags);
			properties.ParseInt(ActionExplodeTarget.PropParticleIndex, ref this.particleIndex);
			properties.ParseBool(ActionExplodeTarget.PropIgnoreHeatMap, ref this.ignoreHeatMap);
		}

		// Token: 0x0600AF5A RID: 44890 RVA: 0x00447D10 File Offset: 0x00445F10
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionExplodeTarget
			{
				targetGroup = this.targetGroup,
				blastPowerText = this.blastPowerText,
				blockDamageText = this.blockDamageText,
				blockRadiusText = this.blockRadiusText,
				entityDamageText = this.entityDamageText,
				entityRadiusText = this.entityRadiusText,
				particleIndex = this.particleIndex,
				blockTags = this.blockTags
			};
		}

		// Token: 0x04008885 RID: 34949
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blastPowerText;

		// Token: 0x04008886 RID: 34950
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockDamageText;

		// Token: 0x04008887 RID: 34951
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockRadiusText;

		// Token: 0x04008888 RID: 34952
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityDamageText;

		// Token: 0x04008889 RID: 34953
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityRadiusText;

		// Token: 0x0400888A RID: 34954
		[PublicizedFrom(EAccessModifier.Protected)]
		public int particleIndex = 13;

		// Token: 0x0400888B RID: 34955
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool ignoreHeatMap = true;

		// Token: 0x0400888C RID: 34956
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockTags = "";

		// Token: 0x0400888D RID: 34957
		public static string PropBlastPower = "blast_power";

		// Token: 0x0400888E RID: 34958
		public static string PropBlockDamage = "block_damage";

		// Token: 0x0400888F RID: 34959
		public static string PropBlockRadius = "block_radius";

		// Token: 0x04008890 RID: 34960
		public static string PropBlockTags = "block_tags";

		// Token: 0x04008891 RID: 34961
		public static string PropEntityDamage = "entity_damage";

		// Token: 0x04008892 RID: 34962
		public static string PropEntityRadius = "entity_radius";

		// Token: 0x04008893 RID: 34963
		public static string PropParticleIndex = "particle_index";

		// Token: 0x04008894 RID: 34964
		public static string PropIgnoreHeatMap = "ignore_heatmap";
	}
}
