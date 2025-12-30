using System;
using UnityEngine.Scripting;

// Token: 0x02000D10 RID: 3344
[Preserve]
public class XUiC_MainMenuPlayerName : XUiController
{
	// Token: 0x0600681C RID: 26652 RVA: 0x002A3430 File Offset: 0x002A1630
	public override void Init()
	{
		base.Init();
		XUiC_MainMenuPlayerName.ID = base.WindowGroup.ID;
	}

	// Token: 0x0600681D RID: 26653 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x0600681E RID: 26654 RVA: 0x002A3448 File Offset: 0x002A1648
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "name")
		{
			_value = (GamePrefs.GetString(EnumGamePrefs.PlayerName) ?? "");
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x0600681F RID: 26655 RVA: 0x002A3473 File Offset: 0x002A1673
	public static void OpenIfNotOpen(XUi _xuiInstance)
	{
		_xuiInstance.playerUI.windowManager.OpenIfNotOpen(XUiC_MainMenuPlayerName.ID, false, true, true);
	}

	// Token: 0x06006820 RID: 26656 RVA: 0x002A348D File Offset: 0x002A168D
	public static void Close(XUi _xuiInstance)
	{
		_xuiInstance.playerUI.windowManager.Close(XUiC_MainMenuPlayerName.ID);
	}

	// Token: 0x04004E79 RID: 20089
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";
}
