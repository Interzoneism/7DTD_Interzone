using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class BlockToolTerrainAdjust : IBlockTool
{
	// Token: 0x06000C22 RID: 3106 RVA: 0x00052F76 File Offset: 0x00051176
	public BlockToolTerrainAdjust(BlockTools.Brush _brush, NguiWdwTerrainEditor _parentWindow)
	{
		this.brush = _brush;
		this.parentWindow = _parentWindow;
		this.isButtonDown = false;
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x00002914 File Offset: 0x00000B14
	public void CheckSpecialKeys(Event ev, PlayerActionsLocal playerActions)
	{
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x00052FA0 File Offset: 0x000511A0
	public void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo, PlayerActionsLocal playerActions)
	{
		if (_data == null || _data.actionData == null || _data.actionData[0] == null)
		{
			return;
		}
		this.parentWindow.lastPosition = new Vector3i(_hitInfo.hit.pos);
		this.parentWindow.lastDirection = _hitInfo.ray.direction;
		if (Input.GetMouseButton(0) && this.isButtonDown)
		{
			if (Time.time - _data.actionData[0].lastUseTime < 0.1f)
			{
				return;
			}
			_data.actionData[0].lastUseTime = Time.time;
			if (!_data.hitInfo.bHitValid || !GameUtils.IsBlockOrTerrain(_data.hitInfo.tag))
			{
				return;
			}
			this.blockChanges.Clear();
			BlockTools.PlaceTerrain(_data.world, _data.hitInfo.hit.clrIdx, _data.hitInfo.hit.blockPos + new Vector3i(this.parentWindow.lastDirection.x * 2f, this.parentWindow.lastDirection.y * 2f, this.parentWindow.lastDirection.z * 2f), this.brush, this.blockChanges);
			_data.world.SetBlocksRPC(this.blockChanges);
			return;
		}
		else
		{
			if (!Input.GetMouseButton(1) || !this.isButtonDown)
			{
				if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && this.isButtonDown)
				{
					this.isButtonDown = false;
				}
				return;
			}
			if (Time.time - _data.actionData[0].lastUseTime < 0.1f)
			{
				return;
			}
			_data.actionData[0].lastUseTime = Time.time;
			if (!_data.hitInfo.bHitValid || !GameUtils.IsBlockOrTerrain(_data.hitInfo.tag))
			{
				return;
			}
			this.blockChanges.Clear();
			BlockTools.RemoveTerrain(_data.world, _data.hitInfo.hit.clrIdx, _data.hitInfo.hit.blockPos, this.brush, this.blockChanges);
			_data.world.SetBlocksRPC(this.blockChanges);
			return;
		}
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		return false;
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0002B133 File Offset: 0x00029333
	public string GetDebugOutput()
	{
		return "";
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x000531E1 File Offset: 0x000513E1
	public virtual bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x000531E1 File Offset: 0x000513E1
	public bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x000531F4 File Offset: 0x000513F4
	[PublicizedFrom(EAccessModifier.Private)]
	public sbyte[] GetLocalDensityMap(WorldBase _world, Vector3i blockTargetPos, Vector3i[] localArea)
	{
		sbyte[] array = new sbyte[localArea.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _world.GetDensity(0, localArea[i].x, localArea[i].y, localArea[i].z);
		}
		return array;
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00053247 File Offset: 0x00051447
	public override string ToString()
	{
		return "Dig/Place Terrain";
	}

	// Token: 0x04000A3D RID: 2621
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTools.Brush brush;

	// Token: 0x04000A3E RID: 2622
	[PublicizedFrom(EAccessModifier.Private)]
	public NguiWdwTerrainEditor parentWindow;

	// Token: 0x04000A3F RID: 2623
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData lastData;

	// Token: 0x04000A40 RID: 2624
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isButtonDown;

	// Token: 0x04000A41 RID: 2625
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockChangeInfo> blockChanges = new List<BlockChangeInfo>();
}
