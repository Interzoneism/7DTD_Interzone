using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200133B RID: 4923
public class vp_Switch : vp_Interactable
{
	// Token: 0x060098FE RID: 39166 RVA: 0x003CDABC File Offset: 0x003CBCBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		if (this.AudioSource == null)
		{
			this.AudioSource = ((base.GetComponent<AudioSource>() == null) ? base.gameObject.AddComponent<AudioSource>() : base.GetComponent<AudioSource>());
		}
	}

	// Token: 0x060098FF RID: 39167 RVA: 0x003CDAF9 File Offset: 0x003CBCF9
	public override bool TryInteract(vp_FPPlayerEventHandler player)
	{
		if (this.Target == null)
		{
			return false;
		}
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		this.PlaySound();
		this.Target.SendMessage(this.TargetMessage, SendMessageOptions.DontRequireReceiver);
		return true;
	}

	// Token: 0x06009900 RID: 39168 RVA: 0x003CDB3C File Offset: 0x003CBD3C
	public virtual void PlaySound()
	{
		if (this.AudioSource == null)
		{
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

	// Token: 0x06009901 RID: 39169 RVA: 0x003CDBC0 File Offset: 0x003CBDC0
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
		this.TryInteract(this.m_Player);
	}

	// Token: 0x0400759E RID: 30110
	public GameObject Target;

	// Token: 0x0400759F RID: 30111
	public string TargetMessage = "";

	// Token: 0x040075A0 RID: 30112
	public AudioSource AudioSource;

	// Token: 0x040075A1 RID: 30113
	public Vector2 SwitchPitchRange = new Vector2(1f, 1.5f);

	// Token: 0x040075A2 RID: 30114
	public List<AudioClip> SwitchSounds = new List<AudioClip>();
}
