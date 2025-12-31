using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020011DD RID: 4573
public class OnPostRenderDispatcher : MonoBehaviour
{
	// Token: 0x06008EBE RID: 36542 RVA: 0x00391D92 File Offset: 0x0038FF92
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		OnPostRenderDispatcher.Instance = this;
	}

	// Token: 0x06008EBF RID: 36543 RVA: 0x00391D9C File Offset: 0x0038FF9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnPostRender()
	{
		for (int i = 0; i < this.listeners.Count; i++)
		{
			this.listeners[i].OnPostRender();
		}
	}

	// Token: 0x06008EC0 RID: 36544 RVA: 0x00391DD0 File Offset: 0x0038FFD0
	public void Add(IOnPostRender _p)
	{
		this.listeners.Add(_p);
	}

	// Token: 0x06008EC1 RID: 36545 RVA: 0x00391DDE File Offset: 0x0038FFDE
	public void Remove(IOnPostRender _p)
	{
		this.listeners.Remove(_p);
	}

	// Token: 0x04006E70 RID: 28272
	public static OnPostRenderDispatcher Instance;

	// Token: 0x04006E71 RID: 28273
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<IOnPostRender> listeners = new List<IOnPostRender>();
}
