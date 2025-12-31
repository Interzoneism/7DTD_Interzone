using System;
using UnityEngine.Scripting;

// Token: 0x02000C89 RID: 3209
[Preserve]
public class XUiC_DewCollectorModGrid : XUiC_ItemStackGrid
{
	// Token: 0x17000A18 RID: 2584
	// (get) Token: 0x060062FA RID: 25338 RVA: 0x00076E19 File Offset: 0x00075019
	public override XUiC_ItemStack.StackLocationTypes StackLocation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return XUiC_ItemStack.StackLocationTypes.Workstation;
		}
	}

	// Token: 0x060062FB RID: 25339 RVA: 0x00283480 File Offset: 0x00281680
	public override void Init()
	{
		base.Init();
		string[] array = this.requiredMods.Split(',', StringSplitOptions.None);
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = this.itemControllers[i] as XUiC_RequiredItemStack;
			if (xuiC_RequiredItemStack != null)
			{
				if (i < array.Length)
				{
					xuiC_RequiredItemStack.RequiredItemClass = ItemClass.GetItemClass(array[i], false);
					xuiC_RequiredItemStack.RequiredItemOnly = this.requiredModsOnly;
				}
				else
				{
					xuiC_RequiredItemStack.RequiredItemClass = null;
					xuiC_RequiredItemStack.RequiredItemOnly = false;
				}
				xuiC_RequiredItemStack.StackLocation = this.StackLocation;
			}
		}
	}

	// Token: 0x060062FC RID: 25340 RVA: 0x00283503 File Offset: 0x00281703
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.UpdateBackend(stackList);
		this.tileEntity.ModSlots = stackList;
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x060062FD RID: 25341 RVA: 0x00283529 File Offset: 0x00281729
	public void SetTileEntity(TileEntityDewCollector te)
	{
		this.tileEntity = te;
		this.SetStacks(te.ModSlots);
	}

	// Token: 0x060062FE RID: 25342 RVA: 0x00283540 File Offset: 0x00281740
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "required_mods"))
			{
				if (!(name == "required_mods_only"))
				{
					return false;
				}
				this.requiredModsOnly = StringParsers.ParseBool(value, 0, -1, true);
			}
			else
			{
				this.requiredMods = value;
			}
			return true;
		}
		return flag;
	}

	// Token: 0x060062FF RID: 25343 RVA: 0x00283598 File Offset: 0x00281798
	public bool TryAddMod(ItemClass newItemClass, ItemStack newItemStack)
	{
		if (!this.requiredModsOnly)
		{
			return false;
		}
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_RequiredItemStack xuiC_RequiredItemStack = (XUiC_RequiredItemStack)this.itemControllers[i];
			if (xuiC_RequiredItemStack.RequiredItemClass == newItemClass && xuiC_RequiredItemStack.ItemStack.IsEmpty())
			{
				xuiC_RequiredItemStack.ItemStack = newItemStack.Clone();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06006300 RID: 25344 RVA: 0x002835F5 File Offset: 0x002817F5
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.currentDewCollectorModGrid = this;
	}

	// Token: 0x06006301 RID: 25345 RVA: 0x00283609 File Offset: 0x00281809
	public override void OnClose()
	{
		base.OnClose();
		base.xui.currentDewCollectorModGrid = null;
	}

	// Token: 0x04004A95 RID: 19093
	[PublicizedFrom(EAccessModifier.Private)]
	public string requiredMods = "";

	// Token: 0x04004A96 RID: 19094
	[PublicizedFrom(EAccessModifier.Private)]
	public bool requiredModsOnly;

	// Token: 0x04004A97 RID: 19095
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityDewCollector tileEntity;
}
