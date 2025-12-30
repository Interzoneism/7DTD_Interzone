using System;
using Steamworks;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x020018D2 RID: 6354
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierSteam : PlatformUserIdentifierAbs
	{
		// Token: 0x17001560 RID: 5472
		// (get) Token: 0x0600BBA3 RID: 48035 RVA: 0x00075C39 File Offset: 0x00073E39
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.Steam;
			}
		}

		// Token: 0x17001561 RID: 5473
		// (get) Token: 0x0600BBA4 RID: 48036 RVA: 0x00476368 File Offset: 0x00474568
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.Steam);

		// Token: 0x17001562 RID: 5474
		// (get) Token: 0x0600BBA5 RID: 48037 RVA: 0x00476370 File Offset: 0x00474570
		public override string ReadablePlatformUserIdentifier { get; }

		// Token: 0x17001563 RID: 5475
		// (get) Token: 0x0600BBA6 RID: 48038 RVA: 0x00476378 File Offset: 0x00474578
		public override string CombinedString { get; }

		// Token: 0x17001564 RID: 5476
		// (get) Token: 0x0600BBA7 RID: 48039 RVA: 0x00476380 File Offset: 0x00474580
		// (set) Token: 0x0600BBA8 RID: 48040 RVA: 0x00476388 File Offset: 0x00474588
		public byte[] Ticket
		{
			get
			{
				return this.ticket;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this.ticket = value;
			}
		}

		// Token: 0x0600BBA9 RID: 48041 RVA: 0x00476394 File Offset: 0x00474594
		public UserIdentifierSteam(string _steamId)
		{
			ulong steamId;
			if (_steamId.Length != 17 || !ulong.TryParse(_steamId, out steamId))
			{
				throw new ArgumentException("Not a valid SteamID: " + _steamId, "_steamId");
			}
			this.SteamId = steamId;
			this.ReadablePlatformUserIdentifier = _steamId;
			this.CombinedString = this.PlatformIdentifierString + "_" + _steamId;
			this.hashcode = (steamId.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600BBAA RID: 48042 RVA: 0x0047641C File Offset: 0x0047461C
		public UserIdentifierSteam(ulong _steamId)
		{
			if (_steamId < 10000000000000000UL || _steamId > 99999999999999999UL)
			{
				throw new ArgumentException("Not a valid SteamID: " + _steamId.ToString(), "_steamId");
			}
			this.SteamId = _steamId;
			this.ReadablePlatformUserIdentifier = _steamId.ToString();
			this.CombinedString = this.PlatformIdentifierString + "_" + _steamId.ToString();
			this.hashcode = (_steamId.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600BBAB RID: 48043 RVA: 0x004764BC File Offset: 0x004746BC
		public UserIdentifierSteam(CSteamID _steamId)
		{
			this.SteamId = _steamId.m_SteamID;
			this.ReadablePlatformUserIdentifier = _steamId.ToString();
			string platformIdentifierString = this.PlatformIdentifierString;
			string str = "_";
			CSteamID csteamID = _steamId;
			this.CombinedString = platformIdentifierString + str + csteamID.ToString();
			this.hashcode = (_steamId.m_SteamID.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600BBAC RID: 48044 RVA: 0x00476540 File Offset: 0x00474740
		public override bool DecodeTicket(string _ticket)
		{
			if (string.IsNullOrEmpty(_ticket))
			{
				return false;
			}
			try
			{
				this.Ticket = Convert.FromBase64String(_ticket);
			}
			catch (FormatException ex)
			{
				Log.Error("Convert.FromBase64String: " + ex.Message);
				Log.Exception(ex);
				return false;
			}
			return true;
		}

		// Token: 0x0600BBAD RID: 48045 RVA: 0x0047659C File Offset: 0x0047479C
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
			UserIdentifierSteam userIdentifierSteam = _other as UserIdentifierSteam;
			return userIdentifierSteam != null && userIdentifierSteam.SteamId == this.SteamId;
		}

		// Token: 0x0600BBAE RID: 48046 RVA: 0x004765CE File Offset: 0x004747CE
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x040092B3 RID: 37555
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public byte[] ticket;

		// Token: 0x040092B4 RID: 37556
		public UserIdentifierSteam OwnerId;

		// Token: 0x040092B5 RID: 37557
		public readonly ulong SteamId;

		// Token: 0x040092B6 RID: 37558
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
