using System;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200070B RID: 1803
[Preserve]
public class NetPackageChunk : NetPackage
{
	// Token: 0x17000559 RID: 1369
	// (get) Token: 0x06003504 RID: 13572 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int Channel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x1700055A RID: 1370
	// (get) Token: 0x06003505 RID: 13573 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700055B RID: 1371
	// (get) Token: 0x06003506 RID: 13574 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x00162430 File Offset: 0x00160630
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageChunk()
	{
		if (this.chunk != null)
		{
			this.chunk.InProgressNetworking = false;
			this.chunk = null;
		}
		if (this.serializedData != null)
		{
			MemoryPools.poolMS.FreeSync(this.serializedData);
			this.serializedData = null;
		}
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x00162494 File Offset: 0x00160694
	public NetPackageChunk Setup(Chunk _chunk, bool _bOverwriteExisting = false)
	{
		this.chunk = _chunk;
		this.bOverwriteExisting = _bOverwriteExisting;
		this.serializedData = MemoryPools.poolMS.AllocSync(true);
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.serializedData);
			this.chunk.write(pooledBinaryWriter);
		}
		return this;
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x00162504 File Offset: 0x00160704
	public override void read(PooledBinaryReader _reader)
	{
		_reader.ReadByte();
		this.bOverwriteExisting = _reader.ReadBoolean();
		if (this.bOverwriteExisting)
		{
			this.chunkX = (int)_reader.ReadInt16();
			this.chunkY = (int)_reader.ReadInt16();
			this.chunkZ = (int)_reader.ReadInt16();
		}
		this.dataLen = _reader.ReadInt32();
		this.data = _reader.ReadBytes(this.dataLen);
		if (!this.bOverwriteExisting)
		{
			if (this.chunk == null)
			{
				this.chunk = MemoryPools.PoolChunks.AllocSync(true);
			}
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(new MemoryStream(this.data));
				this.chunk.read(pooledBinaryReader, uint.MaxValue);
			}
		}
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x001625D8 File Offset: 0x001607D8
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(0);
		_writer.Write(this.bOverwriteExisting);
		if (this.bOverwriteExisting)
		{
			_writer.Write((short)this.chunk.X);
			_writer.Write((short)this.chunk.Y);
			_writer.Write((short)this.chunk.Z);
		}
		_writer.Write((int)this.serializedData.Length);
		this.serializedData.Position = 0L;
		StreamUtils.StreamCopy(this.serializedData, _writer.BaseStream, null, true);
		MemoryPools.poolMS.FreeSync(this.serializedData);
		this.serializedData = null;
		this.chunk = null;
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x00162690 File Offset: 0x00160890
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			Log.Warning("Received chunk while world is not set up");
			if (this.chunk != null)
			{
				MemoryPools.PoolChunks.FreeSync(this.chunk);
				this.chunk = null;
			}
			return;
		}
		long key = (!this.bOverwriteExisting) ? this.chunk.Key : WorldChunkCache.MakeChunkKey(this.chunkX, this.chunkZ);
		Chunk chunkSync;
		if ((chunkSync = _world.ChunkCache.GetChunkSync(key)) != null && !this.bOverwriteExisting)
		{
			string name = base.GetType().Name;
			string str = ": chunk already loaded ";
			Chunk chunk = this.chunk;
			Log.Error(name + str + ((chunk != null) ? chunk.ToString() : null));
			return;
		}
		if (this.bOverwriteExisting)
		{
			Bounds bounds = Chunk.CalculateAABB(this.chunkX, this.chunkY, this.chunkZ);
			MultiBlockManager.Instance.DeregisterTrackedBlockDatas(bounds);
		}
		if (!this.bOverwriteExisting)
		{
			_world.ChunkCache.AddChunkSync(this.chunk, false);
			this.chunk.NeedsRegeneration = true;
			this.chunk = null;
			return;
		}
		if (chunkSync != null)
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(new MemoryStream(this.data));
				chunkSync.OnUnload(_world);
				_world.ChunkCache.RemoveChunkSync(chunkSync.Key);
				chunkSync.Reset();
				chunkSync.read(pooledBinaryReader, uint.MaxValue);
				_world.ChunkCache.AddChunkSync(chunkSync, false);
				this.data = null;
			}
		}
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x00162808 File Offset: 0x00160A08
	public override int GetLength()
	{
		return 14 + (int)this.serializedData.Length;
	}

	// Token: 0x04002B44 RID: 11076
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk chunk;

	// Token: 0x04002B45 RID: 11077
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledMemoryStream serializedData;

	// Token: 0x04002B46 RID: 11078
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bOverwriteExisting;

	// Token: 0x04002B47 RID: 11079
	[PublicizedFrom(EAccessModifier.Private)]
	public int dataLen;

	// Token: 0x04002B48 RID: 11080
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;

	// Token: 0x04002B49 RID: 11081
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkX;

	// Token: 0x04002B4A RID: 11082
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkY;

	// Token: 0x04002B4B RID: 11083
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkZ;
}
