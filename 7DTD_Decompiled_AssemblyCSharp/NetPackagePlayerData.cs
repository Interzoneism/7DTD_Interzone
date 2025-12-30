using System;
using UnityEngine.Scripting;

// Token: 0x0200076E RID: 1902
[Preserve]
public class NetPackagePlayerData : NetPackage
{
	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x06003753 RID: 14163 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0016AA29 File Offset: 0x00168C29
	public NetPackagePlayerData Setup(EntityPlayer _player)
	{
		this.playerDataFile = new PlayerDataFile();
		if (_player != null)
		{
			this.playerDataFile.FromPlayer(_player);
		}
		return this;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x0016AA4C File Offset: 0x00168C4C
	public override void read(PooledBinaryReader _reader)
	{
		this.playerDataFile = new PlayerDataFile();
		this.playerDataFile.Read(_reader, uint.MaxValue);
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x0016AA66 File Offset: 0x00168C66
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.playerDataFile.Write(_writer);
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x0016AA7B File Offset: 0x00168C7B
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!base.ValidEntityIdForSender(this.playerDataFile.id, false))
		{
			return;
		}
		_callbacks.SavePlayerData(base.Sender, this.playerDataFile);
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x0015DCEC File Offset: 0x0015BEEC
	public override int GetLength()
	{
		return 50;
	}

	// Token: 0x04002CE8 RID: 11496
	[PublicizedFrom(EAccessModifier.Protected)]
	public PlayerDataFile playerDataFile;
}
