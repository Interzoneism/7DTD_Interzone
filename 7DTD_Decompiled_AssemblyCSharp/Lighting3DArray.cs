using System;

// Token: 0x020009E5 RID: 2533
public class Lighting3DArray : Array3DWithOffset<Lighting>
{
	// Token: 0x06004DA0 RID: 19872 RVA: 0x001EA776 File Offset: 0x001E8976
	public Lighting3DArray() : base(3, 3, 3)
	{
	}

	// Token: 0x06004DA1 RID: 19873 RVA: 0x001EA781 File Offset: 0x001E8981
	public void SetBlockCache(INeighborBlockCache _nBlocks)
	{
		this.nBlocks = _nBlocks;
	}

	// Token: 0x06004DA2 RID: 19874 RVA: 0x001EA78C File Offset: 0x001E898C
	public void SetPosition(Vector3i _blockPos)
	{
		this.stab = this.nBlocks.GetChunk(0, 0).GetStability(_blockPos.x, _blockPos.y, _blockPos.z);
		if (this.blockPos.x != _blockPos.x || this.blockPos.z != _blockPos.z || this.blockPos.y != _blockPos.y + 1)
		{
			this.available = 0;
		}
		else
		{
			int num = 26;
			for (int i = 0; i < 18; i++)
			{
				this.data[num] = this.data[num - 9];
				num--;
			}
			this.available <<= 9;
		}
		this.blockPos = _blockPos;
	}

	// Token: 0x06004DA3 RID: 19875 RVA: 0x001EA84C File Offset: 0x001E8A4C
	[PublicizedFrom(EAccessModifier.Private)]
	public Lighting GetLight(IChunk _c, int _x, int _y, int _z)
	{
		if (_c == null)
		{
			return Lighting.one;
		}
		_x &= 15;
		_z &= 15;
		return new Lighting(_c.GetLight(_x, _y, _z, Chunk.LIGHT_TYPE.SUN), 0, this.stab);
	}

	// Token: 0x170007FB RID: 2043
	public override Lighting this[int _x, int _y, int _z]
	{
		get
		{
			int index = base.GetIndex(_x, _y, _z);
			int num = 1 << index;
			if ((this.available & num) == 0)
			{
				this.data[index] = this.GetLight(this.nBlocks.GetChunk(_x, _z), this.blockPos.x + _x, this.blockPos.y + _y, this.blockPos.z + _z);
				this.available |= num;
			}
			return this.data[index];
		}
		set
		{
			int index = base.GetIndex(_x, _y, _z);
			this.data[index] = value;
			this.available |= 1 << index;
		}
	}

	// Token: 0x04003B33 RID: 15155
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSize = 3;

	// Token: 0x04003B34 RID: 15156
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSize2d = 9;

	// Token: 0x04003B35 RID: 15157
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSize3d = 27;

	// Token: 0x04003B36 RID: 15158
	[PublicizedFrom(EAccessModifier.Private)]
	public int available;

	// Token: 0x04003B37 RID: 15159
	[PublicizedFrom(EAccessModifier.Private)]
	public INeighborBlockCache nBlocks;

	// Token: 0x04003B38 RID: 15160
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04003B39 RID: 15161
	[PublicizedFrom(EAccessModifier.Private)]
	public byte stab;
}
