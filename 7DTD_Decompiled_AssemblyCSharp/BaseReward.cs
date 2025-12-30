using System;
using System.IO;

// Token: 0x02000916 RID: 2326
public abstract class BaseReward
{
	// Token: 0x17000739 RID: 1849
	// (get) Token: 0x0600454D RID: 17741 RVA: 0x001BC162 File Offset: 0x001BA362
	// (set) Token: 0x0600454E RID: 17742 RVA: 0x001BC16A File Offset: 0x001BA36A
	public string ID { get; set; }

	// Token: 0x1700073A RID: 1850
	// (get) Token: 0x0600454F RID: 17743 RVA: 0x001BC173 File Offset: 0x001BA373
	// (set) Token: 0x06004550 RID: 17744 RVA: 0x001BC17B File Offset: 0x001BA37B
	public string Value { get; set; }

	// Token: 0x1700073B RID: 1851
	// (get) Token: 0x06004551 RID: 17745 RVA: 0x001BC184 File Offset: 0x001BA384
	// (set) Token: 0x06004552 RID: 17746 RVA: 0x001BC18C File Offset: 0x001BA38C
	public Quest OwnerQuest { get; set; }

	// Token: 0x1700073C RID: 1852
	// (get) Token: 0x06004553 RID: 17747 RVA: 0x001BC195 File Offset: 0x001BA395
	// (set) Token: 0x06004554 RID: 17748 RVA: 0x001BC1B2 File Offset: 0x001BA3B2
	public string Description
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupReward();
				this.displaySetup = true;
			}
			return this.description;
		}
		set
		{
			this.description = value;
		}
	}

	// Token: 0x1700073D RID: 1853
	// (get) Token: 0x06004555 RID: 17749 RVA: 0x001BC1BB File Offset: 0x001BA3BB
	// (set) Token: 0x06004556 RID: 17750 RVA: 0x001BC1D8 File Offset: 0x001BA3D8
	public string ValueText
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupReward();
				this.displaySetup = true;
			}
			return this.valueText;
		}
		set
		{
			this.valueText = value;
		}
	}

	// Token: 0x1700073E RID: 1854
	// (get) Token: 0x06004557 RID: 17751 RVA: 0x001BC1E1 File Offset: 0x001BA3E1
	// (set) Token: 0x06004558 RID: 17752 RVA: 0x001BC1FE File Offset: 0x001BA3FE
	public string Icon
	{
		get
		{
			if (!this.displaySetup)
			{
				this.SetupReward();
				this.displaySetup = true;
			}
			return this.icon;
		}
		set
		{
			this.icon = value;
		}
	}

	// Token: 0x1700073F RID: 1855
	// (get) Token: 0x06004559 RID: 17753 RVA: 0x001BC207 File Offset: 0x001BA407
	// (set) Token: 0x0600455A RID: 17754 RVA: 0x001BC20F File Offset: 0x001BA40F
	public string IconAtlas { get; set; }

	// Token: 0x17000740 RID: 1856
	// (get) Token: 0x0600455B RID: 17755 RVA: 0x001BC218 File Offset: 0x001BA418
	// (set) Token: 0x0600455C RID: 17756 RVA: 0x001BC220 File Offset: 0x001BA420
	public bool HiddenReward { get; set; }

	// Token: 0x17000741 RID: 1857
	// (get) Token: 0x0600455D RID: 17757 RVA: 0x001BC229 File Offset: 0x001BA429
	// (set) Token: 0x0600455E RID: 17758 RVA: 0x001BC231 File Offset: 0x001BA431
	public bool Optional { get; set; }

	// Token: 0x17000742 RID: 1858
	// (get) Token: 0x0600455F RID: 17759 RVA: 0x001BC23A File Offset: 0x001BA43A
	// (set) Token: 0x06004560 RID: 17760 RVA: 0x001BC242 File Offset: 0x001BA442
	public bool isChosenReward { get; set; }

	// Token: 0x17000743 RID: 1859
	// (get) Token: 0x06004561 RID: 17761 RVA: 0x001BC24B File Offset: 0x001BA44B
	// (set) Token: 0x06004562 RID: 17762 RVA: 0x001BC253 File Offset: 0x001BA453
	public bool isChainReward { get; set; }

	// Token: 0x17000744 RID: 1860
	// (get) Token: 0x06004563 RID: 17763 RVA: 0x001BC25C File Offset: 0x001BA45C
	// (set) Token: 0x06004564 RID: 17764 RVA: 0x001BC264 File Offset: 0x001BA464
	public bool isFixedLocation { get; set; }

	// Token: 0x17000745 RID: 1861
	// (get) Token: 0x06004565 RID: 17765 RVA: 0x001BC26D File Offset: 0x001BA46D
	// (set) Token: 0x06004566 RID: 17766 RVA: 0x001BC275 File Offset: 0x001BA475
	public BaseReward.ReceiveStages ReceiveStage { get; set; }

	// Token: 0x17000746 RID: 1862
	// (get) Token: 0x06004567 RID: 17767 RVA: 0x001BC27E File Offset: 0x001BA47E
	// (set) Token: 0x06004568 RID: 17768 RVA: 0x001BC286 File Offset: 0x001BA486
	public byte RewardIndex { get; set; }

	// Token: 0x06004569 RID: 17769 RVA: 0x001BC290 File Offset: 0x001BA490
	public BaseReward()
	{
		this.IconAtlas = "UIAtlas";
		this.ReceiveStage = BaseReward.ReceiveStages.QuestCompletion;
		this.isFixedLocation = false;
	}

	// Token: 0x0600456A RID: 17770 RVA: 0x001BC2E0 File Offset: 0x001BA4E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CopyValues(BaseReward reward)
	{
		reward.ID = this.ID;
		reward.Value = this.Value;
		reward.ReceiveStage = this.ReceiveStage;
		reward.HiddenReward = this.HiddenReward;
		reward.Optional = this.Optional;
		reward.isChosenReward = this.isChosenReward;
		reward.isChainReward = this.isChainReward;
		reward.isFixedLocation = this.isFixedLocation;
		reward.RewardIndex = this.RewardIndex;
	}

	// Token: 0x0600456B RID: 17771 RVA: 0x001BC359 File Offset: 0x001BA559
	public virtual void HandleVariables()
	{
		this.ID = this.OwnerQuest.ParseVariable(this.ID);
		this.Value = this.OwnerQuest.ParseVariable(this.Value);
	}

	// Token: 0x0600456C RID: 17772 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupReward()
	{
	}

	// Token: 0x0600456D RID: 17773 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void GiveReward(EntityPlayer player)
	{
	}

	// Token: 0x0600456E RID: 17774 RVA: 0x001BC389 File Offset: 0x001BA589
	public void GiveReward()
	{
		this.GiveReward(this.OwnerQuest.OwnerJournal.OwnerPlayer);
	}

	// Token: 0x0600456F RID: 17775 RVA: 0x001BC3A1 File Offset: 0x001BA5A1
	public virtual ItemStack GetRewardItem()
	{
		return ItemStack.Empty;
	}

	// Token: 0x06004570 RID: 17776 RVA: 0x00019766 File Offset: 0x00017966
	public virtual BaseReward Clone()
	{
		return null;
	}

	// Token: 0x06004571 RID: 17777 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupGlobalRewardSettings()
	{
	}

	// Token: 0x06004572 RID: 17778 RVA: 0x001BC3A8 File Offset: 0x001BA5A8
	public virtual void Read(BinaryReader _br)
	{
		this.RewardIndex = _br.ReadByte();
	}

	// Token: 0x06004573 RID: 17779 RVA: 0x001BC3B6 File Offset: 0x001BA5B6
	public virtual void Write(BinaryWriter _bw)
	{
		_bw.Write(this.RewardIndex);
	}

	// Token: 0x06004574 RID: 17780 RVA: 0x001BC3C4 File Offset: 0x001BA5C4
	public virtual void ParseProperties(DynamicProperties properties)
	{
		if (properties.Values.ContainsKey(BaseReward.PropID))
		{
			this.ID = properties.Values[BaseReward.PropID];
		}
		if (properties.Values.ContainsKey(BaseReward.PropValue))
		{
			this.Value = properties.Values[BaseReward.PropValue];
		}
		if (properties.Values.ContainsKey(BaseReward.PropReceiveStage))
		{
			string a = properties.Values[BaseReward.PropReceiveStage];
			if (!(a == "start"))
			{
				if (!(a == "complete"))
				{
					if (a == "aftercomplete")
					{
						this.ReceiveStage = BaseReward.ReceiveStages.AfterCompleteNotification;
					}
				}
				else
				{
					this.ReceiveStage = BaseReward.ReceiveStages.QuestCompletion;
				}
			}
			else
			{
				this.ReceiveStage = BaseReward.ReceiveStages.QuestStart;
			}
		}
		if (properties.Values.ContainsKey(BaseReward.PropOptional))
		{
			bool optional;
			StringParsers.TryParseBool(properties.Values[BaseReward.PropOptional], out optional, 0, -1, true);
			this.Optional = optional;
		}
		if (properties.Values.ContainsKey(BaseReward.PropHidden))
		{
			bool hiddenReward;
			StringParsers.TryParseBool(properties.Values[BaseReward.PropHidden], out hiddenReward, 0, -1, true);
			this.HiddenReward = hiddenReward;
		}
		if (properties.Values.ContainsKey(BaseReward.PropIsChosen))
		{
			bool isChosenReward;
			StringParsers.TryParseBool(properties.Values[BaseReward.PropIsChosen], out isChosenReward, 0, -1, true);
			this.isChosenReward = isChosenReward;
		}
		if (properties.Values.ContainsKey(BaseReward.PropIsFixed))
		{
			bool isFixedLocation;
			StringParsers.TryParseBool(properties.Values[BaseReward.PropIsFixed], out isFixedLocation, 0, -1, true);
			this.isFixedLocation = isFixedLocation;
		}
		if (properties.Values.ContainsKey(BaseReward.PropIsChain))
		{
			bool isChainReward;
			StringParsers.TryParseBool(properties.Values[BaseReward.PropIsChain], out isChainReward, 0, -1, true);
			this.isChainReward = isChainReward;
		}
	}

	// Token: 0x06004575 RID: 17781 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string GetRewardText()
	{
		return "";
	}

	// Token: 0x04003640 RID: 13888
	[PublicizedFrom(EAccessModifier.Private)]
	public bool displaySetup;

	// Token: 0x04003641 RID: 13889
	[PublicizedFrom(EAccessModifier.Private)]
	public string description = "";

	// Token: 0x04003642 RID: 13890
	[PublicizedFrom(EAccessModifier.Private)]
	public string valueText = "";

	// Token: 0x04003643 RID: 13891
	[PublicizedFrom(EAccessModifier.Private)]
	public string icon = "";

	// Token: 0x0400364F RID: 13903
	public static string PropID = "id";

	// Token: 0x04003650 RID: 13904
	public static string PropValue = "value";

	// Token: 0x04003651 RID: 13905
	public static string PropOptional = "optional";

	// Token: 0x04003652 RID: 13906
	public static string PropReceiveStage = "stage";

	// Token: 0x04003653 RID: 13907
	public static string PropHidden = "hidden";

	// Token: 0x04003654 RID: 13908
	public static string PropIsChosen = "ischosen";

	// Token: 0x04003655 RID: 13909
	public static string PropIsChain = "chainreward";

	// Token: 0x04003656 RID: 13910
	public static string PropIsFixed = "isfixed";

	// Token: 0x02000917 RID: 2327
	public enum RewardTypes
	{
		// Token: 0x04003658 RID: 13912
		Exp,
		// Token: 0x04003659 RID: 13913
		Item,
		// Token: 0x0400365A RID: 13914
		Level,
		// Token: 0x0400365B RID: 13915
		Quest,
		// Token: 0x0400365C RID: 13916
		Recipe,
		// Token: 0x0400365D RID: 13917
		ShowTip,
		// Token: 0x0400365E RID: 13918
		Skill,
		// Token: 0x0400365F RID: 13919
		SkillPoints
	}

	// Token: 0x02000918 RID: 2328
	public enum ReceiveStages
	{
		// Token: 0x04003661 RID: 13921
		QuestStart,
		// Token: 0x04003662 RID: 13922
		QuestCompletion,
		// Token: 0x04003663 RID: 13923
		AfterCompleteNotification
	}
}
