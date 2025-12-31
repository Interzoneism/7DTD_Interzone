using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02001302 RID: 4866
public class vp_DamageHandler : MonoBehaviour
{
	// Token: 0x17000F7A RID: 3962
	// (get) Token: 0x06009790 RID: 38800 RVA: 0x003C5249 File Offset: 0x003C3449
	public static Dictionary<Collider, vp_DamageHandler> DamageHandlersByCollider
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (vp_DamageHandler.m_DamageHandlersByCollider == null)
			{
				vp_DamageHandler.m_DamageHandlersByCollider = new Dictionary<Collider, vp_DamageHandler>(100);
			}
			return vp_DamageHandler.m_DamageHandlersByCollider;
		}
	}

	// Token: 0x17000F7B RID: 3963
	// (get) Token: 0x06009791 RID: 38801 RVA: 0x003C5263 File Offset: 0x003C3463
	public Transform Transform
	{
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x17000F7C RID: 3964
	// (get) Token: 0x06009792 RID: 38802 RVA: 0x003C5285 File Offset: 0x003C3485
	public vp_Respawner Respawner
	{
		get
		{
			if (this.m_Respawner == null)
			{
				this.m_Respawner = base.GetComponent<vp_Respawner>();
			}
			return this.m_Respawner;
		}
	}

	// Token: 0x17000F7D RID: 3965
	// (get) Token: 0x06009793 RID: 38803 RVA: 0x003C52A7 File Offset: 0x003C34A7
	// (set) Token: 0x06009794 RID: 38804 RVA: 0x003C52C9 File Offset: 0x003C34C9
	public Transform Source
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Source == null)
			{
				this.m_Source = this.Transform;
			}
			return this.m_Source;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_Source = value;
		}
	}

	// Token: 0x17000F7E RID: 3966
	// (get) Token: 0x06009795 RID: 38805 RVA: 0x003C52D2 File Offset: 0x003C34D2
	// (set) Token: 0x06009796 RID: 38806 RVA: 0x003C52F4 File Offset: 0x003C34F4
	public Transform OriginalSource
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_OriginalSource == null)
			{
				this.m_OriginalSource = this.Transform;
			}
			return this.m_OriginalSource;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_OriginalSource = value;
		}
	}

	// Token: 0x17000F7F RID: 3967
	// (get) Token: 0x06009797 RID: 38807 RVA: 0x003C52FD File Offset: 0x003C34FD
	// (set) Token: 0x06009798 RID: 38808 RVA: 0x003C5305 File Offset: 0x003C3505
	[Obsolete("This property will be removed in an upcoming release.")]
	public Transform Sender
	{
		get
		{
			return this.Source;
		}
		set
		{
			this.Source = value;
		}
	}

	// Token: 0x06009799 RID: 38809 RVA: 0x003C530E File Offset: 0x003C350E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Audio = base.GetComponent<AudioSource>();
		this.CurrentHealth = this.MaxHealth;
		this.CheckForObsoleteParams();
	}

	// Token: 0x0600979A RID: 38810 RVA: 0x003C532E File Offset: 0x003C352E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		SceneManager.sceneLoaded += this.NotifyLevelWasLoaded;
	}

	// Token: 0x0600979B RID: 38811 RVA: 0x003C5341 File Offset: 0x003C3541
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		SceneManager.sceneLoaded -= this.NotifyLevelWasLoaded;
	}

	// Token: 0x0600979C RID: 38812 RVA: 0x003C5354 File Offset: 0x003C3554
	public virtual void Damage(float damage)
	{
		this.Damage(new vp_DamageInfo(damage, null));
	}

	// Token: 0x0600979D RID: 38813 RVA: 0x003C5364 File Offset: 0x003C3564
	public virtual void Damage(vp_DamageInfo damageInfo)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		if (this.CurrentHealth <= 0f)
		{
			return;
		}
		if (damageInfo != null)
		{
			if (damageInfo.Source != null)
			{
				this.Source = damageInfo.Source;
			}
			if (damageInfo.OriginalSource != null)
			{
				this.OriginalSource = damageInfo.OriginalSource;
			}
		}
		this.CurrentHealth = Mathf.Min(this.CurrentHealth - damageInfo.Damage, this.MaxHealth);
		if (vp_Gameplay.isMaster)
		{
			if (vp_Gameplay.isMultiplayer && damageInfo.Source != null)
			{
				vp_GlobalEvent<Transform, Transform, float>.Send("Damage", this.Transform.root, damageInfo.OriginalSource, damageInfo.Damage, vp_GlobalEventMode.REQUIRE_LISTENER);
			}
			if (this.CurrentHealth <= 0f)
			{
				if (this.m_InstaKill)
				{
					base.SendMessage("Die");
					return;
				}
				vp_Timer.In(UnityEngine.Random.Range(this.MinDeathDelay, this.MaxDeathDelay), delegate()
				{
					base.SendMessage("Die");
				}, null);
			}
		}
	}

	// Token: 0x0600979E RID: 38814 RVA: 0x003C5470 File Offset: 0x003C3670
	public virtual void DieBySources(Transform[] sourceAndOriginalSource)
	{
		if (sourceAndOriginalSource.Length != 2)
		{
			Debug.LogWarning("Warning (" + ((this != null) ? this.ToString() : null) + ") 'DieBySources' argument must contain 2 transforms.");
			return;
		}
		this.Source = sourceAndOriginalSource[0];
		this.OriginalSource = sourceAndOriginalSource[1];
		this.Die();
	}

	// Token: 0x0600979F RID: 38815 RVA: 0x003C54C0 File Offset: 0x003C36C0
	public virtual void DieBySource(Transform source)
	{
		this.Source = source;
		this.OriginalSource = source;
		this.Die();
	}

	// Token: 0x060097A0 RID: 38816 RVA: 0x003C54E4 File Offset: 0x003C36E4
	public virtual void Die()
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
				GameObject gameObject2 = (GameObject)vp_Utility.Instantiate(gameObject, this.Transform.position, this.Transform.rotation);
				if (this.Source != null && gameObject2 != null)
				{
					vp_TargetEvent<Transform>.Send(gameObject2.transform, "SetSource", this.OriginalSource, vp_TargetEventOptions.DontRequireReceiver);
				}
			}
		}
		if (this.Respawner == null)
		{
			vp_Utility.Destroy(base.gameObject);
		}
		else
		{
			this.RemoveBulletHoles();
			vp_Utility.Activate(base.gameObject, false);
		}
		this.m_InstaKill = false;
	}

	// Token: 0x060097A1 RID: 38817 RVA: 0x003C55DC File Offset: 0x003C37DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Reset()
	{
		this.CurrentHealth = this.MaxHealth;
		this.Source = null;
		this.OriginalSource = null;
	}

	// Token: 0x060097A2 RID: 38818 RVA: 0x003C55F8 File Offset: 0x003C37F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void RemoveBulletHoles()
	{
		vp_HitscanBullet[] componentsInChildren = base.GetComponentsInChildren<vp_HitscanBullet>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			vp_Utility.Destroy(componentsInChildren[i].gameObject);
		}
	}

	// Token: 0x060097A3 RID: 38819 RVA: 0x003C5628 File Offset: 0x003C3828
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnCollisionEnter(Collision collision)
	{
		float num = collision.relativeVelocity.sqrMagnitude * 0.1f;
		float num2 = (num > this.ImpactDamageThreshold) ? (num * this.ImpactDamageMultiplier) : 0f;
		if (num2 <= 0f)
		{
			return;
		}
		if (this.CurrentHealth - num2 <= 0f)
		{
			this.m_InstaKill = true;
		}
		this.Damage(num2);
	}

	// Token: 0x060097A4 RID: 38820 RVA: 0x003C5689 File Offset: 0x003C3889
	public static vp_DamageHandler GetDamageHandlerOfCollider(Collider col)
	{
		if (!vp_DamageHandler.DamageHandlersByCollider.TryGetValue(col, out vp_DamageHandler.m_GetDamageHandlerOfColliderResult))
		{
			vp_DamageHandler.m_GetDamageHandlerOfColliderResult = col.transform.root.GetComponentInChildren<vp_DamageHandler>();
			vp_DamageHandler.DamageHandlersByCollider.Add(col, vp_DamageHandler.m_GetDamageHandlerOfColliderResult);
		}
		return vp_DamageHandler.m_GetDamageHandlerOfColliderResult;
	}

	// Token: 0x060097A5 RID: 38821 RVA: 0x003C56C7 File Offset: 0x003C38C7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void NotifyLevelWasLoaded(Scene scene, LoadSceneMode mode)
	{
		vp_DamageHandler.DamageHandlersByCollider.Clear();
	}

	// Token: 0x060097A6 RID: 38822 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Respawn()
	{
	}

	// Token: 0x060097A7 RID: 38823 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Reactivate()
	{
	}

	// Token: 0x060097A8 RID: 38824 RVA: 0x003C56D4 File Offset: 0x003C38D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckForObsoleteParams()
	{
		if (this.DeathEffect != null)
		{
			Debug.LogWarning(((this != null) ? this.ToString() : null) + "'DeathEffect' is obsolete! Please use the 'DeathSpawnObjects' array instead.");
		}
		string text = "";
		if (this.Respawns)
		{
			text += "Respawns, ";
		}
		if (this.MinRespawnTime != -99999f)
		{
			text += "MinRespawnTime, ";
		}
		if (this.MaxRespawnTime != -99999f)
		{
			text += "MaxRespawnTime, ";
		}
		if (this.RespawnCheckRadius != -99999f)
		{
			text += "RespawnCheckRadius, ";
		}
		if (this.RespawnSound != null)
		{
			text += "RespawnSound, ";
		}
		if (text != "")
		{
			text = text.Remove(text.LastIndexOf(", "));
			Debug.LogWarning(string.Format("Warning + (" + ((this != null) ? this.ToString() : null) + ") The following parameters are obsolete: \"{0}\". Creating a temp vp_Respawner component. To remove this warning, see the UFPS menu -> Wizards -> Convert Old DamageHandlers.", text));
			this.CreateTempRespawner();
		}
	}

	// Token: 0x060097A9 RID: 38825 RVA: 0x003C57D7 File Offset: 0x003C39D7
	public bool CreateTempRespawner()
	{
		if (base.GetComponent<vp_Respawner>() || base.GetComponent<vp_PlayerRespawner>())
		{
			this.DisableOldParams();
			return false;
		}
		vp_DamageHandler.CreateRespawnerForDamageHandler(this);
		this.DisableOldParams();
		return true;
	}

	// Token: 0x060097AA RID: 38826 RVA: 0x003C5808 File Offset: 0x003C3A08
	public static int GenerateRespawnersForAllDamageHandlers()
	{
		vp_PlayerDamageHandler[] array = UnityEngine.Object.FindObjectsOfType(typeof(vp_PlayerDamageHandler)) as vp_PlayerDamageHandler[];
		if (array != null && array.Length != 0)
		{
			foreach (vp_PlayerDamageHandler vp_PlayerDamageHandler in array)
			{
				if (!(vp_PlayerDamageHandler.transform.GetComponent<vp_FPPlayerEventHandler>() == null))
				{
					vp_FPPlayerDamageHandler vp_FPPlayerDamageHandler = vp_PlayerDamageHandler.gameObject.AddComponent<vp_FPPlayerDamageHandler>();
					vp_FPPlayerDamageHandler.AllowFallDamage = vp_PlayerDamageHandler.AllowFallDamage;
					vp_FPPlayerDamageHandler.DeathEffect = vp_PlayerDamageHandler.DeathEffect;
					vp_FPPlayerDamageHandler.DeathSound = vp_PlayerDamageHandler.DeathSound;
					vp_FPPlayerDamageHandler.DeathSpawnObjects = vp_PlayerDamageHandler.DeathSpawnObjects;
					vp_FPPlayerDamageHandler.FallImpactPitch = vp_PlayerDamageHandler.FallImpactPitch;
					vp_FPPlayerDamageHandler.FallImpactSounds = vp_PlayerDamageHandler.FallImpactSounds;
					vp_FPPlayerDamageHandler.FallImpactThreshold = vp_PlayerDamageHandler.FallImpactThreshold;
					vp_FPPlayerDamageHandler.ImpactDamageMultiplier = vp_PlayerDamageHandler.ImpactDamageMultiplier;
					vp_FPPlayerDamageHandler.ImpactDamageThreshold = vp_PlayerDamageHandler.ImpactDamageThreshold;
					vp_FPPlayerDamageHandler.m_Audio = vp_PlayerDamageHandler.m_Audio;
					vp_FPPlayerDamageHandler.CurrentHealth = vp_PlayerDamageHandler.CurrentHealth;
					vp_FPPlayerDamageHandler.m_StartPosition = vp_PlayerDamageHandler.m_StartPosition;
					vp_FPPlayerDamageHandler.m_StartRotation = vp_PlayerDamageHandler.m_StartRotation;
					vp_FPPlayerDamageHandler.MaxDeathDelay = vp_PlayerDamageHandler.MaxDeathDelay;
					vp_FPPlayerDamageHandler.MaxHealth = vp_PlayerDamageHandler.MaxHealth;
					vp_FPPlayerDamageHandler.MaxRespawnTime = vp_PlayerDamageHandler.MaxRespawnTime;
					vp_FPPlayerDamageHandler.MinDeathDelay = vp_PlayerDamageHandler.MinDeathDelay;
					vp_FPPlayerDamageHandler.MinRespawnTime = vp_PlayerDamageHandler.MinRespawnTime;
					vp_FPPlayerDamageHandler.RespawnCheckRadius = vp_PlayerDamageHandler.RespawnCheckRadius;
					vp_FPPlayerDamageHandler.Respawns = vp_PlayerDamageHandler.Respawns;
					vp_FPPlayerDamageHandler.RespawnSound = vp_PlayerDamageHandler.RespawnSound;
					UnityEngine.Object.DestroyImmediate(vp_PlayerDamageHandler);
				}
			}
		}
		vp_DamageHandler[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(vp_DamageHandler)) as vp_DamageHandler[];
		vp_DamageHandler[] array4 = UnityEngine.Object.FindObjectsOfType(typeof(vp_FPPlayerDamageHandler)) as vp_DamageHandler[];
		int num = 0;
		vp_DamageHandler[] array5 = array3;
		for (int i = 0; i < array5.Length; i++)
		{
			if (array5[i].CreateTempRespawner())
			{
				num++;
			}
		}
		array5 = array4;
		for (int i = 0; i < array5.Length; i++)
		{
			if (array5[i].CreateTempRespawner())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060097AB RID: 38827 RVA: 0x003C5A08 File Offset: 0x003C3C08
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisableOldParams()
	{
		this.Respawns = false;
		this.MinRespawnTime = -99999f;
		this.MaxRespawnTime = -99999f;
		this.RespawnCheckRadius = -99999f;
		this.RespawnSound = null;
	}

	// Token: 0x060097AC RID: 38828 RVA: 0x003C5A3C File Offset: 0x003C3C3C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateRespawnerForDamageHandler(vp_DamageHandler damageHandler)
	{
		if (damageHandler.gameObject.GetComponent<vp_Respawner>() || damageHandler.gameObject.GetComponent<vp_PlayerRespawner>())
		{
			return;
		}
		vp_Respawner vp_Respawner;
		if (damageHandler is vp_FPPlayerDamageHandler)
		{
			vp_Respawner = damageHandler.gameObject.AddComponent<vp_PlayerRespawner>();
		}
		else
		{
			vp_Respawner = damageHandler.gameObject.AddComponent<vp_Respawner>();
		}
		if (vp_Respawner == null)
		{
			return;
		}
		if (damageHandler.MinRespawnTime != -99999f)
		{
			vp_Respawner.MinRespawnTime = damageHandler.MinRespawnTime;
		}
		if (damageHandler.MaxRespawnTime != -99999f)
		{
			vp_Respawner.MaxRespawnTime = damageHandler.MaxRespawnTime;
		}
		if (damageHandler.RespawnCheckRadius != -99999f)
		{
			vp_Respawner.ObstructionRadius = damageHandler.RespawnCheckRadius;
		}
		if (damageHandler.RespawnSound != null)
		{
			vp_Respawner.SpawnSound = damageHandler.RespawnSound;
		}
	}

	// Token: 0x040073F8 RID: 29688
	public float MaxHealth = 1f;

	// Token: 0x040073F9 RID: 29689
	public GameObject[] DeathSpawnObjects;

	// Token: 0x040073FA RID: 29690
	public float MinDeathDelay;

	// Token: 0x040073FB RID: 29691
	public float MaxDeathDelay;

	// Token: 0x040073FC RID: 29692
	public float CurrentHealth;

	// Token: 0x040073FD RID: 29693
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_InstaKill;

	// Token: 0x040073FE RID: 29694
	public AudioClip DeathSound;

	// Token: 0x040073FF RID: 29695
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x04007400 RID: 29696
	public float ImpactDamageThreshold = 10f;

	// Token: 0x04007401 RID: 29697
	public float ImpactDamageMultiplier;

	// Token: 0x04007402 RID: 29698
	[HideInInspector]
	public bool Respawns;

	// Token: 0x04007403 RID: 29699
	[HideInInspector]
	public float MinRespawnTime = -99999f;

	// Token: 0x04007404 RID: 29700
	[HideInInspector]
	public float MaxRespawnTime = -99999f;

	// Token: 0x04007405 RID: 29701
	[HideInInspector]
	public float RespawnCheckRadius = -99999f;

	// Token: 0x04007406 RID: 29702
	[HideInInspector]
	public AudioClip RespawnSound;

	// Token: 0x04007407 RID: 29703
	[HideInInspector]
	public GameObject DeathEffect;

	// Token: 0x04007408 RID: 29704
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Dictionary<Collider, vp_DamageHandler> m_DamageHandlersByCollider;

	// Token: 0x04007409 RID: 29705
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static vp_DamageHandler m_GetDamageHandlerOfColliderResult;

	// Token: 0x0400740A RID: 29706
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_StartPosition;

	// Token: 0x0400740B RID: 29707
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Quaternion m_StartRotation;

	// Token: 0x0400740C RID: 29708
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x0400740D RID: 29709
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Respawner m_Respawner;

	// Token: 0x0400740E RID: 29710
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Source;

	// Token: 0x0400740F RID: 29711
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_OriginalSource;
}
