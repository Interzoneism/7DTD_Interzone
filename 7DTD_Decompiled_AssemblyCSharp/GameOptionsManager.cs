using System;
using System.Runtime.CompilerServices;
using InControl;
using Platform;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000F7E RID: 3966
public static class GameOptionsManager
{
	// Token: 0x140000E7 RID: 231
	// (add) Token: 0x06007E7F RID: 32383 RVA: 0x00333D88 File Offset: 0x00331F88
	// (remove) Token: 0x06007E80 RID: 32384 RVA: 0x00333DBC File Offset: 0x00331FBC
	public static event Action<int, int> ResolutionChanged;

	// Token: 0x140000E8 RID: 232
	// (add) Token: 0x06007E81 RID: 32385 RVA: 0x00333DF0 File Offset: 0x00331FF0
	// (remove) Token: 0x06007E82 RID: 32386 RVA: 0x00333E24 File Offset: 0x00332024
	public static event Action<int> TextureQualityChanged;

	// Token: 0x140000E9 RID: 233
	// (add) Token: 0x06007E83 RID: 32387 RVA: 0x00333E58 File Offset: 0x00332058
	// (remove) Token: 0x06007E84 RID: 32388 RVA: 0x00333E8C File Offset: 0x0033208C
	public static event Action<int> TextureFilterChanged;

	// Token: 0x140000EA RID: 234
	// (add) Token: 0x06007E85 RID: 32389 RVA: 0x00333EC0 File Offset: 0x003320C0
	// (remove) Token: 0x06007E86 RID: 32390 RVA: 0x00333EF4 File Offset: 0x003320F4
	public static event Action<int> ShadowDistanceChanged;

	// Token: 0x140000EB RID: 235
	// (add) Token: 0x06007E87 RID: 32391 RVA: 0x00333F28 File Offset: 0x00332128
	// (remove) Token: 0x06007E88 RID: 32392 RVA: 0x00333F5C File Offset: 0x0033215C
	public static event Action OnGameOptionsApplied;

	// Token: 0x06007E89 RID: 32393 RVA: 0x00333F90 File Offset: 0x00332190
	[PublicizedFrom(EAccessModifier.Private)]
	static GameOptionsManager()
	{
		GamePrefs.OnGamePrefChanged += GameOptionsManager.OnGamePrefChanged;
		GameOptionsManager.ValidateFoV();
		GameOptionsManager.ValidateTreeDistance();
		GameOptionsManager.ValidateHudSize();
	}

	// Token: 0x06007E8A RID: 32394 RVA: 0x0033407C File Offset: 0x0033227C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnGamePrefChanged(EnumGamePrefs _pref)
	{
		if (_pref == EnumGamePrefs.OptionsGfxFOV)
		{
			GameOptionsManager.ValidateFoV();
		}
		if (_pref == EnumGamePrefs.OptionsGfxTreeDistance)
		{
			GameOptionsManager.ValidateTreeDistance();
		}
		if (_pref == EnumGamePrefs.OptionsHudSize)
		{
			GameOptionsManager.ValidateHudSize();
		}
		if (_pref == EnumGamePrefs.OptionsMumblePositionalAudioSupport)
		{
			GameOptionsManager.UpdateMumblePositionalAudioState();
		}
	}

	// Token: 0x06007E8B RID: 32395 RVA: 0x003340B0 File Offset: 0x003322B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateFoV()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
		if (@int < Constants.cMinCameraFieldOfView)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cMinCameraFieldOfView);
			return;
		}
		if (@int > Constants.cMaxCameraFieldOfView)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cMaxCameraFieldOfView);
		}
	}

	// Token: 0x06007E8C RID: 32396 RVA: 0x003340EE File Offset: 0x003322EE
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateTreeDistance()
	{
		if (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTreeDistance) < 2)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTreeDistance, 2);
		}
	}

	// Token: 0x06007E8D RID: 32397 RVA: 0x00334108 File Offset: 0x00332308
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateHudSize()
	{
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
		if ((double)@float < 0.01)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsHudSize, 1f);
			return;
		}
		if ((double)@float < 0.5)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsHudSize, 0.5f);
		}
	}

	// Token: 0x06007E8E RID: 32398 RVA: 0x00334159 File Offset: 0x00332359
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateMumblePositionalAudioState()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsMumblePositionalAudioSupport))
		{
			MumblePositionalAudio.Init();
			return;
		}
		MumblePositionalAudio.Destroy();
	}

	// Token: 0x06007E8F RID: 32399 RVA: 0x00334174 File Offset: 0x00332374
	public static void ApplyAllOptions(LocalPlayerUI _playerUi)
	{
		QualitySettings.antiAliasing = 0;
		float streamingMipmapBudget = GameOptionsPlatforms.GetStreamingMipmapBudget();
		QualitySettings.streamingMipmapsMemoryBudget = streamingMipmapBudget;
		Log.Out("ApplyAllOptions streaming budget {0} MB", new object[]
		{
			streamingMipmapBudget
		});
		QualitySettings.softParticles = (GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter) >= 0.51f);
		GameOptionsManager.ApplyScreenResolution();
		AudioListener.volume = Math.Min(GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), 1f);
		GameOptionsManager.ApplyShadowQuality();
		GameOptionsManager.ApplyTextureQuality(-1);
		GameOptionsManager.ApplyTextureFilter();
		GameOptionsManager.ApplyCameraOptions(null);
		Shader.globalMaximumLOD = 400 + GamePrefs.GetInt(EnumGamePrefs.OptionsGfxObjQuality) * 100;
		QualitySettings.lodBias = GameOptionsManager.GetLODBias();
		GameOptionsManager.ApplyTerrainOptions();
		MeshDescription.SetGrassQuality();
		MeshDescription.SetWaterQuality();
		Action onGameOptionsApplied = GameOptionsManager.OnGameOptionsApplied;
		if (onGameOptionsApplied == null)
		{
			return;
		}
		onGameOptionsApplied();
	}

	// Token: 0x06007E90 RID: 32400 RVA: 0x00334234 File Offset: 0x00332434
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyScreenResolution()
	{
		ValueTuple<int, int, FullScreenMode> screenOptions = PlatformApplicationManager.Application.ScreenOptions;
		int item = screenOptions.Item1;
		int item2 = screenOptions.Item2;
		FullScreenMode item3 = screenOptions.Item3;
		Resolution currentResolution = PlatformApplicationManager.Application.GetCurrentResolution();
		Log.Out("ApplyAllOptions current screen {0} x {1}, {2}hz, window {3} x {4}, mode {5}", new object[]
		{
			currentResolution.width,
			currentResolution.height,
			currentResolution.refreshRateRatio.value,
			Screen.width,
			Screen.height,
			Screen.fullScreenMode
		});
		GameOptionsManager.SetResolution(item, item2, item3);
	}

	// Token: 0x06007E91 RID: 32401 RVA: 0x003342E0 File Offset: 0x003324E0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyShadowQuality()
	{
		Vector3 vector = new Vector3(0.06f, 0.15f, 0.35f);
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance);
		switch (@int)
		{
		case 0:
			QualitySettings.shadowDistance = 35f;
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowCascade2Split = 0.33f;
			goto IL_C8;
		case 1:
			QualitySettings.shadowDistance = 80f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = vector;
			goto IL_C8;
		case 2:
			QualitySettings.shadowDistance = 120f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = vector;
			goto IL_C8;
		case 3:
			QualitySettings.shadowDistance = 200f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = vector * 0.8f;
			goto IL_C8;
		}
		QualitySettings.shadowDistance = 300f;
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowCascade4Split = vector * 0.6f;
		IL_C8:
		if (GameOptionsManager.ShadowDistanceChanged != null)
		{
			GameOptionsManager.ShadowDistanceChanged(@int);
		}
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowQuality))
		{
		case 0:
			QualitySettings.shadows = ShadowQuality.Disable;
			return;
		case 1:
			QualitySettings.shadows = ShadowQuality.HardOnly;
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			return;
		case 2:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			return;
		case 3:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.High;
			return;
		case 4:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.High;
			return;
		case 5:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			return;
		default:
			return;
		}
	}

	// Token: 0x06007E92 RID: 32402 RVA: 0x00334438 File Offset: 0x00332638
	public static float GetLODBias()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxObjQuality);
		switch (@int)
		{
		case 0:
			return 0.5f;
		case 1:
			return 0.65f;
		case 2:
			return 0.8f;
		case 3:
			return 1.2f;
		case 4:
			return 1.7f;
		default:
			return (float)@int / 100f;
		}
	}

	// Token: 0x06007E93 RID: 32403 RVA: 0x00334490 File Offset: 0x00332690
	public static void CheckResolution()
	{
		if (GameOptionsManager.screenExclusiveCheckDelay > 0 && --GameOptionsManager.screenExclusiveCheckDelay == 0)
		{
			GameOptionsManager.screenExclusiveCheckDelay = 10;
			if (Screen.width != GameOptionsManager.screenWidth || Screen.height != GameOptionsManager.screenHeight)
			{
				Log.Warning("Fullscreen Exclusive failed! Reverting to {0} x {1}", new object[]
				{
					GameOptionsManager.screenWidth,
					GameOptionsManager.screenHeight
				});
				GameOptionsManager.SetResolution(GameOptionsManager.screenWidth, GameOptionsManager.screenHeight, FullScreenMode.Windowed);
			}
		}
	}

	// Token: 0x06007E94 RID: 32404 RVA: 0x00334510 File Offset: 0x00332710
	public static void SetResolution(int _width, int _height, FullScreenMode _fullscreen = FullScreenMode.Windowed)
	{
		if (Screen.width == _width && Screen.height == _height && Screen.fullScreenMode == _fullscreen)
		{
			return;
		}
		Resolution currentResolution = PlatformApplicationManager.Application.GetCurrentResolution();
		Log.Out("SetResolution was screen {0} x {1}, {2}hz, window {3} x {4}, mode {5}", new object[]
		{
			currentResolution.width,
			currentResolution.height,
			currentResolution.refreshRateRatio.value,
			Screen.width,
			Screen.height,
			Screen.fullScreenMode
		});
		Log.Out("SetResolution to {0} x {1}, mode {2}", new object[]
		{
			_width,
			_height,
			_fullscreen
		});
		GameOptionsManager.screenWidth = _width;
		GameOptionsManager.screenHeight = _height;
		GameOptionsManager.screenExclusiveCheckDelay = ((_fullscreen == FullScreenMode.ExclusiveFullScreen) ? 10 : 0);
		PlatformApplicationManager.Application.SetResolution(_width, _height, _fullscreen);
		if (GameOptionsManager.ResolutionChanged != null)
		{
			GameOptionsManager.ResolutionChanged(_width, _height);
		}
	}

	// Token: 0x06007E95 RID: 32405 RVA: 0x00334614 File Offset: 0x00332814
	public static int GetTextureQuality(int _overrideValue = -1)
	{
		if (_overrideValue != -1)
		{
			return _overrideValue;
		}
		int num = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTexQuality);
		if (Constants.Is32BitOs && num < 2)
		{
			num = 2;
		}
		return Utils.FastMax(num, GameOptionsManager.CalcTextureQualityMin());
	}

	// Token: 0x06007E96 RID: 32406 RVA: 0x00334649 File Offset: 0x00332849
	public static int CalcTextureQualityMin()
	{
		return GameOptionsPlatforms.CalcTextureQualityMin();
	}

	// Token: 0x06007E97 RID: 32407 RVA: 0x00334650 File Offset: 0x00332850
	public static void ApplyTextureQuality(int _overrideValue = -1)
	{
		int textureQuality = GameOptionsManager.GetTextureQuality(_overrideValue);
		QualitySettings.streamingMipmapsActive = true;
		QualitySettings.streamingMipmapsMaxLevelReduction = Math.Max(3, GameRenderManager.TextureMipmapLimit);
		GameRenderManager.TextureMipmapLimit = textureQuality;
		float value = 0.6776996f;
		if (textureQuality > 0)
		{
			if (textureQuality == 1)
			{
				value = 0.6f;
			}
			else if (textureQuality == 2)
			{
				value = 0.5f;
			}
			else if (textureQuality == 3)
			{
				value = 0.4f;
			}
		}
		Shader.SetGlobalFloat("_MipSlope", value);
		if (GameOptionsManager.TextureQualityChanged != null)
		{
			GameOptionsManager.TextureQualityChanged(textureQuality);
		}
		Log.Out("Texture quality is set to " + GameRenderManager.TextureMipmapLimit.ToString());
	}

	// Token: 0x06007E98 RID: 32408 RVA: 0x003346E5 File Offset: 0x003328E5
	public static int GetTextureFilter()
	{
		return GameOptionsPlatforms.ApplyTextureFilterLimit(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTexFilter));
	}

	// Token: 0x06007E99 RID: 32409 RVA: 0x003346F8 File Offset: 0x003328F8
	public static void ApplyTextureFilter()
	{
		int textureFilter = GameOptionsManager.GetTextureFilter();
		QualitySettings.anisotropicFiltering = ((textureFilter == 0) ? AnisotropicFiltering.Disable : ((textureFilter <= 3) ? AnisotropicFiltering.Enable : AnisotropicFiltering.ForceEnable));
		if (GameOptionsManager.TextureFilterChanged != null)
		{
			GameOptionsManager.TextureFilterChanged(textureFilter);
		}
		Log.Out("ApplyTextureFilter {0}, AF {1}", new object[]
		{
			textureFilter,
			QualitySettings.anisotropicFiltering
		});
	}

	// Token: 0x06007E9A RID: 32410 RVA: 0x00334758 File Offset: 0x00332958
	public static void ApplyCameraOptions(EntityPlayerLocal _playerLocal = null)
	{
		bool enabled = false;
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance);
		float value;
		if (@int - 8 > 4)
		{
			value = 0f;
		}
		else
		{
			value = 9.5f;
		}
		Shader.SetGlobalFloat("_MinDistantMip", value);
		GameRenderManager.ApplyCameraOptions(_playerLocal);
		Camera[] array;
		if (_playerLocal)
		{
			array = _playerLocal.GetComponentsInChildren<Camera>();
		}
		else
		{
			array = Camera.allCameras;
		}
		foreach (Camera camera in array)
		{
			if ((camera.cullingMask & 4096) == 0)
			{
				DepthOfField depthOfField;
				if (camera.TryGetComponent<DepthOfField>(out depthOfField))
				{
					depthOfField.enabled = enabled;
				}
				camera.allowHDR = true;
				int num = GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) ? GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV) : Constants.cDefaultCameraFieldOfView;
				camera.fieldOfView = (float)num;
				camera.renderingPath = RenderingPath.DeferredShading;
				if (QualitySettings.antiAliasing != 0 && camera.actualRenderingPath == RenderingPath.DeferredShading)
				{
					Log.Warning("QualitySettings antialiasing has been enabled but the rendering path is set to deferred. This is incompatible and wastes memory and so will be disabled");
					QualitySettings.antiAliasing = 0;
				}
				camera.farClipPlane = 2000f;
			}
		}
	}

	// Token: 0x06007E9B RID: 32411 RVA: 0x00334850 File Offset: 0x00332A50
	public static void ApplyTerrainOptions()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTerrainQuality);
		if (@int <= 1)
		{
			Shader.EnableKeyword("GAME_TERRAINLOWQ");
		}
		else
		{
			Shader.DisableKeyword("GAME_TERRAINLOWQ");
		}
		if (@int == 0)
		{
			Shader.DisableKeyword("_MAX3LAYER");
			Shader.EnableKeyword("_MAX2LAYER");
		}
		else if (@int <= 1)
		{
			Shader.DisableKeyword("_MAX2LAYER");
			Shader.EnableKeyword("_MAX3LAYER");
		}
		else
		{
			Shader.DisableKeyword("_MAX2LAYER");
			Shader.DisableKeyword("_MAX3LAYER");
		}
		Log.Out("ApplyTerrainOptions {0}", new object[]
		{
			@int
		});
	}

	// Token: 0x06007E9C RID: 32412 RVA: 0x003348E4 File Offset: 0x00332AE4
	public static void SetGraphicsQuality()
	{
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset))
		{
		case 0:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, 0f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cDefaultCameraFieldOfView - 7);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, 0f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSReflections, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, 0f);
			if (GameManager.Instance.World == null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, 5);
				GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, false);
				return;
			}
			break;
		case 1:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, 0f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cDefaultCameraFieldOfView - 4);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, 0.25f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSReflections, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, 0.2f);
			if (GameManager.Instance.World == null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, 5);
				GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, true);
				return;
			}
			break;
		case 2:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, 0f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cDefaultCameraFieldOfView);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, 0.5f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSReflections, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, 0.5f);
			if (GameManager.Instance.World == null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, 6);
				GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, true);
				return;
			}
			break;
		case 3:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 4);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, 0.5f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cDefaultCameraFieldOfView);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, 0.75f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSReflections, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, 0.75f);
			if (GameManager.Instance.World == null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, 6);
				GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, true);
				return;
			}
			break;
		case 4:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 4);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, 0.6f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, 2);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cDefaultCameraFieldOfView);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, 0);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 4);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, 1f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 4);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, 4);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSAO, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSSReflections, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, true);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, 1);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, 1f);
			if (GameManager.Instance.World == null)
			{
				GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, 7);
				GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, true);
				return;
			}
			break;
		case 5:
		case 7:
			break;
		case 6:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDynamicScale, DeviceFlag.XBoxSeriesS.IsCurrent() ? 0.6f : 0.5f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFSRPreset, 0);
			GameOptionsManager.<SetGraphicsQuality>g__ApplyConsoleCommonGfxOptions|40_0();
			GameOptionsManager.<SetGraphicsQuality>g__ApplyConsolePerformanceGfxOptions|40_2();
			return;
		case 8:
			GamePrefs.Set(EnumGamePrefs.OptionsGfxDynamicScale, 0.75f);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxAA, 3);
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFSRPreset, 2);
			GameOptionsManager.<SetGraphicsQuality>g__ApplyConsoleCommonGfxOptions|40_0();
			GameOptionsManager.<SetGraphicsQuality>g__ApplyConsoleQualityGfxOptions|40_1();
			break;
		default:
			return;
		}
	}

	// Token: 0x06007E9D RID: 32413 RVA: 0x00334E70 File Offset: 0x00333070
	public static bool ResetGameOptions(GameOptionsManager.ResetType _resetType)
	{
		if (_resetType == GameOptionsManager.ResetType.Audio)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsAmbientVolumeLevel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsAmbientVolumeLevel));
			GamePrefs.Set(EnumGamePrefs.OptionsMusicVolumeLevel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsMusicVolumeLevel));
			GamePrefs.Set(EnumGamePrefs.OptionsMenuMusicVolumeLevel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsMenuMusicVolumeLevel));
			GamePrefs.Set(EnumGamePrefs.OptionsDynamicMusicEnabled, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsDynamicMusicEnabled));
			GamePrefs.Set(EnumGamePrefs.OptionsDynamicMusicDailyTime, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsDynamicMusicDailyTime));
			GamePrefs.Set(EnumGamePrefs.OptionsMicVolumeLevel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsMicVolumeLevel));
			GamePrefs.Set(EnumGamePrefs.OptionsVoiceVolumeLevel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsVoiceVolumeLevel));
			GamePrefs.Set(EnumGamePrefs.OptionsOverallAudioVolumeLevel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsOverallAudioVolumeLevel));
			GamePrefs.Set(EnumGamePrefs.OptionsVoiceChatEnabled, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsVoiceChatEnabled));
			GamePrefs.Set(EnumGamePrefs.OptionsAudioOcclusion, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsAudioOcclusion));
			GamePrefs.Set(EnumGamePrefs.OptionsSubtitlesEnabled, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsSubtitlesEnabled));
		}
		if (_resetType == GameOptionsManager.ResetType.Graphics)
		{
			GameOptionsManager.ResetGraphicsOptions();
		}
		if (_resetType == GameOptionsManager.ResetType.Controls)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsLookSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsLookSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsZoomSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsZoomSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsZoomAccel, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsZoomAccel));
			GamePrefs.Set(EnumGamePrefs.OptionsInvertMouse, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsInvertMouse));
			GamePrefs.Set(EnumGamePrefs.OptionsVehicleLookSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsVehicleLookSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsControlsSprintLock, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsControlsSprintLock));
			foreach (PlayerActionsBase playerActionsBase in PlatformManager.NativePlatform.Input.ActionSets)
			{
				playerActionsBase.Reset();
			}
			GameOptionsManager.SaveControls();
		}
		if (_resetType == GameOptionsManager.ResetType.Controller)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsAllowController, true);
			GamePrefs.Set(EnumGamePrefs.OptionsControllerVibration, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerVibration));
			GamePrefs.Set(EnumGamePrefs.OptionsInterfaceSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsInterfaceSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerSensitivityX, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerSensitivityX));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerSensitivityY, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerSensitivityY));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerLookInvert, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerLookInvert));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerJoystickLayout, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerJoystickLayout));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerLookAcceleration, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerLookAcceleration));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerZoomSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerZoomSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerLookAxisDeadzone, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerLookAxisDeadzone));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerMoveAxisDeadzone, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerMoveAxisDeadzone));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerCursorSnap, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerCursorSnap));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerCursorHoverSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerCursorHoverSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerVehicleSensitivity, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerVehicleSensitivity));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerAimAssists, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerAimAssists));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerWeaponAiming, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerWeaponAiming));
			GamePrefs.Set(EnumGamePrefs.OptionsControlsSprintLock, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsControlsSprintLock));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerTriggerEffects, (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerTriggerEffects));
			GamePrefs.Set(EnumGamePrefs.OptionsControllerIconStyle, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerIconStyle));
			foreach (PlayerActionsBase playerActionsBase2 in PlatformManager.NativePlatform.Input.ActionSets)
			{
				playerActionsBase2.Reset();
			}
			GameOptionsManager.SaveControls();
		}
		if (_resetType == GameOptionsManager.ResetType.All)
		{
			GamePrefs.PropertyDecl[] propertyList = GamePrefs.GetPropertyList();
			for (int i = 0; i < propertyList.Length; i++)
			{
				switch (propertyList[i].type)
				{
				case GamePrefs.EnumType.Int:
					GamePrefs.Set(propertyList[i].name, (int)GamePrefs.GetDefault(propertyList[i].name));
					break;
				case GamePrefs.EnumType.Float:
					GamePrefs.Set(propertyList[i].name, (float)GamePrefs.GetDefault(propertyList[i].name));
					break;
				case GamePrefs.EnumType.String:
					GamePrefs.Set(propertyList[i].name, (string)GamePrefs.GetDefault(propertyList[i].name));
					break;
				case GamePrefs.EnumType.Bool:
					GamePrefs.Set(propertyList[i].name, (bool)GamePrefs.GetDefault(propertyList[i].name));
					break;
				case GamePrefs.EnumType.Binary:
					GamePrefs.Set(propertyList[i].name, (string)GamePrefs.GetDefault(propertyList[i].name));
					break;
				}
			}
			foreach (PlayerActionsBase playerActionsBase3 in PlatformManager.NativePlatform.Input.ActionSets)
			{
				playerActionsBase3.Reset();
			}
			GameOptionsManager.SaveControls();
		}
		return true;
	}

	// Token: 0x06007E9E RID: 32414 RVA: 0x003353BC File Offset: 0x003335BC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ResetGraphicsOptions()
	{
		GamePrefs.Set(EnumGamePrefs.OptionsGfxResolution, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxResolution));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDynamicMode, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxDynamicMode));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxUpscalerMode));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxFSRPreset, (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxFSRPreset));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDynamicScale, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxDynamicScale));
		GamePrefs.Set(EnumGamePrefs.OptionsGfxBrightness, (float)GamePrefs.GetDefault(EnumGamePrefs.OptionsGfxBrightness));
		EnumGamePrefs vsyncCountPref = PlatformApplicationManager.Application.VSyncCountPref;
		GamePrefs.Set(vsyncCountPref, (int)GamePrefs.GetDefault(vsyncCountPref));
		int value = GameOptionsPlatforms.CalcDefaultGfxPreset();
		GamePrefs.Set(EnumGamePrefs.OptionsGfxQualityPreset, value);
		int defaultUpscalerMode = GameOptionsPlatforms.DefaultUpscalerMode;
		GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, defaultUpscalerMode);
		GameOptionsManager.SetGraphicsQuality();
		GamePrefs.Set(EnumGamePrefs.DynamicMeshEnabled, (bool)GamePrefs.GetDefault(EnumGamePrefs.DynamicMeshEnabled));
		GamePrefs.Set(EnumGamePrefs.DynamicMeshDistance, (int)GamePrefs.GetDefault(EnumGamePrefs.DynamicMeshDistance));
		GamePrefs.Set(EnumGamePrefs.NoGraphicsMode, (bool)GamePrefs.GetDefault(EnumGamePrefs.NoGraphicsMode));
	}

	// Token: 0x06007E9F RID: 32415 RVA: 0x003354E8 File Offset: 0x003336E8
	public static void SaveControls()
	{
		string[] array = new string[PlatformManager.NativePlatform.Input.ActionSets.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = PlatformManager.NativePlatform.Input.ActionSets[i].Save();
		}
		string value = string.Join(";", array);
		SdPlayerPrefs.SetString("Controls", value);
		GameOptionsManager.ApplyAllowControllerOption();
	}

	// Token: 0x06007EA0 RID: 32416 RVA: 0x00335556 File Offset: 0x00333756
	public static void LoadControls()
	{
		GameOptionsManager.LoadControls(SdPlayerPrefs.GetString("Controls", string.Empty));
	}

	// Token: 0x06007EA1 RID: 32417 RVA: 0x0033556C File Offset: 0x0033376C
	public static void LoadControls(string prefString)
	{
		PlatformManager.NativePlatform.Input.LoadActionSetsFromStrings(prefString.Split(';', StringSplitOptions.None));
		GameOptionsManager.ApplyAllowControllerOption();
		GameOptionsManager.RestoreNonBindableControllerActionsToDefaults();
	}

	// Token: 0x06007EA2 RID: 32418 RVA: 0x00335590 File Offset: 0x00333790
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyAllowControllerOption()
	{
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.OptionsAllowController);
		for (int i = 0; i < PlatformManager.NativePlatform.Input.ActionSets.Count; i++)
		{
			PlatformManager.NativePlatform.Input.ActionSets[i].Device = (@bool ? null : InputDevice.Null);
		}
	}

	// Token: 0x06007EA3 RID: 32419 RVA: 0x003355EC File Offset: 0x003337EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RestoreNonBindableControllerActionsToDefaults()
	{
		PlatformManager.NativePlatform.Input.GetActionSetForName("gui").ResetControllerBindings();
		PlatformManager.NativePlatform.Input.GetActionSetForName("permanent").ResetControllerBindings();
	}

	// Token: 0x06007EA4 RID: 32420 RVA: 0x00335620 File Offset: 0x00333820
	public static double GetUiSizeLimit(double _aspectRation)
	{
		int num = 0;
		while (_aspectRation > GameOptionsManager.uiScaleLimits[num].Item1)
		{
			num++;
		}
		return GameOptionsManager.uiScaleLimits[num].Item2;
	}

	// Token: 0x06007EA5 RID: 32421 RVA: 0x00335658 File Offset: 0x00333858
	public static double GetUiSizeLimit()
	{
		Vector2i currentScreenSize = GameOptionsManager.CurrentScreenSize;
		return GameOptionsManager.GetUiSizeLimit((double)currentScreenSize.x / (double)currentScreenSize.y);
	}

	// Token: 0x06007EA6 RID: 32422 RVA: 0x00335680 File Offset: 0x00333880
	public static float GetActiveUiScale()
	{
		float v = (float)GameOptionsManager.GetUiSizeLimit();
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
		return Utils.FastMin(v, @float);
	}

	// Token: 0x17000D3A RID: 3386
	// (get) Token: 0x06007EA7 RID: 32423 RVA: 0x003356A4 File Offset: 0x003338A4
	public static Vector2i CurrentScreenSize
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return new Vector2i(Screen.width, Screen.height);
		}
	}

	// Token: 0x06007EA8 RID: 32424 RVA: 0x003356B8 File Offset: 0x003338B8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <SetGraphicsQuality>g__ApplyConsoleCommonGfxOptions|40_0()
	{
		GamePrefs.Set(EnumGamePrefs.OptionsGfxOcclusion, true);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxBloom, true);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxDOF, false);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxMotionBlur, 2);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxSunShafts, true);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxAASharpness, 0.85f);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTexQuality, DeviceFlag.XBoxSeriesS.IsCurrent() ? 1 : 0);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTexFilter, 3);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectQuality, 3);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxReflectShadows, true);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterPtlLimiter, 0.75f);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxViewDistance, 6);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxLODDistance, 0.75f);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxWaterQuality, 1);
	}

	// Token: 0x06007EA9 RID: 32425 RVA: 0x00335764 File Offset: 0x00333964
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <SetGraphicsQuality>g__ApplyConsoleQualityGfxOptions|40_1()
	{
		GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 3);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, DeviceFlag.XBoxSeriesS.IsCurrent() ? 2 : 3);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 4);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 3);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, DeviceFlag.XBoxSeriesS.IsCurrent() ? 3 : 4);
	}

	// Token: 0x06007EAA RID: 32426 RVA: 0x003357B5 File Offset: 0x003339B5
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <SetGraphicsQuality>g__ApplyConsolePerformanceGfxOptions|40_2()
	{
		GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 1);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowQuality, 2);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxTerrainQuality, 2);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxGrassDistance, 2);
		GamePrefs.Set(EnumGamePrefs.OptionsGfxObjQuality, 2);
	}

	// Token: 0x04006076 RID: 24694
	public const string cPrefFullscreen = "Screenmanager Fullscreen mode";

	// Token: 0x04006077 RID: 24695
	[PublicizedFrom(EAccessModifier.Private)]
	public static int screenWidth;

	// Token: 0x04006078 RID: 24696
	[PublicizedFrom(EAccessModifier.Private)]
	public static int screenHeight;

	// Token: 0x04006079 RID: 24697
	[PublicizedFrom(EAccessModifier.Private)]
	public static int screenExclusiveCheckDelay;

	// Token: 0x0400607A RID: 24698
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cScreenExclusiveFrameWait = 10;

	// Token: 0x0400607C RID: 24700
	[TupleElementNames(new string[]
	{
		"aspectLimit",
		"scaleLimit"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ValueTuple<double, double>[] uiScaleLimits = new ValueTuple<double, double>[]
	{
		new ValueTuple<double, double>(1.2, 0.65),
		new ValueTuple<double, double>(1.26, 0.7),
		new ValueTuple<double, double>(1.34, 0.75),
		new ValueTuple<double, double>(1.51, 0.85),
		new ValueTuple<double, double>(1.61, 0.9),
		new ValueTuple<double, double>(1000.0, 1.0)
	};

	// Token: 0x02000F7F RID: 3967
	public enum ResetType
	{
		// Token: 0x0400607E RID: 24702
		All,
		// Token: 0x0400607F RID: 24703
		Graphics,
		// Token: 0x04006080 RID: 24704
		Audio,
		// Token: 0x04006081 RID: 24705
		Controls,
		// Token: 0x04006082 RID: 24706
		Controller
	}
}
