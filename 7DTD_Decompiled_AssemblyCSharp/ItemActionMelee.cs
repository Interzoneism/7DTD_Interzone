using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000528 RID: 1320
[Preserve]
public class ItemActionMelee : ItemActionAttack
{
	// Token: 0x06002AC3 RID: 10947 RVA: 0x00119358 File Offset: 0x00117558
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionMelee.InventoryDataMelee(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x00119361 File Offset: 0x00117561
	public override ItemClass.EnumCrosshairType GetCrosshairType(ItemActionData _actionData)
	{
		if (this.isShowOverlay((ItemActionAttackData)_actionData))
		{
			return ItemClass.EnumCrosshairType.Damage;
		}
		return ItemClass.EnumCrosshairType.Plus;
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x00119374 File Offset: 0x00117574
	public override WorldRayHitInfo GetExecuteActionTarget(ItemActionData _actionData)
	{
		ItemActionMelee.InventoryDataMelee inventoryDataMelee = (ItemActionMelee.InventoryDataMelee)_actionData;
		EntityAlive holdingEntity = inventoryDataMelee.invData.holdingEntity;
		inventoryDataMelee.ray = holdingEntity.GetLookRay();
		if (holdingEntity.IsBreakingBlocks)
		{
			if (inventoryDataMelee.ray.direction.y < 0f)
			{
				inventoryDataMelee.ray.direction = new Vector3(inventoryDataMelee.ray.direction.x, 0f, inventoryDataMelee.ray.direction.z);
				ItemActionMelee.InventoryDataMelee inventoryDataMelee2 = inventoryDataMelee;
				inventoryDataMelee2.ray.origin = inventoryDataMelee2.ray.origin + new Vector3(0f, -0.7f, 0f);
			}
		}
		else if (holdingEntity.GetAttackTarget() != null)
		{
			Vector3 direction = holdingEntity.GetAttackTargetHitPosition() - inventoryDataMelee.ray.origin;
			inventoryDataMelee.ray = new Ray(inventoryDataMelee.ray.origin, direction);
		}
		ItemActionMelee.InventoryDataMelee inventoryDataMelee3 = inventoryDataMelee;
		inventoryDataMelee3.ray.origin = inventoryDataMelee3.ray.origin - 0.15f * inventoryDataMelee.ray.direction;
		int modelLayer = holdingEntity.GetModelLayer();
		holdingEntity.SetModelLayer(2, false, null);
		float distance = Utils.FastMax(this.Range, this.BlockRange) + 0.15f;
		if (holdingEntity is EntityEnemy && holdingEntity.IsBreakingBlocks)
		{
			Voxel.Raycast(inventoryDataMelee.invData.world, inventoryDataMelee.ray, distance, 1073807360, 128, 0.4f);
		}
		else
		{
			EntityAlive x = null;
			int layerMask = -538767381;
			if (Voxel.Raycast(inventoryDataMelee.invData.world, inventoryDataMelee.ray, distance, layerMask, 128, this.SphereRadius))
			{
				x = (ItemActionAttack.GetEntityFromHit(Voxel.voxelRayHitInfo) as EntityAlive);
			}
			if (x == null)
			{
				Voxel.Raycast(inventoryDataMelee.invData.world, inventoryDataMelee.ray, distance, -538488837, 128, this.SphereRadius);
			}
		}
		holdingEntity.SetModelLayer(modelLayer, false, null);
		return _actionData.GetUpdatedHitInfo();
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x00119570 File Offset: 0x00117770
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		ItemActionMelee.InventoryDataMelee inventoryDataMelee = (ItemActionMelee.InventoryDataMelee)_actionData;
		if (_bReleased)
		{
			inventoryDataMelee.bFirstHitInARow = true;
			return;
		}
		if (Time.time - inventoryDataMelee.lastUseTime < this.Delay)
		{
			return;
		}
		inventoryDataMelee.lastUseTime = Time.time;
		if (inventoryDataMelee.invData.itemValue.MaxUseTimes > 0 && inventoryDataMelee.invData.itemValue.UseTimes >= (float)inventoryDataMelee.invData.itemValue.MaxUseTimes)
		{
			EntityPlayerLocal player = _actionData.invData.holdingEntity as EntityPlayerLocal;
			if (this.item.Properties.Values.ContainsKey(ItemClass.PropSoundJammed))
			{
				Manager.PlayInsidePlayerHead(this.item.Properties.Values[ItemClass.PropSoundJammed], -1, 0f, false, false);
			}
			GameManager.ShowTooltip(player, "ttItemNeedsRepair", false, false, 0f);
			return;
		}
		_actionData.invData.holdingEntity.RightArmAnimationAttack = true;
		ItemActionAttack.AttackHitInfo attackHitInfo;
		inventoryDataMelee.bHarvesting = this.checkHarvesting(_actionData, out attackHitInfo);
		if (inventoryDataMelee.bHarvesting)
		{
			_actionData.invData.holdingEntity.HarvestingAnimation = true;
		}
		string soundStart = this.soundStart;
		if (soundStart != null)
		{
			_actionData.invData.holdingEntity.PlayOneShot(soundStart, false, false, false, null);
		}
		inventoryDataMelee.bAttackStarted = true;
		if ((double)inventoryDataMelee.invData.holdingEntity.speedForward > 0.009)
		{
			this.rayCastDelay = AnimationDelayData.AnimationDelay[inventoryDataMelee.invData.item.HoldType.Value].RayCastMoving;
			return;
		}
		this.rayCastDelay = AnimationDelayData.AnimationDelay[inventoryDataMelee.invData.item.HoldType.Value].RayCast;
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x00119720 File Offset: 0x00117920
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isShowOverlay(ItemActionData actionData)
	{
		if (!base.isShowOverlay(actionData))
		{
			return false;
		}
		if (((ItemActionMelee.InventoryDataMelee)actionData).bFirstHitInARow && Time.time - actionData.lastUseTime <= this.rayCastDelay)
		{
			return false;
		}
		WorldRayHitInfo executeActionTarget = this.GetExecuteActionTarget(actionData);
		return executeActionTarget.bHitValid && (executeActionTarget.tag == null || !GameUtils.IsBlockOrTerrain(executeActionTarget.tag) || executeActionTarget.hit.distanceSq <= this.BlockRange * this.BlockRange) && (executeActionTarget.tag == null || !executeActionTarget.tag.StartsWith("E_") || executeActionTarget.hit.distanceSq <= this.Range * this.Range);
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x001197D8 File Offset: 0x001179D8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool checkHarvesting(ItemActionData _actionData, out ItemActionAttack.AttackHitInfo myAttackHitInfo)
	{
		WorldRayHitInfo executeActionTarget = this.GetExecuteActionTarget(_actionData);
		ItemValue itemValue = _actionData.invData.itemValue;
		myAttackHitInfo = new ItemActionAttack.AttackHitInfo
		{
			WeaponTypeTag = ItemActionAttack.MeleeTag
		};
		ItemActionAttack.Hit(executeActionTarget, _actionData.invData.holdingEntity.entityId, (this.DamageType == EnumDamageTypes.None) ? EnumDamageTypes.Bashing : this.DamageType, base.GetDamageBlock(itemValue, ItemActionAttack.GetBlockHit(_actionData.invData.world, executeActionTarget), _actionData.invData.holdingEntity, _actionData.indexInEntityOfAction), base.GetDamageEntity(itemValue, _actionData.invData.holdingEntity, _actionData.indexInEntityOfAction), 1f, 1f, 0f, ItemAction.GetDismemberChance(_actionData, executeActionTarget), this.item.MadeOfMaterial.id, this.damageMultiplier, this.getBuffActions(_actionData), myAttackHitInfo, 1, this.ActionExp, this.ActionExpBonusMultiplier, this, this.ToolBonuses, ItemActionAttack.EnumAttackMode.Simulate, null, -1, null);
		if (myAttackHitInfo.bKilled)
		{
			return false;
		}
		if (myAttackHitInfo.itemsToDrop != null && myAttackHitInfo.itemsToDrop.ContainsKey(EnumDropEvent.Harvest))
		{
			List<Block.SItemDropProb> list = myAttackHitInfo.itemsToDrop[EnumDropEvent.Harvest];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].toolCategory != null && this.ToolBonuses != null && this.ToolBonuses.ContainsKey(list[i].toolCategory))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x00119934 File Offset: 0x00117B34
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionMelee.InventoryDataMelee inventoryDataMelee = (ItemActionMelee.InventoryDataMelee)_actionData;
		return Time.time - inventoryDataMelee.lastUseTime < this.Delay + 0.1f;
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x00119968 File Offset: 0x00117B68
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionMelee.InventoryDataMelee inventoryDataMelee = (ItemActionMelee.InventoryDataMelee)_actionData;
		if (!inventoryDataMelee.bAttackStarted || Time.time - inventoryDataMelee.lastUseTime < this.rayCastDelay)
		{
			return;
		}
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		if (this.rayCastDelay <= 0f && !holdingEntity.IsAttackImpact())
		{
			return;
		}
		inventoryDataMelee.bAttackStarted = false;
		ItemActionAttackData.HitDelegate hitDelegate = inventoryDataMelee.hitDelegate;
		inventoryDataMelee.hitDelegate = null;
		if (!holdingEntity.IsAttackValid())
		{
			return;
		}
		float value = EffectManager.GetValue(PassiveEffects.StaminaLoss, inventoryDataMelee.invData.itemValue, 0f, holdingEntity, null, (_actionData.indexInEntityOfAction == 0) ? FastTags<TagGroup.Global>.Parse("primary") : FastTags<TagGroup.Global>.Parse("secondary"), true, true, true, true, true, 1, true, false);
		holdingEntity.AddStamina(-value);
		float damageScale = 1f;
		WorldRayHitInfo worldRayHitInfo;
		if (hitDelegate != null)
		{
			worldRayHitInfo = hitDelegate(out damageScale);
		}
		else
		{
			worldRayHitInfo = this.GetExecuteActionTarget(_actionData);
		}
		if (worldRayHitInfo == null || !worldRayHitInfo.bHitValid)
		{
			return;
		}
		if (worldRayHitInfo.tag != null && GameUtils.IsBlockOrTerrain(worldRayHitInfo.tag) && worldRayHitInfo.hit.distanceSq > this.BlockRange * this.BlockRange)
		{
			return;
		}
		if (worldRayHitInfo.tag != null && worldRayHitInfo.tag.StartsWith("E_") && worldRayHitInfo.hit.distanceSq > this.Range * this.Range)
		{
			return;
		}
		if (inventoryDataMelee.invData.itemValue.MaxUseTimes > 0)
		{
			_actionData.invData.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, inventoryDataMelee.invData.itemValue, 1f, holdingEntity, null, _actionData.invData.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
			base.HandleItemBreak(_actionData);
		}
		if (ItemAction.ShowDebugDisplayHit)
		{
			DebugLines.Create("MeleeHit", holdingEntity.RootTransform, holdingEntity.position, worldRayHitInfo.hit.pos, new Color(0.7f, 0f, 0f), new Color(1f, 1f, 0f), 0.05f, 0.02f, 1f);
		}
		this.hitTheTarget(inventoryDataMelee, worldRayHitInfo, damageScale);
		if (inventoryDataMelee.bFirstHitInARow)
		{
			inventoryDataMelee.bFirstHitInARow = false;
		}
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x00119B9C File Offset: 0x00117D9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void hitTheTarget(ItemActionMelee.InventoryDataMelee _actionData, WorldRayHitInfo hitInfo, float damageScale)
	{
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		ItemValue itemValue = _actionData.invData.itemValue;
		float weaponCondition = 1f;
		if (itemValue.MaxUseTimes > 0)
		{
			weaponCondition = ((float)itemValue.MaxUseTimes - itemValue.UseTimes) / (float)itemValue.MaxUseTimes;
		}
		_actionData.attackDetails.WeaponTypeTag = ItemActionAttack.MeleeTag;
		int num = 1;
		if (this.bUseParticleHarvesting && (this.particleHarvestingCategory == null || this.particleHarvestingCategory == this.item.MadeOfMaterial.id))
		{
			num |= 4;
		}
		float num2 = base.GetDamageBlock(itemValue, ItemActionAttack.GetBlockHit(_actionData.invData.world, hitInfo), holdingEntity, _actionData.indexInEntityOfAction) * damageScale;
		float damageEntity = base.GetDamageEntity(itemValue, holdingEntity, _actionData.indexInEntityOfAction);
		int entityId = holdingEntity.entityId;
		EnumDamageTypes damageType = (this.DamageType == EnumDamageTypes.None) ? EnumDamageTypes.Bashing : this.DamageType;
		float blockDamage = num2;
		float entityDamage = damageEntity;
		Stat stamina = holdingEntity.Stats.Stamina;
		ItemActionAttack.Hit(hitInfo, entityId, damageType, blockDamage, entityDamage, (stamina != null) ? stamina.ValuePercent : 1f, weaponCondition, 0f, ItemAction.GetDismemberChance(_actionData, hitInfo), this.item.MadeOfMaterial.SurfaceCategory, this.damageMultiplier, this.getBuffActions(_actionData), _actionData.attackDetails, num, this.ActionExp, this.ActionExpBonusMultiplier, this, this.ToolBonuses, _actionData.bHarvesting ? ItemActionAttack.EnumAttackMode.RealAndHarvesting : ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, -1, null);
		GameUtils.HarvestOnAttack(_actionData, this.ToolBonuses);
	}

	// Token: 0x04002145 RID: 8517
	[PublicizedFrom(EAccessModifier.Private)]
	public float rayCastDelay;

	// Token: 0x02000529 RID: 1321
	[PublicizedFrom(EAccessModifier.Protected)]
	public class InventoryDataMelee : ItemActionAttackData
	{
		// Token: 0x06002ACD RID: 10957 RVA: 0x00112618 File Offset: 0x00110818
		public InventoryDataMelee(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002146 RID: 8518
		public bool bAttackStarted;

		// Token: 0x04002147 RID: 8519
		public Ray ray;

		// Token: 0x04002148 RID: 8520
		public bool bHarvesting;

		// Token: 0x04002149 RID: 8521
		public bool bFirstHitInARow;
	}
}
