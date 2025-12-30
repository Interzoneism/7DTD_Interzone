using System;
using UnityEngine.Scripting;

// Token: 0x02000EB3 RID: 3763
[Preserve]
public class XUiC_WorkstationOutputGrid : XUiC_WorkstationGrid
{
	// Token: 0x06007710 RID: 30480 RVA: 0x003079F1 File Offset: 0x00305BF1
	public void UpdateData(ItemStack[] stackList)
	{
		this.UpdateBackend(stackList);
	}

	// Token: 0x06007711 RID: 30481 RVA: 0x003079FA File Offset: 0x00305BFA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.UpdateBackend(stackList);
		this.workstationData.SetOutputStacks(stackList);
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}
}
