using System;
using System.Runtime.CompilerServices;
using PI.NGSS;
using UnityEngine;

// Token: 0x020010FD RID: 4349
public class SkyManager : MonoBehaviour
{
	// Token: 0x17000E44 RID: 3652
	// (get) Token: 0x06008864 RID: 34916 RVA: 0x003733DA File Offset: 0x003715DA
	public static Transform SunLightT
	{
		get
		{
			return SkyManager.sunLightT;
		}
	}

	// Token: 0x06008865 RID: 34917 RVA: 0x003733E1 File Offset: 0x003715E1
	public static bool IsBloodMoonVisible()
	{
		return GameUtils.IsBloodMoonTime(new ValueTuple<int, int>(SkyManager.duskTime - 4, SkyManager.dawnTime + 2), (int)SkyManager.TimeOfDay(), SkyManager.bloodmoonDay, (int)SkyManager.dayCount);
	}

	// Token: 0x06008866 RID: 34918 RVA: 0x0037340C File Offset: 0x0037160C
	public static float BloodMoonVisiblePercent()
	{
		int num = (int)SkyManager.dayCount;
		float num2 = SkyManager.TimeOfDay();
		if (num != SkyManager.bloodmoonDay)
		{
			return (float)((num > 1 && num == SkyManager.bloodmoonDay + 1 && num2 <= (float)(SkyManager.dawnTime + 2)) ? 1 : 0);
		}
		float num3 = (float)SkyManager.duskTime - num2;
		if (num3 < 0f)
		{
			return 1f;
		}
		float num4 = 1f - num3 / 4f;
		if (num4 >= 0f)
		{
			return num4;
		}
		return 0f;
	}

	// Token: 0x06008867 RID: 34919 RVA: 0x00373481 File Offset: 0x00371681
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		GameOptionsManager.ShadowDistanceChanged += this.OnShadowDistanceChanged;
		GameStats.OnChangedDelegates += this.OnGameStatsChanged;
	}

	// Token: 0x06008868 RID: 34920 RVA: 0x003734A5 File Offset: 0x003716A5
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		GameOptionsManager.ShadowDistanceChanged -= this.OnShadowDistanceChanged;
		GameStats.OnChangedDelegates -= this.OnGameStatsChanged;
	}

	// Token: 0x06008869 RID: 34921 RVA: 0x003734CC File Offset: 0x003716CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnShadowDistanceChanged(int optionsShadowDistance)
	{
		int shadowCustomResolution;
		switch (optionsShadowDistance)
		{
		case 0:
		case 1:
		case 2:
			shadowCustomResolution = 1024;
			goto IL_36;
		case 3:
			shadowCustomResolution = 2048;
			goto IL_36;
		}
		shadowCustomResolution = 4096;
		IL_36:
		SkyManager.sunLight.shadowCustomResolution = shadowCustomResolution;
		SkyManager.moonLight.shadowCustomResolution = shadowCustomResolution;
	}

	// Token: 0x0600886A RID: 34922 RVA: 0x00373525 File Offset: 0x00371725
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGameStatsChanged(EnumGameStats _gameState, object _newValue)
	{
		if (_gameState != EnumGameStats.DayLightLength)
		{
			if (_gameState == EnumGameStats.BloodMoonDay)
			{
				SkyManager.bloodmoonDay = GameStats.GetInt(EnumGameStats.BloodMoonDay);
				return;
			}
		}
		else
		{
			ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
			SkyManager.duskTime = valueTuple.Item1;
			SkyManager.dawnTime = valueTuple.Item2;
		}
	}

	// Token: 0x0600886B RID: 34923 RVA: 0x0037355F File Offset: 0x0037175F
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		SkyManager.Reset();
	}

	// Token: 0x0600886C RID: 34924 RVA: 0x00373566 File Offset: 0x00371766
	public static void Loaded(GameObject _obj)
	{
		_obj.SetActive(true);
		_obj.transform.parent = GameManager.Instance.transform;
		SkyManager.skyManager = _obj.GetComponent<SkyManager>();
		SkyManager.Reset();
	}

	// Token: 0x0600886D RID: 34925 RVA: 0x00373594 File Offset: 0x00371794
	public static void Cleanup()
	{
		if (SkyManager.skyManager != null)
		{
			UnityEngine.Object.DestroyImmediate(SkyManager.skyManager.gameObject);
		}
	}

	// Token: 0x0600886E RID: 34926 RVA: 0x003735B4 File Offset: 0x003717B4
	public static void SetSkyEnabled(bool _enabled)
	{
		if (_enabled)
		{
			SkyManager.SetFogDebug(-1f, float.MinValue, float.MinValue);
			SkyManager.mainCamera.backgroundColor = Color.black;
		}
		else
		{
			SkyManager.SetFogDebug(0f, float.MinValue, float.MinValue);
			SkyManager.mainCamera.backgroundColor = new Color(0.44f, 0.48f, 0.52f);
		}
		SkyManager.atmosphereSphere.gameObject.SetActive(_enabled);
		SkyManager.cloudsSphere.gameObject.SetActive(_enabled);
	}

	// Token: 0x0600886F RID: 34927 RVA: 0x0037363B File Offset: 0x0037183B
	public static Color GetSkyColor()
	{
		return SkyManager.SkyColor;
	}

	// Token: 0x06008870 RID: 34928 RVA: 0x00373642 File Offset: 0x00371842
	public static void SetSkyColor(Color c)
	{
		SkyManager.SkyColor = c;
	}

	// Token: 0x06008871 RID: 34929 RVA: 0x0037364A File Offset: 0x0037184A
	public static void SetGameTime(ulong _time)
	{
		SkyManager.dayCount = _time / 24000f + 1f;
		SkyManager.timeOfDay = _time;
	}

	// Token: 0x06008872 RID: 34930 RVA: 0x00373666 File Offset: 0x00371866
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float TimeOfDay()
	{
		return SkyManager.timeOfDay % 24000UL / 1000f;
	}

	// Token: 0x06008873 RID: 34931 RVA: 0x0037367C File Offset: 0x0037187C
	public static float GetTimeOfDayAsMinutes()
	{
		return SkyManager.TimeOfDay() / 24f * (float)GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
	}

	// Token: 0x06008874 RID: 34932 RVA: 0x00373692 File Offset: 0x00371892
	public static void SetCloudTextures(Texture _mainTex, Texture _blendTex)
	{
		SkyManager.cloudMainTex = _mainTex;
		SkyManager.cloudBlendTex = _blendTex;
	}

	// Token: 0x06008875 RID: 34933 RVA: 0x003736A0 File Offset: 0x003718A0
	public static void SetCloudTransition(float t)
	{
		SkyManager.cloudTransition = t;
	}

	// Token: 0x06008876 RID: 34934 RVA: 0x003736A8 File Offset: 0x003718A8
	public static Color GetFogColor()
	{
		return SkyManager.fogColor;
	}

	// Token: 0x06008877 RID: 34935 RVA: 0x003736AF File Offset: 0x003718AF
	public static void SetFogColor(Color c)
	{
		if (SkyManager.fogDebugColor.a > 0f)
		{
			c = SkyManager.fogDebugColor;
		}
		SkyManager.fogColor = c;
	}

	// Token: 0x06008878 RID: 34936 RVA: 0x003736CF File Offset: 0x003718CF
	public static float GetFogDensity()
	{
		return SkyManager.fogDensity;
	}

	// Token: 0x06008879 RID: 34937 RVA: 0x003736D8 File Offset: 0x003718D8
	public static void SetFogDensity(float density)
	{
		if (SkyManager.fogDebugDensity >= 0f)
		{
			density = SkyManager.fogDebugDensity;
		}
		SkyManager.fogDensity = density;
		float num = density - 0.65f;
		if (num < 0f)
		{
			num = 0f;
		}
		SkyManager.fogLightScale = 1f - num * 1.7f;
	}

	// Token: 0x0600887A RID: 34938 RVA: 0x00373726 File Offset: 0x00371926
	public static float GetFogStart()
	{
		return SkyManager.fogStart;
	}

	// Token: 0x0600887B RID: 34939 RVA: 0x0037372D File Offset: 0x0037192D
	public static float GetFogEnd()
	{
		return SkyManager.fogEnd;
	}

	// Token: 0x0600887C RID: 34940 RVA: 0x00373734 File Offset: 0x00371934
	public static void SetFogFade(float start, float end)
	{
		float t = 1f;
		World world = GameManager.Instance.World;
		if (world != null)
		{
			t = (world.GetPrimaryPlayer().bPlayingSpawnIn ? 1f : 0.01f);
		}
		SkyManager.fogStart = Mathf.Lerp(SkyManager.fogStart, start, t);
		SkyManager.fogEnd = Mathf.Lerp(SkyManager.fogEnd, end, t);
		if (SkyManager.fogDebugDensity >= 0f)
		{
			if (SkyManager.fogDebugStart > -1000f)
			{
				SkyManager.fogStart = SkyManager.fogDebugStart;
			}
			if (SkyManager.fogDebugEnd > -1000f)
			{
				SkyManager.fogEnd = SkyManager.fogDebugEnd;
			}
		}
	}

	// Token: 0x0600887D RID: 34941 RVA: 0x003737C9 File Offset: 0x003719C9
	public static void SetFogDebug(float density = -1f, float start = -3.4028235E+38f, float end = -3.4028235E+38f)
	{
		SkyManager.fogDebugDensity = density;
		SkyManager.fogDebugStart = start;
		SkyManager.fogDebugEnd = end;
		SkyManager.SetFogDensity(0f);
	}

	// Token: 0x0600887E RID: 34942 RVA: 0x003737E7 File Offset: 0x003719E7
	public static void SetFogDebugColor(Color color = default(Color))
	{
		SkyManager.fogDebugColor = color;
		SkyManager.SetFogColor(Color.gray);
	}

	// Token: 0x0600887F RID: 34943 RVA: 0x003737F9 File Offset: 0x003719F9
	public static void Reset()
	{
		SkyManager.bNeedsReset = true;
	}

	// Token: 0x06008880 RID: 34944 RVA: 0x00373804 File Offset: 0x00371A04
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearStatics()
	{
		SkyManager.random = null;
		SkyManager.cloudTransition = 1f;
		SkyManager.cloudMainTexOld = null;
		SkyManager.cloudBlendTex = null;
		SkyManager.parent = null;
		SkyManager.sunLightT = null;
		SkyManager.sunLight = null;
		SkyManager.moonLightT = null;
		SkyManager.moonLight = null;
		SkyManager.moonSpriteT = null;
		SkyManager.mainCamera = null;
		SkyManager.moonSpriteMat = null;
		SkyManager.cloudsSphere = null;
		SkyManager.cloudsSphereMtrl = null;
		SkyManager.atmosphereSphere = null;
		SkyManager.atmosphereMtrl = null;
	}

	// Token: 0x06008881 RID: 34945 RVA: 0x00373878 File Offset: 0x00371A78
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		SkyManager.random = GameRandomManager.Instance.CreateGameRandom();
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Exponential;
		this.sunAxis = Vector3.forward;
		this.sunStartV = Quaternion.AngleAxis(20f, Vector3.up) * Vector3.left;
		this.moonStartV = Quaternion.AngleAxis(35f, Vector3.up) * Vector3.right;
		this.starAxis = Vector3.forward;
		if (SkyManager.parent == null)
		{
			SkyManager.parent = GameObject.Find("SkySystem(Clone)");
			if (SkyManager.parent == null)
			{
				return;
			}
		}
		if (SkyManager.sunLightT == null)
		{
			Transform x = SkyManager.parent.transform.Find("SunLight");
			if (x != null)
			{
				SkyManager.sunLightT = x;
			}
		}
		if (SkyManager.sunLight == null && SkyManager.sunLightT != null)
		{
			SkyManager.sunLight = SkyManager.sunLightT.transform.GetComponent<Light>();
		}
		if (SkyManager.moonLightT == null)
		{
			SkyManager.moonLightT = SkyManager.parent.transform.Find("MoonLight");
			if (SkyManager.moonLightT)
			{
				SkyManager.moonLight = SkyManager.moonLightT.GetComponent<Light>();
			}
		}
		SkyManager.GetMaterialAndTransform("MoonSprite", ref SkyManager.moonSpriteT, ref SkyManager.moonSpriteMat);
		SkyManager.GetMaterialAndTransform("CloudsSphere", ref SkyManager.cloudsSphere, ref SkyManager.cloudsSphereMtrl);
		SkyManager.GetMaterialAndTransform("AtmosphereSphere", ref SkyManager.atmosphereSphere, ref SkyManager.atmosphereMtrl);
		MeshFilter component = SkyManager.moonSpriteT.GetComponent<MeshFilter>();
		if (component != null)
		{
			Mesh mesh = component.mesh;
			if (mesh != null)
			{
				mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000000f);
			}
		}
		if (SkyManager.cloudsSphereMtrl)
		{
			SkyManager.cloudsSphereMtrl.SetFloat("_CloudSpeed", this.cloudSpeed);
		}
		if (SkyManager.mainCamera == null)
		{
			this.GetMainCamera();
		}
		this.OnShadowDistanceChanged(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance));
		SkyManager.bloodmoonDay = GameStats.GetInt(EnumGameStats.BloodMoonDay);
		ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
		SkyManager.duskTime = valueTuple.Item1;
		SkyManager.dawnTime = valueTuple.Item2;
	}

	// Token: 0x06008882 RID: 34946 RVA: 0x00373AAC File Offset: 0x00371CAC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetMaterialAndTransform(string objectName, ref Transform trans, ref Material mtrl)
	{
		Transform transform = SkyManager.parent.transform.Find(objectName);
		if (trans == null)
		{
			trans = transform;
		}
		if (mtrl == null)
		{
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			mtrl = component.material;
		}
	}

	// Token: 0x06008883 RID: 34947 RVA: 0x00373AF0 File Offset: 0x00371CF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetMainCamera()
	{
		SkyManager.mainCamera = Camera.main;
	}

	// Token: 0x06008884 RID: 34948 RVA: 0x00373AFC File Offset: 0x00371CFC
	public static bool IsDark()
	{
		float num = SkyManager.TimeOfDay();
		return num < (float)SkyManager.dawnTime || num > (float)SkyManager.duskTime;
	}

	// Token: 0x06008885 RID: 34949 RVA: 0x00373B23 File Offset: 0x00371D23
	public static float GetDawnTime()
	{
		return (float)SkyManager.dawnTime;
	}

	// Token: 0x06008886 RID: 34950 RVA: 0x00373B2B File Offset: 0x00371D2B
	public static float GetDawnTimeAsMinutes()
	{
		return (float)SkyManager.dawnTime / 24f * (float)GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
	}

	// Token: 0x06008887 RID: 34951 RVA: 0x00373B42 File Offset: 0x00371D42
	public static float GetDuskTime()
	{
		return (float)SkyManager.duskTime;
	}

	// Token: 0x06008888 RID: 34952 RVA: 0x00373B4A File Offset: 0x00371D4A
	public static float GetDuskTimeAsMinutes()
	{
		return (float)SkyManager.duskTime / 24f * (float)GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
	}

	// Token: 0x06008889 RID: 34953 RVA: 0x00373B64 File Offset: 0x00371D64
	[PublicizedFrom(EAccessModifier.Private)]
	public static float CalcDayPercent()
	{
		float num;
		if (SkyManager.worldRotation < 0.5f)
		{
			num = Mathf.Pow(1f - Utils.FastAbs(0.25f - SkyManager.worldRotation) * 4f, 0.6f);
			num = num * 0.68f + 0.5f;
			if (num > 1f)
			{
				num = 1f;
			}
		}
		else
		{
			num = Mathf.Pow(1f - Utils.FastAbs(0.75f - SkyManager.worldRotation) * 4f, 0.6f);
			num = 0.5f - num * 0.68f;
			if (num < 0f)
			{
				num = 0f;
			}
		}
		return num;
	}

	// Token: 0x0600888A RID: 34954 RVA: 0x00373C08 File Offset: 0x00371E08
	public static void TriggerLightning(Vector3 _position)
	{
		if (!SkyManager.triggerLightning)
		{
			SkyManager.lightningFlashes = SkyManager.random.RandomRange(2, 6);
			float num = Time.time;
			for (int i = 0; i < SkyManager.lightningFlashes; i++)
			{
				float num2 = SkyManager.random.RandomRange(0.1f, 0.21f);
				num += num2;
				SkyManager.lightningEndTimes[i] = num;
			}
			SkyManager.lightningIndex = -1;
			SkyManager.lightningDir.x = SkyManager.random.RandomFloat * 360f;
			SkyManager.lightningDir.y = (float)SkyManager.random.RandomRange(8, 20);
			SkyManager.triggerLightning = true;
		}
	}

	// Token: 0x0600888B RID: 34955 RVA: 0x00373CA8 File Offset: 0x00371EA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSunMoonAngles()
	{
		if (!SkyManager.sunLight)
		{
			return;
		}
		if (!SkyManager.moonLight)
		{
			return;
		}
		if (Time.time - SkyManager.worldRotationTime >= 0.2f || SkyManager.bUpdateSunMoonNow)
		{
			SkyManager.worldRotationTime = Time.time;
			SkyManager.bUpdateSunMoonNow = false;
			float num = SkyManager.TimeOfDay();
			if (num >= (float)SkyManager.dawnTime && num < (float)SkyManager.duskTime)
			{
				SkyManager.worldRotationTarget = (num - (float)SkyManager.dawnTime) / (float)(SkyManager.duskTime - SkyManager.dawnTime);
			}
			else
			{
				float num2 = (float)(24 - SkyManager.duskTime);
				float num3 = num2 + (float)SkyManager.dawnTime;
				if (num < (float)SkyManager.dawnTime)
				{
					SkyManager.worldRotationTarget = (num2 + num) / num3;
				}
				else
				{
					SkyManager.worldRotationTarget = (num - (float)SkyManager.duskTime) / num3;
				}
				SkyManager.worldRotationTarget += 1f;
			}
			SkyManager.worldRotationTarget *= 0.5f;
			SkyManager.worldRotationTarget = Mathf.Clamp01(SkyManager.worldRotationTarget);
		}
		float num4 = SkyManager.worldRotationTarget - SkyManager.worldRotation;
		float num5 = SkyManager.worldRotationTarget;
		if (num4 < -0.5f)
		{
			num5 += 1f;
		}
		else if (num4 > 0.5f)
		{
			num5 -= 1f;
		}
		SkyManager.worldRotation = Mathf.Lerp(SkyManager.worldRotation, num5, 0.05f);
		if (SkyManager.worldRotation < 0f)
		{
			SkyManager.worldRotation += 1f;
		}
		else if (SkyManager.worldRotation >= 1f)
		{
			SkyManager.worldRotation -= 1f;
		}
		SkyManager.dayPercent = SkyManager.CalcDayPercent();
		float angle = SkyManager.worldRotation * 360f;
		SkyManager.sunDirV = Quaternion.AngleAxis(angle, this.sunAxis) * this.sunStartV;
		SkyManager.moonLightRot = Quaternion.LookRotation(Quaternion.AngleAxis(angle, this.sunAxis) * this.moonStartV);
		float num6 = SkyManager.worldRotation * 360f;
		if (SkyManager.sunIntensity >= 0.001f)
		{
			if (num6 < 14f)
			{
				num6 = 14f;
			}
			if (num6 > 166f)
			{
				num6 = 166f;
			}
			Vector3 eulerAngles = Quaternion.LookRotation(Quaternion.AngleAxis(num6, this.sunAxis) * this.sunStartV).eulerAngles;
			SkyManager.sunLightT.localEulerAngles = eulerAngles;
			SkyManager.sunLight.shadowStrength = 1f;
			SkyManager.sunLight.shadows = ((SkyManager.sunIntensity > 0f) ? LightShadows.Soft : LightShadows.None);
			SkyManager.moonLight.enabled = false;
		}
		else if (SkyManager.moonLightColor.grayscale > 0f)
		{
			if (num6 < 166f)
			{
				num6 = 166f;
			}
			if (num6 > 346f)
			{
				num6 = 346f;
			}
			Vector3 eulerAngles2 = Quaternion.LookRotation(Quaternion.AngleAxis(num6, this.sunAxis) * this.moonStartV).eulerAngles;
			SkyManager.moonLightT.localEulerAngles = eulerAngles2;
			float num7 = SkyManager.fogLightScale * SkyManager.moonBright;
			float t = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxBrightness) * 2f;
			num7 *= Utils.FastLerp(0.2f, 1f, t);
			SkyManager.moonLight.intensity = num7;
			SkyManager.moonLight.color = SkyManager.moonLightColor;
			SkyManager.moonLight.shadowStrength = 1f;
			SkyManager.moonLight.shadows = ((num7 > 0f) ? LightShadows.Soft : LightShadows.None);
			SkyManager.moonLight.enabled = true;
		}
		else
		{
			SkyManager.moonLight.enabled = false;
		}
		SkyManager.sunMoonDirV = SkyManager.sunDirV;
		if (SkyManager.sunIntensity < 0.001f)
		{
			SkyManager.sunMoonDirV = SkyManager.moonLightRot * Vector3.forward;
		}
		if (!GameManager.IsDedicatedServer && SkyManager.mainCamera)
		{
			Vector3 position = SkyManager.mainCamera.transform.position;
			if (SkyManager.moonSpriteT)
			{
				SkyManager.moonSpriteT.position = SkyManager.moonLightRot * Vector3.forward * -45000f;
				SkyManager.moonSpriteT.rotation = Quaternion.LookRotation(SkyManager.moonSpriteT.position, Vector3.up);
				SkyManager.moonSpriteT.position += position;
				float num8 = 6857.143f;
				if (SkyManager.IsBloodMoonVisible())
				{
					num8 *= 1.3f;
				}
				SkyManager.moonSpriteT.localScale = new Vector3(num8, num8, num8);
			}
			this.UpdateSunShaftSettings();
		}
		SkyManager.atmosphereSphere.Rotate(this.starAxis, SkyManager.worldRotation * 0.004f);
		if (this.bUpdateShaders && SkyManager.cloudsSphereMtrl)
		{
			SkyManager.cloudsSphereMtrl.SetVector("_SunDir", SkyManager.sunDirV);
			SkyManager.cloudsSphereMtrl.SetVector("_SunMoonDir", SkyManager.sunMoonDirV);
		}
	}

	// Token: 0x0600888C RID: 34956 RVA: 0x00374154 File Offset: 0x00372354
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSunShaftSettings()
	{
		Vector3 vector = SkyManager.GetSunDirection();
		if (vector.y <= 0.09f)
		{
			SkyManager.sunShaftSettings.sunShaftIntensity = 0.1f + SkyManager.GetFogDensity() * 0.5f;
			SkyManager.sunShaftSettings.sunColor = SkyManager.GetSunLightColor();
			SkyManager.sunShaftSettings.sunThreshold = new Color(0.87f, 0.74f, 0.65f, 1f);
		}
		else
		{
			vector = SkyManager.GetMoonDirection();
			SkyManager.sunShaftSettings.sunShaftIntensity = 0.06f + SkyManager.GetFogDensity() * 0.85f;
			SkyManager.sunShaftSettings.sunColor = SkyManager.GetMoonLightColor();
			SkyManager.sunShaftSettings.sunThreshold = new Color(0.8f, 0.6f, 0.6f, 1f);
		}
		SkyManager.sunShaftSettings.sunPosition = vector * -100000f;
	}

	// Token: 0x0600888D RID: 34957 RVA: 0x0037422B File Offset: 0x0037242B
	public static SunShaftsEffect.SunSettings GetSunShaftSettings()
	{
		return SkyManager.sunShaftSettings;
	}

	// Token: 0x0600888E RID: 34958 RVA: 0x00374232 File Offset: 0x00372432
	public static float GetLuma(Color color)
	{
		return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
	}

	// Token: 0x0600888F RID: 34959 RVA: 0x0037425A File Offset: 0x0037245A
	[PublicizedFrom(EAccessModifier.Private)]
	public bool VerifyCamera()
	{
		if (GameManager.IsDedicatedServer)
		{
			return true;
		}
		if (SkyManager.mainCamera == null)
		{
			this.GetMainCamera();
			if (SkyManager.mainCamera == null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06008890 RID: 34960 RVA: 0x00374288 File Offset: 0x00372488
	[PublicizedFrom(EAccessModifier.Private)]
	public bool VerifyValidMaterials()
	{
		return !(SkyManager.atmosphereMtrl == null) && !(SkyManager.cloudsSphere == null) && !(SkyManager.atmosphereSphere == null) && !(SkyManager.moonLight == null) && !(SkyManager.sunLight == null) && !(SkyManager.moonSpriteT == null) && !(SkyManager.moonSpriteMat == null);
	}

	// Token: 0x06008891 RID: 34961 RVA: 0x003742FF File Offset: 0x003724FF
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetIfNeeded()
	{
		if (SkyManager.bNeedsReset)
		{
			this.ClearStatics();
			this.Init();
			SkyManager.bNeedsReset = false;
		}
	}

	// Token: 0x06008892 RID: 34962 RVA: 0x0037431C File Offset: 0x0037251C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFogShader()
	{
		this.fogParams.x = SkyManager.GetFogStart();
		this.fogParams.y = SkyManager.GetFogEnd();
		this.fogParams.z = 1f;
		this.fogParams.w = Mathf.Pow(SkyManager.GetFogDensity(), 2f);
	}

	// Token: 0x06008893 RID: 34963 RVA: 0x00374374 File Offset: 0x00372574
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateShaderGlobals()
	{
		Color c = SkyManager.GetFogColor();
		float num = SkyManager.GetFogDensity();
		num *= num * num;
		RenderSettings.fogDensity = num;
		RenderSettings.fogColor = c;
		Shader.SetGlobalVector("_FogParams", new Vector4(num, this.fogParams.x, this.fogParams.y, 0f));
		Shader.SetGlobalColor("_FogColor", c.linear);
		Shader.SetGlobalVector("SunColor", SkyManager.sunLight.color);
		Shader.SetGlobalVector("FogColor", c);
		Shader.SetGlobalFloat("_HighResViewDistance", (float)GameUtils.GetViewDistance() * 16f);
		Shader.SetGlobalFloat("_DayPercent", SkyManager.dayPercent);
	}

	// Token: 0x06008894 RID: 34964 RVA: 0x0037442C File Offset: 0x0037262C
	public void Update()
	{
		if (SkyManager.parent == null)
		{
			this.Init();
			if (SkyManager.parent == null)
			{
				return;
			}
		}
		SkyManager.sMaxSunIntensity = this.maxSunIntensity;
		if (SkyManager.mainCamera)
		{
			float num = SkyManager.fogDensity + 0.001f;
			num *= SkyManager.fogDensity * SkyManager.fogDensity;
			SkyManager.mainCamera.farClipPlane = Utils.FastClamp(6f / num + 400f, 200f, 2800f);
		}
		int num2 = Time.frameCount & 1;
		this.bUpdateShaders = (num2 == 0 || SkyManager.triggerLightning);
		if (!this.VerifyCamera())
		{
			return;
		}
		this.ResetIfNeeded();
		this.UpdateSunMoonAngles();
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		float magnitude = (SkyManager.parent.transform.position - SkyManager.mainCamera.transform.position).magnitude;
		if (magnitude < 0.001f)
		{
			this.bMovingSky = false;
			this.targetSkyPosition = SkyManager.mainCamera.transform.position;
		}
		else if (!this.bMovingSky && magnitude > 5f)
		{
			this.bMovingSky = true;
			this.targetSkyPosition = SkyManager.mainCamera.transform.position;
		}
		if (this.bMovingSky)
		{
			SkyManager.parent.transform.position = Vector3.Lerp(SkyManager.parent.transform.position, this.targetSkyPosition, Mathf.Clamp01(Time.deltaTime * 10f));
		}
		this.UpdateFogShader();
		if (!this.bUpdateShaders)
		{
			return;
		}
		if (!this.VerifyValidMaterials())
		{
			return;
		}
		SkyManager.sSunFadeHeight = this.sunFadeHeight;
		this.UpdateShaderGlobals();
		bool flag = SkyManager.IsBloodMoonVisible();
		int num3 = 0;
		SkyManager.moonSpriteColor = new Color(1f, 0.14f, 0.05f) * 1.5f;
		if (!flag)
		{
			num3 = (int)(SkyManager.dayCount + 5.5f) % 7;
			SkyManager.moonSpriteColor = Color.Lerp(Color.white, SkyManager.moonLightColor, 0.2f);
		}
		float num4 = SkyManager.moonPhases[num3];
		float f = num4 * 3.1415927f;
		Vector3 v = new Vector3(-Mathf.Sin(f), 0f, Mathf.Cos(f));
		SkyManager.moonSpriteMat.SetVector("_LightDir", v);
		SkyManager.moonSpriteMat.SetColor("_Color", SkyManager.moonSpriteColor);
		SkyManager.moonBright = 1f - num4;
		if (SkyManager.moonBright < 0f)
		{
			SkyManager.moonBright = -SkyManager.moonBright;
		}
		float b = ((Mathf.Pow(SkyManager.moonLightColor.grayscale, 0.45f) - 0.5f) * 0.5f + 0.5f) * SkyManager.moonBright;
		float value = Mathf.Max(SkyManager.sunIntensity, b);
		if (WeatherManager.currentWeather != null)
		{
			SkyManager.starIntensity = 1f - WeatherManager.currentWeather.CloudThickness() * 0.01f;
		}
		if (this.bUpdateShaders)
		{
			SkyManager.atmosphereMtrl.SetColor("_SkyColor", SkyManager.GetSkyColor());
			SkyManager.atmosphereMtrl.SetFloat("_Stars", SkyManager.starIntensity);
			SkyManager.atmosphereMtrl.SetVector("_SunDir", SkyManager.sunDirV);
			SkyManager.cloudsSphereMtrl.SetFloat("_CloudTransition", SkyManager.cloudTransition);
			SkyManager.cloudsSphereMtrl.SetFloat("_LightIntensity", value);
			SkyManager.cloudsSphereMtrl.SetColor("_SkyColor", SkyManager.GetSkyColor());
			SkyManager.cloudsSphereMtrl.SetColor("_SunColor", SkyManager.sunLight.color);
			SkyManager.cloudsSphereMtrl.SetVector("_SunDir", SkyManager.sunDirV);
			SkyManager.cloudsSphereMtrl.SetVector("_SunMoonDir", SkyManager.sunMoonDirV);
			SkyManager.cloudsSphereMtrl.SetColor("_MoonColor", SkyManager.moonSpriteColor);
		}
		if (SkyManager.cloudMainTex != SkyManager.cloudMainTexOld)
		{
			SkyManager.cloudsSphereMtrl.SetTexture("_CloudMainTex", SkyManager.cloudMainTex);
			SkyManager.cloudMainTexOld = SkyManager.cloudMainTex;
		}
		if (SkyManager.cloudBlendTex != SkyManager.cloudBlendTexOld)
		{
			SkyManager.cloudsSphereMtrl.SetTexture("_CloudBlendTex", SkyManager.cloudBlendTex);
			SkyManager.cloudBlendTexOld = SkyManager.cloudBlendTex;
		}
		if (SkyManager.triggerLightning)
		{
			float deltaTime = Time.deltaTime;
			Light light = SkyManager.moonLight;
			SkyManager.lightningIntensity -= 4f * deltaTime;
			if (SkyManager.lightningIndex < 0 || Time.time >= SkyManager.lightningEndTimes[SkyManager.lightningIndex])
			{
				SkyManager.lightningIndex++;
				SkyManager.lightningIntensity = 0.75f + SkyManager.random.RandomFloat * 0.25f;
				SkyManager.lightningDir.x = SkyManager.lightningDir.x + SkyManager.random.RandomRange(-20f, 20f);
				SkyManager.lightningDirVel.x = SkyManager.random.RandomRange(-50f, 50f);
				SkyManager.lightningDir.y = SkyManager.lightningDir.y + (float)SkyManager.random.RandomRange(-8, 8);
			}
			if (SkyManager.lightningIndex >= SkyManager.lightningFlashes)
			{
				SkyManager.triggerLightning = false;
				SkyManager.cloudsSphereMtrl.SetFloat("_Lightning", 0f);
			}
			else
			{
				SkyManager.lightningIntensity = Utils.FastMax(0f, SkyManager.lightningIntensity);
				SkyManager.cloudsSphereMtrl.SetFloat("_Lightning", SkyManager.lightningIntensity);
				SkyManager.lightningDir += SkyManager.lightningDirVel * deltaTime;
				float f2 = SkyManager.lightningDir.x * 0.017453292f;
				Vector3 a;
				a.x = Mathf.Sin(f2);
				a.y = Mathf.Sin(SkyManager.lightningDir.y * 0.017453292f) * 1.5f;
				a.z = Mathf.Cos(f2);
				SkyManager.cloudsSphereMtrl.SetVector("_LightningDir", a.normalized);
				Transform transform = GameManager.Instance.World.GetPrimaryPlayer().transform;
				Transform transform2 = light.transform;
				Vector3 b2 = a * 200f;
				b2.y = 300f;
				transform2.position = transform.position + b2;
				transform2.LookAt(transform);
				light.color = Color.white;
				light.intensity = SkyManager.lightningIntensity * 0.9f;
				light.shadows = LightShadows.Hard;
				light.shadowStrength = 1f;
				light.enabled = true;
			}
		}
		if (SkyManager.frustumShadows == null)
		{
			SkyManager.frustumShadows = SkyManager.mainCamera.GetComponent<NGSS_FrustumShadows_7DTD>();
		}
		if (SkyManager.frustumShadows != null)
		{
			SkyManager.frustumShadows.mainShadowsLight = ((!SkyManager.IsDark()) ? SkyManager.sunLight : SkyManager.moonLight);
		}
	}

	// Token: 0x06008895 RID: 34965 RVA: 0x00374AB7 File Offset: 0x00372CB7
	public static float GetSunAngle()
	{
		return SkyManager.sunDirV.y;
	}

	// Token: 0x06008896 RID: 34966 RVA: 0x00374AC3 File Offset: 0x00372CC3
	public static Vector3 GetSunDirection()
	{
		return SkyManager.sunDirV;
	}

	// Token: 0x06008897 RID: 34967 RVA: 0x00374ACA File Offset: 0x00372CCA
	public static float GetSunPercent()
	{
		return -SkyManager.sunDirV.y;
	}

	// Token: 0x06008898 RID: 34968 RVA: 0x00374AD7 File Offset: 0x00372CD7
	public static Vector3 GetSunLightDirection()
	{
		if (!(SkyManager.sunLight == null))
		{
			return SkyManager.sunLightT.forward;
		}
		return Vector3.down;
	}

	// Token: 0x06008899 RID: 34969 RVA: 0x00374AF6 File Offset: 0x00372CF6
	public static float GetSunIntensity()
	{
		return SkyManager.sunIntensity;
	}

	// Token: 0x0600889A RID: 34970 RVA: 0x00374AFD File Offset: 0x00372CFD
	public static Color GetSunLightColor()
	{
		if (SkyManager.sunLight)
		{
			return SkyManager.sunLight.color;
		}
		return Color.black;
	}

	// Token: 0x0600889B RID: 34971 RVA: 0x00374B1B File Offset: 0x00372D1B
	public static void SetSunColor(Color color)
	{
		if (SkyManager.sunLight != null)
		{
			SkyManager.sunLight.color = color;
		}
	}

	// Token: 0x0600889C RID: 34972 RVA: 0x00374B38 File Offset: 0x00372D38
	public static void SetSunIntensity(float i)
	{
		SkyManager.sunIntensity = i;
		float sunAngle = SkyManager.GetSunAngle();
		if (sunAngle >= -SkyManager.sSunFadeHeight)
		{
			SkyManager.sunIntensity = -sunAngle * 10f * SkyManager.sunIntensity * (float)((sunAngle < 0f) ? 1 : 0);
		}
		SkyManager.sunIntensity = Mathf.Clamp(SkyManager.sunIntensity, 0f, SkyManager.sMaxSunIntensity);
		SkyManager.sunIntensity *= 1.5f;
		if (SkyManager.sunLight != null)
		{
			SkyManager.sunLight.intensity = SkyManager.sunIntensity * SkyManager.fogLightScale;
		}
	}

	// Token: 0x0600889D RID: 34973 RVA: 0x00374BC6 File Offset: 0x00372DC6
	public static float GetMoonAmbientScale(float add, float mpy)
	{
		return Utils.FastLerp(add + SkyManager.moonBright * mpy, 1f, SkyManager.dayPercent * 3.030303f);
	}

	// Token: 0x0600889E RID: 34974 RVA: 0x00374BE6 File Offset: 0x00372DE6
	public static float GetMoonBrightness()
	{
		return SkyManager.moonLightColor.grayscale * SkyManager.moonBright;
	}

	// Token: 0x0600889F RID: 34975 RVA: 0x00374BF8 File Offset: 0x00372DF8
	public static Color GetMoonLightColor()
	{
		return SkyManager.moonLightColor;
	}

	// Token: 0x060088A0 RID: 34976 RVA: 0x00374BFF File Offset: 0x00372DFF
	public static Vector3 GetMoonDirection()
	{
		return SkyManager.moonLightRot * Vector3.forward;
	}

	// Token: 0x060088A1 RID: 34977 RVA: 0x00374C10 File Offset: 0x00372E10
	public static void SetMoonLightColor(Color color)
	{
		SkyManager.moonLightColor = color;
	}

	// Token: 0x060088A2 RID: 34978 RVA: 0x00374C18 File Offset: 0x00372E18
	public static float GetWorldRotation()
	{
		return SkyManager.worldRotation;
	}

	// Token: 0x04006A50 RID: 27216
	public static SkyManager skyManager;

	// Token: 0x04006A51 RID: 27217
	public static float dayCount;

	// Token: 0x04006A52 RID: 27218
	public static bool indoorFogOn = true;

	// Token: 0x04006A53 RID: 27219
	public static Material atmosphereMtrl;

	// Token: 0x04006A54 RID: 27220
	public static Transform atmosphereSphere;

	// Token: 0x04006A55 RID: 27221
	public static Material cloudsSphereMtrl;

	// Token: 0x04006A56 RID: 27222
	public static Transform cloudsSphere;

	// Token: 0x04006A57 RID: 27223
	public static bool bUpdateSunMoonNow = false;

	// Token: 0x04006A58 RID: 27224
	public static float sSunFadeHeight = 0.1f;

	// Token: 0x04006A59 RID: 27225
	public static float dayPercent;

	// Token: 0x04006A5A RID: 27226
	public static GameRandom random;

	// Token: 0x04006A5B RID: 27227
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float sMaxSunIntensity = 0.7f;

	// Token: 0x04006A5C RID: 27228
	public float maxSunIntensity = 0.7f;

	// Token: 0x04006A5D RID: 27229
	public float sunHeight = 0.1f;

	// Token: 0x04006A5E RID: 27230
	public float moonHeight = 0.095f;

	// Token: 0x04006A5F RID: 27231
	public float sunFadeHeight = 0.1f;

	// Token: 0x04006A60 RID: 27232
	public float cloudSpeed = 0.05f;

	// Token: 0x04006A61 RID: 27233
	public float ShowFogDensity;

	// Token: 0x04006A62 RID: 27234
	public Color ShowSkyColor;

	// Token: 0x04006A63 RID: 27235
	public Color ShowFogColor;

	// Token: 0x04006A64 RID: 27236
	public float maxFarClippingPlane = 1500f;

	// Token: 0x04006A65 RID: 27237
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dawnTime = 4;

	// Token: 0x04006A66 RID: 27238
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int duskTime = 22;

	// Token: 0x04006A67 RID: 27239
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static ulong timeOfDay;

	// Token: 0x04006A68 RID: 27240
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int bloodmoonDay = 7;

	// Token: 0x04006A69 RID: 27241
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float cloudTransition = 0f;

	// Token: 0x04006A6A RID: 27242
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogDensity;

	// Token: 0x04006A6B RID: 27243
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogLightScale;

	// Token: 0x04006A6C RID: 27244
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogStart = 20f;

	// Token: 0x04006A6D RID: 27245
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogEnd = 80f;

	// Token: 0x04006A6E RID: 27246
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogDebugDensity = -1f;

	// Token: 0x04006A6F RID: 27247
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogDebugStart = 0f;

	// Token: 0x04006A70 RID: 27248
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float fogDebugEnd = 0f;

	// Token: 0x04006A71 RID: 27249
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Color fogDebugColor;

	// Token: 0x04006A72 RID: 27250
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float worldRotationTime = 0f;

	// Token: 0x04006A73 RID: 27251
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float worldRotation = 0f;

	// Token: 0x04006A74 RID: 27252
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float worldRotationTarget = 0f;

	// Token: 0x04006A75 RID: 27253
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float sunIntensity = 1f;

	// Token: 0x04006A76 RID: 27254
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float starIntensity = 1f;

	// Token: 0x04006A77 RID: 27255
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool bNeedsReset = false;

	// Token: 0x04006A78 RID: 27256
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Color SkyColor;

	// Token: 0x04006A79 RID: 27257
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Color fogColor;

	// Token: 0x04006A7A RID: 27258
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Transform sunLightT;

	// Token: 0x04006A7B RID: 27259
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Light sunLight;

	// Token: 0x04006A7C RID: 27260
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector3 sunDirV;

	// Token: 0x04006A7D RID: 27261
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector3 sunMoonDirV;

	// Token: 0x04006A7E RID: 27262
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly float[] moonPhases = new float[]
	{
		0.05f,
		0.35f,
		0.55f,
		0.75f,
		1.4f,
		1.63f,
		1.82f
	};

	// Token: 0x04006A7F RID: 27263
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Transform moonLightT;

	// Token: 0x04006A80 RID: 27264
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Light moonLight;

	// Token: 0x04006A81 RID: 27265
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Transform moonSpriteT;

	// Token: 0x04006A82 RID: 27266
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Material moonSpriteMat;

	// Token: 0x04006A83 RID: 27267
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float moonBright;

	// Token: 0x04006A84 RID: 27268
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Color moonLightColor;

	// Token: 0x04006A85 RID: 27269
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Color moonSpriteColor;

	// Token: 0x04006A86 RID: 27270
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Quaternion moonLightRot;

	// Token: 0x04006A87 RID: 27271
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture cloudMainTex;

	// Token: 0x04006A88 RID: 27272
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture cloudMainTexOld;

	// Token: 0x04006A89 RID: 27273
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture cloudBlendTex;

	// Token: 0x04006A8A RID: 27274
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture cloudBlendTexOld;

	// Token: 0x04006A8B RID: 27275
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject parent;

	// Token: 0x04006A8C RID: 27276
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Camera mainCamera;

	// Token: 0x04006A8D RID: 27277
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static NGSS_FrustumShadows_7DTD frustumShadows;

	// Token: 0x04006A8E RID: 27278
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWorldRotationUpdateFreq = 0.2f;

	// Token: 0x04006A8F RID: 27279
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cStarRotationSpeed = 0.004f;

	// Token: 0x04006A90 RID: 27280
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bUpdateShaders;

	// Token: 0x04006A91 RID: 27281
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector4 fogParams;

	// Token: 0x04006A92 RID: 27282
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 sunAxis;

	// Token: 0x04006A93 RID: 27283
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 sunStartV;

	// Token: 0x04006A94 RID: 27284
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 moonStartV;

	// Token: 0x04006A95 RID: 27285
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 starAxis;

	// Token: 0x04006A96 RID: 27286
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool triggerLightning;

	// Token: 0x04006A97 RID: 27287
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int lightningFlashes;

	// Token: 0x04006A98 RID: 27288
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float[] lightningEndTimes = new float[]
	{
		1f,
		2f,
		3f,
		4f,
		5f
	};

	// Token: 0x04006A99 RID: 27289
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int lightningIndex;

	// Token: 0x04006A9A RID: 27290
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector2 lightningDir;

	// Token: 0x04006A9B RID: 27291
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector2 lightningDirVel;

	// Token: 0x04006A9C RID: 27292
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float lightningIntensity;

	// Token: 0x04006A9D RID: 27293
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static SunShaftsEffect.SunSettings sunShaftSettings;

	// Token: 0x04006A9E RID: 27294
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 targetSkyPosition;

	// Token: 0x04006A9F RID: 27295
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bMovingSky = true;
}
