using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016B3 RID: 5811
	[Preserve]
	public class ActionTeleport : ActionBaseTeleport
	{
		// Token: 0x0600B0A7 RID: 45223 RVA: 0x0044F730 File Offset: 0x0044D930
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			World world = GameManager.Instance.World;
			Vector3 vector = Vector3.zero;
			switch (this.offsetType)
			{
			case ActionTeleport.OffsetTypes.None:
				vector = this.target_position;
				break;
			case ActionTeleport.OffsetTypes.Relative:
				vector = target.position + target.transform.TransformDirection(this.target_position);
				break;
			case ActionTeleport.OffsetTypes.World:
				vector = target.position + this.target_position;
				break;
			}
			if (vector.y > 0f)
			{
				base.TeleportEntity(target, vector);
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600B0A8 RID: 45224 RVA: 0x0044F7BB File Offset: 0x0044D9BB
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseVec(ActionTeleport.PropTargetPosition, ref this.target_position);
			properties.ParseEnum<ActionTeleport.OffsetTypes>(ActionTeleport.PropOffsetType, ref this.offsetType);
		}

		// Token: 0x0600B0A9 RID: 45225 RVA: 0x0044F7E6 File Offset: 0x0044D9E6
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTeleport
			{
				targetGroup = this.targetGroup,
				target_position = this.target_position,
				offsetType = this.offsetType,
				teleportDelayText = this.teleportDelayText
			};
		}

		// Token: 0x04008A20 RID: 35360
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3 target_position;

		// Token: 0x04008A21 RID: 35361
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTeleport.OffsetTypes offsetType;

		// Token: 0x04008A22 RID: 35362
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetPosition = "target_position";

		// Token: 0x04008A23 RID: 35363
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOffsetType = "offset_type";

		// Token: 0x020016B4 RID: 5812
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum OffsetTypes
		{
			// Token: 0x04008A25 RID: 35365
			None,
			// Token: 0x04008A26 RID: 35366
			Relative,
			// Token: 0x04008A27 RID: 35367
			World
		}
	}
}
