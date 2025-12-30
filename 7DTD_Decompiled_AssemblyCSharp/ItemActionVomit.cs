using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000554 RID: 1364
[Preserve]
public class ItemActionVomit : ItemActionLauncher
{
	// Token: 0x06002C0A RID: 11274 RVA: 0x00125E9E File Offset: 0x0012409E
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionVomit.ItemActionDataVomit(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x00125EA8 File Offset: 0x001240A8
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		_props.ParseInt("AnimType", ref this.animType);
		this.warningDelay = 1.2f;
		_props.ParseFloat("WarningDelay", ref this.warningDelay);
		this.warningMax = 3;
		_props.ParseInt("WarningMax", ref this.warningMax);
		_props.ParseString("Sound_warning", ref this.soundWarning);
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x00125F12 File Offset: 0x00124112
	[PublicizedFrom(EAccessModifier.Private)]
	public void resetAttack(ItemActionVomit.ItemActionDataVomit _actionData)
	{
		_actionData.numWarningsPlayed = 0;
		_actionData.warningTime = 0f;
		_actionData.bAttackStarted = false;
		_actionData.isActive = false;
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x00125F34 File Offset: 0x00124134
	[PublicizedFrom(EAccessModifier.Protected)]
	public override int GetActionEffectsValues(ItemActionData _actionData, out Vector3 _startPos, out Vector3 _direction)
	{
		ItemActionVomit.ItemActionDataVomit itemActionDataVomit = (ItemActionVomit.ItemActionDataVomit)_actionData;
		Ray lookRay = itemActionDataVomit.invData.holdingEntity.GetLookRay();
		_startPos = lookRay.origin;
		_direction = lookRay.direction;
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		ItemValue holdingItemItemValue = holdingEntity.inventory.holdingItemItemValue;
		ItemClass forId = ItemClass.GetForId(ItemClass.GetItem(this.MagazineItemNames[(int)holdingItemItemValue.SelectedAmmoTypeIndex], false).type);
		if (((ItemActionProjectile)((forId.Actions[0] is ItemActionProjectile) ? forId.Actions[0] : forId.Actions[1])).FlyTime < 0f)
		{
			Transform transform = itemActionDataVomit.projectileJointT;
			if (!transform)
			{
				transform = holdingEntity.emodel.GetRightHandTransform();
			}
			_startPos = transform.position + Origin.position;
			EntityAlive attackTarget = holdingEntity.GetAttackTarget();
			if (attackTarget)
			{
				Vector3 chestPosition = attackTarget.getChestPosition();
				if (holdingEntity.IsInFrontOfMe(chestPosition))
				{
					_direction = chestPosition;
					return 1;
				}
			}
			return 0;
		}
		_direction = this.getDirectionOffset(itemActionDataVomit, _direction, 0);
		return 0;
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x0012605C File Offset: 0x0012425C
	public override void ItemActionEffects(GameManager _gameManager, ItemActionData _actionData, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
		ItemActionVomit.ItemActionDataVomit itemActionDataVomit = (ItemActionVomit.ItemActionDataVomit)_actionData;
		if (itemActionDataVomit.muzzle == null)
		{
			itemActionDataVomit.muzzle = _actionData.invData.holdingEntity.emodel.GetRightHandTransform();
		}
		if (_firingState != 0)
		{
			itemActionDataVomit.numVomits++;
			bool flag = _userData > 0;
			Vector3 dirOrPos = _direction;
			Vector3 direction = itemActionDataVomit.invData.holdingEntity.GetLookRay().direction;
			int burstCount = this.GetBurstCount(_actionData);
			for (int i = 0; i < burstCount; i++)
			{
				if (!flag)
				{
					dirOrPos = this.getDirectionRandomOffset(itemActionDataVomit, direction);
				}
				base.instantiateProjectile(_actionData, default(Vector3)).GetComponent<ProjectileMoveScript>().Fire(_startPos, dirOrPos, _actionData.invData.holdingEntity, this.hitmaskOverride, -1f, flag);
			}
		}
		base.ItemActionEffects(_gameManager, _actionData, _firingState, _startPos, _direction, _userData);
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x0012613C File Offset: 0x0012433C
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		ItemActionVomit.ItemActionDataVomit itemActionDataVomit = (ItemActionVomit.ItemActionDataVomit)_actionData;
		if (_bReleased)
		{
			base.ExecuteAction(_actionData, _bReleased);
			this.resetAttack(itemActionDataVomit);
			return;
		}
		itemActionDataVomit.isActive = true;
		float time = Time.time;
		if (time - itemActionDataVomit.m_LastShotTime < this.Delay)
		{
			return;
		}
		if (itemActionDataVomit.warningTime > 0f && time < itemActionDataVomit.warningTime)
		{
			return;
		}
		if (!itemActionDataVomit.bAttackStarted)
		{
			EntityAlive holdingEntity = _actionData.invData.holdingEntity;
			if (itemActionDataVomit.numWarningsPlayed < this.warningMax - 1 && holdingEntity.rand.RandomFloat < 0.5f)
			{
				itemActionDataVomit.numWarningsPlayed++;
				itemActionDataVomit.warningTime = time + this.warningDelay;
				holdingEntity.PlayOneShot(this.soundWarning, false, false, false, null);
				holdingEntity.Raging = true;
				return;
			}
			itemActionDataVomit.bAttackStarted = true;
			itemActionDataVomit.numVomits = 0;
			holdingEntity.StartAnimAction(this.animType + 3000);
			if (this.warningMax > 0)
			{
				holdingEntity.PlayOneShot(this.soundWarning, false, false, false, null);
				itemActionDataVomit.warningTime = time + this.warningDelay;
				return;
			}
		}
		if (itemActionDataVomit.numVomits >= this.GetMaxAmmoCount(itemActionDataVomit))
		{
			itemActionDataVomit.isActive = false;
			return;
		}
		itemActionDataVomit.curBurstCount = 0;
		base.ExecuteAction(_actionData, _bReleased);
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x00126273 File Offset: 0x00124473
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return ((ItemActionVomit.ItemActionDataVomit)_actionData).isActive;
	}

	// Token: 0x04002251 RID: 8785
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cRadius = -1f;

	// Token: 0x04002252 RID: 8786
	[PublicizedFrom(EAccessModifier.Private)]
	public int animType;

	// Token: 0x04002253 RID: 8787
	[PublicizedFrom(EAccessModifier.Private)]
	public float warningDelay;

	// Token: 0x04002254 RID: 8788
	[PublicizedFrom(EAccessModifier.Private)]
	public int warningMax;

	// Token: 0x04002255 RID: 8789
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundWarning;

	// Token: 0x02000555 RID: 1365
	public class ItemActionDataVomit : ItemActionLauncher.ItemActionDataLauncher
	{
		// Token: 0x06002C12 RID: 11282 RVA: 0x00112C1D File Offset: 0x00110E1D
		public ItemActionDataVomit(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002256 RID: 8790
		public float warningTime;

		// Token: 0x04002257 RID: 8791
		public int numWarningsPlayed;

		// Token: 0x04002258 RID: 8792
		public int numVomits;

		// Token: 0x04002259 RID: 8793
		public bool bAttackStarted;

		// Token: 0x0400225A RID: 8794
		public bool isActive;
	}
}
