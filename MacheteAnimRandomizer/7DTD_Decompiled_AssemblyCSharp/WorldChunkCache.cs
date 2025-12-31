using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

// Token: 0x02000A92 RID: 2706
public class WorldChunkCache
{
	// Token: 0x0600537E RID: 21374 RVA: 0x000467D6 File Offset: 0x000449D6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long MakeChunkKey(int x, int y)
	{
		return ((long)y & 16777215L) << 24 | ((long)x & 16777215L);
	}

	// Token: 0x0600537F RID: 21375 RVA: 0x00217BE0 File Offset: 0x00215DE0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long MakeChunkKey(int x, int y, int clrIdx)
	{
		return ((long)clrIdx & 255L) << 56 | ((long)y & 16777215L) << 24 | ((long)x & 16777215L);
	}

	// Token: 0x06005380 RID: 21376 RVA: 0x00217C05 File Offset: 0x00215E05
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long MakeChunkKey(long key, int clrIdx)
	{
		return ((long)clrIdx & 255L) << 56 | (key & 72057594037927935L);
	}

	// Token: 0x06005381 RID: 21377 RVA: 0x00217C1F File Offset: 0x00215E1F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int extractX(long key)
	{
		return (int)((int)key << 8) >> 8;
	}

	// Token: 0x06005382 RID: 21378 RVA: 0x00217C27 File Offset: 0x00215E27
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int extractZ(long key)
	{
		return (int)(key >> 16) >> 8;
	}

	// Token: 0x06005383 RID: 21379 RVA: 0x00217C30 File Offset: 0x00215E30
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i extractXZ(long key)
	{
		Vector2i result;
		result.x = WorldChunkCache.extractX(key);
		result.y = WorldChunkCache.extractZ(key);
		return result;
	}

	// Token: 0x06005384 RID: 21380 RVA: 0x00217C58 File Offset: 0x00215E58
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int extractClrIdx(long key)
	{
		return (int)(key >> 56 & 255L);
	}

	// Token: 0x06005385 RID: 21381 RVA: 0x00217C66 File Offset: 0x00215E66
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Chunk GetChunkSync(int _x, int _y)
	{
		return this.GetChunkSync(WorldChunkCache.MakeChunkKey(_x, _y));
	}

	// Token: 0x06005386 RID: 21382 RVA: 0x00217C78 File Offset: 0x00215E78
	public virtual Chunk GetChunkSync(long _key)
	{
		this.sync.EnterReadLock();
		Chunk result;
		this.chunks.dict.TryGetValue(_key & 72057594037927935L, out result);
		this.sync.ExitReadLock();
		return result;
	}

	// Token: 0x06005387 RID: 21383 RVA: 0x00217CBC File Offset: 0x00215EBC
	public virtual void RemoveChunkSync(long _key)
	{
		this.sync.EnterWriteLock();
		Chunk chunk;
		if (this.chunks.dict.TryGetValue(_key & 72057594037927935L, out chunk))
		{
			this.chunks.Remove(_key & 72057594037927935L);
			this.isChunkArrayDirty = true;
		}
		this.chunkKeys.Remove(_key);
		this.isChunkKeysDirty = true;
		this.sync.ExitWriteLock();
		if (chunk != null)
		{
			for (int i = 0; i < this.chunkCallbacks.Count; i++)
			{
				this.chunkCallbacks[i].OnChunkBeforeRemove(chunk);
			}
		}
	}

	// Token: 0x06005388 RID: 21384 RVA: 0x00217D60 File Offset: 0x00215F60
	public void NotifyOnChunkBeforeSave(Chunk _c)
	{
		for (int i = 0; i < this.chunkCallbacks.Count; i++)
		{
			this.chunkCallbacks[i].OnChunkBeforeSave(_c);
		}
	}

	// Token: 0x06005389 RID: 21385 RVA: 0x00217D98 File Offset: 0x00215F98
	public virtual bool AddChunkSync(Chunk _chunk, bool _bOmitCallbacks = false)
	{
		this.ChunkMinPos.x = Utils.FastMin(this.ChunkMinPos.x, _chunk.X);
		this.ChunkMinPos.y = Utils.FastMin(this.ChunkMinPos.y, _chunk.Z);
		this.ChunkMaxPos.x = Utils.FastMax(this.ChunkMaxPos.x, _chunk.X);
		this.ChunkMaxPos.y = Utils.FastMax(this.ChunkMaxPos.y, _chunk.Z);
		this.sync.EnterWriteLock();
		long key = _chunk.Key;
		if (this.chunkKeys.Contains(key))
		{
			this.sync.ExitWriteLock();
			return false;
		}
		this.chunks.Add(key & 72057594037927935L, _chunk);
		this.chunkKeys.Add(key);
		this.isChunkArrayDirty = true;
		this.isChunkKeysDirty = true;
		this.sync.ExitWriteLock();
		int num = 0;
		while (!_bOmitCallbacks && num < this.chunkCallbacks.Count)
		{
			this.chunkCallbacks[num].OnChunkAdded(_chunk);
			num++;
		}
		return true;
	}

	// Token: 0x0600538A RID: 21386 RVA: 0x00217EC4 File Offset: 0x002160C4
	public List<Chunk> GetChunkArrayCopySync()
	{
		this.sync.EnterReadLock();
		if (this.isChunkArrayDirty)
		{
			this.chunkArrayCopy = new List<Chunk>(this.chunks.list.Count);
			this.chunkArrayCopy.AddRange(this.chunks.list);
			this.isChunkArrayDirty = false;
		}
		this.sync.ExitReadLock();
		return this.chunkArrayCopy;
	}

	// Token: 0x0600538B RID: 21387 RVA: 0x00217F31 File Offset: 0x00216131
	public LinkedList<Chunk> GetChunkArray()
	{
		return this.chunks.list;
	}

	// Token: 0x0600538C RID: 21388 RVA: 0x00217F40 File Offset: 0x00216140
	public HashSetLong GetChunkKeysCopySync()
	{
		if (this.isChunkKeysDirty)
		{
			this.sync.EnterReadLock();
			this.chunkKeysCopy.Clear();
			foreach (long item in this.chunkKeys)
			{
				this.chunkKeysCopy.Add(item);
			}
			this.sync.ExitReadLock();
			this.isChunkKeysDirty = false;
		}
		return this.chunkKeysCopy;
	}

	// Token: 0x0600538D RID: 21389 RVA: 0x00217FD4 File Offset: 0x002161D4
	public int Count()
	{
		return this.chunks.list.Count;
	}

	// Token: 0x0600538E RID: 21390 RVA: 0x00217FE6 File Offset: 0x002161E6
	public ReaderWriterLockSlim GetSyncRoot()
	{
		return this.sync;
	}

	// Token: 0x0600538F RID: 21391 RVA: 0x00217FEE File Offset: 0x002161EE
	public virtual bool ContainsChunkSync(long key)
	{
		this.sync.EnterReadLock();
		bool result = this.chunks.dict.ContainsKey(key);
		this.sync.ExitReadLock();
		return result;
	}

	// Token: 0x06005390 RID: 21392 RVA: 0x00218017 File Offset: 0x00216217
	public void AddChunkCallback(IChunkCallback _callback)
	{
		this.chunkCallbacks.Add(_callback);
	}

	// Token: 0x06005391 RID: 21393 RVA: 0x00218025 File Offset: 0x00216225
	public void RemoveChunkCallback(IChunkCallback _callback)
	{
		this.chunkCallbacks.Remove(_callback);
	}

	// Token: 0x06005392 RID: 21394 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Update()
	{
	}

	// Token: 0x06005393 RID: 21395 RVA: 0x00218034 File Offset: 0x00216234
	public virtual void Clear()
	{
		this.sync.EnterWriteLock();
		List<Chunk> list = new List<Chunk>();
		list.AddRange(this.GetChunkArray());
		this.chunks.Clear();
		this.chunkKeys.Clear();
		this.isChunkArrayDirty = true;
		this.isChunkKeysDirty = true;
		this.sync.ExitWriteLock();
		MemoryPools.PoolChunks.FreeSync(list);
	}

	// Token: 0x06005394 RID: 21396 RVA: 0x0021809C File Offset: 0x0021629C
	public bool GetNeighborChunks(Chunk _chunk, Chunk[] neighbours)
	{
		this.sync.EnterReadLock();
		int x = _chunk.X;
		int z = _chunk.Z;
		Chunk chunk = this.GetChunk(x + 1, z);
		if (chunk != null)
		{
			neighbours[0] = chunk;
			chunk = this.GetChunk(x - 1, z);
			if (chunk != null)
			{
				neighbours[1] = chunk;
				chunk = this.GetChunk(x, z + 1);
				if (chunk != null)
				{
					neighbours[2] = chunk;
					chunk = this.GetChunk(x, z - 1);
					if (chunk != null)
					{
						neighbours[3] = chunk;
						chunk = this.GetChunk(x + 1, z + 1);
						if (chunk != null)
						{
							neighbours[4] = chunk;
							chunk = this.GetChunk(x - 1, z - 1);
							if (chunk != null)
							{
								neighbours[5] = chunk;
								chunk = this.GetChunk(x - 1, z + 1);
								if (chunk != null)
								{
									neighbours[6] = chunk;
									chunk = this.GetChunk(x + 1, z - 1);
									if (chunk != null)
									{
										neighbours[7] = chunk;
										this.sync.ExitReadLock();
										return true;
									}
								}
							}
						}
					}
				}
			}
		}
		this.sync.ExitReadLock();
		return false;
	}

	// Token: 0x06005395 RID: 21397 RVA: 0x0021817C File Offset: 0x0021637C
	public bool HasNeighborChunks(Chunk _chunk)
	{
		this.sync.EnterReadLock();
		int x = _chunk.X;
		int z = _chunk.Z;
		if (this.GetChunk(x + 1, z) != null && this.GetChunk(x - 1, z) != null && this.GetChunk(x, z + 1) != null && this.GetChunk(x, z - 1) != null && this.GetChunk(x + 1, z + 1) != null && this.GetChunk(x - 1, z - 1) != null && this.GetChunk(x - 1, z + 1) != null && this.GetChunk(x + 1, z - 1) != null)
		{
			this.sync.ExitReadLock();
			return true;
		}
		this.sync.ExitReadLock();
		return false;
	}

	// Token: 0x06005396 RID: 21398 RVA: 0x00218224 File Offset: 0x00216424
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk GetChunk(int _x, int _y)
	{
		long key = WorldChunkCache.MakeChunkKey(_x, _y);
		Chunk result;
		this.chunks.dict.TryGetValue(key, out result);
		return result;
	}

	// Token: 0x04003FA2 RID: 16290
	public const long InvalidChunk = 9223372036854775807L;

	// Token: 0x04003FA3 RID: 16291
	public Vector2i ChunkMinPos = Vector2i.zero;

	// Token: 0x04003FA4 RID: 16292
	public Vector2i ChunkMaxPos = Vector2i.zero;

	// Token: 0x04003FA5 RID: 16293
	public DictionaryLinkedList<long, Chunk> chunks = new DictionaryLinkedList<long, Chunk>();

	// Token: 0x04003FA6 RID: 16294
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IChunkCallback> chunkCallbacks = new List<IChunkCallback>();

	// Token: 0x04003FA7 RID: 16295
	public HashSetLong chunkKeys = new HashSetLong();

	// Token: 0x04003FA8 RID: 16296
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile bool isChunkKeysDirty = true;

	// Token: 0x04003FA9 RID: 16297
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSetLong chunkKeysCopy = new HashSetLong();

	// Token: 0x04003FAA RID: 16298
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile bool isChunkArrayDirty = true;

	// Token: 0x04003FAB RID: 16299
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Chunk> chunkArrayCopy = new List<Chunk>();

	// Token: 0x04003FAC RID: 16300
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
}
