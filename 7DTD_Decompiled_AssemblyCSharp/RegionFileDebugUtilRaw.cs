using System;

// Token: 0x02000A27 RID: 2599
public class RegionFileDebugUtilRaw : IRegionFileDebugUtil
{
	// Token: 0x06004F7A RID: 20346 RVA: 0x001F6AE8 File Offset: 0x001F4CE8
	public string GetLocationString(int chunkX, int chunkZ)
	{
		int num = (int)Math.Floor((double)chunkX / 8.0);
		int num2 = (int)Math.Floor((double)chunkZ / 8.0);
		return string.Format("XZ: {0}/{1}  Region: r.{2}.{3}", new object[]
		{
			chunkX,
			chunkZ,
			num,
			num2
		});
	}
}
