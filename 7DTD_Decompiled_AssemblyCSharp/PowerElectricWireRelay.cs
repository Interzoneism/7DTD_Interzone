using System;

// Token: 0x02000851 RID: 2129
public class PowerElectricWireRelay : PowerConsumer
{
	// Token: 0x1700065D RID: 1629
	// (get) Token: 0x06003D35 RID: 15669 RVA: 0x0011934C File Offset: 0x0011754C
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.ElectricWireRelay;
		}
	}
}
