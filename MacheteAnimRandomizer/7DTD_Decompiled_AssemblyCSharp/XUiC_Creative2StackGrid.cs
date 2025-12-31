using System;
using UnityEngine.Scripting;

// Token: 0x02000C6E RID: 3182
[Preserve]
public class XUiC_Creative2StackGrid : XUiC_ItemStackGrid
{
	// Token: 0x17000A13 RID: 2579
	// (get) Token: 0x0600620E RID: 25102 RVA: 0x0027CA0C File Offset: 0x0027AC0C
	// (set) Token: 0x0600620F RID: 25103 RVA: 0x0027CA14 File Offset: 0x0027AC14
	public int Length { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000A14 RID: 2580
	// (get) Token: 0x06006210 RID: 25104 RVA: 0x0027CA1D File Offset: 0x0027AC1D
	// (set) Token: 0x06006211 RID: 25105 RVA: 0x0027CA25 File Offset: 0x0027AC25
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			this.page = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x06006212 RID: 25106 RVA: 0x0027CA35 File Offset: 0x0027AC35
	public override void Init()
	{
		base.Init();
		this.Length = this.itemControllers.Length;
		this.IsDirty = false;
	}

	// Token: 0x06006213 RID: 25107 RVA: 0x0027CA52 File Offset: 0x0027AC52
	public override ItemStack[] GetSlots()
	{
		return this.items;
	}

	// Token: 0x06006214 RID: 25108 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
	}

	// Token: 0x06006215 RID: 25109 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetStacks(ItemStack[] stackList)
	{
	}

	// Token: 0x06006216 RID: 25110 RVA: 0x0027CA5C File Offset: 0x0027AC5C
	public void SetSlots(ItemStack[] stackList)
	{
		if (stackList == null)
		{
			return;
		}
		this.items = stackList;
		this.IsDirty = true;
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		for (int i = 0; i < this.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			xuiC_ItemStack.InfoWindow = childByType;
			xuiC_ItemStack.StackLocation = XUiC_ItemStack.StackLocationTypes.Creative;
			this.itemControllers[i].ViewComponent.IsVisible = true;
		}
	}

	// Token: 0x06006217 RID: 25111 RVA: 0x0027CAC0 File Offset: 0x0027ACC0
	public override void Update(float _dt)
	{
		if (!base.ViewComponent.IsVisible)
		{
			return;
		}
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.IsDirty && base.xui.PlayerInventory != null)
		{
			for (int i = 0; i < this.Length; i++)
			{
				int num = i + this.Length * this.page;
				XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
				if (xuiC_ItemStack != null)
				{
					if (num < this.items.Length)
					{
						xuiC_ItemStack.ItemStack = this.items[num];
					}
					else
					{
						xuiC_ItemStack.ItemStack = ItemStack.Empty.Clone();
						if (xuiC_ItemStack.Selected)
						{
							xuiC_ItemStack.Selected = false;
						}
					}
				}
			}
			this.IsDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x040049C6 RID: 18886
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;
}
