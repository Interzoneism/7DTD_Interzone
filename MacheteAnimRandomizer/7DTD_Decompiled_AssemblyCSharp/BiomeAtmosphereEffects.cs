using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000994 RID: 2452
public class BiomeAtmosphereEffects
{
	// Token: 0x060049C0 RID: 18880 RVA: 0x001D35DC File Offset: 0x001D17DC
	public void Init(World _world)
	{
		this.world = _world;
		this.worldColorSpectrums = new AtmosphereEffect[255];
		this.worldColorSpectrums[0] = AtmosphereEffect.Load("default", null);
		foreach (KeyValuePair<uint, BiomeDefinition> keyValuePair in _world.Biomes.GetBiomeMap())
		{
			this.worldColorSpectrums[(int)keyValuePair.Value.m_Id] = AtmosphereEffect.Load(keyValuePair.Value.m_SpectrumName, this.worldColorSpectrums[0]);
		}
		this.ForceDefault = false;
	}

	// Token: 0x060049C1 RID: 18881 RVA: 0x001D368C File Offset: 0x001D188C
	public void Reload()
	{
		this.Init(this.world);
		this.Update();
	}

	// Token: 0x060049C2 RID: 18882 RVA: 0x001D36A0 File Offset: 0x001D18A0
	public virtual void Update()
	{
		EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		if (this.ForceDefault)
		{
			this.currentBiomeIntensity = BiomeIntensity.Default;
			return;
		}
		Vector3i blockPosition = primaryPlayer.GetBlockPosition();
		BiomeIntensity biomeIntensity;
		if (!blockPosition.Equals(this.playerPosition) && this.world.GetBiomeIntensity(blockPosition, out biomeIntensity))
		{
			this.playerPosition = blockPosition;
			if (!this.currentBiomeIntensity.Equals(biomeIntensity))
			{
				WorldBiomes biomes = GameManager.Instance.World.Biomes;
				BiomeDefinition biome = biomes.GetBiome(biomeIntensity.biomeId0);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity0;
				}
				this.nearBiomes[0] = biome;
				biome = biomes.GetBiome(biomeIntensity.biomeId1);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity1;
				}
				this.nearBiomes[1] = biome;
				biome = biomes.GetBiome(biomeIntensity.biomeId2);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity1;
				}
				this.nearBiomes[2] = biome;
				biome = biomes.GetBiome(biomeIntensity.biomeId3);
				if (biome != null)
				{
					biome.currentPlayerIntensity = biomeIntensity.intensity2;
				}
				this.nearBiomes[3] = biome;
			}
			this.currentBiomeIntensity = biomeIntensity;
		}
	}

	// Token: 0x060049C3 RID: 18883 RVA: 0x001D37CB File Offset: 0x001D19CB
	public virtual Color GetSkyColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Sky);
	}

	// Token: 0x060049C4 RID: 18884 RVA: 0x001D37DB File Offset: 0x001D19DB
	public virtual Color GetAmbientColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Ambient);
	}

	// Token: 0x060049C5 RID: 18885 RVA: 0x001D37EB File Offset: 0x001D19EB
	public virtual Color GetSunColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Sun);
	}

	// Token: 0x060049C6 RID: 18886 RVA: 0x001D37FB File Offset: 0x001D19FB
	public virtual Color GetMoonColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Moon);
	}

	// Token: 0x060049C7 RID: 18887 RVA: 0x001D380B File Offset: 0x001D1A0B
	public virtual Color GetFogColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.Fog);
	}

	// Token: 0x060049C8 RID: 18888 RVA: 0x001D381B File Offset: 0x001D1A1B
	public virtual Color GetFogFadeColorSpectrum(float _v)
	{
		return this.getColorFromSpectrum(this.currentBiomeIntensity, _v, AtmosphereEffect.ESpecIdx.FogFade);
	}

	// Token: 0x060049C9 RID: 18889 RVA: 0x000B195B File Offset: 0x000AFB5B
	public virtual Color GetCloudsColor(float _v)
	{
		return Color.white;
	}

	// Token: 0x060049CA RID: 18890 RVA: 0x001D382C File Offset: 0x001D1A2C
	[PublicizedFrom(EAccessModifier.Private)]
	public Color getColorFromSpectrum(BiomeIntensity _bi, float _v, AtmosphereEffect.ESpecIdx _spectrumIdx)
	{
		float intensity = _bi.intensity0;
		float intensity2 = _bi.intensity1;
		float intensity3 = _bi.intensity2;
		float num = _bi.intensity3;
		num = 0f;
		return (this.worldColorSpectrums[(int)_bi.biomeId0].spectrums[(int)_spectrumIdx].GetValue(_v) * intensity + this.worldColorSpectrums[(int)_bi.biomeId1].spectrums[(int)_spectrumIdx].GetValue(_v) * intensity2 + this.worldColorSpectrums[(int)_bi.biomeId2].spectrums[(int)_spectrumIdx].GetValue(_v) * intensity3 + this.worldColorSpectrums[(int)_bi.biomeId3].spectrums[(int)_spectrumIdx].GetValue(_v) * num) / (intensity + intensity2 + intensity3 + num);
	}

	// Token: 0x04003906 RID: 14598
	public BiomeDefinition[] nearBiomes = new BiomeDefinition[4];

	// Token: 0x04003907 RID: 14599
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeIntensity currentBiomeIntensity = BiomeIntensity.Default;

	// Token: 0x04003908 RID: 14600
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04003909 RID: 14601
	[PublicizedFrom(EAccessModifier.Private)]
	public AtmosphereEffect[] worldColorSpectrums;

	// Token: 0x0400390A RID: 14602
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i playerPosition;

	// Token: 0x0400390B RID: 14603
	public bool ForceDefault;
}
