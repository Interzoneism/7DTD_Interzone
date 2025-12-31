using System;
using UnityEngine.Scripting;

// Token: 0x02000633 RID: 1587
[Preserve]
public class MinEventActionLogItemData : MinEventActionBase
{
	// Token: 0x060030B6 RID: 12470 RVA: 0x0014C83C File Offset: 0x0014AA3C
	public override void Execute(MinEventParams _params)
	{
		if (_params.Self.inventory.holdingItem == null)
		{
			return;
		}
		Log.Out("Debug Item: '{0}' Tags: {1}", new object[]
		{
			_params.Self.inventory.holdingItem.GetItemName(),
			_params.Self.inventory.holdingItem.ItemTags.ToString()
		});
	}

	// Token: 0x0400272E RID: 10030
	[PublicizedFrom(EAccessModifier.Private)]
	public string message;
}
