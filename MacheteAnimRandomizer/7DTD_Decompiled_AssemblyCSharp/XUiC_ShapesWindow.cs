using System;
using System.Collections.Generic;
using GUI_2;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000E28 RID: 3624
[Preserve]
public class XUiC_ShapesWindow : XUiController
{
	// Token: 0x17000B6B RID: 2923
	// (get) Token: 0x0600717C RID: 29052 RVA: 0x002E394A File Offset: 0x002E1B4A
	// (set) Token: 0x0600717D RID: 29053 RVA: 0x002E3952 File Offset: 0x002E1B52
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			if (this.page != value)
			{
				this.page = value;
				this.shapeGrid.Page = this.page;
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging == null)
				{
					return;
				}
				xuiC_Paging.SetPage(this.page);
			}
		}
	}

	// Token: 0x0600717E RID: 29054 RVA: 0x002E398C File Offset: 0x002E1B8C
	public override void Init()
	{
		base.Init();
		this.resultCount = (XUiV_Label)base.GetChildById("resultCount").ViewComponent;
		this.pager = base.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += delegate()
			{
				this.Page = this.pager.CurrentPageNumber;
			};
		}
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].OnScroll += this.HandleOnScroll;
		}
		base.OnScroll += this.HandleOnScroll;
		this.shapeGrid = base.Parent.GetChildByType<XUiC_ShapeStackGrid>();
		XUiController[] childrenByType = this.shapeGrid.GetChildrenByType<XUiC_ShapeStack>(null);
		XUiController[] array = childrenByType;
		this.shapeGrid.Owner = this;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].OnScroll += this.HandleOnScroll;
		}
		this.length = array.Length;
		this.txtInput = (XUiC_TextInput)this.windowGroup.Controller.GetChildById("searchInput");
		if (this.txtInput != null)
		{
			this.txtInput.OnChangeHandler += this.HandleOnChangedHandler;
			this.txtInput.Text = "";
		}
		XUiController childById = base.GetChildById("favorites");
		if (childById != null)
		{
			this.favoritesBtn = (childById.ViewComponent as XUiV_Button);
			if (this.favoritesBtn != null)
			{
				childById.OnPress += this.HandleFavoritesChanged;
			}
		}
		this.lblTotal = Localization.Get("lblTotalItems", false);
		this.categoryList = (XUiC_CategoryList)base.GetChildById("categories");
		if (this.categoryList != null)
		{
			this.categoryList.AllowUnselect = true;
			this.categoryList.CategoryChanged += this.CategoryList_CategoryChanged;
		}
	}

	// Token: 0x0600717F RID: 29055 RVA: 0x002E3B61 File Offset: 0x002E1D61
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFavoritesChanged(XUiController _sender, int _mouseButton)
	{
		this.showFavorites = !this.showFavorites;
		this.favoritesBtn.Selected = this.showFavorites;
		this.UpdateAll();
	}

	// Token: 0x06007180 RID: 29056 RVA: 0x002E3B89 File Offset: 0x002E1D89
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnChangedHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.Page = 0;
		this.UpdateShapesList();
	}

	// Token: 0x06007181 RID: 29057 RVA: 0x002E3B98 File Offset: 0x002E1D98
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnScroll(XUiController _sender, float _delta)
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

	// Token: 0x06007182 RID: 29058 RVA: 0x002E3BC8 File Offset: 0x002E1DC8
	public void UpgradeDowngradeShapes(BlockValue _targetBv)
	{
		string blockName = _targetBv.Block.GetBlockName();
		Block autoShapeHelperBlock = _targetBv.Block.GetAutoShapeHelperBlock();
		ItemValue itemValue = new BlockValue((uint)autoShapeHelperBlock.blockID).ToItemValue();
		itemValue.Meta = autoShapeHelperBlock.GetAlternateBlockIndex(blockName);
		this.ItemValue = itemValue;
		this.UpdateAll();
	}

	// Token: 0x06007183 RID: 29059 RVA: 0x002E3C20 File Offset: 0x002E1E20
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAltList()
	{
		this.altBlocks = this.ItemValue.ToBlockValue().Block.GetAltBlocks();
	}

	// Token: 0x06007184 RID: 29060 RVA: 0x002E3C4C File Offset: 0x002E1E4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateCategories()
	{
		XUiC_CategoryEntry currentCategory = this.categoryList.CurrentCategory;
		string text = (currentCategory != null) ? currentCategory.CategoryName : null;
		Block.GetShapeCategories(this.altBlocks, this.shapeCategories);
		for (int i = 0; i < this.categoryList.Children.Count; i++)
		{
			if (i < this.shapeCategories.Count)
			{
				this.categoryList.SetCategoryEntry(i, this.shapeCategories[i].Name, this.shapeCategories[i].Icon, this.shapeCategories[i].LocalizedName);
			}
			else
			{
				this.categoryList.SetCategoryEmpty(i);
			}
		}
		if (text != null)
		{
			this.categoryList.SetCategory(text);
			return;
		}
		if (!this.openedBefore)
		{
			this.categoryList.SetCategoryToFirst();
			this.openedBefore = true;
		}
	}

	// Token: 0x06007185 RID: 29061 RVA: 0x002E3D24 File Offset: 0x002E1F24
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateShapesList()
	{
		List<string> favoriteShapes = base.xui.playerUI.entityPlayer.favoriteShapes;
		this.currentItems.Clear();
		this.length = this.shapeGrid.Length;
		string text = this.txtInput.Text;
		XUiC_CategoryEntry currentCategory = this.categoryList.CurrentCategory;
		string text2 = (currentCategory != null) ? currentCategory.CategoryName : null;
		ShapesFromXml.ShapeCategory shapeCategory = null;
		if (!string.IsNullOrEmpty(text2))
		{
			shapeCategory = ShapesFromXml.shapeCategories[text2];
		}
		for (int i = 0; i < this.altBlocks.Length; i++)
		{
			Block block = this.altBlocks[i];
			string blockName = block.GetBlockName();
			string localizedBlockName = block.GetLocalizedBlockName();
			if ((!this.showFavorites || favoriteShapes.Contains(XUiC_ShapeStack.GetFavoritesEntryName(block))) && (string.IsNullOrEmpty(text) || blockName.ContainsCaseInsensitive(text) || localizedBlockName.ContainsCaseInsensitive(text)) && (shapeCategory == null || block.ShapeCategories.Contains(shapeCategory)) && !block.Properties.GetString("ShapeMenu").EqualsCaseInsensitive("false"))
			{
				this.currentItems.Add(new XUiC_ShapeStackGrid.ShapeData
				{
					Block = block,
					Index = i
				});
			}
		}
		this.currentItems.Sort((XUiC_ShapeStackGrid.ShapeData _shapeA, XUiC_ShapeStackGrid.ShapeData _shapeB) => StringComparer.Ordinal.Compare(_shapeA.Block.SortOrder, _shapeB.Block.SortOrder));
		XUiC_Paging xuiC_Paging = this.pager;
		if (xuiC_Paging != null)
		{
			xuiC_Paging.SetLastPageByElementsAndPageLength(this.currentItems.Count, this.length);
		}
		this.shapeGrid.SetShapes(this.currentItems, this.ItemValue.Meta);
		int num = this.currentItems.FindIndex((XUiC_ShapeStackGrid.ShapeData _data) => _data.Index == this.ItemValue.Meta);
		if (num < 0)
		{
			this.Page = 0;
		}
		else
		{
			this.Page = num / this.length;
		}
		this.resultCount.Text = string.Format(this.lblTotal, this.currentItems.Count.ToString());
	}

	// Token: 0x06007186 RID: 29062 RVA: 0x002E3F21 File Offset: 0x002E2121
	public void UpdateAll()
	{
		this.updateAltList();
		this.updateCategories();
		this.UpdateShapesList();
		this.IsDirty = true;
	}

	// Token: 0x06007187 RID: 29063 RVA: 0x002E3F3C File Offset: 0x002E213C
	public void RefreshItemStack()
	{
		XUiC_ItemStack stackController = this.StackController;
	}

	// Token: 0x06007188 RID: 29064 RVA: 0x002E3F48 File Offset: 0x002E2148
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftTrigger, "igcoCategoryLeft", XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightTrigger, "igcoCategoryRight", XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory, 0f);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonNorth, "igcoToggleFavorite", XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts, 0f);
		int holdingItemIdx = base.xui.playerUI.entityPlayer.inventory.holdingItemIdx;
		XUiC_Toolbelt childByType = ((XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow("toolbelt")).Controller.GetChildByType<XUiC_Toolbelt>();
		base.xui.dragAndDrop.InMenu = true;
		if (childByType != null)
		{
			this.StackController = childByType.GetSlotControl(holdingItemIdx);
			this.StackController.AssembleLock = true;
		}
		this.windowGroup.Controller.GetChildByType<XUiC_WindowNonPagingHeader>().SetHeader(Localization.Get("xuiShapes", false).ToUpper());
		this.UpdateAll();
	}

	// Token: 0x06007189 RID: 29065 RVA: 0x002E408C File Offset: 0x002E228C
	public override void OnClose()
	{
		base.OnClose();
		base.xui.currentToolTip.ToolTip = "";
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuCategory);
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		bool childByType = ((XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow("toolbelt")).Controller.GetChildByType<XUiC_Toolbelt>() != null;
		base.xui.dragAndDrop.InMenu = false;
		if (childByType)
		{
			this.StackController.AssembleLock = false;
			this.StackController.ItemStack = new ItemStack(this.ItemValue, this.StackController.ItemStack.count);
			this.ItemValue = ItemValue.None;
			this.StackController.ForceRefreshItemStack();
		}
		if (base.xui.playerUI.windowManager.IsWindowOpen("windowpaging"))
		{
			base.xui.playerUI.windowManager.Close("windowpaging");
		}
	}

	// Token: 0x0600718A RID: 29066 RVA: 0x002E4190 File Offset: 0x002E2390
	[PublicizedFrom(EAccessModifier.Private)]
	public void CategoryList_CategoryChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.UpdateShapesList();
	}

	// Token: 0x0600718B RID: 29067 RVA: 0x002E4198 File Offset: 0x002E2398
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.viewComponent.IsVisible && this.IsDirty)
		{
			base.RefreshBindings(false);
			this.shapeGrid.IsDirty = true;
		}
	}

	// Token: 0x0600718C RID: 29068 RVA: 0x002E41C9 File Offset: 0x002E23C9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "keyboardonly")
		{
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
			{
				_value = "true";
			}
			else
			{
				_value = "false";
			}
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x04005653 RID: 22099
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label resultCount;

	// Token: 0x04005654 RID: 22100
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ShapeStackGrid shapeGrid;

	// Token: 0x04005655 RID: 22101
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x04005656 RID: 22102
	[PublicizedFrom(EAccessModifier.Private)]
	public int length;

	// Token: 0x04005657 RID: 22103
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showFavorites;

	// Token: 0x04005658 RID: 22104
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button favoritesBtn;

	// Token: 0x04005659 RID: 22105
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x0400565A RID: 22106
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_CategoryList categoryList;

	// Token: 0x0400565B RID: 22107
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging pager;

	// Token: 0x0400565C RID: 22108
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController paintbrushButton;

	// Token: 0x0400565D RID: 22109
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController paintrollerButton;

	// Token: 0x0400565E RID: 22110
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblPaintEyeDropper;

	// Token: 0x0400565F RID: 22111
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblCopyBlock;

	// Token: 0x04005660 RID: 22112
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTotal;

	// Token: 0x04005661 RID: 22113
	public ItemValue ItemValue = ItemValue.None;

	// Token: 0x04005662 RID: 22114
	[PublicizedFrom(EAccessModifier.Private)]
	public Block[] altBlocks;

	// Token: 0x04005663 RID: 22115
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ShapesFromXml.ShapeCategory> shapeCategories = new List<ShapesFromXml.ShapeCategory>();

	// Token: 0x04005664 RID: 22116
	public XUiC_ItemStack StackController;

	// Token: 0x04005665 RID: 22117
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_ShapeStackGrid.ShapeData> currentItems = new List<XUiC_ShapeStackGrid.ShapeData>();

	// Token: 0x04005666 RID: 22118
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openedBefore;
}
