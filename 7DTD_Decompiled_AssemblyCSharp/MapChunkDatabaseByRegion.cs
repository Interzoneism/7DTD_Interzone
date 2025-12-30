using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

// Token: 0x020009EB RID: 2539
public class MapChunkDatabaseByRegion : IMapChunkDatabase
{
	// Token: 0x06004DCD RID: 19917 RVA: 0x001EB330 File Offset: 0x001E9530
	public MapChunkDatabaseByRegion(int playerId)
	{
		this.m_regions = new Dictionary<Vector2i, MapChunkDatabaseByRegion.RegionData>();
		this.m_chunksSent = new HashSet<int>();
		this.m_chunkIdsToSend = new List<int>();
		this.m_chunkDataToSend = new List<ushort[]>();
		this.m_playerId = playerId;
	}

	// Token: 0x06004DCE RID: 19918 RVA: 0x001EB384 File Offset: 0x001E9584
	public void Clear()
	{
		object regionsLock = this.m_regionsLock;
		lock (regionsLock)
		{
			this.m_regions.Clear();
		}
	}

	// Token: 0x06004DCF RID: 19919 RVA: 0x001EB3CC File Offset: 0x001E95CC
	public ushort[] GetMapColors(long _chunkKey)
	{
		Vector2i key;
		Vector2i offset;
		this.ToRegionAndOffset(_chunkKey, out key, out offset);
		object regionsLock = this.m_regionsLock;
		MapChunkDatabaseByRegion.RegionData regionData;
		lock (regionsLock)
		{
			if (!this.m_regions.TryGetValue(key, out regionData))
			{
				return null;
			}
		}
		return regionData.GetChunkData(offset);
	}

	// Token: 0x06004DD0 RID: 19920 RVA: 0x001EB434 File Offset: 0x001E9634
	public void Add(int _chunkX, int _chunkZ, ushort[] _mapColors)
	{
		this.Add(_chunkX, _chunkZ, _mapColors, false);
	}

	// Token: 0x06004DD1 RID: 19921 RVA: 0x001EB440 File Offset: 0x001E9640
	public void Add(List<int> _chunks, List<ushort[]> _mapPieces)
	{
		for (int i = 0; i < _chunks.Count; i++)
		{
			int chunkX;
			int chunkZ;
			IMapChunkDatabase.FromChunkDBKey(_chunks[i], out chunkX, out chunkZ);
			this.Add(chunkX, chunkZ, _mapPieces[i], true);
		}
		this.m_networkDataAvailable = true;
	}

	// Token: 0x06004DD2 RID: 19922 RVA: 0x001EB488 File Offset: 0x001E9688
	public bool Contains(long _chunkKey)
	{
		Vector2i key;
		Vector2i offset;
		this.ToRegionAndOffset(_chunkKey, out key, out offset);
		object regionsLock = this.m_regionsLock;
		MapChunkDatabaseByRegion.RegionData regionData;
		lock (regionsLock)
		{
			if (!this.m_regions.TryGetValue(key, out regionData))
			{
				return false;
			}
		}
		return regionData.GetChunkData(offset) != null;
	}

	// Token: 0x06004DD3 RID: 19923 RVA: 0x001EB4F4 File Offset: 0x001E96F4
	public bool IsNetworkDataAvail()
	{
		return this.m_networkDataAvailable;
	}

	// Token: 0x06004DD4 RID: 19924 RVA: 0x001EB4FC File Offset: 0x001E96FC
	public void ResetNetworkDataAvail()
	{
		this.m_networkDataAvailable = false;
	}

	// Token: 0x06004DD5 RID: 19925 RVA: 0x001EB508 File Offset: 0x001E9708
	public NetPackage GetMapChunkPackagesToSend()
	{
		if (!this.m_clientMapMiddlePositionUpdated)
		{
			return null;
		}
		this.m_clientMapMiddlePositionUpdated = false;
		this.m_chunkIdsToSend.Clear();
		this.m_chunkDataToSend.Clear();
		int num = World.toChunkXZ(this.m_clientMapMiddlePosition.x);
		int num2 = World.toChunkXZ(this.m_clientMapMiddlePosition.y);
		int num3 = 8;
		object regionsLock = this.m_regionsLock;
		lock (regionsLock)
		{
			for (int i = -num3; i <= num3; i++)
			{
				for (int j = -num3; j <= num3; j++)
				{
					long worldChunkKey = WorldChunkCache.MakeChunkKey(num + i, num2 + j);
					int item = IMapChunkDatabase.ToChunkDBKey(worldChunkKey);
					if (!this.m_chunksSent.Contains(item))
					{
						Vector2i key;
						Vector2i offset;
						this.ToRegionAndOffset(worldChunkKey, out key, out offset);
						MapChunkDatabaseByRegion.RegionData regionData;
						if (this.m_regions.TryGetValue(key, out regionData))
						{
							ushort[] chunkData = regionData.GetChunkData(offset);
							if (chunkData != null)
							{
								this.m_chunksSent.Add(item);
								this.m_chunkIdsToSend.Add(item);
								this.m_chunkDataToSend.Add(chunkData);
							}
						}
					}
				}
			}
		}
		if (this.m_chunkIdsToSend.Count == 0)
		{
			return null;
		}
		return NetPackageManager.GetPackage<NetPackageMapChunks>().Setup(this.m_playerId, this.m_chunkIdsToSend, this.m_chunkDataToSend);
	}

	// Token: 0x06004DD6 RID: 19926 RVA: 0x001EB664 File Offset: 0x001E9864
	public void LoadAsync(ThreadManager.TaskInfo _taskInfo)
	{
		IMapChunkDatabase.DirectoryPlayerId directoryPlayerId = (IMapChunkDatabase.DirectoryPlayerId)_taskInfo.parameter;
		this.Load(Path.Join(directoryPlayerId.dir, directoryPlayerId.file));
	}

	// Token: 0x06004DD7 RID: 19927 RVA: 0x001EB6A0 File Offset: 0x001E98A0
	public void SaveAsync(ThreadManager.TaskInfo _taskInfo)
	{
		IMapChunkDatabase.DirectoryPlayerId directoryPlayerId = (IMapChunkDatabase.DirectoryPlayerId)_taskInfo.parameter;
		this.Save(Path.Join(directoryPlayerId.dir, directoryPlayerId.file));
	}

	// Token: 0x06004DD8 RID: 19928 RVA: 0x001EB6DA File Offset: 0x001E98DA
	public void SetClientMapMiddlePosition(Vector2i _pos)
	{
		this.m_clientMapMiddlePosition = _pos;
		this.m_clientMapMiddlePositionUpdated = true;
	}

	// Token: 0x06004DD9 RID: 19929 RVA: 0x001EB6EC File Offset: 0x001E98EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Add(int chunkX, int chunkZ, ushort[] mapColors, bool skipCopy)
	{
		Vector2i key;
		Vector2i offset;
		this.ToRegionAndOffset(chunkX, chunkZ, out key, out offset);
		object regionsLock = this.m_regionsLock;
		MapChunkDatabaseByRegion.RegionData regionData;
		lock (regionsLock)
		{
			if (!this.m_regions.TryGetValue(key, out regionData))
			{
				regionData = new MapChunkDatabaseByRegion.RegionData();
				this.m_regions.Add(key, regionData);
			}
		}
		regionData.SetChunkData(offset, mapColors, skipCopy);
	}

	// Token: 0x06004DDA RID: 19930 RVA: 0x001EB764 File Offset: 0x001E9964
	[PublicizedFrom(EAccessModifier.Private)]
	public void Load(string rootDirectory)
	{
		if (!SdDirectory.Exists(rootDirectory))
		{
			return;
		}
		object regionsLock = this.m_regionsLock;
		lock (regionsLock)
		{
			HashSet<Vector2i> hashSet = new HashSet<Vector2i>(this.m_regions.Keys);
			foreach (SdFileInfo sdFileInfo in new SdDirectoryInfo(rootDirectory).EnumerateFiles())
			{
				Vector2i vector2i;
				if (this.TryGetRegionPosFromFileName(sdFileInfo.Name, out vector2i))
				{
					try
					{
						hashSet.Add(vector2i);
						MapChunkDatabaseByRegion.RegionData regionData;
						if (!this.m_regions.TryGetValue(vector2i, out regionData))
						{
							regionData = new MapChunkDatabaseByRegion.RegionData();
							this.m_regions.Add(vector2i, regionData);
						}
						regionData.Load(sdFileInfo.FullName);
						hashSet.Remove(vector2i);
					}
					catch (Exception ex)
					{
						Log.Warning(string.Format("[{0}] Failed to load region ({1}, {2}) from '{3}': {4}", new object[]
						{
							"MapChunkDatabaseByRegion",
							vector2i.x,
							vector2i.y,
							sdFileInfo.FullName,
							ex
						}));
					}
				}
			}
			foreach (Vector2i key in hashSet)
			{
				this.m_regions.Remove(key);
			}
		}
	}

	// Token: 0x06004DDB RID: 19931 RVA: 0x001EB92C File Offset: 0x001E9B2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Save(string rootDirectory)
	{
		SdDirectory.CreateDirectory(rootDirectory);
		Dictionary<Vector2i, string> dictionary = new Dictionary<Vector2i, string>();
		foreach (SdFileInfo sdFileInfo in new SdDirectoryInfo(rootDirectory).EnumerateFiles())
		{
			Vector2i key;
			if (this.TryGetRegionPosFromFileName(sdFileInfo.Name, out key))
			{
				dictionary[key] = sdFileInfo.FullName;
			}
		}
		object regionsLock = this.m_regionsLock;
		lock (regionsLock)
		{
			foreach (KeyValuePair<Vector2i, MapChunkDatabaseByRegion.RegionData> keyValuePair in this.m_regions)
			{
				Vector2i vector2i;
				MapChunkDatabaseByRegion.RegionData regionData;
				keyValuePair.Deconstruct(out vector2i, out regionData);
				Vector2i vector2i2 = vector2i;
				MapChunkDatabaseByRegion.RegionData regionData2 = regionData;
				string regionDataPath = MapChunkDatabaseByRegion.GetRegionDataPath(rootDirectory, vector2i2);
				try
				{
					dictionary.Remove(vector2i2);
					regionData2.Save(regionDataPath);
				}
				catch (Exception ex)
				{
					Log.Warning(string.Format("[{0}] Failed to save region ({1}, {2}) to '{3}': {4}", new object[]
					{
						"MapChunkDatabaseByRegion",
						vector2i2.x,
						vector2i2.y,
						regionDataPath,
						ex
					}));
				}
			}
		}
		foreach (KeyValuePair<Vector2i, string> keyValuePair2 in dictionary)
		{
			Vector2i vector2i;
			string text;
			keyValuePair2.Deconstruct(out vector2i, out text);
			Vector2i vector2i3 = vector2i;
			string text2 = text;
			try
			{
				SdFile.Delete(text2);
			}
			catch (Exception ex2)
			{
				Log.Warning(string.Format("[{0}] Failed to delete region ({1}, {2}) to '{3}': {4}", new object[]
				{
					"MapChunkDatabaseByRegion",
					vector2i3.x,
					vector2i3.y,
					text2,
					ex2
				}));
			}
		}
	}

	// Token: 0x06004DDC RID: 19932 RVA: 0x001EBB44 File Offset: 0x001E9D44
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToRegionAndOffset(long worldChunkKey, out Vector2i regionPos, out Vector2i chunkOffset)
	{
		int chunkX = WorldChunkCache.extractX(worldChunkKey);
		int chunkZ = WorldChunkCache.extractZ(worldChunkKey);
		this.ToRegionAndOffset(chunkX, chunkZ, out regionPos, out chunkOffset);
	}

	// Token: 0x06004DDD RID: 19933 RVA: 0x001EBB69 File Offset: 0x001E9D69
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToRegionAndOffset(Vector3i chunkPos, out Vector2i regionPos, out Vector2i chunkOffset)
	{
		this.ToRegionAndOffset(chunkPos.x, chunkPos.y, out regionPos, out chunkOffset);
	}

	// Token: 0x06004DDE RID: 19934 RVA: 0x001EBB80 File Offset: 0x001E9D80
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToRegionAndOffset(int chunkX, int chunkZ, out Vector2i regionPos, out Vector2i chunkOffset)
	{
		int num = chunkX >> 5;
		int num2 = chunkZ >> 5;
		regionPos = new Vector2i(num, num2);
		int x = chunkX - num * 32;
		int y = chunkZ - num2 * 32;
		chunkOffset = new Vector2i(x, y);
	}

	// Token: 0x06004DDF RID: 19935 RVA: 0x001EBBC0 File Offset: 0x001E9DC0
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetRegionDataPath(string rootDirectory, Vector2i regionPos)
	{
		return Path.Join(rootDirectory, string.Format("{0}{1}{2}{3}{4}", new object[]
		{
			"r.",
			regionPos.x,
			".",
			regionPos.y,
			".7rm"
		}));
	}

	// Token: 0x06004DE0 RID: 19936 RVA: 0x001EBC24 File Offset: 0x001E9E24
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryGetRegionPosFromFileName(StringSpan name, out Vector2i regionPos)
	{
		if (name.IndexOf("r.") != 0)
		{
			regionPos = default(Vector2i);
			return false;
		}
		name = name.Slice("r.".Length);
		int num = name.Length - ".7rm".Length;
		if (num < 0 || name.LastIndexOf(".7rm") != num)
		{
			regionPos = default(Vector2i);
			return false;
		}
		name = name.Slice(0, num);
		int x = 0;
		bool flag = false;
		int y = 0;
		bool flag2 = false;
		foreach (StringSpan stringSpan in name.GetSplitEnumerator(".", StringSplitOptions.None))
		{
			if (flag2)
			{
				regionPos = default(Vector2i);
				return false;
			}
			if (flag)
			{
				if (!int.TryParse(stringSpan.AsSpan(), out y))
				{
					regionPos = default(Vector2i);
					return false;
				}
				flag2 = true;
			}
			else
			{
				if (!int.TryParse(stringSpan.AsSpan(), out x))
				{
					regionPos = default(Vector2i);
					return false;
				}
				flag = true;
			}
		}
		if (!flag || !flag2)
		{
			regionPos = default(Vector2i);
			return false;
		}
		regionPos = new Vector2i(x, y);
		return true;
	}

	// Token: 0x04003B53 RID: 15187
	[PublicizedFrom(EAccessModifier.Private)]
	public const string FilePrefix = "r.";

	// Token: 0x04003B54 RID: 15188
	[PublicizedFrom(EAccessModifier.Private)]
	public const string FileElementSeparator = ".";

	// Token: 0x04003B55 RID: 15189
	[PublicizedFrom(EAccessModifier.Private)]
	public const string FilePostfix = ".7rm";

	// Token: 0x04003B56 RID: 15190
	[PublicizedFrom(EAccessModifier.Private)]
	public const int REGION_CHUNK_WIDTH = 32;

	// Token: 0x04003B57 RID: 15191
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CHUNK_TO_REGION_SHIFT = 5;

	// Token: 0x04003B58 RID: 15192
	[PublicizedFrom(EAccessModifier.Private)]
	public const int REGION_CHUNK_AREA = 1024;

	// Token: 0x04003B59 RID: 15193
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CHUNK_DATA_LENGTH = 256;

	// Token: 0x04003B5A RID: 15194
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ushort[] EMPTY_CHUNK_DATA = new ushort[256];

	// Token: 0x04003B5B RID: 15195
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<Vector2i, MapChunkDatabaseByRegion.RegionData> m_regions;

	// Token: 0x04003B5C RID: 15196
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object m_regionsLock = new object();

	// Token: 0x04003B5D RID: 15197
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<int> m_chunksSent;

	// Token: 0x04003B5E RID: 15198
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<int> m_chunkIdsToSend;

	// Token: 0x04003B5F RID: 15199
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ushort[]> m_chunkDataToSend;

	// Token: 0x04003B60 RID: 15200
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_playerId;

	// Token: 0x04003B61 RID: 15201
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_networkDataAvailable;

	// Token: 0x04003B62 RID: 15202
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i m_clientMapMiddlePosition;

	// Token: 0x04003B63 RID: 15203
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_clientMapMiddlePositionUpdated;

	// Token: 0x020009EC RID: 2540
	[PublicizedFrom(EAccessModifier.Private)]
	public class RegionData
	{
		// Token: 0x06004DE2 RID: 19938 RVA: 0x001EBD54 File Offset: 0x001E9F54
		public RegionData()
		{
			this.m_regionData = new ushort[32][][];
			for (int i = 0; i < 32; i++)
			{
				this.m_regionData[i] = new ushort[32][];
			}
			this.m_dirty = true;
		}

		// Token: 0x06004DE3 RID: 19939 RVA: 0x001EBD97 File Offset: 0x001E9F97
		public ushort[] GetChunkData(Vector2i offset)
		{
			return this.m_regionData[offset.y][offset.x];
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x001EBDB0 File Offset: 0x001E9FB0
		public void SetChunkData(Vector2i offset, ushort[] mapColors, bool skipCopy)
		{
			if (skipCopy)
			{
				this.m_regionData[offset.y][offset.x] = mapColors;
				this.m_dirty = true;
				return;
			}
			ushort[] array = this.m_regionData[offset.y][offset.x];
			if (array == null)
			{
				array = new ushort[256];
				this.m_regionData[offset.y][offset.x] = array;
			}
			Array.Copy(mapColors, array, 256);
			this.m_dirty = true;
		}

		// Token: 0x06004DE5 RID: 19941 RVA: 0x001EBE28 File Offset: 0x001EA028
		public void Load(string regionFilePath)
		{
			using (Stream stream = SdFile.Open(regionFilePath, FileMode.Open, FileAccess.Read))
			{
				using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
					{
						pooledBinaryReader.SetBaseStream(stream);
						pooledBinaryReader.ReadInt32();
						pooledBinaryReader.SetBaseStream(gzipStream);
						ushort[] array = null;
						for (int i = 0; i < 32; i++)
						{
							for (int j = 0; j < 32; j++)
							{
								if (array == null)
								{
									array = new ushort[256];
								}
								for (int k = 0; k < 256; k++)
								{
									array[k] = pooledBinaryReader.ReadUInt16();
								}
								if (MapChunkDatabaseByRegion.EMPTY_CHUNK_DATA.SequenceEqual(array))
								{
									this.m_regionData[i][j] = null;
								}
								else
								{
									this.m_regionData[i][j] = array;
									array = null;
								}
							}
						}
						this.m_lastRegionPath = regionFilePath;
						this.m_dirty = false;
					}
				}
			}
		}

		// Token: 0x06004DE6 RID: 19942 RVA: 0x001EBF38 File Offset: 0x001EA138
		public void Save(string regionFilePath)
		{
			if (!this.m_dirty && regionFilePath == this.m_lastRegionPath)
			{
				return;
			}
			using (Stream stream = SdFile.Open(regionFilePath, FileMode.Create, FileAccess.Write))
			{
				using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Compress))
				{
					using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(true))
					{
						pooledBinaryWriter.SetBaseStream(stream);
						pooledBinaryWriter.Write(1);
						pooledBinaryWriter.SetBaseStream(gzipStream);
						for (int i = 0; i < 32; i++)
						{
							for (int j = 0; j < 32; j++)
							{
								ushort[] array = this.m_regionData[i][j] ?? MapChunkDatabaseByRegion.EMPTY_CHUNK_DATA;
								for (int k = 0; k < 256; k++)
								{
									pooledBinaryWriter.Write(array[k]);
								}
							}
						}
						this.m_lastRegionPath = regionFilePath;
						this.m_dirty = false;
					}
				}
			}
		}

		// Token: 0x04003B64 RID: 15204
		[PublicizedFrom(EAccessModifier.Private)]
		public const int VERSION = 1;

		// Token: 0x04003B65 RID: 15205
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ushort[][][] m_regionData;

		// Token: 0x04003B66 RID: 15206
		[PublicizedFrom(EAccessModifier.Private)]
		public string m_lastRegionPath;

		// Token: 0x04003B67 RID: 15207
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_dirty;
	}
}
