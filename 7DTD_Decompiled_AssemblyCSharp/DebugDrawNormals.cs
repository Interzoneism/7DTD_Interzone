using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010CA RID: 4298
[ExecuteInEditMode]
public class DebugDrawNormals : MonoBehaviour
{
	// Token: 0x0600872A RID: 34602 RVA: 0x0036BAC0 File Offset: 0x00369CC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.die)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		this.TriangleCount = 0;
		this.VertCount = 0;
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component)
		{
			this.MeshCount = 1;
			this.Draw(component);
			return;
		}
		MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
		this.MeshCount = componentsInChildren.Length;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.Draw(componentsInChildren[i]);
		}
		if (componentsInChildren.Length == 0)
		{
			this.SetDie();
		}
	}

	// Token: 0x0600872B RID: 34603 RVA: 0x0036BB3C File Offset: 0x00369D3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Draw(MeshFilter mf)
	{
		Mesh sharedMesh = mf.sharedMesh;
		if (!sharedMesh)
		{
			return;
		}
		DebugDrawNormals.Data data;
		if (this.list.Count > 0 && !this.Record)
		{
			data = this.list[0];
		}
		else
		{
			data = new DebugDrawNormals.Data();
			this.list.Add(data);
			this.Record = false;
		}
		sharedMesh.GetVertices(data.verts);
		sharedMesh.GetNormals(data.normals);
		sharedMesh.GetTriangles(data.indices, 0);
		this.VertCount += data.verts.Count;
		this.TriangleCount += data.indices.Count / 3;
		Matrix4x4 localToWorldMatrix = mf.transform.localToWorldMatrix;
		for (int i = 0; i < this.list.Count; i++)
		{
			DebugDrawNormals.Data data2 = this.list[i];
			for (int j = 0; j < data2.normals.Count; j++)
			{
				Utils.DrawRay(localToWorldMatrix.MultiplyPoint(data2.verts[j]), localToWorldMatrix.MultiplyVector(data2.normals[j]) * this.VertexNormalScale, Color.white, Color.blue, 3, 0f);
			}
			for (int k = 0; k < data2.indices.Count - 2; k += 3)
			{
				Vector3 vector = data2.verts[data2.indices[k]];
				Vector3 vector2 = data2.verts[data2.indices[k + 1]];
				Vector3 vector3 = data2.verts[data2.indices[k + 2]];
				Vector3 point = (vector + vector2 + vector3) * 0.33333334f;
				Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
				Utils.DrawRay(localToWorldMatrix.MultiplyPoint(point), localToWorldMatrix.MultiplyVector(normalized) * this.TriangleNormalScale, Color.yellow, Color.red, 3, 0f);
			}
		}
	}

	// Token: 0x0600872C RID: 34604 RVA: 0x0036BD74 File Offset: 0x00369F74
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			this.SetDie();
		}
	}

	// Token: 0x0600872D RID: 34605 RVA: 0x0036BD89 File Offset: 0x00369F89
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDie()
	{
		this.list = null;
		this.die = true;
	}

	// Token: 0x0400690E RID: 26894
	public float VertexNormalScale = 0.05f;

	// Token: 0x0400690F RID: 26895
	public float TriangleNormalScale = 0.05f;

	// Token: 0x04006910 RID: 26896
	public int MeshCount;

	// Token: 0x04006911 RID: 26897
	public int TriangleCount;

	// Token: 0x04006912 RID: 26898
	public int VertCount;

	// Token: 0x04006913 RID: 26899
	public bool Record;

	// Token: 0x04006914 RID: 26900
	public List<DebugDrawNormals.Data> list = new List<DebugDrawNormals.Data>();

	// Token: 0x04006915 RID: 26901
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool die;

	// Token: 0x020010CB RID: 4299
	[Serializable]
	public class Data
	{
		// Token: 0x04006916 RID: 26902
		public List<Vector3> verts = new List<Vector3>();

		// Token: 0x04006917 RID: 26903
		public List<Vector3> normals = new List<Vector3>();

		// Token: 0x04006918 RID: 26904
		public List<int> indices = new List<int>();
	}
}
