using System;
using UnityEngine;

// Token: 0x0200109B RID: 4251
public struct HitInfoDetails
{
	// Token: 0x17000DF7 RID: 3575
	// (get) Token: 0x0600861C RID: 34332 RVA: 0x00367A94 File Offset: 0x00365C94
	public BlockValue blockValue
	{
		get
		{
			return this.voxelData.BlockValue;
		}
	}

	// Token: 0x17000DF8 RID: 3576
	// (get) Token: 0x0600861D RID: 34333 RVA: 0x00367AA1 File Offset: 0x00365CA1
	public WaterValue waterValue
	{
		get
		{
			return this.voxelData.WaterValue;
		}
	}

	// Token: 0x0600861E RID: 34334 RVA: 0x00367AAE File Offset: 0x00365CAE
	public void Clear()
	{
		this.pos = Vector3.zero;
		this.blockPos = Vector3i.zero;
		this.blockFace = BlockFace.Top;
		this.voxelData.Clear();
		this.clrIdx = 0;
		this.distanceSq = 0f;
	}

	// Token: 0x0600861F RID: 34335 RVA: 0x00367AEC File Offset: 0x00365CEC
	public void CopyFrom(HitInfoDetails _other)
	{
		this.pos = _other.pos;
		this.blockPos = _other.blockPos;
		this.blockFace = _other.blockFace;
		this.voxelData = _other.voxelData;
		this.clrIdx = _other.clrIdx;
		this.distanceSq = _other.distanceSq;
	}

	// Token: 0x06008620 RID: 34336 RVA: 0x00367B44 File Offset: 0x00365D44
	public HitInfoDetails Clone()
	{
		return new HitInfoDetails
		{
			pos = this.pos,
			blockPos = this.blockPos,
			blockFace = this.blockFace,
			voxelData = this.voxelData,
			clrIdx = this.clrIdx,
			distanceSq = this.distanceSq
		};
	}

	// Token: 0x04006831 RID: 26673
	public Vector3 pos;

	// Token: 0x04006832 RID: 26674
	public Vector3i blockPos;

	// Token: 0x04006833 RID: 26675
	public HitInfoDetails.VoxelData voxelData;

	// Token: 0x04006834 RID: 26676
	public BlockFace blockFace;

	// Token: 0x04006835 RID: 26677
	public float distanceSq;

	// Token: 0x04006836 RID: 26678
	public int clrIdx;

	// Token: 0x0200109C RID: 4252
	public struct VoxelData : IEquatable<HitInfoDetails.VoxelData>
	{
		// Token: 0x06008621 RID: 34337 RVA: 0x00367BA8 File Offset: 0x00365DA8
		public void Set(BlockValue _bv, WaterValue _wv)
		{
			this.BlockValue = _bv;
			this.WaterValue = _wv;
		}

		// Token: 0x06008622 RID: 34338 RVA: 0x00367BB8 File Offset: 0x00365DB8
		public static HitInfoDetails.VoxelData GetFrom(ChunkCluster _cc, Vector3i _blockPos)
		{
			return new HitInfoDetails.VoxelData
			{
				BlockValue = _cc.GetBlock(_blockPos),
				WaterValue = _cc.GetWater(_blockPos)
			};
		}

		// Token: 0x06008623 RID: 34339 RVA: 0x00367BEC File Offset: 0x00365DEC
		public static HitInfoDetails.VoxelData GetFrom(World _world, Vector3i _blockPos)
		{
			return new HitInfoDetails.VoxelData
			{
				BlockValue = _world.GetBlock(_blockPos),
				WaterValue = _world.GetWater(_blockPos)
			};
		}

		// Token: 0x06008624 RID: 34340 RVA: 0x00367C20 File Offset: 0x00365E20
		public static HitInfoDetails.VoxelData GetFrom(IChunk _chunk, int _x, int _y, int _z)
		{
			return new HitInfoDetails.VoxelData
			{
				BlockValue = _chunk.GetBlock(_x, _y, _z),
				WaterValue = _chunk.GetWater(_x, _y, _z)
			};
		}

		// Token: 0x06008625 RID: 34341 RVA: 0x00367C56 File Offset: 0x00365E56
		public bool IsOnlyAir()
		{
			return this.BlockValue.isair && !this.WaterValue.HasMass();
		}

		// Token: 0x06008626 RID: 34342 RVA: 0x00367C75 File Offset: 0x00365E75
		public bool IsOnlyWater()
		{
			return this.BlockValue.isair && this.WaterValue.HasMass();
		}

		// Token: 0x06008627 RID: 34343 RVA: 0x00367C91 File Offset: 0x00365E91
		public bool Equals(HitInfoDetails.VoxelData _other)
		{
			return this.BlockValue.Equals(_other.BlockValue) && this.WaterValue.HasMass() == _other.WaterValue.HasMass();
		}

		// Token: 0x06008628 RID: 34344 RVA: 0x00367CC1 File Offset: 0x00365EC1
		public void Clear()
		{
			this.BlockValue = BlockValue.Air;
			this.WaterValue = WaterValue.Empty;
		}

		// Token: 0x04006837 RID: 26679
		public BlockValue BlockValue;

		// Token: 0x04006838 RID: 26680
		public WaterValue WaterValue;
	}
}
