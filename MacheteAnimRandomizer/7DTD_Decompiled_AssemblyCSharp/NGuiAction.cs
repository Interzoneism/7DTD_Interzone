using System;
using InControl;

// Token: 0x02001027 RID: 4135
public class NGuiAction
{
	// Token: 0x06008308 RID: 33544 RVA: 0x0034EF8C File Offset: 0x0034D18C
	public NGuiAction()
	{
	}

	// Token: 0x06008309 RID: 33545 RVA: 0x0034EF9B File Offset: 0x0034D19B
	public NGuiAction(string _text, PlayerAction _hotkey) : this(_text, null, false)
	{
		this.hotkey = _hotkey;
	}

	// Token: 0x0600830A RID: 33546 RVA: 0x0034EFAD File Offset: 0x0034D1AD
	public NGuiAction(string _text, string _icon, bool _isToggle) : this(_text, _icon, null, _isToggle, null)
	{
	}

	// Token: 0x0600830B RID: 33547 RVA: 0x0034EFBA File Offset: 0x0034D1BA
	public NGuiAction(string _text, string _icon, string _description, bool _isToggle, PlayerAction _hotkey)
	{
		this.text = _text;
		this.icon = _icon;
		this.description = _description;
		this.hotkey = _hotkey;
		this.bToggle = _isToggle;
		this.bEnabled = true;
	}

	// Token: 0x0600830C RID: 33548 RVA: 0x0034EFF5 File Offset: 0x0034D1F5
	public virtual void OnClick()
	{
		if (this.IsEnabled() && this.clickActionDelegate != null)
		{
			this.clickActionDelegate();
		}
		this.UpdateUI();
	}

	// Token: 0x0600830D RID: 33549 RVA: 0x0034F018 File Offset: 0x0034D218
	public virtual void OnRelease()
	{
		if (this.IsEnabled() && this.releaseActionDelegate != null)
		{
			this.releaseActionDelegate();
		}
		this.UpdateUI();
	}

	// Token: 0x0600830E RID: 33550 RVA: 0x0034F03B File Offset: 0x0034D23B
	public virtual void OnDoubleClick()
	{
		if (this.IsEnabled() && this.doubleClickActionDelegate != null)
		{
			this.doubleClickActionDelegate();
		}
		this.UpdateUI();
	}

	// Token: 0x0600830F RID: 33551 RVA: 0x0034F05E File Offset: 0x0034D25E
	public virtual void OnSelect(bool _bSelected)
	{
		if (this.IsEnabled())
		{
			if (this.IsToggle())
			{
				this.SetChecked(_bSelected);
			}
			if (this.selectActionDelegate != null)
			{
				this.selectActionDelegate(_bSelected);
			}
		}
		this.UpdateUI();
	}

	// Token: 0x06008310 RID: 33552 RVA: 0x0034F091 File Offset: 0x0034D291
	public virtual bool IsActive()
	{
		return this.isVisibleDelegate == null || this.isVisibleDelegate();
	}

	// Token: 0x06008311 RID: 33553 RVA: 0x0034F0A8 File Offset: 0x0034D2A8
	public virtual string GetIcon()
	{
		return this.icon;
	}

	// Token: 0x06008312 RID: 33554 RVA: 0x0034F0B0 File Offset: 0x0034D2B0
	public virtual string GetText()
	{
		return this.text;
	}

	// Token: 0x06008313 RID: 33555 RVA: 0x0034F0B8 File Offset: 0x0034D2B8
	public void SetText(string _text)
	{
		this.text = _text;
		this.UpdateUI();
	}

	// Token: 0x06008314 RID: 33556 RVA: 0x0034F0C7 File Offset: 0x0034D2C7
	public virtual string GetTooltip()
	{
		return this.tooltip;
	}

	// Token: 0x06008315 RID: 33557 RVA: 0x0034F0CF File Offset: 0x0034D2CF
	public NGuiAction SetTooltip(string _tooltip)
	{
		this.tooltip = (string.IsNullOrEmpty(_tooltip) ? null : Localization.Get(_tooltip, false));
		return this;
	}

	// Token: 0x06008316 RID: 33558 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int GetColumnCount()
	{
		return 0;
	}

	// Token: 0x06008317 RID: 33559 RVA: 0x00019766 File Offset: 0x00017966
	public virtual string GetColumnIcon(int _col)
	{
		return null;
	}

	// Token: 0x06008318 RID: 33560 RVA: 0x00019766 File Offset: 0x00017966
	public virtual string GetColumnText(int _col)
	{
		return null;
	}

	// Token: 0x06008319 RID: 33561 RVA: 0x0034F0EA File Offset: 0x0034D2EA
	public virtual string GetDescription()
	{
		return this.description;
	}

	// Token: 0x0600831A RID: 33562 RVA: 0x0034F0F2 File Offset: 0x0034D2F2
	public virtual NGuiAction SetDescription(string _desc)
	{
		this.description = _desc;
		return this;
	}

	// Token: 0x0600831B RID: 33563 RVA: 0x0034F0FC File Offset: 0x0034D2FC
	public virtual PlayerAction GetHotkey()
	{
		return this.hotkey;
	}

	// Token: 0x0600831C RID: 33564 RVA: 0x0034F104 File Offset: 0x0034D304
	public virtual NGuiAction SetEnabled(bool _bEnabled)
	{
		this.bEnabled = _bEnabled;
		this.UpdateUI();
		return this;
	}

	// Token: 0x0600831D RID: 33565 RVA: 0x0034F114 File Offset: 0x0034D314
	public virtual bool IsEnabled()
	{
		if (this.isEnabledDelegate != null)
		{
			return this.isEnabledDelegate();
		}
		return this.bEnabled;
	}

	// Token: 0x0600831E RID: 33566 RVA: 0x0034F130 File Offset: 0x0034D330
	public virtual bool IsToggle()
	{
		return this.bToggle;
	}

	// Token: 0x0600831F RID: 33567 RVA: 0x0034F138 File Offset: 0x0034D338
	public virtual void SetChecked(bool _bChecked)
	{
		this.bChecked = _bChecked;
		this.UpdateUI();
	}

	// Token: 0x06008320 RID: 33568 RVA: 0x0034F147 File Offset: 0x0034D347
	public virtual bool IsChecked()
	{
		if (this.isCheckedDelegate != null)
		{
			return this.isCheckedDelegate();
		}
		return this.bChecked;
	}

	// Token: 0x06008321 RID: 33569 RVA: 0x0034F163 File Offset: 0x0034D363
	public virtual NGuiAction SetIsCheckedDelegate(NGuiAction.IsCheckedDelegate _checkedDelegate)
	{
		this.isCheckedDelegate = _checkedDelegate;
		return this;
	}

	// Token: 0x06008322 RID: 33570 RVA: 0x0034F16D File Offset: 0x0034D36D
	public virtual NGuiAction SetIsVisibleDelegate(NGuiAction.IsVisibleDelegate _isVisibleDelegate)
	{
		this.isVisibleDelegate = _isVisibleDelegate;
		return this;
	}

	// Token: 0x06008323 RID: 33571 RVA: 0x0034F177 File Offset: 0x0034D377
	public virtual NGuiAction SetIsEnabledDelegate(NGuiAction.IsEnabledDelegate _isEnabledDelegate)
	{
		this.isEnabledDelegate = _isEnabledDelegate;
		return this;
	}

	// Token: 0x06008324 RID: 33572 RVA: 0x0034F181 File Offset: 0x0034D381
	public virtual NGuiAction SetClickActionDelegate(NGuiAction.OnClickActionDelegate _actionDelegate)
	{
		this.clickActionDelegate = _actionDelegate;
		return this;
	}

	// Token: 0x06008325 RID: 33573 RVA: 0x0034F18B File Offset: 0x0034D38B
	public virtual NGuiAction SetReleaseActionDelegate(NGuiAction.OnReleaseActionDelegate _actionDelegate)
	{
		this.releaseActionDelegate = _actionDelegate;
		return this;
	}

	// Token: 0x06008326 RID: 33574 RVA: 0x0034F195 File Offset: 0x0034D395
	public virtual NGuiAction SetDoubleClickActionDelegate(NGuiAction.OnDoubleClickActionDelegate _actionDelegate)
	{
		this.doubleClickActionDelegate = _actionDelegate;
		return this;
	}

	// Token: 0x06008327 RID: 33575 RVA: 0x0034F19F File Offset: 0x0034D39F
	public virtual NGuiAction SetSelectActionDelegate(NGuiAction.OnSelectActionDelegate _selectActionDelegate)
	{
		this.selectActionDelegate = _selectActionDelegate;
		return this;
	}

	// Token: 0x06008328 RID: 33576 RVA: 0x00002914 File Offset: 0x00000B14
	public void UpdateUI()
	{
	}

	// Token: 0x06008329 RID: 33577 RVA: 0x0034F1A9 File Offset: 0x0034D3A9
	public override string ToString()
	{
		if (this.text == null)
		{
			return string.Empty;
		}
		return this.text;
	}

	// Token: 0x04006524 RID: 25892
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction hotkey;

	// Token: 0x04006525 RID: 25893
	public static NGuiAction Separator = new NGuiAction("Sep", null);

	// Token: 0x04006526 RID: 25894
	[PublicizedFrom(EAccessModifier.Private)]
	public string text;

	// Token: 0x04006527 RID: 25895
	[PublicizedFrom(EAccessModifier.Private)]
	public string icon;

	// Token: 0x04006528 RID: 25896
	[PublicizedFrom(EAccessModifier.Private)]
	public string description;

	// Token: 0x04006529 RID: 25897
	[PublicizedFrom(EAccessModifier.Private)]
	public string tooltip;

	// Token: 0x0400652A RID: 25898
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.OnClickActionDelegate clickActionDelegate;

	// Token: 0x0400652B RID: 25899
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.OnReleaseActionDelegate releaseActionDelegate;

	// Token: 0x0400652C RID: 25900
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.OnDoubleClickActionDelegate doubleClickActionDelegate;

	// Token: 0x0400652D RID: 25901
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.OnSelectActionDelegate selectActionDelegate;

	// Token: 0x0400652E RID: 25902
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.IsVisibleDelegate isVisibleDelegate;

	// Token: 0x0400652F RID: 25903
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.IsCheckedDelegate isCheckedDelegate;

	// Token: 0x04006530 RID: 25904
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiAction.IsEnabledDelegate isEnabledDelegate;

	// Token: 0x04006531 RID: 25905
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bEnabled;

	// Token: 0x04006532 RID: 25906
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bToggle;

	// Token: 0x04006533 RID: 25907
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bChecked;

	// Token: 0x04006534 RID: 25908
	public NGuiAction.EnumKeyMode KeyMode = NGuiAction.EnumKeyMode.FireOnPress;

	// Token: 0x02001028 RID: 4136
	[Flags]
	public enum EnumKeyMode
	{
		// Token: 0x04006536 RID: 25910
		None = 0,
		// Token: 0x04006537 RID: 25911
		FireOnPress = 1,
		// Token: 0x04006538 RID: 25912
		FireOnRelease = 2,
		// Token: 0x04006539 RID: 25913
		FireOnRepeat = 4
	}

	// Token: 0x02001029 RID: 4137
	// (Invoke) Token: 0x0600832C RID: 33580
	public delegate void OnClickActionDelegate();

	// Token: 0x0200102A RID: 4138
	// (Invoke) Token: 0x06008330 RID: 33584
	public delegate void OnReleaseActionDelegate();

	// Token: 0x0200102B RID: 4139
	// (Invoke) Token: 0x06008334 RID: 33588
	public delegate void OnDoubleClickActionDelegate();

	// Token: 0x0200102C RID: 4140
	// (Invoke) Token: 0x06008338 RID: 33592
	public delegate void OnSelectActionDelegate(bool _bSelected);

	// Token: 0x0200102D RID: 4141
	// (Invoke) Token: 0x0600833C RID: 33596
	public delegate bool IsEnabledDelegate();

	// Token: 0x0200102E RID: 4142
	// (Invoke) Token: 0x06008340 RID: 33600
	public delegate bool IsVisibleDelegate();

	// Token: 0x0200102F RID: 4143
	// (Invoke) Token: 0x06008344 RID: 33604
	public delegate bool IsCheckedDelegate();
}
