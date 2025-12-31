using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001671 RID: 5745
	[Preserve]
	public class ActionGetLandClaimPosition : BaseAction
	{
		// Token: 0x0600AF77 RID: 44919 RVA: 0x00449C84 File Offset: 0x00447E84
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			World world = GameManager.Instance.World;
			List<Vector3i> lpblocks = world.GetGameManager().GetPersistentPlayerList().GetPlayerDataFromEntityID(base.Owner.Target.entityId).LPBlocks;
			if (lpblocks == null || lpblocks.Count == 0)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			int num = (GameStats.GetInt(EnumGameStats.LandClaimSize) - 1) / 2;
			int num2 = num * num;
			bool flag = false;
			for (int i = 0; i < lpblocks.Count; i++)
			{
				Vector3i vector3i = lpblocks[i];
				if (BlockLandClaim.IsPrimary(world.GetBlock(vector3i)) && base.Owner.Target.GetDistanceSq(vector3i) < (float)num2)
				{
					flag = true;
					base.Owner.TargetPosition = vector3i;
				}
			}
			if (!flag)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF78 RID: 44920 RVA: 0x00449D42 File Offset: 0x00447F42
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionGetLandClaimPosition();
		}
	}
}
