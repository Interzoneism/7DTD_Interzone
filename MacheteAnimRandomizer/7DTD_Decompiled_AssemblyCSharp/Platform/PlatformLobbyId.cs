using System;
using System.IO;
using System.Text;

namespace Platform
{
	// Token: 0x02001848 RID: 6216
	public class PlatformLobbyId
	{
		// Token: 0x0600B85C RID: 47196 RVA: 0x00469723 File Offset: 0x00467923
		public PlatformLobbyId(EPlatformIdentifier _platformId, string _lobbyId)
		{
			this.PlatformIdentifier = _platformId;
			this.LobbyId = _lobbyId;
		}

		// Token: 0x0600B85D RID: 47197 RVA: 0x00469739 File Offset: 0x00467939
		public int GetWriteLength(Encoding encoding)
		{
			return 1 + this.LobbyId.GetBinaryWriterLength(encoding);
		}

		// Token: 0x0600B85E RID: 47198 RVA: 0x00469749 File Offset: 0x00467949
		public void Write(BinaryWriter _writer)
		{
			_writer.Write((byte)this.PlatformIdentifier);
			if (this.PlatformIdentifier != EPlatformIdentifier.None)
			{
				_writer.Write(this.LobbyId);
			}
		}

		// Token: 0x0600B85F RID: 47199 RVA: 0x0046976C File Offset: 0x0046796C
		public static PlatformLobbyId Read(BinaryReader _reader)
		{
			byte b = _reader.ReadByte();
			string lobbyId = (b != 0) ? _reader.ReadString() : string.Empty;
			return new PlatformLobbyId((EPlatformIdentifier)b, lobbyId);
		}

		// Token: 0x0400904F RID: 36943
		public static readonly PlatformLobbyId None = new PlatformLobbyId(EPlatformIdentifier.None, string.Empty);

		// Token: 0x04009050 RID: 36944
		public readonly EPlatformIdentifier PlatformIdentifier;

		// Token: 0x04009051 RID: 36945
		public readonly string LobbyId;
	}
}
