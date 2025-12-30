using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AD4 RID: 2772
public class TerrainFromRaw : TerrainGeneratorWithBiomeResource
{
	// Token: 0x0600554F RID: 21839 RVA: 0x0022CA9C File Offset: 0x0022AC9C
	public void Init(HeightMap _heightMap, IBiomeProvider _biomeProvider, string levelName, int _seed)
	{
		base.Init(null, _biomeProvider, _seed);
		this.heightMap = _heightMap;
		this.terrainWidth = this.heightMap.GetWidth() << this.heightMap.GetScaleShift();
		this.terrainHeight = this.heightMap.GetHeight() << this.heightMap.GetScaleShift();
		this.terrainWidthHalf = this.terrainWidth / 2;
		this.terrainHeightHalf = this.terrainHeight / 2;
	}

	// Token: 0x06005550 RID: 21840 RVA: 0x0022CB16 File Offset: 0x0022AD16
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool checkCoordinates(ref int _x, ref int _z)
	{
		_x += this.terrainWidthHalf;
		if ((ulong)_x >= (ulong)((long)this.terrainWidth))
		{
			return false;
		}
		_z += this.terrainHeightHalf;
		return (ulong)_z < (ulong)((long)this.terrainHeight);
	}

	// Token: 0x06005551 RID: 21841 RVA: 0x0022CB4C File Offset: 0x0022AD4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fillDensityInBlock(Chunk _chunk, int _x, int _y, int _z, BlockValue _bv)
	{
		int x = (_chunk.X << 4) + _x;
		int z = (_chunk.Z << 4) + _z;
		if (!this.checkCoordinates(ref x, ref z))
		{
			return;
		}
		float num = this.heightMap.GetAt(x, z) + 0.5f;
		int num2 = (int)num;
		sbyte b;
		if (_y < num2)
		{
			b = MarchingCubes.DensityTerrain;
		}
		else if (_y > num2 + 1)
		{
			b = MarchingCubes.DensityAir;
		}
		else
		{
			float num3 = num - (float)num2;
			if (num2 == _y)
			{
				b = (sbyte)((float)MarchingCubes.DensityTerrain * num3);
			}
			else
			{
				b = (sbyte)((float)MarchingCubes.DensityAir * (1f - num3));
			}
			if (b == 0)
			{
				if (_bv.Block.shape.IsTerrain())
				{
					b = -1;
				}
				else
				{
					b = 1;
				}
			}
		}
		_chunk.SetDensity(_x, _y, _z, b);
	}

	// Token: 0x06005552 RID: 21842 RVA: 0x0022CC05 File Offset: 0x0022AE05
	public override sbyte GetDensityAt(int _xWorld, int _yWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity)
	{
		if (!this.checkCoordinates(ref _xWorld, ref _zWorld))
		{
			return MarchingCubes.DensityAir;
		}
		if (this.heightMap.GetAt(_xWorld, _zWorld) > (float)_yWorld)
		{
			return MarchingCubes.DensityAir;
		}
		return MarchingCubes.DensityTerrain;
	}

	// Token: 0x06005553 RID: 21843 RVA: 0x0022CC35 File Offset: 0x0022AE35
	public override byte GetTerrainHeightByteAt(int _x, int _z)
	{
		if (!this.checkCoordinates(ref _x, ref _z))
		{
			return 0;
		}
		return (byte)(this.heightMap.GetAt(_x, _z) + 0.5f);
	}

	// Token: 0x06005554 RID: 21844 RVA: 0x0022CC59 File Offset: 0x0022AE59
	public override float GetTerrainHeightAt(int _x, int _z)
	{
		if (!this.checkCoordinates(ref _x, ref _z))
		{
			return 0f;
		}
		return this.heightMap.GetAt(_x, _z);
	}

	// Token: 0x06005555 RID: 21845 RVA: 0x0022CC7C File Offset: 0x0022AE7C
	public List<float[,]> ConvertToUnityHeightmap(int _sliceAtWidth)
	{
		int width = this.heightMap.GetWidth();
		int height = this.heightMap.GetHeight();
		int scaleSteps = this.heightMap.GetScaleSteps();
		List<float[,]> list = new List<float[,]>();
		int num = width / _sliceAtWidth;
		int num2 = height / _sliceAtWidth;
		new Terrain[num, num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float[,] array = new float[_sliceAtWidth, _sliceAtWidth];
				for (int k = _sliceAtWidth - 1; k >= 0; k--)
				{
					for (int l = _sliceAtWidth - 1; l >= 0; l--)
					{
						float at = this.heightMap.GetAt(j * _sliceAtWidth + k * scaleSteps, i * _sliceAtWidth + l * scaleSteps);
						array[l, k] = at / 256f;
					}
				}
				list.Add(array);
			}
		}
		return list;
	}

	// Token: 0x040041F8 RID: 16888
	[PublicizedFrom(EAccessModifier.Private)]
	public HeightMap heightMap;

	// Token: 0x040041F9 RID: 16889
	[PublicizedFrom(EAccessModifier.Private)]
	public int terrainWidth;

	// Token: 0x040041FA RID: 16890
	[PublicizedFrom(EAccessModifier.Private)]
	public int terrainHeight;

	// Token: 0x040041FB RID: 16891
	[PublicizedFrom(EAccessModifier.Private)]
	public int terrainWidthHalf;

	// Token: 0x040041FC RID: 16892
	[PublicizedFrom(EAccessModifier.Private)]
	public int terrainHeightHalf;
}
