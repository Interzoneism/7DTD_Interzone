using System;
using UnityEngine.Scripting;

// Token: 0x0200089E RID: 2206
[Preserve]
public class QuestActionUnlockPOI : BaseQuestAction
{
	// Token: 0x0600405F RID: 16479 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x06004060 RID: 16480 RVA: 0x001A48E7 File Offset: 0x001A2AE7
	public override void PerformAction(Quest ownerQuest)
	{
		ownerQuest.HandleUnlockPOI(null);
	}

	// Token: 0x06004061 RID: 16481 RVA: 0x001A48F0 File Offset: 0x001A2AF0
	public override BaseQuestAction Clone()
	{
		QuestActionUnlockPOI questActionUnlockPOI = new QuestActionUnlockPOI();
		base.CopyValues(questActionUnlockPOI);
		return questActionUnlockPOI;
	}
}
