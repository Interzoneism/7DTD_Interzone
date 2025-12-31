using System;
using Unity.Mathematics;

// Token: 0x02000B5C RID: 2908
public struct WaterVoxelState : IEquatable<WaterVoxelState>
{
	// Token: 0x06005A72 RID: 23154 RVA: 0x00244897 File Offset: 0x00242A97
	public WaterVoxelState(byte stateBits)
	{
		this.stateBits = stateBits;
	}

	// Token: 0x06005A73 RID: 23155 RVA: 0x002448A0 File Offset: 0x00242AA0
	public WaterVoxelState(WaterVoxelState other)
	{
		this.stateBits = other.stateBits;
	}

	// Token: 0x06005A74 RID: 23156 RVA: 0x002448AE File Offset: 0x00242AAE
	public bool IsDefault()
	{
		return this.stateBits == 0;
	}

	// Token: 0x06005A75 RID: 23157 RVA: 0x002448B9 File Offset: 0x00242AB9
	public bool IsSolidYPos()
	{
		return (this.stateBits & 1) > 0;
	}

	// Token: 0x06005A76 RID: 23158 RVA: 0x002448C6 File Offset: 0x00242AC6
	public bool IsSolidYNeg()
	{
		return (this.stateBits & 2) > 0;
	}

	// Token: 0x06005A77 RID: 23159 RVA: 0x002448D3 File Offset: 0x00242AD3
	public bool IsSolidXPos()
	{
		return (this.stateBits & 32) > 0;
	}

	// Token: 0x06005A78 RID: 23160 RVA: 0x002448E1 File Offset: 0x00242AE1
	public bool IsSolidXNeg()
	{
		return (this.stateBits & 8) > 0;
	}

	// Token: 0x06005A79 RID: 23161 RVA: 0x002448EE File Offset: 0x00242AEE
	public bool IsSolidZPos()
	{
		return (this.stateBits & 4) > 0;
	}

	// Token: 0x06005A7A RID: 23162 RVA: 0x002448FB File Offset: 0x00242AFB
	public bool IsSolidZNeg()
	{
		return (this.stateBits & 16) > 0;
	}

	// Token: 0x06005A7B RID: 23163 RVA: 0x0024490C File Offset: 0x00242B0C
	public bool IsSolidXZ(int2 side)
	{
		if (side.x > 0)
		{
			return this.IsSolidXPos();
		}
		if (side.x < 0)
		{
			return this.IsSolidXNeg();
		}
		if (side.y > 0)
		{
			return this.IsSolidZPos();
		}
		if (side.y < 0)
		{
			return this.IsSolidZNeg();
		}
		return this.IsSolid();
	}

	// Token: 0x06005A7C RID: 23164 RVA: 0x0024495F File Offset: 0x00242B5F
	public bool IsSolid()
	{
		return this.stateBits != 0 && (~this.stateBits & 63) == 0;
	}

	// Token: 0x06005A7D RID: 23165 RVA: 0x00244978 File Offset: 0x00242B78
	public void SetSolid(BlockFaceFlag flags)
	{
		this.stateBits = (byte)flags;
	}

	// Token: 0x06005A7E RID: 23166 RVA: 0x00244982 File Offset: 0x00242B82
	public void SetSolidMask(BlockFaceFlag mask, bool value)
	{
		if (value)
		{
			this.stateBits |= (byte)mask;
			return;
		}
		this.stateBits &= (byte)(~(byte)mask);
	}

	// Token: 0x06005A7F RID: 23167 RVA: 0x002449A9 File Offset: 0x00242BA9
	public bool Equals(WaterVoxelState other)
	{
		return this.stateBits == other.stateBits;
	}

	// Token: 0x0400452F RID: 17711
	[PublicizedFrom(EAccessModifier.Private)]
	public byte stateBits;
}
