using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001304 RID: 4868
[RequireComponent(typeof(AudioSource))]
public class vp_Explosion : MonoBehaviour
{
	// Token: 0x17000F80 RID: 3968
	// (get) Token: 0x060097B1 RID: 38833 RVA: 0x003C5B7C File Offset: 0x003C3D7C
	public float DistanceModifier
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_DistanceModifier == 0f)
			{
				this.m_DistanceModifier = 1f - Vector3.Distance(this.Transform.position, this.m_TargetTransform.position) / this.Radius;
			}
			return this.m_DistanceModifier;
		}
	}

	// Token: 0x17000F81 RID: 3969
	// (get) Token: 0x060097B2 RID: 38834 RVA: 0x003C5BCA File Offset: 0x003C3DCA
	public Transform Transform
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x17000F82 RID: 3970
	// (get) Token: 0x060097B3 RID: 38835 RVA: 0x003C5BEC File Offset: 0x003C3DEC
	// (set) Token: 0x060097B4 RID: 38836 RVA: 0x003C5C0E File Offset: 0x003C3E0E
	public Transform Source
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Source == null)
			{
				this.m_Source = base.transform;
			}
			return this.m_Source;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_Source = value;
		}
	}

	// Token: 0x17000F83 RID: 3971
	// (get) Token: 0x060097B5 RID: 38837 RVA: 0x003C5C17 File Offset: 0x003C3E17
	// (set) Token: 0x060097B6 RID: 38838 RVA: 0x003C5C39 File Offset: 0x003C3E39
	public Transform OriginalSource
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_OriginalSource == null)
			{
				this.m_OriginalSource = base.transform;
			}
			return this.m_OriginalSource;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_OriginalSource = value;
		}
	}

	// Token: 0x17000F84 RID: 3972
	// (get) Token: 0x060097B7 RID: 38839 RVA: 0x003C5C42 File Offset: 0x003C3E42
	public AudioSource Audio
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Audio == null)
			{
				this.m_Audio = base.GetComponent<AudioSource>();
			}
			return this.m_Audio;
		}
	}

	// Token: 0x060097B8 RID: 38840 RVA: 0x003C5C64 File Offset: 0x003C3E64
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		this.Source = base.transform;
		this.OriginalSource = null;
		vp_TargetEvent<Transform>.Register(base.transform, "SetSource", new Action<Transform>(this.SetSource));
	}

	// Token: 0x060097B9 RID: 38841 RVA: 0x003C5C95 File Offset: 0x003C3E95
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		this.Source = null;
		this.OriginalSource = null;
		vp_TargetEvent<Transform>.Unregister(base.transform, "SetSource", new Action<Transform>(this.SetSource));
	}

	// Token: 0x060097BA RID: 38842 RVA: 0x003C5CC1 File Offset: 0x003C3EC1
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_HaveExploded)
		{
			if (!this.Audio.isPlaying)
			{
				vp_Utility.Destroy(base.gameObject);
			}
			return;
		}
		this.DoExplode();
	}

	// Token: 0x060097BB RID: 38843 RVA: 0x003C5CEC File Offset: 0x003C3EEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoExplode()
	{
		this.m_HaveExploded = true;
		foreach (GameObject gameObject in this.FXPrefabs)
		{
			if (gameObject != null)
			{
				vp_Utility.Instantiate(gameObject, this.Transform.position, this.Transform.rotation);
			}
		}
		this.m_DHandlersHitByThisExplosion.Clear();
		foreach (Collider collider in Physics.OverlapSphere(this.Transform.position, this.Radius, -738197525))
		{
			if (!collider.gameObject.isStatic)
			{
				this.m_DistanceModifier = 0f;
				if (collider != null && collider != base.GetComponent<Collider>())
				{
					this.m_TargetCollider = collider;
					this.m_TargetTransform = collider.transform;
					this.AddUFPSCameraShake();
					if (!this.TargetInCover())
					{
						this.m_TargetRigidbody = collider.GetComponent<Rigidbody>();
						if (this.m_TargetRigidbody != null)
						{
							this.AddRigidbodyForce();
						}
						else
						{
							this.AddUFPSForce();
						}
						vp_Explosion.m_TargetDHandler = vp_DamageHandler.GetDamageHandlerOfCollider(this.m_TargetCollider);
						if (vp_Explosion.m_TargetDHandler != null)
						{
							this.DoUFPSDamage(this.DistanceModifier * this.Damage);
						}
						else if (!this.RequireDamageHandler)
						{
							this.DoUnityDamage(this.DistanceModifier * this.Damage);
						}
					}
				}
			}
		}
		this.Audio.clip = this.Sound;
		this.Audio.pitch = UnityEngine.Random.Range(this.SoundMinPitch, this.SoundMaxPitch) * Time.timeScale;
		if (!this.Audio.playOnAwake)
		{
			this.Audio.Play();
		}
	}

	// Token: 0x060097BC RID: 38844 RVA: 0x003C5EC4 File Offset: 0x003C40C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool TargetInCover()
	{
		if (!this.AllowCover)
		{
			return false;
		}
		this.m_Ray.origin = this.Transform.position;
		this.m_Ray.direction = (this.m_TargetCollider.bounds.center - this.Transform.position).normalized;
		if (Physics.Raycast(this.m_Ray, out this.m_RaycastHit, this.Radius + 1f) && this.m_RaycastHit.collider == this.m_TargetCollider)
		{
			return false;
		}
		this.m_Ray.direction = (vp_3DUtility.HorizontalVector(this.m_TargetCollider.bounds.center) + Vector3.up * this.m_TargetCollider.bounds.max.y - this.Transform.position).normalized;
		return !Physics.Raycast(this.m_Ray, out this.m_RaycastHit, this.Radius + 1f) || !(this.m_RaycastHit.collider == this.m_TargetCollider);
	}

	// Token: 0x060097BD RID: 38845 RVA: 0x003C5FFC File Offset: 0x003C41FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AddRigidbodyForce()
	{
		if (this.m_TargetRigidbody.isKinematic)
		{
			return;
		}
		this.m_Ray.origin = this.m_TargetTransform.position;
		this.m_Ray.direction = -Vector3.up;
		if (!Physics.Raycast(this.m_Ray, out this.m_RaycastHit, 1f))
		{
			this.UpForce = 0f;
		}
		this.m_TargetRigidbody.AddExplosionForce(this.Force / Time.timeScale / vp_TimeUtility.AdjustedTimeScale, this.Transform.position, this.Radius, this.UpForce);
	}

	// Token: 0x060097BE RID: 38846 RVA: 0x003C609C File Offset: 0x003C429C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AddUFPSForce()
	{
		vp_TargetEvent<Vector3>.Send(this.m_TargetTransform.root, "ForceImpact", (this.m_TargetTransform.position - this.Transform.position).normalized * this.Force * 0.001f * this.DistanceModifier, vp_TargetEventOptions.DontRequireReceiver);
	}

	// Token: 0x060097BF RID: 38847 RVA: 0x003C6102 File Offset: 0x003C4302
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AddUFPSCameraShake()
	{
		vp_TargetEvent<float>.Send(this.m_TargetTransform.root, "CameraBombShake", this.DistanceModifier * this.CameraShake, vp_TargetEventOptions.DontRequireReceiver);
	}

	// Token: 0x060097C0 RID: 38848 RVA: 0x003C6128 File Offset: 0x003C4328
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DoUFPSDamage(float damage)
	{
		if (this.m_DHandlersHitByThisExplosion.ContainsKey(vp_Explosion.m_TargetDHandler))
		{
			return;
		}
		this.m_DHandlersHitByThisExplosion.Add(vp_Explosion.m_TargetDHandler, null);
		vp_Explosion.m_TargetDHandler.Damage(new vp_DamageInfo(damage, this.Source, this.OriginalSource));
	}

	// Token: 0x060097C1 RID: 38849 RVA: 0x003C6175 File Offset: 0x003C4375
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoUnityDamage(float damage)
	{
		this.m_TargetCollider.gameObject.BroadcastMessage(this.DamageMessageName, damage, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x060097C2 RID: 38850 RVA: 0x003C5C39 File Offset: 0x003C3E39
	public void SetSource(Transform source)
	{
		this.m_OriginalSource = source;
	}

	// Token: 0x04007413 RID: 29715
	public float Radius = 15f;

	// Token: 0x04007414 RID: 29716
	public float Force = 1000f;

	// Token: 0x04007415 RID: 29717
	public float UpForce = 10f;

	// Token: 0x04007416 RID: 29718
	public float Damage = 10f;

	// Token: 0x04007417 RID: 29719
	public bool AllowCover;

	// Token: 0x04007418 RID: 29720
	public float CameraShake = 1f;

	// Token: 0x04007419 RID: 29721
	public string DamageMessageName = "Damage";

	// Token: 0x0400741A RID: 29722
	public bool RequireDamageHandler = true;

	// Token: 0x0400741B RID: 29723
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_HaveExploded;

	// Token: 0x0400741C RID: 29724
	public AudioClip Sound;

	// Token: 0x0400741D RID: 29725
	public float SoundMinPitch = 0.8f;

	// Token: 0x0400741E RID: 29726
	public float SoundMaxPitch = 1.2f;

	// Token: 0x0400741F RID: 29727
	public List<GameObject> FXPrefabs = new List<GameObject>();

	// Token: 0x04007420 RID: 29728
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Ray m_Ray;

	// Token: 0x04007421 RID: 29729
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public RaycastHit m_RaycastHit;

	// Token: 0x04007422 RID: 29730
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider m_TargetCollider;

	// Token: 0x04007423 RID: 29731
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_TargetTransform;

	// Token: 0x04007424 RID: 29732
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody m_TargetRigidbody;

	// Token: 0x04007425 RID: 29733
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_DistanceModifier;

	// Token: 0x04007426 RID: 29734
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<vp_DamageHandler, object> m_DHandlersHitByThisExplosion = new Dictionary<vp_DamageHandler, object>(50);

	// Token: 0x04007427 RID: 29735
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static vp_DamageHandler m_TargetDHandler;

	// Token: 0x04007428 RID: 29736
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x04007429 RID: 29737
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Source;

	// Token: 0x0400742A RID: 29738
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_OriginalSource;

	// Token: 0x0400742B RID: 29739
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_Audio;
}
