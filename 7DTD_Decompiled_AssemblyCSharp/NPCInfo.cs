using System;
using System.Collections.Generic;
using MusicUtils.Enums;

// Token: 0x020007E9 RID: 2025
public class NPCInfo
{
	// Token: 0x170005E4 RID: 1508
	// (get) Token: 0x06003A39 RID: 14905 RVA: 0x00177178 File Offset: 0x00175378
	public List<QuestEntry> Quests
	{
		get
		{
			QuestList quest = QuestList.GetQuest(this.QuestListName);
			if (quest != null)
			{
				return quest.Quests;
			}
			return null;
		}
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x001771C1 File Offset: 0x001753C1
	public static void InitStatic()
	{
		NPCInfo.npcInfoList = new Dictionary<string, NPCInfo>();
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x001771CD File Offset: 0x001753CD
	public void Init()
	{
		NPCInfo.npcInfoList[this.Id] = this;
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x001771E0 File Offset: 0x001753E0
	public static void Cleanup()
	{
		NPCInfo.npcInfoList = null;
	}

	// Token: 0x04002F0E RID: 12046
	public static Dictionary<string, NPCInfo> npcInfoList;

	// Token: 0x04002F0F RID: 12047
	public string Id;

	// Token: 0x04002F10 RID: 12048
	public string Name;

	// Token: 0x04002F11 RID: 12049
	public string Faction;

	// Token: 0x04002F12 RID: 12050
	public string Portrait;

	// Token: 0x04002F13 RID: 12051
	public string LocalizationID;

	// Token: 0x04002F14 RID: 12052
	public string VoiceSet = "";

	// Token: 0x04002F15 RID: 12053
	public NPCInfo.StanceTypes CurrentStance;

	// Token: 0x04002F16 RID: 12054
	public SectionType DmsSectionType;

	// Token: 0x04002F17 RID: 12055
	public byte QuestFaction;

	// Token: 0x04002F18 RID: 12056
	public string QuestListName = "trader_quests";

	// Token: 0x04002F19 RID: 12057
	public int TraderID = -1;

	// Token: 0x04002F1A RID: 12058
	public string DialogID;

	// Token: 0x020007EA RID: 2026
	public enum StanceTypes
	{
		// Token: 0x04002F1C RID: 12060
		None,
		// Token: 0x04002F1D RID: 12061
		Like,
		// Token: 0x04002F1E RID: 12062
		Neutral,
		// Token: 0x04002F1F RID: 12063
		Dislike
	}
}
