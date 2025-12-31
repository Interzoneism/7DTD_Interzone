using System;
using UnityEngine.Scripting;

// Token: 0x0200089B RID: 2203
[Preserve]
public class QuestActionTrackChallenge : BaseQuestAction
{
	// Token: 0x06004051 RID: 16465 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x06004052 RID: 16466 RVA: 0x001A46A8 File Offset: 0x001A28A8
	public override void PerformAction(Quest ownerQuest)
	{
		EntityPlayerLocal ownerPlayer = ownerQuest.OwnerJournal.OwnerPlayer;
		XUi xui = ownerPlayer.PlayerUI.xui;
		if (xui.QuestTracker.TrackedQuest == null && xui.QuestTracker.TrackedChallenge == null && xui.Recipes.TrackedRecipe == null && ownerPlayer.challengeJournal.ChallengeDictionary.ContainsKey(this.ID))
		{
			xui.QuestTracker.TrackedChallenge = ownerPlayer.challengeJournal.ChallengeDictionary[this.ID];
		}
	}

	// Token: 0x06004053 RID: 16467 RVA: 0x001A4730 File Offset: 0x001A2930
	public override BaseQuestAction Clone()
	{
		QuestActionTrackChallenge questActionTrackChallenge = new QuestActionTrackChallenge();
		base.CopyValues(questActionTrackChallenge);
		return questActionTrackChallenge;
	}

	// Token: 0x06004054 RID: 16468 RVA: 0x001A474B File Offset: 0x001A294B
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(QuestActionTrackChallenge.PropChallenge, ref this.ID);
	}

	// Token: 0x04003397 RID: 13207
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropChallenge = "challenge";
}
