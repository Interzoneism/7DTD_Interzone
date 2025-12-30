using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001669 RID: 5737
	[Preserve]
	public class ActionFailQuest : ActionBaseClientAction
	{
		// Token: 0x0600AF5D RID: 44893 RVA: 0x00447E04 File Offset: 0x00446004
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				if (this.QuestID == "")
				{
					Quest quest = entityPlayer.QuestJournal.ActiveQuest;
					if (quest == null)
					{
						quest = entityPlayer.QuestJournal.FindActiveQuest();
					}
					if (quest != null)
					{
						quest.CloseQuest(Quest.QuestState.Failed, null);
						entityPlayer.QuestJournal.ActiveQuest = null;
						if (this.RemoveQuest)
						{
							entityPlayer.QuestJournal.ForceRemoveQuest(quest);
							return;
						}
					}
				}
				else
				{
					Quest quest2 = entityPlayer.QuestJournal.FindActiveQuest(this.QuestID, -1);
					if (quest2 != null)
					{
						quest2.CloseQuest(Quest.QuestState.Failed, null);
						if (this.RemoveQuest)
						{
							entityPlayer.QuestJournal.ForceRemoveQuest(quest2);
						}
					}
				}
			}
		}

		// Token: 0x0600AF5E RID: 44894 RVA: 0x00447EAC File Offset: 0x004460AC
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionFailQuest.PropQuestID, ref this.QuestID);
			properties.ParseBool(ActionFailQuest.PropRemoveQuest, ref this.RemoveQuest);
			if (this.QuestID != "")
			{
				this.QuestID = this.QuestID.ToLower();
			}
		}

		// Token: 0x0600AF5F RID: 44895 RVA: 0x00447F05 File Offset: 0x00446105
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionFailQuest
			{
				QuestID = this.QuestID,
				RemoveQuest = this.RemoveQuest
			};
		}

		// Token: 0x04008895 RID: 34965
		public string QuestID = "";

		// Token: 0x04008896 RID: 34966
		public bool RemoveQuest;

		// Token: 0x04008897 RID: 34967
		public static string PropQuestID = "quest";

		// Token: 0x04008898 RID: 34968
		public static string PropRemoveQuest = "remove_quest";
	}
}
