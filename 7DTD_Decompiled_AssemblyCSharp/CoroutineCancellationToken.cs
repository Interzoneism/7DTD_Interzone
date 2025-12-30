using System;

// Token: 0x0200108A RID: 4234
public class CoroutineCancellationToken
{
	// Token: 0x060085C3 RID: 34243 RVA: 0x0036591C File Offset: 0x00363B1C
	public void Cancel()
	{
		this.cancelled = true;
	}

	// Token: 0x060085C4 RID: 34244 RVA: 0x00365925 File Offset: 0x00363B25
	public bool IsCancelled()
	{
		return this.cancelled;
	}

	// Token: 0x040067D4 RID: 26580
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cancelled;
}
