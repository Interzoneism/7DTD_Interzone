using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E2B RID: 3627
[Preserve]
public class XUiC_SimpleButton : XUiController
{
	// Token: 0x140000C2 RID: 194
	// (add) Token: 0x0600719C RID: 29084 RVA: 0x002E4698 File Offset: 0x002E2898
	// (remove) Token: 0x0600719D RID: 29085 RVA: 0x002E46D0 File Offset: 0x002E28D0
	public new event XUiEvent_OnPressEventHandler OnPressed;

	// Token: 0x17000B6C RID: 2924
	// (get) Token: 0x0600719E RID: 29086 RVA: 0x002E4705 File Offset: 0x002E2905
	// (set) Token: 0x0600719F RID: 29087 RVA: 0x002E471C File Offset: 0x002E291C
	public string Text
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

	// Token: 0x17000B6D RID: 2925
	// (get) Token: 0x060071A0 RID: 29088 RVA: 0x002E4732 File Offset: 0x002E2932
	// (set) Token: 0x060071A1 RID: 29089 RVA: 0x002E473F File Offset: 0x002E293F
	public string Tooltip
	{
		get
		{
			return this.button.ToolTip;
		}
		set
		{
			if (this.button.ToolTip != value)
			{
				this.button.ToolTip = value;
			}
		}
	}

	// Token: 0x17000B6E RID: 2926
	// (get) Token: 0x060071A2 RID: 29090 RVA: 0x002E4760 File Offset: 0x002E2960
	// (set) Token: 0x060071A3 RID: 29091 RVA: 0x002E476D File Offset: 0x002E296D
	public string DisabledToolTip
	{
		get
		{
			return this.button.DisabledToolTip;
		}
		set
		{
			if (this.button.DisabledToolTip != value)
			{
				this.button.DisabledToolTip = value;
			}
		}
	}

	// Token: 0x17000B6F RID: 2927
	// (get) Token: 0x060071A4 RID: 29092 RVA: 0x002E478E File Offset: 0x002E298E
	public XUiV_Label Label
	{
		get
		{
			return this.label;
		}
	}

	// Token: 0x17000B70 RID: 2928
	// (get) Token: 0x060071A5 RID: 29093 RVA: 0x002E4796 File Offset: 0x002E2996
	public XUiV_Button Button
	{
		get
		{
			return this.button;
		}
	}

	// Token: 0x17000B71 RID: 2929
	// (get) Token: 0x060071A6 RID: 29094 RVA: 0x002E479E File Offset: 0x002E299E
	// (set) Token: 0x060071A7 RID: 29095 RVA: 0x002E47A8 File Offset: 0x002E29A8
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
					this.updateLabelFontSize();
					this.updateLabelFontColor();
				}
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000B72 RID: 2930
	// (get) Token: 0x060071A8 RID: 29096 RVA: 0x002E4826 File Offset: 0x002E2A26
	// (set) Token: 0x060071A9 RID: 29097 RVA: 0x002E4842 File Offset: 0x002E2A42
	public bool IsVisible
	{
		get
		{
			return this.button.IsVisible || this.label.IsVisible;
		}
		set
		{
			this.button.IsVisible = value;
			this.label.IsVisible = value;
			if (this.border != null)
			{
				this.border.IsVisible = value;
			}
		}
	}

	// Token: 0x17000B73 RID: 2931
	// (get) Token: 0x060071AA RID: 29098 RVA: 0x002E4870 File Offset: 0x002E2A70
	// (set) Token: 0x060071AB RID: 29099 RVA: 0x002E4896 File Offset: 0x002E2A96
	public int FontSizeDefault
	{
		get
		{
			if (this.fontSizeDefault != 0)
			{
				return this.fontSizeDefault;
			}
			if (this.label == null)
			{
				return 0;
			}
			return this.label.FontSize;
		}
		set
		{
			if (value != this.fontSizeDefault)
			{
				this.fontSizeDefault = value;
				this.updateLabelFontSize();
			}
		}
	}

	// Token: 0x17000B74 RID: 2932
	// (get) Token: 0x060071AC RID: 29100 RVA: 0x002E48AE File Offset: 0x002E2AAE
	// (set) Token: 0x060071AD RID: 29101 RVA: 0x002E48C5 File Offset: 0x002E2AC5
	public int FontSizeHover
	{
		get
		{
			if (this.fontSizeHover != 0)
			{
				return this.fontSizeHover;
			}
			return this.FontSizeDefault;
		}
		set
		{
			if (value != this.fontSizeHover)
			{
				this.fontSizeHover = value;
				this.updateLabelFontSize();
			}
		}
	}

	// Token: 0x060071AE RID: 29102 RVA: 0x002E48E0 File Offset: 0x002E2AE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLabelFontSize()
	{
		if (this.label != null)
		{
			if (this.isEnabled)
			{
				this.label.FontSize = (this.isOver ? this.FontSizeHover : this.FontSizeDefault);
				return;
			}
			this.label.FontSize = this.FontSizeDefault;
		}
	}

	// Token: 0x060071AF RID: 29103 RVA: 0x002E4930 File Offset: 0x002E2B30
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLabelFontColor()
	{
		if (this.label != null)
		{
			if (this.isEnabled)
			{
				this.label.Color = ((this.isOver && this.HoveredLabelColor != null) ? this.HoveredLabelColor.Value : this.EnabledLabelColor);
				return;
			}
			this.label.Color = this.DisabledLabelColor;
		}
	}

	// Token: 0x060071B0 RID: 29104 RVA: 0x002E4994 File Offset: 0x002E2B94
	public override void Init()
	{
		base.Init();
		this.button = (base.GetChildById("clickable").ViewComponent as XUiV_Button);
		this.button.Controller.OnPress += this.Btn_OnPress;
		this.button.Controller.OnHover += this.Btn_OnHover;
		this.label = (base.GetChildById("btnLabel").ViewComponent as XUiV_Label);
		if (this.label != null)
		{
			this.label.Color = this.EnabledLabelColor;
		}
		XUiController childById = base.GetChildById("border");
		if (childById != null)
		{
			this.border = (childById.ViewComponent as XUiV_Sprite);
		}
	}

	// Token: 0x060071B1 RID: 29105 RVA: 0x002E4A4E File Offset: 0x002E2C4E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Btn_OnHover(XUiController _sender, bool _isOver)
	{
		this.isOver = _isOver;
		this.updateLabelFontSize();
		this.updateLabelFontColor();
		this.IsDirty = true;
	}

	// Token: 0x060071B2 RID: 29106 RVA: 0x002E4A6A File Offset: 0x002E2C6A
	[PublicizedFrom(EAccessModifier.Private)]
	public void Btn_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.isEnabled && this.OnPressed != null)
		{
			this.OnPressed(this, _mouseButton);
		}
	}

	// Token: 0x060071B3 RID: 29107 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x060071B4 RID: 29108 RVA: 0x002E4A8C File Offset: 0x002E2C8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "hovered")
		{
			_value = (this.isOver && this.isEnabled).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060071B5 RID: 29109 RVA: 0x002E4ACC File Offset: 0x002E2CCC
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (base.ParseAttribute(name, value, _parent))
		{
			return true;
		}
		if (!(name == "enabled_font_color"))
		{
			if (!(name == "hovered_font_color"))
			{
				if (!(name == "disabled_font_color"))
				{
					if (!(name == "font_size_default"))
					{
						if (!(name == "font_size_hover"))
						{
							if (!(name == "button_enabled"))
							{
								return false;
							}
							this.Enabled = StringParsers.ParseBool(value, 0, -1, true);
						}
						else
						{
							this.FontSizeHover = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
						}
					}
					else
					{
						this.FontSizeDefault = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
					}
				}
				else
				{
					this.DisabledLabelColor = StringParsers.ParseColor32(value);
				}
			}
			else
			{
				this.HoveredLabelColor = new Color?(StringParsers.ParseColor32(value));
			}
		}
		else
		{
			this.EnabledLabelColor = StringParsers.ParseColor32(value);
		}
		return true;
	}

	// Token: 0x0400566F RID: 22127
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label label;

	// Token: 0x04005670 RID: 22128
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button button;

	// Token: 0x04005671 RID: 22129
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite border;

	// Token: 0x04005672 RID: 22130
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isEnabled = true;

	// Token: 0x04005673 RID: 22131
	public string Tag;

	// Token: 0x04005674 RID: 22132
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04005675 RID: 22133
	[PublicizedFrom(EAccessModifier.Private)]
	public int fontSizeDefault;

	// Token: 0x04005676 RID: 22134
	[PublicizedFrom(EAccessModifier.Private)]
	public int fontSizeHover;

	// Token: 0x04005677 RID: 22135
	public Color EnabledLabelColor;

	// Token: 0x04005678 RID: 22136
	public Color? HoveredLabelColor;

	// Token: 0x04005679 RID: 22137
	public Color DisabledLabelColor;
}
