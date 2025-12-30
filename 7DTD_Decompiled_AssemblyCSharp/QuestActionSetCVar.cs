using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000895 RID: 2197
[Preserve]
public class QuestActionSetCVar : BaseQuestAction
{
	// Token: 0x0600402A RID: 16426 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x0600402B RID: 16427 RVA: 0x001A3D0D File Offset: 0x001A1F0D
	public override void PerformAction(Quest ownerQuest)
	{
		ownerQuest.OwnerJournal.OwnerPlayer.Buffs.SetCustomVar(this.ID, StringParsers.ParseFloat(this.Value, 0, -1, NumberStyles.Any), true, CVarOperation.set);
	}

	// Token: 0x0600402C RID: 16428 RVA: 0x001A3D40 File Offset: 0x001A1F40
	public override BaseQuestAction Clone()
	{
		QuestActionSetCVar questActionSetCVar = new QuestActionSetCVar();
		base.CopyValues(questActionSetCVar);
		return questActionSetCVar;
	}

	// Token: 0x0600402D RID: 16429 RVA: 0x001A3D5B File Offset: 0x001A1F5B
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(QuestActionSetCVar.PropCVar, ref this.ID);
	}

	// Token: 0x0400337F RID: 13183
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCVar = "cvar";
}
