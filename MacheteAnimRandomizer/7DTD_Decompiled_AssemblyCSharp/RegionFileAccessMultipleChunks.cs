using System;
using System.Collections.Generic;
using System.IO;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000A1A RID: 2586
public abstract class RegionFileAccessMultipleChunks : RegionFileAccessAbstract
{
	// Token: 0x06004F40 RID: 20288 RVA: 0x001F5ACE File Offset: 0x001F3CCE
	public RegionFileAccessMultipleChunks()
	{
		this.writeStream = new ChunkMemoryStreamWriter();
		this.readStream = new ChunkMemoryStreamReader();
	}

	// Token: 0x06004F41 RID: 20289 RVA: 0x001F5B04 File Offset: 0x001F3D04
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void ReadDirectory(string _dir, Action<long, string, uint> _chunkAndTimeStampHandler, int chunksPerRegionPerDimension)
	{
		if (_dir != null)
		{
			if (!SdDirectory.Exists(_dir))
			{
				SdDirectory.CreateDirectory(_dir);
				return;
			}
			foreach (SdFileInfo sdFileInfo in new SdDirectoryInfo(_dir).GetFiles())
			{
				string[] array = sdFileInfo.Name.Split('.', StringSplitOptions.None);
				int num;
				int num2;
				if (array.Length != 4)
				{
					if (!sdFileInfo.Name.EqualsCaseInsensitive("PendingResets.7pr"))
					{
						Debug.LogError("Invalid region file name: " + sdFileInfo.FullName);
					}
				}
				else if (!int.TryParse(array[1], out num) || !int.TryParse(array[2], out num2))
				{
					Debug.LogError("Failed to parse region coordinates from region file name: " + sdFileInfo.FullName);
				}
				else
				{
					string text = array[3];
					RegionFile rfc = this.GetRFC(num, num2, _dir, text);
					for (int j = num * chunksPerRegionPerDimension; j < num * chunksPerRegionPerDimension + chunksPerRegionPerDimension; j++)
					{
						for (int k = num2 * chunksPerRegionPerDimension; k < num2 * chunksPerRegionPerDimension + chunksPerRegionPerDimension; k++)
						{
							if (rfc.HasChunk(j, k))
							{
								uint arg;
								rfc.GetTimestampInfo(j, k, out arg);
								_chunkAndTimeStampHandler(WorldChunkCache.MakeChunkKey(j, k), text, arg);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06004F42 RID: 20290 RVA: 0x001F5C30 File Offset: 0x001F3E30
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFile GetRFC(int _regionX, int _regionZ, string _dir, string _ext)
	{
		Dictionary<string, RegionFileAccessMultipleChunks.Region> obj = this.regionTable;
		RegionFile result;
		lock (obj)
		{
			RegionFileAccessMultipleChunks.Region region;
			if (!this.regionTable.TryGetValue(_dir, out region))
			{
				region = new RegionFileAccessMultipleChunks.Region();
				this.regionTable.Add(_dir, region);
			}
			Vector2 key = new Vector2((float)_regionX, (float)_regionZ);
			RegionFileAccessMultipleChunks.RegionExtensions regionExtensions;
			if (!region.TryGetValue(key, out regionExtensions))
			{
				regionExtensions = new RegionFileAccessMultipleChunks.RegionExtensions();
				region.Add(key, regionExtensions);
			}
			RegionFile regionFile;
			if (!regionExtensions.TryGetValue(_ext, out regionFile))
			{
				regionFile = this.OpenRegionFile(_dir, _regionX, _regionZ, _ext);
				regionExtensions.Add(_ext, regionFile);
			}
			result = regionFile;
		}
		return result;
	}

	// Token: 0x06004F43 RID: 20291
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract RegionFile OpenRegionFile(string _dir, int _regionX, int _regionZ, string _ext);

	// Token: 0x06004F44 RID: 20292
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void GetRegionCoords(int _chunkX, int _chunkZ, out int _regionX, out int _regionZ);

	// Token: 0x06004F45 RID: 20293 RVA: 0x001F5CE0 File Offset: 0x001F3EE0
	public override Stream GetOutputStream(string _dir, int _chunkX, int _chunkZ, string _ext)
	{
		this.writeStream.Init(this, _dir, _chunkX, _chunkZ, _ext);
		return this.writeStream;
	}

	// Token: 0x06004F46 RID: 20294 RVA: 0x001F5CFC File Offset: 0x001F3EFC
	public override Stream GetInputStream(string _dir, int _chunkX, int _chunkZ, string _ext)
	{
		int regionX;
		int regionZ;
		this.GetRegionCoords(_chunkX, _chunkZ, out regionX, out regionZ);
		RegionFile rfc = this.GetRFC(regionX, regionZ, _dir, _ext);
		if (!rfc.HasChunk(_chunkX, _chunkZ))
		{
			return null;
		}
		rfc.ReadData(_chunkX, _chunkZ, this.readStream);
		this.readStream.Position = 0L;
		return this.readStream;
	}

	// Token: 0x06004F47 RID: 20295 RVA: 0x001F5D50 File Offset: 0x001F3F50
	public void Write(string _dir, int _chunkX, int _chunkZ, string _ext, byte[] _buf, int _bufLength)
	{
		int regionX;
		int regionZ;
		this.GetRegionCoords(_chunkX, _chunkZ, out regionX, out regionZ);
		this.GetRFC(regionX, regionZ, _dir, _ext).WriteData(_chunkX, _chunkZ, _bufLength, 0, _buf, true);
	}

	// Token: 0x06004F48 RID: 20296 RVA: 0x001F5D81 File Offset: 0x001F3F81
	[PublicizedFrom(EAccessModifier.Private)]
	public int MediumIntByteArrayToInt(byte[] bytes)
	{
		return (int)bytes[0] | (int)bytes[1] << 8 | (int)bytes[2] << 16;
	}

	// Token: 0x06004F49 RID: 20297 RVA: 0x001F5D94 File Offset: 0x001F3F94
	public override void Remove(string _dir, int _chunkX, int _chunkZ)
	{
		int num;
		int num2;
		this.GetRegionCoords(_chunkX, _chunkZ, out num, out num2);
		Dictionary<string, RegionFileAccessMultipleChunks.Region> obj = this.regionTable;
		lock (obj)
		{
			RegionFileAccessMultipleChunks.Region region;
			if (this.regionTable.TryGetValue(_dir, out region))
			{
				Vector2 key = new Vector2((float)num, (float)num2);
				RegionFileAccessMultipleChunks.RegionExtensions regionExtensions;
				if (region.TryGetValue(key, out regionExtensions))
				{
					foreach (RegionFile regionFile in regionExtensions.Values)
					{
						if (regionFile.HasChunk(_chunkX, _chunkZ))
						{
							regionFile.RemoveChunk(_chunkX, _chunkZ);
							this.regionsWithRemovedChunks.Add(regionFile);
						}
					}
				}
			}
		}
	}

	// Token: 0x06004F4A RID: 20298 RVA: 0x001F5E64 File Offset: 0x001F4064
	public override void OptimizeLayouts()
	{
		foreach (RegionFile regionFile in this.regionsWithRemovedChunks)
		{
			using (RegionFileAccessMultipleChunks.s_OptimizeLayoutsMarker.Auto())
			{
				if (regionFile.ChunkCount() > 0)
				{
					regionFile.OptimizeLayout();
				}
				else
				{
					int regionX;
					int regionZ;
					string path;
					regionFile.GetPositionAndPath(out regionX, out regionZ, out path);
					this.RemoveRegionFromCache(regionX, regionZ, Path.GetDirectoryName(path));
					SdFile.Delete(path);
				}
			}
		}
		this.regionsWithRemovedChunks.Clear();
	}

	// Token: 0x06004F4B RID: 20299 RVA: 0x001F5F1C File Offset: 0x001F411C
	public override void Close()
	{
		this.OptimizeLayouts();
		this.ClearCache();
	}

	// Token: 0x06004F4C RID: 20300 RVA: 0x001F5F2C File Offset: 0x001F412C
	public override void ClearCache()
	{
		Dictionary<string, RegionFileAccessMultipleChunks.Region> obj = this.regionTable;
		lock (obj)
		{
			foreach (RegionFileAccessMultipleChunks.Region region in this.regionTable.Values)
			{
				foreach (RegionFileAccessMultipleChunks.RegionExtensions regionExtensions in region.Values)
				{
					foreach (RegionFile regionFile in regionExtensions.Values)
					{
						regionFile.SaveHeaderData();
						regionFile.Close();
					}
					regionExtensions.Clear();
				}
				region.Clear();
			}
			this.regionTable.Clear();
		}
	}

	// Token: 0x06004F4D RID: 20301 RVA: 0x001F6048 File Offset: 0x001F4248
	public override void RemoveRegionFromCache(int _regionX, int _regionZ, string _dir)
	{
		Dictionary<string, RegionFileAccessMultipleChunks.Region> obj = this.regionTable;
		lock (obj)
		{
			RegionFileAccessMultipleChunks.Region region;
			if (this.regionTable.TryGetValue(_dir, out region))
			{
				Vector2 key = new Vector2((float)_regionX, (float)_regionZ);
				RegionFileAccessMultipleChunks.RegionExtensions regionExtensions;
				if (region.TryGetValue(key, out regionExtensions))
				{
					foreach (RegionFile regionFile in regionExtensions.Values)
					{
						regionFile.SaveHeaderData();
						regionFile.Close();
					}
					regionExtensions.Clear();
					region.Remove(key);
				}
			}
		}
	}

	// Token: 0x06004F4E RID: 20302 RVA: 0x001F6104 File Offset: 0x001F4304
	public override int GetChunkByteCount(string _dir, int _chunkX, int _chunkZ)
	{
		int num;
		int num2;
		this.GetRegionCoords(_chunkX, _chunkZ, out num, out num2);
		int num3 = 0;
		Dictionary<string, RegionFileAccessMultipleChunks.Region> obj = this.regionTable;
		lock (obj)
		{
			RegionFileAccessMultipleChunks.Region region;
			if (!this.regionTable.TryGetValue(_dir, out region))
			{
				return 0;
			}
			RegionFileAccessMultipleChunks.RegionExtensions regionExtensions;
			if (!region.TryGetValue(new Vector2((float)num, (float)num2), out regionExtensions))
			{
				return 0;
			}
			foreach (RegionFile regionFile in regionExtensions.Values)
			{
				num3 += regionFile.GetChunkByteCount(_chunkX, _chunkZ);
			}
		}
		return num3;
	}

	// Token: 0x06004F4F RID: 20303 RVA: 0x001F61C8 File Offset: 0x001F43C8
	public override long GetTotalByteCount(string _dir)
	{
		long num = 0L;
		Dictionary<string, RegionFileAccessMultipleChunks.Region> obj = this.regionTable;
		lock (obj)
		{
			foreach (RegionFileAccessMultipleChunks.Region region in this.regionTable.Values)
			{
				foreach (RegionFileAccessMultipleChunks.RegionExtensions regionExtensions in region.Values)
				{
					foreach (RegionFile regionFile in regionExtensions.Values)
					{
						num += regionFile.Length;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x04003CA8 RID: 15528
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, RegionFileAccessMultipleChunks.Region> regionTable = new Dictionary<string, RegionFileAccessMultipleChunks.Region>();

	// Token: 0x04003CA9 RID: 15529
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<RegionFile> regionsWithRemovedChunks = new HashSet<RegionFile>();

	// Token: 0x04003CAA RID: 15530
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkMemoryStreamWriter writeStream;

	// Token: 0x04003CAB RID: 15531
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkMemoryStreamReader readStream;

	// Token: 0x04003CAC RID: 15532
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_OptimizeLayoutsMarker = new ProfilerMarker("RegionFileAccess.OptimizeLayout");

	// Token: 0x02000A1B RID: 2587
	[PublicizedFrom(EAccessModifier.Private)]
	public class Region : Dictionary<Vector2, RegionFileAccessMultipleChunks.RegionExtensions>
	{
		// Token: 0x06004F51 RID: 20305 RVA: 0x001F62D5 File Offset: 0x001F44D5
		public Region() : base(Vector2EqualityComparer.Instance)
		{
		}
	}

	// Token: 0x02000A1C RID: 2588
	[PublicizedFrom(EAccessModifier.Private)]
	public class RegionExtensions : Dictionary<string, RegionFile>
	{
	}
}
