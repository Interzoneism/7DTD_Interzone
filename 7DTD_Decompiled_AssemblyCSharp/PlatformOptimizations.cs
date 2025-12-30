using System;
using Platform;

// Token: 0x0200005B RID: 91
public static class PlatformOptimizations
{
	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000F5AB File Offset: 0x0000D7AB
	public static int MaxWorldSizeHost
	{
		get
		{
			return LaunchPrefs.MaxWorldSizeHost.Value;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000F5B7 File Offset: 0x0000D7B7
	public static bool EnforceMaxWorldSizeHost
	{
		get
		{
			return PlatformOptimizations.MaxWorldSizeHost >= 0;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000F5C4 File Offset: 0x0000D7C4
	public static int MaxWorldSizeClient
	{
		get
		{
			return LaunchPrefs.MaxWorldSizeClient.Value;
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000F5D0 File Offset: 0x0000D7D0
	public static bool EnforceMaxWorldSizeClient
	{
		get
		{
			return PlatformOptimizations.MaxWorldSizeClient >= 0;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x060001AA RID: 426 RVA: 0x0000F5DD File Offset: 0x0000D7DD
	public static XUiC_WorldGenerationWindowGroup.PreviewQuality MaxRWGPreviewQuality
	{
		get
		{
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() && Submission.Enabled)
			{
				return XUiC_WorldGenerationWindowGroup.PreviewQuality.High;
			}
			return EnumUtils.MaxValue<XUiC_WorldGenerationWindowGroup.PreviewQuality>();
		}
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000F5F8 File Offset: 0x0000D7F8
	public static void ConfigureGameObjectPoolForPlatform(GameObjectPool pool)
	{
		if (!(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			return;
		}
		pool.MaxPooledInstancesPerItem = 500;
		pool.MaxDestroysPerUpdate = int.MaxValue;
		pool.ShrinkThresholdHigh = new GameObjectPool.ShrinkThreshold(100, 10, 0f);
		pool.ShrinkThresholdMedium = new GameObjectPool.ShrinkThreshold(50, 2, 0.1f);
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00002914 File Offset: 0x00000B14
	public static void Init()
	{
	}

	// Token: 0x0400025F RID: 607
	[PublicizedFrom(EAccessModifier.Private)]
	public const ulong kB = 1024UL;

	// Token: 0x04000260 RID: 608
	[PublicizedFrom(EAccessModifier.Private)]
	public const ulong MB = 1048576UL;

	// Token: 0x04000261 RID: 609
	[PublicizedFrom(EAccessModifier.Private)]
	public const ulong GB = 1073741824UL;

	// Token: 0x04000262 RID: 610
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag ENABLE_FILE_BACKED_ARRAYS_PLATFORMS = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x04000263 RID: 611
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag ENABLE_FILE_BACKED_BLOCK_PROPERTIES_PLATFORMS = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x04000264 RID: 612
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag ENABLE_FILE_BACKED_TERRAIN_TILES_PLATFORMS = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x04000265 RID: 613
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag ENABLE_FILE_BACKED_RADIATION_TILES_PLATFORMS = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x04000266 RID: 614
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag LOAD_HALF_RES_ASSETS = DeviceFlag.XBoxSeriesS;

	// Token: 0x04000267 RID: 615
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag RESTART_PROCESS_SUPPORTED = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x04000268 RID: 616
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag RESTART_AFTER_RWG = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x04000269 RID: 617
	[PublicizedFrom(EAccessModifier.Private)]
	public const DeviceFlag LIMITED_SAVE_DATA = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

	// Token: 0x0400026A RID: 618
	public static readonly bool FileBackedArrays = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x0400026B RID: 619
	public static readonly bool FileBackedBlockProperties = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x0400026C RID: 620
	public static readonly bool FileBackedTerrainTiles = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x0400026D RID: 621
	public static readonly bool FileBackedRadiationTiles = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x0400026E RID: 622
	public static readonly bool LoadHalfResAssets = DeviceFlag.XBoxSeriesS.IsCurrent();

	// Token: 0x0400026F RID: 623
	public static readonly bool RestartProcessSupported = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x04000270 RID: 624
	public static readonly bool RestartAfterRwg = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x04000271 RID: 625
	public static readonly bool MeshLodReduction = false;

	// Token: 0x04000272 RID: 626
	public static readonly bool LimitedSaveData = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent();

	// Token: 0x04000273 RID: 627
	public static readonly int DefaultMaxWorldSizeHost = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() ? 8192 : -1;

	// Token: 0x04000274 RID: 628
	public const int DefaultMaxWorldSizeClient = -1;
}
