using System;
using System.Collections.Generic;
using Challenges;
using UnityEngine.Scripting;

// Token: 0x02000C25 RID: 3109
[Preserve]
public class XUiC_ChallengeEntryList : XUiController
{
	// Token: 0x170009DD RID: 2525
	// (get) Token: 0x06005F9B RID: 24475 RVA: 0x0026CD64 File Offset: 0x0026AF64
	// (set) Token: 0x06005F9C RID: 24476 RVA: 0x0026CD6C File Offset: 0x0026AF6C
	public XUiC_ChallengeEntry SelectedEntry
	{
		get
		{
			return this.selectedEntry;
		}
		set
		{
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = false;
			}
			this.selectedEntry = value;
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = true;
			}
			this.journalWindowGroup.SetEntry(this.selectedEntry);
		}
	}

	// Token: 0x06005F9D RID: 24477 RVA: 0x0026CDBC File Offset: 0x0026AFBC
	public override void Init()
	{
		base.Init();
		this.journalWindowGroup = (XUiC_ChallengeWindowGroup)base.WindowGroup.Controller;
		for (int i = 0; i < this.children.Count; i++)
		{
			if (this.children[i] is XUiC_ChallengeEntry)
			{
				XUiC_ChallengeEntry xuiC_ChallengeEntry = (XUiC_ChallengeEntry)this.children[i];
				xuiC_ChallengeEntry.Owner = this;
				xuiC_ChallengeEntry.JournalUIHandler = this.journalWindowGroup;
				this.entryList.Add(xuiC_ChallengeEntry);
			}
		}
	}

	// Token: 0x06005F9E RID: 24478 RVA: 0x0026CE40 File Offset: 0x0026B040
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			ChallengeObjectiveChallengeComplete challengeObjectiveChallengeComplete = null;
			string b = "";
			string b2 = "";
			if (base.xui.QuestTracker.TrackedChallenge != null)
			{
				challengeObjectiveChallengeComplete = base.xui.QuestTracker.TrackedChallenge.GetChallengeCompleteObjective();
				if (challengeObjectiveChallengeComplete != null && challengeObjectiveChallengeComplete.IsRedeemed)
				{
					if (challengeObjectiveChallengeComplete.IsGroup)
					{
						b2 = challengeObjectiveChallengeComplete.ChallengeName;
					}
					else
					{
						b = challengeObjectiveChallengeComplete.ChallengeName;
					}
				}
				else
				{
					challengeObjectiveChallengeComplete = null;
				}
			}
			for (int i = 0; i < this.entryList.Count; i++)
			{
				XUiC_ChallengeEntry xuiC_ChallengeEntry = this.entryList[i];
				if (xuiC_ChallengeEntry != null)
				{
					xuiC_ChallengeEntry.OnPress -= this.OnPressEntry;
					xuiC_ChallengeEntry.Selected = (this.selectedEntry == xuiC_ChallengeEntry || (this.selectedEntry == null && xuiC_ChallengeEntry.Tracked));
					if (i < this.challengeList.Count)
					{
						xuiC_ChallengeEntry.Entry = this.challengeList[i];
						if (xuiC_ChallengeEntry.IsChallengeVisible)
						{
							xuiC_ChallengeEntry.OnPress += this.OnPressEntry;
							xuiC_ChallengeEntry.ViewComponent.SoundPlayOnClick = true;
							xuiC_ChallengeEntry.ViewComponent.SoundPlayOnHover = true;
						}
						else
						{
							xuiC_ChallengeEntry.ViewComponent.SoundPlayOnClick = false;
							xuiC_ChallengeEntry.ViewComponent.SoundPlayOnHover = false;
						}
						xuiC_ChallengeEntry.IsRedeemBlinking = false;
						if (xuiC_ChallengeEntry.Entry.ChallengeState == Challenge.ChallengeStates.Completed && challengeObjectiveChallengeComplete != null && (xuiC_ChallengeEntry.Entry.ChallengeClass.Name.EqualsCaseInsensitive(b) || xuiC_ChallengeEntry.Entry.ChallengeGroup.Name.EqualsCaseInsensitive(b2)))
						{
							xuiC_ChallengeEntry.IsRedeemBlinking = true;
						}
						if (xuiC_ChallengeEntry.Entry.IsTracked && this.selectedEntry == null)
						{
							this.Owner.Select();
							this.SelectedEntry = xuiC_ChallengeEntry;
							this.journalWindowGroup.SetEntry(this.selectedEntry);
							xuiC_ChallengeEntry.SelectCursorElement(true, false);
						}
					}
					else
					{
						xuiC_ChallengeEntry.Entry = null;
						xuiC_ChallengeEntry.ViewComponent.SoundPlayOnClick = false;
					}
				}
			}
			this.isDirty = false;
		}
	}

	// Token: 0x06005F9F RID: 24479 RVA: 0x0026D052 File Offset: 0x0026B252
	public void MarkDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x06005FA0 RID: 24480 RVA: 0x0026D05B File Offset: 0x0026B25B
	public void UnSelect()
	{
		this.SelectedEntry = null;
	}

	// Token: 0x06005FA1 RID: 24481 RVA: 0x0026D064 File Offset: 0x0026B264
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressEntry(XUiController _sender, int _mouseButton)
	{
		XUiC_ChallengeEntry xuiC_ChallengeEntry = _sender as XUiC_ChallengeEntry;
		if (xuiC_ChallengeEntry != null)
		{
			this.Owner.Select();
			this.SelectedEntry = xuiC_ChallengeEntry;
			this.SelectedEntry.JournalUIHandler.SetEntry(this.SelectedEntry);
			if (InputUtils.ShiftKeyPressed)
			{
				Challenge entry = xuiC_ChallengeEntry.Entry;
				if (entry.IsActive && !entry.IsTracked)
				{
					entry.IsTracked = true;
					base.xui.QuestTracker.TrackedChallenge = entry;
				}
			}
			this.isDirty = true;
		}
	}

	// Token: 0x06005FA2 RID: 24482 RVA: 0x0026D0E0 File Offset: 0x0026B2E0
	public void SetChallengeEntryList(List<Challenge> newChallengeList)
	{
		this.challengeList = newChallengeList;
		this.isDirty = true;
	}

	// Token: 0x06005FA3 RID: 24483 RVA: 0x0026D0F0 File Offset: 0x0026B2F0
	public void SetEntryByChallenge(Challenge newChallenge)
	{
		for (int i = 0; i < this.entryList.Count; i++)
		{
			XUiC_ChallengeEntry xuiC_ChallengeEntry = this.entryList[i];
			if (xuiC_ChallengeEntry != null && xuiC_ChallengeEntry.Entry == newChallenge)
			{
				this.SelectedEntry = xuiC_ChallengeEntry;
				return;
			}
		}
	}

	// Token: 0x06005FA4 RID: 24484 RVA: 0x0026D134 File Offset: 0x0026B334
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
	}

	// Token: 0x06005FA5 RID: 24485 RVA: 0x0026D152 File Offset: 0x0026B352
	public override void OnClose()
	{
		base.OnClose();
		this.SelectedEntry = null;
	}

	// Token: 0x04004808 RID: 18440
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04004809 RID: 18441
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_ChallengeEntry> entryList = new List<XUiC_ChallengeEntry>();

	// Token: 0x0400480A RID: 18442
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400480B RID: 18443
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ChallengeEntry selectedEntry;

	// Token: 0x0400480C RID: 18444
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Challenge> challengeList;

	// Token: 0x0400480D RID: 18445
	public XUiC_ChallengeEntryListWindow ChallengeEntryListWindow;

	// Token: 0x0400480E RID: 18446
	public XUiC_ChallengeGroupEntry Owner;

	// Token: 0x0400480F RID: 18447
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ChallengeWindowGroup journalWindowGroup;
}
