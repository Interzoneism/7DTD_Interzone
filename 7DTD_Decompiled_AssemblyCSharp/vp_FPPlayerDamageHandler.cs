using System;
using UnityEngine;

// Token: 0x02001354 RID: 4948
[RequireComponent(typeof(vp_FPPlayerEventHandler))]
public class vp_FPPlayerDamageHandler : vp_PlayerDamageHandler
{
	// Token: 0x17000FEC RID: 4076
	// (get) Token: 0x06009A66 RID: 39526 RVA: 0x003D617A File Offset: 0x003D437A
	public vp_FPPlayerEventHandler FPPlayer
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_FPPlayer == null)
			{
				this.m_FPPlayer = base.transform.GetComponent<vp_FPPlayerEventHandler>();
			}
			return this.m_FPPlayer;
		}
	}

	// Token: 0x17000FED RID: 4077
	// (get) Token: 0x06009A67 RID: 39527 RVA: 0x003D61A1 File Offset: 0x003D43A1
	public vp_FPCamera FPCamera
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_FPCamera == null)
			{
				this.m_FPCamera = base.transform.GetComponentInChildren<vp_FPCamera>();
			}
			return this.m_FPCamera;
		}
	}

	// Token: 0x17000FEE RID: 4078
	// (get) Token: 0x06009A68 RID: 39528 RVA: 0x003D61C8 File Offset: 0x003D43C8
	public CharacterController CharacterController
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_CharacterController == null)
			{
				this.m_CharacterController = base.transform.root.GetComponentInChildren<CharacterController>();
			}
			return this.m_CharacterController;
		}
	}

	// Token: 0x06009A69 RID: 39529 RVA: 0x003D61F4 File Offset: 0x003D43F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		if (this.FPPlayer != null)
		{
			this.FPPlayer.Register(this);
		}
		this.RefreshColliders();
	}

	// Token: 0x06009A6A RID: 39530 RVA: 0x003D6216 File Offset: 0x003D4416
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		if (this.FPPlayer != null)
		{
			this.FPPlayer.Unregister(this);
		}
	}

	// Token: 0x06009A6B RID: 39531 RVA: 0x003D6232 File Offset: 0x003D4432
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (this.FPPlayer.Dead.Active && Time.timeScale < 1f)
		{
			vp_TimeUtility.FadeTimeScale(1f, 0.05f);
		}
	}

	// Token: 0x06009A6C RID: 39532 RVA: 0x003D6264 File Offset: 0x003D4464
	public override void Damage(float damage)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		base.Damage(damage);
		this.FPPlayer.HUDDamageFlash.Send(new vp_DamageInfo(damage, null));
		this.FPPlayer.HeadImpact.Send((UnityEngine.Random.value < 0.5f) ? (damage * this.CameraShakeFactor) : (-(damage * this.CameraShakeFactor)));
	}

	// Token: 0x06009A6D RID: 39533 RVA: 0x003D62E0 File Offset: 0x003D44E0
	public override void Damage(vp_DamageInfo damageInfo)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		base.Damage(damageInfo);
		this.FPPlayer.HUDDamageFlash.Send(damageInfo);
		if (damageInfo.Source != null)
		{
			this.m_DamageAngle = vp_3DUtility.LookAtAngleHorizontal(this.FPCamera.Transform.position, this.FPCamera.Transform.forward, damageInfo.Source.position);
			this.m_DamageAngleFactor = ((Mathf.Abs(this.m_DamageAngle) > 30f) ? 1f : Mathf.Lerp(0f, 1f, Mathf.Abs(this.m_DamageAngle) * 0.033f));
			this.FPPlayer.HeadImpact.Send(damageInfo.Damage * this.CameraShakeFactor * this.m_DamageAngleFactor * (float)((this.m_DamageAngle < 0f) ? 1 : -1));
		}
	}

	// Token: 0x06009A6E RID: 39534 RVA: 0x003D63E3 File Offset: 0x003D45E3
	public override void Die()
	{
		base.Die();
		if (!base.enabled || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		this.FPPlayer.InputAllowGameplay.Set(false);
	}

	// Token: 0x06009A6F RID: 39535 RVA: 0x003D6418 File Offset: 0x003D4618
	public virtual void RefreshColliders()
	{
		if (this.CharacterController != null && this.CharacterController.enabled)
		{
			foreach (Collider collider in base.Colliders)
			{
				if (collider.enabled)
				{
					Physics.IgnoreCollision(this.CharacterController, collider, true);
				}
			}
		}
	}

	// Token: 0x06009A70 RID: 39536 RVA: 0x003D6494 File Offset: 0x003D4694
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Reset()
	{
		base.Reset();
		if (!Application.isPlaying)
		{
			return;
		}
		this.FPPlayer.InputAllowGameplay.Set(true);
		this.FPPlayer.HUDDamageFlash.Send(null);
		this.RefreshColliders();
	}

	// Token: 0x06009A71 RID: 39537 RVA: 0x003D64E1 File Offset: 0x003D46E1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnStart_Crouch()
	{
		this.RefreshColliders();
	}

	// Token: 0x06009A72 RID: 39538 RVA: 0x003D64E1 File Offset: 0x003D46E1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnStop_Crouch()
	{
		this.RefreshColliders();
	}

	// Token: 0x040076B0 RID: 30384
	public float CameraShakeFactor = 0.02f;

	// Token: 0x040076B1 RID: 30385
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_DamageAngle;

	// Token: 0x040076B2 RID: 30386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float m_DamageAngleFactor = 1f;

	// Token: 0x040076B3 RID: 30387
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_FPPlayer;

	// Token: 0x040076B4 RID: 30388
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPCamera m_FPCamera;

	// Token: 0x040076B5 RID: 30389
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public CharacterController m_CharacterController;
}
