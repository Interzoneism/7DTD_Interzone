using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000A70 RID: 2672
public class VoxelMeshTerrain : VoxelMesh
{
	// Token: 0x0600517B RID: 20859 RVA: 0x0020B464 File Offset: 0x00209664
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitStatic()
	{
		VoxelMeshTerrain.mainTexPropertyIds = new int[3];
		VoxelMeshTerrain.mainTexPropertyNames = new string[3];
		VoxelMeshTerrain.bumpMapPropertyIds = new int[3];
		VoxelMeshTerrain.bumpMapPropertyNames = new string[3];
		VoxelMeshTerrain.sideTexPropertyIds = new int[3];
		VoxelMeshTerrain.sideTexPropertyNames = new string[3];
		VoxelMeshTerrain.sideBumpMapPropertyIds = new int[3];
		VoxelMeshTerrain.sideBumpMapPropertyNames = new string[3];
		for (int i = 0; i < VoxelMeshTerrain.mainTexPropertyIds.Length; i++)
		{
			VoxelMeshTerrain.mainTexPropertyNames[i] = "_MainTex" + ((i > 0) ? (i + 1).ToString() : "");
			VoxelMeshTerrain.mainTexPropertyIds[i] = Shader.PropertyToID(VoxelMeshTerrain.mainTexPropertyNames[i]);
			VoxelMeshTerrain.bumpMapPropertyNames[i] = "_BumpMap" + ((i > 0) ? (i + 1).ToString() : "");
			VoxelMeshTerrain.bumpMapPropertyIds[i] = Shader.PropertyToID(VoxelMeshTerrain.bumpMapPropertyNames[i]);
			VoxelMeshTerrain.sideTexPropertyNames[i] = "_SideTex" + ((i > 0) ? (i + 1).ToString() : "");
			VoxelMeshTerrain.sideTexPropertyIds[i] = Shader.PropertyToID(VoxelMeshTerrain.sideTexPropertyNames[i]);
			VoxelMeshTerrain.sideBumpMapPropertyNames[i] = "_SideBumpMap" + ((i > 0) ? (i + 1).ToString() : "");
			VoxelMeshTerrain.sideBumpMapPropertyIds[i] = Shader.PropertyToID(VoxelMeshTerrain.sideBumpMapPropertyNames[i]);
		}
		VoxelMeshTerrain.InitMicroSplat();
		VoxelMeshTerrain.isInitStatic = true;
	}

	// Token: 0x0600517C RID: 20860 RVA: 0x0020B5D4 File Offset: 0x002097D4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void InitMicroSplat()
	{
		VoxelMeshTerrain.msPropData = LoadManager.LoadAssetFromAddressables<MicroSplatPropData>("TerrainTextures", "Microsplat/MicroSplatTerrainInGame_propdata.asset", null, null, false, true, false).Asset;
		VoxelMeshTerrain.msPropTex = VoxelMeshTerrain.msPropData.GetTexture();
		VoxelMeshTerrain.msProcData = LoadManager.LoadAssetFromAddressables<MicroSplatProceduralTextureConfig>("TerrainTextures", "Microsplat/MicroSplatTerrainInGame_proceduraltexture.asset", null, null, false, true, false).Asset;
		VoxelMeshTerrain.msProcCurveTex = VoxelMeshTerrain.msProcData.GetCurveTexture();
		VoxelMeshTerrain.msProcParamTex = VoxelMeshTerrain.msProcData.GetParamTexture();
	}

	// Token: 0x0600517D RID: 20861 RVA: 0x0020B64C File Offset: 0x0020984C
	public VoxelMeshTerrain(int _meshIndex, int _minSize = 500) : base(_meshIndex, _minSize, VoxelMesh.CreateFlags.Default)
	{
		this.m_Uvs3 = new ArrayListMP<Vector2>(MemoryPools.poolVector2, _minSize);
		this.m_Uvs4 = new ArrayListMP<Vector2>(MemoryPools.poolVector2, _minSize);
	}

	// Token: 0x0600517E RID: 20862 RVA: 0x0020B69B File Offset: 0x0020989B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int EncodeTexIds(int _mainTexId, int _sideTexId)
	{
		return _mainTexId << 16 | _sideTexId;
	}

	// Token: 0x0600517F RID: 20863 RVA: 0x0020160A File Offset: 0x001FF80A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int DecodeMainTexId(int _fullTexId)
	{
		return _fullTexId >> 16;
	}

	// Token: 0x06005180 RID: 20864 RVA: 0x000407A6 File Offset: 0x0003E9A6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int DecodeSideTexId(int _fullTexId)
	{
		return _fullTexId & 65535;
	}

	// Token: 0x06005181 RID: 20865 RVA: 0x0020B6A4 File Offset: 0x002098A4
	public override Color[] UpdateColors(byte _suncolor, byte _blockcolor)
	{
		float b = (float)_suncolor / 15f;
		float a = (float)_blockcolor / 15f;
		for (int i = 0; i < this.m_ColorVertices.Count; i++)
		{
			Color value = this.m_ColorVertices[i];
			value.b = b;
			value.a = a;
			this.m_ColorVertices[i] = value;
		}
		return this.m_ColorVertices.ToArray();
	}

	// Token: 0x06005182 RID: 20866 RVA: 0x0020B710 File Offset: 0x00209910
	public override void GetColorForTextureId(int _subMeshIdx, ref Transvoxel.BuildVertex _data)
	{
		if (!World.IsSplatMapAvailable || this.IsPreviewVoxelMesh)
		{
			_data.uv = (_data.uv2 = (_data.uv3 = (_data.uv4 = VoxelMeshTerrain.cUvEmpty)));
			_data.color = this.submeshes[_subMeshIdx].GetColorForTextureId(_data.texture);
			return;
		}
		int texId = VoxelMeshTerrain.DecodeMainTexId(_data.texture);
		this.GetColorForTexId(texId, ref _data);
	}

	// Token: 0x06005183 RID: 20867 RVA: 0x0020B788 File Offset: 0x00209988
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetColorForTexId(int _texId, ref Transvoxel.BuildVertex _data)
	{
		_data.uv = (_data.uv2 = (_data.uv3 = (_data.uv4 = VoxelMeshTerrain.cUvEmpty)));
		if (_data.bTopSoil)
		{
			_data.color = VoxelMeshTerrain.cColSplatMap;
			return;
		}
		_data.color = VoxelMeshTerrain.cColUnderTerrain4to10;
		if (_texId <= 33)
		{
			if (_texId <= 2)
			{
				if (_texId == 1)
				{
					_data.uv2 = VoxelMeshTerrain.cUvUnderTerrain0_1;
					return;
				}
				if (_texId == 2)
				{
					_data.color = VoxelMeshTerrain.cColUnderTerrain1;
					return;
				}
			}
			else
			{
				if (_texId == 10)
				{
					_data.uv = VoxelMeshTerrain.cUvUnderTerrain1_0;
					return;
				}
				if (_texId == 11)
				{
					_data.color = VoxelMeshTerrain.cColUnderTerrain2;
					return;
				}
				if (_texId == 33)
				{
					_data.uv = VoxelMeshTerrain.cUvUnderTerrain0_1;
					return;
				}
			}
		}
		else if (_texId <= 300)
		{
			if (_texId == 34)
			{
				_data.color = VoxelMeshTerrain.cColUnderTerrain3;
				return;
			}
			if (_texId == 184)
			{
				_data.uv3 = VoxelMeshTerrain.cUvUnderTerrain1_0;
				return;
			}
			if (_texId == 300)
			{
				_data.uv2 = VoxelMeshTerrain.cUvUnderTerrain1_0;
				return;
			}
		}
		else
		{
			if (_texId == 316)
			{
				_data.uv4 = VoxelMeshTerrain.cUvUnderTerrain1_0;
				return;
			}
			if (_texId == 438)
			{
				_data.uv4 = VoxelMeshTerrain.cUvUnderTerrain0_1;
				return;
			}
			if (_texId == 440)
			{
				_data.uv3 = VoxelMeshTerrain.cUvUnderTerrain0_1;
				return;
			}
		}
		_data.color = VoxelMeshTerrain.cColSplatMap;
	}

	// Token: 0x06005184 RID: 20868 RVA: 0x0020B8D0 File Offset: 0x00209AD0
	public override int FindOrCreateSubMesh(int _t0, int _t1, int _t2)
	{
		this.texIds[0] = _t0;
		this.texIds[1] = ((_t1 != _t0) ? _t1 : -1);
		this.texIds[2] = ((_t2 != _t0 && _t2 != _t1) ? _t2 : -1);
		for (int i = 0; i < this.submeshes.Count; i++)
		{
			if (this.submeshes[i].Contains(this.texIds))
			{
				return i;
			}
		}
		for (int j = 0; j < this.submeshes.Count; j++)
		{
			if (this.submeshes[j].CanAdd(this.texIds))
			{
				return j;
			}
		}
		TerrainSubMesh item = new TerrainSubMesh(this.submeshes, 4096);
		item.Add(this.texIds);
		this.submeshes.Add(item);
		return this.submeshes.Count - 1;
	}

	// Token: 0x06005185 RID: 20869 RVA: 0x0020B9AC File Offset: 0x00209BAC
	public override void AddIndices(int _i0, int _i1, int _i2, int _submesh)
	{
		ArrayListMP<int> triangles = this.submeshes[_submesh].triangles;
		int num = triangles.Alloc(3);
		triangles.Items[num] = _i0;
		triangles.Items[num + 1] = _i1;
		triangles.Items[num + 2] = _i2;
	}

	// Token: 0x06005186 RID: 20870 RVA: 0x0020B9F1 File Offset: 0x00209BF1
	public override void ClearMesh()
	{
		base.ClearMesh();
		this.submeshes.Clear();
	}

	// Token: 0x06005187 RID: 20871 RVA: 0x0020BA04 File Offset: 0x00209C04
	public override void Finished()
	{
		this.m_Triangles = 0;
		for (int i = 0; i < this.submeshes.Count; i++)
		{
			this.m_Triangles += this.submeshes[i].triangles.Count / 3;
		}
	}

	// Token: 0x06005188 RID: 20872 RVA: 0x0020BA54 File Offset: 0x00209C54
	[PublicizedFrom(EAccessModifier.Private)]
	public void ConfigureTerrainMaterial(Material mat, ChunkProviderGenerateWorldFromRaw cpr)
	{
		mat.SetTexture("_CustomControl0", cpr.splats[0]);
		mat.SetTexture("_CustomControl1", cpr.splats[1]);
		if (VoxelMeshTerrain.msPropTex)
		{
			mat.SetTexture("_PerTexProps", VoxelMeshTerrain.msPropTex);
		}
		if (VoxelMeshTerrain.msProcData)
		{
			mat.SetTexture("_ProcTexCurves", VoxelMeshTerrain.msProcCurveTex);
			mat.SetTexture("_ProcTexParams", VoxelMeshTerrain.msProcParamTex);
			mat.SetInt("_PCLayerCount", VoxelMeshTerrain.msProcData.layers.Count);
			if (cpr.procBiomeMask1 != null && mat.HasProperty("_ProcTexBiomeMask"))
			{
				mat.SetTexture("_ProcTexBiomeMask", cpr.procBiomeMask1);
			}
			if (cpr.procBiomeMask2 != null && mat.HasProperty("_ProcTexBiomeMask2"))
			{
				mat.SetTexture("_ProcTexBiomeMask2", cpr.procBiomeMask2);
			}
		}
		Vector2i worldSize = cpr.GetWorldSize();
		mat.SetVector("_WorldDim", new Vector4((float)worldSize.x, (float)worldSize.y));
	}

	// Token: 0x06005189 RID: 20873 RVA: 0x0020BB6C File Offset: 0x00209D6C
	public void ApplyMaterials(MeshRenderer _mr, TextureAtlasTerrain _ta, float _tilingFac, bool _bDistant = false)
	{
		if (!VoxelMeshTerrain.isInitStatic)
		{
			this.InitStatic();
		}
		if (World.IsSplatMapAvailable && !this.IsPreviewVoxelMesh)
		{
			MeshDescription meshDescription = MeshDescription.meshes[5];
			ChunkProviderGenerateWorldFromRaw cpr;
			if (GameManager.Instance != null && GameManager.Instance.World != null && (cpr = (GameManager.Instance.World.ChunkCache.ChunkProvider as ChunkProviderGenerateWorldFromRaw)) != null)
			{
				this.ConfigureTerrainMaterial(meshDescription.material, cpr);
				this.ConfigureTerrainMaterial(meshDescription.materialDistant, cpr);
			}
			Material[] array = _mr.sharedMaterials;
			if (this.submeshes.Count != array.Length)
			{
				array = new Material[this.submeshes.Count];
			}
			for (int i = 0; i < this.submeshes.Count; i++)
			{
				if (VoxelMeshTerrain.DecodeMainTexId(this.submeshes[i].textureIds.Data[0]) >= 5000)
				{
					array[i] = new Material(_bDistant ? MeshDescription.meshes[1].materialDistant : MeshDescription.meshes[1].material);
				}
				else
				{
					array[i] = meshDescription.material;
				}
			}
			_mr.sharedMaterials = array;
			return;
		}
		Material[] array2 = _mr.sharedMaterials;
		Utils.CleanupMaterials<Material[]>(array2);
		if (this.submeshes.Count != array2.Length)
		{
			array2 = new Material[this.submeshes.Count];
		}
		for (int j = 0; j < this.submeshes.Count; j++)
		{
			if (VoxelMeshTerrain.DecodeMainTexId(this.submeshes[j].textureIds.Data[0]) >= 5000)
			{
				array2[j] = new Material(_bDistant ? MeshDescription.meshes[1].materialDistant : MeshDescription.meshes[1].material);
			}
			else if (this.IsPreviewVoxelMesh && World.IsSplatMapAvailable)
			{
				array2[j] = new Material(_bDistant ? MeshDescription.meshes[5].prefabTerrainMaterialDistant : MeshDescription.meshes[5].prefabPreviewMaterial);
			}
			else
			{
				array2[j] = new Material(_bDistant ? MeshDescription.meshes[5].materialDistant : MeshDescription.meshes[5].material);
			}
			Utils.MarkMaterialAsSafeForManualCleanup(array2[j]);
		}
		for (int k = 0; k < this.submeshes.Count; k++)
		{
			for (int l = 0; l < this.submeshes[k].textureIds.Size; l++)
			{
				if (this.submeshes[k].textureIds.DataAvail[l])
				{
					int fullTexId = this.submeshes[k].textureIds.Data[l];
					int num = VoxelMeshTerrain.DecodeMainTexId(fullTexId);
					int num2 = VoxelMeshTerrain.DecodeSideTexId(fullTexId);
					if (num < 5000)
					{
						if (num < 0 || num >= _ta.uvMapping.Length)
						{
							Log.Error(string.Format("Error in terrain mesh generation, texture id {0} not found", num));
							return;
						}
						Vector2 value = new Vector2(1f / ((float)_ta.uvMapping[num].blockW * _tilingFac), 1f / ((float)_ta.uvMapping[num].blockH * _tilingFac));
						array2[k].SetTexture(VoxelMeshTerrain.mainTexPropertyIds[l], _ta.diffuse[num]);
						array2[k].SetTextureScale(VoxelMeshTerrain.mainTexPropertyNames[l], value);
						array2[k].SetTexture(VoxelMeshTerrain.bumpMapPropertyIds[l], _ta.normal[num]);
						array2[k].SetTextureScale(VoxelMeshTerrain.bumpMapPropertyNames[l], value);
						value = new Vector2(1f / ((float)_ta.uvMapping[num2].blockW * _tilingFac), 1f / ((float)_ta.uvMapping[num2].blockH * _tilingFac));
						array2[k].SetTexture(VoxelMeshTerrain.sideTexPropertyIds[l], _ta.diffuse[num2]);
						array2[k].SetTextureScale(VoxelMeshTerrain.sideTexPropertyNames[l], value);
						array2[k].SetTexture(VoxelMeshTerrain.sideBumpMapPropertyIds[l], _ta.normal[num2]);
						array2[k].SetTextureScale(VoxelMeshTerrain.sideBumpMapPropertyNames[l], value);
					}
				}
			}
		}
		_mr.sharedMaterials = array2;
	}

	// Token: 0x0600518A RID: 20874 RVA: 0x0020BFA0 File Offset: 0x0020A1A0
	public override int CopyToMesh(MeshFilter[] _mf, MeshRenderer[] _mr, int _lodLevel, Action _onCopyComplete = null)
	{
		MeshFilter meshFilter = _mf[0];
		Mesh mesh = meshFilter.sharedMesh;
		int count = this.m_Vertices.Count;
		if (count == 0)
		{
			if (mesh)
			{
				meshFilter.sharedMesh = null;
				VoxelMesh.AddPooledMesh(mesh);
			}
			if (_onCopyComplete != null)
			{
				_onCopyComplete();
			}
			return 0;
		}
		if (!mesh)
		{
			mesh = VoxelMesh.GetPooledMesh();
			meshFilter.sharedMesh = mesh;
		}
		else
		{
			mesh.Clear(false);
		}
		MeshRenderer mr = _mr[0];
		TextureAtlasTerrain ta = (TextureAtlasTerrain)MeshDescription.meshes[5].textureAtlas;
		this.ApplyMaterials(mr, ta, 1f, false);
		if (count != this.m_ColorVertices.Count)
		{
			Log.Error("ERROR: VMT.mesh[{2}].Vertices.Count ({0}) != VMT.mesh[{2}].ColorSides.Count ({1})", new object[]
			{
				count,
				this.m_ColorVertices.Count,
				this.meshIndex
			});
		}
		if (count != this.m_Uvs.Count)
		{
			Log.Error("ERROR: VMT.mesh[{2}].Vertices.Count ({0}) != VMT.mesh[{2}].Uvs.Count ({1})", new object[]
			{
				count,
				this.m_Uvs.Count,
				this.meshIndex
			});
		}
		MeshUnsafeCopyHelper.CopyVertices(this.m_Vertices, mesh);
		MeshUnsafeCopyHelper.CopyUV(this.m_Uvs, mesh);
		if (this.UvsCrack != null)
		{
			MeshUnsafeCopyHelper.CopyUV2(this.UvsCrack, mesh);
		}
		if (this.m_Uvs3 != null && this.m_Uvs3.Items != null)
		{
			MeshUnsafeCopyHelper.CopyUV3(this.m_Uvs3, mesh);
		}
		if (this.m_Uvs4 != null && this.m_Uvs4.Items != null)
		{
			MeshUnsafeCopyHelper.CopyUV4(this.m_Uvs4, mesh);
		}
		MeshUnsafeCopyHelper.CopyColors(this.m_ColorVertices, mesh);
		mesh.subMeshCount = this.submeshes.Count;
		for (int i = 0; i < this.submeshes.Count; i++)
		{
			MeshUnsafeCopyHelper.CopyTriangles(this.submeshes[i].triangles, mesh, i);
		}
		if (this.m_Normals.Count == 0)
		{
			mesh.RecalculateNormals();
		}
		else
		{
			if (count != this.m_Normals.Count)
			{
				Log.Error("ERROR: Vertices.Count ({0}) != Normals.Count ({1})", new object[]
				{
					count,
					this.m_Normals.Count,
					this.CurTriangleIndex
				});
			}
			MeshUnsafeCopyHelper.CopyNormals(this.m_Normals, mesh);
		}
		mesh.RecalculateTangents();
		mesh.RecalculateUVDistributionMetrics(1E-09f);
		GameUtils.SetMeshVertexAttributes(mesh, false);
		mesh.UploadMeshData(false);
		if (_onCopyComplete != null)
		{
			_onCopyComplete();
		}
		return this.m_Triangles;
	}

	// Token: 0x0600518B RID: 20875 RVA: 0x0020C218 File Offset: 0x0020A418
	[PublicizedFrom(EAccessModifier.Private)]
	public string MakeTransformPathName(Transform _t, int _parentMax = 1)
	{
		string text = _t.name;
		for (int i = 0; i < _parentMax; i++)
		{
			_t = _t.parent;
			if (!_t)
			{
				break;
			}
			text = _t.name + "," + text;
		}
		return text;
	}

	// Token: 0x0600518C RID: 20876 RVA: 0x0020C25C File Offset: 0x0020A45C
	public override int CopyToColliders(int _clrIdx, MeshCollider _meshCollider, out Mesh mesh)
	{
		if (this.m_Vertices.Count == 0)
		{
			GameManager.Instance.World.m_ChunkManager.BakeDestroyCancel(_meshCollider);
			mesh = null;
			return 0;
		}
		mesh = base.ResetMesh(_meshCollider);
		int num = 0;
		mesh.subMeshCount = this.submeshes.Count;
		MeshUnsafeCopyHelper.CopyVertices(this.m_Vertices, mesh);
		for (int i = 0; i < this.submeshes.Count; i++)
		{
			MeshUnsafeCopyHelper.CopyTriangles(this.submeshes[i].triangles, mesh, i);
			num += this.submeshes[i].triangles.Count / 3;
		}
		return num;
	}

	// Token: 0x04003E81 RID: 16001
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool isInitStatic;

	// Token: 0x04003E82 RID: 16002
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] mainTexPropertyIds;

	// Token: 0x04003E83 RID: 16003
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] mainTexPropertyNames;

	// Token: 0x04003E84 RID: 16004
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] bumpMapPropertyIds;

	// Token: 0x04003E85 RID: 16005
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] bumpMapPropertyNames;

	// Token: 0x04003E86 RID: 16006
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] sideTexPropertyIds;

	// Token: 0x04003E87 RID: 16007
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] sideTexPropertyNames;

	// Token: 0x04003E88 RID: 16008
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] sideBumpMapPropertyIds;

	// Token: 0x04003E89 RID: 16009
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] sideBumpMapPropertyNames;

	// Token: 0x04003E8A RID: 16010
	[PublicizedFrom(EAccessModifier.Private)]
	public static MicroSplatPropData msPropData;

	// Token: 0x04003E8B RID: 16011
	[PublicizedFrom(EAccessModifier.Private)]
	public static Texture2D msPropTex;

	// Token: 0x04003E8C RID: 16012
	[PublicizedFrom(EAccessModifier.Private)]
	public static MicroSplatProceduralTextureConfig msProcData;

	// Token: 0x04003E8D RID: 16013
	[PublicizedFrom(EAccessModifier.Private)]
	public static Texture2D msProcCurveTex;

	// Token: 0x04003E8E RID: 16014
	[PublicizedFrom(EAccessModifier.Private)]
	public static Texture2D msProcParamTex;

	// Token: 0x04003E8F RID: 16015
	public bool IsPreviewVoxelMesh;

	// Token: 0x04003E90 RID: 16016
	public List<TerrainSubMesh> submeshes = new List<TerrainSubMesh>();

	// Token: 0x04003E91 RID: 16017
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color cColSplatMap = new Color(0f, 0f, 0f, 0f);

	// Token: 0x04003E92 RID: 16018
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color cColUnderTerrain4to10 = new Color(0f, 0f, 0f, 1f);

	// Token: 0x04003E93 RID: 16019
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color cColUnderTerrain1 = new Color(1f, 0f, 0f, 1f);

	// Token: 0x04003E94 RID: 16020
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color cColUnderTerrain2 = new Color(0f, 1f, 0f, 1f);

	// Token: 0x04003E95 RID: 16021
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color cColUnderTerrain3 = new Color(0f, 0f, 1f, 1f);

	// Token: 0x04003E96 RID: 16022
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector2 cUvEmpty = Vector2.zero;

	// Token: 0x04003E97 RID: 16023
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector2 cUvUnderTerrain1_0 = new Vector2(1f, 0f);

	// Token: 0x04003E98 RID: 16024
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector2 cUvUnderTerrain0_1 = new Vector2(0f, 1f);

	// Token: 0x04003E99 RID: 16025
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] texIds = new int[3];
}
