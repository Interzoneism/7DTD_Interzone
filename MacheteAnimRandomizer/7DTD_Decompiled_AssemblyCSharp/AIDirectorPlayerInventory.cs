using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020003C2 RID: 962
public struct AIDirectorPlayerInventory : IEquatable<AIDirectorPlayerInventory>
{
	// Token: 0x06001D55 RID: 7509 RVA: 0x000B7648 File Offset: 0x000B5848
	public static AIDirectorPlayerInventory FromEntity(EntityAlive entity)
	{
		AIDirectorPlayerInventory result;
		result.bag = AIDirectorPlayerInventory.TrackedItemsFromBag(entity.bag);
		result.belt = AIDirectorPlayerInventory.TrackedItemsFromInventory(entity.inventory);
		return result;
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x000B767C File Offset: 0x000B587C
	public bool Equals(AIDirectorPlayerInventory other)
	{
		return this.bag != null == (other.bag != null) && this.belt != null == (other.belt != null) && AIDirectorPlayerInventory.OrderIndependantEquals(this.bag, other.bag) && AIDirectorPlayerInventory.OrderIndependantEquals(this.belt, other.belt);
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x000B76DC File Offset: 0x000B58DC
	public static List<AIDirectorPlayerInventory.ItemId> TrackedItemsFromBag(Bag bag)
	{
		List<AIDirectorPlayerInventory.ItemId> list = null;
		foreach (ItemStack itemStack in bag.GetSlots())
		{
			if (!itemStack.IsEmpty())
			{
				ItemClass itemClass = itemStack.itemValue.ItemClass;
				if (itemClass != null && itemClass.Smell != null)
				{
					if (list == null)
					{
						list = new List<AIDirectorPlayerInventory.ItemId>();
					}
					AIDirectorPlayerInventory.AppendId(list, AIDirectorPlayerInventory.ItemId.FromStack(itemStack));
				}
			}
		}
		return list;
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x000B773C File Offset: 0x000B593C
	public static List<AIDirectorPlayerInventory.ItemId> TrackedItemsFromInventory(Inventory inv)
	{
		List<AIDirectorPlayerInventory.ItemId> list = null;
		for (int i = 0; i < inv.GetItemCount(); i++)
		{
			ItemStack item = inv.GetItem(i);
			if (!item.IsEmpty())
			{
				ItemClass itemClass = item.itemValue.ItemClass;
				if (itemClass != null && itemClass.Smell != null)
				{
					if (list == null)
					{
						list = new List<AIDirectorPlayerInventory.ItemId>();
					}
					AIDirectorPlayerInventory.AppendId(list, AIDirectorPlayerInventory.ItemId.FromStack(item));
				}
			}
		}
		return list;
	}

	// Token: 0x06001D59 RID: 7513 RVA: 0x000B779C File Offset: 0x000B599C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void AppendId(List<AIDirectorPlayerInventory.ItemId> list, AIDirectorPlayerInventory.ItemId id)
	{
		for (int i = 0; i < list.Count; i++)
		{
			AIDirectorPlayerInventory.ItemId itemId = list[i];
			if (itemId.id == id.id)
			{
				itemId.count += id.count;
				list[i] = itemId;
				return;
			}
		}
		list.Add(id);
	}

	// Token: 0x06001D5A RID: 7514 RVA: 0x000B77F4 File Offset: 0x000B59F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool OrderIndependantEquals(List<AIDirectorPlayerInventory.ItemId> a, List<AIDirectorPlayerInventory.ItemId> b)
	{
		if (a == null && b == null)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		if (a.Count != b.Count)
		{
			return false;
		}
		for (int i = 0; i < a.Count; i++)
		{
			AIDirectorPlayerInventory.ItemId item = a[i];
			if (!b.Contains(item))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04001418 RID: 5144
	public List<AIDirectorPlayerInventory.ItemId> bag;

	// Token: 0x04001419 RID: 5145
	public List<AIDirectorPlayerInventory.ItemId> belt;

	// Token: 0x020003C3 RID: 963
	public struct ItemId : IEquatable<AIDirectorPlayerInventory.ItemId>
	{
		// Token: 0x06001D5B RID: 7515 RVA: 0x000B7848 File Offset: 0x000B5A48
		public static AIDirectorPlayerInventory.ItemId FromStack(ItemStack stack)
		{
			AIDirectorPlayerInventory.ItemId result;
			result.id = stack.itemValue.type;
			result.count = stack.count;
			return result;
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x000B7878 File Offset: 0x000B5A78
		public static AIDirectorPlayerInventory.ItemId Read(BinaryReader stream)
		{
			AIDirectorPlayerInventory.ItemId result;
			result.id = (int)stream.ReadInt16();
			result.count = (int)stream.ReadInt16();
			return result;
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x000B78A0 File Offset: 0x000B5AA0
		public void Write(BinaryWriter stream)
		{
			stream.Write((short)this.id);
			stream.Write((short)this.count);
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x000B78BC File Offset: 0x000B5ABC
		public bool Equals(AIDirectorPlayerInventory.ItemId other)
		{
			return this.id == other.id && this.count == other.count;
		}

		// Token: 0x0400141A RID: 5146
		public const int kNetworkSize = 4;

		// Token: 0x0400141B RID: 5147
		public int id;

		// Token: 0x0400141C RID: 5148
		public int count;
	}
}
