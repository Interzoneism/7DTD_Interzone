using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200056A RID: 1386
[Preserve]
public class ItemClassTimeBomb : ItemClass
{
	// Token: 0x06002CD8 RID: 11480 RVA: 0x0012AF68 File Offset: 0x00129168
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(this.Properties, this.Effects);
		this.Properties.ParseBool("ExplodeOnHit", ref this.bExplodeOnHitGround);
		this.Properties.ParseBool("FuseStartOnDrop", ref this.dropStarts);
		this.Properties.ParseBool("FusePrimeOnActivate", ref this.mustPrime);
		string @string = this.Properties.GetString("ActivationTransformToHide");
		if (@string.Length > 0)
		{
			this.activationTransformToHide = @string.Split(';', StringSplitOptions.None);
		}
		this.activationEmissive = this.Properties.GetString("ActivationEmissive");
		float num = 2f;
		this.Properties.ParseFloat("FuseTime", ref num);
		this.explodeAfterTicks = (int)(num * 20f);
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x0012B039 File Offset: 0x00129239
	public override void StartHolding(ItemInventoryData _data, Transform _modelTransform)
	{
		base.StartHolding(_data, _modelTransform);
		this.OnHoldingReset(_data);
		this.activateHolding(_data.itemValue, 0, _modelTransform);
		_data.Changed();
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x0012B060 File Offset: 0x00129260
	public override void OnHoldingItemActivated(ItemInventoryData _data)
	{
		ItemValue itemValue = _data.itemValue;
		if (itemValue.Meta != 0)
		{
			return;
		}
		if (_data.holdingEntity.emodel.avatarController != null)
		{
			_data.holdingEntity.emodel.avatarController.CancelEvent("WeaponPreFireCancel");
		}
		this.setActivationTransformsActive(_data.holdingEntity.inventory.models[_data.holdingEntity.inventory.holdingItemIdx], true);
		_data.holdingEntity.RightArmAnimationUse = true;
		int ticks = this.explodeAfterTicks;
		if (this.mustPrime)
		{
			ticks = -1;
		}
		this.activateHolding(itemValue, ticks, _data.model);
		_data.Changed();
		AudioSource audioSource = (_data.model != null) ? _data.model.GetComponentInChildren<AudioSource>() : null;
		if (audioSource)
		{
			audioSource.Play();
		}
		if (!_data.holdingEntity.isEntityRemote)
		{
			_data.gameManager.SimpleRPC(_data.holdingEntity.entityId, SimpleRPCType.OnActivateItem, false, false);
		}
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x0012B15C File Offset: 0x0012935C
	public override void OnHoldingUpdate(ItemInventoryData _data)
	{
		base.OnHoldingUpdate(_data);
		if (_data.holdingEntity.isEntityRemote)
		{
			return;
		}
		ItemValue itemValue = _data.itemValue;
		if (itemValue.Meta > 0)
		{
			itemValue.Meta--;
			if (itemValue.Meta == 0)
			{
				Vector3 vector = (_data.model != null) ? (_data.model.position + Origin.position) : _data.holdingEntity.GetPosition();
				MinEventParams.CachedEventParam.Self = _data.holdingEntity;
				MinEventParams.CachedEventParam.Position = vector;
				MinEventParams.CachedEventParam.ItemValue = itemValue;
				itemValue.FireEvent(MinEventTypes.onProjectileImpact, MinEventParams.CachedEventParam);
				_data.gameManager.ExplosionServer(0, vector, World.worldToBlockPos(vector), Quaternion.identity, this.explosion, _data.holdingEntity.entityId, 0.1f, false, itemValue.Clone());
				this.activateHolding(itemValue, 0, _data.model);
				_data.holdingEntity.inventory.DecHoldingItem(1);
				return;
			}
			_data.Changed();
		}
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x0012B26C File Offset: 0x0012946C
	public override void OnHoldingReset(ItemInventoryData _data)
	{
		_data.itemValue.Meta = 0;
		this.setActivationTransformsActive(_data.holdingEntity.inventory.models[_data.holdingEntity.inventory.holdingItemIdx], false);
		if (!_data.holdingEntity.isEntityRemote)
		{
			_data.gameManager.SimpleRPC(_data.holdingEntity.entityId, SimpleRPCType.OnResetItem, false, false);
		}
		if (_data.holdingEntity.emodel.avatarController != null)
		{
			_data.holdingEntity.emodel.avatarController.TriggerEvent("WeaponPreFireCancel");
		}
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x0012B308 File Offset: 0x00129508
	[PublicizedFrom(EAccessModifier.Private)]
	public void activateHolding(ItemValue iv, int ticks, Transform _t)
	{
		iv.Meta = ticks;
		if (_t != null)
		{
			OnActivateItemGameObjectReference component = _t.GetComponent<OnActivateItemGameObjectReference>();
			if (component != null)
			{
				bool flag = ticks != 0;
				if (component.IsActivated() != flag)
				{
					component.ActivateItem(flag);
				}
			}
		}
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x0012B34C File Offset: 0x0012954C
	public override void OnMeshCreated(ItemWorldData _data)
	{
		EntityItem entityItem = _data.entityItem;
		ItemValue itemValue = entityItem.itemStack.itemValue;
		this.setActivationTransformsActive(entityItem.GetModelTransform(), itemValue.Meta != 0);
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x0012B381 File Offset: 0x00129581
	public override bool CanDrop(ItemValue _iv = null)
	{
		return this.bCanDrop && (_iv == null || _iv.Meta == 0);
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x0012B39B File Offset: 0x0012959B
	public override void Deactivate(ItemValue _iv)
	{
		_iv.Meta = 0;
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x0012B3A4 File Offset: 0x001295A4
	public override void OnDroppedUpdate(ItemWorldData _data)
	{
		EntityItem entityItem = _data.entityItem;
		bool flag = _data.world.IsRemote();
		ItemValue itemValue = entityItem.itemStack.itemValue;
		if (itemValue.Meta == 65535)
		{
			itemValue.Meta = -1;
		}
		Vector3 vector = entityItem.PhysicsMasterGetFinalPosition();
		if (!flag && this.bExplodeOnHitGround && (!this.mustPrime || itemValue.Meta > 0) && (entityItem.isCollided || _data.world.IsWater(vector)))
		{
			itemValue.FireEvent(MinEventTypes.onProjectileImpact, new MinEventParams
			{
				Self = (_data.world.GetEntity(_data.belongsEntityId) as EntityAlive),
				IsLocal = true,
				Position = vector,
				ItemValue = itemValue
			});
			itemValue.Meta = 1;
		}
		if (!flag && ((this.dropStarts && itemValue.Meta == 0) || (this.mustPrime && itemValue.Meta <= -1)))
		{
			Animator componentInChildren = entityItem.gameObject.GetComponentInChildren<Animator>();
			if (componentInChildren != null)
			{
				componentInChildren.SetBool("PinPulled", true);
			}
			itemValue.Meta = this.explodeAfterTicks;
		}
		if (itemValue.Meta > 0)
		{
			OnActivateItemGameObjectReference onActivateItemGameObjectReference = (entityItem.GetModelTransform() != null) ? entityItem.GetModelTransform().GetComponent<OnActivateItemGameObjectReference>() : null;
			if (onActivateItemGameObjectReference != null && !onActivateItemGameObjectReference.IsActivated())
			{
				onActivateItemGameObjectReference.ActivateItem(true);
				this.setActivationTransformsActive(entityItem.GetModelTransform(), true);
			}
			if (flag)
			{
				return;
			}
			this.tickSoundDelay -= 0.05f;
			if (this.tickSoundDelay <= 0f)
			{
				this.tickSoundDelay = entityItem.itemClass.SoundTickDelay;
				entityItem.PlayOneShot(entityItem.itemClass.SoundTick, false, false, false, null);
			}
			itemValue.Meta--;
			if (itemValue.Meta == 0)
			{
				entityItem.SetDead();
				_data.gameManager.ExplosionServer(0, vector, World.worldToBlockPos(vector), Quaternion.identity, this.explosion, _data.belongsEntityId, 0f, false, itemValue.Clone());
				return;
			}
			entityItem.itemStack.itemValue = itemValue;
		}
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x0012B5B4 File Offset: 0x001297B4
	public override void OnDamagedByExplosion(ItemWorldData _data)
	{
		_data.gameManager.ExplosionServer(0, _data.entityItem.GetPosition(), World.worldToBlockPos(_data.entityItem.GetPosition()), Quaternion.identity, this.explosion, _data.belongsEntityId, _data.entityItem.rand.RandomRange(0.1f, 0.3f), false, _data.entityItem.itemStack.itemValue.Clone());
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x0012B629 File Offset: 0x00129829
	public override bool CanCollect(ItemValue _iv)
	{
		return _iv.Meta == 0;
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x0012B634 File Offset: 0x00129834
	[PublicizedFrom(EAccessModifier.Private)]
	public void setActivationTransformsActive(Transform item, bool isActive)
	{
		if (this.activationTransformToHide != null)
		{
			foreach (string name in this.activationTransformToHide)
			{
				Transform transform = item.FindInChilds(name, false);
				if (transform)
				{
					transform.gameObject.SetActive(isActive);
				}
			}
		}
		if (!string.IsNullOrEmpty(this.activationEmissive))
		{
			float value = (float)(isActive ? 1 : 0);
			foreach (Renderer renderer in item.GetComponentsInChildren<Renderer>(true))
			{
				if (renderer.CompareTag(this.activationEmissive))
				{
					renderer.material.SetFloat("_EmissionMultiply", value);
				}
			}
		}
	}

	// Token: 0x04002377 RID: 9079
	public bool bExplodeOnHitGround;

	// Token: 0x04002378 RID: 9080
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPrimed = -1;

	// Token: 0x04002379 RID: 9081
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosion;

	// Token: 0x0400237A RID: 9082
	[PublicizedFrom(EAccessModifier.Private)]
	public bool dropStarts;

	// Token: 0x0400237B RID: 9083
	[PublicizedFrom(EAccessModifier.Private)]
	public bool mustPrime;

	// Token: 0x0400237C RID: 9084
	[PublicizedFrom(EAccessModifier.Private)]
	public int explodeAfterTicks;

	// Token: 0x0400237D RID: 9085
	[PublicizedFrom(EAccessModifier.Private)]
	public float tickSoundDelay;

	// Token: 0x0400237E RID: 9086
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] activationTransformToHide;

	// Token: 0x0400237F RID: 9087
	[PublicizedFrom(EAccessModifier.Private)]
	public string activationEmissive;
}
