using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
[AddComponentMenu("NGUI/Examples/UI Equipment Slot")]
public class UIEquipmentSlot : UIItemSlot
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x0600012E RID: 302 RVA: 0x0000D624 File Offset: 0x0000B824
	public override InvGameItem observedItem
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (!(this.equipment != null))
			{
				return null;
			}
			return this.equipment.GetItem(this.slot);
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x0000D647 File Offset: 0x0000B847
	[PublicizedFrom(EAccessModifier.Protected)]
	public override InvGameItem Replace(InvGameItem item)
	{
		if (!(this.equipment != null))
		{
			return item;
		}
		return this.equipment.Replace(this.slot, item);
	}

	// Token: 0x040001C1 RID: 449
	public InvEquipment equipment;

	// Token: 0x040001C2 RID: 450
	public InvBaseItem.Slot slot;
}
