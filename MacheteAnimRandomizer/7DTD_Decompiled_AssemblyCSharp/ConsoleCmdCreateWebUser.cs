using System;
using System.Collections.Generic;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001CB RID: 459
[Preserve]
public class ConsoleCmdCreateWebUser : ConsoleCmdAbstract
{
	// Token: 0x06000DDD RID: 3549 RVA: 0x0005CA9A File Offset: 0x0005AC9A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"createwebuser"
		};
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0005CAAA File Offset: 0x0005ACAA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Create a web dashboard user account";
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x0002B133 File Offset: 0x00029333
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0005CAB4 File Offset: 0x0005ACB4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_senderInfo.NetworkConnection != null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be executed from the in-game console.");
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (_senderInfo.IsLocalGame)
			{
				string @string = GamePrefs.GetString(EnumGamePrefs.PlayerName);
				PlatformUserIdentifierAbs platformUserId = PlatformManager.NativePlatform.User.PlatformUserId;
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				PlatformUserIdentifierAbs crossPlatformUserId;
				if (crossplatformPlatform == null)
				{
					crossPlatformUserId = null;
				}
				else
				{
					IUserClient user = crossplatformPlatform.User;
					crossPlatformUserId = ((user != null) ? user.PlatformUserId : null);
				}
				string token = this.createToken(@string, platformUserId, crossPlatformUserId);
				string url = this.createRegistrationPageUrl(token, true);
				this.openUserRegistrationPage(url);
				return;
			}
			string token2 = this.createToken(_senderInfo.RemoteClientInfo.playerName, _senderInfo.RemoteClientInfo.PlatformId, _senderInfo.RemoteClientInfo.CrossplatformId);
			string s = this.createRegistrationPageUrl(token2, false);
			_senderInfo.RemoteClientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup(this.GetCommands()[0] + " " + Convert.ToBase64String(Encoding.UTF8.GetBytes(s)), true));
			return;
		}
		else
		{
			if (_params.Count < 1)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Missing URL in server reply");
				return;
			}
			string string2 = Encoding.UTF8.GetString(Convert.FromBase64String(_params[0]));
			this.openUserRegistrationPage(string2);
			return;
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x0005CBE0 File Offset: 0x0005ADE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void openUserRegistrationPage(string _url)
	{
		GameManager.Instance.SetConsoleWindowVisible(false);
		XUiC_MessageBoxWindowGroup.ShowUrlConfirmationDialog(LocalPlayerUI.GetUIForPrimaryPlayer().xui, _url, true, new Func<string, bool>(Utils.OpenSystemBrowser), null, Localization.Get("xuiOpenUserCreationConfirmationText", false), null);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Requested browser for user creation at " + _url);
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0005CC38 File Offset: 0x0005AE38
	[PublicizedFrom(EAccessModifier.Private)]
	public string createRegistrationPageUrl(string _token, bool _isLocalOnListenServer = false)
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.WebDashboardPort);
		string str;
		if (_isLocalOnListenServer)
		{
			str = string.Format("http://localhost:{0}/", @int);
		}
		else
		{
			string @string = GamePrefs.GetString(EnumGamePrefs.WebDashboardUrl);
			if (!string.IsNullOrEmpty(@string))
			{
				string str2 = @string;
				string text = @string;
				str = str2 + ((text[text.Length - 1] == '/') ? "" : "/");
			}
			else
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Server does not specify an explicit WebDashboardUrl, using game server's public IP");
				str = string.Format("http://{0}:{1}/", SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoString.IP), @int);
			}
		}
		return str + "app/createuser?token=" + _token;
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x0005CCDC File Offset: 0x0005AEDC
	[PublicizedFrom(EAccessModifier.Private)]
	public string createToken(string _playerName, PlatformUserIdentifierAbs _platformUserId, PlatformUserIdentifierAbs _crossPlatformUserId)
	{
		return "-requires WebDashboard code-";
	}

	// Token: 0x04000AED RID: 2797
	[PublicizedFrom(EAccessModifier.Private)]
	public const string registrationPagePath = "app/createuser";
}
