using System;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020010FA RID: 4346
public sealed class ScreenEffectsProxyRenderer : PostProcessEffectRendererProxy<ScreenEffectsProxy>
{
	// Token: 0x0600885A RID: 34906 RVA: 0x003731F8 File Offset: 0x003713F8
	public override void AddSubRenderersTo(List<PostProcessEffectRenderer> renderList)
	{
		foreach (ScreenEffects.ScreenEffect item in ScreenEffects.Instance.activeEffects)
		{
			renderList.Add(item);
		}
	}
}
