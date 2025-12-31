using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.Platforms;
using Platform;
using Twitch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000F91 RID: 3985
public class GameSparksManager : MonoBehaviour
{
	// Token: 0x17000D44 RID: 3396
	// (get) Token: 0x06007F0E RID: 32526 RVA: 0x00339806 File Offset: 0x00337A06
	public int sessionUpdateIntervalSec
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return (this.DebugEnabled ? 2 : 15) * 60;
		}
	}

	// Token: 0x06007F0F RID: 32527 RVA: 0x00339818 File Offset: 0x00337A18
	public static GameSparksManager Instance()
	{
		if (!(GameSparksManager.instance != null))
		{
			return GameSparksManager.instance;
		}
		if (!GS.Available && GS.Instance == null)
		{
			Log.Warning("[GSM] Connection Lost, attempting to Reconnect...");
			GS.Reconnect();
			return null;
		}
		return GameSparksManager.instance;
	}

	// Token: 0x17000D45 RID: 3397
	// (get) Token: 0x06007F10 RID: 32528 RVA: 0x00339851 File Offset: 0x00337A51
	public bool DebugEnabled
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GameSparksSettings.DebugBuild;
		}
	}

	// Token: 0x06007F11 RID: 32529 RVA: 0x0012CE9D File Offset: 0x0012B09D
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06007F12 RID: 32530 RVA: 0x00339858 File Offset: 0x00337A58
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Start()
	{
		base.gameObject.AddComponent<GameSparksUnity>();
		GS.GameSparksAvailable = (Action<bool>)Delegate.Combine(GS.GameSparksAvailable, new Action<bool>(delegate(bool _gsAvailable)
		{
			if (_gsAvailable)
			{
				Log.Out("[GSM] GameSparks Connected");
				return;
			}
			Log.Out("[GSM] GameSparks Disconnected");
		}));
		yield return new WaitUntil(() => GS.Available);
		yield return new WaitForSeconds(1f);
		this.DeviceAuth(new GameSparksManager.OnPlayerAuthenticated(this.PlayerAuthenticated), new GameSparksManager.OnError(this.AuthError));
		yield break;
	}

	// Token: 0x06007F13 RID: 32531 RVA: 0x00339868 File Offset: 0x00337A68
	[PublicizedFrom(EAccessModifier.Private)]
	public void AuthError(string _error)
	{
		if (_error.Contains("cannot authenticate this month"))
		{
			Log.Out("[GSM] Skipping me");
			return;
		}
		if (_error.Contains("timeout"))
		{
			Log.Error("[GSM] AuthError TimeOut");
			return;
		}
		if (_error.Contains("UNRECOGNISED"))
		{
			Log.Error("[GSM] AuthError UNRECOGNISED");
			return;
		}
		Log.Error("[GSM] AuthError" + _error);
	}

	// Token: 0x06007F14 RID: 32532 RVA: 0x003398CD File Offset: 0x00337ACD
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerAuthenticated(PlayerDetails _playerDetails)
	{
		this.ProgramStarted();
	}

	// Token: 0x06007F15 RID: 32533 RVA: 0x003398D8 File Offset: 0x00337AD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void DeviceAuth(GameSparksManager.OnPlayerAuthenticated _onPlayerAuthSuccess, GameSparksManager.OnError _onAuthError)
	{
		if (this.DebugEnabled)
		{
			Log.Out("AuthRequest with DeviceID " + base.gameObject.GetComponent<PlatformBase>().DeviceId);
		}
		new DeviceAuthenticationRequest().SetDurable(true).Send(delegate(AuthenticationResponse _response)
		{
			if (!_response.HasErrors && _onPlayerAuthSuccess != null)
			{
				Log.Out("[GSM] Device Authentication suceeded ...");
				this.authenticated = true;
				_onPlayerAuthSuccess(new PlayerDetails(_response.DisplayName, _response.UserId, _response.ScriptData));
				return;
			}
			this.authenticated = false;
			if (_onAuthError == null)
			{
				Log.Warning("[GSM] Device Authentication failed ...");
			}
			GameSparksManager.OnError onAuthError = _onAuthError;
			if (onAuthError == null)
			{
				return;
			}
			onAuthError(_response.Errors.JSON);
		});
	}

	// Token: 0x06007F16 RID: 32534 RVA: 0x00339944 File Offset: 0x00337B44
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrepareAndSendRequest(GSRequestData _data, string _eventKey)
	{
		if (!this.authenticated)
		{
			if (this.DebugEnabled)
			{
				Log.Out(_eventKey + " not sent: not authenticated");
			}
			return;
		}
		if (this.DebugEnabled)
		{
			Log.Out(_eventKey + " data: " + _data.JSON);
		}
		_data.AddNumber("currentGameVersion", Constants.cVersionInformation.NumericalRepresentation);
		LogEventRequest request = new LogEventRequest().SetEventKey(_eventKey).SetEventAttribute("sessionDetails", _data);
		this.SendRequest(request, _eventKey);
	}

	// Token: 0x06007F17 RID: 32535 RVA: 0x003399C8 File Offset: 0x00337BC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendRequest(LogEventRequest _request, string _eventKey)
	{
		_request.Send(delegate(LogEventResponse _response)
		{
			if (!_response.HasErrors)
			{
				if (this.DebugEnabled)
				{
					Log.Out("[GSM] " + _eventKey + " success");
					return;
				}
			}
			else
			{
				Log.Out("[GSM] " + _eventKey + " ERROR: " + _response.Errors.JSON);
			}
		});
	}

	// Token: 0x06007F18 RID: 32536 RVA: 0x003399FC File Offset: 0x00337BFC
	public void ProgramStarted()
	{
		string eventKey = "PROGRAM_START";
		GSRequestData gsrequestData = new GSRequestData();
		gsrequestData.AddString("uniqueID", base.gameObject.GetComponent<PlatformBase>().DeviceId);
		gsrequestData.AddBoolean("IsDedicated", GameManager.IsDedicatedServer);
		GSRequestData gsrequestData2 = new GSRequestData();
		gsrequestData2.AddString("Text", Constants.cVersionInformation.ShortString);
		gsrequestData2.AddString("LongText", Constants.cVersionInformation.LongString);
		gsrequestData2.AddString("Type", Constants.cVersionInformation.ReleaseType.ToStringCached<VersionInformation.EGameReleaseType>());
		gsrequestData2.AddNumber("Major", Constants.cVersionInformation.Major);
		gsrequestData2.AddNumber("Minor", Constants.cVersionInformation.Minor);
		gsrequestData2.AddNumber("Build", Constants.cVersionInformation.Build);
		gsrequestData.AddObject("GameVersion", gsrequestData2);
		if (!GameManager.IsDedicatedServer)
		{
			gsrequestData.AddString("BuildPlatform", Application.platform.ToStringCached<RuntimePlatform>());
			gsrequestData.AddString("OperatingSystemFamily", SystemInfo.operatingSystemFamily.ToStringCached<OperatingSystemFamily>());
			gsrequestData.AddString("OperatingSystemFull", SystemInfo.operatingSystem);
			gsrequestData.AddString("ProcessorType", SystemInfo.processorType);
			gsrequestData.AddNumber("ProcessorCount", SystemInfo.processorCount);
			gsrequestData.AddNumber("ProcessorClockMHz", MathUtils.RoundToSignificantDigits((double)SystemInfo.processorFrequency, 2));
			gsrequestData.AddNumber("SystemMemoryMB", MathUtils.TruncateToSignificantDigits((double)SystemInfo.systemMemorySize, 2));
			GSRequestData gsrequestData3 = gsrequestData;
			string paramName = "Country";
			IUtils utils = PlatformManager.NativePlatform.Utils;
			gsrequestData3.AddString(paramName, ((utils != null) ? utils.GetCountry() : null) ?? "-n/a-");
			gsrequestData.AddString("Language", Localization.language.ToLower());
			GSRequestData gsrequestData4 = gsrequestData;
			string paramName2 = "EacActive";
			IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
			gsrequestData4.AddBoolean(paramName2, antiCheatClient != null && antiCheatClient.ClientAntiCheatEnabled());
			gsrequestData.AddString("GraphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
			gsrequestData.AddString("GraphicsDeviceName", SystemInfo.graphicsDeviceName);
			gsrequestData.AddNumber("GraphicsMemoryMB", MathUtils.RoundToSignificantDigits((double)SystemInfo.graphicsMemorySize, 2));
			gsrequestData.AddString("GraphicsApi", SystemInfo.graphicsDeviceType.ToStringCached<GraphicsDeviceType>());
			gsrequestData.AddString("GraphicsVersion", SystemInfo.graphicsDeviceVersion);
			gsrequestData.AddNumber("GraphicsShaderLevel", (float)SystemInfo.graphicsShaderLevel / 10f);
			gsrequestData.AddBoolean("GameSenseInstalled", GameSenseManager.GameSenseInstalled);
			Display main = Display.main;
			ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string> valueTuple = XUiC_OptionsVideo.ResolutionInfo.DimensionsToAspectRatio(main.systemWidth, main.systemHeight);
			float item = valueTuple.Item2;
			string item2 = valueTuple.Item3;
			GSRequestData gsrequestData5 = new GSRequestData();
			gsrequestData5.AddString("Text", string.Format("{0}x{1}", main.systemWidth, main.systemHeight));
			gsrequestData5.AddNumber("Width", main.systemWidth);
			gsrequestData5.AddNumber("Height", main.systemHeight);
			gsrequestData5.AddNumber("AspectRatio", item);
			gsrequestData5.AddString("AspectRatioName", item2);
			gsrequestData.AddObject("ScreenResolution", gsrequestData5);
			GSRequestData gsrequestData6 = new GSRequestData();
			ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string> valueTuple2 = XUiC_OptionsVideo.ResolutionInfo.DimensionsToAspectRatio(Screen.width, Screen.height);
			float item3 = valueTuple2.Item2;
			string item4 = valueTuple2.Item3;
			gsrequestData6.AddString("Text", string.Format("{0}x{1}", Screen.width, Screen.height));
			gsrequestData6.AddNumber("Width", Screen.width);
			gsrequestData6.AddNumber("Height", Screen.height);
			gsrequestData6.AddNumber("AspectRatio", item3);
			gsrequestData6.AddString("AspectRatioName", item4);
			gsrequestData.AddObject("Resolution", gsrequestData6);
			gsrequestData.AddString("FullscreenMode", Screen.fullScreenMode.ToStringCached<FullScreenMode>());
			GSRequestData gsrequestData7 = new GSRequestData();
			foreach (EnumGamePrefs enumGamePrefs in EnumUtils.Values<EnumGamePrefs>())
			{
				string text = enumGamePrefs.ToStringCached<EnumGamePrefs>();
				if (text.StartsWith("Options", StringComparison.Ordinal))
				{
					GamePrefs.EnumType? prefType = GamePrefs.GetPrefType(enumGamePrefs);
					if (prefType != null)
					{
						switch (prefType.GetValueOrDefault())
						{
						case GamePrefs.EnumType.Int:
							gsrequestData7.AddNumber(text, GamePrefs.GetInt(enumGamePrefs));
							break;
						case GamePrefs.EnumType.Float:
							gsrequestData7.AddNumber(text, GamePrefs.GetFloat(enumGamePrefs));
							break;
						case GamePrefs.EnumType.String:
							gsrequestData7.AddString(text, GamePrefs.GetString(enumGamePrefs));
							break;
						case GamePrefs.EnumType.Bool:
							gsrequestData7.AddBoolean(text, GamePrefs.GetBool(enumGamePrefs));
							break;
						case GamePrefs.EnumType.Binary:
							Log.Warning("Options GamePref with type Binary: " + text);
							break;
						default:
							Log.Warning("Options GamePref with unknown type: " + text);
							break;
						}
					}
					else
					{
						Log.Warning("Options GamePref with no declaration entry: " + text);
					}
				}
			}
			gsrequestData.AddObject("GameSettings", gsrequestData7);
		}
		else
		{
			gsrequestData.AddString("DediBuildPlatform", Application.platform.ToStringCached<RuntimePlatform>());
			gsrequestData.AddString("DediOperatingSystemFamily", SystemInfo.operatingSystemFamily.ToStringCached<OperatingSystemFamily>());
			gsrequestData.AddString("DediOperatingSystemFull", SystemInfo.operatingSystem);
			gsrequestData.AddString("DediProcessorType", SystemInfo.processorType);
			gsrequestData.AddNumber("DediProcessorCount", SystemInfo.processorCount);
			gsrequestData.AddNumber("DediProcessorClockMHz", MathUtils.RoundToSignificantDigits((double)SystemInfo.processorFrequency, 2));
			gsrequestData.AddNumber("DediSystemMemoryMB", MathUtils.TruncateToSignificantDigits((double)SystemInfo.systemMemorySize, 2));
		}
		this.PrepareAndSendRequest(gsrequestData, eventKey);
	}

	// Token: 0x06007F19 RID: 32537 RVA: 0x00339F94 File Offset: 0x00338194
	public void PrepareNewSession()
	{
		GameSparksCollector.GetSessionUpdateDataAndReset();
		GameSparksCollector.GetSessionTotalData(true);
		PlatformManager.NativePlatform.Input.ResetInputStyleUsage();
	}

	// Token: 0x06007F1A RID: 32538 RVA: 0x00339FB4 File Offset: 0x003381B4
	public void SessionStarted(string _world, string _gameMode, bool _isServer)
	{
		GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
		string value = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? GameManager.Instance.World.Guid : GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
		int value2 = gameServerInfo.GetValue(GameInfoInt.GameDifficulty);
		bool flag = value2 == 1 || value2 == 2;
		bool flag2 = false;
		flag2 = gameServerInfo.GetValue(GameInfoBool.ModdedConfig);
		string eventKey = "SESSION_START";
		GSRequestData gsrequestData = new GSRequestData();
		gsrequestData.AddString("uniqueID", value);
		gsrequestData.AddBoolean("StockSettings", flag);
		gsrequestData.AddBoolean("VanillaConfig", !flag2);
		string value3;
		if (GameManager.IsDedicatedServer)
		{
			value3 = "DedicatedServer";
		}
		else if (_isServer)
		{
			value3 = "ListenServer";
		}
		else if (gameServerInfo.IsDedicated)
		{
			value3 = "ClientOnDedicated";
		}
		else
		{
			value3 = "ClientOnListen";
		}
		gsrequestData.AddString("PeerType", value3);
		GSRequestData gsrequestData2 = new GSRequestData();
		List<Mod> loadedMods = ModManager.GetLoadedMods();
		if (loadedMods.Count == 0)
		{
			gsrequestData2.AddNumber("_Vanilla_", 1);
		}
		else
		{
			foreach (Mod mod in loadedMods)
			{
				gsrequestData2.AddNumber(mod.Name, 1);
			}
		}
		gsrequestData.AddObject(GameManager.IsDedicatedServer ? "ModsLoadedDedi" : "ModsLoadedClient", gsrequestData2);
		if (_isServer)
		{
			GSRequestData gsrequestData3 = new GSRequestData();
			foreach (KeyValuePair<GameInfoString, string> keyValuePair in gameServerInfo.Strings)
			{
				if (!GameSparksManager.IgnoredGameInfoStrings.Contains(keyValuePair.Key))
				{
					gsrequestData3.AddString(keyValuePair.Key.ToStringCached<GameInfoString>(), keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<GameInfoInt, int> keyValuePair2 in gameServerInfo.Ints)
			{
				if (!GameSparksManager.IgnoredGameInfoInts.Contains(keyValuePair2.Key))
				{
					gsrequestData3.AddNumber(keyValuePair2.Key.ToStringCached<GameInfoInt>(), keyValuePair2.Value);
				}
			}
			foreach (KeyValuePair<GameInfoBool, bool> keyValuePair3 in gameServerInfo.Bools)
			{
				if (!GameSparksManager.IgnoredGameInfoBools.Contains(keyValuePair3.Key))
				{
					gsrequestData3.AddBoolean(keyValuePair3.Key.ToStringCached<GameInfoBool>(), keyValuePair3.Value);
				}
			}
			gsrequestData.AddObject("WorldSettings", gsrequestData3);
		}
		this.PrepareAndSendRequest(gsrequestData, eventKey);
		this.sessionUpdateCoroutine = null;
		this.endCoroutine = false;
		GameSparksCollector.CollectGamePlayData = (flag && !flag2);
		this.sessionUpdateCoroutine = ThreadManager.StartCoroutine(this.SessionUpdate());
		GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.UsedTwitchIntegration, null, 0, false, GameSparksCollector.GSDataCollection.SessionTotal);
		GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.PeakConcurrentClients, null, 0, false, GameSparksCollector.GSDataCollection.SessionTotal);
		GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.PeakConcurrentPlayers, null, GameManager.IsDedicatedServer ? 0 : 1, false, GameSparksCollector.GSDataCollection.SessionTotal);
		this.nextDediSessionEndTransmitTime = Time.unscaledTime + 28800f;
	}

	// Token: 0x06007F1B RID: 32539 RVA: 0x0033A30C File Offset: 0x0033850C
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SessionUpdate()
	{
		while (!this.endCoroutine)
		{
			yield return new WaitForSecondsRealtime((float)this.sessionUpdateIntervalSec);
			if (this.endCoroutine)
			{
				yield break;
			}
			if (TwitchManager.HasInstance && TwitchManager.Current.IsReady)
			{
				GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.UsedTwitchIntegration, null, 1, false, GameSparksCollector.GSDataCollection.SessionTotal);
			}
			string eventKey = "SESSION_UPDATE";
			GSRequestData sessionUpdateDataAndReset = GameSparksCollector.GetSessionUpdateDataAndReset();
			if (GameManager.IsDedicatedServer && this.nextDediSessionEndTransmitTime <= Time.unscaledTime)
			{
				foreach (KeyValuePair<string, object> keyValuePair in GameSparksCollector.GetSessionTotalData(false).BaseData)
				{
					sessionUpdateDataAndReset.Add(keyValuePair.Key, keyValuePair.Value);
				}
				this.nextDediSessionEndTransmitTime = Time.unscaledTime + 28800f;
			}
			this.PrepareAndSendRequest(sessionUpdateDataAndReset, eventKey);
		}
		yield break;
	}

	// Token: 0x06007F1C RID: 32540 RVA: 0x0033A31C File Offset: 0x0033851C
	public void SessionEnded()
	{
		if (this.sessionUpdateCoroutine != null)
		{
			this.endCoroutine = true;
			ThreadManager.StopCoroutine(this.sessionUpdateCoroutine);
			this.sessionUpdateCoroutine = null;
		}
		if (TwitchManager.HasInstance && TwitchManager.Current.IsReady)
		{
			GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.UsedTwitchIntegration, null, 1, false, GameSparksCollector.GSDataCollection.SessionTotal);
		}
		string eventKey = "SESSION_END";
		GSRequestData sessionUpdateDataAndReset = GameSparksCollector.GetSessionUpdateDataAndReset();
		sessionUpdateDataAndReset.AddString("uniqueID", base.gameObject.GetComponent<PlatformBase>().DeviceId);
		foreach (KeyValuePair<string, object> keyValuePair in GameSparksCollector.GetSessionTotalData(true).BaseData)
		{
			sessionUpdateDataAndReset.Add(keyValuePair.Key, keyValuePair.Value);
		}
		GSRequestData child = new GSRequestData();
		sessionUpdateDataAndReset.AddObject("RunningTotals", child);
		if (!GameManager.IsDedicatedServer)
		{
			sessionUpdateDataAndReset.AddString("InputDeviceStyle", PlatformManager.NativePlatform.Input.MostUsedInputStyle().ToStringCached<PlayerInputManager.InputStyle>());
			PlatformManager.NativePlatform.Input.ResetInputStyleUsage();
		}
		this.PrepareAndSendRequest(sessionUpdateDataAndReset, eventKey);
	}

	// Token: 0x04006202 RID: 25090
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int dediSessionEndIntervalSec = 28800;

	// Token: 0x04006203 RID: 25091
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<GameInfoString> IgnoredGameInfoStrings = new List<GameInfoString>
	{
		GameInfoString.GameHost,
		GameInfoString.GameName,
		GameInfoString.IP,
		GameInfoString.ServerDescription,
		GameInfoString.ServerLoginConfirmationText,
		GameInfoString.ServerWebsiteURL
	};

	// Token: 0x04006204 RID: 25092
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<GameInfoInt> IgnoredGameInfoInts = new List<GameInfoInt>
	{
		GameInfoInt.CurrentPlayers,
		GameInfoInt.DayCount,
		GameInfoInt.Port,
		GameInfoInt.CurrentServerTime
	};

	// Token: 0x04006205 RID: 25093
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<GameInfoBool> IgnoredGameInfoBools = new List<GameInfoBool>
	{
		GameInfoBool.Architecture64
	};

	// Token: 0x04006206 RID: 25094
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool authenticated;

	// Token: 0x04006207 RID: 25095
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameSparksManager instance;

	// Token: 0x04006208 RID: 25096
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Coroutine sessionUpdateCoroutine;

	// Token: 0x04006209 RID: 25097
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool endCoroutine;

	// Token: 0x0400620A RID: 25098
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextDediSessionEndTransmitTime = -1f;

	// Token: 0x02000F92 RID: 3986
	// (Invoke) Token: 0x06007F20 RID: 32544
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate void OnError(string _error);

	// Token: 0x02000F93 RID: 3987
	// (Invoke) Token: 0x06007F24 RID: 32548
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate void OnPlayerAuthenticated(PlayerDetails _playerDetails);
}
