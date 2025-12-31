using System;
using UnityEngine;

// Token: 0x020012EF RID: 4847
public class vp_DoomsDayDevice : MonoBehaviour
{
	// Token: 0x060096FA RID: 38650 RVA: 0x003BF8F8 File Offset: 0x003BDAF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Player = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
		if (this.m_Player != null)
		{
			this.m_PlayerAudioSource = this.m_Player.GetComponent<AudioSource>();
		}
		this.m_DeviceAudioSource = base.GetComponent<AudioSource>();
		this.m_Button = GameObject.Find("ForbiddenButton");
		if (this.m_Button != null)
		{
			this.m_OriginalButtonPos = this.m_Button.transform.localPosition;
			this.m_OriginalButtonColor = this.m_Button.GetComponent<Renderer>().material.color;
		}
		this.m_PulsingLight = this.m_Button.GetComponentInChildren<vp_PulsingLight>();
		if (this.m_PulsingLight != null)
		{
			this.m_OriginalPulsingLightMaxIntensity = this.m_PulsingLight.m_MaxIntensity;
		}
	}

	// Token: 0x060096FB RID: 38651 RVA: 0x003BF9CC File Offset: 0x003BDBCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
		if (this.m_Button != null)
		{
			this.m_Button.transform.localPosition = this.m_OriginalButtonPos;
			this.m_Button.GetComponent<Renderer>().material.color = this.m_OriginalButtonColor;
		}
		if (this.m_DeviceAudioSource != null)
		{
			this.m_DeviceAudioSource.pitch = 1f;
			this.m_DeviceAudioSource.volume = 1f;
		}
		if (this.m_PulsingLight != null)
		{
			this.m_PulsingLight.m_MaxIntensity = this.m_OriginalPulsingLightMaxIntensity;
		}
	}

	// Token: 0x060096FC RID: 38652 RVA: 0x003BFA7F File Offset: 0x003BDC7F
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x060096FD RID: 38653 RVA: 0x003BFA9C File Offset: 0x003BDC9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (this.Initiated)
		{
			if (this.m_Button != null)
			{
				this.m_Button.GetComponent<Renderer>().material.color = Color.Lerp(this.m_Button.GetComponent<Renderer>().material.color, this.m_OriginalButtonColor * 0.2f, Time.deltaTime * 1.5f);
			}
			if (this.m_DeviceAudioSource != null)
			{
				this.m_DeviceAudioSource.pitch -= Time.deltaTime * 0.35f;
			}
			if (this.m_PulsingLight != null)
			{
				this.m_PulsingLight.m_MaxIntensity = 2.5f;
			}
		}
	}

	// Token: 0x060096FE RID: 38654 RVA: 0x003BFB58 File Offset: 0x003BDD58
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitiateDoomsDay()
	{
		if (this.Initiated)
		{
			return;
		}
		this.Initiated = true;
		if (this.m_Button != null)
		{
			this.m_Button.transform.localPosition += Vector3.down * 0.02f;
		}
		if (this.m_PlayerAudioSource != null)
		{
			this.m_PlayerAudioSource.PlayOneShot(this.EarthQuakeSound);
		}
		this.m_Player.CameraEarthQuake.TryStart<Vector3>(new Vector3(0.05f, 0.05f, 10f));
		vp_Timer.In(3f, delegate()
		{
			base.SendMessage("Die");
		}, null);
		vp_Timer.In(3f, delegate()
		{
			this.Initiated = false;
		}, null);
	}

	// Token: 0x040072D1 RID: 29393
	public AudioClip EarthQuakeSound;

	// Token: 0x040072D2 RID: 29394
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;

	// Token: 0x040072D3 RID: 29395
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool Initiated;

	// Token: 0x040072D4 RID: 29396
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GameObject m_Button;

	// Token: 0x040072D5 RID: 29397
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PulsingLight m_PulsingLight;

	// Token: 0x040072D6 RID: 29398
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_PlayerAudioSource;

	// Token: 0x040072D7 RID: 29399
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_DeviceAudioSource;

	// Token: 0x040072D8 RID: 29400
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_OriginalButtonPos;

	// Token: 0x040072D9 RID: 29401
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_OriginalButtonColor;

	// Token: 0x040072DA RID: 29402
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_OriginalPulsingLightMaxIntensity;
}
