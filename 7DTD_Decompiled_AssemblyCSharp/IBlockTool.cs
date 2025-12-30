using System;
using UnityEngine;

// Token: 0x02000196 RID: 406
public interface IBlockTool
{
	// Token: 0x06000C4C RID: 3148
	void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo, PlayerActionsLocal playerActions);

	// Token: 0x06000C4D RID: 3149
	void CheckSpecialKeys(Event _event, PlayerActionsLocal playerActions);

	// Token: 0x06000C4E RID: 3150
	bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput);

	// Token: 0x06000C4F RID: 3151
	bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions);

	// Token: 0x06000C50 RID: 3152
	bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions);

	// Token: 0x06000C51 RID: 3153
	string GetDebugOutput();
}
