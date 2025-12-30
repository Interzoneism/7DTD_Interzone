using System;

// Token: 0x02000B0C RID: 2828
public class TileEntitySecureDoor : TileEntitySecure
{
	// Token: 0x0600579F RID: 22431 RVA: 0x0023888F File Offset: 0x00236A8F
	public TileEntitySecureDoor(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x060057A0 RID: 22432 RVA: 0x0018853A File Offset: 0x0018673A
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.SecureDoor;
	}
}
