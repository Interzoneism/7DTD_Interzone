using System;
using System.Collections;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DDD RID: 3549
[Preserve]
public class XUiC_RecipeStack : XUiController
{
	// Token: 0x17000B2E RID: 2862
	// (get) Token: 0x06006F31 RID: 28465 RVA: 0x002D609C File Offset: 0x002D429C
	// (set) Token: 0x06006F32 RID: 28466 RVA: 0x002D60A4 File Offset: 0x002D42A4
	public int OutputQuality
	{
		get
		{
			return this.outputQuality;
		}
		set
		{
			this.outputQuality = value;
		}
	}

	// Token: 0x17000B2F RID: 2863
	// (get) Token: 0x06006F33 RID: 28467 RVA: 0x002D60AD File Offset: 0x002D42AD
	// (set) Token: 0x06006F34 RID: 28468 RVA: 0x002D60B5 File Offset: 0x002D42B5
	public int StartingEntityId
	{
		get
		{
			return this.startingEntityId;
		}
		set
		{
			this.startingEntityId = value;
		}
	}

	// Token: 0x17000B30 RID: 2864
	// (get) Token: 0x06006F35 RID: 28469 RVA: 0x002D60BE File Offset: 0x002D42BE
	// (set) Token: 0x06006F36 RID: 28470 RVA: 0x002D60C6 File Offset: 0x002D42C6
	public ItemValue OriginalItem
	{
		get
		{
			return this.originalItem;
		}
		set
		{
			this.originalItem = value;
		}
	}

	// Token: 0x17000B31 RID: 2865
	// (get) Token: 0x06006F37 RID: 28471 RVA: 0x002D60CF File Offset: 0x002D42CF
	// (set) Token: 0x06006F38 RID: 28472 RVA: 0x002D60D7 File Offset: 0x002D42D7
	public int AmountToRepair
	{
		get
		{
			return this.amountToRepair;
		}
		set
		{
			this.amountToRepair = value;
		}
	}

	// Token: 0x17000B32 RID: 2866
	// (get) Token: 0x06006F39 RID: 28473 RVA: 0x002D60E0 File Offset: 0x002D42E0
	// (set) Token: 0x06006F3A RID: 28474 RVA: 0x002D60E8 File Offset: 0x002D42E8
	public bool IsCrafting
	{
		get
		{
			return this.isCrafting;
		}
		set
		{
			this.isCrafting = value;
		}
	}

	// Token: 0x17000B33 RID: 2867
	// (get) Token: 0x06006F3B RID: 28475 RVA: 0x002D60F1 File Offset: 0x002D42F1
	// (set) Token: 0x06006F3C RID: 28476 RVA: 0x002D6116 File Offset: 0x002D4316
	public string LockIconSprite
	{
		get
		{
			if (this.lockIcon != null)
			{
				return ((XUiV_Sprite)this.lockIcon.ViewComponent).SpriteName;
			}
			return "";
		}
		set
		{
			if (this.lockIcon != null)
			{
				((XUiV_Sprite)this.lockIcon.ViewComponent).SpriteName = value;
			}
		}
	}

	// Token: 0x06006F3D RID: 28477 RVA: 0x002D6138 File Offset: 0x002D4338
	public void CopyTo(XUiC_RecipeStack _recipeStack)
	{
		_recipeStack.recipe = this.recipe;
		_recipeStack.craftingTimeLeft = this.craftingTimeLeft;
		_recipeStack.totalCraftTimeLeft = this.totalCraftTimeLeft;
		_recipeStack.recipeCount = this.recipeCount;
		_recipeStack.IsCrafting = this.IsCrafting;
		_recipeStack.originalItem = this.originalItem;
		_recipeStack.amountToRepair = this.amountToRepair;
		_recipeStack.LockIconSprite = this.LockIconSprite;
		_recipeStack.outputQuality = this.outputQuality;
		_recipeStack.startingEntityId = this.startingEntityId;
		_recipeStack.outputItemValue = this.outputItemValue;
		_recipeStack.oneItemCraftTime = this.oneItemCraftTime;
	}

	// Token: 0x06006F3E RID: 28478 RVA: 0x002D61D8 File Offset: 0x002D43D8
	public override void Init()
	{
		base.Init();
		this.background = base.GetChildById("background");
		this.overlay = base.GetChildById("overlay");
		this.lockIcon = base.GetChildById("lockIcon");
		this.itemIcon = base.GetChildById("itemIcon");
		this.timer = base.GetChildById("timer");
		this.count = base.GetChildById("count");
		this.cancel = base.GetChildById("cancel");
		if (this.background != null)
		{
			this.background.OnPress += this.HandleOnPress;
			this.background.OnHover += this.HandleOnHover;
		}
		this.inventoryFullDropping = Localization.Get("xuiInventoryFullDropping", false);
	}

	// Token: 0x06006F3F RID: 28479 RVA: 0x002D62A9 File Offset: 0x002D44A9
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnHover(XUiController _sender, bool _isOver)
	{
		this.isOver = _isOver;
	}

	// Token: 0x06006F40 RID: 28480 RVA: 0x002D62B2 File Offset: 0x002D44B2
	public void ForceCancel()
	{
		this.HandleOnPress(null, -1);
	}

	// Token: 0x06006F41 RID: 28481 RVA: 0x002D62BC File Offset: 0x002D44BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnPress(XUiController _sender, int _mouseButton)
	{
		if (this.recipe == null)
		{
			return;
		}
		XUiC_WorkstationMaterialInputGrid childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationMaterialInputGrid>();
		XUiC_WorkstationInputGrid childByType2 = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationInputGrid>();
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (childByType != null)
		{
			for (int i = 0; i < this.recipe.ingredients.Count; i++)
			{
				childByType.SetWeight(this.recipe.ingredients[i].itemValue.Clone(), this.recipe.ingredients[i].count * this.recipeCount);
			}
		}
		else
		{
			if (this.originalItem != null && !this.originalItem.Equals(ItemValue.None))
			{
				ItemStack itemStack = new ItemStack(this.originalItem.Clone(), 1);
				if (!base.xui.PlayerInventory.AddItem(itemStack))
				{
					GameManager.ShowTooltip(entityPlayer, this.inventoryFullDropping, false, false, 0f);
					GameManager.Instance.ItemDropServer(new ItemStack(this.originalItem.Clone(), 1), entityPlayer.position, Vector3.zero, entityPlayer.entityId, 120f, false);
				}
				this.originalItem = ItemValue.None.Clone();
			}
			int[] array = new int[this.recipe.ingredients.Count];
			for (int j = 0; j < this.recipe.ingredients.Count; j++)
			{
				array[j] = this.recipe.ingredients[j].count * this.recipeCount;
				ItemStack itemStack2 = new ItemStack(this.recipe.ingredients[j].itemValue.Clone(), array[j]);
				bool flag;
				if (childByType2 != null)
				{
					flag = (childByType2.AddToItemStackArray(itemStack2) != -1);
				}
				else
				{
					flag = base.xui.PlayerInventory.AddItem(itemStack2, true);
				}
				if (flag)
				{
					array[j] = 0;
				}
				else
				{
					array[j] = itemStack2.count;
				}
			}
			bool flag2 = false;
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k] > 0)
				{
					flag2 = true;
					GameManager.Instance.ItemDropServer(new ItemStack(this.recipe.ingredients[k].itemValue.Clone(), array[k]), entityPlayer.position, Vector3.zero, entityPlayer.entityId, 120f, false);
				}
			}
			if (flag2)
			{
				GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, this.inventoryFullDropping, false, false, 0f);
			}
		}
		this.isCrafting = false;
		this.ClearRecipe();
		XUiC_CraftingQueue owner = this.Owner;
		if (owner != null)
		{
			owner.RefreshQueue();
		}
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x06006F42 RID: 28482 RVA: 0x002D6588 File Offset: 0x002D4788
	public override void Update(float _dt)
	{
		if (this.isInventoryFull)
		{
			if (this.recipe != null && this.outputItemValue != null)
			{
				XUiC_WorkstationOutputGrid childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationOutputGrid>();
				bool flag = false;
				ItemStack[] array = new ItemStack[0];
				if (childByType != null)
				{
					array = childByType.GetSlots();
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].CanStackWith(new ItemStack(this.outputItemValue, this.recipe.count), false))
						{
							array[i].count += this.recipe.count;
							flag = true;
							break;
						}
						if (array[i].IsEmpty())
						{
							array[i] = new ItemStack(this.outputItemValue, this.recipe.count);
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					childByType.SetSlots(array);
					childByType.UpdateData(array);
					childByType.IsDirty = true;
					this.isInventoryFull = false;
					this.recipeCount--;
					if (this.recipeCount <= 0)
					{
						this.isCrafting = false;
						if (this.recipe != null || this.craftingTimeLeft != 0f)
						{
							this.ClearRecipe();
						}
					}
					else
					{
						this.craftingTimeLeft += this.oneItemCraftTime;
					}
					base.Update(_dt);
					return;
				}
				if (!base.xui.dragAndDrop.CurrentStack.IsEmpty() && base.xui.dragAndDrop.CurrentStack.itemValue.ItemClass is ItemClassQuest)
				{
					base.Update(_dt);
					return;
				}
				ItemStack itemStack = new ItemStack(this.outputItemValue, this.recipe.count);
				if (!base.xui.PlayerInventory.AddItemNoPartial(itemStack, false))
				{
					this.updateRecipeData();
					if (itemStack.count != this.recipe.count)
					{
						base.xui.PlayerInventory.DropItem(itemStack);
						QuestEventManager.Current.CraftedItem(itemStack);
						this.isInventoryFull = false;
						this.recipeCount--;
						if (this.recipeCount <= 0)
						{
							this.isCrafting = false;
							if (this.recipe != null || this.craftingTimeLeft != 0f)
							{
								this.ClearRecipe();
							}
						}
						else
						{
							this.craftingTimeLeft += this.oneItemCraftTime;
						}
					}
					base.Update(_dt);
					return;
				}
				QuestEventManager.Current.CraftedItem(new ItemStack(this.outputItemValue, this.recipe.count));
				this.isInventoryFull = false;
				this.recipeCount--;
				if (this.recipeCount <= 0)
				{
					this.isCrafting = false;
					if (this.recipe != null || this.craftingTimeLeft != 0f)
					{
						this.ClearRecipe();
					}
				}
				else
				{
					this.craftingTimeLeft += this.oneItemCraftTime;
				}
				base.Update(_dt);
				return;
			}
			else
			{
				this.isInventoryFull = false;
				this.isCrafting = false;
			}
		}
		if (this.recipe == null)
		{
			this.isCrafting = false;
		}
		if (this.recipeCount > 0)
		{
			if (this.isCrafting && this.craftingTimeLeft <= 0f && this.recipe != null && this.outputStack())
			{
				this.recipeCount--;
				if (this.recipeCount <= 0)
				{
					this.isCrafting = false;
					if (this.recipe != null || this.craftingTimeLeft != 0f)
					{
						this.ClearRecipe();
					}
				}
				else
				{
					this.craftingTimeLeft += this.oneItemCraftTime;
				}
			}
		}
		else
		{
			this.isCrafting = false;
			if (this.recipe != null && (this.recipe != null || this.craftingTimeLeft != 0f))
			{
				this.ClearRecipe();
			}
		}
		if (base.ViewComponent.IsVisible)
		{
			this.updateRecipeData();
		}
		if (this.recipeCount > 0 && this.isCrafting)
		{
			this.craftingTimeLeft -= _dt;
			this.totalCraftTimeLeft = this.oneItemCraftTime * ((float)this.recipeCount - 1f) + this.craftingTimeLeft;
		}
		else
		{
			if (this.craftingTimeLeft < 0f)
			{
				this.craftingTimeLeft = 0f;
			}
			if (this.totalCraftTimeLeft < 0f)
			{
				this.totalCraftTimeLeft = 0f;
			}
		}
		base.Update(_dt);
	}

	// Token: 0x06006F43 RID: 28483 RVA: 0x002D69A0 File Offset: 0x002D4BA0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool outputStack()
	{
		if (this.recipe == null)
		{
			return false;
		}
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (entityPlayer == null)
		{
			return false;
		}
		if (this.originalItem == null || this.originalItem.Equals(ItemValue.None))
		{
			this.outputItemValue = new ItemValue(this.recipe.itemValueType, this.outputQuality, this.outputQuality, false, null, 1f);
			ItemClass itemClass = this.outputItemValue.ItemClass;
			if (this.outputItemValue == null)
			{
				return false;
			}
			if (itemClass == null)
			{
				return false;
			}
			if (entityPlayer.entityId == this.startingEntityId)
			{
				this.giveExp(this.outputItemValue, itemClass);
			}
			else
			{
				XUiC_WorkstationWindowGroup xuiC_WorkstationWindowGroup = this.windowGroup.Controller as XUiC_WorkstationWindowGroup;
				if (xuiC_WorkstationWindowGroup != null)
				{
					xuiC_WorkstationWindowGroup.WorkstationData.TileEntity.AddCraftComplete(this.startingEntityId, this.outputItemValue, this.recipe.GetName(), this.recipe.IsScrap ? this.recipe.ingredients[0].itemValue.ItemClass.GetItemName() : "", this.recipe.craftExpGain, this.recipe.count);
				}
			}
			if (this.recipe.GetName().Equals("meleeToolRepairT0StoneAxe"))
			{
				IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager != null)
				{
					achievementManager.SetAchievementStat(EnumAchievementDataStat.StoneAxeCrafted, 1);
				}
			}
			else if (this.recipe.GetName().Equals("frameShapes:VariantHelper"))
			{
				IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager2 != null)
				{
					achievementManager2.SetAchievementStat(EnumAchievementDataStat.WoodFrameCrafted, 1);
				}
			}
		}
		else if (this.amountToRepair > 0)
		{
			ItemValue itemValue = this.originalItem.Clone();
			itemValue.UseTimes -= (float)this.amountToRepair;
			ItemClass itemClass2 = itemValue.ItemClass;
			if (itemValue.UseTimes < 0f)
			{
				itemValue.UseTimes = 0f;
			}
			this.outputItemValue = itemValue.Clone();
			QuestEventManager.Current.RepairedItem(this.outputItemValue);
			this.amountToRepair = 0;
		}
		if (this.outputItemValue != null)
		{
			GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.CraftedItems, this.outputItemValue.ItemClass.Name, this.recipe.count, true, GameSparksCollector.GSDataCollection.SessionUpdates);
		}
		else
		{
			this.outputItemValue = this.originalItem;
		}
		XUiC_WorkstationOutputGrid childByType = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationOutputGrid>();
		if (childByType != null && (this.originalItem == null || this.originalItem.Equals(ItemValue.None)))
		{
			ItemStack itemStack = new ItemStack(this.outputItemValue, this.recipe.count);
			ItemStack[] slots = childByType.GetSlots();
			bool flag = false;
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].CanStackWith(itemStack, false))
				{
					slots[i].count += this.recipe.count;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < slots.Length; j++)
				{
					if (slots[j].IsEmpty())
					{
						slots[j] = itemStack;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				childByType.SetSlots(slots);
				childByType.UpdateData(slots);
				childByType.IsDirty = true;
				if (this.recipe.IsScrap)
				{
					QuestEventManager.Current.ScrappedItem(this.recipe.ingredients[0]);
					entityPlayer.equipment.UnlockCosmeticItem(this.recipe.ingredients[0].itemValue.ItemClass);
				}
				else
				{
					QuestEventManager.Current.CraftedItem(itemStack);
				}
				if (this.playSound)
				{
					if (this.recipe.craftingArea != null)
					{
						WorkstationData workstationData = CraftingManager.GetWorkstationData(this.recipe.craftingArea);
						if (workstationData != null)
						{
							Manager.PlayInsidePlayerHead(workstationData.CraftCompleteSound, -1, 0f, false, false);
						}
					}
					else
					{
						Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
					}
				}
			}
			else if (!this.AddItemToInventory())
			{
				this.isInventoryFull = true;
				string text = "No room in workstation output, crafting has been halted until space is cleared.";
				if (Localization.Exists("wrnWorkstationOutputFull", false))
				{
					text = Localization.Get("wrnWorkstationOutputFull", false);
				}
				GameManager.ShowTooltip(entityPlayer, text, false, false, 0f);
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				return false;
			}
		}
		else
		{
			if (!base.xui.dragAndDrop.CurrentStack.IsEmpty() && base.xui.dragAndDrop.CurrentStack.itemValue.ItemClass is ItemClassQuest)
			{
				return false;
			}
			ItemStack itemStack2 = new ItemStack(this.outputItemValue, this.recipe.count);
			if (!base.xui.PlayerInventory.AddItemNoPartial(itemStack2, false))
			{
				if (itemStack2.count != this.recipe.count)
				{
					base.xui.PlayerInventory.DropItem(itemStack2);
					QuestEventManager.Current.CraftedItem(itemStack2);
					return true;
				}
				this.isInventoryFull = true;
				string text2 = "No room in inventory, crafting has been halted until space is cleared.";
				if (Localization.Exists("wrnInventoryFull", false))
				{
					text2 = Localization.Get("wrnInventoryFull", false);
				}
				GameManager.ShowTooltip(entityPlayer, text2, false, false, 0f);
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				return false;
			}
			else
			{
				if (this.originalItem != null && !this.originalItem.IsEmpty())
				{
					if (this.recipe.ingredients.Count > 0)
					{
						QuestEventManager.Current.ScrappedItem(this.recipe.ingredients[0]);
						entityPlayer.equipment.UnlockCosmeticItem(this.recipe.ingredients[0].itemValue.ItemClass);
					}
				}
				else
				{
					itemStack2.count = this.recipe.count - itemStack2.count;
					if (this.recipe.IsScrap)
					{
						QuestEventManager.Current.ScrappedItem(this.recipe.ingredients[0]);
						entityPlayer.equipment.UnlockCosmeticItem(this.recipe.ingredients[0].itemValue.ItemClass);
					}
					else
					{
						QuestEventManager.Current.CraftedItem(itemStack2);
					}
				}
				if (this.playSound)
				{
					Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
				}
			}
		}
		if (!this.isInventoryFull)
		{
			this.originalItem = ItemValue.None.Clone();
		}
		return true;
	}

	// Token: 0x06006F44 RID: 28484 RVA: 0x002D6FE4 File Offset: 0x002D51E4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool AddItemToInventory()
	{
		ItemStack itemStack = new ItemStack(this.outputItemValue, this.recipe.count);
		if (!base.xui.PlayerInventory.AddItemNoPartial(itemStack, false))
		{
			this.updateRecipeData();
			return false;
		}
		QuestEventManager.Current.CraftedItem(new ItemStack(this.outputItemValue, this.recipe.count));
		this.isInventoryFull = false;
		if (this.playSound)
		{
			if (this.recipe.craftingArea != null)
			{
				WorkstationData workstationData = CraftingManager.GetWorkstationData(this.recipe.craftingArea);
				if (workstationData != null)
				{
					Manager.PlayInsidePlayerHead(workstationData.CraftCompleteSound, -1, 0f, false, false);
				}
			}
			else
			{
				Manager.PlayInsidePlayerHead("craft_complete_item", -1, 0f, false, false);
			}
		}
		if (this.recipeCount <= 0)
		{
			this.isCrafting = false;
			if (this.recipe != null || this.craftingTimeLeft != 0f)
			{
				this.ClearRecipe();
			}
		}
		return true;
	}

	// Token: 0x06006F45 RID: 28485 RVA: 0x002D70C8 File Offset: 0x002D52C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void giveExp(ItemValue _iv, ItemClass _ic)
	{
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		int num = (int)base.xui.playerUI.entityPlayer.Buffs.GetCustomVar("_craftCount_" + this.recipe.GetName());
		base.xui.playerUI.entityPlayer.Buffs.SetCustomVar("_craftCount_" + this.recipe.GetName(), (float)(num + 1), true, CVarOperation.set);
		base.xui.playerUI.entityPlayer.Progression.AddLevelExp(this.recipe.craftExpGain / (num + 1), "_xpFromCrafting", Progression.XPTypes.Crafting, true, true);
		entityPlayer.totalItemsCrafted += 1U;
		XUiC_RecipeStack.itemCraftedAchievementUpdate();
	}

	// Token: 0x06006F46 RID: 28486 RVA: 0x002D718F File Offset: 0x002D538F
	public bool HasRecipe()
	{
		return this.recipe != null;
	}

	// Token: 0x06006F47 RID: 28487 RVA: 0x002D719A File Offset: 0x002D539A
	public Recipe GetRecipe()
	{
		return this.recipe;
	}

	// Token: 0x06006F48 RID: 28488 RVA: 0x002D71A2 File Offset: 0x002D53A2
	public void ClearRecipe()
	{
		this.SetRecipe(null, 0, 0f, true, -1, -1, -1f);
	}

	// Token: 0x06006F49 RID: 28489 RVA: 0x002D71BA File Offset: 0x002D53BA
	public int GetRecipeCount()
	{
		return this.recipeCount;
	}

	// Token: 0x06006F4A RID: 28490 RVA: 0x002D71C2 File Offset: 0x002D53C2
	public float GetRecipeCraftingTimeLeft()
	{
		return this.craftingTimeLeft;
	}

	// Token: 0x06006F4B RID: 28491 RVA: 0x002D71CA File Offset: 0x002D53CA
	public float GetTotalRecipeCraftingTimeLeft()
	{
		return this.totalCraftTimeLeft;
	}

	// Token: 0x06006F4C RID: 28492 RVA: 0x002D71D2 File Offset: 0x002D53D2
	public float GetOneItemCraftTime()
	{
		return this.oneItemCraftTime;
	}

	// Token: 0x06006F4D RID: 28493 RVA: 0x002D71DC File Offset: 0x002D53DC
	public bool SetRepairRecipe(float _repairTimeLeft, ItemValue _itemToRepair, int _amountToRepair)
	{
		if (this.isCrafting || (this.originalItem != null && this.originalItem.type != 0))
		{
			return false;
		}
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		this.recipeCount = 1;
		this.craftingTimeLeft = _repairTimeLeft;
		this.originalItem = _itemToRepair.Clone();
		this.amountToRepair = _amountToRepair;
		this.totalCraftTimeLeft = _repairTimeLeft;
		this.oneItemCraftTime = _repairTimeLeft;
		if (this.lockIcon != null && _itemToRepair.type != 0)
		{
			((XUiV_Sprite)this.lockIcon.ViewComponent).SpriteName = "ui_game_symbol_wrench";
		}
		this.outputQuality = (int)this.originalItem.Quality;
		this.StartingEntityId = entityPlayer.entityId;
		this.recipe = new Recipe();
		this.recipe.craftingTime = _repairTimeLeft;
		this.recipe.count = 1;
		this.recipe.itemValueType = this.originalItem.type;
		this.recipe.craftExpGain = Mathf.Clamp(this.amountToRepair, 0, 200);
		ItemClass itemClass = this.originalItem.ItemClass;
		if (itemClass.RepairTools != null && itemClass.RepairTools.Length > 0)
		{
			ItemClass itemClass2 = ItemClass.GetItemClass(itemClass.RepairTools[0].Value, false);
			if (itemClass2 != null)
			{
				int num = Mathf.CeilToInt((float)_amountToRepair / (float)itemClass2.RepairAmount.Value);
				this.recipe.ingredients.Add(new ItemStack(ItemClass.GetItem(itemClass.RepairTools[0].Value, false), num));
			}
		}
		this.updateRecipeData();
		return true;
	}

	// Token: 0x06006F4E RID: 28494 RVA: 0x002D736C File Offset: 0x002D556C
	public bool SetRecipe(Recipe _recipe, int _count = 1, float craftTime = -1f, bool recipeModification = false, int startingEntityId = -1, int _outputQuality = -1, float _oneItemCraftTime = -1f)
	{
		if ((this.isCrafting || (this.recipe != null && _recipe != null)) && !recipeModification)
		{
			return false;
		}
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (startingEntityId == -1)
		{
			startingEntityId = entityPlayer.entityId;
		}
		this.StartingEntityId = startingEntityId;
		this.recipe = _recipe;
		this.recipeCount = _count;
		this.craftingTimeLeft = ((craftTime == -1f) ? ((_recipe != null) ? _recipe.craftingTime : 0f) : craftTime);
		if (this.originalItem != null && !this.originalItem.Equals(ItemValue.None))
		{
			this.originalItem = ItemValue.None.Clone();
		}
		this.amountToRepair = 0;
		this.oneItemCraftTime = ((_oneItemCraftTime == -1f) ? ((_recipe != null) ? _recipe.craftingTime : 0f) : _oneItemCraftTime);
		this.totalCraftTimeLeft = this.oneItemCraftTime * ((float)_count - 1f) + this.craftingTimeLeft;
		if (this.lockIcon != null && this.recipe != null)
		{
			WorkstationData workstationData = CraftingManager.GetWorkstationData(this.recipe.craftingArea);
			if (workstationData != null)
			{
				((XUiV_Sprite)this.lockIcon.ViewComponent).SpriteName = workstationData.CraftIcon;
			}
		}
		if (_outputQuality == -1)
		{
			if (this.recipe != null)
			{
				this.outputQuality = this.recipe.craftingTier;
			}
			else
			{
				this.outputQuality = 1;
			}
		}
		else
		{
			this.outputQuality = _outputQuality;
		}
		this.ClearDisplayFromLastRecipe();
		this.updateRecipeData();
		return true;
	}

	// Token: 0x06006F4F RID: 28495 RVA: 0x002D74D4 File Offset: 0x002D56D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearDisplayFromLastRecipe()
	{
		if (this.timer != null)
		{
			((XUiV_Label)this.timer.ViewComponent).SetTextImmediately("");
			this.timer.ViewComponent.IsVisible = true;
		}
		if (this.count != null)
		{
			this.count.ViewComponent.IsVisible = true;
			((XUiV_Label)this.count.ViewComponent).SetTextImmediately("");
		}
		if (this.cancel != null)
		{
			Color color = ((XUiV_Sprite)this.cancel.ViewComponent).Color;
			((XUiV_Sprite)this.cancel.ViewComponent).SetColorImmediately(new Color(color.r, color.g, color.b, 0f));
		}
	}

	// Token: 0x06006F50 RID: 28496 RVA: 0x002D7598 File Offset: 0x002D5798
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateRecipeData()
	{
		if (this.recipe == null && (this.originalItem == null || this.originalItem.type == 0))
		{
			if (this.lockIcon != null)
			{
				this.lockIcon.ViewComponent.IsVisible = false;
			}
			if (this.overlay != null)
			{
				this.overlay.ViewComponent.IsVisible = false;
			}
			if (this.itemIcon != null)
			{
				this.itemIcon.ViewComponent.IsVisible = false;
			}
			if (this.timer != null)
			{
				this.timer.ViewComponent.IsVisible = false;
			}
			if (this.count != null)
			{
				this.count.ViewComponent.IsVisible = false;
			}
			if (this.cancel != null)
			{
				this.cancel.ViewComponent.IsVisible = false;
				return;
			}
		}
		else
		{
			if (this.lockIcon != null)
			{
				this.lockIcon.ViewComponent.IsVisible = true;
			}
			if (this.overlay != null)
			{
				this.overlay.ViewComponent.IsVisible = true;
			}
			if (this.itemIcon != null)
			{
				ItemClass itemClass = (this.recipe != null) ? ItemClass.GetForId(this.recipe.itemValueType) : this.originalItem.ItemClass;
				if (itemClass != null)
				{
					((XUiV_Sprite)this.itemIcon.ViewComponent).SetSpriteImmediately(itemClass.GetIconName());
					this.itemIcon.ViewComponent.IsVisible = true;
					((XUiV_Sprite)this.itemIcon.ViewComponent).Color = itemClass.GetIconTint(null);
				}
			}
			if (this.timer != null)
			{
				((XUiV_Label)this.timer.ViewComponent).SetTextImmediately(this.craftingTimeToString(this.totalCraftTimeLeft + 0.5f));
				this.timer.ViewComponent.IsVisible = true;
			}
			if (this.count != null)
			{
				this.count.ViewComponent.IsVisible = true;
				((XUiV_Label)this.count.ViewComponent).SetTextImmediately((this.recipeCount * this.recipe.count).ToString());
			}
			if (this.cancel != null)
			{
				Color color = ((XUiV_Sprite)this.cancel.ViewComponent).Color;
				if (this.isOver && UICamera.hoveredObject == this.background.ViewComponent.UiTransform.gameObject)
				{
					((XUiV_Sprite)this.cancel.ViewComponent).Color = new Color(color.r, color.g, color.b, 0.75f);
				}
				else
				{
					((XUiV_Sprite)this.cancel.ViewComponent).Color = new Color(color.r, color.g, color.b, 0f);
				}
				this.cancel.ViewComponent.IsVisible = true;
			}
		}
	}

	// Token: 0x06006F51 RID: 28497 RVA: 0x002D7854 File Offset: 0x002D5A54
	public override void OnOpen()
	{
		base.OnOpen();
		this.isInventoryFull = false;
		if (this.cancel != null)
		{
			this.isOver = false;
			Color color = ((XUiV_Sprite)this.cancel.ViewComponent).Color;
			((XUiV_Sprite)this.cancel.ViewComponent).Color = new Color(color.r, color.g, color.b, 0f);
		}
		this.playSound = true;
	}

	// Token: 0x06006F52 RID: 28498 RVA: 0x002D78CB File Offset: 0x002D5ACB
	public override void OnClose()
	{
		base.OnClose();
		this.playSound = false;
	}

	// Token: 0x06006F53 RID: 28499 RVA: 0x002D78DC File Offset: 0x002D5ADC
	[PublicizedFrom(EAccessModifier.Private)]
	public string craftingTimeToString(float time)
	{
		return string.Format("{0}:{1}", ((int)(time / 60f)).ToString("0").PadLeft(2, '0'), ((int)(time % 60f)).ToString("0").PadLeft(2, '0'));
	}

	// Token: 0x06006F54 RID: 28500 RVA: 0x002D792D File Offset: 0x002D5B2D
	public override void Cleanup()
	{
		base.Cleanup();
		this.stopAchievementUpdateCoroutine();
	}

	// Token: 0x06006F55 RID: 28501 RVA: 0x002D793B File Offset: 0x002D5B3B
	public static void HandleCraftXPGained()
	{
		XUiC_RecipeStack.itemCraftedAchievementUpdate();
	}

	// Token: 0x06006F56 RID: 28502 RVA: 0x002D7942 File Offset: 0x002D5B42
	[PublicizedFrom(EAccessModifier.Private)]
	public static void itemCraftedAchievementUpdate()
	{
		XUiC_RecipeStack.itemsCraftedSinceLastAchievementUpdate++;
		XUiC_RecipeStack.lastItemCraftedTime = Time.unscaledTime;
		XUiC_RecipeStack.startAchievementUpdateCoroutine();
	}

	// Token: 0x06006F57 RID: 28503 RVA: 0x002D795F File Offset: 0x002D5B5F
	[PublicizedFrom(EAccessModifier.Private)]
	public static void startAchievementUpdateCoroutine()
	{
		if (XUiC_RecipeStack.sendCraftedItemsForAchievementsCoroutine == null)
		{
			XUiC_RecipeStack.sendCraftedItemsForAchievementsCoroutine = ThreadManager.StartCoroutine(XUiC_RecipeStack.sendCraftedItemsForAchievementsCo());
		}
	}

	// Token: 0x06006F58 RID: 28504 RVA: 0x002D7977 File Offset: 0x002D5B77
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopAchievementUpdateCoroutine()
	{
		if (XUiC_RecipeStack.sendCraftedItemsForAchievementsCoroutine != null)
		{
			ThreadManager.StopCoroutine(XUiC_RecipeStack.sendCraftedItemsForAchievementsCoroutine);
			XUiC_RecipeStack.sendCraftedItemsForAchievementsCoroutine = null;
			XUiC_RecipeStack.doSendCraftingStatsForAchievements();
		}
	}

	// Token: 0x06006F59 RID: 28505 RVA: 0x002D7995 File Offset: 0x002D5B95
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator sendCraftedItemsForAchievementsCo()
	{
		if (XUiC_RecipeStack.sendCraftedItemsForAchievementsInterval == null)
		{
			XUiC_RecipeStack.sendCraftedItemsForAchievementsInterval = new WaitForSeconds(30f);
		}
		for (;;)
		{
			yield return XUiC_RecipeStack.sendCraftedItemsForAchievementsInterval;
			XUiC_RecipeStack.doSendCraftingStatsForAchievements();
		}
		yield break;
	}

	// Token: 0x06006F5A RID: 28506 RVA: 0x002D79A0 File Offset: 0x002D5BA0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void doSendCraftingStatsForAchievements()
	{
		if (XUiC_RecipeStack.itemsCraftedSinceLastAchievementUpdate == 0)
		{
			return;
		}
		if (XUiC_RecipeStack.itemsCraftedSinceLastAchievementUpdate >= 20 || Time.unscaledTime > XUiC_RecipeStack.lastItemCraftedTime + 15f || XUiC_RecipeStack.sendCraftedItemsForAchievementsCoroutine == null)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.ItemsCrafted, XUiC_RecipeStack.itemsCraftedSinceLastAchievementUpdate);
			}
			XUiC_RecipeStack.itemsCraftedSinceLastAchievementUpdate = 0;
		}
	}

	// Token: 0x04005465 RID: 21605
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;

	// Token: 0x04005466 RID: 21606
	[PublicizedFrom(EAccessModifier.Private)]
	public int recipeCount;

	// Token: 0x04005467 RID: 21607
	[PublicizedFrom(EAccessModifier.Private)]
	public int recipeTier;

	// Token: 0x04005468 RID: 21608
	[PublicizedFrom(EAccessModifier.Private)]
	public float craftingTimeLeft;

	// Token: 0x04005469 RID: 21609
	[PublicizedFrom(EAccessModifier.Private)]
	public float totalCraftTimeLeft;

	// Token: 0x0400546A RID: 21610
	[PublicizedFrom(EAccessModifier.Private)]
	public float oneItemCraftTime = -1f;

	// Token: 0x0400546B RID: 21611
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isCrafting;

	// Token: 0x0400546C RID: 21612
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInventoryFull;

	// Token: 0x0400546D RID: 21613
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue originalItem;

	// Token: 0x0400546E RID: 21614
	[PublicizedFrom(EAccessModifier.Private)]
	public int amountToRepair;

	// Token: 0x0400546F RID: 21615
	[PublicizedFrom(EAccessModifier.Private)]
	public int outputQuality;

	// Token: 0x04005470 RID: 21616
	[PublicizedFrom(EAccessModifier.Private)]
	public int startingEntityId = -1;

	// Token: 0x04005471 RID: 21617
	public XUiC_CraftingQueue Owner;

	// Token: 0x04005472 RID: 21618
	[PublicizedFrom(EAccessModifier.Private)]
	public bool playSound;

	// Token: 0x04005473 RID: 21619
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController timer;

	// Token: 0x04005474 RID: 21620
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController count;

	// Token: 0x04005475 RID: 21621
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController itemIcon;

	// Token: 0x04005476 RID: 21622
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController lockIcon;

	// Token: 0x04005477 RID: 21623
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController overlay;

	// Token: 0x04005478 RID: 21624
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController background;

	// Token: 0x04005479 RID: 21625
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController cancel;

	// Token: 0x0400547A RID: 21626
	[PublicizedFrom(EAccessModifier.Private)]
	public string inventoryFullDropping;

	// Token: 0x0400547B RID: 21627
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x0400547C RID: 21628
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue outputItemValue;

	// Token: 0x0400547D RID: 21629
	[PublicizedFrom(EAccessModifier.Private)]
	public static Coroutine sendCraftedItemsForAchievementsCoroutine;

	// Token: 0x0400547E RID: 21630
	[PublicizedFrom(EAccessModifier.Private)]
	public static float lastItemCraftedTime;

	// Token: 0x0400547F RID: 21631
	[PublicizedFrom(EAccessModifier.Private)]
	public static int itemsCraftedSinceLastAchievementUpdate;

	// Token: 0x04005480 RID: 21632
	[PublicizedFrom(EAccessModifier.Private)]
	public static WaitForSeconds sendCraftedItemsForAchievementsInterval;
}
