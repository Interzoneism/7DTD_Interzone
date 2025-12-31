using System;
using UnityEngine;

// Token: 0x02000493 RID: 1171
public class RandomPositionGenerator
{
	// Token: 0x0600262C RID: 9772 RVA: 0x000F7320 File Offset: 0x000F5520
	public static Vector3 CalcAround(EntityAlive _entity, int _maxXZ, int _maxY)
	{
		Vector3 result;
		if (!RandomPositionGenerator.CalcAround(_entity, _maxXZ, _maxY, false, out result) && _entity.isSwimming)
		{
			RandomPositionGenerator.CalcAround(_entity, _maxXZ, _maxY, true, out result);
		}
		return result;
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x000F7350 File Offset: 0x000F5550
	public static Vector3 CalcTowards(EntityAlive _entity, int _minXZ, int _maxXZ, int _maxY, Vector3 _position)
	{
		Vector3 dirV = _position - _entity.position;
		return RandomPositionGenerator.CalcInDir(_entity, _minXZ, _maxXZ, _maxY, dirV, 80f);
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000F737C File Offset: 0x000F557C
	public static Vector3 CalcAway(EntityAlive _entity, int _minXZ, int _maxXZ, int _maxY, Vector3 _position)
	{
		Vector3 dirV = _entity.position - _position;
		return RandomPositionGenerator.CalcInDir(_entity, _minXZ, _maxXZ, _maxY, dirV, 80f);
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000F73A8 File Offset: 0x000F55A8
	public static Vector3 CalcInDir(EntityAlive _entity, int _minXZ, int _maxXZ, int _maxY, Vector3 _dirV, float _angleMax = 80f)
	{
		Vector3 result;
		if (!RandomPositionGenerator.CalcDir(_entity, _minXZ, _maxXZ, _maxY, _dirV, _angleMax, false, out result) && _entity.isSwimming)
		{
			RandomPositionGenerator.CalcDir(_entity, _minXZ, _maxXZ, _maxY, _dirV, _angleMax, true, out result);
		}
		return result;
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000F73E4 File Offset: 0x000F55E4
	public static Vector3 CalcNear(EntityAlive _entity, Vector3 target, int _xzDist, int _yDist)
	{
		GameRandom rand = _entity.rand;
		int num = rand.RandomRange(2 * _xzDist) - _xzDist;
		int num2 = rand.RandomRange(2 * _yDist) - _yDist;
		int num3 = rand.RandomRange(2 * _xzDist) - _xzDist;
		num += Utils.Fastfloor(target.x);
		num2 += Utils.Fastfloor(target.y);
		num3 += Utils.Fastfloor(target.z);
		return new Vector3((float)num, (float)num2, (float)num3);
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000F7450 File Offset: 0x000F5650
	public static Vector3 CalcPositionInDirection(Entity _entity, Vector3 _startPos, Vector3 _dirV, float _dist, float _angleMax)
	{
		World world = _entity.world;
		_dirV.y = 0f;
		Vector3 vector = _dirV.normalized;
		Quaternion q = Quaternion.Euler(0f, _angleMax * (_entity.rand.RandomFloat - 0.5f), 0f);
		vector = Matrix4x4.TRS(Vector3.zero, q, Vector3.one).MultiplyVector(vector);
		Vector3i vector3i = World.worldToBlockPos(_startPos);
		Chunk chunk;
		do
		{
			vector3i.x = (int)(_startPos.x + vector.x * _dist);
			vector3i.z = (int)(_startPos.z + vector.z * _dist);
			chunk = (Chunk)world.GetChunkFromWorldPos(vector3i.x, 0, vector3i.z);
			if (chunk != null)
			{
				break;
			}
			_dist -= 8f;
		}
		while (_dist > 0f);
		if (_dist <= 0f || chunk == null)
		{
			return Vector3.zero;
		}
		int x = World.toBlockXZ(vector3i.x);
		int z = World.toBlockXZ(vector3i.z);
		BlockValue blockNoDamage = chunk.GetBlockNoDamage(x, vector3i.y, z);
		if (!blockNoDamage.Block.IsMovementBlocked(world, vector3i, blockNoDamage, BlockFaceFlag.None))
		{
			do
			{
				int num = vector3i.y - 1;
				vector3i.y = num;
				if (num < 0)
				{
					goto IL_1B7;
				}
				blockNoDamage = chunk.GetBlockNoDamage(x, vector3i.y, z);
			}
			while (!blockNoDamage.Block.IsMovementBlocked(world, vector3i, blockNoDamage, BlockFaceFlag.None));
			vector3i.y++;
			return vector3i;
		}
		do
		{
			int num = vector3i.y + 1;
			vector3i.y = num;
			if (num >= 256)
			{
				goto Block_5;
			}
			blockNoDamage = chunk.GetBlockNoDamage(x, vector3i.y, z);
		}
		while (blockNoDamage.Block.IsMovementBlocked(world, vector3i, blockNoDamage, BlockFaceFlag.None));
		return vector3i;
		Block_5:
		IL_1B7:
		return Vector3.zero;
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000F761C File Offset: 0x000F581C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool CalcAround(EntityAlive _entity, int _xzDist, int _yDist, bool _canSwim, out Vector3 _destPos)
	{
		GameRandom rand = _entity.rand;
		World world = _entity.world;
		ChunkCluster chunkCache = world.ChunkCache;
		Vector3 worldPos = _entity.position;
		if (_entity.IsSleeper)
		{
			worldPos = _entity.SleeperSpawnPosition;
		}
		Vector3i vector3i = World.worldToBlockPos(worldPos);
		bool flag = false;
		if (_entity.hasHome())
		{
			flag = (_entity.getHomePosition().getDistance(vector3i.x, vector3i.y, vector3i.z) + 4f < (float)(_entity.getMaximumHomeDistance() + _xzDist));
		}
		for (int i = 0; i < 30; i++)
		{
			Vector3i vector3i2;
			vector3i2.x = rand.RandomRange(2 * _xzDist) - _xzDist;
			vector3i2.z = rand.RandomRange(2 * _xzDist) - _xzDist;
			vector3i2.y = rand.RandomRange(2 * _yDist) - _yDist;
			vector3i2.x += vector3i.x;
			vector3i2.y += vector3i.y;
			vector3i2.z += vector3i.z;
			if (chunkCache.GetBlock(vector3i2).isair && (_canSwim || !world.IsWater(vector3i2)) && (!flag || _entity.isWithinHomeDistance(vector3i2.x, vector3i2.y, vector3i2.z)) && vector3i2.y >= 0)
			{
				if (!_canSwim)
				{
					bool flag2 = false;
					Vector3i vector3i3 = vector3i2;
					for (int j = 0; j < 10; j++)
					{
						vector3i3.y--;
						BlockValue block = chunkCache.GetBlock(vector3i3);
						if (world.IsWater(vector3i3))
						{
							flag2 = true;
							break;
						}
						if (block.Block.IsMovementBlocked(world, vector3i3, block, BlockFaceFlag.None))
						{
							break;
						}
					}
					if (flag2)
					{
						goto IL_1C6;
					}
				}
				_destPos = new Vector3((float)vector3i2.x + 0.5f, (float)vector3i2.y, (float)vector3i2.z + 0.5f);
				return true;
			}
			IL_1C6:;
		}
		_destPos = Vector3.zero;
		return false;
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000F780C File Offset: 0x000F5A0C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool CalcDir(EntityAlive _entity, int _distMinXZ, int _distMaxXZ, int _distMaxY, Vector3 _directionVec, float _angleMax, bool _canSwim, out Vector3 _destPos)
	{
		if (_directionVec == Vector3.zero)
		{
			return RandomPositionGenerator.CalcAround(_entity, _distMaxXZ, _distMaxY, _canSwim, out _destPos);
		}
		GameRandom rand = _entity.rand;
		ChunkCluster chunkCache = _entity.world.ChunkCache;
		Vector3i vector3i = World.worldToBlockPos(_entity.position);
		if (_distMaxXZ < _distMinXZ)
		{
			_distMaxXZ = _distMinXZ;
		}
		bool flag = false;
		if (_entity.hasHome())
		{
			float num = _entity.getHomePosition().getDistance(vector3i.x, vector3i.y, vector3i.z) + 1f;
			if ((float)_distMinXZ > num)
			{
				_distMinXZ = (int)num;
			}
			if ((float)_distMaxXZ > num)
			{
				_distMaxXZ = (int)num;
			}
			flag = ((float)(_entity.getMaximumHomeDistance() + _distMaxXZ) - num >= 2f);
		}
		float maxExclusive = (float)(_distMaxXZ - _distMinXZ);
		Vector2 vector;
		vector.x = _directionVec.x;
		vector.y = _directionVec.z;
		vector.Normalize();
		for (int i = 0; i < 30; i++)
		{
			float f = _angleMax * (rand.RandomFloat - 0.5f) * 0.017453292f;
			float num2 = (float)_distMinXZ + rand.RandomRange(maxExclusive);
			float num3 = Mathf.Sin(f);
			float num4 = Mathf.Cos(f);
			Vector2 vector2;
			vector2.x = vector.x * num4 - vector.y * num3;
			vector2.y = vector.x * num3 + vector.y * num4;
			vector2.x *= num2;
			vector2.y *= num2;
			Vector3i vector3i2;
			vector3i2.x = Utils.Fastfloor(vector2.x);
			vector3i2.z = Utils.Fastfloor(vector2.y);
			vector3i2.y = rand.RandomRange(2 * _distMaxY) - _distMaxY;
			vector3i2.x += vector3i.x;
			vector3i2.y += vector3i.y;
			vector3i2.z += vector3i.z;
			if (chunkCache.GetBlock(vector3i2).isair && (_canSwim || !_entity.world.IsWater(vector3i2)) && (!flag || _entity.isWithinHomeDistance(vector3i2.x, vector3i2.y, vector3i2.z)))
			{
				_destPos = new Vector3((float)vector3i2.x + 0.5f, (float)vector3i2.y, (float)vector3i2.z + 0.5f);
				return true;
			}
		}
		_destPos = Vector3.zero;
		return false;
	}
}
