using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000567 RID: 1383
[Preserve]
public class ItemClassModifier : ItemClass
{
	// Token: 0x06002CCA RID: 11466 RVA: 0x0012AC28 File Offset: 0x00128E28
	public static ItemClassModifier GetItemModWithAnyTags(FastTags<TagGroup.Global> tags, FastTags<TagGroup.Global> installedModTypes, GameRandom random)
	{
		for (int i = 0; i < ItemClass.list.Length; i++)
		{
			ItemClassModifier itemClassModifier = ItemClass.list[i] as ItemClassModifier;
			if (itemClassModifier != null && !itemClassModifier.HasAnyTags(installedModTypes) && itemClassModifier.InstallableTags.Test_AnySet(tags) && !itemClassModifier.DisallowedTags.Test_AnySet(tags))
			{
				ItemClassModifier.modIds.Add(itemClassModifier.Id);
			}
		}
		if (ItemClassModifier.modIds.Count == 0)
		{
			return null;
		}
		ItemClassModifier result = ItemClass.GetForId(ItemClassModifier.modIds[random.RandomRange(ItemClassModifier.modIds.Count)]) as ItemClassModifier;
		ItemClassModifier.modIds.Clear();
		return result;
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x0012ACC8 File Offset: 0x00128EC8
	public static ItemClassModifier GetCosmeticItemMod(FastTags<TagGroup.Global> itemTags, FastTags<TagGroup.Global> installedModTypes, GameRandom random)
	{
		bool isEmpty = installedModTypes.IsEmpty;
		for (int i = 0; i < ItemClass.list.Length; i++)
		{
			ItemClassModifier itemClassModifier = ItemClass.list[i] as ItemClassModifier;
			if (itemClassModifier != null && (isEmpty || !itemClassModifier.HasAnyTags(installedModTypes)) && itemClassModifier.HasAnyTags(ItemClassModifier.CosmeticModTypes) && itemClassModifier.InstallableTags.Test_AnySet(itemTags) && !itemClassModifier.DisallowedTags.Test_AnySet(itemTags) && random.RandomFloat <= itemClassModifier.CosmeticInstallChance)
			{
				ItemClassModifier.modIds.Add(itemClassModifier.Id);
			}
		}
		if (ItemClassModifier.modIds.Count == 0)
		{
			return null;
		}
		ItemClassModifier result = ItemClass.GetForId(ItemClassModifier.modIds[random.RandomRange(ItemClassModifier.modIds.Count)]) as ItemClassModifier;
		ItemClassModifier.modIds.Clear();
		return result;
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x0012AD90 File Offset: 0x00128F90
	public static ItemClassModifier GetDesiredItemModWithAnyTags(FastTags<TagGroup.Global> tags, FastTags<TagGroup.Global> installedModTypes, FastTags<TagGroup.Global> desiredModTypes, GameRandom random)
	{
		bool isEmpty = installedModTypes.IsEmpty;
		bool isEmpty2 = desiredModTypes.IsEmpty;
		for (int i = 0; i < ItemClass.list.Length; i++)
		{
			ItemClassModifier itemClassModifier = ItemClass.list[i] as ItemClassModifier;
			if (itemClassModifier != null && (isEmpty || !itemClassModifier.HasAnyTags(installedModTypes)) && (isEmpty2 || itemClassModifier.HasAnyTags(desiredModTypes)) && itemClassModifier.InstallableTags.Test_AnySet(tags) && !itemClassModifier.DisallowedTags.Test_AnySet(tags))
			{
				ItemClassModifier.modIds.Add(itemClassModifier.Id);
			}
		}
		if (ItemClassModifier.modIds.Count == 0)
		{
			return null;
		}
		ItemClassModifier result = ItemClass.GetForId(ItemClassModifier.modIds[random.RandomRange(ItemClassModifier.modIds.Count)]) as ItemClassModifier;
		ItemClassModifier.modIds.Clear();
		return result;
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x0012AE50 File Offset: 0x00129050
	public bool GetPropertyOverride(string _propertyName, string _itemName, ref string _value)
	{
		if (this.PropertyOverrides.ContainsKey(_itemName) && this.PropertyOverrides[_itemName].Values.ContainsKey(_propertyName))
		{
			_value = this.PropertyOverrides[_itemName].Values[_propertyName];
			return true;
		}
		if (this.PropertyOverrides.ContainsKey("*") && this.PropertyOverrides["*"].Values.ContainsKey(_propertyName))
		{
			_value = this.PropertyOverrides["*"].Values[_propertyName];
			return true;
		}
		return false;
	}

	// Token: 0x0400236A RID: 9066
	public static ItemClassModifier[] modList = new ItemClassModifier[1000];

	// Token: 0x0400236B RID: 9067
	public FastTags<TagGroup.Global> InstallableTags;

	// Token: 0x0400236C RID: 9068
	public FastTags<TagGroup.Global> DisallowedTags;

	// Token: 0x0400236D RID: 9069
	public ItemClassModifier.ModifierTypes Type;

	// Token: 0x0400236E RID: 9070
	public Dictionary<string, DynamicProperties> PropertyOverrides;

	// Token: 0x0400236F RID: 9071
	public float CosmeticInstallChance;

	// Token: 0x04002370 RID: 9072
	public static FastTags<TagGroup.Global> CosmeticModTypes = FastTags<TagGroup.Global>.Parse("dye,nametag,charm");

	// Token: 0x04002371 RID: 9073
	public static FastTags<TagGroup.Global> CosmeticItemTags = FastTags<TagGroup.Global>.Parse("canHaveCosmetic");

	// Token: 0x04002372 RID: 9074
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<int> modIds = new List<int>();

	// Token: 0x02000568 RID: 1384
	public enum ModifierTypes
	{
		// Token: 0x04002374 RID: 9076
		Mod,
		// Token: 0x04002375 RID: 9077
		Attachment
	}
}
