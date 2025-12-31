using System;
using System.Runtime.CompilerServices;

// Token: 0x02001252 RID: 4690
public struct Vector3b : IEquatable<Vector3b>
{
	// Token: 0x060092F3 RID: 37619 RVA: 0x003A8861 File Offset: 0x003A6A61
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3b(byte _x, byte _y, byte _z)
	{
		this.x = _x;
		this.y = _y;
		this.z = _z;
	}

	// Token: 0x060092F4 RID: 37620 RVA: 0x003A8878 File Offset: 0x003A6A78
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3b(int _x, int _y, int _z)
	{
		this.x = (byte)_x;
		this.y = (byte)_y;
		this.z = (byte)_z;
	}

	// Token: 0x060092F5 RID: 37621 RVA: 0x003A8892 File Offset: 0x003A6A92
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3i ToVector3i()
	{
		return new Vector3i((int)this.x, (int)this.y, (int)this.z);
	}

	// Token: 0x060092F6 RID: 37622 RVA: 0x003A88AB File Offset: 0x003A6AAB
	public override int GetHashCode()
	{
		return (int)this.x << 16 | (int)this.y << 8 | (int)this.z;
	}

	// Token: 0x060092F7 RID: 37623 RVA: 0x003A88C6 File Offset: 0x003A6AC6
	public override bool Equals(object obj)
	{
		return obj != null && obj is Vector3b && this.Equals((Vector3b)obj);
	}

	// Token: 0x060092F8 RID: 37624 RVA: 0x003A88E3 File Offset: 0x003A6AE3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector3b other)
	{
		return this.x == other.x && this.y == other.y && this.z == other.z;
	}

	// Token: 0x0400705D RID: 28765
	public byte x;

	// Token: 0x0400705E RID: 28766
	public byte y;

	// Token: 0x0400705F RID: 28767
	public byte z;
}
