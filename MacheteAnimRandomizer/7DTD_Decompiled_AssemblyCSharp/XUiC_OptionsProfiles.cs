using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D55 RID: 3413
[Preserve]
public class XUiC_OptionsProfiles : XUiController
{
	// Token: 0x06006A9A RID: 27290 RVA: 0x002B6374 File Offset: 0x002B4574
	public override void Init()
	{
		base.Init();
		XUiC_OptionsProfiles.ID = base.WindowGroup.ID;
		this.profiles = base.GetChildByType<XUiC_ProfilesList>();
		this.profiles.SelectionChanged += this.Profiles_OnSelectionChanged;
		this.btnOk = base.GetChildById("btnOk").GetChildByType<XUiC_SimpleButton>();
		this.btnOk.OnPressed += this.BtnOk_OnPressed;
		this.btnProfileCreate = base.GetChildById("btnProfileCreate").GetChildByType<XUiC_SimpleButton>();
		this.btnProfileCreate.OnPressed += this.BtnProfileCreate_OnPressed;
		this.btnProfileCreate.ViewComponent.NavDownTarget = this.btnOk.ViewComponent;
		this.btnProfileDelete = base.GetChildById("btnProfileDelete").GetChildByType<XUiC_SimpleButton>();
		this.btnProfileDelete.OnPressed += this.BtnProfileDelete_OnPressed;
		this.btnProfileDelete.ViewComponent.NavDownTarget = this.btnOk.ViewComponent;
		this.btnProfileEdit = base.GetChildById("btnProfileEdit").GetChildByType<XUiC_SimpleButton>();
		this.btnProfileEdit.OnPressed += this.BtnProfileEdit_OnPressed;
		this.btnProfileEdit.ViewComponent.NavDownTarget = this.btnOk.ViewComponent;
		this.deleteProfilePanel = (XUiV_Panel)base.GetChildById("deleteProfilePanel").ViewComponent;
		((XUiC_SimpleButton)this.deleteProfilePanel.Controller.GetChildById("btnCancel")).OnPressed += this.BtnCancelDelete_OnPressed;
		((XUiC_SimpleButton)this.deleteProfilePanel.Controller.GetChildById("btnConfirm")).OnPressed += this.BtnConfirmDelete_OnPressed;
		this.deleteProfileText = (XUiV_Label)this.deleteProfilePanel.Controller.GetChildById("deleteText").ViewComponent;
		this.createProfilePanel = (XUiV_Panel)base.GetChildById("createProfilePanel").ViewComponent;
		((XUiC_SimpleButton)this.createProfilePanel.Controller.GetChildById("btnCancel")).OnPressed += this.BtnCancelCreate_OnPressed;
		this.createProfileConfirm = (XUiC_SimpleButton)this.createProfilePanel.Controller.GetChildById("btnConfirm");
		this.createProfileConfirm.OnPressed += this.BtnConfirmCreate_OnPressed;
		this.createProfileName = (XUiC_TextInput)this.createProfilePanel.Controller.GetChildById("createProfileName");
		this.createProfileName.OnSubmitHandler += this.CreateProfileName_OnSubmitHandler;
	}

	// Token: 0x06006A9B RID: 27291 RVA: 0x002B6609 File Offset: 0x002B4809
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnProfileEdit_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenCustomCharacterWindow();
	}

	// Token: 0x06006A9C RID: 27292 RVA: 0x002B6614 File Offset: 0x002B4814
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnProfileDelete_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.deleteProfilePanel.IsVisible = true;
		this.deleteProfileText.Text = string.Format(Localization.Get("xuiProfilesDeleteConfirmation", false), Utils.EscapeBbCodes(this.profiles.SelectedEntry.GetEntry().name, false, false));
		base.xui.playerUI.CursorController.SetNavigationLockView(this.deleteProfilePanel, this.deleteProfilePanel.Controller.GetChildById("btnCancel").ViewComponent);
	}

	// Token: 0x06006A9D RID: 27293 RVA: 0x002B669C File Offset: 0x002B489C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnProfileCreate_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.createProfilePanel.IsVisible = true;
		this.createProfileName.Text = "";
		base.xui.playerUI.CursorController.SetNavigationLockView(this.createProfilePanel, null);
		this.createProfileName.SelectOrVirtualKeyboard(false);
	}

	// Token: 0x06006A9E RID: 27294 RVA: 0x0028BEAC File Offset: 0x0028A0AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnOk_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x06006A9F RID: 27295 RVA: 0x002B66ED File Offset: 0x002B48ED
	[PublicizedFrom(EAccessModifier.Private)]
	public void Profiles_OnSelectionChanged(XUiC_ListEntry<XUiC_ProfilesList.ListEntry> _previousEntry, XUiC_ListEntry<XUiC_ProfilesList.ListEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			ProfileSDF.SetSelectedProfile(_newEntry.GetEntry().name);
			ProfileSDF.Save();
		}
		this.updateButtonStates();
	}

	// Token: 0x06006AA0 RID: 27296 RVA: 0x002B670D File Offset: 0x002B490D
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancelDelete_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.deleteProfilePanel.IsVisible = false;
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		this.btnProfileDelete.SelectCursorElement(false, false);
	}

	// Token: 0x06006AA1 RID: 27297 RVA: 0x002B6740 File Offset: 0x002B4940
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnConfirmDelete_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.deleteProfilePanel.IsVisible = false;
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		this.btnProfileDelete.SelectCursorElement(false, false);
		ProfileSDF.DeleteProfile(this.profiles.SelectedEntry.GetEntry().name);
		this.playerProfileCount--;
		string selectedProfile = "";
		string[] array = ProfileSDF.GetProfiles();
		if (array.Length != 0)
		{
			selectedProfile = array[0];
		}
		ProfileSDF.SetSelectedProfile(selectedProfile);
		this.profiles.RebuildList(false);
	}

	// Token: 0x06006AA2 RID: 27298 RVA: 0x002B67CC File Offset: 0x002B49CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancelCreate_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.createProfilePanel.IsVisible = false;
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		this.btnProfileCreate.SelectCursorElement(false, false);
	}

	// Token: 0x06006AA3 RID: 27299 RVA: 0x002B6800 File Offset: 0x002B4A00
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnConfirmCreate_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.createProfileConfirm.Enabled)
		{
			this.createProfilePanel.IsVisible = false;
			base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
			this.btnProfileCreate.SelectCursorElement(false, false);
			string text = this.createProfileName.Text.Trim();
			ProfileSDF.SaveProfile(text, "", true, "White", 1, "Blue01", "", "", "", "", "");
			ProfileSDF.SetSelectedProfile(text);
			ProfileSDF.Save();
			this.playerProfileCount++;
			this.profiles.RebuildList(false);
			this.OpenCustomCharacterWindow();
		}
	}

	// Token: 0x06006AA4 RID: 27300 RVA: 0x002B68B8 File Offset: 0x002B4AB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OpenCustomCharacterWindow()
	{
		Action onCloseAction = this.OnCloseAction;
		this.OnCloseAction = null;
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		this.OnCloseAction = onCloseAction;
		base.xui.playerUI.windowManager.Open(XUiC_CustomCharacterWindowGroup.ID, true, false, true);
	}

	// Token: 0x06006AA5 RID: 27301 RVA: 0x002B6917 File Offset: 0x002B4B17
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateProfileName_OnInputAbortedHandler(XUiController _sender)
	{
		this.BtnCancelCreate_OnPressed(this, -1);
	}

	// Token: 0x06006AA6 RID: 27302 RVA: 0x002B6921 File Offset: 0x002B4B21
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateProfileName_OnSubmitHandler(XUiController _sender, string _text)
	{
		ThreadManager.AddSingleTaskMainThread("OpenProfileEditorWindow", delegate(object _func)
		{
			this.BtnConfirmCreate_OnPressed(this, -1);
		}, null);
	}

	// Token: 0x06006AA7 RID: 27303 RVA: 0x002B693C File Offset: 0x002B4B3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateButtonStates()
	{
		bool flag = ProfileSDF.CurrentProfileName().Length != 0;
		bool flag2 = this.profiles.SelectedEntry != null;
		this.btnOk.Enabled = (flag && flag2);
		bool flag3 = flag && flag2;
		PlayerProfile playerProfile = PlayerProfile.LoadLocalProfile();
		Archetype archetype = Archetype.GetArchetype(playerProfile.ProfileArchetype);
		if (archetype == null)
		{
			archetype = Archetype.GetArchetype(playerProfile.IsMale ? "BaseMale" : "BaseFemale");
		}
		if (archetype != null)
		{
			flag3 &= archetype.CanCustomize;
		}
		this.btnProfileEdit.Enabled = flag3;
		this.btnProfileDelete.Enabled = flag3;
		this.btnProfileCreate.Enabled = true;
	}

	// Token: 0x06006AA8 RID: 27304 RVA: 0x002B69DC File Offset: 0x002B4BDC
	public override void OnOpen()
	{
		base.OnOpen();
		string text = ProfileSDF.CurrentProfileName();
		this.playerProfileCount = 0;
		foreach (XUiC_ProfilesList.ListEntry listEntry in this.profiles.AllEntries())
		{
			ProfileSDF.SetSelectedProfile(listEntry.name);
			if (string.IsNullOrEmpty(text))
			{
				text = listEntry.name;
				this.profiles.SelectByName(text);
			}
			if (Archetype.GetArchetype(PlayerProfile.LoadLocalProfile().ProfileArchetype).CanCustomize)
			{
				this.playerProfileCount++;
			}
		}
		ProfileSDF.SetSelectedProfile(text);
		this.deleteProfilePanel.IsVisible = false;
		this.createProfilePanel.IsVisible = false;
		this.updateButtonStates();
	}

	// Token: 0x06006AA9 RID: 27305 RVA: 0x002B6AAC File Offset: 0x002B4CAC
	public override void OnClose()
	{
		base.OnClose();
		if (this.OnCloseAction != null)
		{
			this.OnCloseAction();
			this.OnCloseAction = null;
			return;
		}
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006AAA RID: 27306 RVA: 0x002B6AEC File Offset: 0x002B4CEC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.createProfilePanel.IsVisible)
		{
			string text = this.createProfileName.Text.Trim();
			bool flag = text.Length > 0 && text.IndexOf('.') < 0 && !ProfileSDF.ProfileExists(text);
			this.createProfileConfirm.Enabled = flag;
			this.createProfileName.ActiveTextColor = (flag ? Color.white : Color.red);
		}
	}

	// Token: 0x06006AAB RID: 27307 RVA: 0x002B6B65 File Offset: 0x002B4D65
	public static void Open(XUi _xuiInstance, Action _onCloseAction = null)
	{
		_xuiInstance.FindWindowGroupByName(XUiC_OptionsProfiles.ID).GetChildByType<XUiC_OptionsProfiles>().OnCloseAction = _onCloseAction;
		_xuiInstance.playerUI.windowManager.Open(XUiC_OptionsProfiles.ID, true, false, true);
	}

	// Token: 0x0400505A RID: 20570
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MAX_USER_PROFILES = -1;

	// Token: 0x0400505B RID: 20571
	public static string ID = "";

	// Token: 0x0400505C RID: 20572
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ProfilesList profiles;

	// Token: 0x0400505D RID: 20573
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerProfileCount;

	// Token: 0x0400505E RID: 20574
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnOk;

	// Token: 0x0400505F RID: 20575
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnProfileCreate;

	// Token: 0x04005060 RID: 20576
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnProfileDelete;

	// Token: 0x04005061 RID: 20577
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnProfileEdit;

	// Token: 0x04005062 RID: 20578
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel deleteProfilePanel;

	// Token: 0x04005063 RID: 20579
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label deleteProfileText;

	// Token: 0x04005064 RID: 20580
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel createProfilePanel;

	// Token: 0x04005065 RID: 20581
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput createProfileName;

	// Token: 0x04005066 RID: 20582
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton createProfileConfirm;

	// Token: 0x04005067 RID: 20583
	[PublicizedFrom(EAccessModifier.Private)]
	public Action OnCloseAction;
}
