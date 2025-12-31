using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class DismembermentManager
{
	// Token: 0x060004A8 RID: 1192 RVA: 0x00021567 File Offset: 0x0001F767
	public static string GetAssetBundlePath(string prefabPath)
	{
		return "@:Entities/Zombies/" + prefabPath + ".prefab";
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0002157C File Offset: 0x0001F77C
	public static bool IsDefaultGib(string matName)
	{
		for (int i = 0; i < DismembermentManager.DefaultBundleGibs.Length; i++)
		{
			if (DismembermentManager.DefaultBundleGibs[i] == matName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060004AA RID: 1194 RVA: 0x000215B0 File Offset: 0x0001F7B0
	public Material GibCapsMaterial
	{
		get
		{
			if (!this.zombieGibCapsMaterial)
			{
				Material material = DataLoader.LoadAsset<Material>("@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps.mat", false);
				this.zombieGibCapsMaterial = UnityEngine.Object.Instantiate<Material>(material);
				this.zombieGibCapsMaterial.name = material.name + "(global)";
				if (DismembermentManager.DebugLogEnabled)
				{
					Log.Out("{0} material: {1}", new object[]
					{
						this.zombieGibCapsMaterial ? "load" : "load failed",
						"@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps.mat"
					});
				}
			}
			return this.zombieGibCapsMaterial;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060004AB RID: 1195 RVA: 0x00021640 File Offset: 0x0001F840
	public Material GibCapsRadMaterial
	{
		get
		{
			if (!this.zombieGibCapsMaterialRadiated)
			{
				Material material = DataLoader.LoadAsset<Material>("@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps_IsRadiated.mat", false);
				this.zombieGibCapsMaterialRadiated = UnityEngine.Object.Instantiate<Material>(material);
				this.zombieGibCapsMaterialRadiated.name = material.name + "(global)";
				if (DismembermentManager.DebugLogEnabled)
				{
					Log.Out("{0} material: {1}", new object[]
					{
						this.zombieGibCapsMaterialRadiated ? "load" : "load failed",
						"@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps_IsRadiated.mat"
					});
				}
			}
			return this.zombieGibCapsMaterialRadiated;
		}
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x000216CE File Offset: 0x0001F8CE
	public static Texture GetShaderTexture(Material _mat)
	{
		if (_mat.HasTexture("_ZombieColor"))
		{
			return _mat.GetTexture("_ZombieColor");
		}
		if (_mat.HasTexture("_Albedo"))
		{
			return _mat.GetTexture("_Albedo");
		}
		return null;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00021703 File Offset: 0x0001F903
	public static void SetShaderTexture(Material _mat, Texture _altColor)
	{
		if (_mat.HasTexture("_ZombieColor"))
		{
			_mat.SetTexture("_ZombieColor", _altColor);
		}
		if (_mat.HasTexture("_Albedo"))
		{
			_mat.SetTexture("_Albedo", _altColor);
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060004AE RID: 1198 RVA: 0x00021737 File Offset: 0x0001F937
	public static DismembermentManager Instance
	{
		get
		{
			return DismembermentManager.instance;
		}
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00021751 File Offset: 0x0001F951
	public static void Init()
	{
		DismembermentManager.instance = new DismembermentManager();
		if (DismembermentManager.DebugLogEnabled)
		{
			Log.Out("DismembermentManager Init");
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00021770 File Offset: 0x0001F970
	public static void Cleanup()
	{
		if (DismembermentManager.instance != null)
		{
			List<DismemberedPart> list = DismembermentManager.instance.parts;
			for (int i = 0; i < list.Count; i++)
			{
				list[i].CleanupDetached();
			}
			list.Clear();
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x000217B2 File Offset: 0x0001F9B2
	public void AddPart(DismemberedPart part)
	{
		this.parts.Add(part);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x000217C0 File Offset: 0x0001F9C0
	public void Update()
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			DismemberedPart dismemberedPart = this.parts[i];
			dismemberedPart.Update();
			if (dismemberedPart.ReadyForCleanup)
			{
				dismemberedPart.CleanupDetached();
				this.parts.RemoveAt(i);
				i--;
			}
		}
		if (this.parts.Count > 25)
		{
			int num = this.parts.Count - 25;
			for (int j = 0; j < num; j++)
			{
				this.parts[j].ReadyForCleanup = true;
			}
		}
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00021850 File Offset: 0x0001FA50
	public static float GetImpactForce(ItemClass ic, float strength)
	{
		if (ic != null)
		{
			if (ic.HasAnyTags(DismembermentManager.shotgunTags))
			{
				return 1.5f;
			}
			if (ic.HasAnyTags(DismembermentManager.sledgeTags))
			{
				return Mathf.Clamp(1f + Mathf.Abs(strength), 1f, 1.5f);
			}
			if (ic.HasAnyTags(DismembermentManager.knifeTags))
			{
				return Mathf.Abs(1f * strength) * 0.67f;
			}
		}
		return 1f;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x000218C4 File Offset: 0x0001FAC4
	public static EnumBodyPartHit GetBodyPartHit(uint bodyDamageFlag)
	{
		if (bodyDamageFlag == 1U)
		{
			return EnumBodyPartHit.Head;
		}
		if (bodyDamageFlag == 2U)
		{
			return EnumBodyPartHit.LeftUpperArm;
		}
		if (bodyDamageFlag == 4U)
		{
			return EnumBodyPartHit.LeftLowerArm;
		}
		if (bodyDamageFlag == 8U)
		{
			return EnumBodyPartHit.RightUpperArm;
		}
		if (bodyDamageFlag == 16U)
		{
			return EnumBodyPartHit.RightLowerArm;
		}
		if (bodyDamageFlag == 32U)
		{
			return EnumBodyPartHit.LeftUpperLeg;
		}
		if (bodyDamageFlag == 64U)
		{
			return EnumBodyPartHit.LeftLowerLeg;
		}
		if (bodyDamageFlag == 128U)
		{
			return EnumBodyPartHit.RightUpperLeg;
		}
		if (bodyDamageFlag == 256U)
		{
			return EnumBodyPartHit.RightLowerLeg;
		}
		return EnumBodyPartHit.None;
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00021924 File Offset: 0x0001FB24
	public static EnumBodyPartHit GetBodyPartHit(string _propKey)
	{
		if (_propKey.ContainsCaseInsensitive("L_HeadGore"))
		{
			return EnumBodyPartHit.Head;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftUpperArmGore"))
		{
			return EnumBodyPartHit.LeftUpperArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftLowerArmGore"))
		{
			return EnumBodyPartHit.LeftLowerArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightUpperArmGore"))
		{
			return EnumBodyPartHit.RightUpperArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightLowerArmGore"))
		{
			return EnumBodyPartHit.RightLowerArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftUpperLegGore"))
		{
			return EnumBodyPartHit.LeftUpperLeg;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftLowerLegGore"))
		{
			return EnumBodyPartHit.LeftLowerLeg;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightUpperLegGore"))
		{
			return EnumBodyPartHit.RightUpperLeg;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightLowerLegGore"))
		{
			return EnumBodyPartHit.RightLowerLeg;
		}
		return EnumBodyPartHit.None;
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x000219C8 File Offset: 0x0001FBC8
	public static string GetDamageTag(EnumDamageTypes _damageType, bool lastHitRanged)
	{
		if (_damageType == EnumDamageTypes.Piercing && lastHitRanged)
		{
			return "blunt";
		}
		switch (_damageType)
		{
		case EnumDamageTypes.Piercing:
		case EnumDamageTypes.Slashing:
			return "blade";
		case EnumDamageTypes.Bashing:
		case EnumDamageTypes.Crushing:
			return "blunt";
		case EnumDamageTypes.Heat:
			return "blunt";
		}
		return null;
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00021A18 File Offset: 0x0001FC18
	public static DismemberedPartData DismemberPart(uint bodyDamageFlag, EnumDamageTypes damageType, EntityAlive _entity, bool isBiped, bool useLegacy = false)
	{
		return DismembermentManager.dismemberPart(DismembermentManager.GetBodyPartHit(bodyDamageFlag), damageType, _entity, isBiped, useLegacy);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00021A2C File Offset: 0x0001FC2C
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismemberedPartData dismemberPart(EnumBodyPartHit partHit, EnumDamageTypes damageType, EntityAlive _entity, bool isBiped, bool useLegacy = false)
	{
		if (!DismembermentManager.hasDismemberedPart(partHit, isBiped))
		{
			return null;
		}
		DismemberedPartData dismemberedPartData = new DismemberedPartData();
		string[] dismemberedPart = DismembermentManager.getDismemberedPart(partHit, isBiped);
		dismemberedPartData.propertyKey = dismemberedPart[0];
		dismemberedPartData.prefabPath = dismemberedPart[1];
		dismemberedPartData.damageTypeKey = DismembermentManager.GetDamageTag(damageType, _entity.lastHitRanged);
		DynamicProperties properties = _entity.EntityClass.Properties;
		if (useLegacy || !properties.Contains(dismemberedPartData.propertyKey) || string.IsNullOrEmpty(properties.Values[dismemberedPartData.propertyKey]))
		{
			return dismemberedPartData;
		}
		if (properties.Data.ContainsKey(dismemberedPartData.propertyKey))
		{
			string[] array = properties.Values[dismemberedPartData.propertyKey].Split(';', StringSplitOptions.None);
			string[] array2 = properties.Data[dismemberedPartData.propertyKey].Split(';', StringSplitOptions.None);
			if (array[0].ContainsCaseInsensitive("linked"))
			{
				string v = array2[0].Replace("target=", "");
				array = properties.Values[v].Split(';', StringSplitOptions.None);
				array2 = properties.Data[v].Split(';', StringSplitOptions.None);
				dismemberedPartData.isLinked = true;
			}
			DismemberedPartData dismemberedPartData2 = DismembermentManager.readRandomPart(array, array2, dismemberedPartData.damageTypeKey);
			if (dismemberedPartData2 == null && dismemberedPartData.damageTypeKey == "blunt" && !dismemberedPartData.prefabPath.ContainsCaseInsensitive("blunt"))
			{
				DismemberedPartData dismemberedPartData3 = DismembermentManager.readRandomPart(array, array2, "blade");
				if (dismemberedPartData3 != null && (dismemberedPartData3.useMask || dismemberedPartData3.scaleOutLimb))
				{
					dismemberedPartData2 = dismemberedPartData3;
				}
			}
			if (dismemberedPartData2 != null)
			{
				if (!dismemberedPartData2.isLinked && dismemberedPartData2.Invalid)
				{
					return dismemberedPartData;
				}
				if (!string.IsNullOrEmpty(dismemberedPartData2.prefabPath))
				{
					dismemberedPartData.prefabPath = dismemberedPartData2.prefabPath;
				}
				dismemberedPartData.scale = dismemberedPartData2.scale;
				if (dismemberedPartData2.hasRotOffset)
				{
					dismemberedPartData.SetRot(dismemberedPartData2.rot);
				}
				dismemberedPartData.targetBone = dismemberedPartData2.targetBone;
				dismemberedPartData.attachToParent = dismemberedPartData2.attachToParent;
				dismemberedPartData.particlePaths = dismemberedPartData2.particlePaths;
				dismemberedPartData.isDetachable = dismemberedPartData2.isDetachable;
				dismemberedPartData.offset = dismemberedPartData2.offset;
				dismemberedPartData.useMask = dismemberedPartData2.useMask;
				dismemberedPartData.scaleOutLimb = dismemberedPartData2.scaleOutLimb;
				dismemberedPartData.solTarget = dismemberedPartData2.solTarget;
				dismemberedPartData.solScale = dismemberedPartData2.solScale;
				dismemberedPartData.hasSolScale = dismemberedPartData2.hasSolScale;
				dismemberedPartData.childTargetObj = dismemberedPartData2.childTargetObj;
				dismemberedPartData.insertBoneObj = dismemberedPartData2.insertBoneObj;
				if (properties.Contains("DismemberMaterial"))
				{
					dismemberedPartData.dismemberMatPath = properties.Values["DismemberMaterial"];
				}
			}
		}
		if (DismembermentManager.DebugLogEnabled)
		{
			Log.Out("[{0}.DismemberPart] - entityClass: {1}{2}", new object[]
			{
				"DismembermentManager",
				EntityClass.list[_entity.entityClass].entityClassName,
				dismemberedPartData.Log()
			});
		}
		return dismemberedPartData;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00021D0C File Offset: 0x0001FF0C
	public static void ActivateDetachable(Transform rootT, string targetPart)
	{
		Transform transform = rootT;
		Transform transform2 = transform.Find("Physics");
		if (transform2)
		{
			transform = transform2;
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			bool active = child.name.ContainsCaseInsensitive(targetPart);
			child.gameObject.SetActive(active);
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00021D64 File Offset: 0x0001FF64
	public static DismemberedPartData GetPartData(EntityAlive _entity)
	{
		string[] dismemberedPart = DismembermentManager.getDismemberedPart(_entity.bodyDamage.bodyPartHit, true);
		if (dismemberedPart != null)
		{
			string text = dismemberedPart[0];
			DynamicProperties properties = _entity.EntityClass.Properties;
			if (properties.Data.ContainsKey(text))
			{
				string[] array = properties.Values[text].Split(';', StringSplitOptions.None);
				string a = array[0];
				string[] array2 = properties.Data[text].Split(';', StringSplitOptions.None);
				string text2 = array2[0].Replace("target=", "");
				bool flag = a.ContainsCaseInsensitive("linked");
				if (flag)
				{
					array = properties.Values[text2].Split(';', StringSplitOptions.None);
					array2 = properties.Data[text2].Split(';', StringSplitOptions.None);
				}
				DismemberedPartData dismemberedPartData = DismembermentManager.readPart(array2);
				if (dismemberedPartData != null)
				{
					dismemberedPartData.propertyKey = text2.Trim();
					dismemberedPartData.prefabPath = array[0].Trim();
					dismemberedPartData.isLinked = flag;
					return dismemberedPartData;
				}
			}
		}
		return null;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00021E61 File Offset: 0x00020061
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool hasDismemberedPart(EnumBodyPartHit part, bool isBiped = true)
	{
		if (isBiped)
		{
			return DismembermentManager.BipedDismemberments.ContainsKey(part);
		}
		return DismembermentManager.QuadrupedDismemberments.ContainsKey(part);
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00021E80 File Offset: 0x00020080
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] getDismemberedPart(EnumBodyPartHit part, bool isBiped = true)
	{
		string[] result = null;
		if (isBiped)
		{
			DismembermentManager.BipedDismemberments.TryGetValue(part, out result);
		}
		else
		{
			DismembermentManager.QuadrupedDismemberments.TryGetValue(part, out result);
		}
		return result;
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00021EB4 File Offset: 0x000200B4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void readString(string rawString, DismemberedPartData data)
	{
		string[] array = rawString.Split('=', StringSplitOptions.None);
		string text = array[0].Trim();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1361572173U)
		{
			if (num <= 845187144U)
			{
				if (num != 91623701U)
				{
					if (num != 660706664U)
					{
						if (num == 845187144U)
						{
							if (text == "target")
							{
								data.targetBone = array[1].Trim();
								return;
							}
						}
					}
					else if (text == "atp")
					{
						bool.TryParse(array[1], out data.attachToParent);
						return;
					}
				}
				else if (text == "solscale")
				{
					string[] array2 = array[1].Split(',', StringSplitOptions.None);
					float.TryParse(array2[0], out data.solScale.x);
					float.TryParse(array2[1], out data.solScale.y);
					float.TryParse(array2[2], out data.solScale.z);
					data.hasSolScale = true;
					return;
				}
			}
			else if (num <= 1213057714U)
			{
				if (num != 1158553358U)
				{
					if (num == 1213057714U)
					{
						if (text == "detach")
						{
							bool.TryParse(array[1], out data.isDetachable);
							return;
						}
					}
				}
				else if (text == "rot")
				{
					string[] array3 = array[1].Split(',', StringSplitOptions.None);
					if (array3.Length != 3)
					{
						return;
					}
					Vector3 zero = Vector3.zero;
					float.TryParse(array3[0], out zero.x);
					float.TryParse(array3[1], out zero.y);
					float.TryParse(array3[2], out zero.z);
					if (zero != Vector3.zero)
					{
						data.SetRot(zero);
						return;
					}
					return;
				}
			}
			else if (num != 1325231910U)
			{
				if (num == 1361572173U)
				{
					if (text == "type")
					{
						string a = array[1].Trim();
						if (a == "blunt" || a == "blade" || a == "bullet" || a == "explosive")
						{
							data.damageTypeKey = array[1].Trim();
							return;
						}
						return;
					}
				}
			}
			else if (text == "oset")
			{
				string[] array4 = array[1].Split(',', StringSplitOptions.None);
				float.TryParse(array4[0], out data.offset.x);
				float.TryParse(array4[1], out data.offset.y);
				float.TryParse(array4[2], out data.offset.z);
				return;
			}
		}
		else if (num <= 2531611380U)
		{
			if (num != 2095122494U)
			{
				if (num != 2190941297U)
				{
					if (num == 2531611380U)
					{
						if (text == "soltarget")
						{
							data.solTarget = array[1].Trim();
							return;
						}
					}
				}
				else if (text == "scale")
				{
					string[] array5 = array[1].Split(',', StringSplitOptions.None);
					float.TryParse(array5[0], out data.scale.x);
					float.TryParse(array5[1], out data.scale.y);
					float.TryParse(array5[2], out data.scale.z);
					return;
				}
			}
			else if (text == "ico")
			{
				data.childTargetObj = array[1].Trim();
				return;
			}
		}
		else if (num <= 3740252708U)
		{
			if (num != 2631859207U)
			{
				if (num == 3740252708U)
				{
					if (text == "particles")
					{
						data.particlePaths = array[1].Split(',', StringSplitOptions.None);
						return;
					}
				}
			}
			else if (text == "ibo")
			{
				data.insertBoneObj = array[1].Trim();
				return;
			}
		}
		else if (num != 3795205537U)
		{
			if (num == 3883353449U)
			{
				if (text == "mask")
				{
					bool.TryParse(array[1], out data.useMask);
					return;
				}
			}
		}
		else if (text == "sol")
		{
			bool.TryParse(array[1], out data.scaleOutLimb);
			return;
		}
		data.Invalid = true;
		if (DismembermentManager.DebugLogEnabled)
		{
			Log.Warning("[{0}.readString] entityclasses.xml unknown key:{1} in raw:{2}", new object[]
			{
				"DismembermentManager",
				text,
				rawString
			});
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x00022330 File Offset: 0x00020530
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismemberedPartData readRandomPart(string[] prefabs, string[] data, string tag)
	{
		List<DismemberedPartData> list = new List<DismemberedPartData>();
		for (int i = 0; i < data.Length; i++)
		{
			DismemberedPartData dismemberedPartData = new DismemberedPartData();
			if (i < prefabs.Length)
			{
				string prefabPath = prefabs[i].Trim();
				dismemberedPartData.prefabPath = prefabPath;
			}
			if (data[i].Contains('+'.ToString()))
			{
				string[] array = data[i].Split('+', StringSplitOptions.None);
				for (int j = 0; j < array.Length; j++)
				{
					DismembermentManager.readString(array[j], dismemberedPartData);
				}
				if (!string.IsNullOrEmpty(dismemberedPartData.damageTypeKey) && dismemberedPartData.damageTypeKey == tag)
				{
					list.Add(dismemberedPartData);
				}
			}
			else
			{
				DismembermentManager.readString(data[i], dismemberedPartData);
				if (!string.IsNullOrEmpty(dismemberedPartData.damageTypeKey) && dismemberedPartData.damageTypeKey == tag)
				{
					list.Add(dismemberedPartData);
				}
			}
		}
		if (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		return null;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x00022424 File Offset: 0x00020624
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismemberedPartData readPart(string[] data)
	{
		List<DismemberedPartData> list = new List<DismemberedPartData>();
		for (int i = 0; i < data.Length; i++)
		{
			DismemberedPartData dismemberedPartData = new DismemberedPartData();
			string text = data[i];
			if (text.Contains('+'.ToString()))
			{
				string[] array = text.Split('+', StringSplitOptions.None);
				for (int j = 0; j < array.Length; j++)
				{
					DismembermentManager.readString(array[j], dismemberedPartData);
				}
				list.Add(dismemberedPartData);
			}
			else
			{
				DismembermentManager.readString(text, dismemberedPartData);
				list.Add(dismemberedPartData);
			}
		}
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x000224B4 File Offset: 0x000206B4
	public static void SpawnParticleEffect(ParticleEffect _pe, int _entityId = -1)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!GameManager.IsDedicatedServer)
			{
				GameManager.Instance.SpawnParticleEffectClient(_pe, _entityId, false, true);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, false), false, -1, _entityId, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, false), false);
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0002252C File Offset: 0x0002072C
	public static void AddDebugArmObjects(Transform partT, Transform parentT)
	{
		if (partT && parentT && partT.name.ContainsCaseInsensitive("arm"))
		{
			GameObject gameObject = DataLoader.LoadAsset<GameObject>("@:Entities/Zombies/Gibs/Debug/debugAxisObj.prefab", false);
			if (gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				gameObject2.transform.SetParent(parentT);
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				if (partT.name.ContainsCaseInsensitive("right"))
				{
					gameObject2.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
				}
				gameObject2.transform.localScale = Vector3.one * 0.5f;
				Transform transform = parentT.FindRecursive("rot");
				if (transform)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
					gameObject3.transform.SetParent(transform);
					gameObject3.transform.localPosition = Vector3.zero;
					gameObject3.transform.localRotation = Quaternion.identity;
					gameObject3.transform.localScale = Vector3.one * 0.33f;
					gameObject3.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
				}
			}
		}
	}

	// Token: 0x040004F5 RID: 1269
	public const string cClassName = "DismembermentManager";

	// Token: 0x040004F6 RID: 1270
	public const string cManagedLimbsParentName = "DismemberedLimbs";

	// Token: 0x040004F7 RID: 1271
	public static bool DebugLogEnabled;

	// Token: 0x040004F8 RID: 1272
	public static bool DebugShowArmRotations;

	// Token: 0x040004F9 RID: 1273
	public static bool DebugDismemberExplosions;

	// Token: 0x040004FA RID: 1274
	public static bool DebugBulletTime;

	// Token: 0x040004FB RID: 1275
	public static bool DebugBloodParticles;

	// Token: 0x040004FC RID: 1276
	public static bool DebugDontCreateParts;

	// Token: 0x040004FD RID: 1277
	public static EnumBodyPartHit DebugBodyPartHit;

	// Token: 0x040004FE RID: 1278
	public static bool DebugUseLegacy;

	// Token: 0x040004FF RID: 1279
	public static bool DebugExplosiveCleanup;

	// Token: 0x04000500 RID: 1280
	public const string DestroyedCapRoot = "pos";

	// Token: 0x04000501 RID: 1281
	public const string PhysicsRootName = "Physics";

	// Token: 0x04000502 RID: 1282
	public const string DetachableRootName = "Detachable";

	// Token: 0x04000503 RID: 1283
	public const string cSubTagHeadAccessories = "HeadAccessories";

	// Token: 0x04000504 RID: 1284
	public const string DetachableArmName = "HalfArm";

	// Token: 0x04000505 RID: 1285
	public const string DetachableLegName = "HalfLeg";

	// Token: 0x04000506 RID: 1286
	public const string ZombieSkinPrefix = "HD_";

	// Token: 0x04000507 RID: 1287
	public const string MatPropRadiated = "_IsRadiated";

	// Token: 0x04000508 RID: 1288
	public const string MatPropIrradiated = "_Irradiated";

	// Token: 0x04000509 RID: 1289
	public const string MatPropFade = "_Fade";

	// Token: 0x0400050A RID: 1290
	public static FastTags<TagGroup.Global> radiatedTag = FastTags<TagGroup.Global>.Parse("radiated");

	// Token: 0x0400050B RID: 1291
	public const string cCensorGoreSearch = "_CGore";

	// Token: 0x0400050C RID: 1292
	public static readonly List<string> BluntCensors = new List<string>
	{
		"zombieLab",
		"zombieUtilityWorker"
	};

	// Token: 0x0400050D RID: 1293
	public const string cAssetBundleZombies = "@:Entities/Zombies/";

	// Token: 0x0400050E RID: 1294
	public const string cAssetBundleSearchName = "Dismemberment";

	// Token: 0x0400050F RID: 1295
	public const string cAssetBundleFolder = "@:Entities/Zombies/";

	// Token: 0x04000510 RID: 1296
	public const string cPrefabExt = ".prefab";

	// Token: 0x04000511 RID: 1297
	public const string cLOD0 = "LOD0";

	// Token: 0x04000512 RID: 1298
	public const string cLOD1 = "LOD1";

	// Token: 0x04000513 RID: 1299
	public const string cLOD2 = "LOD2";

	// Token: 0x04000514 RID: 1300
	public const string MatPropLeftLowerLeg = "_LeftLowerLeg";

	// Token: 0x04000515 RID: 1301
	public const string MatPropRightLowerLeg = "_RightLowerLeg";

	// Token: 0x04000516 RID: 1302
	public const string MatPropLeftUpperLeg = "_LeftUpperLeg";

	// Token: 0x04000517 RID: 1303
	public const string MatPropRightUpperLeg = "_RightUpperLeg";

	// Token: 0x04000518 RID: 1304
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cAssetBundleGibMats = "Common/Gibs/Materials";

	// Token: 0x04000519 RID: 1305
	[PublicizedFrom(EAccessModifier.Private)]
	public const string CAssetBundleDefaultGib = "gib_dismemberment";

	// Token: 0x0400051A RID: 1306
	[PublicizedFrom(EAccessModifier.Private)]
	public const string CAssetBundleDefaultGibBlood = "gib_bloodcap";

	// Token: 0x0400051B RID: 1307
	public const string CAssetBundleDefaultGibChunk = "ZombieGibs_caps";

	// Token: 0x0400051C RID: 1308
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDismemberMatXmlProp = "DismemberMaterial";

	// Token: 0x0400051D RID: 1309
	public static string[] DefaultBundleGibs = new string[]
	{
		"gib_dismemberment",
		"gib_bloodcap",
		"ZombieGibs_caps"
	};

	// Token: 0x0400051E RID: 1310
	public const string cGlobalMatName = "(global)";

	// Token: 0x0400051F RID: 1311
	public const string cLocalMatName = "(local)";

	// Token: 0x04000520 RID: 1312
	public const string cInstanceMatName = "(Instance)";

	// Token: 0x04000521 RID: 1313
	public const string cMatExt = ".mat";

	// Token: 0x04000522 RID: 1314
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cGibCapsMatPath = "@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps.mat";

	// Token: 0x04000523 RID: 1315
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cGibCapsMatRadPath = "@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps_IsRadiated.mat";

	// Token: 0x04000524 RID: 1316
	[PublicizedFrom(EAccessModifier.Private)]
	public Material zombieGibCapsMaterial;

	// Token: 0x04000525 RID: 1317
	[PublicizedFrom(EAccessModifier.Private)]
	public Material zombieGibCapsMaterialRadiated;

	// Token: 0x04000526 RID: 1318
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDefaultMatBaseTex = "_ZombieColor";

	// Token: 0x04000527 RID: 1319
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDefaultMatBaseTexAlt = "_Albedo";

	// Token: 0x04000528 RID: 1320
	public const float cDefaultDetachLimbLifeTime = 10f;

	// Token: 0x04000529 RID: 1321
	public const int cDefaultDetachLimbMax = 25;

	// Token: 0x0400052A RID: 1322
	public const int cDefaultDetachLimbCleanupCount = 5;

	// Token: 0x0400052B RID: 1323
	public const int cMaxLimbsFromExplosiveDeath = 3;

	// Token: 0x0400052C RID: 1324
	public List<DismemberedPart> parts = new List<DismemberedPart>();

	// Token: 0x0400052D RID: 1325
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismembermentManager instance;

	// Token: 0x0400052E RID: 1326
	public const string cDynamicGore = "DynamicGore";

	// Token: 0x0400052F RID: 1327
	public static FastTags<TagGroup.Global> rangedTags = FastTags<TagGroup.Global>.Parse("ranged");

	// Token: 0x04000530 RID: 1328
	public static FastTags<TagGroup.Global> launcherTags = FastTags<TagGroup.Global>.Parse("launcher");

	// Token: 0x04000531 RID: 1329
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> shotgunTags = FastTags<TagGroup.Global>.Parse("shotgun");

	// Token: 0x04000532 RID: 1330
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> sledgeTags = FastTags<TagGroup.Global>.Parse("sledge");

	// Token: 0x04000533 RID: 1331
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> knifeTags = FastTags<TagGroup.Global>.Parse("knife");

	// Token: 0x04000534 RID: 1332
	[PublicizedFrom(EAccessModifier.Private)]
	public const float MaxForce = 1.5f;

	// Token: 0x04000535 RID: 1333
	public const string cXmlTag = "DismemberTag_";

	// Token: 0x04000536 RID: 1334
	public const char cParamSplit = ';';

	// Token: 0x04000537 RID: 1335
	public const char cRawSplit = '+';

	// Token: 0x04000538 RID: 1336
	public const char cDataSplit = '=';

	// Token: 0x04000539 RID: 1337
	public const char cCommaDel = ',';

	// Token: 0x0400053A RID: 1338
	public static readonly Dictionary<EnumBodyPartHit, string[]> BipedDismemberments = new Dictionary<EnumBodyPartHit, string[]>
	{
		{
			EnumBodyPartHit.Head,
			new string[]
			{
				"DismemberTag_L_HeadGore",
				"Common/Dismemberment/HeadGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperLeg,
			new string[]
			{
				"DismemberTag_L_LeftUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerLeg,
			new string[]
			{
				"DismemberTag_L_LeftLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperLeg,
			new string[]
			{
				"DismemberTag_L_RightUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerLeg,
			new string[]
			{
				"DismemberTag_L_RightLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperArm,
			new string[]
			{
				"DismemberTag_L_LeftUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerArm,
			new string[]
			{
				"DismemberTag_L_LeftLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperArm,
			new string[]
			{
				"DismemberTag_L_RightUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerArm,
			new string[]
			{
				"DismemberTag_L_RightLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		}
	};

	// Token: 0x0400053B RID: 1339
	public static readonly Dictionary<EnumBodyPartHit, string[]> QuadrupedDismemberments = new Dictionary<EnumBodyPartHit, string[]>
	{
		{
			EnumBodyPartHit.Head,
			new string[]
			{
				"DismemberTag_L_HeadGore",
				"Common/Dismemberment/HeadGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperLeg,
			new string[]
			{
				"DismemberTag_L_LeftUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerLeg,
			new string[]
			{
				"DismemberTag_L_LeftLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperLeg,
			new string[]
			{
				"DismemberTag_L_RightUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerLeg,
			new string[]
			{
				"DismemberTag_L_RightLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperArm,
			new string[]
			{
				"DismemberTag_L_LeftUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerArm,
			new string[]
			{
				"DismemberTag_L_LeftLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperArm,
			new string[]
			{
				"DismemberTag_L_RightUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerArm,
			new string[]
			{
				"DismemberTag_L_RightLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		}
	};

	// Token: 0x0400053C RID: 1340
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDebugAxisPath = "@:Entities/Zombies/Gibs/Debug/debugAxisObj.prefab";

	// Token: 0x020000C0 RID: 192
	public static class DamageKeys
	{
		// Token: 0x0400053D RID: 1341
		public const string blade = "blade";

		// Token: 0x0400053E RID: 1342
		public const string blunt = "blunt";

		// Token: 0x0400053F RID: 1343
		public const string bullet = "bullet";

		// Token: 0x04000540 RID: 1344
		public const string exlosive = "explosive";
	}

	// Token: 0x020000C1 RID: 193
	public enum DamageTags
	{
		// Token: 0x04000542 RID: 1346
		none,
		// Token: 0x04000543 RID: 1347
		blade,
		// Token: 0x04000544 RID: 1348
		blunt,
		// Token: 0x04000545 RID: 1349
		any
	}

	// Token: 0x020000C2 RID: 194
	public static class ParseKeys
	{
		// Token: 0x04000546 RID: 1350
		public const string cType = "type";

		// Token: 0x04000547 RID: 1351
		public const string cTarget = "target";

		// Token: 0x04000548 RID: 1352
		public const string cAttachToParent = "atp";

		// Token: 0x04000549 RID: 1353
		public const string cDetach = "detach";

		// Token: 0x0400054A RID: 1354
		public const string cMask = "mask";

		// Token: 0x0400054B RID: 1355
		public const string cScaleOutLimb = "sol";

		// Token: 0x0400054C RID: 1356
		public const string cSolTarget = "soltarget";

		// Token: 0x0400054D RID: 1357
		public const string cSolScale = "solscale";

		// Token: 0x0400054E RID: 1358
		public const string cInsertChildObj = "ico";

		// Token: 0x0400054F RID: 1359
		public const string cInsertBoneObj = "ibo";
	}
}
