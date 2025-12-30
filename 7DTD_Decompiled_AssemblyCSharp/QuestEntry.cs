using System;
using UnityEngine.Scripting;

// Token: 0x02000453 RID: 1107
[Preserve]
public class QuestEntry
{
	// Token: 0x060022CD RID: 8909 RVA: 0x000DB670 File Offset: 0x000D9870
	public QuestEntry(string questID, float prob, int startStage, int endStage)
	{
		this.QuestID = questID;
		this.Prob = prob;
		this.StartStage = startStage;
		this.EndStage = endStage;
		this.QuestClass = QuestClass.GetQuest(this.QuestID);
	}

	// Token: 0x040019EA RID: 6634
	public float Prob = 1f;

	// Token: 0x040019EB RID: 6635
	public int StartStage = -1;

	// Token: 0x040019EC RID: 6636
	public int EndStage = -1;

	// Token: 0x040019ED RID: 6637
	public string QuestID;

	// Token: 0x040019EE RID: 6638
	public QuestClass QuestClass;
}
