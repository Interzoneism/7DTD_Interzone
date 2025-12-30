using System;
using UnityEngine;

// Token: 0x020010DB RID: 4315
public class ImageEffect_TurretView : MonoBehaviour
{
	// Token: 0x060087DB RID: 34779 RVA: 0x0036F973 File Offset: 0x0036DB73
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.material.SetTexture("_BackBuffer", source);
		Graphics.Blit(source, destination, this.material);
	}

	// Token: 0x040069A0 RID: 27040
	public Material material;
}
