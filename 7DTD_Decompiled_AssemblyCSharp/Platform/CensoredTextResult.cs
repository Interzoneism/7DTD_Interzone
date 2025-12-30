using System;

namespace Platform
{
	// Token: 0x02001826 RID: 6182
	public struct CensoredTextResult
	{
		// Token: 0x170014D3 RID: 5331
		// (get) Token: 0x0600B7CE RID: 47054 RVA: 0x004688E6 File Offset: 0x00466AE6
		public readonly bool Success { get; }

		// Token: 0x170014D4 RID: 5332
		// (get) Token: 0x0600B7CF RID: 47055 RVA: 0x004688EE File Offset: 0x00466AEE
		public readonly string OriginalText { get; }

		// Token: 0x170014D5 RID: 5333
		// (get) Token: 0x0600B7D0 RID: 47056 RVA: 0x004688F6 File Offset: 0x00466AF6
		public readonly string CensoredText { get; }

		// Token: 0x0600B7D1 RID: 47057 RVA: 0x004688FE File Offset: 0x00466AFE
		public CensoredTextResult(bool _success, string _originalText, string _censoredText)
		{
			this.Success = _success;
			this.OriginalText = _originalText;
			this.CensoredText = _censoredText;
		}
	}
}
