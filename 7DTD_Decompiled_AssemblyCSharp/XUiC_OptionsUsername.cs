using System;
using UnityEngine.Scripting;

// Token: 0x02000D5E RID: 3422
[Preserve]
public class XUiC_OptionsUsername : XUiController
{
	// Token: 0x06006AEE RID: 27374 RVA: 0x002BA2BC File Offset: 0x002B84BC
	public override void Init()
	{
		base.Init();
		XUiC_OptionsUsername.ID = base.WindowGroup.ID;
		this.txtUsername = (XUiC_TextInput)base.GetChildById("txtUsername");
		this.txtUsername.OnSubmitHandler += this.TxtUsername_OnSubmitHandler;
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnOk")).OnPressed += this.BtnOk_OnPressed;
	}

	// Token: 0x06006AEF RID: 27375 RVA: 0x002BA34E File Offset: 0x002B854E
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtUsername_OnSubmitHandler(XUiController _sender, string _text)
	{
		this.BtnOk_OnPressed(_sender, -1);
	}

	// Token: 0x06006AF0 RID: 27376 RVA: 0x002BA358 File Offset: 0x002B8558
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOk_OnPressed(XUiController _sender, int _mouseButton)
	{
		GamePrefs.Set(EnumGamePrefs.PlayerName, this.txtUsername.Text);
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006AF1 RID: 27377 RVA: 0x002BA3B4 File Offset: 0x002B85B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006AF2 RID: 27378 RVA: 0x002BA3F3 File Offset: 0x002B85F3
	public override void OnOpen()
	{
		base.OnOpen();
		this.txtUsername.Text = GamePrefs.GetString(EnumGamePrefs.PlayerName);
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
	}

	// Token: 0x040050EA RID: 20714
	public static string ID = "";

	// Token: 0x040050EB RID: 20715
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtUsername;
}
