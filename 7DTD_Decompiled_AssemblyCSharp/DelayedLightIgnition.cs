using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class DelayedLightIgnition : MonoBehaviour
{
	// Token: 0x060001F7 RID: 503 RVA: 0x0001120A File Offset: 0x0000F40A
	public void Awake()
	{
		this.myLight = base.GetComponent<Light>();
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x00011218 File Offset: 0x0000F418
	public void Start()
	{
		this.myLight.enabled = false;
		this.timer = this.delay;
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00011234 File Offset: 0x0000F434
	public void Update()
	{
		if (this.myLight != null && !this.myLight.enabled)
		{
			if (this.timer <= 0f)
			{
				this.myLight.enabled = true;
			}
			this.timer -= Time.deltaTime;
		}
	}

	// Token: 0x040002C2 RID: 706
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light myLight;

	// Token: 0x040002C3 RID: 707
	public float delay = 0.5f;

	// Token: 0x040002C4 RID: 708
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timer;
}
