using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002A RID: 42
[ExecuteInEditMode]
[RequireComponent(typeof(Terrain))]
[DisallowMultipleComponent]
public class MicroSplatTerrain : MicroSplatObject
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060000FE RID: 254 RVA: 0x0000BDD4 File Offset: 0x00009FD4
	// (remove) Token: 0x060000FF RID: 255 RVA: 0x0000BE08 File Offset: 0x0000A008
	public static event MicroSplatTerrain.MaterialSyncAll OnMaterialSyncAll;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000100 RID: 256 RVA: 0x0000BE3C File Offset: 0x0000A03C
	// (remove) Token: 0x06000101 RID: 257 RVA: 0x0000BE74 File Offset: 0x0000A074
	public event MicroSplatTerrain.MaterialSync OnMaterialSync;

	// Token: 0x06000102 RID: 258 RVA: 0x0000BEA9 File Offset: 0x0000A0A9
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.terrain = base.GetComponent<Terrain>();
	}

	// Token: 0x06000103 RID: 259 RVA: 0x0000BEB7 File Offset: 0x0000A0B7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.terrain = base.GetComponent<Terrain>();
		MicroSplatTerrain.sInstances.Add(this);
		if (this.reenabled)
		{
			this.Sync();
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x0000BEDE File Offset: 0x0000A0DE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.Sync();
	}

	// Token: 0x06000105 RID: 261 RVA: 0x0000BEE6 File Offset: 0x0000A0E6
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		MicroSplatTerrain.sInstances.Remove(this);
		this.Cleanup();
		this.reenabled = true;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000BF01 File Offset: 0x0000A101
	[PublicizedFrom(EAccessModifier.Private)]
	public void Cleanup()
	{
		if (this.matInstance != null && this.matInstance != this.templateMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.matInstance);
			this.terrain.materialTemplate = null;
		}
	}

	// Token: 0x06000107 RID: 263 RVA: 0x0000BF3C File Offset: 0x0000A13C
	public void Sync()
	{
		if (this.templateMaterial == null)
		{
			return;
		}
		Material material;
		if (this.terrain.materialTemplate == this.matInstance && this.matInstance != null)
		{
			this.terrain.materialTemplate.CopyPropertiesFromMaterial(this.templateMaterial);
			material = this.terrain.materialTemplate;
		}
		else
		{
			material = new Material(this.templateMaterial);
		}
		if (this.terrain.drawInstanced && this.keywordSO.IsKeywordEnabled("_TESSDISTANCE") && this.keywordSO.IsKeywordEnabled("_MSRENDERLOOP_SURFACESHADER"))
		{
			Debug.LogWarning("Disabling terrain instancing when tessellation is enabled, as Unity has not made surface shader tessellation compatible with terrain instancing");
			this.terrain.drawInstanced = false;
		}
		material.hideFlags = HideFlags.HideAndDontSave;
		this.terrain.materialTemplate = material;
		this.matInstance = material;
		base.ApplyMaps(material);
		if (this.keywordSO.IsKeywordEnabled("_CUSTOMSPLATTEXTURES"))
		{
			material.SetTexture("_CustomControl0", (this.customControl0 != null) ? this.customControl0 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl1", (this.customControl1 != null) ? this.customControl1 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl2", (this.customControl2 != null) ? this.customControl2 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl3", (this.customControl3 != null) ? this.customControl3 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl4", (this.customControl4 != null) ? this.customControl4 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl5", (this.customControl5 != null) ? this.customControl5 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl6", (this.customControl6 != null) ? this.customControl6 : Texture2D.blackTexture);
			material.SetTexture("_CustomControl7", (this.customControl7 != null) ? this.customControl7 : Texture2D.blackTexture);
		}
		else
		{
			if (this.terrain == null || this.terrain.terrainData == null)
			{
				Debug.LogError("Terrain or terrain data is null, cannot sync");
				return;
			}
			Texture2D[] alphamapTextures = this.terrain.terrainData.alphamapTextures;
			base.ApplyControlTextures(alphamapTextures, material);
		}
		base.ApplyBlendMap();
		if (this.OnMaterialSync != null)
		{
			this.OnMaterialSync(material);
		}
	}

	// Token: 0x06000108 RID: 264 RVA: 0x0000C1BF File Offset: 0x0000A3BF
	public override Bounds GetBounds()
	{
		return this.terrain.terrainData.bounds;
	}

	// Token: 0x06000109 RID: 265 RVA: 0x0000C1D4 File Offset: 0x0000A3D4
	public new static void SyncAll()
	{
		for (int i = 0; i < MicroSplatTerrain.sInstances.Count; i++)
		{
			MicroSplatTerrain.sInstances[i].Sync();
		}
		if (MicroSplatTerrain.OnMaterialSyncAll != null)
		{
			MicroSplatTerrain.OnMaterialSyncAll();
		}
	}

	// Token: 0x04000170 RID: 368
	[HideInInspector]
	public Shader addPass;

	// Token: 0x04000173 RID: 371
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<MicroSplatTerrain> sInstances = new List<MicroSplatTerrain>();

	// Token: 0x04000174 RID: 372
	public Terrain terrain;

	// Token: 0x04000175 RID: 373
	[HideInInspector]
	public Texture2D customControl0;

	// Token: 0x04000176 RID: 374
	[HideInInspector]
	public Texture2D customControl1;

	// Token: 0x04000177 RID: 375
	[HideInInspector]
	public Texture2D customControl2;

	// Token: 0x04000178 RID: 376
	[HideInInspector]
	public Texture2D customControl3;

	// Token: 0x04000179 RID: 377
	[HideInInspector]
	public Texture2D customControl4;

	// Token: 0x0400017A RID: 378
	[HideInInspector]
	public Texture2D customControl5;

	// Token: 0x0400017B RID: 379
	[HideInInspector]
	public Texture2D customControl6;

	// Token: 0x0400017C RID: 380
	[HideInInspector]
	public Texture2D customControl7;

	// Token: 0x0400017D RID: 381
	[HideInInspector]
	public bool reenabled;

	// Token: 0x0200002B RID: 43
	// (Invoke) Token: 0x0600010D RID: 269
	public delegate void MaterialSyncAll();

	// Token: 0x0200002C RID: 44
	// (Invoke) Token: 0x06000111 RID: 273
	public delegate void MaterialSync(Material m);
}
