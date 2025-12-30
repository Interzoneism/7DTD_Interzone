using System;

// Token: 0x02000F31 RID: 3889
public static class BundleTags
{
	// Token: 0x17000D04 RID: 3332
	// (get) Token: 0x06007BF6 RID: 31734 RVA: 0x00322B30 File Offset: 0x00320D30
	public static string Tag
	{
		get
		{
			if (!PlatformOptimizations.LoadHalfResAssets)
			{
				return string.Empty;
			}
			return "_halfres";
		}
	}

	// Token: 0x04005DD7 RID: 24023
	public const string TagHalfRes = "_halfres";
}
