using System;

// Token: 0x0200093A RID: 2362
public static class SaveDataLimitUtils
{
	// Token: 0x060046F3 RID: 18163 RVA: 0x001BFE50 File Offset: 0x001BE050
	public static long CalculatePlayerMapSize(Vector2i worldSize)
	{
		int num = worldSize.x * worldSize.y;
		if (num <= 0)
		{
			throw new ArgumentException(string.Format("Expected a positive value for the world area, but was: {0}", num), "worldSize");
		}
		return Math.Min((long)num / 256L * 516L, 270532608L);
	}

	// Token: 0x040036AF RID: 13999
	[PublicizedFrom(EAccessModifier.Private)]
	public const long CHUNK_SIZE = 16L;

	// Token: 0x040036B0 RID: 14000
	[PublicizedFrom(EAccessModifier.Private)]
	public const long CHUNK_AREA = 256L;

	// Token: 0x040036B1 RID: 14001
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PLAYER_MAP_OVERHEAD_PER_CHUNK = 516L;

	// Token: 0x040036B2 RID: 14002
	[PublicizedFrom(EAccessModifier.Private)]
	public const long PLAYER_MAP_MAX_OVERHEAD = 270532608L;
}
