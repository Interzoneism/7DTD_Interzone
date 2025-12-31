using System;

// Token: 0x02000E6F RID: 3695
public class TimerEventData
{
	// Token: 0x140000CC RID: 204
	// (add) Token: 0x0600740A RID: 29706 RVA: 0x002F31DC File Offset: 0x002F13DC
	// (remove) Token: 0x0600740B RID: 29707 RVA: 0x002F3214 File Offset: 0x002F1414
	public event TimerEventHandler Event;

	// Token: 0x140000CD RID: 205
	// (add) Token: 0x0600740C RID: 29708 RVA: 0x002F324C File Offset: 0x002F144C
	// (remove) Token: 0x0600740D RID: 29709 RVA: 0x002F3284 File Offset: 0x002F1484
	public event TimerEventHandler CloseEvent;

	// Token: 0x140000CE RID: 206
	// (add) Token: 0x0600740E RID: 29710 RVA: 0x002F32BC File Offset: 0x002F14BC
	// (remove) Token: 0x0600740F RID: 29711 RVA: 0x002F32F4 File Offset: 0x002F14F4
	public event TimerEventHandler AlternateEvent;

	// Token: 0x06007410 RID: 29712 RVA: 0x002F3329 File Offset: 0x002F1529
	public void HandleEvent()
	{
		if (this.Event != null)
		{
			this.Event(this);
		}
	}

	// Token: 0x06007411 RID: 29713 RVA: 0x002F333F File Offset: 0x002F153F
	public void HandleAlternateEvent()
	{
		if (this.AlternateEvent != null)
		{
			this.AlternateEvent(this);
		}
	}

	// Token: 0x06007412 RID: 29714 RVA: 0x002F3355 File Offset: 0x002F1555
	public void HandleCloseEvent(float _timeLeft)
	{
		this.timeLeft = _timeLeft;
		if (this.CloseEvent != null)
		{
			this.CloseEvent(this);
		}
	}

	// Token: 0x04005842 RID: 22594
	public object Data;

	// Token: 0x04005845 RID: 22597
	public bool CloseOnHit;

	// Token: 0x04005846 RID: 22598
	public float alternateTime = -1f;

	// Token: 0x04005848 RID: 22600
	public float timeLeft = -1f;
}
