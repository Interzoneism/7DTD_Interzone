using System;
using UnityEngine.Scripting;

// Token: 0x02000922 RID: 2338
[Preserve]
public class RewardTreasureItem : BaseReward
{
	// Token: 0x060045B2 RID: 17842 RVA: 0x001BD783 File Offset: 0x001BB983
	public override void SetupReward()
	{
		base.Description = ItemClass.GetItemClass(base.ID, false).Name;
		base.ValueText = base.Value;
		base.Icon = "ui_game_symbol_hand";
		base.IconAtlas = "ItemIconAtlas";
	}

	// Token: 0x060045B3 RID: 17843 RVA: 0x001BD7C0 File Offset: 0x001BB9C0
	public override void GiveReward(EntityPlayer player)
	{
		if (base.OwnerQuest == null)
		{
			return;
		}
		ItemValue item = ItemClass.GetItem(base.ID, false);
		ItemStack.Empty.Clone();
		ItemValue itemValue = new ItemValue(item.type, true);
		int num = 1;
		if (base.Value != null && base.Value != "")
		{
			if (int.TryParse(base.Value, out num))
			{
				if (itemValue.HasQuality)
				{
					itemValue = new ItemValue(item.type, num, num, true, null, 1f);
					num = 1;
				}
				else
				{
					itemValue = new ItemValue(item.type, true);
				}
			}
			else if (base.Value.Contains("-"))
			{
				string[] array = base.Value.Split('-', StringSplitOptions.None);
				int num2 = Convert.ToInt32(array[0]);
				int num3 = Convert.ToInt32(array[1]);
				if (itemValue.HasQuality)
				{
					itemValue = new ItemValue(item.type, num2, num3, true, null, 1f);
					num = 1;
				}
				else
				{
					WorldBase world = GameManager.Instance.World;
					itemValue = new ItemValue(item.type, true);
					num = world.GetGameRandom().RandomRange(num2, num3);
				}
			}
		}
		string[] array2 = base.OwnerQuest.DataVariables["treasurecontainer"].Split(',', StringSplitOptions.None);
		Vector3i zero = Vector3i.zero;
		if (array2.Length == 3)
		{
			zero = new Vector3i(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]));
			((TileEntityLootContainer)GameManager.Instance.World.GetTileEntity(0, zero)).AddItem(new ItemStack(itemValue, num));
		}
	}

	// Token: 0x060045B4 RID: 17844 RVA: 0x001BD94C File Offset: 0x001BBB4C
	public override BaseReward Clone()
	{
		RewardTreasureItem rewardTreasureItem = new RewardTreasureItem();
		base.CopyValues(rewardTreasureItem);
		return rewardTreasureItem;
	}
}
