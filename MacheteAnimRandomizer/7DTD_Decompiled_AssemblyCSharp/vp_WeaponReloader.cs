using System;
using UnityEngine;

// Token: 0x0200136B RID: 4971
public class vp_WeaponReloader : MonoBehaviour
{
	// Token: 0x06009BB7 RID: 39863 RVA: 0x003DE9F4 File Offset: 0x003DCBF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
	}

	// Token: 0x06009BB8 RID: 39864 RVA: 0x003DEA27 File Offset: 0x003DCC27
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.m_Weapon = base.transform.GetComponent<vp_Weapon>();
	}

	// Token: 0x06009BB9 RID: 39865 RVA: 0x003DEA3A File Offset: 0x003DCC3A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x06009BBA RID: 39866 RVA: 0x003DEA56 File Offset: 0x003DCC56
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x06009BBB RID: 39867 RVA: 0x003DEA74 File Offset: 0x003DCC74
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_Reload()
	{
		return this.m_Player.CurrentWeaponWielded.Get() && (this.m_Player.CurrentWeaponMaxAmmoCount.Get() == 0 || this.m_Player.CurrentWeaponAmmoCount.Get() != this.m_Player.CurrentWeaponMaxAmmoCount.Get()) && this.m_Player.CurrentWeaponClipCount.Get() >= 1;
	}

	// Token: 0x06009BBC RID: 39868 RVA: 0x003DEAFC File Offset: 0x003DCCFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Reload()
	{
		this.m_Player.Reload.AutoDuration = this.m_Player.CurrentWeaponReloadDuration.Get();
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = Time.timeScale;
			this.m_Audio.PlayOneShot(this.SoundReload);
		}
	}

	// Token: 0x06009BBD RID: 39869 RVA: 0x003DEB5D File Offset: 0x003DCD5D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_Reload()
	{
		this.m_Player.RefillCurrentWeapon.Try();
	}

	// Token: 0x17001037 RID: 4151
	// (get) Token: 0x06009BBE RID: 39870 RVA: 0x003DEB75 File Offset: 0x003DCD75
	public virtual float OnValue_CurrentWeaponReloadDuration
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.ReloadDuration;
		}
	}

	// Token: 0x04007864 RID: 30820
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Weapon m_Weapon;

	// Token: 0x04007865 RID: 30821
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;

	// Token: 0x04007866 RID: 30822
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x04007867 RID: 30823
	public AudioClip SoundReload;

	// Token: 0x04007868 RID: 30824
	public float ReloadDuration = 1f;
}
