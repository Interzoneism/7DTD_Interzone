using System;
using System.Collections;

// Token: 0x02000AD8 RID: 2776
public class WorldBiomeProviderFromHeight : IBiomeProvider
{
	// Token: 0x0600556A RID: 21866 RVA: 0x0022D35E File Offset: 0x0022B55E
	public WorldBiomeProviderFromHeight(string _levelName, ITerrainGenerator _terrainGenerator, int _waterLevel, BiomeDefinition _waterBiome, BiomeDefinition _otherBiome)
	{
		this.worldName = _levelName;
		this.waterBiome = _waterBiome;
		this.otherBiome = _otherBiome;
		this.waterLevel = _waterLevel;
		this.terrainGenerator = _terrainGenerator;
	}

	// Token: 0x0600556B RID: 21867 RVA: 0x0022D38B File Offset: 0x0022B58B
	public string GetWorldName()
	{
		return this.worldName;
	}

	// Token: 0x0600556C RID: 21868 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetBiomeSizeFactor(float _factor)
	{
	}

	// Token: 0x0600556D RID: 21869 RVA: 0x0022D393 File Offset: 0x0022B593
	public IEnumerator InitData()
	{
		yield break;
	}

	// Token: 0x0600556E RID: 21870 RVA: 0x00002914 File Offset: 0x00000B14
	public void Init(int _seed, string _worldName, WorldBiomes _biomes, string _params1, string _params2)
	{
	}

	// Token: 0x0600556F RID: 21871 RVA: 0x000ECF59 File Offset: 0x000EB159
	public int GetSubBiomeIdxAt(BiomeDefinition bd, int _x, int _y, int _z)
	{
		return -1;
	}

	// Token: 0x06005570 RID: 21872 RVA: 0x0022D39B File Offset: 0x0022B59B
	public BiomeDefinition GetBiomeAt(int x, int z, out float _intensity)
	{
		_intensity = 1f;
		return this.GetBiomeAt(x, z);
	}

	// Token: 0x06005571 RID: 21873 RVA: 0x0022D3AC File Offset: 0x0022B5AC
	public BiomeDefinition GetBiomeAt(int x, int z)
	{
		if (this.terrainGenerator.GetTerrainHeightAt(x, z) <= (float)this.waterLevel)
		{
			return this.waterBiome;
		}
		return this.otherBiome;
	}

	// Token: 0x06005572 RID: 21874 RVA: 0x0022D3D1 File Offset: 0x0022B5D1
	public Vector2i GetSize()
	{
		return new Vector2i(1, 1);
	}

	// Token: 0x06005573 RID: 21875 RVA: 0x0022D3DA File Offset: 0x0022B5DA
	public BiomeIntensity GetBiomeIntensityAt(int _x, int _z)
	{
		return BiomeIntensity.Default;
	}

	// Token: 0x06005574 RID: 21876 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetHumidityAt(int x, int z)
	{
		return 0f;
	}

	// Token: 0x06005575 RID: 21877 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetTemperatureAt(int x, int z)
	{
		return 0f;
	}

	// Token: 0x06005576 RID: 21878 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetRadiationAt(int x, int z)
	{
		return 0f;
	}

	// Token: 0x06005577 RID: 21879 RVA: 0x0018D09D File Offset: 0x0018B29D
	public BlockValue GetTopmostBlockValue(int xWorld, int zWorld)
	{
		return BlockValue.Air;
	}

	// Token: 0x04004203 RID: 16899
	[PublicizedFrom(EAccessModifier.Private)]
	public string worldName;

	// Token: 0x04004204 RID: 16900
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition waterBiome;

	// Token: 0x04004205 RID: 16901
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition otherBiome;

	// Token: 0x04004206 RID: 16902
	[PublicizedFrom(EAccessModifier.Private)]
	public ITerrainGenerator terrainGenerator;

	// Token: 0x04004207 RID: 16903
	[PublicizedFrom(EAccessModifier.Private)]
	public int waterLevel;
}
