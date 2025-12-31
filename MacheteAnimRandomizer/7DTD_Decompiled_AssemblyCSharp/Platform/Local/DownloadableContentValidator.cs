using System;

namespace Platform.Local
{
	// Token: 0x020018EC RID: 6380
	public class DownloadableContentValidator : IEntitlementValidator
	{
		// Token: 0x0600BC8F RID: 48271 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BC90 RID: 48272 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsAvailableOnPlatform(EntitlementSetEnum _dlcSet)
		{
			return false;
		}

		// Token: 0x0600BC91 RID: 48273 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool HasEntitlement(EntitlementSetEnum _dlcSet)
		{
			return false;
		}

		// Token: 0x0600BC92 RID: 48274 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsEntitlementPurchasable(EntitlementSetEnum _dlcSet)
		{
			return false;
		}

		// Token: 0x0600BC93 RID: 48275 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool OpenStore(EntitlementSetEnum _dlcSet, Action<EntitlementSetEnum> _onDlcPurchased)
		{
			return false;
		}
	}
}
