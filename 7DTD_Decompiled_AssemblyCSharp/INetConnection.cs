using System;
using System.Collections.Generic;

// Token: 0x020006E8 RID: 1768
public interface INetConnection
{
	// Token: 0x06003426 RID: 13350
	void Disconnect(bool _kick);

	// Token: 0x06003427 RID: 13351
	bool IsDisconnected();

	// Token: 0x06003428 RID: 13352
	void GetPackages(List<NetPackage> _dstBuf);

	// Token: 0x06003429 RID: 13353
	void AddToSendQueue(NetPackage _package);

	// Token: 0x0600342A RID: 13354
	void AddToSendQueue(List<NetPackage> _packages);

	// Token: 0x0600342B RID: 13355
	void FlushSendQueue();

	// Token: 0x0600342C RID: 13356
	void AppendToReaderStream(byte[] _data, int _size);

	// Token: 0x0600342D RID: 13357
	void UpgradeToFullConnection();

	// Token: 0x0600342E RID: 13358
	NetConnectionStatistics GetStats();

	// Token: 0x0600342F RID: 13359
	void SetEncryptionModule(IEncryptionModule _module);
}
