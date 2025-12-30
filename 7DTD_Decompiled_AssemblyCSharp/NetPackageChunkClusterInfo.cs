using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200070C RID: 1804
[Preserve]
public class NetPackageChunkClusterInfo : NetPackage
{
	// Token: 0x1700055C RID: 1372
	// (get) Token: 0x0600350E RID: 13582 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x0016281C File Offset: 0x00160A1C
	public NetPackageChunkClusterInfo Setup(ChunkCluster _chunkCluster)
	{
		this.index = _chunkCluster.ClusterIdx;
		this.name = _chunkCluster.Name;
		this.cMinPos = _chunkCluster.ChunkMinPos;
		this.cMaxPos = _chunkCluster.ChunkMaxPos;
		this.bInfinite = !_chunkCluster.IsFixedSize;
		this.pos = _chunkCluster.Position;
		return this;
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x00162878 File Offset: 0x00160A78
	public override void read(PooledBinaryReader _br)
	{
		this.index = (int)_br.ReadUInt16();
		this.name = _br.ReadString();
		this.cMinPos = new Vector2i(_br.ReadInt32(), _br.ReadInt32());
		this.cMaxPos = new Vector2i(_br.ReadInt32(), _br.ReadInt32());
		this.bInfinite = _br.ReadBoolean();
		this.pos = StreamUtils.ReadVector3(_br);
	}

	// Token: 0x06003511 RID: 13585 RVA: 0x001628E4 File Offset: 0x00160AE4
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((ushort)this.index);
		_bw.Write(this.name);
		_bw.Write(this.cMinPos.x);
		_bw.Write(this.cMinPos.y);
		_bw.Write(this.cMaxPos.x);
		_bw.Write(this.cMaxPos.y);
		_bw.Write(this.bInfinite);
		StreamUtils.Write(_bw, this.pos);
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x0016296D File Offset: 0x00160B6D
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.ChunkClusterInfo(this.name, this.index, this.bInfinite, this.cMinPos, this.cMaxPos, this.pos);
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x00162999 File Offset: 0x00160B99
	public override int GetLength()
	{
		return 40;
	}

	// Token: 0x04002B4C RID: 11084
	[PublicizedFrom(EAccessModifier.Private)]
	public int index;

	// Token: 0x04002B4D RID: 11085
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04002B4E RID: 11086
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i cMinPos;

	// Token: 0x04002B4F RID: 11087
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i cMaxPos;

	// Token: 0x04002B50 RID: 11088
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bInfinite;

	// Token: 0x04002B51 RID: 11089
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 pos;
}
