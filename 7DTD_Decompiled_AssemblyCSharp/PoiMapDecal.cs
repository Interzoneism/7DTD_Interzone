using System;

// Token: 0x02000A8A RID: 2698
public class PoiMapDecal
{
	// Token: 0x0600534C RID: 21324 RVA: 0x00215990 File Offset: 0x00213B90
	public PoiMapDecal(int _texIndex, BlockFace _face, float _prob)
	{
		this.textureIndex = _texIndex;
		this.face = _face;
		this.m_Prob = _prob;
	}

	// Token: 0x04003F81 RID: 16257
	public int textureIndex;

	// Token: 0x04003F82 RID: 16258
	public BlockFace face;

	// Token: 0x04003F83 RID: 16259
	public float m_Prob;
}
