using System;
using System.IO;

// Token: 0x02000995 RID: 2453
public class BlockChangeInfo
{
	// Token: 0x060049CC RID: 18892 RVA: 0x001D3919 File Offset: 0x001D1B19
	public BlockChangeInfo()
	{
		this.pos = Vector3i.zero;
		this.blockValue = BlockValue.Air;
		this.density = MarchingCubes.DensityAir;
		this.changedByEntityId = -1;
	}

	// Token: 0x060049CD RID: 18893 RVA: 0x001D3950 File Offset: 0x001D1B50
	public BlockChangeInfo(int _clrIdx, Vector3i _pos, BlockValue _blockValue)
	{
		this.pos = _pos;
		this.blockValue = _blockValue;
		this.bChangeBlockValue = true;
		this.bUpdateLight = false;
		this.clrIdx = _clrIdx;
	}

	// Token: 0x060049CE RID: 18894 RVA: 0x001D3982 File Offset: 0x001D1B82
	public BlockChangeInfo(Vector3i _blockPos, BlockValue _blockValue, bool _updateLight, bool _bOnlyDamage = false) : this(0, _blockPos, _blockValue, _updateLight)
	{
		this.bChangeDamage = _bOnlyDamage;
	}

	// Token: 0x060049CF RID: 18895 RVA: 0x001D3996 File Offset: 0x001D1B96
	public BlockChangeInfo(int _clrIdx, Vector3i _pos, BlockValue _blockValue, int _changedEntityId) : this(_clrIdx, _pos, _blockValue)
	{
		this.changedByEntityId = _changedEntityId;
	}

	// Token: 0x060049D0 RID: 18896 RVA: 0x001D39A9 File Offset: 0x001D1BA9
	public BlockChangeInfo(Vector3i _blockPos, BlockValue _blockValue, sbyte _density) : this(0, _blockPos, _blockValue, _density)
	{
	}

	// Token: 0x060049D1 RID: 18897 RVA: 0x001D39B5 File Offset: 0x001D1BB5
	public BlockChangeInfo(int _x, int _y, int _z, BlockValue _blockValue, bool _updateLight) : this(0, new Vector3i(_x, _y, _z), _blockValue, _updateLight)
	{
	}

	// Token: 0x060049D2 RID: 18898 RVA: 0x001D39CA File Offset: 0x001D1BCA
	public BlockChangeInfo(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _updateLight) : this(_clrIdx, _blockPos, _blockValue)
	{
		this.bUpdateLight = _updateLight;
	}

	// Token: 0x060049D3 RID: 18899 RVA: 0x001D39DD File Offset: 0x001D1BDD
	public BlockChangeInfo(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _updateLight, int _changingEntityId) : this(_clrIdx, _blockPos, _blockValue, _updateLight)
	{
		this.changedByEntityId = _changingEntityId;
	}

	// Token: 0x060049D4 RID: 18900 RVA: 0x001D39F2 File Offset: 0x001D1BF2
	public BlockChangeInfo(int _clrIdx, Vector3i _blockPos, sbyte _density, bool _bForceDensityChange = false)
	{
		this.clrIdx = _clrIdx;
		this.pos = _blockPos;
		this.density = _density;
		this.bChangeDensity = true;
		this.bForceDensityChange = _bForceDensityChange;
		this.changedByEntityId = -1;
	}

	// Token: 0x060049D5 RID: 18901 RVA: 0x001D3A2C File Offset: 0x001D1C2C
	public BlockChangeInfo(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density)
	{
		this.pos = _blockPos;
		this.blockValue = _blockValue;
		this.bChangeBlockValue = true;
		this.density = _density;
		this.bChangeDensity = true;
		this.bUpdateLight = true;
		this.clrIdx = _clrIdx;
		this.changedByEntityId = -1;
	}

	// Token: 0x060049D6 RID: 18902 RVA: 0x001D3A7F File Offset: 0x001D1C7F
	public BlockChangeInfo(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density, int _changedByEntityId) : this(_clrIdx, _blockPos, _blockValue, _density)
	{
		this.changedByEntityId = _changedByEntityId;
	}

	// Token: 0x060049D7 RID: 18903 RVA: 0x001D3A94 File Offset: 0x001D1C94
	public BlockChangeInfo(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density, TextureFullArray _tex) : this(_clrIdx, _blockPos, _blockValue, _density)
	{
		this.bChangeTexture = true;
		this.textureFull = _tex;
		this.changedByEntityId = -1;
	}

	// Token: 0x060049D8 RID: 18904 RVA: 0x001D3AB8 File Offset: 0x001D1CB8
	public override bool Equals(object other)
	{
		BlockChangeInfo blockChangeInfo = other as BlockChangeInfo;
		return blockChangeInfo != null && (this.pos.Equals(blockChangeInfo.pos) && this.blockValue.type == blockChangeInfo.blockValue.type) && this.density == blockChangeInfo.density;
	}

	// Token: 0x060049D9 RID: 18905 RVA: 0x001D3B0C File Offset: 0x001D1D0C
	public void Read(BinaryReader _br)
	{
		this.clrIdx = (int)_br.ReadByte();
		this.pos = StreamUtils.ReadVector3i(_br);
		this.changedByEntityId = _br.ReadInt32();
		int num = (int)_br.ReadByte();
		this.bChangeBlockValue = ((num & 1) != 0);
		this.bChangeDensity = ((num & 2) != 0);
		this.bUpdateLight = ((num & 4) != 0);
		this.bChangeDamage = ((num & 8) != 0);
		this.bChangeTexture = ((num & 16) != 0);
		if (this.bChangeBlockValue)
		{
			this.blockValue.rawData = _br.ReadUInt32();
			this.blockValue.damage = (int)_br.ReadUInt16();
		}
		if (this.bChangeDensity)
		{
			this.density = _br.ReadSByte();
			this.bForceDensityChange = _br.ReadBoolean();
		}
		if (this.bChangeTexture)
		{
			this.textureFull.Read(_br, 1);
		}
	}

	// Token: 0x060049DA RID: 18906 RVA: 0x001D3BE0 File Offset: 0x001D1DE0
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((byte)this.clrIdx);
		StreamUtils.Write(_bw, this.pos);
		_bw.Write(this.changedByEntityId);
		int num = this.bChangeBlockValue ? 1 : 0;
		num |= (this.bChangeDensity ? 2 : 0);
		num |= (this.bUpdateLight ? 4 : 0);
		num |= (this.bChangeDamage ? 8 : 0);
		num |= (this.bChangeTexture ? 16 : 0);
		_bw.Write((byte)num);
		if (this.bChangeBlockValue)
		{
			_bw.Write(this.blockValue.rawData);
			_bw.Write((ushort)this.blockValue.damage);
		}
		if (this.bChangeDensity)
		{
			_bw.Write(this.density);
			_bw.Write(this.bForceDensityChange);
		}
		if (this.bChangeTexture)
		{
			this.textureFull.Write(_bw);
		}
	}

	// Token: 0x060049DB RID: 18907 RVA: 0x001D3CC3 File Offset: 0x001D1EC3
	public override int GetHashCode()
	{
		return this.pos.GetHashCode();
	}

	// Token: 0x060049DC RID: 18908 RVA: 0x001D3CD6 File Offset: 0x001D1ED6
	public static bool operator ==(BlockChangeInfo point1, BlockChangeInfo point2)
	{
		return (point1 == null && point2 == null) || point1.Equals(point2);
	}

	// Token: 0x060049DD RID: 18909 RVA: 0x001D3CE7 File Offset: 0x001D1EE7
	public static bool operator !=(BlockChangeInfo point1, BlockChangeInfo point2)
	{
		return !(point1 == point2);
	}

	// Token: 0x0400390C RID: 14604
	public static BlockChangeInfo Empty = new BlockChangeInfo();

	// Token: 0x0400390D RID: 14605
	public int clrIdx;

	// Token: 0x0400390E RID: 14606
	public Vector3i pos;

	// Token: 0x0400390F RID: 14607
	public bool bChangeBlockValue;

	// Token: 0x04003910 RID: 14608
	public bool bChangeDamage;

	// Token: 0x04003911 RID: 14609
	public BlockValue blockValue;

	// Token: 0x04003912 RID: 14610
	public bool bChangeDensity;

	// Token: 0x04003913 RID: 14611
	public bool bForceDensityChange;

	// Token: 0x04003914 RID: 14612
	public sbyte density;

	// Token: 0x04003915 RID: 14613
	public bool bUpdateLight;

	// Token: 0x04003916 RID: 14614
	public bool bChangeTexture;

	// Token: 0x04003917 RID: 14615
	public TextureFullArray textureFull;

	// Token: 0x04003918 RID: 14616
	public int changedByEntityId = -1;
}
