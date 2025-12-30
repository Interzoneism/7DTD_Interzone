using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000E38 RID: 3640
[Preserve]
public class XUiC_SkillList : XUiController
{
	// Token: 0x17000B8F RID: 2959
	// (get) Token: 0x06007234 RID: 29236 RVA: 0x002E87E3 File Offset: 0x002E69E3
	// (set) Token: 0x06007235 RID: 29237 RVA: 0x002E87EC File Offset: 0x002E69EC
	public XUiC_SkillEntry SelectedEntry
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
				base.xui.selectedSkill = this.selectedEntry.Skill;
			}
		}
	}

	// Token: 0x17000B90 RID: 2960
	// (get) Token: 0x06007236 RID: 29238 RVA: 0x002E883E File Offset: 0x002E6A3E
	// (set) Token: 0x06007237 RID: 29239 RVA: 0x002E8846 File Offset: 0x002E6A46
	public XUiC_CategoryList CategoryList { get; set; }

	// Token: 0x17000B91 RID: 2961
	// (get) Token: 0x06007238 RID: 29240 RVA: 0x002E884F File Offset: 0x002E6A4F
	// (set) Token: 0x06007239 RID: 29241 RVA: 0x002E8857 File Offset: 0x002E6A57
	public XUiC_SkillListWindow SkillListWindow { get; set; }

	// Token: 0x17000B92 RID: 2962
	// (get) Token: 0x0600723A RID: 29242 RVA: 0x002E8860 File Offset: 0x002E6A60
	// (set) Token: 0x0600723B RID: 29243 RVA: 0x002E8868 File Offset: 0x002E6A68
	public string Category
	{
		get
		{
			return this.category;
		}
		set
		{
			if (this.category != value)
			{
				this.category = value;
				if (Progression.ProgressionClasses.ContainsKey(this.category))
				{
					this.attributeClass = Progression.ProgressionClasses[this.category];
				}
			}
		}
	}

	// Token: 0x17000B93 RID: 2963
	// (get) Token: 0x0600723C RID: 29244 RVA: 0x002E88A7 File Offset: 0x002E6AA7
	public ProgressionClass.DisplayTypes DisplayType
	{
		get
		{
			if (this.attributeClass == null)
			{
				return ProgressionClass.DisplayTypes.Standard;
			}
			return this.attributeClass.DisplayType;
		}
	}

	// Token: 0x17000B94 RID: 2964
	// (get) Token: 0x0600723D RID: 29245 RVA: 0x002E88BE File Offset: 0x002E6ABE
	public int PageCount
	{
		get
		{
			if (this.pagingControl != null)
			{
				return this.pagingControl.LastPageNumber;
			}
			return 1;
		}
	}

	// Token: 0x0600723E RID: 29246 RVA: 0x002E88D8 File Offset: 0x002E6AD8
	public override void Init()
	{
		base.Init();
		this.Category = "";
		XUiController parent = this.parent.Parent;
		this.skillEntries = base.GetChildrenByType<XUiC_SkillEntry>(null);
		this.pagingControl = parent.GetChildByType<XUiC_Paging>();
		if (this.pagingControl != null)
		{
			this.pagingControl.OnPageChanged += this.PagingControl_OnPageChanged;
		}
		this.txtInput = parent.GetChildByType<XUiC_TextInput>();
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler += this.TxtInput_OnChangeHandler;
			this.txtInput.Text = "";
		}
		for (int i = 0; i < this.skillEntries.Length; i++)
		{
			this.skillEntries[i].skillList = this;
			this.skillEntries[i].OnPress += this.XUiC_SkillEntry_OnPress;
			this.skillEntries[i].OnScroll += this.HandleOnScroll;
		}
	}

	// Token: 0x0600723F RID: 29247 RVA: 0x002E89C8 File Offset: 0x002E6BC8
	public void SetSelectedByUnlockData(RecipeUnlockData unlockData)
	{
		switch (unlockData.UnlockType)
		{
		case RecipeUnlockData.UnlockTypes.Perk:
			this.selectName = unlockData.Perk.Name;
			if (unlockData.Perk.IsPerk)
			{
				this.CategoryList.SetCategory(unlockData.Perk.Parent.ParentName);
				return;
			}
			break;
		case RecipeUnlockData.UnlockTypes.Book:
			this.selectName = unlockData.Perk.ParentName;
			if (unlockData.Perk.IsBook)
			{
				this.CategoryList.SetCategory(unlockData.Perk.Parent.ParentName);
				return;
			}
			break;
		case RecipeUnlockData.UnlockTypes.Skill:
			this.selectName = unlockData.Perk.Name;
			if (unlockData.Perk.IsCrafting)
			{
				this.CategoryList.SetCategory(unlockData.Perk.Parent.Name);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06007240 RID: 29248 RVA: 0x002E8AA1 File Offset: 0x002E6CA1
	[PublicizedFrom(EAccessModifier.Internal)]
	public int GetActiveCount()
	{
		return this.currentSkills.Count;
	}

	// Token: 0x06007241 RID: 29249 RVA: 0x002E8AB0 File Offset: 0x002E6CB0
	public void SetFilterText(string _text)
	{
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler -= this.TxtInput_OnChangeHandler;
			this.filterText = _text;
			this.txtInput.Text = _text;
			this.txtInput.OnChangeHandler += this.TxtInput_OnChangeHandler;
		}
	}

	// Token: 0x06007242 RID: 29250 RVA: 0x002E8B06 File Offset: 0x002E6D06
	public void SelectFirstEntry()
	{
		this.SelectedEntry = this.skillEntries[0];
		this.SelectedEntry.SelectCursorElement(true, false);
	}

	// Token: 0x06007243 RID: 29251 RVA: 0x002E8B24 File Offset: 0x002E6D24
	public void SetSelected(XUiC_SkillEntry _entry)
	{
		this.SelectedEntry = _entry;
		this.selectName = "";
	}

	// Token: 0x06007244 RID: 29252 RVA: 0x002E8B38 File Offset: 0x002E6D38
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_SkillEntry_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_SkillEntry xuiC_SkillEntry = (XUiC_SkillEntry)_sender;
		if (xuiC_SkillEntry.Skill == null || xuiC_SkillEntry.Skill.ProgressionClass.Type == ProgressionType.Skill)
		{
			return;
		}
		this.SetSelected(xuiC_SkillEntry);
	}

	// Token: 0x06007245 RID: 29253 RVA: 0x002E8B70 File Offset: 0x002E6D70
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtInput_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.filterText = _text.Trim();
		if (this.filterText == "")
		{
			if (this.attributeClass.DisplayType != ProgressionClass.DisplayTypes.Book)
			{
				this.CategoryList.SetCategoryToFirst();
				return;
			}
			this.CategoryList.SetCategory(this.Category);
			return;
		}
		else
		{
			if (this.attributeClass == null || this.attributeClass.DisplayType != ProgressionClass.DisplayTypes.Book)
			{
				this.CategoryList.SetCategory("");
				return;
			}
			this.CategoryList.SetCategory(this.Category);
			return;
		}
	}

	// Token: 0x06007246 RID: 29254 RVA: 0x002E8BFF File Offset: 0x002E6DFF
	[PublicizedFrom(EAccessModifier.Private)]
	public void PagingControl_OnPageChanged()
	{
		XUiC_Paging xuiC_Paging = this.pagingControl;
		this.listSkills((xuiC_Paging != null) ? xuiC_Paging.GetPage() : 0);
	}

	// Token: 0x06007247 RID: 29255 RVA: 0x002E8C19 File Offset: 0x002E6E19
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			this.HandlePageDown(this, new EventArgs());
			return;
		}
		this.HandlePageUp(this, new EventArgs());
	}

	// Token: 0x06007248 RID: 29256 RVA: 0x002E8C3C File Offset: 0x002E6E3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePageDown(XUiController _sender, EventArgs _e)
	{
		XUiC_Paging xuiC_Paging = this.pagingControl;
		if (xuiC_Paging == null)
		{
			return;
		}
		xuiC_Paging.PageDown();
	}

	// Token: 0x06007249 RID: 29257 RVA: 0x002E8C4F File Offset: 0x002E6E4F
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePageUp(XUiController _sender, EventArgs _e)
	{
		XUiC_Paging xuiC_Paging = this.pagingControl;
		if (xuiC_Paging == null)
		{
			return;
		}
		xuiC_Paging.PageUp();
	}

	// Token: 0x0600724A RID: 29258 RVA: 0x002E8C62 File Offset: 0x002E6E62
	public void RefreshSkillList()
	{
		XUiC_Paging xuiC_Paging = this.pagingControl;
		if (xuiC_Paging != null)
		{
			xuiC_Paging.Reset();
		}
		this.updateFilteredList();
		this.PagingControl_OnPageChanged();
	}

	// Token: 0x0600724B RID: 29259 RVA: 0x002E8C84 File Offset: 0x002E6E84
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateFilteredList()
	{
		this.currentSkills.Clear();
		string a = this.Category.Trim();
		bool flag = this.filterText != "";
		foreach (ProgressionValue progressionValue in this.skills)
		{
			ProgressionClass progressionClass = (progressionValue != null) ? progressionValue.ProgressionClass : null;
			if (progressionClass != null && progressionClass.ValidDisplay(this.DisplayType) && progressionClass.Name != null && !progressionClass.IsBook && !progressionClass.Hidden && (!flag || progressionClass.NameKey.ContainsCaseInsensitive(this.filterText) || Localization.Get(progressionClass.NameKey, false).ContainsCaseInsensitive(this.filterText)))
			{
				if (a == "" || a.EqualsCaseInsensitive(progressionClass.Name))
				{
					this.currentSkills.Add(progressionValue);
				}
				else
				{
					ProgressionClass parent = progressionClass.Parent;
					if (parent != null && parent != progressionClass && (((progressionClass.IsSkill || progressionClass.IsCrafting) && a.EqualsCaseInsensitive(progressionClass.Parent.Name)) || ((progressionClass.IsPerk || progressionClass.IsBookGroup) && a.EqualsCaseInsensitive(progressionClass.Parent.Parent.Name))))
					{
						this.currentSkills.Add(progressionValue);
					}
				}
			}
		}
		this.currentSkills.Sort(ProgressionClass.ListSortOrderComparer.Instance);
		if (this.filterText == "")
		{
			for (int i = 0; i < this.currentSkills.Count; i++)
			{
				if (this.currentSkills[i].ProgressionClass.IsAttribute)
				{
					while (i % this.skillEntries.Length != 0)
					{
						this.currentSkills.Insert(i, null);
						i++;
					}
				}
			}
		}
		XUiC_Paging xuiC_Paging = this.pagingControl;
		if (xuiC_Paging != null)
		{
			xuiC_Paging.SetLastPageByElementsAndPageLength(this.currentSkills.Count, this.skillEntries.Length);
		}
		if (!string.IsNullOrEmpty(this.selectName))
		{
			int j = 0;
			while (j < this.currentSkills.Count)
			{
				if (this.currentSkills[j].Name == this.selectName)
				{
					XUiC_Paging xuiC_Paging2 = this.pagingControl;
					if (xuiC_Paging2 == null)
					{
						return;
					}
					xuiC_Paging2.SetPage(j / this.skillEntries.Length);
					return;
				}
				else
				{
					j++;
				}
			}
		}
	}

	// Token: 0x0600724C RID: 29260 RVA: 0x002E8F24 File Offset: 0x002E7124
	[PublicizedFrom(EAccessModifier.Private)]
	public void listSkills(int page)
	{
		int num = this.skillEntries.Length * page;
		this.SelectedEntry = null;
		for (int i = 0; i < this.skillEntries.Length; i++)
		{
			int num2 = i + num;
			XUiC_SkillEntry xuiC_SkillEntry = this.skillEntries[i];
			if (num2 < this.currentSkills.Count && this.currentSkills[num2] != null && Progression.ProgressionClasses.ContainsKey(this.currentSkills[num2].Name))
			{
				xuiC_SkillEntry.Skill = this.currentSkills[num2];
				if (this.selectName != "")
				{
					if (xuiC_SkillEntry.Skill.ProgressionClass.Name == this.selectName)
					{
						this.SelectedEntry = xuiC_SkillEntry;
						xuiC_SkillEntry.IsSelected = true;
						xuiC_SkillEntry.RefreshBindings(false);
						((XUiC_SkillWindowGroup)base.WindowGroup.Controller).CurrentSkill = xuiC_SkillEntry.Skill;
					}
				}
				else if (this.SelectedEntry == null && i == 0)
				{
					this.SelectedEntry = xuiC_SkillEntry;
					base.xui.selectedSkill = xuiC_SkillEntry.Skill;
					((XUiC_SkillWindowGroup)base.WindowGroup.Controller).CurrentSkill = this.selectedEntry.Skill;
					((XUiC_SkillWindowGroup)base.WindowGroup.Controller).IsDirty = true;
				}
				if (base.xui.selectedSkill != null)
				{
					xuiC_SkillEntry.IsSelected = (xuiC_SkillEntry.Skill.Name == base.xui.selectedSkill.Name);
					xuiC_SkillEntry.RefreshBindings(false);
				}
				else
				{
					xuiC_SkillEntry.IsSelected = false;
					xuiC_SkillEntry.RefreshBindings(false);
				}
				xuiC_SkillEntry.DisplayType = this.DisplayType;
				xuiC_SkillEntry.ViewComponent.Enabled = true;
				if (xuiC_SkillEntry.Skill.ProgressionClass.IsAttribute)
				{
					XUiView viewComponent = base.xui.GetWindow("windowSkillAttributeInfo").Controller.GetChildById("0").ViewComponent;
					xuiC_SkillEntry.ViewComponent.NavRightTarget = viewComponent;
				}
				else if (xuiC_SkillEntry.Skill.ProgressionClass.IsPerk)
				{
					XUiView viewComponent2 = base.xui.GetWindow("windowSkillPerkInfo").Controller.GetChildById("0").ViewComponent;
					xuiC_SkillEntry.ViewComponent.NavRightTarget = viewComponent2;
				}
				else if (xuiC_SkillEntry.Skill.ProgressionClass.IsBookGroup)
				{
					XUiView viewComponent3 = base.xui.GetWindow("windowSkillBookInfo").Controller.GetChildById("0").ViewComponent;
					xuiC_SkillEntry.ViewComponent.NavRightTarget = viewComponent3;
				}
				else if (xuiC_SkillEntry.Skill.ProgressionClass.IsCrafting)
				{
					XUiView viewComponent4 = base.xui.GetWindow("windowSkillCraftingInfo").Controller.GetChildById("0").ViewComponent;
					xuiC_SkillEntry.ViewComponent.NavRightTarget = viewComponent4;
				}
			}
			else
			{
				xuiC_SkillEntry.Skill = null;
				xuiC_SkillEntry.IsSelected = false;
				xuiC_SkillEntry.ViewComponent.Enabled = false;
				xuiC_SkillEntry.DisplayType = ProgressionClass.DisplayTypes.Standard;
				xuiC_SkillEntry.ViewComponent.NavRightTarget = null;
				xuiC_SkillEntry.RefreshBindings(false);
			}
		}
		if (this.SelectedEntry == null)
		{
			this.SelectedEntry = this.skillEntries[0];
			this.SelectedEntry.IsSelected = true;
			this.SelectedEntry.RefreshBindings(false);
			((XUiC_SkillWindowGroup)base.WindowGroup.Controller).CurrentSkill = this.SelectedEntry.Skill;
			((XUiC_SkillWindowGroup)base.WindowGroup.Controller).IsDirty = true;
			this.selectName = "";
		}
		base.RefreshBindings(false);
		this.SkillListWindow.RefreshBindings(false);
	}

	// Token: 0x0600724D RID: 29261 RVA: 0x002E92B4 File Offset: 0x002E74B4
	public override void OnOpen()
	{
		base.OnOpen();
		this.skills.Clear();
		base.xui.playerUI.entityPlayer.Progression.GetDict().CopyValuesTo(this.skills);
		this.updateFilteredList();
		this.PagingControl_OnPageChanged();
	}

	// Token: 0x0600724E RID: 29262 RVA: 0x002E9303 File Offset: 0x002E7503
	public override void OnClose()
	{
		base.OnClose();
		this.selectName = "";
	}

	// Token: 0x0600724F RID: 29263 RVA: 0x002E9318 File Offset: 0x002E7518
	public XUiC_SkillEntry GetEntryForSkill(ProgressionValue _skill)
	{
		foreach (XUiC_SkillEntry xuiC_SkillEntry in this.skillEntries)
		{
			if (xuiC_SkillEntry.Skill == _skill)
			{
				return xuiC_SkillEntry;
			}
		}
		return null;
	}

	// Token: 0x040056ED RID: 22253
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ProgressionValue> skills = new List<ProgressionValue>();

	// Token: 0x040056EE RID: 22254
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ProgressionValue> currentSkills = new List<ProgressionValue>();

	// Token: 0x040056EF RID: 22255
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillEntry[] skillEntries;

	// Token: 0x040056F0 RID: 22256
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pagingControl;

	// Token: 0x040056F1 RID: 22257
	[PublicizedFrom(EAccessModifier.Private)]
	public string filterText = "";

	// Token: 0x040056F2 RID: 22258
	[PublicizedFrom(EAccessModifier.Private)]
	public string selectName;

	// Token: 0x040056F3 RID: 22259
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillEntry selectedEntry;

	// Token: 0x040056F6 RID: 22262
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x040056F7 RID: 22263
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionClass attributeClass;

	// Token: 0x040056F8 RID: 22264
	[PublicizedFrom(EAccessModifier.Private)]
	public string category = "";
}
