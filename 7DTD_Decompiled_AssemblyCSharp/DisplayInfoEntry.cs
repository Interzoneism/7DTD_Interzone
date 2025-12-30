using System;

// Token: 0x02000ECD RID: 3789
public class DisplayInfoEntry
{
	// Token: 0x17000C26 RID: 3110
	// (get) Token: 0x060077D5 RID: 30677 RVA: 0x0030C5AE File Offset: 0x0030A7AE
	// (set) Token: 0x060077D6 RID: 30678 RVA: 0x0030C5B6 File Offset: 0x0030A7B6
	public FastTags<TagGroup.Global> Tags
	{
		get
		{
			return this.tags;
		}
		set
		{
			this.tags = value;
			this.TagsSet = true;
		}
	}

	// Token: 0x04005B4D RID: 23373
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tags = FastTags<TagGroup.Global>.none;

	// Token: 0x04005B4E RID: 23374
	public bool TagsSet;

	// Token: 0x04005B4F RID: 23375
	public PassiveEffects StatType;

	// Token: 0x04005B50 RID: 23376
	public string CustomName = "";

	// Token: 0x04005B51 RID: 23377
	public string TitleOverride;

	// Token: 0x04005B52 RID: 23378
	public DisplayInfoEntry.DisplayTypes DisplayType;

	// Token: 0x04005B53 RID: 23379
	public bool ShowInverted;

	// Token: 0x04005B54 RID: 23380
	public bool NegativePreferred;

	// Token: 0x04005B55 RID: 23381
	public bool DisplayLeadingPlus;

	// Token: 0x02000ECE RID: 3790
	public enum DisplayTypes
	{
		// Token: 0x04005B57 RID: 23383
		Integer,
		// Token: 0x04005B58 RID: 23384
		Decimal1,
		// Token: 0x04005B59 RID: 23385
		Decimal2,
		// Token: 0x04005B5A RID: 23386
		Bool,
		// Token: 0x04005B5B RID: 23387
		Percent,
		// Token: 0x04005B5C RID: 23388
		Time
	}
}
