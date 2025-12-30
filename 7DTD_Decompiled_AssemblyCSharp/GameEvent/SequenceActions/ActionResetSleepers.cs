using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200169A RID: 5786
	[Preserve]
	public class ActionResetSleepers : BaseAction
	{
		// Token: 0x0600B031 RID: 45105 RVA: 0x0044D6A4 File Offset: 0x0044B8A4
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			World world = GameManager.Instance.World;
			int sleeperVolumeCount = world.GetSleeperVolumeCount();
			for (int i = 0; i < sleeperVolumeCount; i++)
			{
				SleeperVolume sleeperVolume = world.GetSleeperVolume(i);
				if (sleeperVolume != null)
				{
					sleeperVolume.DespawnAndReset(world);
				}
			}
			Log.Out("Reset {0} sleeper volumes", new object[]
			{
				sleeperVolumeCount
			});
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B032 RID: 45106 RVA: 0x0044D6FC File Offset: 0x0044B8FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionResetSleepers();
		}
	}
}
