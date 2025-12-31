using System;
using UnityEngine;

// Token: 0x02000AD5 RID: 2773
public abstract class TerrainGeneratorWithBiomeResource : ITerrainGenerator
{
	// Token: 0x06005557 RID: 21847 RVA: 0x0022CD4F File Offset: 0x0022AF4F
	public virtual void Init(World _world, IBiomeProvider _biomeProvider, int _seed)
	{
		this.biomeProvider = _biomeProvider;
		this.perlinNoise = new PerlinNoise(_seed);
	}

	// Token: 0x06005558 RID: 21848
	public abstract byte GetTerrainHeightByteAt(int _xWorld, int _zWorld);

	// Token: 0x06005559 RID: 21849 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public virtual float GetTerrainHeightAt(int x, int z)
	{
		return 0f;
	}

	// Token: 0x0600555A RID: 21850 RVA: 0x000F387A File Offset: 0x000F1A7A
	public virtual Vector3 GetTerrainNormalAt(int _xWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity)
	{
		return Vector3.up;
	}

	// Token: 0x0600555B RID: 21851 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isTerrainAt(int _x, int _y, int _z)
	{
		return true;
	}

	// Token: 0x0600555C RID: 21852
	public abstract sbyte GetDensityAt(int _xWorld, int _yWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity);

	// Token: 0x0600555D RID: 21853 RVA: 0x0022CD64 File Offset: 0x0022AF64
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void fillDensityInBlock(Chunk _chunk, int _x, int _y, int _z, BlockValue _bv)
	{
		sbyte density = _bv.Block.shape.IsTerrain() ? MarchingCubes.DensityTerrain : MarchingCubes.DensityAir;
		_chunk.SetDensity(_x, _y, _z, density);
	}

	// Token: 0x0600555E RID: 21854 RVA: 0x0022CD9C File Offset: 0x0022AF9C
	public virtual void GenerateTerrain(World _world, Chunk _chunk, GameRandom _random)
	{
		this.GenerateTerrain(_world, _chunk, _random, Vector3i.zero, Vector3i.zero, false, false);
	}

	// Token: 0x0600555F RID: 21855 RVA: 0x0022CDB4 File Offset: 0x0022AFB4
	public virtual void GenerateTerrain(World _world, Chunk _chunk, GameRandom _random, Vector3i _areaStart, Vector3i _areaSize, bool _bFillEmptyBlocks, bool _isReset)
	{
		int num = 0;
		int num2 = 16;
		int num3 = 0;
		int num4 = 16;
		if (_areaSize.x > 0 && _areaSize.z > 0)
		{
			Vector3i vector3i = _chunk.ToWorldPos(Vector3i.zero);
			Vector3i vector3i2 = vector3i + new Vector3i(16, 0, 16);
			if (vector3i2.x <= _areaStart.x || vector3i.x > _areaStart.x + _areaSize.x || vector3i2.z <= _areaStart.z || vector3i.z > _areaStart.z + _areaSize.z)
			{
				return;
			}
			num = _areaStart.x - vector3i.x;
			num2 = vector3i2.x - (_areaStart.x + _areaSize.x);
			num3 = _areaStart.z - vector3i.z;
			num4 = vector3i2.z - (_areaStart.z + _areaSize.z);
			if (num < 0)
			{
				num = 0;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num2 < 0)
			{
				num2 = 16;
			}
			else
			{
				num2 = 16 - num2;
			}
			if (num4 < 0)
			{
				num4 = 16;
			}
			else
			{
				num4 = 16 - num4;
			}
		}
		for (int i = num3; i < num4; i++)
		{
			int blockWorldPosZ = _chunk.GetBlockWorldPosZ(i);
			for (int j = num; j < num2; j++)
			{
				int blockWorldPosX = _chunk.GetBlockWorldPosX(j);
				BiomeDefinition biomeDefinition = this.biomeProvider.GetBiomeAt(blockWorldPosX, blockWorldPosZ);
				if (biomeDefinition != null)
				{
					_chunk.SetBiomeId(j, i, biomeDefinition.m_Id);
					byte terrainHeightByteAt = this.GetTerrainHeightByteAt(blockWorldPosX, blockWorldPosZ);
					_chunk.SetTerrainHeight(j, i, terrainHeightByteAt);
					_chunk.SetHeight(j, i, terrainHeightByteAt);
					int subBiomeIdxAt = this.biomeProvider.GetSubBiomeIdxAt(biomeDefinition, blockWorldPosX, (int)terrainHeightByteAt, blockWorldPosZ);
					if (subBiomeIdxAt >= 0 && subBiomeIdxAt < biomeDefinition.subbiomes.Count)
					{
						biomeDefinition = biomeDefinition.subbiomes[subBiomeIdxAt];
					}
					int num5 = (int)(terrainHeightByteAt + 1 & byte.MaxValue);
					if (_bFillEmptyBlocks)
					{
						for (int k = 255; k >= num5; k--)
						{
							_chunk.SetBlockRaw(j, k, i, BlockValue.Air);
							_chunk.SetDensity(j, k, i, MarchingCubes.DensityAir);
						}
					}
					this.fillDensityInBlock(_chunk, j, num5, i, BlockValue.Air);
					num5--;
					if (num5 >= 0)
					{
						int num6 = 0;
						while (num6 < biomeDefinition.m_Layers.Count && num5 >= 0)
						{
							BiomeLayer biomeLayer = biomeDefinition.m_Layers[num6];
							int num7 = biomeLayer.m_Depth;
							if (num7 == -1)
							{
								num7 = (int)terrainHeightByteAt - biomeDefinition.TotalLayerDepth;
							}
							int num8 = 0;
							while (num8 < num7 && num5 >= 0)
							{
								if (num6 != biomeDefinition.m_Layers.Count - 1 && num5 < biomeDefinition.m_Layers[biomeDefinition.m_Layers.Count - 1].m_Depth)
								{
									num6 = biomeDefinition.m_Layers.Count - 2;
									break;
								}
								BlockValue blockValue = BlockValue.Air;
								if (this.isTerrainAt(j, num5, i))
								{
									int count = biomeLayer.m_Resources.Count;
									if (count > 0 && GameUtils.GetOreNoiseAt(this.perlinNoise, blockWorldPosX, num5, blockWorldPosZ) > 0f)
									{
										float randomFloat = _random.RandomFloat;
										for (int l = 0; l < count; l++)
										{
											if (randomFloat < biomeLayer.SumResourceProbs[l])
											{
												blockValue = biomeLayer.m_Resources[l].blockValues[0];
												break;
											}
										}
									}
									if (blockValue.isair)
									{
										int num9 = biomeLayer.m_Block.blockValues.Length - 1;
										if (num9 > 0)
										{
											num9 = _random.RandomRange(0, num9 + 1);
										}
										blockValue = biomeLayer.m_Block.blockValues[num9];
									}
									_chunk.SetBlockRaw(j, num5, i, blockValue);
								}
								this.fillDensityInBlock(_chunk, j, num5, i, blockValue);
								num8++;
								num5--;
							}
							num6++;
						}
					}
				}
			}
		}
	}

	// Token: 0x06005560 RID: 21856 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public TerrainGeneratorWithBiomeResource()
	{
	}

	// Token: 0x040041FD RID: 16893
	[PublicizedFrom(EAccessModifier.Private)]
	public IBiomeProvider biomeProvider;

	// Token: 0x040041FE RID: 16894
	[PublicizedFrom(EAccessModifier.Private)]
	public PerlinNoise perlinNoise;
}
