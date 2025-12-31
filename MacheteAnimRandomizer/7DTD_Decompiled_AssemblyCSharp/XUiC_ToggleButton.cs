using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E72 RID: 3698
[Preserve]
public class XUiC_ToggleButton : XUiController
{
	// Token: 0x140000CF RID: 207
	// (add) Token: 0x06007425 RID: 29733 RVA: 0x002F3528 File Offset: 0x002F1728
	// (remove) Token: 0x06007426 RID: 29734 RVA: 0x002F3560 File Offset: 0x002F1760
	public event XUiEvent_ToggleButtonValueChanged OnValueChanged;

	// Token: 0x17000BCC RID: 3020
	// (get) Token: 0x06007427 RID: 29735 RVA: 0x002F3595 File Offset: 0x002F1795
	// (set) Token: 0x06007428 RID: 29736 RVA: 0x002F35AC File Offset: 0x002F17AC
	public string Label
	{
		get
		{
			if (this.label == null)
			{
				return null;
			}
			return this.label.Text;
		}
		set
		{
			if (this.label != null)
			{
				this.label.Text = value;
			}
		}
	}

	// Token: 0x17000BCD RID: 3021
	// (get) Token: 0x06007429 RID: 29737 RVA: 0x002F35C2 File Offset: 0x002F17C2
	// (set) Token: 0x0600742A RID: 29738 RVA: 0x002F35D9 File Offset: 0x002F17D9
	public string Tooltip
	{
		get
		{
			if (this.button == null)
			{
				return null;
			}
			return this.button.ToolTip;
		}
		set
		{
			if (this.button != null)
			{
				this.button.ToolTip = value;
			}
		}
	}

	// Token: 0x17000BCE RID: 3022
	// (get) Token: 0x0600742B RID: 29739 RVA: 0x002F35EF File Offset: 0x002F17EF
	// (set) Token: 0x0600742C RID: 29740 RVA: 0x002F35F7 File Offset: 0x002F17F7
	public bool Value
	{
		get
		{
			return this.val;
		}
		set
		{
			if (value != this.val)
			{
				this.val = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000BCF RID: 3023
	// (get) Token: 0x0600742D RID: 29741 RVA: 0x002F3610 File Offset: 0x002F1810
	// (set) Token: 0x0600742E RID: 29742 RVA: 0x002F3618 File Offset: 0x002F1818
	public bool Enabled
	{
		get
		{
			return this.isEnabled;
		}
		set
		{
			if (value != this.isEnabled || (this.button != null && value != this.button.Enabled))
			{
				this.isEnabled = value;
				if (this.button != null)
				{
					this.button.Enabled = value;
				}
				if (this.label != null)
				{
					this.label.Color = (value ? this.EnabledLabelColor : this.DisabledLabelColor);
				}
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x0600742F RID: 29743 RVA: 0x002F368C File Offset: 0x002F188C
	public override void Init()
	{
		base.Init();
		this.button = (base.GetChildById("clickable").ViewComponent as XUiV_Button);
		this.button.Controller.OnPress += this.Btn_OnPress;
		this.label = (base.GetChildById("btnLabel").ViewComponent as XUiV_Label);
		if (this.label != null)
		{
			this.label.Color = this.EnabledLabelColor;
		}
	}

	// Token: 0x06007430 RID: 29744 RVA: 0x002F370A File Offset: 0x002F190A
	[PublicizedFrom(EAccessModifier.Private)]
	public void Btn_OnPress(XUiController _sender, int _mouseButton)
	{
		if (!this.isEnabled)
		{
			return;
		}
		this.val = !this.val;
		this.IsDirty = true;
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this, this.val);
		}
	}

	// Token: 0x06007431 RID: 29745 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x06007432 RID: 29746 RVA: 0x002F3745 File Offset: 0x002F1945
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "value")
		{
			value = this.val.ToString();
			return true;
		}
		return false;
	}

	// Token: 0x06007433 RID: 29747 RVA: 0x002F3764 File Offset: 0x002F1964
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (base.ParseAttribute(name, value, _parent))
		{
			return true;
		}
		if (!(name == "enabled_font_color"))
		{
			if (!(name == "disabled_font_color"))
			{
				if (!(name == "toggle_value"))
				{
					return false;
				}
				this.Value = StringParsers.ParseBool(value, 0, -1, true);
			}
			else
			{
				this.DisabledLabelColor = StringParsers.ParseColor32(value);
			}
		}
		else
		{
			this.EnabledLabelColor = StringParsers.ParseColor32(value);
		}
		return true;
	}

	// Token: 0x0400584E RID: 22606
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label label;

	// Token: 0x0400584F RID: 22607
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button button;

	// Token: 0x04005850 RID: 22608
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool val;

	// Token: 0x04005851 RID: 22609
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isEnabled = true;

	// Token: 0x04005852 RID: 22610
	public string Tag;

	// Token: 0x04005853 RID: 22611
	public Color EnabledLabelColor;

	// Token: 0x04005854 RID: 22612
	public Color DisabledLabelColor;
}
