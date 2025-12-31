using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016CD RID: 5837
	[Preserve]
	public class ActionBlockReplace : ActionBaseBlockAction
	{
		// Token: 0x0600B125 RID: 45349 RVA: 0x004517E4 File Offset: 0x0044F9E4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool CheckValid(World world, Vector3i currentPos)
		{
			BlockValue block = world.GetBlock(currentPos + Vector3i.down);
			return !block.isair && !block.Block.IsTerrainDecoration;
		}

		// Token: 0x0600B126 RID: 45350 RVA: 0x00451820 File Offset: 0x0044FA20
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (this.blockTo == null)
			{
				return null;
			}
			if (this.emptyOnly && !blockValue.isair)
			{
				return null;
			}
			if (!blockValue.Block.blockMaterial.CanDestroy)
			{
				return null;
			}
			BlockValue blockValue2 = Block.GetBlockValue(this.blockTo[this.random.RandomRange(0, this.blockTo.Length)], false);
			if (blockValue.type != blockValue2.type)
			{
				return new BlockChangeInfo(0, currentPos, blockValue2, true);
			}
			return null;
		}

		// Token: 0x0600B127 RID: 45351 RVA: 0x0045189C File Offset: 0x0044FA9C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			string text = "";
			this.Properties.ParseString(ActionBlockReplace.PropBlockTo, ref text);
			if (text != "")
			{
				this.blockTo = text.Split(',', StringSplitOptions.None);
			}
			properties.ParseBool(ActionBlockReplace.PropEmptyOnly, ref this.emptyOnly);
		}

		// Token: 0x0600B128 RID: 45352 RVA: 0x004518F5 File Offset: 0x0044FAF5
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockReplace
			{
				blockTo = this.blockTo,
				emptyOnly = this.emptyOnly
			};
		}

		// Token: 0x04008AA9 RID: 35497
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] blockTo;

		// Token: 0x04008AAA RID: 35498
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool emptyOnly;

		// Token: 0x04008AAB RID: 35499
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBlockTo = "block_to";

		// Token: 0x04008AAC RID: 35500
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEmptyOnly = "empty_only";
	}
}
