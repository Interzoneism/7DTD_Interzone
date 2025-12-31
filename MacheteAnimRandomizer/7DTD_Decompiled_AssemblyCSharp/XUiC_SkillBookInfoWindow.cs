using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000E30 RID: 3632
[Preserve]
public class XUiC_SkillBookInfoWindow : XUiC_InfoWindow
{
	// Token: 0x17000B7C RID: 2940
	// (get) Token: 0x060071E0 RID: 29152 RVA: 0x002E6329 File Offset: 0x002E4529
	public ProgressionValue CurrentSkill
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (base.xui.selectedSkill == null || !base.xui.selectedSkill.ProgressionClass.IsBookGroup)
			{
				return null;
			}
			return base.xui.selectedSkill;
		}
	}

	// Token: 0x060071E1 RID: 29153 RVA: 0x002E635C File Offset: 0x002E455C
	public override void Init()
	{
		base.Init();
		base.GetChildrenByType<XUiC_SkillBookLevel>(this.perkEntries);
		int num = 1;
		foreach (XUiC_SkillBookLevel xuiC_SkillBookLevel in this.perkEntries)
		{
			xuiC_SkillBookLevel.ListIndex = num - 1;
			xuiC_SkillBookLevel.HiddenEntriesWithPaging = this.hiddenEntriesWithPaging;
			xuiC_SkillBookLevel.MaxEntriesWithoutPaging = this.perkEntries.Count;
			xuiC_SkillBookLevel.OnScroll += this.Entry_OnScroll;
		}
		this.actionItemList = base.GetChildByType<XUiC_ItemActionList>();
		this.skillsPerPage = this.perkEntries.Count - this.hiddenEntriesWithPaging;
	}

	// Token: 0x060071E2 RID: 29154 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void SkillChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x060071E3 RID: 29155 RVA: 0x002E6418 File Offset: 0x002E4618
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSkill()
	{
		if (this.CurrentSkill != null && this.actionItemList != null)
		{
			this.actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
		}
		if (this.CurrentSkill != null)
		{
			base.xui.playerUI.entityPlayer.Progression.GetPerkList(this.perkList, this.CurrentSkill.Name);
		}
		XUiC_SkillEntry entryForSkill = this.windowGroup.Controller.GetChildByType<XUiC_SkillList>().GetEntryForSkill(this.CurrentSkill);
		int num = 0;
		foreach (XUiC_SkillBookLevel xuiC_SkillBookLevel in this.perkEntries)
		{
			if (num < this.perkList.Count)
			{
				xuiC_SkillBookLevel.Perk = this.perkList[num];
				xuiC_SkillBookLevel.Volume = num + 1;
				xuiC_SkillBookLevel.OnHover += this.Entry_OnHover;
				xuiC_SkillBookLevel.CompletionReward = (num == this.perkList.Count - 1);
				if (entryForSkill != null)
				{
					xuiC_SkillBookLevel.ViewComponent.NavLeftTarget = entryForSkill.ViewComponent;
				}
			}
			else
			{
				xuiC_SkillBookLevel.Perk = null;
				xuiC_SkillBookLevel.Volume = -1;
				xuiC_SkillBookLevel.OnHover -= this.Entry_OnHover;
				xuiC_SkillBookLevel.CompletionReward = false;
			}
			num++;
		}
	}

	// Token: 0x17000B7D RID: 2941
	// (get) Token: 0x060071E4 RID: 29156 RVA: 0x002E656C File Offset: 0x002E476C
	// (set) Token: 0x060071E5 RID: 29157 RVA: 0x002E6574 File Offset: 0x002E4774
	public ProgressionValue HoveredPerk
	{
		get
		{
			return this.hoveredPerk;
		}
		set
		{
			if (this.hoveredPerk != value)
			{
				this.hoveredPerk = value;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x060071E6 RID: 29158 RVA: 0x002E6590 File Offset: 0x002E4790
	[PublicizedFrom(EAccessModifier.Private)]
	public void Entry_OnHover(XUiController _sender, bool _isOver)
	{
		XUiC_SkillBookLevel xuiC_SkillBookLevel = _sender as XUiC_SkillBookLevel;
		if (_isOver && xuiC_SkillBookLevel != null)
		{
			this.HoveredPerk = xuiC_SkillBookLevel.Perk;
			return;
		}
		this.HoveredPerk = null;
	}

	// Token: 0x060071E7 RID: 29159 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void Pager_OnPageChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x060071E8 RID: 29160 RVA: 0x002E65BE File Offset: 0x002E47BE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Entry_OnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging == null)
			{
				return;
			}
			xuiC_Paging.PageDown();
			return;
		}
		else
		{
			XUiC_Paging xuiC_Paging2 = this.pager;
			if (xuiC_Paging2 == null)
			{
				return;
			}
			xuiC_Paging2.PageUp();
			return;
		}
	}

	// Token: 0x060071E9 RID: 29161 RVA: 0x002E65EB File Offset: 0x002E47EB
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.actionItemList != null)
		{
			this.actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
		}
		XUiEventManager.Instance.OnSkillExperienceAdded += this.Current_OnSkillExperienceAdded;
		this.IsDirty = true;
	}

	// Token: 0x060071EA RID: 29162 RVA: 0x002E6625 File Offset: 0x002E4825
	public override void OnClose()
	{
		base.OnClose();
		XUiEventManager.Instance.OnSkillExperienceAdded -= this.Current_OnSkillExperienceAdded;
	}

	// Token: 0x060071EB RID: 29163 RVA: 0x002E6643 File Offset: 0x002E4843
	public override void Update(float _dt)
	{
		if (this.IsDirty)
		{
			this.IsDirty = false;
			this.UpdateSkill();
			base.RefreshBindings(this.IsDirty);
		}
		base.Update(_dt);
	}

	// Token: 0x060071EC RID: 29164 RVA: 0x002E666D File Offset: 0x002E486D
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_OnSkillExperienceAdded(ProgressionValue _changedSkill, int _newXp)
	{
		if (this.CurrentSkill == _changedSkill)
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x060071ED RID: 29165 RVA: 0x002E6680 File Offset: 0x002E4880
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "hidden_entries_with_paging")
		{
			this.hiddenEntriesWithPaging = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			foreach (XUiC_SkillBookLevel xuiC_SkillBookLevel in this.perkEntries)
			{
				if (xuiC_SkillBookLevel != null)
				{
					xuiC_SkillBookLevel.HiddenEntriesWithPaging = this.hiddenEntriesWithPaging;
				}
			}
			this.skillsPerPage = this.perkEntries.Count - this.hiddenEntriesWithPaging;
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x060071EE RID: 29166 RVA: 0x002E671C File Offset: 0x002E491C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 2606420134U)
		{
			if (num <= 1275709072U)
			{
				if (num != 443815844U)
				{
					if (num == 1275709072U)
					{
						if (_bindingName == "maxSkillLevel")
						{
							_value = ((this.CurrentSkill != null) ? this.maxSkillLevelFormatter.Format((float)ProgressionClass.GetCalculatedMaxLevel(entityPlayer, this.CurrentSkill)) : "0");
							return true;
						}
					}
				}
				else if (_bindingName == "skillLevel")
				{
					_value = ((this.CurrentSkill != null) ? this.skillLevelFormatter.Format(this.CurrentSkill.GetCalculatedLevel(entityPlayer)) : "0");
					return true;
				}
			}
			else if (num != 1283949528U)
			{
				if (num == 2606420134U)
				{
					if (_bindingName == "groupdescription")
					{
						if (this.CurrentSkill != null)
						{
							_value = Localization.Get(this.CurrentSkill.ProgressionClass.DescKey, false);
						}
						else
						{
							_value = "";
						}
						return true;
					}
				}
			}
			else if (_bindingName == "currentlevel")
			{
				_value = Localization.Get("xuiSkillLevel", false);
				return true;
			}
		}
		else if (num <= 3504806855U)
		{
			if (num != 3268933568U)
			{
				if (num == 3504806855U)
				{
					if (_bindingName == "groupname")
					{
						_value = ((this.CurrentSkill != null) ? Localization.Get(this.CurrentSkill.ProgressionClass.NameKey, false) : "Skill Info");
						return true;
					}
				}
			}
			else if (_bindingName == "showPaging")
			{
				_value = "false";
				return true;
			}
		}
		else if (num != 4010384093U)
		{
			if (num == 4294521801U)
			{
				if (_bindingName == "detailsdescription")
				{
					if (this.CurrentSkill != null)
					{
						if (this.hoveredPerk != null)
						{
							if (string.IsNullOrEmpty(this.hoveredPerk.ProgressionClass.LongDescKey))
							{
								_value = Localization.Get(this.hoveredPerk.ProgressionClass.DescKey, false);
							}
							else
							{
								_value = Localization.Get(this.hoveredPerk.ProgressionClass.LongDescKey, false);
							}
						}
						else
						{
							_value = Localization.Get(this.CurrentSkill.ProgressionClass.LongDescKey, false);
						}
					}
					else
					{
						_value = "";
					}
					return true;
				}
			}
		}
		else if (_bindingName == "groupicon")
		{
			_value = ((this.CurrentSkill != null) ? this.CurrentSkill.ProgressionClass.Icon : "ui_game_symbol_skills");
			return true;
		}
		return false;
	}

	// Token: 0x040056A3 RID: 22179
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemActionList actionItemList;

	// Token: 0x040056A4 RID: 22180
	[PublicizedFrom(EAccessModifier.Private)]
	public int hiddenEntriesWithPaging = 1;

	// Token: 0x040056A5 RID: 22181
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_SkillBookLevel> perkEntries = new List<XUiC_SkillBookLevel>();

	// Token: 0x040056A6 RID: 22182
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;

	// Token: 0x040056A7 RID: 22183
	[PublicizedFrom(EAccessModifier.Private)]
	public int skillsPerPage;

	// Token: 0x040056A8 RID: 22184
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ProgressionValue> perkList = new List<ProgressionValue>();

	// Token: 0x040056A9 RID: 22185
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionValue hoveredPerk;

	// Token: 0x040056AA RID: 22186
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat skillLevelFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x040056AB RID: 22187
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat maxSkillLevelFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x040056AC RID: 22188
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> buyCostFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString() + " " + Localization.Get("xuiSkillPoints", false));

	// Token: 0x040056AD RID: 22189
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> expCostFormatter = new CachedStringFormatter<int>((int _i) => _i.ToString() + " " + Localization.Get("RewardExp_keyword", false));
}
