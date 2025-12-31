using System;
using System.Collections.Generic;

// Token: 0x02000A8C RID: 2700
public class PoiMapElement
{
	// Token: 0x0600534E RID: 21326 RVA: 0x002159CC File Offset: 0x00213BCC
	public PoiMapElement(uint _color, string _name, BlockValue _blockValue, BlockValue _blockBelow, int _iSO, int _ypos, int _yposFill, int _iST)
	{
		this.m_sModelName = _name;
		this.m_uColorId = _color;
		this.m_YPos = _ypos;
		this.m_YPosFill = _yposFill;
		this.m_BlockValue = _blockValue;
		this.m_BlockBelow = _blockBelow;
	}

	// Token: 0x0600534F RID: 21327 RVA: 0x00215A22 File Offset: 0x00213C22
	public PoiMapDecal GetDecal(int _index)
	{
		if (_index >= 0 && _index < this.decals.Count)
		{
			return this.decals[_index];
		}
		return null;
	}

	// Token: 0x06005350 RID: 21328 RVA: 0x00215A44 File Offset: 0x00213C44
	public PoiMapDecal GetRandomDecal(GameRandom _random)
	{
		for (int i = 0; i < this.decals.Count; i++)
		{
			PoiMapDecal poiMapDecal = this.decals[i];
			if (_random.RandomFloat < poiMapDecal.m_Prob)
			{
				return poiMapDecal;
			}
		}
		return null;
	}

	// Token: 0x06005351 RID: 21329 RVA: 0x00215A88 File Offset: 0x00213C88
	public PoiMapBlock GetRandomBlockOnTop(GameRandom _random)
	{
		for (int i = 0; i < this.blocksOnTop.Count; i++)
		{
			PoiMapBlock poiMapBlock = this.blocksOnTop[i];
			if (_random.RandomFloat < poiMapBlock.m_Prob)
			{
				return poiMapBlock;
			}
		}
		return null;
	}

	// Token: 0x04003F87 RID: 16263
	public uint m_uColorId;

	// Token: 0x04003F88 RID: 16264
	public string m_sModelName;

	// Token: 0x04003F89 RID: 16265
	public BlockValue m_BlockValue;

	// Token: 0x04003F8A RID: 16266
	public BlockValue m_BlockBelow;

	// Token: 0x04003F8B RID: 16267
	public int m_YPos;

	// Token: 0x04003F8C RID: 16268
	public int m_YPosFill;

	// Token: 0x04003F8D RID: 16269
	public List<PoiMapDecal> decals = new List<PoiMapDecal>();

	// Token: 0x04003F8E RID: 16270
	public List<PoiMapBlock> blocksOnTop = new List<PoiMapBlock>();
}
