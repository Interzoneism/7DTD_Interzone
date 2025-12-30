using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004FF RID: 1279
[Preserve]
public class ItemActionActivate : ItemAction
{
	// Token: 0x060029E7 RID: 10727 RVA: 0x00110270 File Offset: 0x0010E470
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		_actionData.lastUseTime = Time.time;
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		if (EffectManager.GetValue(PassiveEffects.DisableItem, holdingEntity.inventory.holdingItemItemValue, 0f, holdingEntity, null, _actionData.invData.item.ItemTags, true, true, true, true, true, 1, true, false) > 0f)
		{
			_actionData.lastUseTime = Time.time + 1f;
			Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
			return;
		}
		if (this.soundStart != null)
		{
			_actionData.invData.holdingEntity.PlayOneShot(this.soundStart, this.Sound_in_head, false, false, null);
		}
		_actionData.invData.holdingEntity.inventory.holdingItem.OnHoldingItemActivated(_actionData.invData.holdingEntity.inventory.holdingItemData);
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AllowConcurrentActions()
	{
		return false;
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x00110362 File Offset: 0x0010E562
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return _actionData.HasExecuted;
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x0011036A File Offset: 0x0010E56A
	public override void CancelAction(ItemActionData _actionData)
	{
		base.CancelAction(_actionData);
		_actionData.HasExecuted = false;
		_actionData.lastUseTime = Time.time;
	}
}
