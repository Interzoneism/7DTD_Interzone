using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200135E RID: 4958
public class vp_PlayerDamageHandler : vp_DamageHandler
{
	// Token: 0x17001010 RID: 4112
	// (get) Token: 0x06009B14 RID: 39700 RVA: 0x003DBA53 File Offset: 0x003D9C53
	public vp_PlayerEventHandler Player
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = base.transform.GetComponent<vp_PlayerEventHandler>();
			}
			return this.m_Player;
		}
	}

	// Token: 0x17001011 RID: 4113
	// (get) Token: 0x06009B15 RID: 39701 RVA: 0x003DBA7A File Offset: 0x003D9C7A
	public vp_PlayerInventory Inventory
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Inventory == null)
			{
				this.m_Inventory = base.transform.root.GetComponentInChildren<vp_PlayerInventory>();
			}
			return this.m_Inventory;
		}
	}

	// Token: 0x17001012 RID: 4114
	// (get) Token: 0x06009B16 RID: 39702 RVA: 0x003DBAA8 File Offset: 0x003D9CA8
	public List<Collider> Colliders
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Colliders == null)
			{
				this.m_Colliders = new List<Collider>();
				foreach (Collider collider in base.GetComponentsInChildren<Collider>())
				{
					if (collider.gameObject.layer == 23)
					{
						this.m_Colliders.Add(collider);
					}
				}
			}
			return this.m_Colliders;
		}
	}

	// Token: 0x06009B17 RID: 39703 RVA: 0x003DBB02 File Offset: 0x003D9D02
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		if (this.Player != null)
		{
			this.Player.Register(this);
		}
	}

	// Token: 0x06009B18 RID: 39704 RVA: 0x003DBB1E File Offset: 0x003D9D1E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		if (this.Player != null)
		{
			this.Player.Unregister(this);
		}
	}

	// Token: 0x06009B19 RID: 39705 RVA: 0x003DBB3A File Offset: 0x003D9D3A
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (this.Inventory != null)
		{
			this.m_InventoryWasEnabledAtStart = this.Inventory.enabled;
		}
	}

	// Token: 0x06009B1A RID: 39706 RVA: 0x003DBB5C File Offset: 0x003D9D5C
	public override void Die()
	{
		if (!base.enabled || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = Time.timeScale;
			this.m_Audio.PlayOneShot(this.DeathSound);
		}
		foreach (GameObject gameObject in this.DeathSpawnObjects)
		{
			if (gameObject != null)
			{
				vp_Utility.Instantiate(gameObject, base.transform.position, base.transform.rotation);
			}
		}
		foreach (Collider collider in this.Colliders)
		{
			collider.enabled = false;
		}
		if (this.Inventory != null && this.Inventory.enabled)
		{
			this.Inventory.enabled = false;
		}
		this.Player.SetWeapon.Argument = 0;
		this.Player.SetWeapon.Start(0f);
		this.Player.Dead.Start(0f);
		this.Player.Run.Stop(0f);
		this.Player.Jump.Stop(0f);
		this.Player.Crouch.Stop(0f);
		this.Player.Zoom.Stop(0f);
		this.Player.Attack.Stop(0f);
		this.Player.Reload.Stop(0f);
		this.Player.Climb.Stop(0f);
		this.Player.Interact.Stop(0f);
		if (vp_Gameplay.isMultiplayer && vp_Gameplay.isMaster)
		{
			vp_GlobalEvent<Transform>.Send("Kill", base.transform.root);
		}
	}

	// Token: 0x06009B1B RID: 39707 RVA: 0x003DBD64 File Offset: 0x003D9F64
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Reset()
	{
		base.Reset();
		if (!Application.isPlaying)
		{
			return;
		}
		this.Player.Dead.Stop(0f);
		this.Player.Stop.Send();
		foreach (Collider collider in this.Colliders)
		{
			collider.enabled = true;
		}
		if (this.Inventory != null && !this.Inventory.enabled)
		{
			this.Inventory.enabled = this.m_InventoryWasEnabledAtStart;
		}
		if (this.m_Audio != null)
		{
			this.m_Audio.pitch = Time.timeScale;
			this.m_Audio.PlayOneShot(this.RespawnSound);
		}
	}

	// Token: 0x17001013 RID: 4115
	// (get) Token: 0x06009B1C RID: 39708 RVA: 0x003DBE48 File Offset: 0x003DA048
	// (set) Token: 0x06009B1D RID: 39709 RVA: 0x003DBE50 File Offset: 0x003DA050
	public virtual float OnValue_Health
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.CurrentHealth;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.CurrentHealth = Mathf.Min(value, this.MaxHealth);
		}
	}

	// Token: 0x17001014 RID: 4116
	// (get) Token: 0x06009B1E RID: 39710 RVA: 0x003DBE64 File Offset: 0x003DA064
	public virtual float OnValue_MaxHealth
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.MaxHealth;
		}
	}

	// Token: 0x06009B1F RID: 39711 RVA: 0x003DBE6C File Offset: 0x003DA06C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_FallImpact(float impact)
	{
		if (this.Player.Dead.Active || !this.AllowFallDamage || impact <= this.FallImpactThreshold)
		{
			return;
		}
		vp_AudioUtility.PlayRandomSound(this.m_Audio, this.FallImpactSounds, this.FallImpactPitch);
		float damage = Mathf.Abs(this.DeathOnFallImpactThreshold ? this.MaxHealth : (this.MaxHealth * impact));
		this.Damage(damage);
	}

	// Token: 0x040077C3 RID: 30659
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;

	// Token: 0x040077C4 RID: 30660
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_PlayerInventory m_Inventory;

	// Token: 0x040077C5 RID: 30661
	public bool AllowFallDamage = true;

	// Token: 0x040077C6 RID: 30662
	public float FallImpactThreshold = 0.15f;

	// Token: 0x040077C7 RID: 30663
	public bool DeathOnFallImpactThreshold;

	// Token: 0x040077C8 RID: 30664
	public Vector2 FallImpactPitch = new Vector2(1f, 1.5f);

	// Token: 0x040077C9 RID: 30665
	public List<AudioClip> FallImpactSounds = new List<AudioClip>();

	// Token: 0x040077CA RID: 30666
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_FallImpactMultiplier = 2f;

	// Token: 0x040077CB RID: 30667
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_InventoryWasEnabledAtStart = true;

	// Token: 0x040077CC RID: 30668
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<Collider> m_Colliders;
}
