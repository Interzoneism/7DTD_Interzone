using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000292 RID: 658
public class RecipeQueueItem
{
	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x060012A2 RID: 4770 RVA: 0x00073F04 File Offset: 0x00072104
	// (set) Token: 0x060012A3 RID: 4771 RVA: 0x00073F34 File Offset: 0x00072134
	public Recipe Recipe
	{
		get
		{
			if (this.cachedRecipe == null)
			{
				return this.cachedRecipe = CraftingManager.GetRecipe(this.recipeHashCode);
			}
			return this.cachedRecipe;
		}
		set
		{
			this.cachedRecipe = value;
			this.recipeHashCode = ((this.cachedRecipe != null) ? this.cachedRecipe.GetHashCode() : 0);
		}
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x00073F5C File Offset: 0x0007215C
	public void Write(BinaryWriter _bw, uint version)
	{
		bool flag = this.Recipe != null;
		_bw.Write(flag ? this.Recipe.GetHashCode() : 0);
		_bw.Write(this.Multiplier);
		_bw.Write(this.IsCrafting);
		_bw.Write(this.CraftingTimeLeft);
		bool flag2 = this.RepairItem != null;
		_bw.Write(flag2);
		if (flag2)
		{
			this.RepairItem.Write(_bw);
			_bw.Write(this.AmountToRepair);
		}
		_bw.Write(this.Quality);
		_bw.Write(this.StartingEntityId);
		_bw.Write(this.OneItemCraftTime);
		_bw.Write(this.Recipe != null && this.Recipe.scrapable);
		if (this.Recipe != null && this.Recipe.scrapable)
		{
			_bw.Write(this.Recipe.itemValueType);
			_bw.Write(this.Recipe.count);
			_bw.Write(this.Recipe.ingredients.Count);
			for (int i = 0; i < this.Recipe.ingredients.Count; i++)
			{
				this.Recipe.ingredients[i].Write(_bw);
			}
			_bw.Write(this.Recipe.craftingTime);
			_bw.Write(this.Recipe.craftExpGain);
			_bw.Write(this.Recipe.IsScrap);
		}
		if (flag)
		{
			ItemClass outputItemClass = this.Recipe.GetOutputItemClass();
			NameIdMapping nameIdMapping = outputItemClass.IsBlock() ? Block.nameIdMapping : ItemClass.nameIdMapping;
			if (nameIdMapping == null)
			{
				return;
			}
			nameIdMapping.AddMapping(outputItemClass.Id, outputItemClass.Name, false);
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x0007410C File Offset: 0x0007230C
	public void Read(BinaryReader _br, uint version)
	{
		this.recipeHashCode = _br.ReadInt32();
		this.cachedRecipe = CraftingManager.GetRecipe(this.recipeHashCode);
		this.Multiplier = _br.ReadInt16();
		this.IsCrafting = _br.ReadBoolean();
		this.CraftingTimeLeft = _br.ReadSingle();
		if (_br.ReadBoolean())
		{
			if (version > 39U)
			{
				this.RepairItem = ItemValue.ReadOrNull(_br);
			}
			else
			{
				this.RepairItem = new ItemValue(_br.ReadInt32(), false);
			}
			this.AmountToRepair = _br.ReadUInt16();
		}
		if (version > 0U)
		{
			this.Quality = _br.ReadByte();
			this.StartingEntityId = _br.ReadInt32();
		}
		if (version > 41U)
		{
			this.OneItemCraftTime = _br.ReadSingle();
		}
		if (version > 43U && _br.ReadBoolean())
		{
			this.cachedRecipe = new Recipe();
			this.cachedRecipe.itemValueType = _br.ReadInt32();
			this.cachedRecipe.count = _br.ReadInt32();
			this.cachedRecipe.scrapable = true;
			int num = _br.ReadInt32();
			this.Recipe.ingredients = new List<ItemStack>();
			for (int i = 0; i < num; i++)
			{
				this.Recipe.ingredients.Add(new ItemStack().Read(_br));
			}
			this.cachedRecipe.craftingTime = _br.ReadSingle();
			this.cachedRecipe.craftExpGain = _br.ReadInt32();
			if (version > 46U)
			{
				this.cachedRecipe.IsScrap = _br.ReadBoolean();
			}
		}
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x00074284 File Offset: 0x00072484
	[PublicizedFrom(EAccessModifier.Internal)]
	public void WriteDelta(BinaryWriter _bw, RecipeQueueItem _last)
	{
		_bw.Write((this.Recipe != null) ? this.Recipe.GetHashCode() : 0);
		if (this.Multiplier < 0)
		{
			Log.Error("Multiplier is less than 0!");
			Log.Out(Environment.StackTrace);
			this.Multiplier = 0;
		}
		_bw.Write(this.CraftingTimeLeft - _last.CraftingTimeLeft);
		_last.CraftingTimeLeft += this.CraftingTimeLeft - _last.CraftingTimeLeft;
		_bw.Write(this.Multiplier - _last.Multiplier);
		_last.Multiplier += this.Multiplier - _last.Multiplier;
		_bw.Write(this.IsCrafting);
		bool flag = this.RepairItem != null;
		_bw.Write(flag);
		if (flag)
		{
			this.RepairItem.Write(_bw);
			_bw.Write(this.AmountToRepair);
		}
		_bw.Write(this.Quality);
		_bw.Write(this.StartingEntityId);
		_bw.Write(this.OneItemCraftTime);
		_bw.Write(this.Recipe != null && this.Recipe.scrapable);
		if (this.Recipe != null && this.Recipe.scrapable)
		{
			_bw.Write(this.Recipe.itemValueType);
			_bw.Write(this.Recipe.count);
			_bw.Write(this.Recipe.scrapable);
			_bw.Write(this.Recipe.ingredients.Count);
			for (int i = 0; i < this.Recipe.ingredients.Count; i++)
			{
				this.Recipe.ingredients[i].Write(_bw);
			}
			_bw.Write(this.Recipe.craftingTime);
			_bw.Write(this.Recipe.craftExpGain);
			_bw.Write(this.Recipe.IsScrap);
		}
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x00074470 File Offset: 0x00072670
	[PublicizedFrom(EAccessModifier.Internal)]
	public void ReadDelta(BinaryReader _br, RecipeQueueItem _last)
	{
		this.recipeHashCode = _br.ReadInt32();
		this.cachedRecipe = CraftingManager.GetRecipe(this.recipeHashCode);
		float num = _br.ReadSingle();
		this.CraftingTimeLeft = _last.CraftingTimeLeft + num;
		int num2 = (int)_br.ReadInt16();
		this.Multiplier = (short)((int)_last.Multiplier + num2);
		this.IsCrafting = _br.ReadBoolean();
		if (_br.ReadBoolean())
		{
			this.RepairItem = ItemValue.ReadOrNull(_br);
			this.AmountToRepair = _br.ReadUInt16();
		}
		this.Quality = _br.ReadByte();
		this.StartingEntityId = _br.ReadInt32();
		this.OneItemCraftTime = _br.ReadSingle();
		if (_br.ReadBoolean())
		{
			this.cachedRecipe = new Recipe();
			this.cachedRecipe.itemValueType = _br.ReadInt32();
			this.cachedRecipe.count = _br.ReadInt32();
			this.cachedRecipe.scrapable = true;
			int num3 = _br.ReadInt32();
			this.Recipe.ingredients = new List<ItemStack>();
			for (int i = 0; i < num3; i++)
			{
				this.Recipe.ingredients.Add(new ItemStack().Read(_br));
			}
			this.cachedRecipe.craftingTime = _br.ReadSingle();
			this.cachedRecipe.craftExpGain = _br.ReadInt32();
			this.cachedRecipe.IsScrap = _br.ReadBoolean();
		}
	}

	// Token: 0x04000C31 RID: 3121
	public short Multiplier;

	// Token: 0x04000C32 RID: 3122
	public float CraftingTimeLeft;

	// Token: 0x04000C33 RID: 3123
	public float OneItemCraftTime = -1f;

	// Token: 0x04000C34 RID: 3124
	public bool IsCrafting;

	// Token: 0x04000C35 RID: 3125
	public ItemValue RepairItem;

	// Token: 0x04000C36 RID: 3126
	public ushort AmountToRepair;

	// Token: 0x04000C37 RID: 3127
	public byte Quality;

	// Token: 0x04000C38 RID: 3128
	public int StartingEntityId = -1;

	// Token: 0x04000C39 RID: 3129
	[PublicizedFrom(EAccessModifier.Private)]
	public int recipeHashCode;

	// Token: 0x04000C3A RID: 3130
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe cachedRecipe;
}
