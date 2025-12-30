using System;

// Token: 0x0200009A RID: 154
[PublicizedFrom(EAccessModifier.Internal)]
public class AudioBiome
{
	// Token: 0x060002C7 RID: 711 RVA: 0x00015638 File Offset: 0x00013838
	public AudioBiome()
	{
		int num = Enum.GetNames(typeof(AudioObject.Trigger)).Length;
		this.triggers = new AudioTrigger[num];
		for (int i = 0; i < num; i++)
		{
			this.triggers[i] = new AudioTrigger((AudioObject.Trigger)i);
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00015683 File Offset: 0x00013883
	public void Add(AudioObject _audioObject)
	{
		this.triggers[(int)_audioObject.trigger].Add(_audioObject);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00015698 File Offset: 0x00013898
	public void TransitionFrom(float _biomeTransition)
	{
		for (int i = 3; i < this.triggers.Length - 1; i++)
		{
			AudioTrigger audioTrigger = this.triggers[i];
			if (i != 5 && i != 4)
			{
				audioTrigger.TurnOff();
				audioTrigger.SetVolume(1f - _biomeTransition);
			}
			audioTrigger.Update();
		}
	}

	// Token: 0x060002CA RID: 714 RVA: 0x000156E4 File Offset: 0x000138E4
	public void TransitionTo(float _biomeTransition)
	{
		for (int i = 3; i < this.triggers.Length - 1; i++)
		{
			AudioTrigger audioTrigger = this.triggers[i];
			if (i != 5 && i != 4)
			{
				audioTrigger.SetVolume(_biomeTransition);
			}
			audioTrigger.Update();
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00015724 File Offset: 0x00013924
	public void Pause()
	{
		for (int i = 0; i < this.triggers.Length; i++)
		{
			this.triggers[i].Pause();
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00015754 File Offset: 0x00013954
	public void UnPause()
	{
		for (int i = 0; i < this.triggers.Length; i++)
		{
			this.triggers[i].UnPause();
		}
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00015784 File Offset: 0x00013984
	public void TurnOff()
	{
		for (int i = 0; i < this.triggers.Length; i++)
		{
			this.triggers[i].TurnOff();
		}
	}

	// Token: 0x04000339 RID: 825
	public AudioTrigger[] triggers;
}
