using System;
using UnityEngine;

// Token: 0x02001253 RID: 4691
public struct Vector3d : IEquatable<Vector3d>
{
	// Token: 0x060092F9 RID: 37625 RVA: 0x003A8911 File Offset: 0x003A6B11
	public Vector3d(double _x, double _y, double _z)
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
	}

	// Token: 0x060092FA RID: 37626 RVA: 0x003A8928 File Offset: 0x003A6B28
	public Vector3d(Vector3 _v)
	{
		this.x = (double)_v.x;
		this.y = (double)_v.y;
		this.z = (double)_v.z;
	}

	// Token: 0x060092FB RID: 37627 RVA: 0x003A8951 File Offset: 0x003A6B51
	public Vector3d(Vector3i _v)
	{
		this.x = (double)_v.x;
		this.y = (double)_v.y;
		this.z = (double)_v.z;
	}

	// Token: 0x060092FC RID: 37628 RVA: 0x003A897A File Offset: 0x003A6B7A
	public bool Equals(double _x, double _y, double _z)
	{
		return this.x == _x && this.y == _y && this.z == _z;
	}

	// Token: 0x060092FD RID: 37629 RVA: 0x003A899C File Offset: 0x003A6B9C
	public override bool Equals(object obj)
	{
		Vector3d other = (Vector3d)obj;
		return this.Equals(other);
	}

	// Token: 0x060092FE RID: 37630 RVA: 0x003A89B7 File Offset: 0x003A6BB7
	public bool Equals(Vector3d other)
	{
		return other.x == this.x && other.y == this.y && other.z == this.z;
	}

	// Token: 0x060092FF RID: 37631 RVA: 0x003A89E5 File Offset: 0x003A6BE5
	public static bool operator ==(Vector3d one, Vector3d other)
	{
		return one.x == other.x && one.y == other.y && one.z == other.z;
	}

	// Token: 0x06009300 RID: 37632 RVA: 0x003A8A13 File Offset: 0x003A6C13
	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
	}

	// Token: 0x06009301 RID: 37633 RVA: 0x003A8A3C File Offset: 0x003A6C3C
	public static bool operator !=(Vector3d one, Vector3d other)
	{
		return !(one == other);
	}

	// Token: 0x06009302 RID: 37634 RVA: 0x003A8A48 File Offset: 0x003A6C48
	public static Vector3d operator -(Vector3d one, Vector3d other)
	{
		return new Vector3d(one.x - other.x, one.y - other.y, one.z - other.z);
	}

	// Token: 0x06009303 RID: 37635 RVA: 0x003A8A76 File Offset: 0x003A6C76
	public static Vector3d operator +(Vector3d one, Vector3d other)
	{
		return new Vector3d(one.x + other.x, one.y + other.y, one.z + other.z);
	}

	// Token: 0x06009304 RID: 37636 RVA: 0x003A8AA4 File Offset: 0x003A6CA4
	public static Vector3d operator *(Vector3d a, double d)
	{
		return new Vector3d(a.x * d, a.y * d, a.z * d);
	}

	// Token: 0x06009305 RID: 37637 RVA: 0x003A8AC3 File Offset: 0x003A6CC3
	public static Vector3d operator *(double d, Vector3d a)
	{
		return new Vector3d(a.x * d, a.y * d, a.z * d);
	}

	// Token: 0x06009306 RID: 37638 RVA: 0x003A8AE2 File Offset: 0x003A6CE2
	public override string ToString()
	{
		return this.ToCultureInvariantString();
	}

	// Token: 0x06009307 RID: 37639 RVA: 0x003A8AEC File Offset: 0x003A6CEC
	public string ToCultureInvariantString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.x.ToCultureInvariantString("F1"),
			", ",
			this.y.ToCultureInvariantString("F1"),
			", ",
			this.z.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x06009308 RID: 37640 RVA: 0x003A8B60 File Offset: 0x003A6D60
	public static Vector3d Cross(Vector3d _a, Vector3d _b)
	{
		return new Vector3d(_a.y * _b.z - _a.z * _b.y, _a.z * _b.x - _a.x * _b.z, _a.x * _b.y - _a.y * _b.x);
	}

	// Token: 0x04007060 RID: 28768
	public double x;

	// Token: 0x04007061 RID: 28769
	public double y;

	// Token: 0x04007062 RID: 28770
	public double z;

	// Token: 0x04007063 RID: 28771
	public static readonly Vector3d Zero = new Vector3d(0.0, 0.0, 0.0);
}
