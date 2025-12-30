using System;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C10 RID: 3088
[Preserve]
public class XUiC_BlockedPlayersList : XUiController
{
	// Token: 0x06005ECB RID: 24267 RVA: 0x00266CFC File Offset: 0x00264EFC
	public override void Init()
	{
		base.Init();
		this.noClick = (XUiV_Panel)base.GetChildById("noClick").ViewComponent;
		this.blockedPlayerList = (XUiV_Grid)base.GetChildById("blockList").ViewComponent;
		this.blockedEntries = base.GetChildrenByType<XUiC_PlayersBlockedListEntry>(null);
		this.blockedPager = (XUiC_Paging)base.GetChildById("blockedPager");
		this.blockedPager.OnPageChanged += this.updateBlockedList;
		this.blockedCounter = (XUiV_Label)base.GetChildById("blockedCounter").ViewComponent;
		this.recentPlayerList = (XUiV_Grid)base.GetChildById("recentList").ViewComponent;
		this.recentEntries = base.GetChildrenByType<XUiC_PlayersRecentListEntry>(null);
		this.recentPager = (XUiC_Paging)base.GetChildById("recentPager");
		this.recentPager.OnPageChanged += this.updateRecentList;
		this.recentCounter = (XUiV_Label)base.GetChildById("recentCounter").ViewComponent;
		for (int i = 0; i < this.blockedEntries.Length; i++)
		{
			this.blockedEntries[i].BlockList = this;
			this.blockedEntries[i].IsAlternating = (i % 2 == 0);
		}
		for (int j = 0; j < this.recentEntries.Length; j++)
		{
			this.recentEntries[j].BlockList = this;
			this.recentEntries[j].IsAlternating = (j % 2 == 0);
		}
	}

	// Token: 0x06005ECC RID: 24268 RVA: 0x00266E74 File Offset: 0x00265074
	public override void OnOpen()
	{
		base.OnOpen();
		if (BlockedPlayerList.Instance == null)
		{
			return;
		}
		this.noClick.Enabled = false;
		this.blockedPager.Reset();
		this.recentPager.Reset();
		BlockedPlayerList.Instance.UpdatePlayersSeenInWorld(GameManager.Instance.World);
		ThreadManager.StartCoroutine(BlockedPlayerList.Instance.ResolveUserDetails());
		this.updateBlockedList();
		this.updateRecentList();
	}

	// Token: 0x06005ECD RID: 24269 RVA: 0x00266EE1 File Offset: 0x002650E1
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (BlockedPlayerList.Instance == null)
		{
			return;
		}
		if (!this.IsDirty)
		{
			return;
		}
		this.IsDirty = false;
		this.updateBlockedList();
		this.updateRecentList();
		base.RefreshBindings(false);
	}

	// Token: 0x06005ECE RID: 24270 RVA: 0x00266F18 File Offset: 0x00265118
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (!(_bindingName == "blockedCount"))
		{
			if (!(_bindingName == "recentCount"))
			{
				return base.GetBindingValueInternal(ref _value, _bindingName);
			}
			if (BlockedPlayerList.Instance != null)
			{
				_value = string.Format("{0}/{1}", BlockedPlayerList.Instance.EntryCount(false, false), 100);
				return true;
			}
			return false;
		}
		else
		{
			if (BlockedPlayerList.Instance != null)
			{
				_value = string.Format("{0}/{1}", BlockedPlayerList.Instance.EntryCount(true, false), 500);
				return true;
			}
			return false;
		}
	}

	// Token: 0x06005ECF RID: 24271 RVA: 0x00266FAC File Offset: 0x002651AC
	public void DisplayMessage(string _header, string _message)
	{
		this.noClick.Enabled = true;
		XUiC_MessageBoxWindowGroup.ShowMessageBox(base.xui, _header, _message, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, new Action(this.<DisplayMessage>g__DisableNoClick|13_0), new Action(this.<DisplayMessage>g__DisableNoClick|13_0), false, true, false);
	}

	// Token: 0x06005ED0 RID: 24272 RVA: 0x00266FF0 File Offset: 0x002651F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBlockedList()
	{
		for (int i = 0; i < this.blockedPlayerList.Rows; i++)
		{
			this.blockedEntries[i].Clear();
		}
		if (BlockedPlayerList.Instance.PendingResolve())
		{
			this.blockedEntries[0].PlayerName.SetGenericName(Localization.Get("xuiFetchingData", false));
			this.IsDirty = true;
			return;
		}
		this.blockedPager.SetLastPageByElementsAndPageLength(BlockedPlayerList.Instance.EntryCount(true, true), this.blockedPlayerList.Rows);
		int num = this.blockedPlayerList.Rows * this.blockedPager.GetPage();
		int num2 = 0;
		foreach (BlockedPlayerList.ListEntry listEntry in BlockedPlayerList.Instance.GetEntriesOrdered(true, true))
		{
			if (num2 < num)
			{
				num2++;
			}
			else
			{
				int num3 = num2 - num;
				if (num3 >= this.blockedPlayerList.Rows)
				{
					break;
				}
				this.blockedEntries[num3].UpdateEntry(listEntry.PlayerData.PrimaryId);
				num2++;
			}
		}
		for (int j = num2 - num; j < this.blockedPlayerList.Rows; j++)
		{
			this.blockedEntries[j].Clear();
		}
	}

	// Token: 0x06005ED1 RID: 24273 RVA: 0x00267138 File Offset: 0x00265338
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateRecentList()
	{
		for (int i = 0; i < this.recentPlayerList.Rows; i++)
		{
			this.recentEntries[i].Clear();
		}
		if (BlockedPlayerList.Instance.PendingResolve())
		{
			this.recentEntries[0].PlayerName.SetGenericName(Localization.Get("xuiFetchingData", false));
			this.IsDirty = true;
			return;
		}
		this.recentPager.SetLastPageByElementsAndPageLength(BlockedPlayerList.Instance.EntryCount(false, true), this.recentPlayerList.Rows);
		int num = this.recentPlayerList.Rows * this.recentPager.GetPage();
		int num2 = 0;
		foreach (BlockedPlayerList.ListEntry listEntry in BlockedPlayerList.Instance.GetEntriesOrdered(false, true))
		{
			if (num2 < num)
			{
				num2++;
			}
			else
			{
				int num3 = num2 - num;
				if (num3 >= this.recentPlayerList.Rows)
				{
					break;
				}
				this.recentEntries[num3].UpdateEntry(listEntry.PlayerData.PrimaryId);
				num2++;
			}
		}
		for (int j = num2 - num; j < this.recentPlayerList.Rows; j++)
		{
			this.recentEntries[j].Clear();
		}
	}

	// Token: 0x06005ED3 RID: 24275 RVA: 0x00267280 File Offset: 0x00265480
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <DisplayMessage>g__DisableNoClick|13_0()
	{
		this.noClick.Enabled = false;
	}

	// Token: 0x0400477C RID: 18300
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel noClick;

	// Token: 0x0400477D RID: 18301
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Grid blockedPlayerList;

	// Token: 0x0400477E RID: 18302
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PlayersBlockedListEntry[] blockedEntries;

	// Token: 0x0400477F RID: 18303
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging blockedPager;

	// Token: 0x04004780 RID: 18304
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label blockedCounter;

	// Token: 0x04004781 RID: 18305
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Grid recentPlayerList;

	// Token: 0x04004782 RID: 18306
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PlayersRecentListEntry[] recentEntries;

	// Token: 0x04004783 RID: 18307
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging recentPager;

	// Token: 0x04004784 RID: 18308
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label recentCounter;
}
