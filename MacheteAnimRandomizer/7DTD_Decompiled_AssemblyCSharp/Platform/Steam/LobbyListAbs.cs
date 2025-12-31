using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Platform.Steam
{
	// Token: 0x020018B9 RID: 6329
	public abstract class LobbyListAbs : IServerListInterface
	{
		// Token: 0x0600BAC9 RID: 47817 RVA: 0x004726E5 File Offset: 0x004708E5
		[PublicizedFrom(EAccessModifier.Protected)]
		public LobbyListAbs()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			Application.wantsToQuit += this.OnApplicationQuit;
		}

		// Token: 0x17001547 RID: 5447
		// (get) Token: 0x0600BACA RID: 47818 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsPrefiltered
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600BACB RID: 47819
		public abstract void Init(IPlatform _owner);

		// Token: 0x0600BACC RID: 47820 RVA: 0x00472707 File Offset: 0x00470907
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _errorCallback)
		{
			this.gameServerFoundCallback = _serverFound;
		}

		// Token: 0x17001548 RID: 5448
		// (get) Token: 0x0600BACD RID: 47821 RVA: 0x00472710 File Offset: 0x00470910
		public bool IsRefreshing
		{
			get
			{
				return this.isRefreshing;
			}
		}

		// Token: 0x0600BACE RID: 47822
		public abstract void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters);

		// Token: 0x0600BACF RID: 47823
		public abstract void StopSearch();

		// Token: 0x0600BAD0 RID: 47824 RVA: 0x00472718 File Offset: 0x00470918
		public virtual void Disconnect()
		{
			this.StopSearch();
			this.gameServerFoundCallback = null;
		}

		// Token: 0x0600BAD1 RID: 47825 RVA: 0x000424BD File Offset: 0x000406BD
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BAD2 RID: 47826 RVA: 0x00472727 File Offset: 0x00470927
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool OnApplicationQuit()
		{
			this.Disconnect();
			return true;
		}

		// Token: 0x0600BAD3 RID: 47827 RVA: 0x00472730 File Offset: 0x00470930
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator restartRefreshCo(float _delay)
		{
			yield return new WaitForSeconds(_delay);
			this.StartSearch(null);
			yield break;
		}

		// Token: 0x0600BAD4 RID: 47828 RVA: 0x00472748 File Offset: 0x00470948
		[PublicizedFrom(EAccessModifier.Protected)]
		public void ParseLobbyData(CSteamID _lobbyId, EServerRelationType _source)
		{
			if (this.gameServerFoundCallback == null)
			{
				return;
			}
			GameServerInfo gameServerInfo = new GameServerInfo
			{
				IsLobby = true
			};
			int lobbyDataCount = SteamMatchmaking.GetLobbyDataCount(_lobbyId);
			for (int i = 0; i < lobbyDataCount; i++)
			{
				string key;
				string value;
				if (SteamMatchmaking.GetLobbyDataByIndex(_lobbyId, i, out key, 100, out value, 200))
				{
					gameServerInfo.ParseAny(key, value);
				}
			}
			if (PlatformManager.CrossplatformPlatform == null)
			{
				gameServerInfo.SetValue(GameInfoString.UniqueId, gameServerInfo.GetValue(GameInfoString.SteamID));
			}
			gameServerInfo.IsFriends = (_source == EServerRelationType.Friends);
			this.gameServerFoundCallback(this.owner, gameServerInfo, _source);
		}

		// Token: 0x0400922F RID: 37423
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatform owner;

		// Token: 0x04009230 RID: 37424
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameServerFoundCallback gameServerFoundCallback;

		// Token: 0x04009231 RID: 37425
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isRefreshing;
	}
}
