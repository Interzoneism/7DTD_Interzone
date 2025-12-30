using System;
using System.IO;

// Token: 0x020006E7 RID: 1767
public interface IEncryptionModule
{
	// Token: 0x06003424 RID: 13348
	bool EncryptStream(ClientInfo _cInfo, MemoryStream _stream);

	// Token: 0x06003425 RID: 13349
	bool DecryptStream(ClientInfo _cInfo, MemoryStream _stream);
}
