using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D90 RID: 3472
[Preserve]
public class XUiC_PrefabEditorHelp : XUiController
{
	// Token: 0x06006CB6 RID: 27830 RVA: 0x002C70B0 File Offset: 0x002C52B0
	public override void Init()
	{
		base.Init();
		XUiC_PrefabEditorHelp.ID = base.WindowGroup.ID;
		base.GetChildById("outclick").OnPress += this.Close_OnPress;
		this.findLabels(this);
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006CB7 RID: 27831 RVA: 0x002C70FC File Offset: 0x002C52FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Close_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(XUiC_PrefabEditorHelp.ID);
	}

	// Token: 0x06006CB8 RID: 27832 RVA: 0x002C7118 File Offset: 0x002C5318
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		foreach (XUiV_Label xuiV_Label in this.labels)
		{
			xuiV_Label.ForceTextUpdate();
		}
	}

	// Token: 0x06006CB9 RID: 27833 RVA: 0x002C7170 File Offset: 0x002C5370
	[PublicizedFrom(EAccessModifier.Private)]
	public void findLabels(XUiController _controller)
	{
		foreach (XUiController xuiController in _controller.Children)
		{
			XUiV_Label xuiV_Label = xuiController.ViewComponent as XUiV_Label;
			if (xuiV_Label != null)
			{
				this.labels.Add(xuiV_Label);
			}
			this.findLabels(xuiController);
		}
	}

	// Token: 0x040052BF RID: 21183
	public static string ID = "";

	// Token: 0x040052C0 RID: 21184
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiV_Label> labels = new List<XUiV_Label>();
}
