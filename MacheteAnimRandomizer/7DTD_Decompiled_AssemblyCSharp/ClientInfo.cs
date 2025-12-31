using System;
using System.Collections.Generic;
using Platform.Local;
using Platform.Steam;

// Token: 0x020006C1 RID: 1729
public class ClientInfo : IEquatable<ClientInfo>
{
	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x060032C4 RID: 12996 RVA: 0x00157F61 File Offset: 0x00156161
	public PlatformUserIdentifierAbs InternalId
	{
		get
		{
			return this.CrossplatformId ?? this.PlatformId;
		}
	}

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x060032C5 RID: 12997 RVA: 0x00157F73 File Offset: 0x00156173
	public string ip
	{
		get
		{
			return this.network.GetIP(this);
		}
	}

	// Token: 0x060032C6 RID: 12998 RVA: 0x00157F84 File Offset: 0x00156184
	public ClientInfo()
	{
		int num;
		do
		{
			num = ++ClientInfo.lastClientNumber;
			if (num > 1000000)
			{
				num = (ClientInfo.lastClientNumber = 1);
			}
		}
		while (SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForClientNumber(num) != null);
		this.ClientNumber = num;
	}

	// Token: 0x060032C7 RID: 12999 RVA: 0x0015800C File Offset: 0x0015620C
	public override string ToString()
	{
		string text = null;
		UserIdentifierSteam userIdentifierSteam = this.PlatformId as UserIdentifierSteam;
		if (userIdentifierSteam != null)
		{
			UserIdentifierSteam ownerId = userIdentifierSteam.OwnerId;
			text = ((ownerId != null) ? ownerId.CombinedString : null);
		}
		string format = "EntityID={0}, PltfmId='{1}', CrossId='{2}', OwnerID='{3}', PlayerName='{4}', ClientNumber='{5}'";
		object[] array = new object[6];
		array[0] = this.entityId;
		int num = 1;
		PlatformUserIdentifierAbs platformId = this.PlatformId;
		array[num] = (((platformId != null) ? platformId.CombinedString : null) ?? "<unknown>");
		int num2 = 2;
		PlatformUserIdentifierAbs crossplatformId = this.CrossplatformId;
		array[num2] = (((crossplatformId != null) ? crossplatformId.CombinedString : null) ?? "<unknown/none>");
		array[3] = (text ?? "<unknown/none>");
		array[4] = this.playerName;
		array[5] = this.ClientNumber;
		return string.Format(format, array);
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x001580BB File Offset: 0x001562BB
	public void UpdatePing()
	{
		this.ping = this.network.GetPing(this);
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x001580D0 File Offset: 0x001562D0
	public void SendPackage(NetPackage _package)
	{
		if (!_package.AllowedBeforeAuth && !this.loginDone)
		{
			Log.Warning(string.Format("Ignoring {0}, not logged in yet", _package));
			return;
		}
		this.netConnection[_package.Channel].AddToSendQueue(_package);
		if (_package.FlushQueue)
		{
			this.netConnection[_package.Channel].FlushSendQueue();
		}
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x0015812C File Offset: 0x0015632C
	public void SetAntiCheatEncryption(IEncryptionModule encryptionModule)
	{
		INetConnection[] array = this.netConnection;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetEncryptionModule(encryptionModule);
		}
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x00158157 File Offset: 0x00156357
	public bool Equals(ClientInfo _other)
	{
		return this == _other;
	}

	// Token: 0x040029A0 RID: 10656
	[PublicizedFrom(EAccessModifier.Private)]
	public static int lastClientNumber;

	// Token: 0x040029A1 RID: 10657
	public INetworkServer network;

	// Token: 0x040029A2 RID: 10658
	public readonly int ClientNumber;

	// Token: 0x040029A3 RID: 10659
	public long litenetPeerConnectId = -1L;

	// Token: 0x040029A4 RID: 10660
	public PlatformUserIdentifierAbs PlatformId = new UserIdentifierLocal("<none>");

	// Token: 0x040029A5 RID: 10661
	public PlatformUserIdentifierAbs CrossplatformId;

	// Token: 0x040029A6 RID: 10662
	public ulong DiscordUserId;

	// Token: 0x040029A7 RID: 10663
	public bool requiresAntiCheat = true;

	// Token: 0x040029A8 RID: 10664
	public ClientInfo.EDeviceType device = ClientInfo.EDeviceType.Unknown;

	// Token: 0x040029A9 RID: 10665
	public bool loginDone;

	// Token: 0x040029AA RID: 10666
	public bool acAuthDone;

	// Token: 0x040029AB RID: 10667
	public INetConnection[] netConnection;

	// Token: 0x040029AC RID: 10668
	public bool bAttachedToEntity;

	// Token: 0x040029AD RID: 10669
	public int entityId = -1;

	// Token: 0x040029AE RID: 10670
	public string playerName;

	// Token: 0x040029AF RID: 10671
	public string compatibilityVersion;

	// Token: 0x040029B0 RID: 10672
	public readonly Dictionary<string, int> groupMemberships = new Dictionary<string, int>(StringComparer.Ordinal);

	// Token: 0x040029B1 RID: 10673
	public int groupMembershipsWaiting;

	// Token: 0x040029B2 RID: 10674
	public PlayerDataFile latestPlayerData;

	// Token: 0x040029B3 RID: 10675
	public int ping;

	// Token: 0x040029B4 RID: 10676
	public bool disconnecting;

	// Token: 0x020006C2 RID: 1730
	public enum EDeviceType
	{
		// Token: 0x040029B6 RID: 10678
		Linux,
		// Token: 0x040029B7 RID: 10679
		Mac,
		// Token: 0x040029B8 RID: 10680
		Windows,
		// Token: 0x040029B9 RID: 10681
		PlayStation,
		// Token: 0x040029BA RID: 10682
		Xbox,
		// Token: 0x040029BB RID: 10683
		Unknown
	}
}
