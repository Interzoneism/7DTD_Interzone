using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class FrameRateLimiter : MonoBehaviour
{
	// Token: 0x06000093 RID: 147 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00009618 File Offset: 0x00007818
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.MaxFrames < 4f)
		{
			this.MaxFrames = 4f;
		}
		if (this.MaxFrames < 60f)
		{
			int num = (int)(1000.0 / (double)this.MaxFrames - (double)(Time.deltaTime * 1000f));
			if (num > 0)
			{
				Thread.Sleep(num);
			}
		}
	}

	// Token: 0x040000D5 RID: 213
	public float MaxFrames = 9999f;
}
