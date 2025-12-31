using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200017B RID: 379
[Preserve]
public class BlockShapeDistantDecoTree : BlockShapeDistantDeco
{
	// Token: 0x06000B34 RID: 2868 RVA: 0x000494BF File Offset: 0x000476BF
	public BlockShapeDistantDecoTree()
	{
		this.Has45DegreeRotations = true;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x000494D0 File Offset: 0x000476D0
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
			gameObject.layer = 23;
		}
		Transform transform = _ebcd.transform.Find("rootBall");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00049561 File Offset: 0x00047761
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!DecoManager.Instance.IsEnabled)
		{
			base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
			return;
		}
		DecoManager.Instance.RemoveDecorationAt(_blockPos);
	}
}
