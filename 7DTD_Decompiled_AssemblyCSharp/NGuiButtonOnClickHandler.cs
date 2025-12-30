using System;
using UnityEngine;

// Token: 0x02001034 RID: 4148
public class NGuiButtonOnClickHandler : MonoBehaviour
{
	// Token: 0x0600834B RID: 33611 RVA: 0x0034F1D1 File Offset: 0x0034D3D1
	public virtual void OnClick()
	{
		if (this.OnClickDelegate != null)
		{
			this.OnClickDelegate.NGuiButtonOnClick(base.transform);
		}
	}

	// Token: 0x0600834C RID: 33612 RVA: 0x0034F1EC File Offset: 0x0034D3EC
	public virtual void OnDoubleClick()
	{
		if (this.OnDoubleClickDelegate != null)
		{
			this.OnDoubleClickDelegate.NGuiButtonOnDoubleClick(base.transform);
		}
	}

	// Token: 0x0600834D RID: 33613 RVA: 0x0034F207 File Offset: 0x0034D407
	public void OnHover(bool _isOver)
	{
		if (this.OnHoverDelegate != null)
		{
			this.OnHoverDelegate.NGuiButtonOnHover(base.transform, _isOver);
		}
	}

	// Token: 0x0600834E RID: 33614 RVA: 0x0034F223 File Offset: 0x0034D423
	public void OnIsHeld()
	{
		if (this.OnIsHeldDelegate != null)
		{
			this.OnIsHeldDelegate.NGuiButtonOnIsHeld(base.transform);
		}
	}

	// Token: 0x0400653A RID: 25914
	public INGuiButtonOnClick OnClickDelegate;

	// Token: 0x0400653B RID: 25915
	public INGuiButtonOnDoubleClick OnDoubleClickDelegate;

	// Token: 0x0400653C RID: 25916
	public INGuiButtonOnHover OnHoverDelegate;

	// Token: 0x0400653D RID: 25917
	public INGuiButtonOnIsHeld OnIsHeldDelegate;
}
