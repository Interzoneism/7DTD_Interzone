using System;

namespace Platform
{
	// Token: 0x020017E8 RID: 6120
	public interface IEntitlementValidator
	{
		// Token: 0x0600B6A7 RID: 46759
		void Init(IPlatform _owner);

		// Token: 0x0600B6A8 RID: 46760
		bool HasEntitlement(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600B6A9 RID: 46761
		bool IsAvailableOnPlatform(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600B6AA RID: 46762
		bool IsEntitlementPurchasable(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600B6AB RID: 46763
		bool OpenStore(EntitlementSetEnum _entitlementSet, Action<EntitlementSetEnum> _onPurchased);
	}
}
