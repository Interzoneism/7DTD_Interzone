using System;
using UnityEngine;

// Token: 0x020009D7 RID: 2519
public class ChunkVertexLayer : IMemoryPoolableObject
{
	// Token: 0x06004D30 RID: 19760 RVA: 0x001EA1B8 File Offset: 0x001E83B8
	public ChunkVertexLayer()
	{
		this.wPow = 4;
		this.hPow = 4;
		int num = 1 << this.wPow;
		int num2 = 1 << this.hPow;
		this.m_Vertices = new Vector3[num * num2];
		this.yPos = new float[num * num2];
		this.valid = new bool[num * num2];
	}

	// Token: 0x06004D31 RID: 19761 RVA: 0x001EA21C File Offset: 0x001E841C
	public void Reset()
	{
		for (int i = 0; i < this.valid.Length; i++)
		{
			this.m_Vertices[i] = Vector3.zero;
			this.yPos[i] = 0f;
			this.valid[i] = false;
		}
	}

	// Token: 0x06004D32 RID: 19762 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06004D33 RID: 19763 RVA: 0x001EA264 File Offset: 0x001E8464
	public bool getAt(int _x, int _y, out Vector3 _vec)
	{
		int offs = _x + (_y << this.wPow);
		return this.getAt(offs, out _vec);
	}

	// Token: 0x06004D34 RID: 19764 RVA: 0x001EA287 File Offset: 0x001E8487
	public bool getAt(int _offs, out Vector3 _vec)
	{
		_vec = this.m_Vertices[_offs];
		return this.valid[_offs];
	}

	// Token: 0x06004D35 RID: 19765 RVA: 0x001EA2A4 File Offset: 0x001E84A4
	public void setAt(int _x, int _y, Vector3 _v)
	{
		int num = _x + (_y << this.wPow);
		this.m_Vertices[num] = _v;
		this.valid[num] = true;
	}

	// Token: 0x06004D36 RID: 19766 RVA: 0x001EA2D8 File Offset: 0x001E84D8
	public bool getYPosAt(int _x, int _y, out float _ypos)
	{
		int offs = _x + (_y << this.wPow);
		return this.getYPosAt(offs, out _ypos);
	}

	// Token: 0x06004D37 RID: 19767 RVA: 0x001EA2FB File Offset: 0x001E84FB
	public bool getYPosAt(int _offs, out float _ypos)
	{
		_ypos = this.yPos[_offs];
		return this.valid[_offs];
	}

	// Token: 0x06004D38 RID: 19768 RVA: 0x001EA310 File Offset: 0x001E8510
	public void setYPosAt(int _x, int _y, float _ypos)
	{
		int num = _x + (_y << this.wPow);
		this.yPos[num] = _ypos;
		this.valid[num] = true;
	}

	// Token: 0x06004D39 RID: 19769 RVA: 0x001EA33D File Offset: 0x001E853D
	public void setInvalid(int _offs)
	{
		this.valid[_offs] = false;
	}

	// Token: 0x06004D3A RID: 19770 RVA: 0x001EA348 File Offset: 0x001E8548
	public int GetUsedMem()
	{
		return this.m_Vertices.Length * 13 + 8;
	}

	// Token: 0x04003B14 RID: 15124
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] m_Vertices;

	// Token: 0x04003B15 RID: 15125
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] yPos;

	// Token: 0x04003B16 RID: 15126
	[PublicizedFrom(EAccessModifier.Private)]
	public bool[] valid;

	// Token: 0x04003B17 RID: 15127
	[PublicizedFrom(EAccessModifier.Private)]
	public int wPow;

	// Token: 0x04003B18 RID: 15128
	[PublicizedFrom(EAccessModifier.Private)]
	public int hPow;

	// Token: 0x04003B19 RID: 15129
	public static int InstanceCount;
}
