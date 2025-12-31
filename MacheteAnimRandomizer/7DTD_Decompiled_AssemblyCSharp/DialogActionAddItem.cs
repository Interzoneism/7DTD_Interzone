using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200029D RID: 669
[Preserve]
public class DialogActionAddItem : BaseDialogAction
{
	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001304 RID: 4868 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.AddItem;
		}
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x000759C0 File Offset: 0x00073BC0
	public override void PerformAction(EntityPlayer player)
	{
		ItemValue item = ItemClass.GetItem(base.ID, false);
		ItemValue itemValue = new ItemValue(ItemClass.GetItem(base.ID, false).type, true);
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
			else if (base.Value.Contains(","))
			{
				string[] array = base.Value.Split(',', StringSplitOptions.None);
				int num2 = Convert.ToInt32(array[0]);
				int num3 = Convert.ToInt32(array[1]);
				if (itemValue.HasQuality)
				{
					itemValue = new ItemValue(item.type, num2, num3, true, null, 1f);
					num = 1;
				}
				else
				{
					itemValue = new ItemValue(item.type, true);
					num = UnityEngine.Random.Range(num2, num3);
				}
			}
		}
		LocalPlayerUI.primaryUI.xui.PlayerInventory.AddItem(new ItemStack(itemValue, num));
	}

	// Token: 0x04000C8B RID: 3211
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
