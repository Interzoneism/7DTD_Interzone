using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;

// Token: 0x020007E6 RID: 2022
public class ServerListManager
{
	// Token: 0x170005DF RID: 1503
	// (get) Token: 0x06003A25 RID: 14885 RVA: 0x00176EC8 File Offset: 0x001750C8
	public static ServerListManager Instance
	{
		get
		{
			ServerListManager result;
			if ((result = ServerListManager.instance) == null)
			{
				result = (ServerListManager.instance = new ServerListManager());
			}
			return result;
		}
	}

	// Token: 0x170005E0 RID: 1504
	// (get) Token: 0x06003A26 RID: 14886 RVA: 0x00176EDE File Offset: 0x001750DE
	// (set) Token: 0x06003A27 RID: 14887 RVA: 0x00176EE6 File Offset: 0x001750E6
	public bool IsRefreshing { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170005E1 RID: 1505
	// (get) Token: 0x06003A28 RID: 14888 RVA: 0x00176EEF File Offset: 0x001750EF
	public bool IsPrefilteredSearch { get; }

	// Token: 0x06003A29 RID: 14889 RVA: 0x00176EF8 File Offset: 0x001750F8
	public void StartSearch(List<IServerListInterface.ServerFilter> _activeFilters)
	{
		this.IsRefreshing = true;
		foreach (IServerListInterface serverListInterface in this.serverLists)
		{
			serverListInterface.StartSearch(_activeFilters);
		}
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x00176F50 File Offset: 0x00175150
	public void StopSearch()
	{
		this.IsRefreshing = false;
		foreach (IServerListInterface serverListInterface in this.serverLists)
		{
			serverListInterface.StopSearch();
		}
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x00176FA8 File Offset: 0x001751A8
	[PublicizedFrom(EAccessModifier.Private)]
	public ServerListManager()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		Application.wantsToQuit += delegate()
		{
			this.Disconnect();
			return true;
		};
		IList<IServerListInterface> serverListInterfaces = PlatformManager.MultiPlatform.ServerListInterfaces;
		if (serverListInterfaces != null)
		{
			this.serverLists.AddRange(serverListInterfaces);
		}
		using (List<IServerListInterface>.Enumerator enumerator = this.serverLists.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsPrefiltered)
				{
					this.IsPrefilteredSearch = 1;
				}
			}
		}
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x00177048 File Offset: 0x00175248
	public void Disconnect()
	{
		foreach (IServerListInterface serverListInterface in this.serverLists)
		{
			serverListInterface.Disconnect();
		}
		this.IsRefreshing = false;
	}

	// Token: 0x06003A2D RID: 14893 RVA: 0x001770A0 File Offset: 0x001752A0
	public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _errorCallback)
	{
		foreach (IServerListInterface serverListInterface in this.serverLists)
		{
			serverListInterface.RegisterGameServerFoundCallback(_serverFound, _maxResultsCallback, _errorCallback);
		}
	}

	// Token: 0x04002F08 RID: 12040
	[PublicizedFrom(EAccessModifier.Private)]
	public static ServerListManager instance;

	// Token: 0x04002F0B RID: 12043
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<IServerListInterface> serverLists = new List<IServerListInterface>();
}
