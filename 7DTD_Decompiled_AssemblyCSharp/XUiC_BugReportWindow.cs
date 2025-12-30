using System;
using System.Collections;
using Backtrace.Unity.Model;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C17 RID: 3095
[Preserve]
public class XUiC_BugReportWindow : XUiController
{
	// Token: 0x06005EFD RID: 24317 RVA: 0x00268A94 File Offset: 0x00266C94
	public override void Init()
	{
		base.Init();
		XUiC_BugReportWindow.ID = base.WindowGroup.ID;
		this.windowGroup.isEscClosable = false;
		this.btnSubmit = base.GetChildById("btnSubmit").GetChildByType<XUiC_SimpleButton>();
		this.btnSubmit.OnPressed += this.BtnSubmitOnPressed;
		base.GetChildById("btnCancel").GetChildByType<XUiC_SimpleButton>().OnPressed += this.BtnCancelOnPressed;
		this.txtDescription = (base.GetChildById("txtDescription") as XUiC_TextInput);
		this.txtDescription.OnChangeHandler += this.TxtDescriptionOnChanged;
		this.comboAttachScreenshot = (base.GetChildById("comboAttachScreenshot") as XUiC_ComboBoxBool);
		this.comboAttachScreenshot.OnValueChanged += this.ComboAttachScreenshot_OnValueChanged;
		this.comboAttachSave = (base.GetChildById("comboAttachSave") as XUiC_ComboBoxBool);
		this.comboAttachSave.OnValueChanged += this.ComboAttachSave_OnValueChanged;
		this.saveSelectWindow = this.windowGroup.Controller.GetChildByType<XUiC_BugReportSaveSelect>();
		if (this.saveSelectWindow != null)
		{
			this.saveSelectWindow.GetChildByType<XUiC_BugReportSavesList>().SelectionChanged += this.List_SelectionChanged;
		}
		this.lblAttachSaveDescInGame = base.GetChildById("lblAttachSaveDescInGame");
		this.lblAttachSaveDescMenu = base.GetChildById("lblAttachSaveDescMenu");
	}

	// Token: 0x06005EFE RID: 24318 RVA: 0x00268BF4 File Offset: 0x00266DF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void List_SelectionChanged(XUiC_ListEntry<XUiC_BugReportSavesList.ListEntry> _previousEntry, XUiC_ListEntry<XUiC_BugReportSavesList.ListEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			this.selectedSaveInfo = _newEntry.GetEntry().saveEntryInfo;
		}
		else
		{
			this.selectedSaveInfo = null;
		}
		this.CheckCanSubmit();
	}

	// Token: 0x06005EFF RID: 24319 RVA: 0x00268C1C File Offset: 0x00266E1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboAttachSave_OnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.attachSave = _newValue;
		if (!this.inGame)
		{
			this.saveSelectWindow.ViewComponent.IsVisible = this.attachSave;
		}
		if (!this.attachSave && !this.inGame)
		{
			this.selectedSaveInfo = null;
			if (this.saveSelectWindow != null)
			{
				this.saveSelectWindow.list.SelectedEntry = null;
			}
		}
		this.CheckCanSubmit();
	}

	// Token: 0x06005F00 RID: 24320 RVA: 0x00268C84 File Offset: 0x00266E84
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboAttachScreenshot_OnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.attachScreenshot = _newValue;
	}

	// Token: 0x06005F01 RID: 24321 RVA: 0x00268C8D File Offset: 0x00266E8D
	public static void Open(XUi _xui, bool _fromMainMenu)
	{
		_xui.playerUI.windowManager.Open(XUiC_BugReportWindow.ID, true, false, true);
		XUiC_BugReportWindow.fromMainMenu = _fromMainMenu;
	}

	// Token: 0x06005F02 RID: 24322 RVA: 0x00268CB0 File Offset: 0x00266EB0
	public override void OnOpen()
	{
		base.OnOpen();
		this.inGame = (GameManager.Instance.World != null);
		this.txtDescription.Text = "";
		this.attachSave = false;
		this.attachScreenshot = false;
		this.selectedSaveInfo = null;
		this.uploading = false;
		this.comboAttachSave.Value = false;
		this.comboAttachScreenshot.Value = false;
		this.lblAttachSaveDescMenu.ViewComponent.IsVisible = (BacktraceUtils.BugReportAttachSaveFeature && !this.inGame);
		this.lblAttachSaveDescInGame.ViewComponent.IsVisible = (BacktraceUtils.BugReportAttachSaveFeature && this.inGame);
		if (this.inGame)
		{
			GameManager.Instance.Pause(true);
		}
		base.RefreshBindings(false);
		this.CheckCanSubmit();
	}

	// Token: 0x06005F03 RID: 24323 RVA: 0x00268D80 File Offset: 0x00266F80
	public override void OnClose()
	{
		base.OnClose();
		this.uploading = false;
		base.xui.playerUI.playerInput.PermanentActions.Cancel.Enabled = true;
		if (XUiC_BugReportWindow.fromMainMenu)
		{
			base.xui.playerUI.windowManager.Open(XUiC_OptionsGeneral.ID, true, false, true);
		}
		if (this.inGame && !XUiC_BugReportWindow.fromMainMenu)
		{
			GameManager.Instance.Pause(false);
		}
	}

	// Token: 0x06005F04 RID: 24324 RVA: 0x00268DF8 File Offset: 0x00266FF8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.inGame && this.saveSelectWindow != null && this.saveSelectWindow.ViewComponent.IsVisible != this.attachSave)
		{
			this.saveSelectWindow.ViewComponent.IsVisible = this.attachSave;
		}
		else if (this.inGame && this.saveSelectWindow != null && this.saveSelectWindow.ViewComponent.IsVisible)
		{
			this.saveSelectWindow.ViewComponent.IsVisible = false;
		}
		if (!this.uploading && (base.xui.playerUI.playerInput.PermanentActions.Cancel.WasReleased || base.xui.playerUI.playerInput.GUIActions.Cancel.WasReleased))
		{
			base.xui.playerUI.windowManager.Close(XUiC_BugReportWindow.ID);
		}
	}

	// Token: 0x06005F05 RID: 24325 RVA: 0x00268EE3 File Offset: 0x002670E3
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtDescriptionOnChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.CheckCanSubmit();
	}

	// Token: 0x06005F06 RID: 24326 RVA: 0x00268EEC File Offset: 0x002670EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckCanSubmit()
	{
		if (this.inGame)
		{
			this.btnSubmit.Enabled = (this.canSubmit && this.txtDescription.Text.Length > 0);
			return;
		}
		this.btnSubmit.Enabled = (this.canSubmit && this.txtDescription.Text.Length > 0);
	}

	// Token: 0x06005F07 RID: 24327 RVA: 0x00268F54 File Offset: 0x00267154
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSubmitOnPressed(XUiController _sender, int _mouseButton)
	{
		if (this.canSubmit && this.txtDescription.Text.Length > 0)
		{
			if (this.inGame)
			{
				SaveInfoProvider.SaveEntryInfo saveEntryInfo2;
				if (GameManager.Instance.World.IsRemote())
				{
					string @string = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
					SaveInfoProvider.SaveEntryInfo saveEntryInfo;
					if (SaveInfoProvider.Instance.TryGetRemoteSaveEntry(@string, out saveEntryInfo))
					{
						this.selectedSaveInfo = saveEntryInfo;
					}
					else
					{
						Log.Error("Could not get save info entry for remote world");
					}
				}
				else if (SaveInfoProvider.Instance.TryGetLocalSaveEntry(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName), out saveEntryInfo2))
				{
					this.selectedSaveInfo = saveEntryInfo2;
				}
				else
				{
					Log.Error("Could not get save info entry for local world");
				}
			}
			XUiC_BugReportWindow.lastSubmissionTime = Time.time;
			ThreadManager.StartCoroutine(this.SubmitRoutine());
		}
	}

	// Token: 0x06005F08 RID: 24328 RVA: 0x0026900F File Offset: 0x0026720F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SubmitRoutine()
	{
		this.uploading = true;
		base.xui.playerUI.playerInput.PermanentActions.Cancel.Enabled = false;
		string screenshotPath = null;
		if (this.attachScreenshot)
		{
			base.ViewComponent.UiTransform.gameObject.SetActive(false);
			GameManager.Instance.Pause(false);
			int num;
			for (int i = 0; i < 10; i = num + 1)
			{
				yield return null;
				num = i;
			}
			yield return ThreadManager.CoroutineWrapperWithExceptionCallback(GameUtils.TakeScreenshotEnum(GameUtils.EScreenshotMode.File, PlatformApplicationManager.Application.temporaryCachePath + "/" + Application.productName, 0f, false, 0, 0, false), delegate(Exception _exception)
			{
				Log.Exception(_exception);
			});
			GameManager.Instance.Pause(true);
			yield return null;
			base.ViewComponent.UiTransform.gameObject.SetActive(true);
			screenshotPath = GameUtils.lastSavedScreenshotFilename;
		}
		yield return null;
		XUiC_ProgressWindow.Open(base.xui.playerUI, Localization.Get("xuiBugReportUploading", false), null, true, false, false, true);
		yield return new WaitForSecondsRealtime(0.5f);
		BacktraceUtils.SendBugReport(this.txtDescription.Text, screenshotPath, this.selectedSaveInfo, this.attachSave, new Action<BacktraceResult>(this.BugReportCallBack));
		yield break;
	}

	// Token: 0x06005F09 RID: 24329 RVA: 0x0026901E File Offset: 0x0026721E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BugReportCallBack(BacktraceResult _result)
	{
		Log.Out("Bug Report Send callback: {0}", new object[]
		{
			(_result == null) ? "null" : _result.message
		});
		this.HandlePostSubmissionClose();
	}

	// Token: 0x06005F0A RID: 24330 RVA: 0x0026904C File Offset: 0x0026724C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePostSubmissionClose()
	{
		XUiC_ProgressWindow.Close(base.xui.playerUI);
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, Localization.Get("xuiBugReportHeader", false), Localization.Get("xuiBugReportSubmitted", false), delegate()
		{
			if (XUiC_BugReportWindow.fromMainMenu)
			{
				base.xui.playerUI.windowManager.Open(XUiC_OptionsGeneral.ID, true, false, true);
			}
		}, XUiC_BugReportWindow.fromMainMenu, true);
	}

	// Token: 0x06005F0B RID: 24331 RVA: 0x002690BC File Offset: 0x002672BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "can_submit")
		{
			this.canSubmit = (Time.time - XUiC_BugReportWindow.lastSubmissionTime >= 600f || XUiC_BugReportWindow.lastSubmissionTime < 0f);
			_value = this.canSubmit.ToString();
			return true;
		}
		if (_bindingName == "in_game")
		{
			_value = this.inGame.ToString();
			return true;
		}
		if (!(_bindingName == "attach_saves_enabled"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = BacktraceUtils.BugReportAttachSaveFeature.ToString();
		return true;
	}

	// Token: 0x06005F0C RID: 24332 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancelOnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x040047A9 RID: 18345
	public static float lastSubmissionTime = -1f;

	// Token: 0x040047AA RID: 18346
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtDescription;

	// Token: 0x040047AB RID: 18347
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnSubmit;

	// Token: 0x040047AC RID: 18348
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboAttachScreenshot;

	// Token: 0x040047AD RID: 18349
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboAttachSave;

	// Token: 0x040047AE RID: 18350
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_BugReportSaveSelect saveSelectWindow;

	// Token: 0x040047AF RID: 18351
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblAttachSaveDescInGame;

	// Token: 0x040047B0 RID: 18352
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lblAttachSaveDescMenu;

	// Token: 0x040047B1 RID: 18353
	public static string ID;

	// Token: 0x040047B2 RID: 18354
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canSubmit;

	// Token: 0x040047B3 RID: 18355
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool fromMainMenu;

	// Token: 0x040047B4 RID: 18356
	[PublicizedFrom(EAccessModifier.Private)]
	public bool inGame;

	// Token: 0x040047B5 RID: 18357
	[PublicizedFrom(EAccessModifier.Private)]
	public bool attachScreenshot;

	// Token: 0x040047B6 RID: 18358
	[PublicizedFrom(EAccessModifier.Private)]
	public bool attachSave;

	// Token: 0x040047B7 RID: 18359
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveInfoProvider.SaveEntryInfo selectedSaveInfo;

	// Token: 0x040047B8 RID: 18360
	[PublicizedFrom(EAccessModifier.Private)]
	public bool uploading;
}
