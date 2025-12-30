using System;
using UnityEngine.Scripting;

// Token: 0x02000D06 RID: 3334
[Preserve]
public class XUiC_LoginProgress : XUiController
{
	// Token: 0x060067B4 RID: 26548 RVA: 0x002A0D64 File Offset: 0x0029EF64
	public override void Init()
	{
		base.Init();
		XUiC_LoginProgress.ID = base.WindowGroup.ID;
		XUiV_Label label = base.GetChildById("lblText").ViewComponent as XUiV_Label;
		this.textAnimator = new TextEllipsisAnimator(string.Empty, label);
	}

	// Token: 0x060067B5 RID: 26549 RVA: 0x002A0DAE File Offset: 0x0029EFAE
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.ViewComponent.IsVisible)
		{
			this.textAnimator.GetNextAnimatedString(_dt);
		}
	}

	// Token: 0x060067B6 RID: 26550 RVA: 0x002A0DD0 File Offset: 0x0029EFD0
	public static void Open(XUi xui, string message)
	{
		XUiC_LoginProgress childByType = xui.FindWindowGroupByName(XUiC_LoginProgress.ID).GetChildByType<XUiC_LoginProgress>();
		xui.playerUI.windowManager.Open(XUiC_LoginProgress.ID, true, true, true);
		childByType.textAnimator.SetBaseString(message + "...", TextEllipsisAnimator.AnimationMode.All);
	}

	// Token: 0x04004E40 RID: 20032
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = string.Empty;

	// Token: 0x04004E41 RID: 20033
	[PublicizedFrom(EAccessModifier.Private)]
	public TextEllipsisAnimator textAnimator;
}
