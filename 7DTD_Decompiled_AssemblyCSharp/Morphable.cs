using System;
using UnityEngine;

// Token: 0x020010E7 RID: 4327
public class Morphable : MonoBehaviour
{
	// Token: 0x0600880C RID: 34828 RVA: 0x00370F30 File Offset: 0x0036F130
	public void MorphHeadgear(Archetype archetype, bool _ignoreDlcEntitlements)
	{
		MeshMorph meshMorph = DataLoader.LoadAsset<MeshMorph>(string.Concat(new string[]
		{
			this.MorphSetPath,
			"/",
			archetype.Race,
			archetype.Variant.ToString("00"),
			"/",
			this.MorphName,
			".asset"
		}), _ignoreDlcEntitlements);
		if (this.SkinnedMeshRenderer == null || this.SkinnedMeshRenderer.sharedMesh == null || meshMorph == null || meshMorph.Vertices == null || meshMorph.Vertices.Length == 0)
		{
			Debug.LogError("Morphable: SkinnedMeshRenderer or MeshMorph not found", this);
			return;
		}
		Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(this.SkinnedMeshRenderer.sharedMesh);
		mesh.name = Morphable.cMeshPrefix + this.SkinnedMeshRenderer.gameObject.name;
		mesh.SetVertices(meshMorph.Vertices);
		mesh.RecalculateBounds();
		this.SkinnedMeshRenderer.sharedMesh = mesh;
	}

	// Token: 0x040069DF RID: 27103
	public SkinnedMeshRenderer SkinnedMeshRenderer;

	// Token: 0x040069E0 RID: 27104
	public string MorphSetPath;

	// Token: 0x040069E1 RID: 27105
	public string MorphName;

	// Token: 0x040069E2 RID: 27106
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string cMeshPrefix = "MeshMorph-";
}
