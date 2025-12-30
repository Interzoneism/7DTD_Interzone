using System;
using UnityEngine.Scripting;

// Token: 0x0200070D RID: 1805
[Preserve]
public class NetPackageChunkRemove : NetPackage
{
	// Token: 0x1700055D RID: 1373
	// (get) Token: 0x06003515 RID: 13589 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int Channel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x0016299D File Offset: 0x00160B9D
	public NetPackageChunkRemove Setup(long _chunkKey)
	{
		this.chunkKey = _chunkKey;
		return this;
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x001629A7 File Offset: 0x00160BA7
	public override void read(PooledBinaryReader _reader)
	{
		this.chunkKey = _reader.ReadInt64();
	}

	// Token: 0x06003518 RID: 13592 RVA: 0x001629B5 File Offset: 0x00160BB5
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.chunkKey);
	}

	// Token: 0x06003519 RID: 13593 RVA: 0x001629CA File Offset: 0x00160BCA
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_callbacks.RemoveChunk(this.chunkKey);
	}

	// Token: 0x0600351A RID: 13594 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x1700055E RID: 1374
	// (get) Token: 0x0600351B RID: 13595 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002B52 RID: 11090
	[PublicizedFrom(EAccessModifier.Private)]
	public long chunkKey;
}
