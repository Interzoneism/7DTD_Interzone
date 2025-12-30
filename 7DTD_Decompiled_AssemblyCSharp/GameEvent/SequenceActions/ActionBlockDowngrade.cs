using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016C8 RID: 5832
	[Preserve]
	public class ActionBlockDowngrade : ActionBaseBlockAction
	{
		// Token: 0x0600B114 RID: 45332 RVA: 0x00451450 File Offset: 0x0044F650
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				BlockValue blockValue2 = blockValue.Block.DowngradeBlock;
				blockValue2 = BlockPlaceholderMap.Instance.Replace(blockValue2, GameManager.Instance.World.GetGameRandom(), currentPos.x, currentPos.z, false);
				blockValue2.rotation = blockValue.rotation;
				blockValue2.meta = blockValue.meta;
				if (!blockValue2.isair)
				{
					world.AddPendingDowngradeBlock(currentPos);
					return new BlockChangeInfo(0, currentPos, blockValue2);
				}
			}
			return null;
		}

		// Token: 0x0600B115 RID: 45333 RVA: 0x004514D1 File Offset: 0x0044F6D1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockDowngrade();
		}
	}
}
