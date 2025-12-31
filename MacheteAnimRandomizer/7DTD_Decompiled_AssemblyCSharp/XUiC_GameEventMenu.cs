using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000CB5 RID: 3253
[Preserve]
public class XUiC_GameEventMenu : XUiController
{
	// Token: 0x0600649F RID: 25759 RVA: 0x0028C160 File Offset: 0x0028A360
	public override void Init()
	{
		base.Init();
		XUiC_GameEventMenu.ID = base.WindowGroup.ID;
		this.categoryList = (XUiC_CategoryList)base.GetChildById("categories");
		this.categoryList.CategoryChanged += this.CategoryList_CategoryChanged;
		this.gameEventsList = (XUiC_GameEventsList)base.GetChildById("gameevents");
		this.gameEventsList.SelectionChanged += this.EntitiesList_SelectionChanged;
		this.cbxTarget = (XUiC_ComboBoxList<string>)base.GetChildById("cbxTarget");
	}

	// Token: 0x060064A0 RID: 25760 RVA: 0x0028C1F3 File Offset: 0x0028A3F3
	[PublicizedFrom(EAccessModifier.Private)]
	public void CategoryList_CategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.gameEventsList.Category = _categoryEntry.CategoryName;
		this.categoryDisplay = _categoryEntry.CategoryDisplayName;
		base.RefreshBindings(false);
	}

	// Token: 0x060064A1 RID: 25761 RVA: 0x0028C21C File Offset: 0x0028A41C
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntitiesList_SelectionChanged(XUiC_ListEntry<XUiC_GameEventsList.GameEventEntry> _previousEntry, XUiC_ListEntry<XUiC_GameEventsList.GameEventEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			this.gameEventsList.ClearSelection();
			if (_newEntry.GetEntry() != null)
			{
				XUiC_GameEventsList.GameEventEntry entry = _newEntry.GetEntry();
				this.BtnSpawns_OnPress(entry.name);
			}
		}
	}

	// Token: 0x060064A2 RID: 25762 RVA: 0x0028C254 File Offset: 0x0028A454
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSpawns_OnPress(string _name)
	{
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		EntityPlayer entityPlayer2 = this.PlayerList[this.cbxTarget.Value];
		if (entityPlayer2 == entityPlayer || !entityPlayer2.IsAdmin)
		{
			GameEventManager.Current.HandleAction(_name, entityPlayer, entityPlayer2, false, "", "", false, true, "", null);
		}
	}

	// Token: 0x060064A3 RID: 25763 RVA: 0x0028C2BC File Offset: 0x0028A4BC
	public override void OnOpen()
	{
		base.OnOpen();
		this.categoryList.SetupCategoriesBasedOnGameEventCategories(GameEventManager.Current.CategoryList);
		this.categoryList.SetCategoryToFirst();
		this.cbxTarget.Elements.Clear();
		this.PlayerList.Clear();
		int selectedIndex = 0;
		for (int i = 0; i < GameManager.Instance.World.Players.list.Count; i++)
		{
			EntityPlayer entityPlayer = GameManager.Instance.World.Players.list[i];
			this.cbxTarget.Elements.Add(entityPlayer.EntityName);
			this.PlayerList.Add(entityPlayer.EntityName, entityPlayer);
			if (entityPlayer is EntityPlayerLocal)
			{
				selectedIndex = i;
			}
		}
		this.cbxTarget.SelectedIndex = selectedIndex;
	}

	// Token: 0x060064A4 RID: 25764 RVA: 0x0028C38A File Offset: 0x0028A58A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "headertitle")
		{
			if (this.categoryDisplay == "")
			{
				value = "Game Events";
			}
			else
			{
				value = string.Format("Game Events - {0}", this.categoryDisplay);
			}
			return true;
		}
		return false;
	}

	// Token: 0x04004BE0 RID: 19424
	public static string ID = "";

	// Token: 0x04004BE1 RID: 19425
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList categoryList;

	// Token: 0x04004BE2 RID: 19426
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_GameEventsList gameEventsList;

	// Token: 0x04004BE3 RID: 19427
	[PublicizedFrom(EAccessModifier.Private)]
	public string categoryDisplay = "";

	// Token: 0x04004BE4 RID: 19428
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> cbxTarget;

	// Token: 0x04004BE5 RID: 19429
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, EntityPlayer> PlayerList = new Dictionary<string, EntityPlayer>();
}
