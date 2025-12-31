using System;
using UnityEngine.Scripting;

// Token: 0x02000DA8 RID: 3496
[Preserve]
public class XUiC_ProgressWindow : XUiController
{
	// Token: 0x17000AF2 RID: 2802
	// (set) Token: 0x06006D51 RID: 27985 RVA: 0x002C995C File Offset: 0x002C7B5C
	public string ProgressText
	{
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.lblProgress.Text = value;
		}
	}

	// Token: 0x17000AF3 RID: 2803
	// (get) Token: 0x06006D52 RID: 27986 RVA: 0x002C996A File Offset: 0x002C7B6A
	// (set) Token: 0x06006D53 RID: 27987 RVA: 0x002C9972 File Offset: 0x002C7B72
	public bool UseShadow
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.useShadow;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.useShadow != value)
			{
				this.IsDirty = true;
				this.useShadow = value;
			}
		}
	}

	// Token: 0x06006D54 RID: 27988 RVA: 0x002C998C File Offset: 0x002C7B8C
	public override void Init()
	{
		base.Init();
		XUiC_ProgressWindow.ID = base.WindowGroup.ID;
		this.lblProgress = (XUiV_Label)base.GetChildById("lblProgress").ViewComponent;
		this.ellipsisAnimator = new TextEllipsisAnimator(null, this.lblProgress);
	}

	// Token: 0x06006D55 RID: 27989 RVA: 0x002C99DC File Offset: 0x002C7BDC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.ellipsisAnimator.GetNextAnimatedString(_dt);
		if (this.escapeDelegate != null && base.xui.playerUI.playerInput != null && (base.xui.playerUI.playerInput.Menu.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Cancel.WasPressed))
		{
			this.escapeDelegate();
		}
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006D56 RID: 27990 RVA: 0x002C9A74 File Offset: 0x002C7C74
	public override void OnOpen()
	{
		base.OnOpen();
		((XUiV_Window)base.ViewComponent).Panel.alpha = 1f;
		base.xui.playerUI.CursorController.SetNavigationTarget(null);
		base.xui.playerUI.CursorController.Locked = true;
	}

	// Token: 0x06006D57 RID: 27991 RVA: 0x002C9ACD File Offset: 0x002C7CCD
	public override void OnClose()
	{
		base.OnClose();
		this.escapeDelegate = null;
		base.xui.playerUI.CursorController.Locked = false;
	}

	// Token: 0x06006D58 RID: 27992 RVA: 0x002C9AF2 File Offset: 0x002C7CF2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "use_shadow")
		{
			_value = this.useShadow.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06006D59 RID: 27993 RVA: 0x002C9B18 File Offset: 0x002C7D18
	public static bool IsWindowOpen()
	{
		XUiC_ProgressWindow childByType = LocalPlayerUI.primaryUI.xui.FindWindowGroupByName(XUiC_ProgressWindow.ID).GetChildByType<XUiC_ProgressWindow>();
		return childByType != null && childByType.IsOpen;
	}

	// Token: 0x06006D5A RID: 27994 RVA: 0x002C9B4C File Offset: 0x002C7D4C
	public static void Open(LocalPlayerUI _playerUi, string _text, Action _escDelegate = null, bool _modal = true, bool _escClosable = true, bool _closeOpenWindows = true, bool _useShadow = false)
	{
		if (_playerUi != null && _playerUi.xui != null)
		{
			XUiC_ProgressWindow childByType = _playerUi.xui.FindWindowGroupByName(XUiC_ProgressWindow.ID).GetChildByType<XUiC_ProgressWindow>();
			childByType.baseText = _text;
			childByType.ellipsisAnimator.SetBaseString(childByType.baseText, TextEllipsisAnimator.AnimationMode.All);
			_playerUi.windowManager.Open(XUiC_ProgressWindow.ID, _modal, _escClosable, _closeOpenWindows);
			childByType.escapeDelegate = _escDelegate;
			childByType.UseShadow = _useShadow;
		}
	}

	// Token: 0x06006D5B RID: 27995 RVA: 0x002C9BC3 File Offset: 0x002C7DC3
	public static void Close(LocalPlayerUI _playerUi)
	{
		if (_playerUi != null && _playerUi.xui != null)
		{
			_playerUi.windowManager.CloseIfOpen(XUiC_ProgressWindow.ID);
		}
	}

	// Token: 0x06006D5C RID: 27996 RVA: 0x002C9BEC File Offset: 0x002C7DEC
	public static void SetText(LocalPlayerUI _playerUi, string _text, bool _clearEscDelegate = true)
	{
		if (_playerUi != null && _playerUi.xui != null)
		{
			XUiC_ProgressWindow childByType = _playerUi.xui.FindWindowGroupByName(XUiC_ProgressWindow.ID).GetChildByType<XUiC_ProgressWindow>();
			childByType.baseText = _text;
			childByType.ellipsisAnimator.SetBaseString(childByType.baseText, TextEllipsisAnimator.AnimationMode.All);
			if (_clearEscDelegate)
			{
				childByType.escapeDelegate = null;
			}
		}
	}

	// Token: 0x06006D5D RID: 27997 RVA: 0x002C9C49 File Offset: 0x002C7E49
	public static string GetText(LocalPlayerUI _playerUi)
	{
		return _playerUi.xui.FindWindowGroupByName(XUiC_ProgressWindow.ID).GetChildByType<XUiC_ProgressWindow>().baseText;
	}

	// Token: 0x06006D5E RID: 27998 RVA: 0x002C9C65 File Offset: 0x002C7E65
	public static void SetEscDelegate(LocalPlayerUI _playerUi, Action _escapeDelegate)
	{
		_playerUi.xui.FindWindowGroupByName(XUiC_ProgressWindow.ID).GetChildByType<XUiC_ProgressWindow>().escapeDelegate = _escapeDelegate;
	}

	// Token: 0x040052F7 RID: 21239
	public static string ID = "";

	// Token: 0x040052F8 RID: 21240
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useShadow;

	// Token: 0x040052F9 RID: 21241
	[PublicizedFrom(EAccessModifier.Private)]
	public Action escapeDelegate;

	// Token: 0x040052FA RID: 21242
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblProgress;

	// Token: 0x040052FB RID: 21243
	[PublicizedFrom(EAccessModifier.Private)]
	public TextEllipsisAnimator ellipsisAnimator;

	// Token: 0x040052FC RID: 21244
	[PublicizedFrom(EAccessModifier.Private)]
	public string baseText;
}
