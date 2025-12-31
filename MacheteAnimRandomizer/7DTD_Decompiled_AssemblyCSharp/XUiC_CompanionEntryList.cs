using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C5A RID: 3162
[Preserve]
public class XUiC_CompanionEntryList : XUiController
{
	// Token: 0x06006161 RID: 24929 RVA: 0x00277C68 File Offset: 0x00275E68
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_CompanionEntry>(null);
		XUiController[] array = childrenByType;
		for (int i = 0; i < array.Length; i++)
		{
			this.entryList.Add((XUiC_CompanionEntry)array[i]);
		}
		this.yOffset = (float)this.viewComponent.Position.y;
	}

	// Token: 0x06006162 RID: 24930 RVA: 0x00277CC0 File Offset: 0x00275EC0
	public override void OnOpen()
	{
		base.OnOpen();
		this.RefreshPartyList();
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		CompanionGroup companions = entityPlayer.Companions;
		companions.OnGroupChanged = (OnCompanionGroupChanged)Delegate.Combine(companions.OnGroupChanged, new OnCompanionGroupChanged(this.RefreshPartyList));
		entityPlayer.PartyJoined += this.Party_Changed;
		entityPlayer.PartyChanged += this.Party_Changed;
		entityPlayer.PartyLeave += this.Party_Changed;
		if (entityPlayer.Party != null)
		{
			entityPlayer.Party.PartyMemberAdded += this.Party_PartyMemberChanged;
			entityPlayer.Party.PartyMemberRemoved += this.Party_PartyMemberChanged;
			entityPlayer.Party.PartyLeaderChanged += this.Party_Changed;
		}
	}

	// Token: 0x06006163 RID: 24931 RVA: 0x00277D94 File Offset: 0x00275F94
	public override void OnClose()
	{
		base.OnClose();
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		CompanionGroup companions = entityPlayer.Companions;
		companions.OnGroupChanged = (OnCompanionGroupChanged)Delegate.Remove(companions.OnGroupChanged, new OnCompanionGroupChanged(this.RefreshPartyList));
		entityPlayer.PartyJoined -= this.Party_Changed;
		entityPlayer.PartyChanged -= this.Party_Changed;
		entityPlayer.PartyLeave -= this.Party_Changed;
		if (entityPlayer.Party != null)
		{
			entityPlayer.Party.PartyMemberAdded -= this.Party_PartyMemberChanged;
			entityPlayer.Party.PartyMemberRemoved -= this.Party_PartyMemberChanged;
			entityPlayer.Party.PartyLeaderChanged -= this.Party_Changed;
		}
	}

	// Token: 0x06006164 RID: 24932 RVA: 0x00277E62 File Offset: 0x00276062
	[PublicizedFrom(EAccessModifier.Private)]
	public void Party_Changed(Party _affectedParty, EntityPlayer _player)
	{
		this.RefreshPartyList();
	}

	// Token: 0x06006165 RID: 24933 RVA: 0x00277E62 File Offset: 0x00276062
	[PublicizedFrom(EAccessModifier.Private)]
	public void Party_PartyMemberChanged(EntityPlayer player)
	{
		this.RefreshPartyList();
	}

	// Token: 0x06006166 RID: 24934 RVA: 0x00277E6C File Offset: 0x0027606C
	public void RefreshPartyList()
	{
		int i = 0;
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		int num = 0;
		if (entityPlayer.Party != null)
		{
			num = (entityPlayer.Party.MemberList.Count - 1) * 40;
		}
		if (entityPlayer.Companions != null)
		{
			num += (entityPlayer.Companions.Count - 1) * 40;
		}
		this.viewComponent.Position = new Vector2i(this.viewComponent.Position.x, (int)this.yOffset - num);
		this.viewComponent.UiTransform.localPosition = new Vector3((float)this.viewComponent.Position.x, (float)this.viewComponent.Position.y);
		if (entityPlayer.Companions != null)
		{
			for (int j = 0; j < entityPlayer.Companions.Count; j++)
			{
				EntityAlive companion = entityPlayer.Companions[j];
				if (i >= this.entryList.Count)
				{
					IL_117:
					while (i < this.entryList.Count)
					{
						this.entryList[i].SetCompanion(null);
						i++;
					}
					return;
				}
				this.entryList[i++].SetCompanion(companion);
			}
			goto IL_117;
		}
		for (int k = 0; k < this.entryList.Count; k++)
		{
			this.entryList[k].SetCompanion(null);
		}
	}

	// Token: 0x0400493F RID: 18751
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_CompanionEntry> entryList = new List<XUiC_CompanionEntry>();

	// Token: 0x04004940 RID: 18752
	[PublicizedFrom(EAccessModifier.Private)]
	public float yOffset;
}
