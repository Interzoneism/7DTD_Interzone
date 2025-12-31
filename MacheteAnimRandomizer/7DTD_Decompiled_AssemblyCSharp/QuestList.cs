using System;
using System.Collections.Generic;

// Token: 0x02000913 RID: 2323
public class QuestList
{
	// Token: 0x17000738 RID: 1848
	// (get) Token: 0x0600453C RID: 17724 RVA: 0x001BBCC7 File Offset: 0x001B9EC7
	// (set) Token: 0x0600453D RID: 17725 RVA: 0x001BBCCF File Offset: 0x001B9ECF
	public string ID { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600453E RID: 17726 RVA: 0x001BBCD8 File Offset: 0x001B9ED8
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestList(string id)
	{
		this.ID = id;
	}

	// Token: 0x0600453F RID: 17727 RVA: 0x001BBCF4 File Offset: 0x001B9EF4
	public static QuestList NewList(string id)
	{
		if (QuestList.s_QuestLists.ContainsKey(id))
		{
			return null;
		}
		QuestList questList = new QuestList(id.ToLower());
		QuestList.s_QuestLists[id] = questList;
		return questList;
	}

	// Token: 0x06004540 RID: 17728 RVA: 0x001BBD29 File Offset: 0x001B9F29
	[PublicizedFrom(EAccessModifier.Internal)]
	public static QuestList GetQuest(string questListID)
	{
		if (!QuestList.s_QuestLists.ContainsKey(questListID))
		{
			return null;
		}
		return QuestList.s_QuestLists[questListID];
	}

	// Token: 0x04003634 RID: 13876
	public static Dictionary<string, QuestList> s_QuestLists = new CaseInsensitiveStringDictionary<QuestList>();

	// Token: 0x04003636 RID: 13878
	public List<QuestEntry> Quests = new List<QuestEntry>();
}
