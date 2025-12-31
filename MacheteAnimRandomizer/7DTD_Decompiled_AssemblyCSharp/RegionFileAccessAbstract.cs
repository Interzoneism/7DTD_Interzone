using System;
using System.IO;

// Token: 0x02000A17 RID: 2583
public abstract class RegionFileAccessAbstract
{
	// Token: 0x06004F2B RID: 20267 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public RegionFileAccessAbstract()
	{
	}

	// Token: 0x06004F2C RID: 20268 RVA: 0x001F5964 File Offset: 0x001F3B64
	public static bool ExtractKey(string _filename, out long _key)
	{
		_key = 0L;
		if (_filename.Length > 0 && _filename.StartsWith("r.") && _filename.EndsWith(".ttc"))
		{
			string text = GameIO.RemoveExtension(_filename.Substring(2), ".ttc");
			int num = text.IndexOf(".");
			int x;
			int y;
			if (num > 0 && int.TryParse(text.Substring(0, num), out x) && int.TryParse(text.Substring(num + 1), out y))
			{
				_key = WorldChunkCache.MakeChunkKey(x, y);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004F2D RID: 20269 RVA: 0x001F59E9 File Offset: 0x001F3BE9
	public static string MakeFilename(int _x, int _z)
	{
		return string.Concat(new string[]
		{
			"r.",
			_x.ToString(),
			".",
			_z.ToString(),
			".ttc"
		});
	}

	// Token: 0x06004F2E RID: 20270 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Close()
	{
	}

	// Token: 0x1700081D RID: 2077
	// (get) Token: 0x06004F2F RID: 20271
	public abstract int ChunksPerRegionPerDimension { get; }

	// Token: 0x06004F30 RID: 20272
	public abstract void ReadDirectory(string _dir, Action<long, string, uint> _chunkAndTimeStampHandler);

	// Token: 0x06004F31 RID: 20273
	public abstract Stream GetOutputStream(string _dir, int _chunkX, int _chunkZ, string _ext);

	// Token: 0x06004F32 RID: 20274
	public abstract Stream GetInputStream(string _dir, int _chunkX, int _chunkZ, string _ext);

	// Token: 0x06004F33 RID: 20275
	public abstract void Remove(string _dir, int _chunkX, int _chunkZ);

	// Token: 0x06004F34 RID: 20276 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OptimizeLayouts()
	{
	}

	// Token: 0x06004F35 RID: 20277
	public abstract void ClearCache();

	// Token: 0x06004F36 RID: 20278
	public abstract void RemoveRegionFromCache(int _regionX, int _regionZ, string _dir);

	// Token: 0x06004F37 RID: 20279 RVA: 0x000424BD File Offset: 0x000406BD
	public virtual int GetChunkByteCount(string _dir, int _chunkX, int _chunkZ)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06004F38 RID: 20280 RVA: 0x000424BD File Offset: 0x000406BD
	public virtual long GetTotalByteCount(string _dir)
	{
		throw new NotImplementedException();
	}
}
