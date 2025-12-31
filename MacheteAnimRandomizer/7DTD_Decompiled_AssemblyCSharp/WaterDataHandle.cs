using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000B60 RID: 2912
public struct WaterDataHandle : IDisposable
{
	// Token: 0x1700093C RID: 2364
	// (get) Token: 0x06005A82 RID: 23170 RVA: 0x002449F4 File Offset: 0x00242BF4
	public UnsafeBitArraySetIndicesEnumerator ActiveVoxelIndices
	{
		get
		{
			return new UnsafeBitArraySetIndicesEnumerator(this.activeVoxels);
		}
	}

	// Token: 0x1700093D RID: 2365
	// (get) Token: 0x06005A83 RID: 23171 RVA: 0x00244A01 File Offset: 0x00242C01
	public UnsafeParallelHashMap<int, int>.Enumerator FlowVoxels
	{
		get
		{
			return this.flowVoxels.GetEnumerator();
		}
	}

	// Token: 0x1700093E RID: 2366
	// (get) Token: 0x06005A84 RID: 23172 RVA: 0x00244A0E File Offset: 0x00242C0E
	public bool HasActiveWater
	{
		get
		{
			return this.activeVoxels.TestAny(0, this.activeVoxels.Length);
		}
	}

	// Token: 0x1700093F RID: 2367
	// (get) Token: 0x06005A85 RID: 23173 RVA: 0x00244A27 File Offset: 0x00242C27
	public bool HasFlows
	{
		get
		{
			return !this.flowVoxels.IsEmpty;
		}
	}

	// Token: 0x06005A86 RID: 23174 RVA: 0x00244A38 File Offset: 0x00242C38
	public static WaterDataHandle AllocateNew(Allocator allocator)
	{
		WaterDataHandle result = default(WaterDataHandle);
		result.Allocate(allocator);
		return result;
	}

	// Token: 0x06005A87 RID: 23175 RVA: 0x00244A58 File Offset: 0x00242C58
	[PublicizedFrom(EAccessModifier.Private)]
	public void Allocate(Allocator allocator)
	{
		this.voxelData = new UnsafeChunkData<int>(allocator);
		this.voxelState = new UnsafeChunkData<WaterVoxelState>(allocator);
		this.groundWaterHeights = new UnsafeChunkXZMap<GroundWaterBounds>(allocator);
		this.activeVoxels = new UnsafeBitArray(65536, allocator, NativeArrayOptions.ClearMemory);
		this.flowVoxels = new UnsafeParallelHashMap<int, int>(1000, allocator);
		this.flowsFromOtherChunks = new UnsafeFixedBuffer<WaterFlow>(16384, allocator);
		this.activationsFromOtherChunks = new UnsafeFixedBuffer<int>(16384, allocator);
		this.voxelsToWakeup = new UnsafeParallelHashSet<int>(256, allocator);
	}

	// Token: 0x06005A88 RID: 23176 RVA: 0x00244B08 File Offset: 0x00242D08
	public bool IsInGroundWater(int _x, int _y, int _z)
	{
		GroundWaterBounds groundWaterBounds = this.groundWaterHeights.Get(_x, _z);
		return groundWaterBounds.IsGroundWater && _y >= (int)groundWaterBounds.bottom && _y <= (int)groundWaterBounds.waterHeight;
	}

	// Token: 0x06005A89 RID: 23177 RVA: 0x00244B45 File Offset: 0x00242D45
	public void SetVoxelActive(int _x, int _y, int _z)
	{
		this.activeVoxels.Set(WaterDataHandle.GetVoxelIndex(_x, _y, _z), true);
	}

	// Token: 0x06005A8A RID: 23178 RVA: 0x00244B5B File Offset: 0x00242D5B
	public void SetVoxelActive(int _index)
	{
		this.activeVoxels.Set(_index, true);
	}

	// Token: 0x06005A8B RID: 23179 RVA: 0x00244B6A File Offset: 0x00242D6A
	public void EnqueueVoxelActive(int _x, int _y, int _z)
	{
		this.EnqueueVoxelActive(WaterDataHandle.GetVoxelIndex(_x, _y, _z));
	}

	// Token: 0x06005A8C RID: 23180 RVA: 0x00244B7A File Offset: 0x00242D7A
	public void EnqueueVoxelActive(int _index)
	{
		this.activationsFromOtherChunks.AddThreadSafe(_index);
	}

	// Token: 0x06005A8D RID: 23181 RVA: 0x00244B88 File Offset: 0x00242D88
	public void EnqueueVoxelWakeup(int _x, int _y, int _z)
	{
		this.EnqueueVoxelWakeup(WaterDataHandle.GetVoxelIndex(_x, _y, _z));
	}

	// Token: 0x06005A8E RID: 23182 RVA: 0x00244B98 File Offset: 0x00242D98
	public void EnqueueVoxelWakeup(int _index)
	{
		this.voxelsToWakeup.Add(_index);
	}

	// Token: 0x06005A8F RID: 23183 RVA: 0x00244BA8 File Offset: 0x00242DA8
	public void ApplyEnqueuedActivations()
	{
		NativeArray<int> nativeArray = this.activationsFromOtherChunks.AsNativeArray();
		for (int i = 0; i < nativeArray.Length; i++)
		{
			int voxelActive = nativeArray[i];
			this.SetVoxelActive(voxelActive);
		}
		this.activationsFromOtherChunks.Clear();
	}

	// Token: 0x06005A90 RID: 23184 RVA: 0x00244BEE File Offset: 0x00242DEE
	public void SetVoxelInactive(int _index)
	{
		this.activeVoxels.Set(_index, false);
	}

	// Token: 0x06005A91 RID: 23185 RVA: 0x00244C00 File Offset: 0x00242E00
	public void SetVoxelMass(int _x, int _y, int _z, int _mass)
	{
		int voxelIndex = WaterDataHandle.GetVoxelIndex(_x, _y, _z);
		this.SetVoxelMass(voxelIndex, _mass);
	}

	// Token: 0x06005A92 RID: 23186 RVA: 0x00244C1F File Offset: 0x00242E1F
	public void SetVoxelMass(int _index, int _mass)
	{
		if (_mass > 195)
		{
			this.activeVoxels.Set(_index, true);
		}
		else
		{
			this.activeVoxels.Set(_index, false);
		}
		this.voxelData.Set(_index, _mass);
	}

	// Token: 0x06005A93 RID: 23187 RVA: 0x00244C54 File Offset: 0x00242E54
	public void SetVoxelSolid(int _x, int _y, int _z, BlockFaceFlag _flags)
	{
		int voxelIndex = WaterDataHandle.GetVoxelIndex(_x, _y, _z);
		WaterVoxelState waterVoxelState = this.voxelState.Get(voxelIndex);
		WaterVoxelState value = default(WaterVoxelState);
		value.SetSolid(_flags);
		this.voxelState.Set(voxelIndex, value);
		GroundWaterBounds groundWaterBounds = this.groundWaterHeights.Get(_x, _z);
		if (groundWaterBounds.IsGroundWater)
		{
			if (waterVoxelState.IsSolidYNeg() && !value.IsSolidYNeg() && _y == (int)groundWaterBounds.bottom)
			{
				groundWaterBounds.bottom = (byte)this.FindGroundWaterBottom(voxelIndex);
				this.groundWaterHeights.Set(_x, _z, groundWaterBounds);
				return;
			}
			if (waterVoxelState.IsSolidYPos() && !value.IsSolidYPos() && _y + 1 == (int)groundWaterBounds.bottom)
			{
				groundWaterBounds.bottom = (byte)this.FindGroundWaterBottom(voxelIndex);
				this.groundWaterHeights.Set(_x, _z, groundWaterBounds);
				return;
			}
			if (!waterVoxelState.IsSolidYNeg() && value.IsSolidYNeg() && _y > (int)groundWaterBounds.bottom && _y <= (int)groundWaterBounds.waterHeight)
			{
				groundWaterBounds.bottom = (byte)_y;
				this.groundWaterHeights.Set(_x, _z, groundWaterBounds);
				return;
			}
			if (!waterVoxelState.IsSolidYPos() && value.IsSolidYPos())
			{
				int num = _y + 1;
				if (num > (int)groundWaterBounds.bottom && num <= (int)groundWaterBounds.waterHeight)
				{
					groundWaterBounds.bottom = (byte)num;
					this.groundWaterHeights.Set(_x, _z, groundWaterBounds);
				}
			}
		}
	}

	// Token: 0x06005A94 RID: 23188 RVA: 0x00244DA2 File Offset: 0x00242FA2
	public void ApplyFlow(int _x, int _y, int _z, int _flow)
	{
		this.ApplyFlow(WaterDataHandle.GetVoxelIndex(_x, _y, _z), _flow);
	}

	// Token: 0x06005A95 RID: 23189 RVA: 0x00244DB4 File Offset: 0x00242FB4
	public void ApplyFlow(int _index, int _flow)
	{
		int num;
		if (this.flowVoxels.TryGetValue(_index, out num))
		{
			_flow += num;
		}
		this.flowVoxels[_index] = _flow;
	}

	// Token: 0x06005A96 RID: 23190 RVA: 0x00244DE4 File Offset: 0x00242FE4
	public void EnqueueFlow(int _voxelIndex, int _flow)
	{
		this.flowsFromOtherChunks.AddThreadSafe(new WaterFlow
		{
			voxelIndex = _voxelIndex,
			flow = _flow
		});
	}

	// Token: 0x06005A97 RID: 23191 RVA: 0x00244E18 File Offset: 0x00243018
	public void ApplyEnqueuedFlows()
	{
		NativeArray<WaterFlow> nativeArray = this.flowsFromOtherChunks.AsNativeArray();
		for (int i = 0; i < nativeArray.Length; i++)
		{
			WaterFlow waterFlow = nativeArray[i];
			this.ApplyFlow(waterFlow.voxelIndex, waterFlow.flow);
		}
		this.flowsFromOtherChunks.Clear();
	}

	// Token: 0x06005A98 RID: 23192 RVA: 0x00244E6C File Offset: 0x0024306C
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindGroundWaterBottom(int _fromIndex)
	{
		for (int i = _fromIndex; i >= 0; i -= 256)
		{
			WaterVoxelState waterVoxelState = this.voxelState.Get(i);
			if (waterVoxelState.IsSolidYNeg())
			{
				return WaterDataHandle.GetVoxelY(i);
			}
			if (waterVoxelState.IsSolidYPos())
			{
				int num = math.min(i + 256, 255);
				if (num <= _fromIndex)
				{
					return WaterDataHandle.GetVoxelY(num);
				}
			}
		}
		return 0;
	}

	// Token: 0x06005A99 RID: 23193 RVA: 0x00244ED0 File Offset: 0x002430D0
	public void InitializeFromChunk(Chunk _chunk, GroundWaterHeightMap _groundWaterHeightMap)
	{
		if (!this.voxelData.IsCreated || !this.activeVoxels.IsCreated)
		{
			Debug.LogError("Could not initialize WaterDataHandle because it has not been allocated");
			return;
		}
		this.Clear();
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					WaterVoxelState value = default(WaterVoxelState);
					BlockValue blockNoDamage = _chunk.GetBlockNoDamage(k, i, j);
					Block block = blockNoDamage.Block;
					byte rotation = blockNoDamage.rotation;
					value.SetSolid(BlockFaceFlags.RotateFlags(block.WaterFlowMask, rotation));
					int voxelIndex = WaterDataHandle.GetVoxelIndex(k, i, j);
					int mass = _chunk.GetWater(k, i, j).GetMass();
					if (mass > 195)
					{
						this.activeVoxels.Set(voxelIndex, true);
						this.voxelData.Set(voxelIndex, mass);
					}
					if (!value.IsDefault())
					{
						this.voxelState.Set(voxelIndex, value);
					}
				}
			}
		}
		this.voxelState.CheckSameValues();
		if (_groundWaterHeightMap.TryInit())
		{
			for (int l = 0; l < 16; l++)
			{
				for (int m = 0; m < 16; m++)
				{
					Vector3i vector3i = _chunk.ToWorldPos(m, 0, l);
					int num;
					if (_groundWaterHeightMap.TryGetWaterHeightAt(vector3i.x, vector3i.z, out num))
					{
						int groundHeight = this.FindGroundWaterBottom(WaterDataHandle.GetVoxelIndex(m, num, l));
						this.groundWaterHeights.Set(m, l, new GroundWaterBounds(groundHeight, num));
					}
				}
			}
		}
	}

	// Token: 0x06005A9A RID: 23194 RVA: 0x00245060 File Offset: 0x00243260
	public void Clear()
	{
		if (this.voxelData.IsCreated)
		{
			this.voxelData.Clear();
		}
		if (this.voxelState.IsCreated)
		{
			this.voxelState.Clear();
		}
		if (this.groundWaterHeights.IsCreated)
		{
			this.groundWaterHeights.Clear();
		}
		if (this.activeVoxels.IsCreated)
		{
			this.activeVoxels.Clear();
		}
		if (this.flowVoxels.IsCreated)
		{
			this.flowVoxels.Clear();
		}
		if (this.flowsFromOtherChunks.IsCreated)
		{
			this.flowsFromOtherChunks.Clear();
		}
		if (this.activationsFromOtherChunks.IsCreated)
		{
			this.activationsFromOtherChunks.Clear();
		}
		if (this.voxelsToWakeup.IsCreated)
		{
			this.voxelsToWakeup.Clear();
		}
	}

	// Token: 0x06005A9B RID: 23195 RVA: 0x00245130 File Offset: 0x00243330
	public void Dispose()
	{
		if (this.voxelData.IsCreated)
		{
			this.voxelData.Dispose();
		}
		if (this.voxelState.IsCreated)
		{
			this.voxelState.Dispose();
		}
		if (this.groundWaterHeights.IsCreated)
		{
			this.groundWaterHeights.Dispose();
		}
		if (this.activeVoxels.IsCreated)
		{
			this.activeVoxels.Dispose();
		}
		if (this.flowVoxels.IsCreated)
		{
			this.flowVoxels.Dispose();
		}
		if (this.flowsFromOtherChunks.IsCreated)
		{
			this.flowsFromOtherChunks.Dispose();
		}
		if (this.activationsFromOtherChunks.IsCreated)
		{
			this.activationsFromOtherChunks.Dispose();
		}
		if (this.voxelsToWakeup.IsCreated)
		{
			this.voxelsToWakeup.Dispose();
		}
	}

	// Token: 0x06005A9C RID: 23196 RVA: 0x00245200 File Offset: 0x00243400
	public int CalculateOwnedBytes()
	{
		return 0 + this.voxelData.CalculateOwnedBytes() + this.voxelState.CalculateOwnedBytes() + this.groundWaterHeights.CalculateOwnedBytes() + ProfilerUtils.CalculateUnsafeBitArrayBytes(this.activeVoxels) + ProfilerUtils.CalculateUnsafeParallelHashMapBytes<int, int>(this.flowVoxels) + this.flowsFromOtherChunks.CalculateOwnedBytes() + this.activationsFromOtherChunks.CalculateOwnedBytes() + ProfilerUtils.CalculateUnsafeParallelHashSetBytes<int>(this.voxelsToWakeup);
	}

	// Token: 0x06005A9D RID: 23197 RVA: 0x00245270 File Offset: 0x00243470
	public string GetMemoryStats()
	{
		return string.Format("voxelData: {0:F2} KB, voxelState: {1:F2} KB, groundWaterHeights: {2:F2} KB, activeVoxels: ({3:F2} KB), flowVoxels: ({4},{5},{6:F2} KB), flowsFromOtherChunks: {7:F2} KB, activationsFromOtherChunks: {8:F2} KB, voxelsToWakeup {9:F2} KB, Total: {10:F2} MB", new object[]
		{
			(double)this.voxelData.CalculateOwnedBytes() * 0.0009765625,
			(double)this.voxelState.CalculateOwnedBytes() * 0.0009765625,
			(double)this.groundWaterHeights.CalculateOwnedBytes() * 0.0009765625,
			(double)ProfilerUtils.CalculateUnsafeBitArrayBytes(this.activeVoxels) * 0.0009765625,
			this.flowVoxels.Count(),
			this.flowVoxels.Capacity,
			(double)ProfilerUtils.CalculateUnsafeParallelHashMapBytes<int, int>(this.flowVoxels) * 0.0009765625,
			(double)this.flowsFromOtherChunks.CalculateOwnedBytes() * 0.0009765625,
			(double)this.activationsFromOtherChunks.CalculateOwnedBytes() * 0.0009765625,
			(double)ProfilerUtils.CalculateUnsafeParallelHashSetBytes<int>(this.voxelsToWakeup) * 0.0009765625,
			(double)this.CalculateOwnedBytes() * 9.5367431640625E-07
		});
	}

	// Token: 0x06005A9E RID: 23198 RVA: 0x002453BF File Offset: 0x002435BF
	public static int GetVoxelIndex(int _x, int _y, int _z)
	{
		return _x + _y * 256 + _z * 16;
	}

	// Token: 0x06005A9F RID: 23199 RVA: 0x002453D0 File Offset: 0x002435D0
	public static int3 GetVoxelCoords(int index)
	{
		int3 result = default(int3);
		result.y = index / 256;
		int num = index % 256;
		result.z = num / 16;
		result.x = num % 16;
		return result;
	}

	// Token: 0x06005AA0 RID: 23200 RVA: 0x00245412 File Offset: 0x00243612
	public static int GetVoxelY(int _index)
	{
		return _index / 256;
	}

	// Token: 0x04004537 RID: 17719
	public UnsafeChunkData<int> voxelData;

	// Token: 0x04004538 RID: 17720
	public UnsafeChunkData<WaterVoxelState> voxelState;

	// Token: 0x04004539 RID: 17721
	public UnsafeChunkXZMap<GroundWaterBounds> groundWaterHeights;

	// Token: 0x0400453A RID: 17722
	public UnsafeBitArray activeVoxels;

	// Token: 0x0400453B RID: 17723
	public UnsafeParallelHashMap<int, int> flowVoxels;

	// Token: 0x0400453C RID: 17724
	public UnsafeFixedBuffer<WaterFlow> flowsFromOtherChunks;

	// Token: 0x0400453D RID: 17725
	public UnsafeFixedBuffer<int> activationsFromOtherChunks;

	// Token: 0x0400453E RID: 17726
	public UnsafeParallelHashSet<int> voxelsToWakeup;
}
