using System;
using System.Text.RegularExpressions;
using Epic.OnlineServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Platform.EOS
{
	// Token: 0x0200194C RID: 6476
	[Preserve]
	[DoNotTouchSerializableFlags]
	[Serializable]
	public class UserIdentifierEos : PlatformUserIdentifierAbs
	{
		// Token: 0x170015E1 RID: 5601
		// (get) Token: 0x0600BEE5 RID: 48869 RVA: 0x000282C0 File Offset: 0x000264C0
		public override EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.EOS;
			}
		}

		// Token: 0x170015E2 RID: 5602
		// (get) Token: 0x0600BEE6 RID: 48870 RVA: 0x00486039 File Offset: 0x00484239
		public override string PlatformIdentifierString { get; } = PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.EOS);

		// Token: 0x170015E3 RID: 5603
		// (get) Token: 0x0600BEE7 RID: 48871 RVA: 0x00486041 File Offset: 0x00484241
		public override string ReadablePlatformUserIdentifier
		{
			get
			{
				return this.ProductUserIdString;
			}
		}

		// Token: 0x170015E4 RID: 5604
		// (get) Token: 0x0600BEE8 RID: 48872 RVA: 0x00486049 File Offset: 0x00484249
		public override string CombinedString { get; }

		// Token: 0x0600BEE9 RID: 48873 RVA: 0x00486051 File Offset: 0x00484251
		public static string CreateCombinedString(string _puidString)
		{
			return PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.EOS) + "_" + _puidString;
		}

		// Token: 0x0600BEEA RID: 48874 RVA: 0x00486064 File Offset: 0x00484264
		public static string CreateCombinedString(ProductUserId _puid)
		{
			return PlatformManager.PlatformStringFromEnum(EPlatformIdentifier.EOS) + "_" + UserIdentifierEos.CreateStringFromPuid(_puid);
		}

		// Token: 0x170015E5 RID: 5605
		// (get) Token: 0x0600BEEB RID: 48875 RVA: 0x0048607C File Offset: 0x0048427C
		public string ProductUserIdString
		{
			get
			{
				string result;
				if ((result = this.productUserIdString) == null)
				{
					result = (this.productUserIdString = UserIdentifierEos.CreateStringFromPuid(this.productUserId));
				}
				return result;
			}
		}

		// Token: 0x170015E6 RID: 5606
		// (get) Token: 0x0600BEEC RID: 48876 RVA: 0x004860A8 File Offset: 0x004842A8
		public ProductUserId ProductUserId
		{
			get
			{
				ProductUserId result;
				if ((result = this.productUserId) == null)
				{
					result = (this.productUserId = UserIdentifierEos.CreatePuidFromString(this.productUserIdString));
				}
				return result;
			}
		}

		// Token: 0x170015E7 RID: 5607
		// (get) Token: 0x0600BEED RID: 48877 RVA: 0x004860D3 File Offset: 0x004842D3
		// (set) Token: 0x0600BEEE RID: 48878 RVA: 0x004860DB File Offset: 0x004842DB
		public string Ticket
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

		// Token: 0x0600BEEF RID: 48879 RVA: 0x004860E4 File Offset: 0x004842E4
		public UserIdentifierEos(string _puid)
		{
			if (string.IsNullOrEmpty(_puid))
			{
				throw new ArgumentException("Empty or null PUID", "_puid");
			}
			if (!UserIdentifierEos.puidMatcher.IsMatch(_puid))
			{
				throw new ArgumentException("Invalid PUID '" + _puid + "'", "_puid");
			}
			this.productUserIdString = _puid;
			this.CombinedString = UserIdentifierEos.CreateCombinedString(_puid);
			this.hashcode = (this.ProductUserIdString.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600BEF0 RID: 48880 RVA: 0x00486174 File Offset: 0x00484374
		public UserIdentifierEos(ProductUserId _puid)
		{
			if (_puid == null)
			{
				throw new ArgumentException("Null PUID", "_puid");
			}
			this.productUserId = _puid;
			this.CombinedString = UserIdentifierEos.CreateCombinedString(this.ProductUserIdString);
			this.hashcode = (this.ProductUserIdString.GetHashCode() ^ (int)this.PlatformIdentifier * 397);
		}

		// Token: 0x0600BEF1 RID: 48881 RVA: 0x004861E4 File Offset: 0x004843E4
		public static string CreateStringFromPuid(ProductUserId _puid)
		{
			if (!ThreadManager.IsMainThread())
			{
				Log.Warning("CreateStringFromPuid NOT ON MAIN THREAD! From:\n" + StackTraceUtility.ExtractStackTrace() + "\n");
			}
			if (_puid == null)
			{
				Log.Error("CreateStringFromPuid with null PUID! From:\n" + StackTraceUtility.ExtractStackTrace() + "\n");
				return null;
			}
			return _puid.ToString();
		}

		// Token: 0x0600BEF2 RID: 48882 RVA: 0x0048623B File Offset: 0x0048443B
		public static ProductUserId CreatePuidFromString(string _puidString)
		{
			if (_puidString == null)
			{
				Log.Error("CreatePuidFromString with null PUID string! From:\n" + StackTraceUtility.ExtractStackTrace() + "\n");
				return null;
			}
			return ProductUserId.FromString(_puidString);
		}

		// Token: 0x0600BEF3 RID: 48883 RVA: 0x00486266 File Offset: 0x00484466
		public override bool DecodeTicket(string _ticket)
		{
			if (string.IsNullOrEmpty(_ticket))
			{
				return false;
			}
			this.Ticket = _ticket;
			return true;
		}

		// Token: 0x0600BEF4 RID: 48884 RVA: 0x0048627C File Offset: 0x0048447C
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
			UserIdentifierEos userIdentifierEos = _other as UserIdentifierEos;
			return userIdentifierEos != null && string.Equals(userIdentifierEos.ProductUserIdString, this.ProductUserIdString, StringComparison.Ordinal);
		}

		// Token: 0x0600BEF5 RID: 48885 RVA: 0x004862B2 File Offset: 0x004844B2
		public override int GetHashCode()
		{
			return this.hashcode;
		}

		// Token: 0x0400949E RID: 38046
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex puidMatcher = new Regex("^[0-9a-fA-F]{8,32}$", RegexOptions.Compiled);

		// Token: 0x040094A1 RID: 38049
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public string ticket;

		// Token: 0x040094A2 RID: 38050
		[PublicizedFrom(EAccessModifier.Private)]
		public string productUserIdString;

		// Token: 0x040094A3 RID: 38051
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public ProductUserId productUserId;

		// Token: 0x040094A4 RID: 38052
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int hashcode;
	}
}
