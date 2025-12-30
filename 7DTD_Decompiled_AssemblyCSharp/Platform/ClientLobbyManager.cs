using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x020017C3 RID: 6083
	public class ClientLobbyManager
	{
		// Token: 0x0600B5D9 RID: 46553 RVA: 0x00466013 File Offset: 0x00464213
		public ClientLobbyManager()
		{
			ConnectionManager.OnClientDisconnected += this.OnClientDisconnected;
		}

		// Token: 0x0600B5DA RID: 46554 RVA: 0x00466044 File Offset: 0x00464244
		public bool TryGetLobbyId(EPlatformIdentifier platform, out PlatformLobbyId lobbyId)
		{
			object obj = this.lockObj;
			bool result;
			lock (obj)
			{
				ClientLobbyManager.Lobby lobby;
				if (this.lobbies.TryGetValue(platform, out lobby))
				{
					lobbyId = lobby.Id;
					result = true;
				}
				else
				{
					lobbyId = null;
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600B5DB RID: 46555 RVA: 0x004660A0 File Offset: 0x004642A0
		public void RegisterLobbyClient(PlatformLobbyId platformLobbyId, ClientInfo client, bool overwrite = false)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.Contains(client))
			{
				Log.Warning(string.Format("[ClientLobbyManager] could not register {0} for client lobby {1} : {2} as they are no longer connected", client.playerName, platformLobbyId.PlatformIdentifier, platformLobbyId.LobbyId));
				return;
			}
			object obj = this.lockObj;
			lock (obj)
			{
				ClientLobbyManager.Lobby lobby;
				if (!this.lobbies.TryGetValue(platformLobbyId.PlatformIdentifier, out lobby))
				{
					Log.Out(string.Format("[ClientLobbyManager] registering new lobby for client platform {0} : {1}", platformLobbyId.PlatformIdentifier, platformLobbyId.LobbyId));
					lobby = new ClientLobbyManager.Lobby(platformLobbyId);
					lobby.AddClient(client);
					this.lobbies.Add(platformLobbyId.PlatformIdentifier, lobby);
				}
				else if (lobby.Id.LobbyId.Equals(platformLobbyId.LobbyId))
				{
					lobby.AddClient(client);
				}
				else if (overwrite)
				{
					Log.Warning(string.Format("[ClientLobbyManager] overwriting existing lobby for {0}", platformLobbyId.PlatformIdentifier));
					ClientLobbyManager.Lobby lobby2 = new ClientLobbyManager.Lobby(platformLobbyId);
					lobby2.AddClient(client);
					foreach (ClientInfo clientInfo in lobby.Clients)
					{
						clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageLobbyJoin>().Setup(platformLobbyId));
						lobby2.AddClient(clientInfo);
					}
					this.lobbies[platformLobbyId.PlatformIdentifier] = lobby2;
				}
				else
				{
					Log.Warning(string.Format("[ClientLobbyManager] a different client lobby already registered for {0}, sending to client", platformLobbyId.PlatformIdentifier));
					client.SendPackage(NetPackageManager.GetPackage<NetPackageLobbyJoin>().Setup(lobby.Id));
				}
			}
		}

		// Token: 0x0600B5DC RID: 46556 RVA: 0x00466270 File Offset: 0x00464470
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientDisconnected(ClientInfo client)
		{
			object obj = this.lockObj;
			lock (obj)
			{
				ClientLobbyManager.Lobby lobby;
				if (this.lobbies.TryGetValue(client.PlatformId.PlatformIdentifier, out lobby))
				{
					lobby.RemoveClient(client);
					if (lobby.IsEmpty)
					{
						Log.Out(string.Format("[ClientLobbyManager] removing registered lobby {0} : {1}", lobby.Id.PlatformIdentifier, lobby.Id.LobbyId));
						this.lobbies.Remove(client.PlatformId.PlatformIdentifier);
					}
				}
			}
		}

		// Token: 0x04008F29 RID: 36649
		[PublicizedFrom(EAccessModifier.Private)]
		public object lockObj = new object();

		// Token: 0x04008F2A RID: 36650
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<EPlatformIdentifier, ClientLobbyManager.Lobby> lobbies = new Dictionary<EPlatformIdentifier, ClientLobbyManager.Lobby>();

		// Token: 0x020017C4 RID: 6084
		[PublicizedFrom(EAccessModifier.Private)]
		public class Lobby
		{
			// Token: 0x17001468 RID: 5224
			// (get) Token: 0x0600B5DD RID: 46557 RVA: 0x00466314 File Offset: 0x00464514
			public PlatformLobbyId Id
			{
				get
				{
					return this.id;
				}
			}

			// Token: 0x17001469 RID: 5225
			// (get) Token: 0x0600B5DE RID: 46558 RVA: 0x0046631C File Offset: 0x0046451C
			public bool IsEmpty
			{
				get
				{
					return this.clients.Count == 0;
				}
			}

			// Token: 0x1700146A RID: 5226
			// (get) Token: 0x0600B5DF RID: 46559 RVA: 0x0046632C File Offset: 0x0046452C
			public IReadOnlyList<ClientInfo> Clients
			{
				get
				{
					return this.clients;
				}
			}

			// Token: 0x0600B5E0 RID: 46560 RVA: 0x00466334 File Offset: 0x00464534
			public Lobby(PlatformLobbyId id)
			{
				this.id = id;
			}

			// Token: 0x0600B5E1 RID: 46561 RVA: 0x0046634E File Offset: 0x0046454E
			public Lobby(EPlatformIdentifier platform, string lobbyId)
			{
				this.id = new PlatformLobbyId(platform, lobbyId);
			}

			// Token: 0x0600B5E2 RID: 46562 RVA: 0x00466370 File Offset: 0x00464570
			public void AddClient(ClientInfo client)
			{
				this.clients.Add(client);
				Log.Out(string.Format("[ClientLobbyManager] registered member {0} for client lobby {1} : {2}. Total members: {3}", new object[]
				{
					client.playerName,
					this.id.PlatformIdentifier,
					this.id.LobbyId,
					this.clients.Count
				}));
			}

			// Token: 0x0600B5E3 RID: 46563 RVA: 0x004663DC File Offset: 0x004645DC
			public void RemoveClient(ClientInfo client)
			{
				if (this.clients.Remove(client))
				{
					Log.Out(string.Format("[ClientLobbyManager] removed member {0} from client lobby {1} : {2}. Total members: {3}", new object[]
					{
						client.playerName,
						this.id.PlatformIdentifier,
						this.id.LobbyId,
						this.clients.Count
					}));
					return;
				}
				Log.Warning(string.Format("[ClientLobbyManager] remove member {0} from client lobby {1} : {2} failed. They are not a member", client.playerName, this.id.PlatformIdentifier, this.id.LobbyId));
			}

			// Token: 0x04008F2B RID: 36651
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly PlatformLobbyId id;

			// Token: 0x04008F2C RID: 36652
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly List<ClientInfo> clients = new List<ClientInfo>();
		}
	}
}
