using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200056F RID: 1391
public class ThrownWeaponMoveScript : MonoBehaviour
{
	// Token: 0x06002CF9 RID: 11513 RVA: 0x0012C6F7 File Offset: 0x0012A8F7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (ThrownWeaponMoveScript.gameManager == null)
		{
			ThrownWeaponMoveScript.gameManager = (GameManager)UnityEngine.Object.FindObjectOfType(typeof(GameManager));
		}
	}

	// Token: 0x06002CFA RID: 11514 RVA: 0x0012C720 File Offset: 0x0012A920
	public void Fire(Vector3 _idealStartPosition, Vector3 _flyDirection, Entity _firingEntity, int _hmOverride = 0, float _velocity = -1f)
	{
		this.flyDirection = _flyDirection.normalized;
		this.idealPosition = _idealStartPosition;
		this.firingEntity = _firingEntity;
		this.velocity = this.flyDirection.normalized * EffectManager.GetValue(PassiveEffects.ProjectileVelocity, this.itemValueWeapon, (_velocity == -1f) ? ((float)this.itemActionThrownWeapon.Velocity) : _velocity, _firingEntity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.bArmed = true;
		this.hmOverride = _hmOverride;
		this.waterCollisionParticles.Init(this.ProjectileOwnerID, this.itemWeapon.MadeOfMaterial.SurfaceCategory, "water", 16);
		if (_idealStartPosition == Vector3.zero)
		{
			this.previousPosition = base.transform.position + Origin.position;
		}
		else
		{
			this.previousPosition = _idealStartPosition;
		}
		CapsuleCollider component = base.transform.GetComponent<CapsuleCollider>();
		if (component != null)
		{
			component.enabled = false;
		}
		this.gravity = Vector3.up * EffectManager.GetValue(PassiveEffects.ProjectileGravity, this.itemValueWeapon, this.itemActionThrownWeapon.Gravity, _firingEntity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (base.transform.GetComponent<OnActivateItemGameObjectReference>() != null)
		{
			base.transform.GetComponent<OnActivateItemGameObjectReference>().ActivateItem(true);
		}
		this.timeShotStarted = Time.time;
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
			Utils.SetLayerRecursively(base.transform.gameObject, 0);
		}
		base.transform.position = _idealStartPosition - Origin.position;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002CFB RID: 11515 RVA: 0x0012C8F0 File Offset: 0x0012AAF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FixedUpdate()
	{
		if (!this.bArmed)
		{
			if (base.transform.lossyScale.x < 0.01f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (!base.transform.gameObject.activeInHierarchy)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (!this.stuckInEntity)
			{
				base.transform.position = this.FinalPosition - Origin.position;
				return;
			}
		}
		else
		{
			if (ThrownWeaponMoveScript.gameManager == null || ThrownWeaponMoveScript.gameManager.World == null)
			{
				return;
			}
			if (Time.time - this.timeShotStarted >= this.itemActionThrownWeapon.FlyTime)
			{
				this.velocity += this.gravity * Time.fixedDeltaTime;
			}
			Vector3 vector = this.velocity * Time.fixedDeltaTime;
			if (!ThrownWeaponMoveScript.gameManager.World.IsChunkAreaCollidersLoaded(base.transform.position + vector + Origin.position))
			{
				vector = this.gravity * Time.fixedDeltaTime;
			}
			base.transform.LookAt(base.transform.position + vector);
			base.transform.Rotate(Vector3.right, 90f);
			this.checkCollision(vector);
			if (this.bArmed)
			{
				base.transform.position = base.transform.position + vector;
				if (Time.time - this.timeShotStarted >= this.itemActionThrownWeapon.LifeTime)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}

	// Token: 0x06002CFC RID: 11516 RVA: 0x0012CA94 File Offset: 0x0012AC94
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void checkCollision(Vector3 _amountToMove)
	{
		if (ThrownWeaponMoveScript.gameManager == null || ThrownWeaponMoveScript.gameManager.World == null)
		{
			return;
		}
		Vector3 a = _amountToMove;
		float num = a.magnitude;
		if (num <= 0f)
		{
			return;
		}
		Vector3 vector = a * (1f / num);
		num += 0.5f;
		EntityAlive entityAlive = (EntityAlive)this.firingEntity;
		Ray ray = new Ray(base.transform.position + Origin.position + vector * -0.2f, vector);
		this.waterCollisionParticles.CheckCollision(ray.origin, ray.direction, num, (entityAlive != null) ? entityAlive.entityId : -1);
		int layerId = 0;
		if (entityAlive != null && entityAlive.emodel != null)
		{
			layerId = entityAlive.GetModelLayer();
			entityAlive.SetModelLayer(2, false, null);
		}
		bool flag = Voxel.Raycast(ThrownWeaponMoveScript.gameManager.World, ray, num, -538750981, (this.hmOverride == 0) ? 8 : this.hmOverride, 0f);
		if (entityAlive != null && entityAlive.emodel != null)
		{
			entityAlive.SetModelLayer(layerId, false, null);
		}
		if (flag && (GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag) || Voxel.voxelRayHitInfo.tag.StartsWith("E_")))
		{
			if (this.firingEntity != null && !this.firingEntity.isEntityRemote)
			{
				this.stuckInEntity = (ItemActionAttack.FindHitEntity(Voxel.voxelRayHitInfo) as EntityAlive);
				entityAlive.MinEventContext.Other = this.stuckInEntity;
				entityAlive.FireEvent(MinEventTypes.onProjectileImpact, false);
				MinEventParams.CachedEventParam.Self = entityAlive;
				MinEventParams.CachedEventParam.Position = Voxel.voxelRayHitInfo.hit.pos;
				MinEventParams.CachedEventParam.ItemValue = this.itemValueWeapon;
				this.itemWeapon.FireEvent(MinEventTypes.onProjectileImpact, MinEventParams.CachedEventParam);
				ItemActionAttack.AttackHitInfo attackDetails = new ItemActionAttack.AttackHitInfo
				{
					WeaponTypeTag = ItemActionAttack.ThrownTag
				};
				if (this.itemValueWeapon.MaxUseTimes > 0)
				{
					this.itemValueWeapon.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, this.itemValueWeapon, 1f, this.firingEntity as EntityAlive, null, this.itemValueWeapon.ItemClass.ItemTags | FastTags<TagGroup.Global>.Parse("Secondary"), true, true, true, true, true, 1, true, false);
				}
				ItemActionAttack.Hit(Voxel.voxelRayHitInfo, this.ProjectileOwnerID, EnumDamageTypes.Piercing, this.itemActionThrownWeapon.GetDamageBlock(this.itemValueWeapon, ItemActionAttack.GetBlockHit(ThrownWeaponMoveScript.gameManager.World, Voxel.voxelRayHitInfo), entityAlive, 1), this.itemActionThrownWeapon.GetDamageEntity(this.itemValueWeapon, entityAlive, 1), 1f, 1f, EffectManager.GetValue(PassiveEffects.CriticalChance, this.itemValueWeapon, this.itemWeapon.CritChance.Value, entityAlive, null, this.itemWeapon.ItemTags, true, true, true, true, true, 1, true, false), ItemAction.GetDismemberChance(this.actionData, Voxel.voxelRayHitInfo), this.itemWeapon.MadeOfMaterial.SurfaceCategory, null, this.getBuffActions(), attackDetails, 1, this.itemActionThrownWeapon.ActionExp, this.itemActionThrownWeapon.ActionExpBonusMultiplier, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, -1, this.itemValueWeapon);
				this.NavObject = NavObjectManager.Instance.RegisterNavObject(this.itemWeapon.NavObject, base.transform, "", false);
				if (GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag))
				{
					this.ProjectileID = ProjectileManager.AddProjectileItem(base.transform, -1, Voxel.voxelRayHitInfo.hit.pos, a.normalized, this.itemValueWeapon.type);
				}
				else
				{
					this.ProjectileID = ProjectileManager.AddProjectileItem(base.transform, -1, Voxel.voxelRayHitInfo.hit.pos, a.normalized, this.itemValueWeapon.type);
				}
				Utils.SetLayerRecursively(ProjectileManager.GetProjectile(this.ProjectileID).gameObject, 14);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.bArmed = false;
		}
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x0012CE9D File Offset: 0x0012B09D
	public void DropStuckProjectile()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x0012CEAC File Offset: 0x0012B0AC
	public void OnDestroy()
	{
		if (GameManager.Instance == null || GameManager.Instance.World == null || this.firingEntity == null || base.transform == null || this.itemValueWeapon == null)
		{
			return;
		}
		if (this.stuckInEntity != null)
		{
			NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
			if (this.ProjectileID != -1 && this.firingEntity != null && !this.firingEntity.isEntityRemote)
			{
				Vector3 bellyPosition = this.stuckInEntity.getBellyPosition();
				if (GameManager.Instance.World.IsChunkAreaLoaded(Mathf.CeilToInt(bellyPosition.x), Mathf.CeilToInt(bellyPosition.y), Mathf.CeilToInt(bellyPosition.z)))
				{
					GameManager.Instance.ItemDropServer(new ItemStack(this.itemValueWeapon, 1), bellyPosition, Vector3.zero, this.ProjectileOwnerID, 1000f, false);
					return;
				}
			}
		}
		else if (this.ProjectileID != -1 && this.firingEntity != null && !this.firingEntity.isEntityRemote && GameManager.Instance.World.IsChunkAreaLoaded(Mathf.CeilToInt(base.transform.position.x + Origin.position.x), Mathf.CeilToInt(base.transform.position.y + Origin.position.y), Mathf.CeilToInt(base.transform.position.z + Origin.position.z)))
		{
			GameManager.Instance.ItemDropServer(new ItemStack(this.itemValueWeapon, 1), base.transform.position + Origin.position + Vector3.up, Vector3.zero, this.ProjectileOwnerID, 1000f, false);
		}
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x0012D093 File Offset: 0x0012B293
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<string> getBuffActions()
	{
		return this.itemActionThrownWeapon.BuffActions;
	}

	// Token: 0x040023A2 RID: 9122
	public const int InvalidID = -1;

	// Token: 0x040023A3 RID: 9123
	public int ProjectileID = -1;

	// Token: 0x040023A4 RID: 9124
	public int ProjectileOwnerID;

	// Token: 0x040023A5 RID: 9125
	public ItemActionThrownWeapon itemActionThrownWeapon;

	// Token: 0x040023A6 RID: 9126
	public ItemClass itemWeapon;

	// Token: 0x040023A7 RID: 9127
	public ItemValue itemValueWeapon;

	// Token: 0x040023A8 RID: 9128
	public ItemActionThrowAway.MyInventoryData actionData;

	// Token: 0x040023A9 RID: 9129
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static GameManager gameManager;

	// Token: 0x040023AA RID: 9130
	public Vector3 flyDirection;

	// Token: 0x040023AB RID: 9131
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 idealPosition;

	// Token: 0x040023AC RID: 9132
	public Vector3 velocity;

	// Token: 0x040023AD RID: 9133
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeShotStarted;

	// Token: 0x040023AE RID: 9134
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bArmed;

	// Token: 0x040023AF RID: 9135
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Entity firingEntity;

	// Token: 0x040023B0 RID: 9136
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 previousPosition;

	// Token: 0x040023B1 RID: 9137
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 gravity;

	// Token: 0x040023B2 RID: 9138
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bOnIdealPos;

	// Token: 0x040023B3 RID: 9139
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int hmOverride;

	// Token: 0x040023B4 RID: 9140
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public NavObject NavObject;

	// Token: 0x040023B5 RID: 9141
	public Vector3 FinalPosition;

	// Token: 0x040023B6 RID: 9142
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CollisionParticleController waterCollisionParticles = new CollisionParticleController();

	// Token: 0x040023B7 RID: 9143
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive stuckInEntity;
}
