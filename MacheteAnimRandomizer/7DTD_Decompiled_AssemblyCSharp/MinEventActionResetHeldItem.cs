using System;
using UnityEngine.Scripting;

// Token: 0x02000656 RID: 1622
[Preserve]
public class MinEventActionResetHeldItem : MinEventActionTargetedBase
{
	// Token: 0x0600313A RID: 12602 RVA: 0x0014F979 File Offset: 0x0014DB79
	public override void Execute(MinEventParams _params)
	{
		ItemActionData itemActionData = _params.ItemActionData;
		if (itemActionData == null)
		{
			return;
		}
		itemActionData.invData.item.OnHoldingReset(_params.ItemActionData.invData);
	}
}
