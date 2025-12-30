using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D1 RID: 5841
	[Preserve]
	public class ActionRemoveSpawnedBlocks : BaseAction
	{
		// Token: 0x0600B138 RID: 45368 RVA: 0x00451CAC File Offset: 0x0044FEAC
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			for (int i = 0; i < GameEventManager.Current.blockEntries.Count; i++)
			{
				GameEventManager.SpawnedBlocksEntry spawnedBlocksEntry = GameEventManager.Current.blockEntries[i];
				if (!this.targetOnly || spawnedBlocksEntry.Target == base.Owner.Target)
				{
					spawnedBlocksEntry.TimeAlive = 1f;
					spawnedBlocksEntry.IsDespawn = this.despawn;
				}
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B139 RID: 45369 RVA: 0x00451D1C File Offset: 0x0044FF1C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionRemoveSpawnedBlocks.PropTargetOnly))
			{
				this.targetOnly = StringParsers.ParseBool(properties.Values[ActionRemoveSpawnedBlocks.PropTargetOnly], 0, -1, true);
			}
			properties.ParseBool(ActionRemoveSpawnedBlocks.PropDespawn, ref this.despawn);
		}

		// Token: 0x0600B13A RID: 45370 RVA: 0x00451D71 File Offset: 0x0044FF71
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRemoveSpawnedBlocks
			{
				targetOnly = this.targetOnly,
				despawn = this.despawn
			};
		}

		// Token: 0x04008AB4 RID: 35508
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool targetOnly;

		// Token: 0x04008AB5 RID: 35509
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool despawn;

		// Token: 0x04008AB6 RID: 35510
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetOnly = "target_only";

		// Token: 0x04008AB7 RID: 35511
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropDespawn = "despawn";
	}
}
