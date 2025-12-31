using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000E36 RID: 3638
[Preserve]
public class XUiC_SkillCraftingInfoWindow : XUiC_InfoWindow
{
	// Token: 0x17000B8A RID: 2954
	// (get) Token: 0x0600721B RID: 29211 RVA: 0x002E7D3E File Offset: 0x002E5F3E
	// (set) Token: 0x0600721C RID: 29212 RVA: 0x002E7D46 File Offset: 0x002E5F46
	public XUiC_SkillCraftingInfoEntry SelectedEntry
	{
		get
		{
			return this.selectedEntry;
		}
		set
		{
			if (this.selectedEntry != null)
			{
				this.selectedEntry.IsSelected = false;
			}
			this.selectedEntry = value;
			if (this.selectedEntry != null)
			{
				this.selectedEntry.IsSelected = true;
			}
		}
	}

	// Token: 0x17000B8B RID: 2955
	// (get) Token: 0x0600721D RID: 29213 RVA: 0x002E7D77 File Offset: 0x002E5F77
	public ProgressionValue CurrentSkill
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (base.xui.selectedSkill == null || !base.xui.selectedSkill.ProgressionClass.IsCrafting)
			{
				return null;
			}
			return base.xui.selectedSkill;
		}
	}

	// Token: 0x17000B8C RID: 2956
	// (get) Token: 0x0600721E RID: 29214 RVA: 0x002E7DAA File Offset: 0x002E5FAA
	// (set) Token: 0x0600721F RID: 29215 RVA: 0x002E7DB2 File Offset: 0x002E5FB2
	public ProgressionClass.DisplayData HoveredData
	{
		get
		{
			return this.hoveredLevel;
		}
		set
		{
			if (this.hoveredLevel != value)
			{
				this.hoveredLevel = value;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x17000B8D RID: 2957
	// (get) Token: 0x06007220 RID: 29216 RVA: 0x002E7DCB File Offset: 0x002E5FCB
	// (set) Token: 0x06007221 RID: 29217 RVA: 0x002E7DD3 File Offset: 0x002E5FD3
	public ProgressionClass.DisplayData SelectedData
	{
		get
		{
			return this.selectedLevel;
		}
		set
		{
			if (this.selectedLevel != value)
			{
				this.selectedLevel = value;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x17000B8E RID: 2958
	// (get) Token: 0x06007222 RID: 29218 RVA: 0x002E7DEC File Offset: 0x002E5FEC
	public ProgressionClass.DisplayData CurrentData
	{
		get
		{
			if (this.hoveredLevel != null)
			{
				return this.hoveredLevel;
			}
			return this.selectedLevel;
		}
	}

	// Token: 0x06007223 RID: 29219 RVA: 0x002E7E04 File Offset: 0x002E6004
	public override void Init()
	{
		base.Init();
		base.GetChildrenByType<XUiC_SkillCraftingInfoEntry>(this.levelEntries);
		int num = 1;
		foreach (XUiC_SkillCraftingInfoEntry xuiC_SkillCraftingInfoEntry in this.levelEntries)
		{
			xuiC_SkillCraftingInfoEntry.ListIndex = num - 1;
			xuiC_SkillCraftingInfoEntry.Data = null;
			xuiC_SkillCraftingInfoEntry.HiddenEntriesWithPaging = this.hiddenEntriesWithPaging;
			xuiC_SkillCraftingInfoEntry.MaxEntriesWithoutPaging = this.levelEntries.Count;
			xuiC_SkillCraftingInfoEntry.OnHover += this.Entry_OnHover;
			xuiC_SkillCraftingInfoEntry.OnPress += this.Entry_OnPress;
		}
		for (int i = 0; i < 14; i++)
		{
			XUiController childById = base.GetChildById(string.Format("itemIcon{0}", i + 1));
			childById.CustomData = i;
			childById.OnPress += this.Image_OnPress;
		}
		this.actionItemList = base.GetChildByType<XUiC_ItemActionList>();
		this.skillsPerPage = this.levelEntries.Count - this.hiddenEntriesWithPaging;
		this.pager = base.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += this.Pager_OnPageChanged;
		}
	}

	// Token: 0x06007224 RID: 29220 RVA: 0x002E7F48 File Offset: 0x002E6148
	[PublicizedFrom(EAccessModifier.Private)]
	public void Image_OnPress(XUiController _sender, int _mouseButton)
	{
		int index = (int)_sender.CustomData;
		XUi xui = _sender.xui;
		if (this.CurrentData == null || this.CurrentData.GetUnlockItemRecipes(index) == null)
		{
			return;
		}
		xui.playerUI.windowManager.CloseIfOpen("looting");
		List<XUiC_RecipeList> childrenByType = xui.GetChildrenByType<XUiC_RecipeList>();
		XUiC_RecipeList xuiC_RecipeList = null;
		for (int i = 0; i < childrenByType.Count; i++)
		{
			if (childrenByType[i].WindowGroup != null && childrenByType[i].WindowGroup.isShowing)
			{
				xuiC_RecipeList = childrenByType[i];
				break;
			}
		}
		if (xuiC_RecipeList == null)
		{
			XUiC_WindowSelector.OpenSelectorAndWindow(xui.playerUI.entityPlayer, "crafting");
			xuiC_RecipeList = xui.GetChildByType<XUiC_RecipeList>();
		}
		if (xuiC_RecipeList != null)
		{
			xuiC_RecipeList.SetRecipeDataByItems(this.CurrentData.GetUnlockItemRecipes(index));
		}
	}

	// Token: 0x06007225 RID: 29221 RVA: 0x002E8014 File Offset: 0x002E6214
	[PublicizedFrom(EAccessModifier.Private)]
	public void Entry_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_SkillCraftingInfoEntry xuiC_SkillCraftingInfoEntry = _sender as XUiC_SkillCraftingInfoEntry;
		if (xuiC_SkillCraftingInfoEntry == null)
		{
			xuiC_SkillCraftingInfoEntry = (_sender.Parent as XUiC_SkillCraftingInfoEntry);
		}
		if (xuiC_SkillCraftingInfoEntry == null)
		{
			this.SelectedEntry = null;
			this.SelectedData = null;
			return;
		}
		if (this.SelectedData != xuiC_SkillCraftingInfoEntry.Data && xuiC_SkillCraftingInfoEntry.Data != null)
		{
			this.SelectedEntry = xuiC_SkillCraftingInfoEntry;
			this.SelectedData = xuiC_SkillCraftingInfoEntry.Data;
			return;
		}
		this.SelectedEntry = null;
		this.SelectedData = null;
	}

	// Token: 0x06007226 RID: 29222 RVA: 0x002E8084 File Offset: 0x002E6284
	[PublicizedFrom(EAccessModifier.Private)]
	public void Entry_OnHover(XUiController _sender, bool _isOver)
	{
		XUiC_SkillCraftingInfoEntry xuiC_SkillCraftingInfoEntry = _sender as XUiC_SkillCraftingInfoEntry;
		if (xuiC_SkillCraftingInfoEntry == null)
		{
			xuiC_SkillCraftingInfoEntry = (_sender.Parent as XUiC_SkillCraftingInfoEntry);
		}
		if (_isOver && xuiC_SkillCraftingInfoEntry != null)
		{
			this.HoveredData = xuiC_SkillCraftingInfoEntry.Data;
			return;
		}
		this.HoveredData = null;
	}

	// Token: 0x06007227 RID: 29223 RVA: 0x002E80C4 File Offset: 0x002E62C4
	public void SkillChanged()
	{
		XUiC_Paging xuiC_Paging = this.pager;
		if (xuiC_Paging != null)
		{
			xuiC_Paging.SetLastPageByElementsAndPageLength((this.CurrentSkill != null && this.CurrentSkill.ProgressionClass.MaxLevel > this.levelEntries.Count) ? (this.CurrentSkill.ProgressionClass.MaxLevel - 1) : 0, this.skillsPerPage);
		}
		XUiC_Paging xuiC_Paging2 = this.pager;
		if (xuiC_Paging2 != null)
		{
			xuiC_Paging2.Reset();
		}
		this.IsDirty = true;
		this.SelectedData = null;
		this.SelectedEntry = null;
	}

	// Token: 0x06007228 RID: 29224 RVA: 0x002E8148 File Offset: 0x002E6348
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSkill()
	{
		if (this.CurrentSkill != null && this.actionItemList != null)
		{
			this.actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Skill, this);
		}
		XUiC_Paging xuiC_Paging = this.pager;
		int num = ((xuiC_Paging != null) ? xuiC_Paging.GetPage() : 0) * this.skillsPerPage;
		ProgressionClass progressionClass = (this.CurrentSkill != null) ? this.CurrentSkill.ProgressionClass : null;
		if (progressionClass != null && progressionClass.DisplayDataList != null)
		{
			XUiC_SkillEntry entryForSkill = this.windowGroup.Controller.GetChildByType<XUiC_SkillList>().GetEntryForSkill(this.CurrentSkill);
			using (List<XUiC_SkillCraftingInfoEntry>.Enumerator enumerator = this.levelEntries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XUiC_SkillCraftingInfoEntry xuiC_SkillCraftingInfoEntry = enumerator.Current;
					xuiC_SkillCraftingInfoEntry.Data = ((progressionClass.DisplayDataList.Count > num) ? progressionClass.DisplayDataList[num] : null);
					xuiC_SkillCraftingInfoEntry.IsDirty = true;
					if (entryForSkill != null)
					{
						xuiC_SkillCraftingInfoEntry.ViewComponent.NavLeftTarget = entryForSkill.ViewComponent;
					}
					num++;
				}
				return;
			}
		}
		foreach (XUiC_SkillCraftingInfoEntry xuiC_SkillCraftingInfoEntry2 in this.levelEntries)
		{
			xuiC_SkillCraftingInfoEntry2.Data = null;
			xuiC_SkillCraftingInfoEntry2.IsDirty = true;
		}
	}

	// Token: 0x06007229 RID: 29225 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void Pager_OnPageChanged()
	{
		this.IsDirty = true;
	}

	// Token: 0x0600722A RID: 29226 RVA: 0x002E829C File Offset: 0x002E649C
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

	// Token: 0x0600722B RID: 29227 RVA: 0x002E82D6 File Offset: 0x002E64D6
	public override void OnClose()
	{
		base.OnClose();
		XUiEventManager.Instance.OnSkillExperienceAdded -= this.Current_OnSkillExperienceAdded;
	}

	// Token: 0x0600722C RID: 29228 RVA: 0x002E82F4 File Offset: 0x002E64F4
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

	// Token: 0x0600722D RID: 29229 RVA: 0x002E831E File Offset: 0x002E651E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_OnSkillExperienceAdded(ProgressionValue _changedSkill, int _newXp)
	{
		if (this.CurrentSkill == _changedSkill)
		{
			this.IsDirty = true;
		}
	}

	// Token: 0x0600722E RID: 29230 RVA: 0x002E8330 File Offset: 0x002E6530
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "hidden_entries_with_paging")
		{
			this.hiddenEntriesWithPaging = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			foreach (XUiC_SkillCraftingInfoEntry xuiC_SkillCraftingInfoEntry in this.levelEntries)
			{
				if (xuiC_SkillCraftingInfoEntry != null)
				{
					xuiC_SkillCraftingInfoEntry.HiddenEntriesWithPaging = this.hiddenEntriesWithPaging;
				}
			}
			this.skillsPerPage = this.levelEntries.Count - this.hiddenEntriesWithPaging;
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600722F RID: 29231 RVA: 0x002E83CC File Offset: 0x002E65CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1283949528U)
		{
			if (num <= 464048759U)
			{
				if (num != 443815844U)
				{
					if (num == 464048759U)
					{
						if (_bindingName == "alwaysfalse")
						{
							_value = "false";
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
			else if (num != 1275709072U)
			{
				if (num == 1283949528U)
				{
					if (_bindingName == "currentlevel")
					{
						_value = Localization.Get("xuiSkillLevel", false);
						return true;
					}
				}
			}
			else if (_bindingName == "maxSkillLevel")
			{
				_value = ((this.CurrentSkill != null) ? this.maxSkillLevelFormatter.Format((float)ProgressionClass.GetCalculatedMaxLevel(entityPlayer, this.CurrentSkill)) : "0");
				return true;
			}
		}
		else if (num <= 3268933568U)
		{
			if (num != 2606420134U)
			{
				if (num == 3268933568U)
				{
					if (_bindingName == "showPaging")
					{
						_value = "false";
						return true;
					}
				}
			}
			else if (_bindingName == "groupdescription")
			{
				_value = ((this.CurrentSkill != null) ? Localization.Get(this.CurrentSkill.ProgressionClass.DescKey, false) : "");
				return true;
			}
		}
		else if (num != 3504806855U)
		{
			if (num != 4010384093U)
			{
				if (num == 4294521801U)
				{
					if (_bindingName == "detailsdescription")
					{
						_value = "";
						return true;
					}
				}
			}
			else if (_bindingName == "groupicon")
			{
				_value = ((this.CurrentSkill != null) ? this.CurrentSkill.ProgressionClass.Icon : "ui_game_symbol_skills");
				return true;
			}
		}
		else if (_bindingName == "groupname")
		{
			_value = ((this.CurrentSkill != null) ? Localization.Get(this.CurrentSkill.ProgressionClass.NameKey, false) : "Skill Info");
			return true;
		}
		if (_bindingName.StartsWith("unlock_icon_atlas"))
		{
			if (this.CurrentData != null)
			{
				int index = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon_atlas", ""), 0, -1, NumberStyles.Integer) - 1;
				_value = this.CurrentData.GetUnlockItemIconAtlas(entityPlayer, index);
			}
			else
			{
				_value = "ItemIconAtlas";
			}
			return true;
		}
		if (_bindingName.StartsWith("unlock_icon_locked"))
		{
			if (this.CurrentData != null)
			{
				int index2 = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon_locked", ""), 0, -1, NumberStyles.Integer) - 1;
				_value = this.CurrentData.GetUnlockItemLocked(entityPlayer, index2).ToString();
			}
			else
			{
				_value = "false";
			}
			return true;
		}
		if (_bindingName.StartsWith("unlock_icon_tooltip"))
		{
			if (this.CurrentData != null)
			{
				int index3 = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon_tooltip", ""), 0, -1, NumberStyles.Integer) - 1;
				_value = this.CurrentData.GetUnlockItemName(index3);
			}
			else
			{
				_value = "";
			}
			return true;
		}
		if (_bindingName.StartsWith("unlock_icon"))
		{
			if (this.CurrentData != null)
			{
				int index4 = StringParsers.ParseSInt32(_bindingName.Replace("unlock_icon", ""), 0, -1, NumberStyles.Integer) - 1;
				_value = this.CurrentData.GetUnlockItemIconName(index4);
			}
			else
			{
				_value = "";
			}
			return true;
		}
		return false;
	}

	// Token: 0x040056E0 RID: 22240
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemActionList actionItemList;

	// Token: 0x040056E1 RID: 22241
	[PublicizedFrom(EAccessModifier.Private)]
	public int hiddenEntriesWithPaging = 1;

	// Token: 0x040056E2 RID: 22242
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_SkillCraftingInfoEntry> levelEntries = new List<XUiC_SkillCraftingInfoEntry>();

	// Token: 0x040056E3 RID: 22243
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;

	// Token: 0x040056E4 RID: 22244
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillCraftingInfoEntry selectedEntry;

	// Token: 0x040056E5 RID: 22245
	[PublicizedFrom(EAccessModifier.Private)]
	public int skillsPerPage;

	// Token: 0x040056E6 RID: 22246
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionClass.DisplayData hoveredLevel;

	// Token: 0x040056E7 RID: 22247
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionClass.DisplayData selectedLevel;

	// Token: 0x040056E8 RID: 22248
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat skillLevelFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x040056E9 RID: 22249
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat maxSkillLevelFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x040056EA RID: 22250
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, float> attributeSetValueFormatter = new CachedStringFormatter<string, float>((string _s, float _f) => _s + ": " + _f.ToCultureInvariantString("0.#"));
}
