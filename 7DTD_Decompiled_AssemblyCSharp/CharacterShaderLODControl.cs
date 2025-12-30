using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class CharacterShaderLODControl : MonoBehaviour
{
	// Token: 0x06000089 RID: 137 RVA: 0x00009014 File Offset: 0x00007214
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		this.materials = new List<Material>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Material material in array[i].materials)
			{
				if (material.shader.name.Contains("Game/SDCS/"))
				{
					this.materials.Add(material);
				}
			}
		}
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00009084 File Offset: 0x00007284
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Camera.main == null)
		{
			return;
		}
		int maximumLOD;
		if (Vector3.Distance(Camera.main.transform.position, base.transform.position) <= this.transitionDistance)
		{
			maximumLOD = 200;
		}
		else
		{
			maximumLOD = 100;
		}
		foreach (Material material in this.materials)
		{
			material.shader.maximumLOD = maximumLOD;
		}
	}

	// Token: 0x040000C0 RID: 192
	public float transitionDistance = 5f;

	// Token: 0x040000C1 RID: 193
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Material> materials;
}
