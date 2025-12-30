using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Audio;
using Discord.Sdk;
using Newtonsoft.Json;
using Platform;
using Twitch;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020002BB RID: 699
public class DiscordManager
{
	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001377 RID: 4983 RVA: 0x00076F3B File Offset: 0x0007513B
	public static DiscordManager Instance
	{
		get
		{
			DiscordManager result;
			if ((result = DiscordManager.instance) == null)
			{
				result = (DiscordManager.instance = new DiscordManager());
			}
			return result;
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06001378 RID: 4984 RVA: 0x00076F54 File Offset: 0x00075154
	// (remove) Token: 0x06001379 RID: 4985 RVA: 0x00076F8C File Offset: 0x0007518C
	public event DiscordManager.UserAuthorizationResultCallback UserAuthorizationResult;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x0600137A RID: 4986 RVA: 0x00076FC4 File Offset: 0x000751C4
	// (remove) Token: 0x0600137B RID: 4987 RVA: 0x00076FFC File Offset: 0x000751FC
	public event Action<DiscordManager.EDiscordStatus> StatusChanged;

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x0600137C RID: 4988 RVA: 0x00077034 File Offset: 0x00075234
	// (remove) Token: 0x0600137D RID: 4989 RVA: 0x0007706C File Offset: 0x0007526C
	public event DiscordManager.LocalUserChangedCallback LocalUserChanged;

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x0600137E RID: 4990 RVA: 0x000770A4 File Offset: 0x000752A4
	// (remove) Token: 0x0600137F RID: 4991 RVA: 0x000770DC File Offset: 0x000752DC
	public event DiscordManager.LobbyStateChangedCallback LobbyStateChanged;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06001380 RID: 4992 RVA: 0x00077114 File Offset: 0x00075314
	// (remove) Token: 0x06001381 RID: 4993 RVA: 0x0007714C File Offset: 0x0007534C
	public event DiscordManager.LobbyMembersChangedCallback LobbyMembersChanged;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06001382 RID: 4994 RVA: 0x00077184 File Offset: 0x00075384
	// (remove) Token: 0x06001383 RID: 4995 RVA: 0x000771BC File Offset: 0x000753BC
	public event DiscordManager.CallChangedCallback CallChanged;

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06001384 RID: 4996 RVA: 0x000771F4 File Offset: 0x000753F4
	// (remove) Token: 0x06001385 RID: 4997 RVA: 0x0007722C File Offset: 0x0007542C
	public event DiscordManager.CallStatusChangedCallback CallStatusChanged;

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06001386 RID: 4998 RVA: 0x00077264 File Offset: 0x00075464
	// (remove) Token: 0x06001387 RID: 4999 RVA: 0x0007729C File Offset: 0x0007549C
	public event DiscordManager.CallMembersChangedCallback CallMembersChanged;

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06001388 RID: 5000 RVA: 0x000772D4 File Offset: 0x000754D4
	// (remove) Token: 0x06001389 RID: 5001 RVA: 0x0007730C File Offset: 0x0007550C
	public event DiscordManager.VoiceStateChangedCallback VoiceStateChanged;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x0600138A RID: 5002 RVA: 0x00077344 File Offset: 0x00075544
	// (remove) Token: 0x0600138B RID: 5003 RVA: 0x0007737C File Offset: 0x0007557C
	public event DiscordManager.SelfMuteStateChangedCallback SelfMuteStateChanged;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x0600138C RID: 5004 RVA: 0x000773B4 File Offset: 0x000755B4
	// (remove) Token: 0x0600138D RID: 5005 RVA: 0x000773EC File Offset: 0x000755EC
	public event DiscordManager.FriendsListChangedCallback FriendsListChanged;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x0600138E RID: 5006 RVA: 0x00077424 File Offset: 0x00075624
	// (remove) Token: 0x0600138F RID: 5007 RVA: 0x0007745C File Offset: 0x0007565C
	public event DiscordManager.RelationshipChangedCallback RelationshipChanged;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06001390 RID: 5008 RVA: 0x00077494 File Offset: 0x00075694
	// (remove) Token: 0x06001391 RID: 5009 RVA: 0x000774CC File Offset: 0x000756CC
	public event DiscordManager.ActivityInviteReceivedCallback ActivityInviteReceived;

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06001392 RID: 5010 RVA: 0x00077504 File Offset: 0x00075704
	// (remove) Token: 0x06001393 RID: 5011 RVA: 0x0007753C File Offset: 0x0007573C
	public event DiscordManager.ActivityJoiningCallback ActivityJoining;

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06001394 RID: 5012 RVA: 0x00077574 File Offset: 0x00075774
	// (remove) Token: 0x06001395 RID: 5013 RVA: 0x000775AC File Offset: 0x000757AC
	public event DiscordManager.PendingActionsUpdateCallback PendingActionsUpdate;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06001396 RID: 5014 RVA: 0x000775E4 File Offset: 0x000757E4
	// (remove) Token: 0x06001397 RID: 5015 RVA: 0x0007761C File Offset: 0x0007581C
	public event DiscordManager.AudioDevicesChangedCallback AudioDevicesChanged;

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001398 RID: 5016 RVA: 0x00077654 File Offset: 0x00075854
	public DiscordManager.EDiscordStatus Status
	{
		get
		{
			if (this.client == null)
			{
				return DiscordManager.EDiscordStatus.NotInitialized;
			}
			DiscordManager.EDiscordStatus result;
			switch (this.client.GetStatus())
			{
			case Discord.Sdk.Client.Status.Disconnected:
				result = DiscordManager.EDiscordStatus.Disconnected;
				break;
			case Discord.Sdk.Client.Status.Connecting:
				result = DiscordManager.EDiscordStatus.Connecting;
				break;
			case Discord.Sdk.Client.Status.Connected:
				result = DiscordManager.EDiscordStatus.Connecting;
				break;
			case Discord.Sdk.Client.Status.Ready:
				result = DiscordManager.EDiscordStatus.Ready;
				break;
			case Discord.Sdk.Client.Status.Reconnecting:
				result = DiscordManager.EDiscordStatus.Connecting;
				break;
			case Discord.Sdk.Client.Status.Disconnecting:
				result = DiscordManager.EDiscordStatus.Disconnecting;
				break;
			default:
				result = DiscordManager.EDiscordStatus.Disconnected;
				break;
			}
			return result;
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001399 RID: 5017 RVA: 0x000776B2 File Offset: 0x000758B2
	public bool IsReady
	{
		get
		{
			return this.Status == DiscordManager.EDiscordStatus.Ready;
		}
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x0600139A RID: 5018 RVA: 0x000776BD File Offset: 0x000758BD
	public bool IsInitialized
	{
		get
		{
			return this.Status > DiscordManager.EDiscordStatus.NotInitialized;
		}
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x0600139B RID: 5019 RVA: 0x000776C8 File Offset: 0x000758C8
	// (set) Token: 0x0600139C RID: 5020 RVA: 0x000776D0 File Offset: 0x000758D0
	public DiscordManager.DiscordUser LocalUser
	{
		get
		{
			return this.localUser;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.localUser == value)
			{
				return;
			}
			this.localUser = value;
			this.localDiscordOrEntityIdChanged();
			DiscordManager.LocalUserChangedCallback localUserChanged = this.LocalUserChanged;
			if (localUserChanged != null)
			{
				localUserChanged(this.localUser != null);
			}
			this.refreshCachedUserHandlesAndRelationships();
		}
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x0007770C File Offset: 0x0007590C
	[PublicizedFrom(EAccessModifier.Private)]
	public DiscordManager()
	{
		this.setLogLevelsFromCmdLine();
		if (!DiscordManager.SupportsProvisionalAccounts)
		{
			Log.Warning("[Discord] Full Discord integration only available when running with EOS cross platform provider!");
		}
		this.Settings = DiscordManager.DiscordSettings.Load();
		this.userMappings = new DiscordManager.DiscordUserMappingManager(this);
		this.AuthManager = new DiscordManager.AuthAndLoginManager(this);
		this.Presence = new DiscordManager.PresenceManager(this);
		this.globalLobby = new DiscordManager.LobbyInfo(this, DiscordManager.ELobbyType.Global);
		this.partyLobby = new DiscordManager.LobbyInfo(this, DiscordManager.ELobbyType.Party);
		this.AudioOutput = new DiscordManager.AudioDeviceConfig(this, true);
		this.AudioInput = new DiscordManager.AudioDeviceConfig(this, false);
		this.UserSettings = DiscordManager.DiscordUserSettingsManager.Load();
		this.registerGameEventHandlers();
		this.ActivityInviteReceived += delegate(DiscordManager.DiscordUser _, bool _, ActivityActionTypes _)
		{
			this.updatePendingActionsEvent();
		};
		this.RelationshipChanged += delegate(DiscordManager.DiscordUser _)
		{
			this.updatePendingActionsEvent();
		};
		this.FriendsListChanged += this.updatePendingActionsEvent;
		if (GameManager.IsDedicatedServer || !this.Settings.DiscordFirstTimeInfoShown || this.Settings.DiscordDisabled)
		{
			return;
		}
		this.Init(false);
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x00077814 File Offset: 0x00075A14
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLogLevelsFromCmdLine()
	{
		string launchArgument = GameUtils.GetLaunchArgument("discordloglevel");
		LoggingSeverity loggingSeverity;
		if (launchArgument != null && EnumUtils.TryParse<LoggingSeverity>(launchArgument, out loggingSeverity, true))
		{
			DiscordManager.logLevel = loggingSeverity;
		}
		launchArgument = GameUtils.GetLaunchArgument("discordloglevelrtc");
		LoggingSeverity loggingSeverity2;
		if (launchArgument != null && EnumUtils.TryParse<LoggingSeverity>(launchArgument, out loggingSeverity2, true))
		{
			DiscordManager.logLevelRtc = loggingSeverity2;
		}
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x00077860 File Offset: 0x00075A60
	public void Init(bool _forceReinit = false)
	{
		if (!VoiceHelpers.VoiceAllowed || !PermissionsManager.IsMultiplayerAllowed())
		{
			return;
		}
		if (this.client != null)
		{
			if (!_forceReinit)
			{
				return;
			}
			this.client.Disconnect();
		}
		else
		{
			NativeMethods.UnhandledException += DiscordManager.nativeMethodException;
			Log.Out(string.Format("[Discord] Initializing, version {0}.{1}.{2}, # {3}", new object[]
			{
				Discord.Sdk.Client.GetVersionMajor(),
				Discord.Sdk.Client.GetVersionMinor(),
				Discord.Sdk.Client.GetVersionPatch(),
				Discord.Sdk.Client.GetVersionHash()
			}));
			this.client = new Discord.Sdk.Client();
			this.client.SetLogDir("", LoggingSeverity.None);
			this.client.SetVoiceLogDir("", LoggingSeverity.None);
			this.client.AddLogCallback(new Discord.Sdk.Client.LogCallback(this.OnDiscordLogMessageReceived), DiscordManager.logLevel);
			this.client.AddVoiceLogCallback(new Discord.Sdk.Client.LogCallback(this.OnDiscordRtcLogMessageReceived), DiscordManager.logLevelRtc);
			this.registerGameStartupForInvites();
		}
		this.client.UpdateToken(AuthorizationTokenType.Bearer, "", delegate(ClientResult _)
		{
		});
		this.registerGlobalDiscordCallbacks();
		this.client.SetAutomaticGainControl(true);
		this.client.SetEchoCancellation(true);
		this.client.SetNoiseSuppression(true);
		this.client.SetOutputVolume((float)this.Settings.OutputVolume);
		this.client.SetInputVolume((float)this.Settings.InputVolume);
		this.updateAudioDeviceList();
		this.Settings.OutputDeviceChanged += delegate(string _)
		{
			this.AudioOutput.UpdateAudioDeviceList();
		};
		this.Settings.InputDeviceChanged += delegate(string _)
		{
			this.AudioInput.UpdateAudioDeviceList();
		};
		this.Settings.OutputVolumeChanged += delegate(int _v)
		{
			this.client.SetOutputVolume((float)_v);
		};
		this.Settings.InputVolumeChanged += delegate(int _v)
		{
			this.client.SetInputVolume((float)_v);
		};
		this.Settings.VoiceModePttChanged += delegate(bool _)
		{
			DiscordManager.LobbyInfo activeVoiceLobby = this.ActiveVoiceLobby;
			if (activeVoiceLobby == null)
			{
				return;
			}
			activeVoiceLobby.VoiceCall.SetPushToTalkMode();
		};
		Log.Out("[Discord] Initialized");
		Action<DiscordManager.EDiscordStatus> statusChanged = this.StatusChanged;
		if (statusChanged != null)
		{
			statusChanged(this.Status);
		}
		this.registerGameChatHandling();
		DiscordManager.CallInfo.LoadSounds();
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x00077A8C File Offset: 0x00075C8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerGlobalDiscordCallbacks()
	{
		this.client.SetDeviceChangeCallback(new Discord.Sdk.Client.DeviceChangeCallback(this.OnDeviceChanged));
		this.client.SetLobbyCreatedCallback(new Discord.Sdk.Client.LobbyCreatedCallback(this.OnLobbyCreated));
		this.client.SetLobbyDeletedCallback(new Discord.Sdk.Client.LobbyDeletedCallback(this.OnLobbyDeleted));
		this.client.SetLobbyUpdatedCallback(new Discord.Sdk.Client.LobbyUpdatedCallback(this.OnLobbyUpdated));
		this.client.SetLobbyMemberAddedCallback(new Discord.Sdk.Client.LobbyMemberAddedCallback(this.OnLobbyMemberAdded));
		this.client.SetLobbyMemberRemovedCallback(new Discord.Sdk.Client.LobbyMemberRemovedCallback(this.OnLobbyMemberRemoved));
		this.client.SetLobbyMemberUpdatedCallback(new Discord.Sdk.Client.LobbyMemberUpdatedCallback(this.OnLobbyMemberUpdated));
		this.client.SetMessageCreatedCallback(new Discord.Sdk.Client.MessageCreatedCallback(this.OnMessageCreated));
		this.client.SetMessageDeletedCallback(new Discord.Sdk.Client.MessageDeletedCallback(this.OnMessageDeleted));
		this.client.SetMessageUpdatedCallback(new Discord.Sdk.Client.MessageUpdatedCallback(this.OnMessageUpdated));
		this.client.SetNoAudioInputCallback(new Discord.Sdk.Client.NoAudioInputCallback(this.OnNoAudioInput));
		this.client.SetRelationshipCreatedCallback(new Discord.Sdk.Client.RelationshipCreatedCallback(this.OnRelationshipCreated));
		this.client.SetRelationshipDeletedCallback(new Discord.Sdk.Client.RelationshipDeletedCallback(this.OnRelationshipDeleted));
		this.client.SetUserUpdatedCallback(new Discord.Sdk.Client.UserUpdatedCallback(this.OnUserUpdated));
		this.client.SetVoiceParticipantChangedCallback(new Discord.Sdk.Client.VoiceParticipantChangedCallback(this.OnVoiceParticipantChanged));
		this.AuthManager.RegisterDiscordCallbacks();
		this.Presence.RegisterDiscordCallbacks();
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x00077C08 File Offset: 0x00075E08
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerGameStartupForInvites()
	{
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			return;
		}
		bool flag;
		switch (PlatformManager.NativePlatform.PlatformIdentifier)
		{
		case EPlatformIdentifier.Local:
			flag = this.<registerGameStartupForInvites>g__RegisterNativePcLauncher|97_1();
			goto IL_83;
		case EPlatformIdentifier.EOS:
			flag = false;
			goto IL_83;
		case EPlatformIdentifier.Steam:
			flag = this.<registerGameStartupForInvites>g__RegisterSteamLauncher|97_0();
			goto IL_83;
		case EPlatformIdentifier.XBL:
			flag = this.<registerGameStartupForInvites>g__RegisterNativePcLauncher|97_1();
			goto IL_83;
		case EPlatformIdentifier.EGS:
			flag = false;
			goto IL_83;
		}
		throw new ArgumentOutOfRangeException("PlatformIdentifier", "Invalid native platform " + PlatformManager.NativePlatform.PlatformIdentifier.ToStringCached<EPlatformIdentifier>() + " for registering Discord launch command");
		IL_83:
		bool flag2 = flag;
		Log.Out("[Discord] Registering game for Discord invites: " + (flag2 ? "Success" : "Failed"));
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x00077CB8 File Offset: 0x00075EB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void cleanup(ref ModEvents.SGameShutdownData _data)
	{
		this.Settings.Save();
		if (this.client == null)
		{
			return;
		}
		Log.Out("[Discord] Cleanup");
		this.leaveLobbies(true);
		this.client.Disconnect();
		this.client.Dispose();
		this.client = null;
		Action<DiscordManager.EDiscordStatus> statusChanged = this.StatusChanged;
		if (statusChanged == null)
		{
			return;
		}
		statusChanged(this.Status);
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060013A3 RID: 5027 RVA: 0x00077D1D File Offset: 0x00075F1D
	public static bool SupportsProvisionalAccounts
	{
		get
		{
			return PlatformManager.CrossplatformPlatform != null && PlatformManager.CrossplatformPlatform.PlatformIdentifier == EPlatformIdentifier.EOS;
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x060013A4 RID: 5028 RVA: 0x00077D35 File Offset: 0x00075F35
	public static bool SupportsFullAccounts
	{
		get
		{
			return !DeviceFlag.PS5.IsCurrent();
		}
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x00077D44 File Offset: 0x00075F44
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRelationshipCreated(ulong _userId, bool _isDiscordRelationship)
	{
		DiscordManager.logCallbackInfo(string.Format("OnRelationshipCreated: {0}, {1}", _userId, _isDiscordRelationship), LogType.Log);
		DiscordManager.DiscordUser user = this.GetUser(_userId);
		user.UpdateRelationship();
		user.UpdatePresenceInfo();
		DiscordManager.RelationshipChangedCallback relationshipChanged = this.RelationshipChanged;
		if (relationshipChanged != null)
		{
			relationshipChanged(user);
		}
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged == null)
		{
			return;
		}
		friendsListChanged();
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x00077DA4 File Offset: 0x00075FA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRelationshipDeleted(ulong _userId, bool _isDiscordRelationship)
	{
		DiscordManager.logCallbackInfo(string.Format("OnRelationshipDeleted: {0}, {1}", _userId, _isDiscordRelationship), LogType.Log);
		DiscordManager.DiscordUser user = this.GetUser(_userId);
		user.UpdateRelationship();
		user.UpdatePresenceInfo();
		DiscordManager.RelationshipChangedCallback relationshipChanged = this.RelationshipChanged;
		if (relationshipChanged != null)
		{
			relationshipChanged(user);
		}
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged == null)
		{
			return;
		}
		friendsListChanged();
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x00077E04 File Offset: 0x00076004
	[PublicizedFrom(EAccessModifier.Private)]
	public void clearFriends()
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			discordUser.UpdateRelationship();
		}
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged == null)
		{
			return;
		}
		friendsListChanged();
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x00077E74 File Offset: 0x00076074
	[PublicizedFrom(EAccessModifier.Private)]
	public void getFriends()
	{
		if (!this.IsReady)
		{
			DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
			if (friendsListChanged == null)
			{
				return;
			}
			friendsListChanged();
			return;
		}
		else
		{
			foreach (RelationshipHandle relationshipHandle in this.client.GetRelationships())
			{
				this.GetUser(relationshipHandle.Id()).UpdatePresenceInfo();
			}
			DiscordManager.FriendsListChangedCallback friendsListChanged2 = this.FriendsListChanged;
			if (friendsListChanged2 == null)
			{
				return;
			}
			friendsListChanged2();
			return;
		}
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x00077EDC File Offset: 0x000760DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnUserUpdated(ulong _userId)
	{
		DiscordManager.DiscordUser user = this.GetUser(_userId);
		user.UpdatePresenceInfo();
		user.UpdateRelationship();
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged != null)
		{
			friendsListChanged();
		}
		DiscordManager.logCallbackInfo(string.Format("OnUserUpdated: user={0}", user), LogType.Log);
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x00077F20 File Offset: 0x00076120
	[PublicizedFrom(EAccessModifier.Private)]
	public void onLogMessageReceivedGeneric(string _message, LoggingSeverity _severity, string _prefix)
	{
		Match match = DiscordManager.logMessageMatcher.Match(_message);
		if (match.Success)
		{
			if (match.Groups[4].Value.IndexOf('\n') >= 0)
			{
				_message = match.Groups[4].Value + " (" + match.Groups[3].Value + ")\n";
			}
			else
			{
				_message = match.Groups[4].Value + " (" + match.Groups[3].Value + ")";
			}
		}
		else if (_message.IndexOf('\n') == _message.Length - 1)
		{
			_message = _message.Substring(0, _message.Length - 1);
		}
		switch (_severity)
		{
		case LoggingSeverity.Verbose:
			Log.Out("[Discord]" + _prefix + "[Log](Verb) " + _message);
			return;
		case LoggingSeverity.Info:
			Log.Out("[Discord]" + _prefix + "[Log](Info) " + _message);
			return;
		case LoggingSeverity.Warning:
			Log.Warning("[Discord]" + _prefix + "[Log] " + _message);
			return;
		case LoggingSeverity.Error:
			Log.Error("[Discord]" + _prefix + "[Log] " + _message);
			return;
		case LoggingSeverity.None:
			Log.Out("[Discord]" + _prefix + "[Log](None) " + _message);
			return;
		default:
			Log.Error(string.Format("[Discord]{0}[Log] Unknown log severity ({1}): {2}", _prefix, _severity, _message));
			return;
		}
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x00078094 File Offset: 0x00076294
	[PublicizedFrom(EAccessModifier.Private)]
	public static void nativeMethodException(Exception _e)
	{
		Log.Error("[Discord] Exception: " + _e.Message);
		Log.Exception(_e);
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x000780B1 File Offset: 0x000762B1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDiscordLogMessageReceived(string _message, LoggingSeverity _severity)
	{
		this.onLogMessageReceivedGeneric(_message, _severity, "");
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x000780C0 File Offset: 0x000762C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDiscordRtcLogMessageReceived(string _message, LoggingSeverity _severity)
	{
		this.onLogMessageReceivedGeneric(_message, _severity, "[RTC]");
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x000780CF File Offset: 0x000762CF
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnMessageCreated(ulong _messageId)
	{
		this.handleMessage(true, _messageId);
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x000780D9 File Offset: 0x000762D9
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnMessageDeleted(ulong _messageId, ulong _channelId)
	{
		DiscordManager.logCallbackInfo(string.Format("OnMessageDeleted: msg={0}, channel={1}", _messageId, _channelId), LogType.Log);
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x000780F7 File Offset: 0x000762F7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnMessageUpdated(ulong _messageId)
	{
		this.handleMessage(false, _messageId);
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x00078104 File Offset: 0x00076304
	[PublicizedFrom(EAccessModifier.Private)]
	public void handleMessage(bool _created, ulong _messageId)
	{
		if (this.client == null)
		{
			return;
		}
		using (MessageHandle messageHandle = this.client.GetMessageHandle(_messageId))
		{
			if (messageHandle == null)
			{
				DiscordManager.logCallbackInfo(string.Format("{0}: msg={1}, No message handle", _created ? "OnMessageCreated" : "OnMessageUpdated", _messageId), LogType.Warning);
			}
			else
			{
				ulong num = messageHandle.ChannelId();
				ulong num2 = messageHandle.Id();
				string text = messageHandle.Content();
				messageHandle.RawContent();
				using (AdditionalContent additionalContent = messageHandle.AdditionalContent())
				{
					AdditionalContentType? additionalContentType = (additionalContent != null) ? new AdditionalContentType?(additionalContent.Type()) : null;
					DiscordManager.DiscordUser user = this.GetUser(messageHandle.AuthorId());
					bool isLocalAccount = user.IsLocalAccount;
					DiscordManager.DiscordUser discordUser = isLocalAccount ? this.GetUser(messageHandle.RecipientId()) : this.LocalUser;
					DiscordManager.DiscordUser discordUser2 = isLocalAccount ? discordUser : user;
					DiscordManager.logCallbackInfo(string.Format("{0}: msg={1} channel={2} sender='{3}' recipient='{4}' outbound='{5}' text='<redacted>' rawContent='<redacted>'", new object[]
					{
						_created ? "OnMessageCreated" : "OnMessageUpdated",
						num2,
						num,
						user.DisplayName,
						discordUser.DisplayName,
						isLocalAccount
					}), LogType.Log);
					if (additionalContent != null)
					{
						DiscordManager.logCallbackInfo(string.Format("{0}: Additional content: Count={1}, type={2} title='{3}'", new object[]
						{
							_created ? "OnMessageCreated" : "OnMessageUpdated",
							additionalContent.Count(),
							additionalContentType.Value.ToStringCached<AdditionalContentType>(),
							additionalContent.Title()
						}), LogType.Log);
					}
					if (_created)
					{
						if (this.Settings.DmPrivacyMode && !discordUser2.MessageSentFromGame)
						{
							if (DiscordManager.logLevel == LoggingSeverity.Verbose)
							{
								Log.Out(string.Format("[Discord] Not showing received DM: Privacy mode active and no message sent to the user yet ({0})", discordUser2));
							}
						}
						else
						{
							LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
							if (!(uiforPrimaryPlayer == null) && !(uiforPrimaryPlayer.entityPlayer == null))
							{
								EMessageSender messageSenderType = EMessageSender.SenderIdAsPlayer;
								int senderId;
								if (!this.userMappings.TryGetEntityId(discordUser2.ID, out senderId))
								{
									messageSenderType = EMessageSender.None;
									senderId = -1;
								}
								XUiC_Chat.EnforceTargetExists(uiforPrimaryPlayer.xui, EChatType.Discord, discordUser2.ID.ToString());
								if (!string.IsNullOrEmpty(text))
								{
									XUiC_ChatOutput.AddMessage(uiforPrimaryPlayer.xui, EnumGameMessages.Chat, text, EChatType.Discord, isLocalAccount ? EChatDirection.Outbound : EChatDirection.Inbound, senderId, discordUser2.DisplayName, discordUser2.ID.ToString(), messageSenderType, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
								}
								if (additionalContent != null)
								{
									if (additionalContentType != null)
									{
										string text2;
										switch (additionalContentType.GetValueOrDefault())
										{
										case AdditionalContentType.Other:
											text2 = Localization.Get("discordMessageAdditionalContentTypeOther", false);
											break;
										case AdditionalContentType.Attachment:
											text2 = ((additionalContent.Count() <= 1) ? Localization.Get("discordMessageAdditionalContentTypeAttachmentSingle", false) : string.Format(Localization.Get("discordMessageAdditionalContentTypeAttachmentMultiple", false), additionalContent.Count()));
											break;
										case AdditionalContentType.Poll:
											text2 = Localization.Get("discordMessageAdditionalContentTypePoll", false);
											break;
										case AdditionalContentType.VoiceMessage:
											text2 = Localization.Get("discordMessageAdditionalContentTypeVoiceMessage", false);
											break;
										case AdditionalContentType.Thread:
											text2 = Localization.Get("discordMessageAdditionalContentTypeThread", false);
											break;
										case AdditionalContentType.Embed:
											text2 = Localization.Get("discordMessageAdditionalContentTypeEmbed", false);
											break;
										case AdditionalContentType.Sticker:
											text2 = Localization.Get("discordMessageAdditionalContentTypeSticker", false);
											break;
										default:
											goto IL_31A;
										}
										string text3 = text2;
										string message = (this.client.CanOpenMessageInDiscord(num2) && !this.LocalUser.IsProvisionalAccount && !(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent()) ? (XUiUtils.BuildUrlFunctionString("DiscordMessageButton", new ValueTuple<string, string>("MessageId", num2.ToString())) + text3 + " [sp=ui_game_symbol_external_link][/url]") : text3;
										XUiC_ChatOutput.AddMessage(uiforPrimaryPlayer.xui, EnumGameMessages.Chat, message, EChatType.Discord, isLocalAccount ? EChatDirection.Outbound : EChatDirection.Inbound, senderId, discordUser2.DisplayName, discordUser2.ID.ToString(), messageSenderType, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
										goto IL_3C0;
									}
									IL_31A:
									throw new ArgumentOutOfRangeException("additionalContentType", additionalContentType.Value, "Invalid AdditionalContentType");
								}
							}
						}
					}
					IL_3C0:;
				}
			}
		}
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x00078520 File Offset: 0x00076720
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerGameChatHandling()
	{
		XUiUtils.RegisterLabelUrlHandler("DiscordMessageButton", new XUiUtils.HandleTextUrlDelegate(this.<registerGameChatHandling>g__DiscordButtonHandler|120_0));
		XUiC_Chat.RegisterCustomMessagingHandler(EChatType.Discord, new XUiC_Chat.IsValidTarget(this.<registerGameChatHandling>g__IsValidTarget|120_3), new XUiC_Chat.GetTargetDisplayName(this.<registerGameChatHandling>g__GetTargetDisplayName|120_2), new XUiC_Chat.SendMessage(this.<registerGameChatHandling>g__SendMessage|120_1));
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0007856D File Offset: 0x0007676D
	public DiscordManager.LobbyInfo GetLobby(DiscordManager.ELobbyType _type)
	{
		if (_type != DiscordManager.ELobbyType.Global)
		{
			return this.partyLobby;
		}
		return this.globalLobby;
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0007857F File Offset: 0x0007677F
	[PublicizedFrom(EAccessModifier.Private)]
	public void leaveLobbies(bool _manual = true)
	{
		this.LeaveLobbyVoice(_manual);
		this.globalLobby.Leave(_manual);
		this.partyLobby.Leave(_manual);
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x000785A0 File Offset: 0x000767A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyCreated(ulong _lobbyId)
	{
		DiscordManager.logCallbackInfo(string.Format("OnLobbyCreated: lobby={0}", _lobbyId), LogType.Log);
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x000785B8 File Offset: 0x000767B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyDeleted(ulong _lobbyId)
	{
		DiscordManager.logCallbackInfo(string.Format("OnLobbyDeleted: lobby={0}", _lobbyId), LogType.Log);
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x000785D0 File Offset: 0x000767D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyUpdated(ulong _lobbyId)
	{
		DiscordManager.logCallbackInfo(string.Format("OnLobbyUpdated: lobby={0}", _lobbyId), LogType.Log);
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x000785E8 File Offset: 0x000767E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyMemberAdded(ulong _lobbyId, ulong _memberId)
	{
		this.OnLobbyMemberChanged(_lobbyId, _memberId, DiscordManager.LobbyMemberActionType.Add);
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x000785F3 File Offset: 0x000767F3
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyMemberRemoved(ulong _lobbyId, ulong _memberId)
	{
		this.OnLobbyMemberChanged(_lobbyId, _memberId, DiscordManager.LobbyMemberActionType.Remove);
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x000785FE File Offset: 0x000767FE
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyMemberUpdated(ulong _lobbyId, ulong _memberId)
	{
		this.OnLobbyMemberChanged(_lobbyId, _memberId, DiscordManager.LobbyMemberActionType.Update);
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x0007860C File Offset: 0x0007680C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLobbyMemberChanged(ulong _lobbyId, ulong _memberId, DiscordManager.LobbyMemberActionType _actionType)
	{
		DiscordManager.DiscordUser user = this.GetUser(_memberId);
		DiscordManager.logCallbackInfo(string.Format("OnLobbyMember{0}: lobby={1} user={2}", _actionType.ToStringCached<DiscordManager.LobbyMemberActionType>(), _lobbyId, user), LogType.Log);
		this.globalLobby.UpdateMembers();
		this.partyLobby.UpdateMembers();
		DiscordManager.LobbyInfo activeVoiceLobby = this.ActiveVoiceLobby;
		if (activeVoiceLobby == null)
		{
			return;
		}
		activeVoiceLobby.VoiceCall.UpdateMembers(false);
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x060013BC RID: 5052 RVA: 0x0007866A File Offset: 0x0007686A
	// (set) Token: 0x060013BD RID: 5053 RVA: 0x0007867D File Offset: 0x0007687D
	public bool Mute
	{
		get
		{
			Discord.Sdk.Client client = this.client;
			return client != null && client.GetSelfMuteAll();
		}
		set
		{
			Discord.Sdk.Client client = this.client;
			if (client != null)
			{
				client.SetSelfMuteAll(value);
			}
			DiscordManager.SelfMuteStateChangedCallback selfMuteStateChanged = this.SelfMuteStateChanged;
			if (selfMuteStateChanged == null)
			{
				return;
			}
			selfMuteStateChanged(value, this.Deaf);
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x060013BE RID: 5054 RVA: 0x000786A8 File Offset: 0x000768A8
	// (set) Token: 0x060013BF RID: 5055 RVA: 0x000786BB File Offset: 0x000768BB
	public bool Deaf
	{
		get
		{
			Discord.Sdk.Client client = this.client;
			return client != null && client.GetSelfDeafAll();
		}
		set
		{
			Discord.Sdk.Client client = this.client;
			if (client != null)
			{
				client.SetSelfDeafAll(value);
			}
			DiscordManager.SelfMuteStateChangedCallback selfMuteStateChanged = this.SelfMuteStateChanged;
			if (selfMuteStateChanged == null)
			{
				return;
			}
			selfMuteStateChanged(this.Mute, value);
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x060013C0 RID: 5056 RVA: 0x000786E6 File Offset: 0x000768E6
	public DiscordManager.LobbyInfo ActiveVoiceLobby
	{
		get
		{
			if (this.partyLobby.IsInVoice)
			{
				return this.partyLobby;
			}
			if (this.globalLobby.IsInVoice)
			{
				return this.globalLobby;
			}
			return null;
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x060013C1 RID: 5057 RVA: 0x00078714 File Offset: 0x00076914
	public bool AnyLobbyInUnstableVoiceState
	{
		get
		{
			Call.Status status = this.globalLobby.VoiceCall.Status;
			if (status != Call.Status.Connected && status != Call.Status.Disconnected)
			{
				return true;
			}
			status = this.partyLobby.VoiceCall.Status;
			return status != Call.Status.Connected && status != Call.Status.Disconnected;
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x00078758 File Offset: 0x00076958
	public void JoinLobbyVoice(DiscordManager.ELobbyType _type)
	{
		DiscordManager.LobbyInfo lobby = this.GetLobby(_type);
		if (this.ActiveVoiceLobby == lobby)
		{
			return;
		}
		this.LeaveLobbyVoice(true);
		lobby.VoiceCall.Join();
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x00078789 File Offset: 0x00076989
	public void LeaveLobbyVoice(bool _manual = true)
	{
		DiscordManager.LobbyInfo activeVoiceLobby = this.ActiveVoiceLobby;
		if (activeVoiceLobby == null)
		{
			return;
		}
		activeVoiceLobby.VoiceCall.Leave(_manual);
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x000787A1 File Offset: 0x000769A1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnNoAudioInput(bool _inputDetected)
	{
		DiscordManager.logCallbackInfo(string.Format("OnNoAudioInput: inputDetected={0}", _inputDetected), LogType.Log);
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x000787BC File Offset: 0x000769BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVoiceParticipantChanged(ulong _lobbyId, ulong _userId, bool _added)
	{
		DiscordManager.DiscordUser user = this.GetUser(_userId);
		DiscordManager.logCallbackInfo(string.Format("OnVoiceParticipantChanged: lobby={0} user={1} added={2}", _lobbyId, user, _added), LogType.Log);
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x000787EE File Offset: 0x000769EE
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDeviceChanged(AudioDevice[] _inputDevices, AudioDevice[] _outputDevices)
	{
		DiscordManager.logCallbackInfo(string.Format("OnDeviceChanged: input devices: {0}, output devices: {1}", _inputDevices.Length, _outputDevices.Length), LogType.Log);
		this.AudioOutput.ApplyDevicesFound(_outputDevices);
		this.AudioInput.ApplyDevicesFound(_inputDevices);
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x00078828 File Offset: 0x00076A28
	[PublicizedFrom(EAccessModifier.Private)]
	public void fireAudioDevicesChanged(DiscordManager.AudioDeviceConfig _config)
	{
		DiscordManager.AudioDevicesChangedCallback audioDevicesChanged = this.AudioDevicesChanged;
		if (audioDevicesChanged == null)
		{
			return;
		}
		audioDevicesChanged(_config);
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0007883B File Offset: 0x00076A3B
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAudioDeviceList()
	{
		this.AudioOutput.UpdateAudioDeviceList();
		this.AudioInput.UpdateAudioDeviceList();
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x00078854 File Offset: 0x00076A54
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerGameEventHandlers()
	{
		ModEvents.GameFocus.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameFocusData>(this.gameFocusChanged));
		ModEvents.MainMenuOpening.RegisterHandler(new ModEvents.ModEventInterruptibleHandlerDelegate<ModEvents.SMainMenuOpeningData>(this.mainMenuOpening));
		ModEvents.UnityUpdate.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SUnityUpdateData>(this.update));
		ModEvents.ServerRegistered.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SServerRegisteredData>(this.serverStarted));
		ModEvents.PlayerJoinedGame.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SPlayerJoinedGameData>(this.playerJoined));
		ModEvents.PlayerSpawning.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SPlayerSpawningData>(this.playerSpawning));
		ModEvents.PlayerSpawnedInWorld.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SPlayerSpawnedInWorldData>(this.playerSpawned));
		ModEvents.PlayerDisconnected.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SPlayerDisconnectedData>(this.playerDisconnected));
		ModEvents.GameStarting.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartingData>(this.gameStarting));
		ModEvents.GameUpdate.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameUpdateData>(this.inGameUpdate));
		ModEvents.WorldShuttingDown.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SWorldShuttingDownData>(this.gameEnded));
		ModEvents.GameShutdown.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameShutdownData>(this.cleanup));
		GameManager.Instance.OnLocalPlayerChanged += this.localPlayerChangedEvent;
		PlatformUserManager.BlockedStateChanged += this.playerBlockStateChanged;
		World world = GameManager.Instance.World;
		EntityPlayerLocal entityPlayerLocal = (world != null) ? world.GetPrimaryPlayer() : null;
		if (entityPlayerLocal != null)
		{
			this.playerCreated(entityPlayerLocal);
		}
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x000789B7 File Offset: 0x00076BB7
	[PublicizedFrom(EAccessModifier.Private)]
	public void gameFocusChanged(ref ModEvents.SGameFocusData _data)
	{
		if (GameManager.IsDedicatedServer || !this.IsInitialized)
		{
			return;
		}
		this.client.SetShowingChat(_data.IsFocused);
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x000789DC File Offset: 0x00076BDC
	[PublicizedFrom(EAccessModifier.Private)]
	public ModEvents.EModEventResult mainMenuOpening(ref ModEvents.SMainMenuOpeningData _data)
	{
		if (_data.OpenedBefore)
		{
			return ModEvents.EModEventResult.Continue;
		}
		if (PlatformManager.MultiPlatform.User.UserStatus != EUserStatus.LoggedIn)
		{
			return ModEvents.EModEventResult.Continue;
		}
		if (GameManager.IsDedicatedServer || !VoiceHelpers.VoiceAllowed || !PermissionsManager.IsMultiplayerAllowed())
		{
			return ModEvents.EModEventResult.Continue;
		}
		if (!this.Settings.DiscordFirstTimeInfoShown && !this.Settings.DiscordDisabled)
		{
			LocalPlayerUI.primaryUI.windowManager.Open(XUiC_DiscordInfo.ID, true, false, true);
			return ModEvents.EModEventResult.StopHandlersAndVanilla;
		}
		if (this.Settings.DiscordDisabled)
		{
			return ModEvents.EModEventResult.Continue;
		}
		this.Init(false);
		if (this.AuthManager.IsLoggingIn || this.Status != DiscordManager.EDiscordStatus.Disconnected)
		{
			return ModEvents.EModEventResult.Continue;
		}
		XUiC_DiscordLogin.Open(null, true, true, true, true, false);
		this.AuthManager.AutoLogin();
		return ModEvents.EModEventResult.StopHandlersAndVanilla;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x00078A95 File Offset: 0x00076C95
	[PublicizedFrom(EAccessModifier.Private)]
	public void update(ref ModEvents.SUnityUpdateData _data)
	{
		if (!this.IsReady)
		{
			return;
		}
		this.handlePushToTalkButton();
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x00078AA8 File Offset: 0x00076CA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void handlePushToTalkButton()
	{
		DiscordManager.LobbyInfo activeVoiceLobby = this.ActiveVoiceLobby;
		if (activeVoiceLobby == null || !activeVoiceLobby.IsInVoice)
		{
			return;
		}
		if (this.Settings.VoiceModePtt)
		{
			this.ActiveVoiceLobby.VoiceCall.SetPushToTalkActive(VoiceHelpers.PushToTalkPressed());
			return;
		}
		if (VoiceHelpers.PushToTalkWasPressed())
		{
			this.Mute = !this.Mute;
		}
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x00078B03 File Offset: 0x00076D03
	[PublicizedFrom(EAccessModifier.Private)]
	public void serverStarted(ref ModEvents.SServerRegisteredData _data)
	{
		if (this.globalLobby.Secret != null)
		{
			return;
		}
		this.ReceivedLobbySecret(DiscordManager.ELobbyType.Global, SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoString.IP) + "_" + Utils.GenerateGuid());
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x00078B39 File Offset: 0x00076D39
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerJoined(ref ModEvents.SPlayerJoinedGameData _data)
	{
		this.userMappings.SendMappingsToClient(_data.ClientInfo);
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x00078B4C File Offset: 0x00076D4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerSpawning(ref ModEvents.SPlayerSpawningData _data)
	{
		_data.ClientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageDiscordLobbySecret>().Setup(DiscordManager.ELobbyType.Global, this.globalLobby.Secret));
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x00078B70 File Offset: 0x00076D70
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerSpawned(ref ModEvents.SPlayerSpawnedInWorldData _data)
	{
		this.updateUserNames();
		if (GameManager.IsDedicatedServer || !this.IsInitialized)
		{
			return;
		}
		if (!_data.IsLocalPlayer)
		{
			return;
		}
		this.client.SetShowingChat(true);
		if (this.Settings.AutoJoinVoiceMode == DiscordManager.EAutoJoinVoiceMode.Global && this.globalLobby.Secret != null)
		{
			this.JoinLobbyVoice(DiscordManager.ELobbyType.Global);
		}
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x00078BCC File Offset: 0x00076DCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerDisconnected(ref ModEvents.SPlayerDisconnectedData _data)
	{
		int entityId = _data.ClientInfo.entityId;
		if (entityId == -1)
		{
			return;
		}
		this.userMappings.UpdateMapping(entityId, true, 0UL);
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged != null)
		{
			friendsListChanged();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageDiscordIdMappings>().Setup(entityId, true, 0UL), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x00078C44 File Offset: 0x00076E44
	public void ReceivedLobbySecret(DiscordManager.ELobbyType _lobbyType, string _secret)
	{
		DiscordManager.LobbyInfo lobby = this.GetLobby(_lobbyType);
		Log.Out("[Discord] " + _lobbyType.ToStringCached<DiscordManager.ELobbyType>() + " lobby: " + _secret);
		lobby.Secret = _secret;
		if (GameManager.IsDedicatedServer || !this.IsReady)
		{
			return;
		}
		lobby.Join(true);
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x00078C92 File Offset: 0x00076E92
	public void LeftParty()
	{
		this.partyLobby.Secret = null;
		if (GameManager.IsDedicatedServer || !this.IsInitialized)
		{
			return;
		}
		this.partyLobby.Leave(false);
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x00078CBC File Offset: 0x00076EBC
	public void UserMappingReceived(int _entityId, bool _remove, ulong _discordId, bool _batch = false)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageDiscordIdMappings>().Setup(_entityId, _remove, _discordId), false, -1, -1, -1, null, 192, false);
		}
		this.userMappings.UpdateMapping(_entityId, _remove, _discordId);
		if (_discordId > 0UL)
		{
			this.GetUser(_discordId).UpdatePlayerName(null);
		}
		if (!_batch)
		{
			DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
			if (friendsListChanged == null)
			{
				return;
			}
			friendsListChanged();
		}
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x00078D34 File Offset: 0x00076F34
	public void UserMappingsReceived(List<int> _entityIds, List<ulong> _discordIds)
	{
		for (int i = 0; i < _entityIds.Count; i++)
		{
			this.UserMappingReceived(_entityIds[i], false, _discordIds[i], true);
		}
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged == null)
		{
			return;
		}
		friendsListChanged();
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x00078D78 File Offset: 0x00076F78
	[PublicizedFrom(EAccessModifier.Private)]
	public void gameStarting(ref ModEvents.SGameStartingData _data)
	{
		this.Presence.SetRichPresenceState(new IRichPresence.PresenceStates?(_data.AsServer ? IRichPresence.PresenceStates.Loading : IRichPresence.PresenceStates.Connecting));
		this.resetPendingOutgoingJoinRequests();
		if (PlatformManager.MultiPlatform.User.UserStatus != EUserStatus.LoggedIn)
		{
			return;
		}
		if (GameManager.IsDedicatedServer || !VoiceHelpers.VoiceAllowed || !PermissionsManager.IsMultiplayerAllowed())
		{
			return;
		}
		if (this.Settings.DiscordDisabled)
		{
			return;
		}
		this.Init(false);
		if (this.AuthManager.IsLoggingIn || this.Status != DiscordManager.EDiscordStatus.Disconnected)
		{
			return;
		}
		this.AuthManager.AutoLogin();
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x00078E08 File Offset: 0x00077008
	[PublicizedFrom(EAccessModifier.Private)]
	public void inGameUpdate(ref ModEvents.SGameUpdateData _data)
	{
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime < this.nextActivityUpdate)
		{
			return;
		}
		this.nextActivityUpdate = unscaledTime + 10f;
		this.Presence.SetRichPresenceState(new IRichPresence.PresenceStates?(IRichPresence.PresenceStates.InGame));
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x00078E44 File Offset: 0x00077044
	[PublicizedFrom(EAccessModifier.Private)]
	public void gameEnded(ref ModEvents.SWorldShuttingDownData _data)
	{
		this.globalLobby.Secret = null;
		this.partyLobby.Secret = null;
		this.userMappings.Clear();
		this.Presence.SetRichPresenceState(new IRichPresence.PresenceStates?(IRichPresence.PresenceStates.Menu));
		if (GameManager.IsDedicatedServer || !this.IsInitialized)
		{
			return;
		}
		this.leaveLobbies(true);
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged != null)
		{
			friendsListChanged();
		}
		this.client.SetShowingChat(false);
		this.Settings.Save();
		this.UserSettings.Save();
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x00078ED0 File Offset: 0x000770D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void localPlayerChangedEvent(EntityPlayerLocal _newLocalPlayer)
	{
		if (!(_newLocalPlayer != null))
		{
			this.playerDestroyed();
			return;
		}
		this.playerCreated(_newLocalPlayer);
		DiscordManager.DiscordUser discordUser = this.LocalUser;
		if (discordUser != null)
		{
			discordUser.UpdatePlayerName(_newLocalPlayer);
		}
		this.localDiscordOrEntityIdChanged();
		DiscordManager.FriendsListChangedCallback friendsListChanged = this.FriendsListChanged;
		if (friendsListChanged == null)
		{
			return;
		}
		friendsListChanged();
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x00078F1C File Offset: 0x0007711C
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerCreated(EntityPlayerLocal _newLocalPlayer)
	{
		if (PlatformManager.MultiPlatform.User.UserStatus == EUserStatus.OfflineMode)
		{
			return;
		}
		this.localPlayer = _newLocalPlayer;
		this.localPlayer.PartyJoined += this.playerJoinedParty;
		this.localPlayer.PartyLeave += this.playerLeftParty;
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x00078F74 File Offset: 0x00077174
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerDestroyed()
	{
		if (this.localPlayer != null)
		{
			this.localPlayer.PartyJoined -= this.playerJoinedParty;
			this.localPlayer.PartyLeave -= this.playerLeftParty;
			this.localPlayer = null;
		}
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x00078FC4 File Offset: 0x000771C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerJoinedParty(Party _affectedParty, EntityPlayer _player)
	{
		this.ReceivedLobbySecret(DiscordManager.ELobbyType.Party, string.Format("{0}_{1}", this.globalLobby.Secret, _affectedParty.PartyID));
		if (this.Settings.AutoJoinVoiceMode == DiscordManager.EAutoJoinVoiceMode.Party && this.ActiveVoiceLobby == null)
		{
			this.JoinLobbyVoice(DiscordManager.ELobbyType.Party);
		}
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x00079015 File Offset: 0x00077215
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerLeftParty(Party _affectedParty, EntityPlayer _player)
	{
		this.LeftParty();
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x00079020 File Offset: 0x00077220
	[PublicizedFrom(EAccessModifier.Private)]
	public void playerBlockStateChanged(IPlatformUserData _ppd, EBlockType _blockType, EUserBlockState _blockState)
	{
		if (_blockType != EBlockType.VoiceChat)
		{
			return;
		}
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		PersistentPlayerData persistentPlayerData = (persistentPlayers != null) ? persistentPlayers.GetPlayerData(_ppd.PrimaryId) : null;
		if (persistentPlayerData == null || persistentPlayerData.EntityId == -1)
		{
			return;
		}
		DiscordManager.DiscordUser user;
		if (!this.TryGetUserFromEntityId(persistentPlayerData.EntityId, out user))
		{
			return;
		}
		DiscordManager.LobbyInfo activeVoiceLobby = this.ActiveVoiceLobby;
		if (activeVoiceLobby == null)
		{
			return;
		}
		activeVoiceLobby.VoiceCall.UpdateBlockState(user, _blockState.IsBlocked());
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x00079089 File Offset: 0x00077289
	public void OpenDiscordSocialSettings()
	{
		this.client.OpenConnectedGamesSettingsInDiscord(delegate(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("OpenConnectedGamesSettingsInDiscord", null, _result, true);
		});
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x000790B8 File Offset: 0x000772B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void localDiscordOrEntityIdChanged()
	{
		if (this.localPlayer == null)
		{
			return;
		}
		DiscordManager.DiscordUserMappingManager discordUserMappingManager = this.userMappings;
		int entityId = this.localPlayer.entityId;
		bool remove = false;
		DiscordManager.DiscordUser discordUser = this.LocalUser;
		discordUserMappingManager.UpdateMapping(entityId, remove, (discordUser != null) ? discordUser.ID : 0UL);
		DiscordManager.DiscordUser discordUser2 = this.LocalUser;
		if (discordUser2 != null)
		{
			discordUser2.UpdatePlayerName(null);
		}
		ConnectionManager connectionManager = SingletonMonoBehaviour<ConnectionManager>.Instance;
		NetPackageDiscordIdMappings package = NetPackageManager.GetPackage<NetPackageDiscordIdMappings>();
		int entityId2 = this.localPlayer.entityId;
		bool remove2 = false;
		DiscordManager.DiscordUser discordUser3 = this.LocalUser;
		connectionManager.SendToClientsOrServer(package.Setup(entityId2, remove2, (discordUser3 != null) ? discordUser3.ID : 0UL));
		if (!GameManager.IsDedicatedServer && this.IsReady)
		{
			this.globalLobby.Join(false);
			this.partyLobby.Join(false);
		}
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x0007916C File Offset: 0x0007736C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateUserNames()
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			discordUser.UpdatePlayerName(null);
		}
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x000791CC File Offset: 0x000773CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void resetPendingOutgoingJoinRequests()
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			discordUser.PendingOutgoingJoinRequest = false;
		}
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x0007922C File Offset: 0x0007742C
	[PublicizedFrom(EAccessModifier.Private)]
	public void refreshCachedUserHandlesAndRelationships()
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			discordUser.TryUpdateDiscordHandle();
		}
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x0007928C File Offset: 0x0007748C
	public int GetPendingActionsCount()
	{
		int num = 0;
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num2;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num2, out discordUser);
			if (discordUser.PendingAction)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x000792F4 File Offset: 0x000774F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePendingActionsEvent()
	{
		DiscordManager.PendingActionsUpdateCallback pendingActionsUpdate = this.PendingActionsUpdate;
		if (pendingActionsUpdate == null)
		{
			return;
		}
		pendingActionsUpdate(this.GetPendingActionsCount());
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0007930C File Offset: 0x0007750C
	public DiscordManager.DiscordUser GetUser(ulong _userId)
	{
		DiscordManager.DiscordUser discordUser;
		if (!this.knownUsers.TryGetValue(_userId, out discordUser))
		{
			discordUser = new DiscordManager.DiscordUser(this, _userId, false);
			this.knownUsers[_userId] = discordUser;
		}
		discordUser.TryUpdateDiscordHandle();
		return discordUser;
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x00079348 File Offset: 0x00077548
	public bool TryGetUserFromEntityId(int _entityId, out DiscordManager.DiscordUser _user)
	{
		ulong key;
		if (!this.userMappings.TryGetDiscordId(_entityId, out key))
		{
			_user = null;
			return false;
		}
		return this.knownUsers.TryGetValue(key, out _user);
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x00079377 File Offset: 0x00077577
	public bool TryGetUserFromEntity(EntityPlayer _entity, out DiscordManager.DiscordUser _user)
	{
		if (_entity == null)
		{
			_user = null;
			return false;
		}
		return this.TryGetUserFromEntityId(_entity.entityId, out _user);
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x00079394 File Offset: 0x00077594
	public void GetAllUsers(IList<DiscordManager.DiscordUser> _target)
	{
		this.knownUsers.CopyValuesTo(_target);
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x000793A4 File Offset: 0x000775A4
	public void GetFriends(HashSet<DiscordManager.DiscordUser> _target)
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			DiscordManager.DiscordUser discordUser2 = discordUser;
			if (discordUser2.IsFriend)
			{
				_target.Add(discordUser2);
			}
		}
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x00079410 File Offset: 0x00077610
	public void GetBlockedUsers(HashSet<DiscordManager.DiscordUser> _target)
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			DiscordManager.DiscordUser discordUser2 = discordUser;
			if (discordUser2.IsBlocked)
			{
				_target.Add(discordUser2);
			}
		}
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0007947C File Offset: 0x0007767C
	public void GetUsersWithPendingAction(HashSet<DiscordManager.DiscordUser> _target)
	{
		foreach (KeyValuePair<ulong, DiscordManager.DiscordUser> keyValuePair in this.knownUsers)
		{
			ulong num;
			DiscordManager.DiscordUser discordUser;
			keyValuePair.Deconstruct(out num, out discordUser);
			DiscordManager.DiscordUser discordUser2 = discordUser;
			if (discordUser2.PendingAction)
			{
				_target.Add(discordUser2);
			}
		}
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x000794E8 File Offset: 0x000776E8
	public void GetInServer(HashSet<DiscordManager.DiscordUser> _target)
	{
		this.userMappings.GetAll(delegate(int _, ulong _discordId)
		{
			if (_discordId <= 0UL)
			{
				return;
			}
			DiscordManager.DiscordUser user = this.GetUser(_discordId);
			if (user.IsLocalAccount)
			{
				return;
			}
			user.RequestAvatar();
			_target.Add(user);
		});
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x00079520 File Offset: 0x00077720
	[PublicizedFrom(EAccessModifier.Private)]
	public static void logCallbackInfo(string _message, LogType _logType = LogType.Log)
	{
		switch (_logType)
		{
		case LogType.Error:
		case LogType.Exception:
			Log.Error("[Discord][CB] " + _message);
			return;
		case LogType.Warning:
			if (DiscordManager.logLevel <= LoggingSeverity.Warning)
			{
				Log.Warning("[Discord][CB] " + _message);
			}
			return;
		}
		if (DiscordManager.logLevel <= LoggingSeverity.Info)
		{
			Log.Out("[Discord][CB] " + _message);
		}
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0007958C File Offset: 0x0007778C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void logCallbackInfoWithClientResult(string _callbackName, string _message, ClientResult _result, bool _disposeClientResult = false)
	{
		bool flag = _result.Type() != ErrorType.None;
		LogType logType = LogType.Error;
		if (!flag)
		{
			logType = LogType.Log;
			if (DiscordManager.logLevel > LoggingSeverity.Info)
			{
				if (_disposeClientResult)
				{
					_result.Dispose();
				}
				return;
			}
		}
		string text = DiscordManager.clientResultToString(_result, _disposeClientResult);
		DiscordManager.logCallbackInfo(string.IsNullOrEmpty(_message) ? (_callbackName + " (" + text + ")") : string.Concat(new string[]
		{
			_callbackName,
			" (",
			text,
			"): ",
			_message
		}), logType);
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x00079608 File Offset: 0x00077808
	[PublicizedFrom(EAccessModifier.Private)]
	public static string clientResultToString(ClientResult _result, bool _dispose = false)
	{
		string result = string.Format("{0}/{1}/{2}/'{3}'", new object[]
		{
			_result.Type().ToStringCached<ErrorType>(),
			_result.ErrorCode(),
			_result.Status().ToStringCached<HttpStatusCode>(),
			_result.Error()
		});
		if (_dispose)
		{
			_result.Dispose();
		}
		return result;
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x000796D7 File Offset: 0x000778D7
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <registerGameStartupForInvites>g__RegisterSteamLauncher|97_0()
	{
		return this.client.RegisterLaunchSteamApplication(1296840202995896363UL, 251570U);
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x000796F4 File Offset: 0x000778F4
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <registerGameStartupForInvites>g__RegisterNativePcLauncher|97_1()
	{
		RuntimePlatform platform = Application.platform;
		if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
		{
			return this.client.RegisterLaunchCommand(1296840202995896363UL, "com.company.7dLauncher");
		}
		return this.client.RegisterLaunchCommand(1296840202995896363UL, GameIO.GetLauncherExecutablePath());
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x00079744 File Offset: 0x00077944
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <registerGameChatHandling>g__DiscordButtonHandler|120_0(XUiView _sender, string _sourceUrl, Dictionary<string, string> _urlElements)
	{
		string input;
		if (!_urlElements.TryGetValue("MessageId", out input))
		{
			Log.Warning("DiscordButton URL (" + _sourceUrl + "): No MessageId defined");
			return;
		}
		ulong messageId;
		if (!StringParsers.TryParseUInt64(input, out messageId, 0, -1, NumberStyles.Integer))
		{
			Log.Warning("DiscordButton URL (" + _sourceUrl + "): Invalid MessageId");
			return;
		}
		this.client.OpenMessageInDiscord(messageId, new Discord.Sdk.Client.ProvisionalUserMergeRequiredCallback(DiscordManager.<registerGameChatHandling>g__ProvisionalUserMergeRequiredCallback|120_5), delegate(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("OpenMessageInDiscord", messageId.ToString(), _result, true);
		});
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x000797CC File Offset: 0x000779CC
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <registerGameChatHandling>g__ProvisionalUserMergeRequiredCallback|120_5()
	{
		Log.Warning("[Discord] ProvisionalUserMergeRequiredCallback fired!");
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x000797D8 File Offset: 0x000779D8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <registerGameChatHandling>g__SendMessage|120_1(EChatType _chatType, string _targetId, string _message)
	{
		ulong num;
		if (!ulong.TryParse(_targetId, out num))
		{
			throw new ArgumentException("Could not parse chat Discord id '" + _targetId + "'");
		}
		this.GetUser(num).MessageSentFromGame = true;
		this.client.SendUserMessage(num, _message, delegate(ClientResult _result, ulong _messageId)
		{
			DiscordManager.logCallbackInfoWithClientResult("SendUserMessage", _messageId.ToString(), _result, true);
		});
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x00079840 File Offset: 0x00077A40
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public string <registerGameChatHandling>g__GetTargetDisplayName|120_2(EChatType _chatType, string _targetId)
	{
		ulong userId;
		if (!ulong.TryParse(_targetId, out userId))
		{
			throw new ArgumentException("Could not parse chat Discord id '" + _targetId + "'");
		}
		DiscordManager.DiscordUser user = this.GetUser(userId);
		return string.Format(Localization.Get("xuiChatTargetWhisper", false), user.DisplayName + " [discord] ");
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x00079898 File Offset: 0x00077A98
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <registerGameChatHandling>g__IsValidTarget|120_3(EChatType _chatType, string _targetId)
	{
		if (_targetId == null)
		{
			return false;
		}
		ulong num;
		if (!ulong.TryParse(_targetId, out num))
		{
			throw new ArgumentException("Could not parse chat Discord id '" + _targetId + "' for target validation");
		}
		ulong num2 = num;
		DiscordManager.DiscordUser discordUser = this.LocalUser;
		return num2 != ((discordUser != null) ? discordUser.ID : 0UL);
	}

	// Token: 0x04000CDB RID: 3291
	[PublicizedFrom(EAccessModifier.Private)]
	public const ulong DiscordApplicationId = 1296840202995896363UL;

	// Token: 0x04000CDC RID: 3292
	[PublicizedFrom(EAccessModifier.Private)]
	public const ulong DiscordClientId = 1296840202995896363UL;

	// Token: 0x04000CDD RID: 3293
	[PublicizedFrom(EAccessModifier.Private)]
	public static DiscordManager instance;

	// Token: 0x04000CEE RID: 3310
	public readonly DiscordManager.DiscordSettings Settings;

	// Token: 0x04000CEF RID: 3311
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DiscordManager.DiscordUserSettingsManager UserSettings;

	// Token: 0x04000CF0 RID: 3312
	[PublicizedFrom(EAccessModifier.Private)]
	public Discord.Sdk.Client client;

	// Token: 0x04000CF1 RID: 3313
	[PublicizedFrom(EAccessModifier.Private)]
	public DiscordManager.DiscordUser localUser;

	// Token: 0x04000CF2 RID: 3314
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DiscordManager.DiscordUserMappingManager userMappings;

	// Token: 0x04000CF3 RID: 3315
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<ulong, DiscordManager.DiscordUser> knownUsers = new Dictionary<ulong, DiscordManager.DiscordUser>();

	// Token: 0x04000CF4 RID: 3316
	[PublicizedFrom(EAccessModifier.Private)]
	public static LoggingSeverity logLevel = LoggingSeverity.Warning;

	// Token: 0x04000CF5 RID: 3317
	[PublicizedFrom(EAccessModifier.Private)]
	public static LoggingSeverity logLevelRtc = LoggingSeverity.Warning;

	// Token: 0x04000CF6 RID: 3318
	public readonly DiscordManager.AuthAndLoginManager AuthManager;

	// Token: 0x04000CF7 RID: 3319
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex logMessageMatcher = new Regex("^\\[([^\\]]+)\\] \\[(\\d+)\\] \\(([^)]+)\\): (.*)\\n$", RegexOptions.Compiled | RegexOptions.Singleline);

	// Token: 0x04000CF8 RID: 3320
	[PublicizedFrom(EAccessModifier.Private)]
	public const string UrlTypeDiscordMessageButton = "DiscordMessageButton";

	// Token: 0x04000CF9 RID: 3321
	[PublicizedFrom(EAccessModifier.Private)]
	public const string UrlFieldMessageId = "MessageId";

	// Token: 0x04000CFA RID: 3322
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DiscordManager.LobbyInfo globalLobby;

	// Token: 0x04000CFB RID: 3323
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DiscordManager.LobbyInfo partyLobby;

	// Token: 0x04000CFC RID: 3324
	public readonly DiscordManager.PresenceManager Presence;

	// Token: 0x04000CFD RID: 3325
	public readonly DiscordManager.AudioDeviceConfig AudioOutput;

	// Token: 0x04000CFE RID: 3326
	public readonly DiscordManager.AudioDeviceConfig AudioInput;

	// Token: 0x04000CFF RID: 3327
	[PublicizedFrom(EAccessModifier.Private)]
	public float nextActivityUpdate;

	// Token: 0x04000D00 RID: 3328
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal localPlayer;

	// Token: 0x04000D01 RID: 3329
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CurrentFileVersion = 1;

	// Token: 0x020002BC RID: 700
	public class AuthAndLoginManager
	{
		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06001401 RID: 5121 RVA: 0x000798E3 File Offset: 0x00077AE3
		// (set) Token: 0x06001402 RID: 5122 RVA: 0x000798EB File Offset: 0x00077AEB
		public DiscordManager.EDiscordAccountType IsLoggingInWith { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x000798F4 File Offset: 0x00077AF4
		public bool IsLoggingIn
		{
			get
			{
				return this.IsLoggingInWith > DiscordManager.EDiscordAccountType.None;
			}
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x000798FF File Offset: 0x00077AFF
		public AuthAndLoginManager(DiscordManager _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0007990E File Offset: 0x00077B0E
		public void RegisterDiscordCallbacks()
		{
			this.owner.client.SetStatusChangedCallback(new Discord.Sdk.Client.OnStatusChanged(this.OnStatusChanged));
			this.owner.client.SetTokenExpirationCallback(new Discord.Sdk.Client.TokenExpirationCallback(this.OnTokenExpiration));
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x00079948 File Offset: 0x00077B48
		public void LoginWithPlatformDefaultAccountType()
		{
			if (DiscordManager.SupportsProvisionalAccounts)
			{
				this.LoginProvisionalAccount();
				return;
			}
			this.LoginDiscordUser();
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x00079960 File Offset: 0x00077B60
		public void AutoLogin()
		{
			switch (this.owner.Settings.LastAccountType)
			{
			case DiscordManager.EDiscordAccountType.None:
				this.LoginWithPlatformDefaultAccountType();
				return;
			case DiscordManager.EDiscordAccountType.Regular:
				this.LoginDiscordUser();
				return;
			case DiscordManager.EDiscordAccountType.Provisional:
				this.LoginProvisionalAccount();
				return;
			default:
				throw new ArgumentOutOfRangeException("LastAccountType");
			}
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x000799B1 File Offset: 0x00077BB1
		[PublicizedFrom(EAccessModifier.Private)]
		public void prepareLoginStart(DiscordManager.EDiscordAccountType _accountType)
		{
			this.IsLoggingInWith = _accountType;
			this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.None;
			this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.None;
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x000799C8 File Offset: 0x00077BC8
		public void AbortAuth()
		{
			if (!this.inAuthProcess)
			{
				return;
			}
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
			{
				this.owner.client.AbortGetTokenFromDevice();
			}
			else
			{
				this.owner.client.AbortAuthorize();
			}
			this.inAuthProcess = false;
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00079A08 File Offset: 0x00077C08
		public void LoginDiscordUser()
		{
			this.owner.Init(false);
			if (this.owner.IsReady && !this.owner.LocalUser.IsProvisionalAccount)
			{
				Log.Out("[Discord] Already logged in");
				return;
			}
			this.prepareLoginStart(DiscordManager.EDiscordAccountType.Regular);
			if (!string.IsNullOrEmpty(this.owner.Settings.AccessToken))
			{
				this.loginWithStoredTokens();
				return;
			}
			if (!string.IsNullOrEmpty(this.owner.Settings.RefreshToken))
			{
				this.refreshToken();
				return;
			}
			Log.Out("[Discord] Logging in with Discord user");
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
			{
				this.loginDiscordUserConsole();
				return;
			}
			this.loginDiscordUserPC();
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x00079AAE File Offset: 0x00077CAE
		[PublicizedFrom(EAccessModifier.Private)]
		public void loginWithStoredTokens()
		{
			Log.Out("[Discord] Logging in with existing access token");
			this.owner.client.UpdateToken(AuthorizationTokenType.Bearer, this.owner.Settings.AccessToken, new Discord.Sdk.Client.UpdateTokenCallback(this.updateTokenCallback));
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x00079AE8 File Offset: 0x00077CE8
		[PublicizedFrom(EAccessModifier.Private)]
		public void loginDiscordUserPC()
		{
			AuthorizationCodeVerifier codeVerifier = this.owner.client.CreateAuthorizationCodeVerifier();
			AuthorizationArgs authorizationArgs = new AuthorizationArgs();
			authorizationArgs.SetClientId(1296840202995896363UL);
			authorizationArgs.SetScopes(Discord.Sdk.Client.GetDefaultCommunicationScopes());
			authorizationArgs.SetCodeChallenge(codeVerifier.Challenge());
			DiscordManager.UserAuthorizationResultCallback userAuthorizationResult = this.owner.UserAuthorizationResult;
			if (userAuthorizationResult != null)
			{
				userAuthorizationResult(false, DiscordManager.EFullAccountLoginResult.RequestingAuth, DiscordManager.EProvisionalAccountLoginResult.None, false);
			}
			this.inAuthProcess = true;
			this.owner.client.Authorize(authorizationArgs, delegate(ClientResult _result, string _code, string _uri)
			{
				this.inAuthProcess = false;
				ErrorType errorType = _result.Type();
				int num = _result.ErrorCode();
				DiscordManager.logCallbackInfoWithClientResult("Authorize (PC)", string.Concat(new string[]
				{
					"code='",
					_code,
					"' uri='",
					_uri,
					"'"
				}), _result, true);
				if (this.owner.client == null)
				{
					return;
				}
				if (errorType != ErrorType.None)
				{
					if (errorType == ErrorType.Aborted)
					{
						this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.AuthCancelled;
						Log.Out("[Discord] Auth aborted");
					}
					else if (num == 5000)
					{
						this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.AuthCancelled;
						Log.Out("[Discord] Auth cancelled");
					}
					else
					{
						this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.AuthFailed;
						Log.Out("[Discord] Auth failed");
					}
					DiscordManager.UserAuthorizationResultCallback userAuthorizationResult2 = this.owner.UserAuthorizationResult;
					if (userAuthorizationResult2 != null)
					{
						userAuthorizationResult2(false, this.fullAccountLoginResult, DiscordManager.EProvisionalAccountLoginResult.None, false);
					}
					this.loginProvisionalAccountInternal(false);
					return;
				}
				DiscordManager.UserAuthorizationResultCallback userAuthorizationResult3 = this.owner.UserAuthorizationResult;
				if (userAuthorizationResult3 != null)
				{
					userAuthorizationResult3(false, DiscordManager.EFullAccountLoginResult.AuthAccepted, DiscordManager.EProvisionalAccountLoginResult.None, false);
				}
				if (!DiscordManager.SupportsProvisionalAccounts)
				{
					this.owner.client.GetToken(1296840202995896363UL, _code, codeVerifier.Verifier(), _uri, new Discord.Sdk.Client.TokenExchangeCallback(this.tokenExchangeCallbackFullAccount));
					return;
				}
				string authTicket = PlatformManager.CrossplatformPlatform.AuthenticationClient.GetAuthTicket();
				if (string.IsNullOrEmpty(authTicket))
				{
					Log.Error("[Discord] Logging in with merged account failed, could not fetch EOS IdToken");
					this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.PlatformError;
					this.invokeAuthResultCallback(false);
					return;
				}
				this.owner.client.GetTokenFromProvisionalMerge(1296840202995896363UL, _code, codeVerifier.Verifier(), _uri, AuthenticationExternalAuthType.EpicOnlineServicesIdToken, authTicket, new Discord.Sdk.Client.TokenExchangeCallback(this.tokenExchangeCallbackFullAccount));
			});
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x00079B88 File Offset: 0x00077D88
		[PublicizedFrom(EAccessModifier.Private)]
		public void loginDiscordUserConsole()
		{
			DeviceAuthorizationArgs deviceAuthorizationArgs = new DeviceAuthorizationArgs();
			deviceAuthorizationArgs.SetClientId(1296840202995896363UL);
			deviceAuthorizationArgs.SetScopes(Discord.Sdk.Client.GetDefaultCommunicationScopes());
			string authTicket = PlatformManager.CrossplatformPlatform.AuthenticationClient.GetAuthTicket();
			if (string.IsNullOrEmpty(authTicket))
			{
				Log.Error("[Discord] Logging in with merged account failed, could not fetch EOS IdToken");
				this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.PlatformError;
				this.invokeAuthResultCallback(false);
				return;
			}
			this.inAuthProcess = true;
			this.owner.client.GetTokenFromDeviceProvisionalMerge(deviceAuthorizationArgs, AuthenticationExternalAuthType.EpicOnlineServicesIdToken, authTicket, new Discord.Sdk.Client.TokenExchangeCallback(this.tokenExchangeCallbackFullAccount));
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00079C10 File Offset: 0x00077E10
		[PublicizedFrom(EAccessModifier.Private)]
		public void tokenExchangeCallbackFullAccount(ClientResult _result, string _accessToken, string _refreshToken, AuthorizationTokenType _tokenType, int _expiresIn, string _scope)
		{
			this.inAuthProcess = false;
			DiscordManager.logCallbackInfoWithClientResult("OnTokenExchange (Full)", string.Format("tokenType={0}, expires={1}, scope='{2}'", _tokenType.ToStringCached<AuthorizationTokenType>(), _expiresIn, _scope), _result, true);
			if (_accessToken != "")
			{
				this.owner.Settings.AccessToken = _accessToken;
				this.owner.Settings.RefreshToken = _refreshToken;
				this.owner.client.UpdateToken(AuthorizationTokenType.Bearer, _accessToken, new Discord.Sdk.Client.UpdateTokenCallback(this.updateTokenCallback));
				return;
			}
			Log.Warning("[Discord] Failed retrieving account token!");
			this.owner.Settings.LastAccountType = DiscordManager.EDiscordAccountType.None;
			this.owner.Settings.AccessToken = null;
			this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.TokenExchangeFailed;
			this.loginProvisionalAccountInternal(false);
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00079CD4 File Offset: 0x00077ED4
		[PublicizedFrom(EAccessModifier.Private)]
		public void refreshToken()
		{
			string refreshToken = this.owner.Settings.RefreshToken;
			if (string.IsNullOrEmpty(refreshToken))
			{
				Log.Warning("[Discord] No refresh token");
				return;
			}
			Log.Out("[Discord] Trying to refresh access token");
			this.owner.Settings.RefreshToken = null;
			this.owner.client.RefreshToken(1296840202995896363UL, refreshToken, new Discord.Sdk.Client.TokenExchangeCallback(this.<refreshToken>g__RefreshTokenExchangeCallback|20_0));
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00079D48 File Offset: 0x00077F48
		public void UnmergeAccount()
		{
			this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.None;
			this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.None;
			this.IsLoggingInWith = DiscordManager.EDiscordAccountType.Provisional;
			if (!DiscordManager.SupportsProvisionalAccounts)
			{
				Log.Error("[Discord] Unmerging account only available when running with EOS");
				return;
			}
			if (!this.owner.IsReady || this.owner.LocalUser.IsProvisionalAccount)
			{
				Log.Out("[Discord] Not logged in with full account");
				return;
			}
			string authTicket = PlatformManager.CrossplatformPlatform.AuthenticationClient.GetAuthTicket();
			if (string.IsNullOrEmpty(authTicket))
			{
				Log.Error("[Discord] Unmerging account failed, could not fetch EOS IdToken");
				return;
			}
			this.owner.client.UnmergeIntoProvisionalAccount(1296840202995896363UL, AuthenticationExternalAuthType.EpicOnlineServicesIdToken, authTicket, delegate(ClientResult _result)
			{
				bool flag = _result.Type() != ErrorType.None;
				DiscordManager.logCallbackInfoWithClientResult("UnmergeIntoProvisionalAccount", null, _result, true);
				if (flag)
				{
					this.invokeAuthResultCallback(false);
					return;
				}
			});
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00079DF0 File Offset: 0x00077FF0
		public void LoginProvisionalAccount()
		{
			this.owner.Init(false);
			this.prepareLoginStart(DiscordManager.EDiscordAccountType.Provisional);
			this.loginProvisionalAccountInternal(false);
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x00079E0C File Offset: 0x0007800C
		[PublicizedFrom(EAccessModifier.Private)]
		public void loginProvisionalAccountInternal(bool _isRefresh)
		{
			if (!DiscordManager.SupportsProvisionalAccounts)
			{
				Log.Error("[Discord] Provisional account login only available when running with EOS");
				this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.NotSupported;
				this.invokeAuthResultCallback(false);
				return;
			}
			if (this.owner.IsReady && !_isRefresh)
			{
				Log.Out("[Discord] Already logged in");
				this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.Success;
				this.IsLoggingInWith = DiscordManager.EDiscordAccountType.Provisional;
				this.invokeAuthResultCallback(true);
				return;
			}
			if (PlatformManager.CrossplatformPlatform.User.UserStatus != EUserStatus.LoggedIn)
			{
				Log.Out("[Discord] Can not log in with provisional account, not logged in to cross platform provider");
				this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.PlatformError;
				this.invokeAuthResultCallback(false);
				return;
			}
			Log.Out(_isRefresh ? "[Discord] Refreshing provisional account token" : "[Discord] Logging in with provisional account");
			string authTicket = PlatformManager.CrossplatformPlatform.AuthenticationClient.GetAuthTicket();
			if (string.IsNullOrEmpty(authTicket))
			{
				Log.Error("[Discord] Logging in with provisional account failed, could not fetch EOS IdToken");
				this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.PlatformError;
				this.invokeAuthResultCallback(false);
				return;
			}
			this.owner.client.GetProvisionalToken(1296840202995896363UL, AuthenticationExternalAuthType.EpicOnlineServicesIdToken, authTicket, new Discord.Sdk.Client.TokenExchangeCallback(this.<loginProvisionalAccountInternal>g__TokenExchangeCallbackProvisional|24_0));
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00079EFF File Offset: 0x000780FF
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateTokenCallback(ClientResult _result)
		{
			bool flag = _result.Type() != ErrorType.None;
			DiscordManager.logCallbackInfoWithClientResult("UpdateToken", null, _result, true);
			if (flag)
			{
				Log.Error("[Discord] UpdateToken failed!");
				return;
			}
			this.owner.client.Connect();
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x00079F34 File Offset: 0x00078134
		public void Disconnect()
		{
			this.IsLoggingInWith = DiscordManager.EDiscordAccountType.None;
			if (this.owner.client == null)
			{
				return;
			}
			this.owner.leaveLobbies(true);
			if (this.owner.Status != DiscordManager.EDiscordStatus.Disconnected)
			{
				this.owner.client.Disconnect();
			}
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x00079F80 File Offset: 0x00078180
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnStatusChanged(Discord.Sdk.Client.Status _status, Discord.Sdk.Client.Error _error, int _errorDetail)
		{
			DiscordManager.logCallbackInfo(string.Format("OnStatusChanged status={0} error={1} errorDetail={2}", _status, _error, _errorDetail), (_error == Discord.Sdk.Client.Error.None) ? LogType.Log : LogType.Error);
			if (_status == Discord.Sdk.Client.Status.Disconnected && _error == Discord.Sdk.Client.Error.ConnectionFailed)
			{
				if (this.IsLoggingInWith == DiscordManager.EDiscordAccountType.Regular)
				{
					this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.ConnectionFailed;
				}
				else
				{
					this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.ConnectionFailed;
				}
				this.invokeAuthResultCallback(false);
			}
			if (_errorDetail == 4004)
			{
				if (!string.IsNullOrEmpty(this.owner.Settings.RefreshToken))
				{
					this.refreshToken();
					return;
				}
				this.loginProvisionalAccountInternal(false);
				return;
			}
			else
			{
				if (_errorDetail == 4003)
				{
					DiscordManager.DiscordUser localUser = this.owner.LocalUser;
					if (localUser == null || !localUser.IsProvisionalAccount)
					{
						this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.TokenRevoked;
						this.owner.Settings.AccessToken = null;
						this.owner.Settings.RefreshToken = null;
						this.loginProvisionalAccountInternal(false);
						return;
					}
				}
				switch (_status)
				{
				case Discord.Sdk.Client.Status.Disconnected:
				case Discord.Sdk.Client.Status.Disconnecting:
					this.IsLoggingInWith = DiscordManager.EDiscordAccountType.None;
					this.owner.leaveLobbies(false);
					if (this.owner.LocalUser != null)
					{
						this.owner.LocalUser.Dispose();
						this.owner.LocalUser = null;
					}
					this.owner.clearFriends();
					break;
				case Discord.Sdk.Client.Status.Connecting:
				case Discord.Sdk.Client.Status.Connected:
				case Discord.Sdk.Client.Status.Reconnecting:
				case Discord.Sdk.Client.Status.HttpWait:
					break;
				case Discord.Sdk.Client.Status.Ready:
				{
					UserHandle currentUser = this.owner.client.GetCurrentUser();
					bool flag = currentUser.IsProvisional();
					this.owner.Settings.LastAccountType = (flag ? DiscordManager.EDiscordAccountType.Provisional : DiscordManager.EDiscordAccountType.Regular);
					ulong num = currentUser.Id();
					bool flag2 = this.owner.LocalUser == null || this.owner.LocalUser.ID != num;
					if (flag2)
					{
						DiscordManager.DiscordUser localUser2 = this.owner.LocalUser;
						if (localUser2 != null)
						{
							localUser2.Dispose();
						}
						this.owner.LocalUser = new DiscordManager.DiscordUser(this.owner, num, true);
						this.owner.knownUsers[num] = this.owner.LocalUser;
					}
					this.owner.knownUsers[this.owner.LocalUser.ID] = this.owner.LocalUser;
					if (flag)
					{
						this.owner.client.UpdateProvisionalAccountDisplayName(GamePrefs.GetString(EnumGamePrefs.PlayerName), delegate(ClientResult _result)
						{
							DiscordManager.logCallbackInfoWithClientResult("UpdateProvisionalAccountDisplayName", null, _result, true);
						});
					}
					this.owner.getFriends();
					this.owner.Presence.SetRichPresenceState(null);
					this.owner.Settings.Save();
					if (!flag)
					{
						this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.Success;
					}
					else
					{
						this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.Success;
					}
					if (flag2)
					{
						Log.Out("[Discord] Logged in");
						this.invokeAuthResultCallback(true);
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("_status", _status, null);
				}
				Action<DiscordManager.EDiscordStatus> statusChanged = this.owner.StatusChanged;
				if (statusChanged == null)
				{
					return;
				}
				statusChanged(this.owner.Status);
				return;
			}
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0007A266 File Offset: 0x00078466
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnTokenExpiration()
		{
			DiscordManager.logCallbackInfo("OnTokenExpiration", LogType.Log);
			if (this.owner.LocalUser.IsProvisionalAccount)
			{
				this.loginProvisionalAccountInternal(true);
				return;
			}
			this.refreshToken();
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0007A294 File Offset: 0x00078494
		[PublicizedFrom(EAccessModifier.Private)]
		public void invokeAuthResultCallback(bool _success)
		{
			bool isExpectedSuccess = this.fullAccountLoginResult == DiscordManager.EFullAccountLoginResult.Success || (this.fullAccountLoginResult == DiscordManager.EFullAccountLoginResult.None && this.provisionalAccountLoginResult == DiscordManager.EProvisionalAccountLoginResult.Success);
			DiscordManager.UserAuthorizationResultCallback userAuthorizationResult = this.owner.UserAuthorizationResult;
			if (userAuthorizationResult != null)
			{
				userAuthorizationResult(true, this.fullAccountLoginResult, this.provisionalAccountLoginResult, isExpectedSuccess);
			}
			if (!_success)
			{
				this.IsLoggingInWith = DiscordManager.EDiscordAccountType.None;
			}
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x0007A2F0 File Offset: 0x000784F0
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <refreshToken>g__RefreshTokenExchangeCallback|20_0(ClientResult _result, string _accessToken, string _refreshToken, AuthorizationTokenType _tokenType, int _expiresIn, string _scope)
		{
			DiscordManager.logCallbackInfoWithClientResult("OnTokenExchange (Refresh)", string.Format("tokenType={0}, expires={1}, scope='{2}'", _tokenType.ToStringCached<AuthorizationTokenType>(), _expiresIn, _scope), _result, true);
			if (_accessToken != "")
			{
				this.owner.Settings.AccessToken = _accessToken;
				this.owner.Settings.RefreshToken = _refreshToken;
				this.owner.client.UpdateToken(AuthorizationTokenType.Bearer, _accessToken, new Discord.Sdk.Client.UpdateTokenCallback(this.updateTokenCallback));
				return;
			}
			Log.Warning("[Discord] Failed refreshing token!");
			this.owner.Settings.LastAccountType = DiscordManager.EDiscordAccountType.None;
			this.owner.Settings.AccessToken = null;
			this.fullAccountLoginResult = DiscordManager.EFullAccountLoginResult.TokenRefreshFailed;
			this.loginProvisionalAccountInternal(false);
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x0007A3CC File Offset: 0x000785CC
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <loginProvisionalAccountInternal>g__TokenExchangeCallbackProvisional|24_0(ClientResult _result, string _accessToken, string _refreshToken, AuthorizationTokenType _tokenType, int _expiresIn, string _scope)
		{
			if (_result.ErrorCode() == 530010)
			{
				this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.PlatformIdLinkedToDiscordAccount;
				Log.Out("[Discord] Can not login with provisional account, platform ID already linked to a Discord account");
				this.invokeAuthResultCallback(false);
				_result.Dispose();
				return;
			}
			DiscordManager.logCallbackInfoWithClientResult("OnTokenExchange (Provisional)", string.Format("tokenType={0}, expires={1}, scope='{2}'", _tokenType.ToStringCached<AuthorizationTokenType>(), _expiresIn, _scope), _result, true);
			if (_accessToken != "")
			{
				this.owner.client.UpdateToken(AuthorizationTokenType.Bearer, _accessToken, new Discord.Sdk.Client.UpdateTokenCallback(this.updateTokenCallback));
				return;
			}
			Log.Warning("[Discord] Failed retrieving provisional token!");
			this.provisionalAccountLoginResult = DiscordManager.EProvisionalAccountLoginResult.TokenExchangeFailed;
			this.invokeAuthResultCallback(false);
		}

		// Token: 0x04000D02 RID: 3330
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager owner;

		// Token: 0x04000D04 RID: 3332
		[PublicizedFrom(EAccessModifier.Private)]
		public DiscordManager.EFullAccountLoginResult fullAccountLoginResult;

		// Token: 0x04000D05 RID: 3333
		[PublicizedFrom(EAccessModifier.Private)]
		public bool inAuthProcess;

		// Token: 0x04000D06 RID: 3334
		[PublicizedFrom(EAccessModifier.Private)]
		public DiscordManager.EProvisionalAccountLoginResult provisionalAccountLoginResult;
	}

	// Token: 0x020002BF RID: 703
	public class CallInfo
	{
		// Token: 0x06001420 RID: 5152 RVA: 0x0007A668 File Offset: 0x00078868
		public static void LoadSounds()
		{
			LoadManager.LoadAsset<AudioClip>("@:Sounds/UI/ui_discord_join.wav", delegate(AudioClip _o)
			{
				DiscordManager.CallInfo.soundOtherJoin = _o;
			}, null, false, false);
			LoadManager.LoadAsset<AudioClip>("@:Sounds/UI/ui_discord_leave.wav", delegate(AudioClip _o)
			{
				DiscordManager.CallInfo.soundOtherLeave = _o;
			}, null, false, false);
			LoadManager.LoadAsset<AudioClip>("@:Sounds/UI/ui_discord_kicked.wav", delegate(AudioClip _o)
			{
				DiscordManager.CallInfo.soundSelfNoManualLeave = _o;
			}, null, false, false);
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06001421 RID: 5153 RVA: 0x0007A6FC File Offset: 0x000788FC
		// (set) Token: 0x06001422 RID: 5154 RVA: 0x0007A704 File Offset: 0x00078904
		public Call.Status Status { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x0007A70D File Offset: 0x0007890D
		public bool IsJoined
		{
			get
			{
				return this.call != null && this.Status == Call.Status.Connected;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06001424 RID: 5156 RVA: 0x0007A722 File Offset: 0x00078922
		public Call Call
		{
			get
			{
				return this.call;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x0007A72A File Offset: 0x0007892A
		// (set) Token: 0x06001426 RID: 5158 RVA: 0x0007A732 File Offset: 0x00078932
		public bool IsSpeaking { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x06001427 RID: 5159 RVA: 0x0007A73C File Offset: 0x0007893C
		public CallInfo(DiscordManager.LobbyInfo _ownerLobby, DiscordManager _ownerManager)
		{
			this.ownerLobby = _ownerLobby;
			this.ownerManager = _ownerManager;
			this.ownerManager.Settings.VoiceVadModeChanged += this.OnVadModeChanged;
			this.ownerManager.Settings.VoiceVadThresholdChanged += this.OnVadThresholdChanged;
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0007A7AC File Offset: 0x000789AC
		public void Join()
		{
			if (!this.ownerLobby.IsJoined)
			{
				Log.Error("[Discord] Failed to start call for lobby, lobby not entered yet");
				return;
			}
			this.startedJoining = true;
			this.manualLeave = false;
			this.Status = Call.Status.Joining;
			DiscordManager.CallStatusChangedCallback callStatusChanged = this.ownerManager.CallStatusChanged;
			if (callStatusChanged != null)
			{
				callStatusChanged(this, this.Status);
			}
			this.call = this.ownerManager.client.StartCall(this.ownerLobby.Id);
			if (this.call == null)
			{
				Log.Error(string.Format("[Discord] Failed to start call for lobby {0}", this.ownerLobby.Id));
				this.Leave(false);
				return;
			}
			this.SetPushToTalkMode();
			this.call.SetPTTReleaseDelay(200U);
			this.call.SetVADThreshold(this.ownerManager.Settings.VoiceVadModeAuto, (float)this.ownerManager.Settings.VoiceVadThreshold);
			this.call.SetStatusChangedCallback(new Call.OnStatusChanged(this.OnCallStatusChanged));
			this.call.SetParticipantChangedCallback(new Call.OnParticipantChanged(this.OnParticipantChanged));
			this.call.SetOnVoiceStateChangedCallback(new Call.OnVoiceStateChanged(this.OnVoiceStateChanged));
			this.call.SetSpeakingStatusChangedCallback(new Call.OnSpeakingStatusChanged(this.OnSpeakingStatusChanged));
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x0007A8F4 File Offset: 0x00078AF4
		public void Leave(bool _manual)
		{
			this.startedJoining = false;
			this.manualLeave = _manual;
			this.Status = Call.Status.Disconnected;
			this.IsSpeaking = false;
			this.ownerManager.client.EndCall(this.ownerLobby.Id, delegate
			{
			});
			Call call = this.call;
			if (call != null)
			{
				call.Dispose();
			}
			this.call = null;
			this.UpdateMembers(false);
			DiscordManager.CallChangedCallback callChanged = this.ownerManager.CallChanged;
			if (callChanged == null)
			{
				return;
			}
			callChanged(null);
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0007A98C File Offset: 0x00078B8C
		public void SetPushToTalkMode()
		{
			this.call.SetAudioMode(this.ownerManager.Settings.VoiceModePtt ? AudioModeType.MODE_PTT : AudioModeType.MODE_VAD);
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0007A9AF File Offset: 0x00078BAF
		public void SetPushToTalkActive(bool _pushToTalkPressed)
		{
			this.call.SetPTTActive(_pushToTalkPressed);
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x0007A9BD File Offset: 0x00078BBD
		public float GetParticipantVolume(DiscordManager.DiscordUser _user)
		{
			Call call = this.call;
			if (call == null)
			{
				return 0f;
			}
			return call.GetParticipantVolume(_user.ID);
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0007A9DA File Offset: 0x00078BDA
		public void SetParticipantVolume(DiscordManager.DiscordUser _user, float _volume)
		{
			Call call = this.call;
			if (call == null)
			{
				return;
			}
			call.SetParticipantVolume(_user.ID, _volume);
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0007A9F4 File Offset: 0x00078BF4
		public void UpdateMembers(bool _applySavedVolume = false)
		{
			if (!this.IsJoined)
			{
				this.callMembersCurrent.Clear();
				this.callMembersOld.Clear();
			}
			else
			{
				Dictionary<ulong, DiscordManager.CallInfo.MemberState> dictionary = this.callMembersCurrent;
				Dictionary<ulong, DiscordManager.CallInfo.MemberState> dictionary2 = this.callMembersOld;
				this.callMembersOld = dictionary;
				this.callMembersCurrent = dictionary2;
				this.callMembersCurrent.Clear();
				foreach (ulong num in this.call.GetParticipants())
				{
					DiscordManager.DiscordUser user = this.ownerManager.GetUser(num);
					using (VoiceStateHandle voiceStateHandle = this.call.GetVoiceStateHandle(num))
					{
						DiscordManager.CallInfo.MemberState value;
						if (!this.callMembersOld.TryGetValue(num, out value))
						{
							value = default(DiscordManager.CallInfo.MemberState);
						}
						value.Speaking = false;
						value.Muted = voiceStateHandle.SelfMute();
						value.Deafened = voiceStateHandle.SelfDeaf();
						this.callMembersCurrent.Add(num, value);
						if (_applySavedVolume)
						{
							user.Volume = this.ownerManager.UserSettings.GetUserVolume(user.ID);
						}
					}
				}
				this.callMembersOld.Clear();
			}
			DiscordManager.CallMembersChangedCallback callMembersChanged = this.ownerManager.CallMembersChanged;
			if (callMembersChanged == null)
			{
				return;
			}
			callMembersChanged(this);
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0007AB3C File Offset: 0x00078D3C
		public bool TryGetMember(ulong _userId, out DiscordManager.CallInfo.MemberState _state)
		{
			return this.callMembersCurrent.TryGetValue(_userId, out _state);
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0007AB4B File Offset: 0x00078D4B
		public void GetMembers(List<ulong> _target)
		{
			this.callMembersCurrent.CopyKeysTo(_target);
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0007AB5C File Offset: 0x00078D5C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnCallStatusChanged(Call.Status _status, Call.Error _error, int _errorDetail)
		{
			DiscordManager.logCallbackInfo(string.Format("Call: Status changed: status={0} error={1} errorDetail={2}", _status.ToStringCached<Call.Status>(), _error.ToStringCached<Call.Error>(), _errorDetail), (_error == Call.Error.None) ? LogType.Log : LogType.Error);
			if (this.startedJoining)
			{
				this.Status = _status;
			}
			switch (_status)
			{
			case Call.Status.Disconnected:
			case Call.Status.Disconnecting:
				if (!this.manualLeave)
				{
					Manager.PlayXUiSound(DiscordManager.CallInfo.soundSelfNoManualLeave, 1f);
				}
				this.IsSpeaking = false;
				if (this.startedJoining)
				{
					this.Leave(false);
				}
				break;
			case Call.Status.Joining:
			case Call.Status.Connecting:
			case Call.Status.SignalingConnected:
			case Call.Status.Reconnecting:
				break;
			case Call.Status.Connected:
			{
				if (DiscordManager.logLevel == LoggingSeverity.Verbose)
				{
					float inputVolume = this.ownerManager.client.GetInputVolume();
					AudioModeType audioMode = this.call.GetAudioMode();
					VADThresholdSettings vadthreshold = this.call.GetVADThreshold();
					Log.Out(string.Format("[Discord] AUDIO SETTINGS: InputVolume={0}, AudioMode={1}, VAD auto={2}, threshold={3}", new object[]
					{
						inputVolume,
						audioMode.ToStringCached<AudioModeType>(),
						vadthreshold.Automatic(),
						vadthreshold.VadThreshold()
					}));
				}
				this.UpdateMembers(true);
				DiscordManager.CallChangedCallback callChanged = this.ownerManager.CallChanged;
				if (callChanged != null)
				{
					callChanged(this);
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("_status", _status, null);
			}
			DiscordManager.CallStatusChangedCallback callStatusChanged = this.ownerManager.CallStatusChanged;
			if (callStatusChanged == null)
			{
				return;
			}
			callStatusChanged(this, this.Status);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0007ACB8 File Offset: 0x00078EB8
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnSpeakingStatusChanged(ulong _userId, bool _isPlayingSound)
		{
			DiscordManager.DiscordUser localUser = this.ownerManager.LocalUser;
			ulong? num = (localUser != null) ? new ulong?(localUser.ID) : null;
			bool flag = _userId == num.GetValueOrDefault() & num != null;
			DiscordManager.logCallbackInfo(string.Format("Call: Speaking state changed: user={0}({1}) isPlaying={2}", _userId, flag ? "SELF" : "other", _isPlayingSound), LogType.Log);
			if (flag)
			{
				this.IsSpeaking = _isPlayingSound;
			}
			DiscordManager.CallInfo.MemberState value;
			if (!this.callMembersCurrent.TryGetValue(_userId, out value))
			{
				Log.Warning(string.Format("[Discord] Speaking status changed for user not in call member list ({0})", _userId));
				return;
			}
			value.Speaking = _isPlayingSound;
			this.callMembersCurrent[_userId] = value;
			DiscordManager.VoiceStateChangedCallback voiceStateChanged = this.ownerManager.VoiceStateChanged;
			if (voiceStateChanged == null)
			{
				return;
			}
			voiceStateChanged(flag, _userId);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x0007AD84 File Offset: 0x00078F84
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnParticipantChanged(ulong _userId, bool _added)
		{
			DiscordManager.DiscordUser localUser = this.ownerManager.LocalUser;
			ulong? num = (localUser != null) ? new ulong?(localUser.ID) : null;
			bool flag = _userId == num.GetValueOrDefault() & num != null;
			DiscordManager.logCallbackInfo(string.Format("Call: Participant changed: user={0}({1}) added={2}", _userId, flag ? "SELF" : "other", _added), LogType.Log);
			this.UpdateMembers(false);
			if (_added)
			{
				DiscordManager.DiscordUser user = this.ownerManager.GetUser(_userId);
				int entityId;
				if (!this.ownerManager.userMappings.TryGetEntityId(_userId, out entityId))
				{
					return;
				}
				PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
				PersistentPlayerData persistentPlayerData = (persistentPlayers != null) ? persistentPlayers.GetPlayerDataFromEntityID(entityId) : null;
				if (persistentPlayerData == null)
				{
					return;
				}
				if (!flag)
				{
					IPlatformUserData orCreate = PlatformUserManager.GetOrCreate(persistentPlayerData.PrimaryId);
					this.UpdateBlockState(user, orCreate.Blocked[EBlockType.VoiceChat].IsBlocked());
					user.Volume = this.ownerManager.UserSettings.GetUserVolume(user.ID);
				}
			}
			if (!flag)
			{
				Manager.PlayXUiSound(_added ? DiscordManager.CallInfo.soundOtherJoin : DiscordManager.CallInfo.soundOtherLeave, 1f);
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0007AEA8 File Offset: 0x000790A8
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnVoiceStateChanged(ulong _userId)
		{
			DiscordManager.DiscordUser localUser = this.ownerManager.LocalUser;
			ulong? num = (localUser != null) ? new ulong?(localUser.ID) : null;
			bool flag = _userId == num.GetValueOrDefault() & num != null;
			DiscordManager.logCallbackInfo(string.Format("Call: Voice state changed: user={0}({1})", _userId, flag ? "SELF" : "other"), LogType.Log);
			DiscordManager.CallInfo.MemberState value;
			if (!this.callMembersCurrent.TryGetValue(_userId, out value))
			{
				Log.Warning(string.Format("[Discord] VoiceState changed for user not in call member list ({0})", _userId));
				return;
			}
			using (VoiceStateHandle voiceStateHandle = this.call.GetVoiceStateHandle(_userId))
			{
				if (voiceStateHandle == null)
				{
					Log.Warning(string.Format("[Discord] VoiceState changed for user {0} but can not get current state", _userId));
				}
				else
				{
					value.Muted = voiceStateHandle.SelfMute();
					value.Deafened = voiceStateHandle.SelfDeaf();
					this.callMembersCurrent[_userId] = value;
					DiscordManager.VoiceStateChangedCallback voiceStateChanged = this.ownerManager.VoiceStateChanged;
					if (voiceStateChanged != null)
					{
						voiceStateChanged(flag, _userId);
					}
				}
			}
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0007AFBC File Offset: 0x000791BC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnVadModeChanged(bool _auto)
		{
			if (!this.IsJoined)
			{
				return;
			}
			VADThresholdSettings vadthreshold = this.call.GetVADThreshold();
			this.call.SetVADThreshold(_auto, vadthreshold.VadThreshold());
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0007AFF0 File Offset: 0x000791F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnVadThresholdChanged(int _threshold)
		{
			if (!this.IsJoined)
			{
				return;
			}
			VADThresholdSettings vadthreshold = this.call.GetVADThreshold();
			this.call.SetVADThreshold(vadthreshold.Automatic(), (float)_threshold);
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0007B028 File Offset: 0x00079228
		public void UpdateBlockState(DiscordManager.DiscordUser _user, bool _isBlocked)
		{
			DiscordManager.CallInfo.MemberState memberState;
			if (!this.callMembersCurrent.TryGetValue(_user.ID, out memberState))
			{
				return;
			}
			_user.LocalMuted = _isBlocked;
		}

		// Token: 0x04000D0B RID: 3339
		[PublicizedFrom(EAccessModifier.Private)]
		public static AudioClip soundOtherJoin;

		// Token: 0x04000D0C RID: 3340
		[PublicizedFrom(EAccessModifier.Private)]
		public static AudioClip soundOtherLeave;

		// Token: 0x04000D0D RID: 3341
		[PublicizedFrom(EAccessModifier.Private)]
		public static AudioClip soundSelfNoManualLeave;

		// Token: 0x04000D0E RID: 3342
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager.LobbyInfo ownerLobby;

		// Token: 0x04000D0F RID: 3343
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager ownerManager;

		// Token: 0x04000D10 RID: 3344
		[PublicizedFrom(EAccessModifier.Private)]
		public Call call;

		// Token: 0x04000D11 RID: 3345
		[PublicizedFrom(EAccessModifier.Private)]
		public bool startedJoining;

		// Token: 0x04000D12 RID: 3346
		[PublicizedFrom(EAccessModifier.Private)]
		public bool manualLeave;

		// Token: 0x04000D15 RID: 3349
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<ulong, DiscordManager.CallInfo.MemberState> callMembersCurrent = new Dictionary<ulong, DiscordManager.CallInfo.MemberState>();

		// Token: 0x04000D16 RID: 3350
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<ulong, DiscordManager.CallInfo.MemberState> callMembersOld = new Dictionary<ulong, DiscordManager.CallInfo.MemberState>();

		// Token: 0x020002C0 RID: 704
		public struct MemberState
		{
			// Token: 0x04000D17 RID: 3351
			public bool Speaking;

			// Token: 0x04000D18 RID: 3352
			public bool Muted;

			// Token: 0x04000D19 RID: 3353
			public bool Deafened;
		}
	}

	// Token: 0x020002C2 RID: 706
	public enum EDiscordStatus
	{
		// Token: 0x04000D20 RID: 3360
		NotInitialized,
		// Token: 0x04000D21 RID: 3361
		Disconnected,
		// Token: 0x04000D22 RID: 3362
		Ready,
		// Token: 0x04000D23 RID: 3363
		Connecting,
		// Token: 0x04000D24 RID: 3364
		Disconnecting
	}

	// Token: 0x020002C3 RID: 707
	public enum ELobbyType : byte
	{
		// Token: 0x04000D26 RID: 3366
		Global,
		// Token: 0x04000D27 RID: 3367
		Party
	}

	// Token: 0x020002C4 RID: 708
	public enum EDiscordAccountType
	{
		// Token: 0x04000D29 RID: 3369
		None,
		// Token: 0x04000D2A RID: 3370
		Regular,
		// Token: 0x04000D2B RID: 3371
		Provisional
	}

	// Token: 0x020002C5 RID: 709
	public enum EFullAccountLoginResult
	{
		// Token: 0x04000D2D RID: 3373
		None,
		// Token: 0x04000D2E RID: 3374
		Success,
		// Token: 0x04000D2F RID: 3375
		RequestingAuth,
		// Token: 0x04000D30 RID: 3376
		AuthAccepted,
		// Token: 0x04000D31 RID: 3377
		AuthCancelled,
		// Token: 0x04000D32 RID: 3378
		AuthFailed,
		// Token: 0x04000D33 RID: 3379
		TokenExchangeFailed,
		// Token: 0x04000D34 RID: 3380
		TokenRefreshFailed,
		// Token: 0x04000D35 RID: 3381
		TokenRevoked,
		// Token: 0x04000D36 RID: 3382
		ConnectionFailed,
		// Token: 0x04000D37 RID: 3383
		PlatformError
	}

	// Token: 0x020002C6 RID: 710
	public enum EProvisionalAccountLoginResult
	{
		// Token: 0x04000D39 RID: 3385
		None,
		// Token: 0x04000D3A RID: 3386
		Success,
		// Token: 0x04000D3B RID: 3387
		NotSupported,
		// Token: 0x04000D3C RID: 3388
		PlatformIdLinkedToDiscordAccount,
		// Token: 0x04000D3D RID: 3389
		TokenExchangeFailed,
		// Token: 0x04000D3E RID: 3390
		ConnectionFailed,
		// Token: 0x04000D3F RID: 3391
		PlatformError
	}

	// Token: 0x020002C7 RID: 711
	public enum EAutoJoinVoiceMode
	{
		// Token: 0x04000D41 RID: 3393
		None,
		// Token: 0x04000D42 RID: 3394
		Global,
		// Token: 0x04000D43 RID: 3395
		Party
	}

	// Token: 0x020002C8 RID: 712
	// (Invoke) Token: 0x0600143F RID: 5183
	public delegate void UserAuthorizationResultCallback(bool _isDone, DiscordManager.EFullAccountLoginResult _fullAccResult, DiscordManager.EProvisionalAccountLoginResult _provisionalAccResult, bool _isExpectedSuccess);

	// Token: 0x020002C9 RID: 713
	// (Invoke) Token: 0x06001443 RID: 5187
	public delegate void LocalUserChangedCallback(bool _loggedIn);

	// Token: 0x020002CA RID: 714
	// (Invoke) Token: 0x06001447 RID: 5191
	public delegate void LobbyStateChangedCallback(DiscordManager.LobbyInfo _lobby, bool _isReady, bool _isJoined);

	// Token: 0x020002CB RID: 715
	// (Invoke) Token: 0x0600144B RID: 5195
	public delegate void LobbyMembersChangedCallback(DiscordManager.LobbyInfo _lobby);

	// Token: 0x020002CC RID: 716
	// (Invoke) Token: 0x0600144F RID: 5199
	public delegate void CallChangedCallback(DiscordManager.CallInfo _newCall);

	// Token: 0x020002CD RID: 717
	// (Invoke) Token: 0x06001453 RID: 5203
	public delegate void CallStatusChangedCallback(DiscordManager.CallInfo _call, Call.Status _callStatus);

	// Token: 0x020002CE RID: 718
	// (Invoke) Token: 0x06001457 RID: 5207
	public delegate void CallMembersChangedCallback(DiscordManager.CallInfo _call);

	// Token: 0x020002CF RID: 719
	// (Invoke) Token: 0x0600145B RID: 5211
	public delegate void VoiceStateChangedCallback(bool _self, ulong _userId);

	// Token: 0x020002D0 RID: 720
	// (Invoke) Token: 0x0600145F RID: 5215
	public delegate void SelfMuteStateChangedCallback(bool _selfMute, bool _selfDeaf);

	// Token: 0x020002D1 RID: 721
	// (Invoke) Token: 0x06001463 RID: 5219
	public delegate void FriendsListChangedCallback();

	// Token: 0x020002D2 RID: 722
	// (Invoke) Token: 0x06001467 RID: 5223
	public delegate void RelationshipChangedCallback(DiscordManager.DiscordUser _user);

	// Token: 0x020002D3 RID: 723
	// (Invoke) Token: 0x0600146B RID: 5227
	public delegate void ActivityInviteReceivedCallback(DiscordManager.DiscordUser _user, bool _cleared, ActivityActionTypes _type);

	// Token: 0x020002D4 RID: 724
	// (Invoke) Token: 0x0600146F RID: 5231
	public delegate void ActivityJoiningCallback();

	// Token: 0x020002D5 RID: 725
	// (Invoke) Token: 0x06001473 RID: 5235
	public delegate void PendingActionsUpdateCallback(int _pendingActionsCount);

	// Token: 0x020002D6 RID: 726
	// (Invoke) Token: 0x06001477 RID: 5239
	public delegate void AudioDevicesChangedCallback(DiscordManager.AudioDeviceConfig _inOutConfig);

	// Token: 0x020002D7 RID: 727
	[PublicizedFrom(EAccessModifier.Private)]
	public enum LobbyMemberActionType
	{
		// Token: 0x04000D45 RID: 3397
		Add,
		// Token: 0x04000D46 RID: 3398
		Remove,
		// Token: 0x04000D47 RID: 3399
		Update
	}

	// Token: 0x020002D8 RID: 728
	public class AudioDeviceConfig
	{
		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600147A RID: 5242 RVA: 0x0007B076 File Offset: 0x00079276
		// (set) Token: 0x0600147B RID: 5243 RVA: 0x0007B07E File Offset: 0x0007927E
		public Dictionary<string, DiscordManager.DiscordAudioDevice> CurrentAudioDevices { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new Dictionary<string, DiscordManager.DiscordAudioDevice>();

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x0600147C RID: 5244 RVA: 0x0007B087 File Offset: 0x00079287
		// (set) Token: 0x0600147D RID: 5245 RVA: 0x0007B08F File Offset: 0x0007928F
		public string ActiveAudioDevice { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x0600147E RID: 5246 RVA: 0x0007B098 File Offset: 0x00079298
		public string ConfigAudioDevice
		{
			get
			{
				if (!this.IsOutput)
				{
					return this.owner.Settings.SelectedInputDevice;
				}
				return this.owner.Settings.SelectedOutputDevice;
			}
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0007B0C3 File Offset: 0x000792C3
		public AudioDeviceConfig(DiscordManager _owner, bool _isOutput)
		{
			this.owner = _owner;
			this.IsOutput = _isOutput;
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0007B0EF File Offset: 0x000792EF
		public void UpdateAudioDeviceList()
		{
			if (this.owner.client == null)
			{
				this.CurrentAudioDevices.Clear();
				this.owner.fireAudioDevicesChanged(this);
				return;
			}
			this.getDevices();
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0007B11C File Offset: 0x0007931C
		[PublicizedFrom(EAccessModifier.Private)]
		public void getDevices()
		{
			if (this.IsOutput)
			{
				this.owner.client.GetOutputDevices(new Discord.Sdk.Client.GetOutputDevicesCallback(this.ApplyDevicesFound));
				return;
			}
			this.owner.client.GetInputDevices(new Discord.Sdk.Client.GetInputDevicesCallback(this.ApplyDevicesFound));
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0007B16C File Offset: 0x0007936C
		[PublicizedFrom(EAccessModifier.Private)]
		public void swapAndClearAudioDeviceList()
		{
			Dictionary<string, DiscordManager.DiscordAudioDevice> currentAudioDevices = this.CurrentAudioDevices;
			Dictionary<string, DiscordManager.DiscordAudioDevice> currentAudioDevices2 = this.oldAudioDevices;
			this.oldAudioDevices = currentAudioDevices;
			this.CurrentAudioDevices = currentAudioDevices2;
			this.CurrentAudioDevices.Clear();
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x0007B1A4 File Offset: 0x000793A4
		public void ApplyDevicesFound(AudioDevice[] _devices)
		{
			this.swapAndClearAudioDeviceList();
			for (int i = 0; i < _devices.Length; i++)
			{
				DiscordManager.DiscordAudioDevice discordAudioDevice = new DiscordManager.DiscordAudioDevice(_devices[i], this.IsOutput);
				this.CurrentAudioDevices[discordAudioDevice.Identifier] = discordAudioDevice;
			}
			if (this.IsOutput)
			{
				this.owner.client.GetCurrentOutputDevice(new Discord.Sdk.Client.GetCurrentOutputDeviceCallback(this.getCurrentDeviceCallbackFn));
				return;
			}
			this.owner.client.GetCurrentInputDevice(new Discord.Sdk.Client.GetCurrentInputDeviceCallback(this.getCurrentDeviceCallbackFn));
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x0007B22C File Offset: 0x0007942C
		[PublicizedFrom(EAccessModifier.Private)]
		public void getCurrentDeviceCallbackFn(AudioDevice _device)
		{
			bool flag = this.CurrentAudioDevices.Count == this.oldAudioDevices.Count && this.CurrentAudioDevices.Keys.All(new Func<string, bool>(this.oldAudioDevices.ContainsKey));
			this.oldAudioDevices.Clear();
			string text = _device.Id();
			bool flag2 = text == this.ActiveAudioDevice;
			this.ActiveAudioDevice = text;
			Log.Out(string.Format("[Discord] Current {0} device: {1} // {2} // {3}", new object[]
			{
				this.IsOutput ? "output" : "input",
				_device.Id(),
				_device.Name(),
				_device.IsDefault()
			}));
			DiscordManager.DiscordAudioDevice discordAudioDevice;
			if (this.CurrentAudioDevices.TryGetValue(this.ConfigAudioDevice, out discordAudioDevice) && this.ConfigAudioDevice != text)
			{
				Log.Out(string.Format("[Discord] Setting {0} device from config: {1} // {2} // {3}", new object[]
				{
					this.IsOutput ? "output" : "input",
					discordAudioDevice.Identifier,
					discordAudioDevice,
					discordAudioDevice.IsDefault
				}));
				if (this.IsOutput)
				{
					this.owner.client.SetOutputDevice(this.ConfigAudioDevice, delegate(ClientResult _result)
					{
						DiscordManager.logCallbackInfoWithClientResult("SetOutputDevice", null, _result, true);
					});
				}
				else
				{
					this.owner.client.SetInputDevice(this.ConfigAudioDevice, delegate(ClientResult _result)
					{
						DiscordManager.logCallbackInfoWithClientResult("SetInputDevice", null, _result, true);
					});
				}
				this.ActiveAudioDevice = this.ConfigAudioDevice;
			}
			if (!flag || !flag2)
			{
				this.owner.fireAudioDevicesChanged(this);
			}
		}

		// Token: 0x04000D48 RID: 3400
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager owner;

		// Token: 0x04000D49 RID: 3401
		public readonly bool IsOutput;

		// Token: 0x04000D4B RID: 3403
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, DiscordManager.DiscordAudioDevice> oldAudioDevices = new Dictionary<string, DiscordManager.DiscordAudioDevice>();
	}

	// Token: 0x020002DA RID: 730
	public class DiscordAudioDevice : IPartyVoice.VoiceAudioDevice
	{
		// Token: 0x06001489 RID: 5257 RVA: 0x0007B415 File Offset: 0x00079615
		public DiscordAudioDevice(AudioDevice _device, bool _isOutput) : base(_isOutput, _device.IsDefault())
		{
			this.id = _device.Id();
			this.name = _device.Name();
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0007B43C File Offset: 0x0007963C
		public override string ToString()
		{
			if (!this.IsDefault)
			{
				return this.name;
			}
			return "(Default) " + this.name;
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x0600148B RID: 5259 RVA: 0x0007B45D File Offset: 0x0007965D
		public override string Identifier
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x04000D50 RID: 3408
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string id;

		// Token: 0x04000D51 RID: 3409
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string name;
	}

	// Token: 0x020002DB RID: 731
	public class DiscordSettings
	{
		// Token: 0x14000015 RID: 21
		// (add) Token: 0x0600148C RID: 5260 RVA: 0x0007B468 File Offset: 0x00079668
		// (remove) Token: 0x0600148D RID: 5261 RVA: 0x0007B4A0 File Offset: 0x000796A0
		public event Action<string> OutputDeviceChanged;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x0600148E RID: 5262 RVA: 0x0007B4D8 File Offset: 0x000796D8
		// (remove) Token: 0x0600148F RID: 5263 RVA: 0x0007B510 File Offset: 0x00079710
		public event Action<string> InputDeviceChanged;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06001490 RID: 5264 RVA: 0x0007B548 File Offset: 0x00079748
		// (remove) Token: 0x06001491 RID: 5265 RVA: 0x0007B580 File Offset: 0x00079780
		public event Action<int> OutputVolumeChanged;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001492 RID: 5266 RVA: 0x0007B5B8 File Offset: 0x000797B8
		// (remove) Token: 0x06001493 RID: 5267 RVA: 0x0007B5F0 File Offset: 0x000797F0
		public event Action<int> InputVolumeChanged;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001494 RID: 5268 RVA: 0x0007B628 File Offset: 0x00079828
		// (remove) Token: 0x06001495 RID: 5269 RVA: 0x0007B660 File Offset: 0x00079860
		public event Action<bool> VoiceModePttChanged;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001496 RID: 5270 RVA: 0x0007B698 File Offset: 0x00079898
		// (remove) Token: 0x06001497 RID: 5271 RVA: 0x0007B6D0 File Offset: 0x000798D0
		public event Action<bool> VoiceVadModeChanged;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06001498 RID: 5272 RVA: 0x0007B708 File Offset: 0x00079908
		// (remove) Token: 0x06001499 RID: 5273 RVA: 0x0007B740 File Offset: 0x00079940
		public event Action<int> VoiceVadThresholdChanged;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x0600149A RID: 5274 RVA: 0x0007B778 File Offset: 0x00079978
		// (remove) Token: 0x0600149B RID: 5275 RVA: 0x0007B7B0 File Offset: 0x000799B0
		public event Action<bool> DmPrivacyModeChanged;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x0600149C RID: 5276 RVA: 0x0007B7E8 File Offset: 0x000799E8
		// (remove) Token: 0x0600149D RID: 5277 RVA: 0x0007B820 File Offset: 0x00079A20
		public event Action<DiscordManager.EAutoJoinVoiceMode> AutoJoinVoiceModeChanged;

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x0007B855 File Offset: 0x00079A55
		// (set) Token: 0x0600149F RID: 5279 RVA: 0x0007B85D File Offset: 0x00079A5D
		public string SelectedOutputDevice
		{
			get
			{
				return this.selectedOutputDevice;
			}
			set
			{
				if (this.selectedOutputDevice == value)
				{
					return;
				}
				this.selectedOutputDevice = value;
				Action<string> outputDeviceChanged = this.OutputDeviceChanged;
				if (outputDeviceChanged == null)
				{
					return;
				}
				outputDeviceChanged(value);
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x0007B886 File Offset: 0x00079A86
		// (set) Token: 0x060014A1 RID: 5281 RVA: 0x0007B88E File Offset: 0x00079A8E
		public string SelectedInputDevice
		{
			get
			{
				return this.selectedInputDevice;
			}
			set
			{
				if (this.selectedInputDevice == value)
				{
					return;
				}
				this.selectedInputDevice = value;
				Action<string> inputDeviceChanged = this.InputDeviceChanged;
				if (inputDeviceChanged == null)
				{
					return;
				}
				inputDeviceChanged(value);
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060014A2 RID: 5282 RVA: 0x0007B8B7 File Offset: 0x00079AB7
		// (set) Token: 0x060014A3 RID: 5283 RVA: 0x0007B8BF File Offset: 0x00079ABF
		public int OutputVolume
		{
			get
			{
				return this.outputVolume;
			}
			set
			{
				if (this.outputVolume == value)
				{
					return;
				}
				this.outputVolume = Mathf.Clamp(value, 0, 200);
				Action<int> outputVolumeChanged = this.OutputVolumeChanged;
				if (outputVolumeChanged == null)
				{
					return;
				}
				outputVolumeChanged(value);
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x0007B8EE File Offset: 0x00079AEE
		// (set) Token: 0x060014A5 RID: 5285 RVA: 0x0007B8F6 File Offset: 0x00079AF6
		public int InputVolume
		{
			get
			{
				return this.inputVolume;
			}
			set
			{
				if (this.inputVolume == value)
				{
					return;
				}
				this.inputVolume = Mathf.Clamp(value, 0, 200);
				Action<int> inputVolumeChanged = this.InputVolumeChanged;
				if (inputVolumeChanged == null)
				{
					return;
				}
				inputVolumeChanged(value);
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060014A6 RID: 5286 RVA: 0x0007B925 File Offset: 0x00079B25
		// (set) Token: 0x060014A7 RID: 5287 RVA: 0x0007B92D File Offset: 0x00079B2D
		public bool VoiceModePtt
		{
			get
			{
				return this.voiceModePtt;
			}
			set
			{
				if (this.voiceModePtt == value)
				{
					return;
				}
				this.voiceModePtt = value;
				Action<bool> voiceModePttChanged = this.VoiceModePttChanged;
				if (voiceModePttChanged == null)
				{
					return;
				}
				voiceModePttChanged(value);
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x0007B951 File Offset: 0x00079B51
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x0007B959 File Offset: 0x00079B59
		public bool VoiceVadModeAuto
		{
			get
			{
				return this.voiceVadModeAuto;
			}
			set
			{
				if (this.voiceVadModeAuto == value)
				{
					return;
				}
				this.voiceVadModeAuto = value;
				Action<bool> voiceVadModeChanged = this.VoiceVadModeChanged;
				if (voiceVadModeChanged == null)
				{
					return;
				}
				voiceVadModeChanged(value);
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x0007B97D File Offset: 0x00079B7D
		// (set) Token: 0x060014AB RID: 5291 RVA: 0x0007B985 File Offset: 0x00079B85
		public int VoiceVadThreshold
		{
			get
			{
				return this.voiceVadThreshold;
			}
			set
			{
				if (this.voiceVadThreshold == value)
				{
					return;
				}
				this.voiceVadThreshold = Mathf.Clamp(value, -100, 0);
				Action<int> voiceVadThresholdChanged = this.VoiceVadThresholdChanged;
				if (voiceVadThresholdChanged == null)
				{
					return;
				}
				voiceVadThresholdChanged(value);
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x0007B9B1 File Offset: 0x00079BB1
		// (set) Token: 0x060014AD RID: 5293 RVA: 0x0007B9B9 File Offset: 0x00079BB9
		public bool DmPrivacyMode
		{
			get
			{
				return this.dmPrivacyMode;
			}
			set
			{
				if (this.dmPrivacyMode == value)
				{
					return;
				}
				this.dmPrivacyMode = value;
				Action<bool> dmPrivacyModeChanged = this.DmPrivacyModeChanged;
				if (dmPrivacyModeChanged == null)
				{
					return;
				}
				dmPrivacyModeChanged(value);
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x0007B9DD File Offset: 0x00079BDD
		// (set) Token: 0x060014AF RID: 5295 RVA: 0x0007B9E5 File Offset: 0x00079BE5
		public DiscordManager.EAutoJoinVoiceMode AutoJoinVoiceMode
		{
			get
			{
				return this.autoJoinVoiceMode;
			}
			set
			{
				if (this.autoJoinVoiceMode == value)
				{
					return;
				}
				this.autoJoinVoiceMode = value;
				Action<DiscordManager.EAutoJoinVoiceMode> autoJoinVoiceModeChanged = this.AutoJoinVoiceModeChanged;
				if (autoJoinVoiceModeChanged == null)
				{
					return;
				}
				autoJoinVoiceModeChanged(value);
			}
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x0007BA0C File Offset: 0x00079C0C
		public void ResetToDefaults()
		{
			this.SelectedOutputDevice = "default";
			this.SelectedInputDevice = "default";
			this.OutputVolume = 100;
			this.InputVolume = 100;
			this.VoiceModePtt = false;
			this.VoiceVadModeAuto = true;
			this.VoiceVadThreshold = -60;
			this.DmPrivacyMode = true;
			this.AutoJoinVoiceMode = DiscordManager.EAutoJoinVoiceMode.None;
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x0007BA64 File Offset: 0x00079C64
		public static DiscordManager.DiscordSettings Load()
		{
			if (!SdPlayerPrefs.HasKey("DiscordSettings"))
			{
				return new DiscordManager.DiscordSettings();
			}
			DiscordManager.DiscordSettings result;
			try
			{
				DiscordManager.DiscordSettings discordSettings = JsonConvert.DeserializeObject<DiscordManager.DiscordSettings>(SdPlayerPrefs.GetString("DiscordSettings"));
				Log.Out(string.Format("[Discord] Loaded settings with DiscordDisabled={0}", discordSettings.DiscordDisabled));
				result = discordSettings;
			}
			catch (JsonException e)
			{
				Log.Error("[Discord] Failed loading settings:");
				Log.Exception(e);
				DiscordManager.DiscordSettings discordSettings2 = new DiscordManager.DiscordSettings();
				discordSettings2.Save();
				result = discordSettings2;
			}
			return result;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x0007BAE0 File Offset: 0x00079CE0
		public void Save()
		{
			Log.Out(string.Format("[Discord] Saving settings with DiscordDisabled={0}", this.DiscordDisabled));
			SdPlayerPrefs.SetString("DiscordSettings", JsonConvert.SerializeObject(this));
			SdPlayerPrefs.Save();
		}

		// Token: 0x04000D5B RID: 3419
		public bool DiscordFirstTimeInfoShown;

		// Token: 0x04000D5C RID: 3420
		public bool DiscordDisabled;

		// Token: 0x04000D5D RID: 3421
		public DiscordManager.EDiscordAccountType LastAccountType;

		// Token: 0x04000D5E RID: 3422
		public string AccessToken;

		// Token: 0x04000D5F RID: 3423
		public string RefreshToken;

		// Token: 0x04000D60 RID: 3424
		[PublicizedFrom(EAccessModifier.Private)]
		public string selectedOutputDevice = "default";

		// Token: 0x04000D61 RID: 3425
		[PublicizedFrom(EAccessModifier.Private)]
		public string selectedInputDevice = "default";

		// Token: 0x04000D62 RID: 3426
		[PublicizedFrom(EAccessModifier.Private)]
		public int outputVolume = 100;

		// Token: 0x04000D63 RID: 3427
		[PublicizedFrom(EAccessModifier.Private)]
		public int inputVolume = 100;

		// Token: 0x04000D64 RID: 3428
		[PublicizedFrom(EAccessModifier.Private)]
		public bool voiceModePtt;

		// Token: 0x04000D65 RID: 3429
		[PublicizedFrom(EAccessModifier.Private)]
		public bool voiceVadModeAuto = true;

		// Token: 0x04000D66 RID: 3430
		[PublicizedFrom(EAccessModifier.Private)]
		public int voiceVadThreshold = -60;

		// Token: 0x04000D67 RID: 3431
		[PublicizedFrom(EAccessModifier.Private)]
		public bool dmPrivacyMode = true;

		// Token: 0x04000D68 RID: 3432
		[PublicizedFrom(EAccessModifier.Private)]
		public DiscordManager.EAutoJoinVoiceMode autoJoinVoiceMode;

		// Token: 0x04000D69 RID: 3433
		public const string DiscordSettingsPlayerPrefName = "DiscordSettings";
	}

	// Token: 0x020002DC RID: 732
	public class DiscordUser : IDisposable, IEquatable<DiscordManager.DiscordUser>
	{
		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x0007BB63 File Offset: 0x00079D63
		// (set) Token: 0x060014B5 RID: 5301 RVA: 0x0007BB6B File Offset: 0x00079D6B
		public bool IsProvisionalAccount { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x0007BB74 File Offset: 0x00079D74
		public StatusType DiscordState
		{
			get
			{
				UserHandle userHandle = this.userHandle;
				if (userHandle == null)
				{
					return StatusType.Offline;
				}
				return userHandle.Status();
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060014B7 RID: 5303 RVA: 0x0007BB87 File Offset: 0x00079D87
		public string DiscordStateLocalized
		{
			get
			{
				string str = "discordState";
				UserHandle userHandle = this.userHandle;
				return Localization.Get(str + ((userHandle != null) ? userHandle.Status() : StatusType.Offline).ToStringCached<StatusType>(), false);
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x0007BBB0 File Offset: 0x00079DB0
		public string DisplayName
		{
			get
			{
				return this.playerName ?? this.DiscordDisplayName;
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060014B9 RID: 5305 RVA: 0x0007BBC2 File Offset: 0x00079DC2
		public string DiscordDisplayName
		{
			get
			{
				return this.discordDisplayName ?? "<unknown>";
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x0007BBD3 File Offset: 0x00079DD3
		public string DiscordUserName
		{
			get
			{
				UserHandle userHandle = this.userHandle;
				return ((userHandle != null) ? userHandle.Username() : null) ?? "<unknown>";
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060014BB RID: 5307 RVA: 0x0007BBF0 File Offset: 0x00079DF0
		public string PlayerName
		{
			get
			{
				return this.playerName ?? "<unknown>";
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x0007BC01 File Offset: 0x00079E01
		public Texture2D Avatar
		{
			get
			{
				return this.avatar;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060014BD RID: 5309 RVA: 0x0007BC09 File Offset: 0x00079E09
		public bool InGlobalLobby
		{
			get
			{
				return this.ownerManager.globalLobby.HasMember(this.ID);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x0007BC21 File Offset: 0x00079E21
		public bool InPartyLobby
		{
			get
			{
				return this.ownerManager.partyLobby.HasMember(this.ID);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060014BF RID: 5311 RVA: 0x0007BC39 File Offset: 0x00079E39
		public bool PendingAction
		{
			get
			{
				return this.PendingIncomingJoinRequest || this.PendingIncomingInvite || this.PendingFriendRequest;
			}
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x0007BC53 File Offset: 0x00079E53
		public DiscordUser(DiscordManager _ownerManager, ulong _id, bool _isLocalAccount = false)
		{
			this.ownerManager = _ownerManager;
			this.ID = _id;
			this.IsLocalAccount = _isLocalAccount;
			this.TryUpdateDiscordHandle();
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0007BC78 File Offset: 0x00079E78
		public void TryUpdateDiscordHandle()
		{
			this.userHandle = ((!this.ownerManager.IsReady) ? null : this.ownerManager.client.GetUser(this.ID));
			if (this.userHandle != null)
			{
				this.IsProvisionalAccount = this.userHandle.IsProvisional();
				this.updateDisplayName();
			}
			this.UpdateRelationship();
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x0007BCD8 File Offset: 0x00079ED8
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateDisplayName()
		{
			string text = this.userHandle.DisplayName();
			foreach (char c in text)
			{
				if (c >= '\ud800' && c < '')
				{
					this.discordDisplayName = this.userHandle.Username();
					return;
				}
			}
			this.discordDisplayName = text;
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x0007BD35 File Offset: 0x00079F35
		public void RequestAvatar()
		{
			if (!this.avatarStartedDownload && this.userHandle != null)
			{
				ThreadManager.StartCoroutine(this.downloadDiscordAvatar());
			}
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x0007BD53 File Offset: 0x00079F53
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator downloadDiscordAvatar()
		{
			this.avatarStartedDownload = true;
			string text = null;
			try
			{
				text = this.userHandle.AvatarUrl(UserHandle.AvatarType.Png, UserHandle.AvatarType.Png);
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			if (string.IsNullOrEmpty(text))
			{
				yield break;
			}
			MicroStopwatch mswDownload = new MicroStopwatch(true);
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(text);
			www.SendWebRequest();
			while (!www.isDone)
			{
				yield return null;
			}
			if (www.result == UnityWebRequest.Result.Success)
			{
				Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
				this.avatar = TextureUtils.CloneTexture(texture, false, false, true);
				UnityEngine.Object.DestroyImmediate(texture);
				DiscordManager.FriendsListChangedCallback friendsListChanged = this.ownerManager.FriendsListChanged;
				if (friendsListChanged != null)
				{
					friendsListChanged();
				}
				if (DiscordManager.logLevel == LoggingSeverity.Verbose)
				{
					Log.Out(string.Format("[Discord] Downloading avatar for user {0} took {1} ms. Size: {2} B, resolution: {3} x {4}", new object[]
					{
						this.DiscordDisplayName,
						mswDownload.ElapsedMilliseconds,
						www.downloadedBytes,
						this.avatar.width,
						this.avatar.height
					}));
				}
			}
			else if (DiscordManager.logLevel <= LoggingSeverity.Warning)
			{
				Log.Warning("[Discord] Retrieving avatar for user " + this.DiscordDisplayName + " failed: " + www.error);
			}
			yield break;
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x0007BD64 File Offset: 0x00079F64
		public void UpdatePlayerName(EntityPlayer _entity = null)
		{
			if (_entity != null)
			{
				this.playerName = _entity.PlayerDisplayName;
				return;
			}
			int key;
			if (!this.ownerManager.userMappings.TryGetEntityId(this.ID, out key))
			{
				return;
			}
			if (GameManager.Instance.World == null || !GameManager.Instance.World.Players.dict.TryGetValue(key, out _entity))
			{
				return;
			}
			this.playerName = _entity.PlayerDisplayName;
			DiscordManager.FriendsListChangedCallback friendsListChanged = this.ownerManager.FriendsListChanged;
			if (friendsListChanged == null)
			{
				return;
			}
			friendsListChanged();
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060014C6 RID: 5318 RVA: 0x0007BDF0 File Offset: 0x00079FF0
		public bool InCurrentVoice
		{
			get
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				DiscordManager.CallInfo.MemberState memberState;
				return activeVoiceLobby != null && activeVoiceLobby.VoiceCall.TryGetMember(this.ID, out memberState);
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060014C7 RID: 5319 RVA: 0x0007BE20 File Offset: 0x0007A020
		// (set) Token: 0x060014C8 RID: 5320 RVA: 0x0007BE50 File Offset: 0x0007A050
		public double Volume
		{
			get
			{
				return (double)(this.InCurrentVoice ? (this.ownerManager.ActiveVoiceLobby.VoiceCall.GetParticipantVolume(this) / 100f) : 0f);
			}
			set
			{
				if (this.IsLocalAccount)
				{
					return;
				}
				if (!this.InCurrentVoice)
				{
					return;
				}
				this.ownerManager.ActiveVoiceLobby.VoiceCall.SetParticipantVolume(this, Mathf.Clamp((float)value * 100f, 0f, 200f));
				this.ownerManager.UserSettings.SetUserVolume(this.ID, value);
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060014C9 RID: 5321 RVA: 0x0007BEB4 File Offset: 0x0007A0B4
		public bool IsSpeaking
		{
			get
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				DiscordManager.CallInfo.MemberState memberState;
				return activeVoiceLobby != null && activeVoiceLobby.VoiceCall.TryGetMember(this.ID, out memberState) && memberState.Speaking;
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060014CA RID: 5322 RVA: 0x0007BEEF File Offset: 0x0007A0EF
		public bool IsMutedLocalOrRemote
		{
			get
			{
				return this.IsMuted || this.LocalMuted;
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060014CB RID: 5323 RVA: 0x0007BF04 File Offset: 0x0007A104
		// (set) Token: 0x060014CC RID: 5324 RVA: 0x0007BF5C File Offset: 0x0007A15C
		public bool LocalMuted
		{
			get
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				bool? flag;
				if (activeVoiceLobby == null)
				{
					flag = null;
				}
				else
				{
					Call call = activeVoiceLobby.VoiceCall.Call;
					flag = ((call != null) ? new bool?(call.GetLocalMute(this.ID)) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
			set
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				if (activeVoiceLobby == null)
				{
					return;
				}
				Call call = activeVoiceLobby.VoiceCall.Call;
				if (call == null)
				{
					return;
				}
				call.SetLocalMute(this.ID, value);
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060014CD RID: 5325 RVA: 0x0007BF8C File Offset: 0x0007A18C
		public bool IsMuted
		{
			get
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				DiscordManager.CallInfo.MemberState memberState;
				return activeVoiceLobby != null && activeVoiceLobby.VoiceCall.TryGetMember(this.ID, out memberState) && memberState.Muted;
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060014CE RID: 5326 RVA: 0x0007BFC8 File Offset: 0x0007A1C8
		public bool IsDeafened
		{
			get
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				DiscordManager.CallInfo.MemberState memberState;
				return activeVoiceLobby != null && activeVoiceLobby.VoiceCall.TryGetMember(this.ID, out memberState) && memberState.Deafened;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060014CF RID: 5327 RVA: 0x0007C004 File Offset: 0x0007A204
		public IPartyVoice.EVoiceMemberState VoiceState
		{
			get
			{
				DiscordManager.LobbyInfo activeVoiceLobby = this.ownerManager.ActiveVoiceLobby;
				DiscordManager.CallInfo.MemberState memberState;
				if (activeVoiceLobby == null || !activeVoiceLobby.VoiceCall.TryGetMember(this.ID, out memberState))
				{
					return IPartyVoice.EVoiceMemberState.Disabled;
				}
				if (this.LocalMuted || memberState.Muted)
				{
					return IPartyVoice.EVoiceMemberState.Muted;
				}
				if (memberState.Speaking)
				{
					return IPartyVoice.EVoiceMemberState.VoiceActive;
				}
				return IPartyVoice.EVoiceMemberState.Normal;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x0007C056 File Offset: 0x0007A256
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x0007C05E File Offset: 0x0007A25E
		public RelationshipType DiscordRelationship { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x0007C067 File Offset: 0x0007A267
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x0007C06F File Offset: 0x0007A26F
		public RelationshipType GameRelationship { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x060014D4 RID: 5332 RVA: 0x0007C078 File Offset: 0x0007A278
		public bool IsDiscordFriend
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.DiscordRelationship == RelationshipType.Friend;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x060014D5 RID: 5333 RVA: 0x0007C083 File Offset: 0x0007A283
		public bool IsGameFriend
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.GameRelationship == RelationshipType.Friend;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x060014D6 RID: 5334 RVA: 0x0007C08E File Offset: 0x0007A28E
		public bool IsFriend
		{
			get
			{
				return this.IsDiscordFriend || this.IsGameFriend;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x060014D7 RID: 5335 RVA: 0x0007C0A0 File Offset: 0x0007A2A0
		public bool IsBlocked
		{
			get
			{
				return this.DiscordRelationship == RelationshipType.Blocked;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060014D8 RID: 5336 RVA: 0x0007C0AB File Offset: 0x0007A2AB
		public bool PendingFriendRequest
		{
			get
			{
				return !this.isSpamRequest && (this.DiscordRelationship == RelationshipType.PendingIncoming || this.GameRelationship == RelationshipType.PendingIncoming);
			}
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x0007C0CC File Offset: 0x0007A2CC
		public void UpdateRelationship()
		{
			if (this.ownerManager.IsReady)
			{
				RelationshipHandle relationshipHandle = this.ownerManager.client.GetRelationshipHandle(this.ID);
				this.DiscordRelationship = relationshipHandle.DiscordRelationshipType();
				this.GameRelationship = relationshipHandle.GameRelationshipType();
				this.isSpamRequest = relationshipHandle.IsSpamRequest();
			}
			else
			{
				this.DiscordRelationship = RelationshipType.None;
				this.GameRelationship = RelationshipType.None;
				this.isSpamRequest = false;
			}
			if (this.IsFriend || this.IsBlocked || this.PendingFriendRequest)
			{
				this.RequestAvatar();
			}
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0007C158 File Offset: 0x0007A358
		public void SendFriendRequest(bool _gameFriend)
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			string arg = _gameFriend ? "game" : "Discord";
			switch (_gameFriend ? this.GameRelationship : this.DiscordRelationship)
			{
			case RelationshipType.Friend:
				Log.Out(string.Format("[Discord] Not sending {0} friend request (already friends) to {1}", arg, this));
				return;
			case RelationshipType.PendingIncoming:
				Log.Out(string.Format("[Discord] Accepting {0} friend request from {1}", arg, this));
				goto IL_9C;
			case RelationshipType.PendingOutgoing:
				Log.Out(string.Format("[Discord] Not sending {0} friend request (already sent) to {1}", arg, this));
				return;
			}
			Log.Out(string.Format("[Discord] Sending {0} friend request to {1}", arg, this));
			IL_9C:
			if (_gameFriend)
			{
				this.ownerManager.client.SendGameFriendRequestById(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<SendFriendRequest>g__SendRequestCallback|77_0));
				return;
			}
			this.ownerManager.client.SendDiscordFriendRequestById(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<SendFriendRequest>g__SendRequestCallback|77_0));
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0007C24C File Offset: 0x0007A44C
		public void DeclineFriendRequest(bool _gameFriend)
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			string arg = _gameFriend ? "game" : "Discord";
			if ((_gameFriend ? this.GameRelationship : this.DiscordRelationship) != RelationshipType.PendingIncoming)
			{
				Log.Out(string.Format("[Discord] Not rejecting {0} friend request (no pending request) from {1}", arg, this));
				return;
			}
			Log.Out(string.Format("[Discord] Rejecting {0} friend request from {1}", arg, this));
			if (_gameFriend)
			{
				this.ownerManager.client.RejectGameFriendRequest(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<DeclineFriendRequest>g__RejectRequestCallback|78_0));
				return;
			}
			this.ownerManager.client.RejectDiscordFriendRequest(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<DeclineFriendRequest>g__RejectRequestCallback|78_0));
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0007C2F8 File Offset: 0x0007A4F8
		public void RemoveFriend()
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			if (!this.IsFriend)
			{
				Log.Out(string.Format("[Discord] Not removing friend (neither a game nor Discord friend): {0}", this));
				return;
			}
			this.ownerManager.client.RemoveDiscordAndGameFriend(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<RemoveFriend>g__RemoveFriendCallback|79_0));
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0007C350 File Offset: 0x0007A550
		public void BlockUser()
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			if (this.DiscordRelationship == RelationshipType.Blocked)
			{
				Log.Out(string.Format("[Discord] Not blocking user (already blocked): {0}", this));
				return;
			}
			this.ownerManager.client.BlockUser(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<BlockUser>g__BlockUserCallback|80_0));
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x0007C3A8 File Offset: 0x0007A5A8
		public void UnblockUser()
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			if (this.DiscordRelationship != RelationshipType.Blocked)
			{
				Log.Out(string.Format("[Discord] Not unblocking user (not blocked): {0}", this));
				return;
			}
			this.ownerManager.client.UnblockUser(this.ID, new Discord.Sdk.Client.UpdateRelationshipCallback(DiscordManager.DiscordUser.<UnblockUser>g__UnblockUserUserCallback|81_0));
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x0007C3FF File Offset: 0x0007A5FF
		// (set) Token: 0x060014E0 RID: 5344 RVA: 0x0007C407 File Offset: 0x0007A607
		public Activity Activity { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060014E1 RID: 5345 RVA: 0x0007C410 File Offset: 0x0007A610
		// (set) Token: 0x060014E2 RID: 5346 RVA: 0x0007C418 File Offset: 0x0007A618
		public bool PendingOutgoingJoinRequest { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060014E3 RID: 5347 RVA: 0x0007C421 File Offset: 0x0007A621
		// (set) Token: 0x060014E4 RID: 5348 RVA: 0x0007C429 File Offset: 0x0007A629
		public bool PendingIncomingJoinRequest
		{
			get
			{
				return this.pendingIncomingJoinRequest;
			}
			set
			{
				if (value == this.pendingIncomingJoinRequest)
				{
					return;
				}
				this.pendingIncomingJoinRequest = value;
				DiscordManager.ActivityInviteReceivedCallback activityInviteReceived = this.ownerManager.ActivityInviteReceived;
				if (activityInviteReceived == null)
				{
					return;
				}
				activityInviteReceived(this, !value, ActivityActionTypes.JoinRequest);
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060014E5 RID: 5349 RVA: 0x0007C457 File Offset: 0x0007A657
		public bool PendingIncomingInvite
		{
			get
			{
				return this.incomingInviteActivity != null;
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060014E6 RID: 5350 RVA: 0x0007C462 File Offset: 0x0007A662
		public bool JoinableActivity
		{
			get
			{
				Activity activity = this.Activity;
				return ((activity != null) ? activity.Party() : null) != null;
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060014E7 RID: 5351 RVA: 0x0007C479 File Offset: 0x0007A679
		public bool InGame
		{
			get
			{
				return this.Activity != null;
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060014E8 RID: 5352 RVA: 0x0007C484 File Offset: 0x0007A684
		public bool InSameSession
		{
			get
			{
				int num;
				return this.ownerManager.userMappings.TryGetEntityId(this.ID, out num);
			}
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x0007C4A9 File Offset: 0x0007A6A9
		public void UpdatePresenceInfo()
		{
			if (!this.ownerManager.IsReady)
			{
				this.Activity = null;
				return;
			}
			this.Activity = this.userHandle.GameActivity();
			this.logPresenceInfo();
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0007C4D8 File Offset: 0x0007A6D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void logPresenceInfo()
		{
			StatusType enumValue = this.userHandle.Status();
			string arg = "<null>";
			if (this.Activity != null)
			{
				ulong? num = this.Activity.ApplicationId();
				string text = this.Activity.Details();
				string text2 = this.Activity.Name();
				string text3 = this.Activity.State();
				ActivityTypes enumValue2 = this.Activity.Type();
				using (ActivityAssets activityAssets = this.Activity.Assets())
				{
					using (ActivityParty activityParty = this.Activity.Party())
					{
						string text4 = "null";
						if (activityParty != null)
						{
							text4 = string.Format("id={0}, size={1}, max={2}", activityParty.Id(), activityParty.CurrentSize(), activityParty.MaxSize());
						}
						using (ActivitySecrets activitySecrets = this.Activity.Secrets())
						{
							string text5 = ((activitySecrets != null) ? activitySecrets.Join() : null) ?? "null";
							using (ActivityTimestamps activityTimestamps = this.Activity.Timestamps())
							{
								string text6 = "null";
								if (activityTimestamps != null)
								{
									text6 = string.Format("start={0}, end={1}", activityTimestamps.Start(), activityTimestamps.End());
								}
								arg = string.Format(" appId={0}, type={1}, name='{2}', state={3}, details='{4}', assets={5}, party=<{6}>, secrets.join={7}, timestamps=<{8}> ", new object[]
								{
									num,
									enumValue2.ToStringCached<ActivityTypes>(),
									text2,
									text3,
									text,
									activityAssets != null,
									text4,
									text5,
									text6
								});
							}
						}
					}
				}
			}
			DiscordManager.logCallbackInfo(string.Format("OnPresenceChanged: user={0}, status={1}, activity=<{2}>", this, enumValue.ToStringCached<StatusType>(), arg), LogType.Log);
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0007C6F0 File Offset: 0x0007A8F0
		public void SetIncomingInviteActivity(ActivityInvite _invite)
		{
			this.incomingInviteActivity = _invite;
			DiscordManager.ActivityInviteReceivedCallback activityInviteReceived = this.ownerManager.ActivityInviteReceived;
			if (activityInviteReceived == null)
			{
				return;
			}
			activityInviteReceived(this, _invite == null, ActivityActionTypes.Join);
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x0007C714 File Offset: 0x0007A914
		public void SendInvite()
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			Log.Out(string.Format("[Discord] Sending invite to {0}", this));
			this.PendingIncomingJoinRequest = false;
			this.ownerManager.client.SendActivityInvite(this.ID, "", delegate(ClientResult _result)
			{
				DiscordManager.logCallbackInfoWithClientResult("SendActivityInvite", null, _result, true);
			});
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x0007C780 File Offset: 0x0007A980
		public void SendJoinRequest()
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			Log.Out(string.Format("[Discord] Sending join request to {0}", this));
			this.ownerManager.client.SendActivityJoinRequest(this.ID, delegate(ClientResult _result)
			{
				DiscordManager.logCallbackInfoWithClientResult("SendActivityJoinRequest", null, _result, true);
				this.PendingOutgoingJoinRequest = true;
			});
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x0007C7CD File Offset: 0x0007A9CD
		public void DeclineJoinRequest()
		{
			if (!this.PendingIncomingJoinRequest)
			{
				Log.Out(string.Format("[Discord] Trying to decline incoming join request without first receiving a request from {0}", this));
				return;
			}
			this.PendingIncomingJoinRequest = false;
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x0007C7EF File Offset: 0x0007A9EF
		public void DeclineInvite()
		{
			if (!this.PendingIncomingInvite)
			{
				Log.Out(string.Format("[Discord] Trying to decline invite without first receiving an invite from {0}", this));
				return;
			}
			this.incomingInviteActivity = null;
			DiscordManager.ActivityInviteReceivedCallback activityInviteReceived = this.ownerManager.ActivityInviteReceived;
			if (activityInviteReceived == null)
			{
				return;
			}
			activityInviteReceived(this, true, ActivityActionTypes.Join);
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x0007C82C File Offset: 0x0007AA2C
		public void AcceptInvite(ActivityInvite _invite = null)
		{
			if (!this.ownerManager.IsReady)
			{
				return;
			}
			if (_invite == null)
			{
				_invite = this.incomingInviteActivity;
			}
			this.incomingInviteActivity = null;
			if (_invite == null)
			{
				Log.Out(string.Format("[Discord] Trying to accept invite without first receiving an invite from {0}", this));
				return;
			}
			Log.Out(string.Format("[Discord] Accepting invite from {0}", this));
			this.PendingOutgoingJoinRequest = false;
			DiscordManager.ActivityInviteReceivedCallback activityInviteReceived = this.ownerManager.ActivityInviteReceived;
			if (activityInviteReceived != null)
			{
				activityInviteReceived(this, true, ActivityActionTypes.Join);
			}
			this.ownerManager.client.AcceptActivityInvite(_invite, delegate(ClientResult _result, string _secret)
			{
				DiscordManager.logCallbackInfoWithClientResult("AcceptActivityInvite", "secret=" + _secret, _result, true);
				_invite.Dispose();
			});
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x0007C8DC File Offset: 0x0007AADC
		public override string ToString()
		{
			return string.Format("<Id={0}, local={1}, Discord='{2}', Player='{3}', DcRel={4}, GameRel={5}>", new object[]
			{
				this.ID,
				this.IsLocalAccount,
				this.DiscordDisplayName,
				this.PlayerName,
				this.DiscordRelationship.ToStringCached<RelationshipType>(),
				this.GameRelationship.ToStringCached<RelationshipType>()
			});
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x0007C943 File Offset: 0x0007AB43
		public void Dispose()
		{
			UserHandle userHandle = this.userHandle;
			if (userHandle != null)
			{
				userHandle.Dispose();
			}
			Activity activity = this.Activity;
			if (activity != null)
			{
				activity.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x0007C96D File Offset: 0x0007AB6D
		public bool Equals(DiscordManager.DiscordUser _other)
		{
			return _other != null && (this == _other || this.ID == _other.ID);
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x0007C988 File Offset: 0x0007AB88
		public override bool Equals(object _obj)
		{
			return _obj != null && (this == _obj || (!(_obj.GetType() != base.GetType()) && this.Equals((DiscordManager.DiscordUser)_obj)));
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0007C9B8 File Offset: 0x0007ABB8
		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0007C9D3 File Offset: 0x0007ABD3
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <SendFriendRequest>g__SendRequestCallback|77_0(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("Send*FriendRequestById", null, _result, true);
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x0007C9E2 File Offset: 0x0007ABE2
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <DeclineFriendRequest>g__RejectRequestCallback|78_0(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("Reject*FriendRequest", null, _result, true);
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x0007C9F1 File Offset: 0x0007ABF1
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <RemoveFriend>g__RemoveFriendCallback|79_0(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("RemoveDiscordAndGameFriend", null, _result, true);
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x0007CA00 File Offset: 0x0007AC00
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <BlockUser>g__BlockUserCallback|80_0(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("BlockUser", null, _result, true);
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x0007CA0F File Offset: 0x0007AC0F
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <UnblockUser>g__UnblockUserUserCallback|81_0(ClientResult _result)
		{
			DiscordManager.logCallbackInfoWithClientResult("UnblockUser", null, _result, true);
		}

		// Token: 0x04000D6A RID: 3434
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager ownerManager;

		// Token: 0x04000D6B RID: 3435
		[PublicizedFrom(EAccessModifier.Private)]
		public UserHandle userHandle;

		// Token: 0x04000D6C RID: 3436
		[PublicizedFrom(EAccessModifier.Private)]
		public string discordDisplayName;

		// Token: 0x04000D6D RID: 3437
		public readonly ulong ID;

		// Token: 0x04000D6E RID: 3438
		public readonly bool IsLocalAccount;

		// Token: 0x04000D70 RID: 3440
		[PublicizedFrom(EAccessModifier.Private)]
		public string playerName;

		// Token: 0x04000D71 RID: 3441
		[PublicizedFrom(EAccessModifier.Private)]
		public bool avatarStartedDownload;

		// Token: 0x04000D72 RID: 3442
		[PublicizedFrom(EAccessModifier.Private)]
		public Texture2D avatar;

		// Token: 0x04000D73 RID: 3443
		public bool MessageSentFromGame;

		// Token: 0x04000D76 RID: 3446
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isSpamRequest;

		// Token: 0x04000D79 RID: 3449
		[PublicizedFrom(EAccessModifier.Private)]
		public bool pendingIncomingJoinRequest;

		// Token: 0x04000D7A RID: 3450
		[PublicizedFrom(EAccessModifier.Private)]
		public ActivityInvite incomingInviteActivity;
	}

	// Token: 0x020002E0 RID: 736
	public class DiscordUserMappingManager
	{
		// Token: 0x06001507 RID: 5383 RVA: 0x0007CC34 File Offset: 0x0007AE34
		public DiscordUserMappingManager(DiscordManager _ownerManager)
		{
			this.ownerManager = _ownerManager;
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x0007CC5C File Offset: 0x0007AE5C
		public void UpdateMapping(int _entityId, bool _remove, ulong _discordId)
		{
			ulong key;
			if (this.entityIdToDiscordId.TryGetValue(_entityId, out key))
			{
				this.discordIdToEntityId.Remove(key);
			}
			if (_remove)
			{
				this.entityIdToDiscordId.Remove(_entityId);
				return;
			}
			this.entityIdToDiscordId[_entityId] = _discordId;
			this.discordIdToEntityId[_discordId] = _entityId;
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x0007CCB1 File Offset: 0x0007AEB1
		public bool TryGetDiscordId(int _entity, out ulong _discordId)
		{
			return this.entityIdToDiscordId.TryGetValue(_entity, out _discordId);
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x0007CCC0 File Offset: 0x0007AEC0
		public bool TryGetEntityId(ulong _discordId, out int _entity)
		{
			return this.discordIdToEntityId.TryGetValue(_discordId, out _entity);
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x0007CCD0 File Offset: 0x0007AED0
		public void SendMappingsToClient(ClientInfo _clientInfo)
		{
			List<int> list = new List<int>();
			List<ulong> list2 = new List<ulong>();
			foreach (KeyValuePair<int, ulong> keyValuePair in this.entityIdToDiscordId)
			{
				int num;
				ulong num2;
				keyValuePair.Deconstruct(out num, out num2);
				int item = num;
				ulong item2 = num2;
				list.Add(item);
				list2.Add(item2);
			}
			_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageDiscordIdMappings>().Setup(list, list2));
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x0007CD5C File Offset: 0x0007AF5C
		public void Clear()
		{
			this.discordIdToEntityId.Clear();
			this.entityIdToDiscordId.Clear();
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x0007CD74 File Offset: 0x0007AF74
		public void GetAll(Action<int, ulong> _callback)
		{
			if (GameManager.Instance.World == null)
			{
				return;
			}
			foreach (KeyValuePair<int, ulong> keyValuePair in this.entityIdToDiscordId)
			{
				int num;
				ulong num2;
				keyValuePair.Deconstruct(out num, out num2);
				int arg = num;
				ulong arg2 = num2;
				_callback(arg, arg2);
			}
		}

		// Token: 0x04000D83 RID: 3459
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager ownerManager;

		// Token: 0x04000D84 RID: 3460
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<ulong, int> discordIdToEntityId = new Dictionary<ulong, int>();

		// Token: 0x04000D85 RID: 3461
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<int, ulong> entityIdToDiscordId = new Dictionary<int, ulong>();
	}

	// Token: 0x020002E1 RID: 737
	public class DiscordUserSettingsManager
	{
		// Token: 0x0600150E RID: 5390 RVA: 0x0007CDE8 File Offset: 0x0007AFE8
		public double GetUserVolume(ulong _userId)
		{
			int num;
			if (this.userVolumes.TryGetValue(_userId, out num))
			{
				return (double)num / 100.0;
			}
			return 1.0;
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x0007CE1C File Offset: 0x0007B01C
		public void SetUserVolume(ulong _userId, double _volume)
		{
			int num = Mathf.RoundToInt((float)(_volume * 100.0));
			num = Mathf.Clamp(num, 0, 200);
			if (num >= 99 && num <= 101)
			{
				this.userVolumes.Remove(_userId);
				return;
			}
			this.userVolumes[_userId] = num;
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001510 RID: 5392 RVA: 0x0007CE6D File Offset: 0x0007B06D
		public static string DataFilePath
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return GameIO.GetUserGameDataDir() + "/DiscordUserSettings.dat";
			}
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x0007CE80 File Offset: 0x0007B080
		public static DiscordManager.DiscordUserSettingsManager Load()
		{
			DiscordManager.DiscordUserSettingsManager discordUserSettingsManager = new DiscordManager.DiscordUserSettingsManager();
			if (!SdFile.Exists(DiscordManager.DiscordUserSettingsManager.DataFilePath))
			{
				return discordUserSettingsManager;
			}
			DiscordManager.DiscordUserSettingsManager result;
			try
			{
				using (Stream stream = SdFile.OpenRead(DiscordManager.DiscordUserSettingsManager.DataFilePath))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						pooledBinaryReader.ReadInt32();
						int num = pooledBinaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							ulong key = pooledBinaryReader.ReadUInt64();
							int value = pooledBinaryReader.ReadInt32();
							discordUserSettingsManager.userVolumes[key] = value;
						}
						result = discordUserSettingsManager;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("[Discord] Failed loading UserSettings file: " + ex.Message);
				Log.Exception(ex);
				result = new DiscordManager.DiscordUserSettingsManager();
			}
			return result;
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x0007CF68 File Offset: 0x0007B168
		public void Save()
		{
			using (Stream stream = SdFile.Open(DiscordManager.DiscordUserSettingsManager.DataFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(stream);
					pooledBinaryWriter.Write(1);
					pooledBinaryWriter.Write(this.userVolumes.Count);
					foreach (KeyValuePair<ulong, int> keyValuePair in this.userVolumes)
					{
						ulong num;
						int num2;
						keyValuePair.Deconstruct(out num, out num2);
						ulong value = num;
						int value2 = num2;
						pooledBinaryWriter.Write(value);
						pooledBinaryWriter.Write(value2);
					}
				}
			}
		}

		// Token: 0x04000D86 RID: 3462
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<ulong, int> userVolumes = new Dictionary<ulong, int>();
	}

	// Token: 0x020002E2 RID: 738
	public class LobbyInfo
	{
		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06001514 RID: 5396 RVA: 0x0007D04F File Offset: 0x0007B24F
		// (set) Token: 0x06001515 RID: 5397 RVA: 0x0007D057 File Offset: 0x0007B257
		public string Secret
		{
			get
			{
				return this.secret;
			}
			set
			{
				if (value == this.secret)
				{
					return;
				}
				this.secret = value;
				DiscordManager.LobbyStateChangedCallback lobbyStateChanged = this.ownerManager.LobbyStateChanged;
				if (lobbyStateChanged == null)
				{
					return;
				}
				lobbyStateChanged(this, this.IsReady, this.IsJoined);
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06001516 RID: 5398 RVA: 0x0007D091 File Offset: 0x0007B291
		public bool IsReady
		{
			get
			{
				return !string.IsNullOrEmpty(this.Secret);
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06001517 RID: 5399 RVA: 0x0007D0A1 File Offset: 0x0007B2A1
		public bool IsJoined
		{
			get
			{
				return this.Id > 0UL;
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06001518 RID: 5400 RVA: 0x0007D0AD File Offset: 0x0007B2AD
		// (set) Token: 0x06001519 RID: 5401 RVA: 0x0007D0B5 File Offset: 0x0007B2B5
		public ulong Id
		{
			get
			{
				return this.id;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				if (value == this.id)
				{
					return;
				}
				this.id = value;
				DiscordManager.LobbyStateChangedCallback lobbyStateChanged = this.ownerManager.LobbyStateChanged;
				if (lobbyStateChanged == null)
				{
					return;
				}
				lobbyStateChanged(this, this.IsReady, this.IsJoined);
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600151A RID: 5402 RVA: 0x0007D0EA File Offset: 0x0007B2EA
		public DiscordManager.CallInfo VoiceCall
		{
			get
			{
				return this.voiceCall;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x0600151B RID: 5403 RVA: 0x0007D0F2 File Offset: 0x0007B2F2
		public bool IsInVoice
		{
			get
			{
				return this.voiceCall.IsJoined;
			}
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x0007D0FF File Offset: 0x0007B2FF
		public LobbyInfo(DiscordManager _ownerManager, DiscordManager.ELobbyType _lobbyType)
		{
			this.ownerManager = _ownerManager;
			this.LobbyType = _lobbyType;
			this.voiceCall = new DiscordManager.CallInfo(this, this.ownerManager);
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x0007D134 File Offset: 0x0007B334
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void Finalize()
		{
			try
			{
				LobbyHandle lobbyHandle = this.handle;
				if (lobbyHandle != null)
				{
					lobbyHandle.Dispose();
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x0007D16C File Offset: 0x0007B36C
		public void Join(bool _errorWithoutSecret = true)
		{
			if (this.IsJoined)
			{
				Log.Warning("[Discord] Lobby.Join failed, already in lobby");
				return;
			}
			if (string.IsNullOrEmpty(this.Secret))
			{
				if (_errorWithoutSecret)
				{
					Log.Error("[Discord] Lobby.Join failed, no secret set");
				}
				return;
			}
			Dictionary<string, string> memberMetadata = new Dictionary<string, string>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
			dictionary["GameSession"] = gameServerInfo.GetValue(GameInfoString.UniqueId);
			dictionary["Platform"] = ((PlatformManager.CrossplatformPlatform != null) ? PlatformManager.CrossplatformPlatform.PlatformIdentifier.ToStringCached<EPlatformIdentifier>() : PlatformManager.NativePlatform.PlatformIdentifier.ToStringCached<EPlatformIdentifier>());
			Log.Out(string.Concat(new string[]
			{
				"[Discord] CreateOrJoinLobby: ",
				this.Secret,
				", session=",
				dictionary["GameSession"],
				", platform=",
				dictionary["Platform"]
			}));
			this.ownerManager.client.CreateOrJoinLobbyWithMetadata(this.Secret, dictionary, memberMetadata, delegate(ClientResult _result, ulong _lobbyId)
			{
				DiscordManager.logCallbackInfoWithClientResult("CreateOrJoinLobbyResult", string.Format("lobbyId={0}", _lobbyId), _result, false);
				if (_result.Type() != ErrorType.None)
				{
					_result.Dispose();
					return;
				}
				this.Id = _lobbyId;
				this.handle = this.ownerManager.client.GetLobbyHandle(_lobbyId);
				this.UpdateMembers();
				_result.Dispose();
			});
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x0007D28C File Offset: 0x0007B48C
		public void Leave(bool _manual = true)
		{
			if (this.IsInVoice)
			{
				this.VoiceCall.Leave(_manual);
			}
			if (this.IsJoined)
			{
				this.ownerManager.client.LeaveLobby(this.Id, delegate(ClientResult _result)
				{
					DiscordManager.logCallbackInfoWithClientResult("LeaveLobby", string.Format("type={0}", this.LobbyType), _result, true);
				});
			}
			this.Id = 0UL;
			LobbyHandle lobbyHandle = this.handle;
			if (lobbyHandle != null)
			{
				lobbyHandle.Dispose();
			}
			this.handle = null;
			this.UpdateMembers();
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x0007D300 File Offset: 0x0007B500
		public void UpdateMembers()
		{
			this.lobbyMembers.Clear();
			if (this.IsJoined)
			{
				foreach (ulong num in this.handle.LobbyMemberIds())
				{
					this.ownerManager.GetUser(num);
					this.lobbyMembers.Add(num);
				}
			}
			DiscordManager.LobbyMembersChangedCallback lobbyMembersChanged = this.ownerManager.LobbyMembersChanged;
			if (lobbyMembersChanged == null)
			{
				return;
			}
			lobbyMembersChanged(this);
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x0007D36E File Offset: 0x0007B56E
		public bool HasMember(ulong _userId)
		{
			return this.lobbyMembers.Contains(_userId);
		}

		// Token: 0x04000D87 RID: 3463
		[PublicizedFrom(EAccessModifier.Private)]
		public const string LobbyMetadataKeyGameSession = "GameSession";

		// Token: 0x04000D88 RID: 3464
		[PublicizedFrom(EAccessModifier.Private)]
		public const string LobbyMetadataKeyPlatform = "Platform";

		// Token: 0x04000D89 RID: 3465
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager ownerManager;

		// Token: 0x04000D8A RID: 3466
		public readonly DiscordManager.ELobbyType LobbyType;

		// Token: 0x04000D8B RID: 3467
		[PublicizedFrom(EAccessModifier.Private)]
		public string secret;

		// Token: 0x04000D8C RID: 3468
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong id;

		// Token: 0x04000D8D RID: 3469
		[PublicizedFrom(EAccessModifier.Private)]
		public LobbyHandle handle;

		// Token: 0x04000D8E RID: 3470
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager.CallInfo voiceCall;

		// Token: 0x04000D8F RID: 3471
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<ulong> lobbyMembers = new HashSet<ulong>();
	}

	// Token: 0x020002E3 RID: 739
	public class PresenceManager
	{
		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06001524 RID: 5412 RVA: 0x0007D401 File Offset: 0x0007B601
		public bool JoinableActivitySet
		{
			get
			{
				return !string.IsNullOrEmpty(this.joinSecretJson);
			}
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x0007D411 File Offset: 0x0007B611
		public PresenceManager(DiscordManager _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x0007D43E File Offset: 0x0007B63E
		[PublicizedFrom(EAccessModifier.Private)]
		public ActivityAssets initActivity()
		{
			this.activity = new Activity();
			this.activity.SetName("7 Days To Die");
			this.activity.SetType(ActivityTypes.Playing);
			ActivityAssets activityAssets = new ActivityAssets();
			activityAssets.SetLargeImage("7dtd");
			return activityAssets;
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0007D478 File Offset: 0x0007B678
		public void RegisterDiscordCallbacks()
		{
			this.owner.client.SetActivityInviteCreatedCallback(new Discord.Sdk.Client.ActivityInviteCallback(this.OnActivityInviteCreated));
			this.owner.client.SetActivityInviteUpdatedCallback(new Discord.Sdk.Client.ActivityInviteCallback(this.OnActivityInviteUpdated));
			this.owner.client.SetActivityJoinCallback(new Discord.Sdk.Client.ActivityJoinCallback(this.OnActivityJoin));
			this.SetRichPresenceState(new IRichPresence.PresenceStates?(IRichPresence.PresenceStates.Menu));
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x0007D4E8 File Offset: 0x0007B6E8
		public void SetRichPresenceState(IRichPresence.PresenceStates? _state = null)
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			IRichPresence.PresenceStates presenceStates = _state.GetValueOrDefault();
			if (_state == null)
			{
				presenceStates = this.currentRichPresenceState;
				_state = new IRichPresence.PresenceStates?(presenceStates);
			}
			if (this.currentRichPresenceState == _state.Value)
			{
				IRichPresence.PresenceStates? presenceStates2 = _state;
				presenceStates = IRichPresence.PresenceStates.InGame;
				if (presenceStates2.GetValueOrDefault() == presenceStates & presenceStates2 != null)
				{
					this.refreshRichPresenceData();
				}
				this.sendCurrentRichPresence();
				return;
			}
			this.timeStartedCurrentActivity = (ulong)((_state.Value == IRichPresence.PresenceStates.InGame) ? Utils.CurrentUnixTime : 0U);
			this.joinSecretJson = null;
			this.currentRichPresenceState = _state.Value;
			this.refreshRichPresenceData();
			this.sendCurrentRichPresence();
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0007D58C File Offset: 0x0007B78C
		[PublicizedFrom(EAccessModifier.Private)]
		public void sendCurrentRichPresence()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			if (!this.owner.IsReady)
			{
				return;
			}
			if (this.activity == null)
			{
				return;
			}
			this.owner.client.UpdateRichPresence(this.activity, delegate(ClientResult _result)
			{
				DiscordManager.logCallbackInfoWithClientResult("UpdateRichPresence", null, _result, true);
				DiscordManager.FriendsListChangedCallback friendsListChanged = this.owner.FriendsListChanged;
				if (friendsListChanged != null)
				{
					friendsListChanged();
				}
				Activity activity = this.activity;
				if (activity != null)
				{
					activity.Dispose();
				}
				this.activity = null;
			});
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x0007D5DC File Offset: 0x0007B7DC
		[PublicizedFrom(EAccessModifier.Private)]
		public void refreshRichPresenceData()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			using (ActivityAssets activityAssets = this.initActivity())
			{
				this.setTimestamps();
				this.setDetailsAndState();
				this.setLargeImageAndTooltip(activityAssets);
				this.setSmallImageAndTooltip(activityAssets);
				this.setParty();
				this.setPlatforms();
				this.activity.SetAssets(activityAssets);
			}
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x0007D648 File Offset: 0x0007B848
		[PublicizedFrom(EAccessModifier.Private)]
		public void setDetailsAndState()
		{
			switch (this.currentRichPresenceState)
			{
			case IRichPresence.PresenceStates.Menu:
				this.activity.SetDetails(Localization.Get("discordPresenceDetailsInMenu", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
				this.activity.SetState(null);
				return;
			case IRichPresence.PresenceStates.Loading:
				this.activity.SetDetails(Localization.Get("discordPresenceDetailsStartingGame", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
				this.activity.SetState(null);
				return;
			case IRichPresence.PresenceStates.Connecting:
				this.activity.SetDetails(Localization.Get("discordPresenceDetailsConnectingToServer", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
				this.activity.SetState(null);
				return;
			case IRichPresence.PresenceStates.InGame:
			{
				World world = GameManager.Instance.World;
				EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
				if (primaryPlayer == null)
				{
					this.activity.SetDetails(null);
					this.activity.SetState(null);
					return;
				}
				if (GameManager.Instance.IsEditMode())
				{
					this.activity.SetDetails(Localization.Get(PrefabEditModeManager.Instance.IsActive() ? "discordPresenceDetailsPoiEditor" : "discordPresenceDetailsWorldEditor", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
					this.activity.SetState(null);
					return;
				}
				Party party = primaryPlayer.Party;
				int num = (party != null) ? party.MemberList.Count : 1;
				this.activity.SetState(string.Format(Localization.Get((num > 1) ? "discordPresenceStateInParty" : "discordPresenceStateSolo", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false), num));
				if (TwitchManager.HasInstance && TwitchManager.Current.InitState == TwitchManager.InitStates.Ready)
				{
					this.activity.SetDetails(Localization.Get("discordPresenceDetailsTwitchIntegration", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
					return;
				}
				ulong worldTime = world.worldTime;
				int @int = GameStats.GetInt(EnumUtils.Parse<EnumGameStats>("BloodMoonDay", false));
				ValueTuple<int, int> duskDawnTimes = GameUtils.CalcDuskDawnHours(GamePrefs.GetInt(EnumGamePrefs.DayLightLength));
				if (GameUtils.IsBloodMoonTime(worldTime, duskDawnTimes, @int))
				{
					this.activity.SetDetails(Localization.Get("discordPresenceDetailsBloodMoon", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
					return;
				}
				Quest activeQuest = primaryPlayer.QuestJournal.ActiveQuest;
				if (activeQuest != null)
				{
					this.activity.SetDetails(string.Format(Localization.Get("discordPresenceDetailsQuesting", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false), activeQuest.QuestClass.Name));
					return;
				}
				if (DiscordManager.PresenceManager.<setDetailsAndState>g__PlayerAtHome|15_0(primaryPlayer))
				{
					this.activity.SetDetails(Localization.Get("discordPresenceDetailsAtHome", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
					return;
				}
				this.activity.SetDetails(Localization.Get("discordPresenceDetailsExploring", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false));
				return;
			}
			default:
				throw new ArgumentOutOfRangeException("currentRichPresenceState", this.currentRichPresenceState, null);
			}
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x0007D8C4 File Offset: 0x0007BAC4
		[PublicizedFrom(EAccessModifier.Private)]
		public void setTimestamps()
		{
			if (this.timeStartedCurrentActivity <= 0UL)
			{
				this.activity.SetTimestamps(null);
				return;
			}
			using (ActivityTimestamps activityTimestamps = new ActivityTimestamps())
			{
				activityTimestamps.SetStart(this.timeStartedCurrentActivity);
				this.activity.SetTimestamps(activityTimestamps);
			}
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x0007D924 File Offset: 0x0007BB24
		[PublicizedFrom(EAccessModifier.Private)]
		public void setLargeImageAndTooltip(ActivityAssets _activityAssets)
		{
			DiscordManager.PresenceManager.<>c__DisplayClass18_0 CS$<>8__locals1;
			CS$<>8__locals1._activityAssets = _activityAssets;
			IRichPresence.PresenceStates presenceStates = this.currentRichPresenceState;
			if (presenceStates <= IRichPresence.PresenceStates.Connecting)
			{
				DiscordManager.PresenceManager.<setLargeImageAndTooltip>g__SetDefaultImage|18_0(ref CS$<>8__locals1);
				return;
			}
			if (presenceStates != IRichPresence.PresenceStates.InGame)
			{
				throw new ArgumentOutOfRangeException("currentRichPresenceState", this.currentRichPresenceState, null);
			}
			if (GameManager.Instance.IsEditMode())
			{
				DiscordManager.PresenceManager.<setLargeImageAndTooltip>g__SetDefaultImage|18_0(ref CS$<>8__locals1);
				return;
			}
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer == null)
			{
				DiscordManager.PresenceManager.<setLargeImageAndTooltip>g__SetDefaultImage|18_0(ref CS$<>8__locals1);
				return;
			}
			BiomeDefinition biomeStandingOn = primaryPlayer.biomeStandingOn;
			if (biomeStandingOn == null)
			{
				DiscordManager.PresenceManager.<setLargeImageAndTooltip>g__SetDefaultImage|18_0(ref CS$<>8__locals1);
				return;
			}
			string sBiomeName = biomeStandingOn.m_sBiomeName;
			ValueTuple<string, string> valueTuple;
			if (!this.biomeNameToAssetsMap.TryGetValue(sBiomeName, out valueTuple))
			{
				valueTuple = new ValueTuple<string, string>(Localization.Get("biome_" + sBiomeName, DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false), "biome" + biomeStandingOn.m_BiomeType.ToStringCached<BiomeDefinition.BiomeType>().ToLower());
				this.biomeNameToAssetsMap[sBiomeName] = valueTuple;
			}
			CS$<>8__locals1._activityAssets.SetLargeImage(valueTuple.Item2);
			CS$<>8__locals1._activityAssets.SetLargeText(valueTuple.Item1);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x0007DA38 File Offset: 0x0007BC38
		[PublicizedFrom(EAccessModifier.Private)]
		public void setSmallImageAndTooltip(ActivityAssets _activityAssets)
		{
			DiscordManager.PresenceManager.<>c__DisplayClass20_0 CS$<>8__locals1;
			CS$<>8__locals1._activityAssets = _activityAssets;
			IRichPresence.PresenceStates presenceStates = this.currentRichPresenceState;
			if (presenceStates <= IRichPresence.PresenceStates.Connecting)
			{
				DiscordManager.PresenceManager.<setSmallImageAndTooltip>g__ClearImage|20_0(ref CS$<>8__locals1);
				return;
			}
			if (presenceStates != IRichPresence.PresenceStates.InGame)
			{
				throw new ArgumentOutOfRangeException("currentRichPresenceState", this.currentRichPresenceState, null);
			}
			if (GameManager.Instance.IsEditMode())
			{
				DiscordManager.PresenceManager.<setSmallImageAndTooltip>g__ClearImage|20_0(ref CS$<>8__locals1);
				return;
			}
			World world = GameManager.Instance.World;
			ulong worldTime = world.worldTime;
			int @int = GameStats.GetInt(EnumGameStats.BloodMoonWarning);
			ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(worldTime);
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			int int2 = GameStats.GetInt(EnumUtils.Parse<EnumGameStats>("BloodMoonDay", false));
			ValueTuple<int, int> duskDawnTimes = GameUtils.CalcDuskDawnHours(GamePrefs.GetInt(EnumGamePrefs.DayLightLength));
			bool flag = world.IsDaytime();
			bool flag2 = GameUtils.IsBloodMoonTime(worldTime, duskDawnTimes, int2);
			bool flag3 = !flag2 && @int != -1 && GameStats.GetInt(EnumGameStats.BloodMoonDay) == item && @int <= item2;
			string smallText = ValueDisplayFormatters.WorldTime(worldTime, DiscordManager.PresenceManager.dayTimeFormatString);
			if (flag2)
			{
				CS$<>8__locals1._activityAssets.SetSmallImage("statebloodmoon");
			}
			else if (flag3)
			{
				CS$<>8__locals1._activityAssets.SetSmallImage("statebloodmoonwarning");
			}
			else if (flag)
			{
				CS$<>8__locals1._activityAssets.SetSmallImage("stateday");
			}
			else
			{
				CS$<>8__locals1._activityAssets.SetSmallImage("statenight");
			}
			CS$<>8__locals1._activityAssets.SetSmallText(smallText);
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x0007DB7C File Offset: 0x0007BD7C
		[PublicizedFrom(EAccessModifier.Private)]
		public void setParty()
		{
			if (this.currentRichPresenceState != IRichPresence.PresenceStates.InGame)
			{
				this.activity.SetParty(null);
				this.activity.SetSecrets(null);
				return;
			}
			GameServerInfo currentGameServerInfoServerOrClient = SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentGameServerInfoServerOrClient;
			string value = currentGameServerInfoServerOrClient.GetValue(GameInfoString.UniqueId);
			if (string.IsNullOrEmpty(value))
			{
				this.activity.SetParty(null);
				this.activity.SetSecrets(null);
				return;
			}
			using (ActivityParty activityParty = new ActivityParty())
			{
				activityParty.SetId(value);
				activityParty.SetCurrentSize(GameManager.Instance.World.Players.Count);
				activityParty.SetMaxSize(currentGameServerInfoServerOrClient.GetValue(GameInfoInt.MaxPlayers));
				this.activity.SetParty(activityParty);
				if (string.IsNullOrEmpty(this.joinSecretJson))
				{
					this.joinSecretJson = JsonConvert.SerializeObject(new DiscordManager.PresenceManager.ActivitySecret(currentGameServerInfoServerOrClient));
				}
				using (ActivitySecrets activitySecrets = new ActivitySecrets())
				{
					activitySecrets.SetJoin(this.joinSecretJson);
					this.activity.SetSecrets(activitySecrets);
				}
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x0007DC90 File Offset: 0x0007BE90
		[PublicizedFrom(EAccessModifier.Private)]
		public void setPlatforms()
		{
			if (this.currentRichPresenceState != IRichPresence.PresenceStates.InGame)
			{
				return;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentGameServerInfoServerOrClient.GetValue(GameInfoBool.AllowCrossplay))
			{
				this.activity.SetSupportedPlatforms(ActivityGamePlatforms.Desktop | ActivityGamePlatforms.Xbox | ActivityGamePlatforms.PS5);
				return;
			}
			Activity activity = this.activity;
			ActivityGamePlatforms supportedPlatforms;
			switch (PlatformManager.NativePlatform.PlatformIdentifier)
			{
			case EPlatformIdentifier.None:
				throw new ArgumentException("None", "SetSupportedPlatforms");
			case EPlatformIdentifier.Local:
				supportedPlatforms = ActivityGamePlatforms.Desktop;
				break;
			case EPlatformIdentifier.EOS:
				throw new ArgumentException("EOS", "SetSupportedPlatforms");
			case EPlatformIdentifier.Steam:
				supportedPlatforms = ActivityGamePlatforms.Desktop;
				break;
			case EPlatformIdentifier.XBL:
				supportedPlatforms = ActivityGamePlatforms.Xbox;
				break;
			case EPlatformIdentifier.PSN:
				supportedPlatforms = ActivityGamePlatforms.PS5;
				break;
			case EPlatformIdentifier.EGS:
				supportedPlatforms = ActivityGamePlatforms.Desktop;
				break;
			case EPlatformIdentifier.LAN:
				throw new ArgumentException("LAN", "SetSupportedPlatforms");
			case EPlatformIdentifier.Count:
				throw new ArgumentException("Count", "SetSupportedPlatforms");
			default:
				throw new ArgumentOutOfRangeException();
			}
			activity.SetSupportedPlatforms(supportedPlatforms);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x0007DD6E File Offset: 0x0007BF6E
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnActivityInviteCreated(ActivityInvite _invite)
		{
			this.handleInvite(true, _invite);
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x0007DD78 File Offset: 0x0007BF78
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnActivityInviteUpdated(ActivityInvite _invite)
		{
			this.handleInvite(false, _invite);
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x0007DD84 File Offset: 0x0007BF84
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleInvite(bool _created, ActivityInvite _invite)
		{
			DiscordManager.logCallbackInfo(string.Format("{0}: sender={1}/type={2}/party={3}/message={4}/valid={5}", new object[]
			{
				_created ? "OnActivityInviteCreated" : "OnActivityInviteUpdated",
				_invite.SenderId(),
				_invite.Type().ToStringCached<ActivityActionTypes>(),
				_invite.PartyId(),
				_invite.MessageId(),
				_invite.IsValid()
			}), LogType.Log);
			DiscordManager.DiscordUser user = this.owner.GetUser(_invite.SenderId());
			ActivityActionTypes activityActionTypes = _invite.Type();
			bool flag = _invite.IsValid();
			if (activityActionTypes != ActivityActionTypes.Join)
			{
				if (activityActionTypes == ActivityActionTypes.JoinRequest)
				{
					user.PendingIncomingJoinRequest = flag;
					_invite.Dispose();
					return;
				}
				return;
			}
			else
			{
				if (!user.PendingOutgoingJoinRequest)
				{
					user.SetIncomingInviteActivity(flag ? _invite : null);
					return;
				}
				if (flag)
				{
					user.AcceptInvite(_invite);
					return;
				}
				_invite.Dispose();
				return;
			}
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x0007DE58 File Offset: 0x0007C058
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnActivityJoin(string _joinSecret)
		{
			DiscordManager.logCallbackInfo("OnActivityJoin", LogType.Log);
			DiscordManager.PresenceManager.ActivitySecret activitySecret;
			try
			{
				activitySecret = JsonConvert.DeserializeObject<DiscordManager.PresenceManager.ActivitySecret>(_joinSecret);
			}
			catch (JsonException e)
			{
				Log.Error("[Discord] Failed reading invite secret:");
				Log.Exception(e);
				return;
			}
			DiscordManager.ActivityJoiningCallback activityJoining = this.owner.ActivityJoining;
			if (activityJoining != null)
			{
				activityJoining();
			}
			DiscordInviteListener.ListenerInstance.SetPendingInvite(activitySecret.SessionID, activitySecret.Password);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x0007DF30 File Offset: 0x0007C130
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static bool <setDetailsAndState>g__PlayerAtHome|15_0(EntityPlayerLocal _player)
		{
			SpawnPosition spawnPoint = _player.GetSpawnPoint();
			return (!spawnPoint.IsUndef() && (spawnPoint.position - _player.position).sqrMagnitude <= 2500f) || GameManager.Instance.World.GetLandClaimOwnerInParty(_player, _player.persistentPlayerData);
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x0007DF85 File Offset: 0x0007C185
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <setLargeImageAndTooltip>g__SetDefaultImage|18_0(ref DiscordManager.PresenceManager.<>c__DisplayClass18_0 A_0)
		{
			A_0._activityAssets.SetLargeImage("7dtd");
			A_0._activityAssets.SetLargeText(null);
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0007DFA3 File Offset: 0x0007C1A3
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <setSmallImageAndTooltip>g__ClearImage|20_0(ref DiscordManager.PresenceManager.<>c__DisplayClass20_0 A_0)
		{
			A_0._activityAssets.SetSmallImage(null);
			A_0._activityAssets.SetSmallText(null);
		}

		// Token: 0x04000D90 RID: 3472
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly string discordPresenceLocalizationLanguage = Localization.DefaultLanguage;

		// Token: 0x04000D91 RID: 3473
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DiscordManager owner;

		// Token: 0x04000D92 RID: 3474
		[PublicizedFrom(EAccessModifier.Private)]
		public IRichPresence.PresenceStates currentRichPresenceState = IRichPresence.PresenceStates.InGame;

		// Token: 0x04000D93 RID: 3475
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong timeStartedCurrentActivity = (ulong)Utils.CurrentUnixTime;

		// Token: 0x04000D94 RID: 3476
		[PublicizedFrom(EAccessModifier.Private)]
		public string joinSecretJson;

		// Token: 0x04000D95 RID: 3477
		[PublicizedFrom(EAccessModifier.Private)]
		public Activity activity;

		// Token: 0x04000D96 RID: 3478
		[TupleElementNames(new string[]
		{
			"label",
			"image"
		})]
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, ValueTuple<string, string>> biomeNameToAssetsMap = new Dictionary<string, ValueTuple<string, string>>();

		// Token: 0x04000D97 RID: 3479
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly string dayTimeFormatString = Localization.Get("xuiDay", DiscordManager.PresenceManager.discordPresenceLocalizationLanguage, false) + " {0}, {1:00}:{2:00}";

		// Token: 0x020002E4 RID: 740
		[JsonObject(MemberSerialization.Fields)]
		[PublicizedFrom(EAccessModifier.Private)]
		public class ActivitySecret
		{
			// Token: 0x0600153A RID: 5434 RVA: 0x0007DFC0 File Offset: 0x0007C1C0
			public ActivitySecret(GameServerInfo _gsi)
			{
				this.SessionID = _gsi.GetValue(GameInfoString.UniqueId);
				this.ServerIP = _gsi.GetValue(GameInfoString.IP);
				this.ServerPort = _gsi.GetValue(GameInfoInt.Port);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					this.Password = GamePrefs.GetString(EnumGamePrefs.ServerPassword);
					return;
				}
				this.Password = (ServerInfoCache.Instance.GetPassword(_gsi) ?? "");
			}

			// Token: 0x04000D98 RID: 3480
			[JsonProperty("ID")]
			public readonly string SessionID;

			// Token: 0x04000D99 RID: 3481
			[JsonProperty("IP")]
			public string ServerIP;

			// Token: 0x04000D9A RID: 3482
			[JsonProperty("Port")]
			public int ServerPort;

			// Token: 0x04000D9B RID: 3483
			[JsonProperty("PW")]
			public string Password;
		}
	}
}
