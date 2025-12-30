using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001665 RID: 5733
	[Preserve]
	public class ActionEjectFromVehicle : ActionBaseTargetAction
	{
		// Token: 0x0600AF4D RID: 44877 RVA: 0x004477B8 File Offset: 0x004459B8
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null && entityPlayer.AttachedToEntity != null)
			{
				if (entityPlayer.isEntityRemote)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityPlayer.entityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
				}
				else
				{
					(entityPlayer as EntityPlayerLocal).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
				}
				entityPlayer.SendDetach();
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF4E RID: 44878 RVA: 0x0044783E File Offset: 0x00445A3E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionEjectFromVehicle
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
