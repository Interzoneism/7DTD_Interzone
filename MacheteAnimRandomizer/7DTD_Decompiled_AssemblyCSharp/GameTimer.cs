using System;
using UnityEngine;

// Token: 0x0200119C RID: 4508
public class GameTimer
{
	// Token: 0x17000E9D RID: 3741
	// (get) Token: 0x06008CF6 RID: 36086 RVA: 0x0038A81B File Offset: 0x00388A1B
	public static GameTimer Instance
	{
		get
		{
			if (GameTimer.m_Instance == null)
			{
				GameTimer.m_Instance = new GameTimer(20f);
			}
			return GameTimer.m_Instance;
		}
	}

	// Token: 0x06008CF7 RID: 36087 RVA: 0x0038A838 File Offset: 0x00388A38
	public GameTimer(float _t)
	{
		this.ticksPerSecond = _t;
		this.ms = new MicroStopwatch();
		this.Reset(0UL);
	}

	// Token: 0x06008CF8 RID: 36088 RVA: 0x0038A85A File Offset: 0x00388A5A
	public void Reset(ulong _ticks = 0UL)
	{
		this.elapsedPartialTicks = 0f;
		this.ticks = _ticks;
		this.ticksSincePlayfieldLoaded = 0UL;
		this.elapsedTicksD = 0.0;
		this.lastMillis = 0L;
		this.ms.ResetAndRestart();
	}

	// Token: 0x06008CF9 RID: 36089 RVA: 0x0038A898 File Offset: 0x00388A98
	public void updateTimer(bool _bServerIsStopped)
	{
		if (_bServerIsStopped)
		{
			this.Reset(this.ticks);
			return;
		}
		long elapsedMilliseconds = this.ms.ElapsedMilliseconds;
		long num = elapsedMilliseconds - this.lastMillis;
		this.lastMillis = elapsedMilliseconds;
		this.elapsedTicksD += (double)(Time.timeScale * (float)num) / 1000.0 * (double)this.ticksPerSecond;
		this.elapsedTicks = (int)this.elapsedTicksD;
		this.elapsedPartialTicks = (float)(this.elapsedTicksD - (double)this.elapsedTicks);
		this.elapsedTicksD -= (double)this.elapsedTicks;
		this.ticks += (ulong)((long)this.elapsedTicks);
		this.ticksSincePlayfieldLoaded += (ulong)((long)this.elapsedTicks);
	}

	// Token: 0x04006D8B RID: 28043
	public ulong ticks;

	// Token: 0x04006D8C RID: 28044
	public ulong ticksSincePlayfieldLoaded;

	// Token: 0x04006D8D RID: 28045
	public int elapsedTicks;

	// Token: 0x04006D8E RID: 28046
	public float elapsedPartialTicks;

	// Token: 0x04006D8F RID: 28047
	[PublicizedFrom(EAccessModifier.Private)]
	public double elapsedTicksD;

	// Token: 0x04006D90 RID: 28048
	[PublicizedFrom(EAccessModifier.Private)]
	public float ticksPerSecond;

	// Token: 0x04006D91 RID: 28049
	[PublicizedFrom(EAccessModifier.Private)]
	public long lastMillis;

	// Token: 0x04006D92 RID: 28050
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameTimer m_Instance;

	// Token: 0x04006D93 RID: 28051
	[PublicizedFrom(EAccessModifier.Private)]
	public MicroStopwatch ms;
}
