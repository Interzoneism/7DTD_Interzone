using System;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class GameSparksRTUnity : MonoBehaviour, IRTSessionListener
{
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000096 RID: 150 RVA: 0x00009687 File Offset: 0x00007887
	// (set) Token: 0x06000097 RID: 151 RVA: 0x000096BE File Offset: 0x000078BE
	public static GameSparksRTUnity Instance
	{
		get
		{
			if (GameSparksRTUnity.instance == null)
			{
				GameSparksRTUnity.instance = new GameObject("GameSparksRTUnity").AddComponent<GameSparksRTUnity>();
				UnityEngine.Object.DontDestroyOnLoad(GameSparksRTUnity.instance.gameObject);
			}
			return GameSparksRTUnity.instance;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (GameSparksRTUnity.instance != null && GameSparksRTUnity.instance != value)
			{
				UnityEngine.Object.Destroy(GameSparksRTUnity.instance.gameObject);
			}
			GameSparksRTUnity.instance = value;
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x000096EF File Offset: 0x000078EF
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		GameSparksRTUnity.instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00009704 File Offset: 0x00007904
	public void Configure(MatchFoundMessage message, Action<int> OnPlayerConnect, Action<int> OnPlayerDisconnect, Action<bool> OnReady, Action<RTPacket> OnPacket, GSInstance instance = null)
	{
		if (message.Port == null)
		{
			Debug.Log("Response does not contain a port, exiting.");
			return;
		}
		this.Configure(message.Host, message.Port.Value, message.AccessToken, OnPlayerConnect, OnPlayerDisconnect, OnReady, OnPacket, instance);
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00009754 File Offset: 0x00007954
	public void Configure(FindMatchResponse response, Action<int> OnPlayerConnect, Action<int> OnPlayerDisconnect, Action<bool> OnReady, Action<RTPacket> OnPacket, GSInstance instance = null)
	{
		if (response.Port == null)
		{
			Debug.Log("Response does not contain a port, exiting.");
			return;
		}
		this.Configure(response.Host, response.Port.Value, response.AccessToken, OnPlayerConnect, OnPlayerDisconnect, OnReady, OnPacket, instance);
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000097A4 File Offset: 0x000079A4
	public void Configure(string host, int port, string accessToken, Action<int> OnPlayerConnect, Action<int> OnPlayerDisconnect, Action<bool> OnReady, Action<RTPacket> OnPacket, GSInstance instance = null)
	{
		this.m_OnPlayerConnect = OnPlayerConnect;
		this.m_OnPlayerDisconnect = OnPlayerDisconnect;
		this.m_OnReady = OnReady;
		this.m_OnPacket = OnPacket;
		if (this.session != null)
		{
			this.session.Stop();
		}
		this.session = GameSparksRT.SessionBuilder().SetHost(host).SetPort(port).SetConnectToken(accessToken).SetListener(this).SetGSInstance(instance).Build();
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00009813 File Offset: 0x00007A13
	public void Connect()
	{
		if (this.session != null)
		{
			Debug.Log("Starting Session");
			this.session.Start();
			return;
		}
		Debug.Log("Cannot start Session");
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000983D File Offset: 0x00007A3D
	public void Disconnect()
	{
		if (this.session != null)
		{
			this.session.Stop();
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600009E RID: 158 RVA: 0x00009854 File Offset: 0x00007A54
	public int? PeerId
	{
		get
		{
			if (this.session != null)
			{
				return this.session.PeerId;
			}
			return null;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600009F RID: 159 RVA: 0x0000987E File Offset: 0x00007A7E
	public List<int> ActivePeers
	{
		get
		{
			if (this.session != null)
			{
				return this.session.ActivePeers;
			}
			return null;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060000A0 RID: 160 RVA: 0x00009895 File Offset: 0x00007A95
	public bool Ready
	{
		get
		{
			return this.session != null && this.session.Ready;
		}
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000098AC File Offset: 0x00007AAC
	public int SendData(int opCode, GameSparksRT.DeliveryIntent deliveryIntent, RTData structuredData, params int[] targetPlayers)
	{
		if (this.session != null)
		{
			return this.session.SendRTData(opCode, deliveryIntent, structuredData, targetPlayers);
		}
		return -1;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000098C8 File Offset: 0x00007AC8
	public int SendBytes(int opCode, GameSparksRT.DeliveryIntent deliveryIntent, ArraySegment<byte> unstructuredData, params int[] targetPlayers)
	{
		if (this.session != null)
		{
			return this.session.SendBytes(opCode, deliveryIntent, unstructuredData, targetPlayers);
		}
		return -1;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x000098E4 File Offset: 0x00007AE4
	public int SendRTDataAndBytes(int opCode, GameSparksRT.DeliveryIntent deliveryIntent, ArraySegment<byte> unstructuredData, RTData structuredData, params int[] targetPlayers)
	{
		if (this.session != null)
		{
			return this.session.SendRTDataAndBytes(opCode, deliveryIntent, new ArraySegment<byte>?(unstructuredData), structuredData, targetPlayers);
		}
		return -1;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00009907 File Offset: 0x00007B07
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.Disconnect();
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x0000990F File Offset: 0x00007B0F
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.session != null)
		{
			this.session.Update();
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00009924 File Offset: 0x00007B24
	public void OnPlayerConnect(int peerId)
	{
		if (this.m_OnPlayerConnect != null)
		{
			this.m_OnPlayerConnect(peerId);
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x0000993A File Offset: 0x00007B3A
	public void OnPlayerDisconnect(int peerId)
	{
		if (this.m_OnPlayerDisconnect != null)
		{
			this.m_OnPlayerDisconnect(peerId);
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00009950 File Offset: 0x00007B50
	public void OnReady(bool ready)
	{
		if (this.m_OnReady != null)
		{
			this.m_OnReady(ready);
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00009966 File Offset: 0x00007B66
	public void OnPacket(RTPacket packet)
	{
		if (this.m_OnPacket != null)
		{
			this.m_OnPacket(packet);
		}
	}

	// Token: 0x040000D6 RID: 214
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IRTSession session;

	// Token: 0x040000D7 RID: 215
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Action<int> m_OnPlayerConnect;

	// Token: 0x040000D8 RID: 216
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Action<int> m_OnPlayerDisconnect;

	// Token: 0x040000D9 RID: 217
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Action<bool> m_OnReady;

	// Token: 0x040000DA RID: 218
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Action<RTPacket> m_OnPacket;

	// Token: 0x040000DB RID: 219
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameSparksRTUnity instance;
}
