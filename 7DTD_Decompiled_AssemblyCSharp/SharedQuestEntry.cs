using System;
using UnityEngine;

// Token: 0x02000912 RID: 2322
public class SharedQuestEntry
{
	// Token: 0x17000737 RID: 1847
	// (get) Token: 0x06004539 RID: 17721 RVA: 0x001BBB73 File Offset: 0x001B9D73
	public QuestClass QuestClass
	{
		get
		{
			return QuestClass.GetQuest(this.QuestID);
		}
	}

	// Token: 0x0600453A RID: 17722 RVA: 0x001BBB80 File Offset: 0x001B9D80
	public SharedQuestEntry(int questCode, string questID, string poiName, Vector3 position, Vector3 size, Vector3 returnPos, int sharedByPlayerID, int questGiverID, QuestJournal questJournal, Quest quest)
	{
		this.QuestCode = questCode;
		this.QuestID = questID;
		this.POIName = poiName;
		this.Position = position;
		this.Size = size;
		this.ReturnPos = returnPos;
		this.SharedByPlayerID = sharedByPlayerID;
		this.QuestGiverID = questGiverID;
		this.Quest = ((quest == null) ? QuestClass.CreateQuest(questID) : quest.Clone());
		this.Quest.OwnerJournal = questJournal;
		this.Quest.SetupSharedQuest();
		this.Quest.SharedOwnerID = sharedByPlayerID;
		this.Quest.QuestGiverID = questGiverID;
		this.Quest.QuestCode = questCode;
		this.Quest.AddSharedLocation(position, size);
		if (!this.Quest.DataVariables.ContainsKey("POIName"))
		{
			this.Quest.DataVariables.Add("POIName", poiName);
		}
	}

	// Token: 0x0600453B RID: 17723 RVA: 0x001BBC74 File Offset: 0x001B9E74
	public SharedQuestEntry Clone()
	{
		return new SharedQuestEntry(this.QuestCode, this.QuestID, this.POIName, this.Position, this.Size, this.ReturnPos, this.SharedByPlayerID, this.QuestGiverID, this.Quest.OwnerJournal, this.Quest);
	}

	// Token: 0x0400362B RID: 13867
	public int QuestCode;

	// Token: 0x0400362C RID: 13868
	public string QuestID;

	// Token: 0x0400362D RID: 13869
	public string POIName;

	// Token: 0x0400362E RID: 13870
	public Vector3 Position;

	// Token: 0x0400362F RID: 13871
	public Vector3 Size;

	// Token: 0x04003630 RID: 13872
	public Vector3 ReturnPos;

	// Token: 0x04003631 RID: 13873
	public int SharedByPlayerID = -1;

	// Token: 0x04003632 RID: 13874
	public int QuestGiverID = -1;

	// Token: 0x04003633 RID: 13875
	public Quest Quest;
}
