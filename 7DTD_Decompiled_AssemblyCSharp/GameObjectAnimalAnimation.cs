using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000C5 RID: 197
[Preserve]
public class GameObjectAnimalAnimation : AvatarController
{
	// Token: 0x060004D4 RID: 1236 RVA: 0x00022C34 File Offset: 0x00020E34
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.parentT = EModelBase.FindModel(base.transform);
		for (int i = this.parentT.childCount - 1; i >= 0; i--)
		{
			this.figureT = this.parentT.GetChild(i);
			if (this.figureT.gameObject.activeSelf)
			{
				break;
			}
		}
		this.anim = this.figureT.GetComponent<Animation>();
		if (this.anim["Idle1"])
		{
			this.anim.Play("Idle1");
		}
		this.attack1AS = this.anim["Attack1"];
		this.attack2AS = this.anim["Attack2"];
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00022CF9 File Offset: 0x00020EF9
	public void SetAlwaysWalk(bool _b)
	{
		this.bAlwaysWalk = _b;
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00022D02 File Offset: 0x00020F02
	public override bool IsAnimationAttackPlaying()
	{
		return (this.attack1AS != null && this.attack1AS.enabled) || (this.attack2AS != null && this.attack2AS.enabled);
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00022D3C File Offset: 0x00020F3C
	public override void StartAnimationAttack()
	{
		this.state = GameObjectAnimalAnimation.State.Attack;
		if (this.attack1AS != null)
		{
			if (this.attack2AS != null)
			{
				if (this.entity.rand.RandomFloat > 0.5f)
				{
					this.anim.Play("Attack1");
					return;
				}
				this.anim.Play("Attack2");
				return;
			}
			else
			{
				this.anim.Play("Attack1");
			}
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00022DB8 File Offset: 0x00020FB8
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
		if (this.isDead)
		{
			return;
		}
		this.state = GameObjectAnimalAnimation.State.Pain;
		if (this.anim["Pain"])
		{
			this.anim.Play("Pain");
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00022DF4 File Offset: 0x00020FF4
	public override void StartAnimationJumping()
	{
		if (!this.entity.IsSwimming() && this.anim["Jump"] != null)
		{
			this.state = GameObjectAnimalAnimation.State.Jump;
			this.anim.CrossFade("Jump", 0.2f);
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00022E44 File Offset: 0x00021044
	public override void SetVisible(bool _b)
	{
		if (this.m_bVisible != _b || !this.visInit)
		{
			this.m_bVisible = _b;
			this.visInit = true;
			Transform transform = this.parentT;
			if (transform)
			{
				Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = _b;
				}
			}
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00022EA0 File Offset: 0x000210A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		if (!this.m_bVisible)
		{
			return;
		}
		if (this.entity == null)
		{
			return;
		}
		if (this.entity.IsDead())
		{
			if (!this.isDead)
			{
				this.isDead = true;
				this.anim.Stop();
				if (this.anim["Death"])
				{
					this.anim.CrossFade("Death", 0.5f);
				}
			}
			return;
		}
		if (!this.entity.Jumping && (this.attack1AS == null || !this.attack1AS.enabled) && (this.attack2AS == null || !this.attack2AS.enabled) && (this.anim["Death"] == null || !this.anim["Death"].enabled) && (this.anim["Pain"] == null || !this.anim["Pain"].enabled))
		{
			float num = this.lastAbsMotion;
			float num2 = Mathf.Abs(this.entity.position.x - this.entity.lastTickPos[0].x) * 6f;
			float num3 = Mathf.Abs(this.entity.position.z - this.entity.lastTickPos[0].z) * 6f;
			if (!this.entity.isEntityRemote)
			{
				if (Mathf.Abs(num2 - this.lastAbsMotionX) > 0.01f || Mathf.Abs(num3 - this.lastAbsMotionZ) > 0.01f)
				{
					num = Mathf.Sqrt(num2 * num2 + num3 * num3);
					this.lastAbsMotionX = num2;
					this.lastAbsMotionZ = num3;
					this.lastAbsMotion = num;
				}
			}
			else if (num2 > this.lastAbsMotionX || num3 > this.lastAbsMotionZ)
			{
				num = Mathf.Sqrt(num2 * num2 + num3 * num3);
				this.lastAbsMotionX = num2;
				this.lastAbsMotionZ = num3;
				this.lastAbsMotion = num;
			}
			else
			{
				this.lastAbsMotionX *= 0.9f;
				this.lastAbsMotionZ *= 0.9f;
				this.lastAbsMotion *= 0.9f;
			}
			if (this.bAlwaysWalk || num > 0.15f)
			{
				if (this.entity.IsSwimming() && this.anim["Swim"] != null)
				{
					this.state = GameObjectAnimalAnimation.State.Swim;
					if (!this.anim["Swim"].enabled)
					{
						this.anim.Play("Swim");
					}
					this.anim["Swim"].speed = Mathf.Clamp01(num * 2f);
					return;
				}
				if (num >= 1f)
				{
					if (this.state != GameObjectAnimalAnimation.State.Run)
					{
						this.state = GameObjectAnimalAnimation.State.Run;
						AnimationState animationState = this.anim["Run"];
						if (!animationState.enabled)
						{
							this.anim.CrossFade("Run", 0.5f);
						}
						animationState.speed = Utils.FastMin(num, 1.5f);
					}
				}
				else if (this.state != GameObjectAnimalAnimation.State.Run)
				{
					this.state = GameObjectAnimalAnimation.State.Walk;
					AnimationState animationState2 = this.anim["Walk"];
					if (!animationState2.enabled)
					{
						this.anim.CrossFade("Walk", 0.5f);
					}
					animationState2.speed = num * 2f;
				}
				if (this.stepSoundCounter <= 0f)
				{
					this.stepSoundCounter = 0.3f;
					return;
				}
			}
			else
			{
				this.state = GameObjectAnimalAnimation.State.Idle;
				if (this.anim["Idle2"] != null)
				{
					if (!this.anim["Idle1"].enabled && !this.anim["Idle2"].enabled)
					{
						if (this.entity.rand.RandomFloat > 0.5f)
						{
							this.anim.CrossFade("Idle1", 0.5f);
							return;
						}
						this.anim.CrossFade("Idle2", 0.5f);
						return;
					}
				}
				else if (!this.anim["Idle1"].enabled)
				{
					this.anim.CrossFade("Idle1", 0.5f);
				}
			}
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00023311 File Offset: 0x00021511
	public override Transform GetActiveModelRoot()
	{
		return this.figureT;
	}

	// Token: 0x04000579 RID: 1401
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimIdle1 = "Idle1";

	// Token: 0x0400057A RID: 1402
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimIdle2 = "Idle2";

	// Token: 0x0400057B RID: 1403
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimAttack1 = "Attack1";

	// Token: 0x0400057C RID: 1404
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimAttack2 = "Attack2";

	// Token: 0x0400057D RID: 1405
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimPain = "Pain";

	// Token: 0x0400057E RID: 1406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimJump = "Jump";

	// Token: 0x0400057F RID: 1407
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimDeath = "Death";

	// Token: 0x04000580 RID: 1408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimRun = "Run";

	// Token: 0x04000581 RID: 1409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimWalk = "Walk";

	// Token: 0x04000582 RID: 1410
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cAnimSwim = "Swim";

	// Token: 0x04000583 RID: 1411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform parentT;

	// Token: 0x04000584 RID: 1412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform figureT;

	// Token: 0x04000585 RID: 1413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public new Animation anim;

	// Token: 0x04000586 RID: 1414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AnimationState attack1AS;

	// Token: 0x04000587 RID: 1415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AnimationState attack2AS;

	// Token: 0x04000588 RID: 1416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool visInit;

	// Token: 0x04000589 RID: 1417
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_bVisible;

	// Token: 0x0400058A RID: 1418
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDead;

	// Token: 0x0400058B RID: 1419
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bAlwaysWalk;

	// Token: 0x0400058C RID: 1420
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAbsMotionX;

	// Token: 0x0400058D RID: 1421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAbsMotionZ;

	// Token: 0x0400058E RID: 1422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAbsMotion;

	// Token: 0x0400058F RID: 1423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stepSoundCounter;

	// Token: 0x04000590 RID: 1424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObjectAnimalAnimation.State state;

	// Token: 0x020000C6 RID: 198
	[PublicizedFrom(EAccessModifier.Private)]
	public enum State
	{
		// Token: 0x04000592 RID: 1426
		None,
		// Token: 0x04000593 RID: 1427
		Attack,
		// Token: 0x04000594 RID: 1428
		Idle,
		// Token: 0x04000595 RID: 1429
		Jump,
		// Token: 0x04000596 RID: 1430
		Pain,
		// Token: 0x04000597 RID: 1431
		Run,
		// Token: 0x04000598 RID: 1432
		Swim,
		// Token: 0x04000599 RID: 1433
		Walk
	}
}
