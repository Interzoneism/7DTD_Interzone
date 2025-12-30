using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E80 RID: 3712
[Preserve]
public class XUiC_TwitchActionHistoryEntry : XUiController
{
	// Token: 0x17000BE7 RID: 3047
	// (get) Token: 0x060074DE RID: 29918 RVA: 0x002F8667 File Offset: 0x002F6867
	// (set) Token: 0x060074DF RID: 29919 RVA: 0x002F866F File Offset: 0x002F686F
	public TwitchActionHistoryEntry HistoryItem
	{
		get
		{
			return this.historyItem;
		}
		set
		{
			base.ViewComponent.Enabled = (value != null);
			this.historyItem = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BE8 RID: 3048
	// (get) Token: 0x060074E0 RID: 29920 RVA: 0x002F868E File Offset: 0x002F688E
	// (set) Token: 0x060074E1 RID: 29921 RVA: 0x002F8696 File Offset: 0x002F6896
	public XUiC_TwitchInfoWindowGroup TwitchInfoUIHandler { get; set; }

	// Token: 0x060074E2 RID: 29922 RVA: 0x002F86A0 File Offset: 0x002F68A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.historyItem != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2472102578U)
		{
			if (num <= 1320097209U)
			{
				if (num != 765459171U)
				{
					if (num == 1320097209U)
					{
						if (bindingName == "username")
						{
							if (flag)
							{
								if (this.historyItem.IsRefunded)
								{
									value = this.historyItem.UserName;
								}
								else
								{
									value = string.Format("[{0}]{1}[-]", this.historyItem.UserColor, this.historyItem.UserName);
								}
							}
							else
							{
								value = "";
							}
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
			else if (num != 1656712805U)
			{
				if (num == 2472102578U)
				{
					if (bindingName == "command")
					{
						value = (flag ? this.historyItem.Command : "");
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
		else if (num <= 3284901973U)
		{
			if (num != 2511012101U)
			{
				if (num == 3284901973U)
				{
					if (bindingName == "command_with_cost")
					{
						value = (flag ? this.historyItem.Command : "");
						return true;
					}
				}
			}
			else if (bindingName == "commandcolor")
			{
				if (flag)
				{
					if (this.historyItem.Action != null)
					{
						if (this.historyItem.IsRefunded)
						{
							value = this.disabledColor;
						}
						else if (this.historyItem.Action.IsPositive)
						{
							value = this.positiveColor;
						}
						else
						{
							value = this.negativeColor;
						}
					}
					else if (this.historyItem.Vote != null)
					{
						value = this.historyItem.Vote.TitleColor;
					}
					else if (this.historyItem.EventEntry != null)
					{
						value = "255,255,255,255";
					}
				}
				return true;
			}
		}
		else if (num != 3644377122U)
		{
			if (num == 3898356536U)
			{
				if (bindingName == "cost")
				{
					value = (flag ? this.historyItem.Action.CurrentCost.ToString() : "");
					return true;
				}
			}
		}
		else if (bindingName == "textstatecolor")
		{
			value = "255,255,255,255";
			if (flag)
			{
				value = (this.historyItem.IsRefunded ? this.disabledColor : this.enabledColor);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060074E3 RID: 29923 RVA: 0x00284555 File Offset: 0x00282755
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
	}

	// Token: 0x060074E4 RID: 29924 RVA: 0x002F8968 File Offset: 0x002F6B68
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		if (this.historyItem == null)
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

	// Token: 0x060074E5 RID: 29925 RVA: 0x002F8998 File Offset: 0x002F6B98
	public override void OnCursorSelected()
	{
		base.OnCursorSelected();
		base.GetParentByType<XUiC_TwitchActionHistoryEntryList>().SelectedEntry = this;
		this.TwitchInfoUIHandler.SetEntry(this);
	}

	// Token: 0x060074E6 RID: 29926 RVA: 0x00284594 File Offset: 0x00282794
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
	}

	// Token: 0x060074E7 RID: 29927 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x060074E8 RID: 29928 RVA: 0x002F89B8 File Offset: 0x002F6BB8
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

	// Token: 0x040058E1 RID: 22753
	[PublicizedFrom(EAccessModifier.Private)]
	public string enabledColor;

	// Token: 0x040058E2 RID: 22754
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor;

	// Token: 0x040058E3 RID: 22755
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x040058E4 RID: 22756
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x040058E5 RID: 22757
	[PublicizedFrom(EAccessModifier.Private)]
	public string positiveColor = "0,0,255";

	// Token: 0x040058E6 RID: 22758
	[PublicizedFrom(EAccessModifier.Private)]
	public string negativeColor = "255,0,0";

	// Token: 0x040058E7 RID: 22759
	public new bool Selected;

	// Token: 0x040058E8 RID: 22760
	public bool IsHovered;

	// Token: 0x040058E9 RID: 22761
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchActionHistoryEntry historyItem;

	// Token: 0x040058EB RID: 22763
	public XUiC_TwitchActionHistoryEntryList Owner;
}
