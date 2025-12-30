using System;
using UnityEngine.Scripting;

// Token: 0x02000CA1 RID: 3233
[Preserve]
public class XUiC_AssembleDroneWindow : XUiC_AssembleWindow
{
	// Token: 0x060063CD RID: 25549 RVA: 0x002874AB File Offset: 0x002856AB
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
	}

	// Token: 0x17000A2A RID: 2602
	// (set) Token: 0x060063CE RID: 25550 RVA: 0x002874BA File Offset: 0x002856BA
	public override ItemStack ItemStack
	{
		set
		{
			this.group.CurrentVehicleEntity.LoadMods();
			base.ItemStack = value;
		}
	}

	// Token: 0x060063CF RID: 25551 RVA: 0x002874D3 File Offset: 0x002856D3
	public override void OnChanged()
	{
		this.group.OnItemChanged(this.ItemStack);
		this.isDirty = true;
	}

	// Token: 0x04004B27 RID: 19239
	public XUiC_DroneWindowGroup group;
}
