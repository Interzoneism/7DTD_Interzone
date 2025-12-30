using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004F4 RID: 1268
public class ItemInventoryData
{
	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x0600293E RID: 10558 RVA: 0x0010D600 File Offset: 0x0010B800
	public Transform model
	{
		get
		{
			return this.holdingEntity.inventory.models[this.slotIdx];
		}
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x0600293F RID: 10559 RVA: 0x0010D619 File Offset: 0x0010B819
	// (set) Token: 0x06002940 RID: 10560 RVA: 0x0010D631 File Offset: 0x0010B831
	public ItemValue itemValue
	{
		get
		{
			return this.holdingEntity.inventory[this.slotIdx];
		}
		set
		{
			this.holdingEntity.inventory[this.slotIdx] = value;
		}
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x0010D64A File Offset: 0x0010B84A
	public void Changed()
	{
		this.holdingEntity.inventory.Changed();
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x0010D65C File Offset: 0x0010B85C
	public ItemInventoryData(ItemClass _item, ItemStack _itemStack, IGameManager _gameManager, EntityAlive _holdingEntity, int _slotIdx)
	{
		this.item = _item;
		this.itemStack = _itemStack;
		this.world = _holdingEntity.world;
		this.holdingEntity = _holdingEntity;
		this.gameManager = _gameManager;
		this.slotIdx = _slotIdx;
		this.hitInfo = new WorldRayHitInfo();
		this.actionData = new List<ItemActionData>();
		this.actionData.Add(null);
		this.actionData.Add(null);
	}

	// Token: 0x0400203A RID: 8250
	public ItemClass item;

	// Token: 0x0400203B RID: 8251
	public ItemStack itemStack;

	// Token: 0x0400203C RID: 8252
	public readonly EntityAlive holdingEntity;

	// Token: 0x0400203D RID: 8253
	public int holdingEntitySoundID = -2;

	// Token: 0x0400203E RID: 8254
	public World world;

	// Token: 0x0400203F RID: 8255
	public IGameManager gameManager;

	// Token: 0x04002040 RID: 8256
	public List<ItemActionData> actionData;

	// Token: 0x04002041 RID: 8257
	public WorldRayHitInfo hitInfo;

	// Token: 0x04002042 RID: 8258
	public int slotIdx;

	// Token: 0x020004F5 RID: 1269
	public enum SoundPlayType
	{
		// Token: 0x04002044 RID: 8260
		None = -2,
		// Token: 0x04002045 RID: 8261
		IdleReady,
		// Token: 0x04002046 RID: 8262
		Idle
	}
}
