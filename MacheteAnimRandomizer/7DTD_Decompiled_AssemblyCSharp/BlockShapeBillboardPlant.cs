using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000174 RID: 372
[Preserve]
public class BlockShapeBillboardPlant : BlockShapeBillboardRotatedAbstract
{
	// Token: 0x06000B0F RID: 2831 RVA: 0x00048038 File Offset: 0x00046238
	public BlockShapeBillboardPlant()
	{
		this.boundsArr = new Bounds[]
		{
			BoundsUtils.BoundsForMinMax(0.3f, 0f, 0.3f, 0.7f, 1f, 0.7f)
		};
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00048084 File Offset: 0x00046284
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createVertices()
	{
		this.vertices = new Vector3[]
		{
			new Vector3(-0.35f, -0.15f, -0.35f),
			new Vector3(-0.34f, 1.15f, -0.35f),
			new Vector3(1.36f, 1.15f, -0.35f),
			new Vector3(1.35f, -0.15f, -0.35f),
			new Vector3(-0.35f, -0.15f, 1.35f),
			new Vector3(-0.34f, 1.15f, 1.35f),
			new Vector3(1.36f, 1.15f, 1.35f),
			new Vector3(1.35f, -0.15f, 1.35f)
		};
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00048175 File Offset: 0x00046375
	public override Quaternion GetRotation(BlockValue _blockValue)
	{
		return Quaternion.AngleAxis((float)(20 * _blockValue.rotation), Vector3.up);
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0004818C File Offset: 0x0004638C
	public override void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		byte sun = _lightingAround[LightingAround.Pos.Middle].sun;
		byte block = _lightingAround[LightingAround.Pos.Middle].block;
		Block block2 = _blockValue.Block;
		int meshIndex = (int)block2.MeshIndex;
		VoxelMesh voxelMesh = _meshes[meshIndex];
		if (meshIndex == 3)
		{
			Rect uvrectFromSideAndMetadata = block2.getUVRectFromSideAndMetadata(meshIndex, BlockFace.Top, Vector3.zero, _blockValue);
			BlockShapeBillboardPlant.RenderData renderData;
			renderData.count = 2 + MeshDescription.GrassQualityPlanes;
			renderData.count2 = 0;
			renderData.offsetY = -0.05f;
			renderData.scale = 1.25f;
			renderData.height = renderData.scale;
			renderData.sideShift = 0.04f;
			renderData.rotation = (float)(20 * _blockValue.rotation);
			BlockShapeBillboardPlant.RenderSpinMesh(voxelMesh, _drawPos, _vertices, uvrectFromSideAndMetadata, sun, block, renderData);
			BlockShapeBillboardPlant.AddCollider(voxelMesh, _drawPos, 0.85f);
			return;
		}
		Vector3[] array = base.rotateVertices(this.vertices, _drawPos, _blockValue);
		voxelMesh.AddBlockSide(array[7], array[0], array[1], array[6], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		voxelMesh.AddBlockSide(array[0], array[7], array[6], array[1], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		voxelMesh.AddBlockSide(array[3], array[4], array[5], array[2], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		voxelMesh.AddBlockSide(array[4], array[3], array[2], array[5], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		MemoryPools.poolVector3.Free(array);
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0004712E File Offset: 0x0004532E
	public override Bounds[] GetBounds(BlockValue _blockValue)
	{
		return this.boundsArr;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00048339 File Offset: 0x00046539
	public override byte Rotate(bool _isLeft, int _rotation)
	{
		_rotation = (_rotation + (_isLeft ? -1 : 1) & 3);
		return (byte)_rotation;
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0004834A File Offset: 0x0004654A
	public override BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		_blockValue.rotation = (byte)((int)_blockValue.rotation + _rotCount & 3);
		return _blockValue;
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x00048360 File Offset: 0x00046560
	public static void RenderSpinMesh(VoxelMesh _mesh, Vector3 _drawPos, Vector3[] _vertices, Rect uvTex, byte _sunlight, byte _blocklight, BlockShapeBillboardPlant.RenderData _data)
	{
		float num = _drawPos.y;
		if (_vertices != null)
		{
			num += _data.offsetY;
		}
		float num2 = num + _data.height;
		float num3 = _drawPos.x + 0.5f;
		float num4 = _drawPos.z + 0.5f;
		float num5 = 180f / (float)_data.count;
		num5 += 180f;
		float num6 = 0.5f * _data.scale;
		for (int i = 0; i < _data.count; i++)
		{
			float f = ((float)i * num5 + _data.rotation) * 0.017453292f;
			float num7 = Mathf.Sin(f);
			float num8 = Mathf.Cos(f);
			float num9 = -num6;
			float sideShift = _data.sideShift;
			Vector3 vector;
			vector.x = num3 + num9 * num8 - sideShift * num7;
			vector.y = num;
			vector.z = num4 + num9 * num7 + sideShift * num8;
			num9 = num6;
			Vector3 vector2;
			vector2.x = num3 + num9 * num8 - sideShift * num7;
			vector2.y = num;
			vector2.z = num4 + num9 * num7 + sideShift * num8;
			float num10 = num2;
			num10 += (float)(i % 3) * -0.15f;
			Vector3 v;
			v.x = vector2.x;
			v.y = num10;
			v.z = vector2.z;
			Vector3 vector3;
			vector3.x = vector.x;
			vector3.y = num10;
			vector3.z = vector.z;
			Vector3 vector4;
			vector4.x = num7;
			vector4.y = 0f;
			vector4.z = -num8;
			Vector4 tangent = (vector2 - vector).normalized;
			tangent.w = -1f;
			Vector3 vector5 = -vector4;
			_mesh.AddRectangle(vector, BlockShape.uvZero, vector2, BlockShape.uvRightBot, v, BlockShape.uvOne, vector3, BlockShape.uvLeftTop, vector4, vector4, tangent, uvTex, _sunlight, _blocklight);
			tangent.w = 1f;
			_mesh.AddRectangle(vector, BlockShape.uvZero, vector3, BlockShape.uvLeftTop, v, BlockShape.uvOne, vector2, BlockShape.uvRightBot, vector5, vector5, tangent, uvTex, _sunlight, _blocklight);
		}
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x00048590 File Offset: 0x00046790
	public static void RenderGridMesh(VoxelMesh _mesh, Vector3 _drawPos, Vector3[] _vertices, Rect uvTex, byte _sunlight, byte _blocklight, BlockShapeBillboardPlant.RenderData _data)
	{
		float num = _drawPos.y;
		if (_vertices != null)
		{
			num += _data.offsetY;
		}
		float num2 = num + _data.height - 0.12f;
		float num3 = _drawPos.x + 0.5f;
		float num4 = _drawPos.z + 0.5f;
		float num5 = _data.rotation;
		float num6 = 165f / (float)_data.count2;
		float num7 = _data.sideShift * 2f / ((float)_data.count - 0.99f);
		float num8 = _data.sideShift * 1.85f;
		Vector3 vector;
		vector.y = 0f;
		Vector3 normal;
		normal.y = 0f;
		for (int i = 0; i < _data.count2; i++)
		{
			float num9 = -_data.sideShift;
			float f = num5 * 0.017453292f;
			float num10 = Mathf.Sin(f);
			float num11 = Mathf.Cos(f);
			vector.x = -num11;
			vector.z = -num10;
			for (int j = 0; j < _data.count; j++)
			{
				float num12 = num3 - num8 * num10;
				float num13 = num4 + num8 * num11;
				Vector3 vector2;
				vector2.x = num12 + num9 * num11;
				vector2.y = num;
				vector2.z = num13 + num9 * num10;
				float num14 = num2;
				num14 += (float)(j % 3) * 0.06f;
				float num15 = num9 * 0.7f;
				Vector3 vector3;
				vector3.x = num12 + num15 * num11;
				vector3.y = num14;
				vector3.z = num13 + num15 * num10;
				num12 = num3 + num8 * num10;
				num13 = num4 - num8 * num11;
				Vector3 vector4;
				vector4.x = num12 + num9 * num11;
				vector4.y = num;
				vector4.z = num13 + num9 * num10;
				Vector3 vector5;
				vector5.x = num12 + num15 * num11;
				vector5.y = num14;
				vector5.z = num13 + num15 * num10;
				Vector3 vector6;
				vector6.x = vector.x;
				vector6.y = (num15 - num9) * 2.2f;
				vector6.z = vector.z;
				vector6 *= 1f / vector6.magnitude;
				normal.x = -vector.x;
				normal.z = -vector.z;
				Vector4 tangent = (vector4 - vector2).normalized;
				tangent.w = -1f;
				if ((j & 1) == 0)
				{
					_mesh.AddRectangle(vector2, BlockShape.uvZero, vector4, BlockShape.uvRightBot, vector5, BlockShape.uvOne, vector3, BlockShape.uvLeftTop, vector, vector6, tangent, uvTex, _sunlight, _blocklight);
					tangent.w = 1f;
					_mesh.AddRectangle(vector4, BlockShape.uvRightBot, vector2, BlockShape.uvZero, vector3, BlockShape.uvLeftTop, vector5, BlockShape.uvOne, normal, -vector6, tangent, uvTex, _sunlight, _blocklight);
				}
				else
				{
					_mesh.AddRectangle(vector2, BlockShape.uvRightBot, vector4, BlockShape.uvZero, vector5, BlockShape.uvLeftTop, vector3, BlockShape.uvOne, vector, vector6, tangent, uvTex, _sunlight, _blocklight);
					tangent.w = 1f;
					_mesh.AddRectangle(vector4, BlockShape.uvZero, vector2, BlockShape.uvRightBot, vector3, BlockShape.uvOne, vector5, BlockShape.uvLeftTop, normal, -vector6, tangent, uvTex, _sunlight, _blocklight);
				}
				num9 += num7;
			}
			num5 += num6;
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x000488F4 File Offset: 0x00046AF4
	public static void AddCollider(VoxelMesh _mesh, Vector3 _drawPos, float _height)
	{
		float num = _drawPos.x + 0.5f;
		float num2 = _drawPos.z + 0.5f;
		float y = _drawPos.y;
		float y2 = y + _height;
		Vector3 vector;
		vector.x = num - 0.45f;
		vector.y = y;
		vector.z = num2;
		Vector3 vector2;
		vector2.x = num + 0.45f;
		vector2.y = y;
		vector2.z = num2;
		Vector3 v;
		v.x = vector2.x;
		v.y = y2;
		v.z = num2;
		Vector3 v2;
		v2.x = vector.x;
		v2.y = y2;
		v2.z = num2;
		_mesh.AddRectangleColliderPair(vector, vector2, v, v2);
		vector.x = num;
		vector.z = num2 - 0.45f;
		vector2.x = num;
		vector2.z = num2 + 0.45f;
		v.x = num;
		v.z = vector2.z;
		v2.x = num;
		v2.z = vector.z;
		_mesh.AddRectangleColliderPair(vector, vector2, v, v2);
	}

	// Token: 0x040009C2 RID: 2498
	[PublicizedFrom(EAccessModifier.Private)]
	public const float xzAdd = 0.35f;

	// Token: 0x040009C3 RID: 2499
	[PublicizedFrom(EAccessModifier.Private)]
	public const float yAdd = 0.15f;

	// Token: 0x02000175 RID: 373
	public struct RenderData
	{
		// Token: 0x040009C4 RID: 2500
		public int count;

		// Token: 0x040009C5 RID: 2501
		public int count2;

		// Token: 0x040009C6 RID: 2502
		public float height;

		// Token: 0x040009C7 RID: 2503
		public float rotation;

		// Token: 0x040009C8 RID: 2504
		public float scale;

		// Token: 0x040009C9 RID: 2505
		public float sideShift;

		// Token: 0x040009CA RID: 2506
		public float offsetY;
	}
}
