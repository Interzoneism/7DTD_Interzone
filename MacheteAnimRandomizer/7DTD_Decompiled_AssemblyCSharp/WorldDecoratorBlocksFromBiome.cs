using System;
using System.Collections.Generic;
using System.Threading;

// Token: 0x02000ADD RID: 2781
public class WorldDecoratorBlocksFromBiome : IWorldDecorator
{
	// Token: 0x060055A4 RID: 21924 RVA: 0x0022F17C File Offset: 0x0022D37C
	public WorldDecoratorBlocksFromBiome(IBiomeProvider _biomeProvider, DynamicPrefabDecorator _prefabDecorator)
	{
		this.biomeProvider = _biomeProvider;
		this.prefabDecorator = _prefabDecorator;
	}

	// Token: 0x060055A5 RID: 21925 RVA: 0x0022F1D0 File Offset: 0x0022D3D0
	public void DecorateChunkOverlapping(World _world, Chunk _chunk, Chunk cX1Z0, Chunk cX0Z1, Chunk cX1Z1, int seed)
	{
		this.rwlock.EnterWriteLock();
		GameRandom gameRandom = Utils.RandomFromSeedOnPos(_chunk.X, _chunk.Z, seed);
		if (this.resourceNoise == null)
		{
			this.resourceNoise = new PerlinNoise(seed);
		}
		for (int i = this.biomePositionsHighId; i >= 0; i--)
		{
			List<Vector2i> list = this.biomePositions[i];
			if (list != null)
			{
				list.Clear();
			}
		}
		IChunkProvider chunkProvider = _world.ChunkCache.ChunkProvider;
		BiomeDefinition biome = _world.GetBiome("underwater");
		int num = 0;
		int blockWorldPosX = _chunk.GetBlockWorldPosX(0);
		int blockWorldPosZ = _chunk.GetBlockWorldPosZ(0);
		bool flag = this.prefabDecorator.IsWithinTraderArea(_chunk.worldPosIMin, _chunk.worldPosIMax);
		for (int j = 0; j < 16; j++)
		{
			int z = blockWorldPosZ + j;
			Vector2i item;
			item.y = j;
			for (int k = 0; k < 16; k++)
			{
				int x = blockWorldPosX + k;
				if (!flag || this.prefabDecorator.GetTraderAtPosition(new Vector3i(x, 0, z), 0) == null)
				{
					int poiblockIdOverride = chunkProvider.GetPOIBlockIdOverride(x, z);
					BiomeDefinition biomeDefinition;
					if (poiblockIdOverride > 0 && Block.list[poiblockIdOverride].blockMaterial.IsLiquid)
					{
						biomeDefinition = biome;
					}
					else
					{
						biomeDefinition = this.biomeProvider.GetBiomeAt(x, z);
					}
					if (biomeDefinition == null)
					{
						this.rwlock.ExitWriteLock();
						return;
					}
					List<Vector2i> list2 = this.biomePositions[(int)biomeDefinition.m_Id];
					if (list2 == null)
					{
						list2 = new List<Vector2i>(256);
						this.biomePositions[(int)biomeDefinition.m_Id] = list2;
						this.biomePositionsHighId = Utils.FastMax(this.biomePositionsHighId, (int)biomeDefinition.m_Id);
					}
					item.x = k;
					list2.Add(item);
					int terrainHeight = (int)_chunk.GetTerrainHeight(k, j);
					int subBiomeIdxAt = this.biomeProvider.GetSubBiomeIdxAt(biomeDefinition, x, terrainHeight, z);
					if (subBiomeIdxAt >= 0)
					{
						biomeDefinition = biomeDefinition.subbiomes[subBiomeIdxAt];
					}
					this.chunkBiomes[num] = biomeDefinition;
					num++;
				}
			}
		}
		this.decoratePrefabs(_world, _chunk, cX1Z0, cX0Z1, cX1Z1, flag, gameRandom);
		this.decorateSingleBlocks(_world, _chunk, cX1Z0, cX0Z1, cX1Z1, flag, gameRandom);
		GameRandomManager.Instance.FreeGameRandom(gameRandom);
		this.rwlock.ExitWriteLock();
	}

	// Token: 0x060055A6 RID: 21926 RVA: 0x0022F404 File Offset: 0x0022D604
	[PublicizedFrom(EAccessModifier.Private)]
	public void decoratePrefabs(World _world, Chunk _chunk, Chunk cX1Z0, Chunk cX0Z1, Chunk cX1Z1, bool _chunkOverlapsTrader, GameRandom _random)
	{
		if (this.prefabDecorator == null)
		{
			return;
		}
		for (int i = 0; i <= this.biomePositionsHighId; i++)
		{
			List<Vector2i> list = this.biomePositions[i];
			if (list != null)
			{
				int count = list.Count;
				if (count != 0)
				{
					for (int j = 0; j < count; j++)
					{
						int x = list[j].x;
						int y = list[j].y;
						int blockWorldPosX = _chunk.GetBlockWorldPosX(x);
						int blockWorldPosZ = _chunk.GetBlockWorldPosZ(y);
						if (!_chunkOverlapsTrader || this.prefabDecorator.GetTraderAtPosition(new Vector3i(blockWorldPosX, 0, blockWorldPosZ), 0) == null)
						{
							BiomeDefinition biomeDefinition = this.chunkBiomes[x + y * 16];
							if (biomeDefinition.m_DecoPrefabs.Count != 0)
							{
								BiomePrefabDecoration biomePrefabDecoration = biomeDefinition.m_DecoPrefabs[_random.RandomRange(biomeDefinition.m_DecoPrefabs.Count)];
								if (biomePrefabDecoration != null && biomePrefabDecoration.prob >= _random.RandomFloat)
								{
									EnumDecoAllowed decoAllowedAt = _chunk.GetDecoAllowedAt(x, y);
									if (!decoAllowedAt.IsNothing() && !decoAllowedAt.GetStreetOnly())
									{
										EnumDecoAllowedSlope slope = decoAllowedAt.GetSlope();
										if (!biomePrefabDecoration.isDecorateOnSlopes)
										{
											if (slope >= EnumDecoAllowedSlope.Sloped)
											{
												goto IL_3F1;
											}
										}
										else if (slope >= EnumDecoAllowedSlope.Steep)
										{
											goto IL_3F1;
										}
										Prefab prefab = this.prefabDecorator.GetPrefab(biomePrefabDecoration.prefabName, true, true, false);
										if (prefab == null)
										{
											Log.Error("Prefab with name '" + biomePrefabDecoration.prefabName + "' not found!");
										}
										else
										{
											Vector3i size = prefab.size;
											if (x + size.x / 2 < 16 && y + size.z / 2 < 16)
											{
												int num = (int)(_chunk.GetTerrainHeight(x + size.x / 2, y + size.z / 2) + 1);
												if ((biomePrefabDecoration.checkResourceOffsetY >= 2147483647 || (num + prefab.yOffset >= 0 && GameUtils.CheckOreNoiseAt(this.resourceNoise, blockWorldPosX, num + biomePrefabDecoration.checkResourceOffsetY, blockWorldPosZ))) && _chunk.GetBlock(x, num + 1, y).isair)
												{
													BlockValue block = _chunk.GetBlock(x, num - 1, y);
													if (!block.isair && !block.Equals(Block.GetBlockValue("water", false)))
													{
														bool flag = true;
														bool flag2 = size.x > 1 || size.z > 1;
														int num2 = 0;
														while (flag && num2 < size.x)
														{
															int num3 = x + num2;
															int x2 = World.toBlockXZ(blockWorldPosX + num2);
															int num4 = 0;
															while (flag && num4 < size.z)
															{
																Chunk chunk = _chunk;
																int num5 = y + num4;
																if (num3 >= 16)
																{
																	chunk = cX1Z0;
																	if (num5 >= 16)
																	{
																		chunk = cX1Z1;
																	}
																}
																else if (num5 >= 16)
																{
																	chunk = cX0Z1;
																}
																int z = World.toBlockXZ(blockWorldPosZ + num4);
																EnumDecoAllowed decoAllowedAt2 = chunk.GetDecoAllowedAt(x2, z);
																if (decoAllowedAt2.IsNothing())
																{
																	flag = false;
																}
																if (flag && decoAllowedAt2.GetStreetOnly())
																{
																	flag = false;
																}
																if (flag && flag2 && decoAllowedAt2.GetSize() >= EnumDecoAllowedSize.NoBigOnlySmall)
																{
																	flag = false;
																}
																EnumDecoAllowedSlope slope2 = decoAllowedAt2.GetSlope();
																if (!biomePrefabDecoration.isDecorateOnSlopes)
																{
																	if (flag && slope2 >= EnumDecoAllowedSlope.Sloped)
																	{
																		flag = false;
																	}
																	if (flag && (int)chunk.GetHeight(x2, z) != num - 1)
																	{
																		flag = false;
																	}
																}
																else if (flag && slope2 >= EnumDecoAllowedSlope.Steep)
																{
																	flag = false;
																}
																if ((int)chunk.GetHeight(x2, z) + prefab.size.y >= 255)
																{
																	flag = false;
																}
																num4++;
															}
															num2++;
														}
														if (flag)
														{
															int num6 = _random.RandomRange(4);
															if (num6 != 0)
															{
																prefab = prefab.Clone(false);
																prefab.RotateY(true, num6);
															}
															Vector3i destinationPos = new Vector3i(blockWorldPosX, num + prefab.yOffset, blockWorldPosZ);
															prefab.CopyIntoLocal(_world.ChunkClusters[0], destinationPos, false, false, FastTags<TagGroup.Global>.none);
															prefab.SnapTerrainToArea(_world.ChunkClusters[0], destinationPos);
														}
													}
												}
											}
										}
									}
								}
							}
						}
						IL_3F1:;
					}
				}
			}
		}
	}

	// Token: 0x060055A7 RID: 21927 RVA: 0x0022F820 File Offset: 0x0022DA20
	[PublicizedFrom(EAccessModifier.Private)]
	public void decorateSingleBlocks(World _world, Chunk _chunk, Chunk cX1Z0, Chunk cX0Z1, Chunk cX1Z1, bool _chunkOverlapsTrader, GameRandom _random)
	{
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int blockWorldPosX = _chunk.GetBlockWorldPosX(j);
				int blockWorldPosZ = _chunk.GetBlockWorldPosZ(i);
				if (!_chunkOverlapsTrader || this.prefabDecorator.GetTraderAtPosition(new Vector3i(blockWorldPosX, 0, blockWorldPosZ), 0) == null)
				{
					int num = (int)(_chunk.GetTerrainHeight(j, i) + 1);
					if (num < 255)
					{
						Vector3i blockPos = new Vector3i(j, num, i);
						this.decorateSingleBlock(_world, _chunk, cX1Z0, cX0Z1, cX1Z1, _random, blockPos);
					}
				}
			}
		}
	}

	// Token: 0x060055A8 RID: 21928 RVA: 0x0022F8A4 File Offset: 0x0022DAA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void decorateSingleBlock(World _world, Chunk _chunk, Chunk cX1Z0, Chunk cX0Z1, Chunk cX1Z1, GameRandom _random, Vector3i blockPos)
	{
		if (!_chunk.GetBlockNoDamage(blockPos.x, blockPos.y, blockPos.z).isair)
		{
			return;
		}
		bool flag = _chunk.IsWater(blockPos.x, blockPos.y, blockPos.z);
		EnumDecoAllowed decoAllowedAt = _chunk.GetDecoAllowedAt(blockPos.x, blockPos.z);
		if (!flag && decoAllowedAt.IsNothing())
		{
			return;
		}
		if (decoAllowedAt.GetStreetOnly())
		{
			return;
		}
		BlockValue blockNoDamage = _chunk.GetBlockNoDamage(blockPos.x, blockPos.y + 1, blockPos.z);
		BlockValue blockNoDamage2 = _chunk.GetBlockNoDamage(blockPos.x, blockPos.y - 1, blockPos.z);
		if (!blockNoDamage.isair || blockNoDamage2.isair)
		{
			return;
		}
		if (flag && !_chunk.IsWater(blockPos.x, blockPos.y + 1, blockPos.z))
		{
			return;
		}
		int num = blockPos.x + blockPos.z * 16;
		BiomeDefinition biomeDefinition = this.chunkBiomes[num];
		Vector3i worldPos = _chunk.GetWorldPos();
		float terrainNormalY = _chunk.GetTerrainNormalY(blockPos.x, blockPos.z);
		for (int i = 0; i < biomeDefinition.m_DecoBlocks.Count; i++)
		{
			BiomeBlockDecoration deco = biomeDefinition.m_DecoBlocks[i];
			if (this.decorateSingleBlockTryPlaceDeco(_world, _chunk, cX1Z0, cX0Z1, cX1Z1, _random, blockPos, decoAllowedAt, biomeDefinition, deco, worldPos, terrainNormalY))
			{
				return;
			}
		}
	}

	// Token: 0x060055A9 RID: 21929 RVA: 0x0022FA18 File Offset: 0x0022DC18
	[PublicizedFrom(EAccessModifier.Private)]
	public bool decorateSingleBlockTryPlaceDeco(World _world, Chunk _chunk, Chunk cX1Z0, Chunk cX0Z1, Chunk cX1Z1, GameRandom _random, Vector3i blockPos, EnumDecoAllowed decoAllowed, BiomeDefinition bd, BiomeBlockDecoration deco, Vector3i chunkWorldPos, float normalY)
	{
		BlockValue blockValue = deco.blockValues[0];
		Block block = blockValue.Block;
		if (block.IsDistantDecoration && DecoManager.Instance.IsEnabled)
		{
			return false;
		}
		if (!block.CanDecorateOnSlopes)
		{
			if (decoAllowed.GetSlope() >= EnumDecoAllowedSlope.Sloped)
			{
				return false;
			}
		}
		else if (block.SlopeMaxCos > normalY)
		{
			return false;
		}
		if (block.IsPlant() && blockPos.y > 0 && _chunk.GetBlock(blockPos.x, blockPos.y - 1, blockPos.z).Block.blockMaterial.FertileLevel == 0)
		{
			return false;
		}
		if (block.isMultiBlock && block.multiBlockPos.dim.y + blockPos.y > 255)
		{
			return false;
		}
		if (_random.RandomFloat >= deco.prob)
		{
			return false;
		}
		if (deco.checkResourceOffsetY < 2147483647)
		{
			Vector3i vector3i = _chunk.ToWorldPos(blockPos);
			vector3i.y += deco.checkResourceOffsetY;
			if (!GameUtils.CheckOreNoiseAt(this.resourceNoise, vector3i.x, vector3i.y, vector3i.z))
			{
				return false;
			}
		}
		BlockValue blockValue2 = BlockPlaceholderMap.Instance.Replace(blockValue, _random, _chunk, chunkWorldPos.x + blockPos.x, 0, chunkWorldPos.z + blockPos.z, FastTags<TagGroup.Global>.none, false, true);
		if (blockValue2.isair)
		{
			return true;
		}
		if (deco.randomRotateMax > 0)
		{
			blockValue2.rotation = BiomeBlockDecoration.GetRandomRotation(_random.RandomFloat, deco.randomRotateMax);
		}
		Block block2 = blockValue2.Block;
		int decoRadius = DecoUtils.GetDecoRadius(blockValue2, block2);
		if (decoRadius > 0)
		{
			blockPos.x += decoRadius;
			blockPos.z += decoRadius;
			if (blockPos.x >= 16 || blockPos.z >= 16)
			{
				return false;
			}
			blockPos.y = (int)(_chunk.GetTerrainHeight(blockPos.x, blockPos.z) + 1);
			if (block.IsPlant() && _chunk.GetBlock(blockPos.x, blockPos.y - 1, blockPos.z).Block.blockMaterial.FertileLevel == 0)
			{
				return false;
			}
			if (!_chunk.GetBlock(blockPos).isair)
			{
				return false;
			}
		}
		if (!DecoUtils.CanPlaceDeco(_chunk, cX1Z0, cX0Z1, cX1Z1, chunkWorldPos + blockPos, blockValue2, (EnumDecoAllowed da) => !da.GetStreetOnly()))
		{
			return false;
		}
		DecoUtils.ApplyDecoAllowed(_chunk, cX1Z0, cX1Z0, cX1Z1, chunkWorldPos + blockPos, blockValue2);
		blockValue2 = block2.OnBlockPlaced(_world, 0, chunkWorldPos + blockPos, blockValue2, _random);
		_chunk.SetBlock(_world, blockPos.x, blockPos.y, blockPos.z, blockValue2, true, true, false, false, -1);
		if (!block2.shape.IsOmitTerrainSnappingUp && !block2.IsTerrainDecoration)
		{
			_world.ChunkCache.SnapTerrainToPositionAroundLocal(_chunk.ToWorldPos(blockPos) - Vector3i.up);
		}
		return true;
	}

	// Token: 0x04004235 RID: 16949
	[PublicizedFrom(EAccessModifier.Private)]
	public IBiomeProvider biomeProvider;

	// Token: 0x04004236 RID: 16950
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicPrefabDecorator prefabDecorator;

	// Token: 0x04004237 RID: 16951
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();

	// Token: 0x04004238 RID: 16952
	[PublicizedFrom(EAccessModifier.Private)]
	public PerlinNoise resourceNoise;

	// Token: 0x04004239 RID: 16953
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly BiomeDefinition[] chunkBiomes = new BiomeDefinition[256];

	// Token: 0x0400423A RID: 16954
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cBiomeIdMax = 256;

	// Token: 0x0400423B RID: 16955
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<Vector2i>[] biomePositions = new List<Vector2i>[256];

	// Token: 0x0400423C RID: 16956
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomePositionsHighId = -1;
}
