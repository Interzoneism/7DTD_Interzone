using System;
using UnityEngine.Scripting;

// Token: 0x02000717 RID: 1815
[Preserve]
public class NetPackageDecoResetWorldChunk : NetPackage
{
	// Token: 0x17000568 RID: 1384
	// (get) Token: 0x06003557 RID: 13655 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003558 RID: 13656 RVA: 0x00163818 File Offset: 0x00161A18
	public NetPackageDecoResetWorldChunk Setup(long _chunkKey)
	{
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.ms);
			pooledBinaryWriter.Write(_chunkKey);
		}
		return this;
	}

	// Token: 0x06003559 RID: 13657 RVA: 0x00163864 File Offset: 0x00161A64
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageDecoResetWorldChunk()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
	}

	// Token: 0x0600355A RID: 13658 RVA: 0x0016389C File Offset: 0x00161A9C
	public override void read(PooledBinaryReader _br)
	{
		int length = _br.ReadInt32();
		StreamUtils.StreamCopy(_br.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x001638C4 File Offset: 0x00161AC4
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((int)this.ms.Length);
		this.ms.WriteTo(_bw.BaseStream);
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x001638F0 File Offset: 0x00161AF0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			PooledExpandableMemoryStream obj = this.ms;
			lock (obj)
			{
				pooledBinaryReader.SetBaseStream(this.ms);
				this.ms.Position = 0L;
				long worldChunkKey = pooledBinaryReader.ReadInt64();
				DecoManager.Instance.ResetDecosForWorldChunk(worldChunkKey);
			}
		}
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x00163978 File Offset: 0x00161B78
	public override int GetLength()
	{
		return (int)this.ms.Length;
	}

	// Token: 0x04002B84 RID: 11140
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms = MemoryPools.poolMemoryStream.AllocSync(true);
}
