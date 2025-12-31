using System;
using UnityEngine.Scripting;

// Token: 0x020002A2 RID: 674
[Preserve]
public class DialogActionVoice : BaseDialogAction
{
	// Token: 0x17000207 RID: 519
	// (get) Token: 0x06001312 RID: 4882 RVA: 0x00075E2B File Offset: 0x0007402B
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.Voice;
		}
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00075E2E File Offset: 0x0007402E
	public override void PerformAction(EntityPlayer player)
	{
		LocalPlayerUI.primaryUI.xui.Dialog.Respondent.PlayVoiceSetEntry(base.ID, player, true, true);
	}

	// Token: 0x04000C92 RID: 3218
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
