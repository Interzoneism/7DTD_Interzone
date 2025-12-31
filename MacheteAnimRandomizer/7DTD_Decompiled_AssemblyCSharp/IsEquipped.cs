using System;
using UnityEngine.Scripting;

// Token: 0x020005DC RID: 1500
[Preserve]
public class IsEquipped : TargetedCompareRequirementBase
{
	// Token: 0x06002F91 RID: 12177 RVA: 0x00145D30 File Offset: 0x00143F30
	public override bool IsValid(MinEventParams _params)
	{
		bool result = false;
		if (base.IsValid(_params))
		{
			ItemValue itemValue = _params.ItemValue;
			if (itemValue != null && this.target != null)
			{
				if (itemValue.IsMod)
				{
					foreach (ItemValue itemValue2 in this.target.equipment.GetItems())
					{
						if (itemValue2 != null && itemValue2.HasModSlots)
						{
							foreach (ItemValue itemValue3 in itemValue2.Modifications)
							{
								if (itemValue3 != null && itemValue3 == _params.ItemValue)
								{
									result = true;
									break;
								}
							}
						}
					}
				}
				else
				{
					ItemValue[] items = this.target.equipment.GetItems();
					for (int i = 0; i < items.Length; i++)
					{
						if (items[i] == itemValue)
						{
							result = true;
							break;
						}
					}
				}
			}
		}
		return result;
	}
}
