using System;

// Token: 0x020011B0 RID: 4528
public struct IntVect
{
	// Token: 0x06008D9C RID: 36252 RVA: 0x0038E0DF File Offset: 0x0038C2DF
	public IntVect(int x, int y, int z)
	{
		this.m_X = x;
		this.m_Y = y;
		this.m_Z = z;
	}

	// Token: 0x17000EA8 RID: 3752
	// (get) Token: 0x06008D9D RID: 36253 RVA: 0x0038E0F6 File Offset: 0x0038C2F6
	public int X
	{
		get
		{
			return this.m_X;
		}
	}

	// Token: 0x17000EA9 RID: 3753
	// (get) Token: 0x06008D9E RID: 36254 RVA: 0x0038E0FE File Offset: 0x0038C2FE
	public int Y
	{
		get
		{
			return this.m_Y;
		}
	}

	// Token: 0x17000EAA RID: 3754
	// (get) Token: 0x06008D9F RID: 36255 RVA: 0x0038E106 File Offset: 0x0038C306
	public int Z
	{
		get
		{
			return this.m_Z;
		}
	}

	// Token: 0x06008DA0 RID: 36256 RVA: 0x0038E110 File Offset: 0x0038C310
	public override bool Equals(object obj)
	{
		IntVect intVect = (IntVect)obj;
		return intVect.X == this.m_X && intVect.Y == this.m_Y && intVect.Z == this.m_Z;
	}

	// Token: 0x06008DA1 RID: 36257 RVA: 0x0038E153 File Offset: 0x0038C353
	public override int GetHashCode()
	{
		return this.m_X * 8976890 + this.m_Y * 981131 + this.m_Z;
	}

	// Token: 0x06008DA2 RID: 36258 RVA: 0x0038E175 File Offset: 0x0038C375
	public static bool operator ==(IntVect one, IntVect other)
	{
		return one.X == other.X && one.Y == other.Y && one.Z == other.Z;
	}

	// Token: 0x06008DA3 RID: 36259 RVA: 0x0038E1A9 File Offset: 0x0038C3A9
	public static bool operator !=(IntVect one, IntVect other)
	{
		return !(one == other);
	}

	// Token: 0x04006DD3 RID: 28115
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_X;

	// Token: 0x04006DD4 RID: 28116
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_Y;

	// Token: 0x04006DD5 RID: 28117
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int m_Z;
}
