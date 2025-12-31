using System;
using System.Collections.Generic;

// Token: 0x0200090C RID: 2316
public class NPCQuestData
{
	// Token: 0x04003617 RID: 13847
	public Dictionary<int, NPCQuestData.PlayerQuestData> PlayerQuestList = new Dictionary<int, NPCQuestData.PlayerQuestData>();

	// Token: 0x0200090D RID: 2317
	public class PlayerQuestData
	{
		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x060044DF RID: 17631 RVA: 0x001B9456 File Offset: 0x001B7656
		// (set) Token: 0x060044E0 RID: 17632 RVA: 0x001B945E File Offset: 0x001B765E
		public List<Quest> QuestList
		{
			get
			{
				return this.questList;
			}
			set
			{
				this.questList = value;
				this.LastUpdate = GameManager.Instance.World.GetWorldTime() / 24000UL * 24000UL;
			}
		}

		// Token: 0x060044E1 RID: 17633 RVA: 0x001B948A File Offset: 0x001B768A
		public PlayerQuestData(List<Quest> questList)
		{
			this.QuestList = questList;
		}

		// Token: 0x04003618 RID: 13848
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Quest> questList;

		// Token: 0x04003619 RID: 13849
		public ulong LastUpdate;
	}
}
