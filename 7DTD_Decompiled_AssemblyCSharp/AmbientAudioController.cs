using System;
using MusicUtils;
using UnityEngine.Audio;

// Token: 0x02000098 RID: 152
public class AmbientAudioController : IGamePrefsChangedListener
{
	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060002B6 RID: 694 RVA: 0x000152EA File Offset: 0x000134EA
	public static AmbientAudioController Instance
	{
		get
		{
			if (AmbientAudioController.instance == null)
			{
				AmbientAudioController.instance = new AmbientAudioController();
			}
			return AmbientAudioController.instance;
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00015302 File Offset: 0x00013502
	[PublicizedFrom(EAccessModifier.Private)]
	public AmbientAudioController()
	{
		GamePrefs.AddChangeListener(this);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0001535D File Offset: 0x0001355D
	public void SetAmbientVolume(float _val)
	{
		if (AmbientAudioController.master)
		{
			AmbientAudioController.master.SetFloat("ambVol", AmbientAudioController.volumeCurve.GetMixerValue(_val));
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00015386 File Offset: 0x00013586
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsAmbientVolumeLevel)
		{
			this.SetAmbientVolume(GamePrefs.GetFloat(EnumGamePrefs.OptionsAmbientVolumeLevel));
		}
	}

	// Token: 0x04000334 RID: 820
	[PublicizedFrom(EAccessModifier.Private)]
	public static AmbientAudioController instance;

	// Token: 0x04000335 RID: 821
	[PublicizedFrom(EAccessModifier.Private)]
	public static AudioMixer master = DataLoader.LoadAsset<AudioMixer>("@:Sound_Mixers/MasterAudioMixer.mixer", false);

	// Token: 0x04000336 RID: 822
	[PublicizedFrom(EAccessModifier.Private)]
	public static LogarithmicCurve volumeCurve = new LogarithmicCurve(2.0, 6.0, -80f, 0f, 0f, 1f);
}
