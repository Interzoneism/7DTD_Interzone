using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000423 RID: 1059
[Preserve]
public class EntityAnimalStag : EntityAnimal
{
	// Token: 0x060020B3 RID: 8371 RVA: 0x000CBC54 File Offset: 0x000C9E54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component)
		{
			component.center = new Vector3(0f, 0.85f, 0f);
			component.size = new Vector3(0.8f, 1.6f, 0.8f);
		}
		base.Awake();
	}
}
