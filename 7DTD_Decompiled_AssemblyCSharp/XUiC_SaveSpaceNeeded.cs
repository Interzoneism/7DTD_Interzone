using System;
using UnityEngine.Scripting;

// Token: 0x02000DF1 RID: 3569
[Preserve]
public class XUiC_SaveSpaceNeeded : XUiController
{
	// Token: 0x17000B43 RID: 2883
	// (get) Token: 0x06006FDB RID: 28635 RVA: 0x002D9FB8 File Offset: 0x002D81B8
	public bool ShouldShowDataBar
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return SaveInfoProvider.DataLimitEnabled;
		}
	}

	// Token: 0x17000B44 RID: 2884
	// (get) Token: 0x06006FDC RID: 28636 RVA: 0x002D9FBF File Offset: 0x002D81BF
	public bool HasSufficientSpace
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return !SaveInfoProvider.DataLimitEnabled || this.m_pendingBytes <= this.m_totalAvailableBytes;
		}
	}

	// Token: 0x17000B45 RID: 2885
	// (get) Token: 0x06006FDD RID: 28637 RVA: 0x002D9FDB File Offset: 0x002D81DB
	// (set) Token: 0x06006FDE RID: 28638 RVA: 0x002D9FE3 File Offset: 0x002D81E3
	public XUiC_SaveSpaceNeeded.ConfirmationResult Result { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06006FDF RID: 28639 RVA: 0x002D9FEC File Offset: 0x002D81EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1709222174U)
		{
			if (num <= 33042494U)
			{
				if (num != 31044760U)
				{
					if (num == 33042494U)
					{
						if (_bindingName == "langKeyDiscard")
						{
							_value = this.m_langKeyDiscard;
							return true;
						}
					}
				}
				else if (_bindingName == "shouldShowDataBar")
				{
					_value = this.ShouldShowDataBar.ToString();
					return true;
				}
			}
			else if (num != 119933375U)
			{
				if (num != 953006081U)
				{
					if (num == 1709222174U)
					{
						if (_bindingName == "langKeyCancel")
						{
							_value = this.m_langKeyCancel;
							return true;
						}
					}
				}
				else if (_bindingName == "canDiscard")
				{
					_value = this.m_canDiscard.ToString();
					return true;
				}
			}
			else if (_bindingName == "hasSufficientSpace")
			{
				_value = this.HasSufficientSpace.ToString();
				return true;
			}
		}
		else if (num <= 2615038744U)
		{
			if (num != 2066570139U)
			{
				if (num == 2615038744U)
				{
					if (_bindingName == "langKeyBody")
					{
						_value = this.m_langKeyBody;
						return true;
					}
				}
			}
			else if (_bindingName == "canCancel")
			{
				_value = this.m_canCancel.ToString();
				return true;
			}
		}
		else if (num != 2723046083U)
		{
			if (num != 2742710468U)
			{
				if (num == 3429363738U)
				{
					if (_bindingName == "langKeyTitle")
					{
						_value = this.m_langKeyTitle;
						return true;
					}
				}
			}
			else if (_bindingName == "langKeyConfirm")
			{
				_value = this.m_langKeyConfirm;
				return true;
			}
		}
		else if (_bindingName == "langKeyManage")
		{
			_value = this.m_langKeyManage;
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06006FE0 RID: 28640 RVA: 0x002DA1D8 File Offset: 0x002D83D8
	public override void Init()
	{
		base.Init();
		XUiC_SaveSpaceNeeded.ID = base.WindowGroup.ID;
		this.labelTitle = (XUiV_Label)base.GetChildById("titleText").ViewComponent;
		this.labelBody = (XUiV_Label)base.GetChildById("bodyText").ViewComponent;
		this.btnCancel = (XUiC_SimpleButton)base.GetChildById("btnCancel");
		this.btnDiscard = (XUiC_SimpleButton)base.GetChildById("btnDiscard");
		this.btnManage = (XUiC_SimpleButton)base.GetChildById("btnManage");
		this.btnConfirm = (XUiC_SimpleButton)base.GetChildById("btnConfirm");
		this.btnCancel.OnPressed += this.BtnCancel_OnPressed;
		this.btnDiscard.OnPressed += this.BtnDiscard_OnPressed;
		this.btnManage.OnPressed += this.BtnManage_OnPressed;
		this.btnConfirm.OnPressed += this.BtnConfirm_OnPressed;
		this.dataManagementBar = (base.GetChildById("data_bar_controller") as XUiC_DataManagementBar);
	}

	// Token: 0x06006FE1 RID: 28641 RVA: 0x002DA2FB File Offset: 0x002D84FB
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (!this.m_canCancel)
		{
			Log.Error("[SaveSpaceNeeded] Cancel button was pressed even though cancel is hidden?");
			return;
		}
		this.Result = XUiC_SaveSpaceNeeded.ConfirmationResult.Cancelled;
		base.xui.playerUI.windowManager.Close(XUiC_SaveSpaceNeeded.ID);
	}

	// Token: 0x06006FE2 RID: 28642 RVA: 0x002DA331 File Offset: 0x002D8531
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDiscard_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (!this.m_canDiscard)
		{
			Log.Error("[SaveSpaceNeeded] Discard button was pressed even though discard is hidden?");
			return;
		}
		this.Result = XUiC_SaveSpaceNeeded.ConfirmationResult.Discarded;
		base.xui.playerUI.windowManager.Close(XUiC_SaveSpaceNeeded.ID);
	}

	// Token: 0x06006FE3 RID: 28643 RVA: 0x002DA368 File Offset: 0x002D8568
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnManage_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow(XUiC_LoadingScreen.ID);
		object obj;
		if (xuiWindowGroup == null)
		{
			obj = null;
		}
		else
		{
			XUiController controller = xuiWindowGroup.Controller;
			obj = ((controller != null) ? controller.GetChildByType<XUiC_LoadingScreen>() : null);
		}
		object obj2 = obj;
		if (obj2 != null)
		{
			obj2.SetTipsVisible(false);
		}
		XUiC_DataManagement.OpenDataManagementWindow(this, new Action(this.OnDataManagementWindowClosed));
	}

	// Token: 0x06006FE4 RID: 28644 RVA: 0x002DA3CA File Offset: 0x002D85CA
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnConfirm_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (!this.HasSufficientSpace)
		{
			Log.Error("[SaveSpaceNeeded] Confirm button was pressed even though there isn't enough free space?");
			return;
		}
		this.Result = XUiC_SaveSpaceNeeded.ConfirmationResult.Confirmed;
		base.xui.playerUI.windowManager.Close(XUiC_SaveSpaceNeeded.ID);
	}

	// Token: 0x06006FE5 RID: 28645 RVA: 0x002DA400 File Offset: 0x002D8600
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDataManagementWindowClosed()
	{
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow(XUiC_LoadingScreen.ID);
		object obj;
		if (xuiWindowGroup == null)
		{
			obj = null;
		}
		else
		{
			XUiController controller = xuiWindowGroup.Controller;
			obj = ((controller != null) ? controller.GetChildByType<XUiC_LoadingScreen>() : null);
		}
		object obj2 = obj;
		if (obj2 != null)
		{
			obj2.SetTipsVisible(true);
		}
		this.UpdateBarValues();
	}

	// Token: 0x06006FE6 RID: 28646 RVA: 0x002DA458 File Offset: 0x002D8658
	public override void OnOpen()
	{
		base.OnOpen();
		if (!XUiC_SaveSpaceNeeded.m_openedProperly)
		{
			Log.Error("[SaveSpaceNeeded] XUiC_SaveSpaceNeeded should be opened with the static Open method so that InitInternal is executed.");
			base.xui.playerUI.windowManager.Close(XUiC_SaveSpaceNeeded.ID);
			return;
		}
		XUiC_SaveSpaceNeeded.m_openedProperly = false;
		this.Result = XUiC_SaveSpaceNeeded.ConfirmationResult.Pending;
		this.m_wasCursorHidden = base.xui.playerUI.CursorController.GetCursorHidden();
		base.xui.playerUI.CursorController.SetCursorHidden(false);
		this.m_wasCursorLocked = base.xui.playerUI.CursorController.Locked;
		base.xui.playerUI.CursorController.Locked = false;
		this.m_previousLockView = base.xui.playerUI.CursorController.lockNavigationToView;
	}

	// Token: 0x06006FE7 RID: 28647 RVA: 0x002DA524 File Offset: 0x002D8724
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.CursorController.SetCursorHidden(this.m_wasCursorHidden);
		base.xui.playerUI.CursorController.SetNavigationLockView(this.m_previousLockView, null);
		base.xui.playerUI.CursorController.Locked = this.m_wasCursorLocked;
		if (this.m_protectedPaths != null)
		{
			foreach (string text in this.m_protectedPaths)
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					SaveInfoProvider.Instance.SetDirectoryProtected(text, false);
				}
			}
		}
		ParentControllerState parentControllerState = this.m_parentControllerState;
		if (parentControllerState != null)
		{
			parentControllerState.Restore();
		}
		this.m_pendingBytes = 0L;
		this.m_protectedPaths = null;
		this.m_canCancel = true;
		this.m_canDiscard = true;
		this.m_langKeyTitle = "xuiSave";
		this.m_langKeyBody = "xuiDmSavingBody";
		this.m_langKeyCancel = "xuiCancel";
		this.m_langKeyDiscard = "xuiDiscard";
		this.m_langKeyConfirm = "xuiConfirm";
		this.m_langKeyManage = "xuiDmManageSaves";
	}

	// Token: 0x06006FE8 RID: 28648 RVA: 0x002DA630 File Offset: 0x002D8830
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitInternal(long pendingBytes, string[] protectedPaths, XUiController parentController, bool autoConfirm, bool canCancel, bool canDiscard, string langKeyTitle, string langKeyBody, string langKeyCancel, string langKeyDiscard, string langKeyConfirm, string langKeyManage)
	{
		this.m_pendingBytes = pendingBytes;
		this.m_protectedPaths = protectedPaths;
		this.m_canCancel = canCancel;
		this.m_canDiscard = canDiscard;
		this.m_langKeyTitle = (string.IsNullOrWhiteSpace(langKeyTitle) ? "xuiSave" : langKeyTitle);
		this.m_langKeyBody = (string.IsNullOrWhiteSpace(langKeyBody) ? "xuiDmSavingBody" : langKeyBody);
		this.m_langKeyCancel = (string.IsNullOrWhiteSpace(langKeyCancel) ? "xuiCancel" : langKeyCancel);
		this.m_langKeyDiscard = (string.IsNullOrWhiteSpace(langKeyDiscard) ? "xuiDiscard" : langKeyDiscard);
		this.m_langKeyConfirm = (string.IsNullOrWhiteSpace(langKeyConfirm) ? "xuiConfirm" : langKeyConfirm);
		this.m_langKeyManage = (string.IsNullOrWhiteSpace(langKeyManage) ? "xuiDmManageSaves" : langKeyManage);
		this.m_parentControllerState = new ParentControllerState(parentController);
		this.m_parentControllerState.Hide();
		if (this.m_protectedPaths != null)
		{
			foreach (string text in this.m_protectedPaths)
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					SaveInfoProvider.Instance.SetDirectoryProtected(text, true);
				}
			}
		}
		if (!autoConfirm || SaveInfoProvider.DataLimitEnabled)
		{
			this.UpdateBarValues();
			if (this.m_pendingBytes != 0L)
			{
				Log.Out("[SaveSpaceNeeded] Pending Bytes: " + this.m_pendingBytes.FormatSize(true) + ", Total Available Bytes: " + this.m_totalAvailableBytes.FormatSize(true));
			}
		}
		if (!autoConfirm || !this.HasSufficientSpace)
		{
			return;
		}
		if (this.m_pendingBytes != 0L)
		{
			Log.Out("[SaveSpaceNeeded] Auto-Confirming.");
		}
		this.BtnConfirm_OnPressed(this.btnConfirm, -1);
	}

	// Token: 0x06006FE9 RID: 28649 RVA: 0x002DA7A8 File Offset: 0x002D89A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBarValues()
	{
		SaveInfoProvider instance = SaveInfoProvider.Instance;
		this.dataManagementBar.ViewComponent.IsVisible = SaveInfoProvider.DataLimitEnabled;
		this.dataManagementBar.SetDisplayMode(XUiC_DataManagementBar.DisplayMode.Preview);
		this.dataManagementBar.SetUsedBytes(instance.TotalUsedBytes);
		this.dataManagementBar.SetAllowanceBytes(instance.TotalAllowanceBytes);
		this.dataManagementBar.SetPendingBytes(this.m_pendingBytes);
		this.m_totalAvailableBytes = instance.TotalAvailableBytes;
		base.RefreshBindings(false);
		if (this.HasSufficientSpace)
		{
			this.btnConfirm.SelectCursorElement(true, false);
			return;
		}
		this.btnManage.SelectCursorElement(true, false);
	}

	// Token: 0x06006FEA RID: 28650 RVA: 0x002DA848 File Offset: 0x002D8A48
	public static XUiC_SaveSpaceNeeded Open(long pendingBytes, string protectedPath, XUiController parentController = null, bool autoConfirm = false, bool canCancel = true, bool canDiscard = true, string title = null, string body = null, string cancel = null, string discard = null, string confirm = null, string manage = null)
	{
		return XUiC_SaveSpaceNeeded.Open(pendingBytes, new string[]
		{
			protectedPath
		}, parentController, autoConfirm, canCancel, canDiscard, title, body, cancel, discard, confirm, manage);
	}

	// Token: 0x06006FEB RID: 28651 RVA: 0x002DA878 File Offset: 0x002D8A78
	public static XUiC_SaveSpaceNeeded Open(long pendingBytes, string[] protectedPaths, XUiController parentController = null, bool autoConfirm = false, bool canCancel = true, bool canDiscard = true, string title = null, string body = null, string cancel = null, string discard = null, string confirm = null, string manage = null)
	{
		GUIWindowManager windowManager = LocalPlayerUI.primaryUI.xui.playerUI.windowManager;
		XUiC_SaveSpaceNeeded.m_openedProperly = true;
		windowManager.Open(XUiC_SaveSpaceNeeded.ID, true, true, false);
		XUiC_SaveSpaceNeeded.m_openedProperly = false;
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)windowManager.GetWindow(XUiC_SaveSpaceNeeded.ID);
		XUiC_SaveSpaceNeeded xuiC_SaveSpaceNeeded;
		if (xuiWindowGroup == null)
		{
			xuiC_SaveSpaceNeeded = null;
		}
		else
		{
			XUiController controller = xuiWindowGroup.Controller;
			xuiC_SaveSpaceNeeded = ((controller != null) ? controller.GetChildByType<XUiC_SaveSpaceNeeded>() : null);
		}
		XUiC_SaveSpaceNeeded xuiC_SaveSpaceNeeded2 = xuiC_SaveSpaceNeeded;
		if (xuiC_SaveSpaceNeeded2 == null)
		{
			Log.Error("[SaveSpaceNeeded] Failed to retrieve reference to XUiC_SaveSpaceNeeded instance.");
		}
		else
		{
			xuiC_SaveSpaceNeeded2.InitInternal(pendingBytes, protectedPaths, parentController, autoConfirm, canCancel, canDiscard, title, body, cancel, discard, confirm, manage);
		}
		return xuiC_SaveSpaceNeeded2;
	}

	// Token: 0x040054E4 RID: 21732
	[PublicizedFrom(EAccessModifier.Private)]
	public const string LangKeyDefaultTitle = "xuiSave";

	// Token: 0x040054E5 RID: 21733
	[PublicizedFrom(EAccessModifier.Private)]
	public const string LangKeyDefaultBody = "xuiDmSavingBody";

	// Token: 0x040054E6 RID: 21734
	[PublicizedFrom(EAccessModifier.Private)]
	public const string LangKeyDefaultCancel = "xuiCancel";

	// Token: 0x040054E7 RID: 21735
	[PublicizedFrom(EAccessModifier.Private)]
	public const string LangKeyDefaultDiscard = "xuiDiscard";

	// Token: 0x040054E8 RID: 21736
	[PublicizedFrom(EAccessModifier.Private)]
	public const string LangKeyDefaultConfirm = "xuiConfirm";

	// Token: 0x040054E9 RID: 21737
	[PublicizedFrom(EAccessModifier.Private)]
	public const string LangKeyDefaultManage = "xuiDmManageSaves";

	// Token: 0x040054EA RID: 21738
	public static string ID = "";

	// Token: 0x040054EB RID: 21739
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool m_openedProperly;

	// Token: 0x040054EC RID: 21740
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelTitle;

	// Token: 0x040054ED RID: 21741
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label labelBody;

	// Token: 0x040054EE RID: 21742
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnCancel;

	// Token: 0x040054EF RID: 21743
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDiscard;

	// Token: 0x040054F0 RID: 21744
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnManage;

	// Token: 0x040054F1 RID: 21745
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnConfirm;

	// Token: 0x040054F2 RID: 21746
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar dataManagementBar;

	// Token: 0x040054F3 RID: 21747
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView m_previousLockView;

	// Token: 0x040054F4 RID: 21748
	[PublicizedFrom(EAccessModifier.Private)]
	public long m_pendingBytes;

	// Token: 0x040054F5 RID: 21749
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] m_protectedPaths;

	// Token: 0x040054F6 RID: 21750
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_canCancel = true;

	// Token: 0x040054F7 RID: 21751
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_canDiscard = true;

	// Token: 0x040054F8 RID: 21752
	[PublicizedFrom(EAccessModifier.Private)]
	public string m_langKeyTitle = "xuiSave";

	// Token: 0x040054F9 RID: 21753
	[PublicizedFrom(EAccessModifier.Private)]
	public string m_langKeyBody = "xuiDmSavingBody";

	// Token: 0x040054FA RID: 21754
	[PublicizedFrom(EAccessModifier.Private)]
	public string m_langKeyCancel = "xuiCancel";

	// Token: 0x040054FB RID: 21755
	[PublicizedFrom(EAccessModifier.Private)]
	public string m_langKeyDiscard = "xuiDiscard";

	// Token: 0x040054FC RID: 21756
	[PublicizedFrom(EAccessModifier.Private)]
	public string m_langKeyConfirm = "xuiConfirm";

	// Token: 0x040054FD RID: 21757
	[PublicizedFrom(EAccessModifier.Private)]
	public string m_langKeyManage = "xuiDmManageSaves";

	// Token: 0x040054FE RID: 21758
	[PublicizedFrom(EAccessModifier.Private)]
	public ParentControllerState m_parentControllerState;

	// Token: 0x040054FF RID: 21759
	[PublicizedFrom(EAccessModifier.Private)]
	public long m_totalAvailableBytes;

	// Token: 0x04005500 RID: 21760
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_wasCursorHidden;

	// Token: 0x04005501 RID: 21761
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_wasCursorLocked;

	// Token: 0x02000DF2 RID: 3570
	public enum ConfirmationResult
	{
		// Token: 0x04005504 RID: 21764
		Pending,
		// Token: 0x04005505 RID: 21765
		Cancelled,
		// Token: 0x04005506 RID: 21766
		Discarded,
		// Token: 0x04005507 RID: 21767
		Confirmed
	}
}
