using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A02 RID: 2562
public class MeshDescriptionCollection : MonoBehaviour
{
	// Token: 0x1700080A RID: 2058
	// (get) Token: 0x06004E7A RID: 20090 RVA: 0x001EFB07 File Offset: 0x001EDD07
	public MeshDescription[] Meshes
	{
		get
		{
			return this.currentMeshes;
		}
	}

	// Token: 0x06004E7B RID: 20091 RVA: 0x001EFB10 File Offset: 0x001EDD10
	public void Init()
	{
		MeshDescription meshDescription = this.meshes[0];
		if (meshDescription.TexDiffuse || meshDescription.TexNormal || meshDescription.TexSpecular)
		{
			Log.Error("MeshDescriptionCollection should not have MESH_OPAQUE textures");
		}
		meshDescription = this.meshes[5];
		if (meshDescription.TexDiffuse || meshDescription.TexNormal || meshDescription.TexSpecular)
		{
			Log.Error("MeshDescriptionCollection should not have MESH_TERRAIN textures");
		}
		this.currentMeshes = new MeshDescription[this.meshes.Length];
		for (int i = 0; i < this.meshes.Length; i++)
		{
			meshDescription = new MeshDescription(this.meshes[i]);
			this.currentMeshes[i] = meshDescription;
		}
	}

	// Token: 0x06004E7C RID: 20092 RVA: 0x001EFBCE File Offset: 0x001EDDCE
	public IEnumerator LoadTextureArrays(bool _isReload = false)
	{
		MicroStopwatch ms = new MicroStopwatch(true);
		MeshDescription[] mds = this.currentMeshes;
		for (int i = 0; i < mds.Length; i++)
		{
			mds[i].UnloadTextureArrays(i);
		}
		if (_isReload)
		{
			Resources.UnloadUnusedAssets();
		}
		int num;
		for (int index = 0; index < mds.Length; index = num + 1)
		{
			MeshDescription meshDescription = mds[index];
			yield return meshDescription.LoadTextureArraysForQuality(this, index, this.quality, _isReload);
			yield return null;
			num = index;
		}
		Log.Out("LoadTextureArraysForQuality took {0}", new object[]
		{
			(float)ms.ElapsedMilliseconds * 0.001f
		});
		if (GameManager.Instance != null && GameManager.Instance.prefabLODManager != null)
		{
			GameManager.Instance.prefabLODManager.UpdateMaterials();
		}
		yield break;
	}

	// Token: 0x06004E7D RID: 20093 RVA: 0x001EFBE4 File Offset: 0x001EDDE4
	public IEnumerator LoadTextureArraysForQuality(bool _isReload = false)
	{
		if (GameManager.IsDedicatedServer)
		{
			yield break;
		}
		int num = GameOptionsManager.GetTextureQuality(-1);
		if (num >= 3)
		{
			num = 2;
		}
		Log.Out("LoadTextureArraysForQuality quality {0} to {1}, reload {2}", new object[]
		{
			this.quality,
			num,
			_isReload
		});
		if (_isReload && num == this.quality)
		{
			yield break;
		}
		this.quality = num;
		yield return this.LoadTextureArrays(_isReload);
		yield break;
	}

	// Token: 0x06004E7E RID: 20094 RVA: 0x001EFBFC File Offset: 0x001EDDFC
	public void SetTextureArraysFilter()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		int textureFilter = GameOptionsManager.GetTextureFilter();
		int num = MeshDescriptionCollection.filterToAnisoLevel[textureFilter];
		Log.Out("SetTextureArraysFilter {0}, AF {1}", new object[]
		{
			textureFilter,
			num
		});
		MeshDescription[] array = this.currentMeshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetTextureFilter(i, num);
		}
	}

	// Token: 0x06004E7F RID: 20095 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x04003C1E RID: 15390
	public MeshDescription[] meshes;

	// Token: 0x04003C1F RID: 15391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshDescription[] currentMeshes;

	// Token: 0x04003C20 RID: 15392
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int quality = -1;

	// Token: 0x04003C21 RID: 15393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int[] filterToAnisoLevel = new int[]
	{
		1,
		2,
		4,
		8,
		9
	};
}
