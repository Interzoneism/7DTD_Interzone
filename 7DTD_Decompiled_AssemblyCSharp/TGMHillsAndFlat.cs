using System;
using LibNoise;
using LibNoise.Modifiers;

// Token: 0x02000AD7 RID: 2775
public class TGMHillsAndFlat : TGMAbstract
{
	// Token: 0x06005567 RID: 21863 RVA: 0x0022D214 File Offset: 0x0022B414
	public TGMHillsAndFlat()
	{
		int num = 0;
		IModule source = new ScaleBiasOutput(new FastBillow
		{
			Frequency = 4.0
		})
		{
			Scale = 0.4,
			Bias = 1.0
		};
		IModule source2 = new ScaleBiasOutput(new FastTurbulence(new ScaleBiasOutput(new FastRidgedMultifractal(num)
		{
			Frequency = 5.0
		})
		{
			Scale = 1.2,
			Bias = 4.0
		})
		{
			Power = 0.45,
			Frequency = 3.0,
			Roughness = 3
		})
		{
			Scale = 0.800000011920929,
			Bias = 9.0
		};
		Select select = new Select(new FastNoise(num + 1)
		{
			Frequency = 3.0
		}, source, source2);
		select.SetBounds(0.0, 0.5);
		select.EdgeFalloff = 0.5;
		this.outputModule = select;
		this.IsSeedSet = true;
	}

	// Token: 0x06005568 RID: 21864 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetSeed(int _seed)
	{
	}

	// Token: 0x06005569 RID: 21865 RVA: 0x0022D339 File Offset: 0x0022B539
	public override float GetValue(float _x, float _z, float _biomeIntens)
	{
		return 1f * _biomeIntens + (float)this.outputModule.GetValue((double)_x, 0.0, (double)_z) * _biomeIntens;
	}

	// Token: 0x04004202 RID: 16898
	[PublicizedFrom(EAccessModifier.Private)]
	public IModule outputModule;
}
