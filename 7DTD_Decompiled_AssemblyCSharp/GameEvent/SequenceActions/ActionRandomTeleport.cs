using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001687 RID: 5767
	[Preserve]
	public class ActionRandomTeleport : ActionBaseTeleport
	{
		// Token: 0x0600AFCB RID: 45003 RVA: 0x0044B5CC File Offset: 0x004497CC
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (!(entityPlayer != null))
			{
				return BaseAction.ActionCompleteStates.Complete;
			}
			float distance = GameManager.Instance.World.RandomRange(this.minDistance, this.maxDistance);
			this.position = ObjectiveRandomGoto.CalculateRandomPoint(entityPlayer.entityId, distance, "", true, BiomeFilterTypes.SameBiome, "");
			if (this.position.y >= 0)
			{
				Vector3 vector = this.position.ToVector3();
				vector.y = -2000f;
				base.TeleportEntity(entityPlayer, vector);
				return BaseAction.ActionCompleteStates.Complete;
			}
			this.maxTries--;
			if (this.maxTries != 0)
			{
				return BaseAction.ActionCompleteStates.InComplete;
			}
			return BaseAction.ActionCompleteStates.InCompleteRefund;
		}

		// Token: 0x0600AFCC RID: 45004 RVA: 0x0044B671 File Offset: 0x00449871
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseFloat(ActionRandomTeleport.PropMinDistance, ref this.minDistance);
			properties.ParseFloat(ActionRandomTeleport.PropMaxDistance, ref this.maxDistance);
			properties.ParseInt(ActionRandomTeleport.PropMaxTries, ref this.maxTries);
		}

		// Token: 0x0600AFCD RID: 45005 RVA: 0x0044B6B0 File Offset: 0x004498B0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRandomTeleport
			{
				targetGroup = this.targetGroup,
				minDistance = this.minDistance,
				maxDistance = this.maxDistance,
				maxTries = this.maxTries,
				teleportDelayText = this.teleportDelayText
			};
		}

		// Token: 0x04008956 RID: 35158
		[PublicizedFrom(EAccessModifier.Protected)]
		public float minDistance = 100f;

		// Token: 0x04008957 RID: 35159
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxDistance = 200f;

		// Token: 0x04008958 RID: 35160
		[PublicizedFrom(EAccessModifier.Protected)]
		public int maxTries = 20;

		// Token: 0x04008959 RID: 35161
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinDistance = "min_distance";

		// Token: 0x0400895A RID: 35162
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxDistance = "max_distance";

		// Token: 0x0400895B RID: 35163
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxTries = "max_tries";

		// Token: 0x0400895C RID: 35164
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i position;
	}
}
