using System;
using UnityEngine;

// Token: 0x02001307 RID: 4871
[RequireComponent(typeof(AudioSource))]
public class vp_Shooter : vp_Component
{
	// Token: 0x17000F85 RID: 3973
	// (get) Token: 0x060097D2 RID: 38866 RVA: 0x003C67E5 File Offset: 0x003C49E5
	public GameObject ProjectileSpawnPoint
	{
		get
		{
			return this.m_ProjectileSpawnPoint;
		}
	}

	// Token: 0x17000F86 RID: 3974
	// (get) Token: 0x060097D3 RID: 38867 RVA: 0x003C67F0 File Offset: 0x003C49F0
	public GameObject MuzzleFlash
	{
		get
		{
			if (this.m_MuzzleFlash == null && this.MuzzleFlashPrefab != null && this.ProjectileSpawnPoint != null)
			{
				this.m_MuzzleFlash = (GameObject)vp_Utility.Instantiate(this.MuzzleFlashPrefab, this.ProjectileSpawnPoint.transform.position, this.ProjectileSpawnPoint.transform.rotation);
				this.m_MuzzleFlash.name = base.transform.name + "MuzzleFlash";
				this.m_MuzzleFlash.transform.parent = this.ProjectileSpawnPoint.transform;
			}
			return this.m_MuzzleFlash;
		}
	}

	// Token: 0x060097D4 RID: 38868 RVA: 0x003C68A4 File Offset: 0x003C4AA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		if (this.m_ProjectileSpawnPoint == null)
		{
			this.m_ProjectileSpawnPoint = base.gameObject;
		}
		this.m_ProjectileDefaultSpawnpoint = this.m_ProjectileSpawnPoint;
		if (this.GetFirePosition == null)
		{
			this.GetFirePosition = (() => this.FirePosition);
		}
		if (this.GetFireRotation == null)
		{
			this.GetFireRotation = (() => this.m_ProjectileSpawnPoint.transform.rotation);
		}
		if (this.GetFireSeed == null)
		{
			this.GetFireSeed = (() => UnityEngine.Random.Range(0, 100));
		}
		this.m_CharacterController = this.m_ProjectileSpawnPoint.transform.root.GetComponentInChildren<CharacterController>();
		this.m_NextAllowedFireTime = Time.time;
		this.ProjectileSpawnDelay = Mathf.Min(this.ProjectileSpawnDelay, this.ProjectileFiringRate - 0.1f);
	}

	// Token: 0x060097D5 RID: 38869 RVA: 0x003C6981 File Offset: 0x003C4B81
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		base.Audio.playOnAwake = false;
		base.Audio.dopplerLevel = 0f;
		base.RefreshDefaultState();
		this.Refresh();
	}

	// Token: 0x060097D6 RID: 38870 RVA: 0x003C69B1 File Offset: 0x003C4BB1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		this.FirePosition = this.m_ProjectileSpawnPoint.transform.position;
	}

	// Token: 0x060097D7 RID: 38871 RVA: 0x003C69C9 File Offset: 0x003C4BC9
	public virtual bool CanFire()
	{
		return Time.time >= this.m_NextAllowedFireTime;
	}

	// Token: 0x060097D8 RID: 38872 RVA: 0x003C69DB File Offset: 0x003C4BDB
	public virtual bool TryFire()
	{
		if (Time.time < this.m_NextAllowedFireTime)
		{
			return false;
		}
		this.Fire();
		return true;
	}

	// Token: 0x060097D9 RID: 38873 RVA: 0x003C69F4 File Offset: 0x003C4BF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Fire()
	{
		this.m_NextAllowedFireTime = Time.time + this.ProjectileFiringRate;
		if (this.SoundFireDelay == 0f)
		{
			this.PlayFireSound();
		}
		else
		{
			vp_Timer.In(this.SoundFireDelay, new vp_Timer.Callback(this.PlayFireSound), null);
		}
		if (this.ProjectileSpawnDelay == 0f)
		{
			this.SpawnProjectiles();
		}
		else
		{
			vp_Timer.In(this.ProjectileSpawnDelay, delegate()
			{
				this.SpawnProjectiles();
			}, null);
		}
		if (this.ShellEjectDelay == 0f)
		{
			this.EjectShell();
		}
		else
		{
			vp_Timer.In(this.ShellEjectDelay, new vp_Timer.Callback(this.EjectShell), null);
		}
		if (this.MuzzleFlashDelay == 0f)
		{
			this.ShowMuzzleFlash();
			return;
		}
		vp_Timer.In(this.MuzzleFlashDelay, new vp_Timer.Callback(this.ShowMuzzleFlash), null);
	}

	// Token: 0x060097DA RID: 38874 RVA: 0x003C6ACC File Offset: 0x003C4CCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void PlayFireSound()
	{
		if (base.Audio == null)
		{
			return;
		}
		base.Audio.pitch = UnityEngine.Random.Range(this.SoundFirePitch.x, this.SoundFirePitch.y) * Time.timeScale;
		base.Audio.clip = this.SoundFire;
		base.Audio.Play();
	}

	// Token: 0x060097DB RID: 38875 RVA: 0x003C6B30 File Offset: 0x003C4D30
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SpawnProjectiles()
	{
		if (this.ProjectilePrefab == null)
		{
			return;
		}
		if (this.m_SendFireEventToNetworkFunc != null)
		{
			this.m_SendFireEventToNetworkFunc();
		}
		this.m_CurrentFirePosition = this.GetFirePosition();
		this.m_CurrentFireRotation = this.GetFireRotation();
		this.m_CurrentFireSeed = this.GetFireSeed();
		for (int i = 0; i < this.ProjectileCount; i++)
		{
			GameObject gameObject = (GameObject)vp_Utility.Instantiate(this.ProjectilePrefab, this.m_CurrentFirePosition, this.m_CurrentFireRotation);
			gameObject.SendMessage("SetSource", this.ProjectileSourceIsRoot ? base.Root : base.Transform, SendMessageOptions.DontRequireReceiver);
			gameObject.transform.localScale = new Vector3(this.ProjectileScale, this.ProjectileScale, this.ProjectileScale);
			this.SetSpread(this.m_CurrentFireSeed * (i + 1), gameObject.transform);
		}
	}

	// Token: 0x060097DC RID: 38876 RVA: 0x003C6C20 File Offset: 0x003C4E20
	public void SetSpread(int seed, Transform target)
	{
		UnityEngine.Random.InitState(seed);
		target.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
		target.Rotate(0f, UnityEngine.Random.Range(-this.ProjectileSpread, this.ProjectileSpread), 0f);
	}

	// Token: 0x060097DD RID: 38877 RVA: 0x003C6C74 File Offset: 0x003C4E74
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void ShowMuzzleFlash()
	{
		if (this.MuzzleFlash == null)
		{
			return;
		}
		if (this.m_MuzzleFlashSpawnPoint != null && this.ProjectileSpawnPoint != null)
		{
			this.MuzzleFlash.transform.position = this.m_MuzzleFlashSpawnPoint.transform.position;
			this.MuzzleFlash.transform.rotation = this.ProjectileSpawnPoint.transform.rotation;
		}
		this.MuzzleFlash.SendMessage("Shoot", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060097DE RID: 38878 RVA: 0x003C6D00 File Offset: 0x003C4F00
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void EjectShell()
	{
		if (this.ShellPrefab == null)
		{
			return;
		}
		GameObject gameObject = (GameObject)vp_Utility.Instantiate(this.ShellPrefab, (this.m_ShellEjectSpawnPoint == null) ? (this.FirePosition + this.m_ProjectileSpawnPoint.transform.TransformDirection(this.ShellEjectPosition)) : this.m_ShellEjectSpawnPoint.transform.position, this.m_ProjectileSpawnPoint.transform.rotation);
		gameObject.transform.localScale = new Vector3(this.ShellScale, this.ShellScale, this.ShellScale);
		vp_Layer.Set(gameObject.gameObject, 29, false);
		if (gameObject.GetComponent<Rigidbody>())
		{
			Vector3 force = (this.m_ShellEjectSpawnPoint == null) ? (base.transform.TransformDirection(this.ShellEjectDirection).normalized * this.ShellEjectVelocity) : (this.m_ShellEjectSpawnPoint.transform.forward.normalized * this.ShellEjectVelocity);
			gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
		}
		if (this.m_CharacterController)
		{
			Vector3 velocity = this.m_CharacterController.velocity;
			gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
		}
		if (this.ShellEjectSpin > 0f)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				gameObject.GetComponent<Rigidbody>().AddRelativeTorque(-UnityEngine.Random.rotation.eulerAngles * this.ShellEjectSpin);
				return;
			}
			gameObject.GetComponent<Rigidbody>().AddRelativeTorque(UnityEngine.Random.rotation.eulerAngles * this.ShellEjectSpin);
		}
	}

	// Token: 0x060097DF RID: 38879 RVA: 0x003C6EAF File Offset: 0x003C50AF
	public virtual void DisableFiring(float seconds = 10000000f)
	{
		this.m_NextAllowedFireTime = Time.time + seconds;
	}

	// Token: 0x060097E0 RID: 38880 RVA: 0x003C6EBE File Offset: 0x003C50BE
	public virtual void EnableFiring()
	{
		this.m_NextAllowedFireTime = Time.time;
	}

	// Token: 0x060097E1 RID: 38881 RVA: 0x003C6ECC File Offset: 0x003C50CC
	public override void Refresh()
	{
		if (this.MuzzleFlash != null)
		{
			if (this.m_MuzzleFlashSpawnPoint == null)
			{
				if (this.ProjectileSpawnPoint == this.m_ProjectileDefaultSpawnpoint)
				{
					this.m_MuzzleFlashSpawnPoint = vp_Utility.GetTransformByNameInChildren(this.ProjectileSpawnPoint.transform, "muzzle", false, false);
				}
				else
				{
					this.m_MuzzleFlashSpawnPoint = vp_Utility.GetTransformByNameInChildren(base.Transform, "muzzle", false, false);
				}
			}
			if (this.m_MuzzleFlashSpawnPoint != null)
			{
				if (this.ProjectileSpawnPoint == this.m_ProjectileDefaultSpawnpoint)
				{
					this.m_MuzzleFlash.transform.parent = this.ProjectileSpawnPoint.transform.parent.parent.parent;
				}
				else
				{
					this.m_MuzzleFlash.transform.parent = this.ProjectileSpawnPoint.transform;
				}
			}
			else
			{
				this.m_MuzzleFlash.transform.parent = this.ProjectileSpawnPoint.transform;
				this.MuzzleFlash.transform.localPosition = this.MuzzleFlashPosition;
				this.MuzzleFlash.transform.rotation = this.ProjectileSpawnPoint.transform.rotation;
			}
			this.MuzzleFlash.transform.localScale = this.MuzzleFlashScale;
			this.MuzzleFlash.SendMessage("SetFadeSpeed", this.MuzzleFlashFadeSpeed, SendMessageOptions.DontRequireReceiver);
		}
		if (this.ShellPrefab != null && this.m_ShellEjectSpawnPoint == null && this.ProjectileSpawnPoint != null)
		{
			if (this.ProjectileSpawnPoint == this.m_ProjectileDefaultSpawnpoint)
			{
				this.m_ShellEjectSpawnPoint = vp_Utility.GetTransformByNameInChildren(this.ProjectileSpawnPoint.transform, "shell", false, false);
				return;
			}
			this.m_ShellEjectSpawnPoint = vp_Utility.GetTransformByNameInChildren(base.Transform, "shell", false, false);
		}
	}

	// Token: 0x060097E2 RID: 38882 RVA: 0x003C70A2 File Offset: 0x003C52A2
	public override void Activate()
	{
		base.Activate();
		if (this.MuzzleFlash != null)
		{
			vp_Utility.Activate(this.MuzzleFlash, true);
		}
	}

	// Token: 0x060097E3 RID: 38883 RVA: 0x003C70C4 File Offset: 0x003C52C4
	public override void Deactivate()
	{
		base.Deactivate();
		if (this.MuzzleFlash != null)
		{
			vp_Utility.Activate(this.MuzzleFlash, false);
		}
	}

	// Token: 0x060097E4 RID: 38884 RVA: 0x003C70E8 File Offset: 0x003C52E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DrawProjectileDebugInfo(int projectileIndex)
	{
		GameObject gameObject = vp_3DUtility.DebugPointer(null);
		gameObject.transform.rotation = this.GetFireRotation();
		gameObject.transform.position = this.GetFirePosition();
		GameObject gameObject2 = vp_3DUtility.DebugBall(null);
		RaycastHit raycastHit;
		if (Physics.Linecast(gameObject.transform.position, gameObject.transform.position + gameObject.transform.forward * 1000f, out raycastHit, 1084850176) && !raycastHit.collider.isTrigger && base.Root.InverseTransformPoint(raycastHit.point).z > 0f)
		{
			gameObject2.transform.position = raycastHit.point;
		}
	}

	// Token: 0x04007444 RID: 29764
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public CharacterController m_CharacterController;

	// Token: 0x04007445 RID: 29765
	public GameObject m_ProjectileSpawnPoint;

	// Token: 0x04007446 RID: 29766
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GameObject m_ProjectileDefaultSpawnpoint;

	// Token: 0x04007447 RID: 29767
	public GameObject ProjectilePrefab;

	// Token: 0x04007448 RID: 29768
	public float ProjectileScale = 1f;

	// Token: 0x04007449 RID: 29769
	public float ProjectileFiringRate = 0.3f;

	// Token: 0x0400744A RID: 29770
	public float ProjectileSpawnDelay;

	// Token: 0x0400744B RID: 29771
	public int ProjectileCount = 1;

	// Token: 0x0400744C RID: 29772
	public float ProjectileSpread;

	// Token: 0x0400744D RID: 29773
	public bool ProjectileSourceIsRoot = true;

	// Token: 0x0400744E RID: 29774
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_NextAllowedFireTime;

	// Token: 0x0400744F RID: 29775
	public Vector3 MuzzleFlashPosition = Vector3.zero;

	// Token: 0x04007450 RID: 29776
	public Vector3 MuzzleFlashScale = Vector3.one;

	// Token: 0x04007451 RID: 29777
	public float MuzzleFlashFadeSpeed = 0.075f;

	// Token: 0x04007452 RID: 29778
	public GameObject MuzzleFlashPrefab;

	// Token: 0x04007453 RID: 29779
	public float MuzzleFlashDelay;

	// Token: 0x04007454 RID: 29780
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GameObject m_MuzzleFlash;

	// Token: 0x04007455 RID: 29781
	public Transform m_MuzzleFlashSpawnPoint;

	// Token: 0x04007456 RID: 29782
	public GameObject ShellPrefab;

	// Token: 0x04007457 RID: 29783
	public float ShellScale = 1f;

	// Token: 0x04007458 RID: 29784
	public Vector3 ShellEjectDirection = new Vector3(1f, 1f, 1f);

	// Token: 0x04007459 RID: 29785
	public Vector3 ShellEjectPosition = new Vector3(1f, 0f, 1f);

	// Token: 0x0400745A RID: 29786
	public float ShellEjectVelocity = 0.2f;

	// Token: 0x0400745B RID: 29787
	public float ShellEjectDelay;

	// Token: 0x0400745C RID: 29788
	public float ShellEjectSpin;

	// Token: 0x0400745D RID: 29789
	public Transform m_ShellEjectSpawnPoint;

	// Token: 0x0400745E RID: 29790
	public AudioClip SoundFire;

	// Token: 0x0400745F RID: 29791
	public float SoundFireDelay;

	// Token: 0x04007460 RID: 29792
	public Vector2 SoundFirePitch = new Vector2(1f, 1f);

	// Token: 0x04007461 RID: 29793
	public vp_Shooter.NetworkFunc m_SendFireEventToNetworkFunc;

	// Token: 0x04007462 RID: 29794
	public vp_Shooter.FirePositionFunc GetFirePosition;

	// Token: 0x04007463 RID: 29795
	public vp_Shooter.FireRotationFunc GetFireRotation;

	// Token: 0x04007464 RID: 29796
	public vp_Shooter.FireSeedFunc GetFireSeed;

	// Token: 0x04007465 RID: 29797
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CurrentFirePosition = Vector3.zero;

	// Token: 0x04007466 RID: 29798
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Quaternion m_CurrentFireRotation = Quaternion.identity;

	// Token: 0x04007467 RID: 29799
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_CurrentFireSeed;

	// Token: 0x04007468 RID: 29800
	public Vector3 FirePosition = Vector3.zero;

	// Token: 0x02001308 RID: 4872
	// (Invoke) Token: 0x060097EA RID: 38890
	public delegate void NetworkFunc();

	// Token: 0x02001309 RID: 4873
	// (Invoke) Token: 0x060097EE RID: 38894
	public delegate Vector3 FirePositionFunc();

	// Token: 0x0200130A RID: 4874
	// (Invoke) Token: 0x060097F2 RID: 38898
	public delegate Quaternion FireRotationFunc();

	// Token: 0x0200130B RID: 4875
	// (Invoke) Token: 0x060097F6 RID: 38902
	public delegate int FireSeedFunc();
}
