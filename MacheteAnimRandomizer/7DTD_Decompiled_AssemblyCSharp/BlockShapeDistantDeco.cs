using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200017A RID: 378
[Preserve]
public class BlockShapeDistantDeco : BlockShapeModelEntity
{
	// Token: 0x06000B2E RID: 2862 RVA: 0x0004937A File Offset: 0x0004757A
	public override void Init(Block _block)
	{
		base.Init(_block);
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00049383 File Offset: 0x00047583
	public override void OnBlockValueChanged(WorldBase _world, Vector3i _blockPos, int _clrIdx, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockValueChanged(_world, _blockPos, _clrIdx, _oldBlockValue, _newBlockValue);
			return;
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0004939F File Offset: 0x0004759F
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
			return;
		}
		if (!_blockValue.Block.IsDistantDecoration)
		{
			return;
		}
		DecoManager.Instance.AddDecorationAt((World)_world, _blockValue, _blockPos, true);
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x000493DC File Offset: 0x000475DC
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		}
		DecoManager.Instance.RemoveDecorationAt(_blockPos);
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x00049404 File Offset: 0x00047604
	public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
			return;
		}
		if (_world.IsRemote())
		{
			if (!_blockValue.Block.IsDistantDecoration)
			{
				return;
			}
			DecoManager.Instance.AddDecorationAt((World)_world, _blockValue, _blockPos, false);
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x00049454 File Offset: 0x00047654
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
			return;
		}
		_ebcd.transform.tag = "T_Deco";
		Collider[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject gameObject = componentsInChildren[i].gameObject;
			gameObject.tag = "T_Deco";
			gameObject.layer = 16;
		}
	}
}
