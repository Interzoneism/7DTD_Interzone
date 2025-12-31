using System;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000551 RID: 1361
[Preserve]
public class ItemActionThrownWeapon : ItemActionThrowAway
{
	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06002BE2 RID: 11234 RVA: 0x00124CA7 File Offset: 0x00122EA7
	// (set) Token: 0x06002BE3 RID: 11235 RVA: 0x00124CAF File Offset: 0x00122EAF
	public new ExplosionData Explosion { get; set; }

	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x06002BE4 RID: 11236 RVA: 0x00124CB8 File Offset: 0x00122EB8
	// (set) Token: 0x06002BE5 RID: 11237 RVA: 0x00124CC0 File Offset: 0x00122EC0
	public new int Velocity { get; set; }

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x06002BE6 RID: 11238 RVA: 0x00124CC9 File Offset: 0x00122EC9
	// (set) Token: 0x06002BE7 RID: 11239 RVA: 0x00124CD1 File Offset: 0x00122ED1
	public new float FlyTime { get; set; }

	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x06002BE8 RID: 11240 RVA: 0x00124CDA File Offset: 0x00122EDA
	// (set) Token: 0x06002BE9 RID: 11241 RVA: 0x00124CE2 File Offset: 0x00122EE2
	public new float LifeTime { get; set; }

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06002BEA RID: 11242 RVA: 0x00124CEB File Offset: 0x00122EEB
	// (set) Token: 0x06002BEB RID: 11243 RVA: 0x00124CF3 File Offset: 0x00122EF3
	public new float CollisionRadius { get; set; }

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06002BEC RID: 11244 RVA: 0x00124CFC File Offset: 0x00122EFC
	// (set) Token: 0x06002BED RID: 11245 RVA: 0x00124D04 File Offset: 0x00122F04
	public float Gravity { get; set; }

	// Token: 0x06002BEE RID: 11246 RVA: 0x00124D10 File Offset: 0x00122F10
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		this.Explosion = new ExplosionData(this.Properties, this.item.Effects);
		if (this.Properties.Values.ContainsKey("Velocity"))
		{
			this.Velocity = (int)StringParsers.ParseFloat(this.Properties.Values["Velocity"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.Velocity = 1;
		}
		if (this.Properties.Values.ContainsKey("FlyTime"))
		{
			this.FlyTime = StringParsers.ParseFloat(this.Properties.Values["FlyTime"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.FlyTime = 20f;
		}
		if (this.Properties.Values.ContainsKey("LifeTime"))
		{
			this.LifeTime = StringParsers.ParseFloat(this.Properties.Values["LifeTime"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.LifeTime = 100f;
		}
		if (this.Properties.Values.ContainsKey("CollisionRadius"))
		{
			this.CollisionRadius = StringParsers.ParseFloat(this.Properties.Values["CollisionRadius"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.CollisionRadius = 0.05f;
		}
		if (this.Properties.Values.ContainsKey("Gravity"))
		{
			this.Gravity = StringParsers.ParseFloat(this.Properties.Values["Gravity"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.Gravity = -9.81f;
		}
		if (_props.Values.ContainsKey("DamageEntity"))
		{
			this.damageEntity = StringParsers.ParseFloat(_props.Values["DamageEntity"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.damageEntity = 0f;
		}
		if (_props.Values.ContainsKey("DamageBlock"))
		{
			this.damageBlock = StringParsers.ParseFloat(_props.Values["DamageBlock"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.damageBlock = 0f;
		}
		this.hitmaskOverride = Voxel.ToHitMask(_props.GetString("Hitmask_override"));
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x00124F4C File Offset: 0x0012314C
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return ((ItemActionThrowAway.MyInventoryData)_actionData).m_bActivated;
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x00124F59 File Offset: 0x00123159
	public override void StartHolding(ItemActionData _actionData)
	{
		base.StartHolding(_actionData);
		ItemActionThrowAway.MyInventoryData myInventoryData = (ItemActionThrowAway.MyInventoryData)_actionData;
		myInventoryData.m_bActivated = false;
		myInventoryData.m_ActivateTime = 0f;
		myInventoryData.m_LastThrowTime = 0f;
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x00124F84 File Offset: 0x00123184
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionThrowAway.MyInventoryData myInventoryData = (ItemActionThrowAway.MyInventoryData)_actionData;
		float num = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? 0.1f : AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast;
		if (myInventoryData.m_LastThrowTime <= 0f || Time.time - myInventoryData.m_LastThrowTime < num)
		{
			return;
		}
		GameManager.Instance.ItemActionEffectsServer(myInventoryData.invData.holdingEntity.entityId, myInventoryData.invData.slotIdx, myInventoryData.indexInEntityOfAction, 0, Vector3.zero, Vector3.zero, 0);
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x00125024 File Offset: 0x00123224
	public override void ItemActionEffects(GameManager _gameManager, ItemActionData _actionData, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
		_actionData.invData.holdingEntity.emodel.avatarController.TriggerEvent("WeaponFire");
		(_actionData as ItemActionThrowAway.MyInventoryData).m_LastThrowTime = 0f;
		if (!_actionData.invData.holdingEntity.isEntityRemote)
		{
			this.throwAway(_actionData as ItemActionThrowAway.MyInventoryData);
		}
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x00125080 File Offset: 0x00123280
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		ItemActionThrowAway.MyInventoryData myInventoryData = (ItemActionThrowAway.MyInventoryData)_actionData;
		if (!myInventoryData.m_bActivated && Time.time - myInventoryData.m_LastThrowTime < this.Delay)
		{
			return;
		}
		if (_actionData.invData.itemValue.PercentUsesLeft == 0f)
		{
			if (_bReleased)
			{
				EntityPlayerLocal player = _actionData.invData.holdingEntity as EntityPlayerLocal;
				if (this.item.Properties.Values.ContainsKey(ItemClass.PropSoundJammed))
				{
					Manager.PlayInsidePlayerHead(this.item.Properties.Values[ItemClass.PropSoundJammed], -1, 0f, false, false);
				}
				GameManager.ShowTooltip(player, "ttItemNeedsRepair", false, false, 0f);
			}
			return;
		}
		if (!_bReleased)
		{
			if (!myInventoryData.m_bActivated && !myInventoryData.m_bCanceled)
			{
				myInventoryData.m_bActivated = true;
				myInventoryData.m_ActivateTime = Time.time;
				myInventoryData.invData.holdingEntity.emodel.avatarController.TriggerEvent("WeaponPreFire");
			}
			return;
		}
		if (myInventoryData.m_bCanceled)
		{
			myInventoryData.m_bCanceled = false;
			return;
		}
		if (!myInventoryData.m_bActivated)
		{
			return;
		}
		myInventoryData.m_ThrowStrength = Mathf.Min(this.maxStrainTime, Time.time - myInventoryData.m_ActivateTime) / this.maxStrainTime * this.maxThrowStrength;
		myInventoryData.m_LastThrowTime = Time.time;
		myInventoryData.m_bActivated = false;
		if (!myInventoryData.invData.holdingEntity.isEntityRemote)
		{
			myInventoryData.invData.holdingEntity.emodel.avatarController.TriggerEvent("WeaponFire");
		}
		if (this.soundStart != null)
		{
			myInventoryData.invData.holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
		}
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x00125220 File Offset: 0x00123420
	public override void CancelAction(ItemActionData _actionData)
	{
		ItemActionThrowAway.MyInventoryData myInventoryData = (ItemActionThrowAway.MyInventoryData)_actionData;
		myInventoryData.invData.holdingEntity.emodel.avatarController.TriggerEvent("WeaponPreFireCancel");
		myInventoryData.m_bActivated = false;
		myInventoryData.m_bCanceled = true;
		myInventoryData.m_ActivateTime = 0f;
		myInventoryData.m_LastThrowTime = 0f;
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x00125278 File Offset: 0x00123478
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void throwAway(ItemActionThrowAway.MyInventoryData _actionData)
	{
		ThrownWeaponMoveScript thrownWeaponMoveScript = this.instantiateProjectile(_actionData);
		ItemInventoryData invData = _actionData.invData;
		Vector3 lookVector = _actionData.invData.holdingEntity.GetLookVector();
		_actionData.invData.holdingEntity.getHeadPosition();
		Vector3 origin = _actionData.invData.holdingEntity.GetLookRay().origin;
		if (_actionData.invData.holdingEntity as EntityPlayerLocal != null)
		{
			float value = EffectManager.GetValue(PassiveEffects.StaminaLoss, _actionData.invData.holdingEntity.inventory.holdingItemItemValue, 2f, _actionData.invData.holdingEntity, null, _actionData.invData.holdingEntity.inventory.holdingItem.ItemTags | FastTags<TagGroup.Global>.Parse((_actionData.indexInEntityOfAction == 0) ? "primary" : "secondary"), true, true, true, true, true, 1, true, false);
			_actionData.invData.holdingEntity.Stats.Stamina.Value -= value;
		}
		thrownWeaponMoveScript.Fire(origin, lookVector, _actionData.invData.holdingEntity, this.hitmaskOverride, _actionData.m_ThrowStrength);
		_actionData.invData.holdingEntity.inventory.DecHoldingItem(1);
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x001253B0 File Offset: 0x001235B0
	public ThrownWeaponMoveScript instantiateProjectile(ItemActionData _actionData)
	{
		ItemValue holdingItemItemValue = _actionData.invData.holdingEntity.inventory.holdingItemItemValue;
		int entityId = _actionData.invData.holdingEntity.entityId;
		new ItemValue(_actionData.invData.item.Id, false);
		Transform transform = UnityEngine.Object.Instantiate<GameObject>(_actionData.invData.holdingEntity.inventory.models[_actionData.invData.holdingEntity.inventory.holdingItemIdx].gameObject).transform;
		Utils.ForceMaterialsInstance(transform.gameObject);
		transform.parent = null;
		transform.position = _actionData.invData.model.transform.position;
		transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		Utils.SetLayerRecursively(transform.gameObject, 0);
		ThrownWeaponMoveScript thrownWeaponMoveScript = transform.gameObject.AddMissingComponent<ThrownWeaponMoveScript>();
		thrownWeaponMoveScript.itemActionThrownWeapon = this;
		thrownWeaponMoveScript.itemWeapon = _actionData.invData.item;
		thrownWeaponMoveScript.itemValueWeapon = _actionData.invData.itemValue;
		thrownWeaponMoveScript.actionData = (_actionData as ItemActionThrowAway.MyInventoryData);
		thrownWeaponMoveScript.ProjectileOwnerID = _actionData.invData.holdingEntity.entityId;
		transform.gameObject.SetActive(true);
		_actionData.invData.model.gameObject.SetActive(false);
		_actionData.invData.holdingEntity.MinEventContext.Self = _actionData.invData.holdingEntity;
		_actionData.invData.holdingEntity.MinEventContext.Transform = transform;
		_actionData.invData.holdingEntity.MinEventContext.Tags = this.usePassedInTransformTag;
		thrownWeaponMoveScript.itemValueWeapon.FireEvent(MinEventTypes.onSelfHoldingItemThrown, _actionData.invData.holdingEntity.MinEventContext);
		return thrownWeaponMoveScript;
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x00125570 File Offset: 0x00123770
	public float GetDamageEntity(ItemValue _itemValue, EntityAlive _holdingEntity = null, int actionIndex = 0)
	{
		this.tmpTag = ((actionIndex == 0) ? ItemActionAttack.PrimaryTag : ItemActionAttack.SecondaryTag);
		this.tmpTag |= ((_itemValue.ItemClass == null) ? ItemActionAttack.MeleeTag : _itemValue.ItemClass.ItemTags);
		if (_holdingEntity != null)
		{
			this.tmpTag |= (_holdingEntity.CurrentStanceTag | _holdingEntity.CurrentMovementTag);
		}
		return EffectManager.GetValue(PassiveEffects.EntityDamage, _itemValue, this.damageEntity, _holdingEntity, null, this.tmpTag, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x00125608 File Offset: 0x00123808
	public float GetDamageBlock(ItemValue _itemValue, BlockValue _blockValue, EntityAlive _holdingEntity = null, int actionIndex = 0)
	{
		this.tmpTag = ((actionIndex == 0) ? ItemActionAttack.PrimaryTag : ItemActionAttack.SecondaryTag);
		this.tmpTag |= ((_itemValue.ItemClass == null) ? ItemActionAttack.MeleeTag : _itemValue.ItemClass.ItemTags);
		if (_holdingEntity != null)
		{
			this.tmpTag |= (_holdingEntity.CurrentStanceTag | _holdingEntity.CurrentMovementTag);
		}
		this.tmpTag |= _blockValue.Block.Tags;
		float value = EffectManager.GetValue(PassiveEffects.BlockDamage, _itemValue, this.damageBlock, _holdingEntity, null, this.tmpTag, true, true, true, true, true, 1, true, false);
		return Utils.FastMin((float)_blockValue.Block.blockMaterial.MaxIncomingDamage, value);
	}

	// Token: 0x04002243 RID: 8771
	public float damageEntity;

	// Token: 0x04002244 RID: 8772
	public float damageBlock;

	// Token: 0x04002245 RID: 8773
	[PublicizedFrom(EAccessModifier.Protected)]
	public int hitmaskOverride;

	// Token: 0x04002246 RID: 8774
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> usePassedInTransformTag = FastTags<TagGroup.Global>.Parse("usePassedInTransform");

	// Token: 0x04002247 RID: 8775
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tmpTag;
}
