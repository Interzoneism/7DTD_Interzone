using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200133A RID: 4922
public class vp_PlatformSwitch : vp_Interactable
{
	// Token: 0x060098F9 RID: 39161 RVA: 0x003CD851 File Offset: 0x003CBA51
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		if (this.AudioSource == null)
		{
			this.AudioSource = ((base.GetComponent<AudioSource>() == null) ? base.gameObject.AddComponent<AudioSource>() : base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x060098FA RID: 39162 RVA: 0x003CD890 File Offset: 0x003CBA90
	public override bool TryInteract(vp_FPPlayerEventHandler player)
	{
		if (this.Platform == null)
		{
			return false;
		}
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		if (Time.time < this.m_Timeout)
		{
			return false;
		}
		this.PlaySound();
		this.Platform.SendMessage("GoTo", (this.Platform.TargetedWaypoint == 0) ? 1 : 0, SendMessageOptions.DontRequireReceiver);
		this.m_Timeout = Time.time + this.SwitchTimeout;
		this.m_IsSwitched = !this.m_IsSwitched;
		return true;
	}

	// Token: 0x060098FB RID: 39163 RVA: 0x003CD920 File Offset: 0x003CBB20
	public virtual void PlaySound()
	{
		if (this.AudioSource == null)
		{
			Debug.LogWarning("Audio Source is not set");
			return;
		}
		if (this.SwitchSounds.Count == 0)
		{
			return;
		}
		AudioClip audioClip = this.SwitchSounds[UnityEngine.Random.Range(0, this.SwitchSounds.Count)];
		if (audioClip == null)
		{
			return;
		}
		this.AudioSource.pitch = UnityEngine.Random.Range(this.SwitchPitchRange.x, this.SwitchPitchRange.y);
		this.AudioSource.PlayOneShot(audioClip);
	}

	// Token: 0x060098FC RID: 39164 RVA: 0x003CD9B0 File Offset: 0x003CBBB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnTriggerEnter(Collider col)
	{
		if (this.InteractType != vp_Interactable.vp_InteractType.Trigger)
		{
			return;
		}
		foreach (string b in this.RecipientTags)
		{
			if (col.gameObject.tag == b)
			{
				goto IL_4F;
			}
		}
		return;
		IL_4F:
		if (this.m_Player == null)
		{
			this.m_Player = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
		}
		if (this.m_Player.Interactable.Get() != null && this.m_Player.Interactable.Get().GetComponent<Collider>() == col)
		{
			return;
		}
		this.TryInteract(this.m_Player);
	}

	// Token: 0x04007597 RID: 30103
	public float SwitchTimeout;

	// Token: 0x04007598 RID: 30104
	public vp_MovingPlatform Platform;

	// Token: 0x04007599 RID: 30105
	public AudioSource AudioSource;

	// Token: 0x0400759A RID: 30106
	public Vector2 SwitchPitchRange = new Vector2(1f, 1.5f);

	// Token: 0x0400759B RID: 30107
	public List<AudioClip> SwitchSounds = new List<AudioClip>();

	// Token: 0x0400759C RID: 30108
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_IsSwitched;

	// Token: 0x0400759D RID: 30109
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_Timeout;
}
