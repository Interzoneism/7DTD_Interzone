using System;

namespace Platform
{
	// Token: 0x02001825 RID: 6181
	public struct CensoredTextRequest
	{
		// Token: 0x170014D0 RID: 5328
		// (get) Token: 0x0600B7C9 RID: 47049 RVA: 0x004688A9 File Offset: 0x00466AA9
		public readonly string Input { get; }

		// Token: 0x170014D1 RID: 5329
		// (get) Token: 0x0600B7CA RID: 47050 RVA: 0x004688B1 File Offset: 0x00466AB1
		// (set) Token: 0x0600B7CB RID: 47051 RVA: 0x004688B9 File Offset: 0x00466AB9
		public int CensoredLength { readonly get; set; }

		// Token: 0x170014D2 RID: 5330
		// (get) Token: 0x0600B7CC RID: 47052 RVA: 0x004688C2 File Offset: 0x00466AC2
		public readonly Action<CensoredTextResult> Callback { get; }

		// Token: 0x0600B7CD RID: 47053 RVA: 0x004688CA File Offset: 0x00466ACA
		public CensoredTextRequest(string _input, Action<CensoredTextResult> _callback)
		{
			this.Input = _input;
			this.CensoredLength = _input.Length;
			this.Callback = _callback;
		}
	}
}
