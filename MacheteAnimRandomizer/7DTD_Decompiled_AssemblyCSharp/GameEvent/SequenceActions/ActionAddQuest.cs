using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001649 RID: 5705
	[Preserve]
	public class ActionAddQuest : ActionBaseClientAction
	{
		// Token: 0x0600AEC8 RID: 44744 RVA: 0x00444200 File Offset: 0x00442400
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				Quest q = QuestClass.CreateQuest(this.QuestID);
				entityPlayer.QuestJournal.AddQuest(q, this.Notify);
			}
		}

		// Token: 0x0600AEC9 RID: 44745 RVA: 0x00444235 File Offset: 0x00442435
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddQuest.PropQuestID, ref this.QuestID);
			properties.ParseBool(ActionAddQuest.PropNotify, ref this.Notify);
		}

		// Token: 0x0600AECA RID: 44746 RVA: 0x00444260 File Offset: 0x00442460
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddQuest
			{
				targetGroup = this.targetGroup,
				QuestID = this.QuestID,
				Notify = this.Notify
			};
		}

		// Token: 0x040087CB RID: 34763
		public string QuestID;

		// Token: 0x040087CC RID: 34764
		public bool Notify = true;

		// Token: 0x040087CD RID: 34765
		public static string PropQuestID = "quest";

		// Token: 0x040087CE RID: 34766
		public static string PropNotify = "notify";
	}
}
