using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E8F RID: 3727
[Preserve]
public class XUiC_TwitchLeaderboardEntry : XUiController
{
	// Token: 0x17000BF5 RID: 3061
	// (get) Token: 0x06007581 RID: 30081 RVA: 0x002FDB41 File Offset: 0x002FBD41
	// (set) Token: 0x06007582 RID: 30082 RVA: 0x002FDB49 File Offset: 0x002FBD49
	public TwitchLeaderboardEntry LeaderboardEntry
	{
		get
		{
			return this.leaderboardEntry;
		}
		set
		{
			base.ViewComponent.Enabled = (value != null);
			this.leaderboardEntry = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BF6 RID: 3062
	// (get) Token: 0x06007583 RID: 30083 RVA: 0x002FDB68 File Offset: 0x002FBD68
	// (set) Token: 0x06007584 RID: 30084 RVA: 0x002FDB70 File Offset: 0x002FBD70
	public XUiC_TwitchInfoWindowGroup TwitchInfoUIHandler { get; set; }

	// Token: 0x06007585 RID: 30085 RVA: 0x002FDB7C File Offset: 0x002FBD7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.leaderboardEntry != null;
		if (bindingName == "username")
		{
			value = (flag ? string.Format("[{0}]{1}[-]", this.leaderboardEntry.UserColor, this.leaderboardEntry.UserName) : "");
			return true;
		}
		if (!(bindingName == "kills"))
		{
			return false;
		}
		value = (flag ? this.leaderboardEntry.Kills.ToString() : "");
		return true;
	}

	// Token: 0x06007586 RID: 30086 RVA: 0x00284555 File Offset: 0x00282755
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
	}

	// Token: 0x06007587 RID: 30087 RVA: 0x00284594 File Offset: 0x00282794
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
	}

	// Token: 0x06007588 RID: 30088 RVA: 0x002FDBFC File Offset: 0x002FBDFC
	public override void OnCursorSelected()
	{
		base.OnCursorSelected();
		base.GetParentByType<XUiC_TwitchLeaderboardEntryList>().SelectedEntry = this;
	}

	// Token: 0x06007589 RID: 30089 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x0600758A RID: 30090 RVA: 0x002FDC10 File Offset: 0x002FBE10
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

	// Token: 0x0400599D RID: 22941
	[PublicizedFrom(EAccessModifier.Private)]
	public string enabledColor;

	// Token: 0x0400599E RID: 22942
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor;

	// Token: 0x0400599F RID: 22943
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x040059A0 RID: 22944
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x040059A1 RID: 22945
	[PublicizedFrom(EAccessModifier.Private)]
	public string positiveColor = "0,0,255";

	// Token: 0x040059A2 RID: 22946
	[PublicizedFrom(EAccessModifier.Private)]
	public string negativeColor = "255,0,0";

	// Token: 0x040059A3 RID: 22947
	public new bool Selected;

	// Token: 0x040059A4 RID: 22948
	public bool IsHovered;

	// Token: 0x040059A5 RID: 22949
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchLeaderboardEntry leaderboardEntry;

	// Token: 0x040059A7 RID: 22951
	public XUiC_TwitchLeaderboardEntryList Owner;
}
