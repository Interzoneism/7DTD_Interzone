using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.RTC;
using Epic.OnlineServices.RTCAudio;
using UnityEngine;

namespace Platform.EOS
{
	// Token: 0x0200194E RID: 6478
	public class Voice : IPartyVoice
	{
		// Token: 0x170015E8 RID: 5608
		// (get) Token: 0x0600BEF9 RID: 48889 RVA: 0x004862D4 File Offset: 0x004844D4
		public UserIdentifierEos localUserIdentifier
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				IPlatform platform = this.owner;
				object obj;
				if (platform == null)
				{
					obj = null;
				}
				else
				{
					IUserClient user = platform.User;
					obj = ((user != null) ? user.PlatformUserId : null);
				}
				return (UserIdentifierEos)obj;
			}
		}

		// Token: 0x170015E9 RID: 5609
		// (get) Token: 0x0600BEFA RID: 48890 RVA: 0x004862F9 File Offset: 0x004844F9
		public ProductUserId localProductUserId
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				UserIdentifierEos localUserIdentifier = this.localUserIdentifier;
				if (localUserIdentifier == null)
				{
					return null;
				}
				return localUserIdentifier.ProductUserId;
			}
		}

		// Token: 0x1400012D RID: 301
		// (add) Token: 0x0600BEFB RID: 48891 RVA: 0x0048630C File Offset: 0x0048450C
		// (remove) Token: 0x0600BEFC RID: 48892 RVA: 0x0048636C File Offset: 0x0048456C
		public event Action Initialized
		{
			add
			{
				object obj = this.initializedDelegateLock;
				lock (obj)
				{
					this.initializedDelegates = (Action)Delegate.Combine(this.initializedDelegates, value);
					if (this.Status == EPartyVoiceStatus.Ok)
					{
						value();
					}
				}
			}
			remove
			{
				object obj = this.initializedDelegateLock;
				lock (obj)
				{
					this.initializedDelegates = (Action)Delegate.Remove(this.initializedDelegates, value);
				}
			}
		}

		// Token: 0x170015EA RID: 5610
		// (get) Token: 0x0600BEFD RID: 48893 RVA: 0x004863C0 File Offset: 0x004845C0
		// (set) Token: 0x0600BEFE RID: 48894 RVA: 0x004863C8 File Offset: 0x004845C8
		public EPartyVoiceStatus Status { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EPartyVoiceStatus.Uninitialized;

		// Token: 0x170015EB RID: 5611
		// (get) Token: 0x0600BEFF RID: 48895 RVA: 0x004863D1 File Offset: 0x004845D1
		public bool InLobby
		{
			get
			{
				return this.lobbyId != null && this.roomEntered;
			}
		}

		// Token: 0x170015EC RID: 5612
		// (get) Token: 0x0600BF00 RID: 48896 RVA: 0x004863E3 File Offset: 0x004845E3
		public bool InLobbyOrProgress
		{
			get
			{
				return this.InLobby || this.createInProgress || this.joinInProgress;
			}
		}

		// Token: 0x0600BF01 RID: 48897 RVA: 0x004863FD File Offset: 0x004845FD
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.api = (Api)this.owner.Api;
			this.api.ClientApiInitialized += this.OnClientApiInitialized;
		}

		// Token: 0x0600BF02 RID: 48898 RVA: 0x00486434 File Offset: 0x00484634
		public void Destroy()
		{
			if (this.Status == EPartyVoiceStatus.Ok)
			{
				this.OnPartyVoiceUninitialize();
			}
			this.Status = EPartyVoiceStatus.Uninitialized;
			this.lobbyInterface = null;
			this.audioInterface = null;
			this.rtcInterface = null;
			this.api.ClientApiInitialized -= this.OnClientApiInitialized;
			this.api = null;
			this.owner = null;
		}

		// Token: 0x0600BF03 RID: 48899 RVA: 0x00486490 File Offset: 0x00484690
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientApiInitialized()
		{
			PlatformInterface platformInterface = this.api.PlatformInterface;
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.rtcInterface = ((platformInterface != null) ? platformInterface.GetRTCInterface() : null);
				RTCInterface rtcinterface = this.rtcInterface;
				this.audioInterface = ((rtcinterface != null) ? rtcinterface.GetAudioInterface() : null);
				this.lobbyInterface = ((platformInterface != null) ? platformInterface.GetLobbyInterface() : null);
			}
			lockObject = this.initializedDelegateLock;
			lock (lockObject)
			{
				if (this.rtcInterface != null && this.audioInterface != null && this.lobbyInterface != null)
				{
					this.Status = EPartyVoiceStatus.Ok;
					this.OnPartyVoiceInitialized();
					Log.Out("[EOS-Voice] Successfully initialized.");
					Action action = this.initializedDelegates;
					if (action != null)
					{
						action();
					}
				}
				else
				{
					this.Status = EPartyVoiceStatus.PermanentError;
					Log.Warning("[EOS-Voice] Failed to initialize.");
				}
			}
		}

		// Token: 0x0600BF04 RID: 48900 RVA: 0x004865A0 File Offset: 0x004847A0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPartyVoiceInitialized()
		{
			this.AddNotifications();
		}

		// Token: 0x0600BF05 RID: 48901 RVA: 0x004865A8 File Offset: 0x004847A8
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPartyVoiceUninitialize()
		{
			this.RemoveNotifications();
		}

		// Token: 0x0600BF06 RID: 48902 RVA: 0x004865B0 File Offset: 0x004847B0
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddNotifications()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not add notifications because voice is currently not ready.");
				return;
			}
			this.AddAudioDevicesNotifications();
		}

		// Token: 0x0600BF07 RID: 48903 RVA: 0x004865CB File Offset: 0x004847CB
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveNotifications()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not remove notifications because voice is currently not ready.");
				return;
			}
			this.RemoveAudioDevicesNotifications();
		}

		// Token: 0x0600BF08 RID: 48904 RVA: 0x004865E8 File Offset: 0x004847E8
		public void CreateLobby(Action<string> _lobbyCreatedCallback)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not create lobby because voice is currently not valid.");
				return;
			}
			if (this.lobbyId != null)
			{
				Log.Error("[EOS-Voice] Can not create lobby while already in another");
				Log.Error(StackTraceUtility.ExtractStackTrace());
				return;
			}
			this.createInProgress = true;
			Log.Out("[EOS-Voice] Creating lobby");
			EosHelpers.AssertMainThread("Voice.Create");
			CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
			{
				LocalUserId = this.localProductUserId,
				MaxLobbyMembers = 8U,
				PermissionLevel = LobbyPermissionLevel.Joinviapresence,
				PresenceEnabled = false,
				AllowInvites = false,
				EnableRTCRoom = true,
				BucketId = "PartyVoice",
				LocalRTCOptions = new LocalRTCOptions?(new LocalRTCOptions
				{
					LocalAudioDeviceInputStartsMuted = true
				})
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.CreateLobby(ref createLobbyOptions, _lobbyCreatedCallback, delegate(ref CreateLobbyCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Create lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						this.createInProgress = false;
						Action<string> action = (Action<string>)_callbackData.ClientData;
						if (action == null)
						{
							return;
						}
						action(null);
						return;
					}
					else
					{
						this.lobbyEntered(_callbackData.LobbyId);
						Action<string> action2 = (Action<string>)_callbackData.ClientData;
						if (action2 == null)
						{
							return;
						}
						action2(this.lobbyId);
						return;
					}
				});
			}
		}

		// Token: 0x0600BF09 RID: 48905 RVA: 0x004866F8 File Offset: 0x004848F8
		public void JoinLobby(string _lobbyId)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not join lobby because voice is currently not ready.");
				return;
			}
			if (string.IsNullOrEmpty(_lobbyId))
			{
				Log.Error("[EOS-Voice] Can not join lobby, missing id");
				return;
			}
			if (this.lobbyId != null)
			{
				if (this.lobbyId != _lobbyId)
				{
					Log.Error("[EOS-Voice] Can not join lobby while already in another");
				}
				return;
			}
			Log.Out("[EOS-Voice] Joining lobby");
			this.joinInProgress = true;
			ThreadManager.StartCoroutine(this.tryJoinLobbyCo(_lobbyId));
		}

		// Token: 0x0600BF0A RID: 48906 RVA: 0x0048676C File Offset: 0x0048496C
		public void LeaveLobby()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not leave lobby because voice is currently not ready.");
				return;
			}
			if (this.lobbyId == null)
			{
				return;
			}
			if (this.leaveInProgress)
			{
				return;
			}
			this.leaveInProgress = true;
			Log.Out("[EOS-Voice] Leaving lobby");
			EosHelpers.AssertMainThread("Voice.Leave");
			LeaveLobbyOptions leaveLobbyOptions = new LeaveLobbyOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.LeaveLobby(ref leaveLobbyOptions, null, delegate(ref LeaveLobbyCallbackInfo _callbackData)
				{
					this.lobbyLeft();
					this.leaveInProgress = false;
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Leave lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
					}
				});
			}
		}

		// Token: 0x0600BF0B RID: 48907 RVA: 0x00486828 File Offset: 0x00484A28
		public void PromoteLeader(PlatformUserIdentifierAbs _newLeaderIdentifier)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not promote leader because voice is currently not ready.");
				return;
			}
			if (!this.IsLobbyOwner())
			{
				return;
			}
			UserIdentifierEos userIdentifierEos = _newLeaderIdentifier as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				Log.Error(string.Format("[EOS-Voice] New leader user identifier is not an EOS identifier: {0}", _newLeaderIdentifier));
				return;
			}
			Log.Out("[EOS-Voice] Promoting lobby owner");
			EosHelpers.AssertMainThread("Voice.Prom");
			PromoteMemberOptions promoteMemberOptions = new PromoteMemberOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId,
				TargetUserId = userIdentifierEos.ProductUserId
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.PromoteMember(ref promoteMemberOptions, null, delegate(ref PromoteMemberCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Promoting leader failed: " + _callbackData.ResultCode.ToStringCached<Result>());
					}
				});
			}
		}

		// Token: 0x0600BF0C RID: 48908 RVA: 0x00486914 File Offset: 0x00484B14
		public bool IsLobbyOwner()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not check if lobby owner because voice is currently not ready.");
				return false;
			}
			if (this.lobbyId == null)
			{
				return false;
			}
			EosHelpers.AssertMainThread("Voice.Own");
			CopyLobbyDetailsHandleOptions copyLobbyDetailsHandleOptions = new CopyLobbyDetailsHandleOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId
			};
			object lockObject = AntiCheatCommon.LockObject;
			LobbyDetails lobbyDetails;
			Result result;
			lock (lockObject)
			{
				result = this.lobbyInterface.CopyLobbyDetailsHandle(ref copyLobbyDetailsHandleOptions, out lobbyDetails);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-Voice] Getting local lobby details failed: " + result.ToStringCached<Result>());
				return false;
			}
			LobbyDetailsGetLobbyOwnerOptions lobbyDetailsGetLobbyOwnerOptions = default(LobbyDetailsGetLobbyOwnerOptions);
			lockObject = AntiCheatCommon.LockObject;
			ProductUserId lobbyOwner;
			lock (lockObject)
			{
				lobbyOwner = lobbyDetails.GetLobbyOwner(ref lobbyDetailsGetLobbyOwnerOptions);
			}
			bool result2 = lobbyOwner == this.localProductUserId;
			lobbyDetails.Release();
			return result2;
		}

		// Token: 0x1400012E RID: 302
		// (add) Token: 0x0600BF0D RID: 48909 RVA: 0x00486A20 File Offset: 0x00484C20
		// (remove) Token: 0x0600BF0E RID: 48910 RVA: 0x00486A58 File Offset: 0x00484C58
		public event Action<IPartyVoice.EVoiceChannelAction> OnLocalPlayerStateChanged;

		// Token: 0x1400012F RID: 303
		// (add) Token: 0x0600BF0F RID: 48911 RVA: 0x00486A90 File Offset: 0x00484C90
		// (remove) Token: 0x0600BF10 RID: 48912 RVA: 0x00486AC8 File Offset: 0x00484CC8
		public event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceChannelAction> OnRemotePlayerStateChanged;

		// Token: 0x14000130 RID: 304
		// (add) Token: 0x0600BF11 RID: 48913 RVA: 0x00486B00 File Offset: 0x00484D00
		// (remove) Token: 0x0600BF12 RID: 48914 RVA: 0x00486B38 File Offset: 0x00484D38
		public event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> OnRemotePlayerVoiceStateChanged;

		// Token: 0x0600BF13 RID: 48915 RVA: 0x00486B70 File Offset: 0x00484D70
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddAudioDevicesNotifications()
		{
			if (this.audioDevicesChangedNotificationId == 0UL)
			{
				this.RefreshAudioDevices();
				AddNotifyAudioDevicesChangedOptions addNotifyAudioDevicesChangedOptions = default(AddNotifyAudioDevicesChangedOptions);
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioDevicesChangedNotificationId = this.audioInterface.AddNotifyAudioDevicesChanged(ref addNotifyAudioDevicesChangedOptions, null, new OnAudioDevicesChangedCallback(this.<AddAudioDevicesNotifications>g__OnAudioDevicesChanged|60_0));
				}
			}
		}

		// Token: 0x0600BF14 RID: 48916 RVA: 0x00486BE0 File Offset: 0x00484DE0
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveAudioDevicesNotifications()
		{
			if (this.audioDevicesChangedNotificationId != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					RTCAudioInterface rtcaudioInterface = this.audioInterface;
					if (rtcaudioInterface != null)
					{
						rtcaudioInterface.RemoveNotifyAudioDevicesChanged(this.audioDevicesChangedNotificationId);
					}
				}
				this.audioDevicesChangedNotificationId = 0UL;
			}
		}

		// Token: 0x0600BF15 RID: 48917 RVA: 0x00486C40 File Offset: 0x00484E40
		[PublicizedFrom(EAccessModifier.Private)]
		public void RefreshAudioDevices()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not refresh audio devices because voice is currently not ready.");
				return;
			}
			QueryInputDevicesInformationOptions queryInputDevicesInformationOptions = default(QueryInputDevicesInformationOptions);
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.QueryInputDevicesInformation(ref queryInputDevicesInformationOptions, null, new OnQueryInputDevicesInformationCallback(this.OnQueryInputDevicesInformation));
			}
			QueryOutputDevicesInformationOptions queryOutputDevicesInformationOptions = default(QueryOutputDevicesInformationOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.QueryOutputDevicesInformation(ref queryOutputDevicesInformationOptions, null, new OnQueryOutputDevicesInformationCallback(this.OnQueryOutputDevicesInformation));
			}
		}

		// Token: 0x0600BF16 RID: 48918 RVA: 0x00486CF8 File Offset: 0x00484EF8
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnQueryInputDevicesInformation(ref OnQueryInputDevicesInformationCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Log.Error("[EOS-Voice] Query Input Devices Information Failed: " + data.ResultCode.ToStringCached<Result>());
				return;
			}
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Warning(string.Format("[EOS-Voice] can not query input devices information. Voice status: {0}", this.Status));
				return;
			}
			GetInputDevicesCountOptions getInputDevicesCountOptions = default(GetInputDevicesCountOptions);
			object lockObject = AntiCheatCommon.LockObject;
			uint inputDevicesCount;
			lock (lockObject)
			{
				inputDevicesCount = this.audioInterface.GetInputDevicesCount(ref getInputDevicesCountOptions);
			}
			IList<IPartyVoice.VoiceAudioDevice> list = new List<IPartyVoice.VoiceAudioDevice>();
			for (uint num = 0U; num < inputDevicesCount; num += 1U)
			{
				CopyInputDeviceInformationByIndexOptions copyInputDeviceInformationByIndexOptions = new CopyInputDeviceInformationByIndexOptions
				{
					DeviceIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				InputDeviceInformation? inputDeviceInformation;
				Result result;
				lock (lockObject)
				{
					result = this.audioInterface.CopyInputDeviceInformationByIndex(ref copyInputDeviceInformationByIndexOptions, out inputDeviceInformation);
				}
				try
				{
					if (result != Result.Success || inputDeviceInformation == null)
					{
						Log.Warning(string.Format("[EOS-Voice] Could not query input device {0}: {1}", num, result.ToStringCached<Result>()));
					}
					else
					{
						list.Add(new Voice.EosAudioDevice(inputDeviceInformation.Value));
					}
				}
				finally
				{
					bool flag2 = inputDeviceInformation != null;
				}
			}
			this.inputDevices = list;
		}

		// Token: 0x0600BF17 RID: 48919 RVA: 0x00486E58 File Offset: 0x00485058
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnQueryOutputDevicesInformation(ref OnQueryOutputDevicesInformationCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Log.Error("[EOS-Voice] Query Output Devices Information Failed: " + data.ResultCode.ToStringCached<Result>());
				return;
			}
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Warning(string.Format("[EOS-Voice] can not query output devices information. Voice status: {0}", this.Status));
				return;
			}
			IList<IPartyVoice.VoiceAudioDevice> list = new List<IPartyVoice.VoiceAudioDevice>();
			GetOutputDevicesCountOptions getOutputDevicesCountOptions = default(GetOutputDevicesCountOptions);
			object lockObject = AntiCheatCommon.LockObject;
			uint outputDevicesCount;
			lock (lockObject)
			{
				outputDevicesCount = this.audioInterface.GetOutputDevicesCount(ref getOutputDevicesCountOptions);
			}
			for (uint num = 0U; num < outputDevicesCount; num += 1U)
			{
				CopyOutputDeviceInformationByIndexOptions copyOutputDeviceInformationByIndexOptions = new CopyOutputDeviceInformationByIndexOptions
				{
					DeviceIndex = num
				};
				lockObject = AntiCheatCommon.LockObject;
				OutputDeviceInformation? outputDeviceInformation;
				Result result;
				lock (lockObject)
				{
					result = this.audioInterface.CopyOutputDeviceInformationByIndex(ref copyOutputDeviceInformationByIndexOptions, out outputDeviceInformation);
				}
				try
				{
					if (result != Result.Success || outputDeviceInformation == null)
					{
						Log.Warning(string.Format("[EOS-Voice] Could not query output device {0}: {1}", num, result.ToStringCached<Result>()));
					}
					else
					{
						list.Add(new Voice.EosAudioDevice(outputDeviceInformation.Value));
					}
				}
				finally
				{
					bool flag2 = outputDeviceInformation != null;
				}
			}
			this.outputDevices = list;
		}

		// Token: 0x0600BF18 RID: 48920 RVA: 0x00486FB8 File Offset: 0x004851B8
		[return: TupleElementNames(new string[]
		{
			"inputDevices",
			"outputDevices"
		})]
		public ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>> GetDevicesList()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not get voice devices because voice is currently not ready.");
				return new ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>>(Array.Empty<IPartyVoice.VoiceAudioDevice>(), Array.Empty<IPartyVoice.VoiceAudioDevice>());
			}
			EosHelpers.AssertMainThread("Voice.GetDev");
			return new ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>>(this.inputDevices, this.outputDevices);
		}

		// Token: 0x0600BF19 RID: 48921 RVA: 0x00486FF8 File Offset: 0x004851F8
		public void SetInputDevice(string _device)
		{
			Voice.<>c__DisplayClass66_0 CS$<>8__locals1 = new Voice.<>c__DisplayClass66_0();
			CS$<>8__locals1._device = _device;
			CS$<>8__locals1.<>4__this = this;
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not set input device because voice is currently not ready.");
				return;
			}
			EosHelpers.AssertMainThread("Voice.SetIn");
			SetInputDeviceSettingsOptions setInputDeviceSettingsOptions = new SetInputDeviceSettingsOptions
			{
				LocalUserId = this.localProductUserId,
				RealDeviceId = CS$<>8__locals1._device,
				PlatformAEC = true
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.SetInputDeviceSettings(ref setInputDeviceSettingsOptions, null, new OnSetInputDeviceSettingsCallback(CS$<>8__locals1.<SetInputDevice>g__OnSetInputDeviceSettings|0));
			}
		}

		// Token: 0x0600BF1A RID: 48922 RVA: 0x004870B0 File Offset: 0x004852B0
		public void SetOutputDevice(string _device)
		{
			Voice.<>c__DisplayClass67_0 CS$<>8__locals1 = new Voice.<>c__DisplayClass67_0();
			CS$<>8__locals1._device = _device;
			CS$<>8__locals1.<>4__this = this;
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not set output device because voice is currently not ready.");
				return;
			}
			EosHelpers.AssertMainThread("Voice.SetOut");
			SetOutputDeviceSettingsOptions setOutputDeviceSettingsOptions = new SetOutputDeviceSettingsOptions
			{
				LocalUserId = this.localProductUserId,
				RealDeviceId = CS$<>8__locals1._device
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.SetOutputDeviceSettings(ref setOutputDeviceSettingsOptions, null, new OnSetOutputDeviceSettingsCallback(CS$<>8__locals1.<SetOutputDevice>g__OnSetOutputDeviceSettings|0));
			}
		}

		// Token: 0x170015ED RID: 5613
		// (get) Token: 0x0600BF1B RID: 48923 RVA: 0x00487160 File Offset: 0x00485360
		// (set) Token: 0x0600BF1C RID: 48924 RVA: 0x00487168 File Offset: 0x00485368
		public bool MuteSelf
		{
			get
			{
				return this.muteSelf;
			}
			set
			{
				if (this.Status != EPartyVoiceStatus.Ok)
				{
					Log.Error("[EOS-Voice] Can not mute self because voice is currently not ready.");
					return;
				}
				if (value == this.muteSelf)
				{
					return;
				}
				this.muteSelf = value;
				if (this.roomName == null)
				{
					return;
				}
				EosHelpers.AssertMainThread("Voice.Mute");
				UpdateSendingOptions updateSendingOptions = new UpdateSendingOptions
				{
					LocalUserId = this.localProductUserId,
					RoomName = this.roomName,
					AudioStatus = (value ? RTCAudioStatus.Disabled : RTCAudioStatus.Enabled)
				};
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioInterface.UpdateSending(ref updateSendingOptions, null, delegate(ref UpdateSendingCallbackInfo _callbackData)
					{
						if (_callbackData.ResultCode != Result.Success)
						{
							Log.Error("[EOS-Voice] Toggling voice sending state failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						}
					});
				}
			}
		}

		// Token: 0x170015EE RID: 5614
		// (get) Token: 0x0600BF1D RID: 48925 RVA: 0x0048723C File Offset: 0x0048543C
		// (set) Token: 0x0600BF1E RID: 48926 RVA: 0x00487244 File Offset: 0x00485444
		public bool MuteOthers
		{
			get
			{
				return this.muteOthers;
			}
			set
			{
				if (this.Status != EPartyVoiceStatus.Ok)
				{
					Log.Error("[EOS-Voice] Can not mute others because voice is currently not ready.");
					return;
				}
				if (value == this.muteOthers)
				{
					return;
				}
				this.muteOthers = value;
				if (this.roomName == null)
				{
					return;
				}
				EosHelpers.AssertMainThread("Voice.MuteOth");
				UpdateReceivingOptions updateReceivingOptions = new UpdateReceivingOptions
				{
					LocalUserId = this.localProductUserId,
					RoomName = this.roomName,
					AudioEnabled = !value
				};
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioInterface.UpdateReceiving(ref updateReceivingOptions, null, delegate(ref UpdateReceivingCallbackInfo _callbackData)
					{
						if (_callbackData.ResultCode != Result.Success)
						{
							Log.Error("[EOS-Voice] Toggling voice receiving state failed: " + _callbackData.ResultCode.ToStringCached<Result>());
							return;
						}
						Log.Out(string.Format("[EOS-Voice] Mute all state changed to: {0}", value));
					});
				}
			}
		}

		// Token: 0x170015EF RID: 5615
		// (get) Token: 0x0600BF1F RID: 48927 RVA: 0x00487320 File Offset: 0x00485520
		// (set) Token: 0x0600BF20 RID: 48928 RVA: 0x00487328 File Offset: 0x00485528
		public float OutputVolume
		{
			get
			{
				return this.outputVolume;
			}
			set
			{
				if (this.Status != EPartyVoiceStatus.Ok)
				{
					Log.Error("[EOS-Voice] Can not set output volume because voice is currently not ready.");
					return;
				}
				if ((double)value > (double)this.outputVolume - 0.01 && (double)value < (double)this.outputVolume + 0.01)
				{
					return;
				}
				this.outputVolume = value;
				this.SetRoomReceivingVolume(this.outputVolume);
			}
		}

		// Token: 0x0600BF21 RID: 48929 RVA: 0x00487388 File Offset: 0x00485588
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetRoomReceivingVolume(float platformVolume)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not set room receiving volume because voice is currently not ready.");
				return;
			}
			if (this.roomName == null)
			{
				return;
			}
			EosHelpers.AssertMainThread("Voice.Vol");
			UpdateReceivingVolumeOptions updateReceivingVolumeOptions = new UpdateReceivingVolumeOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName,
				Volume = platformVolume * 50f
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.audioInterface.UpdateReceivingVolume(ref updateReceivingVolumeOptions, null, new OnUpdateReceivingVolumeCallback(Voice.<SetRoomReceivingVolume>g__OnUpdateReceivingVolume|80_0));
			}
		}

		// Token: 0x0600BF22 RID: 48930 RVA: 0x00487438 File Offset: 0x00485638
		public void BlockUser(PlatformUserIdentifierAbs _userIdentifier, bool _block)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				Log.Error("[EOS-Voice] Can not block user because voice is currently not ready.");
				return;
			}
			if (this.roomName == null)
			{
				return;
			}
			if (this.muteOthers)
			{
				return;
			}
			UserIdentifierEos userIdentifierEos = _userIdentifier as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				Log.Error(string.Format("[EOS-Voice] Block user identifier is not an EOS identifier: {0}", _userIdentifier));
				return;
			}
			Log.Out(string.Format("[EOS-Voice] Blocking user: {0} = {1}", userIdentifierEos, _block));
			EosHelpers.AssertMainThread("Voice.Block");
			BlockParticipantOptions blockParticipantOptions = new BlockParticipantOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName,
				ParticipantId = userIdentifierEos.ProductUserId,
				Blocked = _block
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.rtcInterface.BlockParticipant(ref blockParticipantOptions, null, delegate(ref BlockParticipantCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Blocking user failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						return;
					}
					if (_block)
					{
						this.blockedUsers.Add(_userIdentifier);
						Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged = this.OnRemotePlayerVoiceStateChanged;
						if (onRemotePlayerVoiceStateChanged == null)
						{
							return;
						}
						onRemotePlayerVoiceStateChanged(_userIdentifier, IPartyVoice.EVoiceMemberState.Muted);
						return;
					}
					else
					{
						this.blockedUsers.Remove(_userIdentifier);
						Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged2 = this.OnRemotePlayerVoiceStateChanged;
						if (onRemotePlayerVoiceStateChanged2 == null)
						{
							return;
						}
						onRemotePlayerVoiceStateChanged2(_userIdentifier, IPartyVoice.EVoiceMemberState.Normal);
						return;
					}
				});
			}
		}

		// Token: 0x0600BF23 RID: 48931 RVA: 0x00487560 File Offset: 0x00485760
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator tryJoinLobbyCo(string _lobbyId)
		{
			Voice.<>c__DisplayClass83_0 CS$<>8__locals1 = new Voice.<>c__DisplayClass83_0();
			CS$<>8__locals1.<>4__this = this;
			EosHelpers.AssertMainThread("Voice.Join");
			int attempts = 0;
			CS$<>8__locals1.lobbyIdFound = null;
			CS$<>8__locals1.lobbyDetails = null;
			object lockObject;
			while (attempts < 5)
			{
				Voice.<>c__DisplayClass83_1 CS$<>8__locals2 = new Voice.<>c__DisplayClass83_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				int num = attempts;
				attempts = num + 1;
				Log.Out(string.Format("[EOS-Voice] Trying to find lobby for id {0}, attempt {1}", _lobbyId, attempts));
				CreateLobbySearchOptions createLobbySearchOptions = new CreateLobbySearchOptions
				{
					MaxResults = 10U
				};
				lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.lobbyInterface.CreateLobbySearch(ref createLobbySearchOptions, out CS$<>8__locals2.lobbySearch);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-Voice] Create lobby search failed: " + result.ToStringCached<Result>());
					this.joinInProgress = false;
					yield break;
				}
				LobbySearchSetLobbyIdOptions lobbySearchSetLobbyIdOptions = new LobbySearchSetLobbyIdOptions
				{
					LobbyId = _lobbyId
				};
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					result = CS$<>8__locals2.lobbySearch.SetLobbyId(ref lobbySearchSetLobbyIdOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-Voice] Set lobby search lobbyid failed: " + result.ToStringCached<Result>());
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						CS$<>8__locals2.lobbySearch.Release();
					}
					this.joinInProgress = false;
					yield break;
				}
				CS$<>8__locals2.findDone = false;
				CS$<>8__locals2.findError = false;
				CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
				LobbySearchFindOptions lobbySearchFindOptions = new LobbySearchFindOptions
				{
					LocalUserId = this.localProductUserId
				};
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					CS$<>8__locals2.lobbySearch.Find(ref lobbySearchFindOptions, null, delegate(ref LobbySearchFindCallbackInfo _callbackData)
					{
						object lockObject2;
						if (_callbackData.ResultCode == Result.NotFound)
						{
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							return;
						}
						if (_callbackData.ResultCode != Result.Success)
						{
							Log.Error("[EOS-Voice] Find lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							CS$<>8__locals2.findError = true;
							return;
						}
						LobbySearchGetSearchResultCountOptions lobbySearchGetSearchResultCountOptions = default(LobbySearchGetSearchResultCountOptions);
						lockObject2 = AntiCheatCommon.LockObject;
						uint searchResultCount;
						lock (lockObject2)
						{
							searchResultCount = CS$<>8__locals2.lobbySearch.GetSearchResultCount(ref lobbySearchGetSearchResultCountOptions);
						}
						if (searchResultCount != 1U)
						{
							Log.Error(string.Format("[EOS-Voice] Find lobby returned unexpected number of results ({0})", searchResultCount));
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							return;
						}
						LobbySearchCopySearchResultByIndexOptions lobbySearchCopySearchResultByIndexOptions = new LobbySearchCopySearchResultByIndexOptions
						{
							LobbyIndex = 0U
						};
						lockObject2 = AntiCheatCommon.LockObject;
						Result result2;
						lock (lockObject2)
						{
							result2 = CS$<>8__locals2.lobbySearch.CopySearchResultByIndex(ref lobbySearchCopySearchResultByIndexOptions, out CS$<>8__locals2.CS$<>8__locals1.lobbyDetails);
						}
						if (result2 != Result.Success)
						{
							Log.Error("[EOS-Voice] Get lobby details failed: " + result2.ToStringCached<Result>());
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
							CS$<>8__locals2.findDone = true;
							CS$<>8__locals2.findError = true;
							return;
						}
						LobbyDetailsCopyInfoOptions lobbyDetailsCopyInfoOptions = default(LobbyDetailsCopyInfoOptions);
						lockObject2 = AntiCheatCommon.LockObject;
						LobbyDetailsInfo? lobbyDetailsInfo;
						lock (lockObject2)
						{
							result2 = CS$<>8__locals2.CS$<>8__locals1.lobbyDetails.CopyInfo(ref lobbyDetailsCopyInfoOptions, out lobbyDetailsInfo);
						}
						if (result2 != Result.Success)
						{
							Log.Error("[EOS-Voice] Get lobby details info failed: " + result2.ToStringCached<Result>());
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.CS$<>8__locals1.lobbyDetails.Release();
							}
							CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
							lockObject2 = AntiCheatCommon.LockObject;
							lock (lockObject2)
							{
								CS$<>8__locals2.lobbySearch.Release();
							}
							CS$<>8__locals2.findDone = true;
							CS$<>8__locals2.findError = true;
							return;
						}
						CS$<>8__locals2.CS$<>8__locals1.lobbyIdFound = lobbyDetailsInfo.Value.LobbyId;
						Log.Out("[EOS-Voice] Found lobby: " + CS$<>8__locals2.CS$<>8__locals1.lobbyIdFound);
						CS$<>8__locals2.findDone = true;
						lockObject2 = AntiCheatCommon.LockObject;
						lock (lockObject2)
						{
							CS$<>8__locals2.lobbySearch.Release();
						}
					});
					goto IL_278;
				}
				goto IL_261;
				IL_278:
				if (CS$<>8__locals2.findDone)
				{
					if (CS$<>8__locals2.findError)
					{
						CS$<>8__locals2.CS$<>8__locals1.lobbyDetails = null;
						Log.Error("[EOS-Voice] Failed joining voice lobby");
						this.joinInProgress = false;
						yield break;
					}
					if (CS$<>8__locals2.CS$<>8__locals1.lobbyIdFound == null)
					{
						yield return new WaitForSeconds(0.5f);
						CS$<>8__locals2 = null;
						continue;
					}
					break;
				}
				IL_261:
				yield return null;
				goto IL_278;
			}
			if (CS$<>8__locals1.lobbyDetails == null)
			{
				Log.Error("[EOS-Voice] Did not find lobby");
				this.joinInProgress = false;
				yield break;
			}
			Log.Out(string.Format("[EOS-Voice] Found lobby on {0} attempt", attempts));
			JoinLobbyOptions joinLobbyOptions = new JoinLobbyOptions
			{
				LocalUserId = this.localProductUserId,
				PresenceEnabled = false,
				LobbyDetailsHandle = CS$<>8__locals1.lobbyDetails,
				LocalRTCOptions = new LocalRTCOptions?(new LocalRTCOptions
				{
					LocalAudioDeviceInputStartsMuted = true
				})
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.lobbyInterface.JoinLobby(ref joinLobbyOptions, null, delegate(ref JoinLobbyCallbackInfo _callbackData)
				{
					object lockObject2;
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Voice] Join lobby failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						lockObject2 = AntiCheatCommon.LockObject;
						lock (lockObject2)
						{
							CS$<>8__locals1.lobbyDetails.Release();
						}
						CS$<>8__locals1.<>4__this.joinInProgress = false;
						return;
					}
					CS$<>8__locals1.<>4__this.lobbyEntered(CS$<>8__locals1.lobbyIdFound);
					lockObject2 = AntiCheatCommon.LockObject;
					lock (lockObject2)
					{
						CS$<>8__locals1.lobbyDetails.Release();
					}
				});
				yield break;
			}
			yield break;
		}

		// Token: 0x0600BF24 RID: 48932 RVA: 0x00487578 File Offset: 0x00485778
		[PublicizedFrom(EAccessModifier.Private)]
		public void lobbyEntered(string _lobbyId)
		{
			this.blockedUsers.Clear();
			this.lobbyId = _lobbyId;
			GetRTCRoomNameOptions getRTCRoomNameOptions = new GetRTCRoomNameOptions
			{
				LocalUserId = this.localProductUserId,
				LobbyId = this.lobbyId
			};
			object lockObject = AntiCheatCommon.LockObject;
			Utf8String other;
			Result rtcroomName;
			lock (lockObject)
			{
				rtcroomName = this.lobbyInterface.GetRTCRoomName(ref getRTCRoomNameOptions, out other);
			}
			if (rtcroomName != Result.Success)
			{
				Log.Error("[EOS-Voice] Getting local lobby room name failed: " + rtcroomName.ToStringCached<Result>());
			}
			this.roomName = other;
			AddNotifyParticipantStatusChangedOptions addNotifyParticipantStatusChangedOptions = new AddNotifyParticipantStatusChangedOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.participantStatusChangedHandle = this.rtcInterface.AddNotifyParticipantStatusChanged(ref addNotifyParticipantStatusChangedOptions, null, new OnParticipantStatusChangedCallback(this.participantStatusChanged));
			}
			AddNotifyParticipantUpdatedOptions addNotifyParticipantUpdatedOptions = new AddNotifyParticipantUpdatedOptions
			{
				LocalUserId = this.localProductUserId,
				RoomName = this.roomName
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.participantUpdatedHandle = this.audioInterface.AddNotifyParticipantUpdated(ref addNotifyParticipantUpdatedOptions, null, new OnParticipantUpdatedCallback(this.participantVoiceChanged));
			}
			this.SetRoomReceivingVolume(this.outputVolume);
		}

		// Token: 0x0600BF25 RID: 48933 RVA: 0x00487724 File Offset: 0x00485924
		[PublicizedFrom(EAccessModifier.Private)]
		public void participantVoiceChanged(ref ParticipantUpdatedCallbackInfo _data)
		{
			if (Api.DebugLevel == Api.EDebugLevel.Verbose)
			{
				Log.Out(string.Format("[EOS-Voice] Participant update: {0}, speaking={1}, audio={2}", _data.ParticipantId, _data.Speaking, _data.AudioStatus));
			}
			UserIdentifierEos userIdentifierEos = new UserIdentifierEos(_data.ParticipantId);
			if (userIdentifierEos.Equals(this.localUserIdentifier))
			{
				return;
			}
			if (!this.blockedUsers.Contains(userIdentifierEos))
			{
				Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged = this.OnRemotePlayerVoiceStateChanged;
				if (onRemotePlayerVoiceStateChanged != null)
				{
					PlatformUserIdentifierAbs arg = userIdentifierEos;
					IPartyVoice.EVoiceMemberState arg2;
					switch (_data.AudioStatus)
					{
					case RTCAudioStatus.Unsupported:
						arg2 = IPartyVoice.EVoiceMemberState.Disabled;
						break;
					case RTCAudioStatus.Enabled:
						arg2 = IPartyVoice.EVoiceMemberState.VoiceActive;
						break;
					case RTCAudioStatus.Disabled:
						arg2 = IPartyVoice.EVoiceMemberState.Normal;
						break;
					case RTCAudioStatus.AdminDisabled:
						arg2 = IPartyVoice.EVoiceMemberState.Muted;
						break;
					case RTCAudioStatus.NotListeningDisabled:
						arg2 = IPartyVoice.EVoiceMemberState.Muted;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					onRemotePlayerVoiceStateChanged(arg, arg2);
				}
				return;
			}
			Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> onRemotePlayerVoiceStateChanged2 = this.OnRemotePlayerVoiceStateChanged;
			if (onRemotePlayerVoiceStateChanged2 == null)
			{
				return;
			}
			onRemotePlayerVoiceStateChanged2(userIdentifierEos, IPartyVoice.EVoiceMemberState.Muted);
		}

		// Token: 0x0600BF26 RID: 48934 RVA: 0x004877F4 File Offset: 0x004859F4
		[PublicizedFrom(EAccessModifier.Private)]
		public void participantStatusChanged(ref ParticipantStatusChangedCallbackInfo _data)
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				return;
			}
			if (Api.DebugLevel == Api.EDebugLevel.Verbose)
			{
				Log.Out(string.Format("[EOS-Voice] Participant state changed: {0}, {1}", _data.ParticipantId, _data.ParticipantStatus));
			}
			UserIdentifierEos userIdentifierEos = new UserIdentifierEos(_data.ParticipantId);
			if (userIdentifierEos.Equals(this.localUserIdentifier))
			{
				this.roomEntered = (_data.ParticipantStatus == RTCParticipantStatus.Joined);
				if (this.roomEntered)
				{
					this.createInProgress = false;
					this.joinInProgress = false;
					this.muteSelf = true;
					this.muteOthers = false;
				}
				Action<IPartyVoice.EVoiceChannelAction> onLocalPlayerStateChanged = this.OnLocalPlayerStateChanged;
				if (onLocalPlayerStateChanged == null)
				{
					return;
				}
				onLocalPlayerStateChanged((_data.ParticipantStatus == RTCParticipantStatus.Joined) ? IPartyVoice.EVoiceChannelAction.Joined : IPartyVoice.EVoiceChannelAction.Left);
				return;
			}
			else
			{
				Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceChannelAction> onRemotePlayerStateChanged = this.OnRemotePlayerStateChanged;
				if (onRemotePlayerStateChanged == null)
				{
					return;
				}
				onRemotePlayerStateChanged(userIdentifierEos, (_data.ParticipantStatus == RTCParticipantStatus.Joined) ? IPartyVoice.EVoiceChannelAction.Joined : IPartyVoice.EVoiceChannelAction.Left);
				return;
			}
		}

		// Token: 0x0600BF27 RID: 48935 RVA: 0x004878BC File Offset: 0x00485ABC
		[PublicizedFrom(EAccessModifier.Private)]
		public void lobbyLeft()
		{
			if (this.Status != EPartyVoiceStatus.Ok)
			{
				return;
			}
			this.lobbyId = null;
			this.roomName = null;
			this.muteSelf = true;
			this.muteOthers = false;
			this.blockedUsers.Clear();
			if (this.participantStatusChangedHandle != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.rtcInterface.RemoveNotifyParticipantStatusChanged(this.participantStatusChangedHandle);
				}
			}
			if (this.participantUpdatedHandle != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.audioInterface.RemoveNotifyParticipantUpdated(this.participantUpdatedHandle);
				}
			}
			this.participantStatusChangedHandle = 0UL;
			this.participantUpdatedHandle = 0UL;
		}

		// Token: 0x0600BF2B RID: 48939 RVA: 0x00487A6A File Offset: 0x00485C6A
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <AddAudioDevicesNotifications>g__OnAudioDevicesChanged|60_0(ref AudioDevicesChangedCallbackInfo data)
		{
			this.RefreshAudioDevices();
		}

		// Token: 0x0600BF2C RID: 48940 RVA: 0x00487A72 File Offset: 0x00485C72
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <SetRoomReceivingVolume>g__OnUpdateReceivingVolume|80_0(ref UpdateReceivingVolumeCallbackInfo data)
		{
			if (data.ResultCode != Result.Success)
			{
				Log.Error(string.Format("[EOS-Voice] Setting voice output volume for room '{0}' failed: {1}", data.RoomName, data.ResultCode.ToStringCached<Result>()));
			}
		}

		// Token: 0x040094A5 RID: 38053
		[PublicizedFrom(EAccessModifier.Private)]
		public const int voiceLobbyConnectAttempts = 5;

		// Token: 0x040094A6 RID: 38054
		[PublicizedFrom(EAccessModifier.Private)]
		public const float voiceLobbyConnectAttemptInterval = 0.5f;

		// Token: 0x040094A7 RID: 38055
		[PublicizedFrom(EAccessModifier.Private)]
		public const float platformVolumeToEosRtcVolume = 50f;

		// Token: 0x040094A8 RID: 38056
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x040094A9 RID: 38057
		[PublicizedFrom(EAccessModifier.Private)]
		public Api api;

		// Token: 0x040094AA RID: 38058
		[PublicizedFrom(EAccessModifier.Private)]
		public RTCInterface rtcInterface;

		// Token: 0x040094AB RID: 38059
		[PublicizedFrom(EAccessModifier.Private)]
		public RTCAudioInterface audioInterface;

		// Token: 0x040094AC RID: 38060
		[PublicizedFrom(EAccessModifier.Private)]
		public LobbyInterface lobbyInterface;

		// Token: 0x040094AD RID: 38061
		[PublicizedFrom(EAccessModifier.Private)]
		public string lobbyId;

		// Token: 0x040094AE RID: 38062
		[PublicizedFrom(EAccessModifier.Private)]
		public bool createInProgress;

		// Token: 0x040094AF RID: 38063
		[PublicizedFrom(EAccessModifier.Private)]
		public bool joinInProgress;

		// Token: 0x040094B0 RID: 38064
		[PublicizedFrom(EAccessModifier.Private)]
		public bool leaveInProgress;

		// Token: 0x040094B1 RID: 38065
		[PublicizedFrom(EAccessModifier.Private)]
		public string roomName;

		// Token: 0x040094B2 RID: 38066
		[PublicizedFrom(EAccessModifier.Private)]
		public bool roomEntered;

		// Token: 0x040094B3 RID: 38067
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong participantStatusChangedHandle;

		// Token: 0x040094B4 RID: 38068
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong participantUpdatedHandle;

		// Token: 0x040094B5 RID: 38069
		[PublicizedFrom(EAccessModifier.Private)]
		public Action initializedDelegates;

		// Token: 0x040094B6 RID: 38070
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object initializedDelegateLock = new object();

		// Token: 0x040094BB RID: 38075
		[PublicizedFrom(EAccessModifier.Private)]
		public string activeInputDeviceId;

		// Token: 0x040094BC RID: 38076
		[PublicizedFrom(EAccessModifier.Private)]
		public string activeOutputDeviceId;

		// Token: 0x040094BD RID: 38077
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong audioDevicesChangedNotificationId;

		// Token: 0x040094BE RID: 38078
		[PublicizedFrom(EAccessModifier.Private)]
		public IList<IPartyVoice.VoiceAudioDevice> inputDevices;

		// Token: 0x040094BF RID: 38079
		[PublicizedFrom(EAccessModifier.Private)]
		public IList<IPartyVoice.VoiceAudioDevice> outputDevices;

		// Token: 0x040094C0 RID: 38080
		[PublicizedFrom(EAccessModifier.Private)]
		public bool muteSelf;

		// Token: 0x040094C1 RID: 38081
		[PublicizedFrom(EAccessModifier.Private)]
		public bool muteOthers;

		// Token: 0x040094C2 RID: 38082
		[PublicizedFrom(EAccessModifier.Private)]
		public float outputVolume = 1f;

		// Token: 0x040094C3 RID: 38083
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<PlatformUserIdentifierAbs> blockedUsers = new HashSet<PlatformUserIdentifierAbs>();

		// Token: 0x0200194F RID: 6479
		public class EosAudioDevice : IPartyVoice.VoiceAudioDevice
		{
			// Token: 0x0600BF2D RID: 48941 RVA: 0x00487A9C File Offset: 0x00485C9C
			public EosAudioDevice(InputDeviceInformation _device) : base(false, _device.DefaultDevice)
			{
				this.Id = _device.DeviceId;
				this.Name = _device.DeviceName;
				this.LogDevice("Input");
			}

			// Token: 0x0600BF2E RID: 48942 RVA: 0x00487ADB File Offset: 0x00485CDB
			public EosAudioDevice(OutputDeviceInformation _device) : base(true, _device.DefaultDevice)
			{
				this.Id = _device.DeviceId;
				this.Name = _device.DeviceName;
				this.LogDevice("Output");
			}

			// Token: 0x0600BF2F RID: 48943 RVA: 0x00487B1C File Offset: 0x00485D1C
			[PublicizedFrom(EAccessModifier.Private)]
			public void LogDevice(string _inOutString)
			{
				if (GameUtils.GetLaunchArgument("debugeos") != null)
				{
					Log.Out(string.Format("[EOS-Voice] {0} device: Id={1}, Name={2}, Default={3}", new object[]
					{
						_inOutString,
						this.Id,
						this.Name,
						this.IsDefault
					}));
				}
			}

			// Token: 0x0600BF30 RID: 48944 RVA: 0x00487B6E File Offset: 0x00485D6E
			public override string ToString()
			{
				if (!this.IsDefault)
				{
					return this.Name;
				}
				return "(Default) " + this.Name;
			}

			// Token: 0x170015F0 RID: 5616
			// (get) Token: 0x0600BF31 RID: 48945 RVA: 0x00487B8F File Offset: 0x00485D8F
			public override string Identifier
			{
				get
				{
					return this.Id;
				}
			}

			// Token: 0x040094C4 RID: 38084
			public readonly string Id;

			// Token: 0x040094C5 RID: 38085
			public readonly string Name;
		}
	}
}
