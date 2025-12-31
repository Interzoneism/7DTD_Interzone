using System;
using UnityEngine.Scripting;

// Token: 0x0200012A RID: 298
[Preserve]
public class BlockBatteryBank : BlockPowerSource
{
	// Token: 0x0600084E RID: 2126 RVA: 0x0003A10D File Offset: 0x0003830D
	public override TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		if (this.slotItem == null)
		{
			this.slotItem = ItemClass.GetItemClass(this.SlotItemName, false);
		}
		return new TileEntityPowerSource(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.BatteryBank,
			SlotItem = this.slotItem
		};
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0003A142 File Offset: 0x00038342
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetPowerSourceIcon()
	{
		return "battery";
	}
}
