using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x020017EF RID: 6127
	public interface IPartyVoice
	{
		// Token: 0x1400010D RID: 269
		// (add) Token: 0x0600B6C6 RID: 46790
		// (remove) Token: 0x0600B6C7 RID: 46791
		event Action Initialized;

		// Token: 0x1700149A RID: 5274
		// (get) Token: 0x0600B6C8 RID: 46792
		EPartyVoiceStatus Status { get; }

		// Token: 0x1700149B RID: 5275
		// (get) Token: 0x0600B6C9 RID: 46793
		bool InLobby { get; }

		// Token: 0x1700149C RID: 5276
		// (get) Token: 0x0600B6CA RID: 46794
		bool InLobbyOrProgress { get; }

		// Token: 0x0600B6CB RID: 46795
		void Init(IPlatform _owner);

		// Token: 0x0600B6CC RID: 46796
		void Destroy();

		// Token: 0x0600B6CD RID: 46797
		void CreateLobby(Action<string> _lobbyCreatedCallback);

		// Token: 0x0600B6CE RID: 46798
		void JoinLobby(string _lobbyId);

		// Token: 0x0600B6CF RID: 46799
		void LeaveLobby();

		// Token: 0x0600B6D0 RID: 46800
		void PromoteLeader(PlatformUserIdentifierAbs _newLeaderIdentifier);

		// Token: 0x0600B6D1 RID: 46801
		bool IsLobbyOwner();

		// Token: 0x1400010E RID: 270
		// (add) Token: 0x0600B6D2 RID: 46802
		// (remove) Token: 0x0600B6D3 RID: 46803
		event Action<IPartyVoice.EVoiceChannelAction> OnLocalPlayerStateChanged;

		// Token: 0x1400010F RID: 271
		// (add) Token: 0x0600B6D4 RID: 46804
		// (remove) Token: 0x0600B6D5 RID: 46805
		event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceChannelAction> OnRemotePlayerStateChanged;

		// Token: 0x14000110 RID: 272
		// (add) Token: 0x0600B6D6 RID: 46806
		// (remove) Token: 0x0600B6D7 RID: 46807
		event Action<PlatformUserIdentifierAbs, IPartyVoice.EVoiceMemberState> OnRemotePlayerVoiceStateChanged;

		// Token: 0x0600B6D8 RID: 46808
		[return: TupleElementNames(new string[]
		{
			"inputDevices",
			"outputDevices"
		})]
		ValueTuple<IList<IPartyVoice.VoiceAudioDevice>, IList<IPartyVoice.VoiceAudioDevice>> GetDevicesList();

		// Token: 0x0600B6D9 RID: 46809
		void SetInputDevice(string _device);

		// Token: 0x0600B6DA RID: 46810
		void SetOutputDevice(string _device);

		// Token: 0x1700149D RID: 5277
		// (get) Token: 0x0600B6DB RID: 46811
		// (set) Token: 0x0600B6DC RID: 46812
		bool MuteSelf { get; set; }

		// Token: 0x1700149E RID: 5278
		// (get) Token: 0x0600B6DD RID: 46813
		// (set) Token: 0x0600B6DE RID: 46814
		bool MuteOthers { get; set; }

		// Token: 0x1700149F RID: 5279
		// (get) Token: 0x0600B6DF RID: 46815
		// (set) Token: 0x0600B6E0 RID: 46816
		float OutputVolume { get; set; }

		// Token: 0x0600B6E1 RID: 46817
		void BlockUser(PlatformUserIdentifierAbs _userIdentifier, bool _block);

		// Token: 0x020017F0 RID: 6128
		public enum EVoiceMemberState
		{
			// Token: 0x04008FAE RID: 36782
			Disabled,
			// Token: 0x04008FAF RID: 36783
			Normal,
			// Token: 0x04008FB0 RID: 36784
			Muted,
			// Token: 0x04008FB1 RID: 36785
			VoiceActive
		}

		// Token: 0x020017F1 RID: 6129
		public enum EVoiceChannelAction
		{
			// Token: 0x04008FB3 RID: 36787
			Joined,
			// Token: 0x04008FB4 RID: 36788
			Left
		}

		// Token: 0x020017F2 RID: 6130
		public abstract class VoiceAudioDevice
		{
			// Token: 0x0600B6E2 RID: 46818 RVA: 0x004678CD File Offset: 0x00465ACD
			public VoiceAudioDevice(bool _isOutput, bool _isDefault)
			{
				this.IsOutput = _isOutput;
				this.IsDefault = _isDefault;
			}

			// Token: 0x170014A0 RID: 5280
			// (get) Token: 0x0600B6E3 RID: 46819
			public abstract string Identifier { get; }

			// Token: 0x04008FB5 RID: 36789
			public readonly bool IsOutput;

			// Token: 0x04008FB6 RID: 36790
			public readonly bool IsDefault;
		}

		// Token: 0x020017F3 RID: 6131
		public class VoiceAudioDeviceNotFound : IPartyVoice.VoiceAudioDevice
		{
			// Token: 0x0600B6E4 RID: 46820 RVA: 0x004678E3 File Offset: 0x00465AE3
			public VoiceAudioDeviceNotFound() : base(false, false)
			{
			}

			// Token: 0x0600B6E5 RID: 46821 RVA: 0x004678ED File Offset: 0x00465AED
			public override string ToString()
			{
				return Localization.Get("noAudioDeviceFound", false);
			}

			// Token: 0x170014A1 RID: 5281
			// (get) Token: 0x0600B6E6 RID: 46822 RVA: 0x0002B133 File Offset: 0x00029333
			public override string Identifier
			{
				get
				{
					return "";
				}
			}
		}

		// Token: 0x020017F4 RID: 6132
		public class VoiceAudioDeviceDefault : IPartyVoice.VoiceAudioDevice
		{
			// Token: 0x0600B6E7 RID: 46823 RVA: 0x004678E3 File Offset: 0x00465AE3
			public VoiceAudioDeviceDefault() : base(false, false)
			{
			}

			// Token: 0x0600B6E8 RID: 46824 RVA: 0x004678FA File Offset: 0x00465AFA
			public override string ToString()
			{
				return Localization.Get("defaultAudioDevice", false);
			}

			// Token: 0x170014A2 RID: 5282
			// (get) Token: 0x0600B6E9 RID: 46825 RVA: 0x0002B133 File Offset: 0x00029333
			public override string Identifier
			{
				get
				{
					return "";
				}
			}
		}
	}
}
