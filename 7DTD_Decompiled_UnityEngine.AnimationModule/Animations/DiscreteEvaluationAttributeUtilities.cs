using System;

namespace UnityEngine.Animations
{
	// Token: 0x02000040 RID: 64
	internal static class DiscreteEvaluationAttributeUtilities
	{
		// Token: 0x0600029E RID: 670 RVA: 0x0000463C File Offset: 0x0000283C
		public unsafe static int ConvertFloatToDiscreteInt(float f)
		{
			float* ptr = &f;
			int* ptr2 = (int*)ptr;
			return *ptr2;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00004658 File Offset: 0x00002858
		public unsafe static float ConvertDiscreteIntToFloat(int f)
		{
			int* ptr = &f;
			float* ptr2 = (float*)ptr;
			return *ptr2;
		}
	}
}
