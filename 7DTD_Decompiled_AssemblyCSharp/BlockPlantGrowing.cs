using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200011C RID: 284
[Preserve]
public class BlockPlantGrowing : BlockPlant
{
	// Token: 0x060007C3 RID: 1987 RVA: 0x00036813 File Offset: 0x00034A13
	public BlockPlantGrowing()
	{
		this.fertileLevel = 5;
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x00036844 File Offset: 0x00034A44
	public override void LateInit()
	{
		base.LateInit();
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingNextPlant))
		{
			this.nextPlant = ItemClass.GetItem(base.Properties.Values[BlockPlantGrowing.PropGrowingNextPlant], false).ToBlockValue();
			if (this.nextPlant.Equals(BlockValue.Air))
			{
				throw new Exception("Block with name '" + base.Properties.Values[BlockPlantGrowing.PropGrowingNextPlant] + "' not found!");
			}
		}
		this.growOnTop = BlockValue.Air;
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingIsGrowOnTopEnabled) && StringParsers.ParseBool(base.Properties.Values[BlockPlantGrowing.PropGrowingIsGrowOnTopEnabled], 0, -1, true))
		{
			this.bGrowOnTopEnabled = true;
			if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingGrowOnTop))
			{
				this.growOnTop = ItemClass.GetItem(base.Properties.Values[BlockPlantGrowing.PropGrowingGrowOnTop], false).ToBlockValue();
				if (this.growOnTop.Equals(BlockValue.Air))
				{
					throw new Exception("Block with name '" + base.Properties.Values[BlockPlantGrowing.PropGrowingGrowOnTop] + "' not found!");
				}
			}
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingGrowthRate))
		{
			this.growthRate = StringParsers.ParseFloat(base.Properties.Values[BlockPlantGrowing.PropGrowingGrowthRate], 0, -1, NumberStyles.Any);
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingGrowthDeviation))
		{
			this.growthDeviation = StringParsers.ParseFloat(base.Properties.Values[BlockPlantGrowing.PropGrowingGrowthDeviation], 0, -1, NumberStyles.Any);
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingFertileLevel))
		{
			this.fertileLevel = int.Parse(base.Properties.Values[BlockPlantGrowing.PropGrowingFertileLevel]);
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingLightLevelStay))
		{
			this.lightLevelStay = int.Parse(base.Properties.Values[BlockPlantGrowing.PropGrowingLightLevelStay]);
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingLightLevelGrow))
		{
			this.lightLevelGrow = int.Parse(base.Properties.Values[BlockPlantGrowing.PropGrowingLightLevelGrow]);
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingGrowIfAnythinOnTop))
		{
			this.isPlantGrowingIfAnythingOnTop = StringParsers.ParseBool(base.Properties.Values[BlockPlantGrowing.PropGrowingGrowIfAnythinOnTop], 0, -1, true);
		}
		if (base.Properties.Values.ContainsKey(BlockPlantGrowing.PropGrowingIsRandom))
		{
			this.isPlantGrowingRandom = StringParsers.ParseBool(base.Properties.Values[BlockPlantGrowing.PropGrowingIsRandom], 0, -1, true);
		}
		if (this.growthRate > 0f)
		{
			this.BlockTag = BlockTags.GrowablePlant;
			this.IsRandomlyTick = true;
			return;
		}
		this.IsRandomlyTick = false;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00036B50 File Offset: 0x00034D50
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (GameManager.Instance.IsEditMode())
		{
			return true;
		}
		if (!base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		Vector3i blockPos = _blockPos + Vector3i.up;
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			byte light = chunkCluster.GetLight(blockPos, Chunk.LIGHT_TYPE.SUN);
			if ((int)light < this.lightLevelStay || (int)light < this.lightLevelGrow)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00036BB8 File Offset: 0x00034DB8
	public override bool CanGrowOn(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValueOfPlant)
	{
		return this.fertileLevel == 0 || _world.GetBlock(_clrIdx, _blockPos).Block.blockMaterial.FertileLevel >= this.fertileLevel;
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x00036BF4 File Offset: 0x00034DF4
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		if (_ea is EntityPlayerLocal)
		{
			_ea.Progression.AddLevelExp((int)_result.blockValue.Block.blockMaterial.Experience, "_xpOther", Progression.XPTypes.Other, true, true);
		}
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00036C34 File Offset: 0x00034E34
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (this.nextPlant.isair)
		{
			return;
		}
		if (_blockValue.ischild)
		{
			Log.Warning("BlockPlantGrowing OnBlockAdded child at {0}, {1}", new object[]
			{
				_blockPos,
				_blockValue
			});
			return;
		}
		if (!_world.IsRemote())
		{
			this.addScheduledTick(_world, _chunk.ClrIdx, _blockPos);
		}
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00036CA0 File Offset: 0x00034EA0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void addScheduledTick(WorldBase _world, int _clrIdx, Vector3i _blockPos)
	{
		if (!this.isPlantGrowingRandom)
		{
			_world.GetWBT().AddScheduledBlockUpdate(_clrIdx, _blockPos, this.blockID, this.GetTickRate());
			return;
		}
		int num = (int)this.GetTickRate();
		int num2 = (int)((float)num * this.growthDeviation);
		int num3 = num / 2;
		int max = num + num3;
		GameRandom gameRandom = _world.GetGameRandom();
		int num4;
		int num5;
		do
		{
			float randomGaussian = gameRandom.RandomGaussian;
			num4 = Mathf.RoundToInt((float)num + (float)num2 * randomGaussian);
			num5 = Utils.FastClamp(num4, num3, max);
		}
		while (num5 != num4);
		_world.GetWBT().AddScheduledBlockUpdate(_clrIdx, _blockPos, this.blockID, (ulong)((long)num5));
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00036D31 File Offset: 0x00034F31
	public override ulong GetTickRate()
	{
		return (ulong)(this.growthRate * 20f * 60f);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x00036D48 File Offset: 0x00034F48
	public override bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		if (this.nextPlant.isair)
		{
			return false;
		}
		if (!this.CheckPlantAlive(_world, _clrIdx, _blockPos, _blockValue))
		{
			return true;
		}
		if (_bRandomTick)
		{
			this.addScheduledTick(_world, _clrIdx, _blockPos);
			return true;
		}
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return true;
		}
		Vector3i blockPos = _blockPos + Vector3i.up;
		if ((int)chunkCluster.GetLight(blockPos, Chunk.LIGHT_TYPE.SUN) < this.lightLevelGrow)
		{
			this.addScheduledTick(_world, _clrIdx, _blockPos);
			return true;
		}
		BlockValue block = _world.GetBlock(_clrIdx, _blockPos + Vector3i.up);
		if (!this.isPlantGrowingIfAnythingOnTop && !block.isair)
		{
			return true;
		}
		BlockPlant blockPlant = this.nextPlant.Block as BlockPlant;
		if (blockPlant != null && !blockPlant.CanGrowOn(_world, _clrIdx, _blockPos + Vector3i.down, this.nextPlant))
		{
			return true;
		}
		_blockValue.type = this.nextPlant.type;
		BiomeDefinition biome = ((World)_world).GetBiome(_blockPos.x, _blockPos.z);
		if (biome != null && biome.Replacements.ContainsKey(_blockValue.type))
		{
			_blockValue.type = biome.Replacements[_blockValue.type];
		}
		BlockValue blockValue = BlockPlaceholderMap.Instance.Replace(_blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
		blockValue.rotation = _blockValue.rotation;
		blockValue.meta = _blockValue.meta;
		blockValue.meta2 = 0;
		_blockValue = blockValue;
		if (this.bGrowOnTopEnabled)
		{
			_blockValue.meta = (_blockValue.meta + 1 & 15);
		}
		if (this.isPlantGrowingRandom || _ticksIfLoaded <= this.GetTickRate() || !_blockValue.Block.UpdateTick(_world, _clrIdx, _blockPos, _blockValue, false, _ticksIfLoaded - this.GetTickRate(), _rnd))
		{
			_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
		}
		if (!this.growOnTop.isair && _blockPos.y + 1 < 255 && block.isair)
		{
			_blockValue.type = this.growOnTop.type;
			_blockValue = _blockValue.Block.OnBlockPlaced(_world, _clrIdx, _blockPos, _blockValue, _rnd);
			Block block2 = _blockValue.Block;
			if (_blockValue.damage >= block2.blockMaterial.MaxDamage)
			{
				_blockValue.damage = block2.blockMaterial.MaxDamage - 1;
			}
			if (this.isPlantGrowingRandom || _ticksIfLoaded <= this.GetTickRate() || !block2.UpdateTick(_world, _clrIdx, _blockPos + Vector3i.up, _blockValue, false, _ticksIfLoaded - this.GetTickRate(), _rnd))
			{
				_world.SetBlockRPC(_clrIdx, _blockPos + Vector3i.up, _blockValue);
			}
		}
		return true;
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00036FE4 File Offset: 0x000351E4
	public BlockValue ForceNextGrowStage(World _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockValue block = _world.GetBlock(_clrIdx, _blockPos + Vector3i.up);
		if (!this.isPlantGrowingIfAnythingOnTop && !block.isair)
		{
			return _blockValue;
		}
		_blockValue.type = this.nextPlant.type;
		BiomeDefinition biome = _world.GetBiome(_blockPos.x, _blockPos.z);
		if (biome != null && biome.Replacements.ContainsKey(_blockValue.type))
		{
			_blockValue.type = biome.Replacements[_blockValue.type];
		}
		BlockValue blockValue = BlockPlaceholderMap.Instance.Replace(_blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
		blockValue.rotation = _blockValue.rotation;
		blockValue.meta = _blockValue.meta;
		blockValue.meta2 = 0;
		_blockValue = blockValue;
		if (this.bGrowOnTopEnabled)
		{
			_blockValue.meta = (_blockValue.meta + 1 & 15);
		}
		if (!this.growOnTop.isair && _blockPos.y + 1 < 255 && block.isair)
		{
			_blockValue.type = this.growOnTop.type;
			_blockValue = _blockValue.Block.OnBlockPlaced(_world, _clrIdx, _blockPos, _blockValue, _world.GetGameRandom());
			Block block2 = _blockValue.Block;
			if (_blockValue.damage >= block2.blockMaterial.MaxDamage)
			{
				_blockValue.damage = block2.blockMaterial.MaxDamage - 1;
			}
			_world.SetBlockRPC(_clrIdx, _blockPos + Vector3i.up, _blockValue);
		}
		return _blockValue;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00037169 File Offset: 0x00035369
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		if (!_world.IsRemote())
		{
			_world.GetWBT().InvalidateScheduledBlockUpdate(_chunk.ClrIdx, _blockPos, this.blockID);
		}
	}

	// Token: 0x04000836 RID: 2102
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingNextPlant = "PlantGrowing.Next";

	// Token: 0x04000837 RID: 2103
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingGrowthRate = "PlantGrowing.GrowthRate";

	// Token: 0x04000838 RID: 2104
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingGrowthDeviation = "PlantGrowing.GrowthDeviation";

	// Token: 0x04000839 RID: 2105
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingFertileLevel = "PlantGrowing.FertileLevel";

	// Token: 0x0400083A RID: 2106
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingGrowOnTop = "PlantGrowing.GrowOnTop";

	// Token: 0x0400083B RID: 2107
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingIsGrowOnTopEnabled = "PlantGrowing.IsGrowOnTopEnabled";

	// Token: 0x0400083C RID: 2108
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingLightLevelStay = "PlantGrowing.LightLevelStay";

	// Token: 0x0400083D RID: 2109
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingLightLevelGrow = "PlantGrowing.LightLevelGrow";

	// Token: 0x0400083E RID: 2110
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingIsRandom = "PlantGrowing.IsRandom";

	// Token: 0x0400083F RID: 2111
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingGrowIfAnythinOnTop = "PlantGrowing.GrowIfAnythinOnTop";

	// Token: 0x04000840 RID: 2112
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue nextPlant;

	// Token: 0x04000841 RID: 2113
	[PublicizedFrom(EAccessModifier.Protected)]
	public BlockValue growOnTop;

	// Token: 0x04000842 RID: 2114
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bGrowOnTopEnabled;

	// Token: 0x04000843 RID: 2115
	[PublicizedFrom(EAccessModifier.Protected)]
	public float growthRate;

	// Token: 0x04000844 RID: 2116
	[PublicizedFrom(EAccessModifier.Protected)]
	public float growthDeviation = 0.25f;

	// Token: 0x04000845 RID: 2117
	[PublicizedFrom(EAccessModifier.Protected)]
	public int lightLevelGrow = 8;

	// Token: 0x04000846 RID: 2118
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isPlantGrowingRandom = true;

	// Token: 0x04000847 RID: 2119
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isPlantGrowingIfAnythingOnTop = true;
}
