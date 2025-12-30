using System;

// Token: 0x02000850 RID: 2128
public class PowerTripWireRelay : PowerTrigger
{
	// Token: 0x1700065C RID: 1628
	// (get) Token: 0x06003D33 RID: 15667 RVA: 0x000768A9 File Offset: 0x00074AA9
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.TripWireRelay;
		}
	}
}
