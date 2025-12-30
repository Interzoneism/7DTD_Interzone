using System;

// Token: 0x02000A68 RID: 2664
public class VertexDistortion
{
	// Token: 0x06005102 RID: 20738 RVA: 0x00204838 File Offset: 0x00202A38
	[PublicizedFrom(EAccessModifier.Private)]
	static VertexDistortion()
	{
		for (int i = 0; i < VertexDistortion.arrayB.Length; i++)
		{
			VertexDistortion.arrayB[i] *= 1.5f;
		}
	}

	// Token: 0x04003E44 RID: 15940
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] arrayB = new float[]
	{
		0f,
		0.2f,
		0.15f,
		0.1f,
		0.1f,
		0.1f,
		0.1f,
		0.1f,
		0.1f
	};
}
