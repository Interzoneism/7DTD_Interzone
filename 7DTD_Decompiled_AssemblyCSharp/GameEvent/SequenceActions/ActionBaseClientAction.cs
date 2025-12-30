using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001650 RID: 5712
	[Preserve]
	public class ActionBaseClientAction : ActionBaseTargetAction
	{
		// Token: 0x0600AEE9 RID: 44777 RVA: 0x00444784 File Offset: 0x00442984
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				this.OnServerPerform(entityPlayer);
				if (entityPlayer is EntityPlayerLocal)
				{
					this.OnClientPerform(entityPlayer);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(base.Owner.Name, entityPlayer.entityId, base.Owner.ExtraData, base.Owner.Tag, NetPackageGameEventResponse.ResponseTypes.ClientSequenceAction, -1, this.ActionIndex, false), false, entityPlayer.entityId, -1, -1, null, 192, false);
				}
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AEEA RID: 44778 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnServerPerform(Entity target)
		{
		}
	}
}
