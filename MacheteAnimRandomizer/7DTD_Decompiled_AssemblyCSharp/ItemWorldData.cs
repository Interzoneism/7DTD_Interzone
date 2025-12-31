using System;

// Token: 0x020004FB RID: 1275
public class ItemWorldData
{
	// Token: 0x06002993 RID: 10643 RVA: 0x0010F71C File Offset: 0x0010D91C
	public ItemWorldData(IGameManager _gm, ItemValue _itemValue, EntityItem _entityItem, int _belongsEntityId)
	{
		this.gameManager = _gm;
		this.world = _entityItem.world;
		this.entityItem = _entityItem;
		this.belongsEntityId = _belongsEntityId;
	}

	// Token: 0x04002069 RID: 8297
	public IGameManager gameManager;

	// Token: 0x0400206A RID: 8298
	public WorldBase world;

	// Token: 0x0400206B RID: 8299
	public EntityItem entityItem;

	// Token: 0x0400206C RID: 8300
	public int belongsEntityId;
}
