using System;
using UnityEngine;

// Token: 0x020009E4 RID: 2532
public struct Lighting
{
	// Token: 0x06004D9C RID: 19868 RVA: 0x001EA69F File Offset: 0x001E889F
	public Lighting(byte _sun, byte _block, byte _stability)
	{
		this.sun = _sun;
		this.block = _block;
		this.stability = _stability;
	}

	// Token: 0x06004D9D RID: 19869 RVA: 0x001EA6B6 File Offset: 0x001E88B6
	public Color ToColor()
	{
		return new Color((float)this.sun * 0.06666667f, 0f, (float)this.stability * 0.06666667f, (float)this.block * 0.06666667f);
	}

	// Token: 0x06004D9E RID: 19870 RVA: 0x001EA6EC File Offset: 0x001E88EC
	public static Color ToColor(int _sunLight, int _blockLight)
	{
		Color result;
		result.r = (float)_sunLight * 0.06666667f;
		result.g = 0f;
		result.b = 0f;
		result.a = (float)_blockLight * 0.06666667f;
		return result;
	}

	// Token: 0x06004D9F RID: 19871 RVA: 0x001EA730 File Offset: 0x001E8930
	public static Color ToColor(int _sunLight, int _blockLight, float _sideFactor)
	{
		Color result;
		result.r = (float)_sunLight * 0.06666667f * _sideFactor;
		result.g = 0f;
		result.b = 0f;
		result.a = (float)_blockLight * 0.06666667f;
		return result;
	}

	// Token: 0x04003B2E RID: 15150
	public static Lighting one = new Lighting(15, 15, 0);

	// Token: 0x04003B2F RID: 15151
	public byte sun;

	// Token: 0x04003B30 RID: 15152
	public byte block;

	// Token: 0x04003B31 RID: 15153
	public byte stability;

	// Token: 0x04003B32 RID: 15154
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cToPer = 0.06666667f;
}
