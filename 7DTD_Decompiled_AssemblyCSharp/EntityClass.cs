using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200042A RID: 1066
[Preserve]
public class EntityClass
{
	// Token: 0x060020D8 RID: 8408 RVA: 0x000CCDC4 File Offset: 0x000CAFC4
	public static void Add(string _entityClassname, EntityClass _entityClass)
	{
		_entityClass.entityClassName = _entityClassname;
		EntityClass.list[_entityClassname.GetHashCode()] = _entityClass;
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000CCDE0 File Offset: 0x000CAFE0
	public static EntityClass GetEntityClass(int entityClass)
	{
		EntityClass result;
		EntityClass.list.TryGetValue(entityClass, out result);
		return result;
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x000CCDFC File Offset: 0x000CAFFC
	public static string GetEntityClassName(int entityClass)
	{
		EntityClass entityClass2;
		if (EntityClass.list.TryGetValue(entityClass, out entityClass2))
		{
			return entityClass2.entityClassName;
		}
		return "null";
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x000CCE24 File Offset: 0x000CB024
	public static int GetId(string _name)
	{
		foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
		{
			if (keyValuePair.Value.entityClassName == _name)
			{
				return keyValuePair.Key;
			}
		}
		return -1;
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x000CCE98 File Offset: 0x000CB098
	public static int FromString(string _s)
	{
		return _s.GetHashCode();
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000CCEF8 File Offset: 0x000CB0F8
	public EntityClass Init()
	{
		this.censorType = 1;
		string text = "";
		if (this.Properties.Contains(EntityClass.PropCensor))
		{
			text = this.Properties.GetStringValue(EntityClass.PropCensor);
		}
		if (!string.IsNullOrEmpty(text) && text.Contains(","))
		{
			string[] array = text.Split(",", StringSplitOptions.None);
			if (array.Length > 1)
			{
				StringParsers.TryParseSInt32(array[0], out this.censorMode, 0, -1, NumberStyles.Integer);
				StringParsers.TryParseSInt32(array[1], out this.censorType, 0, -1, NumberStyles.Integer);
			}
		}
		else
		{
			this.Properties.ParseInt(EntityClass.PropCensor, ref this.censorMode);
		}
		if (!this.Properties.Values.TryGetValue(EntityClass.PropPrefab, out this.prefabPath) || this.prefabPath.Length == 0)
		{
			throw new Exception("Mandatory property 'prefab' missing in entity_class '" + this.entityClassName + "'");
		}
		string value;
		bool flag;
		if (this.Properties.Values.TryGetValue(EntityClass.PropPrefabCombined, out value) && bool.TryParse(value, out flag) && flag)
		{
			this.IsPrefabCombined = true;
		}
		else if (this.prefabPath[0] == '/')
		{
			this.prefabPath = this.prefabPath.Substring(1);
			this.IsPrefabCombined = true;
		}
		else if (DataLoader.IsInResources(this.prefabPath))
		{
			this.prefabPath = "Prefabs/prefabEntity" + this.prefabPath;
		}
		string text2;
		if (this.Properties.Values.TryGetValue(EntityClass.PropMesh, out text2) && text2.Length > 0)
		{
			if (this.censorMode != 0 && (this.censorType == 1 || this.censorType == 3) && GameManager.Instance && GameManager.Instance.IsGoreCensored())
			{
				text2 = text2.Replace(".", "_CGore.");
			}
			if (DataLoader.IsInResources(text2))
			{
				text2 = "Entities/" + text2;
			}
			this.meshPath = text2;
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropMeshFP))
		{
			string text3 = this.Properties.Values[EntityClass.PropMeshFP];
			if (DataLoader.IsInResources(text3))
			{
				text3 = "Entities/" + text3;
			}
			this.meshFP = DataLoader.LoadAsset<Transform>(text3, false);
			if (this.meshFP == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Could not load file '",
					text3,
					"' for entity_class '",
					this.entityClassName,
					"'"
				}));
			}
		}
		this.entityFlags = EntityFlags.None;
		EntityClass.ParseEntityFlags(this.Properties.GetString(EntityClass.PropEntityFlags), ref this.entityFlags);
		if (this.Properties.Values.ContainsKey(EntityClass.PropClass))
		{
			this.classname = Type.GetType(this.Properties.Values[EntityClass.PropClass]);
			if (this.classname == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Could not instantiate class",
					this.Properties.Values[EntityClass.PropClass],
					"' for entity_class '",
					this.entityClassName,
					"'"
				}));
			}
		}
		this.modelType = typeof(EModelCustom);
		string @string = this.Properties.GetString(EntityClass.PropModelType);
		if (@string.Length > 0)
		{
			this.modelType = ReflectionHelpers.GetTypeWithPrefix("EModel", @string);
			if (this.modelType == null)
			{
				throw new Exception("Model class '" + @string + "' not found!");
			}
		}
		string string2 = this.Properties.GetString(EntityClass.PropAltMats);
		if (string2.Length > 0)
		{
			this.AltMatNames = string2.Split(',', StringSplitOptions.None);
		}
		string string3 = this.Properties.GetString(EntityClass.PropSwapMats);
		if (string3.Length > 0)
		{
			this.MatSwap = string3.Split(",", StringSplitOptions.None);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropParticleOnSpawn))
		{
			this.particleOnSpawn.fileName = this.Properties.Values[EntityClass.PropParticleOnSpawn];
			this.particleOnSpawn.shapeMesh = this.Properties.Params1[EntityClass.PropParticleOnSpawn];
			DataLoader.PreloadBundle(this.particleOnSpawn.fileName);
		}
		this.RagdollOnDeathChance = 0.5f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropRagdollOnDeathChance))
		{
			this.RagdollOnDeathChance = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropRagdollOnDeathChance], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropHasRagdoll))
		{
			this.HasRagdoll = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropHasRagdoll], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropColliders))
		{
			this.CollidersRagdollAsset = this.Properties.Values[EntityClass.PropColliders];
			DataLoader.PreloadBundle(this.CollidersRagdollAsset);
		}
		this.Properties.ParseFloat(EntityClass.PropLookAtAngle, ref this.LookAtAngle);
		if (this.Properties.Values.ContainsKey(EntityClass.PropCrouchYOffsetFP))
		{
			this.crouchYOffsetFP = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropCrouchYOffsetFP], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropParent))
		{
			this.parentGameObjectName = this.Properties.Values[EntityClass.PropParent];
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropSkinTexture))
		{
			this.skinTexture = this.Properties.Values[EntityClass.PropSkinTexture];
			DataLoader.PreloadBundle(this.skinTexture);
		}
		this.bIsEnemyEntity = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsEnemyEntity))
		{
			this.bIsEnemyEntity = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsEnemyEntity], 0, -1, true);
		}
		this.bIsAnimalEntity = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsAnimalEntity))
		{
			this.bIsAnimalEntity = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsAnimalEntity], 0, -1, true);
		}
		this.CorpseBlockId = null;
		if (this.Properties.Values.ContainsKey(EntityClass.PropCorpseBlock))
		{
			this.CorpseBlockId = this.Properties.Values[EntityClass.PropCorpseBlock];
		}
		this.CorpseBlockChance = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropCorpseBlockChance))
		{
			this.CorpseBlockChance = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropCorpseBlockChance], 0, -1, NumberStyles.Any);
		}
		this.CorpseBlockDensity = (int)MarchingCubes.DensityTerrain;
		if (this.Properties.Values.ContainsKey(EntityClass.PropCorpseBlockDensity))
		{
			this.CorpseBlockDensity = int.Parse(this.Properties.Values[EntityClass.PropCorpseBlockDensity]);
			this.CorpseBlockDensity = Math.Max(-128, Math.Min(127, this.CorpseBlockDensity));
		}
		this.RootMotion = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropRootMotion))
		{
			this.RootMotion = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropRootMotion], 0, -1, true);
		}
		this.HasDeathAnim = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropHasDeathAnim))
		{
			this.HasDeathAnim = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropHasDeathAnim], 0, -1, true);
		}
		this.ExperienceValue = 100;
		if (this.Properties.Values.ContainsKey(EntityClass.PropExperienceGain))
		{
			this.ExperienceValue = (int)StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropExperienceGain], 0, -1, NumberStyles.Any);
		}
		string string4 = this.Properties.GetString(EntityClass.PropLootDropEntityClass);
		if (string4.Length > 0)
		{
			this.lootDropEntityClass = EntityClass.FromString(string4);
		}
		this.bIsMale = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsMale))
		{
			this.bIsMale = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsMale], 0, -1, true);
		}
		this.bIsChunkObserver = false;
		if (this.Properties.Values.ContainsKey(EntityClass.PropIsChunkObserver))
		{
			this.bIsChunkObserver = StringParsers.ParseBool(this.Properties.Values[EntityClass.PropIsChunkObserver], 0, -1, true);
		}
		this.SightRange = Constants.cDefaultMonsterSeeDistance;
		if (this.Properties.Values.ContainsKey(EntityClass.PropSightRange))
		{
			this.SightRange = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropSightRange], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropSightLightThreshold))
		{
			this.sightLightThreshold = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropSightLightThreshold]);
		}
		else
		{
			this.sightLightThreshold = new Vector2(30f, 100f);
		}
		this.SleeperNoiseToSense = new Vector2(15f, 15f);
		this.Properties.ParseVec(EntityClass.PropSleeperNoiseToSense, ref this.SleeperNoiseToSense);
		this.SleeperNoiseToSenseSoundChance = 1f;
		this.Properties.ParseFloat(EntityClass.PropSleeperNoiseToSenseSoundChance, ref this.SleeperNoiseToSenseSoundChance);
		this.SleeperNoiseToWake = new Vector2(15f, 15f);
		this.Properties.ParseVec(EntityClass.PropSleeperNoiseToWake, ref this.SleeperNoiseToWake);
		this.SleeperSightToSenseMin = new Vector2(25f, 25f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToSenseMin, ref this.SleeperSightToSenseMin);
		this.SleeperSightToSenseMax = new Vector2(200f, 200f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToSenseMax, ref this.SleeperSightToSenseMax);
		this.SleeperSightToWakeMin = new Vector2(15f, 15f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToWakeMin, ref this.SleeperSightToWakeMin);
		this.SleeperSightToWakeMax = new Vector2(200f, 200f);
		this.Properties.ParseVec(EntityClass.PropSleeperSightToWakeMax, ref this.SleeperSightToWakeMax);
		this.MassKg = 10f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropMass))
		{
			this.MassKg = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropMass], 0, -1, NumberStyles.Any);
		}
		this.MassKg *= 0.454f;
		this.SizeScale = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropSizeScale))
		{
			this.SizeScale = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropSizeScale], 0, -1, NumberStyles.Any);
		}
		string string5 = this.Properties.GetString(EntityClass.PropPhysicsBody);
		if (string5.Length > 0)
		{
			this.PhysicsBody = PhysicsBodyLayout.Find(string5);
		}
		if (this.Properties.Values.ContainsKey("DeadBodyHitPoints"))
		{
			this.DeadBodyHitPoints = int.Parse(this.Properties.Values["DeadBodyHitPoints"]);
		}
		this.Properties.ParseFloat(EntityClass.PropLegCrippleScale, ref this.LegCrippleScale);
		this.Properties.ParseFloat(EntityClass.PropLegCrawlerThreshold, ref this.LegCrawlerThreshold);
		this.DismemberMultiplierHead = 1f;
		this.Properties.ParseFloat(EntityClass.PropDismemberMultiplierHead, ref this.DismemberMultiplierHead);
		this.DismemberMultiplierArms = 1f;
		this.Properties.ParseFloat(EntityClass.PropDismemberMultiplierArms, ref this.DismemberMultiplierArms);
		this.DismemberMultiplierLegs = 1f;
		this.Properties.ParseFloat(EntityClass.PropDismemberMultiplierLegs, ref this.DismemberMultiplierLegs);
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownKneelDamageThreshold))
		{
			this.KnockdownKneelDamageThreshold = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropKnockdownKneelDamageThreshold], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownKneelStunDuration))
		{
			this.KnockdownKneelStunDuration = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownKneelStunDuration]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownProneDamageThreshold))
		{
			this.KnockdownProneDamageThreshold = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropKnockdownProneDamageThreshold], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownProneStunDuration))
		{
			this.KnockdownProneStunDuration = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownProneStunDuration]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownKneelRefillRate))
		{
			this.KnockdownKneelRefillRate = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownKneelRefillRate]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropKnockdownProneRefillRate))
		{
			this.KnockdownProneRefillRate = StringParsers.ParseMinMaxCount(this.Properties.Values[EntityClass.PropKnockdownProneRefillRate]);
		}
		this.LegsExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropLegsExplosionDamageMultiplier))
		{
			this.LegsExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropLegsExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		this.ArmsExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropArmsExplosionDamageMultiplier))
		{
			this.ArmsExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropArmsExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		this.HeadExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropHeadExplosionDamageMultiplier))
		{
			this.HeadExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropHeadExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		this.ChestExplosionDamageMultiplier = 1f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropChestExplosionDamageMultiplier))
		{
			this.ChestExplosionDamageMultiplier = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropChestExplosionDamageMultiplier], 0, -1, NumberStyles.Any);
		}
		Vector3 zero = Vector3.zero;
		this.Properties.ParseVec(EntityClass.PropPainResistPerHit, ref zero, 0f);
		this.PainResistPerHit = zero.x;
		this.PainResistPerHitLowHealth = zero.y;
		this.PainResistPerHitLowHealthPercent = zero.z;
		if (this.Properties.Values.ContainsKey(EntityClass.PropArchetype))
		{
			this.ArchetypeName = this.Properties.Values[EntityClass.PropArchetype];
		}
		this.SwimOffset = 0.9f;
		if (this.Properties.Values.ContainsKey(EntityClass.PropSwimOffset))
		{
			this.SwimOffset = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropSwimOffset], 0, -1, NumberStyles.Any);
		}
		this.SearchRadius = 6f;
		this.Properties.ParseFloat(EntityClass.PropSearchRadius, ref this.SearchRadius);
		if (this.Properties.Values.ContainsKey(EntityClass.PropUMARace))
		{
			this.UMARace = this.Properties.Values[EntityClass.PropUMARace];
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropUMAGeneratedModelName))
		{
			this.UMAGeneratedModelName = this.Properties.Values[EntityClass.PropUMAGeneratedModelName];
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropModelTransformAdjust))
		{
			this.ModelTransformAdjust = StringParsers.ParseVector3(this.Properties.Values[EntityClass.PropModelTransformAdjust], 0, -1);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropAIPackages))
		{
			this.AIPackages = this.Properties.Values[EntityClass.PropAIPackages].Split(',', StringSplitOptions.None);
			for (int i = 0; i < this.AIPackages.Length; i++)
			{
				this.AIPackages[i] = this.AIPackages[i].Trim();
			}
			this.UseAIPackages = true;
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropBuffs))
		{
			string[] array2 = this.Properties.Values[EntityClass.PropBuffs].Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length != 0)
			{
				this.Buffs = new List<string>(array2);
			}
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropMaxTurnSpeed))
		{
			this.MaxTurnSpeed = StringParsers.ParseFloat(this.Properties.Values[EntityClass.PropMaxTurnSpeed], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropTags))
		{
			this.Tags = FastTags<TagGroup.Global>.Parse(this.Properties.Values[EntityClass.PropTags]);
		}
		if (this.Properties.Values.ContainsKey(EntityClass.PropNavObject))
		{
			this.NavObject = this.Properties.Values[EntityClass.PropNavObject];
		}
		this.Properties.ParseVec(EntityClass.PropNavObjectHeadOffset, ref this.NavObjectHeadOffset);
		this.explosionData = new ExplosionData(this.Properties, this.Effects);
		bool flag2 = false;
		this.Properties.ParseBool(EntityClass.PropHideInSpawnMenu, ref flag2);
		if (flag2)
		{
			this.userSpawnType = EntityClass.UserSpawnType.Console;
		}
		this.Properties.ParseEnum<EntityClass.UserSpawnType>(EntityClass.PropUserSpawnType, ref this.userSpawnType);
		this.Properties.ParseBool(EntityClass.PropCanBigHead, ref this.CanBigHead);
		this.Properties.ParseInt(EntityClass.PropDanceType, ref this.DanceTypeID);
		this.Properties.ParseString(EntityClass.PropOnActivateEvent, ref this.onActivateEvent);
		return this;
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000CE0FC File Offset: 0x000CC2FC
	public void CopyFrom(EntityClass _other, HashSet<string> _exclude = null)
	{
		foreach (KeyValuePair<string, string> keyValuePair in _other.Properties.Values.Dict)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair.Key))
			{
				this.Properties.Values[keyValuePair.Key] = _other.Properties.Values[keyValuePair.Key];
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair2 in _other.Properties.Params1.Dict)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair2.Key))
			{
				this.Properties.Params1[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair3 in _other.Properties.Params2.Dict)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair3.Key))
			{
				this.Properties.Params2[keyValuePair3.Key] = keyValuePair3.Value;
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair4 in _other.Properties.Data.Dict)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair4.Key))
			{
				this.Properties.Data[keyValuePair4.Key] = keyValuePair4.Value;
			}
		}
		foreach (KeyValuePair<string, DynamicProperties> keyValuePair5 in _other.Properties.Classes.Dict)
		{
			if (_exclude == null || !_exclude.Contains(keyValuePair5.Key))
			{
				DynamicProperties dynamicProperties = new DynamicProperties();
				dynamicProperties.CopyFrom(keyValuePair5.Value, null);
				this.Properties.Classes[keyValuePair5.Key] = dynamicProperties;
			}
		}
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000CE37C File Offset: 0x000CC57C
	public static void ParseEntityFlags(string _names, ref EntityFlags optionalValue)
	{
		if (_names.Length > 0)
		{
			if (_names.IndexOf(',') >= 0)
			{
				string[] array = _names.Split(EntityClass.commaSeparator, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					EntityFlags entityFlags;
					if (EnumUtils.TryParse<EntityFlags>(array[i], out entityFlags, true))
					{
						optionalValue |= entityFlags;
					}
				}
				return;
			}
			EntityFlags entityFlags2;
			if (EnumUtils.TryParse<EntityFlags>(_names, out entityFlags2, true))
			{
				optionalValue = entityFlags2;
			}
		}
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000CE3DA File Offset: 0x000CC5DA
	public static void Cleanup()
	{
		EntityClass.list.Clear();
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000CE3E8 File Offset: 0x000CC5E8
	public void AddDroppedId(EnumDropEvent _eEvent, string _name, int _minCount, int _maxCount, float _prob, float _stickChance, string _toolCategory, string _tag)
	{
		List<Block.SItemDropProb> list = this.itemsToDrop.ContainsKey(_eEvent) ? this.itemsToDrop[_eEvent] : null;
		if (list == null)
		{
			list = new List<Block.SItemDropProb>();
			this.itemsToDrop[_eEvent] = list;
		}
		list.Add(new Block.SItemDropProb(_name, _minCount, _maxCount, _prob, 1f, _stickChance, _toolCategory, _tag));
	}

	// Token: 0x0400174F RID: 5967
	public static string PropEntityFlags = "EntityFlags";

	// Token: 0x04001750 RID: 5968
	public static string PropEntityType = "EntityType";

	// Token: 0x04001751 RID: 5969
	public static string PropClass = "Class";

	// Token: 0x04001752 RID: 5970
	public static string PropCensor = "Censor";

	// Token: 0x04001753 RID: 5971
	public static string PropMesh = "Mesh";

	// Token: 0x04001754 RID: 5972
	public static string PropMeshFP = "MeshFP";

	// Token: 0x04001755 RID: 5973
	public static string PropPrefab = "Prefab";

	// Token: 0x04001756 RID: 5974
	public static string PropPrefabCombined = "PrefabCombined";

	// Token: 0x04001757 RID: 5975
	public static string PropParent = "Parent";

	// Token: 0x04001758 RID: 5976
	public static string PropAvatarController = "AvatarController";

	// Token: 0x04001759 RID: 5977
	public static string PropLocalAvatarController = "LocalAvatarController";

	// Token: 0x0400175A RID: 5978
	public static string PropSkinTexture = "SkinTexture";

	// Token: 0x0400175B RID: 5979
	public static string PropAltMats = "AltMats";

	// Token: 0x0400175C RID: 5980
	public static string PropSwapMats = "SwapMats";

	// Token: 0x0400175D RID: 5981
	public static string PropMatColor = "MatColor";

	// Token: 0x0400175E RID: 5982
	public static string PropRightHandJointName = "RightHandJointName";

	// Token: 0x0400175F RID: 5983
	public static string PropHandItem = "HandItem";

	// Token: 0x04001760 RID: 5984
	public static string PropHandItemCrawler = "HandItemCrawler";

	// Token: 0x04001761 RID: 5985
	public static string PropMaxHealth = "MaxHealth";

	// Token: 0x04001762 RID: 5986
	public static string PropMaxStamina = "MaxStamina";

	// Token: 0x04001763 RID: 5987
	public static string PropSickness = "Sickness";

	// Token: 0x04001764 RID: 5988
	public static string PropGassiness = "Gassiness";

	// Token: 0x04001765 RID: 5989
	public static string PropWellness = "Wellness";

	// Token: 0x04001766 RID: 5990
	public static string PropFood = "Food";

	// Token: 0x04001767 RID: 5991
	public static string PropWater = "Water";

	// Token: 0x04001768 RID: 5992
	public static string PropMaxViewAngle = "MaxViewAngle";

	// Token: 0x04001769 RID: 5993
	public static string PropWeight = "Weight";

	// Token: 0x0400176A RID: 5994
	public static string PropPushFactor = "PushFactor";

	// Token: 0x0400176B RID: 5995
	public static string PropTimeStayAfterDeath = "TimeStayAfterDeath";

	// Token: 0x0400176C RID: 5996
	public static string PropImmunity = "Immunity";

	// Token: 0x0400176D RID: 5997
	public static string PropIsMale = "IsMale";

	// Token: 0x0400176E RID: 5998
	public static string PropIsChunkObserver = "IsChunkObserver";

	// Token: 0x0400176F RID: 5999
	public static string PropAIFeralSense = "AIFeralSense";

	// Token: 0x04001770 RID: 6000
	public static string PropAIGroupCircle = "AIGroupCircle";

	// Token: 0x04001771 RID: 6001
	public static string PropAINoiseSeekDist = "AINoiseSeekDist";

	// Token: 0x04001772 RID: 6002
	public static string PropAISeeOffset = "AISeeOffset";

	// Token: 0x04001773 RID: 6003
	public static string PropAIPathCostScale = "AIPathCostScale";

	// Token: 0x04001774 RID: 6004
	public static string PropAITask = "AITask-";

	// Token: 0x04001775 RID: 6005
	public static string PropAITargetTask = "AITarget-";

	// Token: 0x04001776 RID: 6006
	public static string PropMoveSpeed = "MoveSpeed";

	// Token: 0x04001777 RID: 6007
	public static string PropMoveSpeedNight = "MoveSpeedNight";

	// Token: 0x04001778 RID: 6008
	public static string PropMoveSpeedAggro = "MoveSpeedAggro";

	// Token: 0x04001779 RID: 6009
	public static string PropMoveSpeedRand = "MoveSpeedRand";

	// Token: 0x0400177A RID: 6010
	public static string PropMoveSpeedPanic = "MoveSpeedPanic";

	// Token: 0x0400177B RID: 6011
	public static string PropMoveSpeedPattern = "MoveSpeedPattern";

	// Token: 0x0400177C RID: 6012
	public static string PropSwimSpeed = "SwimSpeed";

	// Token: 0x0400177D RID: 6013
	public static string PropSwimStrokeRate = "SwimStrokeRate";

	// Token: 0x0400177E RID: 6014
	public static string PropCrouchType = "CrouchType";

	// Token: 0x0400177F RID: 6015
	public static string PropDanceType = "DanceType";

	// Token: 0x04001780 RID: 6016
	public static string PropWalkType = "WalkType";

	// Token: 0x04001781 RID: 6017
	public static string PropCanClimbVertical = "CanClimbVertical";

	// Token: 0x04001782 RID: 6018
	public static string PropCanClimbLadders = "CanClimbLadders";

	// Token: 0x04001783 RID: 6019
	public static string PropJumpDelay = "JumpDelay";

	// Token: 0x04001784 RID: 6020
	public static string PropJumpMaxDistance = "JumpMaxDistance";

	// Token: 0x04001785 RID: 6021
	public static string PropIsEnemyEntity = "IsEnemyEntity";

	// Token: 0x04001786 RID: 6022
	public static string PropIsAnimalEntity = "IsAnimalEntity";

	// Token: 0x04001787 RID: 6023
	public static string PropSoundRandomTime = "SoundRandomTime";

	// Token: 0x04001788 RID: 6024
	public static string PropSoundAlertTime = "SoundAlertTime";

	// Token: 0x04001789 RID: 6025
	public static string PropSoundRandom = "SoundRandom";

	// Token: 0x0400178A RID: 6026
	public static string PropSoundHurt = "SoundHurt";

	// Token: 0x0400178B RID: 6027
	public static string PropSoundJump = "SoundJump";

	// Token: 0x0400178C RID: 6028
	public static string PropSoundHurtSmall = "SoundHurtSmall";

	// Token: 0x0400178D RID: 6029
	public static string PropSoundDrownPain = "SoundDrownPain";

	// Token: 0x0400178E RID: 6030
	public static string PropSoundDrownDeath = "SoundDrownDeath";

	// Token: 0x0400178F RID: 6031
	public static string PropSoundWaterSurface = "SoundWaterSurface";

	// Token: 0x04001790 RID: 6032
	public static string PropSoundDeath = "SoundDeath";

	// Token: 0x04001791 RID: 6033
	public static string PropSoundAttack = "SoundAttack";

	// Token: 0x04001792 RID: 6034
	public static string PropSoundAlert = "SoundAlert";

	// Token: 0x04001793 RID: 6035
	public static string PropSoundSense = "SoundSense";

	// Token: 0x04001794 RID: 6036
	public static string PropSoundStamina = "SoundStamina";

	// Token: 0x04001795 RID: 6037
	public static string PropSoundLiving = "SoundLiving";

	// Token: 0x04001796 RID: 6038
	public static string PropSoundSpawn = "SoundSpawn";

	// Token: 0x04001797 RID: 6039
	public static string PropSoundLand = "SoundLanding";

	// Token: 0x04001798 RID: 6040
	public static string PropSoundStepType = "SoundStepType";

	// Token: 0x04001799 RID: 6041
	public static string PropSoundGiveUp = "SoundGiveUp";

	// Token: 0x0400179A RID: 6042
	public static string PropSoundExplodeWarn = "SoundExplodeWarn";

	// Token: 0x0400179B RID: 6043
	public static string PropSoundTick = "SoundTick";

	// Token: 0x0400179C RID: 6044
	public static string PropExplodeDelay = "ExplodeDelay";

	// Token: 0x0400179D RID: 6045
	public static string PropExplodeHealthThreshold = "ExplodeHealthThreshold";

	// Token: 0x0400179E RID: 6046
	public static string PropLootListOnDeath = "LootListOnDeath";

	// Token: 0x0400179F RID: 6047
	public static string PropLootListAlive = "LootListAlive";

	// Token: 0x040017A0 RID: 6048
	public static string PropLootDropProb = "LootDropProb";

	// Token: 0x040017A1 RID: 6049
	public static string PropLootDropEntityClass = "LootDropEntityClass";

	// Token: 0x040017A2 RID: 6050
	public static string PropAttackTimeoutDay = "AttackTimeoutDay";

	// Token: 0x040017A3 RID: 6051
	public static string PropAttackTimeoutNight = "AttackTimeoutNight";

	// Token: 0x040017A4 RID: 6052
	public static string PropMapIcon = "MapIcon";

	// Token: 0x040017A5 RID: 6053
	public static string PropCompassIcon = "CompassIcon";

	// Token: 0x040017A6 RID: 6054
	public static string PropTrackerIcon = "TrackerIcon";

	// Token: 0x040017A7 RID: 6055
	public static string PropCompassUpIcon = "CompassUpIcon";

	// Token: 0x040017A8 RID: 6056
	public static string PropCompassDownIcon = "CompassDownIcon";

	// Token: 0x040017A9 RID: 6057
	public static string PropParticleOnSpawn = "ParticleOnSpawn";

	// Token: 0x040017AA RID: 6058
	public static string PropParticleOnDeath = "ParticleOnDeath";

	// Token: 0x040017AB RID: 6059
	public static string PropParticleOnDestroy = "ParticleOnDestroy";

	// Token: 0x040017AC RID: 6060
	public static string PropItemsOnEnterGame = "ItemsOnEnterGame";

	// Token: 0x040017AD RID: 6061
	public static string PropFallLandBehavior = "FallLandBehavior";

	// Token: 0x040017AE RID: 6062
	public static string PropDestroyBlockBehavior = "DestroyBlockBehavior";

	// Token: 0x040017AF RID: 6063
	public static string PropDropInventoryBlock = "DropInventoryBlock";

	// Token: 0x040017B0 RID: 6064
	public static string PropModelType = "ModelType";

	// Token: 0x040017B1 RID: 6065
	public static string PropRagdollOnDeathChance = "RagdollOnDeathChance";

	// Token: 0x040017B2 RID: 6066
	public static string PropHasRagdoll = "HasRagdoll";

	// Token: 0x040017B3 RID: 6067
	public static string PropMass = "Mass";

	// Token: 0x040017B4 RID: 6068
	public static string PropSizeScale = "SizeScale";

	// Token: 0x040017B5 RID: 6069
	public static string PropPhysicsBody = "PhysicsBody";

	// Token: 0x040017B6 RID: 6070
	public static string PropColliders = "Colliders";

	// Token: 0x040017B7 RID: 6071
	public static string PropLookAtAngle = "LookAtAngle";

	// Token: 0x040017B8 RID: 6072
	public static string PropCrouchYOffsetFP = "CrouchYOffsetFP";

	// Token: 0x040017B9 RID: 6073
	public static string PropRotateToGround = "RotateToGround";

	// Token: 0x040017BA RID: 6074
	public static string PropCorpseBlock = "CorpseBlock";

	// Token: 0x040017BB RID: 6075
	public static string PropCorpseBlockChance = "CorpseBlockChance";

	// Token: 0x040017BC RID: 6076
	public static string PropCorpseBlockDensity = "CorpseBlockDensity";

	// Token: 0x040017BD RID: 6077
	public static string PropRootMotion = "RootMotion";

	// Token: 0x040017BE RID: 6078
	public static string PropExperienceGain = "ExperienceGain";

	// Token: 0x040017BF RID: 6079
	public static string PropHasDeathAnim = "HasDeathAnim";

	// Token: 0x040017C0 RID: 6080
	public static string PropLegCrippleScale = "LegCrippleScale";

	// Token: 0x040017C1 RID: 6081
	public static string PropLegCrawlerThreshold = "LegCrawlerThreshold";

	// Token: 0x040017C2 RID: 6082
	public static string PropDismemberMultiplierHead = "DismemberMultiplierHead";

	// Token: 0x040017C3 RID: 6083
	public static string PropDismemberMultiplierArms = "DismemberMultiplierArms";

	// Token: 0x040017C4 RID: 6084
	public static string PropDismemberMultiplierLegs = "DismemberMultiplierLegs";

	// Token: 0x040017C5 RID: 6085
	public static string PropKnockdownKneelDamageThreshold = "KnockdownKneelDamageThreshold";

	// Token: 0x040017C6 RID: 6086
	public static string PropKnockdownKneelStunDuration = "KnockdownKneelStunDuration";

	// Token: 0x040017C7 RID: 6087
	public static string PropKnockdownProneDamageThreshold = "KnockdownProneDamageThreshold";

	// Token: 0x040017C8 RID: 6088
	public static string PropKnockdownProneStunDuration = "KnockdownProneStunDuration";

	// Token: 0x040017C9 RID: 6089
	public static string PropKnockdownProneRefillRate = "KnockdownProneRefillRate";

	// Token: 0x040017CA RID: 6090
	public static string PropKnockdownKneelRefillRate = "KnockdownKneelRefillRate";

	// Token: 0x040017CB RID: 6091
	public static string PropArmsExplosionDamageMultiplier = "ArmsExplosionDamageMultiplier";

	// Token: 0x040017CC RID: 6092
	public static string PropLegsExplosionDamageMultiplier = "LegsExplosionDamageMultiplier";

	// Token: 0x040017CD RID: 6093
	public static string PropChestExplosionDamageMultiplier = "ChestExplosionDamageMultiplier";

	// Token: 0x040017CE RID: 6094
	public static string PropHeadExplosionDamageMultiplier = "HeadExplosionDamageMultiplier";

	// Token: 0x040017CF RID: 6095
	public static string PropPainResistPerHit = "PainResistPerHit";

	// Token: 0x040017D0 RID: 6096
	public static string PropArchetype = "Archetype";

	// Token: 0x040017D1 RID: 6097
	public static string PropSwimOffset = "SwimOffset";

	// Token: 0x040017D2 RID: 6098
	public static string PropUMARace = "UMARace";

	// Token: 0x040017D3 RID: 6099
	public static string PropUMAGeneratedModelName = "UMAGeneratedModelName";

	// Token: 0x040017D4 RID: 6100
	public static string PropNPCID = "NPCID";

	// Token: 0x040017D5 RID: 6101
	public static string PropModelTransformAdjust = "ModelTransformAdjust";

	// Token: 0x040017D6 RID: 6102
	public static string PropAIPackages = "AIPackages";

	// Token: 0x040017D7 RID: 6103
	public static string PropBuffs = "Buffs";

	// Token: 0x040017D8 RID: 6104
	public static string PropStealthSoundDecayRate = "StealthSoundDecayRate";

	// Token: 0x040017D9 RID: 6105
	public static string PropSightRange = "SightRange";

	// Token: 0x040017DA RID: 6106
	public static string PropSightLightThreshold = "SightLightThreshold";

	// Token: 0x040017DB RID: 6107
	public static string PropSleeperSightToSenseMin = "SleeperSightToSenseMin";

	// Token: 0x040017DC RID: 6108
	public static string PropSleeperSightToSenseMax = "SleeperSightToSenseMax";

	// Token: 0x040017DD RID: 6109
	public static string PropSleeperSightToWakeMin = "SleeperSightToWakeMin";

	// Token: 0x040017DE RID: 6110
	public static string PropSleeperSightToWakeMax = "SleeperSightToWakeMax";

	// Token: 0x040017DF RID: 6111
	public static string PropSleeperNoiseToSense = "SleeperNoiseToSense";

	// Token: 0x040017E0 RID: 6112
	public static string PropSleeperNoiseToSenseSoundChance = "SleeperNoiseToSenseSoundChance";

	// Token: 0x040017E1 RID: 6113
	public static string PropSleeperNoiseToWake = "SleeperNoiseToWake";

	// Token: 0x040017E2 RID: 6114
	public static string PropSoundSleeperSense = "SoundSleeperSense";

	// Token: 0x040017E3 RID: 6115
	public static string PropSoundSleeperSnore = "SoundSleeperBackToSleep";

	// Token: 0x040017E4 RID: 6116
	public static string PropMaxTurnSpeed = "MaxTurnSpeed";

	// Token: 0x040017E5 RID: 6117
	public static string PropSearchRadius = "SearchRadius";

	// Token: 0x040017E6 RID: 6118
	public static string PropTags = "Tags";

	// Token: 0x040017E7 RID: 6119
	public static string PropNavObject = "NavObject";

	// Token: 0x040017E8 RID: 6120
	public static string PropNavObjectHeadOffset = "NavObjectHeadOffset";

	// Token: 0x040017E9 RID: 6121
	public static string PropStompsSpikes = "StompsSpikes";

	// Token: 0x040017EA RID: 6122
	public static string PropUserSpawnType = "UserSpawnType";

	// Token: 0x040017EB RID: 6123
	public static string PropHideInSpawnMenu = "HideInSpawnMenu";

	// Token: 0x040017EC RID: 6124
	public static string PropCanBigHead = "CanBigHead";

	// Token: 0x040017ED RID: 6125
	public static string PropOnActivateEvent = "ActivateEvent";

	// Token: 0x040017EE RID: 6126
	public static string PropCustomCommandName = "CustomCommandName";

	// Token: 0x040017EF RID: 6127
	public static string PropCustomCommandIcon = "CustomCommandIcon";

	// Token: 0x040017F0 RID: 6128
	public static string PropCustomCommandIconColor = "CustomCommandIconColor";

	// Token: 0x040017F1 RID: 6129
	public static string PropCustomCommandEvent = "CustomCommandEvent";

	// Token: 0x040017F2 RID: 6130
	public static readonly int itemClass = EntityClass.FromString("item");

	// Token: 0x040017F3 RID: 6131
	public static readonly int fallingBlockClass = EntityClass.FromString("fallingBlock");

	// Token: 0x040017F4 RID: 6132
	public static readonly int fallingTreeClass = EntityClass.FromString("fallingTree");

	// Token: 0x040017F5 RID: 6133
	public static readonly int playerMaleClass = EntityClass.FromString("playerMale");

	// Token: 0x040017F6 RID: 6134
	public static readonly int playerFemaleClass = EntityClass.FromString("playerFemale");

	// Token: 0x040017F7 RID: 6135
	public static readonly int playerNewMaleClass = EntityClass.FromString("playerNewMale");

	// Token: 0x040017F8 RID: 6136
	public static readonly int junkDoneClass = EntityClass.FromString("entityJunkDrone");

	// Token: 0x040017F9 RID: 6137
	public static Dictionary<string, Color> sColors = new Dictionary<string, Color>();

	// Token: 0x040017FA RID: 6138
	public static DictionarySave<int, EntityClass> list = new DictionarySave<int, EntityClass>();

	// Token: 0x040017FB RID: 6139
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x040017FC RID: 6140
	public Type classname;

	// Token: 0x040017FD RID: 6141
	public int censorMode;

	// Token: 0x040017FE RID: 6142
	public EntityFlags entityFlags;

	// Token: 0x040017FF RID: 6143
	public int censorType;

	// Token: 0x04001800 RID: 6144
	public string prefabPath;

	// Token: 0x04001801 RID: 6145
	public Transform prefabT;

	// Token: 0x04001802 RID: 6146
	public bool IsPrefabCombined;

	// Token: 0x04001803 RID: 6147
	public string meshPath;

	// Token: 0x04001804 RID: 6148
	public Transform mesh;

	// Token: 0x04001805 RID: 6149
	public Transform meshFP;

	// Token: 0x04001806 RID: 6150
	public string skinTexture;

	// Token: 0x04001807 RID: 6151
	public string parentGameObjectName;

	// Token: 0x04001808 RID: 6152
	public string entityClassName;

	// Token: 0x04001809 RID: 6153
	public EntityClass.UserSpawnType userSpawnType = EntityClass.UserSpawnType.Menu;

	// Token: 0x0400180A RID: 6154
	public bool bIsEnemyEntity;

	// Token: 0x0400180B RID: 6155
	public bool bIsAnimalEntity;

	// Token: 0x0400180C RID: 6156
	public ExplosionData explosionData;

	// Token: 0x0400180D RID: 6157
	public Type modelType;

	// Token: 0x0400180E RID: 6158
	public float MassKg;

	// Token: 0x0400180F RID: 6159
	public float SizeScale;

	// Token: 0x04001810 RID: 6160
	public float RagdollOnDeathChance;

	// Token: 0x04001811 RID: 6161
	public bool HasRagdoll;

	// Token: 0x04001812 RID: 6162
	public string CollidersRagdollAsset;

	// Token: 0x04001813 RID: 6163
	public float LookAtAngle;

	// Token: 0x04001814 RID: 6164
	public float crouchYOffsetFP;

	// Token: 0x04001815 RID: 6165
	public string CorpseBlockId;

	// Token: 0x04001816 RID: 6166
	public float CorpseBlockChance;

	// Token: 0x04001817 RID: 6167
	public int CorpseBlockDensity;

	// Token: 0x04001818 RID: 6168
	public float MaxTurnSpeed;

	// Token: 0x04001819 RID: 6169
	public bool RootMotion;

	// Token: 0x0400181A RID: 6170
	public bool HasDeathAnim;

	// Token: 0x0400181B RID: 6171
	public bool bIsMale;

	// Token: 0x0400181C RID: 6172
	public bool bIsChunkObserver;

	// Token: 0x0400181D RID: 6173
	public int ExperienceValue;

	// Token: 0x0400181E RID: 6174
	public int lootDropEntityClass;

	// Token: 0x0400181F RID: 6175
	public PhysicsBodyLayout PhysicsBody;

	// Token: 0x04001820 RID: 6176
	public int DeadBodyHitPoints;

	// Token: 0x04001821 RID: 6177
	public float LegCrippleScale;

	// Token: 0x04001822 RID: 6178
	public float LegCrawlerThreshold;

	// Token: 0x04001823 RID: 6179
	public float DismemberMultiplierHead;

	// Token: 0x04001824 RID: 6180
	public float DismemberMultiplierArms;

	// Token: 0x04001825 RID: 6181
	public float DismemberMultiplierLegs;

	// Token: 0x04001826 RID: 6182
	public float LowerLegDismemberThreshold;

	// Token: 0x04001827 RID: 6183
	public float LowerLegDismemberBonusChance;

	// Token: 0x04001828 RID: 6184
	public float LowerLegDismemberBaseChance;

	// Token: 0x04001829 RID: 6185
	public float UpperLegDismemberThreshold;

	// Token: 0x0400182A RID: 6186
	public float UpperLegDismemberBonusChance;

	// Token: 0x0400182B RID: 6187
	public float UpperLegDismemberBaseChance;

	// Token: 0x0400182C RID: 6188
	public float LowerArmDismemberThreshold;

	// Token: 0x0400182D RID: 6189
	public float LowerArmDismemberBonusChance;

	// Token: 0x0400182E RID: 6190
	public float LowerArmDismemberBaseChance;

	// Token: 0x0400182F RID: 6191
	public float UpperArmDismemberThreshold;

	// Token: 0x04001830 RID: 6192
	public float UpperArmDismemberBonusChance;

	// Token: 0x04001831 RID: 6193
	public float UpperArmDismemberBaseChance;

	// Token: 0x04001832 RID: 6194
	public float KnockdownKneelDamageThreshold;

	// Token: 0x04001833 RID: 6195
	public float LegsExplosionDamageMultiplier;

	// Token: 0x04001834 RID: 6196
	public float ArmsExplosionDamageMultiplier;

	// Token: 0x04001835 RID: 6197
	public float ChestExplosionDamageMultiplier;

	// Token: 0x04001836 RID: 6198
	public float HeadExplosionDamageMultiplier;

	// Token: 0x04001837 RID: 6199
	public float PainResistPerHit;

	// Token: 0x04001838 RID: 6200
	public float PainResistPerHitLowHealth;

	// Token: 0x04001839 RID: 6201
	public float PainResistPerHitLowHealthPercent;

	// Token: 0x0400183A RID: 6202
	public float SearchRadius;

	// Token: 0x0400183B RID: 6203
	public float SwimOffset;

	// Token: 0x0400183C RID: 6204
	public float SightRange;

	// Token: 0x0400183D RID: 6205
	public Vector2 SleeperSightToSenseMin;

	// Token: 0x0400183E RID: 6206
	public Vector2 SleeperSightToSenseMax;

	// Token: 0x0400183F RID: 6207
	public Vector2 SleeperSightToWakeMin;

	// Token: 0x04001840 RID: 6208
	public Vector2 SleeperSightToWakeMax;

	// Token: 0x04001841 RID: 6209
	public Vector2 sightLightThreshold;

	// Token: 0x04001842 RID: 6210
	public Vector2 NoiseAlert;

	// Token: 0x04001843 RID: 6211
	public Vector2 SleeperNoiseToSense;

	// Token: 0x04001844 RID: 6212
	public float SleeperNoiseToSenseSoundChance;

	// Token: 0x04001845 RID: 6213
	public Vector2 SleeperNoiseToWake;

	// Token: 0x04001846 RID: 6214
	public string UMARace;

	// Token: 0x04001847 RID: 6215
	public string UMAGeneratedModelName;

	// Token: 0x04001848 RID: 6216
	public string[] AltMatNames;

	// Token: 0x04001849 RID: 6217
	public string[] MatSwap;

	// Token: 0x0400184A RID: 6218
	public EntityClass.ParticleData particleOnSpawn;

	// Token: 0x0400184B RID: 6219
	public Vector2 KnockdownKneelStunDuration;

	// Token: 0x0400184C RID: 6220
	public float KnockdownProneDamageThreshold;

	// Token: 0x0400184D RID: 6221
	public Vector2 KnockdownProneStunDuration;

	// Token: 0x0400184E RID: 6222
	public Vector2 KnockdownProneRefillRate;

	// Token: 0x0400184F RID: 6223
	public Vector2 KnockdownKneelRefillRate;

	// Token: 0x04001850 RID: 6224
	public Vector3 ModelTransformAdjust;

	// Token: 0x04001851 RID: 6225
	public string ArchetypeName;

	// Token: 0x04001852 RID: 6226
	public string[] AIPackages;

	// Token: 0x04001853 RID: 6227
	public bool UseAIPackages;

	// Token: 0x04001854 RID: 6228
	public Dictionary<EnumDropEvent, List<Block.SItemDropProb>> itemsToDrop = new EnumDictionary<EnumDropEvent, List<Block.SItemDropProb>>();

	// Token: 0x04001855 RID: 6229
	public List<string> Buffs;

	// Token: 0x04001856 RID: 6230
	public FastTags<TagGroup.Global> Tags;

	// Token: 0x04001857 RID: 6231
	public string NavObject = "";

	// Token: 0x04001858 RID: 6232
	public Vector3 NavObjectHeadOffset = Vector3.zero;

	// Token: 0x04001859 RID: 6233
	public bool CanBigHead = true;

	// Token: 0x0400185A RID: 6234
	public int DanceTypeID;

	// Token: 0x0400185B RID: 6235
	public MinEffectController Effects;

	// Token: 0x0400185C RID: 6236
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] commaSeparator = new char[]
	{
		','
	};

	// Token: 0x0400185D RID: 6237
	public string onActivateEvent = "";

	// Token: 0x0200042B RID: 1067
	public enum CensorModeType
	{
		// Token: 0x0400185F RID: 6239
		None,
		// Token: 0x04001860 RID: 6240
		ZPrefab,
		// Token: 0x04001861 RID: 6241
		Dismemberment,
		// Token: 0x04001862 RID: 6242
		ZPrefabAndDismemberment
	}

	// Token: 0x0200042C RID: 1068
	public enum UserSpawnType
	{
		// Token: 0x04001864 RID: 6244
		None,
		// Token: 0x04001865 RID: 6245
		Console,
		// Token: 0x04001866 RID: 6246
		Menu
	}

	// Token: 0x0200042D RID: 1069
	public struct ParticleData
	{
		// Token: 0x04001867 RID: 6247
		public string fileName;

		// Token: 0x04001868 RID: 6248
		public string shapeMesh;
	}
}
