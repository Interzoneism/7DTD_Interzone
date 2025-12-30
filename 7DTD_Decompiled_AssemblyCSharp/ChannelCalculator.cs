using System;
using System.Collections.Generic;

// Token: 0x0200098E RID: 2446
[PublicizedFrom(EAccessModifier.Internal)]
public class ChannelCalculator
{
	// Token: 0x0600499A RID: 18842 RVA: 0x001D170F File Offset: 0x001CF90F
	public ChannelCalculator(WorldBase _world)
	{
		this.world = _world;
	}

	// Token: 0x170007BD RID: 1981
	// (get) Token: 0x0600499B RID: 18843 RVA: 0x001D171E File Offset: 0x001CF91E
	public static HashSet<Vector3i> list
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (ChannelCalculator.List == null)
			{
				ChannelCalculator.List = new HashSet<Vector3i>();
			}
			return ChannelCalculator.List;
		}
	}

	// Token: 0x170007BE RID: 1982
	// (get) Token: 0x0600499C RID: 18844 RVA: 0x001D1736 File Offset: 0x001CF936
	public static List<Vector3i> list2
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (ChannelCalculator.List2 == null)
			{
				ChannelCalculator.List2 = new List<Vector3i>();
			}
			return ChannelCalculator.List2;
		}
	}

	// Token: 0x0600499D RID: 18845 RVA: 0x001D1750 File Offset: 0x001CF950
	public void BlockRemovedAt(Vector3i _pos, HashSet<Vector3i> _stab0Positions)
	{
		BlockValue block = this.world.GetBlock(_pos);
		Block block2 = block.Block;
		if ((!block.isair && block2.blockMaterial.IsLiquid) || block2.StabilityIgnore)
		{
			return;
		}
		ChannelCalculator.list.Clear();
		ChannelCalculator.list2.Clear();
		this.CalcChangedPositionsFromRemove(_pos, ChannelCalculator.list2, _stab0Positions, null);
		IChunk chunk = null;
		for (int i = 0; i < ChannelCalculator.list2.Count; i++)
		{
			Vector3i vector3i = ChannelCalculator.list2[i];
			if (this.world.GetChunkFromWorldPos(vector3i, ref chunk))
			{
				int x = World.toBlockXZ(vector3i.x);
				int y = World.toBlockY(vector3i.y);
				int z = World.toBlockXZ(vector3i.z);
				int stability = (int)chunk.GetStability(x, y, z);
				if (stability > 1)
				{
					this.ChangeStability(vector3i, stability, null, _stab0Positions, chunk);
				}
			}
		}
	}

	// Token: 0x0600499E RID: 18846 RVA: 0x001D1834 File Offset: 0x001CFA34
	public void BlockPlacedAt(Vector3i _pos, bool _isForceFullStab)
	{
		int num = 15;
		if (!_isForceFullStab)
		{
			_pos.y--;
			num = (int)this.world.GetStability(_pos);
			_pos.y++;
		}
		if (num == 15)
		{
			List<Vector3i> list = new List<Vector3i>();
			for (;;)
			{
				BlockValue block;
				BlockValue blockValue = block = this.world.GetBlock(_pos);
				Block block2;
				if (block.isair || (block2 = blockValue.Block).blockMaterial.IsLiquid || block2.StabilityIgnore)
				{
					goto IL_A9;
				}
				if (!block2.StabilitySupport)
				{
					break;
				}
				this.world.SetStability(_pos, 15);
				list.Add(_pos);
				_pos.y++;
			}
			this.world.SetStability(_pos, 1);
			IL_A9:
			for (int i = list.Count - 1; i >= 0; i--)
			{
				this.ChangeStability(list[i], 15, null, null, null);
			}
			return;
		}
		bool flag;
		int maxStabilityAround = this.getMaxStabilityAround(_pos, out flag);
		int num2 = flag ? maxStabilityAround : (maxStabilityAround - 1);
		BlockValue block3 = this.world.GetBlock(_pos);
		Block block4 = block3.Block;
		if (!block3.isair && !block4.blockMaterial.IsLiquid && !block4.StabilityIgnore)
		{
			if (num2 > 1 && !block4.StabilitySupport)
			{
				num2 = 1;
			}
			this.world.SetStability(_pos, (byte)((num2 < 0) ? 0 : num2));
			this.ChangeStability(_pos, num2, null, null, null);
		}
	}

	// Token: 0x0600499F RID: 18847 RVA: 0x001D199C File Offset: 0x001CFB9C
	[PublicizedFrom(EAccessModifier.Private)]
	public int getMaxStabilityAround(Vector3i _pos, out bool _bFromDownwards)
	{
		_bFromDownwards = false;
		int num = 0;
		int num2 = 0;
		Vector3i[] allDirections = Vector3i.AllDirections;
		for (int i = 0; i < allDirections.Length; i++)
		{
			Vector3i pos = _pos + allDirections[i];
			int stability = (int)this.world.GetStability(pos);
			if (allDirections[i].y == -1)
			{
				num2 = stability;
			}
			if (stability > num && this.world.GetBlock(pos).Block.StabilitySupport)
			{
				num = stability;
			}
		}
		_bFromDownwards = (num == num2);
		return num;
	}

	// Token: 0x060049A0 RID: 18848 RVA: 0x001D1A20 File Offset: 0x001CFC20
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcChangedPositionsFromRemove(Vector3i _pos, List<Vector3i> _neighbors, HashSet<Vector3i> _stab0Positions, IChunk chunk = null)
	{
		int stability = (int)this.world.GetStability(_pos);
		this.world.SetStability(_pos, 0);
		_stab0Positions.Add(_pos);
		foreach (Vector3i vector3i in Vector3i.AllDirections)
		{
			Vector3i vector3i2 = _pos + vector3i;
			if (this.world.GetChunkFromWorldPos(vector3i2, ref chunk))
			{
				Vector3i vector3i3 = World.toBlock(vector3i2);
				BlockValue blockNoDamage = chunk.GetBlockNoDamage(vector3i3.x, vector3i3.y, vector3i3.z);
				if (!blockNoDamage.isair)
				{
					Block block = blockNoDamage.Block;
					if (!block.blockMaterial.IsLiquid && !block.StabilityIgnore)
					{
						int stability2 = (int)chunk.GetStability(vector3i3.x, vector3i3.y, vector3i3.z);
						if (stability2 != 1 || block.StabilitySupport)
						{
							if (stability2 == stability - 1 || (vector3i.y == 1 && stability2 == stability))
							{
								this.CalcChangedPositionsFromRemove(vector3i2, _neighbors, _stab0Positions, chunk);
							}
							else if (stability2 >= stability)
							{
								_neighbors.Add(vector3i2);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060049A1 RID: 18849 RVA: 0x001D1B3C File Offset: 0x001CFD3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeStability(Vector3i _pos, int _stab, List<Vector3i> _changedPositions, HashSet<Vector3i> _stab0Positions = null, IChunk chunk = null)
	{
		foreach (Vector3i other in Vector3i.AllDirections)
		{
			Vector3i vector3i = _pos + other;
			if (this.world.GetChunkFromWorldPos(vector3i, ref chunk))
			{
				Vector3i vector3i2 = World.toBlock(vector3i);
				BlockValue blockNoDamage = chunk.GetBlockNoDamage(vector3i2.x, vector3i2.y, vector3i2.z);
				if (!blockNoDamage.isair)
				{
					Block block = blockNoDamage.Block;
					if (!block.blockMaterial.IsLiquid && !block.StabilityIgnore)
					{
						int num = _stab - 1;
						if ((int)chunk.GetStability(vector3i2.x, vector3i2.y, vector3i2.z) < num)
						{
							if (!block.StabilitySupport && num > 1)
							{
								num = 1;
							}
							if (_stab0Positions != null)
							{
								if (num == 0)
								{
									_stab0Positions.Add(vector3i);
								}
								else
								{
									_stab0Positions.Remove(vector3i);
								}
							}
							if (_changedPositions != null)
							{
								_changedPositions.Add(vector3i);
							}
							chunk.SetStability(vector3i2.x, vector3i2.y, vector3i2.z, (byte)num);
							this.ChangeStability(vector3i, num, _changedPositions, _stab0Positions, chunk);
						}
					}
				}
			}
		}
	}

	// Token: 0x040038DB RID: 14555
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly WorldBase world;

	// Token: 0x040038DC RID: 14556
	[ThreadStatic]
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<Vector3i> List;

	// Token: 0x040038DD RID: 14557
	[ThreadStatic]
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Vector3i> List2;
}
