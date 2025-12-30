using System;
using MusicUtils;
using UnityEngine.Audio;

namespace DynamicMusic
{
	// Token: 0x0200173C RID: 5948
	public class MixerController : IGamePrefsChangedListener
	{
		// Token: 0x170013ED RID: 5101
		// (get) Token: 0x0600B323 RID: 45859 RVA: 0x004587B8 File Offset: 0x004569B8
		public static MixerController Instance
		{
			get
			{
				if (MixerController.instance == null)
				{
					MixerController.instance = new MixerController();
				}
				return MixerController.instance;
			}
		}

		// Token: 0x0600B324 RID: 45860 RVA: 0x004587D0 File Offset: 0x004569D0
		public void Init()
		{
			MixerController.DmsAbsoluteVolumeCurve = new LogarithmicCurve(2.0, 6.0, -80f, 0f, 0f, 1f);
			MixerController.AllCombatVolumeCurve = new LogarithmicCurve(2.0, 6.0, -4f, 0f, 0.7f, 1f);
			MixerController.Master = DataLoader.LoadAsset<AudioMixer>("@:Sound_Mixers/MasterAudioMixer.mixer", false);
			this.SetDynamicMusicVolume(GamePrefs.GetFloat(EnumGamePrefs.OptionsMusicVolumeLevel));
			GamePrefs.AddChangeListener(this);
		}

		// Token: 0x0600B325 RID: 45861 RVA: 0x0045885F File Offset: 0x00456A5F
		public void Update()
		{
			this.SetAllCombatVolume();
		}

		// Token: 0x0600B326 RID: 45862 RVA: 0x00458867 File Offset: 0x00456A67
		public void OnGamePrefChanged(EnumGamePrefs _enum)
		{
			if (_enum.Equals(EnumGamePrefs.OptionsMusicVolumeLevel))
			{
				this.SetDynamicMusicVolume(GamePrefs.GetFloat(EnumGamePrefs.OptionsMusicVolumeLevel));
			}
		}

		// Token: 0x0600B327 RID: 45863 RVA: 0x0045888A File Offset: 0x00456A8A
		public void SetDynamicMusicVolume(float _vol)
		{
			if (MixerController.Master)
			{
				MixerController.Master.SetFloat("dmsVol", MixerController.DmsAbsoluteVolumeCurve.GetMixerValue(_vol));
			}
		}

		// Token: 0x0600B328 RID: 45864 RVA: 0x004588B4 File Offset: 0x00456AB4
		public void OnSnapshotTransition()
		{
			float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsMusicVolumeLevel);
			this.SetDynamicMusicVolume(@float);
		}

		// Token: 0x0600B329 RID: 45865 RVA: 0x004588D0 File Offset: 0x00456AD0
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetAllCombatVolume()
		{
			if (MixerController.Master)
			{
				float mixerValue = MixerController.AllCombatVolumeCurve.GetMixerValue(GameManager.Instance.World.GetPrimaryPlayer().ThreatLevel.Numeric);
				AudioMixer master = MixerController.Master;
				if (master == null)
				{
					return;
				}
				master.SetFloat("AllCbtVol", mixerValue);
			}
		}

		// Token: 0x04008C45 RID: 35909
		[PublicizedFrom(EAccessModifier.Private)]
		public static MixerController instance;

		// Token: 0x04008C46 RID: 35910
		[PublicizedFrom(EAccessModifier.Private)]
		public static AudioMixer Master;

		// Token: 0x04008C47 RID: 35911
		[PublicizedFrom(EAccessModifier.Private)]
		public static LogarithmicCurve DmsAbsoluteVolumeCurve;

		// Token: 0x04008C48 RID: 35912
		[PublicizedFrom(EAccessModifier.Private)]
		public static LogarithmicCurve AllCombatVolumeCurve;
	}
}
