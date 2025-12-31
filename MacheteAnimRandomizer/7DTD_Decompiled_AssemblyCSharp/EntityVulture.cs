using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200047A RID: 1146
[Preserve]
public class EntityVulture : EntityFlying
{
	// Token: 0x06002552 RID: 9554 RVA: 0x000F0D9C File Offset: 0x000EEF9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component)
		{
			component.center = new Vector3(0f, 0.35f, 0f);
			component.size = new Vector3(0.4f, 0.4f, 0.4f);
		}
		base.Awake();
		this.state = EntityVulture.State.WanderStart;
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000F0DFE File Offset: 0x000EEFFE
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		this.Init();
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000F0E0D File Offset: 0x000EF00D
	public override void InitFromPrefab(int _entityClass)
	{
		base.InitFromPrefab(_entityClass);
		this.Init();
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x000F0E1C File Offset: 0x000EF01C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Init()
	{
		if (this.navigator != null)
		{
			this.navigator.setCanDrown(true);
		}
		this.battleFatigueSeconds = this.rand.RandomRange(30f, 60f);
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x000F0E4D File Offset: 0x000EF04D
	public override void SetSleeper()
	{
		base.SetSleeper();
		this.sorter = new EAISetNearestEntityAsTargetSorter(this);
		base.setHomeArea(new Vector3i(this.position), (int)this.sleeperSightRange + 1);
		this.battleFatigueSeconds = float.MaxValue;
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x000F0E88 File Offset: 0x000EF088
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving))
		{
			this.aiManager.UpdateDebugName();
			return;
		}
		if (GameStats.GetInt(EnumGameStats.GameState) == 2)
		{
			return;
		}
		base.CheckDespawn();
		base.GetEntitySenses().ClearIfExpired();
		if (this.IsSleeperPassive)
		{
			return;
		}
		if (this.IsSleeping)
		{
			float seeDistance = this.GetSeeDistance();
			this.world.GetEntitiesInBounds(typeof(EntityPlayer), BoundsUtils.ExpandBounds(this.boundingBox, seeDistance, seeDistance, seeDistance), EntityVulture.list);
			EntityVulture.list.Sort(this.sorter);
			EntityPlayer entityPlayer = null;
			float num = float.MaxValue;
			if (this.noisePlayer != null && this.noisePlayerVolume >= this.sleeperNoiseToWake)
			{
				entityPlayer = this.noisePlayer;
				num = this.noisePlayerDistance;
			}
			for (int i = 0; i < EntityVulture.list.Count; i++)
			{
				EntityPlayer entityPlayer2 = (EntityPlayer)EntityVulture.list[i];
				if (base.CanSee(entityPlayer2))
				{
					float distance = base.GetDistance(entityPlayer2);
					if (base.GetSleeperDisturbedLevel(distance, entityPlayer2.Stealth.lightLevel) >= 2 && distance < num)
					{
						entityPlayer = entityPlayer2;
						num = distance;
					}
				}
			}
			EntityVulture.list.Clear();
			if (entityPlayer == null)
			{
				return;
			}
			base.ConditionalTriggerSleeperWakeUp();
			base.SetAttackTarget(entityPlayer, 1200);
		}
		bool flag = this.Buffs.HasBuff("buffShocked");
		if (flag)
		{
			this.SetState(EntityVulture.State.Stun);
		}
		else
		{
			EntityAlive revengeTarget = base.GetRevengeTarget();
			if (revengeTarget)
			{
				this.battleDuration = 0f;
				this.isBattleFatigued = false;
				base.SetRevengeTarget(null);
				if (revengeTarget != this.attackTarget && (!this.attackTarget || this.rand.RandomFloat < 0.5f))
				{
					base.SetAttackTarget(revengeTarget, 1200);
				}
			}
			if (this.attackTarget != this.currentTarget)
			{
				this.currentTarget = this.attackTarget;
				if (this.currentTarget)
				{
					this.SetState(EntityVulture.State.Attack);
					this.waypoint = this.position;
					this.moveUpdateDelay = 0;
					this.homeCheckDelay = 400;
				}
				else
				{
					this.SetState(EntityVulture.State.AttackStop);
				}
			}
		}
		float sqrMagnitude = (this.waypoint - this.position).sqrMagnitude;
		this.stateTime += 0.05f;
		int num2;
		switch (this.state)
		{
		case EntityVulture.State.Attack:
			this.battleDuration += 0.05f;
			break;
		case EntityVulture.State.AttackReposition:
			if (sqrMagnitude < 2.25f || this.stateTime >= this.stateMaxTime)
			{
				this.SetState(EntityVulture.State.Attack);
				this.motion *= -0.2f;
				this.motion.y = 0f;
			}
			break;
		case EntityVulture.State.AttackStop:
			this.ClearTarget();
			this.SetState(EntityVulture.State.WanderStart);
			break;
		case EntityVulture.State.Home:
			if (sqrMagnitude < 4f || this.stateTime > 30f)
			{
				this.SetState(EntityVulture.State.WanderStart);
			}
			else
			{
				num2 = this.homeSeekDelay - 1;
				this.homeSeekDelay = num2;
				if (num2 <= 0)
				{
					this.homeSeekDelay = 40;
					int minXZ = 10;
					if (this.stateTime > 20f)
					{
						minXZ = -20;
					}
					int maximumHomeDistance = base.getMaximumHomeDistance();
					Vector3 vector = RandomPositionGenerator.CalcTowards(this, minXZ, 30, maximumHomeDistance / 2, base.getHomePosition().position.ToVector3());
					if (!vector.Equals(Vector3.zero))
					{
						this.waypoint = vector;
						this.AdjustWaypoint();
					}
				}
			}
			break;
		case EntityVulture.State.Stun:
		{
			Animator componentInChildren = this.ModelTransform.GetComponentInChildren<Animator>();
			if (flag)
			{
				this.motion = this.rand.RandomOnUnitSphere * -0.075f;
				this.motion.y = this.motion.y + -0.060000002f;
				if (componentInChildren)
				{
					componentInChildren.enabled = false;
				}
				return;
			}
			if (componentInChildren)
			{
				componentInChildren.enabled = true;
			}
			this.SetState(EntityVulture.State.WanderStart);
			break;
		}
		case EntityVulture.State.WanderStart:
			this.homeCheckDelay = 60;
			if (!base.isWithinHomeDistanceCurrentPosition())
			{
				this.StartHome(base.getHomePosition().position.ToVector3());
			}
			else
			{
				this.SetState(EntityVulture.State.Wander);
				this.isCircling = (!this.IsSleeper && this.rand.RandomFloat < 0.4f);
				float num3 = this.position.y;
				RaycastHit raycastHit;
				if (Physics.Raycast(this.position - Origin.position, Vector3.down, out raycastHit, 999f, 65536))
				{
					float num4 = this.rand.RandomRange(this.wanderHeightRange.x, this.wanderHeightRange.y);
					if (this.IsSleeper)
					{
						num4 *= 0.4f;
					}
					num3 += -raycastHit.distance + num4;
				}
				else
				{
					this.isCircling = false;
				}
				bool flag2 = false;
				EntityPlayer entityPlayer3 = null;
				if (!this.isBattleFatigued)
				{
					entityPlayer3 = this.world.GetClosestPlayerSeen(this, 80f, 1f);
					if (entityPlayer3 && base.GetDistanceSq(entityPlayer3) > 400f)
					{
						flag2 = true;
					}
				}
				if (this.isCircling)
				{
					this.wanderChangeDelay = 120;
					Vector3 right = base.transform.right;
					right.y = 0f;
					this.circleReverseScale = 1f;
					if (this.rand.RandomFloat < 0.5f)
					{
						this.circleReverseScale = -1f;
						right.x = -right.x;
						right.z = -right.z;
					}
					this.circleCenter = this.position + right * (3f + this.rand.RandomFloat * 7f);
					this.circleCenter.y = num3;
					if (flag2)
					{
						this.circleCenter.x = this.circleCenter.x * 0.6f + entityPlayer3.position.x * 0.4f;
						this.circleCenter.z = this.circleCenter.z * 0.6f + entityPlayer3.position.z * 0.4f;
					}
				}
				else
				{
					this.wanderChangeDelay = 400;
					this.waypoint = this.position;
					this.waypoint.x = this.waypoint.x + (this.rand.RandomFloat * 16f - 8f);
					this.waypoint.y = num3;
					this.waypoint.z = this.waypoint.z + (this.rand.RandomFloat * 16f - 8f);
					if (flag2)
					{
						this.waypoint.x = this.waypoint.x * 0.6f + entityPlayer3.position.x * 0.4f;
						this.waypoint.z = this.waypoint.z * 0.6f + entityPlayer3.position.z * 0.4f;
					}
					this.AdjustWaypoint();
				}
			}
			break;
		case EntityVulture.State.Wander:
			if (this.isBattleFatigued)
			{
				this.battleDuration -= 0.05f;
				if (this.battleDuration <= 0f)
				{
					this.isBattleFatigued = false;
				}
			}
			num2 = this.wanderChangeDelay - 1;
			this.wanderChangeDelay = num2;
			if (num2 <= 0)
			{
				this.SetState(EntityVulture.State.WanderStart);
			}
			if (this.isCircling)
			{
				Vector3 vector2 = this.circleCenter - this.position;
				float x = vector2.x;
				vector2.x = -vector2.z * this.circleReverseScale;
				vector2.z = x * this.circleReverseScale;
				vector2.y = 0f;
				this.waypoint = this.position + vector2;
			}
			else if (sqrMagnitude < 1f)
			{
				this.SetState(EntityVulture.State.WanderStart);
			}
			num2 = this.targetSwitchDelay - 1;
			this.targetSwitchDelay = num2;
			if (num2 <= 0)
			{
				this.targetSwitchDelay = 40;
				if (this.IsSleeper || this.rand.RandomFloat >= 0.5f)
				{
					EntityPlayer entityPlayer4 = this.FindTarget();
					if (entityPlayer4)
					{
						base.SetAttackTarget(entityPlayer4, 1200);
					}
				}
			}
			break;
		}
		if (this.state != EntityVulture.State.Home)
		{
			num2 = this.homeCheckDelay - 1;
			this.homeCheckDelay = num2;
			if (num2 <= 0)
			{
				this.homeCheckDelay = 60;
				if (!base.isWithinHomeDistanceCurrentPosition())
				{
					this.SetState(EntityVulture.State.AttackStop);
				}
			}
		}
		num2 = this.moveUpdateDelay - 1;
		this.moveUpdateDelay = num2;
		if (num2 <= 0)
		{
			this.moveUpdateDelay = 4 + this.rand.RandomRange(5);
			if (this.currentTarget && this.state == EntityVulture.State.Attack)
			{
				this.waypoint = this.currentTarget.getHeadPosition();
				this.waypoint.y = this.waypoint.y + -0.1f;
				if (this.currentTarget.AttachedToEntity)
				{
					this.waypoint += this.currentTarget.GetVelocityPerSecond() * 0.3f;
				}
				else
				{
					this.waypoint += this.currentTarget.GetVelocityPerSecond() * 0.1f;
				}
				Vector3 a = this.waypoint - this.position;
				a.y = 0f;
				a.Normalize();
				this.waypoint += a * -0.6f;
			}
			float num5;
			if (!this.IsCourseTraversable(this.waypoint, out num5))
			{
				this.waypoint.y = this.waypoint.y + 2f;
				if (this.state == EntityVulture.State.Attack)
				{
					if (this.rand.RandomFloat < 0.1f)
					{
						this.StartAttackReposition();
					}
				}
				else if (this.state != EntityVulture.State.Home && this.state != EntityVulture.State.AttackReposition)
				{
					this.SetState(EntityVulture.State.WanderStart);
				}
			}
		}
		Vector3 a2 = this.waypoint - this.position;
		float magnitude = a2.magnitude;
		Vector3 vector3 = a2 * (1f / magnitude);
		this.glidingPercent = 0f;
		if (vector3.y > 0.57f)
		{
			this.accel = 0.35f;
		}
		else if (vector3.y < -0.34f)
		{
			this.accel = 0.95f;
			this.glidingPercent = 1f;
		}
		else
		{
			this.accel = 0.55f;
			if (this.state == EntityVulture.State.Home || this.state == EntityVulture.State.Wander)
			{
				this.accel = 0.8f;
				if (this.isCircling)
				{
					this.glidingPercent = 1f;
				}
			}
		}
		if (this.attackDelay > 0)
		{
			this.glidingPercent = 0f;
		}
		if (this.currentTarget && this.currentTarget.AttachedToEntity && !this.ignoreTargetAttached)
		{
			if (this.IsBloodMoon && this.accel > 0.5f)
			{
				this.accel = 2.5f;
			}
			this.accel *= this.moveSpeedAggro;
		}
		else
		{
			this.accel *= this.moveSpeed;
		}
		this.motion = this.motion * 0.9f + vector3 * (this.accel * 0.1f);
		if (this.emodel.avatarController)
		{
			this.glidingCurrentPercent = Mathf.MoveTowards(this.glidingCurrentPercent, this.glidingPercent, 0.060000002f);
			this.emodel.avatarController.UpdateFloat("Gliding", this.glidingCurrentPercent, true);
		}
		if (this.attackDelay > 0)
		{
			this.attackDelay--;
		}
		if (this.attack2Delay > 0)
		{
			this.attack2Delay--;
		}
		float num6 = Mathf.Atan2(this.motion.x * this.motionReverseScale, this.motion.z * this.motionReverseScale) * 57.29578f;
		if (this.currentTarget)
		{
			num2 = this.targetSwitchDelay - 1;
			this.targetSwitchDelay = num2;
			if (num2 <= 0)
			{
				this.targetSwitchDelay = 60;
				if (this.state != EntityVulture.State.AttackStop)
				{
					EntityPlayer entityPlayer5 = this.FindTarget();
					if (entityPlayer5 && entityPlayer5 != this.attackTarget)
					{
						base.SetAttackTarget(entityPlayer5, 400);
					}
				}
				float num7 = this.currentTarget.AttachedToEntity ? 0.1f : 0.25f;
				if (this.state != EntityVulture.State.AttackReposition && this.rand.RandomFloat < num7)
				{
					this.StartAttackReposition();
				}
			}
		}
		if (this.currentTarget)
		{
			Vector3 vector4 = this.currentTarget.getHeadPosition();
			vector4 += this.currentTarget.GetVelocityPerSecond() * 0.1f;
			Vector3 vector5 = vector4 - this.position;
			float sqrMagnitude2 = vector5.sqrMagnitude;
			if ((sqrMagnitude2 > 6400f && !this.IsBloodMoon) || this.currentTarget.IsDead())
			{
				this.SetState(EntityVulture.State.AttackStop);
			}
			else if (this.state != EntityVulture.State.AttackReposition)
			{
				if (sqrMagnitude2 < 4f)
				{
					num6 = Mathf.Atan2(vector5.x, vector5.z) * 57.29578f;
				}
				if (this.attackDelay <= 0 && !this.isAttack2On)
				{
					if (sqrMagnitude2 < 0.80999994f && this.position.y >= this.currentTarget.position.y && this.position.y < vector4.y + 0.1f)
					{
						this.AttackAndAdjust(false);
					}
					else if (this.checkBlockedDelay > 0)
					{
						this.checkBlockedDelay--;
					}
					else
					{
						this.checkBlockedDelay = 6;
						Vector3 normalized = vector5.normalized;
						Ray ray = new Ray(this.position + new Vector3(0f, 0.22f, 0f) - normalized * 0.13f, normalized);
						if (Voxel.Raycast(this.world, ray, 0.83f, 1082195968, 128, 0.13f))
						{
							this.AttackAndAdjust(true);
						}
					}
				}
				bool flag3 = false;
				ItemActionVomit.ItemActionDataVomit itemActionDataVomit = this.inventory.holdingItemData.actionData[1] as ItemActionVomit.ItemActionDataVomit;
				if (itemActionDataVomit != null && this.attack2Delay <= 0 && sqrMagnitude2 >= 9f)
				{
					float range = ((ItemActionRanged)this.inventory.holdingItem.Actions[1]).GetRange(itemActionDataVomit);
					if (sqrMagnitude2 < range * range && Utils.FastAbs(Utils.DeltaAngle(num6, this.rotation.y)) < 20f && Utils.FastAbs(Vector3.SignedAngle(vector5, base.transform.forward, Vector3.right)) < 25f)
					{
						flag3 = true;
					}
				}
				if (!this.isAttack2On && flag3)
				{
					this.isAttack2On = true;
					itemActionDataVomit.muzzle = this.emodel.GetHeadTransform();
					itemActionDataVomit.numWarningsPlayed = 999;
				}
				if (this.isAttack2On)
				{
					if (!flag3)
					{
						this.isAttack2On = false;
					}
					else
					{
						this.motion *= 0.7f;
						base.SetLookPosition(vector4);
						this.UseHoldingItem(1, false);
						if (!itemActionDataVomit.isActive)
						{
							this.isAttack2On = false;
						}
					}
					if (!this.isAttack2On)
					{
						if (itemActionDataVomit.numVomits > 0)
						{
							this.StartAttackReposition();
						}
						this.UseHoldingItem(1, true);
						this.attack2Delay = 60;
						base.SetLookPosition(Vector3.zero);
					}
				}
			}
		}
		float magnitude2 = this.motion.magnitude;
		if (magnitude2 < 0.02f)
		{
			this.motion *= 1f / magnitude2 * 0.02f;
		}
		base.SeekYaw(num6, 0f, 20f);
		this.aiManager.UpdateDebugName();
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x000F1E6C File Offset: 0x000F006C
	public override string MakeDebugNameInfo()
	{
		return string.Format("\n{0} {1}\nWaypoint {2}\nTarget {3}, AtkDelay {4}, BtlTime {5}\nSpeed {6}, Motion {7}, Accel {8}", new object[]
		{
			this.state.ToStringCached<EntityVulture.State>(),
			this.stateTime.ToCultureInvariantString("0.00"),
			this.waypoint.ToCultureInvariantString(),
			this.currentTarget ? this.currentTarget.name : "",
			this.attackDelay,
			this.battleDuration.ToCultureInvariantString("0.00"),
			this.motion.magnitude.ToCultureInvariantString("0.000"),
			this.motion.ToCultureInvariantString("0.000"),
			this.accel.ToCultureInvariantString("0.000")
		});
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x000F1F3A File Offset: 0x000F013A
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetState(EntityVulture.State newState)
	{
		this.state = newState;
		this.stateTime = 0f;
		this.motionReverseScale = 1f;
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000F1F5C File Offset: 0x000F015C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AdjustWaypoint()
	{
		int num = 255;
		Vector3i pos = new Vector3i(this.waypoint);
		while (!this.world.GetBlock(pos).isair && --num >= 0)
		{
			this.waypoint.y = this.waypoint.y + 1f;
			pos.y++;
		}
		this.waypoint.y = Mathf.Min(this.waypoint.y, 250f);
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x000F1FDC File Offset: 0x000F01DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartAttackReposition()
	{
		if (!this.IsBloodMoon && this.battleDuration >= this.battleFatigueSeconds)
		{
			this.ClearTarget();
			this.battleDuration = this.rand.RandomRange(80f, 180f);
			this.isBattleFatigued = true;
			this.SetState(EntityVulture.State.Wander);
			return;
		}
		this.SetState(EntityVulture.State.AttackReposition);
		this.stateMaxTime = this.rand.RandomRange(0.8f, 5f);
		this.attackCount = 0;
		this.waypoint = this.position;
		this.waypoint.x = this.waypoint.x + (this.rand.RandomFloat * 8f - 4f);
		this.waypoint.y = this.waypoint.y + (this.rand.RandomFloat * 4f + 3f);
		this.waypoint.z = this.waypoint.z + (this.rand.RandomFloat * 8f - 4f);
		this.moveUpdateDelay = 0;
		this.motion = -this.motion;
		if (this.rand.RandomFloat < 0.5f)
		{
			this.motionReverseScale = -1f;
			this.motion.y = 0.2f;
		}
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000F211B File Offset: 0x000F031B
	[PublicizedFrom(EAccessModifier.Protected)]
	public void StartHome(Vector3 _homePos)
	{
		this.SetState(EntityVulture.State.Home);
		this.homeSeekDelay = 0;
		this.waypoint = _homePos;
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000F2132 File Offset: 0x000F0332
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearTarget()
	{
		base.SetAttackTarget(null, 0);
		base.SetRevengeTarget(null);
		this.currentTarget = null;
	}

	// Token: 0x0600255E RID: 9566 RVA: 0x000F214C File Offset: 0x000F034C
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer FindTarget()
	{
		EntityPlayer entityPlayer;
		if (this.IsBloodMoon)
		{
			entityPlayer = this.world.GetClosestPlayerSeen(this, -1f, 0f);
			if (!entityPlayer)
			{
				entityPlayer = this.world.GetClosestPlayer(this, -1f, false);
			}
			return entityPlayer;
		}
		float lightMin = 26f;
		entityPlayer = this.world.GetClosestPlayerSeen(this, 80f, lightMin);
		if (!entityPlayer || entityPlayer.inWaterPercent >= 0.6f)
		{
			entityPlayer = this.noisePlayer;
		}
		if (entityPlayer)
		{
			if (this.isBattleFatigued)
			{
				return null;
			}
			float num = (float)entityPlayer.Health / entityPlayer.Stats.Health.ModifiedMax;
			if (this.IsSleeper || num <= this.targetAttackHealthPercent)
			{
				return entityPlayer;
			}
		}
		return null;
	}

	// Token: 0x0600255F RID: 9567 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fallHitGround(float _v, Vector3 _fallMotion)
	{
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000F2209 File Offset: 0x000F0409
	public override float GetEyeHeight()
	{
		return 0.3f;
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000D906F File Offset: 0x000D726F
	public override Vector3 GetLookVector()
	{
		if (this.lookAtPosition.Equals(Vector3.zero))
		{
			return base.GetLookVector();
		}
		return this.lookAtPosition - this.getHeadPosition();
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000F2210 File Offset: 0x000F0410
	public override bool CanDamageEntity(int _sourceEntityId)
	{
		Entity entity = this.world.GetEntity(_sourceEntityId);
		return !entity || entity.entityClass != this.entityClass;
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000F2244 File Offset: 0x000F0444
	[PublicizedFrom(EAccessModifier.Private)]
	public void AttackAndAdjust(bool isBlock)
	{
		if (this.UseHoldingItem(0, false))
		{
			this.UseHoldingItem(0, true);
			this.attackDelay = 18;
			this.isCircling = false;
			if (this.currentTarget.AttachedToEntity)
			{
				this.motion *= 0.7f;
			}
			else
			{
				this.motion *= 0.6f;
			}
			int num = this.attackCount + 1;
			this.attackCount = num;
			if (num >= 5 || this.rand.RandomFloat < 0.25f)
			{
				this.StartAttackReposition();
			}
		}
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000F22E4 File Offset: 0x000F04E4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsCourseTraversable(Vector3 _pos, out float _distance)
	{
		float num = _pos.x - this.position.x;
		float num2 = _pos.y - this.position.y;
		float num3 = _pos.z - this.position.z;
		_distance = Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
		if (_distance < 1.5f)
		{
			return true;
		}
		num /= _distance;
		num2 /= _distance;
		num3 /= _distance;
		Bounds boundingBox = this.boundingBox;
		this.collBB.Clear();
		int num4 = 1;
		while ((float)num4 < _distance - 1f)
		{
			boundingBox.center += new Vector3(num, num2, num3);
			this.world.GetCollidingBounds(this, boundingBox, this.collBB);
			if (this.collBB.Count > 0)
			{
				return false;
			}
			num4++;
		}
		return true;
	}

	// Token: 0x04001C44 RID: 7236
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cFlyingMinimumSpeed = 0.02f;

	// Token: 0x04001C45 RID: 7237
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTargetDistanceClose = 0.9f;

	// Token: 0x04001C46 RID: 7238
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTargetDistanceMax = 80f;

	// Token: 0x04001C47 RID: 7239
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTargetAttackOffsetY = -0.1f;

	// Token: 0x04001C48 RID: 7240
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cVomitMinRange = 3f;

	// Token: 0x04001C49 RID: 7241
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cAttackDelay = 18;

	// Token: 0x04001C4A RID: 7242
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cCollisionMask = 1082195968;

	// Token: 0x04001C4B RID: 7243
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueMin = 30f;

	// Token: 0x04001C4C RID: 7244
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueMax = 60f;

	// Token: 0x04001C4D RID: 7245
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueCooldownMin = 80f;

	// Token: 0x04001C4E RID: 7246
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBattleFatigueCooldownMax = 180f;

	// Token: 0x04001C4F RID: 7247
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int moveUpdateDelay;

	// Token: 0x04001C50 RID: 7248
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float motionReverseScale = 1f;

	// Token: 0x04001C51 RID: 7249
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 waypoint;

	// Token: 0x04001C52 RID: 7250
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCircling;

	// Token: 0x04001C53 RID: 7251
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 circleCenter;

	// Token: 0x04001C54 RID: 7252
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float circleReverseScale;

	// Token: 0x04001C55 RID: 7253
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float glidingCurrentPercent;

	// Token: 0x04001C56 RID: 7254
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float glidingPercent;

	// Token: 0x04001C57 RID: 7255
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float accel;

	// Token: 0x04001C58 RID: 7256
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector2 wanderHeightRange = new Vector2(10f, 30f);

	// Token: 0x04001C59 RID: 7257
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive currentTarget;

	// Token: 0x04001C5A RID: 7258
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float targetAttackHealthPercent = 0.8f;

	// Token: 0x04001C5B RID: 7259
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool ignoreTargetAttached;

	// Token: 0x04001C5C RID: 7260
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int targetSwitchDelay;

	// Token: 0x04001C5D RID: 7261
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int homeCheckDelay;

	// Token: 0x04001C5E RID: 7262
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int homeSeekDelay;

	// Token: 0x04001C5F RID: 7263
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int wanderChangeDelay;

	// Token: 0x04001C60 RID: 7264
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int checkBlockedDelay;

	// Token: 0x04001C61 RID: 7265
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float battleDuration;

	// Token: 0x04001C62 RID: 7266
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float battleFatigueSeconds;

	// Token: 0x04001C63 RID: 7267
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isBattleFatigued;

	// Token: 0x04001C64 RID: 7268
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackDelay;

	// Token: 0x04001C65 RID: 7269
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackCount;

	// Token: 0x04001C66 RID: 7270
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attack2Delay;

	// Token: 0x04001C67 RID: 7271
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isAttack2On;

	// Token: 0x04001C68 RID: 7272
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EAISetNearestEntityAsTargetSorter sorter;

	// Token: 0x04001C69 RID: 7273
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Entity> list = new List<Entity>();

	// Token: 0x04001C6A RID: 7274
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVulture.State state;

	// Token: 0x04001C6B RID: 7275
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stateTime;

	// Token: 0x04001C6C RID: 7276
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stateMaxTime;

	// Token: 0x04001C6D RID: 7277
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Bounds> collBB = new List<Bounds>();

	// Token: 0x0200047B RID: 1147
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum State
	{
		// Token: 0x04001C6F RID: 7279
		Attack,
		// Token: 0x04001C70 RID: 7280
		AttackReposition,
		// Token: 0x04001C71 RID: 7281
		AttackStop,
		// Token: 0x04001C72 RID: 7282
		Home,
		// Token: 0x04001C73 RID: 7283
		Stun,
		// Token: 0x04001C74 RID: 7284
		WanderStart,
		// Token: 0x04001C75 RID: 7285
		Wander
	}
}
