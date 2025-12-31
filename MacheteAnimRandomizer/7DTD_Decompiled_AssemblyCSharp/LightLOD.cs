using System;
using Audio;
using UnityEngine;

// Token: 0x02000FF1 RID: 4081
public class LightLOD : MonoBehaviour
{
	// Token: 0x06008178 RID: 33144 RVA: 0x00347300 File Offset: 0x00345500
	public Light GetLight()
	{
		if (!this.myLight)
		{
			if (!this.otherLight)
			{
				this.myLight = base.GetComponent<Light>();
			}
			else
			{
				this.myLight = this.otherLight;
			}
			if (this.myLight)
			{
				this.shadowStateMaster = this.myLight.shadows;
				this.shadowStrengthMaster = this.myLight.shadowStrength;
			}
		}
		return this.myLight;
	}

	// Token: 0x06008179 RID: 33145 RVA: 0x00347376 File Offset: 0x00345576
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.Init();
	}

	// Token: 0x0600817A RID: 33146 RVA: 0x00347380 File Offset: 0x00345580
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		if (this.hasInitialized)
		{
			return;
		}
		this.hasInitialized = true;
		this.selfT = base.transform;
		Light light = this.GetLight();
		if (light)
		{
			if (GameManager.IsDedicatedServer && this.registeredRange == 0f)
			{
				this.SetRange(light.range);
			}
			this.lightIntensityMaster = light.intensity;
			this.lightRangeMaster = light.range;
			this.CalcViewDistance();
			if (this.lightStateType != LightStateType.Static)
			{
				Type type = Type.GetType(this.lightStateType.ToStringCached<LightStateType>());
				this.lightState = (base.gameObject.GetComponent(type) as LightState);
				if (!this.lightState)
				{
					this.lightState = (LightState)base.gameObject.AddComponent(type);
				}
			}
		}
		if (this.RefFlare)
		{
			this.lensFlare = this.RefFlare.GetComponent<LensFlare>();
		}
		else
		{
			this.lensFlare = base.GetComponent<LensFlare>();
		}
		if (this.RefIlluminatedMaterials != null)
		{
			this.maxLightPowerValues = new float[this.RefIlluminatedMaterials.Length];
			this.cachedMaterials = new Material[this.RefIlluminatedMaterials.Length][];
			for (int i = 0; i < this.RefIlluminatedMaterials.Length; i++)
			{
				Transform transform = this.RefIlluminatedMaterials[i];
				if (transform)
				{
					Renderer component = transform.GetComponent<Renderer>();
					if (component != null)
					{
						Material material = component.material;
						if (material != null)
						{
							if (material.HasProperty("_LightPower"))
							{
								this.maxLightPowerValues[i] = Utils.FastMax(material.GetFloat("_LightPower"), 1f);
							}
							else
							{
								this.maxLightPowerValues[i] = 1f;
							}
						}
						this.cachedMaterials[i] = component.materials;
					}
				}
			}
		}
		if (this.lightStateStart != LightStateType.Static)
		{
			this.LightStateType = this.lightStateStart;
		}
		if (!GameManager.IsDedicatedServer)
		{
			this.audioPlayer = base.GetComponent<AudioPlayer>();
		}
	}

	// Token: 0x0600817B RID: 33147 RVA: 0x0034756C File Offset: 0x0034576C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		this.Init();
		if (GameManager.Instance.World == null)
		{
			return;
		}
		this.lightRange = this.lightRangeMaster;
		this.lightIntensity = this.lightIntensityMaster;
		if (this.myLight)
		{
			this.myLight.enabled = false;
		}
		this.isInitialBlockDone = false;
		this.parentT = this.selfT.parent;
		LightManager.LightChanged(this.selfT.position + Origin.position);
		if (GameLightManager.Instance != null)
		{
			GameLightManager.Instance.AddLight(this);
		}
	}

	// Token: 0x0600817C RID: 33148 RVA: 0x00347604 File Offset: 0x00345804
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckInitialBlock()
	{
		if (this.isInitialBlockDone || !this.parentT || !this.selfT)
		{
			return;
		}
		if (this.bed == null)
		{
			this.isInitialBlockDone = true;
			if (!this.isHeld && this.registeredRange == 0f && this.myLight)
			{
				this.SetRange(this.lightRange);
			}
			return;
		}
		World world = GameManager.Instance.World;
		BlockValue block = world.GetBlock(this.bed.pos);
		Block block2 = block.Block;
		if (block2.isMultiBlock && block.ischild)
		{
			Vector3i parentPos = block2.multiBlockPos.GetParentPos(this.bed.pos, block);
			block = world.GetBlock(parentPos);
		}
		if (this.bToggleable)
		{
			this.SwitchOnOff((block.meta & 2) > 0, false);
		}
		if (this.registeredRange == 0f && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.myLight)
		{
			this.SetRange(this.lightRange);
		}
		this.isInitialBlockDone = true;
	}

	// Token: 0x17000D7D RID: 3453
	// (get) Token: 0x0600817D RID: 33149 RVA: 0x0034771C File Offset: 0x0034591C
	// (set) Token: 0x0600817E RID: 33150 RVA: 0x00347724 File Offset: 0x00345924
	public LightStateType LightStateType
	{
		get
		{
			return this.lightStateType;
		}
		set
		{
			if (this.lightStateType == value)
			{
				return;
			}
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			if (this.lightState)
			{
				UnityEngine.Object.Destroy(this.lightState);
			}
			this.lightStateType = value;
			if (this.lightStateType != LightStateType.Static)
			{
				Type type = Type.GetType(this.lightStateType.ToStringCached<LightStateType>());
				this.lightState = (LightState)base.gameObject.AddComponent(type);
			}
		}
	}

	// Token: 0x17000D7E RID: 3454
	// (get) Token: 0x06008180 RID: 33152 RVA: 0x003477B2 File Offset: 0x003459B2
	// (set) Token: 0x0600817F RID: 33151 RVA: 0x00347792 File Offset: 0x00345992
	public float MaxIntensity
	{
		get
		{
			return this.lightIntensity;
		}
		set
		{
			this.lightIntensity = value;
			this.lightIntensityMaster = value;
			LightLOD.MaxIntensityEvent maxIntensityChanged = this.MaxIntensityChanged;
			if (maxIntensityChanged == null)
			{
				return;
			}
			maxIntensityChanged();
		}
	}

	// Token: 0x17000D7F RID: 3455
	// (set) Token: 0x06008181 RID: 33153 RVA: 0x003477BA File Offset: 0x003459BA
	public float LightAngle
	{
		set
		{
			if (this.myLight.type == LightType.Spot)
			{
				if (value > 160f)
				{
					value = 160f;
				}
				this.myLight.spotAngle = value;
			}
		}
	}

	// Token: 0x06008182 RID: 33154 RVA: 0x003477E4 File Offset: 0x003459E4
	public void SetEmissiveColor(bool _on)
	{
		this.SetEmissiveColor(_on ? 1f : 0f);
	}

	// Token: 0x06008183 RID: 33155 RVA: 0x003477FC File Offset: 0x003459FC
	public void SetEmissiveColor(float _newV)
	{
		if (_newV <= 0f)
		{
			this.SetEmissiveColor(LightLOD.EmissiveColorOff);
			return;
		}
		Color color;
		if (!this.EmissiveFromLightColorOn)
		{
			color = this.EmissiveColor;
		}
		else
		{
			Light light = this.GetLight();
			if (!light)
			{
				return;
			}
			color = light.color;
		}
		if (_newV >= 1f)
		{
			this.SetEmissiveColor(color);
		}
		float h;
		float s;
		float num;
		Color.RGBToHSV(color, out h, out s, out num);
		Color emissiveColor = Color.HSVToRGB(h, s, _newV);
		this.SetEmissiveColor(emissiveColor);
	}

	// Token: 0x06008184 RID: 33156 RVA: 0x00347874 File Offset: 0x00345A74
	public void SetEmissiveColor(Color _color)
	{
		if (this.RefIlluminatedMaterials == null)
		{
			return;
		}
		if (this.lastEmissiveColor != null && _color == this.lastEmissiveColor.Value)
		{
			return;
		}
		this.lastEmissiveColor = new Color?(_color);
		bool flag = _color != LightLOD.EmissiveColorOff;
		for (int i = 0; i < this.cachedMaterials.Length; i++)
		{
			Material[] array = this.cachedMaterials[i];
			if (array != null)
			{
				foreach (Material material in array)
				{
					if (material)
					{
						material.SetColor(LightLOD.EmissionColorId, _color);
						if (flag)
						{
							material.EnableKeyword("_EMISSION");
						}
						else
						{
							material.DisableKeyword("_EMISSION");
						}
					}
				}
			}
		}
	}

	// Token: 0x06008185 RID: 33157 RVA: 0x00347928 File Offset: 0x00345B28
	public void SetRange(float _range)
	{
		Light light = this.GetLight();
		this.lightRange = _range;
		this.lightRangeMaster = _range;
		light.range = _range;
		this.CalcViewDistance();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && base.enabled && light.enabled)
		{
			this.UnregisterFromLightManager();
			this.registeredPos = light.transform.position + Origin.position;
			this.registeredRange = LightManager.RegisterLight(light);
		}
	}

	// Token: 0x06008186 RID: 33158 RVA: 0x003479A0 File Offset: 0x00345BA0
	public void TestRegistration()
	{
		if (this.myLight)
		{
			this.SetRange(this.lightRange);
		}
	}

	// Token: 0x06008187 RID: 33159 RVA: 0x003479BB File Offset: 0x00345BBB
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcViewDistance()
	{
		this.lightViewDistance = Utils.FastMax(this.MaxDistance, this.lightRangeMaster * 1.5f);
	}

	// Token: 0x06008188 RID: 33160 RVA: 0x003479DA File Offset: 0x00345BDA
	public void SetBlockEntityData(BlockEntityData _bed)
	{
		this.bed = _bed;
	}

	// Token: 0x06008189 RID: 33161 RVA: 0x003479E3 File Offset: 0x00345BE3
	public void SwitchOnOff(bool _isOn, bool _ignoreToggle = false)
	{
		if ((_ignoreToggle || this.bToggleable) && _isOn != this.bSwitchedOn)
		{
			this.bSwitchedOn = _isOn;
		}
		if (this.bSwitchedOn)
		{
			base.enabled = true;
		}
	}

	// Token: 0x0600818A RID: 33162 RVA: 0x00347A0F File Offset: 0x00345C0F
	public void SetCulled(bool _culled)
	{
		this.isCulled = _culled;
		if (this.myLight)
		{
			this.myLight.enabled = !_culled;
		}
	}

	// Token: 0x0600818B RID: 33163 RVA: 0x00347A34 File Offset: 0x00345C34
	public void OnDisable()
	{
		if (!string.IsNullOrEmpty(this.cyclingAudioClipPath) && this.cyclingAudioHandle != null)
		{
			this.cyclingAudioHandle.SetVolume(0f);
		}
		if (GameManager.Instance.World != null)
		{
			LightManager.LightChanged(this.selfT.position + Origin.position);
			if (GameLightManager.Instance != null)
			{
				GameLightManager.Instance.RemoveLight(this);
			}
		}
		this.UnregisterFromLightManager();
		this.parentT = null;
	}

	// Token: 0x0600818C RID: 33164 RVA: 0x00347AAB File Offset: 0x00345CAB
	[PublicizedFrom(EAccessModifier.Private)]
	public void UnregisterFromLightManager()
	{
		if (this.registeredRange > 0f)
		{
			LightManager.UnRegisterLight(this.registeredPos, this.registeredRange);
			this.registeredRange = 0f;
		}
	}

	// Token: 0x0600818D RID: 33165 RVA: 0x00347AD8 File Offset: 0x00345CD8
	public void FrameUpdate(Vector3 cameraPos)
	{
		bool flag = this.lightStateType > LightStateType.Static;
		this.priority = (float)(flag ? 1 : 0);
		if (this.bRenderingOff || !this.myLight)
		{
			return;
		}
		this.CheckInitialBlock();
		bool flag2 = this.bSwitchedOn && (!flag || this.lightState.CanBeOn);
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		if (!this.isCulled)
		{
			num = (this.selfT.position - cameraPos).sqrMagnitude * this.DistanceScale;
			num2 = Mathf.Sqrt(num) - this.lightRange;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
		}
		float num3 = this.lightViewDistance;
		if (this.bPlayerPlacedLight)
		{
			num3 *= 1.2f;
		}
		if (LightLOD.DebugViewDistance > 0f)
		{
			num3 = Utils.FastMax(LightLOD.DebugViewDistance, this.lightRange + 0.01f);
		}
		float num4 = num3 * num3;
		float num5 = num / num4;
		float num6 = num3 - this.lightRange;
		if (flag2)
		{
			flag2 &= (num2 < num6);
			if (this.lightStateType != LightStateType.Blinking)
			{
				flag2 &= (num5 < (flag ? this.lightState.LODThreshold : 1f));
			}
		}
		if (flag2 && !this.bWorksUnderwater)
		{
			bool flag3 = false;
			if (this.waterLevelRecheckTime != 3.4028235E+38f)
			{
				if (Time.time >= this.waterLevelRecheckTime)
				{
					this.waterLevelRecheckTime = float.MaxValue;
					if (this.WaterLevelDirty)
					{
						this.waterLevelRecheckTime = Time.time + 0.5f;
					}
					this.WaterLevelDirty = false;
					flag3 = (this.bed != null);
				}
			}
			else if (this.WaterLevelDirty)
			{
				this.WaterLevelDirty = false;
				flag3 = (this.bed != null);
				if (flag3)
				{
					this.waterLevelRecheckTime = Time.time + 0.5f;
				}
			}
			if (flag3)
			{
				bool flag4 = GameManager.Instance.World.IsWater(this.bed.pos.x, this.bed.pos.y + 1, this.bed.pos.z);
				if (!flag4 && GameManager.Instance.World.IsWater(this.bed.pos))
				{
					flag4 = (0.6f + (float)this.bed.pos.y > base.transform.position.y);
				}
				if (this.isUnderwater != flag4)
				{
					this.isUnderwater = flag4;
				}
			}
			flag2 &= !this.isUnderwater;
		}
		if (flag2)
		{
			if (!this.myLight.enabled)
			{
				this.priority = Utils.FastMax(this.priority, 1f);
			}
			if (this.bPlayerPlacedLight)
			{
				if (num5 >= 0.64000005f)
				{
					this.myLight.shadows = LightShadows.None;
				}
				else if (num5 >= 0.0625f)
				{
					if (this.shadowStateMaster == LightShadows.Soft)
					{
						this.myLight.shadows = LightShadows.Hard;
					}
					this.myLight.shadowStrength = (1f - Utils.FastClamp01((num5 - 0.36f) / 0.28000003f)) * this.shadowStrengthMaster;
				}
				else
				{
					this.myLight.shadows = this.shadowStateMaster;
					this.myLight.shadowStrength = this.shadowStrengthMaster;
				}
			}
			float num7 = num2 / num6;
			float num8 = 1f - num7 * num7;
			this.myLight.intensity = this.lightIntensity * num8 * (flag ? this.lightState.Intensity : 1f);
			this.myLight.range = this.lightRange * 0.5f + this.lightRange * 0.5f * num8;
			this.SetEmissiveColor(flag ? this.lightState.Emissive : 1f);
			if (flag && this.lightState.AudioFrequency > 0f && !string.IsNullOrEmpty(this.cyclingAudioClipPath))
			{
				float num9 = this.cyclingAudioPlayEveryNCycles * (1f / this.lightState.AudioFrequency);
				float num10 = (Time.time + this.cyclingAudioOffset) % num9;
				if (num10 < this.previousCycleTime)
				{
					this.cyclingAudioHandle = Manager.Play(this.selfT.position + Origin.position, this.cyclingAudioClipPath, -1, true);
				}
				this.previousCycleTime = num10;
			}
		}
		else
		{
			this.SetEmissiveColor(false);
		}
		this.myLight.enabled = flag2;
		if (this.LitRootObject)
		{
			this.LitRootObject.SetActive(flag2);
		}
		if (this.audioPlayer)
		{
			this.audioPlayer.enabled = flag2;
		}
		if (this.lensFlare)
		{
			if (flag2 && num < 10f * num4)
			{
				float num11 = (1f - num / (num4 * 10f)) * this.lightIntensity * 0.33f * this.FlareBrightnessFactor;
				if (num11 > 1f)
				{
					num11 = 1f;
				}
				if (this.lightRange < 4f)
				{
					num11 *= this.lightRange * 0.25f;
				}
				this.lensFlare.brightness = num11;
				this.lensFlare.color = this.myLight.color;
				this.lensFlare.enabled = true;
			}
			else
			{
				this.lensFlare.enabled = false;
			}
		}
		base.enabled = this.bSwitchedOn;
	}

	// Token: 0x0600818E RID: 33166 RVA: 0x0034801F File Offset: 0x0034621F
	public void SetRenderingOn()
	{
		this.bRenderingOff = false;
	}

	// Token: 0x0600818F RID: 33167 RVA: 0x00348028 File Offset: 0x00346228
	public void SetRenderingOff()
	{
		this.bRenderingOff = true;
	}

	// Token: 0x04006402 RID: 25602
	public GameObject LitRootObject;

	// Token: 0x04006403 RID: 25603
	public Transform[] RefIlluminatedMaterials;

	// Token: 0x04006404 RID: 25604
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material[][] cachedMaterials;

	// Token: 0x04006405 RID: 25605
	public Transform RefFlare;

	// Token: 0x04006406 RID: 25606
	public float MaxDistance = 30f;

	// Token: 0x04006407 RID: 25607
	public float DistanceScale = 1f;

	// Token: 0x04006408 RID: 25608
	public float FlareBrightnessFactor = 1f;

	// Token: 0x04006409 RID: 25609
	public bool bPlayerPlacedLight;

	// Token: 0x0400640A RID: 25610
	public bool bSwitchedOn;

	// Token: 0x0400640B RID: 25611
	public bool bToggleable = true;

	// Token: 0x0400640C RID: 25612
	public bool bWorksUnderwater = true;

	// Token: 0x0400640D RID: 25613
	public bool isHeld;

	// Token: 0x0400640E RID: 25614
	public Light otherLight;

	// Token: 0x0400640F RID: 25615
	public LightStateType lightStateStart;

	// Token: 0x04006410 RID: 25616
	public string cyclingAudioClipPath;

	// Token: 0x04006411 RID: 25617
	public float cyclingAudioOffset;

	// Token: 0x04006412 RID: 25618
	public float cyclingAudioPlayEveryNCycles = 1f;

	// Token: 0x04006413 RID: 25619
	public bool EmissiveFromLightColorOn;

	// Token: 0x04006414 RID: 25620
	public Color EmissiveColor = Color.white;

	// Token: 0x04006415 RID: 25621
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly Color EmissiveColorOff = Color.black;

	// Token: 0x04006416 RID: 25622
	public float StateRate = 1f;

	// Token: 0x04006417 RID: 25623
	public float FluxDelay = 1f;

	// Token: 0x04006418 RID: 25624
	public static float DebugViewDistance;

	// Token: 0x04006419 RID: 25625
	public LightLOD.MaxIntensityEvent MaxIntensityChanged;

	// Token: 0x0400641A RID: 25626
	public bool WaterLevelDirty;

	// Token: 0x0400641B RID: 25627
	public float priority;

	// Token: 0x0400641C RID: 25628
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCulled;

	// Token: 0x0400641D RID: 25629
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform selfT;

	// Token: 0x0400641E RID: 25630
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform parentT;

	// Token: 0x0400641F RID: 25631
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04006420 RID: 25632
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasInitialized;

	// Token: 0x04006421 RID: 25633
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockEntityData bed;

	// Token: 0x04006422 RID: 25634
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInitialBlockDone;

	// Token: 0x04006423 RID: 25635
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light myLight;

	// Token: 0x04006424 RID: 25636
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightIntensityMaster;

	// Token: 0x04006425 RID: 25637
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightIntensity;

	// Token: 0x04006426 RID: 25638
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightRangeMaster;

	// Token: 0x04006427 RID: 25639
	[NonSerialized]
	public float lightRange;

	// Token: 0x04006428 RID: 25640
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightViewDistance;

	// Token: 0x04006429 RID: 25641
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bRenderingOff;

	// Token: 0x0400642A RID: 25642
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LensFlare lensFlare;

	// Token: 0x0400642B RID: 25643
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] maxLightPowerValues;

	// Token: 0x0400642C RID: 25644
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 registeredPos;

	// Token: 0x0400642D RID: 25645
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float registeredRange;

	// Token: 0x0400642E RID: 25646
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LightStateType lightStateType;

	// Token: 0x0400642F RID: 25647
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LightState lightState;

	// Token: 0x04006430 RID: 25648
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LightShadows shadowStateMaster;

	// Token: 0x04006431 RID: 25649
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float shadowStrengthMaster;

	// Token: 0x04006432 RID: 25650
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioPlayer audioPlayer;

	// Token: 0x04006433 RID: 25651
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Handle cyclingAudioHandle;

	// Token: 0x04006434 RID: 25652
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUnderwater;

	// Token: 0x04006435 RID: 25653
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float waterLevelRecheckTime;

	// Token: 0x04006436 RID: 25654
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color? lastEmissiveColor;

	// Token: 0x04006437 RID: 25655
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float previousCycleTime;

	// Token: 0x02000FF2 RID: 4082
	// (Invoke) Token: 0x06008193 RID: 33171
	public delegate void MaxIntensityEvent();
}
