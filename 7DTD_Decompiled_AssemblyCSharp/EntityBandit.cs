using System;
using UnityEngine.Scripting;

// Token: 0x02000425 RID: 1061
[Preserve]
public class EntityBandit : EntityHuman
{
	// Token: 0x060020C3 RID: 8387 RVA: 0x000CC4CC File Offset: 0x000CA6CC
	public override void PostInit()
	{
		ItemValue bareHandItemValue = this.inventory.GetBareHandItemValue();
		bareHandItemValue.Quality = (ushort)this.rand.RandomRange(1, 3);
		bareHandItemValue.UseTimes = (float)bareHandItemValue.MaxUseTimes * 0.7f - 1f;
		this.inventory.SetItem(0, bareHandItemValue, 1, true);
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x000CC524 File Offset: 0x000CA724
	public override bool UseHoldingItem(int _actionIndex, bool _isReleased)
	{
		if (!_isReleased)
		{
			ItemActionAttackData itemActionAttackData = this.inventory.holdingItemData.actionData[0] as ItemActionAttackData;
			if (itemActionAttackData != null)
			{
				ItemValue itemValue = itemActionAttackData.invData.itemValue;
				itemValue.UseTimes = (float)itemValue.MaxUseTimes * 0.8f - 1f;
				if (itemActionAttackData is ItemActionRanged.ItemActionDataRanged)
				{
					itemValue.Meta = 2;
				}
			}
		}
		return base.UseHoldingItem(_actionIndex, _isReleased);
	}
}
