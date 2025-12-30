using System;
using UnityEngine.Scripting;

// Token: 0x02000D7B RID: 3451
[Preserve]
public class XUiC_PopupMenuItem : XUiController
{
	// Token: 0x06006BE5 RID: 27621 RVA: 0x002C2B8C File Offset: 0x002C0D8C
	public override void Init()
	{
		base.Init();
		base.OnPress += this.onPressed;
		base.OnHover += this.OnHovered;
		this.parentPopup = base.GetParentByType<XUiC_PopupMenu>();
		this.label = (XUiV_Label)base.GetChildById("lblText").ViewComponent;
		this.label.Overflow = UILabel.Overflow.ResizeFreely;
		this.slider = base.GetChildByType<XUiC_ComboBoxFloat>();
		this.slider.OnValueChanged += this.onValueChanged;
		this.slider.OnHoveredStateChanged += this.sliderHoveredChanged;
	}

	// Token: 0x06006BE6 RID: 27622 RVA: 0x002C2C30 File Offset: 0x002C0E30
	[PublicizedFrom(EAccessModifier.Private)]
	public void sliderHoveredChanged(XUiController _sender, bool _isOverMainArea, bool _isOverAnyPart)
	{
		this.OnHovered(this, _isOverAnyPart);
	}

	// Token: 0x06006BE7 RID: 27623 RVA: 0x002C2C3A File Offset: 0x002C0E3A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnHovered(XUiController _sender, bool _isOver)
	{
		this.isOver = _isOver;
		base.xui.currentPopupMenu.IsOver = _isOver;
		base.RefreshBindings(false);
	}

	// Token: 0x06006BE8 RID: 27624 RVA: 0x002C2C5C File Offset: 0x002C0E5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "menuicon")
		{
			XUiC_PopupMenuItem.Entry entry = this.itemEntry;
			_value = (((entry != null) ? entry.IconName : null) ?? "");
			return true;
		}
		if (_bindingName == "menutext")
		{
			XUiC_PopupMenuItem.Entry entry2 = this.itemEntry;
			_value = (((entry2 != null) ? entry2.Text : null) ?? "");
			return true;
		}
		if (_bindingName == "enabled")
		{
			XUiC_PopupMenuItem.Entry entry3 = this.itemEntry;
			_value = (entry3 != null && entry3.IsEnabled).ToString();
			return true;
		}
		if (_bindingName == "hovered")
		{
			_value = this.isOver.ToString();
			return true;
		}
		if (!(_bindingName == "type"))
		{
			return false;
		}
		XUiC_PopupMenuItem.Entry entry4 = this.itemEntry;
		_value = ((entry4 != null) ? entry4.EntryType : XUiC_PopupMenuItem.EEntryType.Button).ToStringCached<XUiC_PopupMenuItem.EEntryType>();
		return true;
	}

	// Token: 0x06006BE9 RID: 27625 RVA: 0x002C2D38 File Offset: 0x002C0F38
	[PublicizedFrom(EAccessModifier.Private)]
	public void onPressed(XUiController _sender, int _mouseButton)
	{
		if (this.itemEntry == null)
		{
			return;
		}
		if (this.itemEntry.EntryType != XUiC_PopupMenuItem.EEntryType.Button)
		{
			return;
		}
		if (this.itemEntry.IsEnabled)
		{
			this.itemEntry.HandleItemClicked();
		}
		base.xui.currentPopupMenu.ClearItems();
	}

	// Token: 0x06006BEA RID: 27626 RVA: 0x002C2D84 File Offset: 0x002C0F84
	[PublicizedFrom(EAccessModifier.Private)]
	public void onValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		if (this.itemEntry == null)
		{
			return;
		}
		if (this.itemEntry.EntryType != XUiC_PopupMenuItem.EEntryType.Slider)
		{
			return;
		}
		if (this.itemEntry.IsEnabled)
		{
			this.itemEntry.HandleValueChanged(_newValue);
		}
	}

	// Token: 0x06006BEB RID: 27627 RVA: 0x002C2DB8 File Offset: 0x002C0FB8
	public int SetEntry(XUiC_PopupMenuItem.Entry _entry)
	{
		this.itemEntry = _entry;
		base.RefreshBindings(false);
		this.label.SetTextImmediately(this.label.Text);
		base.ViewComponent.IsVisible = (_entry != null);
		if (_entry == null)
		{
			return 0;
		}
		if (_entry.EntryType == XUiC_PopupMenuItem.EEntryType.Slider)
		{
			this.slider.Value = _entry.Value;
			this.slider.Enabled = _entry.IsEnabled;
		}
		if (_entry.EntryType != XUiC_PopupMenuItem.EEntryType.Button)
		{
			return this.parentPopup.SliderMinWidth;
		}
		return (int)this.label.Label.printedSize.x;
	}

	// Token: 0x06006BEC RID: 27628 RVA: 0x002C2E52 File Offset: 0x002C1052
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x04005223 RID: 21027
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04005224 RID: 21028
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PopupMenu parentPopup;

	// Token: 0x04005225 RID: 21029
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label label;

	// Token: 0x04005226 RID: 21030
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat slider;

	// Token: 0x04005227 RID: 21031
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PopupMenuItem.Entry itemEntry;

	// Token: 0x02000D7C RID: 3452
	public enum EEntryType
	{
		// Token: 0x04005229 RID: 21033
		Button,
		// Token: 0x0400522A RID: 21034
		Slider
	}

	// Token: 0x02000D7D RID: 3453
	public class Entry
	{
		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x06006BEE RID: 27630 RVA: 0x002C2E62 File Offset: 0x002C1062
		// (set) Token: 0x06006BEF RID: 27631 RVA: 0x002C2E6A File Offset: 0x002C106A
		public object Tag { get; set; }

		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x06006BF0 RID: 27632 RVA: 0x002C2E74 File Offset: 0x002C1074
		// (remove) Token: 0x06006BF1 RID: 27633 RVA: 0x002C2EAC File Offset: 0x002C10AC
		public event XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate ItemClicked;

		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x06006BF2 RID: 27634 RVA: 0x002C2EE4 File Offset: 0x002C10E4
		// (remove) Token: 0x06006BF3 RID: 27635 RVA: 0x002C2F1C File Offset: 0x002C111C
		public event XUiC_PopupMenuItem.Entry.MenuItemValueChangedDelegate ValueChanged;

		// Token: 0x06006BF4 RID: 27636 RVA: 0x002C2F51 File Offset: 0x002C1151
		public void HandleItemClicked()
		{
			XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate itemClicked = this.ItemClicked;
			if (itemClicked == null)
			{
				return;
			}
			itemClicked(this);
		}

		// Token: 0x06006BF5 RID: 27637 RVA: 0x002C2F64 File Offset: 0x002C1164
		public void HandleValueChanged(double _newValue)
		{
			this.Value = _newValue;
			XUiC_PopupMenuItem.Entry.MenuItemValueChangedDelegate valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(this, this.Value);
		}

		// Token: 0x06006BF6 RID: 27638 RVA: 0x002C2F84 File Offset: 0x002C1184
		public Entry(string _text, string _iconName, bool _isEnabled = true, object _tag = null, XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate _handler = null)
		{
			this.Text = _text;
			this.IconName = _iconName;
			this.IsEnabled = _isEnabled;
			this.Tag = _tag;
			this.EntryType = XUiC_PopupMenuItem.EEntryType.Button;
			if (_handler != null)
			{
				this.ItemClicked += _handler;
			}
		}

		// Token: 0x06006BF7 RID: 27639 RVA: 0x002C2FBC File Offset: 0x002C11BC
		public Entry(string _text, string _iconName, double _initialValue, bool _isEnabled = true, object _tag = null, XUiC_PopupMenuItem.Entry.MenuItemValueChangedDelegate _handler = null)
		{
			this.Text = _text;
			this.IconName = _iconName;
			this.IsEnabled = _isEnabled;
			this.Tag = _tag;
			this.EntryType = XUiC_PopupMenuItem.EEntryType.Slider;
			this.Value = _initialValue;
			if (_handler != null)
			{
				this.ValueChanged += _handler;
			}
		}

		// Token: 0x0400522B RID: 21035
		public readonly string Text;

		// Token: 0x0400522C RID: 21036
		public readonly string IconName;

		// Token: 0x0400522D RID: 21037
		public readonly bool IsEnabled;

		// Token: 0x0400522E RID: 21038
		public double Value;

		// Token: 0x04005230 RID: 21040
		public readonly XUiC_PopupMenuItem.EEntryType EntryType;

		// Token: 0x02000D7E RID: 3454
		// (Invoke) Token: 0x06006BF9 RID: 27641
		public delegate void MenuItemClickedDelegate(XUiC_PopupMenuItem.Entry _entry);

		// Token: 0x02000D7F RID: 3455
		// (Invoke) Token: 0x06006BFD RID: 27645
		public delegate void MenuItemValueChangedDelegate(XUiC_PopupMenuItem.Entry _entry, double _newValue);
	}
}
