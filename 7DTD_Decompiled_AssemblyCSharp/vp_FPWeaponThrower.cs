using System;
using UnityEngine;

// Token: 0x0200135B RID: 4955
public class vp_FPWeaponThrower : vp_WeaponThrower
{
	// Token: 0x17000FF9 RID: 4089
	// (get) Token: 0x06009AC9 RID: 39625 RVA: 0x003D9735 File Offset: 0x003D7935
	public vp_FPWeapon FPWeapon
	{
		get
		{
			if (this.m_FPWeapon == null)
			{
				this.m_FPWeapon = (vp_FPWeapon)base.Transform.GetComponent(typeof(vp_FPWeapon));
			}
			return this.m_FPWeapon;
		}
	}

	// Token: 0x17000FFA RID: 4090
	// (get) Token: 0x06009ACA RID: 39626 RVA: 0x003D976B File Offset: 0x003D796B
	public vp_FPWeaponShooter FPWeaponShooter
	{
		get
		{
			if (this.m_FPWeaponShooter == null)
			{
				this.m_FPWeaponShooter = (vp_FPWeaponShooter)base.Transform.GetComponent(typeof(vp_FPWeaponShooter));
			}
			return this.m_FPWeaponShooter;
		}
	}

	// Token: 0x17000FFB RID: 4091
	// (get) Token: 0x06009ACB RID: 39627 RVA: 0x003D97A4 File Offset: 0x003D79A4
	public Transform FirePosition
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_FirePosition == null)
			{
				GameObject gameObject = new GameObject("ThrownWeaponFirePosition");
				this.m_FirePosition = gameObject.transform;
				this.m_FirePosition.parent = Camera.main.transform;
			}
			return this.m_FirePosition;
		}
	}

	// Token: 0x06009ACC RID: 39628 RVA: 0x003D97F1 File Offset: 0x003D79F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.m_OriginalLookDownActive = this.FPWeapon.LookDownActive;
	}

	// Token: 0x06009ACD RID: 39629 RVA: 0x003D980C File Offset: 0x003D7A0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void RewindAnimation()
	{
		if (!base.Player.IsFirstPerson.Get())
		{
			return;
		}
		if (this.FPWeapon == null)
		{
			return;
		}
		if (this.FPWeapon.WeaponModel == null)
		{
			return;
		}
		if (this.FPWeapon.WeaponModel.GetComponent<Animation>() == null)
		{
			return;
		}
		if (this.FPWeaponShooter == null)
		{
			return;
		}
		if (this.FPWeaponShooter.AnimationFire == null)
		{
			return;
		}
		this.FPWeapon.WeaponModel.GetComponent<Animation>()[this.FPWeaponShooter.AnimationFire.name].time = 0f;
		this.FPWeapon.WeaponModel.GetComponent<Animation>().Play();
		this.FPWeapon.WeaponModel.GetComponent<Animation>().Sample();
		this.FPWeapon.WeaponModel.GetComponent<Animation>().Stop();
	}

	// Token: 0x06009ACE RID: 39630 RVA: 0x003D9900 File Offset: 0x003D7B00
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnStart_Attack()
	{
		base.OnStart_Attack();
		if (base.Player.IsFirstPerson.Get())
		{
			base.Shooter.m_ProjectileSpawnPoint = this.FirePosition.gameObject;
			this.FirePosition.localPosition = this.FirePositionOffset;
			this.FirePosition.localEulerAngles = Vector3.zero;
		}
		else
		{
			base.Shooter.m_ProjectileSpawnPoint = base.Weapon.Weapon3rdPersonModel;
		}
		this.FPWeapon.LookDownActive = false;
		vp_Timer.In(base.Shooter.ProjectileSpawnDelay, delegate()
		{
			if (!base.HaveAmmoForCurrentWeapon)
			{
				this.FPWeapon.SetState("ReWield", true, false, false);
				this.FPWeapon.Refresh();
				vp_Timer.In(1f, delegate()
				{
					if (!base.Player.SetNextWeapon.Try())
					{
						vp_Timer.In(0.5f, delegate()
						{
							this.RewindAnimation();
							base.Player.SetWeapon.Start(0f);
						}, this.m_Timer2);
					}
				}, null);
				return;
			}
			if (base.Player.IsFirstPerson.Get())
			{
				this.FPWeapon.SetState("ReWield", true, false, false);
				this.FPWeapon.Refresh();
				vp_Timer.In(1f, delegate()
				{
					this.RewindAnimation();
					this.FPWeapon.Rendering = true;
					this.FPWeapon.SetState("ReWield", false, false, false);
					this.FPWeapon.Refresh();
				}, this.m_Timer3);
				return;
			}
			vp_Timer.In(0.5f, delegate()
			{
				base.Player.Attack.Stop(0f);
			}, this.m_Timer4);
		}, this.m_Timer1);
	}

	// Token: 0x06009ACF RID: 39631 RVA: 0x003D99A7 File Offset: 0x003D7BA7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_SetWeapon()
	{
		this.RewindAnimation();
	}

	// Token: 0x06009AD0 RID: 39632 RVA: 0x003D99AF File Offset: 0x003D7BAF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnStop_Attack()
	{
		base.OnStop_Attack();
		this.FPWeapon.LookDownActive = this.m_OriginalLookDownActive;
	}

	// Token: 0x0400775D RID: 30557
	public Vector3 FirePositionOffset = new Vector3(0.35f, 0f, 0f);

	// Token: 0x0400775E RID: 30558
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_OriginalLookDownActive;

	// Token: 0x0400775F RID: 30559
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_Timer1 = new vp_Timer.Handle();

	// Token: 0x04007760 RID: 30560
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_Timer2 = new vp_Timer.Handle();

	// Token: 0x04007761 RID: 30561
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_Timer3 = new vp_Timer.Handle();

	// Token: 0x04007762 RID: 30562
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_Timer4 = new vp_Timer.Handle();

	// Token: 0x04007763 RID: 30563
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPWeapon m_FPWeapon;

	// Token: 0x04007764 RID: 30564
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPWeaponShooter m_FPWeaponShooter;

	// Token: 0x04007765 RID: 30565
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_FirePosition;
}
