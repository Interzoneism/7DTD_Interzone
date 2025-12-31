using System;
using UnityEngine.Scripting;

// Token: 0x02000DF5 RID: 3573
[Preserve]
public class XUiC_SelectableEntry : XUiController
{
	// Token: 0x17000B46 RID: 2886
	// (get) Token: 0x06007005 RID: 28677 RVA: 0x002DB836 File Offset: 0x002D9A36
	// (set) Token: 0x06007006 RID: 28678 RVA: 0x002DB840 File Offset: 0x002D9A40
	public new bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			if (value)
			{
				if (base.xui.currentSelectedEntry != null)
				{
					base.xui.currentSelectedEntry.SelectedChanged(false);
					base.xui.currentSelectedEntry.selected = false;
				}
			}
			else if (base.xui.currentSelectedEntry == this)
			{
				base.xui.currentSelectedEntry.SelectedChanged(false);
				base.xui.currentSelectedEntry.selected = false;
				base.xui.currentSelectedEntry = null;
			}
			this.selected = value;
			if (this.selected)
			{
				base.xui.currentSelectedEntry = this;
			}
			this.SelectedChanged(this.selected);
		}
	}

	// Token: 0x06007007 RID: 28679 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SelectedChanged(bool isSelected)
	{
	}

	// Token: 0x04005522 RID: 21794
	[PublicizedFrom(EAccessModifier.Private)]
	public bool selected;
}
