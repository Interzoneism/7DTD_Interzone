using System;

namespace Platform
{
	// Token: 0x02001863 RID: 6243
	public static class EUserBlockStateExtensions
	{
		// Token: 0x0600B908 RID: 47368 RVA: 0x0046BFA0 File Offset: 0x0046A1A0
		public static bool IsBlocked(this EUserBlockState blockState)
		{
			bool result;
			if (blockState != EUserBlockState.NotBlocked)
			{
				if (blockState - EUserBlockState.InGame > 1)
				{
					throw new ArgumentOutOfRangeException("blockState", blockState, string.Format("{0} not implemented for {1}.{2}", "IsBlocked", "EUserBlockState", blockState));
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
