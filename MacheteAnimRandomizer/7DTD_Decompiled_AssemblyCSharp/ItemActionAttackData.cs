using System;
using UnityEngine.Scripting;

// Token: 0x02000517 RID: 1303
[Preserve]
public class ItemActionAttackData : ItemActionData
{
	// Token: 0x06002A6F RID: 10863 RVA: 0x00116CF0 File Offset: 0x00114EF0
	public ItemActionAttackData(ItemInventoryData _inventoryData, int _indexInEntityOfAction) : base(_inventoryData, _indexInEntityOfAction)
	{
		this.attackDetails = new ItemActionAttack.AttackHitInfo();
	}

	// Token: 0x0400211F RID: 8479
	public ItemActionAttackData.HitDelegate hitDelegate;

	// Token: 0x02000518 RID: 1304
	// (Invoke) Token: 0x06002A71 RID: 10865
	public delegate WorldRayHitInfo HitDelegate(out float damageScale);
}
