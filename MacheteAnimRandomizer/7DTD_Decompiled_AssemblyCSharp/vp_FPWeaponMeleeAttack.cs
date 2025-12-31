using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001358 RID: 4952
public class vp_FPWeaponMeleeAttack : vp_Component
{
	// Token: 0x17000FF6 RID: 4086
	// (get) Token: 0x06009AAD RID: 39597 RVA: 0x003D8A00 File Offset: 0x003D6C00
	public vp_FPPlayerEventHandler Player
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_Player == null && base.EventHandler != null)
			{
				this.m_Player = (vp_FPPlayerEventHandler)base.EventHandler;
			}
			return this.m_Player;
		}
	}

	// Token: 0x06009AAE RID: 39598 RVA: 0x003D8A38 File Offset: 0x003D6C38
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.m_Controller = (vp_FPController)base.Root.GetComponent(typeof(vp_FPController));
		this.m_Camera = (vp_FPCamera)base.Root.GetComponentInChildren(typeof(vp_FPCamera));
		this.m_Weapon = (vp_FPWeapon)base.Transform.GetComponent(typeof(vp_FPWeapon));
	}

	// Token: 0x06009AAF RID: 39599 RVA: 0x003D8AAB File Offset: 0x003D6CAB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		this.UpdateAttack();
	}

	// Token: 0x06009AB0 RID: 39600 RVA: 0x003D8ABC File Offset: 0x003D6CBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateAttack()
	{
		if (!this.Player.Attack.Active)
		{
			return;
		}
		if (this.Player.SetWeapon.Active)
		{
			return;
		}
		if (this.m_Weapon == null)
		{
			return;
		}
		if (!this.m_Weapon.Wielded)
		{
			return;
		}
		if (Time.time < this.m_NextAllowedSwingTime)
		{
			return;
		}
		this.m_NextAllowedSwingTime = Time.time + this.SwingRate;
		if (this.AttackPickRandomState)
		{
			this.PickAttack();
		}
		this.m_Weapon.SetState(this.WeaponStatePull, true, false, false);
		this.m_Weapon.Refresh();
		vp_Timer.In(this.SwingDelay, delegate()
		{
			if (this.SoundSwing.Count > 0)
			{
				base.Audio.pitch = UnityEngine.Random.Range(this.SoundSwingPitch.x, this.SoundSwingPitch.y) * Time.timeScale;
				base.Audio.clip = (AudioClip)this.SoundSwing[UnityEngine.Random.Range(0, this.SoundSwing.Count)];
				base.Audio.Play();
			}
			this.m_Weapon.SetState(this.WeaponStatePull, false, false, false);
			this.m_Weapon.SetState(this.WeaponStateSwing, true, false, false);
			this.m_Weapon.Refresh();
			this.m_Weapon.AddSoftForce(this.SwingPositionSoftForce, this.SwingRotationSoftForce, this.SwingSoftForceFrames);
			vp_Timer.In(this.ImpactTime, delegate()
			{
				RaycastHit hit;
				Physics.SphereCast(new Ray(new Vector3(this.m_Controller.Transform.position.x, this.m_Camera.Transform.position.y, this.m_Controller.Transform.position.z), this.m_Camera.Transform.forward), this.DamageRadius, out hit, this.DamageRange, -538750981);
				if (hit.collider != null)
				{
					this.SpawnImpactFX(hit);
					this.ApplyDamage(hit);
					this.ApplyRecoil();
					return;
				}
				vp_Timer.In(this.SwingDuration - this.ImpactTime, delegate()
				{
					this.m_Weapon.StopSprings();
					this.Reset();
				}, this.SwingDurationTimer);
			}, this.ImpactTimer);
		}, this.SwingDelayTimer);
	}

	// Token: 0x06009AB1 RID: 39601 RVA: 0x003D8B78 File Offset: 0x003D6D78
	[PublicizedFrom(EAccessModifier.Private)]
	public void PickAttack()
	{
		int num = this.States.Count - 1;
		do
		{
			num = UnityEngine.Random.Range(0, this.States.Count - 1);
		}
		while (this.States.Count > 1 && num == this.m_AttackCurrent && UnityEngine.Random.value < 0.5f);
		this.m_AttackCurrent = num;
		base.SetState(this.States[this.m_AttackCurrent].Name, true, false, false);
	}

	// Token: 0x06009AB2 RID: 39602 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Attack()
	{
	}

	// Token: 0x06009AB3 RID: 39603 RVA: 0x003D8BF0 File Offset: 0x003D6DF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnImpactFX(RaycastHit hit)
	{
		Quaternion rotation = Quaternion.LookRotation(hit.normal);
		if (this.m_DustPrefab != null)
		{
			vp_Utility.Instantiate(this.m_DustPrefab, hit.point, rotation);
		}
		if (this.m_SparkPrefab != null && UnityEngine.Random.value < this.SparkFactor)
		{
			vp_Utility.Instantiate(this.m_SparkPrefab, hit.point, rotation);
		}
		if (this.m_DebrisPrefab != null)
		{
			vp_Utility.Instantiate(this.m_DebrisPrefab, hit.point, rotation);
		}
		if (this.SoundImpact.Count > 0)
		{
			base.Audio.pitch = UnityEngine.Random.Range(this.SoundImpactPitch.x, this.SoundImpactPitch.y) * Time.timeScale;
			base.Audio.PlayOneShot((AudioClip)this.SoundImpact[UnityEngine.Random.Range(0, this.SoundImpact.Count)]);
		}
	}

	// Token: 0x06009AB4 RID: 39604 RVA: 0x003D8CE4 File Offset: 0x003D6EE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyDamage(RaycastHit hit)
	{
		hit.collider.SendMessage(this.DamageMethodName, this.Damage, SendMessageOptions.DontRequireReceiver);
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody != null && !attachedRigidbody.isKinematic)
		{
			attachedRigidbody.AddForceAtPosition(this.m_Camera.Transform.forward * this.DamageForce / Time.timeScale / vp_TimeUtility.AdjustedTimeScale, hit.point);
		}
	}

	// Token: 0x06009AB5 RID: 39605 RVA: 0x003D8D6C File Offset: 0x003D6F6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyRecoil()
	{
		this.m_Weapon.StopSprings();
		this.m_Weapon.AddForce(this.ImpactPositionSpringRecoil, this.ImpactRotationSpringRecoil);
		this.m_Weapon.AddForce2(this.ImpactPositionSpring2Recoil, this.ImpactRotationSpring2Recoil);
		this.Reset();
	}

	// Token: 0x06009AB6 RID: 39606 RVA: 0x003D8DB8 File Offset: 0x003D6FB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Reset()
	{
		vp_Timer.In(0.05f, delegate()
		{
			if (this.m_Weapon != null)
			{
				this.m_Weapon.SetState(this.WeaponStatePull, false, false, false);
				this.m_Weapon.SetState(this.WeaponStateSwing, false, false, false);
				this.m_Weapon.Refresh();
				if (this.AttackPickRandomState)
				{
					base.ResetState();
				}
			}
		}, this.ResetTimer);
	}

	// Token: 0x0400772E RID: 30510
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPWeapon m_Weapon;

	// Token: 0x0400772F RID: 30511
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPController m_Controller;

	// Token: 0x04007730 RID: 30512
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPCamera m_Camera;

	// Token: 0x04007731 RID: 30513
	public string WeaponStatePull = "Pull";

	// Token: 0x04007732 RID: 30514
	public string WeaponStateSwing = "Swing";

	// Token: 0x04007733 RID: 30515
	public float SwingDelay = 0.5f;

	// Token: 0x04007734 RID: 30516
	public float SwingDuration = 0.5f;

	// Token: 0x04007735 RID: 30517
	public float SwingRate = 1f;

	// Token: 0x04007736 RID: 30518
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_NextAllowedSwingTime;

	// Token: 0x04007737 RID: 30519
	public int SwingSoftForceFrames = 50;

	// Token: 0x04007738 RID: 30520
	public Vector3 SwingPositionSoftForce = new Vector3(-0.5f, -0.1f, 0.3f);

	// Token: 0x04007739 RID: 30521
	public Vector3 SwingRotationSoftForce = new Vector3(50f, -25f, 0f);

	// Token: 0x0400773A RID: 30522
	public float ImpactTime = 0.11f;

	// Token: 0x0400773B RID: 30523
	public Vector3 ImpactPositionSpringRecoil = new Vector3(0.01f, 0.03f, -0.05f);

	// Token: 0x0400773C RID: 30524
	public Vector3 ImpactPositionSpring2Recoil = Vector3.zero;

	// Token: 0x0400773D RID: 30525
	public Vector3 ImpactRotationSpringRecoil = Vector3.zero;

	// Token: 0x0400773E RID: 30526
	public Vector3 ImpactRotationSpring2Recoil = new Vector3(0f, 0f, 10f);

	// Token: 0x0400773F RID: 30527
	public string DamageMethodName = "Damage";

	// Token: 0x04007740 RID: 30528
	public float Damage = 5f;

	// Token: 0x04007741 RID: 30529
	public float DamageRadius = 0.3f;

	// Token: 0x04007742 RID: 30530
	public float DamageRange = 2f;

	// Token: 0x04007743 RID: 30531
	public float DamageForce = 1000f;

	// Token: 0x04007744 RID: 30532
	public bool AttackPickRandomState = true;

	// Token: 0x04007745 RID: 30533
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_AttackCurrent;

	// Token: 0x04007746 RID: 30534
	public float SparkFactor = 0.1f;

	// Token: 0x04007747 RID: 30535
	public GameObject m_DustPrefab;

	// Token: 0x04007748 RID: 30536
	public GameObject m_SparkPrefab;

	// Token: 0x04007749 RID: 30537
	public GameObject m_DebrisPrefab;

	// Token: 0x0400774A RID: 30538
	public List<UnityEngine.Object> SoundSwing = new List<UnityEngine.Object>();

	// Token: 0x0400774B RID: 30539
	public List<UnityEngine.Object> SoundImpact = new List<UnityEngine.Object>();

	// Token: 0x0400774C RID: 30540
	public Vector2 SoundSwingPitch = new Vector2(0.5f, 1.5f);

	// Token: 0x0400774D RID: 30541
	public Vector2 SoundImpactPitch = new Vector2(1f, 1.5f);

	// Token: 0x0400774E RID: 30542
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle SwingDelayTimer = new vp_Timer.Handle();

	// Token: 0x0400774F RID: 30543
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle ImpactTimer = new vp_Timer.Handle();

	// Token: 0x04007750 RID: 30544
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle SwingDurationTimer = new vp_Timer.Handle();

	// Token: 0x04007751 RID: 30545
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle ResetTimer = new vp_Timer.Handle();

	// Token: 0x04007752 RID: 30546
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;
}
