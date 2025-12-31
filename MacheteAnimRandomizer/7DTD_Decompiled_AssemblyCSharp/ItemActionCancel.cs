using System;
using UnityEngine.Scripting;

// Token: 0x02000509 RID: 1289
[Preserve]
public class ItemActionCancel : ItemAction
{
	// Token: 0x06002A17 RID: 10775 RVA: 0x00112624 File Offset: 0x00110824
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (_bReleased)
		{
			int num = 1 - _actionData.indexInEntityOfAction;
			if (_actionData.invData.actionData[num] != null)
			{
				_actionData.invData.item.Actions[num].CancelAction(_actionData.invData.actionData[num]);
			}
		}
	}
}
