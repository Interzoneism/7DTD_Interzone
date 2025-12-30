using System;
using UnityEngine;

// Token: 0x0200016E RID: 366
public abstract class BlockShape
{
	// Token: 0x06000ADB RID: 2779 RVA: 0x0004702C File Offset: 0x0004522C
	public BlockShape()
	{
		this.bounds = BoundsUtils.BoundsForMinMax(0f, 0f, 0f, 1f, 1f, 1f);
		this.boundsArr = new Bounds[]
		{
			this.bounds
		};
		this.IsSolidCube = true;
		this.IsSolidSpace = true;
		this.IsRotatable = false;
		this.IsOmitTerrainSnappingUp = false;
		this.LightOpacity = byte.MaxValue;
		this.minBounds = Vector3.zero;
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x000470BA File Offset: 0x000452BA
	public virtual void Init(Block _block)
	{
		this.block = _block;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void LateInit()
	{
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x000470C3 File Offset: 0x000452C3
	public virtual Quaternion GetRotation(BlockValue _blockValue)
	{
		return Quaternion.identity;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x000470CA File Offset: 0x000452CA
	public virtual Vector3 GetRotationOffset(BlockValue _blockValue)
	{
		return Vector3.zero;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00019766 File Offset: 0x00017966
	public virtual int[][] GetRotationLookup(int _rotation)
	{
		return null;
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x000470D1 File Offset: 0x000452D1
	public virtual byte Rotate(bool _bLeft, int _rotation)
	{
		return (byte)_rotation;
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x000470D5 File Offset: 0x000452D5
	public virtual BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		_blockValue.rotation = (byte)((int)_blockValue.rotation + _rotCount & 15);
		return _blockValue;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x000470EC File Offset: 0x000452EC
	public virtual BlockValue MirrorY(bool _bAlongZ, BlockValue _blockValue)
	{
		_blockValue = this.RotateY(true, _blockValue, 1);
		return this.RotateY(true, _blockValue, 1);
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00047102 File Offset: 0x00045302
	public virtual int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 255;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return true;
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void renderFace(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, BlockFace _face, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void renderFace(Vector3[] _vertices, LightingAround _lightingAround, long _textureFull, VoxelMesh[] _meshes, Vector2 UVdata, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsRenderDecoration()
	{
		return false;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0004710C File Offset: 0x0004530C
	public virtual void renderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		this.renderFull(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, BlockShape.MeshPurpose.World);
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0004712B File Offset: 0x0004532B
	public virtual int MapSideAndRotationToTextureIdx(BlockValue _blockValue, BlockFace _side)
	{
		return (int)_side;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0004712E File Offset: 0x0004532E
	public virtual Bounds[] GetBounds(BlockValue _blockValue)
	{
		return this.boundsArr;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x00047136 File Offset: 0x00045336
	public virtual Vector2 GetPathOffset(int _rotation)
	{
		return Vector2.zero;
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x000470C3 File Offset: 0x000452C3
	public virtual Quaternion GetPreviewRotation()
	{
		return Quaternion.identity;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000470CA File Offset: 0x000452CA
	public virtual Vector3 GetPreviewPosition()
	{
		return Vector3.zero;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0004713D File Offset: 0x0004533D
	public virtual float GetStepHeight(BlockValue blockDef, BlockFace crossingFace)
	{
		return (float)(blockDef.Block.IsCollideMovement ? 1 : 0);
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x00047152 File Offset: 0x00045352
	public virtual bool IsMovementBlocked(BlockValue blockDef, BlockFace crossingFace)
	{
		return this.GetStepHeight(blockDef, crossingFace) > 0.5f;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00047163 File Offset: 0x00045363
	public void SetMinAABB(Vector3 _add)
	{
		this.minBounds = _add;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsTerrain()
	{
		return false;
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockValueChanged(WorldBase _world, Vector3i _blockPos, int _clrIdx, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00019766 File Offset: 0x00017966
	public virtual VoxelMesh GetBoundsMesh(BlockValue _blockValue)
	{
		return null;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0004712B File Offset: 0x0004532B
	public virtual BlockFace GetRotatedBlockFace(BlockValue _blockValue, BlockFace _face)
	{
		return _face;
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0004716C File Offset: 0x0004536C
	public virtual void MirrorFace(EnumMirrorAlong _axis, int _sourceRot, int _targetRot, BlockFace _face, out BlockFace _sourceFace, out BlockFace _targetFace)
	{
		_sourceFace = _face;
		_targetFace = _face;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int GetVertexCount()
	{
		return 0;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int GetTriangleCount()
	{
		return 0;
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00047178 File Offset: 0x00045378
	public virtual string GetName()
	{
		return string.Empty;
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool UseRepairDamageState(BlockValue _blockValue)
	{
		return false;
	}

	// Token: 0x040009A0 RID: 2464
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvZero = Vector2.zero;

	// Token: 0x040009A1 RID: 2465
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvOne = Vector2.one;

	// Token: 0x040009A2 RID: 2466
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvRightBot = new Vector2(1f, 0f);

	// Token: 0x040009A3 RID: 2467
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvLeftTop = new Vector2(0f, 1f);

	// Token: 0x040009A4 RID: 2468
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvMiddle = new Vector2(0.5f, 0.5f);

	// Token: 0x040009A5 RID: 2469
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvMidBot = new Vector2(0.5f, 0f);

	// Token: 0x040009A6 RID: 2470
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvMidTop = new Vector2(0.5f, 1f);

	// Token: 0x040009A7 RID: 2471
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvLeftMid = new Vector2(0f, 0.5f);

	// Token: 0x040009A8 RID: 2472
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvRightMid = new Vector2(1f, 0.5f);

	// Token: 0x040009A9 RID: 2473
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector4 tngRight = new Vector4(1f, 0f, 0f, 1f);

	// Token: 0x040009AA RID: 2474
	[PublicizedFrom(EAccessModifier.Protected)]
	public Block block;

	// Token: 0x040009AB RID: 2475
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds bounds;

	// Token: 0x040009AC RID: 2476
	[PublicizedFrom(EAccessModifier.Protected)]
	public Bounds[] boundsArr;

	// Token: 0x040009AD RID: 2477
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 minBounds;

	// Token: 0x040009AE RID: 2478
	public bool IsSolidCube;

	// Token: 0x040009AF RID: 2479
	public bool IsRotatable;

	// Token: 0x040009B0 RID: 2480
	public bool IsSolidSpace;

	// Token: 0x040009B1 RID: 2481
	public bool IsOmitTerrainSnappingUp;

	// Token: 0x040009B2 RID: 2482
	public bool IsNotifyOnLoadUnload;

	// Token: 0x040009B3 RID: 2483
	public byte LightOpacity;

	// Token: 0x040009B4 RID: 2484
	public int SymmetryType = 1;

	// Token: 0x040009B5 RID: 2485
	public bool Has45DegreeRotations;

	// Token: 0x0200016F RID: 367
	public enum MeshPurpose
	{
		// Token: 0x040009B7 RID: 2487
		World,
		// Token: 0x040009B8 RID: 2488
		Drop,
		// Token: 0x040009B9 RID: 2489
		Hold,
		// Token: 0x040009BA RID: 2490
		Local,
		// Token: 0x040009BB RID: 2491
		Preview
	}
}
