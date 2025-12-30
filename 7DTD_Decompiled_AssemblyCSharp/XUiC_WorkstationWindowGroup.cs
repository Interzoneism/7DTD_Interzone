using System;
using Audio;
using GUI_2;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EB6 RID: 3766
[Preserve]
public class XUiC_WorkstationWindowGroup : XUiC_CraftingWindowGroup
{
	// Token: 0x06007725 RID: 30501 RVA: 0x00307E5D File Offset: 0x0030605D
	public void SetTileEntity(TileEntityWorkstation _te)
	{
		this.WorkstationData = new XUiM_Workstation(_te);
		this.workstationBlock = _te.GetChunk().GetBlock(_te.localChunkPos);
		base.Workstation = this.workstationBlock.Block.GetBlockName();
	}

	// Token: 0x06007726 RID: 30502 RVA: 0x00307E98 File Offset: 0x00306098
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_Destroyed(ITileEntity te)
	{
		if (this.WorkstationData == null || this.WorkstationData.TileEntity == te)
		{
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			return;
		}
		te.Destroyed -= this.TileEntity_Destroyed;
	}

	// Token: 0x06007727 RID: 30503 RVA: 0x00307EEE File Offset: 0x003060EE
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_ToolChanged()
	{
		if (this.toolWindow != null)
		{
			this.toolWindow.SetSlots(this.WorkstationData.GetToolStacks());
			this.SetAllChildrenDirty(false);
		}
	}

	// Token: 0x06007728 RID: 30504 RVA: 0x00307F15 File Offset: 0x00306115
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_OutputChanged()
	{
		if (this.outputWindow != null)
		{
			this.outputWindow.SetSlots(this.WorkstationData.GetOutputStacks());
			this.SetAllChildrenDirty(false);
		}
	}

	// Token: 0x06007729 RID: 30505 RVA: 0x00307F3C File Offset: 0x0030613C
	public void TileEntity_InputChanged()
	{
		if (this.inputWindow != null)
		{
			this.inputWindow.SetSlots(this.WorkstationData.GetInputStacks());
			this.SetAllChildrenDirty(false);
		}
	}

	// Token: 0x0600772A RID: 30506 RVA: 0x00307F63 File Offset: 0x00306163
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_FuelChanged()
	{
		if (this.fuelWindow != null)
		{
			this.fuelWindow.SetSlots(this.WorkstationData.GetFuelStacks());
			this.SetAllChildrenDirty(false);
		}
	}

	// Token: 0x0600772B RID: 30507 RVA: 0x00307F8C File Offset: 0x0030618C
	public override void Init()
	{
		base.Init();
		XUiController xuiController = base.GetChildByType<XUiC_WorkstationToolGrid>();
		if (xuiController != null)
		{
			this.toolWindow = (XUiC_WorkstationToolGrid)xuiController;
		}
		xuiController = base.GetChildByType<XUiC_WorkstationInputGrid>();
		if (xuiController != null)
		{
			this.inputWindow = (XUiC_WorkstationInputGrid)xuiController;
		}
		xuiController = base.GetChildByType<XUiC_WorkstationOutputGrid>();
		if (xuiController != null)
		{
			this.outputWindow = (XUiC_WorkstationOutputGrid)xuiController;
		}
		xuiController = base.GetChildByType<XUiC_WorkstationFuelGrid>();
		if (xuiController != null)
		{
			this.fuelWindow = (XUiC_WorkstationFuelGrid)xuiController;
		}
		xuiController = base.GetChildByType<XUiC_CraftingQueue>();
		if (xuiController != null)
		{
			this.craftingQueue = (XUiC_CraftingQueue)xuiController;
		}
		xuiController = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (xuiController != null)
		{
			this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		}
		xuiController = base.GetChildById("burnTimeLeft");
		if (xuiController != null)
		{
			this.burnTimeLeft = (XUiV_Label)base.GetChildById("burnTimeLeft").ViewComponent;
		}
	}

	// Token: 0x0600772C RID: 30508 RVA: 0x00308050 File Offset: 0x00306250
	public override bool CraftingRequirementsValid(Recipe _recipe)
	{
		bool flag = true;
		if (this.toolWindow != null)
		{
			flag &= this.toolWindow.HasRequirement(_recipe);
		}
		if (this.inputWindow != null)
		{
			flag &= this.inputWindow.HasRequirement(_recipe);
		}
		if (this.outputWindow != null)
		{
			flag &= this.outputWindow.HasRequirement(_recipe);
		}
		if (this.fuelWindow != null)
		{
			flag &= this.fuelWindow.HasRequirement(_recipe);
		}
		return flag;
	}

	// Token: 0x0600772D RID: 30509 RVA: 0x003080BC File Offset: 0x003062BC
	public override string CraftingRequirementsInvalidMessage(Recipe _recipe)
	{
		if (this.toolWindow != null && !this.toolWindow.HasRequirement(_recipe))
		{
			return Localization.Get("ttMissingCraftingTools", false);
		}
		if (this.inputWindow != null && !this.inputWindow.HasRequirement(_recipe))
		{
			return Localization.Get("ttMissingCraftingResources", false);
		}
		if (this.outputWindow != null)
		{
			this.outputWindow.HasRequirement(_recipe);
		}
		if (this.fuelWindow != null && !this.fuelWindow.HasRequirement(_recipe))
		{
			return Localization.Get("ttMissingCraftingFuel", false);
		}
		return "";
	}

	// Token: 0x0600772E RID: 30510 RVA: 0x0030814C File Offset: 0x0030634C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		bool flag = this.craftingQueue.IsCrafting();
		if (flag != this.wasCrafting)
		{
			this.wasCrafting = flag;
			this.syncTEfromUI();
		}
		if (this.windowGroup.isShowing)
		{
			if (this.WorkstationData != null && GameManager.Instance != null && GameManager.Instance.World != null && this.WorkstationData.TileEntity.IsBurning)
			{
				if (this.openTEUpdateTime <= 0f)
				{
					this.WorkstationData.TileEntity.UpdateTick(GameManager.Instance.World);
					this.openTEUpdateTime = 0.5f;
				}
				else
				{
					this.openTEUpdateTime -= _dt;
				}
			}
			if (!base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
			{
				this.wasReleased = true;
			}
			if (this.wasReleased)
			{
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
				{
					this.activeKeyDown = true;
				}
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.WasReleased && this.activeKeyDown)
				{
					this.activeKeyDown = false;
					if (!base.xui.playerUI.windowManager.IsInputActive())
					{
						base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
					}
				}
			}
		}
		if (this.fuelWindow != null && this.craftingQueue != null && this.burnTimeLeft != null && this.WorkstationData != null)
		{
			float totalBurnTimeLeft = this.WorkstationData.GetTotalBurnTimeLeft();
			if ((!this.WorkstationData.GetIsBurning() || totalBurnTimeLeft == 0f) && this.craftingQueue.IsCrafting())
			{
				this.craftingQueue.HaltCrafting();
			}
			else if (this.WorkstationData.GetIsBurning() && totalBurnTimeLeft == 0f && !this.craftingQueue.IsCrafting())
			{
				this.craftingQueue.ResumeCrafting();
			}
			this.burnTimeLeft.Text = string.Format("{0}:{1}", ((int)(totalBurnTimeLeft / 60f)).ToString("00"), ((int)(totalBurnTimeLeft % 60f)).ToString("00"));
		}
	}

	// Token: 0x0600772F RID: 30511 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06007730 RID: 30512 RVA: 0x00308394 File Offset: 0x00306594
	public override bool AddItemToQueue(Recipe _recipe)
	{
		if (this.fuelWindow != null)
		{
			this.fuelWindow.TurnOn();
		}
		return base.AddItemToQueue(_recipe);
	}

	// Token: 0x06007731 RID: 30513 RVA: 0x003083B0 File Offset: 0x003065B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void syncTEfromUI()
	{
		TileEntityWorkstation tileEntity = this.WorkstationData.TileEntity;
		tileEntity.SetDisableModifiedCheck(true);
		if (this.toolWindow != null)
		{
			this.WorkstationData.SetToolStacks(this.toolWindow.GetSlots());
		}
		if (this.inputWindow != null)
		{
			this.WorkstationData.SetInputStacks(this.inputWindow.GetSlots());
		}
		if (this.outputWindow != null)
		{
			this.WorkstationData.SetOutputStacks(this.outputWindow.GetSlots());
		}
		if (this.fuelWindow != null)
		{
			this.WorkstationData.SetFuelStacks(this.fuelWindow.GetSlots());
		}
		if (this.craftingQueue != null)
		{
			XUiC_RecipeStack[] recipesToCraft = this.craftingQueue.GetRecipesToCraft();
			RecipeQueueItem[] array = new RecipeQueueItem[recipesToCraft.Length];
			for (int i = 0; i < recipesToCraft.Length; i++)
			{
				array[i] = new RecipeQueueItem
				{
					Recipe = recipesToCraft[i].GetRecipe(),
					Multiplier = (short)recipesToCraft[i].GetRecipeCount(),
					CraftingTimeLeft = recipesToCraft[i].GetRecipeCraftingTimeLeft(),
					IsCrafting = recipesToCraft[i].IsCrafting,
					Quality = (byte)recipesToCraft[i].OutputQuality,
					StartingEntityId = recipesToCraft[i].StartingEntityId,
					OneItemCraftTime = recipesToCraft[i].GetOneItemCraftTime()
				};
			}
			this.WorkstationData.SetRecipeQueueItems(array);
		}
		tileEntity.SetDisableModifiedCheck(false);
		tileEntity.SetModified();
		tileEntity.ResetTickTime();
	}

	// Token: 0x06007732 RID: 30514 RVA: 0x00308510 File Offset: 0x00306710
	public override void OnOpen()
	{
		this.WorkstationData.SetUserAccessing(true);
		this.WorkstationData.TileEntity.FuelChanged += this.TileEntity_FuelChanged;
		this.WorkstationData.TileEntity.InputChanged += this.TileEntity_InputChanged;
		this.WorkstationData.TileEntity.Destroyed += this.TileEntity_Destroyed;
		base.xui.currentWorkstation = this.workstation;
		this.openTEUpdateTime = 0.5f;
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		if (this.recipeList != null && this.categoryList != null && this.categoryList.SetupCategoriesByWorkstation(base.Workstation))
		{
			this.recipeList.Workstation = base.Workstation;
			this.categoryList.SetCategoryToFirst();
		}
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(Localization.Get(base.Workstation, false));
		}
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		this.syncUIfromTE();
		if (this.craftingQueue != null)
		{
			this.craftingQueue.ClearQueue();
			RecipeQueueItem[] recipeQueueItems = this.WorkstationData.GetRecipeQueueItems();
			for (int i = 0; i < recipeQueueItems.Length; i++)
			{
				if (recipeQueueItems[i] != null)
				{
					this.craftingQueue.AddRecipeToCraftAtIndex(i, recipeQueueItems[i].Recipe, (int)recipeQueueItems[i].Multiplier, recipeQueueItems[i].CraftingTimeLeft, recipeQueueItems[i].IsCrafting, false, (int)recipeQueueItems[i].Quality, recipeQueueItems[i].StartingEntityId, recipeQueueItems[i].OneItemCraftTime);
				}
				else
				{
					this.craftingQueue.AddRecipeToCraftAtIndex(i, null, 0, -1f, false, true, -1, -1, -1f);
				}
			}
			this.craftingQueue.IsDirty = true;
		}
		base.xui.RecenterWindowGroup(this.windowGroup, false);
		for (int j = 0; j < this.children.Count; j++)
		{
			this.children[j].OnOpen();
		}
		WorkstationData workstationData = CraftingManager.GetWorkstationData(this.workstation);
		if (workstationData != null)
		{
			Manager.BroadcastPlayByLocalPlayer(this.WorkstationData.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, workstationData.OpenSound);
			this.WorkstationData.TileEntity.CheckForCraftComplete(base.xui.playerUI.entityPlayer);
		}
		this.IsDirty = true;
		base.IsOpen = true;
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightBumper, "igcoWorkstationTurnOnOff", XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts, 0f);
	}

	// Token: 0x06007733 RID: 30515 RVA: 0x003087BC File Offset: 0x003069BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void syncUIfromTE()
	{
		if (this.WorkstationData != null && GameManager.Instance != null && GameManager.Instance.World != null)
		{
			this.WorkstationData.TileEntity.UpdateTick(GameManager.Instance.World);
		}
		if (this.toolWindow != null)
		{
			this.toolWindow.SetSlots(this.WorkstationData.GetToolStacks());
			this.toolWindow.IsDirty = true;
		}
		if (this.inputWindow != null)
		{
			this.inputWindow.SetSlots(this.WorkstationData.GetInputStacks());
			this.inputWindow.IsDirty = true;
		}
		if (this.outputWindow != null)
		{
			this.outputWindow.SetSlots(this.WorkstationData.GetOutputStacks());
			this.outputWindow.IsDirty = true;
		}
		if (this.fuelWindow != null)
		{
			this.fuelWindow.SetSlots(this.WorkstationData.GetFuelStacks());
			this.fuelWindow.IsDirty = true;
		}
		this.SetAllChildrenDirty(false);
	}

	// Token: 0x06007734 RID: 30516 RVA: 0x003088B4 File Offset: 0x00306AB4
	public override void OnClose()
	{
		this.wasReleased = false;
		this.activeKeyDown = false;
		this.syncTEfromUI();
		this.WorkstationData.SetUserAccessing(false);
		this.WorkstationData.TileEntity.FuelChanged -= this.TileEntity_FuelChanged;
		this.WorkstationData.TileEntity.InputChanged -= this.TileEntity_InputChanged;
		this.WorkstationData.TileEntity.Destroyed -= this.TileEntity_Destroyed;
		base.OnClose();
		GameManager.Instance.TEUnlockServer(this.WorkstationData.TileEntity.GetClrIdx(), this.WorkstationData.TileEntity.ToWorldPos(), this.WorkstationData.TileEntity.entityId, true);
		WorkstationData workstationData = CraftingManager.GetWorkstationData(this.workstation);
		if (workstationData != null)
		{
			Manager.BroadcastPlayByLocalPlayer(this.WorkstationData.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, workstationData.CloseSound);
		}
		base.xui.currentWorkstation = "";
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
	}

	// Token: 0x04005AB8 RID: 23224
	public XUiM_Workstation WorkstationData;

	// Token: 0x04005AB9 RID: 23225
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue workstationBlock;

	// Token: 0x04005ABA RID: 23226
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationToolGrid toolWindow;

	// Token: 0x04005ABB RID: 23227
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationInputGrid inputWindow;

	// Token: 0x04005ABC RID: 23228
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationOutputGrid outputWindow;

	// Token: 0x04005ABD RID: 23229
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationFuelGrid fuelWindow;

	// Token: 0x04005ABE RID: 23230
	[PublicizedFrom(EAccessModifier.Private)]
	public new XUiC_CraftingQueue craftingQueue;

	// Token: 0x04005ABF RID: 23231
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label burnTimeLeft;

	// Token: 0x04005AC0 RID: 23232
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04005AC1 RID: 23233
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasCrafting;

	// Token: 0x04005AC2 RID: 23234
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04005AC3 RID: 23235
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;

	// Token: 0x04005AC4 RID: 23236
	[PublicizedFrom(EAccessModifier.Private)]
	public float openTEUpdateTime;

	// Token: 0x04005AC5 RID: 23237
	[PublicizedFrom(EAccessModifier.Private)]
	public const float openTEUpdateTimeMax = 0.5f;
}
