using System;
using UnityEngine.Scripting;

// Token: 0x0200062A RID: 1578
[Preserve]
public class MinEventActionAddOrRemoveBuff : MinEventActionAddBuff
{
	// Token: 0x0600309D RID: 12445 RVA: 0x0014C0D5 File Offset: 0x0014A2D5
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		this.isAdd = base.CanExecute(_eventType, _params);
		return true;
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x0014C0E6 File Offset: 0x0014A2E6
	public override void Execute(MinEventParams _params)
	{
		if (this.isAdd)
		{
			base.Execute(_params);
			return;
		}
		base.Remove(_params);
	}

	// Token: 0x04002716 RID: 10006
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAdd;
}
