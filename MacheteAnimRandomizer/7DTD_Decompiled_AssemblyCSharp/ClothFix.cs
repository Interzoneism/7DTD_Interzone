using System;
using UnityEngine;

// Token: 0x0200114F RID: 4431
public class ClothFix : MonoBehaviour
{
	// Token: 0x06008ADD RID: 35549 RVA: 0x0038208F File Offset: 0x0038028F
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.cloth = base.GetComponent<Cloth>();
	}

	// Token: 0x06008ADE RID: 35550 RVA: 0x003820A0 File Offset: 0x003802A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.cloth.enabled = false;
		this.cloth.enabled = true;
		MeshCollider[] components = base.GetComponents<MeshCollider>();
		for (int i = 0; i < components.Length; i++)
		{
			UnityEngine.Object.Destroy(components[i]);
		}
	}

	// Token: 0x06008ADF RID: 35551 RVA: 0x003820E2 File Offset: 0x003802E2
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.cloth.enabled = false;
	}

	// Token: 0x04006C9C RID: 27804
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Cloth cloth;
}
