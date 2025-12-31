using System;
using System.Collections.Generic;
using Challenges;
using GUI_2;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000C21 RID: 3105
[Preserve]
public class XUiC_CategoryList : XUiController
{
	// Token: 0x14000093 RID: 147
	// (add) Token: 0x06005F5A RID: 24410 RVA: 0x0026A800 File Offset: 0x00268A00
	// (remove) Token: 0x06005F5B RID: 24411 RVA: 0x0026A838 File Offset: 0x00268A38
	public event XUiEvent_CategoryChangedEventHandler CategoryChanged;

	// Token: 0x14000094 RID: 148
	// (add) Token: 0x06005F5C RID: 24412 RVA: 0x0026A870 File Offset: 0x00268A70
	// (remove) Token: 0x06005F5D RID: 24413 RVA: 0x0026A8A8 File Offset: 0x00268AA8
	public event XUiEvent_CategoryChangedEventHandler CategoryClickChanged;

	// Token: 0x170009D7 RID: 2519
	// (get) Token: 0x06005F5E RID: 24414 RVA: 0x0026A8DD File Offset: 0x00268ADD
	// (set) Token: 0x06005F5F RID: 24415 RVA: 0x0026A8E8 File Offset: 0x00268AE8
	public XUiC_CategoryEntry CurrentCategory
	{
		get
		{
			return this.currentCategory;
		}
		set
		{
			if (this.currentCategory != null)
			{
				this.currentCategory.Selected = false;
			}
			this.currentCategory = value;
			if (this.currentCategory != null)
			{
				this.currentCategory.Selected = true;
				this.currentIndex = this.CategoryButtons.IndexOf(this.currentCategory);
			}
			this.IsDirty = true;
		}
	}

	// Token: 0x170009D8 RID: 2520
	// (get) Token: 0x06005F60 RID: 24416 RVA: 0x0026A942 File Offset: 0x00268B42
	public int MaxCategories
	{
		get
		{
			return this.CategoryButtons.Count;
		}
	}

	// Token: 0x06005F61 RID: 24417 RVA: 0x0026A950 File Offset: 0x00268B50
	public override void Init()
	{
		base.Init();
		base.GetChildrenByType<XUiC_CategoryEntry>(this.CategoryButtons);
		for (int i = 0; i < this.CategoryButtons.Count; i++)
		{
			this.CategoryButtons[i].CategoryList = this;
		}
	}

	// Token: 0x06005F62 RID: 24418 RVA: 0x0026A998 File Offset: 0x00268B98
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		if (!this.AllowKeyPaging || !base.xui.playerUI.windowManager.IsKeyShortcutsAllowed())
		{
			return;
		}
		PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
		if (guiactions.PageUp.WasReleased)
		{
			this.IncrementCategory(1);
			return;
		}
		if (guiactions.PageDown.WasReleased)
		{
			this.IncrementCategory(-1);
		}
	}

	// Token: 0x06005F63 RID: 24419 RVA: 0x0026AA21 File Offset: 0x00268C21
	[PublicizedFrom(EAccessModifier.Internal)]
	public void HandleCategoryChanged()
	{
		XUiEvent_CategoryChangedEventHandler categoryChanged = this.CategoryChanged;
		if (categoryChanged != null)
		{
			categoryChanged(this.CurrentCategory);
		}
		XUiEvent_CategoryChangedEventHandler categoryClickChanged = this.CategoryClickChanged;
		if (categoryClickChanged == null)
		{
			return;
		}
		categoryClickChanged(this.CurrentCategory);
	}

	// Token: 0x06005F64 RID: 24420 RVA: 0x0026AA50 File Offset: 0x00268C50
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryEntry GetCategoryByName(string _category, out int _index)
	{
		_index = 0;
		for (int i = 0; i < this.CategoryButtons.Count; i++)
		{
			if (this.CategoryButtons[i].CategoryName == _category)
			{
				_index = i;
				return this.CategoryButtons[i];
			}
		}
		return null;
	}

	// Token: 0x06005F65 RID: 24421 RVA: 0x0026AAA0 File Offset: 0x00268CA0
	public XUiC_CategoryEntry GetCategoryByIndex(int _index)
	{
		if (_index >= this.CategoryButtons.Count)
		{
			return null;
		}
		return this.CategoryButtons[_index];
	}

	// Token: 0x06005F66 RID: 24422 RVA: 0x0026AABE File Offset: 0x00268CBE
	public void SetCategoryToFirst()
	{
		this.CurrentCategory = this.CategoryButtons[0];
		XUiEvent_CategoryChangedEventHandler categoryChanged = this.CategoryChanged;
		if (categoryChanged == null)
		{
			return;
		}
		categoryChanged(this.CurrentCategory);
	}

	// Token: 0x06005F67 RID: 24423 RVA: 0x0026AAE8 File Offset: 0x00268CE8
	public void SetCategory(string _category)
	{
		int num;
		XUiC_CategoryEntry categoryByName = this.GetCategoryByName(_category, out num);
		if (categoryByName == null && !this.AllowUnselect)
		{
			return;
		}
		this.CurrentCategory = categoryByName;
		XUiEvent_CategoryChangedEventHandler categoryChanged = this.CategoryChanged;
		if (categoryChanged == null)
		{
			return;
		}
		categoryChanged(this.CurrentCategory);
	}

	// Token: 0x06005F68 RID: 24424 RVA: 0x0026AB28 File Offset: 0x00268D28
	[PublicizedFrom(EAccessModifier.Private)]
	public void IncrementCategory(int _offset)
	{
		if (_offset == 0)
		{
			return;
		}
		int num = 0;
		int num2 = NGUIMath.RepeatIndex(this.currentIndex + _offset, this.CategoryButtons.Count);
		XUiC_CategoryEntry xuiC_CategoryEntry = this.CategoryButtons[num2];
		while (num < this.CategoryButtons.Count && (xuiC_CategoryEntry == null || xuiC_CategoryEntry.SpriteName == ""))
		{
			num2 = NGUIMath.RepeatIndex((_offset > 0) ? (num2 + 1) : (num2 - 1), this.CategoryButtons.Count);
			xuiC_CategoryEntry = this.CategoryButtons[num2];
			num++;
		}
		if (xuiC_CategoryEntry != null && xuiC_CategoryEntry.SpriteName != "" && xuiC_CategoryEntry.ViewComponent.Enabled)
		{
			this.CurrentCategory = xuiC_CategoryEntry;
			xuiC_CategoryEntry.PlayButtonClickSound();
			this.HandleCategoryChanged();
		}
	}

	// Token: 0x06005F69 RID: 24425 RVA: 0x0026ABEC File Offset: 0x00268DEC
	public void SetCategoryEmpty(int _index)
	{
		XUiC_CategoryEntry xuiC_CategoryEntry = this.CategoryButtons[_index];
		xuiC_CategoryEntry.CategoryDisplayName = (xuiC_CategoryEntry.CategoryName = (xuiC_CategoryEntry.SpriteName = ""));
		xuiC_CategoryEntry.ViewComponent.IsVisible = false;
		xuiC_CategoryEntry.ViewComponent.IsNavigatable = false;
	}

	// Token: 0x06005F6A RID: 24426 RVA: 0x0026AC3C File Offset: 0x00268E3C
	public void SetCategoryEntry(int _index, string _categoryName, string _spriteName, string _displayName = null)
	{
		XUiC_CategoryEntry xuiC_CategoryEntry = this.CategoryButtons[_index];
		xuiC_CategoryEntry.CategoryDisplayName = (_displayName ?? _categoryName);
		xuiC_CategoryEntry.CategoryName = _categoryName;
		xuiC_CategoryEntry.SpriteName = (_spriteName ?? "");
		xuiC_CategoryEntry.ViewComponent.IsVisible = true;
		xuiC_CategoryEntry.ViewComponent.IsNavigatable = true;
		xuiC_CategoryEntry.ViewComponent.Enabled = true;
		xuiC_CategoryEntry.ViewComponent.DisabledToolTip = null;
	}

	// Token: 0x06005F6B RID: 24427 RVA: 0x0026ACA8 File Offset: 0x00268EA8
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.HideGamepadCallouts)
		{
			base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
			base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftTrigger, "igcoCategoryLeft", XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
			base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightTrigger, "igcoCategoryRight", XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
			base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory, 0f);
		}
	}

	// Token: 0x06005F6C RID: 24428 RVA: 0x0026AD19 File Offset: 0x00268F19
	public override void OnClose()
	{
		base.OnClose();
		if (!this.HideGamepadCallouts)
		{
			base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
		}
	}

	// Token: 0x06005F6D RID: 24429 RVA: 0x0026AD3C File Offset: 0x00268F3C
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "allow_unselect")
		{
			this.AllowUnselect = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		if (_name == "allow_key_paging")
		{
			this.AllowKeyPaging = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		if (!(_name == "hide_gamepad_callouts"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.HideGamepadCallouts = StringParsers.ParseBool(_value, 0, -1, true);
		return true;
	}

	// Token: 0x06005F6E RID: 24430 RVA: 0x0026ADB0 File Offset: 0x00268FB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "currentcategoryicon")
		{
			XUiC_CategoryEntry xuiC_CategoryEntry = this.CurrentCategory;
			_value = (((xuiC_CategoryEntry != null) ? xuiC_CategoryEntry.SpriteName : null) ?? "");
			return true;
		}
		if (!(_bindingName == "currentcategorydisplayname"))
		{
			return false;
		}
		XUiC_CategoryEntry xuiC_CategoryEntry2 = this.CurrentCategory;
		_value = (((xuiC_CategoryEntry2 != null) ? xuiC_CategoryEntry2.CategoryDisplayName : null) ?? "");
		return true;
	}

	// Token: 0x06005F6F RID: 24431 RVA: 0x0026AE18 File Offset: 0x00269018
	public bool SetupCategoriesByWorkstation(string _workstation)
	{
		if (this.currentWorkstation != _workstation)
		{
			this.currentWorkstation = _workstation;
			if (_workstation == "skills")
			{
				int num = 0;
				using (Dictionary<string, ProgressionClass>.Enumerator enumerator = Progression.ProgressionClasses.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, ProgressionClass> keyValuePair = enumerator.Current;
						if (keyValuePair.Value.IsAttribute)
						{
							this.SetCategoryEntry(num, keyValuePair.Value.Name, keyValuePair.Value.Icon, Localization.Get(keyValuePair.Value.Name, false));
							num++;
						}
					}
					return true;
				}
			}
			List<CraftingCategoryDisplayEntry> craftingCategoryDisplayList = UIDisplayInfoManager.Current.GetCraftingCategoryDisplayList(_workstation);
			if (craftingCategoryDisplayList != null)
			{
				int num2 = 0;
				int num3 = 0;
				while (num3 < craftingCategoryDisplayList.Count && num3 < this.CategoryButtons.Count)
				{
					this.SetCategoryEntry(num2, craftingCategoryDisplayList[num3].Name, craftingCategoryDisplayList[num3].Icon, craftingCategoryDisplayList[num3].DisplayName);
					num2++;
					num3++;
				}
				for (int i = num2; i < this.CategoryButtons.Count; i++)
				{
					this.SetCategoryEmpty(num2++);
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06005F70 RID: 24432 RVA: 0x0026AF6C File Offset: 0x0026916C
	public bool SetupCategoriesBasedOnItems(List<ItemStack> _items, int _traderStage)
	{
		List<string> list = new List<string>();
		this.SetCategoryEntry(0, "", "ui_game_symbol_shopping_cart", Localization.Get("lblAll", false));
		list.Add("");
		for (int i = 0; i < _items.Count; i++)
		{
			ItemClass itemClass = _items[i].itemValue.ItemClass;
			TraderStageTemplateGroup traderStageTemplateGroup = null;
			if (itemClass.TraderStageTemplate != null && TraderManager.TraderStageTemplates.ContainsKey(itemClass.TraderStageTemplate))
			{
				traderStageTemplateGroup = TraderManager.TraderStageTemplates[itemClass.TraderStageTemplate];
			}
			if (traderStageTemplateGroup == null || traderStageTemplateGroup.IsWithin(_traderStage, (int)_items[i].itemValue.Quality))
			{
				string[] array = itemClass.Groups;
				if (itemClass.IsBlock())
				{
					array = Block.list[_items[i].itemValue.type].GroupNames;
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (!list.Contains(array[j]))
					{
						CraftingCategoryDisplayEntry traderCategoryDisplay = UIDisplayInfoManager.Current.GetTraderCategoryDisplay(array[j]);
						if (traderCategoryDisplay != null)
						{
							int count = list.Count;
							this.SetCategoryEntry(count, traderCategoryDisplay.Name, traderCategoryDisplay.Icon, traderCategoryDisplay.DisplayName);
							list.Add(array[j]);
						}
					}
				}
			}
		}
		for (int k = list.Count; k < this.CategoryButtons.Count; k++)
		{
			this.SetCategoryEmpty(k);
		}
		return true;
	}

	// Token: 0x06005F71 RID: 24433 RVA: 0x0026B0D8 File Offset: 0x002692D8
	public bool SetupCategoriesBasedOnTwitchCategories(List<TwitchActionManager.ActionCategory> _items)
	{
		List<string> list = new List<string>();
		this.SetCategoryEntry(0, "", "ui_game_symbol_twitch_actions", Localization.Get("lblAll", false));
		list.Add("");
		for (int i = 0; i < _items.Count; i++)
		{
			TwitchActionManager.ActionCategory actionCategory = _items[i];
			if (actionCategory.Icon != "")
			{
				this.SetCategoryEntry(list.Count, actionCategory.Name, actionCategory.Icon, actionCategory.Name);
				list.Add(actionCategory.Name);
			}
		}
		for (int j = list.Count; j < this.CategoryButtons.Count; j++)
		{
			this.SetCategoryEmpty(j);
		}
		return true;
	}

	// Token: 0x06005F72 RID: 24434 RVA: 0x0026B18C File Offset: 0x0026938C
	public bool SetupCategoriesBasedOnTwitchActions(List<TwitchAction> _items)
	{
		List<string> list = new List<string>();
		this.SetCategoryEntry(0, "", "ui_game_symbol_twitch_actions", Localization.Get("lblAll", false));
		list.Add("");
		Dictionary<TwitchActionManager.ActionCategory, int> dictionary = new Dictionary<TwitchActionManager.ActionCategory, int>();
		List<TwitchActionManager.ActionCategory> categoryList = TwitchActionManager.Current.CategoryList;
		for (int i = 0; i < categoryList.Count; i++)
		{
			dictionary.Add(categoryList[i], 0);
		}
		for (int j = 0; j < _items.Count; j++)
		{
			TwitchAction twitchAction = _items[j];
			if (twitchAction.DisplayCategory != null)
			{
				Dictionary<TwitchActionManager.ActionCategory, int> dictionary2 = dictionary;
				TwitchActionManager.ActionCategory displayCategory = twitchAction.DisplayCategory;
				int num = dictionary2[displayCategory];
				dictionary2[displayCategory] = num + 1;
			}
		}
		foreach (TwitchActionManager.ActionCategory actionCategory in dictionary.Keys)
		{
			if (dictionary[actionCategory] > 0)
			{
				this.SetCategoryEntry(list.Count, actionCategory.Name, actionCategory.Icon, actionCategory.DisplayName);
				list.Add(actionCategory.Name);
			}
		}
		for (int k = list.Count; k < this.CategoryButtons.Count; k++)
		{
			this.SetCategoryEmpty(k);
		}
		return true;
	}

	// Token: 0x06005F73 RID: 24435 RVA: 0x0026B2E0 File Offset: 0x002694E0
	public bool SetupCategoriesBasedOnTwitchVoteCategories(List<TwitchVoteType> _items)
	{
		List<string> list = new List<string>();
		this.SetCategoryEntry(0, "", "ui_game_symbol_twitch_vote", Localization.Get("lblAll", false));
		list.Add("");
		for (int i = 0; i < _items.Count; i++)
		{
			TwitchVoteType twitchVoteType = _items[i];
			if (twitchVoteType.Icon != "")
			{
				this.SetCategoryEntry(list.Count, twitchVoteType.Name, twitchVoteType.Icon, twitchVoteType.Title);
				list.Add(twitchVoteType.Name);
			}
		}
		for (int j = list.Count; j < this.CategoryButtons.Count; j++)
		{
			this.SetCategoryEmpty(j);
		}
		return true;
	}

	// Token: 0x06005F74 RID: 24436 RVA: 0x0026B394 File Offset: 0x00269594
	public bool SetupCategoriesBasedOnGameEventCategories(List<GameEventManager.Category> _items)
	{
		List<string> list = new List<string>();
		this.SetCategoryEntry(0, "", "ui_game_symbol_airdrop", Localization.Get("lblAll", false));
		list.Add("");
		for (int i = 0; i < _items.Count; i++)
		{
			GameEventManager.Category category = _items[i];
			if (category.Icon != "")
			{
				this.SetCategoryEntry(list.Count, category.Name, category.Icon, category.Name);
				list.Add(category.Name);
			}
		}
		for (int j = list.Count; j < this.CategoryButtons.Count; j++)
		{
			this.SetCategoryEmpty(j);
		}
		return true;
	}

	// Token: 0x06005F75 RID: 24437 RVA: 0x0026B448 File Offset: 0x00269648
	public bool SetupCategoriesBasedOnChallengeCategories(List<ChallengeCategory> _items)
	{
		List<string> list = new List<string>();
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		for (int i = 0; i < _items.Count; i++)
		{
			ChallengeCategory challengeCategory = _items[i];
			if (challengeCategory.Icon != "" && challengeCategory.CanShow(entityPlayer))
			{
				this.SetCategoryEntry(list.Count, challengeCategory.Name, challengeCategory.Icon, challengeCategory.Title);
				list.Add(challengeCategory.Name);
			}
		}
		for (int j = list.Count; j < this.CategoryButtons.Count; j++)
		{
			this.SetCategoryEmpty(j);
		}
		return true;
	}

	// Token: 0x040047E2 RID: 18402
	[PublicizedFrom(EAccessModifier.Private)]
	public string currentWorkstation = "*";

	// Token: 0x040047E5 RID: 18405
	public readonly List<XUiC_CategoryEntry> CategoryButtons = new List<XUiC_CategoryEntry>();

	// Token: 0x040047E6 RID: 18406
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentIndex;

	// Token: 0x040047E7 RID: 18407
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryEntry currentCategory;

	// Token: 0x040047E8 RID: 18408
	public bool AllowUnselect;

	// Token: 0x040047E9 RID: 18409
	public bool AllowKeyPaging = true;

	// Token: 0x040047EA RID: 18410
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HideGamepadCallouts;
}
