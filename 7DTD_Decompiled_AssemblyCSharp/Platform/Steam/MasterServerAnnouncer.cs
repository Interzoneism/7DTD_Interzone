using System;
using System.Collections;
using System.Net;
using Steamworks;
using UnityEngine;

namespace Platform.Steam
{
	// Token: 0x020018C3 RID: 6339
	public class MasterServerAnnouncer : IMasterServerAnnouncer
	{
		// Token: 0x0600BB23 RID: 47907 RVA: 0x00473941 File Offset: 0x00471B41
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600BB24 RID: 47908 RVA: 0x0047394C File Offset: 0x00471B4C
		public void Update()
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			this.tickDurationStopwatch.Restart();
			GameServer.RunCallbacks();
			long num = this.tickDurationStopwatch.ElapsedMicroseconds / 1000L;
			if (num > 25L)
			{
				Log.Warning(string.Format("[SteamServer] Tick took exceptionally long: {0} ms", num));
			}
			if (this.localGameServerId == CSteamID.Nil)
			{
				return;
			}
			if (this.updateTagsCountdown.HasPassed())
			{
				this.updateTagsCountdown.Reset();
				SteamGameServer.SetGameTags(NetworkUtils.BuildGameTags(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo));
			}
		}

		// Token: 0x17001553 RID: 5459
		// (get) Token: 0x0600BB25 RID: 47909 RVA: 0x004739DF File Offset: 0x00471BDF
		// (set) Token: 0x0600BB26 RID: 47910 RVA: 0x004739E7 File Offset: 0x00471BE7
		public bool GameServerInitialized { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600BB27 RID: 47911 RVA: 0x004739F0 File Offset: 0x00471BF0
		public void AdvertiseServer(Action _onServerRegistered)
		{
			if (this.owner.User.UserStatus == EUserStatus.OfflineMode)
			{
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			this.onServerRegistered = _onServerRegistered;
			this.registerGameCoroutine = this.RegisterGame();
			ThreadManager.StartCoroutine(this.registerGameCoroutine);
		}

		// Token: 0x0600BB28 RID: 47912 RVA: 0x00473A30 File Offset: 0x00471C30
		public void StopServer()
		{
			Log.Out("[Steamworks.NET] Stopping server");
			if (this.registerGameCoroutine != null)
			{
				ThreadManager.StopCoroutine(this.registerGameCoroutine);
				this.registerGameCoroutine = null;
			}
			if (this.localGameServerId != CSteamID.Nil)
			{
				if (!GameManager.IsDedicatedServer && !this.owner.AsServerOnly)
				{
					SteamUser.AdvertiseGame(CSteamID.Nil, 0U, 0);
				}
				SteamGameServer.SetAdvertiseServerActive(false);
				SteamGameServer.LogOff();
			}
			if (this.GameServerInitialized)
			{
				this.GameServerInitialized = false;
				GameServer.Shutdown();
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance != null && SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo != null)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedString -= this.updateSteamKeysString;
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedInt -= this.updateSteamKeysInt;
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedBool -= this.updateSteamKeysBool;
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedAny -= this.updateSteamKeys;
			}
			this.localGameServerId = CSteamID.Nil;
			this.onServerRegistered = null;
		}

		// Token: 0x0600BB29 RID: 47913 RVA: 0x00473B49 File Offset: 0x00471D49
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RegisterGame()
		{
			yield return null;
			if (!GameServer.Init(NetworkUtils.ToInt(IPAddress.Any.ToString()), (ushort)SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoInt.Port), (ushort)SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoInt.Port), EServerMode.eServerModeAuthentication, Platform.Steam.Constants.SteamVersionNr))
			{
				Log.Error("[Steamworks.NET] Could not initialize GameServer");
				Action action = this.onServerRegistered;
				if (action != null)
				{
					action();
				}
				this.onServerRegistered = null;
				yield break;
			}
			yield return null;
			this.GameServerInitialized = true;
			this.owner.Api.ServerApiLoaded();
			Log.Out("[Steamworks.NET] GameServer.Init successful");
			SteamGameServer.SetDedicatedServer(GameManager.IsDedicatedServer);
			SteamGameServer.SetModDir("7DTD");
			SteamGameServer.SetProduct("7DTD");
			SteamGameServer.SetGameDescription("7 Days To Die");
			SteamGameServer.SetMaxPlayerCount(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoInt.MaxPlayers));
			SteamGameServer.SetBotPlayerCount(0);
			SteamGameServer.SetPasswordProtected(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoBool.IsPasswordProtected));
			SteamGameServer.SetMapName(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoString.LevelName));
			SteamGameServer.SetServerName(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoString.GameHost));
			SteamGameServer.SetGameTags(NetworkUtils.BuildGameTags(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo));
			SteamGameServer.LogOnAnonymous();
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoInt.ServerVisibility) == 2)
			{
				Log.Out("[Steamworks.NET] Making server public");
				SteamGameServer.SetAdvertiseServerActive(true);
			}
			float loginTimeout = 30f;
			while (!SteamGameServer.BLoggedOn() && loginTimeout > 0f)
			{
				yield return null;
				loginTimeout -= Time.unscaledDeltaTime;
			}
			if (SteamGameServer.BLoggedOn())
			{
				string text = SteamGameServer.GetPublicIP().ToString();
				this.localGameServerId = SteamGameServer.GetSteamID();
				Log.Out("[Steamworks.NET] GameServer.LogOn successful, SteamID=" + this.localGameServerId.m_SteamID.ToString() + ", public IP=" + Utils.MaskIp(text));
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoString.SteamID, this.localGameServerId.m_SteamID.ToString());
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoString.IP, text);
				if (PlatformManager.CrossplatformPlatform == null)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoString.UniqueId, this.localGameServerId.ToString());
				}
				GamePrefs.Set(EnumGamePrefs.ServerIP, text);
				this.SetGameServerInfo(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance != null && SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo != null)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedString += this.updateSteamKeysString;
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedInt += this.updateSteamKeysInt;
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedBool += this.updateSteamKeysBool;
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedAny += this.updateSteamKeys;
				}
			}
			else
			{
				Log.Error("[Steamworks.NET] GameServer.LogOn timed out");
			}
			if (this.onServerRegistered != null)
			{
				this.onServerRegistered();
				this.onServerRegistered = null;
			}
			this.registerGameCoroutine = null;
			yield break;
		}

		// Token: 0x0600BB2A RID: 47914 RVA: 0x00473B58 File Offset: 0x00471D58
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetGameServerInfo(GameServerInfo _gameServerInfo)
		{
			SteamGameServer.ClearAllKeyValues();
			foreach (GameInfoString gameInfoString in EnumUtils.Values<GameInfoString>())
			{
				SteamGameServer.SetKeyValue(gameInfoString.ToStringCached<GameInfoString>(), _gameServerInfo.GetValue(gameInfoString));
			}
			foreach (GameInfoInt gameInfoInt in EnumUtils.Values<GameInfoInt>())
			{
				SteamGameServer.SetKeyValue(gameInfoInt.ToStringCached<GameInfoInt>(), _gameServerInfo.GetValue(gameInfoInt).ToString());
			}
			foreach (GameInfoBool gameInfoBool in EnumUtils.Values<GameInfoBool>())
			{
				SteamGameServer.SetKeyValue(gameInfoBool.ToStringCached<GameInfoBool>(), _gameServerInfo.GetValue(gameInfoBool).ToString());
			}
		}

		// Token: 0x0600BB2B RID: 47915 RVA: 0x00473C5C File Offset: 0x00471E5C
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSteamKeysString(GameServerInfo _gameServerInfo, GameInfoString _gameInfoString)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (this.localGameServerId == CSteamID.Nil)
			{
				return;
			}
			SteamGameServer.SetKeyValue(_gameInfoString.ToStringCached<GameInfoString>(), _gameServerInfo.GetValue(_gameInfoString));
			if (!this.updateTagsCountdown.IsRunning)
			{
				this.updateTagsCountdown.ResetAndRestart();
			}
		}

		// Token: 0x0600BB2C RID: 47916 RVA: 0x00473CB0 File Offset: 0x00471EB0
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSteamKeysInt(GameServerInfo _gameServerInfo, GameInfoInt _gameInfoInt)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (this.localGameServerId == CSteamID.Nil)
			{
				return;
			}
			SteamGameServer.SetKeyValue(_gameInfoInt.ToStringCached<GameInfoInt>(), _gameServerInfo.GetValue(_gameInfoInt).ToString());
			if (!this.updateTagsCountdown.IsRunning)
			{
				this.updateTagsCountdown.ResetAndRestart();
			}
		}

		// Token: 0x0600BB2D RID: 47917 RVA: 0x00473D0C File Offset: 0x00471F0C
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSteamKeysBool(GameServerInfo _gameServerInfo, GameInfoBool _gameInfoBool)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (this.localGameServerId == CSteamID.Nil)
			{
				return;
			}
			SteamGameServer.SetKeyValue(_gameInfoBool.ToStringCached<GameInfoBool>(), _gameServerInfo.GetValue(_gameInfoBool).ToString());
			if (!this.updateTagsCountdown.IsRunning)
			{
				this.updateTagsCountdown.ResetAndRestart();
			}
		}

		// Token: 0x0600BB2E RID: 47918 RVA: 0x00473D68 File Offset: 0x00471F68
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSteamKeys(GameServerInfo _gameServerInfo)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (this.localGameServerId == CSteamID.Nil)
			{
				return;
			}
			SteamGameServer.SetMapName(_gameServerInfo.GetValue(GameInfoString.LevelName));
			if (!this.updateTagsCountdown.IsRunning)
			{
				this.updateTagsCountdown.ResetAndRestart();
			}
		}

		// Token: 0x0600BB2F RID: 47919 RVA: 0x00473DB8 File Offset: 0x00471FB8
		public string GetServerPorts()
		{
			return GamePrefs.GetInt(EnumGamePrefs.ServerPort).ToString() + "/UDP, " + (GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 1).ToString() + "/UDP";
		}

		// Token: 0x04009258 RID: 37464
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04009259 RID: 37465
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CountdownTimer updateTagsCountdown = new CountdownTimer(300f, false);

		// Token: 0x0400925A RID: 37466
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator registerGameCoroutine;

		// Token: 0x0400925B RID: 37467
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onServerRegistered;

		// Token: 0x0400925C RID: 37468
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MicroStopwatch tickDurationStopwatch = new MicroStopwatch(false);

		// Token: 0x0400925E RID: 37470
		[PublicizedFrom(EAccessModifier.Private)]
		public CSteamID localGameServerId = CSteamID.Nil;
	}
}
