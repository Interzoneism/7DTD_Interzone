using System;
using UnityEngine.Scripting;

// Token: 0x02000128 RID: 296
[Preserve]
public class BlockGenerator : BlockPowerSource
{
	// Token: 0x06000844 RID: 2116 RVA: 0x00039FE0 File Offset: 0x000381E0
	public override TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		if (this.slotItem == null)
		{
			this.slotItem = ItemClass.GetItemClass(this.SlotItemName, false);
		}
		return new TileEntityPowerSource(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.Generator,
			SlotItem = this.slotItem
		};
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0003A015 File Offset: 0x00038215
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetPowerSourceIcon()
	{
		return "electric_generator";
	}

	// Token: 0x0400086C RID: 2156
	public static FastTags<TagGroup.Global> tag = FastTags<TagGroup.Global>.Parse("gasoline");
}
