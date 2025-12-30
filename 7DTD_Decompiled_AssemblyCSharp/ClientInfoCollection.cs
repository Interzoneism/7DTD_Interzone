using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// Token: 0x020006C3 RID: 1731
public class ClientInfoCollection
{
	// Token: 0x060032CC RID: 13004 RVA: 0x00158160 File Offset: 0x00156360
	public ClientInfoCollection()
	{
		this.List = new ReadOnlyCollection<ClientInfo>(this.list);
	}

	// Token: 0x060032CD RID: 13005 RVA: 0x001581BB File Offset: 0x001563BB
	public void Add(ClientInfo _cInfo)
	{
		this.list.Add(_cInfo);
		this.clientNumberMap.Add(_cInfo.ClientNumber, _cInfo);
	}

	// Token: 0x060032CE RID: 13006 RVA: 0x001581DB File Offset: 0x001563DB
	public void Clear()
	{
		this.list.Clear();
		this.clientNumberMap.Clear();
		this.entityIdMap.Clear();
		this.litenetPeerMap.Clear();
		this.userIdMap.Clear();
	}

	// Token: 0x060032CF RID: 13007 RVA: 0x00158214 File Offset: 0x00156414
	public bool Contains(ClientInfo _cInfo)
	{
		return this.list.Contains(_cInfo);
	}

	// Token: 0x060032D0 RID: 13008 RVA: 0x00158224 File Offset: 0x00156424
	public void Remove(ClientInfo _cInfo)
	{
		this.list.Remove(_cInfo);
		this.clientNumberMap.Remove(_cInfo.ClientNumber);
		this.entityIdMap.Remove(_cInfo.entityId);
		if (_cInfo.litenetPeerConnectId >= 0L)
		{
			this.litenetPeerMap.Remove(_cInfo.litenetPeerConnectId);
		}
		if (_cInfo.PlatformId != null)
		{
			this.userIdMap.Remove(_cInfo.PlatformId);
		}
		if (_cInfo.CrossplatformId != null)
		{
			this.userIdMap.Remove(_cInfo.CrossplatformId);
		}
	}

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x060032D1 RID: 13009 RVA: 0x001582B2 File Offset: 0x001564B2
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x060032D2 RID: 13010 RVA: 0x001582C0 File Offset: 0x001564C0
	public ClientInfo ForClientNumber(int _clientNumber)
	{
		ClientInfo result;
		if (this.clientNumberMap.TryGetValue(_clientNumber, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060032D3 RID: 13011 RVA: 0x001582E0 File Offset: 0x001564E0
	public ClientInfo ForEntityId(int _entityId)
	{
		ClientInfo result;
		if (this.entityIdMap.TryGetValue(_entityId, out result))
		{
			return result;
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			if (clientInfo.entityId == _entityId)
			{
				this.entityIdMap.Add(_entityId, clientInfo);
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x060032D4 RID: 13012 RVA: 0x0015833C File Offset: 0x0015653C
	public ClientInfo ForLiteNetPeer(long _peerConnectId)
	{
		ClientInfo result;
		if (this.litenetPeerMap.TryGetValue(_peerConnectId, out result))
		{
			return result;
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			if (clientInfo.litenetPeerConnectId == _peerConnectId)
			{
				this.litenetPeerMap.Add(_peerConnectId, clientInfo);
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x060032D5 RID: 13013 RVA: 0x00158398 File Offset: 0x00156598
	public ClientInfo ForUserId(PlatformUserIdentifierAbs _userIdentifier)
	{
		ClientInfo result;
		if (this.userIdMap.TryGetValue(_userIdentifier, out result))
		{
			return result;
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			if (_userIdentifier.Equals(clientInfo.PlatformId))
			{
				this.userIdMap[_userIdentifier] = clientInfo;
				return clientInfo;
			}
			if (_userIdentifier.Equals(clientInfo.CrossplatformId))
			{
				this.userIdMap[_userIdentifier] = clientInfo;
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x060032D6 RID: 13014 RVA: 0x00158418 File Offset: 0x00156618
	public ClientInfo GetForPlayerName(string _playerName, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		if (_ignoreBlanks)
		{
			_playerName = _playerName.Replace(" ", "");
		}
		for (int i = 0; i < this.list.Count; i++)
		{
			ClientInfo clientInfo = this.list[i];
			string text = clientInfo.playerName ?? string.Empty;
			if (_ignoreBlanks)
			{
				text = text.Replace(" ", "");
			}
			if (string.Equals(text, _playerName, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
			{
				return clientInfo;
			}
		}
		return null;
	}

	// Token: 0x060032D7 RID: 13015 RVA: 0x00158494 File Offset: 0x00156694
	public ClientInfo GetForNameOrId(string _nameOrId, bool _ignoreCase = true, bool _ignoreBlanks = false)
	{
		int entityId;
		if (int.TryParse(_nameOrId, out entityId))
		{
			ClientInfo clientInfo = this.ForEntityId(entityId);
			if (clientInfo != null)
			{
				return clientInfo;
			}
		}
		PlatformUserIdentifierAbs userIdentifier;
		if (PlatformUserIdentifierAbs.TryFromCombinedString(_nameOrId, out userIdentifier))
		{
			ClientInfo clientInfo2 = this.ForUserId(userIdentifier);
			if (clientInfo2 != null)
			{
				return clientInfo2;
			}
		}
		return this.GetForPlayerName(_nameOrId, _ignoreCase, _ignoreBlanks);
	}

	// Token: 0x040029BC RID: 10684
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ClientInfo> list = new List<ClientInfo>();

	// Token: 0x040029BD RID: 10685
	public readonly ReadOnlyCollection<ClientInfo> List;

	// Token: 0x040029BE RID: 10686
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, ClientInfo> clientNumberMap = new Dictionary<int, ClientInfo>();

	// Token: 0x040029BF RID: 10687
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, ClientInfo> entityIdMap = new Dictionary<int, ClientInfo>();

	// Token: 0x040029C0 RID: 10688
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<long, ClientInfo> litenetPeerMap = new Dictionary<long, ClientInfo>();

	// Token: 0x040029C1 RID: 10689
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, ClientInfo> userIdMap = new Dictionary<PlatformUserIdentifierAbs, ClientInfo>();
}
