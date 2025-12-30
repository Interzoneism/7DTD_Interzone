using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200071C RID: 1820
[Preserve]
public class NetPackageEAC : NetPackage
{
	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x0600357D RID: 13693 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600357E RID: 13694 RVA: 0x00163F76 File Offset: 0x00162176
	public NetPackageEAC Setup(int _size, byte[] _data)
	{
		this.data = new byte[_size];
		Array.Copy(_data, this.data, _size);
		return this;
	}

	// Token: 0x0600357F RID: 13695 RVA: 0x00163F94 File Offset: 0x00162194
	public override void read(PooledBinaryReader _reader)
	{
		int num = _reader.ReadInt32();
		this.data = new byte[num];
		for (int i = 0; i < this.data.Length; i++)
		{
			this.data[i] = _reader.ReadByte();
		}
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x00163FD8 File Offset: 0x001621D8
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.data.Length);
		for (int i = 0; i < this.data.Length; i++)
		{
			_writer.Write(this.data[i]);
		}
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x0016401C File Offset: 0x0016221C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
			if (antiCheatServer == null)
			{
				return;
			}
			antiCheatServer.HandleMessageFromClient(base.Sender, this.data);
			return;
		}
		else
		{
			IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
			if (antiCheatClient == null)
			{
				return;
			}
			antiCheatClient.HandleMessageFromServer(this.data);
			return;
		}
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x00164070 File Offset: 0x00162270
	public override int GetLength()
	{
		return 4 + ((this.data != null) ? this.data.Length : 0);
	}

	// Token: 0x04002B90 RID: 11152
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}
