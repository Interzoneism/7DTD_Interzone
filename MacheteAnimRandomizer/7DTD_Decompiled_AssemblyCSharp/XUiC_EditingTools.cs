using System;
using UnityEngine.Scripting;

// Token: 0x02000CA8 RID: 3240
[Preserve]
public class XUiC_EditingTools : XUiController
{
	// Token: 0x0600640F RID: 25615 RVA: 0x0028822C File Offset: 0x0028642C
	public override void Init()
	{
		base.Init();
		XUiC_EditingTools.ID = base.WindowGroup.ID;
		((XUiC_SimpleButton)base.GetChildById("btnBack")).OnPressed += this.BtnBack_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnRwgPreviewer")).OnPressed += this.BtnRwgPreviewerOnOnPressed;
		XUiC_SimpleButton xuiC_SimpleButton = (XUiC_SimpleButton)base.GetChildById("btnPrefabEditor");
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += this.BtnPrefabEditorOnOnPressed;
		}
		((XUiC_SimpleButton)base.GetChildById("btnWorldEditor")).OnPressed += this.BtnLevelEditorOnOnPressed;
	}

	// Token: 0x06006410 RID: 25616 RVA: 0x002882D8 File Offset: 0x002864D8
	public override void OnOpen()
	{
		base.OnOpen();
		this.windowGroup.openWindowOnEsc = XUiC_MainMenu.ID;
	}

	// Token: 0x06006411 RID: 25617 RVA: 0x002882F0 File Offset: 0x002864F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRwgPreviewerOnOnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.FindWindowGroupByName("rwgeditor").GetChildByType<XUiC_WorldGenerationWindowGroup>().LastWindowID = XUiC_EditingTools.ID;
		base.xui.playerUI.windowManager.Open("rwgeditor", true, false, true);
	}

	// Token: 0x06006412 RID: 25618 RVA: 0x0028832E File Offset: 0x0028652E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnPrefabEditorOnOnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		XUiC_EditingTools.OpenPrefabEditor(base.xui);
	}

	// Token: 0x06006413 RID: 25619 RVA: 0x0028835C File Offset: 0x0028655C
	public static void OpenPrefabEditor(XUi xui = null)
	{
		if (xui == null)
		{
			xui = LocalPlayerUI.primaryUI.xui;
		}
		new GameModeEditWorld().ResetGamePrefs();
		GamePrefs.Set(EnumGamePrefs.GameWorld, "Empty");
		GamePrefs.Set(EnumGamePrefs.GameMode, GameModeEditWorld.TypeName);
		GamePrefs.Set(EnumGamePrefs.GameName, "PrefabEditor");
		GamePrefs.Set(EnumGamePrefs.ServerPort, 27020);
		NetworkConnectionError networkConnectionError = SingletonMonoBehaviour<ConnectionManager>.Instance.StartServers(GamePrefs.GetString(EnumGamePrefs.ServerPassword), false);
		if (networkConnectionError != NetworkConnectionError.NoError)
		{
			((XUiC_MessageBoxWindowGroup)((XUiWindowGroup)xui.playerUI.windowManager.GetWindow(XUiC_MessageBoxWindowGroup.ID)).Controller).ShowNetworkError(networkConnectionError);
		}
	}

	// Token: 0x06006414 RID: 25620 RVA: 0x002883F2 File Offset: 0x002865F2
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLevelEditorOnOnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_CreateWorld.ID, true, false, true);
	}

	// Token: 0x06006415 RID: 25621 RVA: 0x0027DCF9 File Offset: 0x0027BEF9
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
	}

	// Token: 0x04004B44 RID: 19268
	public static string ID = "";
}
