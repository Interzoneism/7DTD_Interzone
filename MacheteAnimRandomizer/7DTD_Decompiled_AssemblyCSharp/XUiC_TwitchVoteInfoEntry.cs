using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E93 RID: 3731
[Preserve]
public class XUiC_TwitchVoteInfoEntry : XUiController
{
	// Token: 0x17000BFB RID: 3067
	// (get) Token: 0x060075A6 RID: 30118 RVA: 0x002FE667 File Offset: 0x002FC867
	// (set) Token: 0x060075A7 RID: 30119 RVA: 0x002FE66F File Offset: 0x002FC86F
	public TwitchVote Vote
	{
		get
		{
			return this.vote;
		}
		set
		{
			base.ViewComponent.Enabled = (value != null);
			this.vote = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BFC RID: 3068
	// (get) Token: 0x060075A8 RID: 30120 RVA: 0x002FE68E File Offset: 0x002FC88E
	// (set) Token: 0x060075A9 RID: 30121 RVA: 0x002FE696 File Offset: 0x002FC896
	public XUiC_TwitchInfoWindowGroup TwitchInfoUIHandler { get; set; }

	// Token: 0x17000BFD RID: 3069
	// (get) Token: 0x060075AA RID: 30122 RVA: 0x002FE69F File Offset: 0x002FC89F
	// (set) Token: 0x060075AB RID: 30123 RVA: 0x002FE6A7 File Offset: 0x002FC8A7
	public bool Tracked { get; set; }

	// Token: 0x060075AC RID: 30124 RVA: 0x002FE6B0 File Offset: 0x002FC8B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.vote != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1656712805U)
		{
			if (num != 765459171U)
			{
				if (num != 1129104269U)
				{
					if (num == 1656712805U)
					{
						if (bindingName == "rowstatesprite")
						{
							value = (this.Selected ? "ui_game_select_row" : "menu_empty");
							return true;
						}
					}
				}
				else if (bindingName == "showicon")
				{
					value = ((this.Owner != null) ? (this.Owner.TwitchEntryListWindow.VoteCategory == "").ToString() : "true");
					return true;
				}
			}
			else if (bindingName == "rowstatecolor")
			{
				value = (this.Selected ? "255,255,255,255" : (this.IsHovered ? this.hoverColor : this.rowColor));
				return true;
			}
		}
		else if (num <= 2801367993U)
		{
			if (num != 2104701544U)
			{
				if (num == 2801367993U)
				{
					if (bindingName == "votetitle")
					{
						value = (flag ? this.vote.VoteDescription : "");
						return true;
					}
				}
			}
			else if (bindingName == "votecolor")
			{
				if (flag)
				{
					if (this.vote.Enabled)
					{
						value = ((this.vote.TitleColor == "") ? this.enabledColor : this.vote.TitleColor);
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
			if (num == 4217867520U)
			{
				if (bindingName == "voteicon")
				{
					value = "";
					if (flag && this.vote.MainVoteType != null)
					{
						value = this.vote.MainVoteType.Icon;
					}
					return true;
				}
			}
		}
		else if (bindingName == "iconcolor")
		{
			value = "255,255,255,255";
			if (flag)
			{
				value = (this.vote.Enabled ? this.enabledColor : this.disabledColor);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060075AD RID: 30125 RVA: 0x00284555 File Offset: 0x00282755
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
	}

	// Token: 0x060075AE RID: 30126 RVA: 0x002FE8ED File Offset: 0x002FCAED
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		if (this.Vote == null)
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

	// Token: 0x060075AF RID: 30127 RVA: 0x002FE91D File Offset: 0x002FCB1D
	public override void OnCursorSelected()
	{
		base.OnCursorSelected();
		base.GetParentByType<XUiC_TwitchVoteInfoEntryList>().SelectedEntry = this;
		this.TwitchInfoUIHandler.SetEntry(this);
	}

	// Token: 0x060075B0 RID: 30128 RVA: 0x00284594 File Offset: 0x00282794
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
	}

	// Token: 0x060075B1 RID: 30129 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x060075B2 RID: 30130 RVA: 0x002FE940 File Offset: 0x002FCB40
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

	// Token: 0x040059BE RID: 22974
	[PublicizedFrom(EAccessModifier.Private)]
	public string enabledColor;

	// Token: 0x040059BF RID: 22975
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor;

	// Token: 0x040059C0 RID: 22976
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x040059C1 RID: 22977
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x040059C2 RID: 22978
	[PublicizedFrom(EAccessModifier.Private)]
	public string positiveColor = "0,0,255";

	// Token: 0x040059C3 RID: 22979
	[PublicizedFrom(EAccessModifier.Private)]
	public string negativeColor = "255,0,0";

	// Token: 0x040059C4 RID: 22980
	public new bool Selected;

	// Token: 0x040059C5 RID: 22981
	public bool IsHovered;

	// Token: 0x040059C6 RID: 22982
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchVote vote;

	// Token: 0x040059C8 RID: 22984
	public XUiC_TwitchVoteInfoEntryList Owner;
}
