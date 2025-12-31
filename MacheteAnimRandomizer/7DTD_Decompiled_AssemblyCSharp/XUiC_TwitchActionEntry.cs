using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E7E RID: 3710
[Preserve]
public class XUiC_TwitchActionEntry : XUiController
{
	// Token: 0x17000BE1 RID: 3041
	// (get) Token: 0x060074C0 RID: 29888 RVA: 0x002F7D93 File Offset: 0x002F5F93
	public bool isEnabled
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.action.IsInPreset(this.Owner.CurrentPreset) && this.action.Enabled;
		}
	}

	// Token: 0x17000BE2 RID: 3042
	// (get) Token: 0x060074C1 RID: 29889 RVA: 0x002F7DBA File Offset: 0x002F5FBA
	// (set) Token: 0x060074C2 RID: 29890 RVA: 0x002F7DC2 File Offset: 0x002F5FC2
	public TwitchAction Action
	{
		get
		{
			return this.action;
		}
		set
		{
			base.ViewComponent.Enabled = (value != null);
			this.action = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BE3 RID: 3043
	// (get) Token: 0x060074C3 RID: 29891 RVA: 0x002F7DE1 File Offset: 0x002F5FE1
	// (set) Token: 0x060074C4 RID: 29892 RVA: 0x002F7DE9 File Offset: 0x002F5FE9
	public XUiC_TwitchInfoWindowGroup TwitchInfoUIHandler { get; set; }

	// Token: 0x17000BE4 RID: 3044
	// (get) Token: 0x060074C5 RID: 29893 RVA: 0x002F7DF2 File Offset: 0x002F5FF2
	// (set) Token: 0x060074C6 RID: 29894 RVA: 0x002F7DFA File Offset: 0x002F5FFA
	public bool Tracked { get; set; }

	// Token: 0x060074C7 RID: 29895 RVA: 0x002F7E04 File Offset: 0x002F6004
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.action != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1838951150U)
		{
			if (num <= 782028412U)
			{
				if (num != 765459171U)
				{
					if (num == 782028412U)
					{
						if (bindingName == "actioncommand")
						{
							value = (flag ? this.action.Command : "");
							return true;
						}
					}
				}
				else if (bindingName == "rowstatecolor")
				{
					value = (this.Selected ? "255,255,255,255" : (this.IsHovered ? this.hoverColor : this.rowColor));
					return true;
				}
			}
			else if (num != 1129104269U)
			{
				if (num != 1656712805U)
				{
					if (num == 1838951150U)
					{
						if (bindingName == "actionicon")
						{
							value = "";
							if (flag && this.action.DisplayCategory != null)
							{
								value = this.action.DisplayCategory.Icon;
							}
							return true;
						}
					}
				}
				else if (bindingName == "rowstatesprite")
				{
					value = (this.Selected ? "ui_game_select_row" : "menu_empty");
					return true;
				}
			}
			else if (bindingName == "showicon")
			{
				value = ((this.Owner != null) ? (this.Owner.TwitchEntryListWindow.ActionCategory == "").ToString() : "true");
				return true;
			}
		}
		else if (num <= 2938772023U)
		{
			if (num != 2511012101U)
			{
				if (num == 2938772023U)
				{
					if (bindingName == "actiontitle")
					{
						value = (flag ? (this.action.Title + this.GetModifiedWithColor()) : "");
						return true;
					}
				}
			}
			else if (bindingName == "commandcolor")
			{
				if (flag)
				{
					if (this.isEnabled)
					{
						if (this.action.IsPositive)
						{
							value = this.positiveColor;
						}
						else
						{
							value = this.negativeColor;
						}
					}
					else
					{
						value = this.disabledColor;
					}
				}
				return true;
			}
		}
		else if (num != 3106195591U)
		{
			if (num != 3291327539U)
			{
				if (num == 3644377122U)
				{
					if (bindingName == "textstatecolor")
					{
						value = "255,255,255,255";
						if (flag)
						{
							value = (this.isEnabled ? this.enabledColor : this.disabledColor);
						}
						return true;
					}
				}
			}
			else if (bindingName == "actiondescription")
			{
				value = (flag ? this.action.Description : "");
				return true;
			}
		}
		else if (bindingName == "iconcolor")
		{
			value = "255,255,255,255";
			if (flag)
			{
				value = (this.isEnabled ? this.enabledColor : this.disabledColor);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060074C8 RID: 29896 RVA: 0x00284555 File Offset: 0x00282755
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
	}

	// Token: 0x060074C9 RID: 29897 RVA: 0x002F80FB File Offset: 0x002F62FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		if (this.Action == null)
		{
			this.IsHovered = false;
			return;
		}
		if (this.IsHovered != _isOver)
		{
			this.IsHovered = _isOver;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060074CA RID: 29898 RVA: 0x00284594 File Offset: 0x00282794
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
	}

	// Token: 0x060074CB RID: 29899 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x060074CC RID: 29900 RVA: 0x002F812C File Offset: 0x002F632C
	public string GetModifiedWithColor()
	{
		if (this.Action != null)
		{
			int num = this.Action.ModifiedCost - this.Action.DefaultCost;
			if (num > 0)
			{
				return "[FF0000]*[-]";
			}
			if (num < 0)
			{
				return "[00FF00]*[-]";
			}
		}
		return "";
	}

	// Token: 0x060074CD RID: 29901 RVA: 0x002F8172 File Offset: 0x002F6372
	public override void OnCursorSelected()
	{
		base.OnCursorSelected();
		base.GetParentByType<XUiC_TwitchActionEntryList>().SelectedEntry = this;
		this.TwitchInfoUIHandler.SetEntry(this);
	}

	// Token: 0x060074CE RID: 29902 RVA: 0x002F8194 File Offset: 0x002F6394
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "enabled_color")
		{
			this.enabledColor = value;
			return true;
		}
		if (name == "disabled_color")
		{
			this.disabledColor = value;
			return true;
		}
		if (name == "positive_color")
		{
			this.positiveColor = value;
			return true;
		}
		if (name == "negative_color")
		{
			this.negativeColor = value;
			return true;
		}
		if (name == "row_color")
		{
			this.rowColor = value;
			return true;
		}
		if (!(name == "hover_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.hoverColor = value;
		return true;
	}

	// Token: 0x040058CB RID: 22731
	[PublicizedFrom(EAccessModifier.Private)]
	public string enabledColor;

	// Token: 0x040058CC RID: 22732
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor;

	// Token: 0x040058CD RID: 22733
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x040058CE RID: 22734
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x040058CF RID: 22735
	[PublicizedFrom(EAccessModifier.Private)]
	public string positiveColor = "0,0,255";

	// Token: 0x040058D0 RID: 22736
	[PublicizedFrom(EAccessModifier.Private)]
	public string negativeColor = "255,0,0";

	// Token: 0x040058D1 RID: 22737
	public new bool Selected;

	// Token: 0x040058D2 RID: 22738
	public bool IsHovered;

	// Token: 0x040058D3 RID: 22739
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchAction action;

	// Token: 0x040058D5 RID: 22741
	public XUiC_TwitchActionEntryList Owner;
}
