using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x020018C0 RID: 6336
	public class SteamQueryPortReader
	{
		// Token: 0x14000120 RID: 288
		// (add) Token: 0x0600BB01 RID: 47873 RVA: 0x00472EBC File Offset: 0x004710BC
		// (remove) Token: 0x0600BB02 RID: 47874 RVA: 0x00472EF4 File Offset: 0x004710F4
		[method: PublicizedFrom(EAccessModifier.Private)]
		public event GameServerDetailsCallback GameServerDetailsEvent;

		// Token: 0x0600BB03 RID: 47875 RVA: 0x00472F2C File Offset: 0x0047112C
		public void Init(IPlatform _owner)
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			if (this.matchmakingRulesResponse != null)
			{
				return;
			}
			this.matchmakingRulesResponse = new ISteamMatchmakingRulesResponse(new ISteamMatchmakingRulesResponse.RulesResponded(this.RulesResponded), new ISteamMatchmakingRulesResponse.RulesFailedToRespond(this.RulesFailedToRespond), new ISteamMatchmakingRulesResponse.RulesRefreshComplete(this.RulesRefreshComplete));
		}

		// Token: 0x0600BB04 RID: 47876 RVA: 0x00472F79 File Offset: 0x00471179
		public void Disconnect()
		{
			if (this.rulesRequestHandle != HServerQuery.Invalid)
			{
				SteamMatchmakingServers.CancelServerQuery(this.rulesRequestHandle);
				this.rulesRequestHandle = HServerQuery.Invalid;
			}
			this.GameServerDetailsEvent = null;
		}

		// Token: 0x0600BB05 RID: 47877 RVA: 0x00472FAA File Offset: 0x004711AA
		public void RegisterGameServerCallbacks(GameServerDetailsCallback _details)
		{
			this.GameServerDetailsEvent = _details;
		}

		// Token: 0x0600BB06 RID: 47878 RVA: 0x00472FB3 File Offset: 0x004711B3
		[PublicizedFrom(EAccessModifier.Private)]
		public void RunGameServerDetailsEvent(GameServerInfo _info, bool _success)
		{
			GameServerDetailsCallback gameServerDetailsEvent = this.GameServerDetailsEvent;
			if (gameServerDetailsEvent == null)
			{
				return;
			}
			gameServerDetailsEvent(_info, _success);
		}

		// Token: 0x0600BB07 RID: 47879 RVA: 0x00472FC8 File Offset: 0x004711C8
		public void GetGameServerInfo(GameServerInfo _gameInfo)
		{
			if (_gameInfo.IsLobby)
			{
				this.RunGameServerDetailsEvent(_gameInfo, true);
				return;
			}
			if (_gameInfo.IsNoResponse)
			{
				this.RunGameServerDetailsEvent(_gameInfo, true);
			}
			string text = _gameInfo.GetValue(GameInfoString.IP);
			long num;
			if (!long.TryParse(text.Replace(".", ""), out num))
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(text);
					if (hostEntry.AddressList.Length == 0)
					{
						Log.Out("Steamworks.NET] No valid IP for server found");
						this.RunGameServerDetailsEvent(_gameInfo, false);
						return;
					}
					text = hostEntry.AddressList[0].ToString();
				}
				catch (SocketException ex)
				{
					string str = "Steamworks.NET] No such hostname: \"";
					string str2 = text;
					string str3 = "\": ";
					SocketException ex2 = ex;
					Log.Out(str + str2 + str3 + ((ex2 != null) ? ex2.ToString() : null));
					this.RunGameServerDetailsEvent(_gameInfo, false);
					return;
				}
			}
			SteamQueryPortReader.RulesRequest item = new SteamQueryPortReader.RulesRequest
			{
				GameInfo = _gameInfo,
				Ip = NetworkUtils.ToInt(text),
				Port = (ushort)_gameInfo.GetValue(GameInfoInt.Port)
			};
			this.rulesRequests.Enqueue(item);
			if (this.rulesRequestHandle == HServerQuery.Invalid)
			{
				this.StartNextRulesRequest();
			}
		}

		// Token: 0x0600BB08 RID: 47880 RVA: 0x004730D8 File Offset: 0x004712D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartNextRulesRequest()
		{
			this.currentRulesRequest = null;
			this.rulesRequestHandle = HServerQuery.Invalid;
			if (this.rulesRequests.Count > 0)
			{
				this.currentRulesRequest = this.rulesRequests.Dequeue();
				this.currentRulesRequest.GameInfoClone = new GameServerInfo(this.currentRulesRequest.GameInfo);
				this.rulesRequestHandle = SteamMatchmakingServers.ServerRules(this.currentRulesRequest.Ip, this.currentRulesRequest.Port, this.matchmakingRulesResponse);
			}
		}

		// Token: 0x0600BB09 RID: 47881 RVA: 0x00473158 File Offset: 0x00471358
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesFailedToRespond()
		{
			this.RunGameServerDetailsEvent(this.currentRulesRequest.GameInfo, false);
			this.StartNextRulesRequest();
		}

		// Token: 0x0600BB0A RID: 47882 RVA: 0x00473174 File Offset: 0x00471374
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesRefreshComplete()
		{
			if (!this.currentRulesRequest.DataErrors && this.currentRulesRequest.GameInfoClone.GetValue(GameInfoString.GameName).Length > 0)
			{
				this.currentRulesRequest.GameInfo.Merge(this.currentRulesRequest.GameInfoClone, this.currentRulesRequest.GameInfo.IsLAN ? EServerRelationType.LAN : EServerRelationType.Internet);
				this.RunGameServerDetailsEvent(this.currentRulesRequest.GameInfo, true);
			}
			else
			{
				if (this.currentRulesRequest.DataErrors)
				{
					this.currentRulesRequest.GameInfo.SetValue(GameInfoString.ServerDescription, Localization.Get("xuiServerBrowserFailedRetrievingData", false));
				}
				this.RunGameServerDetailsEvent(this.currentRulesRequest.GameInfo, false);
			}
			this.StartNextRulesRequest();
		}

		// Token: 0x0600BB0B RID: 47883 RVA: 0x00473230 File Offset: 0x00471430
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesResponded(string _rule, string _value)
		{
			SteamQueryPortReader.RulesRequest rulesRequest = this.currentRulesRequest;
			if (rulesRequest.DataErrors)
			{
				return;
			}
			if (_rule.EqualsCaseInsensitive("gameinfo") || _rule.EqualsCaseInsensitive("ping"))
			{
				return;
			}
			if (rulesRequest.GameInfoClone.IsLAN && _rule.EqualsCaseInsensitive("ip"))
			{
				return;
			}
			if (!rulesRequest.GameInfoClone.ParseAny(_rule, _value))
			{
				rulesRequest.DataErrors = true;
			}
		}

		// Token: 0x04009244 RID: 37444
		[PublicizedFrom(EAccessModifier.Private)]
		public ISteamMatchmakingRulesResponse matchmakingRulesResponse;

		// Token: 0x04009245 RID: 37445
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Queue<SteamQueryPortReader.RulesRequest> rulesRequests = new Queue<SteamQueryPortReader.RulesRequest>();

		// Token: 0x04009246 RID: 37446
		[PublicizedFrom(EAccessModifier.Private)]
		public SteamQueryPortReader.RulesRequest currentRulesRequest;

		// Token: 0x04009247 RID: 37447
		[PublicizedFrom(EAccessModifier.Private)]
		public HServerQuery rulesRequestHandle = HServerQuery.Invalid;

		// Token: 0x020018C1 RID: 6337
		[PublicizedFrom(EAccessModifier.Private)]
		public class RulesRequest
		{
			// Token: 0x04009248 RID: 37448
			public uint Ip;

			// Token: 0x04009249 RID: 37449
			public ushort Port;

			// Token: 0x0400924A RID: 37450
			public GameServerInfo GameInfo;

			// Token: 0x0400924B RID: 37451
			public GameServerInfo GameInfoClone;

			// Token: 0x0400924C RID: 37452
			public bool DataErrors;
		}
	}
}
