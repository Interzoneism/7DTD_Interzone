using System;
using UnityEngine.Scripting;

// Token: 0x02000516 RID: 1302
[Preserve]
public class ItemActionData
{
	// Token: 0x06002A6D RID: 10861 RVA: 0x00116C7C File Offset: 0x00114E7C
	public ItemActionData(ItemInventoryData _inventoryData, int _indexInEntityOfAction)
	{
		this.invData = _inventoryData;
		this.indexInEntityOfAction = _indexInEntityOfAction;
		this.ActionTags = FastTags<TagGroup.Global>.Parse((_indexInEntityOfAction == 0) ? "primary" : ((_indexInEntityOfAction == 1) ? "secondary" : "action2"));
		this.EventParms = new MinEventParams();
		this.hitInfo = new WorldRayHitInfo();
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x00116CD8 File Offset: 0x00114ED8
	public WorldRayHitInfo GetUpdatedHitInfo()
	{
		this.hitInfo.CopyFrom(Voxel.voxelRayHitInfo);
		return this.hitInfo;
	}

	// Token: 0x04002115 RID: 8469
	public ItemInventoryData invData;

	// Token: 0x04002116 RID: 8470
	public float lastUseTime;

	// Token: 0x04002117 RID: 8471
	public int indexInEntityOfAction;

	// Token: 0x04002118 RID: 8472
	public bool bWaitForRelease;

	// Token: 0x04002119 RID: 8473
	public ItemActionAttack.AttackHitInfo attackDetails;

	// Token: 0x0400211A RID: 8474
	public FastTags<TagGroup.Global> ActionTags;

	// Token: 0x0400211B RID: 8475
	public bool HasExecuted;

	// Token: 0x0400211C RID: 8476
	public bool uiOpenedByMe;

	// Token: 0x0400211D RID: 8477
	public MinEventParams EventParms;

	// Token: 0x0400211E RID: 8478
	public WorldRayHitInfo hitInfo;
}
