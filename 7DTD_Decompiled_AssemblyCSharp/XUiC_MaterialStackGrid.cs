using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine.Scripting;

// Token: 0x02000D29 RID: 3369
[Preserve]
public class XUiC_MaterialStackGrid : XUiController
{
	// Token: 0x17000AAD RID: 2733
	// (get) Token: 0x060068E7 RID: 26855 RVA: 0x002A9D00 File Offset: 0x002A7F00
	// (set) Token: 0x060068E8 RID: 26856 RVA: 0x002A9D08 File Offset: 0x002A7F08
	public int Length { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000AAE RID: 2734
	// (get) Token: 0x060068E9 RID: 26857 RVA: 0x002A9D11 File Offset: 0x002A7F11
	// (set) Token: 0x060068EA RID: 26858 RVA: 0x002A9D19 File Offset: 0x002A7F19
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			this.page = value;
			this.isDirty = true;
		}
	}

	// Token: 0x060068EB RID: 26859 RVA: 0x002A9D2C File Offset: 0x002A7F2C
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_MaterialStack>(null);
		this.materialControllers = childrenByType;
		this.Length = this.materialControllers.Length;
		this.bAwakeCalled = true;
		this.IsDirty = false;
		this.IsDormant = true;
	}

	// Token: 0x060068EC RID: 26860 RVA: 0x002A9D74 File Offset: 0x002A7F74
	public void SetMaterials(List<BlockTextureData> materialIndexList, int newSelectedMaterial = -1)
	{
		bool isCreative = GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled) && GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled);
		materialIndexList = (from m in materialIndexList
		orderby m.GetLocked(this.xui.playerUI.entityPlayer), m.ID != 0 & isCreative, m.Group, m.SortIndex, m.LocalizedName
		select m).ToList<BlockTextureData>();
		XUiC_MaterialInfoWindow childByType = base.xui.GetChildByType<XUiC_MaterialInfoWindow>();
		int count = materialIndexList.Count;
		this.currentList = materialIndexList;
		if (newSelectedMaterial != -1)
		{
			this.selectedMaterial = BlockTextureData.list[newSelectedMaterial];
			if (this.selectedMaterial.Hidden && !isCreative)
			{
				this.selectedMaterial = null;
			}
		}
		for (int i = 0; i < this.Length; i++)
		{
			int num = i + this.Length * this.page;
			XUiC_MaterialStack xuiC_MaterialStack = (XUiC_MaterialStack)this.materialControllers[i];
			xuiC_MaterialStack.InfoWindow = childByType;
			if (num < count)
			{
				xuiC_MaterialStack.TextureData = materialIndexList[num];
				if (xuiC_MaterialStack.TextureData == this.selectedMaterial)
				{
					xuiC_MaterialStack.Selected = true;
				}
				if (xuiC_MaterialStack.Selected && xuiC_MaterialStack.TextureData != this.selectedMaterial)
				{
					xuiC_MaterialStack.Selected = false;
				}
			}
			else
			{
				xuiC_MaterialStack.TextureData = null;
				if (xuiC_MaterialStack.Selected)
				{
					xuiC_MaterialStack.Selected = false;
				}
			}
		}
		if (this.selectedMaterial == null && newSelectedMaterial != -1)
		{
			for (int j = 0; j < this.materialControllers.Length; j++)
			{
				XUiC_MaterialStack xuiC_MaterialStack2 = this.materialControllers[j] as XUiC_MaterialStack;
				if (xuiC_MaterialStack2.TextureData != null && !xuiC_MaterialStack2.IsLocked)
				{
					xuiC_MaterialStack2.SetSelectedTextureForItem();
					xuiC_MaterialStack2.Selected = true;
					return;
				}
			}
		}
		this.IsDirty = false;
	}

	// Token: 0x060068ED RID: 26861 RVA: 0x002A9F85 File Offset: 0x002A8185
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.IsDormant = false;
	}

	// Token: 0x060068EE RID: 26862 RVA: 0x002A9FAF File Offset: 0x002A81AF
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
		this.IsDormant = true;
	}

	// Token: 0x060068EF RID: 26863 RVA: 0x002A9FDC File Offset: 0x002A81DC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.isDirty)
		{
			int newSelectedMaterial;
			if (base.xui.playerUI.entityPlayer.inventory.holdingItem is ItemClassBlock)
			{
				newSelectedMaterial = (int)(base.xui.playerUI.entityPlayer.inventory.holdingItemItemValue.TextureFullArray[0] & 255L);
			}
			else
			{
				newSelectedMaterial = ((ItemActionTextureBlock.ItemActionTextureBlockData)base.xui.playerUI.entityPlayer.inventory.holdingItemData.actionData[1]).idx;
			}
			this.SetMaterials(this.currentList, newSelectedMaterial);
			this.isDirty = false;
		}
	}

	// Token: 0x04004F22 RID: 20258
	[PublicizedFrom(EAccessModifier.Protected)]
	public int curPageIdx;

	// Token: 0x04004F23 RID: 20259
	[PublicizedFrom(EAccessModifier.Protected)]
	public int numPages;

	// Token: 0x04004F24 RID: 20260
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x04004F25 RID: 20261
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04004F27 RID: 20263
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController[] materialControllers;

	// Token: 0x04004F28 RID: 20264
	[PublicizedFrom(EAccessModifier.Protected)]
	public int[] materialIndices;

	// Token: 0x04004F29 RID: 20265
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bAwakeCalled;

	// Token: 0x04004F2A RID: 20266
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTextureData selectedMaterial;

	// Token: 0x04004F2B RID: 20267
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockTextureData> currentList;
}
