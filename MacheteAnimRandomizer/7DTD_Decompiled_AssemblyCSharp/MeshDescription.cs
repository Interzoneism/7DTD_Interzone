using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x020009FB RID: 2555
[Serializable]
public class MeshDescription
{
	// Token: 0x06004E45 RID: 20037 RVA: 0x001EE204 File Offset: 0x001EC404
	public IEnumerator Init(int _idx, TextureAtlas _ta)
	{
		this.textureAtlas = _ta;
		if (!GameManager.IsDedicatedServer)
		{
			if (this.UseSplatmap(_idx))
			{
				this.materials = new Material[1];
				LoadManager.AddressableRequestTask<Material> assetRequestTask = LoadManager.LoadAssetFromAddressables<Material>("TerrainTextures", "Microsplat/MicroSplatTerrainInGame.mat", null, null, false, ThreadManager.IsInSyncCoroutine, false);
				yield return assetRequestTask;
				this.materials[0] = UnityEngine.Object.Instantiate<Material>(assetRequestTask.Asset);
				this.materials[0].name = "Near Terrain";
				this.materials[0].SetFloat("_ShaderMode", 2f);
				this.materialDistant = new Material(this.materials[0]);
				this.materialDistant.SetFloat("_ShaderMode", 1f);
				this.materialDistant.name = "Distant Terrain";
				this.ReloadTextureArrays(true);
				assetRequestTask.Release();
				if (_idx == 5)
				{
					yield return this.setupPrefabTerrainMaterials(_idx, _ta);
				}
				assetRequestTask = null;
			}
			else
			{
				AssetReference secondaryShader = this.SecondaryShader;
				if (secondaryShader != null && secondaryShader.RuntimeKeyIsValid())
				{
					this.materials = new Material[2];
				}
				else
				{
					this.materials = new Material[1];
				}
				if (_ta == null)
				{
					yield break;
				}
				Material material = this.BaseMaterial;
				if (!material)
				{
					if (this.PrimaryShader == null)
					{
						Log.Out("Null PrimaryShader for " + this.Name);
					}
					Shader shader = DataLoader.LoadAsset<Shader>(this.PrimaryShader, false);
					if (shader == null)
					{
						string str = "Can't find shader: ";
						object runtimeKey = this.PrimaryShader.RuntimeKey;
						Log.Error(str + ((runtimeKey != null) ? runtimeKey.ToString() : null));
					}
					material = new Material(shader);
				}
				this.materials[0] = material;
				if (_idx == 3)
				{
					material.SetTexture("_Albedo", _ta.diffuseTexture);
					material.SetTexture("_Normal", _ta.normalTexture);
					material.SetTexture("_Gloss_AO_SSS", _ta.specularTexture);
				}
				else
				{
					if (_idx != 5 || _ta.diffuseTexture is Texture2D)
					{
						material.SetTexture("_MainTex", _ta.diffuseTexture);
					}
					if (_idx != 5 || _ta.normalTexture is Texture2D)
					{
						material.SetTexture("_BumpMap", _ta.normalTexture);
					}
					material.SetTexture("_MetallicGlossMap", _ta.specularTexture);
					material.SetTexture("_OcclusionMap", _ta.occlusionTexture);
					material.SetTexture("_MaskTex", _ta.maskTexture);
					material.SetTexture("_MaskBumpMapTex", _ta.maskNormalTexture);
					material.SetTexture("_EmissionMap", _ta.emissionTexture);
				}
				if (this.BlendMode != MeshDescription.EnumRenderMode.Default)
				{
					MeshDescription.SetupMaterialWithBlendMode(material, this.BlendMode);
				}
				AssetReference distantShader = this.DistantShader;
				if (distantShader != null && distantShader.RuntimeKeyIsValid())
				{
					this.materialDistant = new Material(DataLoader.LoadAsset<Shader>(this.DistantShader, false));
					if (_idx != 5 || _ta.diffuseTexture is Texture2D)
					{
						this.materialDistant.SetTexture("_MainTex", _ta.diffuseTexture);
					}
					if (_idx != 5 || _ta.normalTexture is Texture2D)
					{
						this.materialDistant.SetTexture("_BumpMap", _ta.normalTexture);
					}
					this.materialDistant.SetTexture("_MetallicGlossMap", _ta.specularTexture);
					this.materialDistant.SetTexture("_OcclusionMap", _ta.occlusionTexture);
					this.materialDistant.SetTexture("_MaskTex", _ta.maskTexture);
					this.materialDistant.SetTexture("_MaskBumpMapTex", _ta.maskNormalTexture);
					this.materialDistant.SetTexture("_EmissionMap", _ta.emissionTexture);
					if (this.BlendMode != MeshDescription.EnumRenderMode.Default)
					{
						MeshDescription.SetupMaterialWithBlendMode(this.materialDistant, this.BlendMode);
					}
				}
				AssetReference secondaryShader2 = this.SecondaryShader;
				if (secondaryShader2 != null && secondaryShader2.RuntimeKeyIsValid())
				{
					Shader shader2 = DataLoader.LoadAsset<Shader>(this.SecondaryShader, false);
					if (shader2 == null)
					{
						string str2 = "Can't find secondary shader: ";
						object runtimeKey2 = this.SecondaryShader.RuntimeKey;
						Log.Error(str2 + ((runtimeKey2 != null) ? runtimeKey2.ToString() : null));
					}
					this.materials[1] = new Material(shader2);
					this.materials[1].CopyPropertiesFromMaterial(this.materials[0]);
				}
			}
		}
		yield break;
	}

	// Token: 0x06004E46 RID: 20038 RVA: 0x001EE221 File Offset: 0x001EC421
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setupPrefabTerrainMaterials(int _idx, TextureAtlas _ta)
	{
		yield return null;
		AssetReference secondaryShader = this.SecondaryShader;
		if (secondaryShader != null && secondaryShader.RuntimeKeyIsValid())
		{
			this.prefabTerrainMaterials = new Material[2];
		}
		else
		{
			this.prefabTerrainMaterials = new Material[1];
		}
		if (_ta == null)
		{
			yield break;
		}
		Material material = this.BaseMaterial;
		if (!material)
		{
			Shader shader = DataLoader.LoadAsset<Shader>(this.PrimaryShader, false);
			if (shader == null)
			{
				string str = "Can't find shader: ";
				object runtimeKey = this.PrimaryShader.RuntimeKey;
				Log.Error(str + ((runtimeKey != null) ? runtimeKey.ToString() : null));
			}
			material = new Material(shader);
		}
		this.prefabTerrainMaterials[0] = material;
		if (_idx != 5 || _ta.diffuseTexture is Texture2D)
		{
			material.SetTexture("_MainTex", _ta.diffuseTexture);
		}
		if (_idx != 5 || _ta.normalTexture is Texture2D)
		{
			material.SetTexture("_BumpMap", _ta.normalTexture);
		}
		material.SetTexture("_MetallicGlossMap", _ta.specularTexture);
		material.SetTexture("_OcclusionMap", _ta.occlusionTexture);
		material.SetTexture("_MaskTex", _ta.maskTexture);
		material.SetTexture("_MaskBumpMapTex", _ta.maskNormalTexture);
		material.SetTexture("_EmissionMap", _ta.emissionTexture);
		if (this.BlendMode != MeshDescription.EnumRenderMode.Default)
		{
			MeshDescription.SetupMaterialWithBlendMode(material, this.BlendMode);
		}
		AssetReference distantShader = this.DistantShader;
		if (distantShader != null && distantShader.RuntimeKeyIsValid())
		{
			this.materialDistant = new Material(DataLoader.LoadAsset<Shader>(this.DistantShader, false));
			if (_idx != 5 || _ta.diffuseTexture is Texture2D)
			{
				this.materialDistant.SetTexture("_MainTex", _ta.diffuseTexture);
			}
			if (_idx != 5 || _ta.normalTexture is Texture2D)
			{
				this.materialDistant.SetTexture("_BumpMap", _ta.normalTexture);
			}
			this.materialDistant.SetTexture("_MetallicGlossMap", _ta.specularTexture);
			this.materialDistant.SetTexture("_OcclusionMap", _ta.occlusionTexture);
			this.materialDistant.SetTexture("_MaskTex", _ta.maskTexture);
			this.materialDistant.SetTexture("_MaskBumpMapTex", _ta.maskNormalTexture);
			this.materialDistant.SetTexture("_EmissionMap", _ta.emissionTexture);
			if (this.BlendMode != MeshDescription.EnumRenderMode.Default)
			{
				MeshDescription.SetupMaterialWithBlendMode(this.materialDistant, this.BlendMode);
			}
		}
		AssetReference secondaryShader2 = this.SecondaryShader;
		if (secondaryShader2 != null && secondaryShader2.RuntimeKeyIsValid())
		{
			Shader shader2 = DataLoader.LoadAsset<Shader>(this.SecondaryShader, false);
			if (shader2 == null)
			{
				string str2 = "Can't find secondary shader: ";
				object runtimeKey2 = this.SecondaryShader.RuntimeKey;
				Log.Error(str2 + ((runtimeKey2 != null) ? runtimeKey2.ToString() : null));
			}
			this.prefabTerrainMaterials[1] = new Material(shader2);
			this.prefabTerrainMaterials[1].CopyPropertiesFromMaterial(this.materials[0]);
		}
		yield break;
	}

	// Token: 0x06004E47 RID: 20039 RVA: 0x001EE240 File Offset: 0x001EC440
	public void ReloadTextureArrays(bool _isSplatmap)
	{
		if (_isSplatmap)
		{
			if (this.material != null)
			{
				this.material.SetTexture("_Diffuse", this.TexDiffuse);
				this.material.SetTexture("_NormalSAO", this.TexNormal);
				this.material.SetTexture("_SmoothAO", this.TexSpecular);
				string str = "Set Microsplat diffuse: ";
				Texture texDiffuse = this.TexDiffuse;
				Log.Out(str + ((texDiffuse != null) ? texDiffuse.ToString() : null));
				string str2 = "Set Microsplat normals: ";
				Texture texNormal = this.TexNormal;
				Log.Out(str2 + ((texNormal != null) ? texNormal.ToString() : null));
				string str3 = "Set Microsplat smooth:  ";
				Texture texSpecular = this.TexSpecular;
				Log.Out(str3 + ((texSpecular != null) ? texSpecular.ToString() : null));
			}
			if (this.materialDistant != null)
			{
				this.materialDistant.SetTexture("_Diffuse", this.TexDiffuse);
				this.materialDistant.SetTexture("_NormalSAO", this.TexNormal);
				this.materialDistant.SetTexture("_SmoothAO", this.TexSpecular);
				return;
			}
		}
		else if (this.bTextureArray && this.materials != null && this.materials.Length != 0 && this.materials[0] != null)
		{
			this.materials[0].mainTexture = this.textureAtlas.diffuseTexture;
			this.materials[0].SetTexture("_BumpMap", this.textureAtlas.normalTexture);
			this.materials[0].SetTexture("_MetallicGlossMap", this.textureAtlas.specularTexture);
			this.materials[0].SetTexture("_OcclusionMap", this.textureAtlas.occlusionTexture);
			this.materials[0].SetTexture("_MaskTex", this.textureAtlas.maskTexture);
			this.materials[0].SetTexture("_MaskBumpMapTex", this.textureAtlas.maskNormalTexture);
			this.materials[0].SetTexture("_EmissionMap", this.textureAtlas.emissionTexture);
			if (this.materialDistant != null)
			{
				this.materialDistant.mainTexture = this.textureAtlas.diffuseTexture;
				this.materialDistant.SetTexture("_BumpMap", this.textureAtlas.normalTexture);
				this.materialDistant.SetTexture("_MetallicGlossMap", this.textureAtlas.specularTexture);
				this.materialDistant.SetTexture("_OcclusionMap", this.textureAtlas.occlusionTexture);
				this.materialDistant.SetTexture("_MaskTex", this.textureAtlas.maskTexture);
				this.materialDistant.SetTexture("_MaskBumpMapTex", this.textureAtlas.maskNormalTexture);
				this.materialDistant.SetTexture("_EmissionMap", this.textureAtlas.emissionTexture);
			}
			if (this.materials.Length > 1 && this.materials[1] != null)
			{
				this.materials[1].CopyPropertiesFromMaterial(this.materials[0]);
			}
		}
	}

	// Token: 0x17000800 RID: 2048
	// (get) Token: 0x06004E48 RID: 20040 RVA: 0x001EE551 File Offset: 0x001EC751
	// (set) Token: 0x06004E49 RID: 20041 RVA: 0x001EE565 File Offset: 0x001EC765
	public Material material
	{
		get
		{
			if (this.materials != null)
			{
				return this.materials[0];
			}
			return null;
		}
		set
		{
			if (this.materials != null)
			{
				this.materials[0] = value;
			}
		}
	}

	// Token: 0x17000801 RID: 2049
	// (get) Token: 0x06004E4A RID: 20042 RVA: 0x001EE578 File Offset: 0x001EC778
	// (set) Token: 0x06004E4B RID: 20043 RVA: 0x001EE58C File Offset: 0x001EC78C
	public Material prefabPreviewMaterial
	{
		get
		{
			if (this.prefabTerrainMaterials != null)
			{
				return this.prefabTerrainMaterials[0];
			}
			return null;
		}
		set
		{
			if (this.prefabTerrainMaterials != null)
			{
				this.prefabTerrainMaterials[0] = value;
			}
		}
	}

	// Token: 0x06004E4C RID: 20044 RVA: 0x001EE59F File Offset: 0x001EC79F
	public bool IsSplatmap(int _index)
	{
		return _index == 5;
	}

	// Token: 0x06004E4D RID: 20045 RVA: 0x001EE5A5 File Offset: 0x001EC7A5
	public bool UseSplatmap(int _index)
	{
		return GameManager.IsSplatMapAvailable() && this.IsSplatmap(_index);
	}

	// Token: 0x06004E4E RID: 20046 RVA: 0x001EE5B8 File Offset: 0x001EC7B8
	public static void SetDebugStabilityShader(bool _bOn)
	{
		if (_bOn)
		{
			Shader shader = Shader.Find("Game/Debug/Stability");
			foreach (MeshDescription meshDescription in MeshDescription.meshes)
			{
				if (meshDescription.bUseDebugStabilityShader)
				{
					meshDescription.materials[0].shader = shader;
				}
			}
			using (LinkedList<Chunk>.Enumerator enumerator = GameManager.Instance.World.ChunkCache.GetChunkArray().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chunk chunk = enumerator.Current;
					chunk.NeedsRegeneration = true;
				}
				goto IL_BF;
			}
		}
		foreach (MeshDescription meshDescription2 in MeshDescription.meshes)
		{
			if (meshDescription2.bUseDebugStabilityShader)
			{
				meshDescription2.materials[0].shader = DataLoader.LoadAsset<Shader>(meshDescription2.PrimaryShader, false);
			}
		}
		IL_BF:
		MeshDescription.bDebugStability = _bOn;
		Camera main = Camera.main;
		if (main)
		{
			LightViewer component = main.GetComponent<LightViewer>();
			if (component != null)
			{
				if (MeshDescription.bDebugStability)
				{
					component.TurnOffAllLights();
					return;
				}
				component.TurnOnAllLights();
			}
		}
	}

	// Token: 0x06004E4F RID: 20047 RVA: 0x001EE6D0 File Offset: 0x001EC8D0
	public static void SetGrassQuality()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (MeshDescription.meshes != null && 3 < MeshDescription.meshes.Length)
		{
			MeshDescription.GrassQualityPlanes = 0;
			float value = 45f;
			switch (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxGrassDistance))
			{
			case 1:
				value = 66f;
				break;
			case 2:
				MeshDescription.GrassQualityPlanes = 1;
				value = 102f;
				break;
			case 3:
				MeshDescription.GrassQualityPlanes = 1;
				value = 123f;
				break;
			}
			MeshDescription.meshes[3].material.SetFloat("_FadeDistance", value);
		}
	}

	// Token: 0x06004E50 RID: 20048 RVA: 0x001EE75C File Offset: 0x001EC95C
	public static void SetWaterQuality()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (MeshDescription.meshes != null && 1 < MeshDescription.meshes.Length)
		{
			Material material = MeshDescription.meshes[1].materials[0];
			int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxWaterQuality);
			if (@int == 0)
			{
				material.shader = GlobalAssets.FindShader("Game/Water Distant Surface");
				material.SetFloat("_MinAlpha", 0f);
				return;
			}
			if (@int != 1)
			{
			}
			material.shader = GlobalAssets.FindShader("Game/Water Surface");
		}
	}

	// Token: 0x06004E51 RID: 20049 RVA: 0x001EE7D4 File Offset: 0x001EC9D4
	public static void Cleanup()
	{
		foreach (MeshDescription meshDescription in MeshDescription.meshes)
		{
			meshDescription.textureAtlas.Cleanup();
			meshDescription.CleanupMats();
		}
		MeshDescription.meshes = new MeshDescription[0];
	}

	// Token: 0x06004E52 RID: 20050 RVA: 0x001EE814 File Offset: 0x001ECA14
	[PublicizedFrom(EAccessModifier.Private)]
	public void CleanupMats()
	{
		if (this.materials != null)
		{
			for (int i = 0; i < this.materials.Length; i++)
			{
				Material material = this.materials[i];
				if (material && material != this.BaseMaterial)
				{
					UnityEngine.Object.Destroy(material);
					this.materials[i] = null;
				}
			}
		}
		UnityEngine.Object.Destroy(this.materialDistant);
	}

	// Token: 0x06004E53 RID: 20051 RVA: 0x001EE875 File Offset: 0x001ECA75
	public MeshDescription()
	{
	}

	// Token: 0x06004E54 RID: 20052 RVA: 0x001EE894 File Offset: 0x001ECA94
	public MeshDescription(MeshDescription other)
	{
		this.Name = other.Name;
		this.Tag = other.Tag;
		this.meshType = other.meshType;
		this.bCastShadows = other.bCastShadows;
		this.bReceiveShadows = other.bReceiveShadows;
		this.bHasLODs = other.bHasLODs;
		this.bUseDebugStabilityShader = other.bUseDebugStabilityShader;
		this.bTerrain = other.bTerrain;
		this.bTextureArray = other.bTextureArray;
		this.bSpecularIsBlack = other.bSpecularIsBlack;
		this.CreateTextureAtlas = other.CreateTextureAtlas;
		this.CreateSpecularMap = other.CreateSpecularMap;
		this.CreateNormalMap = other.CreateNormalMap;
		this.CreateEmissionMap = other.CreateEmissionMap;
		this.CreateHeightMap = other.CreateHeightMap;
		this.CreateOcclusionMap = other.CreateOcclusionMap;
		this.MeshLayerName = other.MeshLayerName;
		this.ColliderLayerName = other.ColliderLayerName;
		this.PrimaryShader = new AssetReference(other.PrimaryShader.AssetGUID);
		this.SecondaryShader = new AssetReference(other.SecondaryShader.AssetGUID);
		this.DistantShader = new AssetReference(other.DistantShader.AssetGUID);
		this.BlendMode = other.BlendMode;
		this.BaseMaterial = other.BaseMaterial;
		this.TextureAtlasClass = other.TextureAtlasClass;
		this.TexDiffuse = other.TexDiffuse;
		this.TexNormal = other.TexNormal;
		this.TexSpecular = other.TexSpecular;
		this.TexEmission = other.TexEmission;
		this.TexHeight = other.TexHeight;
		this.TexOcclusion = other.TexOcclusion;
		this.TexMask = other.TexMask;
		this.TexMaskNormal = other.TexMaskNormal;
		this.MetaData = other.MetaData;
	}

	// Token: 0x06004E55 RID: 20053 RVA: 0x001EEA68 File Offset: 0x001ECC68
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetupMaterialWithBlendMode(Material material, MeshDescription.EnumRenderMode blendMode)
	{
		material.SetFloat("_Mode", (float)blendMode);
		switch (blendMode)
		{
		case MeshDescription.EnumRenderMode.Opaque:
			material.SetInt("_SrcBlend", 1);
			material.SetInt("_DstBlend", 0);
			material.SetInt("_ZWrite", 1);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = -1;
			break;
		case MeshDescription.EnumRenderMode.Cutout:
			material.SetInt("_SrcBlend", 1);
			material.SetInt("_DstBlend", 0);
			material.SetInt("_ZWrite", 1);
			material.EnableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 2450;
			break;
		case MeshDescription.EnumRenderMode.Fade:
			material.SetInt("_SrcBlend", 5);
			material.SetInt("_DstBlend", 10);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		case MeshDescription.EnumRenderMode.Transparent:
			material.SetOverrideTag("RenderType", "Transparent");
			material.SetInt("_SrcBlend", 1);
			material.SetInt("_DstBlend", 10);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		}
		MeshDescription.SetMaterialKeywords(material);
	}

	// Token: 0x06004E56 RID: 20054 RVA: 0x001EEC00 File Offset: 0x001ECE00
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetMaterialKeywords(Material material)
	{
		MeshDescription.SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
		MeshDescription.SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
		int nameID = Shader.PropertyToID("_DetailAlbedoMap");
		int nameID2 = Shader.PropertyToID("_DetailNormalMap");
		MeshDescription.SetKeyword(material, "_DETAIL_MULX2", (material.HasProperty(nameID) && material.GetTexture(nameID)) || (material.HasProperty(nameID2) && material.GetTexture(nameID2)));
	}

	// Token: 0x06004E57 RID: 20055 RVA: 0x001EECAB File Offset: 0x001ECEAB
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetKeyword(Material m, string keyword, bool state)
	{
		if (state)
		{
			m.EnableKeyword(keyword);
			return;
		}
		m.DisableKeyword(keyword);
	}

	// Token: 0x06004E58 RID: 20056 RVA: 0x001EECC0 File Offset: 0x001ECEC0
	public static Material GetOpaqueMaterial()
	{
		if (MeshDescription.meshes.Length == 0)
		{
			return new Material(Shader.Find("Diffuse"));
		}
		MeshDescription meshDescription = MeshDescription.meshes[0];
		Material material;
		if (!meshDescription.bTextureArray)
		{
			material = UnityEngine.Object.Instantiate<Material>(Resources.Load<Material>("Materials/DistantPOI"));
		}
		else
		{
			material = UnityEngine.Object.Instantiate<Material>(Resources.Load<Material>("Materials/DistantPOI_TA"));
		}
		material.SetTexture("_MainTex", meshDescription.TexDiffuse);
		material.SetTexture("_Normal", meshDescription.TexNormal);
		material.SetTexture("_MetallicGlossMap", meshDescription.TexSpecular);
		material.SetTexture("_OcclusionMap", meshDescription.TexOcclusion);
		return material;
	}

	// Token: 0x06004E59 RID: 20057 RVA: 0x001EED5C File Offset: 0x001ECF5C
	public IEnumerator LoadTextureArraysForQuality(MeshDescriptionCollection _meshDescriptionCollection, int _index, int _quality, bool _isReload = false)
	{
		bool isSplatmap = this.IsSplatmap(_index);
		if (isSplatmap || this.bTextureArray)
		{
			yield return this.loadSingleArray(_quality, isSplatmap, MeshDescription.ETextureType.Diffuse);
			yield return null;
			yield return this.loadSingleArray(_quality, isSplatmap, MeshDescription.ETextureType.Normal);
			yield return null;
			yield return this.loadSingleArray(_quality, isSplatmap, MeshDescription.ETextureType.Specular);
			yield return null;
			if (_isReload)
			{
				this.ReloadTextureArrays(isSplatmap);
				if (MeshDescription.meshes.Length != 0)
				{
					this.textureAtlas.LoadTextureAtlas(_index, _meshDescriptionCollection, !GameManager.IsDedicatedServer);
					this.ReloadTextureArrays(isSplatmap);
				}
			}
		}
		yield break;
	}

	// Token: 0x06004E5A RID: 20058 RVA: 0x001EED88 File Offset: 0x001ECF88
	public void UnloadTextureArrays(int _index)
	{
		if (this.IsSplatmap(_index) || this.bTextureArray)
		{
			this.Unload(ref this.TexDiffuse);
			this.Unload(ref this.TexNormal);
			this.Unload(ref this.TexSpecular);
		}
	}

	// Token: 0x06004E5B RID: 20059 RVA: 0x001EEDBF File Offset: 0x001ECFBF
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator loadSingleArray(int _quality, bool _isSplatmap, MeshDescription.ETextureType _texType)
	{
		string folderAddress = _isSplatmap ? "TerrainTextures" : "BlockTextureAtlases";
		string path = _isSplatmap ? ("Microsplat/MicroSplatConfig_" + this.GetFileSuffixForTextureType(_texType, _isSplatmap) + "_tarray") : ("TextureArrays/" + Constants.cPrefixAtlas + this.Name + this.GetFileSuffixForTextureType(_texType, _isSplatmap));
		while (_quality >= 0)
		{
			string assetPath = path + this.GetFileSuffixForQuality(_quality, _isSplatmap) + ".asset";
			Texture2DArray asset;
			if (ThreadManager.IsInSyncCoroutine)
			{
				asset = LoadManager.LoadAssetFromAddressables<Texture2DArray>(folderAddress, assetPath, null, null, false, true, false).Asset;
			}
			else
			{
				LoadManager.AddressableRequestTask<Texture2DArray> request = LoadManager.LoadAssetFromAddressables<Texture2DArray>(folderAddress, assetPath, null, null, false, false, false);
				while (!request.IsDone)
				{
					yield return null;
				}
				asset = request.Asset;
				request = null;
			}
			if (asset != null)
			{
				if (!Application.isEditor && asset.isReadable)
				{
					asset.Apply(false, true);
				}
				switch (_texType)
				{
				case MeshDescription.ETextureType.Diffuse:
					this.TexDiffuse = asset;
					break;
				case MeshDescription.ETextureType.Normal:
					this.TexNormal = asset;
					break;
				case MeshDescription.ETextureType.Specular:
					this.TexSpecular = asset;
					break;
				}
				yield break;
			}
			int num = _quality;
			_quality = num - 1;
		}
		throw new Exception("No Texture2DArray found for " + this.Name + " " + _texType.ToStringCached<MeshDescription.ETextureType>());
		yield break;
	}

	// Token: 0x06004E5C RID: 20060 RVA: 0x001EEDE4 File Offset: 0x001ECFE4
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetFileSuffixForTextureType(MeshDescription.ETextureType _type, bool _isSplatmap)
	{
		switch (_type)
		{
		case MeshDescription.ETextureType.Diffuse:
			if (!_isSplatmap)
			{
				return "";
			}
			return "diff";
		case MeshDescription.ETextureType.Normal:
			if (!_isSplatmap)
			{
				return "_n";
			}
			return "normal";
		case MeshDescription.ETextureType.Specular:
			if (!_isSplatmap)
			{
				return "_s";
			}
			return "smoothAO";
		default:
			throw new ArgumentOutOfRangeException("_type", _type, null);
		}
	}

	// Token: 0x06004E5D RID: 20061 RVA: 0x001EEE43 File Offset: 0x001ED043
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetFileSuffixForQuality(int _quality, bool _isSplatmap)
	{
		if (!_isSplatmap)
		{
			return "_" + _quality.ToString();
		}
		if (_quality != 0)
		{
			return "_" + _quality.ToString();
		}
		return "";
	}

	// Token: 0x06004E5E RID: 20062 RVA: 0x001EEE74 File Offset: 0x001ED074
	[PublicizedFrom(EAccessModifier.Private)]
	public void Unload(ref Texture tex)
	{
		if (tex)
		{
			Log.Out("Unload {0}", new object[]
			{
				tex
			});
			Resources.UnloadAsset(tex);
			LoadManager.ReleaseAddressable<Texture>(tex);
			tex = null;
		}
	}

	// Token: 0x06004E5F RID: 20063 RVA: 0x001EEEA5 File Offset: 0x001ED0A5
	public void SetTextureFilter(int _index, int anisoLevel)
	{
		if (this.IsSplatmap(_index) || this.bTextureArray)
		{
			this.SetAF(this.TexDiffuse, anisoLevel);
			this.SetAF(this.TexNormal, anisoLevel);
			this.SetAF(this.TexSpecular, anisoLevel);
		}
	}

	// Token: 0x06004E60 RID: 20064 RVA: 0x001EEEDF File Offset: 0x001ED0DF
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetAF(Texture tex, int anisoLevel)
	{
		if (tex)
		{
			tex.anisoLevel = anisoLevel;
		}
	}

	// Token: 0x04003BC1 RID: 15297
	public const int cIndexOpaque = 0;

	// Token: 0x04003BC2 RID: 15298
	public const int cIndexWater = 1;

	// Token: 0x04003BC3 RID: 15299
	public const int cIndexTransparent = 2;

	// Token: 0x04003BC4 RID: 15300
	public const int cIndexGrass = 3;

	// Token: 0x04003BC5 RID: 15301
	public const int cIndexDecals = 4;

	// Token: 0x04003BC6 RID: 15302
	public const int cIndexTerrain = 5;

	// Token: 0x04003BC7 RID: 15303
	public const int cIndexCount = 6;

	// Token: 0x04003BC8 RID: 15304
	public const int MESH_OPAQUE = 0;

	// Token: 0x04003BC9 RID: 15305
	public const int MESH_WATER = 1;

	// Token: 0x04003BCA RID: 15306
	public const int MESH_TRANSPARENT = 2;

	// Token: 0x04003BCB RID: 15307
	public const int MESH_GRASS = 3;

	// Token: 0x04003BCC RID: 15308
	public const int MESH_DECALS = 4;

	// Token: 0x04003BCD RID: 15309
	public const int MESH_TERRAIN = 5;

	// Token: 0x04003BCE RID: 15310
	public const int MESH_MODELS = 0;

	// Token: 0x04003BCF RID: 15311
	public static MeshDescription[] meshes = new MeshDescription[0];

	// Token: 0x04003BD0 RID: 15312
	public static int GrassQualityPlanes;

	// Token: 0x04003BD1 RID: 15313
	public string Name;

	// Token: 0x04003BD2 RID: 15314
	public string Tag;

	// Token: 0x04003BD3 RID: 15315
	public VoxelMesh.EnumMeshType meshType;

	// Token: 0x04003BD4 RID: 15316
	public bool bCastShadows;

	// Token: 0x04003BD5 RID: 15317
	public bool bReceiveShadows;

	// Token: 0x04003BD6 RID: 15318
	public bool bHasLODs;

	// Token: 0x04003BD7 RID: 15319
	public bool bUseDebugStabilityShader;

	// Token: 0x04003BD8 RID: 15320
	public bool bTerrain;

	// Token: 0x04003BD9 RID: 15321
	public bool bTextureArray;

	// Token: 0x04003BDA RID: 15322
	public bool bSpecularIsBlack;

	// Token: 0x04003BDB RID: 15323
	public bool CreateTextureAtlas = true;

	// Token: 0x04003BDC RID: 15324
	public bool CreateSpecularMap = true;

	// Token: 0x04003BDD RID: 15325
	public bool CreateNormalMap = true;

	// Token: 0x04003BDE RID: 15326
	public bool CreateEmissionMap;

	// Token: 0x04003BDF RID: 15327
	public bool CreateHeightMap;

	// Token: 0x04003BE0 RID: 15328
	public bool CreateOcclusionMap;

	// Token: 0x04003BE1 RID: 15329
	public string MeshLayerName;

	// Token: 0x04003BE2 RID: 15330
	public string ColliderLayerName;

	// Token: 0x04003BE3 RID: 15331
	public AssetReference PrimaryShader;

	// Token: 0x04003BE4 RID: 15332
	public AssetReference SecondaryShader;

	// Token: 0x04003BE5 RID: 15333
	public AssetReference DistantShader;

	// Token: 0x04003BE6 RID: 15334
	public MeshDescription.EnumRenderMode BlendMode;

	// Token: 0x04003BE7 RID: 15335
	public Material BaseMaterial;

	// Token: 0x04003BE8 RID: 15336
	public string TextureAtlasClass;

	// Token: 0x04003BE9 RID: 15337
	public Texture TexDiffuse;

	// Token: 0x04003BEA RID: 15338
	public Texture TexNormal;

	// Token: 0x04003BEB RID: 15339
	public Texture TexSpecular;

	// Token: 0x04003BEC RID: 15340
	public Texture TexEmission;

	// Token: 0x04003BED RID: 15341
	public Texture TexHeight;

	// Token: 0x04003BEE RID: 15342
	public Texture TexOcclusion;

	// Token: 0x04003BEF RID: 15343
	public Texture2D TexMask;

	// Token: 0x04003BF0 RID: 15344
	public Texture2D TexMaskNormal;

	// Token: 0x04003BF1 RID: 15345
	public TextAsset MetaData;

	// Token: 0x04003BF2 RID: 15346
	[HideInInspector]
	public TextureAtlas textureAtlas;

	// Token: 0x04003BF3 RID: 15347
	[NonSerialized]
	public Material[] materials;

	// Token: 0x04003BF4 RID: 15348
	[NonSerialized]
	public Material materialDistant;

	// Token: 0x04003BF5 RID: 15349
	[NonSerialized]
	public Material[] prefabTerrainMaterials;

	// Token: 0x04003BF6 RID: 15350
	[NonSerialized]
	public Material prefabTerrainMaterialDistant;

	// Token: 0x04003BF7 RID: 15351
	public static bool bDebugStability;

	// Token: 0x020009FC RID: 2556
	public enum EnumRenderMode
	{
		// Token: 0x04003BF9 RID: 15353
		Default = -1,
		// Token: 0x04003BFA RID: 15354
		Opaque,
		// Token: 0x04003BFB RID: 15355
		Cutout,
		// Token: 0x04003BFC RID: 15356
		Fade,
		// Token: 0x04003BFD RID: 15357
		Transparent
	}

	// Token: 0x020009FD RID: 2557
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ETextureType
	{
		// Token: 0x04003BFF RID: 15359
		Diffuse,
		// Token: 0x04003C00 RID: 15360
		Normal,
		// Token: 0x04003C01 RID: 15361
		Specular
	}
}
