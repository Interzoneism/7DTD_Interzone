using System;

// Token: 0x020009E6 RID: 2534
public class LightingAround
{
	// Token: 0x06004DA6 RID: 19878 RVA: 0x001EA93C File Offset: 0x001E8B3C
	public LightingAround(byte _sun, byte _block, byte _stabilityMiddle)
	{
		Lighting lighting = new Lighting(_sun, _block, _stabilityMiddle);
		for (int i = 0; i < 9; i++)
		{
			this.lights[i] = lighting;
		}
	}

	// Token: 0x06004DA7 RID: 19879 RVA: 0x001EA980 File Offset: 0x001E8B80
	public void SetStab(byte _stab)
	{
		for (int i = 0; i < 9; i++)
		{
			this.lights[i].stability = _stab;
		}
	}

	// Token: 0x170007FC RID: 2044
	public Lighting this[LightingAround.Pos _pos]
	{
		get
		{
			return this.lights[(int)_pos];
		}
		set
		{
			this.lights[(int)_pos] = value;
		}
	}

	// Token: 0x04003B3A RID: 15162
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCount = 9;

	// Token: 0x04003B3B RID: 15163
	[PublicizedFrom(EAccessModifier.Private)]
	public Lighting[] lights = new Lighting[9];

	// Token: 0x020009E7 RID: 2535
	public enum Pos
	{
		// Token: 0x04003B3D RID: 15165
		Middle,
		// Token: 0x04003B3E RID: 15166
		X0Y0Z0,
		// Token: 0x04003B3F RID: 15167
		X1Y0Z0,
		// Token: 0x04003B40 RID: 15168
		X1Y0Z1,
		// Token: 0x04003B41 RID: 15169
		X0Y0Z1,
		// Token: 0x04003B42 RID: 15170
		X0Y1Z0,
		// Token: 0x04003B43 RID: 15171
		X1Y1Z0,
		// Token: 0x04003B44 RID: 15172
		X1Y1Z1,
		// Token: 0x04003B45 RID: 15173
		X0Y1Z1
	}
}
