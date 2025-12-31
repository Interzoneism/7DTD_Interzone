using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A1 RID: 161
[PublicizedFrom(EAccessModifier.Internal)]
public class AudioTrigger
{
	// Token: 0x060002E9 RID: 745 RVA: 0x00016A0A File Offset: 0x00014C0A
	public AudioTrigger(AudioObject.Trigger _trigger)
	{
		this.trigger = _trigger;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00016A24 File Offset: 0x00014C24
	public void Add(AudioObject _audioObject)
	{
		this.sound.Add(_audioObject);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00016A34 File Offset: 0x00014C34
	public void Update()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].Update(fixedDeltaTime);
		}
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00016A74 File Offset: 0x00014C74
	public void SetVolume(float _vol)
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].SetBiomeVolume(_vol);
		}
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00016AAC File Offset: 0x00014CAC
	public void Pause()
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].Pause();
		}
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00016AE4 File Offset: 0x00014CE4
	public void UnPause()
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].UnPause();
		}
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00016B1C File Offset: 0x00014D1C
	public void TurnOff()
	{
		for (int i = this.sound.Count - 1; i >= 0; i--)
		{
			this.sound[i].TurnOff(false);
		}
	}

	// Token: 0x0400037F RID: 895
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AudioObject.Trigger trigger;

	// Token: 0x04000380 RID: 896
	public List<AudioObject> sound = new List<AudioObject>();
}
