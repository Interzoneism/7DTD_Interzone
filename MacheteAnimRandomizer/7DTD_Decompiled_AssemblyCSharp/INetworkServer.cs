using System;
using UnityEngine.Networking;

// Token: 0x020007C6 RID: 1990
public interface INetworkServer
{
	// Token: 0x06003965 RID: 14693
	void Update();

	// Token: 0x06003966 RID: 14694
	void LateUpdate();

	// Token: 0x06003967 RID: 14695
	NetworkConnectionError StartServer(int _basePort, string _password);

	// Token: 0x06003968 RID: 14696
	void SetServerPassword(string _password);

	// Token: 0x06003969 RID: 14697
	void StopServer();

	// Token: 0x0600396A RID: 14698
	void DropClient(ClientInfo _clientInfo, bool _clientDisconnect);

	// Token: 0x0600396B RID: 14699
	NetworkError SendData(ClientInfo _clientInfo, int _channel, ArrayListMP<byte> _data, bool reliableDelivery = true);

	// Token: 0x0600396C RID: 14700
	string GetIP(ClientInfo _cInfo);

	// Token: 0x0600396D RID: 14701
	int GetPing(ClientInfo _cInfo);

	// Token: 0x0600396E RID: 14702
	string GetServerPorts(int _basePort);

	// Token: 0x0600396F RID: 14703
	void SetLatencySimulation(bool _enable, int _min, int _max);

	// Token: 0x06003970 RID: 14704
	void SetPacketLossSimulation(bool _enable, int _chance);

	// Token: 0x06003971 RID: 14705
	void EnableStatistics();

	// Token: 0x06003972 RID: 14706
	void DisableStatistics();

	// Token: 0x06003973 RID: 14707
	string PrintNetworkStatistics();

	// Token: 0x06003974 RID: 14708
	void ResetNetworkStatistics();

	// Token: 0x06003975 RID: 14709
	int GetMaximumPacketSize(ClientInfo _cInfo, bool reliable = false);

	// Token: 0x06003976 RID: 14710
	int GetBadPacketCount(ClientInfo _cInfo);
}
