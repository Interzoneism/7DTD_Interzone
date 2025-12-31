using System;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000E3B RID: 3643
[Preserve]
public class XUiC_SkillListWindow : XUiController
{
	// Token: 0x06007267 RID: 29287 RVA: 0x002EA3F0 File Offset: 0x002E85F0
	public override void Init()
	{
		base.Init();
		this.totalItems = Localization.Get("lblTotalItems", false);
		this.pointsAvailable = Localization.Get("xuiPointsAvailable", false);
		this.skillsTitle = Localization.Get("xuiSkills", false);
		this.booksTitle = Localization.Get("lblCategoryBooks", false);
		this.craftingTitle = Localization.Get("xuiCrafting", false);
		this.skillList = base.GetChildByType<XUiC_SkillList>();
		XUiController childByType = base.GetChildByType<XUiC_CategoryList>();
		if (childByType != null)
		{
			this.categoryList = (XUiC_CategoryList)childByType;
			this.categoryList.CategoryChanged += this.CategoryList_CategoryChanged;
		}
		this.skillList.CategoryList = this.categoryList;
		this.skillList.SkillListWindow = this;
	}

	// Token: 0x06007268 RID: 29288 RVA: 0x002EA4AE File Offset: 0x002E86AE
	[PublicizedFrom(EAccessModifier.Private)]
	public void CategoryList_CategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.categoryName = _categoryEntry.CategoryDisplayName;
		this.categoryIcon = _categoryEntry.SpriteName;
		this.count = this.skillList.GetActiveCount();
		base.RefreshBindings(false);
	}

	// Token: 0x06007269 RID: 29289 RVA: 0x002EA4E0 File Offset: 0x002E86E0
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, "igcoSpendPoints", XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts, 0f);
	}

	// Token: 0x0600726A RID: 29290 RVA: 0x002EA531 File Offset: 0x002E8731
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
	}

	// Token: 0x0600726B RID: 29291 RVA: 0x002EA54C File Offset: 0x002E874C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2672683516U)
		{
			if (num <= 1237242696U)
			{
				if (num != 1050865026U)
				{
					if (num == 1237242696U)
					{
						if (bindingName == "isbook")
						{
							if (this.skillList != null)
							{
								value = (this.skillList.DisplayType == ProgressionClass.DisplayTypes.Book).ToString();
							}
							return true;
						}
					}
				}
				else if (bindingName == "isnormal")
				{
					if (this.skillList != null)
					{
						value = (this.skillList.DisplayType == ProgressionClass.DisplayTypes.Standard).ToString();
					}
					return true;
				}
			}
			else if (num != 1741938684U)
			{
				if (num == 2672683516U)
				{
					if (bindingName == "paging_visible")
					{
						if (this.skillList != null)
						{
							value = (this.skillList.PageCount > 0).ToString();
						}
						return true;
					}
				}
			}
			else if (bindingName == "categoryicon")
			{
				value = this.categoryIcon;
				return true;
			}
		}
		else if (num <= 3877939383U)
		{
			if (num != 3822843618U)
			{
				if (num == 3877939383U)
				{
					if (bindingName == "totalskills")
					{
						value = "";
						if (this.skillList != null)
						{
							value = this.totalSkillsFormatter.Format(this.totalItems, this.skillList.GetActiveCount());
						}
						return true;
					}
				}
			}
			else if (bindingName == "titlename")
			{
				value = "";
				if (this.skillList != null)
				{
					switch (this.skillList.DisplayType)
					{
					case ProgressionClass.DisplayTypes.Standard:
						value = this.skillsTitle;
						break;
					case ProgressionClass.DisplayTypes.Book:
						value = this.booksTitle;
						break;
					case ProgressionClass.DisplayTypes.Crafting:
						value = this.craftingTitle;
						break;
					}
				}
				return true;
			}
		}
		else if (num != 3983894959U)
		{
			if (num == 4035727244U)
			{
				if (bindingName == "skillpointsavailable")
				{
					string v = this.pointsAvailable;
					EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
					if (XUi.IsGameRunning() && entityPlayer != null)
					{
						value = this.skillPointsAvailableFormatter.Format(v, entityPlayer.Progression.SkillPoints);
					}
					return true;
				}
			}
		}
		else if (bindingName == "iscrafting")
		{
			if (this.skillList != null)
			{
				value = (this.skillList.DisplayType == ProgressionClass.DisplayTypes.Crafting).ToString();
			}
			return true;
		}
		return false;
	}

	// Token: 0x0400570D RID: 22285
	[PublicizedFrom(EAccessModifier.Private)]
	public string totalItems = "";

	// Token: 0x0400570E RID: 22286
	[PublicizedFrom(EAccessModifier.Private)]
	public int count;

	// Token: 0x0400570F RID: 22287
	[PublicizedFrom(EAccessModifier.Private)]
	public string categoryName = "Intellect";

	// Token: 0x04005710 RID: 22288
	[PublicizedFrom(EAccessModifier.Private)]
	public string categoryIcon = "";

	// Token: 0x04005711 RID: 22289
	[PublicizedFrom(EAccessModifier.Private)]
	public string pointsAvailable;

	// Token: 0x04005712 RID: 22290
	[PublicizedFrom(EAccessModifier.Private)]
	public string skillsTitle = "";

	// Token: 0x04005713 RID: 22291
	[PublicizedFrom(EAccessModifier.Private)]
	public string booksTitle = "";

	// Token: 0x04005714 RID: 22292
	[PublicizedFrom(EAccessModifier.Private)]
	public string craftingTitle;

	// Token: 0x04005715 RID: 22293
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList categoryList;

	// Token: 0x04005716 RID: 22294
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SkillList skillList;

	// Token: 0x04005717 RID: 22295
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, int> totalSkillsFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => string.Format(_s, _i));

	// Token: 0x04005718 RID: 22296
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<string, int> skillPointsAvailableFormatter = new CachedStringFormatter<string, int>((string _s, int _i) => string.Format("{0} {1}", _s, _i));
}
