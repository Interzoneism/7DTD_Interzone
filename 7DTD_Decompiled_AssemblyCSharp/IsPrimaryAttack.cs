using System;
using UnityEngine.Scripting;

// Token: 0x020005FE RID: 1534
[Preserve]
public class IsPrimaryAttack : TargetedCompareRequirementBase
{
	// Token: 0x06002FFF RID: 12287 RVA: 0x00147594 File Offset: 0x00145794
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.target == null || this.target.inventory.holdingItemItemValue.ItemClass.Actions[0] == null)
		{
			return false;
		}
		if (this.invert)
		{
			return !this.target.inventory.holdingItemItemValue.ItemClass.Actions[0].IsActionRunning(this.target.inventory.GetItemActionDataInSlot(this.target.inventory.holdingItemIdx, 0));
		}
		return this.target.inventory.holdingItemItemValue.ItemClass.Actions[0].IsActionRunning(this.target.inventory.GetItemActionDataInSlot(this.target.inventory.holdingItemIdx, 0));
	}
}
