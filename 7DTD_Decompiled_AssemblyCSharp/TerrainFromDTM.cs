using System;

// Token: 0x02000AD3 RID: 2771
[PublicizedFrom(EAccessModifier.Internal)]
public class TerrainFromDTM : TerrainGeneratorWithBiomeResource
{
	// Token: 0x06005549 RID: 21833 RVA: 0x0022C8B1 File Offset: 0x0022AAB1
	public void Init(ArrayWithOffset<byte> _dtm, IBiomeProvider _biomeProvider, string levelName, int _seed)
	{
		base.Init(null, _biomeProvider, _seed);
		this.m_DTM = _dtm;
		this.heightData = HeightMapUtils.ConvertDTMToHeightData(levelName);
		this.heightData = HeightMapUtils.SmoothTerrain(5, this.heightData);
	}

	// Token: 0x0600554A RID: 21834 RVA: 0x0022C8E4 File Offset: 0x0022AAE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fillDensityInBlock(Chunk _chunk, int _x, int _y, int _z, BlockValue _bv)
	{
		int num = (_chunk.X << 4) + _x + this.heightData.GetLength(0) / 2;
		int num2 = (_chunk.Z << 4) + _z + this.heightData.GetLength(1) / 2;
		if (num < 0 || num2 < 0 || num >= this.heightData.GetLength(0) || num2 >= this.heightData.GetLength(1))
		{
			return;
		}
		float num3 = this.heightData[num, num2] + 0.5f;
		int num4 = (int)num3;
		sbyte density;
		if (_y < num4)
		{
			density = MarchingCubes.DensityTerrain;
		}
		else if (_y > num4 + 1)
		{
			density = MarchingCubes.DensityAir;
		}
		else
		{
			float num5 = num3 - (float)num4;
			if (num4 == _y)
			{
				density = (sbyte)((float)MarchingCubes.DensityTerrain * num5);
			}
			else
			{
				density = (sbyte)((float)MarchingCubes.DensityAir * (1f - num5));
			}
		}
		_chunk.SetDensity(_x, _y, _z, density);
	}

	// Token: 0x0600554B RID: 21835 RVA: 0x0022C9B6 File Offset: 0x0022ABB6
	public override sbyte GetDensityAt(int _xWorld, int _yWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity)
	{
		if ((int)this.m_DTM[_xWorld, _zWorld] > _yWorld)
		{
			return MarchingCubes.DensityAir;
		}
		return MarchingCubes.DensityTerrain;
	}

	// Token: 0x0600554C RID: 21836 RVA: 0x0022C9D4 File Offset: 0x0022ABD4
	public override byte GetTerrainHeightByteAt(int _x, int _z)
	{
		int num = _x + this.heightData.GetLength(0) / 2;
		int num2 = _z + this.heightData.GetLength(1) / 2;
		if (num < 0 || num2 < 0 || num >= this.heightData.GetLength(0) || num2 >= this.heightData.GetLength(1))
		{
			return 0;
		}
		return (byte)((int)(this.heightData[num, num2] + 0.5f));
	}

	// Token: 0x0600554D RID: 21837 RVA: 0x0022CA40 File Offset: 0x0022AC40
	public override float GetTerrainHeightAt(int x, int z)
	{
		float result;
		try
		{
			x += this.heightData.GetLength(0) / 2;
			z += this.heightData.GetLength(1) / 2;
			result = this.heightData[x, z];
		}
		catch (Exception)
		{
			result = 0f;
		}
		return result;
	}

	// Token: 0x040041F6 RID: 16886
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayWithOffset<byte> m_DTM;

	// Token: 0x040041F7 RID: 16887
	[PublicizedFrom(EAccessModifier.Private)]
	public float[,] heightData;
}
