using System;

// Token: 0x02000939 RID: 2361
public static class SaveDataLimitExtensions
{
	// Token: 0x060046F0 RID: 18160 RVA: 0x001BFD50 File Offset: 0x001BDF50
	public static bool IsSupported(this SaveDataLimitType saveDataLimitType)
	{
		return !PlatformOptimizations.LimitedSaveData || saveDataLimitType > SaveDataLimitType.Unlimited;
	}

	// Token: 0x060046F1 RID: 18161 RVA: 0x001BFD60 File Offset: 0x001BDF60
	[PublicizedFrom(EAccessModifier.Private)]
	public static long GetRegionSizeLimit(this SaveDataLimitType saveDataLimitType)
	{
		if (!saveDataLimitType.IsSupported())
		{
			throw new ArgumentException(string.Format("Unexpected usage of {0}.{1}.{2}() when not supported by the current device.", "SaveDataLimitType", saveDataLimitType, "GetRegionSizeLimit"), "saveDataLimitType");
		}
		long result;
		switch (saveDataLimitType)
		{
		case SaveDataLimitType.Unlimited:
			result = -1L;
			break;
		case SaveDataLimitType.Short:
			result = 33554432L;
			break;
		case SaveDataLimitType.Medium:
			result = 67108864L;
			break;
		case SaveDataLimitType.Long:
			result = 134217728L;
			break;
		case SaveDataLimitType.VeryLong:
			result = 268435456L;
			break;
		default:
			throw new ArgumentOutOfRangeException("saveDataLimitType", saveDataLimitType, null);
		}
		return result;
	}

	// Token: 0x060046F2 RID: 18162 RVA: 0x001BFDF4 File Offset: 0x001BDFF4
	public static long CalculateTotalSize(this SaveDataLimitType saveDataLimitType, Vector2i worldSize)
	{
		if (!saveDataLimitType.IsSupported())
		{
			throw new ArgumentException(string.Format("Unexpected usage of {0}.{1}.{2}() when not supported by the current device.", "SaveDataLimitType", saveDataLimitType, "CalculateTotalSize"), "saveDataLimitType");
		}
		long regionSizeLimit = saveDataLimitType.GetRegionSizeLimit();
		if (regionSizeLimit <= 0L)
		{
			return -1L;
		}
		long num = SaveDataLimitUtils.CalculatePlayerMapSize(worldSize);
		return 104857600L + num + regionSizeLimit;
	}

	// Token: 0x040036AD RID: 13997
	[PublicizedFrom(EAccessModifier.Private)]
	public const long MB = 1048576L;

	// Token: 0x040036AE RID: 13998
	[PublicizedFrom(EAccessModifier.Private)]
	public const long FLAT_OVERHEAD = 104857600L;
}
