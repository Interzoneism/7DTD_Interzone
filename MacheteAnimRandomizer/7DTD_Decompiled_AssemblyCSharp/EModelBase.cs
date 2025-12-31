using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Assets.DuckType.Jiggle;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

// Token: 0x02000495 RID: 1173
[Preserve]
public abstract class EModelBase : MonoBehaviour
{
	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x0600263E RID: 9790 RVA: 0x000F7AE6 File Offset: 0x000F5CE6
	// (set) Token: 0x0600263F RID: 9791 RVA: 0x000F7AEE File Offset: 0x000F5CEE
	public bool IsFPV { get; set; }

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x06002640 RID: 9792 RVA: 0x000F7AF7 File Offset: 0x000F5CF7
	public virtual Transform NeckTransform
	{
		get
		{
			return this.neckTransform;
		}
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x000F7B00 File Offset: 0x000F5D00
	public virtual void Init(World _world, Entity _entity)
	{
		EntityClass entityClass = EntityClass.list[_entity.entityClass];
		this.visible = false;
		this.entity = _entity;
		this.ragdollChance = entityClass.RagdollOnDeathChance;
		this.bHasRagdoll = entityClass.HasRagdoll;
		this.modelTransformParent = EModelBase.FindModel(base.transform);
		this.createModel(_world, entityClass);
		bool flag = this.entity.RootMotion || this.bHasRagdoll;
		if (GameManager.IsDedicatedServer && !flag)
		{
			if (entityClass.Properties.GetString(EntityClass.PropAvatarController).Length > 0)
			{
				this.avatarController = base.gameObject.AddComponent<AvatarControllerDummy>();
				Animator[] componentsInChildren = base.transform.GetComponentsInChildren<Animator>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
		}
		else
		{
			this.createAvatarController(entityClass);
			if (GameManager.IsDedicatedServer && this.avatarController && flag)
			{
				this.avatarController.SetVisible(true);
			}
		}
		this.InitCommon();
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000F7C00 File Offset: 0x000F5E00
	public virtual void InitFromPrefab(World _world, Entity _entity)
	{
		EntityClass entityClass = EntityClass.list[_entity.entityClass];
		this.entity = _entity;
		this.ragdollChance = entityClass.RagdollOnDeathChance;
		this.bHasRagdoll = entityClass.HasRagdoll;
		this.modelTransformParent = EModelBase.FindModel(base.transform);
		this.modelName = entityClass.mesh.name;
		bool flag = this.entity.RootMotion || this.bHasRagdoll;
		if (GameManager.IsDedicatedServer && !flag)
		{
			this.avatarController = base.transform.gameObject.GetComponent<AvatarControllerDummy>();
			Animator[] componentsInChildren = base.transform.GetComponentsInChildren<Animator>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		else
		{
			if (this.entity is EntityPlayerLocal && entityClass.Properties.Values.ContainsKey(EntityClass.PropLocalAvatarController))
			{
				this.avatarController = (base.transform.gameObject.GetComponent(Type.GetType(entityClass.Properties.Values[EntityClass.PropLocalAvatarController])) as AvatarController);
			}
			else if (entityClass.Properties.Values.ContainsKey(EntityClass.PropAvatarController))
			{
				this.avatarController = (base.transform.gameObject.GetComponent(Type.GetType(entityClass.Properties.Values[EntityClass.PropAvatarController])) as AvatarController);
			}
			if (GameManager.IsDedicatedServer && this.avatarController != null && flag)
			{
				this.avatarController.SetVisible(true);
			}
		}
		this.InitCommon();
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000F7D94 File Offset: 0x000F5F94
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitCommon()
	{
		this.LookAtInit();
		if (this.modelTransformParent)
		{
			this.SwitchModelAndView(false, EntityClass.list[this.entity.entityClass].bIsMale);
		}
		else
		{
			this.modelTransformParent = base.transform;
			this.headTransform = base.transform;
		}
		this.InitRigidBodies();
		this.JiggleInit();
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x000F7DFC File Offset: 0x000F5FFC
	public void InitRigidBodies()
	{
		if (!this.bipedRootTransform)
		{
			return;
		}
		List<Rigidbody> list = new List<Rigidbody>();
		this.bipedRootTransform.GetComponentsInChildren<Rigidbody>(list);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i].gameObject.CompareTag("AudioRigidBody"))
			{
				list.RemoveAt(i);
			}
		}
		float num = EntityClass.list[this.entity.entityClass].MassKg / (float)list.Count;
		if (list.Count == 11)
		{
			num *= 1.1224489f;
			int j = 0;
			while (j < list.Count)
			{
				Rigidbody rigidbody = list[j];
				float num2 = 1f;
				bool flag = false;
				string name = rigidbody.name;
				uint num3 = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num3 <= 2248339218U)
				{
					if (num3 <= 977255260U)
					{
						if (num3 != 819656463U)
						{
							if (num3 != 928972447U)
							{
								if (num3 != 977255260U)
								{
									goto IL_2C9;
								}
								if (!(name == "RightUpLeg"))
								{
									goto IL_2C9;
								}
							}
							else
							{
								if (!(name == "RightForeArm"))
								{
									goto IL_2C9;
								}
								goto IL_295;
							}
						}
						else if (!(name == "LeftUpLeg"))
						{
							goto IL_2C9;
						}
						num2 = 1f;
					}
					else
					{
						if (num3 != 1413449684U)
						{
							if (num3 != 2231561599U)
							{
								if (num3 != 2248339218U)
								{
									goto IL_2C9;
								}
								if (!(name == "Spine2"))
								{
									goto IL_2C9;
								}
							}
							else if (!(name == "Spine1"))
							{
								goto IL_2C9;
							}
						}
						else if (!(name == "Spine"))
						{
							goto IL_2C9;
						}
						num2 = 2f;
						flag = true;
					}
				}
				else
				{
					if (num3 <= 2996251363U)
					{
						if (num3 != 2449536012U)
						{
							if (num3 != 2925602325U)
							{
								if (num3 != 2996251363U)
								{
									goto IL_2C9;
								}
								if (!(name == "Head"))
								{
									goto IL_2C9;
								}
								num2 = 0.8f;
								goto IL_2C9;
							}
							else if (!(name == "RightArm"))
							{
								goto IL_2C9;
							}
						}
						else
						{
							if (!(name == "LeftLeg"))
							{
								goto IL_2C9;
							}
							goto IL_2BF;
						}
					}
					else if (num3 <= 3119934960U)
					{
						if (num3 != 3001187991U)
						{
							if (num3 != 3119934960U)
							{
								goto IL_2C9;
							}
							if (!(name == "LeftForeArm"))
							{
								goto IL_2C9;
							}
							goto IL_295;
						}
						else
						{
							if (!(name == "RightLeg"))
							{
								goto IL_2C9;
							}
							goto IL_2BF;
						}
					}
					else if (num3 != 3537553655U)
					{
						if (num3 != 4018002826U)
						{
							goto IL_2C9;
						}
						if (!(name == "LeftArm"))
						{
							goto IL_2C9;
						}
					}
					else
					{
						if (!(name == "Hips"))
						{
							goto IL_2C9;
						}
						num2 = 2f;
						goto IL_2C9;
					}
					num2 = 0.5f;
					goto IL_2C9;
					IL_2BF:
					num2 = 0.5f;
					flag = true;
				}
				IL_2C9:
				rigidbody.mass = num * num2;
				if (flag && !this.entity.isEntityRemote)
				{
					rigidbody.gameObject.GetOrAddComponent<CollisionCallForward>().Entity = this.entity;
				}
				if (rigidbody.drag <= 0f)
				{
					rigidbody.drag = 0.25f;
				}
				j++;
				continue;
				IL_295:
				num2 = 0.5f;
				flag = true;
				goto IL_2C9;
			}
			return;
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].mass = num;
		}
	}

	// Token: 0x06002645 RID: 9797 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void PostInit()
	{
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x000F8154 File Offset: 0x000F6354
	public static Transform FindModel(Transform _t)
	{
		Transform transform = _t.Find("Graphics/Model");
		if (transform)
		{
			return transform;
		}
		return _t;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnUnload()
	{
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000F8178 File Offset: 0x000F6378
	public void OriginChanged(Vector3 _deltaPos)
	{
		this.ragdollPosePelvisPos += _deltaPos;
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x000F818C File Offset: 0x000F638C
	public virtual Vector3 GetHeadPosition()
	{
		if (this.headTransform == null)
		{
			return this.entity.position + Vector3.up * this.entity.GetEyeHeight();
		}
		return this.headTransform.position + Origin.position;
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000F81E2 File Offset: 0x000F63E2
	public virtual Vector3 GetNavObjectPosition()
	{
		if (this.NavObjectTransform == null)
		{
			return this.GetHeadPosition();
		}
		return this.NavObjectTransform.position + Origin.position;
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x000F8210 File Offset: 0x000F6410
	public virtual Vector3 GetHipPosition()
	{
		if (this.bipedPelvisTransform == null)
		{
			return this.entity.position + Vector3.up * (this.entity.height * 0.5f);
		}
		return this.bipedPelvisTransform.position + Origin.position;
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000F826C File Offset: 0x000F646C
	public virtual Vector3 GetChestPosition()
	{
		if (this.bipedPelvisTransform == null || this.headTransform == null)
		{
			return Vector3.Lerp(this.GetHipPosition(), this.GetHeadPosition(), 0.4f);
		}
		return Vector3.Lerp(this.bipedPelvisTransform.position, this.headTransform.position, 0.6f) + Origin.position;
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000F82D8 File Offset: 0x000F64D8
	public virtual Vector3 GetBellyPosition()
	{
		if (this.bipedPelvisTransform == null || this.headTransform == null)
		{
			return Vector3.Lerp(this.GetHipPosition(), this.GetHeadPosition(), 0.2f);
		}
		return Vector3.Lerp(this.bipedPelvisTransform.position, this.headTransform.position, 0.2f) + Origin.position;
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x000F8344 File Offset: 0x000F6544
	public IKController AddIKController()
	{
		IKController ikcontroller = null;
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			Animator componentInChildren = transform.GetComponentInChildren<Animator>();
			if (componentInChildren)
			{
				ikcontroller = componentInChildren.GetComponent<IKController>();
				if (!ikcontroller)
				{
					ikcontroller = componentInChildren.gameObject.AddComponent<IKController>();
				}
			}
		}
		return ikcontroller;
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x000F8390 File Offset: 0x000F6590
	public void RemoveIKController()
	{
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			IKController componentInChildren = transform.GetComponentInChildren<IKController>();
			if (componentInChildren)
			{
				componentInChildren.Cleanup();
				UnityEngine.Object.Destroy(componentInChildren);
			}
		}
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000F83C8 File Offset: 0x000F65C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CrouchUpdate(EntityAlive _ea)
	{
		Transform transform = this.neckParentTransform;
		Transform parent = transform.parent;
		float crouchBendPer = _ea.crouchBendPer;
		Quaternion rhs = Quaternion.Euler(28f * crouchBendPer, 0f, 0f);
		transform.localRotation *= rhs;
		parent.localRotation *= rhs;
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000F8424 File Offset: 0x000F6624
	[PublicizedFrom(EAccessModifier.Protected)]
	public void LookAtInit()
	{
		EntityClass entityClass = EntityClass.list[this.entity.entityClass];
		this.lookAtMaxAngle = entityClass.LookAtAngle;
		this.lookAtEnabled = (this.lookAtMaxAngle > 0f);
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000F8466 File Offset: 0x000F6666
	public void ClearLookAt()
	{
		this.lookAtBlendPerTarget = 0f;
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000F8473 File Offset: 0x000F6673
	public void SetLookAt(Vector3 _pos)
	{
		this.lookAtPos = _pos;
		this.lookAtBlendPerTarget = this.lookAtFullBlendPer;
		this.lookAtIsPos = true;
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000F848F File Offset: 0x000F668F
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetLookAt()
	{
		this.lookAtBlendPer = 0f;
		this.lookAtBlendPerTarget = 0f;
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000F84A8 File Offset: 0x000F66A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LookAtUpdate(EntityAlive e)
	{
		EnumEntityStunType currentStun = e.bodyDamage.CurrentStun;
		float deltaTime = Time.deltaTime;
		if (e.IsDead() || (currentStun != EnumEntityStunType.None && currentStun != EnumEntityStunType.Getup))
		{
			this.lookAtBlendPerTarget = 0f;
		}
		else if (!this.lookAtIsPos)
		{
			this.lookAtBlendPerTarget -= deltaTime;
			EntityAlive attackTargetLocal = e.GetAttackTargetLocal();
			if (attackTargetLocal && e.CanSee(attackTargetLocal))
			{
				this.lookAtPos = attackTargetLocal.getHeadPosition();
				this.lookAtBlendPerTarget = this.lookAtFullBlendPer;
			}
		}
		if (this.lookAtBlendPer <= 0f && this.lookAtBlendPerTarget <= 0f)
		{
			return;
		}
		this.lookAtFullChangeTime -= deltaTime;
		if (this.lookAtFullChangeTime <= 0f)
		{
			this.lookAtFullChangeTime = 1.3f + 2.7f * e.rand.RandomFloat;
			this.lookAtFullBlendPer = 0.2f + 1.5f * e.rand.RandomFloat;
			if (this.lookAtFullBlendPer > 1f)
			{
				this.lookAtFullBlendPer = 1f;
			}
		}
		this.lookAtBlendPer = Mathf.MoveTowards(this.lookAtBlendPer, this.lookAtBlendPerTarget, deltaTime * 1.5f);
		Quaternion rotation = this.neckParentTransform.rotation;
		Transform transform = this.headTransform;
		Vector3 upwards = rotation * Vector3.up;
		Quaternion quaternion;
		if (this.entity is EntityNPC)
		{
			quaternion = Quaternion.LookRotation(this.lookAtPos - Origin.position - transform.position);
			quaternion *= Quaternion.AngleAxis(-90f, Vector3.forward);
		}
		else
		{
			quaternion = Quaternion.LookRotation(this.lookAtPos - Origin.position - transform.position, upwards);
			quaternion *= Quaternion.Slerp(Quaternion.identity, transform.localRotation, 0.5f);
		}
		Quaternion b = Quaternion.RotateTowards(rotation, quaternion, this.lookAtMaxAngle);
		this.lookAtRot = Quaternion.Slerp(this.lookAtRot, b, 0.16f);
		float num = this.lookAtBlendPer;
		this.neckTransform.rotation = Quaternion.Slerp(this.neckTransform.rotation, this.lookAtRot, num * 0.4f);
		Quaternion rotation2 = transform.rotation;
		transform.rotation = Quaternion.Slerp(rotation2, this.lookAtRot, num);
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000F86F0 File Offset: 0x000F68F0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.FrameUpdateRagdoll();
		if (this.modelTransformParent != this.headTransform)
		{
			this.UpdateHeadState();
		}
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000F8714 File Offset: 0x000F6914
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (this.ragdollIsBlending)
		{
			this.BlendRagdoll();
		}
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive != null && !this.IsRagdollActive)
		{
			if (entityAlive.crouchType > 0)
			{
				this.CrouchUpdate(entityAlive);
			}
			if (this.lookAtEnabled)
			{
				this.LookAtUpdate(entityAlive);
			}
		}
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000F8765 File Offset: 0x000F6965
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Transform GetModelTransform()
	{
		if (!this.modelTransform)
		{
			return this.modelTransformParent;
		}
		return this.modelTransform;
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000F8781 File Offset: 0x000F6981
	public Transform GetModelTransformParent()
	{
		return this.modelTransformParent;
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000F878C File Offset: 0x000F698C
	public virtual Transform GetHitTransform(DamageSource _damageSource)
	{
		string hitTransformName = _damageSource.getHitTransformName();
		if (hitTransformName != null && this.bipedRootTransform)
		{
			return this.bipedRootTransform.FindInChilds(hitTransformName, false);
		}
		return null;
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000F87C0 File Offset: 0x000F69C0
	public virtual Transform GetHitTransform(BodyPrimaryHit _primary)
	{
		if (this.physicsBody != null)
		{
			string tag;
			switch (_primary)
			{
			case BodyPrimaryHit.Torso:
				tag = "E_BP_Body";
				break;
			case BodyPrimaryHit.Head:
				tag = "E_BP_Head";
				break;
			case BodyPrimaryHit.LeftUpperArm:
				tag = "E_BP_LArm";
				break;
			case BodyPrimaryHit.RightUpperArm:
				tag = "E_BP_RArm";
				break;
			case BodyPrimaryHit.LeftUpperLeg:
				tag = "E_BP_LLeg";
				break;
			case BodyPrimaryHit.RightUpperLeg:
				tag = "E_BP_RLeg";
				break;
			case BodyPrimaryHit.LeftLowerArm:
				tag = "E_BP_LLowerArm";
				break;
			case BodyPrimaryHit.RightLowerArm:
				tag = "E_BP_RLowerArm";
				break;
			case BodyPrimaryHit.LeftLowerLeg:
				tag = "E_BP_LLowerLeg";
				break;
			case BodyPrimaryHit.RightLowerLeg:
				tag = "E_BP_RLowerLeg";
				break;
			default:
				return null;
			}
			return this.physicsBody.GetTransformForColliderTag(tag);
		}
		return null;
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000F886A File Offset: 0x000F6A6A
	public virtual Transform GetHeadTransform()
	{
		if (!this.headTransform)
		{
			return base.transform;
		}
		return this.headTransform;
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000F8886 File Offset: 0x000F6A86
	public virtual Transform GetPelvisTransform()
	{
		return this.bipedPelvisTransform;
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000F888E File Offset: 0x000F6A8E
	public virtual Transform GetThirdPersonCameraTransform()
	{
		return base.transform;
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000F8898 File Offset: 0x000F6A98
	public virtual void OnDeath(DamageResponse _dmResponse, ChunkCluster _cc)
	{
		EntityAlive entityAlive = this.entity as EntityAlive;
		bool flag = entityAlive && entityAlive.bodyDamage.CurrentStun > EnumEntityStunType.None;
		bool flag2 = true;
		if (this.HasRagdoll() && (flag2 || !this.entity.HasDeathAnim || entityAlive.IsSleeper || entityAlive.GetWalkType() == 21 || flag || _dmResponse.Random < this.ragdollChance))
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.DoRagdoll(_dmResponse, 999999f);
				return;
			}
			if (!entityAlive.IsSpawned())
			{
				this.SpawnWithRagdoll();
				return;
			}
		}
		else if (this.avatarController != null)
		{
			this.avatarController.StartDeathAnimation(_dmResponse.HitBodyPart, _dmResponse.MovementState, _dmResponse.Random);
			if (this.entity is EntityPlayer && this.bipedRootTransform)
			{
				Transform transform = this.bipedRootTransform.Find("Spine1");
				if (transform)
				{
					RagdollWhenHit ragdollWhenHit;
					if (transform.TryGetComponent<RagdollWhenHit>(out ragdollWhenHit))
					{
						ragdollWhenHit.enabled = true;
						return;
					}
					transform.gameObject.AddComponent<RagdollWhenHit>();
				}
			}
		}
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void restoreTPose(PhysicsBodyInstance physicsBody)
	{
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000F89B4 File Offset: 0x000F6BB4
	public virtual bool HasRagdoll()
	{
		return this.bHasRagdoll && this.physicsBody != null;
	}

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x06002663 RID: 9827 RVA: 0x000F89C9 File Offset: 0x000F6BC9
	public bool IsRagdollActive
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.ragdollState > EModelBase.ERagdollState.Off;
		}
	}

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x06002664 RID: 9828 RVA: 0x000F89D4 File Offset: 0x000F6BD4
	public bool IsRagdollMovement
	{
		get
		{
			return this.ragdollState != EModelBase.ERagdollState.Off && this.ragdollState != EModelBase.ERagdollState.StandCollide;
		}
	}

	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06002665 RID: 9829 RVA: 0x000F89EC File Offset: 0x000F6BEC
	public bool IsRagdollOn
	{
		get
		{
			return this.ragdollState == EModelBase.ERagdollState.On || this.ragdollState == EModelBase.ERagdollState.Dead;
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x06002666 RID: 9830 RVA: 0x000F8A02 File Offset: 0x000F6C02
	public bool IsRagdollDead
	{
		get
		{
			return this.ragdollState == EModelBase.ERagdollState.Dead;
		}
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000F8A10 File Offset: 0x000F6C10
	public void DoRagdoll(float stunTime, EnumBodyPartHit bodyPart, Vector3 forceVec, Vector3 forceWorldPos, bool isRemote)
	{
		if (this.entity.IsFlyMode.Value || this.entity.AttachedToEntity)
		{
			return;
		}
		if (!isRemote && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.entity.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, stunTime, bodyPart, forceVec, forceWorldPos), false);
			return;
		}
		bool flag = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.entity.isEntityRemote;
		if (stunTime == 0f)
		{
			if (!flag)
			{
				this.entity.PhysicsPush(forceVec, forceWorldPos, false);
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entity.entityId, -1, NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, 0f, EnumBodyPartHit.Torso, forceVec, forceWorldPos), false);
			}
			return;
		}
		if (!this.StartRagdoll(stunTime))
		{
			return;
		}
		if (forceVec.sqrMagnitude > 0f && !this.entity.isEntityRemote)
		{
			Vector3 vector = forceVec;
			if (bodyPart == EnumBodyPartHit.None)
			{
				float num = -10f;
				if (vector.y < num)
				{
					vector.y = num;
				}
				this.SetRagdollVelocity(vector);
			}
			else
			{
				BodyPrimaryHit primary = bodyPart.ToPrimary();
				Transform hitTransform = this.GetHitTransform(primary);
				if (hitTransform)
				{
					Rigidbody component = hitTransform.GetComponent<Rigidbody>();
					if (component)
					{
						if (forceWorldPos.sqrMagnitude > 0f)
						{
							component.AddForceAtPosition(vector, forceWorldPos - Origin.position, ForceMode.Impulse);
						}
						else
						{
							component.AddForce(vector, ForceMode.Impulse);
						}
					}
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entity.entityId, -1, NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, stunTime, bodyPart, forceVec, forceWorldPos), false);
		}
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000F8BE8 File Offset: 0x000F6DE8
	public void DoRagdoll(DamageResponse dr, float stunTime = 999999f)
	{
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive && entityAlive.isDisintegrated)
		{
			return;
		}
		float num = (float)dr.Strength;
		DamageSource source = dr.Source;
		if (num > 0f && source != null)
		{
			Vector3 vector = source.getDirection();
			EnumDamageTypes damageType = source.GetDamageType();
			if (damageType != EnumDamageTypes.Falling && damageType != EnumDamageTypes.Crushing)
			{
				float num2;
				if (dr.HitBodyPart == EnumBodyPartHit.None)
				{
					num2 = this.entity.rand.RandomRange(5f, 25f);
				}
				else
				{
					float min = -10f;
					if (stunTime == 0f)
					{
						min = 5f;
					}
					num2 = this.entity.rand.RandomRange(min, 40f);
					num *= 0.5f;
					if (source.damageType == EnumDamageTypes.Bashing)
					{
						num *= 2.5f;
					}
					if (dr.Critical)
					{
						num2 += 25f;
						num *= 2f;
					}
					if ((dr.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
					{
						num *= 0.45f;
					}
					num = Utils.FastMin(20f + num, 500f);
					vector *= num;
				}
				Vector3 axis = Vector3.Cross(vector.normalized, Vector3.up);
				vector = Quaternion.AngleAxis(num2, axis) * vector;
			}
			this.DoRagdoll(stunTime, dr.HitBodyPart, vector, source.getHitTransformPosition(), false);
			return;
		}
		this.DoRagdoll(stunTime, dr.HitBodyPart, Vector3.zero, Vector3.zero, false);
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000F8D5B File Offset: 0x000F6F5B
	public void SetRagdollState(int newState)
	{
		if (this.ragdollState == EModelBase.ERagdollState.On && newState == 2)
		{
			this.ragdollTime = 9999f;
		}
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000F8D75 File Offset: 0x000F6F75
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnWithRagdoll()
	{
		if (this.ragdollState == EModelBase.ERagdollState.Off)
		{
			this.ragdollState = EModelBase.ERagdollState.SpawnWait;
		}
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000F8D88 File Offset: 0x000F6F88
	[PublicizedFrom(EAccessModifier.Private)]
	public bool StartRagdoll(float stunTime)
	{
		if (!this.HasRagdoll())
		{
			return false;
		}
		if (this.ragdollState == EModelBase.ERagdollState.Dead)
		{
			return true;
		}
		bool flag = this.entity.IsDead();
		if (!flag && this.ragdollState == EModelBase.ERagdollState.On)
		{
			this.ragdollDuration = Utils.FastMax(this.ragdollDuration, stunTime);
			return true;
		}
		if (this.entity.IsMarkedForUnload())
		{
			return false;
		}
		this.ragdollAnimator = this.avatarController.GetAnimator();
		if (!this.ragdollAnimator)
		{
			return false;
		}
		bool flag2 = this.ragdollState > EModelBase.ERagdollState.Off;
		this.ragdollState = EModelBase.ERagdollState.On;
		this.ragdollTime = 0f;
		this.entity.OnRagdoll(true);
		this.ragdollIsPlayer = (this.entity is EntityPlayer);
		this.ragdollAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		this.ragdollAnimator.keepAnimatorStateOnDisable = true;
		this.ragdollAnimator.enabled = false;
		Animation component = this.GetModelTransform().GetComponent<Animation>();
		if (component)
		{
			component.cullingType = AnimationCullingType.AlwaysAnimate;
			component.enabled = false;
		}
		this.CaptureRagdollBones();
		this.CaptureRagdollZeroBones();
		if (flag)
		{
			stunTime = 0.3f;
			this.SetRagdollDead();
		}
		if (this.physicsBody != null)
		{
			this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Ragdoll);
		}
		this.entity.PhysicsPause();
		EntityAlive entityAlive = this.entity as EntityAlive;
		entityAlive.SetStun(EnumEntityStunType.Prone);
		entityAlive.bodyDamage.StunDuration = 1f;
		entityAlive.SetCVar("ragdoll", 1f);
		if (!this.ragdollIsPlayer)
		{
			this.entity.PhysicsTransform.gameObject.SetActive(false);
		}
		this.ragdollTime = 0f;
		this.ragdollDuration = stunTime;
		this.ragdollRotY = this.entity.rotation.y;
		if (this.ragdollIsPlayer)
		{
			this.ragdollDuration = 1f;
		}
		if (flag2)
		{
			this.ragdollIsBlending = false;
			this.ragdollAdjustPosDelay = 0f;
		}
		this.ResetLookAt();
		return true;
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000F8F68 File Offset: 0x000F7168
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRagdollDead()
	{
		this.ragdollState = EModelBase.ERagdollState.Dead;
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			Rigidbody rb = this.ragdollPoses[i].rb;
			if (rb)
			{
				rb.maxDepenetrationVelocity = 2f;
				rb.maxAngularVelocity = 1f;
			}
		}
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000F8FC4 File Offset: 0x000F71C4
	public void DisableRagdoll(bool isSetAlive)
	{
		if (this.ragdollState == EModelBase.ERagdollState.Off)
		{
			return;
		}
		if (this.physicsBody != null)
		{
			this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Collision);
		}
		if (this.bipedRootTransform)
		{
			RagdollWhenHit[] componentsInChildren = this.bipedRootTransform.GetComponentsInChildren<RagdollWhenHit>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
		}
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (!isSetAlive && !this.entity.IsDead())
		{
			Vector3 vector = this.headTransform.position - this.bipedPelvisTransform.position;
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			this.ragdollIsFacingUp = false;
			this.ragdollIsAnimal = EntityClass.list[entityAlive.entityClass].bIsAnimalEntity;
			string stateName;
			if (this.ragdollIsAnimal)
			{
				stateName = "Knockdown";
			}
			else
			{
				stateName = "Knockdown - Chest";
				if (this.bipedPelvisTransform.forward.y > 0f)
				{
					this.ragdollIsFacingUp = true;
					stateName = "Knockdown - Back";
					num += 180f;
				}
			}
			this.ragdollRotY = num;
			this.CopyRagdollRot();
			Animation component = this.GetModelTransform().GetComponent<Animation>();
			if (component != null)
			{
				component.cullingType = AnimationCullingType.AlwaysAnimate;
				component.enabled = true;
			}
			this.avatarController.ResetAnimations();
			this.ragdollAnimator.keepAnimatorStateOnDisable = true;
			int layer = this.ragdollIsPlayer ? 5 : 0;
			this.ragdollAnimator.CrossFadeInFixedTime(stateName, 0.25f, layer, 2f, 0f);
			this.ragdollAdjustPosDelay = 0.05f;
			this.ragdollState = EModelBase.ERagdollState.BlendOutGround;
			this.ragdollTime = 0f;
			this.ragdollIsBlending = true;
			entityAlive.SetStun(EnumEntityStunType.Getup);
			return;
		}
		this.bipedPelvisTransform.localPosition = this.ragdollPosePelvisLocalPos;
		this.RestoreRagdollStartRot();
		this.SetRagdollOff();
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000F91A0 File Offset: 0x000F73A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CaptureRagdollBones()
	{
		if (this.ragdollPoses.Count > 0)
		{
			return;
		}
		Animator animator = this.avatarController.GetAnimator();
		if (!animator)
		{
			return;
		}
		this.ragdollPosePelvisLocalPos = this.bipedPelvisTransform.localPosition;
		animator.GetComponentsInChildren<Rigidbody>(EModelBase.ragdollTempRBs);
		for (int i = 0; i < EModelBase.ragdollTempRBs.Count; i++)
		{
			Rigidbody rigidbody = EModelBase.ragdollTempRBs[i];
			GameObject gameObject = rigidbody.gameObject;
			if (!gameObject.CompareTag("Item") && !gameObject.CompareTag("AudioRigidBody"))
			{
				Transform transform = rigidbody.transform;
				EModelBase.RagdollPose item;
				item.t = transform;
				item.rb = rigidbody;
				item.rot = Quaternion.identity;
				item.startRot = transform.localRotation;
				this.ragdollPoses.Add(item);
			}
		}
		EModelBase.ragdollTempRBs.Clear();
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000F927C File Offset: 0x000F747C
	public void CaptureRagdollPositions(List<Vector3> positionList)
	{
		this.CaptureRagdollBones();
		positionList.Clear();
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			positionList.Add(this.ragdollPoses[i].t.position);
		}
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000F92C8 File Offset: 0x000F74C8
	public void ApplyRagdollVelocities(List<Vector3> velocities)
	{
		if (velocities.Count == this.ragdollPoses.Count)
		{
			for (int i = 0; i < this.ragdollPoses.Count; i++)
			{
				Rigidbody rb = this.ragdollPoses[i].rb;
				if (rb)
				{
					rb.velocity = velocities[i];
				}
			}
		}
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000F9328 File Offset: 0x000F7528
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRagdollVelocity(Vector3 _vel)
	{
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			this.ragdollPoses[i].rb.velocity = _vel;
		}
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000F9364 File Offset: 0x000F7564
	[PublicizedFrom(EAccessModifier.Private)]
	public void CopyRagdollRot()
	{
		this.ragdollPosePelvisPos = this.bipedPelvisTransform.position;
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			EModelBase.RagdollPose ragdollPose = this.ragdollPoses[i];
			if (ragdollPose.t == this.bipedPelvisTransform)
			{
				ragdollPose.rot = ragdollPose.t.rotation;
			}
			else
			{
				ragdollPose.rot = ragdollPose.t.localRotation;
			}
			this.ragdollPoses[i] = ragdollPose;
		}
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000F93EC File Offset: 0x000F75EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void RestoreRagdollStartRot()
	{
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			this.ragdollPoses[i].t.localRotation = this.ragdollPoses[i].startRot;
		}
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000F9438 File Offset: 0x000F7638
	[PublicizedFrom(EAccessModifier.Private)]
	public void CaptureRagdollZeroBones()
	{
		if (this.ragdollZeroBones != null)
		{
			return;
		}
		Transform parent = this.headTransform;
		if (!parent)
		{
			return;
		}
		this.ragdollZeroTime = 0.33f;
		this.ragdollZeroBones = new List<Transform>();
		while ((parent = parent.parent) && !(parent == this.bipedPelvisTransform))
		{
			if (!parent.GetComponent<Rigidbody>())
			{
				GameObject gameObject = parent.gameObject;
				if (!gameObject.CompareTag("Item") && !gameObject.CompareTag("AudioRigidBody"))
				{
					this.ragdollZeroBones.Add(parent);
				}
			}
		}
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000F94D0 File Offset: 0x000F76D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BlendRagdollZeroBones()
	{
		if (this.ragdollZeroTime > 0f)
		{
			float deltaTime = Time.deltaTime;
			this.ragdollZeroTime -= deltaTime;
			for (int i = 0; i < this.ragdollZeroBones.Count; i++)
			{
				Transform transform = this.ragdollZeroBones[i];
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, 120f * deltaTime);
			}
		}
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000F953C File Offset: 0x000F773C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BlendRagdoll()
	{
		if (this.ragdollAdjustPosDelay > 0f)
		{
			this.ragdollAdjustPosDelay -= Time.deltaTime;
			if (this.ragdollAdjustPosDelay <= 0f)
			{
				Vector3 vector = this.bipedPelvisTransform.position - this.modelTransform.position;
				Vector3 vector2 = this.ragdollPosePelvisPos;
				vector2.x -= vector.x;
				vector2.z -= vector.z;
				if (this.entity.isEntityRemote)
				{
					vector2 = Vector3.Lerp(vector2, this.entity.targetPos - Origin.position, Time.fixedDeltaTime * 10f);
				}
				int num = 0;
				RaycastHit raycastHit;
				while (num < 5 && Physics.Raycast(vector2, Vector3.down, out raycastHit, 3f, -538750981))
				{
					RootTransformRefEntity component = raycastHit.transform.GetComponent<RootTransformRefEntity>();
					if (!component || component.RootTransform != this.entity.transform)
					{
						vector2.y = raycastHit.point.y + 0.02f;
						break;
					}
					vector2.y = raycastHit.point.y - 0.01f;
					num++;
				}
				this.entity.PhysicsResume(vector2 + Origin.position, this.ragdollRotY);
			}
		}
		float num2 = this.ragdollTime / 0.7f;
		float num3 = num2;
		if (!this.ragdollIsAnimal)
		{
			num3 = (num2 - 0.2f) / 0.8f;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
		}
		this.bipedPelvisTransform.position = Vector3.Lerp(this.ragdollPosePelvisPos, this.bipedPelvisTransform.position, num3);
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			Transform t = this.ragdollPoses[i].t;
			if (t == this.bipedPelvisTransform)
			{
				t.rotation = Quaternion.Slerp(this.ragdollPoses[i].rot, t.rotation, num3);
			}
			else
			{
				t.localRotation = Quaternion.Slerp(this.ragdollPoses[i].rot, t.localRotation, num2);
			}
		}
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000F9788 File Offset: 0x000F7988
	[PublicizedFrom(EAccessModifier.Private)]
	public void FrameUpdateRagdoll()
	{
		if (this.ragdollState == EModelBase.ERagdollState.Off)
		{
			return;
		}
		if (this.ragdollState == EModelBase.ERagdollState.SpawnWait)
		{
			Chunk chunk = (Chunk)this.entity.world.GetChunkFromWorldPos(this.entity.GetBlockPosition());
			if (chunk != null && chunk.IsCollisionMeshGenerated && chunk.IsDisplayed)
			{
				this.ragdollState = EModelBase.ERagdollState.Off;
				this.StartRagdoll(float.MaxValue);
			}
			return;
		}
		bool flag = this.entity.IsDead();
		if (this.pelvisRB && this.IsRagdollMovement)
		{
			if (!flag && this.entity.isEntityRemote)
			{
				Vector3 position = this.pelvisRB.position;
				Vector3 b = this.entity.targetPos - Origin.position;
				this.pelvisRB.AddForce(-EModelBase.serverPosSpringForce * (position - b) - EModelBase.serverPosSpringDamping * this.pelvisRB.velocity, ForceMode.Acceleration);
			}
			if (!(this.entity is EntityPlayer))
			{
				this.entity.SetPosition(this.pelvisRB.position + Origin.position, false);
			}
			else if (!this.entity.isEntityRemote)
			{
				this.entity.SetPosition(this.pelvisRB.position + Origin.position, false);
			}
		}
		this.entity.SetRotationAndStopTurning(new Vector3(0f, this.ragdollRotY, 0f));
		this.ragdollTime += Time.deltaTime;
		switch (this.ragdollState)
		{
		case EModelBase.ERagdollState.On:
			this.BlendRagdollZeroBones();
			if (this.ragdollTime >= this.ragdollDuration && (this.pelvisRB.velocity.sqrMagnitude <= 0.25f || this.ragdollTime > 10f))
			{
				this.DisableRagdoll(false);
				if (!this.entity.isEntityRemote)
				{
					NetPackageEntityRagdoll package = NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, (sbyte)this.ragdollState);
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						this.entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entity.entityId, -1, package, false);
						return;
					}
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
					return;
				}
			}
			break;
		case EModelBase.ERagdollState.BlendOutGround:
			this.RestoreRagdollStartRot();
			if (this.ragdollTime >= 0.25f)
			{
				string stateName = "GetUpChest";
				if (this.ragdollIsFacingUp)
				{
					stateName = "GetUpBack";
				}
				if ((this.entity as EntityAlive).IsWalkTypeACrawl())
				{
					stateName = "CrawlerGetUpChest";
				}
				int layer = this.ragdollIsPlayer ? 5 : 0;
				this.ragdollAnimator.CrossFade(stateName, 0.3f, layer);
				this.ragdollState = EModelBase.ERagdollState.BlendOutStand;
				return;
			}
			break;
		case EModelBase.ERagdollState.BlendOutStand:
			this.RestoreRagdollStartRot();
			if (this.ragdollTime >= 0.7f)
			{
				this.ragdollIsBlending = false;
				this.ragdollState = EModelBase.ERagdollState.Stand;
				this.ragdollTime = 0f;
				return;
			}
			break;
		case EModelBase.ERagdollState.Stand:
			if (this.ragdollTime >= 0.8f)
			{
				if (!this.ragdollIsPlayer)
				{
					this.entity.PhysicsTransform.gameObject.SetActive(true);
				}
				this.ragdollState = EModelBase.ERagdollState.StandCollide;
				return;
			}
			break;
		case EModelBase.ERagdollState.StandCollide:
			if (this.ragdollTime >= 1.7f)
			{
				this.SetRagdollOff();
				this.entity.OnRagdoll(false);
				return;
			}
			break;
		case EModelBase.ERagdollState.SpawnWait:
			break;
		case EModelBase.ERagdollState.Dead:
			this.BlendRagdollZeroBones();
			if (this.ragdollTime >= this.ragdollDuration)
			{
				this.ragdollDuration = float.MaxValue;
				if (this.physicsBody != null)
				{
					this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.RagdollDead);
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000F9B1C File Offset: 0x000F7D1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRagdollOff()
	{
		this.ragdollState = EModelBase.ERagdollState.Off;
		EntityAlive entityAlive = this.entity as EntityAlive;
		entityAlive.SetCVar("ragdoll", 0f);
		entityAlive.ClearStun();
		if (!this.ragdollIsPlayer)
		{
			this.entity.PhysicsTransform.gameObject.SetActive(true);
		}
		this.ragdollPoses.Clear();
		this.ragdollZeroBones = null;
		this.CheckAnimFreeze();
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000F9B86 File Offset: 0x000F7D86
	public string GetRagdollDebugInfo()
	{
		return string.Format("{0:0.#}/{1:0.#} {2}", this.ragdollTime.ToCultureInvariantString(), this.ragdollDuration.ToCultureInvariantString(), this.ragdollState.ToStringCached<EModelBase.ERagdollState>());
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000F9BB4 File Offset: 0x000F7DB4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ClothSimInit()
	{
		this.clothSim = base.GetComponentsInChildren<Cloth>();
		if (this.clothSim != null)
		{
			for (int i = this.clothSim.Length - 1; i >= 0; i--)
			{
				this.clothSim[i].gameObject.SetActive(false);
			}
			if (GameManager.IsDedicatedServer)
			{
				this.clothSim = null;
			}
			this.isClothSimOn = false;
		}
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000F9C14 File Offset: 0x000F7E14
	public void ClothSimOn(bool _on)
	{
		if (this.clothSim == null || this.isClothSimOn == _on)
		{
			return;
		}
		this.isClothSimOn = _on;
		for (int i = this.clothSim.Length - 1; i >= 0; i--)
		{
			this.clothSim[i].gameObject.SetActive(_on);
		}
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000F9C64 File Offset: 0x000F7E64
	[PublicizedFrom(EAccessModifier.Private)]
	public void JiggleInit()
	{
		this.jiggles = base.GetComponentsInChildren<Jiggle>();
		if (GameManager.IsDedicatedServer && this.jiggles != null)
		{
			for (int i = this.jiggles.Length - 1; i >= 0; i--)
			{
				this.jiggles[i].gameObject.SetActive(false);
			}
			this.jiggles = null;
		}
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000F9CBC File Offset: 0x000F7EBC
	public void JiggleOn(bool _on)
	{
		if (this.jiggles == null || this.isJiggleOn == _on)
		{
			return;
		}
		this.isJiggleOn = _on;
		for (int i = this.jiggles.Length - 1; i >= 0; i--)
		{
			this.jiggles[i].gameObject.SetActive(_on);
		}
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x000F9D0C File Offset: 0x000F7F0C
	public virtual void SwitchModelAndView(bool _bFPV, bool _bMale)
	{
		if (this.GetModelTransform() != null)
		{
			Animator component = this.GetModelTransform().GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = !_bFPV;
			}
		}
		this.IsFPV = _bFPV;
		if (this.modelName != null && this.modelTransformParent != null)
		{
			this.modelTransform = this.modelTransformParent.Find(this.modelName);
			this.meshTransform = GameUtils.FindTagInDirectChilds(this.modelTransform, "E_Mesh");
			if (!this.meshTransform)
			{
				this.meshTransform = this.modelTransform.Find("LOD0");
				if (!this.meshTransform)
				{
					Renderer componentInChildren = this.modelTransform.GetComponentInChildren<Renderer>();
					if (componentInChildren)
					{
						this.meshTransform = componentInChildren.transform;
					}
				}
			}
		}
		Transform transform = this.GetModelTransform();
		if (this.avatarController)
		{
			this.avatarController.SwitchModelAndView(this.modelName, _bFPV, _bMale);
		}
		else if (transform)
		{
			transform.gameObject.SetActive(true);
		}
		this.headTransform = GameUtils.FindTagInChilds(transform, "E_BP_Head");
		if (this.headTransform)
		{
			this.neckTransform = this.headTransform.parent;
			this.neckParentTransform = this.neckTransform.parent;
		}
		this.bipedRootTransform = GameUtils.FindTagInChilds(transform, "E_BP_BipedRoot");
		if (this.bipedRootTransform == null)
		{
			foreach (string text in EModelBase.commonBips)
			{
				if ((this.bipedRootTransform = transform.Find(text)) != null || (this.bipedRootTransform = transform.FindInChilds(text, false)) != null)
				{
					break;
				}
			}
		}
		if (this.bipedRootTransform != null)
		{
			if (this.bipedRootTransform.name != "pelvis" && this.bipedRootTransform.name != "Hips")
			{
				this.bipedPelvisTransform = GameUtils.FindChildWithPartialName(this.bipedRootTransform, new string[]
				{
					"pelvis"
				});
				if (this.bipedPelvisTransform == null)
				{
					this.bipedPelvisTransform = GameUtils.FindChildWithPartialName(this.bipedRootTransform, new string[]
					{
						"hips"
					});
					if (this.bipedPelvisTransform == null)
					{
						this.bipedPelvisTransform = GameUtils.FindChildWithPartialName(this.bipedRootTransform, new string[]
						{
							"hip"
						});
					}
				}
			}
			else
			{
				this.bipedPelvisTransform = this.bipedRootTransform;
			}
		}
		if (this.bipedPelvisTransform)
		{
			this.pelvisRB = this.bipedPelvisTransform.GetComponent<Rigidbody>();
			if (!(this.entity is EntityPlayer))
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.modelTransformParent.GetComponentsInChildren<SkinnedMeshRenderer>(true))
				{
					if (skinnedMeshRenderer.quality == SkinQuality.Auto)
					{
						skinnedMeshRenderer.quality = SkinQuality.Bone2;
					}
					Bounds localBounds = skinnedMeshRenderer.localBounds;
					if (skinnedMeshRenderer.CompareTag("E_BP_Eye"))
					{
						if (this.headTransform)
						{
							skinnedMeshRenderer.rootBone = this.headTransform;
							localBounds.center = Vector3.zero;
							localBounds.extents = new Vector3(0.25f, 0.25f, 0.25f);
						}
						skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
					}
					else if (skinnedMeshRenderer.rootBone != this.bipedPelvisTransform)
					{
						skinnedMeshRenderer.rootBone = this.bipedPelvisTransform;
						localBounds.center += -this.bipedPelvisTransform.localPosition;
					}
					skinnedMeshRenderer.localBounds = localBounds;
				}
			}
		}
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive && entityAlive.inventory != null)
		{
			if (this.entity.isEntityRemote)
			{
				entityAlive.inventory.ForceHoldingItemUpdate();
			}
			else if (this.entity is EntityPlayerLocal && (this.entity as EntityPlayerLocal).PlayerUI != null && !(this.entity as EntityPlayerLocal).PlayerUI.windowManager.IsWindowOpen("character"))
			{
				entityAlive.inventory.ForceHoldingItemUpdate();
			}
		}
		if (this.GetRightHandTransform() != null)
		{
			SkinnedMeshRenderer[] componentsInChildren2 = this.GetRightHandTransform().GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int k = 0; k < componentsInChildren2.Length; k++)
			{
				componentsInChildren2[k].updateWhenOffscreen = true;
			}
		}
		this.NavObjectTransform = base.transform.FindInChilds("IconTag", false);
		PhysicsBodyLayout physicsBodyLayout = EntityClass.list[this.entity.entityClass].PhysicsBody;
		if (physicsBodyLayout != null && this.bipedRootTransform != null)
		{
			if (this.physicsBody != null)
			{
				this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Disabled);
			}
			this.physicsBody = new PhysicsBodyInstance(this.bipedRootTransform, physicsBodyLayout, EnumColliderMode.Collision);
			this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Collision);
			RagdollWhenHit[] componentsInChildren3 = this.bipedRootTransform.GetComponentsInChildren<RagdollWhenHit>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].enabled = false;
			}
		}
		else
		{
			this.physicsBody = null;
		}
		if (transform != null)
		{
			Animator component2 = transform.GetComponent<Animator>();
			if (component2 != null)
			{
				component2.enabled = !this.IsFPV;
			}
			this.SetColliderLayers(transform, 0);
		}
		this.CheckAnimFreeze();
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000FA26C File Offset: 0x000F846C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetColliderLayers(Transform modelT, int layer)
	{
		CapsuleCollider[] componentsInChildren = modelT.GetComponentsInChildren<CapsuleCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject gameObject = componentsInChildren[i].gameObject;
			if (!gameObject.CompareTag("LargeEntityBlocker") && !gameObject.CompareTag("Physics"))
			{
				gameObject.layer = layer;
			}
		}
		BoxCollider[] componentsInChildren2 = modelT.GetComponentsInChildren<BoxCollider>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].gameObject.layer = layer;
		}
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x000FA2E0 File Offset: 0x000F84E0
	public virtual void SetInRightHand(Transform _transform)
	{
		if (this.avatarController != null)
		{
			this.avatarController.SetInRightHand(_transform);
		}
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000FA2FC File Offset: 0x000F84FC
	public virtual void SetAlive()
	{
		this.DisableRagdoll(true);
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			Utils.MoveTaggedToLayer(transform.gameObject, "LargeEntityBlocker", 19);
			Animator component = transform.GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = !this.IsFPV;
			}
		}
		if (this.avatarController != null)
		{
			this.avatarController.SetAlive();
		}
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x000FA36C File Offset: 0x000F856C
	public virtual void SetDead()
	{
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			Utils.MoveTaggedToLayer(transform.gameObject, "LargeEntityBlocker", 17);
		}
		if (this.ragdollState == EModelBase.ERagdollState.On)
		{
			this.SetRagdollDead();
			if (this.physicsBody != null && this.physicsBody.Mode != EnumColliderMode.RagdollDead)
			{
				this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.RagdollDead);
			}
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06002683 RID: 9859 RVA: 0x000FA3CC File Offset: 0x000F85CC
	// (set) Token: 0x06002684 RID: 9860 RVA: 0x000FA3D4 File Offset: 0x000F85D4
	public bool visible { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06002685 RID: 9861 RVA: 0x000FA3E0 File Offset: 0x000F85E0
	public virtual void SetVisible(bool _bVisible, bool _isKeepColliders = false)
	{
		this.visible = _bVisible;
		if (_isKeepColliders)
		{
			this.modelTransformParent.GetComponentsInChildren<Renderer>(EModelBase.rendererList);
			for (int i = 0; i < EModelBase.rendererList.Count; i++)
			{
				EModelBase.rendererList[i].enabled = _bVisible;
			}
			EModelBase.rendererList.Clear();
			if (!_bVisible)
			{
				return;
			}
		}
		if (this.avatarController != null)
		{
			this.avatarController.SetVisible(_bVisible);
		}
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000FA458 File Offset: 0x000F8658
	public void SetFade(float _fade)
	{
		if (this.matPropBlock == null)
		{
			this.matPropBlock = new MaterialPropertyBlock();
		}
		this.modelTransformParent.GetComponentsInChildren<Renderer>(EModelBase.rendererList);
		for (int i = 0; i < EModelBase.rendererList.Count; i++)
		{
			Renderer renderer = EModelBase.rendererList[i];
			bool flag = false;
			Material material = renderer.material;
			string name = material.shader.name;
			if (material.HasProperty("_Fade") && name.Contains("Game/Character"))
			{
				flag = true;
			}
			if (renderer.gameObject.CompareTag("LOD") || renderer.gameObject.CompareTag("E_Mesh") || flag)
			{
				this.matPropBlock.SetFloat(EModelBase.fadeId, _fade);
				renderer.SetPropertyBlock(this.matPropBlock);
			}
		}
		EModelBase.rendererList.Clear();
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000FA52E File Offset: 0x000F872E
	public virtual Transform GetRightHandTransform()
	{
		if (this.avatarController != null)
		{
			return this.avatarController.GetRightHandTransform();
		}
		return null;
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetSkinTexture(string _textureName)
	{
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000FA54C File Offset: 0x000F874C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createModel(World _world, EntityClass _ec)
	{
		if (this.modelTransformParent == null)
		{
			return;
		}
		Transform transform = null;
		if (_ec.IsPrefabCombined && this.modelTransformParent.childCount > 0)
		{
			transform = this.modelTransformParent.GetChild(0);
		}
		if (_ec.mesh)
		{
			transform = UnityEngine.Object.Instantiate<Transform>(_ec.mesh, this.modelTransformParent, false);
			transform.name = _ec.mesh.name;
			Vector3 localPosition = transform.localPosition;
			if ((double)localPosition.z < -0.5 || (double)localPosition.z > 0.5)
			{
				Log.Warning("createModel mesh moved {0} {1} {2}", new object[]
				{
					transform.name,
					localPosition.ToString("f3"),
					transform.localRotation
				});
			}
			localPosition.x = 0f;
			localPosition.y = 0f;
			transform.localPosition = localPosition;
		}
		if (transform)
		{
			transform.gameObject.SetActive(true);
			this.modelName = transform.name;
			if (_ec.particleOnSpawn.fileName != null)
			{
				ParticleSystem particleSystem = DataLoader.LoadAsset<ParticleSystem>(_ec.particleOnSpawn.fileName, false);
				if (particleSystem != null)
				{
					ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate<ParticleSystem>(particleSystem);
					particleSystem2.transform.parent = this.modelTransformParent;
					if (_ec.particleOnSpawn.shapeMesh != null && _ec.particleOnSpawn.shapeMesh.Length > 0)
					{
						SkinnedMeshRenderer[] componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>();
						ParticleSystem.ShapeModule shape = particleSystem2.shape;
						shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
						string text = _ec.particleOnSpawn.shapeMesh.ToLower();
						if (text.Contains("setshapetomesh"))
						{
							text = text.Replace("setshapetomesh", "");
							int num = int.Parse(text);
							if (num >= 0 && num < componentsInChildren.Length)
							{
								shape.skinnedMeshRenderer = componentsInChildren[num];
								ParticleSystem[] componentsInChildren2 = particleSystem2.transform.GetComponentsInChildren<ParticleSystem>();
								if (componentsInChildren2 != null)
								{
									for (int i = 0; i < componentsInChildren2.Length; i++)
									{
										shape = componentsInChildren2[i].shape;
										shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
										shape.skinnedMeshRenderer = componentsInChildren[num];
									}
								}
							}
						}
					}
				}
			}
			bool flag = false;
			if (_ec.AltMatNames != null)
			{
				GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(this.entity.entityId);
				int num2 = gameRandom.RandomRange(_ec.AltMatNames.Length + 1) - 1;
				if (num2 >= 0)
				{
					Material altMaterial = DataLoader.LoadAsset<Material>(_ec.AltMatNames[num2], false);
					this.AltMaterial = altMaterial;
					flag = true;
				}
				GameRandomManager.Instance.FreeGameRandom(gameRandom);
			}
			Color value = Color.black;
			string @string = _ec.Properties.GetString(EntityClass.PropMatColor);
			if (@string.Length > 0)
			{
				value = EntityClass.sColors[@string];
				flag = true;
			}
			if (flag)
			{
				base.GetComponentsInChildren<SkinnedMeshRenderer>(true, EModelBase.skinnedRendererList);
				Material material = this.AltMaterial;
				if (@string.Length > 0)
				{
					if (!material)
					{
						for (int j = 0; j < EModelBase.skinnedRendererList.Count; j++)
						{
							SkinnedMeshRenderer skinnedMeshRenderer = EModelBase.skinnedRendererList[j];
							if (skinnedMeshRenderer.CompareTag("LOD"))
							{
								material = skinnedMeshRenderer.sharedMaterials[0];
								break;
							}
						}
					}
					if (@string.Length > 0)
					{
						material = UnityEngine.Object.Instantiate<Material>(material);
						material.SetColor("_EmissiveColor", value);
					}
					this.AltMaterial = material;
				}
				for (int k = 0; k < EModelBase.skinnedRendererList.Count; k++)
				{
					SkinnedMeshRenderer skinnedMeshRenderer2 = EModelBase.skinnedRendererList[k];
					if (skinnedMeshRenderer2.CompareTag("LOD"))
					{
						Material[] sharedMaterials = skinnedMeshRenderer2.sharedMaterials;
						sharedMaterials[0] = material;
						skinnedMeshRenderer2.materials = sharedMaterials;
					}
				}
				EModelBase.skinnedRendererList.Clear();
			}
			if (_ec.MatSwap != null)
			{
				foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>(true))
				{
					if (renderer.CompareTag("LOD"))
					{
						Material[] sharedMaterials2 = renderer.sharedMaterials;
						int num3 = Utils.FastMin(sharedMaterials2.Length, _ec.MatSwap.Length);
						for (int m = 0; m < num3; m++)
						{
							string text2 = _ec.MatSwap[m];
							if (text2 != null && text2.Length > 0)
							{
								Material material2 = DataLoader.LoadAsset<Material>(text2, false);
								if (material2)
								{
									sharedMaterials2[m] = material2;
								}
							}
						}
						renderer.materials = sharedMaterials2;
					}
				}
			}
		}
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000FA9B8 File Offset: 0x000F8BB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createAvatarController(EntityClass _ec)
	{
		string text = (this.entity is EntityPlayerLocal) ? EntityClass.PropLocalAvatarController : EntityClass.PropAvatarController;
		text = _ec.Properties.GetString(text);
		if (text.Length > 0)
		{
			Type type = Type.GetType(text);
			this.avatarController = (base.gameObject.GetComponent(type) as AvatarController);
			if (!this.avatarController)
			{
				this.avatarController = (base.gameObject.AddComponent(type) as AvatarController);
			}
		}
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x000FAA38 File Offset: 0x000F8C38
	public virtual void Detach()
	{
		for (int i = 0; i < this.modelTransformParent.childCount; i++)
		{
			UnityEngine.Object.Destroy(this.modelTransformParent.GetChild(i).gameObject);
		}
		UnityEngine.Object.Destroy(this.avatarController);
		UnityEngine.Object.Destroy(this);
		this.avatarController = null;
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x000FAA8C File Offset: 0x000F8C8C
	[Conditional("DEBUG_RAGDOLL")]
	public void LogRagdoll(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Ragdoll {1}, id{2}, {3}, {4}", new object[]
		{
			GameManager.frameCount,
			this.entity.GetDebugName(),
			this.entity.entityId,
			this.ragdollState,
			_format
		});
		Log.Warning(_format, _args);
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x000FAAF4 File Offset: 0x000F8CF4
	[Conditional("DEBUG_RAGDOLLDO")]
	public void LogRagdollDo(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Ragdoll Do {1}, id{2}, {3}, time {4}, {5}", new object[]
		{
			GameManager.frameCount,
			this.entity.GetDebugName(),
			this.entity.entityId,
			this.ragdollState,
			this.ragdollTime,
			_format
		});
		Log.Warning(_format, _args);
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x000FAB68 File Offset: 0x000F8D68
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckAnimFreeze()
	{
		if (EAIManager.isAnimFreeze)
		{
			EntityAlive entityAlive = this.entity as EntityAlive;
			if (entityAlive && entityAlive.aiManager != null && this.avatarController)
			{
				Animator animator = this.avatarController.GetAnimator();
				if (animator)
				{
					animator.enabled = false;
				}
			}
		}
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x000FABC0 File Offset: 0x000F8DC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHeadState()
	{
		if (this.headTransform == null)
		{
			return;
		}
		switch (this.HeadState)
		{
		case EModelBase.HeadStates.Standard:
			if (this.headTransform.localScale.x != this.HeadStandardSize)
			{
				this.headTransform.localScale = new Vector3(this.HeadStandardSize, this.HeadStandardSize, this.HeadStandardSize);
				return;
			}
			break;
		case EModelBase.HeadStates.Growing:
		{
			float num = this.headTransform.localScale.y + this.headScaleSpeed * Time.deltaTime;
			if (num >= this.HeadBigSize)
			{
				num = this.HeadBigSize;
				this.HeadState = EModelBase.HeadStates.BigHead;
				EntityAlive entityAlive = this.entity as EntityAlive;
				if (entityAlive != null)
				{
					entityAlive.CurrentHeadState = this.HeadState;
				}
			}
			this.headTransform.localScale = new Vector3(num, num, num);
			return;
		}
		case EModelBase.HeadStates.BigHead:
			if (this.headTransform.localScale.x != this.HeadBigSize)
			{
				this.headTransform.localScale = new Vector3(this.HeadBigSize, this.HeadBigSize, this.HeadBigSize);
			}
			break;
		case EModelBase.HeadStates.Shrinking:
		{
			float num2 = this.headTransform.localScale.y - this.headScaleSpeed * Time.deltaTime;
			if (num2 <= this.HeadStandardSize)
			{
				num2 = this.HeadStandardSize;
				this.HeadState = EModelBase.HeadStates.Standard;
				EntityAlive entityAlive2 = this.entity as EntityAlive;
				if (entityAlive2 != null)
				{
					entityAlive2.CurrentHeadState = this.HeadState;
				}
			}
			this.headTransform.localScale = new Vector3(num2, num2, num2);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x000FAD48 File Offset: 0x000F8F48
	public void ForceHeadState(EModelBase.HeadStates headState)
	{
		if (this.headTransform == null)
		{
			return;
		}
		this.HeadState = headState;
		if (headState != EModelBase.HeadStates.Standard)
		{
			if (headState == EModelBase.HeadStates.BigHead)
			{
				float headBigSize = this.HeadBigSize;
				this.headTransform.localScale = new Vector3(headBigSize, headBigSize, headBigSize);
				return;
			}
		}
		else
		{
			float headStandardSize = this.HeadStandardSize;
			this.headTransform.localScale = new Vector3(headStandardSize, headStandardSize, headStandardSize);
		}
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x000FADA7 File Offset: 0x000F8FA7
	public void SetHeadScale(float standard)
	{
		this.HeadStandardSize = standard;
		this.HeadBigSize = Mathf.Min(4.5f, standard * 3f);
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x000FADC8 File Offset: 0x000F8FC8
	[PublicizedFrom(EAccessModifier.Protected)]
	public EModelBase()
	{
	}

	// Token: 0x04001D24 RID: 7460
	public const string ExtFirstPerson = "_FP";

	// Token: 0x04001D25 RID: 7461
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cEmissiveColor = "_EmissiveColor";

	// Token: 0x04001D26 RID: 7462
	public AvatarController avatarController;

	// Token: 0x04001D27 RID: 7463
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelTransformParent;

	// Token: 0x04001D28 RID: 7464
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelTransform;

	// Token: 0x04001D29 RID: 7465
	public Transform meshTransform;

	// Token: 0x04001D2A RID: 7466
	public Transform bipedRootTransform;

	// Token: 0x04001D2B RID: 7467
	public Transform bipedPelvisTransform;

	// Token: 0x04001D2C RID: 7468
	public Rigidbody pelvisRB;

	// Token: 0x04001D2D RID: 7469
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform headTransform;

	// Token: 0x04001D2E RID: 7470
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform neckTransform;

	// Token: 0x04001D2F RID: 7471
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform neckParentTransform;

	// Token: 0x04001D30 RID: 7472
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public PhysicsBodyInstance physicsBody;

	// Token: 0x04001D31 RID: 7473
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string modelName;

	// Token: 0x04001D32 RID: 7474
	public Material AltMaterial;

	// Token: 0x04001D33 RID: 7475
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float ragdollChance;

	// Token: 0x04001D34 RID: 7476
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bHasRagdoll;

	// Token: 0x04001D35 RID: 7477
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Cloth[] clothSim;

	// Token: 0x04001D36 RID: 7478
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isClothSimOn = true;

	// Token: 0x04001D37 RID: 7479
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Jiggle[] jiggles;

	// Token: 0x04001D38 RID: 7480
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isJiggleOn = true;

	// Token: 0x04001D3A RID: 7482
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Entity entity;

	// Token: 0x04001D3B RID: 7483
	public EModelBase.HeadStates HeadState;

	// Token: 0x04001D3C RID: 7484
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float headScaleSpeed = 2f;

	// Token: 0x04001D3D RID: 7485
	public float HeadStandardSize = 1f;

	// Token: 0x04001D3E RID: 7486
	public float HeadBigSize = 3f;

	// Token: 0x04001D3F RID: 7487
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform NavObjectTransform;

	// Token: 0x04001D40 RID: 7488
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float ragdollAlignmentForce = 25f;

	// Token: 0x04001D41 RID: 7489
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float ragdollAlingmentDistance = 0.2f;

	// Token: 0x04001D42 RID: 7490
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float ragdollBlendPositionSpeed = 10f;

	// Token: 0x04001D43 RID: 7491
	public static float serverPosSpringForce = 50f;

	// Token: 0x04001D44 RID: 7492
	public static float serverPosSpringDamping = 0.1f;

	// Token: 0x04001D45 RID: 7493
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Renderer> rendererList = new List<Renderer>();

	// Token: 0x04001D46 RID: 7494
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<SkinnedMeshRenderer> skinnedRendererList = new List<SkinnedMeshRenderer>();

	// Token: 0x04001D47 RID: 7495
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLookAtSlerpPer = 0.16f;

	// Token: 0x04001D48 RID: 7496
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLookAtAnimBlend = 0.5f;

	// Token: 0x04001D49 RID: 7497
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool lookAtEnabled;

	// Token: 0x04001D4A RID: 7498
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtMaxAngle;

	// Token: 0x04001D4B RID: 7499
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lookAtIsPos;

	// Token: 0x04001D4C RID: 7500
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lookAtPos;

	// Token: 0x04001D4D RID: 7501
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion lookAtRot;

	// Token: 0x04001D4E RID: 7502
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtBlendPer;

	// Token: 0x04001D4F RID: 7503
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtBlendPerTarget;

	// Token: 0x04001D50 RID: 7504
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtFullChangeTime;

	// Token: 0x04001D51 RID: 7505
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtFullBlendPer = 1f;

	// Token: 0x04001D52 RID: 7506
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRadgollBlendOutGroundTime = 0.25f;

	// Token: 0x04001D53 RID: 7507
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRadgollBlendOutTime = 0.7f;

	// Token: 0x04001D54 RID: 7508
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRagdollMinDisableVel = 0.5f;

	// Token: 0x04001D55 RID: 7509
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRagdollDeadMaxDepentrationVel = 2f;

	// Token: 0x04001D56 RID: 7510
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRagdollDeadMaxAngularVel = 1f;

	// Token: 0x04001D57 RID: 7511
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cRadgollPlayerAnimLayer = 5;

	// Token: 0x04001D58 RID: 7512
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EModelBase.ERagdollState ragdollState;

	// Token: 0x04001D59 RID: 7513
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollTime;

	// Token: 0x04001D5A RID: 7514
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollDuration;

	// Token: 0x04001D5B RID: 7515
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator ragdollAnimator;

	// Token: 0x04001D5C RID: 7516
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollRotY;

	// Token: 0x04001D5D RID: 7517
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsBlending;

	// Token: 0x04001D5E RID: 7518
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollAdjustPosDelay;

	// Token: 0x04001D5F RID: 7519
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsPlayer;

	// Token: 0x04001D60 RID: 7520
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsAnimal;

	// Token: 0x04001D61 RID: 7521
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsFacingUp;

	// Token: 0x04001D62 RID: 7522
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<EModelBase.RagdollPose> ragdollPoses = new List<EModelBase.RagdollPose>();

	// Token: 0x04001D63 RID: 7523
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 ragdollPosePelvisPos;

	// Token: 0x04001D64 RID: 7524
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 ragdollPosePelvisLocalPos;

	// Token: 0x04001D65 RID: 7525
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<Rigidbody> ragdollTempRBs = new List<Rigidbody>();

	// Token: 0x04001D66 RID: 7526
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollZeroTime;

	// Token: 0x04001D67 RID: 7527
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Transform> ragdollZeroBones;

	// Token: 0x04001D68 RID: 7528
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string[] commonBips = new string[]
	{
		"Bip001",
		"Bip01"
	};

	// Token: 0x04001D6A RID: 7530
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MaterialPropertyBlock matPropBlock;

	// Token: 0x04001D6B RID: 7531
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int fadeId = Shader.PropertyToID("_Fade");

	// Token: 0x02000496 RID: 1174
	public enum HeadStates
	{
		// Token: 0x04001D6D RID: 7533
		Standard,
		// Token: 0x04001D6E RID: 7534
		Growing,
		// Token: 0x04001D6F RID: 7535
		BigHead,
		// Token: 0x04001D70 RID: 7536
		Shrinking
	}

	// Token: 0x02000497 RID: 1175
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ERagdollState
	{
		// Token: 0x04001D72 RID: 7538
		Off,
		// Token: 0x04001D73 RID: 7539
		On,
		// Token: 0x04001D74 RID: 7540
		BlendOutGround,
		// Token: 0x04001D75 RID: 7541
		BlendOutStand,
		// Token: 0x04001D76 RID: 7542
		Stand,
		// Token: 0x04001D77 RID: 7543
		StandCollide,
		// Token: 0x04001D78 RID: 7544
		SpawnWait,
		// Token: 0x04001D79 RID: 7545
		Dead
	}

	// Token: 0x02000498 RID: 1176
	public struct RagdollPose
	{
		// Token: 0x04001D7A RID: 7546
		public Transform t;

		// Token: 0x04001D7B RID: 7547
		public Rigidbody rb;

		// Token: 0x04001D7C RID: 7548
		public Quaternion startRot;

		// Token: 0x04001D7D RID: 7549
		public Quaternion rot;
	}
}
