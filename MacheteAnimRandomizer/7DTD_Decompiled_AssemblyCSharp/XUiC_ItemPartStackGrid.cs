using System;
using UnityEngine.Scripting;

// Token: 0x02000CD7 RID: 3287
[Preserve]
public class XUiC_ItemPartStackGrid : XUiController
{
	// Token: 0x17000A63 RID: 2659
	// (get) Token: 0x060065C5 RID: 26053 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Backpack;
		}
	}

	// Token: 0x17000A64 RID: 2660
	// (get) Token: 0x060065C6 RID: 26054 RVA: 0x0029560B File Offset: 0x0029380B
	// (set) Token: 0x060065C7 RID: 26055 RVA: 0x00295613 File Offset: 0x00293813
	public ItemStack CurrentItem { get; set; }

	// Token: 0x17000A65 RID: 2661
	// (get) Token: 0x060065C8 RID: 26056 RVA: 0x0029561C File Offset: 0x0029381C
	// (set) Token: 0x060065C9 RID: 26057 RVA: 0x00295624 File Offset: 0x00293824
	public XUiC_AssembleWindow AssembleWindow { get; set; }

	// Token: 0x060065CA RID: 26058 RVA: 0x00295630 File Offset: 0x00293830
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_ItemPartStack>(null);
		this.itemControllers = childrenByType;
		this.IsDirty = false;
	}

	// Token: 0x060065CB RID: 26059 RVA: 0x00295659 File Offset: 0x00293859
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		base.Update(_dt);
	}

	// Token: 0x060065CC RID: 26060 RVA: 0x0029567C File Offset: 0x0029387C
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
			XUiC_ItemPartStack xuiC_ItemPartStack = (XUiC_ItemPartStack)this.itemControllers[i];
			if (i < this.CurrentItem.itemValue.Modifications.Length)
			{
				ItemValue itemValue = this.CurrentItem.itemValue.Modifications[i];
				if (itemValue != null && itemValue.ItemClass is ItemClassModifier)
				{
					xuiC_ItemPartStack.SlotType = (itemValue.ItemClass as ItemClassModifier).Type.ToStringCached<ItemClassModifier.ModifierTypes>().ToLower();
				}
				xuiC_ItemPartStack.SlotChangedEvent -= this.HandleSlotChangedEvent;
				xuiC_ItemPartStack.ItemValue = ((itemValue != null) ? itemValue : ItemValue.None.Clone());
				xuiC_ItemPartStack.SlotChangedEvent += this.HandleSlotChangedEvent;
				xuiC_ItemPartStack.SlotNumber = i;
				xuiC_ItemPartStack.InfoWindow = childByType;
				xuiC_ItemPartStack.StackLocation = this.StackLocation;
				xuiC_ItemPartStack.ViewComponent.IsVisible = true;
			}
			else
			{
				xuiC_ItemPartStack.ViewComponent.IsVisible = false;
			}
		}
	}

	// Token: 0x060065CD RID: 26061 RVA: 0x002957A0 File Offset: 0x002939A0
	public void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		XUiC_ItemPartStack xuiC_ItemPartStack = (XUiC_ItemPartStack)this.itemControllers[slotNumber];
		ItemValue itemValue = xuiC_ItemPartStack.ItemStack.IsEmpty() ? ItemValue.None.Clone() : xuiC_ItemPartStack.ItemStack.itemValue;
		if (itemValue.ItemClass != null)
		{
			if (itemValue.ItemClass.ItemTags.Test_AnySet(ItemClassModifier.CosmeticModTypes) && this.CurrentItem.itemValue.CosmeticMods.Length != 0)
			{
				this.CurrentItem.itemValue.CosmeticMods[0] = itemValue;
			}
			else
			{
				this.CurrentItem.itemValue.Modifications[slotNumber] = itemValue;
			}
		}
		else
		{
			this.CurrentItem.itemValue.Modifications[slotNumber] = itemValue;
		}
		this.AssembleWindow.ItemStack = this.CurrentItem;
		this.AssembleWindow.OnChanged();
		base.xui.AssembleItem.RefreshAssembleItem();
	}

	// Token: 0x060065CE RID: 26062 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x060065CF RID: 26063 RVA: 0x0029587B File Offset: 0x00293A7B
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.IsDirty = true;
	}

	// Token: 0x060065D0 RID: 26064 RVA: 0x002958A5 File Offset: 0x00293AA5
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
	}

	// Token: 0x04004CDC RID: 19676
	[PublicizedFrom(EAccessModifier.Protected)]
	public int curPageIdx;

	// Token: 0x04004CDD RID: 19677
	[PublicizedFrom(EAccessModifier.Protected)]
	public int numPages;

	// Token: 0x04004CDE RID: 19678
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController[] itemControllers;

	// Token: 0x04004CDF RID: 19679
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemStack[] items;

	// Token: 0x04004CE2 RID: 19682
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass currentItemClass;
}
