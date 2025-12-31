using System;
using System.Threading;
using UnityEngine;

// Token: 0x02001255 RID: 4693
public class WaitForTargetFPS : MonoBehaviour
{
	// Token: 0x17000F24 RID: 3876
	// (get) Token: 0x0600932F RID: 37679 RVA: 0x003A94C1 File Offset: 0x003A76C1
	// (set) Token: 0x06009330 RID: 37680 RVA: 0x003A94C9 File Offset: 0x003A76C9
	public int TargetFPS
	{
		get
		{
			return this.m_targetFPS;
		}
		set
		{
			if (value != this.m_targetFPS)
			{
				this.m_targetFPS = value;
				this.timePerFrame = 1f / (float)this.m_targetFPS;
				base.enabled = (this.m_targetFPS > 0);
			}
		}
	}

	// Token: 0x06009331 RID: 37681 RVA: 0x003A94FD File Offset: 0x003A76FD
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		this.timePerFrame = 1f / (float)this.TargetFPS;
	}

	// Token: 0x06009332 RID: 37682 RVA: 0x003A9514 File Offset: 0x003A7714
	[PublicizedFrom(EAccessModifier.Protected)]
	public void LateUpdate()
	{
		float num = 0f;
		float num2 = Time.realtimeSinceStartup - this.lastUpdateTime;
		this.lastUpdateTime = Time.realtimeSinceStartup;
		if (!this.SkipSleepThisFrame)
		{
			float num3 = num2 - this.sleepLastFrame;
			if (num3 < this.timePerFrame)
			{
				num = Math.Min(this.timePerFrame - num3, 1f);
				Thread.Sleep((int)(num * 1000f));
			}
		}
		this.sleepLastFrame = num;
		this.SkipSleepThisFrame = false;
	}

	// Token: 0x04007078 RID: 28792
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int m_targetFPS = 20;

	// Token: 0x04007079 RID: 28793
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timePerFrame;

	// Token: 0x0400707A RID: 28794
	public bool SkipSleepThisFrame;

	// Token: 0x0400707B RID: 28795
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float sleepLastFrame;

	// Token: 0x0400707C RID: 28796
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastUpdateTime;
}
