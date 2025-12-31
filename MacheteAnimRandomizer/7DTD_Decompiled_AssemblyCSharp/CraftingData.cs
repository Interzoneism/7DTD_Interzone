using System;
using System.IO;

// Token: 0x0200028F RID: 655
public class CraftingData
{
	// Token: 0x0600129B RID: 4763 RVA: 0x00073CEE File Offset: 0x00071EEE
	public CraftingData()
	{
		this.items = new ItemStack[0];
		this.outputItems = new ItemStack[0];
		this.breakDownType = CraftingData.BreakdownType.None;
		this.RecipeQueueItems = new RecipeQueueItem[0];
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00073D24 File Offset: 0x00071F24
	public void Write(BinaryWriter _bw)
	{
		int num = this.RecipeQueueItems.Length;
		_bw.Write((byte)num);
		for (int i = 0; i < num; i++)
		{
			if (this.RecipeQueueItems[i] == null)
			{
				this.RecipeQueueItems[i] = new RecipeQueueItem();
			}
			this.RecipeQueueItems[i].Write(_bw, 0U);
		}
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x00073D74 File Offset: 0x00071F74
	public void Read(BinaryReader _br, uint _version = 100U)
	{
		int num = (int)_br.ReadByte();
		this.RecipeQueueItems = new RecipeQueueItem[num];
		for (int i = 0; i < num; i++)
		{
			this.RecipeQueueItems[i] = new RecipeQueueItem();
			this.RecipeQueueItems[i].Read(_br, _version);
		}
	}

	// Token: 0x04000C19 RID: 3097
	[PublicizedFrom(EAccessModifier.Private)]
	public const int Version = 100;

	// Token: 0x04000C1A RID: 3098
	public ItemStack[] items;

	// Token: 0x04000C1B RID: 3099
	public ItemStack[] outputItems;

	// Token: 0x04000C1C RID: 3100
	public bool isCrafting;

	// Token: 0x04000C1D RID: 3101
	public ulong lastWorldTick;

	// Token: 0x04000C1E RID: 3102
	public int totalLeftToCraft;

	// Token: 0x04000C1F RID: 3103
	public float currentRecipeTimer;

	// Token: 0x04000C20 RID: 3104
	public Recipe currentRecipeToCraft;

	// Token: 0x04000C21 RID: 3105
	public Recipe lastRecipeToCraft;

	// Token: 0x04000C22 RID: 3106
	public ItemValue repairedItem;

	// Token: 0x04000C23 RID: 3107
	public CraftingData.BreakdownType breakDownType;

	// Token: 0x04000C24 RID: 3108
	public bool isItemPlacedByUser;

	// Token: 0x04000C25 RID: 3109
	public ulong savedWorldTick;

	// Token: 0x04000C26 RID: 3110
	public RecipeQueueItem[] RecipeQueueItems;

	// Token: 0x02000290 RID: 656
	public enum BreakdownType
	{
		// Token: 0x04000C28 RID: 3112
		None,
		// Token: 0x04000C29 RID: 3113
		Part,
		// Token: 0x04000C2A RID: 3114
		Recipe
	}
}
