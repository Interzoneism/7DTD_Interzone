using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000A8E RID: 2702
public class WorldBlockTicker
{
	// Token: 0x06005362 RID: 21346 RVA: 0x00216FB8 File Offset: 0x002151B8
	public WorldBlockTicker(World _world)
	{
		this.world = _world;
	}

	// Token: 0x06005363 RID: 21347 RVA: 0x00217010 File Offset: 0x00215210
	public void Cleanup()
	{
		object obj = this.lockObject;
		lock (obj)
		{
			this.scheduledTicksSorted.Clear();
			this.scheduledTicksDict.Clear();
			this.chunkToScheduledTicks.Clear();
		}
	}

	// Token: 0x06005364 RID: 21348 RVA: 0x0021706C File Offset: 0x0021526C
	public void Tick(ArraySegment<long> _activeChunks, EntityPlayer _thePlayer, GameRandom _rnd)
	{
		if (!GameManager.bTickingActive)
		{
			return;
		}
		if (!this.world.IsRemote())
		{
			this.tickScheduled(_rnd);
		}
		if (!this.world.IsRemote())
		{
			this.tickRandom(_activeChunks, _rnd);
		}
	}

	// Token: 0x06005365 RID: 21349 RVA: 0x002170A0 File Offset: 0x002152A0
	public void AddScheduledBlockUpdate(int _clrIdx, Vector3i _pos, int _blockId, ulong _ticks)
	{
		if (_blockId == 0)
		{
			return;
		}
		WorldBlockTickerEntry worldBlockTickerEntry = new WorldBlockTickerEntry(_clrIdx, _pos, _blockId, _ticks + GameTimer.Instance.ticks);
		object obj = this.lockObject;
		lock (obj)
		{
			if (this.scheduledTicksDict.ContainsKey(worldBlockTickerEntry.GetHashCode()))
			{
				this.remove(_clrIdx, _pos, _blockId);
			}
			this.add(worldBlockTickerEntry);
		}
	}

	// Token: 0x06005366 RID: 21350 RVA: 0x00217118 File Offset: 0x00215318
	[PublicizedFrom(EAccessModifier.Private)]
	public void add(WorldBlockTickerEntry _wbte)
	{
		this.scheduledTicksDict.Add(_wbte.GetHashCode(), _wbte);
		this.scheduledTicksSorted.Add(_wbte, null);
		HashSet<WorldBlockTickerEntry> hashSet = this.chunkToScheduledTicks[_wbte.GetChunkKey()];
		if (hashSet == null)
		{
			hashSet = new HashSet<WorldBlockTickerEntry>();
			this.chunkToScheduledTicks[_wbte.GetChunkKey()] = hashSet;
		}
		hashSet.Add(_wbte);
	}

	// Token: 0x06005367 RID: 21351 RVA: 0x0021717C File Offset: 0x0021537C
	[PublicizedFrom(EAccessModifier.Private)]
	public void execute(WorldBlockTickerEntry _wbte, GameRandom _rnd, ulong _ticksIfLoaded)
	{
		BlockValue block = this.world.GetBlock(_wbte.clrIdx, _wbte.worldPos);
		if (block.type == _wbte.blockID)
		{
			block.Block.UpdateTick(this.world, _wbte.clrIdx, _wbte.worldPos, block, false, _ticksIfLoaded, _rnd);
		}
	}

	// Token: 0x06005368 RID: 21352 RVA: 0x002171D4 File Offset: 0x002153D4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool tickScheduled(GameRandom _rnd)
	{
		object obj = this.lockObject;
		int num;
		lock (obj)
		{
			num = this.scheduledTicksSorted.Count;
			if (num != this.scheduledTicksDict.Count)
			{
				throw new Exception("WBT: Invalid dict state");
			}
		}
		if (num > 100)
		{
			num = 100;
		}
		for (int i = 0; i < num; i++)
		{
			obj = this.lockObject;
			WorldBlockTickerEntry worldBlockTickerEntry;
			lock (obj)
			{
				if (this.scheduledTicksSorted.Count == 0)
				{
					Log.Warning("WorldBlockTicker tickScheduled count 0");
					break;
				}
				worldBlockTickerEntry = (WorldBlockTickerEntry)this.scheduledTicksSorted.GetKey(0);
				if (worldBlockTickerEntry.scheduledTime > GameTimer.Instance.ticks)
				{
					break;
				}
				this.scheduledTicksSorted.Remove(worldBlockTickerEntry);
				this.scheduledTicksDict.Remove(worldBlockTickerEntry.GetHashCode());
				HashSet<WorldBlockTickerEntry> hashSet = this.chunkToScheduledTicks[worldBlockTickerEntry.GetChunkKey()];
				if (hashSet != null)
				{
					hashSet.Remove(worldBlockTickerEntry);
				}
			}
			if (!this.world.IsChunkAreaLoaded(worldBlockTickerEntry.worldPos.x, worldBlockTickerEntry.worldPos.y, worldBlockTickerEntry.worldPos.z))
			{
				int chunkX = World.toChunkXZ(worldBlockTickerEntry.worldPos.x);
				int chunkZ = World.toChunkXZ(worldBlockTickerEntry.worldPos.z);
				if (this.world.GetChunkSync(chunkX, chunkZ) != null)
				{
					this.AddScheduledBlockUpdate(worldBlockTickerEntry.clrIdx, worldBlockTickerEntry.worldPos, worldBlockTickerEntry.blockID, (ulong)(30 + _rnd.RandomRange(0, 15)));
				}
			}
			else
			{
				this.execute(worldBlockTickerEntry, _rnd, 0UL);
			}
		}
		return this.scheduledTicksSorted.Count != 0;
	}

	// Token: 0x06005369 RID: 21353 RVA: 0x002173A8 File Offset: 0x002155A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void tickRandom(ArraySegment<long> _activeChunkSet, GameRandom _rnd)
	{
		if (this.randomTickIndex >= this.randomTickChunkKeys.Count)
		{
			this.randomTickChunkKeys.Clear();
			if (_activeChunkSet.Count > this.randomTickChunkKeys.Capacity)
			{
				this.randomTickChunkKeys.Capacity = _activeChunkSet.Count * 2;
			}
			int num = _activeChunkSet.Offset + _activeChunkSet.Count;
			for (int i = _activeChunkSet.Offset; i < num; i++)
			{
				this.randomTickChunkKeys.Add(_activeChunkSet.Array[i]);
			}
			this.randomTickCountPerFrame = Math.Max(_activeChunkSet.Count / 100, 1);
			this.randomTickIndex = 0;
		}
		int num2 = 0;
		while (this.randomTickIndex < this.randomTickChunkKeys.Count && num2 < this.randomTickCountPerFrame)
		{
			long key = this.randomTickChunkKeys[this.randomTickIndex];
			Chunk chunkSync = this.world.ChunkCache.GetChunkSync(key);
			this.tickChunkRandom(chunkSync, _rnd);
			this.randomTickIndex++;
			num2++;
		}
	}

	// Token: 0x0600536A RID: 21354 RVA: 0x002174B4 File Offset: 0x002156B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void tickChunkRandom(Chunk chunk, GameRandom _rnd)
	{
		if (chunk == null)
		{
			return;
		}
		if (chunk.NeedsLightCalculation)
		{
			return;
		}
		if (GameTimer.Instance.ticks - chunk.LastTimeRandomTicked < 1200UL)
		{
			return;
		}
		ulong ticksIfLoaded = GameTimer.Instance.ticks - chunk.LastTimeRandomTicked;
		chunk.LastTimeRandomTicked = GameTimer.Instance.ticks;
		DictionaryKeyList<Vector3i, int> tickedBlocks = chunk.GetTickedBlocks();
		DictionaryKeyList<Vector3i, int> obj = tickedBlocks;
		lock (obj)
		{
			for (int i = tickedBlocks.list.Count - 1; i >= 0; i--)
			{
				Vector3i vector3i = tickedBlocks.list[i];
				BlockValue block = chunk.GetBlock(World.toBlockXZ(vector3i.x), vector3i.y, World.toBlockXZ(vector3i.z));
				if (this.scheduledTicksDict.Count == 0 || !this.scheduledTicksDict.ContainsKey(WorldBlockTickerEntry.ToHashCode(chunk.ClrIdx, vector3i, block.type)))
				{
					block.Block.UpdateTick(this.world, chunk.ClrIdx, vector3i, block, true, ticksIfLoaded, _rnd);
				}
			}
		}
	}

	// Token: 0x0600536B RID: 21355 RVA: 0x002175E4 File Offset: 0x002157E4
	public void OnChunkAdded(WorldBase _world, Chunk _c, GameRandom _rnd)
	{
		ChunkCustomData chunkCustomData;
		if (!_c.ChunkCustomData.dict.TryGetValue("wbt.sch", out chunkCustomData) || chunkCustomData == null)
		{
			return;
		}
		_c.ChunkCustomData.Remove("wbt.sch");
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			pooledBinaryReader.SetBaseStream(new MemoryStream(chunkCustomData.data));
			int num = (int)pooledBinaryReader.ReadUInt16();
			int version = (int)pooledBinaryReader.ReadByte();
			for (int i = 0; i < num; i++)
			{
				WorldBlockTickerEntry worldBlockTickerEntry = WorldBlockTickerEntry.Read(pooledBinaryReader, _c.X, _c.Z, version);
				object obj = this.lockObject;
				lock (obj)
				{
					if (!this.scheduledTicksDict.ContainsKey(worldBlockTickerEntry.GetHashCode()))
					{
						if (worldBlockTickerEntry.scheduledTime > GameTimer.Instance.ticks)
						{
							this.add(worldBlockTickerEntry);
						}
						else
						{
							this.execute(worldBlockTickerEntry, _rnd, GameTimer.Instance.ticks - worldBlockTickerEntry.scheduledTime);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600536C RID: 21356 RVA: 0x0021770C File Offset: 0x0021590C
	public void OnChunkRemoved(Chunk _c)
	{
		this.addScheduleInformationToChunk(_c, true);
	}

	// Token: 0x0600536D RID: 21357 RVA: 0x00217716 File Offset: 0x00215916
	public void OnChunkBeforeSave(Chunk _c)
	{
		this.addScheduleInformationToChunk(_c, false);
	}

	// Token: 0x0600536E RID: 21358 RVA: 0x00217720 File Offset: 0x00215920
	[PublicizedFrom(EAccessModifier.Private)]
	public void addScheduleInformationToChunk(Chunk _c, bool _bChunkIsRemoved)
	{
		HashSet<WorldBlockTickerEntry> hashSet = this.chunkToScheduledTicks[_c.Key];
		object obj = this.lockObject;
		lock (obj)
		{
			if (hashSet == null)
			{
				return;
			}
			if (_bChunkIsRemoved)
			{
				this.chunkToScheduledTicks.Remove(_c.Key);
			}
		}
		if (hashSet.Count == 0)
		{
			return;
		}
		ChunkCustomData chunkCustomData = new ChunkCustomData("wbt.sch", ulong.MaxValue, false);
		using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
		{
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				pooledBinaryWriter.Write((ushort)hashSet.Count);
				pooledBinaryWriter.Write(1);
				foreach (WorldBlockTickerEntry worldBlockTickerEntry in hashSet)
				{
					worldBlockTickerEntry.Write(pooledBinaryWriter);
					if (_bChunkIsRemoved)
					{
						obj = this.lockObject;
						lock (obj)
						{
							this.scheduledTicksSorted.Remove(worldBlockTickerEntry);
							this.scheduledTicksDict.Remove(worldBlockTickerEntry.GetHashCode());
						}
					}
				}
			}
			chunkCustomData.data = pooledExpandableMemoryStream.ToArray();
		}
		_c.ChunkCustomData.Set("wbt.sch", chunkCustomData);
		_c.isModified = true;
	}

	// Token: 0x0600536F RID: 21359 RVA: 0x002178BC File Offset: 0x00215ABC
	public int GetCount()
	{
		object obj = this.lockObject;
		int count;
		lock (obj)
		{
			count = this.scheduledTicksDict.Count;
		}
		return count;
	}

	// Token: 0x06005370 RID: 21360 RVA: 0x00217904 File Offset: 0x00215B04
	public void InvalidateScheduledBlockUpdate(int _clrIdx, Vector3i _pos, int _blockID)
	{
		if (_blockID == 0)
		{
			return;
		}
		this.remove(_clrIdx, _pos, _blockID);
	}

	// Token: 0x06005371 RID: 21361 RVA: 0x00217914 File Offset: 0x00215B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void remove(int _clrIdx, Vector3i _pos, int _blockID)
	{
		object obj = this.lockObject;
		lock (obj)
		{
			int key = WorldBlockTickerEntry.ToHashCode(_clrIdx, _pos, _blockID);
			WorldBlockTickerEntry worldBlockTickerEntry = null;
			if (this.scheduledTicksDict.ContainsKey(key))
			{
				worldBlockTickerEntry = this.scheduledTicksDict[key];
				this.scheduledTicksDict.Remove(key);
				if (worldBlockTickerEntry != null)
				{
					this.scheduledTicksSorted.Remove(worldBlockTickerEntry);
				}
			}
			long v = WorldChunkCache.MakeChunkKey(World.toChunkXZ(_pos.x), World.toChunkXZ(_pos.z), _clrIdx);
			HashSet<WorldBlockTickerEntry> hashSet = this.chunkToScheduledTicks[v];
			if (hashSet != null)
			{
				hashSet.Remove(worldBlockTickerEntry);
			}
		}
	}

	// Token: 0x04003F94 RID: 16276
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object lockObject = new object();

	// Token: 0x04003F95 RID: 16277
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly SortedList scheduledTicksSorted = new SortedList(new WorldBlockTicker.EntryComparer());

	// Token: 0x04003F96 RID: 16278
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, WorldBlockTickerEntry> scheduledTicksDict = new Dictionary<int, WorldBlockTickerEntry>();

	// Token: 0x04003F97 RID: 16279
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DictionarySave<long, HashSet<WorldBlockTickerEntry>> chunkToScheduledTicks = new DictionarySave<long, HashSet<WorldBlockTickerEntry>>();

	// Token: 0x04003F98 RID: 16280
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly World world;

	// Token: 0x04003F99 RID: 16281
	[PublicizedFrom(EAccessModifier.Private)]
	public int randomTickIndex;

	// Token: 0x04003F9A RID: 16282
	[PublicizedFrom(EAccessModifier.Private)]
	public int randomTickCountPerFrame;

	// Token: 0x04003F9B RID: 16283
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<long> randomTickChunkKeys = new List<long>();

	// Token: 0x02000A8F RID: 2703
	[PublicizedFrom(EAccessModifier.Private)]
	public class EntryComparer : IComparer
	{
		// Token: 0x06005372 RID: 21362 RVA: 0x002179C8 File Offset: 0x00215BC8
		public int Compare(object _o1, object _o2)
		{
			WorldBlockTickerEntry worldBlockTickerEntry = (WorldBlockTickerEntry)_o1;
			WorldBlockTickerEntry worldBlockTickerEntry2 = (WorldBlockTickerEntry)_o2;
			if (worldBlockTickerEntry.scheduledTime < worldBlockTickerEntry2.scheduledTime)
			{
				return -1;
			}
			if (worldBlockTickerEntry.scheduledTime > worldBlockTickerEntry2.scheduledTime)
			{
				return 1;
			}
			if (worldBlockTickerEntry.tickEntryID < worldBlockTickerEntry2.tickEntryID)
			{
				return -1;
			}
			if (worldBlockTickerEntry.tickEntryID > worldBlockTickerEntry2.tickEntryID)
			{
				return 1;
			}
			return 0;
		}
	}
}
