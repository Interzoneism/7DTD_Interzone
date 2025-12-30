using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200124F RID: 4687
public struct Vector2d : IEquatable<Vector2d>
{
	// Token: 0x060092BE RID: 37566 RVA: 0x003A81FE File Offset: 0x003A63FE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2d(double _x, double _y)
	{
		this.x = _x;
		this.y = _y;
	}

	// Token: 0x060092BF RID: 37567 RVA: 0x003A820E File Offset: 0x003A640E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2d(Vector2 _v)
	{
		this.x = (double)_v.x;
		this.y = (double)_v.y;
	}

	// Token: 0x060092C0 RID: 37568 RVA: 0x003A822A File Offset: 0x003A642A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2d(Vector2i _v)
	{
		this.x = (double)_v.x;
		this.y = (double)_v.y;
	}

	// Token: 0x060092C1 RID: 37569 RVA: 0x003A8246 File Offset: 0x003A6446
	public bool Equals(double _x, double _y)
	{
		return this.x == _x && this.y == _y;
	}

	// Token: 0x060092C2 RID: 37570 RVA: 0x003A825C File Offset: 0x003A645C
	public override bool Equals(object obj)
	{
		Vector2d other = (Vector2d)obj;
		return this.Equals(other);
	}

	// Token: 0x060092C3 RID: 37571 RVA: 0x003A8277 File Offset: 0x003A6477
	public bool Equals(Vector2d other)
	{
		return other.x == this.x && other.y == this.y;
	}

	// Token: 0x060092C4 RID: 37572 RVA: 0x003A8297 File Offset: 0x003A6497
	public static bool operator ==(Vector2d one, Vector2d other)
	{
		return one.x == other.x && one.y == other.y;
	}

	// Token: 0x060092C5 RID: 37573 RVA: 0x003A82B7 File Offset: 0x003A64B7
	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
	}

	// Token: 0x060092C6 RID: 37574 RVA: 0x003A82D2 File Offset: 0x003A64D2
	public static bool operator !=(Vector2d one, Vector2d other)
	{
		return !(one == other);
	}

	// Token: 0x060092C7 RID: 37575 RVA: 0x003A82DE File Offset: 0x003A64DE
	public static Vector2d operator -(Vector2d one, Vector2d other)
	{
		return new Vector2d(one.x - other.x, one.y - other.y);
	}

	// Token: 0x060092C8 RID: 37576 RVA: 0x003A82FF File Offset: 0x003A64FF
	public static Vector2d operator +(Vector2d one, Vector2d other)
	{
		return new Vector2d(one.x + other.x, one.y + other.y);
	}

	// Token: 0x060092C9 RID: 37577 RVA: 0x003A8320 File Offset: 0x003A6520
	public static Vector2d operator *(Vector2d a, double d)
	{
		return new Vector2d(a.x * d, a.y * d);
	}

	// Token: 0x060092CA RID: 37578 RVA: 0x003A8337 File Offset: 0x003A6537
	public static Vector2d operator *(double d, Vector2d a)
	{
		return new Vector2d(a.x * d, a.y * d);
	}

	// Token: 0x060092CB RID: 37579 RVA: 0x003A834E File Offset: 0x003A654E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Dot(Vector2d lhs, Vector2d rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y;
	}

	// Token: 0x060092CC RID: 37580 RVA: 0x003A836B File Offset: 0x003A656B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double Dot(Vector2 rhs)
	{
		return this.x * (double)rhs.x + this.y * (double)rhs.y;
	}

	// Token: 0x060092CD RID: 37581 RVA: 0x003A838A File Offset: 0x003A658A
	public override string ToString()
	{
		return this.ToCultureInvariantString();
	}

	// Token: 0x060092CE RID: 37582 RVA: 0x003A8394 File Offset: 0x003A6594
	public string ToCultureInvariantString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.x.ToCultureInvariantString("F1"),
			", ",
			this.y.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x0400704D RID: 28749
	public double x;

	// Token: 0x0400704E RID: 28750
	public double y;

	// Token: 0x0400704F RID: 28751
	public static readonly Vector2d Zero = new Vector2d(0.0, 0.0);
}
