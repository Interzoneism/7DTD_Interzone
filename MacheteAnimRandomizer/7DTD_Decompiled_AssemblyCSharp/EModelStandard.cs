using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200049D RID: 1181
[Preserve]
public class EModelStandard : EModelBase
{
	// Token: 0x060026AF RID: 9903 RVA: 0x000FB64C File Offset: 0x000F984C
	public override void PostInit()
	{
		base.PostInit();
		Transform modelTransform = base.GetModelTransform();
		if (modelTransform)
		{
			base.SetColliderLayers(modelTransform, 0);
		}
	}
}
