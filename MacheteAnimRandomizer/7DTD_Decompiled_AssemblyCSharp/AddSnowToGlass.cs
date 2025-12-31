using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010C0 RID: 4288
public class AddSnowToGlass : MonoBehaviour
{
	// Token: 0x0600870E RID: 34574 RVA: 0x0036A96C File Offset: 0x00368B6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		List<Material> list = new List<Material>();
		MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
		List<int[]> list2 = new List<int[]>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			int num = 0;
			for (int j = 0; j < componentsInChildren[i].materials.Length; j++)
			{
				if (componentsInChildren[i].materials[j].name.Contains(this.glassMaterial.name))
				{
					num++;
				}
			}
			if (num != 0)
			{
				MeshFilter component = componentsInChildren[i].transform.GetComponent<MeshFilter>();
				list.Clear();
				list2.Clear();
				for (int k = 0; k < component.mesh.subMeshCount; k++)
				{
					list2.Add(component.mesh.GetTriangles(k));
				}
				component.mesh.subMeshCount += num;
				int num2 = 0;
				for (int l = 0; l < componentsInChildren[i].materials.Length; l++)
				{
					list.Add(componentsInChildren[i].materials[l]);
					component.mesh.SetTriangles(list2[l], num2++);
					if (componentsInChildren[i].materials[l].name.Contains(this.glassMaterial.name))
					{
						list.Add(this.snowMaterial);
						component.mesh.SetTriangles(list2[l], num2++);
					}
				}
				componentsInChildren[i].materials = list.ToArray();
			}
		}
	}

	// Token: 0x040068D3 RID: 26835
	public Material glassMaterial;

	// Token: 0x040068D4 RID: 26836
	public Material snowMaterial;
}
