using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C0A RID: 3082
[Preserve]
public class XUiC_AssembleWindowGroup : XUiController
{
	// Token: 0x170009C0 RID: 2496
	// (get) Token: 0x06005E75 RID: 24181 RVA: 0x00265038 File Offset: 0x00263238
	// (set) Token: 0x06005E76 RID: 24182 RVA: 0x00265040 File Offset: 0x00263240
	public ItemStack ItemStack
	{
		get
		{
			return this.itemStack;
		}
		set
		{
			this.itemStack = value;
			this.assembleWindow.ItemStack = value;
			this.grid.CurrentItem = value;
			this.grid.SetParts(this.itemStack.itemValue.Modifications);
			this.grid.AssembleWindow = this.assembleWindow;
			this.cosmeticGrid.CurrentItem = value;
			this.cosmeticGrid.SetParts(this.itemStack.itemValue.CosmeticMods);
			this.cosmeticGrid.AssembleWindow = this.assembleWindow;
		}
	}

	// Token: 0x06005E77 RID: 24183 RVA: 0x002650D0 File Offset: 0x002632D0
	public override void Init()
	{
		base.Init();
		this.assembleWindow = base.GetChildByType<XUiC_AssembleWindow>();
		this.grid = base.GetChildByType<XUiC_ItemPartStackGrid>();
		this.cosmeticGrid = base.GetChildByType<XUiC_ItemCosmeticStackGrid>();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
	}

	// Token: 0x06005E78 RID: 24184 RVA: 0x00265108 File Offset: 0x00263308
	public override void OnOpen()
	{
		this.ItemStack = base.xui.AssembleItem.CurrentItem;
		base.OnOpen();
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(Localization.Get("lblContextActionModify", false));
		}
		windowManager.CloseIfOpen("windowpaging");
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		if (base.xui.AssembleItem.CurrentItemStackController != null)
		{
			base.xui.AssembleItem.CurrentItemStackController.Selected = true;
			childByType.SetItemStack(base.xui.AssembleItem.CurrentItemStackController, true);
			this.openEquipmentOnClose = false;
			return;
		}
		if (base.xui.AssembleItem.CurrentEquipmentStackController != null)
		{
			base.xui.AssembleItem.CurrentEquipmentStackController.Selected = false;
			childByType.SetItemStack(base.xui.AssembleItem.CurrentEquipmentStackController, true);
			this.openEquipmentOnClose = true;
		}
	}

	// Token: 0x06005E79 RID: 24185 RVA: 0x00265202 File Offset: 0x00263402
	public override void OnClose()
	{
		base.OnClose();
		XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
		assembleItem.CurrentItem = null;
		assembleItem.CurrentItemStackController = null;
		assembleItem.CurrentEquipmentStackController = null;
		GameManager.Instance.StartCoroutine(this.showCraftingLater());
	}

	// Token: 0x06005E7A RID: 24186 RVA: 0x0026523A File Offset: 0x0026343A
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator showCraftingLater()
	{
		yield return new WaitForEndOfFrame();
		if (base.xui != null && base.xui.playerUI != null && base.xui.playerUI.entityPlayer != null)
		{
			XUiC_WindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, this.openEquipmentOnClose ? "character" : "crafting");
			base.xui.GetChildByType<XUiC_WindowSelector>().OverrideClose = true;
		}
		yield break;
	}

	// Token: 0x06005E7B RID: 24187 RVA: 0x00265249 File Offset: 0x00263449
	public static XUiC_AssembleWindowGroup GetWindowGroup(XUi _xuiInstance)
	{
		return _xuiInstance.FindWindowGroupByName(XUiC_AssembleWindowGroup.ID) as XUiC_AssembleWindowGroup;
	}

	// Token: 0x04004742 RID: 18242
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_AssembleWindow assembleWindow;

	// Token: 0x04004743 RID: 18243
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemPartStackGrid grid;

	// Token: 0x04004744 RID: 18244
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemCosmeticStackGrid cosmeticGrid;

	// Token: 0x04004745 RID: 18245
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04004746 RID: 18246
	public static string ID = "assemble";

	// Token: 0x04004747 RID: 18247
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openEquipmentOnClose;

	// Token: 0x04004748 RID: 18248
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack itemStack;
}
