using System;
using System.Text;

// Token: 0x0200127E RID: 4734
public class CallbackMetric : IMetric
{
	// Token: 0x17000F33 RID: 3891
	// (get) Token: 0x0600941F RID: 37919 RVA: 0x003B176C File Offset: 0x003AF96C
	// (set) Token: 0x06009420 RID: 37920 RVA: 0x003B1774 File Offset: 0x003AF974
	public string Header { get; set; }

	// Token: 0x06009421 RID: 37921 RVA: 0x003B177D File Offset: 0x003AF97D
	public void AppendLastValue(StringBuilder builder)
	{
		builder.Append(this.callback());
	}

	// Token: 0x06009422 RID: 37922 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x0400714A RID: 29002
	public CallbackMetric.GetLastValue callback;

	// Token: 0x0200127F RID: 4735
	// (Invoke) Token: 0x06009425 RID: 37925
	public delegate string GetLastValue();
}
