using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016CF RID: 5839
	[Preserve]
	public class ActionBlockTriggerFall : ActionBaseBlockAction
	{
		// Token: 0x0600B131 RID: 45361 RVA: 0x00451BDF File Offset: 0x0044FDDF
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				return new BlockChangeInfo(0, currentPos, blockValue, true);
			}
			return null;
		}

		// Token: 0x0600B132 RID: 45362 RVA: 0x00451BF8 File Offset: 0x0044FDF8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void ProcessChanges(World world, List<BlockChangeInfo> blockChanges)
		{
			for (int i = 0; i < blockChanges.Count; i++)
			{
				world.AddFallingBlock(blockChanges[i].pos, false);
			}
		}

		// Token: 0x0600B133 RID: 45363 RVA: 0x00451C29 File Offset: 0x0044FE29
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockTriggerFall();
		}
	}
}
