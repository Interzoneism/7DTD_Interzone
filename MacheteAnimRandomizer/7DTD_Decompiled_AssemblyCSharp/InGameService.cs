using System;

// Token: 0x02000E21 RID: 3617
public class InGameService
{
	// Token: 0x17000B60 RID: 2912
	// (get) Token: 0x06007142 RID: 28994 RVA: 0x002E27DF File Offset: 0x002E09DF
	// (set) Token: 0x06007143 RID: 28995 RVA: 0x002E27E7 File Offset: 0x002E09E7
	public InGameService.InGameServiceTypes ServiceType { get; set; }

	// Token: 0x17000B61 RID: 2913
	// (get) Token: 0x06007144 RID: 28996 RVA: 0x002E27F0 File Offset: 0x002E09F0
	// (set) Token: 0x06007145 RID: 28997 RVA: 0x002E27F8 File Offset: 0x002E09F8
	public string Name { get; set; }

	// Token: 0x17000B62 RID: 2914
	// (get) Token: 0x06007146 RID: 28998 RVA: 0x002E2801 File Offset: 0x002E0A01
	// (set) Token: 0x06007147 RID: 28999 RVA: 0x002E2809 File Offset: 0x002E0A09
	public string Description { get; set; }

	// Token: 0x17000B63 RID: 2915
	// (get) Token: 0x06007148 RID: 29000 RVA: 0x002E2812 File Offset: 0x002E0A12
	// (set) Token: 0x06007149 RID: 29001 RVA: 0x002E281A File Offset: 0x002E0A1A
	public string Icon { get; set; }

	// Token: 0x17000B64 RID: 2916
	// (get) Token: 0x0600714A RID: 29002 RVA: 0x002E2823 File Offset: 0x002E0A23
	// (set) Token: 0x0600714B RID: 29003 RVA: 0x002E282B File Offset: 0x002E0A2B
	public int Price { get; set; }

	// Token: 0x04005622 RID: 22050
	public Action<bool> VisibleChangedHandler;

	// Token: 0x02000E22 RID: 3618
	public enum InGameServiceTypes
	{
		// Token: 0x04005624 RID: 22052
		VendingRent
	}
}
