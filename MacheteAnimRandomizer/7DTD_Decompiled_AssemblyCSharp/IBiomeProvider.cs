using System;
using System.Collections;

// Token: 0x02000ACF RID: 2767
public interface IBiomeProvider
{
	// Token: 0x0600552D RID: 21805
	IEnumerator InitData();

	// Token: 0x0600552E RID: 21806
	void Init(int _seed, string _worldName, WorldBiomes _biomes, string _params1, string _params2);

	// Token: 0x0600552F RID: 21807
	int GetSubBiomeIdxAt(BiomeDefinition bd, int _x, int _y, int _z);

	// Token: 0x06005530 RID: 21808
	BiomeDefinition GetBiomeAt(int _x, int _z);

	// Token: 0x06005531 RID: 21809
	BiomeDefinition GetBiomeAt(int _x, int _z, out float _intensity);

	// Token: 0x06005532 RID: 21810 RVA: 0x00019766 File Offset: 0x00017966
	BiomeDefinition GetBiomeOrSubAt(int x, int z)
	{
		return null;
	}

	// Token: 0x06005533 RID: 21811
	float GetHumidityAt(int x, int z);

	// Token: 0x06005534 RID: 21812
	float GetTemperatureAt(int x, int z);

	// Token: 0x06005535 RID: 21813
	float GetRadiationAt(int x, int z);

	// Token: 0x06005536 RID: 21814
	string GetWorldName();

	// Token: 0x06005537 RID: 21815
	BlockValue GetTopmostBlockValue(int xWorld, int zWorld);

	// Token: 0x06005538 RID: 21816 RVA: 0x00002914 File Offset: 0x00000B14
	void Cleanup()
	{
	}
}
