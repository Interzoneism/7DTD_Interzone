using System;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class BlockToolTerrainSmoothing : IBlockTool
{
	// Token: 0x06000C34 RID: 3124 RVA: 0x00053453 File Offset: 0x00051653
	public BlockToolTerrainSmoothing(BlockTools.Brush _brush, NguiWdwTerrainEditor _parentWindow)
	{
		this.brush = _brush;
		this.parentWindow = _parentWindow;
		this.isButtonDown = false;
		this.SetUp22DegreeRules();
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x00002914 File Offset: 0x00000B14
	public void CheckSpecialKeys(Event ev, PlayerActionsLocal playerActions)
	{
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x00053478 File Offset: 0x00051678
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
			this.SmoothTerrain();
			return;
		}
		if (!Input.GetMouseButton(0) && this.isButtonDown)
		{
			this.isButtonDown = false;
		}
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		return false;
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0002B133 File Offset: 0x00029333
	public string GetDebugOutput()
	{
		return "";
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x000534FE File Offset: 0x000516FE
	public bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
	{
		this.isButtonDown = true;
		this.lastData = _data;
		return true;
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x00053510 File Offset: 0x00051710
	[PublicizedFrom(EAccessModifier.Private)]
	public void SmoothTerrain()
	{
		ItemInventoryData itemInventoryData = this.lastData;
		if (Time.time - itemInventoryData.actionData[0].lastUseTime > Constants.cBuildIntervall && itemInventoryData.hitInfo.bHitValid && GameUtils.IsBlockOrTerrain(itemInventoryData.hitInfo.tag))
		{
			Vector3i blockPos = itemInventoryData.hitInfo.hit.blockPos;
			BlockValue block = itemInventoryData.world.GetBlock(blockPos);
			Vector3i[] cubesInBrush = this.brush.GetCubesInBrush();
			for (int i = 0; i < cubesInBrush.Length; i++)
			{
				Vector3i vector3i = blockPos + cubesInBrush[i];
				if (this.HasValidNeighbor(vector3i, 0, itemInventoryData))
				{
					BlockValue block2 = itemInventoryData.world.GetBlock(vector3i);
					bool flag = block2.Equals(BlockValue.Air);
					sbyte averageDensity = this.GetAverageDensity(vector3i, itemInventoryData, 0);
					if (averageDensity <= -1 && flag)
					{
						itemInventoryData.world.SetBlockRPC(vector3i, block, averageDensity);
					}
					else if (averageDensity >= 0 && !flag)
					{
						itemInventoryData.world.SetBlockRPC(vector3i, BlockValue.Air, averageDensity);
					}
					else
					{
						itemInventoryData.world.SetBlockRPC(0, vector3i, block2, averageDensity);
					}
				}
			}
		}
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x00053644 File Offset: 0x00051844
	[PublicizedFrom(EAccessModifier.Private)]
	public void SnapTerrain22()
	{
		ItemInventoryData itemInventoryData = this.lastData;
		if (Time.time - itemInventoryData.actionData[0].lastUseTime > Constants.cBuildIntervall && itemInventoryData.hitInfo.bHitValid && GameUtils.IsBlockOrTerrain(itemInventoryData.hitInfo.tag))
		{
			Vector3i blockPos = itemInventoryData.hitInfo.hit.blockPos;
			BlockValue block = itemInventoryData.world.GetBlock(blockPos);
			Vector3i[] cubesInBrush = this.brush.GetCubesInBrush();
			for (int i = 0; i < cubesInBrush.Length; i++)
			{
				Vector3i vector3i = blockPos + cubesInBrush[i];
				if (this.HasValidNeighbor(vector3i, 0, itemInventoryData))
				{
					bool flag = itemInventoryData.world.GetBlock(cubesInBrush[i]).Equals(BlockValue.Air);
					sbyte outputBasedOnRuleset = this.GetOutputBasedOnRuleset22(vector3i, itemInventoryData);
					if (outputBasedOnRuleset <= -1 && flag)
					{
						itemInventoryData.world.SetBlockRPC(vector3i, block, outputBasedOnRuleset);
					}
					else if (outputBasedOnRuleset >= 0 && !flag)
					{
						itemInventoryData.world.SetBlockRPC(vector3i, BlockValue.Air, outputBasedOnRuleset);
					}
					else
					{
						itemInventoryData.world.SetBlockRPC(vector3i, outputBasedOnRuleset);
					}
				}
			}
		}
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x00053778 File Offset: 0x00051978
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetUp22DegreeRules()
	{
		this.blockMaskRuleset = new BlockMaskRules<sbyte?, char>('*');
		this.blockMaskRuleset.AddRule(new BlockRule<sbyte?, char>(new sbyte?((sbyte)-1), new char[]
		{
			'*',
			'A',
			'*',
			'*',
			'A',
			'*',
			'*',
			'A',
			'*',
			'*',
			'B',
			'*',
			'*',
			'*',
			'*',
			'*',
			'A',
			'*',
			'*',
			'B',
			'*',
			'*',
			'B',
			'*',
			'*',
			'B',
			'*'
		}));
		this.blockMaskRuleset.AddRule(new BlockRule<sbyte?, char>(new sbyte?((sbyte)-1), new char[]
		{
			'*',
			'A',
			'*',
			'*',
			'A',
			'*',
			'*',
			'A',
			'*',
			'*',
			'B',
			'*',
			'*',
			'*',
			'*',
			'*',
			'A',
			'*',
			'*',
			'B',
			'*',
			'*',
			'B',
			'*',
			'*',
			'B',
			'*'
		}));
		this.blockMaskRuleset.AddRule(new BlockRule<sbyte?, char>(new sbyte?((sbyte)-1), new char[]
		{
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'H',
			'A',
			'H',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*'
		}));
		this.blockMaskRuleset.AddRule(new BlockRule<sbyte?, char>(new sbyte?((sbyte)-128), new char[]
		{
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'B',
			'A',
			'B',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*'
		}));
		this.blockMaskRuleset.AddRule(new BlockRule<sbyte?, char>(new sbyte?((sbyte)-128), new char[]
		{
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'B',
			'H',
			'B',
			'*',
			'H',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*',
			'*'
		}));
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00053864 File Offset: 0x00051A64
	[PublicizedFrom(EAccessModifier.Private)]
	public sbyte GetOutputBasedOnRuleset22(Vector3i _pos, ItemInventoryData _data)
	{
		char[] array = new char[27];
		int num = 0;
		for (int i = 1; i >= -1; i--)
		{
			for (int j = 1; j >= -1; j--)
			{
				for (int k = 1; k >= -1; k--)
				{
					int density = (int)_data.world.GetDensity(0, _pos.x + k, _pos.y + i, _pos.z + j);
					if (density >= 0)
					{
						array[num] = 'A';
					}
					else if (density > -32)
					{
						array[num] = 'H';
					}
					else
					{
						array[num] = 'B';
					}
					num++;
				}
			}
		}
		sbyte? output = this.blockMaskRuleset.GetOutput(array);
		if (output != null)
		{
			return output.Value;
		}
		sbyte density2 = _data.world.GetDensity(0, _pos.x, _pos.y, _pos.z);
		if (density2 >= 0)
		{
			return sbyte.MaxValue;
		}
		if (density2 > -32)
		{
			return -1;
		}
		return sbyte.MinValue;
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00053944 File Offset: 0x00051B44
	[PublicizedFrom(EAccessModifier.Private)]
	public void SnapTerrain45()
	{
		ItemInventoryData itemInventoryData = this.lastData;
		if (Time.time - itemInventoryData.actionData[0].lastUseTime > Constants.cBuildIntervall && itemInventoryData.hitInfo.bHitValid && GameUtils.IsBlockOrTerrain(itemInventoryData.hitInfo.tag))
		{
			Vector3i blockPos = itemInventoryData.hitInfo.hit.blockPos;
			BlockValue block = itemInventoryData.world.GetBlock(blockPos);
			Vector3i[] cubesInBrush = this.brush.GetCubesInBrush();
			for (int i = 0; i < cubesInBrush.Length; i++)
			{
				Vector3i vector3i = blockPos + cubesInBrush[i];
				if (this.HasValidNeighbor(vector3i, 0, itemInventoryData))
				{
					bool flag = itemInventoryData.world.GetBlock(vector3i).Equals(BlockValue.Air);
					sbyte b = this.GetAverageDensity(vector3i, itemInventoryData, 0);
					if (this.AirNeighborCount(vector3i, itemInventoryData) > 3 && !flag)
					{
						b = sbyte.MinValue;
					}
					if (b <= -1 && flag)
					{
						itemInventoryData.world.SetBlockRPC(vector3i, block, b);
					}
					else if (b >= 0 && !flag)
					{
						itemInventoryData.world.SetBlockRPC(vector3i, BlockValue.Air, b);
					}
					else
					{
						itemInventoryData.world.SetBlockRPC(vector3i, b);
					}
				}
			}
		}
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
	{
		return false;
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00053A8C File Offset: 0x00051C8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public int AirNeighborCount(Vector3i _pos, ItemInventoryData _data)
	{
		int num = 0;
		for (int i = _pos.x - 1; i <= _pos.x + 1; i++)
		{
			for (int j = _pos.z - 1; j <= _pos.z + 1; j++)
			{
				if ((i != 0 || j != 0) && _data.world.GetBlock(i, _pos.y, j).Equals(BlockValue.Air))
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00053AFC File Offset: 0x00051CFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public sbyte GetAverageLockedDensity(Vector3i _pos, ItemInventoryData _data)
	{
		Vector3i[] cubesInBrush = this.brush.GetCubesInBrush();
		int num = (int)_data.world.GetDensity(0, _pos.x, _pos.y, _pos.z);
		for (int i = 0; i < cubesInBrush.Length; i++)
		{
			Vector3i blockPos = _pos + cubesInBrush[i];
			num += (int)_data.world.GetDensity(0, blockPos);
		}
		return (sbyte)(num / (cubesInBrush.Length + 1));
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x00053B68 File Offset: 0x00051D68
	[PublicizedFrom(EAccessModifier.Protected)]
	public sbyte GetAverageDensity(Vector3i _pos, ItemInventoryData _data, int _clrIdx = 0)
	{
		float num = 0f;
		float num2 = 1f;
		float num3 = num + (float)_data.world.GetDensity(_clrIdx, _pos.x, _pos.y, _pos.z);
		num2 += 1f;
		float num4 = num3 + (float)_data.world.GetDensity(_clrIdx, _pos.x - 1, _pos.y, _pos.z);
		num2 += 1f;
		float num5 = num4 + (float)_data.world.GetDensity(_clrIdx, _pos.x + 1, _pos.y, _pos.z);
		num2 += 1f;
		float num6 = num5 + (float)_data.world.GetDensity(_clrIdx, _pos.x, _pos.y, _pos.z - 1);
		num2 += 1f;
		return (sbyte)((num6 + (float)_data.world.GetDensity(_clrIdx, _pos.x, _pos.y, _pos.z + 1)) / num2);
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x00053C4C File Offset: 0x00051E4C
	[PublicizedFrom(EAccessModifier.Private)]
	public sbyte[] GetLocalDensityMap(Vector3i blockTargetPos, ItemInventoryData data, Vector3i[] localArea)
	{
		sbyte[] array = new sbyte[localArea.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = data.world.GetDensity(0, localArea[i].x, localArea[i].y, localArea[i].z);
		}
		return array;
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x00053CA4 File Offset: 0x00051EA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool HasValidNeighbor(Vector3i _pos, sbyte _denThreshold, ItemInventoryData _data)
	{
		if (_denThreshold < 0)
		{
			return _data.world.GetDensity(0, _pos.x - 1, _pos.y, _pos.z) <= _denThreshold || _data.world.GetDensity(0, _pos.x + 1, _pos.y, _pos.z) <= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y - 1, _pos.z) <= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y + 1, _pos.z) <= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y, _pos.z - 1) <= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y, _pos.z + 1) <= _denThreshold;
		}
		return _denThreshold >= 0 && (_data.world.GetDensity(0, _pos.x - 1, _pos.y, _pos.z) >= _denThreshold || _data.world.GetDensity(0, _pos.x + 1, _pos.y, _pos.z) >= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y - 1, _pos.z) >= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y + 1, _pos.z) >= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y, _pos.z - 1) >= _denThreshold || _data.world.GetDensity(0, _pos.x, _pos.y, _pos.z + 1) >= _denThreshold);
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x00053E80 File Offset: 0x00052080
	public override string ToString()
	{
		return "Smooth Terrain";
	}

	// Token: 0x04000A48 RID: 2632
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTools.Brush brush;

	// Token: 0x04000A49 RID: 2633
	[PublicizedFrom(EAccessModifier.Private)]
	public NguiWdwTerrainEditor parentWindow;

	// Token: 0x04000A4A RID: 2634
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemInventoryData lastData;

	// Token: 0x04000A4B RID: 2635
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isButtonDown;

	// Token: 0x04000A4C RID: 2636
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockMaskRules<sbyte?, char> blockMaskRuleset;
}
