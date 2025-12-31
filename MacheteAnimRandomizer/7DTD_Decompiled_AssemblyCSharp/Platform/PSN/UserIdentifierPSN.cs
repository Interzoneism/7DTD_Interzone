using System;
using UnityEngine.Scripting;

namespace Platform.PSN
{
	// Token: 0x020018E0 RID: 6368
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierPSN : PlatformUserIdentifierAbs
	{
		// Token: 0x17001573 RID: 5491
		// (get) Token: 0x0600BC0D RID: 48141 RVA: 0x00075E2B File Offset: 0x0007402B
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.PSN;
			}
		}

		// Token: 0x17001574 RID: 5492
		// (get) Token: 0x0600BC0E RID: 48142 RVA: 0x0047747E File Offset: 0x0047567E
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.PSN);

		// Token: 0x17001575 RID: 5493
		// (get) Token: 0x0600BC0F RID: 48143 RVA: 0x00477486 File Offset: 0x00475686
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x17001576 RID: 5494
		// (get) Token: 0x0600BC10 RID: 48144 RVA: 0x0047748E File Offset: 0x0047568E
		public override string CombinedString { get; }

		// Token: 0x17001577 RID: 5495
		// (get) Token: 0x0600BC11 RID: 48145 RVA: 0x00477496 File Offset: 0x00475696
		// (set) Token: 0x0600BC12 RID: 48146 RVA: 0x0047749E File Offset: 0x0047569E
		public ulong AccountId { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600BC13 RID: 48147 RVA: 0x004774A8 File Offset: 0x004756A8
		public UserIdentifierPSN(ulong _accountId)
		{
			this.AccountId = _accountId;
			this.ReadablePlatformUserIdentifier = this.AccountId.ToString();
			this.CombinedString = this.PlatformIdentifierString + "_" + this.ReadablePlatformUserIdentifier;
		}

		// Token: 0x0600BC14 RID: 48148 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool DecodeTicket(string _ticket)
		{
			return true;
		}

		// Token: 0x0600BC15 RID: 48149 RVA: 0x004774FE File Offset: 0x004756FE
		public override bool Equals(PlatformUserIdentifierAbs _other)
		{
			return _other != null && (this == _other || (_other is UserIdentifierPSN && this.AccountId == (_other as UserIdentifierPSN).AccountId));
		}

		// Token: 0x0600BC16 RID: 48150 RVA: 0x00477528 File Offset: 0x00475728
		public override int GetHashCode()
		{
			return this.AccountId.GetHashCode();
		}
	}
}
