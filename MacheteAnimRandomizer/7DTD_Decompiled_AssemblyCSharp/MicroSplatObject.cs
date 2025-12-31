using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
[ExecuteAlways]
public class MicroSplatObject : MonoBehaviour
{
	// Token: 0x060000E6 RID: 230 RVA: 0x0000B0DC File Offset: 0x000092DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public long GetOverrideHash()
	{
		long num = 3L * (long)(((this.propData == null) ? 3 : this.propData.GetHashCode()) * 3) * (((this.perPixelNormal == null) ? 7L : this.perPixelNormal.GetNativeTexturePtr().ToInt64()) * 7L) * (long)(((this.keywordSO == null) ? 11 : this.keywordSO.GetHashCode()) * 11) * (((this.procBiomeMask == null) ? 13L : this.procBiomeMask.GetNativeTexturePtr().ToInt64()) * 13L) * (((this.procBiomeMask2 == null) ? 81L : this.procBiomeMask2.GetNativeTexturePtr().ToInt64()) * 81L) * (((this.cavityMap == null) ? 17L : this.cavityMap.GetNativeTexturePtr().ToInt64()) * 17L) * (long)(((this.procTexCfg == null) ? 19 : this.procTexCfg.GetHashCode()) * 19) * (((this.streamTexture == null) ? 41L : this.streamTexture.GetNativeTexturePtr().ToInt64()) * 41L) * (((this.terrainDesc == null) ? 43L : this.terrainDesc.GetNativeTexturePtr().ToInt64()) * 43L) * (((this.geoTextureOverride == null) ? 47L : this.geoTextureOverride.GetNativeTexturePtr().ToInt64()) * 47L) * (((this.globalNormalOverride == null) ? 53L : this.globalNormalOverride.GetNativeTexturePtr().ToInt64()) * 53L) * (((this.globalSAOMOverride == null) ? 59L : this.globalSAOMOverride.GetNativeTexturePtr().ToInt64()) * 59L) * (((this.globalEmisOverride == null) ? 61L : this.globalEmisOverride.GetNativeTexturePtr().ToInt64()) * 61L) * (((this.tintMapOverride == null) ? 71L : this.tintMapOverride.GetNativeTexturePtr().ToInt64()) * 71L);
		if (num == 0L)
		{
			Debug.Log("Override hash returned 0, this should not happen");
		}
		return num;
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x0000B333 File Offset: 0x00009533
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetMap(Material m, string name, Texture2D tex)
	{
		if (m.HasProperty(name) && tex != null)
		{
			m.SetTexture(name, tex);
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x0000B350 File Offset: 0x00009550
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ApplyMaps(Material m)
	{
		this.SetMap(m, "_PerPixelNormal", this.perPixelNormal);
		this.SetMap(m, "_StreamControl", this.streamTexture);
		this.SetMap(m, "_GeoTex", this.geoTextureOverride);
		this.SetMap(m, "_GlobalTintTex", this.tintMapOverride);
		this.SetMap(m, "_GlobalNormalTex", this.globalNormalOverride);
		this.SetMap(m, "_GlobalSAOMTex", this.globalSAOMOverride);
		this.SetMap(m, "_GlobalEmisTex", this.globalEmisOverride);
		if (m.HasProperty("_GeoCurve") && this.propData != null)
		{
			m.SetTexture("_GeoCurve", this.propData.GetGeoCurve());
		}
		if (m.HasProperty("_GeoSlopeTex") && this.propData != null)
		{
			m.SetTexture("_GeoSlopeTex", this.propData.GetGeoSlopeFilter());
		}
		if (m.HasProperty("_GlobalSlopeTex") && this.propData != null)
		{
			m.SetTexture("_GlobalSlopeTex", this.propData.GetGlobalSlopeFilter());
		}
		if (this.propData != null)
		{
			m.SetTexture("_PerTexProps", this.propData.GetTexture());
		}
		if (this.procTexCfg != null)
		{
			if (m.HasProperty("_ProcTexCurves"))
			{
				m.SetTexture("_ProcTexCurves", this.procTexCfg.GetCurveTexture());
				m.SetTexture("_ProcTexParams", this.procTexCfg.GetParamTexture());
				m.SetInt("_PCLayerCount", this.procTexCfg.layers.Count);
				if (this.procBiomeMask != null && m.HasProperty("_ProcTexBiomeMask"))
				{
					m.SetTexture("_ProcTexBiomeMask", this.procBiomeMask);
				}
				if (this.procBiomeMask2 != null && m.HasProperty("_ProcTexBiomeMask2"))
				{
					m.SetTexture("_ProcTexBiomeMask2", this.procBiomeMask2);
				}
			}
			if (m.HasProperty("_PCHeightGradients"))
			{
				m.SetTexture("_PCHeightGradients", this.procTexCfg.GetHeightGradientTexture());
			}
			if (m.HasProperty("_PCHeightHSV"))
			{
				m.SetTexture("_PCHeightHSV", this.procTexCfg.GetHeightHSVTexture());
			}
			if (m.HasProperty("_CavityMap"))
			{
				m.SetTexture("_CavityMap", this.cavityMap);
			}
			if (m.HasProperty("_PCSlopeGradients"))
			{
				m.SetTexture("_PCSlopeGradients", this.procTexCfg.GetSlopeGradientTexture());
			}
			if (m.HasProperty("_PCSlopeHSV"))
			{
				m.SetTexture("_PCSlopeHSV", this.procTexCfg.GetSlopeHSVTexture());
			}
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x0000B5FC File Offset: 0x000097FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ApplyControlTextures(Texture2D[] controls, Material m)
	{
		m.SetTexture("_Control0", (controls.Length != 0) ? controls[0] : Texture2D.blackTexture);
		m.SetTexture("_Control1", (controls.Length > 1) ? controls[1] : Texture2D.blackTexture);
		m.SetTexture("_Control2", (controls.Length > 2) ? controls[2] : Texture2D.blackTexture);
		m.SetTexture("_Control3", (controls.Length > 3) ? controls[3] : Texture2D.blackTexture);
		m.SetTexture("_Control4", (controls.Length > 4) ? controls[4] : Texture2D.blackTexture);
		m.SetTexture("_Control5", (controls.Length > 5) ? controls[5] : Texture2D.blackTexture);
		m.SetTexture("_Control6", (controls.Length > 6) ? controls[6] : Texture2D.blackTexture);
		m.SetTexture("_Control7", (controls.Length > 7) ? controls[7] : Texture2D.blackTexture);
	}

	// Token: 0x060000EA RID: 234 RVA: 0x0000B6E0 File Offset: 0x000098E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SyncBlendMat(Vector3 size)
	{
		if (this.blendMatInstance != null && this.matInstance != null)
		{
			this.blendMatInstance.CopyPropertiesFromMaterial(this.matInstance);
			Vector4 value = default(Vector4);
			value.z = size.x;
			value.w = size.z;
			value.x = base.transform.position.x;
			value.y = base.transform.position.z;
			this.blendMatInstance.SetVector("_TerrainBounds", value);
			this.blendMatInstance.SetTexture("_TerrainDesc", this.terrainDesc);
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x0000B798 File Offset: 0x00009998
	public virtual Bounds GetBounds()
	{
		return default(Bounds);
	}

	// Token: 0x060000EC RID: 236 RVA: 0x0000B7B0 File Offset: 0x000099B0
	public Material GetBlendMatInstance()
	{
		if (this.blendMat != null && this.terrainDesc != null)
		{
			if (this.blendMatInstance == null)
			{
				this.blendMatInstance = new Material(this.blendMat);
				this.SyncBlendMat(this.GetBounds().size);
			}
			if (this.blendMatInstance.shader != this.blendMat.shader)
			{
				this.blendMatInstance.shader = this.blendMat.shader;
				this.SyncBlendMat(this.GetBounds().size);
			}
		}
		return this.blendMatInstance;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x0000B85C File Offset: 0x00009A5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ApplyBlendMap()
	{
		if (this.blendMat != null && this.terrainDesc != null)
		{
			if (this.blendMatInstance == null)
			{
				this.blendMatInstance = new Material(this.blendMat);
			}
			this.SyncBlendMat(this.GetBounds().size);
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00002914 File Offset: 0x00000B14
	public void RevisionFromMat()
	{
	}

	// Token: 0x060000EF RID: 239 RVA: 0x0000B8B8 File Offset: 0x00009AB8
	public static void SyncAll()
	{
		MicroSplatTerrain.SyncAll();
	}

	// Token: 0x04000119 RID: 281
	[HideInInspector]
	public Material templateMaterial;

	// Token: 0x0400011A RID: 282
	[HideInInspector]
	[NonSerialized]
	public Material matInstance;

	// Token: 0x0400011B RID: 283
	[HideInInspector]
	public Material blendMat;

	// Token: 0x0400011C RID: 284
	[HideInInspector]
	public Material blendMatInstance;

	// Token: 0x0400011D RID: 285
	[HideInInspector]
	public MicroSplatKeywords keywordSO;

	// Token: 0x0400011E RID: 286
	[HideInInspector]
	public Texture2D perPixelNormal;

	// Token: 0x0400011F RID: 287
	[HideInInspector]
	public Texture2D terrainDesc;

	// Token: 0x04000120 RID: 288
	public MicroSplatObject.DescriptorFormat descriptorFormat;

	// Token: 0x04000121 RID: 289
	[HideInInspector]
	public Texture2D streamTexture;

	// Token: 0x04000122 RID: 290
	[HideInInspector]
	public Texture2D cavityMap;

	// Token: 0x04000123 RID: 291
	[HideInInspector]
	public MicroSplatProceduralTextureConfig procTexCfg;

	// Token: 0x04000124 RID: 292
	[HideInInspector]
	public Texture2D procBiomeMask;

	// Token: 0x04000125 RID: 293
	[HideInInspector]
	public Texture2D procBiomeMask2;

	// Token: 0x04000126 RID: 294
	[HideInInspector]
	public Texture2D tintMapOverride;

	// Token: 0x04000127 RID: 295
	[HideInInspector]
	public Texture2D globalNormalOverride;

	// Token: 0x04000128 RID: 296
	[HideInInspector]
	public Texture2D globalSAOMOverride;

	// Token: 0x04000129 RID: 297
	[HideInInspector]
	public Texture2D globalEmisOverride;

	// Token: 0x0400012A RID: 298
	[HideInInspector]
	public Texture2D geoTextureOverride;

	// Token: 0x0400012B RID: 299
	[HideInInspector]
	public MicroSplatPropData propData;

	// Token: 0x02000025 RID: 37
	public enum DescriptorFormat
	{
		// Token: 0x0400012D RID: 301
		RGBAHalf,
		// Token: 0x0400012E RID: 302
		RGBAFloat
	}
}
