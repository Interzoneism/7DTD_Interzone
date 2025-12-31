using System;
using UnityEngine.Scripting;

// Token: 0x020005FF RID: 1535
[Preserve]
public class IsSecondaryAttack : TargetedCompareRequirementBase
{
	// Token: 0x06003001 RID: 12289 RVA: 0x0014766C File Offset: 0x0014586C
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.target == null || this.target.inventory.holdingItemItemValue.ItemClass.Actions[1] == null)
		{
			return false;
		}
		if (this.invert)
		{
			return !this.target.inventory.holdingItemItemValue.ItemClass.Actions[1].IsActionRunning(this.target.inventory.GetItemActionDataInSlot(this.target.inventory.holdingItemIdx, 1));
		}
		return this.target.inventory.holdingItemItemValue.ItemClass.Actions[1].IsActionRunning(this.target.inventory.GetItemActionDataInSlot(this.target.inventory.holdingItemIdx, 1));
	}
}
