using System;
using UnityEngine;

// Token: 0x02001191 RID: 4497
public class FPS
{
	// Token: 0x06008C84 RID: 35972 RVA: 0x003887D5 File Offset: 0x003869D5
	public FPS(float _restartTime)
	{
		this.restartTime = _restartTime;
		this.timeleft = _restartTime;
	}

	// Token: 0x06008C85 RID: 35973 RVA: 0x003887F8 File Offset: 0x003869F8
	public bool Update()
	{
		this.timeleft -= Time.unscaledDeltaTime;
		this.accum += Time.unscaledDeltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.Counter = (float)this.frames / this.accum;
			this.timeleft = this.restartTime;
			this.accum = 0f;
			this.frames = 0;
			return true;
		}
		return false;
	}

	// Token: 0x04006D4D RID: 27981
	[PublicizedFrom(EAccessModifier.Private)]
	public float accum;

	// Token: 0x04006D4E RID: 27982
	[PublicizedFrom(EAccessModifier.Private)]
	public int frames;

	// Token: 0x04006D4F RID: 27983
	[PublicizedFrom(EAccessModifier.Private)]
	public float timeleft;

	// Token: 0x04006D50 RID: 27984
	[PublicizedFrom(EAccessModifier.Private)]
	public float restartTime = 0.5f;

	// Token: 0x04006D51 RID: 27985
	public float Counter;
}
