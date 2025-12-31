using System;
using UnityEngine.Scripting;

// Token: 0x0200089C RID: 2204
[Preserve]
public class QuestActionTrackQuest : BaseQuestAction
{
	// Token: 0x06004057 RID: 16471 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x06004058 RID: 16472 RVA: 0x001A4771 File Offset: 0x001A2971
	public override void PerformAction(Quest ownerQuest)
	{
		ownerQuest.Tracked = true;
		ownerQuest.OwnerJournal.RefreshTracked();
	}

	// Token: 0x06004059 RID: 16473 RVA: 0x001A4788 File Offset: 0x001A2988
	public override BaseQuestAction Clone()
	{
		QuestActionTrackQuest questActionTrackQuest = new QuestActionTrackQuest();
		base.CopyValues(questActionTrackQuest);
		return questActionTrackQuest;
	}
}
