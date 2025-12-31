using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001327 RID: 4903
[RequireComponent(typeof(AudioSource))]
public class vp_RandomSpawner : MonoBehaviour
{
	// Token: 0x06009883 RID: 39043 RVA: 0x003CA6D4 File Offset: 0x003C88D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (this.SpawnObjects == null)
		{
			return;
		}
		int index = UnityEngine.Random.Range(0, this.SpawnObjects.Count);
		if (this.SpawnObjects[index] == null)
		{
			return;
		}
		((GameObject)vp_Utility.Instantiate(this.SpawnObjects[index], base.transform.position, base.transform.rotation)).transform.Rotate(UnityEngine.Random.rotation.eulerAngles);
		this.m_Audio = base.GetComponent<AudioSource>();
		this.m_Audio.playOnAwake = true;
		if (this.Sound != null)
		{
			this.m_Audio.rolloffMode = AudioRolloffMode.Linear;
			this.m_Audio.clip = this.Sound;
			this.m_Audio.pitch = UnityEngine.Random.Range(this.SoundMinPitch, this.SoundMaxPitch) * Time.timeScale;
			this.m_Audio.Play();
		}
	}

	// Token: 0x040074FC RID: 29948
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x040074FD RID: 29949
	public AudioClip Sound;

	// Token: 0x040074FE RID: 29950
	public float SoundMinPitch = 0.8f;

	// Token: 0x040074FF RID: 29951
	public float SoundMaxPitch = 1.2f;

	// Token: 0x04007500 RID: 29952
	public bool RandomAngle = true;

	// Token: 0x04007501 RID: 29953
	public List<GameObject> SpawnObjects;
}
