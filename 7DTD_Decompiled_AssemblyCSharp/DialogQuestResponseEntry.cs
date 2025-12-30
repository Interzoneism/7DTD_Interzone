using System;
using UnityEngine.Scripting;

// Token: 0x020002BA RID: 698
[Preserve]
public class DialogQuestResponseEntry : BaseResponseEntry
{
	// Token: 0x06001376 RID: 4982 RVA: 0x00076ED8 File Offset: 0x000750D8
	public DialogQuestResponseEntry(string _questID, string _type, string _returnStatementID, int _listIndex, int _tier)
	{
		base.ID = _questID;
		this.ListIndex = _listIndex;
		base.ResponseType = BaseResponseEntry.ResponseTypes.QuestAdd;
		this.questType = _type;
		this.Tier = _tier;
		this.ReturnStatementID = _returnStatementID;
	}

	// Token: 0x04000CD7 RID: 3287
	public int ListIndex = -1;

	// Token: 0x04000CD8 RID: 3288
	public string ReturnStatementID = "";

	// Token: 0x04000CD9 RID: 3289
	public string questType = "";

	// Token: 0x04000CDA RID: 3290
	public int Tier = -1;
}
