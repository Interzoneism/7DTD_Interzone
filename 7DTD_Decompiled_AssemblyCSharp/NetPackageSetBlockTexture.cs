using System;
using UnityEngine.Scripting;

// Token: 0x0200078F RID: 1935
[Preserve]
public class NetPackageSetBlockTexture : NetPackage, IMemoryPoolableObject
{
	// Token: 0x06003827 RID: 14375 RVA: 0x0016E81A File Offset: 0x0016CA1A
	public NetPackageSetBlockTexture Setup(Vector3i _blockPos, BlockFace _blockFace, int _idx, int _playerIdThatChanged, byte _channel)
	{
		this.blockPos = _blockPos;
		this.blockFace = _blockFace;
		this.idx = (byte)_idx;
		this.playerIdThatChanged = _playerIdThatChanged;
		this.channel = _channel;
		return this;
	}

	// Token: 0x06003828 RID: 14376 RVA: 0x0016E843 File Offset: 0x0016CA43
	public override void read(PooledBinaryReader _br)
	{
		this.blockPos = StreamUtils.ReadVector3i(_br);
		this.blockFace = (BlockFace)_br.ReadByte();
		this.idx = _br.ReadByte();
		this.playerIdThatChanged = _br.ReadInt32();
		this.channel = _br.ReadByte();
	}

	// Token: 0x06003829 RID: 14377 RVA: 0x0016E884 File Offset: 0x0016CA84
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		StreamUtils.Write(_bw, this.blockPos);
		_bw.Write((byte)this.blockFace);
		_bw.Write(this.idx);
		_bw.Write(this.playerIdThatChanged);
		_bw.Write(this.channel);
	}

	// Token: 0x0600382A RID: 14378 RVA: 0x0016E8D4 File Offset: 0x0016CAD4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world != null && _world.ChunkClusters[0] != null)
		{
			GameManager.Instance.SetBlockTextureClient(this.blockPos, this.blockFace, (int)this.idx, this.channel);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			NetPackageSetBlockTexture package = NetPackageManager.GetPackage<NetPackageSetBlockTexture>().Setup(this.blockPos, this.blockFace, (int)this.idx, this.playerIdThatChanged, this.channel);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, this.playerIdThatChanged, -1, null, 192, false);
		}
	}

	// Token: 0x0600382B RID: 14379 RVA: 0x000DCA80 File Offset: 0x000DAC80
	public override int GetLength()
	{
		return 19;
	}

	// Token: 0x0600382C RID: 14380 RVA: 0x00002914 File Offset: 0x00000B14
	public void Reset()
	{
	}

	// Token: 0x0600382D RID: 14381 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x0600382E RID: 14382 RVA: 0x0015E645 File Offset: 0x0015C845
	public static int GetPoolSize()
	{
		return 500;
	}

	// Token: 0x04002D8B RID: 11659
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04002D8C RID: 11660
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockFace blockFace;

	// Token: 0x04002D8D RID: 11661
	[PublicizedFrom(EAccessModifier.Private)]
	public byte idx;

	// Token: 0x04002D8E RID: 11662
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerIdThatChanged;

	// Token: 0x04002D8F RID: 11663
	[PublicizedFrom(EAccessModifier.Private)]
	public byte channel;
}
