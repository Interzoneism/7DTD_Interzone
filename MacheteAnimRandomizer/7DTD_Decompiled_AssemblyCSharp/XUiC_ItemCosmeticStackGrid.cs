using System;
using UnityEngine.Scripting;

// Token: 0x02000CD8 RID: 3288
[Preserve]
public class XUiC_ItemCosmeticStackGrid : XUiController
{
	// Token: 0x17000A66 RID: 2662
	// (get) Token: 0x060065D2 RID: 26066 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Backpack;
		}
	}

	// Token: 0x17000A67 RID: 2663
	// (get) Token: 0x060065D3 RID: 26067 RVA: 0x002958C8 File Offset: 0x00293AC8
	// (set) Token: 0x060065D4 RID: 26068 RVA: 0x002958D0 File Offset: 0x00293AD0
	public ItemStack CurrentItem { get; set; }

	// Token: 0x17000A68 RID: 2664
	// (get) Token: 0x060065D5 RID: 26069 RVA: 0x002958D9 File Offset: 0x00293AD9
	// (set) Token: 0x060065D6 RID: 26070 RVA: 0x002958E1 File Offset: 0x00293AE1
	public XUiC_AssembleWindow AssembleWindow { get; set; }

	// Token: 0x060065D7 RID: 26071 RVA: 0x002958EC File Offset: 0x00293AEC
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_ItemCosmeticStack>(null);
		this.itemControllers = childrenByType;
		this.IsDirty = false;
	}

	// Token: 0x060065D8 RID: 26072 RVA: 0x00295659 File Offset: 0x00293859
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		base.Update(_dt);
	}

	// Token: 0x060065D9 RID: 26073 RVA: 0x00295918 File Offset: 0x00293B18
	public void SetParts(ItemValue[] stackList)
	{
		if (stackList == null)
		{
			return;
		}
		this.currentItemClass = this.CurrentItem.itemValue.ItemClass;
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemCosmeticStack xuiC_ItemCosmeticStack = (XUiC_ItemCosmeticStack)this.itemControllers[i];
			if (i < this.CurrentItem.itemValue.CosmeticMods.Length)
			{
				ItemValue itemValue = this.CurrentItem.itemValue.CosmeticMods[i];
				if (itemValue != null && itemValue.ItemClass is ItemClassModifier)
				{
					xuiC_ItemCosmeticStack.SlotType = (itemValue.ItemClass as ItemClassModifier).Type.ToStringCached<ItemClassModifier.ModifierTypes>().ToLower();
				}
				xuiC_ItemCosmeticStack.SlotChangedEvent -= this.HandleSlotChangedEvent;
				xuiC_ItemCosmeticStack.ItemValue = ((itemValue != null) ? itemValue : ItemValue.None.Clone());
				xuiC_ItemCosmeticStack.SlotChangedEvent += this.HandleSlotChangedEvent;
				xuiC_ItemCosmeticStack.SlotNumber = i;
				xuiC_ItemCosmeticStack.InfoWindow = childByType;
				xuiC_ItemCosmeticStack.StackLocation = this.StackLocation;
				xuiC_ItemCosmeticStack.ViewComponent.IsVisible = true;
			}
			else
			{
				xuiC_ItemCosmeticStack.ViewComponent.IsVisible = false;
			}
		}
	}

	// Token: 0x060065DA RID: 26074 RVA: 0x00295A3C File Offset: 0x00293C3C
	public void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		XUiC_ItemCosmeticStack xuiC_ItemCosmeticStack = (XUiC_ItemCosmeticStack)this.itemControllers[slotNumber];
		ItemValue itemValue = xuiC_ItemCosmeticStack.ItemStack.IsEmpty() ? ItemValue.None.Clone() : xuiC_ItemCosmeticStack.ItemStack.itemValue;
		if (itemValue.ItemClass != null)
		{
			if (!itemValue.ItemClass.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes) && this.CurrentItem.itemValue.Modifications.Length != 0)
			{
				for (int i = 0; i < this.CurrentItem.itemValue.Modifications.Length; i++)
				{
					ItemValue itemValue2 = this.CurrentItem.itemValue.Modifications[i];
					if (itemValue2 == null || itemValue2.IsEmpty())
					{
						this.CurrentItem.itemValue.Modifications[i] = itemValue;
						break;
					}
				}
			}
			else
			{
				this.CurrentItem.itemValue.CosmeticMods[slotNumber] = itemValue;
			}
		}
		else
		{
			this.CurrentItem.itemValue.CosmeticMods[slotNumber] = itemValue;
		}
		this.AssembleWindow.ItemStack = this.CurrentItem;
		this.AssembleWindow.OnChanged();
		base.xui.AssembleItem.RefreshAssembleItem();
	}

	// Token: 0x060065DB RID: 26075 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x060065DC RID: 26076 RVA: 0x0029587B File Offset: 0x00293A7B
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.IsDirty = true;
	}

	// Token: 0x060065DD RID: 26077 RVA: 0x002958A5 File Offset: 0x00293AA5
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
	}

	// Token: 0x04004CE3 RID: 19683
	[PublicizedFrom(EAccessModifier.Protected)]
	public int curPageIdx;

	// Token: 0x04004CE4 RID: 19684
	[PublicizedFrom(EAccessModifier.Protected)]
	public int numPages;

	// Token: 0x04004CE5 RID: 19685
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController[] itemControllers;

	// Token: 0x04004CE6 RID: 19686
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemStack[] items;

	// Token: 0x04004CE9 RID: 19689
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass currentItemClass;
}
