using System;
using UnityEngine.Scripting;

// Token: 0x0200078A RID: 1930
[Preserve]
public class NetPackageRequestToSpawnPlayer : NetPackage
{
	// Token: 0x170005B1 RID: 1457
	// (get) Token: 0x0600380E RID: 14350 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x0016E4B0 File Offset: 0x0016C6B0
	public NetPackageRequestToSpawnPlayer Setup(int _chunkViewDim, PlayerProfile _playerProfile, int _nearEntityId)
	{
		this.chunkViewDim = _chunkViewDim;
		this.playerProfile = _playerProfile;
		this.nearEntityId = _nearEntityId;
		return this;
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x0016E4C8 File Offset: 0x0016C6C8
	public override void read(PooledBinaryReader _reader)
	{
		this.chunkViewDim = (int)_reader.ReadInt16();
		this.playerProfile = PlayerProfile.Read(_reader);
		this.nearEntityId = _reader.ReadInt32();
	}

	// Token: 0x06003811 RID: 14353 RVA: 0x0016E4EE File Offset: 0x0016C6EE
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((short)this.chunkViewDim);
		this.playerProfile.Write(_writer);
		_writer.Write(this.nearEntityId);
	}

	// Token: 0x06003812 RID: 14354 RVA: 0x0016E51C File Offset: 0x0016C71C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.RequestToSpawnPlayer(base.Sender, this.chunkViewDim, this.playerProfile, this.nearEntityId);
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x0015DCEC File Offset: 0x0015BEEC
	public override int GetLength()
	{
		return 50;
	}

	// Token: 0x04002D7E RID: 11646
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkViewDim;

	// Token: 0x04002D7F RID: 11647
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerProfile playerProfile;

	// Token: 0x04002D80 RID: 11648
	[PublicizedFrom(EAccessModifier.Private)]
	public int nearEntityId;
}
