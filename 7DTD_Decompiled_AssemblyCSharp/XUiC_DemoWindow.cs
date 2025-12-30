using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C87 RID: 3207
[Preserve]
public class XUiC_DemoWindow : XUiController
{
	// Token: 0x060062E4 RID: 25316 RVA: 0x00282F5A File Offset: 0x0028115A
	public override void Init()
	{
		base.Init();
		XUiC_DemoWindow.ID = base.WindowGroup.ID;
	}

	// Token: 0x060062E5 RID: 25317 RVA: 0x00282F72 File Offset: 0x00281172
	public override void OnOpen()
	{
		base.OnOpen();
		GameManager.Instance.Pause(true);
		base.RefreshBindings(false);
	}

	// Token: 0x060062E6 RID: 25318 RVA: 0x00282F8C File Offset: 0x0028118C
	public override void OnClose()
	{
		base.OnClose();
		GameManager.Instance.Pause(false);
	}

	// Token: 0x060062E7 RID: 25319 RVA: 0x00272183 File Offset: 0x00270383
	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	// Token: 0x060062E8 RID: 25320 RVA: 0x00282FA0 File Offset: 0x002811A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "is_xbox")
		{
			value = (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent().ToString();
			return true;
		}
		if (!(bindingName == "is_ps5"))
		{
			return false;
		}
		value = (DeviceFlag.PS5.IsCurrent() || (DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent()).ToString();
		return true;
	}

	// Token: 0x04004A90 RID: 19088
	public static string ID = "";
}
