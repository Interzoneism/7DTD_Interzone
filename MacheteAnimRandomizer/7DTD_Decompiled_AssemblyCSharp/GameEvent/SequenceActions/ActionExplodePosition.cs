using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001667 RID: 5735
	[Preserve]
	public class ActionExplodePosition : BaseAction
	{
		// Token: 0x0600AF53 RID: 44883 RVA: 0x004478EC File Offset: 0x00445AEC
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (base.Owner.TargetPosition.y == 0f)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			Vector3 targetPosition = base.Owner.TargetPosition;
			EntityAlive alive = base.Owner.Target as EntityAlive;
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
			GameManager.Instance.ExplosionServer(0, targetPosition, World.worldToBlockPos(targetPosition), Quaternion.identity, explosionData, -1, 0.1f, false, null);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF54 RID: 44884 RVA: 0x004479F4 File Offset: 0x00445BF4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionExplodePosition.PropBlastPower, ref this.blastPowerText);
			properties.ParseString(ActionExplodePosition.PropBlockDamage, ref this.blockDamageText);
			properties.ParseString(ActionExplodePosition.PropBlockRadius, ref this.blockRadiusText);
			properties.ParseString(ActionExplodePosition.PropEntityDamage, ref this.entityDamageText);
			properties.ParseString(ActionExplodePosition.PropEntityRadius, ref this.entityRadiusText);
			properties.ParseString(ActionExplodePosition.PropBlockTags, ref this.blockTags);
			properties.ParseInt(ActionExplodePosition.PropParticleIndex, ref this.particleIndex);
			properties.ParseBool(ActionExplodePosition.PropIgnoreHeatMap, ref this.ignoreHeatMap);
		}

		// Token: 0x0600AF55 RID: 44885 RVA: 0x00447A90 File Offset: 0x00445C90
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionExplodePosition
			{
				blastPowerText = this.blastPowerText,
				blockDamageText = this.blockDamageText,
				blockRadiusText = this.blockRadiusText,
				entityDamageText = this.entityDamageText,
				entityRadiusText = this.entityRadiusText,
				particleIndex = this.particleIndex,
				blockTags = this.blockTags
			};
		}

		// Token: 0x04008875 RID: 34933
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blastPowerText;

		// Token: 0x04008876 RID: 34934
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockDamageText;

		// Token: 0x04008877 RID: 34935
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockRadiusText;

		// Token: 0x04008878 RID: 34936
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityDamageText;

		// Token: 0x04008879 RID: 34937
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityRadiusText;

		// Token: 0x0400887A RID: 34938
		[PublicizedFrom(EAccessModifier.Protected)]
		public int particleIndex = 13;

		// Token: 0x0400887B RID: 34939
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool ignoreHeatMap = true;

		// Token: 0x0400887C RID: 34940
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockTags = "";

		// Token: 0x0400887D RID: 34941
		public static string PropBlastPower = "blast_power";

		// Token: 0x0400887E RID: 34942
		public static string PropBlockDamage = "block_damage";

		// Token: 0x0400887F RID: 34943
		public static string PropBlockRadius = "block_radius";

		// Token: 0x04008880 RID: 34944
		public static string PropBlockTags = "block_tags";

		// Token: 0x04008881 RID: 34945
		public static string PropEntityDamage = "entity_damage";

		// Token: 0x04008882 RID: 34946
		public static string PropEntityRadius = "entity_radius";

		// Token: 0x04008883 RID: 34947
		public static string PropParticleIndex = "particle_index";

		// Token: 0x04008884 RID: 34948
		public static string PropIgnoreHeatMap = "ignore_heatmap";
	}
}
