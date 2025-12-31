using System;
using System.IO;

// Token: 0x02000291 RID: 657
public class CraftCompleteData
{
	// Token: 0x0600129E RID: 4766 RVA: 0x00073DBC File Offset: 0x00071FBC
	public CraftCompleteData()
	{
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00073DDC File Offset: 0x00071FDC
	public CraftCompleteData(int crafterEntityID, ItemStack craftedItemStack, string recipeName, string itemScrapped, int craftExpGain, ushort recipeUsedCount)
	{
		this.CrafterEntityID = crafterEntityID;
		this.CraftedItemStack = craftedItemStack;
		this.RecipeName = recipeName;
		this.ItemScrapped = itemScrapped;
		this.RecipeUsedCount = recipeUsedCount;
		this.CraftExpGain = craftExpGain;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x00073E34 File Offset: 0x00072034
	public void Write(BinaryWriter _bw, int version)
	{
		_bw.Write(this.CrafterEntityID);
		this.CraftedItemStack.Write(_bw);
		_bw.Write(this.RecipeName);
		_bw.Write(this.CraftExpGain);
		_bw.Write(this.RecipeUsedCount);
		_bw.Write(this.ItemScrapped);
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00073E8C File Offset: 0x0007208C
	public void Read(BinaryReader _br, int version)
	{
		this.CrafterEntityID = _br.ReadInt32();
		this.CraftedItemStack = new ItemStack().Read(_br);
		this.RecipeName = _br.ReadString();
		this.CraftExpGain = _br.ReadInt32();
		if (version >= 48)
		{
			this.RecipeUsedCount = _br.ReadUInt16();
		}
		else
		{
			this.RecipeUsedCount = (ushort)this.CraftedItemStack.count;
		}
		if (version >= 49)
		{
			this.ItemScrapped = _br.ReadString();
		}
	}

	// Token: 0x04000C2B RID: 3115
	public int CrafterEntityID;

	// Token: 0x04000C2C RID: 3116
	public ItemStack CraftedItemStack;

	// Token: 0x04000C2D RID: 3117
	public string RecipeName = "";

	// Token: 0x04000C2E RID: 3118
	public string ItemScrapped = "";

	// Token: 0x04000C2F RID: 3119
	public ushort RecipeUsedCount;

	// Token: 0x04000C30 RID: 3120
	public int CraftExpGain;
}
