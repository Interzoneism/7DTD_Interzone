using System;
using UnityEngine;

// Token: 0x020010E2 RID: 4322
[PreferBinarySerialization]
public class MeshMorph : ScriptableObject
{
	// Token: 0x17000E39 RID: 3641
	// (get) Token: 0x060087FE RID: 34814 RVA: 0x00370D55 File Offset: 0x0036EF55
	public Vector3[] Vertices
	{
		get
		{
			return this.vertices;
		}
	}

	// Token: 0x060087FF RID: 34815 RVA: 0x00370D5D File Offset: 0x0036EF5D
	public void Init(SkinnedMeshRenderer source, Vector3[] vertices)
	{
		this.source = source;
		this.vertices = vertices;
	}

	// Token: 0x06008800 RID: 34816 RVA: 0x00370D70 File Offset: 0x0036EF70
	public GameObject GetMorphedSkinnedMesh()
	{
		if (this.source == null || this.source.sharedMesh == null || this.vertices == null || this.vertices.Length == 0)
		{
			Debug.LogError("MeshMorph: source or vertices are null or empty", this);
			return null;
		}
		Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(this.source.sharedMesh);
		mesh.name = MeshMorph.cMeshPrefix + this.source.gameObject.name;
		mesh.SetVertices(this.vertices);
		mesh.RecalculateBounds();
		GameObject gameObject = new GameObject(this.source.gameObject.name);
		gameObject.transform.localPosition = this.source.transform.localPosition;
		gameObject.transform.localRotation = this.source.transform.localRotation;
		gameObject.transform.localScale = this.source.transform.localScale;
		SkinnedMeshRenderer skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
		skinnedMeshRenderer.sharedMesh = mesh;
		skinnedMeshRenderer.sharedMaterials = this.source.sharedMaterials;
		skinnedMeshRenderer.rootBone = this.source.rootBone;
		skinnedMeshRenderer.bones = this.source.bones;
		return gameObject;
	}

	// Token: 0x06008801 RID: 34817 RVA: 0x00370EA4 File Offset: 0x0036F0A4
	public static bool IsInstance(Mesh _mesh)
	{
		return _mesh.name.StartsWith(MeshMorph.cMeshPrefix);
	}

	// Token: 0x040069CC RID: 27084
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public SkinnedMeshRenderer source;

	// Token: 0x040069CD RID: 27085
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] vertices;

	// Token: 0x040069CE RID: 27086
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string cMeshPrefix = "MeshMorph-";
}
