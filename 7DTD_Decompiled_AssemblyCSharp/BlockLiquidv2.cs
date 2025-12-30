using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200010E RID: 270
[Preserve]
public class BlockLiquidv2 : Block
{
	// Token: 0x06000744 RID: 1860 RVA: 0x00033379 File Offset: 0x00031579
	public BlockLiquidv2()
	{
		this.IsRandomlyTick = false;
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0002C313 File Offset: 0x0002A513
	public override void LateInit()
	{
		base.LateInit();
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00033388 File Offset: 0x00031588
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		_damagePoints = 0;
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0002A581 File Offset: 0x00028781
	public override bool IsHealthShownInUI(BlockValue _bv)
	{
		return _bv.damage > 0;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x000333B0 File Offset: 0x000315B0
	public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		base.OnNeighborBlockChange(world, _clrIdx, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
		if (!_newNeighborBlockValue.isair)
		{
			if (_newNeighborBlockValue.type != this.blockID || _oldNeighborBlockValue.type != this.blockID || BlockLiquidv2.Evap(_newNeighborBlockValue) == BlockLiquidv2.Evap(_oldNeighborBlockValue))
			{
				if (_myBlockPos.y == _blockPosThatChanged.y && (_myBlockPos.x - _blockPosThatChanged.x > 0 || _myBlockPos.x - _blockPosThatChanged.x < 0 || _myBlockPos.z - _blockPosThatChanged.z < 0 || _myBlockPos.z - _blockPosThatChanged.z > 0))
				{
					this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), BlockLiquidv2.Evap(_myBlockValue), BlockLiquidv2.FlowDirection.None);
					return;
				}
				this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), BlockLiquidv2.Evap(_myBlockValue), BlockLiquidv2.FlowDirection.None);
			}
			return;
		}
		if (_myBlockPos.x - _blockPosThatChanged.x > 0)
		{
			this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), 0, BlockLiquidv2.FlowDirection.West);
			return;
		}
		if (_myBlockPos.x - _blockPosThatChanged.x < 0)
		{
			this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), 0, BlockLiquidv2.FlowDirection.East);
			return;
		}
		if (_myBlockPos.z - _blockPosThatChanged.z < 0)
		{
			this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), 0, BlockLiquidv2.FlowDirection.North);
			return;
		}
		if (_myBlockPos.z - _blockPosThatChanged.z > 0)
		{
			this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), 0, BlockLiquidv2.FlowDirection.South);
			return;
		}
		this.ChangeThis(world, _clrIdx, _myBlockValue, _myBlockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_myBlockValue), BlockLiquidv2.Evap(_myBlockValue), BlockLiquidv2.FlowDirection.None);
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMovementBlocked(IBlockAccess world, Vector3i _blockPos, BlockValue _blockValue, BlockFace crossingFace)
	{
		return false;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0003354D File Offset: 0x0003174D
	public override ulong GetTickRate()
	{
		return 1UL;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00033551 File Offset: 0x00031751
	public static void UpdateTime()
	{
		BlockLiquidv2.currentTime = Time.time;
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00033560 File Offset: 0x00031760
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckUpdate()
	{
		if (BlockLiquidv2.currentTime > BlockLiquidv2.blockUpdateTimer + 0.5f)
		{
			BlockLiquidv2.blockUpdates = 0;
			BlockLiquidv2.blockUpdateTimer = BlockLiquidv2.currentTime;
		}
		BlockLiquidv2.blockUpdates++;
		return BlockLiquidv2.blockUpdates <= BlockLiquidv2.blockUpdatesPerSecond / 2;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x000335AC File Offset: 0x000317AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeThis(WorldBase _world, int _clusterIndex, BlockValue _blockValue, Vector3i _blockPos, BlockLiquidv2.UpdateID _blockState)
	{
		this.ChangeThis(_world, _clusterIndex, _blockValue, _blockPos, _blockState, BlockLiquidv2.Emissions(_blockValue), BlockLiquidv2.Evap(_blockValue), BlockLiquidv2.Flow(_blockValue));
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x000335D8 File Offset: 0x000317D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeThis(WorldBase _world, int _clusterIndex, BlockValue _blockValue, Vector3i _blockPos, BlockLiquidv2.UpdateID _blockState, int _emissions, int _evaporation)
	{
		this.ChangeThis(_world, _clusterIndex, _blockValue, _blockPos, _blockState, _emissions, 0, BlockLiquidv2.Flow(_blockValue));
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x000335FC File Offset: 0x000317FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeThis(WorldBase _world, int _clusterIndex, BlockValue _blockValue, Vector3i _blockPos, BlockLiquidv2.UpdateID _blockState, int _emissions, int _evaporation, BlockLiquidv2.FlowDirection _flowDirection)
	{
		if (_blockValue.rotation != 8)
		{
			_emissions = BlockLiquidv2.MAX_EMISSIONS;
		}
		_emissions = Mathf.Clamp(_emissions, 0, BlockLiquidv2.MAX_EMISSIONS);
		BlockValue air = BlockValue.Air;
		air.rawData = (uint)this.blockID;
		BlockLiquidv2.SetBlockState(ref air, _blockState);
		air.meta2 = (byte)_emissions;
		air.rotation = 8;
		air.damage = (int)(_evaporation + ((_flowDirection == BlockLiquidv2.FlowDirection.None) ? BlockLiquidv2.FlowDirection.None : ((BlockLiquidv2.FlowDirection)50 + (int)_flowDirection)));
		_world.SetBlockRPC(_clusterIndex, _blockPos, air);
		ulong ticks = 1000UL;
		if (_blockState != BlockLiquidv2.UpdateID.Sleep)
		{
			if (_blockState == BlockLiquidv2.UpdateID.Awake)
			{
				ticks = 1UL;
			}
		}
		else
		{
			ticks = 60UL;
		}
		if (_world.GetWBT() != null)
		{
			_world.GetWBT().AddScheduledBlockUpdate(_clusterIndex, _blockPos, this.blockID, ticks);
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x000336B0 File Offset: 0x000318B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeToAir(WorldBase _world, int _clusterIndex, Vector3i _blockPos)
	{
		BlockValue air = BlockValue.Air;
		WaterSplashCubes.RemoveSplashAt(_blockPos.x, _blockPos.y, _blockPos.z);
		air.rawData = (uint)BlockValue.Air.type;
		air.damage = 0;
		_world.SetBlockRPC(_clusterIndex, _blockPos, air);
		if (_world.GetWBT() != null)
		{
			_world.GetWBT().AddScheduledBlockUpdate(_clusterIndex, _blockPos, this.blockID, this.GetTickRate());
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0003371D File Offset: 0x0003191D
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockLiquidv2.UpdateID State(BlockValue _blockValue)
	{
		return (BlockLiquidv2.UpdateID)_blockValue.meta;
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00033726 File Offset: 0x00031926
	public static int Emissions(BlockValue _blockValue)
	{
		if (_blockValue.rotation != 8)
		{
			return BlockLiquidv2.MAX_EMISSIONS;
		}
		return (int)_blockValue.meta2;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0003373F File Offset: 0x0003193F
	public static int Evap(BlockValue _blockValue)
	{
		if (_blockValue.damage <= 45)
		{
			return _blockValue.damage;
		}
		return 0;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00033753 File Offset: 0x00031953
	[PublicizedFrom(EAccessModifier.Private)]
	public void IncEvap(ref BlockValue _blockValue)
	{
		if (_blockValue.damage > 45)
		{
			_blockValue.damage = 0;
		}
		_blockValue.damage++;
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00033771 File Offset: 0x00031971
	public static BlockLiquidv2.FlowDirection Flow(BlockValue _blockValue)
	{
		if (_blockValue.damage > 50)
		{
			return (BlockLiquidv2.FlowDirection)(_blockValue.damage - 50);
		}
		return BlockLiquidv2.FlowDirection.None;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00033788 File Offset: 0x00031988
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i GetFlowDirection(BlockLiquidv2.FlowDirection _flowDirection)
	{
		Vector3i zero = Vector3i.zero;
		switch (_flowDirection)
		{
		case BlockLiquidv2.FlowDirection.North:
			zero.z = 1;
			break;
		case BlockLiquidv2.FlowDirection.East:
			zero.x = 1;
			break;
		case BlockLiquidv2.FlowDirection.South:
			zero.z = -1;
			break;
		case BlockLiquidv2.FlowDirection.West:
			zero.x = -1;
			break;
		}
		return zero;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x000337DC File Offset: 0x000319DC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsAir(BlockValue _blockValue)
	{
		return _blockValue.isair;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x000337E8 File Offset: 0x000319E8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsPlant(BlockValue _blockValue)
	{
		Block block = _blockValue.Block;
		if (block == null)
		{
			Log.Error("BlockLiquidv2::IsPlant() - Couldn't find block with type [" + _blockValue.type.ToString() + "].  Block is null.");
			return false;
		}
		return block.blockMaterial.IsPlant;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00033830 File Offset: 0x00031A30
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasHoles(BlockValue _blockValue)
	{
		Block block = null;
		try
		{
			block = _blockValue.Block;
		}
		catch (Exception ex)
		{
			Log.Error("BlockLiquidv2::HasHoles() - Couldn't find block with type [" + _blockValue.type.ToString() + "].  Exception is: " + ex.ToString());
			return false;
		}
		if (block == null)
		{
			Log.Error("BlockLiquidv2::HasHoles() - Couldn't find block with type [" + _blockValue.type.ToString() + "].  Block is null.");
			return false;
		}
		try
		{
			bool flag = this.IsWater(_blockValue);
			int facesDrawnFullBitfield = block.shape.getFacesDrawnFullBitfield(_blockValue);
			bool flag2 = facesDrawnFullBitfield == 255 || facesDrawnFullBitfield == 63;
			return block.blockMaterial.IsPlant || (!block.shape.IsSolidCube && !flag && !flag2);
		}
		catch (Exception ex2)
		{
			Log.Error("BlockLiquidv2::HasHoles() - BlockValue type is [" + _blockValue.type.ToString() + "].  Exception is: " + ex2.ToString());
		}
		return false;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00033948 File Offset: 0x00031B48
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsWater(BlockValue _blockValue)
	{
		return _blockValue.type == this.blockID;
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00033959 File Offset: 0x00031B59
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockLiquidv2.FlowDirection GetFlowDirection(Vector3i _dir)
	{
		if (_dir.x == -1)
		{
			return BlockLiquidv2.FlowDirection.West;
		}
		if (_dir.x == 1)
		{
			return BlockLiquidv2.FlowDirection.East;
		}
		if (_dir.z == 1)
		{
			return BlockLiquidv2.FlowDirection.North;
		}
		if (_dir.z == -1)
		{
			return BlockLiquidv2.FlowDirection.South;
		}
		return BlockLiquidv2.FlowDirection.None;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00033988 File Offset: 0x00031B88
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue GetBlock(WorldBase _world, Vector3i _blockPos)
	{
		BlockValue block = _world.GetBlock(_blockPos.x, _blockPos.y, _blockPos.z);
		if (block.rotation != 8)
		{
			block.meta2 = (byte)BlockLiquidv2.MAX_EMISSIONS;
		}
		return block;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000339C8 File Offset: 0x00031BC8
	public override void DoExchangeAction(WorldBase _world, int _clusterIndex, Vector3i _blockPos, BlockValue _blockValue, string _action, int _itemCount)
	{
		if (_blockValue.rotation != 8)
		{
			return;
		}
		if (string.IsNullOrEmpty(_action))
		{
			return;
		}
		if (_world == null)
		{
			return;
		}
		if (_action.Contains("deplete"))
		{
			int num = _action.LastIndexOf("deplete");
			if (num >= 0)
			{
				num += "deplete".Length;
				string text = _action.Substring(num);
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				try
				{
					int num2 = int.Parse(text);
					num2 *= _itemCount;
					if (num2 > 0)
					{
						if (num2 > BlockLiquidv2.Emissions(_blockValue))
						{
							int num3 = Mathf.Clamp(num2 - BlockLiquidv2.Emissions(_blockValue) - 1, 0, BlockLiquidv2.MAX_EMISSIONS);
							if (num3 > 0)
							{
								Vector3i blockPos = Vector3i.zero;
								int num4 = 0;
								while (num4 < 4 && num3 > 0)
								{
									blockPos = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num4];
									BlockValue block = this.GetBlock(_world, blockPos);
									if (this.IsWater(block))
									{
										int num5 = BlockLiquidv2.Emissions(block);
										if (num3 <= num5)
										{
											if (num5 == 0)
											{
												this.ChangeToAir(_world, _clusterIndex, blockPos);
												num3--;
												break;
											}
											this.ChangeThis(_world, _clusterIndex, block, blockPos, BlockLiquidv2.UpdateID.Awake, Mathf.Clamp(BlockLiquidv2.Emissions(block) - num3, 0, BlockLiquidv2.MAX_EMISSIONS), BlockLiquidv2.Evap(block), BlockLiquidv2.Flow(block));
											break;
										}
										else
										{
											this.ChangeToAir(_world, _clusterIndex, blockPos);
											num3 -= BlockLiquidv2.Emissions(block);
										}
									}
									num4++;
								}
							}
							this.ChangeToAir(_world, _clusterIndex, _blockPos);
						}
						else
						{
							this.ChangeThis(_world, _clusterIndex, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, Mathf.Clamp(BlockLiquidv2.Emissions(_blockValue) - num2, 0, BlockLiquidv2.MAX_EMISSIONS), BlockLiquidv2.Evap(_blockValue), BlockLiquidv2.Flow(_blockValue));
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error("BlockLiquidv2::DoExchangeAction() - " + ex.ToString());
				}
			}
		}
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00033B98 File Offset: 0x00031D98
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckDeepWater_Expensive(WorldBase world, Vector3i _blockPos)
	{
		int num = 0;
		Vector3i blockPos;
		blockPos.x = _blockPos.x;
		blockPos.y = _blockPos.y + 1;
		blockPos.z = _blockPos.z;
		BlockValue block = this.GetBlock(world, blockPos);
		while (this.IsWater(block) && num <= 6)
		{
			num++;
			blockPos.y++;
			block = this.GetBlock(world, blockPos);
		}
		return num >= 6;
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00033C08 File Offset: 0x00031E08
	public override bool UpdateTick(WorldBase world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		if (this.State(_blockValue) == BlockLiquidv2.UpdateID.Sleep)
		{
			return true;
		}
		BlockLiquidv2.fallSet++;
		if (BlockLiquidv2.fallSet >= 8)
		{
			BlockLiquidv2.fallSet = 0;
		}
		BlockValue blockValue = BlockValue.Air;
		BlockValue blockValue2 = BlockValue.Air;
		Vector3i vector3i = Vector3i.zero;
		Vector3i zero = Vector3i.zero;
		zero.x = _blockPos.x;
		zero.y = _blockPos.y - 1;
		zero.z = _blockPos.z;
		blockValue = this.GetBlock(world, zero);
		if (this.HasHoles(blockValue))
		{
			zero.x = _blockPos.x;
			zero.y = _blockPos.y - 2;
			zero.z = _blockPos.z;
			blockValue = this.GetBlock(world, zero);
			if (this.IsAir(blockValue) || this.IsPlant(blockValue))
			{
				if (BlockLiquidv2.Emissions(_blockValue) > 0 && this.CheckUpdate())
				{
					if (!this.CheckDeepWater_Expensive(world, _blockPos))
					{
						this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Sleep, 0, 0, BlockLiquidv2.FlowDirection.None);
					}
					this.ChangeThis(world, _clrIdx, _blockValue, zero, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, BlockLiquidv2.FlowDirection.None);
				}
				else
				{
					this.ChangeToAir(world, _clrIdx, _blockPos);
					this.ChangeThis(world, _clrIdx, _blockValue, zero, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue), 0, BlockLiquidv2.FlowDirection.None);
				}
				return true;
			}
			zero.x = _blockPos.x;
			zero.y = _blockPos.y - 1;
			zero.z = _blockPos.z;
			blockValue = this.GetBlock(world, zero);
		}
		if (this.IsAir(blockValue) || this.IsPlant(blockValue))
		{
			if (BlockLiquidv2.Emissions(_blockValue) > 0 && this.CheckUpdate())
			{
				if (!this.CheckDeepWater_Expensive(world, _blockPos))
				{
					this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Sleep, 0, 0, BlockLiquidv2.FlowDirection.None);
				}
				this.ChangeThis(world, _clrIdx, _blockValue, zero, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, BlockLiquidv2.FlowDirection.None);
			}
			else
			{
				this.ChangeToAir(world, _clrIdx, _blockPos);
				this.ChangeThis(world, _clrIdx, _blockValue, zero, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue), 0, BlockLiquidv2.FlowDirection.None);
			}
			return true;
		}
		if (!this.CheckUpdate())
		{
			return true;
		}
		BlockValue[] array = new BlockValue[]
		{
			BlockValue.Air,
			BlockValue.Air,
			BlockValue.Air,
			BlockValue.Air
		};
		BlockValue[] array2 = new BlockValue[]
		{
			BlockValue.Air,
			BlockValue.Air,
			BlockValue.Air,
			BlockValue.Air
		};
		bool[] array3 = new bool[4];
		bool[] array4 = new bool[4];
		for (int i = 0; i < 4; i++)
		{
			array3[i] = false;
		}
		for (int j = 0; j < 4; j++)
		{
			array4[j] = false;
		}
		for (int k = 0; k < 4; k++)
		{
			vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, k];
			blockValue2 = world.GetBlock(vector3i.x, vector3i.y, vector3i.z);
			array[k] = blockValue2;
			array3[k] = true;
			if (this.IsAir(blockValue2) || this.HasHoles(blockValue2))
			{
				Vector3i blockPos;
				blockPos.x = vector3i.x;
				blockPos.y = vector3i.y - 1;
				blockPos.z = vector3i.z;
				blockValue2 = this.GetBlock(world, blockPos);
				array2[k] = blockValue2;
				array4[k] = true;
				if (this.IsAir(blockValue2) || this.IsPlant(blockValue2))
				{
					if (BlockLiquidv2.Emissions(_blockValue) > 0)
					{
						if (!this.CheckDeepWater_Expensive(world, _blockPos))
						{
							this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Sleep, 0, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, k]));
						}
						this.ChangeThis(world, _clrIdx, _blockValue, vector3i, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, k]));
					}
					else
					{
						this.ChangeToAir(world, _clrIdx, _blockPos);
						this.ChangeThis(world, _clrIdx, _blockValue, vector3i, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue), 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, k]));
					}
					return true;
				}
			}
		}
		if (BlockLiquidv2.Emissions(_blockValue) > 0)
		{
			if (this.IsWater(blockValue) && BlockLiquidv2.Emissions(blockValue) < BlockLiquidv2.MAX_EMISSIONS)
			{
				if (!this.CheckDeepWater_Expensive(world, _blockPos))
				{
					this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, BlockLiquidv2.FlowDirection.None);
				}
				this.ChangeThis(world, _clrIdx, blockValue, zero, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue) + 1, 0);
				return true;
			}
			for (int l = 0; l < 4; l++)
			{
				vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, l];
				if (array3[l])
				{
					blockValue2 = array[l];
				}
				else
				{
					blockValue2 = this.GetBlock(world, vector3i);
					array[l] = blockValue2;
					array3[l] = true;
				}
				if (this.IsAir(blockValue2) || this.IsPlant(blockValue2))
				{
					if (!this.CheckDeepWater_Expensive(world, _blockPos))
					{
						this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, 0, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, l]));
					}
					this.ChangeThis(world, _clrIdx, _blockValue, vector3i, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, l]));
					return true;
				}
				if (this.IsWater(blockValue2) && BlockLiquidv2.Emissions(_blockValue) - BlockLiquidv2.Emissions(blockValue2) - 1 > 0)
				{
					if (!this.CheckDeepWater_Expensive(world, _blockPos))
					{
						this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, BlockLiquidv2.FlowDirection.None);
					}
					this.ChangeThis(world, _clrIdx, blockValue2, vector3i, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue2) + 1, 0);
					return true;
				}
			}
		}
		if (this.IsWater(blockValue) && BlockLiquidv2.Emissions(blockValue) < BlockLiquidv2.MAX_EMISSIONS)
		{
			this.ChangeToAir(world, _clrIdx, _blockPos);
			this.ChangeThis(world, _clrIdx, blockValue, zero, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue) + 1, 0);
			return true;
		}
		for (int m = 0; m < 4; m++)
		{
			vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, m];
			if (array4[m])
			{
				blockValue2 = array2[m];
			}
			else
			{
				Vector3i blockPos2;
				blockPos2.x = vector3i.x;
				blockPos2.y = vector3i.y - 1;
				blockPos2.z = vector3i.z;
				blockValue2 = this.GetBlock(world, blockPos2);
				array2[m] = blockValue2;
				array4[m] = true;
			}
			if (this.IsWater(blockValue2) && BlockLiquidv2.Emissions(blockValue2) < BlockLiquidv2.MAX_EMISSIONS)
			{
				this.ChangeToAir(world, _clrIdx, _blockPos);
				this.ChangeThis(world, _clrIdx, blockValue2, vector3i + Vector3i.down, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue2) + 1, 0);
				return true;
			}
		}
		if (BlockLiquidv2.Emissions(_blockValue) < BlockLiquidv2.MAX_EMISSIONS)
		{
			Vector3i blockPos3;
			blockPos3.x = _blockPos.x;
			blockPos3.y = _blockPos.y + 1;
			blockPos3.z = _blockPos.z;
			blockValue2 = this.GetBlock(world, blockPos3);
			if (this.IsWater(blockValue2))
			{
				if (BlockLiquidv2.Emissions(blockValue2) == 0)
				{
					this.ChangeToAir(world, _clrIdx, _blockPos + Vector3i.up);
					this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) + 1, 0);
					return true;
				}
				this.ChangeThis(world, _clrIdx, blockValue2, _blockPos + Vector3i.up, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue2) - 1, 0);
				this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) + 1, 0);
				return true;
			}
		}
		bool flag = false;
		Vector3i blockPos4 = Vector3i.zero;
		BlockValue air = BlockValue.Air;
		BlockLiquidv2.FlowDirection flowDirection = BlockLiquidv2.FlowDirection.None;
		for (int n = 0; n < 4; n++)
		{
			vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, n];
			if (array3[n])
			{
				blockValue2 = array[n];
			}
			else
			{
				blockValue2 = this.GetBlock(world, vector3i);
				array[n] = blockValue2;
				array3[n] = true;
			}
			if (this.IsAir(blockValue2) || this.IsPlant(blockValue2))
			{
				flag = true;
				blockPos4 = vector3i;
				flowDirection = this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, n]);
				break;
			}
		}
		if (BlockLiquidv2.Emissions(_blockValue) < BlockLiquidv2.MAX_EMISSIONS - 1)
		{
			for (int num = 0; num < 4; num++)
			{
				vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num];
				if (array3[num])
				{
					blockValue2 = array[num];
				}
				else
				{
					blockValue2 = this.GetBlock(world, vector3i);
					array[num] = blockValue2;
					array3[num] = true;
				}
				if (this.IsWater(blockValue2) && BlockLiquidv2.Emissions(blockValue2) >= 2 - (flag ? 1 : 0) + BlockLiquidv2.Emissions(_blockValue))
				{
					this.ChangeThis(world, _clrIdx, blockValue2, vector3i, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue2) - 1, 0);
					if (flag)
					{
						this.ChangeThis(world, _clrIdx, _blockValue, blockPos4, BlockLiquidv2.UpdateID.Awake, 0, 0, flowDirection);
					}
					else
					{
						this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) + 1, 0);
					}
					return true;
				}
			}
		}
		if (BlockLiquidv2.Emissions(_blockValue) > 0)
		{
			for (int num2 = 0; num2 < 4; num2++)
			{
				vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num2];
				if (array3[num2])
				{
					blockValue2 = array[num2];
				}
				else
				{
					blockValue2 = this.GetBlock(world, vector3i);
					array[num2] = blockValue2;
					array3[num2] = true;
				}
				if (this.HasHoles(blockValue2))
				{
					vector3i = _blockPos + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num2] + BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num2];
					blockValue2 = this.GetBlock(world, vector3i);
					if (this.IsAir(blockValue2) || this.IsPlant(blockValue2))
					{
						this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num2]));
						this.ChangeThis(world, _clrIdx, blockValue2, vector3i, BlockLiquidv2.UpdateID.Awake, 0, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num2]));
						return true;
					}
					if (this.IsWater(blockValue2) && BlockLiquidv2.Emissions(_blockValue) > BlockLiquidv2.Emissions(blockValue2) + 1)
					{
						this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(_blockValue) - 1, 0, this.GetFlowDirection(BlockLiquidv2.fallDirsSet[BlockLiquidv2.fallSet, num2]));
						this.ChangeThis(world, _clrIdx, blockValue2, vector3i, BlockLiquidv2.UpdateID.Awake, BlockLiquidv2.Emissions(blockValue2) + 1, 0);
						return true;
					}
				}
			}
		}
		if (flag)
		{
			if (this.State(_blockValue) == BlockLiquidv2.UpdateID.Evaporate)
			{
				WaterEvaporationManager.AddToEvapList(_blockPos);
				this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Sleep);
				return true;
			}
			WaterEvaporationManager.AddToRestList(_blockPos);
		}
		this.ChangeThis(world, _clrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Sleep);
		return true;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00034634 File Offset: 0x00032834
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.type == this.blockID && !world.IsRemote())
		{
			if (_blockValue.rotation != 8)
			{
				int count = this.Count;
				this.ChangeThis(world, _chunk.ClrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, count, 0, BlockLiquidv2.FlowDirection.None);
				if (GameTimer.Instance.elapsedTicks > 0)
				{
					_blockValue.meta2 = (byte)BlockLiquidv2.MAX_EMISSIONS;
					_blockValue.rotation = 8;
					this.UpdateTick(world, _chunk.ClrIdx, _blockPos, _blockValue, false, 0UL, null);
					return;
				}
			}
			else
			{
				this.ChangeThis(world, _chunk.ClrIdx, _blockValue, _blockPos, BlockLiquidv2.UpdateID.Awake, (int)_blockValue.meta2, 0, BlockLiquidv2.FlowDirection.None);
				if (GameTimer.Instance.elapsedTicks > 0)
				{
					this.UpdateTick(world, _chunk.ClrIdx, _blockPos, _blockValue, false, 0UL, null);
				}
			}
		}
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00034704 File Offset: 0x00032904
	public static void WaterDataToPlaceholderBlock(WaterValue _data, out BlockValue _bv)
	{
		if ((float)_data.GetMass() > 195f)
		{
			_bv = default(BlockValue);
			_bv.type = 242;
			_bv.damage = 0;
			_bv.meta = 0;
			_bv.meta2 = (byte)BlockLiquidv2.MAX_EMISSIONS;
			_bv.rotation = 8;
			return;
		}
		_bv = BlockValue.Air;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x00034760 File Offset: 0x00032960
	public static void WaterDataToBlockValue(WaterValue _data, out BlockValue _bv)
	{
		if ((float)_data.GetMass() > 195f)
		{
			_bv = default(BlockValue);
			_bv.type = 240;
			_bv.damage = 0;
			_bv.meta = 2;
			_bv.meta2 = (byte)BlockLiquidv2.MAX_EMISSIONS;
			_bv.rotation = 8;
			return;
		}
		_bv = BlockValue.Air;
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x000347BB File Offset: 0x000329BB
	public static void SetBlockState(ref BlockValue _blockValue, BlockLiquidv2.UpdateID _blockState)
	{
		_blockValue.meta = (byte)_blockState;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetWaterRPC()
	{
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x000347C8 File Offset: 0x000329C8
	// Note: this type is marked as 'beforefieldinit'.
	[PublicizedFrom(EAccessModifier.Private)]
	static BlockLiquidv2()
	{
		Vector3i[,] array = new Vector3i[8, 4];
		array[0, 0] = new Vector3i(-1, 0, 0);
		array[0, 1] = new Vector3i(0, 0, -1);
		array[0, 2] = new Vector3i(1, 0, 0);
		array[0, 3] = new Vector3i(0, 0, 1);
		array[1, 0] = new Vector3i(1, 0, 0);
		array[1, 1] = new Vector3i(0, 0, -1);
		array[1, 2] = new Vector3i(0, 0, 1);
		array[1, 3] = new Vector3i(-1, 0, 0);
		array[2, 0] = new Vector3i(0, 0, 1);
		array[2, 1] = new Vector3i(-1, 0, 0);
		array[2, 2] = new Vector3i(0, 0, -1);
		array[2, 3] = new Vector3i(1, 0, 0);
		array[3, 0] = new Vector3i(0, 0, -1);
		array[3, 1] = new Vector3i(0, 0, 1);
		array[3, 2] = new Vector3i(1, 0, 0);
		array[3, 3] = new Vector3i(-1, 0, 0);
		array[4, 0] = new Vector3i(-1, 0, 0);
		array[4, 1] = new Vector3i(1, 0, 0);
		array[4, 2] = new Vector3i(0, 0, -1);
		array[4, 3] = new Vector3i(0, 0, 1);
		array[5, 0] = new Vector3i(1, 0, 0);
		array[5, 1] = new Vector3i(-1, 0, 0);
		array[5, 2] = new Vector3i(0, 0, 1);
		array[5, 3] = new Vector3i(0, 0, -1);
		array[6, 0] = new Vector3i(0, 0, 1);
		array[6, 1] = new Vector3i(-1, 0, 0);
		array[6, 2] = new Vector3i(1, 0, 0);
		array[6, 3] = new Vector3i(0, 0, -1);
		array[7, 0] = new Vector3i(1, 0, 0);
		array[7, 1] = new Vector3i(-1, 0, 0);
		array[7, 2] = new Vector3i(0, 0, 1);
		array[7, 3] = new Vector3i(0, 0, -1);
		BlockLiquidv2.fallDirsSet = array;
		BlockLiquidv2.currentTime = 0f;
	}

	// Token: 0x040007FB RID: 2043
	public static Color Color = new Color32(0, 105, 148, byte.MaxValue);

	// Token: 0x040007FC RID: 2044
	[PublicizedFrom(EAccessModifier.Private)]
	public static int fallSet = 0;

	// Token: 0x040007FD RID: 2045
	[PublicizedFrom(EAccessModifier.Private)]
	public static int MAX_EMISSIONS = 3;

	// Token: 0x040007FE RID: 2046
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ZERO_EMISSIONS = 0;

	// Token: 0x040007FF RID: 2047
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ZERO_EVAPORATION = 0;

	// Token: 0x04000800 RID: 2048
	[PublicizedFrom(EAccessModifier.Private)]
	public const int AUTO_GENERATED = 8;

	// Token: 0x04000801 RID: 2049
	public static int blockUpdatesPerSecond = 16;

	// Token: 0x04000802 RID: 2050
	public static int blockUpdates = 0;

	// Token: 0x04000803 RID: 2051
	[PublicizedFrom(EAccessModifier.Private)]
	public static float blockUpdateTimer = 0f;

	// Token: 0x04000804 RID: 2052
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3i[,] fallDirsSet;

	// Token: 0x04000805 RID: 2053
	[PublicizedFrom(EAccessModifier.Private)]
	public static float currentTime;

	// Token: 0x0200010F RID: 271
	public enum UpdateID
	{
		// Token: 0x04000807 RID: 2055
		Sleep,
		// Token: 0x04000808 RID: 2056
		Evaporate,
		// Token: 0x04000809 RID: 2057
		Awake
	}

	// Token: 0x02000110 RID: 272
	public enum FlowDirection
	{
		// Token: 0x0400080B RID: 2059
		None,
		// Token: 0x0400080C RID: 2060
		North,
		// Token: 0x0400080D RID: 2061
		East,
		// Token: 0x0400080E RID: 2062
		South,
		// Token: 0x0400080F RID: 2063
		West
	}
}
