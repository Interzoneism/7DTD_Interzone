using System;

// Token: 0x02001152 RID: 4434
public class CountdownTimer
{
	// Token: 0x17000E77 RID: 3703
	// (get) Token: 0x06008AE9 RID: 35561 RVA: 0x00382433 File Offset: 0x00380633
	// (set) Token: 0x06008AEA RID: 35562 RVA: 0x00002914 File Offset: 0x00000B14
	public long ElapsedMilliseconds
	{
		get
		{
			return (long)this.Elapsed.TotalMilliseconds;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
		}
	}

	// Token: 0x06008AEB RID: 35563 RVA: 0x00382441 File Offset: 0x00380641
	public CountdownTimer(float _seconds, bool _start = true)
	{
		this.ms = (long)((int)(_seconds * 1000f));
		this.IsRunning = _start;
		this.offset = 0L;
		if (this.IsRunning)
		{
			this.ResetAndRestart();
			return;
		}
		this.Reset();
	}

	// Token: 0x06008AEC RID: 35564 RVA: 0x0038247C File Offset: 0x0038067C
	public void SetTimeout(float _seconds)
	{
		this.ms = (long)((int)(_seconds * 1000f));
	}

	// Token: 0x06008AED RID: 35565 RVA: 0x00382490 File Offset: 0x00380690
	public bool HasPassed()
	{
		bool flag = false;
		if (this.IsRunning)
		{
			this.Update();
			flag = ((this.offset == 0L) ? (this.ElapsedMilliseconds > this.ms) : (this.ElapsedMilliseconds + this.offset > this.ms));
			if (flag)
			{
				this.offset = 0L;
			}
		}
		return flag;
	}

	// Token: 0x06008AEE RID: 35566 RVA: 0x003824E7 File Offset: 0x003806E7
	public void SetPassedIn(float _seconds)
	{
		this.offset = (long)((float)this.ms - _seconds * 1000f) - this.ElapsedMilliseconds;
	}

	// Token: 0x06008AEF RID: 35567 RVA: 0x00382508 File Offset: 0x00380708
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.Elapsed = DateTime.Now.Subtract(this.StartTime);
	}

	// Token: 0x06008AF0 RID: 35568 RVA: 0x0038252E File Offset: 0x0038072E
	public void Reset()
	{
		this.Elapsed = TimeSpan.Zero;
		this.StartTime = DateTime.Now;
		this.IsRunning = false;
	}

	// Token: 0x06008AF1 RID: 35569 RVA: 0x0038254D File Offset: 0x0038074D
	public void ResetAndRestart()
	{
		this.Reset();
		this.IsRunning = true;
	}

	// Token: 0x06008AF2 RID: 35570 RVA: 0x0038255C File Offset: 0x0038075C
	public void Stop()
	{
		this.IsRunning = false;
	}

	// Token: 0x04006C9E RID: 27806
	[PublicizedFrom(EAccessModifier.Private)]
	public long ms;

	// Token: 0x04006C9F RID: 27807
	[PublicizedFrom(EAccessModifier.Private)]
	public long offset;

	// Token: 0x04006CA0 RID: 27808
	public TimeSpan Elapsed;

	// Token: 0x04006CA1 RID: 27809
	public bool IsRunning;

	// Token: 0x04006CA2 RID: 27810
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime StartTime;
}
