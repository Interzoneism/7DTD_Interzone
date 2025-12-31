using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200065F RID: 1631
[Preserve]
public class MinEventActionSetItemInSlot : MinEventActionTargetedBase
{
	// Token: 0x06003159 RID: 12633 RVA: 0x00150540 File Offset: 0x0014E740
	public override void Execute(MinEventParams _params)
	{
		ItemValue item = ItemClass.GetItem(this.itemName, false);
		ItemClassArmor itemClassArmor = item.ItemClass as ItemClassArmor;
		if (itemClassArmor != null && itemClassArmor.EquipSlot == this.slot)
		{
			for (int i = 0; i < this.targets.Count; i++)
			{
				this.targets[i].equipment.SetSlotItem((int)this.slot, item, true);
			}
		}
	}

	// Token: 0x0600315A RID: 12634 RVA: 0x001505AC File Offset: 0x0014E7AC
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "item_name")
			{
				this.itemName = _attribute.Value;
				return true;
			}
			if (localName == "equip_slot")
			{
				Enum.TryParse<EquipmentSlots>(_attribute.Value, out this.slot);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x040027AA RID: 10154
	[PublicizedFrom(EAccessModifier.Private)]
	public string itemName = "";

	// Token: 0x040027AB RID: 10155
	[PublicizedFrom(EAccessModifier.Private)]
	public EquipmentSlots slot = EquipmentSlots.BiomeBadge;
}
