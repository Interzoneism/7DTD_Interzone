using System;
using System.Collections.Generic;
using Challenges;
using UnityEngine.Scripting;

// Token: 0x02000C28 RID: 3112
[Preserve]
public class XUiC_ChallengeGroupList : XUiController
{
	// Token: 0x170009DF RID: 2527
	// (get) Token: 0x06005FBA RID: 24506 RVA: 0x0026D70B File Offset: 0x0026B90B
	// (set) Token: 0x06005FBB RID: 24507 RVA: 0x0026D713 File Offset: 0x0026B913
	public XUiC_ChallengeGroupEntry SelectedGroup
	{
		get
		{
			return this.selectedGroup;
		}
		set
		{
			if (this.selectedGroup != null)
			{
				this.selectedGroup.UnSelect();
			}
			this.selectedGroup = value;
		}
	}

	// Token: 0x06005FBC RID: 24508 RVA: 0x0026D730 File Offset: 0x0026B930
	public override void Init()
	{
		base.Init();
		XUiC_ChallengeWindowGroup xuiC_ChallengeWindowGroup = (XUiC_ChallengeWindowGroup)base.WindowGroup.Controller;
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] is XUiC_ChallengeGroupEntry)
			{
				XUiC_ChallengeGroupEntry xuiC_ChallengeGroupEntry = (XUiC_ChallengeGroupEntry)this.children[i];
				xuiC_ChallengeGroupEntry.Owner = this;
				this.entryList.Add(xuiC_ChallengeGroupEntry);
			}
		}
	}

	// Token: 0x06005FBD RID: 24509 RVA: 0x0026D7A4 File Offset: 0x0026B9A4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.CategoryList.CurrentCategory == null)
		{
			return;
		}
		string categoryName = this.CategoryList.CurrentCategory.CategoryName;
		if (this.isDirty)
		{
			int num = 0;
			for (int i = 0; i < this.challengeGroupList.Count; i++)
			{
				XUiC_ChallengeGroupEntry xuiC_ChallengeGroupEntry = this.entryList[num];
				if (xuiC_ChallengeGroupEntry != null && this.challengeGroupList[i].ChallengeGroup.Category.EqualsCaseInsensitive(categoryName))
				{
					xuiC_ChallengeGroupEntry.Entry = this.challengeGroupList[i];
					xuiC_ChallengeGroupEntry.ViewComponent.SoundPlayOnClick = true;
					if (this.categoryChange)
					{
						xuiC_ChallengeGroupEntry.ChallengeList.SelectedEntry = null;
					}
					num++;
				}
				if (num >= this.entryList.Count)
				{
					break;
				}
			}
			for (int j = num; j < this.entryList.Count; j++)
			{
				XUiC_ChallengeGroupEntry xuiC_ChallengeGroupEntry2 = this.entryList[j];
				xuiC_ChallengeGroupEntry2.Entry = null;
				xuiC_ChallengeGroupEntry2.ViewComponent.SoundPlayOnClick = false;
			}
			this.isDirty = false;
			this.categoryChange = false;
		}
	}

	// Token: 0x06005FBE RID: 24510 RVA: 0x0026D8B8 File Offset: 0x0026BAB8
	public void SetChallengeGroupEntryList(List<ChallengeGroupEntry> newChallengeGroupList, bool newCategoryChange)
	{
		this.challengeGroupList = newChallengeGroupList;
		if (this.CategoryList != null && this.CategoryList.CurrentCategory == null)
		{
			this.CategoryList.SetCategoryToFirst();
		}
		this.categoryChange = newCategoryChange;
		if (base.xui.QuestTracker.TrackedChallenge != null)
		{
			ChallengeGroup challengeGroup = base.xui.QuestTracker.TrackedChallenge.ChallengeGroup;
			int num = 0;
			while (num < this.challengeGroupList.Count && this.challengeGroupList[num].ChallengeGroup != challengeGroup)
			{
				num++;
			}
		}
		this.isDirty = true;
	}

	// Token: 0x06005FBF RID: 24511 RVA: 0x0026D94C File Offset: 0x0026BB4C
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "display_key")
		{
			this.DisplayKey = value;
			return true;
		}
		return base.ParseAttribute(name, value, _parent);
	}

	// Token: 0x06005FC0 RID: 24512 RVA: 0x0026D96D File Offset: 0x0026BB6D
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
	}

	// Token: 0x0400481E RID: 18462
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x0400481F RID: 18463
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_ChallengeGroupEntry> entryList = new List<XUiC_ChallengeGroupEntry>();

	// Token: 0x04004820 RID: 18464
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04004821 RID: 18465
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ChallengeGroupEntry selectedGroup;

	// Token: 0x04004822 RID: 18466
	[PublicizedFrom(EAccessModifier.Private)]
	public bool categoryChange;

	// Token: 0x04004823 RID: 18467
	public string DisplayKey = "";

	// Token: 0x04004824 RID: 18468
	public XUiC_CategoryList CategoryList;

	// Token: 0x04004825 RID: 18469
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ChallengeGroupEntry> challengeGroupList;

	// Token: 0x04004826 RID: 18470
	public XUiC_ChallengeEntryListWindow ChallengeEntryListWindow;
}
