using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02001251 RID: 4689
public struct Vector2i : IEquatable<Vector2i>
{
	// Token: 0x060092DD RID: 37597 RVA: 0x003A855D File Offset: 0x003A675D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2i(int _x, int _y)
	{
		this.x = _x;
		this.y = _y;
	}

	// Token: 0x060092DE RID: 37598 RVA: 0x003A856D File Offset: 0x003A676D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2i(Vector2 vector2)
	{
		this = default(Vector2i);
		this.x = Mathf.FloorToInt(vector2.x);
		this.y = Mathf.FloorToInt(vector2.y);
	}

	// Token: 0x060092DF RID: 37599 RVA: 0x003A8598 File Offset: 0x003A6798
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2i(Vector2Int vector2)
	{
		this = default(Vector2i);
		this.x = vector2.x;
		this.y = vector2.y;
	}

	// Token: 0x060092E0 RID: 37600 RVA: 0x003A855D File Offset: 0x003A675D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(int _x, int _y)
	{
		this.x = _x;
		this.y = _y;
	}

	// Token: 0x060092E1 RID: 37601 RVA: 0x003A85BB File Offset: 0x003A67BB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2Int AsVector2Int()
	{
		return new Vector2Int(this.x, this.y);
	}

	// Token: 0x060092E2 RID: 37602 RVA: 0x003A85CE File Offset: 0x003A67CE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 AsVector2()
	{
		return new Vector2((float)this.x, (float)this.y);
	}

	// Token: 0x060092E3 RID: 37603 RVA: 0x003A85E4 File Offset: 0x003A67E4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object obj)
	{
		Vector2i other = (Vector2i)obj;
		return this.Equals(other);
	}

	// Token: 0x060092E4 RID: 37604 RVA: 0x003A85FF File Offset: 0x003A67FF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector2i other)
	{
		return other.x == this.x && other.y == this.y;
	}

	// Token: 0x060092E5 RID: 37605 RVA: 0x003A8620 File Offset: 0x003A6820
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Distance(Vector2i a, Vector2i b)
	{
		double num = (double)(a.x - b.x);
		double num2 = (double)(a.y - b.y);
		return (float)Math.Sqrt(num * num + num2 * num2);
	}

	// Token: 0x060092E6 RID: 37606 RVA: 0x003A8658 File Offset: 0x003A6858
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSqr(Vector2i a, Vector2i b)
	{
		float num = (float)(a.x - b.x);
		float num2 = (float)(a.y - b.y);
		return num * num + num2 * num2;
	}

	// Token: 0x060092E7 RID: 37607 RVA: 0x003A8688 File Offset: 0x003A6888
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int DistanceSqrInt(Vector2i a, Vector2i b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	// Token: 0x060092E8 RID: 37608 RVA: 0x003A86B8 File Offset: 0x003A68B8
	public void Normalize()
	{
		if (this.x < 0)
		{
			this.x = -1;
		}
		else if (this.x > 0)
		{
			this.x = 1;
		}
		if (this.y < 0)
		{
			this.y = -1;
			return;
		}
		if (this.y > 0)
		{
			this.y = 1;
		}
	}

	// Token: 0x060092E9 RID: 37609 RVA: 0x003A8708 File Offset: 0x003A6908
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector2i one, Vector2i other)
	{
		return one.x == other.x && one.y == other.y;
	}

	// Token: 0x060092EA RID: 37610 RVA: 0x003A8728 File Offset: 0x003A6928
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return this.x * 8976890 + this.y * 981131;
	}

	// Token: 0x060092EB RID: 37611 RVA: 0x003A8743 File Offset: 0x003A6943
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector2i one, Vector2i other)
	{
		return !(one == other);
	}

	// Token: 0x060092EC RID: 37612 RVA: 0x003A874F File Offset: 0x003A694F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i operator +(Vector2i one, Vector2i other)
	{
		return new Vector2i(one.x + other.x, one.y + other.y);
	}

	// Token: 0x060092ED RID: 37613 RVA: 0x003A8770 File Offset: 0x003A6970
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i operator -(Vector2i one, Vector2i other)
	{
		return new Vector2i(one.x - other.x, one.y - other.y);
	}

	// Token: 0x060092EE RID: 37614 RVA: 0x003A8791 File Offset: 0x003A6991
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i operator /(Vector2i one, int div)
	{
		return new Vector2i(one.x / div, one.y / div);
	}

	// Token: 0x060092EF RID: 37615 RVA: 0x003A87A8 File Offset: 0x003A69A8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i operator *(Vector2i a, int i)
	{
		return new Vector2i(a.x * i, a.y * i);
	}

	// Token: 0x060092F0 RID: 37616 RVA: 0x003A87BF File Offset: 0x003A69BF
	public override string ToString()
	{
		return string.Format("{0}, {1}", this.x, this.y);
	}

	// Token: 0x060092F1 RID: 37617 RVA: 0x003A85BB File Offset: 0x003A67BB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2Int(Vector2i _v2i)
	{
		return new Vector2Int(_v2i.x, _v2i.y);
	}

	// Token: 0x04007053 RID: 28755
	public static readonly Vector2i zero = new Vector2i(0, 0);

	// Token: 0x04007054 RID: 28756
	public static readonly Vector2i one = new Vector2i(1, 1);

	// Token: 0x04007055 RID: 28757
	public static readonly Vector2i min = new Vector2i(int.MinValue, int.MinValue);

	// Token: 0x04007056 RID: 28758
	public static readonly Vector2i max = new Vector2i(int.MaxValue, int.MaxValue);

	// Token: 0x04007057 RID: 28759
	public static readonly Vector2i up = new Vector2i(0, 1);

	// Token: 0x04007058 RID: 28760
	public static readonly Vector2i down = new Vector2i(0, -1);

	// Token: 0x04007059 RID: 28761
	public static readonly Vector2i right = new Vector2i(1, 0);

	// Token: 0x0400705A RID: 28762
	public static readonly Vector2i left = new Vector2i(-1, 0);

	// Token: 0x0400705B RID: 28763
	public int x;

	// Token: 0x0400705C RID: 28764
	public int y;
}
