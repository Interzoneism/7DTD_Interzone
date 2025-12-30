using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x0200099F RID: 2463
public class ChunkAreaBiomeSpawnData
{
	// Token: 0x06004B01 RID: 19201 RVA: 0x001DA450 File Offset: 0x001D8650
	public ChunkAreaBiomeSpawnData(Chunk _chunk, byte _biomeId, ChunkCustomData _ccd)
	{
		this.biomeId = _biomeId;
		this.area = new Rect((float)(_chunk.X * 16), (float)(_chunk.Z * 16), 80f, 80f);
		this.chunk = _chunk;
		this.ccd = _ccd;
		this.ccd.TriggerWriteDataDelegate = new ChunkCustomData.TriggerWriteData(this.BeforeWrite);
		if (this.ccd.data != null)
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(new MemoryStream(this.ccd.data));
				this.read(pooledBinaryReader);
			}
		}
	}

	// Token: 0x06004B02 RID: 19202 RVA: 0x001DA518 File Offset: 0x001D8718
	public bool IsSpawnNeeded(WorldBiomes _worldBiomes, ulong _worldTime)
	{
		BiomeDefinition biome = _worldBiomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return false;
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		if (biomeSpawnEntityGroupList == null)
		{
			return false;
		}
		for (int i = 0; i < biomeSpawnEntityGroupList.list.Count; i++)
		{
			BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.list[i];
			ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
			if (!this.entitesSpawned.TryGetValue(biomeSpawnEntityGroupData.idHash, out countsAndTime))
			{
				return true;
			}
			if (countsAndTime.count < countsAndTime.maxCount || _worldTime > countsAndTime.delayWorldTime)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004B03 RID: 19203 RVA: 0x001DA5A4 File Offset: 0x001D87A4
	public bool CanSpawn(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		return this.entitesSpawned.TryGetValue(_idHash, out countsAndTime) && countsAndTime.count < countsAndTime.maxCount;
	}

	// Token: 0x06004B04 RID: 19204 RVA: 0x001DA5D4 File Offset: 0x001D87D4
	public void SetCounts(int _idHash, int _count, int _maxCount)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime value;
		this.entitesSpawned.TryGetValue(_idHash, out value);
		value.count = _count;
		value.maxCount = _maxCount;
		this.entitesSpawned[_idHash] = value;
	}

	// Token: 0x06004B05 RID: 19205 RVA: 0x001DA610 File Offset: 0x001D8810
	public void IncCount(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime value;
		if (!this.entitesSpawned.TryGetValue(_idHash, out value))
		{
			value.count = 1;
		}
		else
		{
			value.count++;
		}
		this.entitesSpawned[_idHash] = value;
		this.chunk.isModified = true;
	}

	// Token: 0x06004B06 RID: 19206 RVA: 0x001DA65C File Offset: 0x001D885C
	public void DecCount(int _idHash, bool _killed)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		if (this.entitesSpawned.TryGetValue(_idHash, out countsAndTime))
		{
			countsAndTime.count = Utils.FastMax(countsAndTime.count - 1, 0);
			if (_killed)
			{
				countsAndTime.maxCount = Utils.FastMax(0, countsAndTime.maxCount - 1);
			}
			this.entitesSpawned[_idHash] = countsAndTime;
			this.chunk.isModified = true;
		}
	}

	// Token: 0x06004B07 RID: 19207 RVA: 0x001DA6C0 File Offset: 0x001D88C0
	public void DecMaxCount(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		if (this.entitesSpawned.TryGetValue(_idHash, out countsAndTime))
		{
			countsAndTime.maxCount = Utils.FastMax(0, countsAndTime.maxCount - 1);
			this.entitesSpawned[_idHash] = countsAndTime;
			this.chunk.isModified = true;
		}
	}

	// Token: 0x06004B08 RID: 19208 RVA: 0x001DA70C File Offset: 0x001D890C
	public ulong GetDelayWorldTime(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		this.entitesSpawned.TryGetValue(_idHash, out countsAndTime);
		return countsAndTime.delayWorldTime;
	}

	// Token: 0x06004B09 RID: 19209 RVA: 0x001DA730 File Offset: 0x001D8930
	public void ResetRespawn(int _idHash, World _world, int _maxCount)
	{
		BiomeDefinition biome = _world.Biomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return;
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		if (biomeSpawnEntityGroupList == null)
		{
			return;
		}
		BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.Find(_idHash);
		if (biomeSpawnEntityGroupData == null)
		{
			return;
		}
		ChunkAreaBiomeSpawnData.CountsAndTime value;
		this.entitesSpawned.TryGetValue(_idHash, out value);
		value.delayWorldTime = _world.worldTime + (ulong)((float)biomeSpawnEntityGroupData.respawnDelayInWorldTime * _world.RandomRange(0.9f, 1.1f));
		value.maxCount = _maxCount;
		this.entitesSpawned[_idHash] = value;
		this.chunk.isModified = true;
	}

	// Token: 0x06004B0A RID: 19210 RVA: 0x001DA7CC File Offset: 0x001D89CC
	public bool DelayAllEnemySpawningUntil(ulong _worldTime, WorldBiomes _worldBiomes)
	{
		bool result = false;
		BiomeDefinition biome = _worldBiomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return false;
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		if (biomeSpawnEntityGroupList == null)
		{
			return false;
		}
		Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime> dictionary = new Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime>();
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair in this.entitesSpawned)
		{
			BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.Find(keyValuePair.Key);
			if (biomeSpawnEntityGroupData != null && EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData.entityGroupName))
			{
				ChunkAreaBiomeSpawnData.CountsAndTime value = keyValuePair.Value;
				bool flag = false;
				if (value.delayWorldTime < _worldTime)
				{
					value.delayWorldTime = _worldTime;
					flag = true;
				}
				if (value.maxCount > 0)
				{
					value.maxCount = 0;
					flag = true;
				}
				if (flag)
				{
					dictionary[keyValuePair.Key] = value;
					result = true;
				}
			}
		}
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair2 in dictionary)
		{
			this.entitesSpawned[keyValuePair2.Key] = keyValuePair2.Value;
		}
		for (int i = 0; i < biomeSpawnEntityGroupList.list.Count; i++)
		{
			BiomeSpawnEntityGroupData biomeSpawnEntityGroupData2 = biomeSpawnEntityGroupList.list[i];
			if (EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData2.entityGroupName) && !this.entitesSpawned.ContainsKey(biomeSpawnEntityGroupData2.idHash))
			{
				this.entitesSpawned[biomeSpawnEntityGroupData2.idHash] = new ChunkAreaBiomeSpawnData.CountsAndTime(0, 0, _worldTime);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06004B0B RID: 19211 RVA: 0x001DA970 File Offset: 0x001D8B70
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(BinaryReader _br)
	{
		int num = (int)_br.ReadByte();
		this.entitesSpawned.Clear();
		int num2 = (int)_br.ReadByte();
		for (int i = 0; i < num2; i++)
		{
			if (num <= 1)
			{
				_br.ReadString();
				_br.ReadUInt16();
				_br.ReadUInt64();
			}
			else
			{
				int key = _br.ReadInt32();
				int num3 = (int)_br.ReadUInt16();
				ChunkAreaBiomeSpawnData.CountsAndTime value;
				value.count = (num3 & 255);
				value.maxCount = num3 >> 8;
				value.delayWorldTime = _br.ReadUInt64();
				this.entitesSpawned[key] = value;
			}
		}
	}

	// Token: 0x06004B0C RID: 19212 RVA: 0x001DAA04 File Offset: 0x001D8C04
	public void BeforeWrite()
	{
		using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
		{
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.ccd.data = pooledExpandableMemoryStream.ToArray();
		}
	}

	// Token: 0x06004B0D RID: 19213 RVA: 0x001DAA7C File Offset: 0x001D8C7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(BinaryWriter _bw)
	{
		_bw.Write(2);
		int num = 0;
		int num2 = Utils.FastMin(this.entitesSpawned.Count, 255);
		_bw.Write((byte)num2);
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair in this.entitesSpawned)
		{
			_bw.Write(keyValuePair.Key);
			_bw.Write((ushort)(keyValuePair.Value.maxCount << 8 | keyValuePair.Value.count));
			_bw.Write(keyValuePair.Value.delayWorldTime);
			if (++num >= num2)
			{
				break;
			}
		}
	}

	// Token: 0x06004B0E RID: 19214 RVA: 0x001DAB3C File Offset: 0x001D8D3C
	public override string ToString()
	{
		World world = GameManager.Instance.World;
		ulong worldTime = world.worldTime;
		BiomeDefinition biome = world.Biomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return "biome? " + this.biomeId.ToString();
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair in this.entitesSpawned)
		{
			string text = "?";
			if (biomeSpawnEntityGroupList != null)
			{
				BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.Find(keyValuePair.Key);
				if (biomeSpawnEntityGroupData != null)
				{
					text = biomeSpawnEntityGroupData.entityGroupName + " " + biomeSpawnEntityGroupData.daytime.ToString();
				}
			}
			ulong num = keyValuePair.Value.delayWorldTime - worldTime;
			if (num < 0UL)
			{
				num = 0UL;
			}
			stringBuilder.Append(string.Format("{0} #{1}/{2} {3}, ", new object[]
			{
				text,
				keyValuePair.Value.count,
				keyValuePair.Value.maxCount,
				GameUtils.WorldTimeDeltaToString(num)
			}));
		}
		return string.Format("biomeId {0}, XZ {1} {2}: {3}", new object[]
		{
			this.biomeId,
			this.area.x.ToCultureInvariantString("0"),
			this.area.y.ToCultureInvariantString("0"),
			stringBuilder.ToString()
		});
	}

	// Token: 0x0400398C RID: 14732
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCurrentSaveVersion = 2;

	// Token: 0x0400398D RID: 14733
	public byte biomeId;

	// Token: 0x0400398E RID: 14734
	public Rect area;

	// Token: 0x0400398F RID: 14735
	public Chunk chunk;

	// Token: 0x04003990 RID: 14736
	public bool checkedPOITags;

	// Token: 0x04003991 RID: 14737
	public FastTags<TagGroup.Poi> poiTags;

	// Token: 0x04003992 RID: 14738
	public int groupsEnabledFlags;

	// Token: 0x04003993 RID: 14739
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCustomData ccd;

	// Token: 0x04003994 RID: 14740
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime> entitesSpawned = new Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime>();

	// Token: 0x020009A0 RID: 2464
	[PublicizedFrom(EAccessModifier.Private)]
	public struct CountsAndTime
	{
		// Token: 0x06004B0F RID: 19215 RVA: 0x001DACE4 File Offset: 0x001D8EE4
		public CountsAndTime(int _count, int _maxCount, ulong _delayWorldTime)
		{
			this.count = _count;
			this.maxCount = _maxCount;
			this.delayWorldTime = _delayWorldTime;
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x001DACFB File Offset: 0x001D8EFB
		public override string ToString()
		{
			return string.Format("cnt {0}, maxCnt {1}, wtime {2}", this.count, this.maxCount, this.delayWorldTime);
		}

		// Token: 0x04003995 RID: 14741
		public int count;

		// Token: 0x04003996 RID: 14742
		public int maxCount;

		// Token: 0x04003997 RID: 14743
		public ulong delayWorldTime;
	}
}
