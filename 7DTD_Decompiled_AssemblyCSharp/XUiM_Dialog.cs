using System;

// Token: 0x02000EDE RID: 3806
public class XUiM_Dialog : XUiModel
{
	// Token: 0x04005B8F RID: 23439
	public EntityNPC Respondent;

	// Token: 0x04005B90 RID: 23440
	public XUiC_DialogWindowGroup DialogWindowGroup;

	// Token: 0x04005B91 RID: 23441
	public Quest QuestTurnIn;

	// Token: 0x04005B92 RID: 23442
	public string ReturnStatement = "";

	// Token: 0x04005B93 RID: 23443
	public bool keepZoomOnClose;

	// Token: 0x04005B94 RID: 23444
	public DialogStatement LastStatement;
}
