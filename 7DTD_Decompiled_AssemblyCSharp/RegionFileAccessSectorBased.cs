using System;

// Token: 0x02000A1E RID: 2590
public class RegionFileAccessSectorBased : RegionFileAccessMultipleChunks
{
	// Token: 0x1700081F RID: 2079
	// (get) Token: 0x06004F58 RID: 20312 RVA: 0x00169085 File Offset: 0x00167285
	public override int ChunksPerRegionPerDimension
	{
		get
		{
			return 32;
		}
	}

	// Token: 0x06004F59 RID: 20313 RVA: 0x001F6361 File Offset: 0x001F4561
	public override void ReadDirectory(string _dir, Action<long, string, uint> _chunkAndTimeStampHandler)
	{
		this.ReadDirectory(_dir, _chunkAndTimeStampHandler, 32);
	}

	// Token: 0x06004F5A RID: 20314 RVA: 0x001F636D File Offset: 0x001F456D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override RegionFile OpenRegionFile(string _dir, int _regionX, int _regionZ, string _ext)
	{
		return RegionFileSectorBased.Get(_dir, _regionX, _regionZ, _ext);
	}

	// Token: 0x06004F5B RID: 20315 RVA: 0x001F6379 File Offset: 0x001F4579
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetRegionCoords(int _chunkX, int _chunkZ, out int _regionX, out int _regionZ)
	{
		_regionX = (int)Math.Floor((double)_chunkX / 32.0);
		_regionZ = (int)Math.Floor((double)_chunkZ / 32.0);
	}
}
