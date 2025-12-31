using System;
using UnityEngine.Networking;

// Token: 0x020007C5 RID: 1989
public interface INetworkClient
{
	// Token: 0x0600395A RID: 14682
	void Update();

	// Token: 0x0600395B RID: 14683
	void LateUpdate();

	// Token: 0x0600395C RID: 14684
	void Connect(GameServerInfo _gsi);

	// Token: 0x0600395D RID: 14685
	void Disconnect();

	// Token: 0x0600395E RID: 14686
	NetworkError SendData(int _channel, ArrayListMP<byte> _data);

	// Token: 0x0600395F RID: 14687
	void SetLatencySimulation(bool _enable, int _min, int _max);

	// Token: 0x06003960 RID: 14688
	void SetPacketLossSimulation(bool _enable, int _chance);

	// Token: 0x06003961 RID: 14689
	void EnableStatistics();

	// Token: 0x06003962 RID: 14690
	void DisableStatistics();

	// Token: 0x06003963 RID: 14691
	string PrintNetworkStatistics();

	// Token: 0x06003964 RID: 14692
	void ResetNetworkStatistics();
}
