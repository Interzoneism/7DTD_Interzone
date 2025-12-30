using System;
using UnityEngine.Scripting;

// Token: 0x02000C48 RID: 3144
[Preserve]
public class XUiC_SlotSelector : XUiC_Selector
{
	// Token: 0x14000096 RID: 150
	// (add) Token: 0x06006094 RID: 24724 RVA: 0x00274020 File Offset: 0x00272220
	// (remove) Token: 0x06006095 RID: 24725 RVA: 0x00274058 File Offset: 0x00272258
	public event XUiEvent_SelectedSlotChanged OnSelectedSlotChanged;

	// Token: 0x06006096 RID: 24726 RVA: 0x0027408D File Offset: 0x0027228D
	public override void OnOpen()
	{
		base.OnOpen();
		this.currentValue.Text = ((EquipmentSlots)this.selectedIndex).ToStringCached<EquipmentSlots>();
	}

	// Token: 0x06006097 RID: 24727 RVA: 0x002740AC File Offset: 0x002722AC
	public override void BackPressed()
	{
		if (this.selectedIndex < 0)
		{
			this.selectedIndex = 7;
		}
		this.currentValue.Text = ((EquipmentSlots)this.selectedIndex).ToStringCached<EquipmentSlots>();
		if (this.OnSelectedSlotChanged != null)
		{
			this.OnSelectedSlotChanged(this.selectedIndex);
		}
	}

	// Token: 0x06006098 RID: 24728 RVA: 0x002740F8 File Offset: 0x002722F8
	public override void ForwardPressed()
	{
		if (this.selectedIndex >= 8)
		{
			this.selectedIndex = 0;
		}
		this.currentValue.Text = ((EquipmentSlots)this.selectedIndex).ToStringCached<EquipmentSlots>();
		if (this.OnSelectedSlotChanged != null)
		{
			this.OnSelectedSlotChanged(this.selectedIndex);
		}
	}
}
