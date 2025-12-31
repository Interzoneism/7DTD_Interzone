using System;
using System.Collections.Generic;

// Token: 0x020007F1 RID: 2033
public class TraderManager
{
	// Token: 0x06003A73 RID: 14963 RVA: 0x00177FC8 File Offset: 0x001761C8
	public TraderManager(WorldBase _world)
	{
		this.world = _world;
		this.Random = GameRandomManager.Instance.CreateGameRandom((int)DateTime.Now.Ticks);
	}

	// Token: 0x06003A74 RID: 14964 RVA: 0x00178000 File Offset: 0x00176200
	public bool TraderInventoryRequested(TraderData trader, int _entityIdThatOpenedIt)
	{
		TraderInfo traderInfo = TraderInfo.traderInfoList[trader.TraderID];
		if (traderInfo == null)
		{
			return false;
		}
		if (traderInfo.ResetInterval < 1)
		{
			return false;
		}
		ulong worldTime = this.world.GetWorldTime();
		if (worldTime < trader.lastInventoryUpdate)
		{
			trader.lastInventoryUpdate = 1UL;
		}
		if ((int)(worldTime - trader.lastInventoryUpdate) < traderInfo.ResetIntervalInTicks && trader.lastInventoryUpdate > 0UL)
		{
			return false;
		}
		ulong num = (ulong)((long)traderInfo.ResetIntervalInTicks);
		trader.lastInventoryUpdate = worldTime / num * num + 1UL;
		this.HandleFullReset(trader, traderInfo);
		trader.TierItemGroups.Clear();
		for (int i = 0; i < traderInfo.TierItemGroups.Count; i++)
		{
			List<ItemStack> list = traderInfo.SpawnTierGroup(this.Random, i);
			trader.TierItemGroups.Add(list.ToArray());
		}
		return true;
	}

	// Token: 0x06003A75 RID: 14965 RVA: 0x001780C8 File Offset: 0x001762C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFullReset(TraderData trader, TraderInfo info)
	{
		trader.PrimaryInventory.Clear();
		if (info.ResetInterval != -1)
		{
			List<ItemStack> list = info.Spawn(this.Random);
			for (int i = 0; i < list.Count; i++)
			{
				ItemStack itemStack = list[i];
				ItemClass forId = ItemClass.GetForId(itemStack.itemValue.type);
				if (!forId.HasQuality)
				{
					for (int j = 0; j < trader.PrimaryInventory.Count; j++)
					{
						ItemStack itemStack2 = trader.PrimaryInventory[j];
						if (itemStack2.itemValue.type == itemStack.itemValue.type && itemStack2.count < forId.Stacknumber.Value)
						{
							int num = Math.Min(forId.Stacknumber.Value - itemStack2.count, itemStack.count);
							itemStack2.count += num;
							itemStack.count -= num;
							if (itemStack.count == 0)
							{
								list[i] = ItemStack.Empty.Clone();
							}
						}
					}
				}
				if (!list[i].IsEmpty())
				{
					trader.PrimaryInventory.Add(list[i]);
				}
			}
		}
	}

	// Token: 0x04002F5E RID: 12126
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldBase world;

	// Token: 0x04002F5F RID: 12127
	public GameRandom Random;

	// Token: 0x04002F60 RID: 12128
	public static float[] QuestTierMod;

	// Token: 0x04002F61 RID: 12129
	public static Dictionary<string, TraderStageTemplateGroup> TraderStageTemplates = new Dictionary<string, TraderStageTemplateGroup>();
}
