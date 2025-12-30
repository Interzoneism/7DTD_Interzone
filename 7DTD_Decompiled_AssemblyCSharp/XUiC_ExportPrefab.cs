using System;
using UnityEngine.Scripting;

// Token: 0x02000CB3 RID: 3251
[Preserve]
public class XUiC_ExportPrefab : XUiController
{
	// Token: 0x0600648A RID: 25738 RVA: 0x0028BD04 File Offset: 0x00289F04
	public override void Init()
	{
		base.Init();
		XUiC_ExportPrefab.ID = base.WindowGroup.ID;
		this.btnSave = (XUiC_SimpleButton)base.GetChildById("btnSave");
		this.btnSave.OnPressed += this.BtnSave_OnPressed;
		this.btnSaveLocal = (XUiC_SimpleButton)base.GetChildById("btnSaveLocal");
		this.btnSaveLocal.OnPressed += this.BtnSaveLocal_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		this.txtSaveName = (XUiC_TextInput)base.GetChildById("txtSaveName");
		this.txtSaveName.OnChangeHandler += this.TxtSaveNameOnOnChangeHandler;
		this.lblPrefabExists = (base.GetChildById("lblPrefabExists").ViewComponent as XUiV_Label);
		this.lblInvalidName = (base.GetChildById("lblInvalidName").ViewComponent as XUiV_Label);
		this.toggleAsPart = base.GetChildByType<XUiC_ToggleButton>();
	}

	// Token: 0x0600648B RID: 25739 RVA: 0x0028BE14 File Offset: 0x0028A014
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtSaveNameOnOnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		bool flag = _text.Length > 0 && !_text.Contains(" ") && GameUtils.ValidateGameName(_text);
		bool flag2 = flag && false;
		this.lblPrefabExists.IsVisible = flag2;
		this.lblInvalidName.IsVisible = !flag;
		this.btnSave.Enabled = (flag && SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient && !flag2);
		this.btnSaveLocal.Enabled = (flag && !flag2);
	}

	// Token: 0x0600648C RID: 25740 RVA: 0x0028BE9A File Offset: 0x0028A09A
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSave_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.SaveAndClose(false);
	}

	// Token: 0x0600648D RID: 25741 RVA: 0x0028BEA3 File Offset: 0x0028A0A3
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSaveLocal_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.SaveAndClose(true);
	}

	// Token: 0x0600648E RID: 25742 RVA: 0x0028BEAC File Offset: 0x0028A0AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x0600648F RID: 25743 RVA: 0x0028BED0 File Offset: 0x0028A0D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveAndClose(bool _local)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		string text = ConsoleCmdExportPrefab.BuildCommandString(this.txtSaveName.Text, BlockToolSelection.Instance.SelectionStart, BlockToolSelection.Instance.SelectionEnd, this.toggleAsPart.Value);
		if (_local || SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GameManager.Instance.m_GUIConsole.AddLines(SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(text, null));
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageConsoleCmdServer>().Setup(text), false);
		}
		if (!GameManager.Instance.m_GUIConsole.isShowing)
		{
			LocalPlayerUI.primaryUI.windowManager.Open(GameManager.Instance.m_GUIConsole, false, false, true);
		}
	}

	// Token: 0x06006490 RID: 25744 RVA: 0x0028BF9C File Offset: 0x0028A19C
	public override void OnOpen()
	{
		base.OnOpen();
		this.txtSaveName.Text = "";
		this.TxtSaveNameOnOnChangeHandler(this, "", true);
		this.IsDirty = true;
	}

	// Token: 0x06006491 RID: 25745 RVA: 0x0028BFC8 File Offset: 0x0028A1C8
	public static void Open(XUi _xui)
	{
		_xui.playerUI.windowManager.Open(XUiC_ExportPrefab.ID, true, false, true);
	}

	// Token: 0x04004BD6 RID: 19414
	public static string ID = "";

	// Token: 0x04004BD7 RID: 19415
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtSaveName;

	// Token: 0x04004BD8 RID: 19416
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnSave;

	// Token: 0x04004BD9 RID: 19417
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnSaveLocal;

	// Token: 0x04004BDA RID: 19418
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblPrefabExists;

	// Token: 0x04004BDB RID: 19419
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblInvalidName;

	// Token: 0x04004BDC RID: 19420
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleAsPart;
}
