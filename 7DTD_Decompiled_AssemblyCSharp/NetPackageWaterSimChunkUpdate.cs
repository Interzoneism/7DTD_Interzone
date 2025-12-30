using System;
using UnityEngine.Scripting;

// Token: 0x020007AD RID: 1965
[Preserve]
public class NetPackageWaterSimChunkUpdate : NetPackage, IMemoryPoolableObject
{
	// Token: 0x170005B8 RID: 1464
	// (get) Token: 0x060038D5 RID: 14549 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x00171069 File Offset: 0x0016F269
	public static int GetPoolSize()
	{
		return 200;
	}

	// Token: 0x060038D7 RID: 14551 RVA: 0x00171070 File Offset: 0x0016F270
	public void SetupForSend(Chunk chunk)
	{
		this.ms = MemoryPools.poolMemoryStream.AllocSync(true);
		this.sendWriter = MemoryPools.poolBinaryWriter.AllocSync(true);
		this.ms.Position = 0L;
		this.sendWriter.SetBaseStream(this.ms);
		this.sendWriter.Write(chunk.X);
		this.sendWriter.Write(chunk.Z);
		this.lengthStreamPos = this.ms.Position;
		this.sendWriter.Write(0);
	}

	// Token: 0x060038D8 RID: 14552 RVA: 0x001710FC File Offset: 0x0016F2FC
	public void AddChange(ushort _voxelIndex, WaterValue _newValue)
	{
		this.sendWriter.Write(_voxelIndex);
		_newValue.Write(this.sendWriter);
		this.numVoxelUpdates++;
	}

	// Token: 0x060038D9 RID: 14553 RVA: 0x00171128 File Offset: 0x0016F328
	public void FinalizeSend()
	{
		this.ms.Position = this.lengthStreamPos;
		this.sendWriter.Write(this.numVoxelUpdates);
		this.sendLength = (int)this.ms.Length;
		this.sendBytes = MemoryPools.poolByte.Alloc(this.sendLength);
		this.ms.Position = 0L;
		this.ms.Read(this.sendBytes, 0, this.sendLength);
		MemoryPools.poolBinaryWriter.FreeSync(this.sendWriter);
		this.sendWriter = null;
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
		this.ms = null;
	}

	// Token: 0x060038DA RID: 14554 RVA: 0x001711D4 File Offset: 0x0016F3D4
	public override void read(PooledBinaryReader _br)
	{
		this.ms = MemoryPools.poolMemoryStream.AllocSync(true);
		this.ms.Position = 0L;
		int length = _br.ReadInt32();
		StreamUtils.StreamCopy(_br.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x0017121A File Offset: 0x0016F41A
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.sendLength);
		_bw.Write(this.sendBytes, 0, this.sendLength);
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x00171244 File Offset: 0x0016F444
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
		{
			this.ms.Position = 0L;
			pooledBinaryReader.SetBaseStream(this.ms);
			int x = pooledBinaryReader.ReadInt32();
			int y = pooledBinaryReader.ReadInt32();
			long chunkKey = WorldChunkCache.MakeChunkKey(x, y);
			WaterSimulationNative.Instance.changeApplier.GetChangeWriter(chunkKey);
			using (WaterSimulationApplyChanges.ChangesForChunk.Writer changeWriter = WaterSimulationNative.Instance.changeApplier.GetChangeWriter(chunkKey))
			{
				int num = pooledBinaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int voxelIndex = (int)pooledBinaryReader.ReadUInt16();
					WaterValue waterValue = WaterValue.FromStream(pooledBinaryReader);
					changeWriter.RecordChange(voxelIndex, waterValue);
				}
			}
		}
	}

	// Token: 0x060038DD RID: 14557 RVA: 0x00171318 File Offset: 0x0016F518
	public override int GetLength()
	{
		return this.sendLength + 4;
	}

	// Token: 0x060038DE RID: 14558 RVA: 0x00171324 File Offset: 0x0016F524
	public void Reset()
	{
		if (this.sendWriter != null)
		{
			MemoryPools.poolBinaryWriter.FreeSync(this.sendWriter);
			this.sendWriter = null;
		}
		if (this.ms != null)
		{
			MemoryPools.poolMemoryStream.FreeSync(this.ms);
			this.ms = null;
		}
		if (this.sendBytes != null)
		{
			MemoryPools.poolByte.Free(this.sendBytes);
			this.sendBytes = null;
		}
		this.lengthStreamPos = 0L;
		this.numVoxelUpdates = 0;
		this.sendLength = 0;
	}

	// Token: 0x060038DF RID: 14559 RVA: 0x001713A4 File Offset: 0x0016F5A4
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x04002DF3 RID: 11763
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms;

	// Token: 0x04002DF4 RID: 11764
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledBinaryWriter sendWriter;

	// Token: 0x04002DF5 RID: 11765
	[PublicizedFrom(EAccessModifier.Private)]
	public long lengthStreamPos;

	// Token: 0x04002DF6 RID: 11766
	[PublicizedFrom(EAccessModifier.Private)]
	public int numVoxelUpdates;

	// Token: 0x04002DF7 RID: 11767
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] sendBytes;

	// Token: 0x04002DF8 RID: 11768
	[PublicizedFrom(EAccessModifier.Private)]
	public int sendLength;
}
