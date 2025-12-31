using System;

// Token: 0x02001138 RID: 4408
public class BlendCycleTimer
{
	// Token: 0x06008A7E RID: 35454 RVA: 0x00380567 File Offset: 0x0037E767
	public BlendCycleTimer(float inTime, float holdTime, float outTime)
	{
		this.m_inTime = inTime;
		this.m_outTime = outTime;
		this.m_holdTime = holdTime;
		this.m_time = 0f;
		this.m_dir = BlendCycleTimer.Dir.Done;
	}

	// Token: 0x06008A7F RID: 35455 RVA: 0x003805A8 File Offset: 0x0037E7A8
	public void Tick(float dt)
	{
		this.m_blendTimer.Tick(dt);
		switch (this.m_dir)
		{
		case BlendCycleTimer.Dir.In:
			this.m_time += dt;
			if (this.m_time >= this.m_inTime)
			{
				this.m_dir = BlendCycleTimer.Dir.Hold;
				this.m_time = 0f;
				return;
			}
			break;
		case BlendCycleTimer.Dir.Hold:
			if (this.m_holdTime != -1f)
			{
				this.m_time += dt;
				if (this.m_time >= this.m_holdTime)
				{
					this.m_dir = BlendCycleTimer.Dir.Out;
					this.m_time = 0f;
					this.m_blendTimer.BlendTo(0f, this.m_outTime);
				}
			}
			break;
		case BlendCycleTimer.Dir.Out:
			this.m_time += dt;
			if (this.m_time >= this.m_outTime)
			{
				this.m_dir = BlendCycleTimer.Dir.Done;
				return;
			}
			break;
		case BlendCycleTimer.Dir.Done:
			break;
		default:
			return;
		}
	}

	// Token: 0x06008A80 RID: 35456 RVA: 0x00380686 File Offset: 0x0037E886
	public void FadeIn()
	{
		this.m_dir = BlendCycleTimer.Dir.In;
		this.m_time = this.Value * this.m_inTime;
		this.m_blendTimer.BlendTo(1f, this.m_inTime);
	}

	// Token: 0x06008A81 RID: 35457 RVA: 0x003806B8 File Offset: 0x0037E8B8
	public void FadeOut()
	{
		this.m_dir = BlendCycleTimer.Dir.Out;
		this.m_time = (1f - this.Value) * this.m_outTime;
		this.m_blendTimer.BlendTo(0f, this.m_outTime);
	}

	// Token: 0x06008A82 RID: 35458 RVA: 0x003806F0 File Offset: 0x0037E8F0
	public void Restart()
	{
		this.m_time = 0f;
		this.m_dir = BlendCycleTimer.Dir.In;
		this.m_blendTimer.BlendTo(0f, 0f);
		this.m_blendTimer.BlendTo(1f, this.m_inTime);
	}

	// Token: 0x17000E75 RID: 3701
	// (get) Token: 0x06008A83 RID: 35459 RVA: 0x0038072F File Offset: 0x0037E92F
	public BlendCycleTimer.Dir Direction
	{
		get
		{
			return this.m_dir;
		}
	}

	// Token: 0x17000E76 RID: 3702
	// (get) Token: 0x06008A84 RID: 35460 RVA: 0x00380737 File Offset: 0x0037E937
	public float Value
	{
		get
		{
			return this.m_blendTimer.Value;
		}
	}

	// Token: 0x04006C49 RID: 27721
	[PublicizedFrom(EAccessModifier.Private)]
	public BlendTimer m_blendTimer = new BlendTimer(0f);

	// Token: 0x04006C4A RID: 27722
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_inTime;

	// Token: 0x04006C4B RID: 27723
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_outTime;

	// Token: 0x04006C4C RID: 27724
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_holdTime;

	// Token: 0x04006C4D RID: 27725
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_time;

	// Token: 0x04006C4E RID: 27726
	[PublicizedFrom(EAccessModifier.Private)]
	public BlendCycleTimer.Dir m_dir;

	// Token: 0x02001139 RID: 4409
	public enum Dir
	{
		// Token: 0x04006C50 RID: 27728
		In,
		// Token: 0x04006C51 RID: 27729
		Hold,
		// Token: 0x04006C52 RID: 27730
		Out,
		// Token: 0x04006C53 RID: 27731
		Done
	}
}
