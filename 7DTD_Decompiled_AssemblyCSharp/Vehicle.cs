using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;

// Token: 0x02000B3D RID: 2877
public class Vehicle
{
	// Token: 0x1700090A RID: 2314
	// (get) Token: 0x06005950 RID: 22864 RVA: 0x0023FF4C File Offset: 0x0023E14C
	public Vector2 CameraDistance
	{
		get
		{
			return this.cameraDistance;
		}
	}

	// Token: 0x1700090B RID: 2315
	// (get) Token: 0x06005951 RID: 22865 RVA: 0x0023FF54 File Offset: 0x0023E154
	public Vector2 CameraTurnRate
	{
		get
		{
			return this.cameraTurnRate;
		}
	}

	// Token: 0x1700090C RID: 2316
	// (get) Token: 0x06005952 RID: 22866 RVA: 0x0023FF5C File Offset: 0x0023E15C
	public float BrakeTorque
	{
		get
		{
			return this.brakeTorque;
		}
	}

	// Token: 0x1700090D RID: 2317
	// (get) Token: 0x06005953 RID: 22867 RVA: 0x0023FF64 File Offset: 0x0023E164
	public float SteerAngleMax
	{
		get
		{
			return this.steerAngleMax;
		}
	}

	// Token: 0x1700090E RID: 2318
	// (get) Token: 0x06005954 RID: 22868 RVA: 0x0023FF6C File Offset: 0x0023E16C
	public float SteerRate
	{
		get
		{
			return this.steerRate;
		}
	}

	// Token: 0x1700090F RID: 2319
	// (get) Token: 0x06005955 RID: 22869 RVA: 0x0023FF74 File Offset: 0x0023E174
	public float SteerCenteringRate
	{
		get
		{
			return this.steerCenteringRate;
		}
	}

	// Token: 0x17000910 RID: 2320
	// (get) Token: 0x06005956 RID: 22870 RVA: 0x0023FF7C File Offset: 0x0023E17C
	public float TiltAngleMax
	{
		get
		{
			return this.tiltAngleMax;
		}
	}

	// Token: 0x17000911 RID: 2321
	// (get) Token: 0x06005957 RID: 22871 RVA: 0x0023FF84 File Offset: 0x0023E184
	public float TiltThreshold
	{
		get
		{
			return this.tiltThreshold;
		}
	}

	// Token: 0x17000912 RID: 2322
	// (get) Token: 0x06005958 RID: 22872 RVA: 0x0023FF8C File Offset: 0x0023E18C
	public float TiltDampening
	{
		get
		{
			return this.tiltDampening;
		}
	}

	// Token: 0x17000913 RID: 2323
	// (get) Token: 0x06005959 RID: 22873 RVA: 0x0023FF94 File Offset: 0x0023E194
	public float TiltDampenThreshold
	{
		get
		{
			return this.tiltDampenThreshold;
		}
	}

	// Token: 0x17000914 RID: 2324
	// (get) Token: 0x0600595A RID: 22874 RVA: 0x0023FF9C File Offset: 0x0023E19C
	public float TiltUpForce
	{
		get
		{
			return this.tiltUpForce;
		}
	}

	// Token: 0x17000915 RID: 2325
	// (get) Token: 0x0600595B RID: 22875 RVA: 0x0023FFA4 File Offset: 0x0023E1A4
	public Vector2 HopForce
	{
		get
		{
			return this.hopForce;
		}
	}

	// Token: 0x17000916 RID: 2326
	// (get) Token: 0x0600595C RID: 22876 RVA: 0x0023FFAC File Offset: 0x0023E1AC
	public float UpAngleMax
	{
		get
		{
			return this.upAngleMax;
		}
	}

	// Token: 0x17000917 RID: 2327
	// (get) Token: 0x0600595D RID: 22877 RVA: 0x0023FFB4 File Offset: 0x0023E1B4
	public float UpForce
	{
		get
		{
			return this.upForce;
		}
	}

	// Token: 0x17000918 RID: 2328
	// (get) Token: 0x0600595E RID: 22878 RVA: 0x0023FFBC File Offset: 0x0023E1BC
	public float UnstickForce
	{
		get
		{
			return this.unstickForce;
		}
	}

	// Token: 0x17000919 RID: 2329
	// (get) Token: 0x0600595F RID: 22879 RVA: 0x0023FFC4 File Offset: 0x0023E1C4
	public float MaxPossibleSpeed
	{
		get
		{
			return this.VelocityMaxTurboForward;
		}
	}

	// Token: 0x1700091A RID: 2330
	// (get) Token: 0x06005960 RID: 22880 RVA: 0x0023FFCC File Offset: 0x0023E1CC
	// (set) Token: 0x06005961 RID: 22881 RVA: 0x0023FFD4 File Offset: 0x0023E1D4
	public PlatformUserIdentifierAbs OwnerId
	{
		get
		{
			return this.m_ownerId;
		}
		set
		{
			this.m_ownerId = value;
			int belongsPlayerId = -1;
			PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(this.m_ownerId);
			if (playerData != null)
			{
				belongsPlayerId = playerData.EntityId;
			}
			this.entity.belongsPlayerId = belongsPlayerId;
		}
	}

	// Token: 0x06005962 RID: 22882 RVA: 0x00240018 File Offset: 0x0023E218
	public Vehicle(string _vehicleName, EntityVehicle _entity)
	{
		this.vehicleName = _vehicleName.ToLower();
		this.entity = _entity;
		this.SetupProperties();
		this.meshT = this.entity.ModelTransform.Find("Mesh");
		if (!this.meshT)
		{
			this.meshT = this.entity.ModelTransform;
		}
		this.vehicleParts = new List<VehiclePart>();
		this.OwnerId = null;
		this.AllowedUsers = new List<PlatformUserIdentifierAbs>();
		this.PasswordHash = 0;
		this.MakeItemValue();
		this.CreateParts();
	}

	// Token: 0x06005963 RID: 22883 RVA: 0x002401DC File Offset: 0x0023E3DC
	public void MakeItemValue()
	{
		string name = this.GetName();
		int type = 0;
		ItemClass itemClass = ItemClass.GetItemClass(name + "Placeable", true);
		if (itemClass != null)
		{
			type = itemClass.Id;
		}
		this.itemValue = new ItemValue(type, 1, 6, false, null, 1f);
		this.SetItemValue(this.itemValue);
	}

	// Token: 0x06005964 RID: 22884 RVA: 0x00240230 File Offset: 0x0023E430
	public void SetItemValue(ItemValue _itemValue)
	{
		this.itemValue = _itemValue;
		if (this.itemValue.CosmeticMods.Length == 0)
		{
			this.itemValue.CosmeticMods = new ItemValue[1];
		}
		int num = this.itemValue.MaxUseTimes;
		if (this.itemValue.type == 0)
		{
			num = 5555;
		}
		int health = num - (int)this.itemValue.UseTimes;
		this.entity.Stats.Health.BaseMax = (float)num;
		this.entity.Stats.Health.OriginalMax = (float)num;
		this.entity.Health = health;
		this.CalcEffects();
		this.SetFuelLevel((float)this.itemValue.Meta / 50f);
		this.CalcMods();
		this.SetColors();
		this.SetSeats();
	}

	// Token: 0x06005965 RID: 22885 RVA: 0x002402FC File Offset: 0x0023E4FC
	public void SetItemValueMods(ItemValue _itemValue)
	{
		ItemValue itemValue = _itemValue.Clone();
		this.itemValue.Modifications = itemValue.Modifications;
		this.itemValue.CosmeticMods = itemValue.CosmeticMods;
		this.CalcEffects();
		this.CalcMods();
		this.SetColors();
		this.SetSeats();
	}

	// Token: 0x06005966 RID: 22886 RVA: 0x0024034C File Offset: 0x0023E54C
	public void SetColors()
	{
		Color white = Color.white;
		Vector3 vector = Block.StringToVector3(this.itemValue.GetPropertyOverride(Block.PropTintColor, "255,255,255"));
		white.r = vector.x;
		white.g = vector.y;
		white.b = vector.z;
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			this.vehicleParts[i].SetColors(white);
		}
	}

	// Token: 0x06005967 RID: 22887 RVA: 0x002403CC File Offset: 0x0023E5CC
	public void SetSeats()
	{
		int num = (int)EffectManager.GetValue(PassiveEffects.VehicleSeats, this.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		int num2 = 0;
		if (this.Properties != null)
		{
			int num3 = 0;
			int num4 = 0;
			DynamicProperties dynamicProperties;
			while (num4 < 99 && this.Properties.Classes.TryGetValue("seat" + num4.ToString(), out dynamicProperties))
			{
				if (dynamicProperties.GetString("mod").Length > 0)
				{
					if (num3 >= num)
					{
						break;
					}
					num3++;
				}
				num2++;
				num4++;
			}
		}
		this.entity.SetAttachMaxCount(num2);
	}

	// Token: 0x06005968 RID: 22888 RVA: 0x00240470 File Offset: 0x0023E670
	public int GetSeatPose(int _seatIndex)
	{
		DynamicProperties dynamicProperties;
		if (this.Properties != null && this.Properties.Classes.TryGetValue("seat" + _seatIndex.ToString(), out dynamicProperties))
		{
			string @string = dynamicProperties.GetString("pose");
			if (@string.Length > 0)
			{
				return int.Parse(@string);
			}
		}
		return 0;
	}

	// Token: 0x06005969 RID: 22889 RVA: 0x002404C8 File Offset: 0x0023E6C8
	public ItemValue GetUpdatedItemValue()
	{
		this.itemValue.UseTimes = (float)((int)this.entity.Stats.Health.BaseMax - this.entity.Health);
		this.itemValue.Meta = (int)(this.GetFuelLevel() * 50f);
		return this.itemValue;
	}

	// Token: 0x0600596A RID: 22890 RVA: 0x00240521 File Offset: 0x0023E721
	public void LoadItems(ItemStack[] _items)
	{
		this.SetItemValue(_items[0].itemValue);
	}

	// Token: 0x0600596B RID: 22891 RVA: 0x00240531 File Offset: 0x0023E731
	public ItemStack[] GetItems()
	{
		return new ItemStack[]
		{
			new ItemStack(this.GetUpdatedItemValue(), 1)
		};
	}

	// Token: 0x0600596C RID: 22892 RVA: 0x00240548 File Offset: 0x0023E748
	public void Update(float _deltaTime)
	{
		this.UpdateEffects(_deltaTime);
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			this.vehicleParts[i].Update(_deltaTime);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !this.HasStorage())
		{
			GameManager.Instance.DropContentOfLootContainerServer(BlockValue.Air, new Vector3i(this.entity.position), this.entity.entityId, null);
		}
	}

	// Token: 0x0600596D RID: 22893 RVA: 0x002405C3 File Offset: 0x0023E7C3
	public void UpdateSimulation()
	{
		this.FireEvent(Vehicle.Event.SimulationUpdate);
	}

	// Token: 0x0600596E RID: 22894 RVA: 0x002405CC File Offset: 0x0023E7CC
	public void FireEvent(Vehicle.Event _event)
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			this.vehicleParts[i].HandleEvent(_event, 0f);
		}
	}

	// Token: 0x0600596F RID: 22895 RVA: 0x00240608 File Offset: 0x0023E808
	public void FireEvent(VehiclePart.Event _event, VehiclePart _fromPart, float _arg)
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			this.vehicleParts[i].HandleEvent(_event, _fromPart, _arg);
		}
	}

	// Token: 0x06005970 RID: 22896 RVA: 0x0024063F File Offset: 0x0023E83F
	public void SetupProperties()
	{
		if (!Vehicle.PropertyMap.TryGetValue(this.vehicleName, out this.Properties))
		{
			Log.Error("Vehicle properties for '{0}' not found!", new object[]
			{
				this.vehicleName
			});
		}
	}

	// Token: 0x06005971 RID: 22897 RVA: 0x00240674 File Offset: 0x0023E874
	public DynamicProperties GetPropertiesForClass(string className)
	{
		if (this.Properties == null)
		{
			return null;
		}
		DynamicProperties result;
		this.Properties.Classes.TryGetValue(className, out result);
		return result;
	}

	// Token: 0x06005972 RID: 22898 RVA: 0x002406A0 File Offset: 0x0023E8A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseGeneralProperties(DynamicProperties properties)
	{
		properties.ParseVec("cameraDistance", ref this.cameraDistance);
		properties.ParseVec("cameraTurnRate", ref this.cameraTurnRate);
		properties.ParseFloat("steerAngleMax", ref this.steerAngleMax);
		properties.ParseFloat("steerRate", ref this.steerRate);
		properties.ParseFloat("steerCenteringRate", ref this.steerCenteringRate);
		properties.ParseFloat("tiltAngleMax", ref this.tiltAngleMax);
		properties.ParseFloat("tiltThreshold", ref this.tiltThreshold);
		properties.ParseFloat("tiltDampening", ref this.tiltDampening);
		properties.ParseFloat("tiltDampenThreshold", ref this.tiltDampenThreshold);
		properties.ParseFloat("tiltUpForce", ref this.tiltUpForce);
		properties.ParseFloat("upAngleMax", ref this.upAngleMax);
		properties.ParseFloat("upForce", ref this.upForce);
		properties.ParseVec("motorTorque_turbo", ref this.MotorTorqueForward, ref this.MotorTorqueBackward, ref this.MotorTorqueTurboForward, ref this.MotorTorqueTurboBackward);
		properties.ParseVec("velocityMax_turbo", ref this.VelocityMaxForward, ref this.VelocityMaxBackward, ref this.VelocityMaxTurboForward, ref this.VelocityMaxTurboBackward);
		properties.ParseFloat("brakeTorque", ref this.brakeTorque);
		properties.ParseVec("hopForce", ref this.hopForce);
		properties.ParseFloat("unstickForce", ref this.unstickForce);
		properties.ParseVec("airDrag_velScale_angVelScale", ref this.AirDragVelScale, ref this.AirDragAngVelScale);
		properties.ParseVec("waterDrag_y_velScale_velMaxScale", ref this.WaterDragY, ref this.WaterDragVelScale, ref this.WaterDragVelMaxScale);
		properties.ParseVec("waterLift_y_depth_force", ref this.WaterLiftY, ref this.WaterLiftDepth, ref this.WaterLiftForce);
		properties.ParseFloat("wheelPtlScale", ref this.WheelPtlScale);
		properties.ParseString("recipeName", ref this.RecipeName);
	}

	// Token: 0x06005973 RID: 22899 RVA: 0x00240868 File Offset: 0x0023EA68
	public void OnXMLChanged()
	{
		this.SetupProperties();
		DynamicProperties properties = this.Properties;
		if (properties == null)
		{
			return;
		}
		this.ParseGeneralProperties(properties);
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair in properties.Classes.Dict)
		{
			VehiclePart vehiclePart = this.FindPart(keyValuePair.Key);
			if (vehiclePart != null)
			{
				DynamicProperties value = keyValuePair.Value;
				vehiclePart.SetProperties(value);
			}
		}
	}

	// Token: 0x06005974 RID: 22900 RVA: 0x002408F4 File Offset: 0x0023EAF4
	public void TriggerUpdateEffects()
	{
		this.effectUpdateDelay = 0f;
	}

	// Token: 0x06005975 RID: 22901 RVA: 0x00240901 File Offset: 0x0023EB01
	public void UpdateEffects(float _deltaTime)
	{
		this.effectUpdateDelay -= _deltaTime;
		if (this.effectUpdateDelay > 0f)
		{
			return;
		}
		this.effectUpdateDelay = 2f;
		this.GetUpdatedItemValue();
		this.CalcEffects();
	}

	// Token: 0x06005976 RID: 22902 RVA: 0x00240938 File Offset: 0x0023EB38
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcEffects()
	{
		EntityAlive entityAlive = this.entity.AttachedMainEntity as EntityAlive;
		FastTags<TagGroup.Global> entityTags = this.entity.EntityTags;
		this.EffectEntityDamagePer = EffectManager.GetValue(PassiveEffects.VehicleEntityDamage, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectBlockDamagePer = EffectManager.GetValue(PassiveEffects.VehicleBlockDamage, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectSelfDamagePer = EffectManager.GetValue(PassiveEffects.VehicleSelfDamage, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectStrongSelfDamagePer = EffectManager.GetValue(PassiveEffects.VehicleStrongSelfDamage, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectLightIntensity = EffectManager.GetValue(PassiveEffects.LightIntensity, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectFuelMaxPer = EffectManager.GetValue(PassiveEffects.VehicleFuelMaxPer, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectFuelUsePer = EffectManager.GetValue(PassiveEffects.VehicleFuelUsePer, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectMotorTorquePer = EffectManager.GetValue(PassiveEffects.VehicleMotorTorquePer, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
		this.EffectVelocityMaxPer = EffectManager.GetValue(PassiveEffects.VehicleVelocityMaxPer, this.itemValue, 1f, entityAlive, null, entityTags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06005977 RID: 22903 RVA: 0x00240AA0 File Offset: 0x0023ECA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcMods()
	{
		this.ModTags = FastTags<TagGroup.Global>.none;
		ItemValue[] modifications = this.itemValue.Modifications;
		if (modifications != null)
		{
			foreach (ItemValue itemValue in modifications)
			{
				if (itemValue != null)
				{
					ItemClassModifier itemClassModifier = itemValue.ItemClass as ItemClassModifier;
					if (itemClassModifier != null)
					{
						this.ModTags |= itemClassModifier.ItemTags;
					}
				}
			}
		}
		for (int j = 0; j < this.vehicleParts.Count; j++)
		{
			this.vehicleParts[j].SetMods();
		}
	}

	// Token: 0x06005978 RID: 22904 RVA: 0x00240B30 File Offset: 0x0023ED30
	public void CreateParts()
	{
		DynamicProperties properties = this.Properties;
		if (properties == null)
		{
			return;
		}
		this.ParseGeneralProperties(properties);
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair in properties.Classes.Dict)
		{
			DynamicProperties value = keyValuePair.Value;
			string @string = value.GetString("class");
			if (@string.Length > 0)
			{
				try
				{
					VehiclePart vehiclePart = (VehiclePart)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("VP", @string));
					vehiclePart.SetVehicle(this);
					vehiclePart.SetTag(keyValuePair.Key);
					vehiclePart.SetProperties(value);
					this.vehicleParts.Add(vehiclePart);
				}
				catch (Exception ex)
				{
					Log.Out(ex.Message);
					Log.Out(ex.StackTrace);
					throw new Exception("No vehicle part class 'VP" + @string + "' found!");
				}
			}
		}
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			this.vehicleParts[i].InitPrefabConnections();
		}
	}

	// Token: 0x06005979 RID: 22905 RVA: 0x00240C64 File Offset: 0x0023EE64
	public VehiclePart FindPart(string _tag)
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			if (this.vehicleParts[i].tag == _tag)
			{
				return this.vehicleParts[i];
			}
		}
		return null;
	}

	// Token: 0x0600597A RID: 22906 RVA: 0x00240CB0 File Offset: 0x0023EEB0
	public string GetPartProperty(string _tag, string _propertyName)
	{
		VehiclePart vehiclePart = this.FindPart(_tag);
		if (vehiclePart == null)
		{
			return string.Empty;
		}
		return vehiclePart.GetProperty(_propertyName);
	}

	// Token: 0x0600597B RID: 22907 RVA: 0x00240CD5 File Offset: 0x0023EED5
	public List<VehiclePart> GetParts()
	{
		return this.vehicleParts;
	}

	// Token: 0x0600597C RID: 22908 RVA: 0x00240CE0 File Offset: 0x0023EEE0
	public static void SetupPreview(Transform rootT)
	{
		Transform transform = rootT.Find("Physics");
		if (transform)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		ParticleSystem[] componentsInChildren = rootT.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x0600597D RID: 22909 RVA: 0x00240D2F File Offset: 0x0023EF2F
	public Transform GetMeshTransform()
	{
		return this.meshT;
	}

	// Token: 0x0600597E RID: 22910 RVA: 0x00240D37 File Offset: 0x0023EF37
	public string GetName()
	{
		return this.vehicleName;
	}

	// Token: 0x0600597F RID: 22911 RVA: 0x00240D3F File Offset: 0x0023EF3F
	public static void Cleanup()
	{
		Vehicle.PropertyMap = new Dictionary<string, DynamicProperties>();
	}

	// Token: 0x06005980 RID: 22912 RVA: 0x00240D4B File Offset: 0x0023EF4B
	public string GetFuelItem()
	{
		if (this.HasEnginePart())
		{
			return "ammoGasCan";
		}
		return "";
	}

	// Token: 0x06005981 RID: 22913 RVA: 0x00240D60 File Offset: 0x0023EF60
	public float GetFuelPercent()
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			if (this.vehicleParts[i].GetType() == typeof(VPFuelTank))
			{
				float num = ((VPFuelTank)this.vehicleParts[i]).GetFuelLevelPercent();
				if (num > 0.993f)
				{
					num = 1f;
				}
				return num;
			}
		}
		return 0f;
	}

	// Token: 0x06005982 RID: 22914 RVA: 0x00240DD4 File Offset: 0x0023EFD4
	public float GetFuelLevel()
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			if (this.vehicleParts[i].GetType() == typeof(VPFuelTank))
			{
				return ((VPFuelTank)this.vehicleParts[i]).GetFuelLevel();
			}
		}
		return 0f;
	}

	// Token: 0x06005983 RID: 22915 RVA: 0x00240E38 File Offset: 0x0023F038
	public float GetMaxFuelLevel()
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			if (this.vehicleParts[i].GetType() == typeof(VPFuelTank))
			{
				return ((VPFuelTank)this.vehicleParts[i]).GetMaxFuelLevel();
			}
		}
		return 0f;
	}

	// Token: 0x06005984 RID: 22916 RVA: 0x00240E9C File Offset: 0x0023F09C
	public void SetFuelLevel(float _fuelLevel)
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			if (this.vehicleParts[i].GetType() == typeof(VPFuelTank))
			{
				((VPFuelTank)this.vehicleParts[i]).SetFuelLevel(_fuelLevel);
				return;
			}
		}
	}

	// Token: 0x06005985 RID: 22917 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetBatteryLevel()
	{
		return 0f;
	}

	// Token: 0x06005986 RID: 22918 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetBatteryLevel(float _amount)
	{
	}

	// Token: 0x06005987 RID: 22919 RVA: 0x00240EFC File Offset: 0x0023F0FC
	public void AddFuel(float _fuelLevel)
	{
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			if (this.vehicleParts[i].GetType() == typeof(VPFuelTank))
			{
				((VPFuelTank)this.vehicleParts[i]).AddFuel(_fuelLevel);
				return;
			}
		}
	}

	// Token: 0x06005988 RID: 22920 RVA: 0x00240F59 File Offset: 0x0023F159
	public int GetRepairAmountNeeded()
	{
		return this.GetMaxHealth() - this.entity.Health;
	}

	// Token: 0x06005989 RID: 22921 RVA: 0x00240F70 File Offset: 0x0023F170
	public void RepairParts(int _add, float _percent)
	{
		int num = _add + (int)((float)this.GetMaxHealth() * _percent);
		num = Utils.FastMin(num, this.GetRepairAmountNeeded());
		this.entity.Health += num;
	}

	// Token: 0x0600598A RID: 22922 RVA: 0x00240FAA File Offset: 0x0023F1AA
	public bool IsDriveable()
	{
		return this.HasSteering();
	}

	// Token: 0x0600598B RID: 22923 RVA: 0x00240FB2 File Offset: 0x0023F1B2
	public bool HasEnginePart()
	{
		return this.FindPart("engine") != null;
	}

	// Token: 0x0600598C RID: 22924 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetEngineQualityPercent()
	{
		return 0f;
	}

	// Token: 0x0600598D RID: 22925 RVA: 0x00240FC2 File Offset: 0x0023F1C2
	public bool HasStorage()
	{
		return this.FindPart("storage") != null;
	}

	// Token: 0x0600598E RID: 22926 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool HasSteering()
	{
		return true;
	}

	// Token: 0x0600598F RID: 22927 RVA: 0x00240FD2 File Offset: 0x0023F1D2
	public bool IsSteeringBroken()
	{
		return !this.HasSteering() || this.FindPart("handlebars").IsBroken();
	}

	// Token: 0x06005990 RID: 22928 RVA: 0x00240FEE File Offset: 0x0023F1EE
	public bool HasLock()
	{
		return this.FindPart("lock") != null;
	}

	// Token: 0x06005991 RID: 22929 RVA: 0x00240FFE File Offset: 0x0023F1FE
	public bool IsLockBroken()
	{
		return this.FindPart("lock").GetHealthPercentage() == 0f;
	}

	// Token: 0x06005992 RID: 22930 RVA: 0x00241017 File Offset: 0x0023F217
	public string GetHornSoundName()
	{
		return this.Properties.GetString("hornSound");
	}

	// Token: 0x06005993 RID: 22931 RVA: 0x00241029 File Offset: 0x0023F229
	public bool HasHorn()
	{
		return this.GetHornSoundName().Length > 0;
	}

	// Token: 0x06005994 RID: 22932 RVA: 0x0024103C File Offset: 0x0023F23C
	public List<IKController.Target> GetIKTargets(int slot)
	{
		List<IKController.Target> list = new List<IKController.Target>();
		if (slot == 0)
		{
			VehiclePart vehiclePart = this.FindPart("handlebars");
			if (vehiclePart != null)
			{
				list.AddRange(vehiclePart.ikTargets);
			}
			VehiclePart vehiclePart2 = this.FindPart("pedals");
			if (vehiclePart2 != null)
			{
				list.AddRange(vehiclePart2.ikTargets);
			}
		}
		VehiclePart vehiclePart3 = this.FindPart("seat" + slot.ToString());
		if (vehiclePart3 != null && vehiclePart3.ikTargets != null)
		{
			list.AddRange(vehiclePart3.ikTargets);
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list;
	}

	// Token: 0x06005995 RID: 22933 RVA: 0x002410C4 File Offset: 0x0023F2C4
	public List<string> GetParticleTransformPaths()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.vehicleParts.Count; i++)
		{
			string property = this.vehicleParts[i].GetProperty("particle_transform");
			if (property != string.Empty)
			{
				list.Add(property);
			}
		}
		return list;
	}

	// Token: 0x06005996 RID: 22934 RVA: 0x00241119 File Offset: 0x0023F319
	public int GetVehicleQuality()
	{
		return (int)this.itemValue.Quality;
	}

	// Token: 0x06005997 RID: 22935 RVA: 0x00241128 File Offset: 0x0023F328
	public int GetHealth()
	{
		int health = this.entity.Health;
		if (health <= 1)
		{
			return 0;
		}
		return health;
	}

	// Token: 0x06005998 RID: 22936 RVA: 0x00241148 File Offset: 0x0023F348
	public int GetMaxHealth()
	{
		return this.entity.GetMaxHealth();
	}

	// Token: 0x06005999 RID: 22937 RVA: 0x00241155 File Offset: 0x0023F355
	public float GetHealthPercent()
	{
		return (float)this.GetHealth() / (float)this.entity.GetMaxHealth();
	}

	// Token: 0x0600599A RID: 22938 RVA: 0x0024116B File Offset: 0x0023F36B
	public float GetPlayerDamagePercent()
	{
		return 0.1f;
	}

	// Token: 0x0600599B RID: 22939 RVA: 0x001315C3 File Offset: 0x0012F7C3
	public float GetNoise()
	{
		return 0.5f;
	}

	// Token: 0x0600599C RID: 22940 RVA: 0x00241174 File Offset: 0x0023F374
	public void SetLocked(bool isLocked, EntityPlayerLocal player)
	{
		if (player == null)
		{
			return;
		}
		if (isLocked)
		{
			this.entity.SetOwner(PlatformManager.InternalLocalUserIdentifier);
			this.entity.isLocked = true;
			this.PasswordHash = 0;
			return;
		}
		this.entity.isLocked = false;
		this.PasswordHash = 0;
	}

	// Token: 0x0400444A RID: 17482
	public static Dictionary<string, DynamicProperties> PropertyMap;

	// Token: 0x0400444B RID: 17483
	public DynamicProperties Properties;

	// Token: 0x0400444C RID: 17484
	public ItemValue itemValue;

	// Token: 0x0400444D RID: 17485
	public List<PlatformUserIdentifierAbs> AllowedUsers;

	// Token: 0x0400444E RID: 17486
	public int PasswordHash;

	// Token: 0x0400444F RID: 17487
	public EntityVehicle entity;

	// Token: 0x04004450 RID: 17488
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs m_ownerId;

	// Token: 0x04004451 RID: 17489
	[PublicizedFrom(EAccessModifier.Private)]
	public string vehicleName;

	// Token: 0x04004452 RID: 17490
	[PublicizedFrom(EAccessModifier.Private)]
	public List<VehiclePart> vehicleParts;

	// Token: 0x04004453 RID: 17491
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform meshT;

	// Token: 0x04004454 RID: 17492
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 cameraDistance = new Vector2(1f, 1f);

	// Token: 0x04004455 RID: 17493
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 cameraTurnRate = new Vector2(1f, 1f);

	// Token: 0x04004456 RID: 17494
	[PublicizedFrom(EAccessModifier.Private)]
	public float upAngleMax = 70f;

	// Token: 0x04004457 RID: 17495
	[PublicizedFrom(EAccessModifier.Private)]
	public float upForce = 5f;

	// Token: 0x04004458 RID: 17496
	[PublicizedFrom(EAccessModifier.Private)]
	public float steerAngleMax = 25f;

	// Token: 0x04004459 RID: 17497
	[PublicizedFrom(EAccessModifier.Private)]
	public float steerRate = 130f;

	// Token: 0x0400445A RID: 17498
	[PublicizedFrom(EAccessModifier.Private)]
	public float steerCenteringRate = 90f;

	// Token: 0x0400445B RID: 17499
	[PublicizedFrom(EAccessModifier.Private)]
	public float tiltAngleMax = 20f;

	// Token: 0x0400445C RID: 17500
	[PublicizedFrom(EAccessModifier.Private)]
	public float tiltThreshold = 3f;

	// Token: 0x0400445D RID: 17501
	[PublicizedFrom(EAccessModifier.Private)]
	public float tiltDampening = 0.22f;

	// Token: 0x0400445E RID: 17502
	[PublicizedFrom(EAccessModifier.Private)]
	public float tiltDampenThreshold = 8f;

	// Token: 0x0400445F RID: 17503
	[PublicizedFrom(EAccessModifier.Private)]
	public float tiltUpForce = 5f;

	// Token: 0x04004460 RID: 17504
	public float MotorTorqueForward = 300f;

	// Token: 0x04004461 RID: 17505
	public float MotorTorqueBackward = 300f;

	// Token: 0x04004462 RID: 17506
	public float MotorTorqueTurboForward;

	// Token: 0x04004463 RID: 17507
	public float MotorTorqueTurboBackward;

	// Token: 0x04004464 RID: 17508
	public float VelocityMaxForward = 10f;

	// Token: 0x04004465 RID: 17509
	public float VelocityMaxBackward = 10f;

	// Token: 0x04004466 RID: 17510
	public float VelocityMaxTurboForward = 10f;

	// Token: 0x04004467 RID: 17511
	public float VelocityMaxTurboBackward = 10f;

	// Token: 0x04004468 RID: 17512
	[PublicizedFrom(EAccessModifier.Private)]
	public float brakeTorque = 4000f;

	// Token: 0x04004469 RID: 17513
	public bool CanTurbo = true;

	// Token: 0x0400446A RID: 17514
	public bool IsTurbo;

	// Token: 0x0400446B RID: 17515
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 hopForce;

	// Token: 0x0400446C RID: 17516
	[PublicizedFrom(EAccessModifier.Private)]
	public float unstickForce = 1f;

	// Token: 0x0400446D RID: 17517
	public float AirDragVelScale = 0.997f;

	// Token: 0x0400446E RID: 17518
	public float AirDragAngVelScale = 1f;

	// Token: 0x0400446F RID: 17519
	public float WaterDragY;

	// Token: 0x04004470 RID: 17520
	public float WaterDragVelScale = 1f;

	// Token: 0x04004471 RID: 17521
	public float WaterDragVelMaxScale = 1f;

	// Token: 0x04004472 RID: 17522
	public float WaterLiftY;

	// Token: 0x04004473 RID: 17523
	public float WaterLiftDepth;

	// Token: 0x04004474 RID: 17524
	public float WaterLiftForce;

	// Token: 0x04004475 RID: 17525
	public float WheelPtlScale;

	// Token: 0x04004476 RID: 17526
	public float CurrentForwardVelocity;

	// Token: 0x04004477 RID: 17527
	public bool CurrentIsAccel;

	// Token: 0x04004478 RID: 17528
	public bool CurrentIsBreak;

	// Token: 0x04004479 RID: 17529
	public float CurrentMotorTorquePercent;

	// Token: 0x0400447A RID: 17530
	public float CurrentSteeringPercent;

	// Token: 0x0400447B RID: 17531
	public Vector3 CurrentVelocity;

	// Token: 0x0400447C RID: 17532
	public string RecipeName;

	// Token: 0x0400447D RID: 17533
	public Material mainEmissiveMat;

	// Token: 0x0400447E RID: 17534
	public float EffectEntityDamagePer;

	// Token: 0x0400447F RID: 17535
	public float EffectBlockDamagePer;

	// Token: 0x04004480 RID: 17536
	public float EffectSelfDamagePer;

	// Token: 0x04004481 RID: 17537
	public float EffectStrongSelfDamagePer;

	// Token: 0x04004482 RID: 17538
	public float EffectLightIntensity;

	// Token: 0x04004483 RID: 17539
	public float EffectFuelMaxPer = 1f;

	// Token: 0x04004484 RID: 17540
	public float EffectFuelUsePer;

	// Token: 0x04004485 RID: 17541
	public float EffectMotorTorquePer;

	// Token: 0x04004486 RID: 17542
	public float EffectVelocityMaxPer;

	// Token: 0x04004487 RID: 17543
	[PublicizedFrom(EAccessModifier.Private)]
	public float effectUpdateDelay;

	// Token: 0x04004488 RID: 17544
	public FastTags<TagGroup.Global> ModTags;

	// Token: 0x02000B3E RID: 2878
	public enum Event
	{
		// Token: 0x0400448A RID: 17546
		Start,
		// Token: 0x0400448B RID: 17547
		Started,
		// Token: 0x0400448C RID: 17548
		Stop,
		// Token: 0x0400448D RID: 17549
		Stopped,
		// Token: 0x0400448E RID: 17550
		SimulationUpdate,
		// Token: 0x0400448F RID: 17551
		HealthChanged
	}
}
