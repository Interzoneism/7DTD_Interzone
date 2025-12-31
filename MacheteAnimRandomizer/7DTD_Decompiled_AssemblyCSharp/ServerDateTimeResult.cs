using System;

// Token: 0x020007DC RID: 2012
public struct ServerDateTimeResult
{
	// Token: 0x170005D8 RID: 1496
	// (get) Token: 0x060039EB RID: 14827 RVA: 0x00175AEE File Offset: 0x00173CEE
	// (set) Token: 0x060039EC RID: 14828 RVA: 0x00175AF6 File Offset: 0x00173CF6
	public bool RequestComplete { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170005D9 RID: 1497
	// (get) Token: 0x060039ED RID: 14829 RVA: 0x00175AFF File Offset: 0x00173CFF
	// (set) Token: 0x060039EE RID: 14830 RVA: 0x00175B07 File Offset: 0x00173D07
	public bool HasError { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170005DA RID: 1498
	// (get) Token: 0x060039EF RID: 14831 RVA: 0x00175B10 File Offset: 0x00173D10
	// (set) Token: 0x060039F0 RID: 14832 RVA: 0x00175B18 File Offset: 0x00173D18
	public int SecondsOffset { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060039F1 RID: 14833 RVA: 0x00175B21 File Offset: 0x00173D21
	public ServerDateTimeResult(bool _requestComplete, bool _hasError, int _secondsOffset)
	{
		this.RequestComplete = _requestComplete;
		this.HasError = _hasError;
		this.SecondsOffset = _secondsOffset;
	}
}
