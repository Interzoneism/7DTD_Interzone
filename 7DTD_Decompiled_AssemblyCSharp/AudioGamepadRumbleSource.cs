using System;
using UnityEngine;

// Token: 0x02001249 RID: 4681
public class AudioGamepadRumbleSource
{
	// Token: 0x0600923E RID: 37438 RVA: 0x003A40D5 File Offset: 0x003A22D5
	public AudioGamepadRumbleSource()
	{
		this.samples = new float[64];
	}

	// Token: 0x0600923F RID: 37439 RVA: 0x003A40EA File Offset: 0x003A22EA
	public void SetAudioSource(AudioSource _audioSource, float _strengthMultiplier, bool _locationBased)
	{
		this.audioSrc = _audioSource;
		this.strengthMultiplier = _strengthMultiplier;
		this.locationBased = _locationBased;
		this.timeAdded = Time.time;
	}

	// Token: 0x06009240 RID: 37440 RVA: 0x003A410C File Offset: 0x003A230C
	public float GetSample(int channel)
	{
		this.audioSrc.GetOutputData(this.samples, channel);
		float num = 0f;
		for (int i = 0; i < 64; i++)
		{
			num += this.samples[i];
		}
		return num / 64f;
	}

	// Token: 0x06009241 RID: 37441 RVA: 0x003A4152 File Offset: 0x003A2352
	public void Clear()
	{
		this.audioSrc = null;
	}

	// Token: 0x04007027 RID: 28711
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSampleCount = 64;

	// Token: 0x04007028 RID: 28712
	public AudioSource audioSrc;

	// Token: 0x04007029 RID: 28713
	public float[] samples;

	// Token: 0x0400702A RID: 28714
	public float strengthMultiplier;

	// Token: 0x0400702B RID: 28715
	public bool locationBased;

	// Token: 0x0400702C RID: 28716
	public float timeAdded;
}
