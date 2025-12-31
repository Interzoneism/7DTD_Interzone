using System;
using System.IO;

namespace Platform
{
	// Token: 0x02001815 RID: 6165
	public interface IRemotePlayerStorageObject
	{
		// Token: 0x0600B791 RID: 46993
		void ReadInto(BinaryReader _reader);

		// Token: 0x0600B792 RID: 46994
		void WriteFrom(BinaryWriter _writer);
	}
}
