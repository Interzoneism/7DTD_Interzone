using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D0 RID: 5840
	[Preserve]
	public class ActionBlockUpgrade : ActionBaseBlockAction
	{
		// Token: 0x0600B135 RID: 45365 RVA: 0x00451C30 File Offset: 0x0044FE30
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				BlockValue blockValue2 = blockValue.Block.UpgradeBlock;
				blockValue2 = BlockPlaceholderMap.Instance.Replace(blockValue2, GameEventManager.Current.Random, currentPos.x, currentPos.z, false);
				blockValue2.rotation = blockValue.rotation;
				blockValue2.meta = blockValue.meta;
				if (!blockValue2.isair)
				{
					return new BlockChangeInfo(0, currentPos, blockValue2);
				}
			}
			return null;
		}

		// Token: 0x0600B136 RID: 45366 RVA: 0x00451CA5 File Offset: 0x0044FEA5
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockUpgrade();
		}
	}
}
