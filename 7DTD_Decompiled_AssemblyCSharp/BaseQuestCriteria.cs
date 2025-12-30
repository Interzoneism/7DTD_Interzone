using System;
using UnityEngine.Scripting;

// Token: 0x0200089F RID: 2207
[Preserve]
public class BaseQuestCriteria
{
	// Token: 0x06004063 RID: 16483 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandleVariables()
	{
	}

	// Token: 0x06004064 RID: 16484 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CheckForQuestGiver(EntityNPC entity)
	{
		return true;
	}

	// Token: 0x06004065 RID: 16485 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CheckForPlayer(EntityPlayer player)
	{
		return true;
	}

	// Token: 0x04003398 RID: 13208
	public string ID;

	// Token: 0x04003399 RID: 13209
	public string Value;

	// Token: 0x0400339A RID: 13210
	public QuestClass OwnerQuestClass;

	// Token: 0x0400339B RID: 13211
	public BaseQuestCriteria.CriteriaTypes CriteriaType;

	// Token: 0x020008A0 RID: 2208
	public enum CriteriaTypes
	{
		// Token: 0x0400339D RID: 13213
		QuestGiver,
		// Token: 0x0400339E RID: 13214
		Player
	}
}
