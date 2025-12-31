using System;
using Epic.OnlineServices;

namespace Platform.EOS
{
	// Token: 0x02001931 RID: 6449
	[PublicizedFrom(EAccessModifier.Internal)]
	public struct EOSSanction
	{
		// Token: 0x170015CD RID: 5581
		// (get) Token: 0x0600BE44 RID: 48708 RVA: 0x00481993 File Offset: 0x0047FB93
		public readonly string ReferenceId { get; }

		// Token: 0x0600BE45 RID: 48709 RVA: 0x0048199B File Offset: 0x0047FB9B
		public EOSSanction(DateTime? expiryDate, Utf8String referenceId)
		{
			this.ReferenceId = referenceId;
			this.expiry = expiryDate.GetValueOrDefault();
		}

		// Token: 0x0400942E RID: 37934
		public DateTime expiry;
	}
}
