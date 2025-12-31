using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x020018B7 RID: 6327
	[PublicizedFrom(EAccessModifier.Internal)]
	public class JoinSessionGameInviteListener : IJoinSessionGameInviteListener
	{
		// Token: 0x0600BABB RID: 47803 RVA: 0x004724E5 File Offset: 0x004706E5
		public void Init(IPlatform _owner)
		{
			if (this.m_friends_serverchange == null)
			{
				this.m_friends_serverchange = Callback<GameServerChangeRequested_t>.Create(new Callback<GameServerChangeRequested_t>.DispatchDelegate(this.Friends_GameServerChangeRequested));
			}
			_owner.Api.ClientApiInitialized += delegate()
			{
				string[] commandLineArgs = Environment.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length - 1; i++)
				{
					ulong ulSteamID;
					if (commandLineArgs[i] == "+connect_lobby" && ulong.TryParse(commandLineArgs[i + 1], out ulSteamID))
					{
						this.SetLobby(new CSteamID(ulSteamID));
					}
				}
			};
		}

		// Token: 0x0600BABC RID: 47804 RVA: 0x00472520 File Offset: 0x00470720
		[return: TupleElementNames(new string[]
		{
			"invite",
			"password"
		})]
		public ValueTuple<string, string> TakePendingInvite()
		{
			string item = this.pendingInvite;
			this.pendingInvite = null;
			string item2 = this.pendingPassword;
			this.pendingPassword = null;
			return new ValueTuple<string, string>(item, item2);
		}

		// Token: 0x0600BABD RID: 47805 RVA: 0x0047254E File Offset: 0x0047074E
		public IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null)
		{
			if (string.IsNullOrEmpty(_invite))
			{
				yield break;
			}
			if (_invite.StartsWith("Lobby:"))
			{
				ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
				if (lobbyHost != null)
				{
					lobbyHost.JoinLobby(_invite.Substring("Lobby:".Length), null);
				}
				yield break;
			}
			string[] array = _invite.Split(':', StringSplitOptions.None);
			string ip = "";
			int port = 0;
			if (array.Length == 2)
			{
				ip = array[0];
				port = Convert.ToInt32(array[1]);
			}
			yield return InviteManager.HandleIpPortInvite(ip, port, _password, _onFinished);
			yield break;
		}

		// Token: 0x0600BABE RID: 47806 RVA: 0x0047256B File Offset: 0x0047076B
		public string GetListenerIdentifier()
		{
			return "STM";
		}

		// Token: 0x0600BABF RID: 47807 RVA: 0x00472572 File Offset: 0x00470772
		public void SetLobby(CSteamID _lobbyId)
		{
			this.pendingInvite = "Lobby:" + _lobbyId.m_SteamID.ToString();
		}

		// Token: 0x0600BAC0 RID: 47808 RVA: 0x00472590 File Offset: 0x00470790
		[PublicizedFrom(EAccessModifier.Private)]
		public void Friends_GameServerChangeRequested(GameServerChangeRequested_t _value)
		{
			Log.Out("[Steamworks.NET] Friends_GameServerChangeRequested");
			this.pendingInvite = _value.m_rgchServer;
			this.pendingPassword = _value.m_rgchPassword;
		}

		// Token: 0x04009226 RID: 37414
		[PublicizedFrom(EAccessModifier.Private)]
		public const string LobbyMarker = "Lobby:";

		// Token: 0x04009227 RID: 37415
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingInvite;

		// Token: 0x04009228 RID: 37416
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingPassword;

		// Token: 0x04009229 RID: 37417
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<GameServerChangeRequested_t> m_friends_serverchange;
	}
}
