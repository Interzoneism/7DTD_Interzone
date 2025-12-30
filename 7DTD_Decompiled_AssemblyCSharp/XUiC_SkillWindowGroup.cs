using System;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000E45 RID: 3653
[Preserve]
public class XUiC_SkillWindowGroup : XUiController
{
	// Token: 0x17000BA2 RID: 2978
	// (get) Token: 0x060072BC RID: 29372 RVA: 0x002ED2F2 File Offset: 0x002EB4F2
	// (set) Token: 0x060072BD RID: 29373 RVA: 0x002ED2FA File Offset: 0x002EB4FA
	public ProgressionValue CurrentSkill
	{
		get
		{
			return this.currentSkill;
		}
		set
		{
			this.currentSkill = value;
		}
	}

	// Token: 0x060072BE RID: 29374 RVA: 0x002ED304 File Offset: 0x002EB504
	public override void Init()
	{
		base.Init();
		this.skillList = base.GetChildByType<XUiC_SkillList>();
		this.skillAttributeInfoWindow = base.GetChildByType<XUiC_SkillAttributeInfoWindow>();
		this.skillSkillInfoWindow = base.GetChildByType<XUiC_SkillSkillInfoWindow>();
		this.skillPerkInfoWindow = base.GetChildByType<XUiC_SkillPerkInfoWindow>();
		this.skillBookInfoWindow = base.GetChildByType<XUiC_SkillBookInfoWindow>();
		this.skillCraftingInfoWindow = base.GetChildByType<XUiC_SkillCraftingInfoWindow>();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_SkillEntry>(null);
		XUiController[] array = childrenByType;
		this.skillEntries = new XUiC_SkillEntry[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this.skillEntries[i] = (XUiC_SkillEntry)array[i];
			this.skillEntries[i].OnPress += this.XUiC_SkillEntry_OnPress;
		}
	}

	// Token: 0x060072BF RID: 29375 RVA: 0x002ED3AE File Offset: 0x002EB5AE
	[PublicizedFrom(EAccessModifier.Private)]
	public void CategoryList_CategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.skillList.Category = _categoryEntry.CategoryName;
		this.skillList.RefreshSkillList();
		this.skillList.SelectFirstEntry();
		this.IsDirty = true;
	}

	// Token: 0x060072C0 RID: 29376 RVA: 0x002ED3DE File Offset: 0x002EB5DE
	[PublicizedFrom(EAccessModifier.Private)]
	public void CategoryList_CategoryClickChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.skillList.Category = _categoryEntry.CategoryName;
		this.skillList.SetFilterText("");
		this.skillList.RefreshSkillList();
		this.skillList.SelectFirstEntry();
		this.IsDirty = true;
	}

	// Token: 0x060072C1 RID: 29377 RVA: 0x0007FB49 File Offset: 0x0007DD49
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_SkillEntry_OnPress(XUiController _sender, int _mouseButton)
	{
		this.IsDirty = true;
	}

	// Token: 0x060072C2 RID: 29378 RVA: 0x002ED41E File Offset: 0x002EB61E
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
	}

	// Token: 0x060072C3 RID: 29379 RVA: 0x002ED454 File Offset: 0x002EB654
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.CurrentSkill = base.xui.selectedSkill;
			this.skillAttributeInfoWindow.SkillChanged();
			this.skillSkillInfoWindow.IsDirty = true;
			this.skillPerkInfoWindow.SkillChanged();
			this.skillBookInfoWindow.SkillChanged();
			this.skillCraftingInfoWindow.SkillChanged();
			if (this.skillList.DisplayType == ProgressionClass.DisplayTypes.Book)
			{
				this.skillBookInfoWindow.ViewComponent.IsVisible = true;
			}
			else if (this.skillList.DisplayType == ProgressionClass.DisplayTypes.Crafting)
			{
				this.skillCraftingInfoWindow.ViewComponent.IsVisible = true;
			}
			else if (base.xui.selectedSkill != null)
			{
				if (base.xui.selectedSkill.ProgressionClass.IsAttribute)
				{
					this.skillAttributeInfoWindow.ViewComponent.IsVisible = true;
				}
				else if (base.xui.selectedSkill.ProgressionClass.IsSkill)
				{
					this.skillSkillInfoWindow.ViewComponent.IsVisible = true;
				}
				else
				{
					this.skillPerkInfoWindow.ViewComponent.IsVisible = true;
				}
			}
			this.IsDirty = false;
		}
	}

	// Token: 0x060072C4 RID: 29380 RVA: 0x002ED57C File Offset: 0x002EB77C
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.categoryList == null)
		{
			XUiController childByType = base.GetChildByType<XUiC_CategoryList>();
			if (childByType != null)
			{
				this.categoryList = (XUiC_CategoryList)childByType;
				this.categoryList.SetupCategoriesByWorkstation("skills");
				this.categoryList.CategoryChanged += this.CategoryList_CategoryChanged;
				this.categoryList.CategoryClickChanged += this.CategoryList_CategoryClickChanged;
			}
		}
		base.xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);
		XUiC_WindowSelector childByType2 = base.xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>();
		if (childByType2 != null)
		{
			childByType2.SetSelected("skills");
		}
		this.IsDirty = true;
		if (this.categoryList.CurrentCategory == null)
		{
			this.categoryList.SetCategoryToFirst();
		}
		this.skillList.Category = this.categoryList.CurrentCategory.CategoryName;
		this.skillList.RefreshSkillList();
		if (base.xui.selectedSkill == null)
		{
			this.skillList.SelectFirstEntry();
		}
		else
		{
			this.skillList.SelectedEntry.SelectCursorElement(true, false);
		}
		this.IsDirty = true;
	}

	// Token: 0x0400576E RID: 22382
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillEntry[] skillEntries;

	// Token: 0x0400576F RID: 22383
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillList skillList;

	// Token: 0x04005770 RID: 22384
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList categoryList;

	// Token: 0x04005771 RID: 22385
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillAttributeInfoWindow skillAttributeInfoWindow;

	// Token: 0x04005772 RID: 22386
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillSkillInfoWindow skillSkillInfoWindow;

	// Token: 0x04005773 RID: 22387
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillPerkInfoWindow skillPerkInfoWindow;

	// Token: 0x04005774 RID: 22388
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillBookInfoWindow skillBookInfoWindow;

	// Token: 0x04005775 RID: 22389
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillCraftingInfoWindow skillCraftingInfoWindow;

	// Token: 0x04005776 RID: 22390
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionValue currentSkill;
}
