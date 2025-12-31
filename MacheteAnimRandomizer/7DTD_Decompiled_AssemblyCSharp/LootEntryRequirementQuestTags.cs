using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200058F RID: 1423
[Preserve]
public class LootEntryRequirementQuestTags : BaseLootEntryRequirement
{
	// Token: 0x06002DCC RID: 11724 RVA: 0x00131384 File Offset: 0x0012F584
	public override void Init(XElement e)
	{
		base.Init(e);
		string str = "";
		if (e.ParseAttribute("quest_tags", ref str))
		{
			this.questTags = FastTags<TagGroup.Global>.Parse(str);
		}
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x001313BE File Offset: 0x0012F5BE
	public override bool CheckRequirement(EntityPlayer player)
	{
		return player.QuestJournal.ActiveQuest != null && player.QuestJournal.ActiveQuest.QuestTags.Test_AnySet(this.questTags);
	}

	// Token: 0x04002473 RID: 9331
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> questTags;
}
