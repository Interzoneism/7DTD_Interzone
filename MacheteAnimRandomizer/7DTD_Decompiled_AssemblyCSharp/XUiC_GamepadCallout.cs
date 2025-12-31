using System;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000CB8 RID: 3256
[Preserve]
public class XUiC_GamepadCallout : XUiController
{
	// Token: 0x17000A46 RID: 2630
	// (get) Token: 0x060064B1 RID: 25777 RVA: 0x0028C573 File Offset: 0x0028A773
	// (set) Token: 0x060064B2 RID: 25778 RVA: 0x0028C57B File Offset: 0x0028A77B
	public XUiV_Sprite icon { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000A47 RID: 2631
	// (get) Token: 0x060064B3 RID: 25779 RVA: 0x0028C584 File Offset: 0x0028A784
	// (set) Token: 0x060064B4 RID: 25780 RVA: 0x0028C58C File Offset: 0x0028A78C
	public XUiV_Label action { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060064B5 RID: 25781 RVA: 0x0028C598 File Offset: 0x0028A798
	public override void Init()
	{
		base.Init();
		this.icon = (XUiV_Sprite)base.GetChildById("icon").ViewComponent;
		this.action = (XUiV_Label)base.GetChildById("action").ViewComponent;
		this.icon.UIAtlas = UIUtils.IconAtlas.name;
	}

	// Token: 0x060064B6 RID: 25782 RVA: 0x0028C5F6 File Offset: 0x0028A7F6
	public void SetupCallout(UIUtils.ButtonIcon _icon, string _action)
	{
		this.icon.SpriteName = UIUtils.GetSpriteName(_icon);
		this.action.Text = Localization.Get(_action, false);
	}
}
