using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200113F RID: 4415
public class BoundsUtils
{
	// Token: 0x06008A97 RID: 35479 RVA: 0x00380CFF File Offset: 0x0037EEFF
	public static Bounds BoundsForMinMax(float mnx, float mny, float mnz, float mxx, float mxy, float mxz)
	{
		return BoundsUtils.BoundsForMinMax(new Vector3(mnx, mny, mnz), new Vector3(mxx, mxy, mxz));
	}

	// Token: 0x06008A98 RID: 35480 RVA: 0x00380D18 File Offset: 0x0037EF18
	public static Bounds BoundsForMinMax(Vector3 _v1, Vector3 _v2)
	{
		Vector3 vector = _v2 - _v1;
		return new Bounds(_v1 + vector / 2f, vector);
	}

	// Token: 0x06008A99 RID: 35481 RVA: 0x00380D44 File Offset: 0x0037EF44
	public static Bounds ExpandBounds(Bounds bounds, float x, float y, float z)
	{
		bounds.Expand(new Vector3(x, y, z));
		return bounds;
	}

	// Token: 0x06008A9A RID: 35482 RVA: 0x00380D56 File Offset: 0x0037EF56
	public static Bounds ContractBounds(Bounds bounds, float x, float y, float z)
	{
		return BoundsUtils.ExpandBounds(bounds, -x, -y, -z);
	}

	// Token: 0x06008A9B RID: 35483 RVA: 0x00380D64 File Offset: 0x0037EF64
	public static float ClipBoundsMoveY(Vector3 bmins, Vector3 bmaxs, float move, Bounds collider)
	{
		Vector3 min = collider.min;
		Vector3 max = collider.max;
		if (move != 0f && bmaxs.x > min.x && bmins.x < max.x && bmaxs.z > min.z && bmins.z < max.z)
		{
			if (move > 0f && min.y >= bmaxs.y)
			{
				move = MathUtils.Clamp(min.y - bmaxs.y, 0f, move);
			}
			else if (move < 0f && max.y <= bmins.y)
			{
				move = MathUtils.Clamp(max.y - bmins.y, move, 0f);
			}
			else if (move < 0f)
			{
				float num = max.y - bmins.y;
				if (num < 0.2f)
				{
					move = num;
				}
			}
			if (Math.Abs(move) < 0.0001f)
			{
				move = 0f;
			}
		}
		return move;
	}

	// Token: 0x06008A9C RID: 35484 RVA: 0x00380E6C File Offset: 0x0037F06C
	public static float ClipBoundsMoveX(Vector3 bmins, Vector3 bmaxs, float move, Bounds collider)
	{
		Vector3 min = collider.min;
		Vector3 max = collider.max;
		if (move != 0f && bmaxs.y > min.y && bmins.y < max.y && bmaxs.z > min.z && bmins.z < max.z)
		{
			if (move > 0f && min.x >= bmaxs.x)
			{
				move = MathUtils.Clamp(min.x - bmaxs.x, 0f, move);
			}
			else if (move < 0f && max.x <= bmins.x)
			{
				move = MathUtils.Clamp(max.x - bmins.x, move, 0f);
			}
			if (Math.Abs(move) < 0.0001f)
			{
				move = 0f;
			}
		}
		return move;
	}

	// Token: 0x06008A9D RID: 35485 RVA: 0x00380F4C File Offset: 0x0037F14C
	public static float ClipBoundsMoveZ(Vector3 bmins, Vector3 bmaxs, float move, Bounds collider)
	{
		Vector3 min = collider.min;
		Vector3 max = collider.max;
		if (move != 0f && bmaxs.x > min.x && bmins.x < max.x && bmaxs.y > min.y && bmins.y < max.y)
		{
			if (move > 0f && min.z >= bmaxs.z)
			{
				move = MathUtils.Clamp(min.z - bmaxs.z, 0f, move);
			}
			else if (move < 0f && max.z <= bmins.z)
			{
				move = MathUtils.Clamp(max.z - bmins.z, move, 0f);
			}
			if (Math.Abs(move) < 0.0001f)
			{
				move = 0f;
			}
		}
		return move;
	}

	// Token: 0x06008A9E RID: 35486 RVA: 0x0038102C File Offset: 0x0037F22C
	public static Vector3 ClipBoundsMove(Bounds bounds, Vector3 move, IList<Bounds> colliderList, int numColliders)
	{
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		move.y = BoundsUtils.ClipBoundsMoveY(min, max, move.y, colliderList, numColliders);
		min.y += move.y;
		max.y += move.y;
		move.x = BoundsUtils.ClipBoundsMoveX(min, max, move.x, colliderList, numColliders);
		min.x += move.x;
		max.x += move.x;
		move.z = BoundsUtils.ClipBoundsMoveZ(min, max, move.z, colliderList, numColliders);
		return move;
	}

	// Token: 0x06008A9F RID: 35487 RVA: 0x003810D0 File Offset: 0x0037F2D0
	public static float ClipBoundsMoveY(Vector3 bmins, Vector3 bmaxs, float move, IList<Bounds> colliderList, int numColliders)
	{
		if (move != 0f)
		{
			for (int i = 0; i < numColliders; i++)
			{
				Bounds bounds = colliderList[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (bmaxs.x > min.x + 0f && bmins.x < max.x - 0f && bmaxs.z > min.z + 0f && bmins.z < max.z - 0f)
				{
					if (move > 0f && min.y >= bmaxs.y + 0f)
					{
						move = MathUtils.Clamp(min.y - bmaxs.y, 0f, move);
					}
					else if (move < 0f && max.y <= bmins.y - 0f)
					{
						move = MathUtils.Clamp(max.y - bmins.y, move, 0f);
					}
					else if (move < 0f)
					{
						float num = max.y - bmins.y;
						if (num < 0.2f)
						{
							move = num;
						}
					}
					if (Math.Abs(move) < 0.0001f)
					{
						move = 0f;
						break;
					}
				}
			}
		}
		return move;
	}

	// Token: 0x06008AA0 RID: 35488 RVA: 0x0038121C File Offset: 0x0037F41C
	public static float ClipBoundsMoveX(Vector3 bmins, Vector3 bmaxs, float move, IList<Bounds> colliderList, int numColliders)
	{
		if (move != 0f)
		{
			for (int i = 0; i < numColliders; i++)
			{
				Bounds bounds = colliderList[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (bmaxs.y > min.y + 0f && bmins.y < max.y - 0f && bmaxs.z > min.z + 0f && bmins.z < max.z - 0f)
				{
					if (move > 0f && min.x >= bmaxs.x + 0f)
					{
						move = MathUtils.Clamp(min.x - bmaxs.x, 0f, move);
					}
					else if (move < 0f && max.x <= bmins.x - 0f)
					{
						move = MathUtils.Clamp(max.x - bmins.x, move, 0f);
					}
					if (Math.Abs(move) < 0.0001f)
					{
						move = 0f;
						break;
					}
				}
			}
		}
		return move;
	}

	// Token: 0x06008AA1 RID: 35489 RVA: 0x00381340 File Offset: 0x0037F540
	public static float ClipBoundsMoveZ(Vector3 bmins, Vector3 bmaxs, float move, IList<Bounds> colliderList, int numColliders)
	{
		if (move != 0f)
		{
			for (int i = 0; i < numColliders; i++)
			{
				Bounds bounds = colliderList[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (bmaxs.x > min.x + 0f && bmins.x < max.x - 0f && bmaxs.y > min.y + 0f && bmins.y < max.y - 0f)
				{
					if (move > 0f && min.z >= bmaxs.z + 0f)
					{
						move = MathUtils.Clamp(min.z - bmaxs.z, 0f, move);
					}
					else if (move < 0f && max.z <= bmins.z - 0f)
					{
						move = MathUtils.Clamp(max.z - bmins.z, move, 0f);
					}
					if (Math.Abs(move) < 0.0001f)
					{
						move = 0f;
						break;
					}
				}
			}
		}
		return move;
	}

	// Token: 0x06008AA2 RID: 35490 RVA: 0x00381464 File Offset: 0x0037F664
	public static bool Intersects(Bounds bounds, Vector3 min1, Vector3 max1)
	{
		Vector3 min2 = bounds.min;
		Vector3 max2 = bounds.max;
		return min2.x <= max1.x && max2.x >= min1.x && min2.y <= max1.y && max2.y >= min1.y && min2.z <= max1.z && max2.z >= min1.z;
	}

	// Token: 0x06008AA3 RID: 35491 RVA: 0x003814DC File Offset: 0x0037F6DC
	public static Bounds ExpandDirectional(Bounds bounds, Vector3 move)
	{
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		if (move.x < 0f)
		{
			min.x += move.x;
		}
		else
		{
			max.x += move.x;
		}
		if (move.y < 0f)
		{
			min.y += move.y;
		}
		else
		{
			max.y += move.y;
		}
		if (move.z < 0f)
		{
			min.z += move.z;
		}
		else
		{
			max.z += move.z;
		}
		bounds.SetMinMax(min, max);
		return bounds;
	}

	// Token: 0x06008AA4 RID: 35492 RVA: 0x00381598 File Offset: 0x0037F798
	public static void WriteBounds(BinaryWriter _bw, Bounds bounds)
	{
		_bw.Write(bounds.min.x);
		_bw.Write(bounds.min.y);
		_bw.Write(bounds.min.z);
		_bw.Write(bounds.max.x);
		_bw.Write(bounds.max.y);
		_bw.Write(bounds.max.z);
	}

	// Token: 0x06008AA5 RID: 35493 RVA: 0x00381614 File Offset: 0x0037F814
	public static Bounds ReadBounds(BinaryReader _br)
	{
		Bounds result = default(Bounds);
		result.SetMinMax(new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle()), new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle()));
		return result;
	}

	// Token: 0x04006C6B RID: 27755
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kClipEpsilon = 0f;

	// Token: 0x04006C6C RID: 27756
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMinMoveClamp = 0.0001f;
}
