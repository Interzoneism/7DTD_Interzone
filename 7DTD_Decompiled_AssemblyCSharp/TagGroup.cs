using System;

// Token: 0x02000674 RID: 1652
public class TagGroup
{
	// Token: 0x02000675 RID: 1653
	public abstract class TagsGroupAbs
	{
		// Token: 0x0600319A RID: 12698 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Protected)]
		public TagsGroupAbs()
		{
		}
	}

	// Token: 0x02000676 RID: 1654
	public class Global : TagGroup.TagsGroupAbs
	{
	}

	// Token: 0x02000677 RID: 1655
	public class Poi : TagGroup.TagsGroupAbs
	{
	}
}
