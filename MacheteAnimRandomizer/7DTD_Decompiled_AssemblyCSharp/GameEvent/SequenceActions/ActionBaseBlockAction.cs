using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016C3 RID: 5827
	[Preserve]
	public class ActionBaseBlockAction : BaseAction
	{
		// Token: 0x0600B0F1 RID: 45297 RVA: 0x0000FB42 File Offset: 0x0000DD42
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool NeedsDamage()
		{
			return false;
		}

		// Token: 0x0600B0F2 RID: 45298 RVA: 0x004509B4 File Offset: 0x0044EBB4
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			List<BlockChangeInfo> list = new List<BlockChangeInfo>();
			this.startPoint = ((base.Owner.TargetPosition.y != 0f) ? base.Owner.TargetPosition : base.Owner.Target.position);
			World world = GameManager.Instance.World;
			this.random = world.GetGameRandom();
			FastTags<TagGroup.Global> other = (this.blockTags != null) ? FastTags<TagGroup.Global>.Parse(this.blockTags) : FastTags<TagGroup.Global>.none;
			FastTags<TagGroup.Global> other2 = (this.excludeTags != null) ? FastTags<TagGroup.Global>.Parse(this.excludeTags) : FastTags<TagGroup.Global>.none;
			IChunk chunk = null;
			if (base.Owner.Target != null && !base.Owner.Target.onGround)
			{
				return BaseAction.ActionCompleteStates.InComplete;
			}
			for (int i = this.minOffset.y; i <= this.maxOffset.y; i++)
			{
				for (int j = this.minOffset.z; j <= this.maxOffset.z; j += this.spacing + 1)
				{
					int num = (int)Utils.FastAbs((float)j);
					for (int k = this.minOffset.x; k <= this.maxOffset.x; k += this.spacing + 1)
					{
						if ((this.innerOffset == -1 || Utils.FastAbs((float)k) > (float)this.innerOffset || num > this.innerOffset) && (this.randomChance <= 0f || this.random.RandomFloat <= this.randomChance))
						{
							Vector3i vector3i = new Vector3i(Utils.Fastfloor(this.startPoint.x + (float)k), Utils.Fastfloor(this.startPoint.y + (float)i), Utils.Fastfloor(this.startPoint.z + (float)j));
							if (vector3i.y >= 0 && world.GetChunkFromWorldPos(vector3i, ref chunk) && world.GetTraderAreaAt(vector3i) == null && (!this.checkSafe || (!this.safeAllowed && world.CanPlaceBlockAt(vector3i, null, false))))
							{
								int x = World.toBlockXZ(vector3i.x);
								int z = World.toBlockXZ(vector3i.z);
								BlockValue blockValue = this.NeedsDamage() ? chunk.GetBlock(x, vector3i.y, z) : chunk.GetBlockNoDamage(x, vector3i.y, z);
								if (!blockValue.ischild && (this.allowTerrain || !blockValue.Block.shape.IsTerrain()) && (this.blockTags == null || blockValue.Block.Tags.Test_AnySet(other)) && (this.excludeTags == null || !blockValue.Block.Tags.Test_AnySet(other2)) && this.CheckValid(world, vector3i))
								{
									BlockChangeInfo blockChangeInfo = this.UpdateBlock(world, vector3i, blockValue);
									if (blockChangeInfo != null)
									{
										list.Add(blockChangeInfo);
									}
								}
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				if (this.maxCount != -1 && this.maxCount < list.Count)
				{
					int num2 = list.Count - this.maxCount;
					for (int l = 0; l < num2; l++)
					{
						list.RemoveAt(this.random.RandomRange(list.Count));
					}
				}
				this.ChangesComplete();
				this.ProcessChanges(world, list);
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InCompleteRefund;
		}

		// Token: 0x0600B0F3 RID: 45299 RVA: 0x00450D23 File Offset: 0x0044EF23
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void ProcessChanges(World world, List<BlockChangeInfo> blockChanges)
		{
			world.SetBlocksRPC(blockChanges);
		}

		// Token: 0x0600B0F4 RID: 45300 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void ChangesComplete()
		{
		}

		// Token: 0x0600B0F5 RID: 45301 RVA: 0x000197A5 File Offset: 0x000179A5
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool CheckValid(World world, Vector3i currentPos)
		{
			return true;
		}

		// Token: 0x0600B0F6 RID: 45302 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			return null;
		}

		// Token: 0x0600B0F7 RID: 45303 RVA: 0x00450D2C File Offset: 0x0044EF2C
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator UpdateBlocks(List<BlockChangeInfo> blockChanges)
		{
			yield return new WaitForSeconds(0.5f);
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
			yield break;
		}

		// Token: 0x0600B0F8 RID: 45304 RVA: 0x00450D3C File Offset: 0x0044EF3C
		public override BaseAction Clone()
		{
			ActionBaseBlockAction actionBaseBlockAction = base.Clone() as ActionBaseBlockAction;
			actionBaseBlockAction.minOffset = this.minOffset;
			actionBaseBlockAction.maxOffset = this.maxOffset;
			actionBaseBlockAction.spacing = this.spacing;
			actionBaseBlockAction.randomChance = this.randomChance;
			actionBaseBlockAction.safeAllowed = this.safeAllowed;
			actionBaseBlockAction.checkSafe = this.checkSafe;
			actionBaseBlockAction.blockTags = this.blockTags;
			actionBaseBlockAction.excludeTags = this.excludeTags;
			actionBaseBlockAction.innerOffset = this.innerOffset;
			actionBaseBlockAction.allowTerrain = this.allowTerrain;
			actionBaseBlockAction.maxCount = this.maxCount;
			return actionBaseBlockAction;
		}

		// Token: 0x0600B0F9 RID: 45305 RVA: 0x00450DD8 File Offset: 0x0044EFD8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseVec(ActionBaseBlockAction.PropMinOffset, ref this.minOffset);
			properties.ParseVec(ActionBaseBlockAction.PropMaxOffset, ref this.maxOffset);
			properties.ParseInt(ActionBaseBlockAction.PropSpacing, ref this.spacing);
			properties.ParseInt(ActionBaseBlockAction.PropInnerOffset, ref this.innerOffset);
			properties.ParseFloat(ActionBaseBlockAction.PropRandomChance, ref this.randomChance);
			if (properties.Contains(ActionBaseBlockAction.PropSafeAllowed))
			{
				properties.ParseBool(ActionBaseBlockAction.PropSafeAllowed, ref this.safeAllowed);
				this.checkSafe = true;
			}
			properties.ParseString(ActionBaseBlockAction.PropBlockTags, ref this.blockTags);
			properties.ParseString(ActionBaseBlockAction.PropExcludeTags, ref this.excludeTags);
			properties.ParseBool(ActionBaseBlockAction.PropAllowTerrain, ref this.allowTerrain);
			properties.ParseInt(ActionBaseBlockAction.PropMaxCount, ref this.maxCount);
		}

		// Token: 0x04008A6F RID: 35439
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3i minOffset = Vector3i.zero;

		// Token: 0x04008A70 RID: 35440
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3i maxOffset = Vector3i.zero;

		// Token: 0x04008A71 RID: 35441
		[PublicizedFrom(EAccessModifier.Protected)]
		public string blockTags;

		// Token: 0x04008A72 RID: 35442
		[PublicizedFrom(EAccessModifier.Protected)]
		public string excludeTags;

		// Token: 0x04008A73 RID: 35443
		[PublicizedFrom(EAccessModifier.Protected)]
		public int spacing;

		// Token: 0x04008A74 RID: 35444
		[PublicizedFrom(EAccessModifier.Protected)]
		public int innerOffset = -1;

		// Token: 0x04008A75 RID: 35445
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool safeAllowed;

		// Token: 0x04008A76 RID: 35446
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool checkSafe;

		// Token: 0x04008A77 RID: 35447
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool allowTerrain;

		// Token: 0x04008A78 RID: 35448
		[PublicizedFrom(EAccessModifier.Protected)]
		public float randomChance = -1f;

		// Token: 0x04008A79 RID: 35449
		[PublicizedFrom(EAccessModifier.Protected)]
		public int maxCount = -1;

		// Token: 0x04008A7A RID: 35450
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBlockTags = "block_tags";

		// Token: 0x04008A7B RID: 35451
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTags = "exclude_tags";

		// Token: 0x04008A7C RID: 35452
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinOffset = "min_offset";

		// Token: 0x04008A7D RID: 35453
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxOffset = "max_offset";

		// Token: 0x04008A7E RID: 35454
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSpacing = "spacing";

		// Token: 0x04008A7F RID: 35455
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRandomChance = "random_chance";

		// Token: 0x04008A80 RID: 35456
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSafeAllowed = "safe_allowed";

		// Token: 0x04008A81 RID: 35457
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropInnerOffset = "inner_offset";

		// Token: 0x04008A82 RID: 35458
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAllowTerrain = "allow_terrain";

		// Token: 0x04008A83 RID: 35459
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxCount = "max_count";

		// Token: 0x04008A84 RID: 35460
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameRandom random;

		// Token: 0x04008A85 RID: 35461
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3 startPoint = Vector3.zero;
	}
}
