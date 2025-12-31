using System;
using UnityEngine.Scripting;

// Token: 0x02000896 RID: 2198
[Preserve]
public class QuestActionShowMessageWindow : BaseQuestAction
{
	// Token: 0x06004030 RID: 16432 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x06004031 RID: 16433 RVA: 0x001A3D81 File Offset: 0x001A1F81
	public override void PerformAction(Quest ownerQuest)
	{
		XUiC_TipWindow.ShowTip(this.message, this.title, XUiM_Player.GetPlayer() as EntityPlayerLocal, null);
	}

	// Token: 0x06004032 RID: 16434 RVA: 0x001A3DA0 File Offset: 0x001A1FA0
	public override BaseQuestAction Clone()
	{
		QuestActionShowMessageWindow questActionShowMessageWindow = new QuestActionShowMessageWindow();
		base.CopyValues(questActionShowMessageWindow);
		questActionShowMessageWindow.message = this.message;
		questActionShowMessageWindow.title = this.title;
		return questActionShowMessageWindow;
	}

	// Token: 0x06004033 RID: 16435 RVA: 0x001A3DD3 File Offset: 0x001A1FD3
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(QuestActionShowMessageWindow.PropMessage, ref this.message);
		properties.ParseString(QuestActionShowMessageWindow.PropTitle, ref this.title);
	}

	// Token: 0x04003380 RID: 13184
	[PublicizedFrom(EAccessModifier.Private)]
	public static string PropMessage = "message";

	// Token: 0x04003381 RID: 13185
	[PublicizedFrom(EAccessModifier.Private)]
	public static string PropTitle = "title";

	// Token: 0x04003382 RID: 13186
	[PublicizedFrom(EAccessModifier.Private)]
	public string message = "";

	// Token: 0x04003383 RID: 13187
	[PublicizedFrom(EAccessModifier.Private)]
	public string title = "";
}
