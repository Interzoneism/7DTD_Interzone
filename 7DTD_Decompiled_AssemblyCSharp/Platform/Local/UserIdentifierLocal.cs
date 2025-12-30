using System;
using UnityEngine.Scripting;

namespace Platform.Local
{
	// Token: 0x020018F1 RID: 6385
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierLocal : PlatformUserIdentifierAbs
	{
		// Token: 0x170015A8 RID: 5544
		// (get) Token: 0x0600BCB6 RID: 48310 RVA: 0x000197A5 File Offset: 0x000179A5
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.Local;
			}
		}

		// Token: 0x170015A9 RID: 5545
		// (get) Token: 0x0600BCB7 RID: 48311 RVA: 0x004786FE File Offset: 0x004768FE
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.Local);

		// Token: 0x170015AA RID: 5546
		// (get) Token: 0x0600BCB8 RID: 48312 RVA: 0x00478706 File Offset: 0x00476906
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x170015AB RID: 5547
		// (get) Token: 0x0600BCB9 RID: 48313 RVA: 0x0047870E File Offset: 0x0047690E
		public override string CombinedString { get; }

		// Token: 0x0600BCBA RID: 48314 RVA: 0x00478718 File Offset: 0x00476918
		public UserIdentifierLocal(string _playername)
		{
			if (string.IsNullOrEmpty(_playername))
			{
				throw new ArgumentException("Playername must not be empty", "_playername");
			}
			this.PlayerName = _playername;
			this.ReadablePlatformUserIdentifier = _playername;
			this.CombinedString = this.PlatformIdentifierString + "_" + _playername;
			this.hashcode = (_playername.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600BCBB RID: 48315 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool DecodeTicket(string _ticket)
		{
			return true;
		}

		// Token: 0x0600BCBC RID: 48316 RVA: 0x00478790 File Offset: 0x00476990
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
			UserIdentifierLocal userIdentifierLocal = _other as UserIdentifierLocal;
			return userIdentifierLocal != null && userIdentifierLocal.PlayerName == this.PlayerName;
		}

		// Token: 0x0600BCBD RID: 48317 RVA: 0x004787C5 File Offset: 0x004769C5
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x04009305 RID: 37637
		public readonly string PlayerName;

		// Token: 0x04009306 RID: 37638
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
