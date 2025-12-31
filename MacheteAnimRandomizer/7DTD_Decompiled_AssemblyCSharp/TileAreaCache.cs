using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x02000A5D RID: 2653
public class TileAreaCache<[IsUnmanaged] T> : ITileArea<T[,]> where T : struct, ValueType
{
	// Token: 0x060050C8 RID: 20680 RVA: 0x00201796 File Offset: 0x001FF996
	public TileAreaCache(TileAreaConfig _config, TileFile<T> _tileFile, int _cacheMax)
	{
		this.tilesDatabase = _tileFile;
		this.config = _config;
		this.cacheMax = _cacheMax;
	}

	// Token: 0x17000838 RID: 2104
	// (get) Token: 0x060050C9 RID: 20681 RVA: 0x002017C9 File Offset: 0x001FF9C9
	public TileAreaConfig Config
	{
		get
		{
			return this.config;
		}
	}

	// Token: 0x17000839 RID: 2105
	public T[,] this[uint _key]
	{
		get
		{
			T[,] result;
			if (this.cache.TryGetValue(_key, out result))
			{
				this.PromoteEntry(_key);
				return result;
			}
			int tileXPos = TileAreaUtils.GetTileXPos(_key);
			int tileZPos = TileAreaUtils.GetTileZPos(_key);
			return this.Cache(_key, tileXPos, tileZPos);
		}
	}

	// Token: 0x1700083A RID: 2106
	public T[,] this[int _tileX, int _tileZ]
	{
		get
		{
			this.config.checkCoordinates(ref _tileX, ref _tileZ);
			uint key = TileAreaUtils.MakeKey(_tileX, _tileZ);
			T[,] result;
			if (this.cache.TryGetValue(key, out result))
			{
				this.PromoteEntry(key);
				return result;
			}
			return this.Cache(key, _tileX, _tileZ);
		}
	}

	// Token: 0x060050CC RID: 20684 RVA: 0x0020185C File Offset: 0x001FFA5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PromoteEntry(uint _key)
	{
		for (LinkedListNode<uint> linkedListNode = this.cacheQueue.First; linkedListNode != this.cacheQueue.Last; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value == _key)
			{
				this.cacheQueue.Remove(linkedListNode);
				this.cacheQueue.AddFirst(linkedListNode);
				return;
			}
		}
	}

	// Token: 0x060050CD RID: 20685 RVA: 0x002018B0 File Offset: 0x001FFAB0
	[PublicizedFrom(EAccessModifier.Private)]
	public T[,] Cache(uint _key, int _tileX, int _tileZ)
	{
		int tileX = _tileX - this.config.tileStart.x;
		int tileZ = _tileZ - this.config.tileStart.y;
		if (!this.tilesDatabase.IsInDatabase(tileX, tileZ))
		{
			return null;
		}
		LinkedListNode<uint> linkedListNode = null;
		T[,] array = null;
		if (this.cacheQueue.Count >= this.cacheMax)
		{
			linkedListNode = this.cacheQueue.Last;
			this.cacheQueue.Remove(linkedListNode);
			array = this.cache[linkedListNode.Value];
			this.cache.Remove(linkedListNode.Value);
		}
		this.tilesDatabase.LoadTile(tileX, tileZ, ref array);
		this.cache.Add(_key, array);
		if (linkedListNode != null)
		{
			linkedListNode.Value = _key;
			this.cacheQueue.AddFirst(linkedListNode);
		}
		else
		{
			this.cacheQueue.AddFirst(_key);
		}
		return array;
	}

	// Token: 0x060050CE RID: 20686 RVA: 0x00201989 File Offset: 0x001FFB89
	public void Cleanup()
	{
		if (this.tilesDatabase != null)
		{
			this.tilesDatabase.Dispose();
			this.tilesDatabase = null;
		}
	}

	// Token: 0x04003DDD RID: 15837
	[PublicizedFrom(EAccessModifier.Private)]
	public TileFile<T> tilesDatabase;

	// Token: 0x04003DDE RID: 15838
	[PublicizedFrom(EAccessModifier.Private)]
	public TileAreaConfig config;

	// Token: 0x04003DDF RID: 15839
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<uint, T[,]> cache = new Dictionary<uint, T[,]>();

	// Token: 0x04003DE0 RID: 15840
	[PublicizedFrom(EAccessModifier.Private)]
	public LinkedList<uint> cacheQueue = new LinkedList<uint>();

	// Token: 0x04003DE1 RID: 15841
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheMax;
}
