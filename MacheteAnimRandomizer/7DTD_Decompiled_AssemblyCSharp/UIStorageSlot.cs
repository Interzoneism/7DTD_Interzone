using System;
using UnityEngine;

// Token: 0x0200003A RID: 58
[AddComponentMenu("NGUI/Examples/UI Storage Slot")]
public class UIStorageSlot : UIItemSlot
{
	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600013F RID: 319 RVA: 0x0000DC5D File Offset: 0x0000BE5D
	public override InvGameItem observedItem
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (!(this.storage != null))
			{
				return null;
			}
			return this.storage.GetItem(this.slot);
		}
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000DC80 File Offset: 0x0000BE80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override InvGameItem Replace(InvGameItem item)
	{
		if (!(this.storage != null))
		{
			return item;
		}
		return this.storage.Replace(this.slot, item);
	}

	// Token: 0x040001D4 RID: 468
	public UIItemStorage storage;

	// Token: 0x040001D5 RID: 469
	public int slot;
}
