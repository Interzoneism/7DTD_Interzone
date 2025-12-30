using System;

// Token: 0x02000992 RID: 2450
public class AtmosphereEffect
{
	// Token: 0x060049BE RID: 18878 RVA: 0x001D34D4 File Offset: 0x001D16D4
	public static AtmosphereEffect Load(string _folder, AtmosphereEffect _default)
	{
		AtmosphereEffect atmosphereEffect = new AtmosphereEffect();
		string str = "@:Textures/Environment/Spectrums/" + ((_folder != null) ? (_folder + "/") : "");
		atmosphereEffect.spectrums[0] = ColorSpectrum.FromTexture(str + "sky.tga");
		atmosphereEffect.spectrums[1] = ColorSpectrum.FromTexture(str + "ambient.tga");
		atmosphereEffect.spectrums[2] = ColorSpectrum.FromTexture(str + "sun.tga");
		atmosphereEffect.spectrums[3] = ColorSpectrum.FromTexture(str + "moon.tga");
		atmosphereEffect.spectrums[4] = ColorSpectrum.FromTexture(str + "fog.tga");
		atmosphereEffect.spectrums[5] = ColorSpectrum.FromTexture(str + "fogfade.tga");
		if (_default != null)
		{
			for (int i = 0; i < atmosphereEffect.spectrums.Length; i++)
			{
				if (atmosphereEffect.spectrums[i] == null)
				{
					atmosphereEffect.spectrums[i] = _default.spectrums[i];
				}
			}
		}
		return atmosphereEffect;
	}

	// Token: 0x040038FD RID: 14589
	public ColorSpectrum[] spectrums = new ColorSpectrum[6];

	// Token: 0x02000993 RID: 2451
	public enum ESpecIdx
	{
		// Token: 0x040038FF RID: 14591
		Sky,
		// Token: 0x04003900 RID: 14592
		Ambient,
		// Token: 0x04003901 RID: 14593
		Sun,
		// Token: 0x04003902 RID: 14594
		Moon,
		// Token: 0x04003903 RID: 14595
		Fog,
		// Token: 0x04003904 RID: 14596
		FogFade,
		// Token: 0x04003905 RID: 14597
		Count
	}
}
