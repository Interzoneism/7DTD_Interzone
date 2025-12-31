using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200056D RID: 1389
public class ProjectileMoveScript : MonoBehaviour
{
	// Token: 0x06002CF1 RID: 11505 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x0012BA50 File Offset: 0x00129C50
	public void Fire(Vector3 _idealStartPos, Vector3 _dirOrPos, Entity _firingEntity, int _hmOverride = 0, float _radius = 0f, bool _isBallistic = false)
	{
		Transform transform = base.transform;
		this.isOnIdealPos = true;
		this.previousPosition = transform.position + Origin.position;
		if (_idealStartPos.y != 0f)
		{
			this.idealPosition = _idealStartPos;
			this.isOnIdealPos = false;
		}
		this.firingEntity = _firingEntity;
		this.hitMask = ((_hmOverride == 0) ? 80 : _hmOverride);
		this.radius = ((_radius >= 0f) ? _radius : this.itemActionProjectile.collisionRadius);
		if (this.itemActionProjectile.FlyTime < 0f)
		{
			EntityAlive entityAlive = _firingEntity as EntityAlive;
			if (entityAlive != null)
			{
				if (_isBallistic)
				{
					Vector3 a = _dirOrPos - _idealStartPos;
					float magnitude = a.magnitude;
					this.flyDirection = a / magnitude;
					float num = Utils.FastLerp(0.4f, -this.itemActionProjectile.FlyTime, magnitude / this.itemActionProjectile.Velocity);
					this.velocity = a / num;
					this.velocity.y = this.velocity.y + this.itemActionProjectile.Gravity * -0.5f * num;
					Vector3 b = this.velocity * 0.015f;
					this.previousPosition += b;
					this.idealPosition += b;
				}
				else
				{
					this.flyDirection = entityAlive.GetForwardVector();
					this.velocity = this.flyDirection * 2f;
					this.explosionDisabled = true;
				}
			}
			this.gravity = this.itemActionProjectile.Gravity;
		}
		else
		{
			this.flyDirection = _dirOrPos.normalized;
			this.velocity = this.flyDirection * EffectManager.GetValue(PassiveEffects.ProjectileVelocity, this.itemValueLauncher, this.itemActionProjectile.Velocity, _firingEntity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			this.gravity = EffectManager.GetValue(PassiveEffects.ProjectileGravity, this.itemValueLauncher, this.itemActionProjectile.Gravity, _firingEntity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		this.waterCollisionParticles.Init(this.ProjectileOwnerID, this.itemProjectile.MadeOfMaterial.SurfaceCategory, "water", 16);
		OnActivateItemGameObjectReference component = transform.GetComponent<OnActivateItemGameObjectReference>();
		if (component)
		{
			component.ActivateItem(true);
		}
		if (transform.parent)
		{
			transform.SetParent(null);
			Utils.SetLayerRecursively(transform.gameObject, 0);
		}
		transform.position = this.previousPosition - Origin.position;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		this.SetState(ProjectileMoveScript.State.Active);
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x0012BD04 File Offset: 0x00129F04
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetState(ProjectileMoveScript.State _state)
	{
		this.state = _state;
		this.stateTime = 0f;
		if (this.state == ProjectileMoveScript.State.Dead)
		{
			Transform transform = base.transform;
			Transform transform2 = transform.Find("MeshExplode");
			if (transform2)
			{
				transform2.gameObject.SetActive(false);
			}
			Light componentInChildren = transform.GetComponentInChildren<Light>();
			if (componentInChildren)
			{
				componentInChildren.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x0012BD6C File Offset: 0x00129F6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		this.stateTime += fixedDeltaTime;
		if (this.state == ProjectileMoveScript.State.Active)
		{
			GameManager instance = GameManager.Instance;
			if (!instance || instance.World == null)
			{
				return;
			}
			if (this.stateTime > this.itemActionProjectile.FlyTime)
			{
				this.velocity.y = this.velocity.y + this.gravity * fixedDeltaTime;
			}
			Transform transform = base.transform;
			Vector3 vector = transform.position;
			Vector3 b = this.velocity * fixedDeltaTime;
			transform.LookAt(vector + b);
			vector += b;
			if (!this.isOnIdealPos)
			{
				this.idealPosition += b;
				vector = Vector3.Lerp(vector, this.idealPosition - Origin.position, this.stateTime * 5f);
				this.isOnIdealPos = (this.stateTime >= 0.2f);
			}
			transform.position = vector;
			if (this.stateTime >= this.itemActionProjectile.LifeTime)
			{
				this.SetState(ProjectileMoveScript.State.Dead);
			}
		}
		else if (this.state == ProjectileMoveScript.State.Sticky)
		{
			if (this.stateTime >= 180f)
			{
				this.SetState(ProjectileMoveScript.State.StickyDestroyed);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else if (this.state == ProjectileMoveScript.State.Dead && this.stateTime > this.itemActionProjectile.DeadTime)
		{
			this.SetState(ProjectileMoveScript.State.Destroyed);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (this.state == ProjectileMoveScript.State.Active)
		{
			this.checkCollision();
		}
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x0012BEE8 File Offset: 0x0012A0E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void checkCollision()
	{
		GameManager instance = GameManager.Instance;
		if (!instance)
		{
			return;
		}
		World world = instance.World;
		if (world == null)
		{
			return;
		}
		Vector3 a;
		if (this.isOnIdealPos)
		{
			a = base.transform.position + Origin.position;
		}
		else
		{
			a = this.idealPosition;
		}
		Vector3 direction = a - this.previousPosition;
		float magnitude = direction.magnitude;
		if (magnitude < 0.04f)
		{
			return;
		}
		Ray ray = new Ray(this.previousPosition, direction);
		this.previousPosition = a;
		EntityAlive entityAlive = (EntityAlive)this.firingEntity;
		this.waterCollisionParticles.CheckCollision(ray.origin, ray.direction, magnitude, entityAlive ? entityAlive.entityId : -1);
		int num = -1;
		if (entityAlive && entityAlive.emodel)
		{
			num = entityAlive.GetModelLayer();
			entityAlive.SetModelLayer(2, false, null);
		}
		float num2 = this.radius + this.collisionStartBack;
		ray.origin -= ray.direction * num2;
		bool flag = Voxel.Raycast(world, ray, magnitude + num2, -538750997, this.hitMask, this.radius);
		this.collisionStartBack = 0.1f;
		if (num >= 0)
		{
			entityAlive.SetModelLayer(num, false, null);
		}
		if (flag && (GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag) || Voxel.voxelRayHitInfo.tag.StartsWith("E_")))
		{
			if (this.firingEntity && !this.firingEntity.isEntityRemote)
			{
				EntityAlive entityAlive2 = ItemActionAttack.FindHitEntity(Voxel.voxelRayHitInfo) as EntityAlive;
				EntityDrone entityDrone = entityAlive2 as EntityDrone;
				EntityAlive entityAlive3 = this.firingEntity as EntityAlive;
				if (entityDrone && entityAlive3 && entityDrone.isAlly(entityAlive3))
				{
					return;
				}
				entityAlive.MinEventContext.Other = entityAlive2;
				ItemActionAttack.AttackHitInfo attackDetails = new ItemActionAttack.AttackHitInfo
				{
					WeaponTypeTag = ItemActionAttack.RangedTag
				};
				MinEventParams.CachedEventParam.Self = entityAlive;
				MinEventParams.CachedEventParam.Position = Voxel.voxelRayHitInfo.hit.pos;
				MinEventParams.CachedEventParam.ItemValue = this.itemValueProjectile;
				MinEventParams.CachedEventParam.Other = entityAlive2;
				MinEventParams.CachedEventParam.ItemActionData = this.actionData;
				this.itemProjectile.FireEvent(MinEventTypes.onProjectilePreImpact, MinEventParams.CachedEventParam);
				float blockDamage = Utils.FastLerp(1f, this.itemActionProjectile.GetDamageBlock(this.itemValueLauncher, ItemActionAttack.GetBlockHit(world, Voxel.voxelRayHitInfo), entityAlive, 0), this.actionData.strainPercent);
				float entityDamage = Utils.FastLerp(1f, this.itemActionProjectile.GetDamageEntity(this.itemValueLauncher, entityAlive, 0), this.actionData.strainPercent);
				ItemActionAttack.Hit(Voxel.voxelRayHitInfo, this.ProjectileOwnerID, EnumDamageTypes.Piercing, blockDamage, entityDamage, 1f, 1f, EffectManager.GetValue(PassiveEffects.CriticalChance, this.itemValueLauncher, this.itemProjectile.CritChance.Value, entityAlive, null, this.itemProjectile.ItemTags, true, true, true, true, true, 1, true, false), ItemAction.GetDismemberChance(this.actionData, Voxel.voxelRayHitInfo), this.itemProjectile.MadeOfMaterial.SurfaceCategory, this.itemActionProjectile.GetDamageMultiplier(), this.getBuffActions(), attackDetails, 1, this.itemActionProjectile.ActionExp, this.itemActionProjectile.ActionExpBonusMultiplier, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, -1, this.itemValueLauncher);
				if (!entityAlive2)
				{
					entityAlive.FireEvent(MinEventTypes.onSelfPrimaryActionMissEntity, true);
				}
				entityAlive.FireEvent(MinEventTypes.onProjectileImpact, false);
				MinEventParams.CachedEventParam.Self = entityAlive;
				MinEventParams.CachedEventParam.Position = Voxel.voxelRayHitInfo.hit.pos;
				MinEventParams.CachedEventParam.ItemValue = this.itemValueProjectile;
				MinEventParams.CachedEventParam.Other = entityAlive2;
				MinEventParams.CachedEventParam.ItemActionData = this.actionData;
				this.itemProjectile.FireEvent(MinEventTypes.onProjectileImpact, MinEventParams.CachedEventParam);
				if (this.itemActionProjectile.Explosion.ParticleIndex > 0 && !this.explosionDisabled)
				{
					Vector3 vector = Voxel.voxelRayHitInfo.hit.pos - direction.normalized * 0.1f;
					Vector3i vector3i = World.worldToBlockPos(vector);
					if (!world.GetBlock(vector3i).isair)
					{
						BlockFace blockFace;
						vector3i = Voxel.OneVoxelStep(vector3i, vector, -direction.normalized, out vector, out blockFace);
					}
					instance.ExplosionServer(Voxel.voxelRayHitInfo.hit.clrIdx, vector, vector3i, Quaternion.identity, this.itemActionProjectile.Explosion, this.ProjectileOwnerID, 0f, false, this.itemValueLauncher);
					this.SetState(ProjectileMoveScript.State.Dead);
					return;
				}
				if (!(entityAlive2 is EntitySwarm))
				{
					if (!this.itemProjectile.IsSticky)
					{
						this.SetState(ProjectileMoveScript.State.Dead);
						return;
					}
					GameRandom gameRandom = world.GetGameRandom();
					if (GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag))
					{
						if (gameRandom.RandomFloat < EffectManager.GetValue(PassiveEffects.ProjectileStickChance, this.itemValueLauncher, 0.5f, entityAlive, null, this.itemProjectile.ItemTags | FastTags<TagGroup.Global>.Parse(Voxel.voxelRayHitInfo.fmcHit.blockValue.Block.blockMaterial.SurfaceCategory), true, true, true, true, true, 1, true, false))
						{
							this.ProjectileID = ProjectileManager.AddProjectileItem(base.transform, -1, Voxel.voxelRayHitInfo.hit.pos, direction.normalized, this.itemValueProjectile.type);
							this.SetState(ProjectileMoveScript.State.Sticky);
							return;
						}
						instance.SpawnParticleEffectServer(new ParticleEffect("impact_metal_on_wood", Voxel.voxelRayHitInfo.hit.pos, Utils.BlockFaceToRotation(Voxel.voxelRayHitInfo.fmcHit.blockFace), 1f, Color.white, string.Format("{0}hit{1}", Voxel.voxelRayHitInfo.fmcHit.blockValue.Block.blockMaterial.SurfaceCategory, this.itemProjectile.MadeOfMaterial.SurfaceCategory), null), this.firingEntity.entityId, false, false);
						this.SetState(ProjectileMoveScript.State.Dead);
						return;
					}
					else
					{
						if (gameRandom.RandomFloat < EffectManager.GetValue(PassiveEffects.ProjectileStickChance, this.itemValueLauncher, 0.5f, entityAlive, null, this.itemProjectile.ItemTags, true, true, true, true, true, 1, true, false))
						{
							this.ProjectileID = ProjectileManager.AddProjectileItem(base.transform, -1, Voxel.voxelRayHitInfo.hit.pos, direction.normalized, this.itemValueProjectile.type);
							Utils.SetLayerRecursively(ProjectileManager.GetProjectile(this.ProjectileID).gameObject, 14);
							this.SetState(ProjectileMoveScript.State.Sticky);
							return;
						}
						instance.SpawnParticleEffectServer(new ParticleEffect("impact_metal_on_wood", Voxel.voxelRayHitInfo.hit.pos, Utils.BlockFaceToRotation(Voxel.voxelRayHitInfo.fmcHit.blockFace), 1f, Color.white, "bullethitwood", null), this.firingEntity.entityId, false, false);
						this.SetState(ProjectileMoveScript.State.Dead);
						return;
					}
				}
			}
			else
			{
				this.SetState(ProjectileMoveScript.State.Dead);
			}
		}
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x0012C5E8 File Offset: 0x0012A7E8
	public void OnDestroy()
	{
		GameManager instance = GameManager.Instance;
		if (!instance || instance.World == null || !this.firingEntity || this.itemValueProjectile == null)
		{
			return;
		}
		if (this.state == ProjectileMoveScript.State.StickyDestroyed)
		{
			return;
		}
		if (this.ProjectileID != -1 && !this.firingEntity.isEntityRemote)
		{
			Vector3 position = base.transform.position;
			if (instance.World.IsChunkAreaLoaded(Mathf.CeilToInt(position.x + Origin.position.x), Mathf.CeilToInt(position.y + Origin.position.y), Mathf.CeilToInt(position.z + Origin.position.z)))
			{
				instance.ItemDropServer(new ItemStack(this.itemValueProjectile, 1), position + Origin.position, Vector3.zero, this.ProjectileOwnerID, 60f, false);
			}
		}
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x0012C6D0 File Offset: 0x0012A8D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<string> getBuffActions()
	{
		return this.itemActionProjectile.BuffActions;
	}

	// Token: 0x04002384 RID: 9092
	public const int InvalidID = -1;

	// Token: 0x04002385 RID: 9093
	public int ProjectileID = -1;

	// Token: 0x04002386 RID: 9094
	public int ProjectileOwnerID;

	// Token: 0x04002387 RID: 9095
	public ItemActionProjectile itemActionProjectile;

	// Token: 0x04002388 RID: 9096
	public ItemClass itemProjectile;

	// Token: 0x04002389 RID: 9097
	public ItemValue itemValueProjectile;

	// Token: 0x0400238A RID: 9098
	public ItemValue itemValueLauncher;

	// Token: 0x0400238B RID: 9099
	public ItemActionLauncher.ItemActionDataLauncher actionData;

	// Token: 0x0400238C RID: 9100
	public Vector3 FinalPosition;

	// Token: 0x0400238D RID: 9101
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 flyDirection;

	// Token: 0x0400238E RID: 9102
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 idealPosition;

	// Token: 0x0400238F RID: 9103
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 velocity;

	// Token: 0x04002390 RID: 9104
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public ProjectileMoveScript.State state;

	// Token: 0x04002391 RID: 9105
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float stateTime;

	// Token: 0x04002392 RID: 9106
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Entity firingEntity;

	// Token: 0x04002393 RID: 9107
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 previousPosition;

	// Token: 0x04002394 RID: 9108
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float gravity;

	// Token: 0x04002395 RID: 9109
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isOnIdealPos;

	// Token: 0x04002396 RID: 9110
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int hitMask;

	// Token: 0x04002397 RID: 9111
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float radius;

	// Token: 0x04002398 RID: 9112
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float collisionStartBack;

	// Token: 0x04002399 RID: 9113
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool explosionDisabled;

	// Token: 0x0400239A RID: 9114
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CollisionParticleController waterCollisionParticles = new CollisionParticleController();

	// Token: 0x0200056E RID: 1390
	public enum State
	{
		// Token: 0x0400239C RID: 9116
		Idle,
		// Token: 0x0400239D RID: 9117
		Active,
		// Token: 0x0400239E RID: 9118
		Sticky,
		// Token: 0x0400239F RID: 9119
		StickyDestroyed,
		// Token: 0x040023A0 RID: 9120
		Dead,
		// Token: 0x040023A1 RID: 9121
		Destroyed
	}
}
