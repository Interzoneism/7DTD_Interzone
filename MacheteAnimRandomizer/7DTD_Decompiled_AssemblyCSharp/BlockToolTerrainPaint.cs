using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class BlockToolTerrainPaint : IBlockTool
{
	// Token: 0x06000C2B RID: 3115 RVA: 0x0005324E File Offset: 0x0005144E
	public BlockToolTerrainPaint(BlockTools.Brush _brush, NguiWdwTerrainEditor _parentWindow)
	{
		this.brush = _brush;
		this.parentWindow = _parentWindow;
		this.isButtonDown = false;
		this.paintBlock = BlockValue.Air;
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00002914 File Offset: 0x00000B14
	public void CheckSpecialKeys(Event ev, PlayerActionsLocal _playerAction)
	{
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00053284 File Offset: 0x00051484
	public void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo, PlayerActionsLocal _playerAction)
	{
		if (_data == null || _data.actionData == null || _data.actionData[0] == null)
		{
			return;
		}
		ItemStack item = GameManager.Instance.World.GetPrimaryPlayer().inventory.GetItem(7);
		this.paintBlock = item.itemValue.ToBlockValue();
		this.parentWindow.lastPosition = new Vector3i(_hitInfo.hit.pos);
		this.parentWindow.lastDirection = _hitInfo.ray.direction;
		if (!Input.GetMouseButton(0) || !this.isButtonDown)
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
		BlockTools.PaintTerrain(_data.world, _data.hitInfo.hit.clrIdx, _data.hitInfo.hit.blockPos, this.brush, this.blockChanges, this.paintBlock);
		_data.world.SetBlocksRPC(this.blockChanges);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		return false;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0002B133 File Offset: 0x00029333
	public string GetDebugOutput()
	{
		return "";
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x000533EC File Offset: 0x000515EC
	public virtual bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal _playerAction)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x000533EC File Offset: 0x000515EC
	public bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal _playerAction)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x00053400 File Offset: 0x00051600
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

	// Token: 0x06000C33 RID: 3123 RVA: 0x00053247 File Offset: 0x00051447
	public override string ToString()
	{
		return "Dig/Place Terrain";
	}

	// Token: 0x04000A42 RID: 2626
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTools.Brush brush;

	// Token: 0x04000A43 RID: 2627
	[PublicizedFrom(EAccessModifier.Private)]
	public NguiWdwTerrainEditor parentWindow;

	// Token: 0x04000A44 RID: 2628
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData lastData;

	// Token: 0x04000A45 RID: 2629
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isButtonDown;

	// Token: 0x04000A46 RID: 2630
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockChangeInfo> blockChanges = new List<BlockChangeInfo>();

	// Token: 0x04000A47 RID: 2631
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue paintBlock;
}
