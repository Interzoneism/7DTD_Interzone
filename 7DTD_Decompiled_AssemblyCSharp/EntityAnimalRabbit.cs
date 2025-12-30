using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000421 RID: 1057
[Preserve]
public class EntityAnimalRabbit : EntityAnimal
{
	// Token: 0x060020AE RID: 8366 RVA: 0x000CBB98 File Offset: 0x000C9D98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component)
		{
			component.center = new Vector3(0f, 0.15f, 0f);
			component.size = new Vector3(0.4f, 0.4f, 0.4f);
		}
		base.Awake();
		Transform transform = base.transform.Find("Graphics/BlobShadowProjector");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsAttackValid()
	{
		return false;
	}
}
