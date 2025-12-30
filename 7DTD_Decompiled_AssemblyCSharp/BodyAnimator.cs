using System;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class BodyAnimator
{
	// Token: 0x1700004F RID: 79
	// (get) Token: 0x0600048A RID: 1162 RVA: 0x00020A79 File Offset: 0x0001EC79
	// (set) Token: 0x0600048B RID: 1163 RVA: 0x00020A95 File Offset: 0x0001EC95
	public Animator Animator
	{
		get
		{
			if (!this.bodyParts.BodyObj.activeInHierarchy)
			{
				return null;
			}
			return this.animator;
		}
		set
		{
			if (this.bodyParts.BodyObj.activeInHierarchy)
			{
				this.animator = value;
			}
		}
	}

	// Token: 0x17000050 RID: 80
	// (set) Token: 0x0600048C RID: 1164 RVA: 0x00020AB0 File Offset: 0x0001ECB0
	public BodyAnimator.EnumState State
	{
		set
		{
			if (this.state != value)
			{
				this.state = value;
				this.bodyParts.BodyObj.SetActive(this.state != BodyAnimator.EnumState.Disabled);
				this.updateVisibility();
				if (this.state != BodyAnimator.EnumState.Disabled)
				{
					this.animator = this.bodyParts.BodyObj.GetComponentInChildren<Animator>();
				}
			}
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x0600048D RID: 1165 RVA: 0x00020B0E File Offset: 0x0001ED0E
	public BodyAnimator.BodyParts Parts
	{
		get
		{
			return this.bodyParts;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x0600048E RID: 1166 RVA: 0x00020B16 File Offset: 0x0001ED16
	// (set) Token: 0x0600048F RID: 1167 RVA: 0x00020B20 File Offset: 0x0001ED20
	public bool RagdollActive
	{
		get
		{
			return this.isRagdoll;
		}
		set
		{
			if (this.isRagdoll != value)
			{
				this.isRagdoll = value;
				if (this.animator)
				{
					this.animator.cullingMode = (this.isRagdoll ? AnimatorCullingMode.AlwaysAnimate : this.defaultCullingMode);
					this.animator.enabled = !this.isRagdoll;
				}
			}
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00020B7C File Offset: 0x0001ED7C
	public virtual void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
		if (this.avatarController != null)
		{
			this.avatarController.UpdateInt(AvatarController.movementStateHash, _movementState, true);
			this.avatarController.UpdateBool(AvatarController.isAliveHash, false, true);
			this.avatarController.UpdateBool(AvatarController.isDeadHash, true, true);
			this.avatarController.UpdateInt(AvatarController.hitBodyPartHash, (int)_bodyPart.ToPrimary().LowerToUpperLimb(), true);
			this.avatarController.UpdateFloat("HitRandomValue", random, true);
			this.avatarController.TriggerEvent("DeathTrigger");
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00020C0C File Offset: 0x0001EE0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void initBodyAnimator(EntityAlive _entity, BodyAnimator.BodyParts _bodyParts, BodyAnimator.EnumState _defaultState)
	{
		this.Entity = _entity;
		this.bodyParts = _bodyParts;
		this.state = _defaultState;
		this.animator = this.bodyParts.BodyObj.GetComponentInChildren<Animator>();
		this.defaultCullingMode = AnimatorCullingMode.AlwaysAnimate;
		this.meshes = this.bodyParts.BodyObj.GetComponentsInChildren<MeshRenderer>();
		this.skinnedMeshes = this.bodyParts.BodyObj.GetComponentsInChildren<SkinnedMeshRenderer>();
		if (this.Entity.emodel != null)
		{
			this.avatarController = this.Entity.emodel.avatarController;
		}
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00020CA0 File Offset: 0x0001EEA0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void cacheLayerStateInfo()
	{
		if (this.animator && this.animator.gameObject.activeInHierarchy)
		{
			this.currentBaseState = this.animator.GetCurrentAnimatorStateInfo(0);
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00020CD3 File Offset: 0x0001EED3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual AnimatorStateInfo getCachedLayerStateInfo(int _layer)
	{
		return this.currentBaseState;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00020CDC File Offset: 0x0001EEDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateVisibility()
	{
		if (this.state != BodyAnimator.EnumState.Disabled)
		{
			bool enabled = this.state == BodyAnimator.EnumState.Visible;
			if (this.meshes != null)
			{
				MeshRenderer[] array = this.meshes;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = enabled;
				}
			}
			if (this.skinnedMeshes != null)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.skinnedMeshes)
				{
					if (skinnedMeshRenderer)
					{
						skinnedMeshRenderer.enabled = enabled;
					}
				}
			}
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void assignLayerWeights()
	{
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetDrunk(float _numBeers)
	{
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00020D55 File Offset: 0x0001EF55
	public virtual void Update()
	{
		this.assignLayerWeights();
		this.updateVisibility();
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00020D63 File Offset: 0x0001EF63
	public virtual void LateUpdate()
	{
		this.cacheLayerStateInfo();
	}

	// Token: 0x040004D5 RID: 1237
	public EntityAlive Entity;

	// Token: 0x040004D6 RID: 1238
	[PublicizedFrom(EAccessModifier.Private)]
	public Animator animator;

	// Token: 0x040004D7 RID: 1239
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimatorStateInfo currentBaseState;

	// Token: 0x040004D8 RID: 1240
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarController avatarController;

	// Token: 0x040004D9 RID: 1241
	[PublicizedFrom(EAccessModifier.Private)]
	public BodyAnimator.BodyParts bodyParts;

	// Token: 0x040004DA RID: 1242
	[PublicizedFrom(EAccessModifier.Private)]
	public BodyAnimator.EnumState state;

	// Token: 0x040004DB RID: 1243
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimatorCullingMode defaultCullingMode;

	// Token: 0x040004DC RID: 1244
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isRagdoll;

	// Token: 0x040004DD RID: 1245
	[PublicizedFrom(EAccessModifier.Private)]
	public SkinnedMeshRenderer[] skinnedMeshes;

	// Token: 0x040004DE RID: 1246
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshRenderer[] meshes;

	// Token: 0x020000BA RID: 186
	public enum EnumState
	{
		// Token: 0x040004E0 RID: 1248
		Visible,
		// Token: 0x040004E1 RID: 1249
		OnlyColliders,
		// Token: 0x040004E2 RID: 1250
		Disabled
	}

	// Token: 0x020000BB RID: 187
	public class BodyParts
	{
		// Token: 0x0600049A RID: 1178 RVA: 0x00020D6B File Offset: 0x0001EF6B
		public BodyParts(Transform _bodyTransform, Transform _rightHand)
		{
			this.BodyObj = _bodyTransform.gameObject;
			this.RightHandT = _rightHand;
		}

		// Token: 0x040004E3 RID: 1251
		public GameObject BodyObj;

		// Token: 0x040004E4 RID: 1252
		public Transform RightHandT;
	}
}
