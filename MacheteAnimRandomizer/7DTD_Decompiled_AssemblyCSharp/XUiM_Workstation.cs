using System;

// Token: 0x02000F00 RID: 3840
public class XUiM_Workstation : XUiModel
{
	// Token: 0x17000C34 RID: 3124
	// (get) Token: 0x06007911 RID: 30993 RVA: 0x00313E61 File Offset: 0x00312061
	public TileEntityWorkstation TileEntity
	{
		get
		{
			return this.tileEntity;
		}
	}

	// Token: 0x06007912 RID: 30994 RVA: 0x00313E69 File Offset: 0x00312069
	public XUiM_Workstation(TileEntityWorkstation _te)
	{
		this.tileEntity = _te;
	}

	// Token: 0x06007913 RID: 30995 RVA: 0x00313E78 File Offset: 0x00312078
	public bool GetIsBurning()
	{
		return this.tileEntity.IsBurning;
	}

	// Token: 0x06007914 RID: 30996 RVA: 0x00313E85 File Offset: 0x00312085
	public bool GetIsBesideWater()
	{
		return this.tileEntity.IsBesideWater;
	}

	// Token: 0x06007915 RID: 30997 RVA: 0x00313E92 File Offset: 0x00312092
	public void SetIsBurning(bool _isBurning)
	{
		this.tileEntity.IsBurning = _isBurning;
		this.tileEntity.ResetTickTime();
	}

	// Token: 0x06007916 RID: 30998 RVA: 0x00313EAB File Offset: 0x003120AB
	public ItemStack[] GetInputStacks()
	{
		return this.tileEntity.Input;
	}

	// Token: 0x06007917 RID: 30999 RVA: 0x00313EB8 File Offset: 0x003120B8
	public void SetInputStacks(ItemStack[] itemStacks)
	{
		this.tileEntity.Input = itemStacks;
	}

	// Token: 0x06007918 RID: 31000 RVA: 0x00313EC6 File Offset: 0x003120C6
	public void SetInputInSlot(int idx, ItemStack itemStack)
	{
		this.tileEntity.Input[idx] = itemStack.Clone();
	}

	// Token: 0x06007919 RID: 31001 RVA: 0x00313EDB File Offset: 0x003120DB
	public ItemStack[] GetOutputStacks()
	{
		return this.tileEntity.Output;
	}

	// Token: 0x0600791A RID: 31002 RVA: 0x00313EE8 File Offset: 0x003120E8
	public void SetOutputStacks(ItemStack[] itemStacks)
	{
		this.tileEntity.Output = itemStacks;
	}

	// Token: 0x0600791B RID: 31003 RVA: 0x00313EF6 File Offset: 0x003120F6
	public void SetOutputInSlot(int idx, ItemStack itemStack)
	{
		this.tileEntity.Output[idx] = itemStack.Clone();
	}

	// Token: 0x0600791C RID: 31004 RVA: 0x00313F0B File Offset: 0x0031210B
	public ItemStack[] GetToolStacks()
	{
		return this.tileEntity.Tools;
	}

	// Token: 0x0600791D RID: 31005 RVA: 0x00313F18 File Offset: 0x00312118
	public void SetToolStacks(ItemStack[] itemStacks)
	{
		this.tileEntity.Tools = itemStacks;
	}

	// Token: 0x0600791E RID: 31006 RVA: 0x00313F26 File Offset: 0x00312126
	public void SetToolInSlot(int idx, ItemStack itemStack)
	{
		this.tileEntity.Tools[idx] = itemStack.Clone();
	}

	// Token: 0x0600791F RID: 31007 RVA: 0x00313F3B File Offset: 0x0031213B
	public ItemStack[] GetFuelStacks()
	{
		return this.tileEntity.Fuel;
	}

	// Token: 0x06007920 RID: 31008 RVA: 0x00313F48 File Offset: 0x00312148
	public void SetFuelStacks(ItemStack[] itemStacks)
	{
		this.tileEntity.Fuel = itemStacks;
	}

	// Token: 0x06007921 RID: 31009 RVA: 0x00313F56 File Offset: 0x00312156
	public void SetFuelInSlot(int idx, ItemStack itemStack)
	{
		this.tileEntity.Fuel[idx] = itemStack.Clone();
	}

	// Token: 0x06007922 RID: 31010 RVA: 0x00313F6B File Offset: 0x0031216B
	public float GetBurnTimeLeft()
	{
		if (this.tileEntity.BurnTimeLeft == 0f)
		{
			return 0f;
		}
		return this.tileEntity.BurnTimeLeft + 0.5f;
	}

	// Token: 0x06007923 RID: 31011 RVA: 0x00313F96 File Offset: 0x00312196
	public float GetTotalBurnTimeLeft()
	{
		if (this.tileEntity.BurnTotalTimeLeft == 0f)
		{
			return 0f;
		}
		return this.tileEntity.BurnTotalTimeLeft + 0.5f;
	}

	// Token: 0x06007924 RID: 31012 RVA: 0x00313FC1 File Offset: 0x003121C1
	public RecipeQueueItem[] GetRecipeQueueItems()
	{
		return this.tileEntity.Queue;
	}

	// Token: 0x06007925 RID: 31013 RVA: 0x00313FCE File Offset: 0x003121CE
	public void SetRecipeQueueItems(RecipeQueueItem[] queueStacks)
	{
		this.tileEntity.Queue = queueStacks;
	}

	// Token: 0x06007926 RID: 31014 RVA: 0x00313FDC File Offset: 0x003121DC
	public void SetQueueInSlot(int idx, RecipeQueueItem queueStack)
	{
		this.tileEntity.Queue[idx] = queueStack;
	}

	// Token: 0x06007927 RID: 31015 RVA: 0x00313FEC File Offset: 0x003121EC
	public void SetUserAccessing(bool isUserAccessing)
	{
		this.tileEntity.SetUserAccessing(isUserAccessing);
	}

	// Token: 0x06007928 RID: 31016 RVA: 0x00313FFA File Offset: 0x003121FA
	public string[] GetMaterialNames()
	{
		return this.tileEntity.MaterialNames;
	}

	// Token: 0x04005BEB RID: 23531
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityWorkstation tileEntity;
}
