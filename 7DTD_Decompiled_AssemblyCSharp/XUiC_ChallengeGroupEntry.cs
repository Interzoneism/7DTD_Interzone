using System;
using System.Collections.Generic;
using Challenges;
using UniLinq;
using UnityEngine.Scripting;

// Token: 0x02000C27 RID: 3111
[Preserve]
public class XUiC_ChallengeGroupEntry : XUiController
{
	// Token: 0x170009DE RID: 2526
	// (get) Token: 0x06005FAF RID: 24495 RVA: 0x0026D48C File Offset: 0x0026B68C
	// (set) Token: 0x06005FB0 RID: 24496 RVA: 0x0026D494 File Offset: 0x0026B694
	public ChallengeGroupEntry Entry
	{
		get
		{
			return this.entry;
		}
		set
		{
			base.ViewComponent.Enabled = (value != null);
			this.entry = value;
			this.group = ((this.entry != null) ? this.entry.ChallengeGroup : null);
			this.IsDirty = true;
		}
	}

	// Token: 0x06005FB1 RID: 24497 RVA: 0x0026D4CF File Offset: 0x0026B6CF
	public override void Init()
	{
		base.Init();
		this.ChallengeList = base.GetChildByType<XUiC_ChallengeEntryList>();
		this.IsDirty = true;
	}

	// Token: 0x06005FB2 RID: 24498 RVA: 0x0026D4EC File Offset: 0x0026B6EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.group != null;
		if (bindingName == "groupname")
		{
			value = "";
			if (flag)
			{
				value = this.group.Title;
			}
			return true;
		}
		if (bindingName == "groupreward")
		{
			value = (flag ? this.group.RewardText : "");
			return true;
		}
		if (bindingName == "groupobjective")
		{
			value = (flag ? this.group.ObjectiveText : "");
			return true;
		}
		if (bindingName == "resetday")
		{
			if (flag)
			{
				value = ((this.group.DayReset == -1) ? "" : this.entry.LastUpdateDay.ToString());
			}
			else
			{
				value = "";
			}
			return true;
		}
		if (bindingName == "hasreset")
		{
			value = (flag ? (this.group.DayReset != -1).ToString() : "false");
			return true;
		}
		if (!(bindingName == "hasentry"))
		{
			return false;
		}
		value = (flag ? "true" : "false");
		return true;
	}

	// Token: 0x06005FB3 RID: 24499 RVA: 0x0026D618 File Offset: 0x0026B818
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.group != null && this.group.UIDirty)
		{
			this.IsDirty = true;
			this.group.UIDirty = false;
		}
		if (this.IsDirty)
		{
			this.currentItems = (from item in this.player.challengeJournal.Challenges
			where item.ChallengeGroup == this.@group
			select item).ToList<Challenge>();
			if (this.ChallengeList != null)
			{
				this.ChallengeList.Owner = this;
				this.ChallengeList.SetChallengeEntryList(this.currentItems);
			}
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x06005FB4 RID: 24500 RVA: 0x0026D6BB File Offset: 0x0026B8BB
	public void Select()
	{
		this.Owner.SelectedGroup = this;
	}

	// Token: 0x06005FB5 RID: 24501 RVA: 0x0026D6C9 File Offset: 0x0026B8C9
	public void UnSelect()
	{
		this.ChallengeList.UnSelect();
	}

	// Token: 0x06005FB6 RID: 24502 RVA: 0x0026D6D6 File Offset: 0x0026B8D6
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		this.IsDirty = true;
	}

	// Token: 0x06005FB7 RID: 24503 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x04004817 RID: 18455
	public bool IsHovered;

	// Token: 0x04004818 RID: 18456
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04004819 RID: 18457
	public XUiC_ChallengeEntryList ChallengeList;

	// Token: 0x0400481A RID: 18458
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Challenge> currentItems;

	// Token: 0x0400481B RID: 18459
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeGroup group;

	// Token: 0x0400481C RID: 18460
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeGroupEntry entry;

	// Token: 0x0400481D RID: 18461
	public XUiC_ChallengeGroupList Owner;
}
