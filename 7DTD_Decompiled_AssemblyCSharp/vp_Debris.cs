using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020012F9 RID: 4857
[RequireComponent(typeof(AudioSource))]
public class vp_Debris : MonoBehaviour
{
	// Token: 0x06009759 RID: 38745 RVA: 0x003C4124 File Offset: 0x003C2324
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Colliders = base.GetComponentsInChildren<Collider>();
		foreach (Collider collider in this.m_Colliders)
		{
			if (collider.GetComponent<Rigidbody>())
			{
				this.m_PiecesInitial.Add(collider, new Dictionary<string, object>
				{
					{
						"Position",
						collider.transform.localPosition
					},
					{
						"Rotation",
						collider.transform.localRotation
					}
				});
			}
		}
	}

	// Token: 0x0600975A RID: 38746 RVA: 0x003C41B8 File Offset: 0x003C23B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.m_Destroy = false;
		this.m_Audio.playOnAwake = true;
		foreach (Collider collider in this.m_Colliders)
		{
			if (collider.GetComponent<Rigidbody>())
			{
				collider.transform.localPosition = (Vector3)this.m_PiecesInitial[collider]["Position"];
				collider.transform.localRotation = (Quaternion)this.m_PiecesInitial[collider]["Rotation"];
				collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
				collider.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				collider.GetComponent<Rigidbody>().AddExplosionForce(this.Force / Time.timeScale / vp_TimeUtility.AdjustedTimeScale, base.transform.position, this.Radius, this.UpForce);
				Collider c = collider;
				vp_Timer.In(UnityEngine.Random.Range(this.LifeTime * 0.5f, this.LifeTime * 0.95f), delegate()
				{
					if (c != null)
					{
						vp_Utility.Destroy(c.gameObject);
					}
				}, null);
			}
		}
		vp_Timer.In(this.LifeTime, delegate()
		{
			this.m_Destroy = true;
		}, null);
		if (this.Sounds.Count > 0)
		{
			this.m_Audio.rolloffMode = AudioRolloffMode.Linear;
			this.m_Audio.clip = this.Sounds[UnityEngine.Random.Range(0, this.Sounds.Count)];
			this.m_Audio.pitch = UnityEngine.Random.Range(this.SoundMinPitch, this.SoundMaxPitch) * Time.timeScale;
			this.m_Audio.Play();
		}
	}

	// Token: 0x0600975B RID: 38747 RVA: 0x003C4369 File Offset: 0x003C2569
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_Destroy && !base.GetComponent<AudioSource>().isPlaying)
		{
			vp_Utility.Destroy(base.gameObject);
		}
	}

	// Token: 0x040073C2 RID: 29634
	public float Radius = 2f;

	// Token: 0x040073C3 RID: 29635
	public float Force = 10f;

	// Token: 0x040073C4 RID: 29636
	public float UpForce = 1f;

	// Token: 0x040073C5 RID: 29637
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x040073C6 RID: 29638
	public List<AudioClip> Sounds = new List<AudioClip>();

	// Token: 0x040073C7 RID: 29639
	public float SoundMinPitch = 0.8f;

	// Token: 0x040073C8 RID: 29640
	public float SoundMaxPitch = 1.2f;

	// Token: 0x040073C9 RID: 29641
	public float LifeTime = 5f;

	// Token: 0x040073CA RID: 29642
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_Destroy;

	// Token: 0x040073CB RID: 29643
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider[] m_Colliders;

	// Token: 0x040073CC RID: 29644
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<Collider, Dictionary<string, object>> m_PiecesInitial = new Dictionary<Collider, Dictionary<string, object>>();
}
