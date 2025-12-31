using System;
using UnityEngine.Scripting;

namespace Platform.XBL
{
	// Token: 0x0200187C RID: 6268
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierXbl : PlatformUserIdentifierAbs
	{
		// Token: 0x17001517 RID: 5399
		// (get) Token: 0x0600B969 RID: 47465 RVA: 0x00075CC0 File Offset: 0x00073EC0
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.XBL;
			}
		}

		// Token: 0x17001518 RID: 5400
		// (get) Token: 0x0600B96A RID: 47466 RVA: 0x0046D6E2 File Offset: 0x0046B8E2
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.XBL);

		// Token: 0x17001519 RID: 5401
		// (get) Token: 0x0600B96B RID: 47467 RVA: 0x0046D6EA File Offset: 0x0046B8EA
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x1700151A RID: 5402
		// (get) Token: 0x0600B96C RID: 47468 RVA: 0x0046D6F2 File Offset: 0x0046B8F2
		public override string CombinedString { get; }

		// Token: 0x1700151B RID: 5403
		// (get) Token: 0x0600B96D RID: 47469 RVA: 0x0046D6FA File Offset: 0x0046B8FA
		public ulong Xuid
		{
			get
			{
				return XblXuidMapper.GetXuid(this);
			}
		}

		// Token: 0x0600B96E RID: 47470 RVA: 0x0046D704 File Offset: 0x0046B904
		public UserIdentifierXbl(string _pxuid)
		{
			this.pxuid = _pxuid;
			this.ReadablePlatformUserIdentifier = _pxuid;
			this.CombinedString = this.PlatformIdentifierString + "_" + _pxuid;
			this.hashcode = (_pxuid.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600B96F RID: 47471 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool DecodeTicket(string _ticket)
		{
			return true;
		}

		// Token: 0x0600B970 RID: 47472 RVA: 0x0046D764 File Offset: 0x0046B964
		public override bool Equals(PlatformUserIdentifierAbs _other)
		{
			if (_other == null)
			{
				return false;
			}
			if (this == _other)
			{
				return true;
			}
			UserIdentifierXbl userIdentifierXbl = _other as UserIdentifierXbl;
			return userIdentifierXbl != null && string.Equals(userIdentifierXbl.pxuid, this.pxuid, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600B971 RID: 47473 RVA: 0x0046D79A File Offset: 0x0046B99A
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x0400910C RID: 37132
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string pxuid;

		// Token: 0x0400910D RID: 37133
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
