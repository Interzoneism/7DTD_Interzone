using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200003C RID: 60
[Serializable]
public class InvBaseItem
{
	// Token: 0x040001D9 RID: 473
	public int id16;

	// Token: 0x040001DA RID: 474
	public string name;

	// Token: 0x040001DB RID: 475
	public string description;

	// Token: 0x040001DC RID: 476
	public InvBaseItem.Slot slot;

	// Token: 0x040001DD RID: 477
	public int minItemLevel = 1;

	// Token: 0x040001DE RID: 478
	public int maxItemLevel = 50;

	// Token: 0x040001DF RID: 479
	public List<InvStat> stats = new List<InvStat>();

	// Token: 0x040001E0 RID: 480
	public GameObject attachment;

	// Token: 0x040001E1 RID: 481
	public Color color = Color.white;

	// Token: 0x040001E2 RID: 482
	public UnityEngine.Object iconAtlas;

	// Token: 0x040001E3 RID: 483
	public string iconName = "";

	// Token: 0x0200003D RID: 61
	public enum Slot
	{
		// Token: 0x040001E5 RID: 485
		None,
		// Token: 0x040001E6 RID: 486
		Weapon,
		// Token: 0x040001E7 RID: 487
		Shield,
		// Token: 0x040001E8 RID: 488
		Body,
		// Token: 0x040001E9 RID: 489
		Shoulders,
		// Token: 0x040001EA RID: 490
		Bracers,
		// Token: 0x040001EB RID: 491
		Boots,
		// Token: 0x040001EC RID: 492
		Trinket,
		// Token: 0x040001ED RID: 493
		_LastDoNotUse
	}
}
