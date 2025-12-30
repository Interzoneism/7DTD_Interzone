using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016B5 RID: 5813
	[Preserve]
	public class ActionTeleportNearby : ActionBaseTeleport
	{
		// Token: 0x0600B0AC RID: 45228 RVA: 0x0044F83C File Offset: 0x0044DA3C
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			World world = GameManager.Instance.World;
			if (base.Owner.TargetPosition != Vector3.zero)
			{
				Vector3 targetPosition = base.Owner.TargetPosition;
				base.TeleportEntity(target, targetPosition);
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600B0AD RID: 45229 RVA: 0x0044F882 File Offset: 0x0044DA82
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTeleportNearby
			{
				targetGroup = this.targetGroup,
				teleportDelayText = this.teleportDelayText
			};
		}
	}
}
