using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000527 RID: 1319
[Preserve]
public class ItemActionMakeFertile : ItemActionMelee
{
	// Token: 0x06002ABF RID: 10943 RVA: 0x00119020 File Offset: 0x00117220
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (!_props.Values.ContainsKey("Fertileblock"))
		{
			throw new Exception("Missing attribute 'fertileblock' in use_action 'MakeFertile'");
		}
		string text = _props.Values["Fertileblock"];
		this.fertileBlock = ItemClass.GetItem(text, false).ToBlockValue();
		if (this.fertileBlock.Equals(BlockValue.Air))
		{
			throw new Exception("Unknown block name '" + text + "' in use_action 'MakeFertile'!");
		}
		if (!_props.Values.ContainsKey("Adjacentblock"))
		{
			throw new Exception("Missing attribute 'adjacentblock' in use_action 'MakeFertile'");
		}
		text = _props.Values["Adjacentblock"];
		this.adjacentBlock = ItemClass.GetItem(text, false).ToBlockValue();
		if (this.adjacentBlock.Equals(BlockValue.Air))
		{
			throw new Exception("Unknown block name '" + text + "' in use_action 'MakeFertile'!");
		}
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x00119104 File Offset: 0x00117304
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void hitTheTarget(ItemActionMelee.InventoryDataMelee _actionData, WorldRayHitInfo hitInfo, float damageScale)
	{
		ItemInventoryData invData = _actionData.invData;
		if (!invData.hitInfo.bHitValid || !GameUtils.IsBlockOrTerrain(invData.hitInfo.tag))
		{
			return;
		}
		Vector3i blockPos = invData.hitInfo.hit.blockPos;
		if (invData.world.GetBlock(blockPos).Block.blockMaterial.FertileLevel < 2)
		{
			base.hitTheTarget(_actionData, hitInfo, damageScale);
			return;
		}
		Vector3 position = _actionData.invData.holdingEntity.GetPosition();
		float f = position.x - invData.hitInfo.hit.pos.x;
		float f2 = position.z - invData.hitInfo.hit.pos.z;
		Vector3i other = Vector3i.right;
		if (Mathf.Abs(f) > Mathf.Abs(f2))
		{
			other = Vector3i.forward;
		}
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		int clrIdx = invData.hitInfo.hit.clrIdx;
		BlockValue block = invData.world.GetBlock(clrIdx, blockPos + other);
		if (block.Block.blockMaterial.FertileLevel >= 2 && block.Block.shape.IsTerrain())
		{
			list.Add(new BlockChangeInfo(clrIdx, blockPos + other, this.adjacentBlock, MarchingCubes.DensityTerrain / 3));
		}
		BlockValue block2 = invData.world.GetBlock(clrIdx, blockPos - other);
		if (block2.Block.blockMaterial.FertileLevel >= 2 && block2.Block.shape.IsTerrain())
		{
			list.Add(new BlockChangeInfo(clrIdx, blockPos - other, this.adjacentBlock, MarchingCubes.DensityTerrain / 3));
		}
		list.Add(new BlockChangeInfo(clrIdx, blockPos, this.fertileBlock, MarchingCubes.DensityTerrain));
		if (_actionData.invData.holdingEntity is EntityPlayerLocal)
		{
			(_actionData.invData.holdingEntity as EntityPlayerLocal).Progression.AddLevelExp((int)this.fertileBlock.Block.blockMaterial.Experience, "_xpOther", Progression.XPTypes.Other, true, true);
		}
		invData.world.SetBlocksRPC(list);
		if (this.soundEnd != null)
		{
			_actionData.invData.holdingEntity.PlayOneShot(this.soundEnd, false, false, false, null);
		}
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x0011934C File Offset: 0x0011754C
	public override RenderCubeType GetFocusType(ItemActionData _actionData)
	{
		return RenderCubeType.FullBlockBothSides;
	}

	// Token: 0x04002143 RID: 8515
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue fertileBlock;

	// Token: 0x04002144 RID: 8516
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue adjacentBlock;
}
