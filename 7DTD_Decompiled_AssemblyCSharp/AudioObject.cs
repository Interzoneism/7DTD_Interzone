using System;
using System.Collections.Generic;
using Audio;
using MusicUtils.Enums;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200009D RID: 157
[Serializable]
public class AudioObject
{
	// Token: 0x060002D2 RID: 722 RVA: 0x000158FC File Offset: 0x00013AFC
	public void Init()
	{
		foreach (AudioClip audioClip in this.audioClips)
		{
			if (audioClip != null)
			{
				AudioSource audioSource = UnityEngine.Object.Instantiate<AudioSource>(this.masterAudioSource);
				if (this.playOrder == AudioObject.PlayOrder.ByValue)
				{
					audioSource.transform.parent = EnvironmentAudioManager.Instance.transform;
					audioSource.loop = true;
					audioSource.gameObject.SetActive(false);
				}
				else
				{
					audioSource.transform.parent = EnvironmentAudioManager.sourceSounds.transform;
				}
				audioSource.clip = audioClip;
				audioSource.name = audioClip.name;
				audioSource.volume = 0f;
				audioSource.outputAudioMixerGroup = this.audioMixerGroup;
				this.runtimeAudioSrcs.Add(audioSource);
			}
		}
		this.audioClips = null;
		if (this.trigger == AudioObject.Trigger.Random)
		{
			this.repeatTime = Time.time + Manager.random.RandomRange(this.repeatFreqRange.x, this.repeatFreqRange.y);
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000159FC File Offset: 0x00013BFC
	public void SetValue(float _value)
	{
		this.value = _value;
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			float num = this.transitionCurve.Evaluate(this.value) * (float)this.runtimeAudioSrcs.Count;
			int num2 = 0;
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				float num3 = Mathf.Clamp01(num - (float)num2);
				if (num > (float)(num2 + 1))
				{
					num3 = 1f - Mathf.Clamp01(num - (float)(num2 + 1));
				}
				audioSource.volume = num3 * (this.music ? EnvironmentAudioManager.musicVolume : 1f) * this.biomeVolume * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
				if (GameManager.Instance != null && GameManager.Instance.World != null && GameManager.Instance.World.GetPrimaryPlayer() != null && GameManager.Instance.World.GetPrimaryPlayer().Stats != null)
				{
					if (this.outdoorOnly)
					{
						audioSource.volume *= EnvironmentAudioManager.Instance.invAmountEnclosedPow;
					}
					else if (this.indoorOnly)
					{
						audioSource.volume *= 1f - EnvironmentAudioManager.Instance.invAmountEnclosedPow;
					}
				}
				audioSource.gameObject.SetActive(audioSource.volume > 0f);
				if (audioSource.volume > 0f && !audioSource.isPlaying)
				{
					if (!audioSource.isActiveAndEnabled)
					{
						audioSource.gameObject.SetActive(true);
					}
					audioSource.Play();
				}
				num2++;
			}
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00015BB4 File Offset: 0x00013DB4
	public void SetBiomeVolume(float _volume)
	{
		this.biomeVolume = _volume;
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00015BC0 File Offset: 0x00013DC0
	public void Pause()
	{
		if (this.currentAudioSrc != null)
		{
			this.currentAudioSrc.Pause();
		}
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Pause();
				}
			}
		}
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00015C3C File Offset: 0x00013E3C
	public void UnPause()
	{
		if (this.currentAudioSrc != null)
		{
			this.currentAudioSrc.UnPause();
		}
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				if (audioSource.volume > 0f)
				{
					audioSource.UnPause();
				}
			}
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00015CC0 File Offset: 0x00013EC0
	public void TurnOff(bool immediate = false)
	{
		this.fadingOut = true;
		if (immediate)
		{
			if (this.currentAudioSrc != null)
			{
				this.currentAudioSrc.Stop();
			}
			if (this.playOrder == AudioObject.PlayOrder.ByValue)
			{
				foreach (AudioSource audioSource in this.runtimeAudioSrcs)
				{
					audioSource.Stop();
				}
			}
			this.DestroySources();
		}
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00015D44 File Offset: 0x00013F44
	[PublicizedFrom(EAccessModifier.Private)]
	public bool PlayConditionPasses()
	{
		World world = GameManager.Instance.World;
		EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
		if (primaryPlayer == null || primaryPlayer.Stats == null || primaryPlayer.IsDead())
		{
			return false;
		}
		if (WeatherManager.currentWeather == null)
		{
			return false;
		}
		if (WeatherManager.currentWeather.Wind() < this.minWind)
		{
			return false;
		}
		float num = world.GetWorldTime() / 24000f;
		float num2 = (num - (float)((int)num)) * 24f;
		float num3 = SkyManager.GetDawnTime() + this.dawnOffset;
		float num4 = SkyManager.GetDuskTime() + this.duskOffset;
		bool flag = SkyManager.IsBloodMoonVisible();
		if (this.outdoorOnly && primaryPlayer.Stats.AmountEnclosed >= 1f)
		{
			return false;
		}
		if (this.indoorOnly && primaryPlayer.Stats.AmountEnclosed <= 0f)
		{
			return false;
		}
		if (this.dayOnly)
		{
			if (num2 < 12f && num2 < num3 - 0.02f)
			{
				return false;
			}
			if (num2 > 12f && num2 > num4 + 0.02f)
			{
				return false;
			}
		}
		if (this.nightOnly)
		{
			if (num2 < 12f && num2 > num3 + 0.02f)
			{
				return false;
			}
			if (num2 > 12f && num2 < num4 - 0.02f)
			{
				return false;
			}
		}
		bool flag2 = false;
		ThreatLevelType category = primaryPlayer.ThreatLevel.Category;
		for (int i = 0; i < this.validThreatLevels.Length; i++)
		{
			if (this.validThreatLevels[i] == category)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			return false;
		}
		switch (this.trigger)
		{
		case AudioObject.Trigger.Dusk:
		{
			bool flag3 = EffectManager.GetValue(PassiveEffects.NoTimeDisplay, null, 0f, primaryPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 1f;
			if (num4 > num2 + 0.01f || num4 < num2 - 0.01f || flag || flag3)
			{
				return false;
			}
			break;
		}
		case AudioObject.Trigger.Dawn:
		{
			bool flag4 = EffectManager.GetValue(PassiveEffects.NoTimeDisplay, null, 0f, primaryPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 1f;
			if (num3 > num2 + 0.01f || num3 < num2 - 0.01f || flag4)
			{
				return false;
			}
			break;
		}
		case AudioObject.Trigger.Day7Dusk:
			if (!flag)
			{
				return false;
			}
			if (num4 > num2 + 0.01f || num4 < num2 - 0.01f)
			{
				return false;
			}
			break;
		case AudioObject.Trigger.Day8Dawn:
			if (SkyManager.dayCount - (float)(8 * ((int)SkyManager.dayCount / 8)) >= 1f)
			{
				return false;
			}
			if (num3 > num2 + 0.01f || num3 < num2 - 0.01f)
			{
				return false;
			}
			break;
		case AudioObject.Trigger.Random:
			if ((num2 > num4 - 0.25f && num2 < num4 + 0.25f) || (num2 > num3 - 0.25f && num2 < num3 + 0.25f))
			{
				return false;
			}
			if (world.dmsConductor != null && world.dmsConductor.IsMusicPlaying)
			{
				return false;
			}
			if (Time.time < this.repeatTime)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00016028 File Offset: 0x00014228
	public void DestroySources()
	{
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			foreach (AudioSource audioSource in this.runtimeAudioSrcs)
			{
				if (audioSource != null)
				{
					UnityEngine.Object.DestroyImmediate(audioSource.gameObject);
				}
			}
			this.runtimeAudioSrcs.Clear();
		}
		if (this.currentAudioSrc != null)
		{
			UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
			this.currentAudioSrc = null;
		}
	}

	// Token: 0x060002DA RID: 730 RVA: 0x000160C4 File Offset: 0x000142C4
	public void SetVolume(float volume)
	{
		if (this.currentAudioSrc != null)
		{
			this.currentAudioSrc.volume = volume;
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x000160E0 File Offset: 0x000142E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayAtPoint()
	{
		if (this.currentAudioSrc != null)
		{
			if (this.playAtPosition != Vector3.zero)
			{
				this.currentAudioSrc.gameObject.transform.position = this.playAtPosition - Origin.position;
			}
			this.currentAudioSrc.Play();
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00016140 File Offset: 0x00014340
	public void Play()
	{
		int index = 0;
		AudioObject.PlayOrder playOrder = this.playOrder;
		if (playOrder != AudioObject.PlayOrder.Random)
		{
			if (playOrder == AudioObject.PlayOrder.FirstToLast)
			{
				this.currentPlayNum = 1;
			}
		}
		else
		{
			index = Manager.random.RandomRange(this.runtimeAudioSrcs.Count);
		}
		if (this.currentAudioSrc)
		{
			UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
		}
		AudioSource original = this.runtimeAudioSrcs[index];
		this.currentAudioSrc = UnityEngine.Object.Instantiate<AudioSource>(original);
		GameObject gameObject = this.currentAudioSrc.gameObject;
		gameObject.name = this.name;
		gameObject.transform.SetParent(EnvironmentAudioManager.Instance.transform, false);
		gameObject.SetActive(true);
		this.currentAudioSrc.volume = (float)((this.trigger == AudioObject.Trigger.Thunder) ? 1 : 0);
		if (!this.currentAudioSrc.isPlaying)
		{
			this.PlayAtPoint();
		}
		if (this.trigger == AudioObject.Trigger.Continual)
		{
			this.currentAudioSrc.loop = true;
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00016229 File Offset: 0x00014429
	public bool IsPlaying()
	{
		return !(this.currentAudioSrc == null) && this.currentAudioSrc.isPlaying;
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00016246 File Offset: 0x00014446
	public void SetPosition(Vector3 _position)
	{
		this.playAtPosition = _position;
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00016250 File Offset: 0x00014450
	public void Update(float deltaTime)
	{
		if (this.runtimeAudioSrcs.Count == 0)
		{
			return;
		}
		if (!(this.currentAudioSrc != null))
		{
			if (this.PlayConditionPasses())
			{
				this.DestroySources();
				this.value = 0f;
				this.fadingOut = false;
				this.playTime = 0f;
				this.loopTime = Time.time;
				this.Play();
			}
			return;
		}
		if (this.playOrder != AudioObject.PlayOrder.ByValue)
		{
			if (this.playOrder != AudioObject.PlayOrder.FirstToLast || this.currentPlayNum == 1 || this.fadingOut)
			{
				this.playTime += (this.fadingOut ? (-deltaTime * 0.02f) : deltaTime);
				float num = this.fadeInSec.Evaluate(this.playTime);
				float time = this.fadeInSec[this.fadeInSec.length - 1].time;
				if (this.playTime >= time)
				{
					this.playTime = time;
				}
				float num2 = num * this.biomeVolume * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
				if (!this.name.Contains("Stinger"))
				{
					num2 *= (this.music ? EnvironmentAudioManager.musicVolume : 1f);
				}
				if (!this.fadingOut)
				{
					if (num2 == 0f)
					{
						num2 = 0.001f;
					}
				}
				else if (num2 < 0.01f)
				{
					num2 = 0f;
				}
				else
				{
					EnvironmentAudioManager.Instance.fadingBiomes = true;
				}
				this.currentAudioSrc.volume = num2;
			}
			else
			{
				this.currentAudioSrc.volume = (this.music ? EnvironmentAudioManager.musicVolume : 1f) * this.biomeVolume * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
			}
			this.currentAudioSrc.gameObject.SetActive(this.currentAudioSrc.volume > 0f && this.currentAudioSrc.isPlaying);
			if (this.playOrder == AudioObject.PlayOrder.FirstToLast && this.currentPlayNum < this.runtimeAudioSrcs.Count && (this.currentAudioSrc.volume == 0f || !this.currentAudioSrc.isPlaying))
			{
				UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
				this.currentAudioSrc = null;
				List<AudioSource> list = this.runtimeAudioSrcs;
				int num3 = this.currentPlayNum;
				this.currentPlayNum = num3 + 1;
				this.currentAudioSrc = UnityEngine.Object.Instantiate<AudioSource>(list[num3]);
				this.currentAudioSrc.transform.parent = EnvironmentAudioManager.Instance.transform;
				this.currentAudioSrc.gameObject.name = this.name;
				this.currentAudioSrc.name = this.name;
				this.currentAudioSrc.gameObject.SetActive(true);
				this.currentAudioSrc.volume = EnvironmentAudioManager.GlobalEnvironmentVolumeScale * (this.music ? EnvironmentAudioManager.musicVolume : 1f) * this.biomeVolume;
				if (!this.currentAudioSrc.isPlaying)
				{
					this.PlayAtPoint();
				}
			}
		}
		if (this.outdoorOnly)
		{
			this.currentAudioSrc.volume = this.currentAudioSrc.volume * EnvironmentAudioManager.Instance.invAmountEnclosedPow;
		}
		else if (this.indoorOnly)
		{
			this.currentAudioSrc.volume = this.currentAudioSrc.volume * (1f - EnvironmentAudioManager.Instance.invAmountEnclosedPow);
		}
		if (this.loopDuration > 0f && Time.time > this.loopTime + this.loopDuration)
		{
			this.TurnOff(false);
		}
		float num4 = GameManager.Instance.World.GetWorldTime() / 24000f;
		float num5 = (num4 - (float)((int)num4)) * 24f;
		float num6 = SkyManager.GetDawnTime() + this.dawnOffset;
		float num7 = SkyManager.GetDuskTime() + this.duskOffset;
		if (this.dayOnly && num5 < 12f && num5 < num6 - 0.02f)
		{
			this.TurnOff(false);
		}
		if (this.dayOnly && num5 > 12f && num5 > num7 + 0.02f)
		{
			this.TurnOff(false);
		}
		if (this.nightOnly && num5 < 12f && num5 > num6 + 0.02f)
		{
			this.TurnOff(false);
		}
		if (this.nightOnly && num5 > 12f && num5 < num7 - 0.02f)
		{
			this.TurnOff(false);
		}
		if (this.playOrder == AudioObject.PlayOrder.ByValue)
		{
			return;
		}
		if (this.trigger == AudioObject.Trigger.Continual && this.currentAudioSrc != null && !this.currentAudioSrc.isPlaying && this.currentAudioSrc.volume > 0f)
		{
			if (!this.currentAudioSrc.isActiveAndEnabled)
			{
				this.currentAudioSrc.gameObject.SetActive(true);
			}
			if (this.currentAudioSrc.isActiveAndEnabled)
			{
				this.PlayAtPoint();
			}
		}
		if (!(this.currentAudioSrc != null) || !this.currentAudioSrc.isPlaying || (this.fadingOut && this.currentAudioSrc.volume <= 0f))
		{
			UnityEngine.Object.DestroyImmediate(this.currentAudioSrc.gameObject);
			this.currentAudioSrc = null;
			if (this.trigger == AudioObject.Trigger.Random)
			{
				this.repeatTime = Time.time + Manager.random.RandomRange(this.repeatFreqRange.x, this.repeatFreqRange.y);
			}
		}
	}

	// Token: 0x04000343 RID: 835
	public static Dictionary<byte, BiomeDefinition.BiomeType> biomeIdMap;

	// Token: 0x04000344 RID: 836
	public string name;

	// Token: 0x04000345 RID: 837
	public AudioMixerGroup audioMixerGroup;

	// Token: 0x04000346 RID: 838
	public AudioSource masterAudioSource;

	// Token: 0x04000347 RID: 839
	public AudioClip[] audioClips;

	// Token: 0x04000348 RID: 840
	public List<AudioSource> runtimeAudioSrcs = new List<AudioSource>();

	// Token: 0x04000349 RID: 841
	public AudioObject.Trigger trigger;

	// Token: 0x0400034A RID: 842
	public bool indoorOnly;

	// Token: 0x0400034B RID: 843
	public bool outdoorOnly;

	// Token: 0x0400034C RID: 844
	public bool dayOnly;

	// Token: 0x0400034D RID: 845
	public bool nightOnly;

	// Token: 0x0400034E RID: 846
	public float duskOffset;

	// Token: 0x0400034F RID: 847
	public float dawnOffset;

	// Token: 0x04000350 RID: 848
	public float minWind;

	// Token: 0x04000351 RID: 849
	public bool affectedByEnv;

	// Token: 0x04000352 RID: 850
	public AudioObject.PlayOrder playOrder;

	// Token: 0x04000353 RID: 851
	public BiomeDefinition.BiomeType[] validBiomes;

	// Token: 0x04000354 RID: 852
	public AnimationCurve fadeInSec = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000355 RID: 853
	public AnimationCurve transitionCurve;

	// Token: 0x04000356 RID: 854
	public Vector2 repeatFreqRange;

	// Token: 0x04000357 RID: 855
	public float loopDuration;

	// Token: 0x04000358 RID: 856
	public bool music;

	// Token: 0x04000359 RID: 857
	public ThreatLevelType[] validThreatLevels;

	// Token: 0x0400035A RID: 858
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource currentAudioSrc;

	// Token: 0x0400035B RID: 859
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float loopTime;

	// Token: 0x0400035C RID: 860
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float repeatTime;

	// Token: 0x0400035D RID: 861
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool fadingOut;

	// Token: 0x0400035E RID: 862
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float value;

	// Token: 0x0400035F RID: 863
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float playTime;

	// Token: 0x04000360 RID: 864
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float biomeVolume = 1f;

	// Token: 0x04000361 RID: 865
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentPlayNum;

	// Token: 0x04000362 RID: 866
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 playAtPosition;

	// Token: 0x04000363 RID: 867
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float timeEpsilon = 0.01f;

	// Token: 0x0200009E RID: 158
	public enum Trigger
	{
		// Token: 0x04000365 RID: 869
		Rain,
		// Token: 0x04000366 RID: 870
		Snow,
		// Token: 0x04000367 RID: 871
		Thunder,
		// Token: 0x04000368 RID: 872
		TimeOfDay,
		// Token: 0x04000369 RID: 873
		Dusk,
		// Token: 0x0400036A RID: 874
		Dawn,
		// Token: 0x0400036B RID: 875
		Day7Times,
		// Token: 0x0400036C RID: 876
		Day7Dusk,
		// Token: 0x0400036D RID: 877
		Day8Dawn,
		// Token: 0x0400036E RID: 878
		Random,
		// Token: 0x0400036F RID: 879
		Continual,
		// Token: 0x04000370 RID: 880
		Wind
	}

	// Token: 0x0200009F RID: 159
	public enum PlayOrder
	{
		// Token: 0x04000372 RID: 882
		Random,
		// Token: 0x04000373 RID: 883
		FirstToLast,
		// Token: 0x04000374 RID: 884
		ByValue
	}
}
