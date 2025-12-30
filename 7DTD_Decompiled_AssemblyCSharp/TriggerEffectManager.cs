using System;
using System.Collections.Generic;
using InControl;
using Platform;
using TriggerEffects;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

// Token: 0x02001242 RID: 4674
[UnityEngine.Scripting.Preserve]
public class TriggerEffectManager : IDisposable
{
	// Token: 0x17000F1C RID: 3868
	// (get) Token: 0x0600921A RID: 37402 RVA: 0x003A3386 File Offset: 0x003A1586
	// (set) Token: 0x0600921B RID: 37403 RVA: 0x003A338E File Offset: 0x003A158E
	public bool Enabled
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this._enabled;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this._enabled = value;
			this._stateChanged = true;
		}
	}

	// Token: 0x0600921C RID: 37404 RVA: 0x003A33A0 File Offset: 0x003A15A0
	public TriggerEffectManager()
	{
		this.PollSetting();
		this._stateChanged = false;
		try
		{
			TriggerEffectDualsensePC.InitTriggerEffectManager(ref this._controllersConnected);
		}
		catch (DllNotFoundException arg)
		{
			Log.Warning(string.Format("[TriggerEffectManager] Failed to load ControllerExt, disabling. Details: {0}", arg));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerTriggerEffects, false);
			return;
		}
		TriggerEffectDualsense.InitTriggerEffectManager();
		for (int i = 0; i < this.vibrationAudioSources.Length; i++)
		{
			this.vibrationAudioSources[i] = new AudioGamepadRumbleSource();
		}
		TriggerEffectManager.UpdateControllerVibrationStrength();
		this.EnableVibration();
		this.InitializeLightbarGradient();
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.OnLastInputStyleChanged;
	}

	// Token: 0x0600921D RID: 37405 RVA: 0x003A3464 File Offset: 0x003A1664
	public void EnableVibration()
	{
		TriggerEffectDualsensePC.EnableVibration();
		TriggerEffectDualsense.EnableVibration();
	}

	// Token: 0x0600921E RID: 37406 RVA: 0x003A3470 File Offset: 0x003A1670
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeLightbarGradient()
	{
		TriggerEffectManager.lightbarGradients = Resources.Load<LightbarGradients>("Data/LightBarGradients");
	}

	// Token: 0x0600921F RID: 37407 RVA: 0x003A3481 File Offset: 0x003A1681
	public static void SetEnabled(bool _enabled)
	{
		GameManager.Instance.triggerEffectManager.Enabled = _enabled;
	}

	// Token: 0x06009220 RID: 37408 RVA: 0x003A3493 File Offset: 0x003A1693
	[PublicizedFrom(EAccessModifier.Private)]
	public byte FloatToByte(float value)
	{
		return (byte)(Mathf.Clamp01(value) * 255f);
	}

	// Token: 0x06009221 RID: 37409 RVA: 0x003A34A4 File Offset: 0x003A16A4
	public void Update()
	{
		if (GameManager.Instance.World != null && TriggerEffectManager.audioRumbleStrength > 0f && !GameManager.Instance.IsPaused() && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			int num = 0;
			int num2 = 0;
			this.targetLeftAudioStrength = 0f;
			this.targetRightAudioStrength = 0f;
			Transform transform;
			if (GameManager.Instance.World != null)
			{
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				transform = ((primaryPlayer != null) ? primaryPlayer.cameraTransform : null);
			}
			else
			{
				transform = LocalPlayerUI.primaryUI.uiCamera.transform;
			}
			foreach (AudioGamepadRumbleSource audioGamepadRumbleSource in this.vibrationAudioSources)
			{
				if (audioGamepadRumbleSource.audioSrc != null)
				{
					if (audioGamepadRumbleSource.audioSrc.isPlaying)
					{
						float num3 = audioGamepadRumbleSource.GetSample(0);
						if (audioGamepadRumbleSource.locationBased)
						{
							float num4 = 1f - Vector3.Distance(transform.position, audioGamepadRumbleSource.audioSrc.transform.position) / audioGamepadRumbleSource.audioSrc.maxDistance;
							if (num4 < 0.9f)
							{
								goto IL_184;
							}
							num3 *= num4;
						}
						num3 *= audioGamepadRumbleSource.strengthMultiplier * audioGamepadRumbleSource.audioSrc.pitch * TriggerEffectManager.audioRumbleStrength;
						if (num3 > 0f)
						{
							num2++;
							this.targetRightAudioStrength += num3;
						}
						else if (num3 < 0f)
						{
							num++;
							this.targetLeftAudioStrength += num3;
						}
					}
					else
					{
						audioGamepadRumbleSource.Clear();
					}
				}
				IL_184:;
			}
			if (num > 0 || num2 > 0)
			{
				if (num > 0)
				{
					this.targetLeftAudioStrength /= (float)num;
				}
				if (num2 > 0)
				{
					this.targetRightAudioStrength /= (float)num2;
				}
				this.leftAudioStrength = Mathf.Lerp(this.leftAudioStrength, this.targetLeftAudioStrength, Time.deltaTime * 15f);
				this.rightAudioStrength = Mathf.Lerp(this.rightAudioStrength, this.targetRightAudioStrength, Time.deltaTime * 15f);
				InputManager.ActiveDevice.Vibrate(-this.leftAudioStrength, this.rightAudioStrength);
				this.SetGamepadVibration(-this.leftAudioStrength + this.rightAudioStrength);
				this.SetDualSenseVibration(this.FloatToByte(-this.leftAudioStrength * 0.25f), this.FloatToByte(this.rightAudioStrength * 0.25f));
			}
			else
			{
				this.SetDualSenseVibration(0, 0);
				InputManager.ActiveDevice.StopVibration();
				this.SetGamepadVibration(0f);
			}
		}
		TriggerEffectDualsensePC.PCConnectedUpdate(ref this._controllersConnected, this._currentEffectLeft.DualsenseEffect, this._currentEffectRight.DualsenseEffect);
		TriggerEffectDualsense.ConnectedUpdate(this._currentEffectLeft.DualsenseEffect, this._currentEffectRight.DualsenseEffect);
		if (this.Enabled && !this.inUI)
		{
			TriggerEffectDualsensePC.PCTriggerUpdate(this._stateChanged, this._currentEffectLeft.DualsenseEffect, this._currentEffectRight.DualsenseEffect);
			TriggerEffectDualsense.Update(this._currentEffectLeft.DualsenseEffect, this._currentEffectRight.DualsenseEffect);
		}
		else if (this._stateChanged && (!this.Enabled || this.inUI))
		{
			this.SetGamepadTriggerEffectOff(TriggerEffectManager.GamepadTrigger.RightTrigger);
			this.SetGamepadTriggerEffectOff(TriggerEffectManager.GamepadTrigger.LeftTrigger);
			TriggerEffectDualsensePC.DualsensePCSetEffectToOff();
			TriggerEffectDualsense.SetEffectToOff();
			this._stateChanged = true;
		}
		this._triggerSetLeft = false;
		this._triggerSetRight = false;
		this._stateChanged = false;
	}

	// Token: 0x06009222 RID: 37410 RVA: 0x003A3805 File Offset: 0x003A1A05
	public void SetGamepadTriggerEffectOff(TriggerEffectManager.GamepadTrigger trigger)
	{
		if (trigger != TriggerEffectManager.GamepadTrigger.LeftTrigger)
		{
			if (trigger != TriggerEffectManager.GamepadTrigger.RightTrigger)
			{
				throw new ArgumentOutOfRangeException("trigger", trigger, null);
			}
			this._currentEffectRight = TriggerEffectManager.NoneEffect;
		}
		else
		{
			this._currentEffectLeft = TriggerEffectManager.NoneEffect;
		}
		this._stateChanged = true;
	}

	// Token: 0x06009223 RID: 37411 RVA: 0x003A3843 File Offset: 0x003A1A43
	public void SetWeaponEffect(int userID, TriggerEffectManager.GamepadTrigger trigger, byte startPosition, byte endPosition, byte strength)
	{
		if (!this.Enabled)
		{
			return;
		}
		TriggerEffectDualsensePC.SetWeaponEffect(userID, trigger, startPosition, endPosition, strength);
		TriggerEffectDualsense.SetWeaponEffect(userID, trigger, startPosition, endPosition, strength);
		this._stateChanged = true;
	}

	// Token: 0x06009224 RID: 37412 RVA: 0x003A386D File Offset: 0x003A1A6D
	public void ResetControllerIdentification()
	{
		TriggerEffectDualsensePC.ResetControllerIdentification();
		TriggerEffectDualsense.ResetControllerIdentification();
		this._stateChanged = true;
	}

	// Token: 0x06009225 RID: 37413 RVA: 0x003A3880 File Offset: 0x003A1A80
	public void SetControllerIdentification()
	{
		TriggerEffectDualsensePC.SetControllerIdentification();
		TriggerEffectDualsense.SetControllerIdentification();
		this._stateChanged = true;
	}

	// Token: 0x06009226 RID: 37414 RVA: 0x003A3893 File Offset: 0x003A1A93
	public void SetTriggerEffectVibration(int userID, TriggerEffectManager.GamepadTrigger trigger, byte position, byte amplitude, byte frequency)
	{
		if (!this.Enabled)
		{
			return;
		}
		TriggerEffectDualsensePC.SetTriggerEffectVibration(userID, trigger, position, amplitude, frequency);
		TriggerEffectDualsense.SetTriggerEffectVibration(userID, trigger, position, amplitude, frequency);
		this._stateChanged = true;
	}

	// Token: 0x06009227 RID: 37415 RVA: 0x003A38BD File Offset: 0x003A1ABD
	public void SetTriggerEffectVibrationMultiplePosition(int userID, TriggerEffectManager.GamepadTrigger trigger, byte[] amplitudes, byte frequency)
	{
		if (!this.Enabled)
		{
			return;
		}
		TriggerEffectDualsensePC.SetTriggerEffectVibrationMultiplePosition(userID, trigger, amplitudes, frequency);
		TriggerEffectDualsense.SetTriggerEffectVibrationMultiplePosition(userID, trigger, amplitudes, frequency);
		this._stateChanged = true;
	}

	// Token: 0x06009228 RID: 37416 RVA: 0x003A38E4 File Offset: 0x003A1AE4
	public void Shutdown()
	{
		this.StopGamepadVibration();
		TriggerEffectManager.LibShutdown();
		TriggerEffectDualsense.ResetControllerIdentification();
		TriggerEffectDualsensePC.ResetControllerIdentification();
		for (int i = 1; i < 5; i++)
		{
			this._controllersConnected[i - 1] = false;
		}
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged -= this.OnLastInputStyleChanged;
		this.Enabled = false;
	}

	// Token: 0x06009229 RID: 37417 RVA: 0x003A393F File Offset: 0x003A1B3F
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LibShutdown()
	{
		TriggerEffectDualsensePC.LibShutdown();
	}

	// Token: 0x0600922A RID: 37418 RVA: 0x003A3946 File Offset: 0x003A1B46
	public void Dispose()
	{
		this.Shutdown();
	}

	// Token: 0x0600922B RID: 37419 RVA: 0x003A394E File Offset: 0x003A1B4E
	public void PollSetting()
	{
		this.Enabled = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerTriggerEffects);
	}

	// Token: 0x17000F1D RID: 3869
	// (get) Token: 0x0600922C RID: 37420 RVA: 0x003A3960 File Offset: 0x003A1B60
	// (set) Token: 0x0600922D RID: 37421 RVA: 0x003A3968 File Offset: 0x003A1B68
	public bool inUI
	{
		get
		{
			return this._inUI;
		}
		set
		{
			if (this._inUI != value)
			{
				this._inUI = value;
				this._stateChanged = true;
			}
		}
	}

	// Token: 0x0600922E RID: 37422 RVA: 0x003A3981 File Offset: 0x003A1B81
	public void SetGamepadVibration(float strength)
	{
		this._currentControllerVibrationStrength = strength;
		this._stateChanged = true;
	}

	// Token: 0x0600922F RID: 37423 RVA: 0x003A3991 File Offset: 0x003A1B91
	public void StopGamepadVibration()
	{
		this._currentControllerVibrationStrength = 0f;
		this._stateChanged = true;
		this.SetDualSenseVibration(0, 0);
		InputManager.ActiveDevice.StopVibration();
	}

	// Token: 0x06009230 RID: 37424 RVA: 0x003A39B7 File Offset: 0x003A1BB7
	public void SetTriggerEffect(TriggerEffectManager.ControllerTriggerEffect effect)
	{
		if (!this.Enabled)
		{
			return;
		}
		this.SetTriggerEffect(TriggerEffectManager.GamepadTrigger.LeftTrigger, effect, false);
		this.SetTriggerEffect(TriggerEffectManager.GamepadTrigger.RightTrigger, effect, false);
	}

	// Token: 0x06009231 RID: 37425 RVA: 0x003A39D4 File Offset: 0x003A1BD4
	public void SetTriggerEffect(TriggerEffectManager.GamepadTrigger trigger, TriggerEffectManager.ControllerTriggerEffect effect, bool asap = false)
	{
		if (!this.Enabled)
		{
			return;
		}
		this._stateChanged = true;
		if (trigger == TriggerEffectManager.GamepadTrigger.LeftTrigger)
		{
			this._currentEffectLeft.DualsenseEffect = effect.DualsenseEffect;
			if (asap)
			{
				TriggerEffectDualsensePC.ApplyImmediate(trigger, this._currentEffectLeft);
			}
			this._triggerSetLeft = true;
			return;
		}
		if (trigger != TriggerEffectManager.GamepadTrigger.RightTrigger)
		{
			return;
		}
		this._currentEffectRight.DualsenseEffect = effect.DualsenseEffect;
		if (asap)
		{
			TriggerEffectDualsensePC.ApplyImmediate(trigger, this._currentEffectRight);
		}
		this._triggerSetRight = true;
	}

	// Token: 0x06009232 RID: 37426 RVA: 0x003A3A48 File Offset: 0x003A1C48
	public static TriggerEffectManager.ControllerTriggerEffect GetTriggerEffect(ValueTuple<string, string> triggerEffectNames)
	{
		if (string.IsNullOrEmpty(triggerEffectNames.Item1) || string.IsNullOrEmpty(triggerEffectNames.Item2) || (triggerEffectNames.Item1.Contains("NoEffect") && triggerEffectNames.Item2.Contains("NoEffect")) || (triggerEffectNames.Item1.Contains("NoneEffect") && triggerEffectNames.Item2.Contains("NoneEffect")))
		{
			return TriggerEffectManager.NoneEffect;
		}
		TriggerEffectManager.ControllerTriggerEffect result = new TriggerEffectManager.ControllerTriggerEffect
		{
			DualsenseEffect = TriggerEffectManager.NoneEffectDs,
			XboxTriggerEffect = TriggerEffectManager.NoneEffectXb
		};
		TriggerEffectManager.TriggerEffectDS dualsenseEffect;
		if (TriggerEffectManager.ControllerTriggerEffectsDS.TryGetValue(triggerEffectNames.Item1, out dualsenseEffect))
		{
			result.DualsenseEffect = dualsenseEffect;
		}
		else
		{
			Debug.LogWarning("Failed to find trigger effect DS: " + triggerEffectNames.Item1);
		}
		TriggerEffectManager.TriggerEffectXB xboxTriggerEffect;
		if (TriggerEffectManager.ControllerTriggerEffectsXb.TryGetValue(triggerEffectNames.Item2, out xboxTriggerEffect))
		{
			result.XboxTriggerEffect = xboxTriggerEffect;
		}
		else
		{
			Debug.LogWarning("Failed to find trigger effect XB: " + triggerEffectNames.Item2);
		}
		return result;
	}

	// Token: 0x06009233 RID: 37427 RVA: 0x003A3B48 File Offset: 0x003A1D48
	public static TriggerEffectManager.ControllerTriggerEffect GetTriggerEffect(string dualsenseTrigger, string impulseTrigger)
	{
		if (string.IsNullOrEmpty(dualsenseTrigger) || string.IsNullOrEmpty(impulseTrigger) || (dualsenseTrigger.Contains("NoEffect") && impulseTrigger.Contains("NoEffect")) || (dualsenseTrigger.Contains("NoneEffect") && impulseTrigger.Contains("NoneEffect")))
		{
			return TriggerEffectManager.NoneEffect;
		}
		TriggerEffectManager.ControllerTriggerEffect result = new TriggerEffectManager.ControllerTriggerEffect
		{
			DualsenseEffect = TriggerEffectManager.NoneEffectDs,
			XboxTriggerEffect = TriggerEffectManager.NoneEffectXb
		};
		TriggerEffectManager.TriggerEffectDS dualsenseEffect;
		if (TriggerEffectManager.ControllerTriggerEffectsDS.TryGetValue(dualsenseTrigger, out dualsenseEffect))
		{
			result.DualsenseEffect = dualsenseEffect;
		}
		else
		{
			Debug.LogWarning("Failed to find trigger effect DS: " + dualsenseTrigger);
		}
		TriggerEffectManager.TriggerEffectXB xboxTriggerEffect;
		if (TriggerEffectManager.ControllerTriggerEffectsXb.TryGetValue(impulseTrigger, out xboxTriggerEffect))
		{
			result.XboxTriggerEffect = xboxTriggerEffect;
		}
		else
		{
			Debug.LogWarning("Failed to find trigger effect XB: " + impulseTrigger);
		}
		return result;
	}

	// Token: 0x06009234 RID: 37428 RVA: 0x003A3C14 File Offset: 0x003A1E14
	public static ValueTuple<TriggerEffectManager.TriggerEffectDS, TriggerEffectManager.TriggerEffectXB> GetTriggerEffectAsTuple(ValueTuple<string, string> triggerEffectNames)
	{
		if (string.IsNullOrEmpty(triggerEffectNames.Item1) || string.IsNullOrEmpty(triggerEffectNames.Item2) || (triggerEffectNames.Item1.Contains("NoEffect") && triggerEffectNames.Item2.Contains("NoEffect")) || (triggerEffectNames.Item1.Contains("NoneEffect") && triggerEffectNames.Item2.Contains("NoneEffect")))
		{
			return new ValueTuple<TriggerEffectManager.TriggerEffectDS, TriggerEffectManager.TriggerEffectXB>(TriggerEffectManager.NoneEffect.DualsenseEffect, TriggerEffectManager.NoneEffect.XboxTriggerEffect);
		}
		ValueTuple<TriggerEffectManager.TriggerEffectDS, TriggerEffectManager.TriggerEffectXB> result = new ValueTuple<TriggerEffectManager.TriggerEffectDS, TriggerEffectManager.TriggerEffectXB>(TriggerEffectManager.NoneEffect.DualsenseEffect, TriggerEffectManager.NoneEffect.XboxTriggerEffect);
		TriggerEffectManager.TriggerEffectDS item;
		if (TriggerEffectManager.ControllerTriggerEffectsDS.TryGetValue(triggerEffectNames.Item1, out item))
		{
			result.Item1 = item;
		}
		else
		{
			Debug.LogWarning("Failed to find trigger effect DS: " + triggerEffectNames.Item1);
		}
		TriggerEffectManager.TriggerEffectXB item2;
		if (TriggerEffectManager.ControllerTriggerEffectsXb.TryGetValue(triggerEffectNames.Item2, out item2))
		{
			result.Item2 = item2;
		}
		else
		{
			Debug.LogWarning("Failed to find trigger effect XB: " + triggerEffectNames.Item2);
		}
		return result;
	}

	// Token: 0x06009235 RID: 37429 RVA: 0x003A3D1E File Offset: 0x003A1F1E
	public static bool SettingDefaultValue()
	{
		return Application.platform == RuntimePlatform.PS5 || Application.platform == RuntimePlatform.GameCoreXboxSeries || Application.platform == RuntimePlatform.WindowsEditor;
	}

	// Token: 0x06009236 RID: 37430 RVA: 0x003A3D3C File Offset: 0x003A1F3C
	public void SetAudioRumbleSource(AudioSource _audioSource, float _strengthMultiplier, bool _locationBased)
	{
		AudioGamepadRumbleSource audioGamepadRumbleSource = null;
		float num = float.MaxValue;
		foreach (AudioGamepadRumbleSource audioGamepadRumbleSource2 in this.vibrationAudioSources)
		{
			if (!(audioGamepadRumbleSource2.audioSrc != null))
			{
				audioGamepadRumbleSource2.SetAudioSource(_audioSource, _strengthMultiplier, _locationBased);
				return;
			}
			if (audioGamepadRumbleSource2.timeAdded < num)
			{
				audioGamepadRumbleSource = audioGamepadRumbleSource2;
				num = audioGamepadRumbleSource2.timeAdded;
			}
		}
		if (audioGamepadRumbleSource != null)
		{
			audioGamepadRumbleSource.SetAudioSource(_audioSource, _strengthMultiplier, _locationBased);
		}
	}

	// Token: 0x06009237 RID: 37431 RVA: 0x003A3DA8 File Offset: 0x003A1FA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDualSenseVibration(byte _smallMotor, byte _largeMotor)
	{
		TriggerEffectDualsense.SetDualSenseVibration(_smallMotor, _largeMotor);
		TriggerEffectDualsensePC.SetDualSenseVibration(_smallMotor, _largeMotor);
	}

	// Token: 0x06009238 RID: 37432 RVA: 0x003A3DB8 File Offset: 0x003A1FB8
	public static void UpdateControllerVibrationStrength()
	{
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsControllerVibrationStrength))
		{
		case 0:
			TriggerEffectManager.audioRumbleStrength = 0f;
			return;
		case 1:
			TriggerEffectManager.audioRumbleStrength = 0.5f;
			return;
		case 2:
			TriggerEffectManager.audioRumbleStrength = 1f;
			return;
		case 3:
			TriggerEffectManager.audioRumbleStrength = 2f;
			return;
		default:
			return;
		}
	}

	// Token: 0x06009239 RID: 37433 RVA: 0x003A3E12 File Offset: 0x003A2012
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLastInputStyleChanged(PlayerInputManager.InputStyle _style)
	{
		if (_style == PlayerInputManager.InputStyle.Keyboard)
		{
			this.StopGamepadVibration();
		}
	}

	// Token: 0x0600923A RID: 37434 RVA: 0x003A3E20 File Offset: 0x003A2020
	public static void UpdateDualSenseLightFromWeather(WeatherManager.BiomeWeather weather)
	{
		if (TriggerEffectManager.lightbarGradients == null || weather == null)
		{
			return;
		}
		float num = GameManager.Instance.World.GetWorldTime() % 24000UL;
		float num2 = SkyManager.GetDawnTime() * 1000f;
		float num3 = SkyManager.GetDuskTime() * 1000f;
		bool flag = num < num2 || num > num3;
		float time;
		if (flag)
		{
			float num4 = 24000f - num3;
			float num5 = num4 + num2;
			if (num >= num3 && num < 24000f)
			{
				time = (num - num3) / num5;
			}
			else
			{
				time = num4 / num5 + num / (num2 + num4);
			}
		}
		else
		{
			float num6 = num3 - num2;
			time = (num - num2) / num6;
		}
		Color dualSenseLightbarColor;
		if (SkyManager.BloodMoonVisiblePercent() == 1f)
		{
			float time2 = (1f + Mathf.Sin(Time.time)) / 2f;
			dualSenseLightbarColor = TriggerEffectManager.lightbarGradients.bloodmoonGradient.Evaluate(time2);
		}
		else if (weather.rainParam.value >= 0.5f || (weather.biomeDefinition != null && (weather.biomeDefinition.m_BiomeType == BiomeDefinition.BiomeType.Wasteland || weather.biomeDefinition.m_BiomeType == BiomeDefinition.BiomeType.burnt_forest)))
		{
			if (flag)
			{
				dualSenseLightbarColor = TriggerEffectManager.lightbarGradients.cloudNightGradient.Evaluate(time);
			}
			else
			{
				dualSenseLightbarColor = TriggerEffectManager.lightbarGradients.cloudDayGradient.Evaluate(time);
			}
		}
		else if (flag)
		{
			dualSenseLightbarColor = TriggerEffectManager.lightbarGradients.nightGradient.Evaluate(time);
		}
		else
		{
			dualSenseLightbarColor = TriggerEffectManager.lightbarGradients.dayGradient.Evaluate(time);
		}
		TriggerEffectManager.SetDualSenseLightbarColor(dualSenseLightbarColor);
	}

	// Token: 0x0600923B RID: 37435 RVA: 0x003A3F94 File Offset: 0x003A2194
	public static void SetDualSenseLightbarColor(Color color)
	{
		for (int i = 1; i < 5; i++)
		{
			TriggerEffectDualsense.SetLightbar(i, (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f));
			TriggerEffectDualsensePC.SetLightbar(i, (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f));
		}
	}

	// Token: 0x0600923C RID: 37436 RVA: 0x003A4007 File Offset: 0x003A2207
	public static void SetMainMenuLightbarColor()
	{
		TriggerEffectManager.SetDualSenseLightbarColor(TriggerEffectManager.lightbarGradients.mainMenuColor);
	}

	// Token: 0x04006FED RID: 28653
	public static readonly Dictionary<string, TriggerEffectManager.TriggerEffectDS> ControllerTriggerEffectsDS = new Dictionary<string, TriggerEffectManager.TriggerEffectDS>();

	// Token: 0x04006FEE RID: 28654
	public static readonly Dictionary<string, TriggerEffectManager.TriggerEffectXB> ControllerTriggerEffectsXb = new Dictionary<string, TriggerEffectManager.TriggerEffectXB>();

	// Token: 0x04006FEF RID: 28655
	[PublicizedFrom(EAccessModifier.Internal)]
	public static readonly TriggerEffectManager.TriggerEffectDS NoneEffectDs = new TriggerEffectManager.TriggerEffectDS
	{
		Effect = TriggerEffectManager.EffectDualsense.Off,
		AmplitudeEndStrength = 0,
		Frequency = 0,
		Position = 0,
		EndPosition = 0,
		Strength = 0,
		Strengths = Array.Empty<byte>()
	};

	// Token: 0x04006FF0 RID: 28656
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly TriggerEffectManager.TriggerEffectXB NoneEffectXb = new TriggerEffectManager.TriggerEffectXB
	{
		Effect = TriggerEffectManager.EffectXbox.Off,
		Strength = 0f
	};

	// Token: 0x04006FF1 RID: 28657
	public static readonly TriggerEffectManager.ControllerTriggerEffect NoneEffect = new TriggerEffectManager.ControllerTriggerEffect
	{
		DualsenseEffect = TriggerEffectManager.NoneEffectDs,
		XboxTriggerEffect = TriggerEffectManager.NoneEffectXb
	};

	// Token: 0x04006FF2 RID: 28658
	[PublicizedFrom(EAccessModifier.Private)]
	public bool[] _controllersConnected = new bool[4];

	// Token: 0x04006FF3 RID: 28659
	[PublicizedFrom(EAccessModifier.Private)]
	public float _currentControllerVibrationStrength;

	// Token: 0x04006FF4 RID: 28660
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _enabled;

	// Token: 0x04006FF5 RID: 28661
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _triggerSetLeft;

	// Token: 0x04006FF6 RID: 28662
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _triggerSetRight;

	// Token: 0x04006FF7 RID: 28663
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _stateChanged;

	// Token: 0x04006FF8 RID: 28664
	[PublicizedFrom(EAccessModifier.Private)]
	public TriggerEffectManager.ControllerTriggerEffect _currentEffectLeft;

	// Token: 0x04006FF9 RID: 28665
	[PublicizedFrom(EAccessModifier.Private)]
	public TriggerEffectManager.ControllerTriggerEffect _currentEffectRight;

	// Token: 0x04006FFA RID: 28666
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cAudioRumbleStrengthSubtle = 0.5f;

	// Token: 0x04006FFB RID: 28667
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cAudioRumbleStrengthStandard = 1f;

	// Token: 0x04006FFC RID: 28668
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cAudioRumbleStrengthStrong = 2f;

	// Token: 0x04006FFD RID: 28669
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDualSenseRumbleStrengthMultiplier = 0.25f;

	// Token: 0x04006FFE RID: 28670
	[PublicizedFrom(EAccessModifier.Private)]
	public static float audioRumbleStrength = 1f;

	// Token: 0x04006FFF RID: 28671
	public AudioGamepadRumbleSource[] vibrationAudioSources = new AudioGamepadRumbleSource[5];

	// Token: 0x04007000 RID: 28672
	[PublicizedFrom(EAccessModifier.Protected)]
	public static LightbarGradients lightbarGradients;

	// Token: 0x04007001 RID: 28673
	[PublicizedFrom(EAccessModifier.Private)]
	public float leftAudioStrength;

	// Token: 0x04007002 RID: 28674
	[PublicizedFrom(EAccessModifier.Private)]
	public float rightAudioStrength;

	// Token: 0x04007003 RID: 28675
	[PublicizedFrom(EAccessModifier.Private)]
	public float targetLeftAudioStrength;

	// Token: 0x04007004 RID: 28676
	[PublicizedFrom(EAccessModifier.Private)]
	public float targetRightAudioStrength;

	// Token: 0x04007005 RID: 28677
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _inUI;

	// Token: 0x02001243 RID: 4675
	public enum GamepadTrigger
	{
		// Token: 0x04007007 RID: 28679
		LeftTrigger,
		// Token: 0x04007008 RID: 28680
		RightTrigger
	}

	// Token: 0x02001244 RID: 4676
	public enum EffectDualsense
	{
		// Token: 0x0400700A RID: 28682
		Off,
		// Token: 0x0400700B RID: 28683
		WeaponSingle,
		// Token: 0x0400700C RID: 28684
		WeaponMultipoint,
		// Token: 0x0400700D RID: 28685
		FeedbackSingle,
		// Token: 0x0400700E RID: 28686
		VibrationSingle,
		// Token: 0x0400700F RID: 28687
		FeedbackSlope,
		// Token: 0x04007010 RID: 28688
		VibrationSlope,
		// Token: 0x04007011 RID: 28689
		FeedbackMultipoint,
		// Token: 0x04007012 RID: 28690
		VibrationMultipoint
	}

	// Token: 0x02001245 RID: 4677
	public struct TriggerEffectDS
	{
		// Token: 0x04007013 RID: 28691
		public TriggerEffectManager.EffectDualsense Effect;

		// Token: 0x04007014 RID: 28692
		public byte Position;

		// Token: 0x04007015 RID: 28693
		public byte EndPosition;

		// Token: 0x04007016 RID: 28694
		public byte Frequency;

		// Token: 0x04007017 RID: 28695
		public byte AmplitudeEndStrength;

		// Token: 0x04007018 RID: 28696
		public byte Strength;

		// Token: 0x04007019 RID: 28697
		public byte[] Strengths;
	}

	// Token: 0x02001246 RID: 4678
	public enum EffectXbox
	{
		// Token: 0x0400701B RID: 28699
		Off,
		// Token: 0x0400701C RID: 28700
		FeedbackSingle,
		// Token: 0x0400701D RID: 28701
		VibrationSingle,
		// Token: 0x0400701E RID: 28702
		FeedbackSlope,
		// Token: 0x0400701F RID: 28703
		VibrationSlope
	}

	// Token: 0x02001247 RID: 4679
	public struct TriggerEffectXB
	{
		// Token: 0x04007020 RID: 28704
		public TriggerEffectManager.EffectXbox Effect;

		// Token: 0x04007021 RID: 28705
		public float Strength;

		// Token: 0x04007022 RID: 28706
		[FormerlySerializedAs("endStrength")]
		[FormerlySerializedAs("Amplitude")]
		public float EndStrength;

		// Token: 0x04007023 RID: 28707
		public float StartPosition;

		// Token: 0x04007024 RID: 28708
		public float EndPosition;
	}

	// Token: 0x02001248 RID: 4680
	public struct ControllerTriggerEffect
	{
		// Token: 0x04007025 RID: 28709
		public TriggerEffectManager.TriggerEffectDS DualsenseEffect;

		// Token: 0x04007026 RID: 28710
		public TriggerEffectManager.TriggerEffectXB XboxTriggerEffect;
	}
}
