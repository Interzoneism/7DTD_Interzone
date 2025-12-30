using System;
using System.Collections;
using System.Collections.Generic;
using Platform;
using UnityEngine;

// Token: 0x02000FCF RID: 4047
public class InviteManager
{
	// Token: 0x17000D63 RID: 3427
	// (get) Token: 0x060080E3 RID: 32995 RVA: 0x00345AFC File Offset: 0x00343CFC
	public static InviteManager Instance
	{
		get
		{
			if (InviteManager._instance == null)
			{
				InviteManager._instance = new InviteManager();
				PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _)
				{
					InviteManager._instance.listeners = PlatformManager.MultiPlatform.InviteListeners;
					if (LaunchPrefs.SessionInvite.Value.Length > 3)
					{
						string text = LaunchPrefs.SessionInvite.Value.Substring(0, 3);
						foreach (IJoinSessionGameInviteListener joinSessionGameInviteListener in InviteManager._instance.listeners)
						{
							if (text == joinSessionGameInviteListener.GetListenerIdentifier())
							{
								InviteManager._instance.pendingListener = joinSessionGameInviteListener;
								InviteManager._instance.pendingInvite = LaunchPrefs.SessionInvite.Value.Substring(3);
								break;
							}
						}
						Log.Error("[InviteManager] Invite string not formatted correctly. The identifier \"" + text + "\" did not match an existing listener");
					}
				};
			}
			return InviteManager._instance;
		}
	}

	// Token: 0x060080E4 RID: 32996 RVA: 0x00345B50 File Offset: 0x00343D50
	public void Update()
	{
		if (this.listeners == null || this.listeners.Count == 0)
		{
			return;
		}
		if (!XUiC_MainMenu.openedOnce)
		{
			return;
		}
		if (this.CheckForInvites())
		{
			this.StopJoinCoroutine();
		}
		if (string.IsNullOrEmpty(this.pendingInvite) || this.pendingListener == null)
		{
			if (this.joinInviteCoroutine != null)
			{
				this.StopJoinCoroutine();
			}
			return;
		}
		if (this.joinInviteCoroutine != null)
		{
			return;
		}
		if (XUiC_WorldGenerationWindowGroup.IsGenerating())
		{
			XUiC_WorldGenerationWindowGroup.CancelGeneration();
			return;
		}
		if (this.profileReady)
		{
			this.joinInviteCoroutine = ThreadManager.StartCoroutine(this.StartJoinIntentCoroutine());
			return;
		}
		if (this.creatingProfile)
		{
			return;
		}
		if (ProfileSDF.CurrentProfileName().Length == 0)
		{
			this.creatingProfile = true;
			XUiC_OptionsProfiles.Open(LocalPlayerUI.primaryUI.xui, delegate
			{
				this.profileReady = true;
				this.creatingProfile = false;
			});
			return;
		}
		this.profileReady = true;
	}

	// Token: 0x060080E5 RID: 32997 RVA: 0x00345C1C File Offset: 0x00343E1C
	public bool HasPendingInvite()
	{
		this.CheckForInvites();
		return !string.IsNullOrEmpty(this.pendingInvite);
	}

	// Token: 0x060080E6 RID: 32998 RVA: 0x00345C33 File Offset: 0x00343E33
	public bool IsConnectingToInvite()
	{
		return this.connectingToSession;
	}

	// Token: 0x060080E7 RID: 32999 RVA: 0x00345C3B File Offset: 0x00343E3B
	public IEnumerable<string> GetCommandLineArguments()
	{
		this.CheckForInvites();
		if (string.IsNullOrEmpty(this.commandLineInvite) && !string.IsNullOrEmpty(this.pendingInvite))
		{
			this.commandLineInvite = this.pendingListener.GetListenerIdentifier() + this.pendingInvite;
		}
		if (string.IsNullOrEmpty(this.commandLineInvite))
		{
			yield break;
		}
		yield return LaunchPrefs.SessionInvite.ToCommandLine(this.commandLineInvite);
		yield break;
	}

	// Token: 0x060080E8 RID: 33000 RVA: 0x00345C4C File Offset: 0x00343E4C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckForInvites()
	{
		if (this.listeners == null || this.listeners.Count == 0)
		{
			return false;
		}
		foreach (IJoinSessionGameInviteListener joinSessionGameInviteListener in this.listeners)
		{
			ValueTuple<string, string> valueTuple = joinSessionGameInviteListener.TakePendingInvite();
			string item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			if (!string.IsNullOrEmpty(item))
			{
				this.pendingListener = joinSessionGameInviteListener;
				this.pendingInvite = item;
				this.pendingPassword = item2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060080E9 RID: 33001 RVA: 0x00345CE4 File Offset: 0x00343EE4
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator StartJoinIntentCoroutine()
	{
		InviteManager.<>c__DisplayClass17_0 CS$<>8__locals1 = new InviteManager.<>c__DisplayClass17_0();
		CS$<>8__locals1.<>4__this = this;
		while (!GameManager.Instance.IsSafeToDisconnect())
		{
			yield return null;
		}
		CS$<>8__locals1.warningAccepted = true;
		yield return CS$<>8__locals1.<StartJoinIntentCoroutine>g__RequestPlayerWarning|0();
		if (!CS$<>8__locals1.warningAccepted)
		{
			this.StopJoinCoroutine();
			yield break;
		}
		CS$<>8__locals1.hasPrivileges = true;
		yield return CS$<>8__locals1.<StartJoinIntentCoroutine>g__CheckMultiplayerPrivileges|1();
		if (!CS$<>8__locals1.hasPrivileges)
		{
			this.StopJoinCoroutine();
			yield break;
		}
		CS$<>8__locals1.gameDisconnected = true;
		yield return CS$<>8__locals1.<StartJoinIntentCoroutine>g__ShowDisconnectDialog|2();
		if (!CS$<>8__locals1.gameDisconnected)
		{
			this.StopJoinCoroutine();
			yield break;
		}
		if (PlatformApplicationManager.IsRestartRequired)
		{
			while (!GameManager.Instance.IsSafeToDisconnect())
			{
				yield return null;
			}
			this.commandLineInvite = this.pendingListener.GetListenerIdentifier() + this.pendingInvite;
			GameManager.Instance.Disconnect();
			this.StopJoinCoroutine();
			yield break;
		}
		yield return CS$<>8__locals1.<StartJoinIntentCoroutine>g__ConnectToSession|3();
		yield break;
	}

	// Token: 0x060080EA RID: 33002 RVA: 0x00345CF3 File Offset: 0x00343EF3
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopJoinCoroutine()
	{
		if (this.joinInviteCoroutine == null)
		{
			return;
		}
		this.pendingInvite = null;
		this.pendingListener = null;
		ThreadManager.StopCoroutine(this.joinInviteCoroutine);
		this.joinInviteCoroutine = null;
		this.connectingToSession = false;
	}

	// Token: 0x060080EB RID: 33003 RVA: 0x00345D25 File Offset: 0x00343F25
	public static IEnumerator HandleSessionIdInvite(string _sessionId, string _password, Action<bool> _onFinished)
	{
		InviteManager.<>c__DisplayClass19_0 CS$<>8__locals1 = new InviteManager.<>c__DisplayClass19_0();
		if (string.IsNullOrEmpty(_sessionId))
		{
			if (_onFinished != null)
			{
				_onFinished(false);
			}
			yield break;
		}
		CS$<>8__locals1.serverInfo = null;
		yield return CS$<>8__locals1.<HandleSessionIdInvite>g__RequestSessionDetails|0(_sessionId);
		if (CS$<>8__locals1.serverInfo == null)
		{
			Log.Error("[InviteManager] Failed to find server details for session " + _sessionId + ".");
			if (_onFinished != null)
			{
				_onFinished(false);
			}
			yield break;
		}
		if (CS$<>8__locals1.serverInfo.AllowsCrossplay)
		{
			InviteManager.<>c__DisplayClass19_1 CS$<>8__locals2 = new InviteManager.<>c__DisplayClass19_1();
			XUiC_MultiplayerPrivilegeNotification window = XUiC_MultiplayerPrivilegeNotification.GetWindow();
			if (window == null)
			{
				Log.Error("[InviteManager] Could not find privilege notification window.");
				if (_onFinished != null)
				{
					_onFinished(false);
				}
				yield break;
			}
			CS$<>8__locals2.crossplayResult = null;
			window.ResolvePrivilegesWithDialog(EUserPerms.Crossplay, delegate(bool result)
			{
				CS$<>8__locals2.crossplayResult = new bool?(result);
			}, 0f, true, delegate
			{
				CS$<>8__locals2.crossplayResult = new bool?(false);
			});
			while (CS$<>8__locals2.crossplayResult == null)
			{
				yield return null;
			}
			yield return null;
			while (XUiC_ProgressWindow.IsWindowOpen())
			{
				yield return null;
			}
			if (!CS$<>8__locals2.crossplayResult.Value)
			{
				Log.Error("[InviteManager] Could not join game. The server allows crossplay but crossplay is not allowed by the local user.");
				if (_onFinished != null)
				{
					_onFinished(false);
				}
				yield break;
			}
			CS$<>8__locals2 = null;
		}
		else if (!CS$<>8__locals1.serverInfo.PlayGroup.IsCurrent())
		{
			Log.Error(string.Format("[InviteManager] Could not join game. The server does not have crossplay enabled and is in a different play group: {0}", CS$<>8__locals1.serverInfo.PlayGroup));
			if (_onFinished != null)
			{
				_onFinished(false);
			}
			string title = Localization.Get("xuiConnectionDenied", false);
			string text = string.Format(Localization.Get("auth_unsupportedplatform", false), Localization.Get("platformName" + PlatformManager.NativePlatform.PlatformIdentifier.ToStringCached<EPlatformIdentifier>(), false));
			XUiC_MessageBoxWindowGroup.ShowMessageBox(LocalPlayerUI.primaryUI.xui, title, text, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
			yield break;
		}
		if (!string.IsNullOrEmpty(_password))
		{
			ServerInfoCache.Instance.SavePassword(CS$<>8__locals1.serverInfo, _password);
		}
		Log.Out("[InviteManager] Got server details, trying to connect");
		if (_onFinished != null)
		{
			_onFinished(true);
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.Connect(CS$<>8__locals1.serverInfo);
		yield break;
	}

	// Token: 0x060080EC RID: 33004 RVA: 0x00345D42 File Offset: 0x00343F42
	public static IEnumerator HandleIpPortInvite(string _ip, int _port, string _password, Action<bool> _onFinished)
	{
		if (string.IsNullOrEmpty(_ip))
		{
			if (_onFinished != null)
			{
				_onFinished(false);
			}
			yield break;
		}
		if (_port < 1 || _port > 65530)
		{
			if (_onFinished != null)
			{
				_onFinished(false);
			}
			yield break;
		}
		GameServerInfo gameServerInfo = new GameServerInfo();
		gameServerInfo.SetValue(GameInfoString.IP, _ip);
		gameServerInfo.SetValue(GameInfoInt.Port, _port);
		PermissionsManager.IsCrossplayAllowed();
		if (!string.IsNullOrEmpty(_password))
		{
			ServerInfoCache.Instance.SavePassword(gameServerInfo, _password);
		}
		Log.Out("[InviteManager] Got server IP/port, trying to connect");
		if (_onFinished != null)
		{
			_onFinished(true);
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.Connect(gameServerInfo);
		yield break;
	}

	// Token: 0x04006395 RID: 25493
	[PublicizedFrom(EAccessModifier.Private)]
	public static InviteManager _instance;

	// Token: 0x04006396 RID: 25494
	[PublicizedFrom(EAccessModifier.Private)]
	public IList<IJoinSessionGameInviteListener> listeners;

	// Token: 0x04006397 RID: 25495
	[PublicizedFrom(EAccessModifier.Private)]
	public IJoinSessionGameInviteListener pendingListener;

	// Token: 0x04006398 RID: 25496
	[PublicizedFrom(EAccessModifier.Private)]
	public string pendingInvite;

	// Token: 0x04006399 RID: 25497
	[PublicizedFrom(EAccessModifier.Private)]
	public string pendingPassword;

	// Token: 0x0400639A RID: 25498
	[PublicizedFrom(EAccessModifier.Private)]
	public string commandLineInvite;

	// Token: 0x0400639B RID: 25499
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine joinInviteCoroutine;

	// Token: 0x0400639C RID: 25500
	[PublicizedFrom(EAccessModifier.Private)]
	public bool connectingToSession;

	// Token: 0x0400639D RID: 25501
	[PublicizedFrom(EAccessModifier.Private)]
	public bool profileReady;

	// Token: 0x0400639E RID: 25502
	[PublicizedFrom(EAccessModifier.Private)]
	public bool creatingProfile;
}
