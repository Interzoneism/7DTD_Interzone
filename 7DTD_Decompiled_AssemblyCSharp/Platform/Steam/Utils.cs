using System;
using InControl;
using Platform.Shared;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x020018D3 RID: 6355
	public class Utils : IUtils
	{
		// Token: 0x0600BBAF RID: 48047 RVA: 0x004765D6 File Offset: 0x004747D6
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600BBB0 RID: 48048 RVA: 0x004765DF File Offset: 0x004747DF
		public bool OpenBrowser(string _url)
		{
			if (global::Utils.IsValidWebUrl(ref _url))
			{
				SteamFriends.ActivateGameOverlayToWebPage(_url, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
			}
			return true;
		}

		// Token: 0x0600BBB1 RID: 48049 RVA: 0x004765F4 File Offset: 0x004747F4
		public string GetPlatformLanguage()
		{
			if (GameManager.IsDedicatedServer)
			{
				return "english";
			}
			if (this.owner.Api.ClientApiStatus != EApiStatus.Ok)
			{
				Log.Warning("[Steam] Unable to get platform language, Steam not initialized");
				return "english";
			}
			string text = SteamUtils.GetSteamUILanguage().ToLower();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "english";
		}

		// Token: 0x0600BBB2 RID: 48050 RVA: 0x0047664C File Offset: 0x0047484C
		public string GetAppLanguage()
		{
			if (GameManager.IsDedicatedServer)
			{
				return "english";
			}
			if (this.owner.Api.ClientApiStatus != EApiStatus.Ok)
			{
				Log.Warning("[Steam] Unable to get app language, Steam not initialized");
				return "english";
			}
			string text = SteamApps.GetCurrentGameLanguage().ToLower();
			if (text == "latam")
			{
				text = "spanish";
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "english";
		}

		// Token: 0x0600BBB3 RID: 48051 RVA: 0x004766B5 File Offset: 0x004748B5
		public string GetCountry()
		{
			if (GameManager.IsDedicatedServer)
			{
				return "??";
			}
			if (this.owner.Api.ClientApiStatus == EApiStatus.Ok)
			{
				return SteamUtils.GetIPCountry();
			}
			Log.Warning("[Steam] Unable to get country, Steam not initialized");
			return "??";
		}

		// Token: 0x0600BBB4 RID: 48052 RVA: 0x004766EB File Offset: 0x004748EB
		public void ClearTempFiles()
		{
			Platform.Shared.Utils.TryDeleteTempCacheContents();
		}

		// Token: 0x0600BBB5 RID: 48053 RVA: 0x004766F2 File Offset: 0x004748F2
		public string GetTempFileName(string prefix = "", string suffix = "")
		{
			return Platform.Shared.Utils.GetRandomTempCacheFileName(prefix, suffix);
		}

		// Token: 0x0600BBB6 RID: 48054 RVA: 0x00002914 File Offset: 0x00000B14
		public void ControllerDisconnected(InputDevice inputDevice)
		{
		}

		// Token: 0x0600BBB7 RID: 48055 RVA: 0x004766FB File Offset: 0x004748FB
		public string GetCrossplayPlayerIcon(EPlayGroup _playGroup, bool _fetchGenericIcons, EPlatformIdentifier _nativePlatform = EPlatformIdentifier.None)
		{
			switch (_playGroup)
			{
			case EPlayGroup.Standalone:
				if (_fetchGenericIcons)
				{
					return "ui_platform_pc";
				}
				break;
			case EPlayGroup.XBS:
				if (_fetchGenericIcons)
				{
					return "ui_platform_console";
				}
				break;
			case EPlayGroup.PS5:
				if (_fetchGenericIcons)
				{
					return "ui_platform_console";
				}
				break;
			}
			return string.Empty;
		}

		// Token: 0x040092B7 RID: 37559
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;
	}
}
