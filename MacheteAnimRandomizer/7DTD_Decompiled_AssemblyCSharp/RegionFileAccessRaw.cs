using System;

// Token: 0x02000A1D RID: 2589
public class RegionFileAccessRaw : RegionFileAccessMultipleChunks
{
	// Token: 0x1700081E RID: 2078
	// (get) Token: 0x06004F53 RID: 20307 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int ChunksPerRegionPerDimension
	{
		get
		{
			return 8;
		}
	}

	// Token: 0x06004F54 RID: 20308 RVA: 0x001F62EA File Offset: 0x001F44EA
	public override void ReadDirectory(string _dir, Action<long, string, uint> _chunkAndTimeStampHandler)
	{
		this.ReadDirectory(_dir, _chunkAndTimeStampHandler, 8);
	}

	// Token: 0x06004F55 RID: 20309 RVA: 0x001F62F8 File Offset: 0x001F44F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override RegionFile OpenRegionFile(string _dir, int _regionX, int _regionZ, string _ext)
	{
		string text = RegionFile.ConstructFullFilePath(_dir, _regionX, _regionZ, _ext);
		if (SdFile.Exists(text))
		{
			return RegionFileRaw.Load(text, _regionX, _regionZ);
		}
		return RegionFileRaw.New(text, _regionX, _regionZ, 1024);
	}

	// Token: 0x06004F56 RID: 20310 RVA: 0x001F632E File Offset: 0x001F452E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetRegionCoords(int _chunkX, int _chunkZ, out int _regionX, out int _regionZ)
	{
		_regionX = (int)Math.Floor((double)_chunkX / 8.0);
		_regionZ = (int)Math.Floor((double)_chunkZ / 8.0);
	}
}
