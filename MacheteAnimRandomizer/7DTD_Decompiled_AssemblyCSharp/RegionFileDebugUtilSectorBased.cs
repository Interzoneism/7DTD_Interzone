using System;

// Token: 0x02000A26 RID: 2598
public class RegionFileDebugUtilSectorBased : IRegionFileDebugUtil
{
	// Token: 0x06004F78 RID: 20344 RVA: 0x001F6A80 File Offset: 0x001F4C80
	public string GetLocationString(int chunkX, int chunkZ)
	{
		int num = (int)Math.Floor((double)chunkX / 32.0);
		int num2 = (int)Math.Floor((double)chunkZ / 32.0);
		return string.Format("XZ: {0}/{1}  Region: r.{2}.{3}", new object[]
		{
			chunkX,
			chunkZ,
			num,
			num2
		});
	}
}
