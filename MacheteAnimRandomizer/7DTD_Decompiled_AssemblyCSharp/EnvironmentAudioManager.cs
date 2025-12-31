using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A2 RID: 162
public class EnvironmentAudioManager : MonoBehaviour, IGamePrefsChangedListener
{
	// Token: 0x060002F0 RID: 752 RVA: 0x00016B53 File Offset: 0x00014D53
	public static void DestroyInstance()
	{
		if (EnvironmentAudioManager.Instance != null && EnvironmentAudioManager.Instance.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(EnvironmentAudioManager.Instance.gameObject);
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00016B83 File Offset: 0x00014D83
	public static IEnumerator CreateNewInstance()
	{
		if (!GameManager.IsDedicatedServer)
		{
			EnvironmentAudioManager.DestroyInstance();
			if (EnvironmentAudioManager.loadedPrefab == null)
			{
				EnvironmentAudioManager.<>c__DisplayClass42_0 CS$<>8__locals1 = new EnvironmentAudioManager.<>c__DisplayClass42_0();
				CS$<>8__locals1.requestTask = LoadManager.LoadAsset<GameObject>("@:Sounds/Prefabs/EnvironmentAudioMaster.prefab", null, null, false, false);
				yield return new WaitUntil(() => CS$<>8__locals1.requestTask.IsDone);
				EnvironmentAudioManager.loadedPrefab = CS$<>8__locals1.requestTask.Asset;
				CS$<>8__locals1 = null;
			}
			EnvironmentAudioManager.Instance = (UnityEngine.Object.Instantiate(EnvironmentAudioManager.loadedPrefab) as GameObject).GetComponent<EnvironmentAudioManager>();
		}
		yield break;
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00016B8C File Offset: 0x00014D8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (EnvironmentAudioManager.sourceSounds == null)
		{
			EnvironmentAudioManager.sourceSounds = new GameObject();
			EnvironmentAudioManager.sourceSounds.name = "SourceSounds";
			EnvironmentAudioManager.sourceSounds.transform.parent = base.transform;
		}
		this.InitRain();
		GamePrefs.AddChangeListener(this);
		AmbientAudioController.Instance.SetAmbientVolume(GamePrefs.GetFloat(EnumGamePrefs.OptionsAmbientVolumeLevel));
		EnvironmentAudioManager.musicVolume = GamePrefs.GetFloat(EnumGamePrefs.OptionsMusicVolumeLevel);
		this.numBiomes = Enum.GetNames(typeof(BiomeDefinition.BiomeType)).Length;
		this.numTriggers = Enum.GetNames(typeof(AudioObject.Trigger)).Length;
		this.prevTriggerValue = new float[this.numTriggers];
		for (int i = 0; i < this.numTriggers; i++)
		{
			this.prevTriggerValue[i] = 0f;
		}
		this.audioBiomes = new AudioBiome[this.numBiomes];
		for (int j = 0; j < this.numBiomes; j++)
		{
			this.audioBiomes[j] = new AudioBiome();
		}
		this.InitSounds();
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x00016C90 File Offset: 0x00014E90
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitRain()
	{
		for (int i = 0; i < this.rainClipsLowToHigh.Length; i++)
		{
			AudioSource audioSource = UnityEngine.Object.Instantiate<AudioSource>(this.rainMasterAudioSource);
			audioSource.clip = this.rainClipsLowToHigh[i];
			audioSource.transform.parent = base.transform;
			audioSource.loop = true;
			this.rainAudioSources.Add(audioSource);
		}
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00016CF0 File Offset: 0x00014EF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitSounds()
	{
		AudioObject[] array = new AudioObject[0 + this.mixedBiomeSounds.Length + this.forestOnlyBiomeSounds.Length + this.snowOnlyBiomeSounds.Length + this.desertOnlyBiomeSounds.Length + this.wastelandOnlyBiomeSounds.Length + this.waterOnlyBiomeSounds.Length + this.burnt_forestOnlyBiomeSounds.Length];
		int num = 0;
		foreach (AudioObject audioObject in this.mixedBiomeSounds)
		{
			array[num++] = audioObject;
		}
		foreach (AudioObject audioObject2 in this.forestOnlyBiomeSounds)
		{
			array[num++] = audioObject2;
		}
		foreach (AudioObject audioObject3 in this.snowOnlyBiomeSounds)
		{
			array[num++] = audioObject3;
		}
		foreach (AudioObject audioObject4 in this.desertOnlyBiomeSounds)
		{
			array[num++] = audioObject4;
		}
		foreach (AudioObject audioObject5 in this.wastelandOnlyBiomeSounds)
		{
			array[num++] = audioObject5;
		}
		foreach (AudioObject audioObject6 in this.waterOnlyBiomeSounds)
		{
			array[num++] = audioObject6;
		}
		foreach (AudioObject audioObject7 in this.burnt_forestOnlyBiomeSounds)
		{
			array[num++] = audioObject7;
		}
		int num2 = 0;
		foreach (AudioObject audioObject8 in array)
		{
			foreach (BiomeDefinition.BiomeType biomeType in audioObject8.validBiomes)
			{
				this.audioBiomes[(int)biomeType].Add(audioObject8);
				if (audioObject8.trigger == AudioObject.Trigger.Day7Times || audioObject8.trigger == AudioObject.Trigger.TimeOfDay)
				{
					num2++;
				}
			}
			audioObject8.Init();
		}
		this.fromBiomeLoops = this.audioBiomes[(int)this.fromBiome];
		this.toBiomeLoops = this.audioBiomes[(int)this.toBiome];
		this.soundsInitDone = true;
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00016EF8 File Offset: 0x000150F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		if (GameManager.Instance.IsPaused() || !this.soundsInitDone)
		{
			return;
		}
		World world = GameManager.Instance.World;
		if (world == null)
		{
			this.TurnOffSounds();
			this.UpdateRainAudio();
			return;
		}
		float deltaTime = Time.deltaTime;
		this.prevBiomeTransition = this.biomeTransition;
		this.biomeTransition += 0.1f * deltaTime;
		this.biomeTransition = Mathf.Clamp01(this.biomeTransition);
		this.fadingBiomes = false;
		if (this.prevBiomeTransition != this.biomeTransition)
		{
			this.fromBiomeLoops.TransitionFrom(this.biomeTransition);
		}
		this.toBiomeLoops.TransitionTo(this.biomeTransition);
		this.invAmountEnclosedPow = Mathf.Lerp(this.invAmountEnclosedPow, this.invAmountEnclosedTarget, deltaTime * 1.5f);
		this.UpdateRainAudio();
		this.UpdateValueTrigger(WeatherManager.Instance.GetCurrentSnowfallPercent(), AudioObject.Trigger.Snow);
		this.UpdateValueTrigger(Mathf.Clamp01(WeatherManager.currentWeather.Wind() * 0.01f + 0.12f), AudioObject.Trigger.Wind);
		if (this.thunderPlaying)
		{
			List<AudioObject> sound = this.toBiomeLoops.triggers[2].sound;
			if (sound.Count > 0)
			{
				AudioObject audioObject = sound[0];
				audioObject.SetVolume(this.invAmountEnclosedPow);
				if (!(this.thunderPlaying = audioObject.IsPlaying()))
				{
					audioObject.DestroySources();
				}
			}
		}
		if (this.thunderTriggerWorldTime > 0)
		{
			List<AudioObject> sound2 = this.toBiomeLoops.triggers[2].sound;
			if (sound2.Count > 0 && (int)world.GetWorldTime() > this.thunderTriggerWorldTime)
			{
				this.thunderTriggerWorldTime = 0;
				if (world.GetPrimaryPlayer() != null)
				{
					sound2[0].SetPosition(this.lightningPos);
					SkyManager.TriggerLightning(this.lightningPos);
					this.thunderTimer = Time.time + world.RandomRange(200f, 1000f) / 343f;
				}
			}
		}
		if (Time.time > this.thunderTimer)
		{
			this.thunderTimer = float.PositiveInfinity;
			List<AudioObject> sound3 = this.toBiomeLoops.triggers[2].sound;
			for (int i = 0; i < sound3.Count; i++)
			{
				AudioObject audioObject2 = sound3[i];
				this.thunderPlaying = true;
				audioObject2.Play();
			}
		}
		if (!this.fadingBiomes)
		{
			if (this.enteredBiome)
			{
				BiomeDefinition.BiomeType newBiome;
				if (!AudioObject.biomeIdMap.TryGetValue(this.biomeEntered, out newBiome))
				{
					return;
				}
				if (this.biomeTransition != 1f)
				{
					this.queuedBiome = newBiome;
				}
				else
				{
					this.SetNewBiome(newBiome);
				}
				this.enteredBiome = false;
			}
			if (this.queuedBiome != BiomeDefinition.BiomeType.Any && this.biomeTransition >= 1f)
			{
				this.SetNewBiome(this.queuedBiome);
			}
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x000171A0 File Offset: 0x000153A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRainAudio()
	{
		float currentRainfallPercent = WeatherManager.Instance.GetCurrentRainfallPercent();
		float num = Time.deltaTime * 0.025f;
		if (currentRainfallPercent <= 0f)
		{
			this.IncrementRainVolumes(-num, -num, -num);
		}
		else if (currentRainfallPercent < 0.28f)
		{
			this.IncrementRainVolumes(num, -num, -num);
		}
		else if (currentRainfallPercent < 0.56f)
		{
			this.IncrementRainVolumes(-num, num, -num);
		}
		else
		{
			this.IncrementRainVolumes(-num, -num, num);
		}
		foreach (AudioSource audioSource in this.rainAudioSources)
		{
			if (audioSource.volume == 0f)
			{
				audioSource.Stop();
			}
			else if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00017274 File Offset: 0x00015474
	[PublicizedFrom(EAccessModifier.Private)]
	public void IncrementRainVolumes(float inc0, float inc1, float inc2)
	{
		float num = 0.25f * this.invAmountEnclosedPow * EnvironmentAudioManager.GlobalEnvironmentVolumeScale;
		this.currentRainVolume[0] = Utils.FastClamp01(this.currentRainVolume[0] + inc0);
		this.rainAudioSources[0].volume = Utils.FastClamp01(this.currentRainVolume[0] * num);
		this.currentRainVolume[1] = Utils.FastClamp01(this.currentRainVolume[1] + inc1);
		this.rainAudioSources[1].volume = Utils.FastClamp01(this.currentRainVolume[1] * num);
		this.currentRainVolume[2] = Utils.FastClamp01(this.currentRainVolume[2] + inc2);
		this.rainAudioSources[2].volume = Utils.FastClamp01(this.currentRainVolume[2] * num);
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0001733C File Offset: 0x0001553C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateValueTrigger(float value, AudioObject.Trigger trigger)
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		if (primaryPlayer.Stats == null)
		{
			return;
		}
		if (this.biomeTransition != this.prevBiomeTransition || this.prevTriggerValue[(int)trigger] != value || this.prevAmountEnclosed != primaryPlayer.Stats.AmountEnclosed)
		{
			this.prevTriggerValue[(int)trigger] = value;
			if (this.biomeTransition < 1f || this.prevBiomeTransition < 1f)
			{
				foreach (AudioObject audioObject in this.fromBiomeLoops.triggers[(int)trigger].sound)
				{
					audioObject.SetBiomeVolume(1f - this.biomeTransition);
					audioObject.SetValue(value);
				}
			}
			foreach (AudioObject audioObject2 in this.toBiomeLoops.triggers[(int)trigger].sound)
			{
				audioObject2.SetBiomeVolume(this.biomeTransition);
				audioObject2.SetValue(value);
			}
			this.prevAmountEnclosed = primaryPlayer.Stats.AmountEnclosed;
			this.invAmountEnclosedTarget = Mathf.Pow(1f - this.prevAmountEnclosed, 2f);
			if (this.invAmountEnclosedPow < 0f)
			{
				this.invAmountEnclosedPow = this.invAmountEnclosedTarget;
			}
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x000174C0 File Offset: 0x000156C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetNewBiome(BiomeDefinition.BiomeType newBiome)
	{
		if (this.audioBiomes == null)
		{
			return;
		}
		this.fromBiome = this.toBiome;
		this.toBiome = newBiome;
		this.queuedBiome = BiomeDefinition.BiomeType.Any;
		this.biomeTransition = 0f;
		this.fromBiomeLoops = this.audioBiomes[(int)this.fromBiome];
		this.toBiomeLoops = this.audioBiomes[(int)this.toBiome];
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00017524 File Offset: 0x00015724
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		for (int i = 0; i < this.rainAudioSources.Count; i++)
		{
			if (!(this.rainAudioSources[i] == null))
			{
				this.rainAudioSources[i].Stop();
				GameObject gameObject = this.rainAudioSources[i].transform.gameObject;
				UnityEngine.Object.DestroyImmediate(this.rainAudioSources[i]);
				if (gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
			}
		}
		this.rainAudioSources.Clear();
		this.TurnOffSounds();
		this.fromBiomeLoops = null;
		this.toBiomeLoops = null;
	}

	// Token: 0x060002FB RID: 763 RVA: 0x000175C2 File Offset: 0x000157C2
	[PublicizedFrom(EAccessModifier.Private)]
	public void TurnOffSounds()
	{
		this.fromBiomeLoops.TurnOff();
		this.toBiomeLoops.TurnOff();
	}

	// Token: 0x060002FC RID: 764 RVA: 0x000175DC File Offset: 0x000157DC
	public void Pause()
	{
		this.toBiomeLoops.Pause();
		this.fromBiomeLoops.Pause();
		for (int i = 0; i < this.rainAudioSources.Count; i++)
		{
			this.rainAudioSources[i].Pause();
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00017628 File Offset: 0x00015828
	public void UnPause()
	{
		this.toBiomeLoops.UnPause();
		this.fromBiomeLoops.UnPause();
		for (int i = 0; i < this.rainAudioSources.Count; i++)
		{
			this.rainAudioSources[i].UnPause();
		}
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00017672 File Offset: 0x00015872
	public void TriggerThunder(Vector3 _pos)
	{
		this.lightningPos = _pos;
		this.thunderTriggerWorldTime = 1;
		this.thunderTimer = float.PositiveInfinity;
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00017690 File Offset: 0x00015890
	public void EnterBiome(BiomeDefinition _biome)
	{
		if (AudioObject.biomeIdMap == null)
		{
			AudioObject.biomeIdMap = new Dictionary<byte, BiomeDefinition.BiomeType>();
			foreach (KeyValuePair<string, byte> keyValuePair in BiomeDefinition.nameToId)
			{
				for (int i = 0; i < BiomeDefinition.BiomeNames.Length; i++)
				{
					if (keyValuePair.Key.EqualsCaseInsensitive(BiomeDefinition.BiomeNames[i]))
					{
						AudioObject.biomeIdMap[keyValuePair.Value] = (BiomeDefinition.BiomeType)i;
						break;
					}
				}
			}
		}
		if (_biome != null)
		{
			this.enteredBiome = true;
			this.biomeEntered = _biome.m_Id;
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00017740 File Offset: 0x00015940
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsMusicVolumeLevel)
		{
			EnvironmentAudioManager.musicVolume = GamePrefs.GetFloat(EnumGamePrefs.OptionsMusicVolumeLevel);
		}
	}

	// Token: 0x04000381 RID: 897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cBiomeTransitionSpeed = 0.1f;

	// Token: 0x04000382 RID: 898
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRainVolumeTransitionSpeed = 0.025f;

	// Token: 0x04000383 RID: 899
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMaxRainVolume = 0.25f;

	// Token: 0x04000384 RID: 900
	public static float GlobalEnvironmentVolumeScale = 0.2f;

	// Token: 0x04000385 RID: 901
	public static float musicVolume = 1f;

	// Token: 0x04000386 RID: 902
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lightningPos;

	// Token: 0x04000387 RID: 903
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool thunderPlaying;

	// Token: 0x04000388 RID: 904
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int thunderTriggerWorldTime;

	// Token: 0x04000389 RID: 905
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float thunderTimer;

	// Token: 0x0400038A RID: 906
	public bool fadingBiomes;

	// Token: 0x0400038B RID: 907
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool enteredBiome;

	// Token: 0x0400038C RID: 908
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public byte biomeEntered;

	// Token: 0x0400038D RID: 909
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float biomeTransition = 1f;

	// Token: 0x0400038E RID: 910
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevBiomeTransition = 1f;

	// Token: 0x0400038F RID: 911
	public float invAmountEnclosedPow = -1f;

	// Token: 0x04000390 RID: 912
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float invAmountEnclosedTarget = -1f;

	// Token: 0x04000391 RID: 913
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevAmountEnclosed = -1f;

	// Token: 0x04000392 RID: 914
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] prevTriggerValue;

	// Token: 0x04000393 RID: 915
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numBiomes;

	// Token: 0x04000394 RID: 916
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numTriggers;

	// Token: 0x04000395 RID: 917
	public static EnvironmentAudioManager Instance;

	// Token: 0x04000396 RID: 918
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioBiome[] audioBiomes;

	// Token: 0x04000397 RID: 919
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition.BiomeType fromBiome;

	// Token: 0x04000398 RID: 920
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition.BiomeType toBiome;

	// Token: 0x04000399 RID: 921
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition.BiomeType queuedBiome;

	// Token: 0x0400039A RID: 922
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioBiome fromBiomeLoops;

	// Token: 0x0400039B RID: 923
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioBiome toBiomeLoops;

	// Token: 0x0400039C RID: 924
	public static GameObject sourceSounds = null;

	// Token: 0x0400039D RID: 925
	public AudioObject[] mixedBiomeSounds;

	// Token: 0x0400039E RID: 926
	public AudioObject[] forestOnlyBiomeSounds;

	// Token: 0x0400039F RID: 927
	public AudioObject[] snowOnlyBiomeSounds;

	// Token: 0x040003A0 RID: 928
	public AudioObject[] desertOnlyBiomeSounds;

	// Token: 0x040003A1 RID: 929
	public AudioObject[] wastelandOnlyBiomeSounds;

	// Token: 0x040003A2 RID: 930
	public AudioObject[] waterOnlyBiomeSounds;

	// Token: 0x040003A3 RID: 931
	public AudioObject[] burnt_forestOnlyBiomeSounds;

	// Token: 0x040003A4 RID: 932
	public AudioSource rainMasterAudioSource;

	// Token: 0x040003A5 RID: 933
	public AudioClip[] rainClipsLowToHigh;

	// Token: 0x040003A6 RID: 934
	public List<AudioSource> rainAudioSources;

	// Token: 0x040003A7 RID: 935
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] currentRainVolume = new float[3];

	// Token: 0x040003A8 RID: 936
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static UnityEngine.Object loadedPrefab = null;

	// Token: 0x040003A9 RID: 937
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool soundsInitDone;
}
