using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02001254 RID: 4692
public struct Vector3i : IEquatable<Vector3i>
{
	// Token: 0x0600930A RID: 37642 RVA: 0x003A8BEA File Offset: 0x003A6DEA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3i(int _x, int _y, int _z)
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
	}

	// Token: 0x0600930B RID: 37643 RVA: 0x003A8C01 File Offset: 0x003A6E01
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3i(Vector3 _v)
	{
		this.x = (int)_v.x;
		this.y = (int)_v.y;
		this.z = (int)_v.z;
	}

	// Token: 0x0600930C RID: 37644 RVA: 0x003A8C2A File Offset: 0x003A6E2A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3i(float _x, float _y, float _z)
	{
		this.x = (int)_x;
		this.y = (int)_y;
		this.z = (int)_z;
	}

	// Token: 0x0600930D RID: 37645 RVA: 0x003A8C44 File Offset: 0x003A6E44
	public void FloorToInt(Vector3 _v)
	{
		this.x = Utils.Fastfloor(_v.x);
		this.y = Utils.Fastfloor(_v.y);
		this.z = Utils.Fastfloor(_v.z);
	}

	// Token: 0x0600930E RID: 37646 RVA: 0x003A8C79 File Offset: 0x003A6E79
	public void RoundToInt(Vector3 _v)
	{
		this.x = Mathf.RoundToInt(_v.x);
		this.y = Mathf.RoundToInt(_v.y);
		this.z = Mathf.RoundToInt(_v.z);
	}

	// Token: 0x0600930F RID: 37647 RVA: 0x003A8CAE File Offset: 0x003A6EAE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(int _x, int _y, int _z)
	{
		return this.x == _x && this.y == _y && this.z == _z;
	}

	// Token: 0x06009310 RID: 37648 RVA: 0x003A8CD0 File Offset: 0x003A6ED0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object obj)
	{
		Vector3i other = (Vector3i)obj;
		return this.Equals(other);
	}

	// Token: 0x06009311 RID: 37649 RVA: 0x003A8CEB File Offset: 0x003A6EEB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector3i other)
	{
		return other.x == this.x && other.y == this.y && other.z == this.z;
	}

	// Token: 0x06009312 RID: 37650 RVA: 0x003A8D19 File Offset: 0x003A6F19
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector3i one, Vector3i other)
	{
		return one.x == other.x && one.y == other.y && one.z == other.z;
	}

	// Token: 0x06009313 RID: 37651 RVA: 0x003A8D47 File Offset: 0x003A6F47
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return this.x * 8976890 + this.y * 981131 + this.z;
	}

	// Token: 0x06009314 RID: 37652 RVA: 0x003A8D69 File Offset: 0x003A6F69
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector3i one, Vector3i other)
	{
		return !(one == other);
	}

	// Token: 0x06009315 RID: 37653 RVA: 0x003A8D75 File Offset: 0x003A6F75
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator -(Vector3i one, Vector3i other)
	{
		return new Vector3i(one.x - other.x, one.y - other.y, one.z - other.z);
	}

	// Token: 0x06009316 RID: 37654 RVA: 0x003A8DA3 File Offset: 0x003A6FA3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator +(Vector3i one, Vector3i other)
	{
		return new Vector3i(one.x + other.x, one.y + other.y, one.z + other.z);
	}

	// Token: 0x06009317 RID: 37655 RVA: 0x003A8DD1 File Offset: 0x003A6FD1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator *(Vector3i a, float d)
	{
		return new Vector3i((float)a.x * d, (float)a.y * d, (float)a.z * d);
	}

	// Token: 0x06009318 RID: 37656 RVA: 0x003A8DF3 File Offset: 0x003A6FF3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator *(float d, Vector3i a)
	{
		return new Vector3i((float)a.x * d, (float)a.y * d, (float)a.z * d);
	}

	// Token: 0x06009319 RID: 37657 RVA: 0x003A8E15 File Offset: 0x003A7015
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator *(Vector3i a, int i)
	{
		return new Vector3i(a.x * i, a.y * i, a.z * i);
	}

	// Token: 0x0600931A RID: 37658 RVA: 0x003A8E34 File Offset: 0x003A7034
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator *(int i, Vector3i a)
	{
		return new Vector3i(a.x * i, a.y * i, a.z * i);
	}

	// Token: 0x0600931B RID: 37659 RVA: 0x003A8E53 File Offset: 0x003A7053
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator /(Vector3i a, int i)
	{
		return new Vector3i(a.x / i, a.y / i, a.z / i);
	}

	// Token: 0x0600931C RID: 37660 RVA: 0x003A8E72 File Offset: 0x003A7072
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator &(Vector3i a, int i)
	{
		return new Vector3i(a.x & i, a.y & i, a.z & i);
	}

	// Token: 0x0600931D RID: 37661 RVA: 0x003A8E91 File Offset: 0x003A7091
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i operator &(int i, Vector3i a)
	{
		return new Vector3i(a.x & i, a.y & i, a.z & i);
	}

	// Token: 0x0600931E RID: 37662 RVA: 0x003A8EB0 File Offset: 0x003A70B0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Volume()
	{
		return this.x * this.y * this.z;
	}

	// Token: 0x0600931F RID: 37663 RVA: 0x003A8EC6 File Offset: 0x003A70C6
	public override string ToString()
	{
		return string.Format("{0}, {1}, {2}", this.x, this.y, this.z);
	}

	// Token: 0x06009320 RID: 37664 RVA: 0x003A8EF3 File Offset: 0x003A70F3
	public string ToStringNoBlanks()
	{
		return string.Format("{0},{1},{2}", this.x, this.y, this.z);
	}

	// Token: 0x06009321 RID: 37665 RVA: 0x003A8F20 File Offset: 0x003A7120
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 ToVector3()
	{
		return new Vector3((float)this.x, (float)this.y, (float)this.z);
	}

	// Token: 0x06009322 RID: 37666 RVA: 0x003A8F3C File Offset: 0x003A713C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 ToVector3Center()
	{
		return new Vector3((float)this.x + 0.5f, (float)this.y + 0.5f, (float)this.z + 0.5f);
	}

	// Token: 0x06009323 RID: 37667 RVA: 0x0020E6DF File Offset: 0x0020C8DF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 ToVector3CenterXZ()
	{
		return new Vector3((float)this.x + 0.5f, (float)this.y, (float)this.z + 0.5f);
	}

	// Token: 0x06009324 RID: 37668 RVA: 0x003A8F6C File Offset: 0x003A716C
	public static Vector3i Parse(string _s)
	{
		string[] array = _s.Split(',', StringSplitOptions.None);
		if (array.Length != 3)
		{
			return Vector3i.zero;
		}
		return new Vector3i(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
	}

	// Token: 0x06009325 RID: 37669 RVA: 0x003A8FAC File Offset: 0x003A71AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i Cross(Vector3i _a, Vector3i _b)
	{
		return new Vector3i(_a.y * _b.z - _a.z * _b.y, _a.z * _b.x - _a.x * _b.z, _a.x * _b.y - _a.y * _b.x);
	}

	// Token: 0x06009326 RID: 37670 RVA: 0x003A900F File Offset: 0x003A720F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i FromVector3Rounded(Vector3 _v)
	{
		return new Vector3i(Mathf.RoundToInt(_v.x), Mathf.RoundToInt(_v.y), Mathf.RoundToInt(_v.z));
	}

	// Token: 0x06009327 RID: 37671 RVA: 0x003A9037 File Offset: 0x003A7237
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i Min(Vector3i v1, Vector3i v2)
	{
		return new Vector3i(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y), Math.Min(v1.z, v2.z));
	}

	// Token: 0x06009328 RID: 37672 RVA: 0x003A9071 File Offset: 0x003A7271
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i Max(Vector3i v1, Vector3i v2)
	{
		return new Vector3i(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y), Math.Max(v1.z, v2.z));
	}

	// Token: 0x06009329 RID: 37673 RVA: 0x003A90AC File Offset: 0x003A72AC
	public static void SortBoundingBoxEdges(ref Vector3i _minEdge, ref Vector3i _maxEdge)
	{
		Vector3i vector3i = _minEdge;
		Vector3i vector3i2 = _maxEdge;
		_minEdge = new Vector3i(Math.Min(vector3i.x, vector3i2.x), Math.Min(vector3i.y, vector3i2.y), Math.Min(vector3i.z, vector3i2.z));
		_maxEdge = new Vector3i(Math.Max(vector3i.x, vector3i2.x), Math.Max(vector3i.y, vector3i2.y), Math.Max(vector3i.z, vector3i2.z));
	}

	// Token: 0x17000F23 RID: 3875
	// (get) Token: 0x0600932A RID: 37674 RVA: 0x003A9143 File Offset: 0x003A7343
	public bool IsValid
	{
		get
		{
			return this != Vector3i.invalid;
		}
	}

	// Token: 0x0600932B RID: 37675 RVA: 0x003A9158 File Offset: 0x003A7358
	public static Vector3i Floor(Vector3 _v)
	{
		Vector3i result;
		result.x = Utils.Fastfloor(_v.x);
		result.y = Utils.Fastfloor(_v.y);
		result.z = Utils.Fastfloor(_v.z);
		return result;
	}

	// Token: 0x0600932C RID: 37676 RVA: 0x003A919C File Offset: 0x003A739C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector3(Vector3i _v3i)
	{
		return _v3i.ToVector3();
	}

	// Token: 0x0600932D RID: 37677 RVA: 0x003A91A5 File Offset: 0x003A73A5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector3Int(Vector3i _v3i)
	{
		return new Vector3Int(_v3i.x, _v3i.y, _v3i.z);
	}

	// Token: 0x04007064 RID: 28772
	public static readonly Vector3i invalid = new Vector3i(int.MinValue, int.MinValue, int.MinValue);

	// Token: 0x04007065 RID: 28773
	public static readonly Vector3i up = new Vector3i(0, 1, 0);

	// Token: 0x04007066 RID: 28774
	public static readonly Vector3i down = new Vector3i(0, -1, 0);

	// Token: 0x04007067 RID: 28775
	public static readonly Vector3i zero = new Vector3i(0, 0, 0);

	// Token: 0x04007068 RID: 28776
	public static readonly Vector3i left = new Vector3i(-1, 0, 0);

	// Token: 0x04007069 RID: 28777
	public static readonly Vector3i right = new Vector3i(1, 0, 0);

	// Token: 0x0400706A RID: 28778
	public static readonly Vector3i forward = new Vector3i(0, 0, 1);

	// Token: 0x0400706B RID: 28779
	public static readonly Vector3i back = new Vector3i(0, 0, -1);

	// Token: 0x0400706C RID: 28780
	public static readonly Vector3i one = new Vector3i(1, 1, 1);

	// Token: 0x0400706D RID: 28781
	public static readonly Vector3i min = new Vector3i(int.MinValue, int.MinValue, int.MinValue);

	// Token: 0x0400706E RID: 28782
	public static readonly Vector3i max = new Vector3i(int.MaxValue, int.MaxValue, int.MaxValue);

	// Token: 0x0400706F RID: 28783
	public static readonly Vector3i[] MIDDLE_AND_ADJACENT_DIRECTIONS = new Vector3i[]
	{
		Vector3i.zero,
		new Vector3i(1, 0, 0),
		new Vector3i(-1, 0, 0),
		new Vector3i(0, 1, 0),
		new Vector3i(0, -1, 0),
		new Vector3i(0, 0, 1),
		new Vector3i(0, 0, -1)
	};

	// Token: 0x04007070 RID: 28784
	public static readonly Vector3i[] AllDirections = new Vector3i[]
	{
		Vector3i.right,
		Vector3i.left,
		Vector3i.up,
		Vector3i.down,
		Vector3i.forward,
		Vector3i.back
	};

	// Token: 0x04007071 RID: 28785
	public static readonly Vector3i[] AllDirectionsShuffled = new Vector3i[]
	{
		Vector3i.right,
		Vector3i.left,
		Vector3i.up,
		Vector3i.down,
		Vector3i.forward,
		Vector3i.back
	};

	// Token: 0x04007072 RID: 28786
	public static readonly Vector3i[] MIDDLE_AND_HORIZONTAL_DIRECTIONS = new Vector3i[]
	{
		Vector3i.zero,
		new Vector3i(1, 0, 0),
		new Vector3i(-1, 0, 0),
		new Vector3i(0, 0, 1),
		new Vector3i(0, 0, -1)
	};

	// Token: 0x04007073 RID: 28787
	public static readonly Vector3i[] HORIZONTAL_DIRECTIONS = new Vector3i[]
	{
		new Vector3i(1, 0, 0),
		new Vector3i(-1, 0, 0),
		new Vector3i(0, 0, 1),
		new Vector3i(0, 0, -1)
	};

	// Token: 0x04007074 RID: 28788
	public static readonly Vector3i[] MIDDLE_AND_HORIZONTAL_DIRECTIONS_DIAGONAL = new Vector3i[]
	{
		Vector3i.zero,
		new Vector3i(1, 0, 0),
		new Vector3i(1, 0, -1),
		new Vector3i(0, 0, -1),
		new Vector3i(-1, 0, -1),
		new Vector3i(-1, 0, 0),
		new Vector3i(-1, 0, 1),
		new Vector3i(0, 0, 1),
		new Vector3i(1, 0, 1)
	};

	// Token: 0x04007075 RID: 28789
	public int x;

	// Token: 0x04007076 RID: 28790
	public int y;

	// Token: 0x04007077 RID: 28791
	public int z;
}
