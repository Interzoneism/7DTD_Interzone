using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D68 RID: 3432
[Preserve]
public class XUiC_Paging : XUiController
{
	// Token: 0x17000AC7 RID: 2759
	// (get) Token: 0x06006B32 RID: 27442 RVA: 0x002BD2BB File Offset: 0x002BB4BB
	// (set) Token: 0x06006B33 RID: 27443 RVA: 0x002BD2C3 File Offset: 0x002BB4C3
	public int CurrentPageNumber
	{
		get
		{
			return this.currentPageNumber;
		}
		set
		{
			value = Mathf.Clamp(value, 0, this.LastPageNumber);
			if (value != this.currentPageNumber)
			{
				this.currentPageNumber = value;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x17000AC8 RID: 2760
	// (get) Token: 0x06006B34 RID: 27444 RVA: 0x002BD2EB File Offset: 0x002BB4EB
	// (set) Token: 0x06006B35 RID: 27445 RVA: 0x002BD2F4 File Offset: 0x002BB4F4
	public int LastPageNumber
	{
		get
		{
			return this.lastPageNumber;
		}
		set
		{
			if (value != this.lastPageNumber)
			{
				this.lastPageNumber = value;
				base.RefreshBindings(false);
				if (this.currentPageNumber > this.lastPageNumber)
				{
					this.CurrentPageNumber = this.lastPageNumber;
					XUiEvent_PageChangedEventHandler onPageChanged = this.OnPageChanged;
					if (onPageChanged == null)
					{
						return;
					}
					onPageChanged();
				}
			}
		}
	}

	// Token: 0x140000B4 RID: 180
	// (add) Token: 0x06006B36 RID: 27446 RVA: 0x002BD344 File Offset: 0x002BB544
	// (remove) Token: 0x06006B37 RID: 27447 RVA: 0x002BD37C File Offset: 0x002BB57C
	public event XUiEvent_PageChangedEventHandler OnPageChanged;

	// Token: 0x06006B38 RID: 27448 RVA: 0x002BD3B4 File Offset: 0x002BB5B4
	public override void Init()
	{
		base.Init();
		this.btnPageDown = base.GetChildById("pageDown");
		if (this.btnPageDown != null)
		{
			this.btnPageDown.OnPress += delegate(XUiController _, int _)
			{
				this.PageDown();
			};
		}
		this.btnPageUp = base.GetChildById("pageUp");
		if (this.btnPageUp != null)
		{
			this.btnPageUp.OnPress += delegate(XUiController _, int _)
			{
				this.PageUp();
			};
		}
		if (!string.IsNullOrEmpty(this.contentParentName))
		{
			this.contentsParent = base.WindowGroup.Controller.GetChildById(this.contentParentName);
		}
		this.handlePageDownAction = new Func<bool>(this.PageDown);
		this.handlePageUpAction = new Func<bool>(this.PageUp);
		this.currentPageNumber = 0;
		base.RefreshBindings(false);
	}

	// Token: 0x06006B39 RID: 27449 RVA: 0x002BD484 File Offset: 0x002BB684
	public bool PageUp()
	{
		if (this.currentPageNumber < this.LastPageNumber)
		{
			this.currentPageNumber++;
			XUiEvent_PageChangedEventHandler onPageChanged = this.OnPageChanged;
			if (onPageChanged != null)
			{
				onPageChanged();
			}
			base.RefreshBindings(false);
			if (this.currentPageNumber == this.lastPageNumber && this.btnPageUp != null && base.xui.playerUI.CursorController.navigationTarget == this.btnPageUp.ViewComponent)
			{
				this.btnPageDown.SelectCursorElement(false, false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06006B3A RID: 27450 RVA: 0x002BD510 File Offset: 0x002BB710
	public bool PageDown()
	{
		if (this.currentPageNumber > 0)
		{
			this.currentPageNumber--;
			XUiEvent_PageChangedEventHandler onPageChanged = this.OnPageChanged;
			if (onPageChanged != null)
			{
				onPageChanged();
			}
			base.RefreshBindings(false);
			if (this.currentPageNumber == 0 && this.btnPageDown != null && base.xui.playerUI.CursorController.navigationTarget == this.btnPageDown.ViewComponent)
			{
				this.btnPageUp.SelectCursorElement(false, false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06006B3B RID: 27451 RVA: 0x002BD58F File Offset: 0x002BB78F
	public int GetPage()
	{
		return this.CurrentPageNumber;
	}

	// Token: 0x06006B3C RID: 27452 RVA: 0x002BD597 File Offset: 0x002BB797
	public void SetPage(int _page)
	{
		this.CurrentPageNumber = _page;
	}

	// Token: 0x06006B3D RID: 27453 RVA: 0x002BD5A0 File Offset: 0x002BB7A0
	public int GetLastPage()
	{
		return this.LastPageNumber;
	}

	// Token: 0x06006B3E RID: 27454 RVA: 0x002BD5A8 File Offset: 0x002BB7A8
	public void SetLastPageByElementsAndPageLength(int _elementCount, int _pageLength)
	{
		this.LastPageNumber = Math.Max(0, Mathf.CeilToInt((float)_elementCount / (float)_pageLength) - 1);
	}

	// Token: 0x06006B3F RID: 27455 RVA: 0x002BD5C4 File Offset: 0x002BB7C4
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "show_max_page")
		{
			this.showMaxPage = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		if (_name == "separator")
		{
			this.separator = _value;
			return true;
		}
		if (_name == "primary_pager")
		{
			this.primaryPager = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		if (_name == "contents_parent")
		{
			this.contentParentName = _value;
			return true;
		}
		if (!(_name == "hotkeys_enabled"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.hotkeysEnabled = StringParsers.ParseBool(_value, 0, -1, true);
		return true;
	}

	// Token: 0x06006B40 RID: 27456 RVA: 0x002BD664 File Offset: 0x002BB864
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "pagenumber")
		{
			_value = this.pagenumberFormatter.Format(this.currentPageNumber + 1);
			return true;
		}
		if (_bindingName == "maxpagenumber")
		{
			_value = this.maxpagenumberFormatter.Format(this.LastPageNumber + 1);
			return true;
		}
		if (_bindingName == "showmaxpage")
		{
			_value = this.showMaxPage.ToString();
			return true;
		}
		if (!(_bindingName == "separator"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.separator;
		return true;
	}

	// Token: 0x06006B41 RID: 27457 RVA: 0x002BD6F6 File Offset: 0x002BB8F6
	public void Reset()
	{
		this.currentPageNumber = 0;
		base.RefreshBindings(false);
	}

	// Token: 0x06006B42 RID: 27458 RVA: 0x002BD706 File Offset: 0x002BB906
	public override void OnOpen()
	{
		base.OnOpen();
		XUiC_Paging.activePagers.Add(this);
	}

	// Token: 0x06006B43 RID: 27459 RVA: 0x002BD719 File Offset: 0x002BB919
	public override void OnClose()
	{
		base.OnClose();
		XUiC_Paging.activePagers.Remove(this);
		if (XUiC_Paging.activePagers.Count == 0)
		{
			base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuPaging);
		}
	}

	// Token: 0x06006B44 RID: 27460 RVA: 0x002BD74C File Offset: 0x002BB94C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		bool flag;
		if (nativePlatform == null)
		{
			flag = false;
		}
		else
		{
			PlayerInputManager input = nativePlatform.Input;
			PlayerInputManager.InputStyle? inputStyle = (input != null) ? new PlayerInputManager.InputStyle?(input.CurrentInputStyle) : null;
			PlayerInputManager.InputStyle inputStyle2 = PlayerInputManager.InputStyle.Keyboard;
			flag = (inputStyle.GetValueOrDefault() == inputStyle2 & inputStyle != null);
		}
		if (flag)
		{
			return;
		}
		if (XUiC_Paging.activePagers.Count == 0)
		{
			return;
		}
		if (XUiC_Paging.activePagers[0] == this && this.hotkeysEnabled && !LocalPlayerUI.IsAnyComboBoxFocused)
		{
			bool flag2 = false;
			foreach (XUiC_Paging xuiC_Paging in XUiC_Paging.activePagers)
			{
				if (XUiC_Paging.activePagers.Count == 1 || xuiC_Paging.contentsParent == null || (base.xui.playerUI.CursorController.CurrentTarget != null && base.xui.playerUI.CursorController.CurrentTarget.Controller.IsChildOf(xuiC_Paging.contentsParent)))
				{
					XUi.HandlePaging(base.xui, xuiC_Paging.handlePageUpAction, xuiC_Paging.handlePageDownAction, false);
					base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuPaging, 0f);
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuPaging);
			}
		}
	}

	// Token: 0x0400516B RID: 20843
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentPageNumber;

	// Token: 0x0400516C RID: 20844
	public bool showMaxPage;

	// Token: 0x0400516D RID: 20845
	public string separator = "/";

	// Token: 0x0400516E RID: 20846
	[PublicizedFrom(EAccessModifier.Private)]
	public bool primaryPager = true;

	// Token: 0x0400516F RID: 20847
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastPageNumber;

	// Token: 0x04005170 RID: 20848
	public bool hotkeysEnabled = true;

	// Token: 0x04005172 RID: 20850
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<bool> handlePageDownAction;

	// Token: 0x04005173 RID: 20851
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<bool> handlePageUpAction;

	// Token: 0x04005174 RID: 20852
	[PublicizedFrom(EAccessModifier.Private)]
	public string contentParentName;

	// Token: 0x04005175 RID: 20853
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController contentsParent;

	// Token: 0x04005176 RID: 20854
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnPageUp;

	// Token: 0x04005177 RID: 20855
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnPageDown;

	// Token: 0x04005178 RID: 20856
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<XUiC_Paging> activePagers = new List<XUiC_Paging>();

	// Token: 0x04005179 RID: 20857
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt pagenumberFormatter = new CachedStringFormatterInt();

	// Token: 0x0400517A RID: 20858
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt maxpagenumberFormatter = new CachedStringFormatterInt();
}
