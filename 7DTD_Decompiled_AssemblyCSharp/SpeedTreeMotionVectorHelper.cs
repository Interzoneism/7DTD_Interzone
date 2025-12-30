using System;
using UnityEngine;

// Token: 0x02001104 RID: 4356
public class SpeedTreeMotionVectorHelper : MonoBehaviour
{
	// Token: 0x060088D4 RID: 35028 RVA: 0x00376603 File Offset: 0x00374803
	public void Init(Renderer renderer)
	{
		this.renderer = renderer;
	}

	// Token: 0x060088D5 RID: 35029 RVA: 0x0037660C File Offset: 0x0037480C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnBecameVisible()
	{
		if (!SpeedTreeWindHistoryBufferManager.Instance.TryRegisterActiveRenderer(this.renderer))
		{
			Debug.LogError("Failed to register tree renderer.");
			base.enabled = false;
		}
	}

	// Token: 0x060088D6 RID: 35030 RVA: 0x00376631 File Offset: 0x00374831
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnBecameInvisible()
	{
		SpeedTreeWindHistoryBufferManager.Instance.DeregisterActiveRenderer(this.renderer);
	}

	// Token: 0x04006AC4 RID: 27332
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer renderer;
}
