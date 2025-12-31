using System;
using UnityEngine.Scripting;

// Token: 0x02000EA6 RID: 3750
[Preserve]
public class XUiC_WindowNonPagingHeader : XUiController
{
	// Token: 0x0600766B RID: 30315 RVA: 0x00303570 File Offset: 0x00301770
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("lblWindowName");
		if (childById != null)
		{
			this.lblWindowName = (XUiV_Label)childById.ViewComponent;
		}
	}

	// Token: 0x0600766C RID: 30316 RVA: 0x003035A3 File Offset: 0x003017A3
	public void SetHeader(string name)
	{
		if (this.lblWindowName != null)
		{
			this.lblWindowName.Text = name;
		}
	}

	// Token: 0x0600766D RID: 30317 RVA: 0x003035B9 File Offset: 0x003017B9
	public override void OnOpen()
	{
		base.OnOpen();
		XUiC_FocusedBlockHealth.SetData(base.xui.playerUI, null, 0f);
		base.xui.dragAndDrop.InMenu = true;
	}

	// Token: 0x0600766E RID: 30318 RVA: 0x003035E8 File Offset: 0x003017E8
	public override void OnClose()
	{
		base.OnClose();
		base.xui.dragAndDrop.InMenu = false;
		if (base.xui.currentSelectedEntry != null)
		{
			base.xui.currentSelectedEntry.Selected = false;
		}
	}

	// Token: 0x04005A56 RID: 23126
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblWindowName;
}
