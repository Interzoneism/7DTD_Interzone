using System;
using System.Collections.Generic;
using System.Globalization;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000E6 RID: 230
[Preserve]
public class Block
{
	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000553 RID: 1363 RVA: 0x00025E24 File Offset: 0x00024024
	public static bool BlocksLoaded
	{
		get
		{
			return Block.list != null;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000554 RID: 1364 RVA: 0x00025E2E File Offset: 0x0002402E
	// (set) Token: 0x06000555 RID: 1365 RVA: 0x00025E4F File Offset: 0x0002404F
	public DynamicProperties Properties
	{
		get
		{
			if (this.dynamicProperties != null)
			{
				return this.dynamicProperties;
			}
			return Block.PropertiesCache.Cache(this.blockID);
		}
		set
		{
			this.dynamicProperties = value;
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000556 RID: 1366 RVA: 0x00025E58 File Offset: 0x00024058
	public RecipeUnlockData[] UnlockedBy
	{
		get
		{
			if (this.unlockedBy == null)
			{
				if (this.Properties.Values.ContainsKey(Block.PropUnlockedBy))
				{
					string[] array = this.Properties.Values[Block.PropUnlockedBy].Split(',', StringSplitOptions.None);
					if (array.Length != 0)
					{
						this.unlockedBy = new RecipeUnlockData[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							this.unlockedBy[i] = new RecipeUnlockData(array[i]);
						}
					}
				}
				else
				{
					this.unlockedBy = new RecipeUnlockData[0];
				}
			}
			return this.unlockedBy;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000557 RID: 1367 RVA: 0x00025EE6 File Offset: 0x000240E6
	public bool IsCollideMovement
	{
		get
		{
			return (this.BlockingType & 2) != 0;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000558 RID: 1368 RVA: 0x00025EF3 File Offset: 0x000240F3
	public bool IsCollideSight
	{
		get
		{
			return (this.BlockingType & 1) != 0;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000559 RID: 1369 RVA: 0x00025F00 File Offset: 0x00024100
	public bool IsCollideBullets
	{
		get
		{
			return (this.BlockingType & 4) != 0;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x0600055A RID: 1370 RVA: 0x00025F0D File Offset: 0x0002410D
	public bool IsCollideRockets
	{
		get
		{
			return (this.BlockingType & 8) != 0;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x0600055B RID: 1371 RVA: 0x00025F1A File Offset: 0x0002411A
	public bool IsCollideMelee
	{
		get
		{
			return (this.BlockingType & 16) != 0;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600055C RID: 1372 RVA: 0x00025F28 File Offset: 0x00024128
	public bool IsCollideArrows
	{
		get
		{
			return (this.BlockingType & 32) != 0;
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x0600055D RID: 1373 RVA: 0x00025F36 File Offset: 0x00024136
	// (set) Token: 0x0600055E RID: 1374 RVA: 0x00025F4D File Offset: 0x0002414D
	public bool IsNotifyOnLoadUnload
	{
		get
		{
			return this.bNotifyOnLoadUnload || this.shape.IsNotifyOnLoadUnload;
		}
		set
		{
			this.bNotifyOnLoadUnload = value;
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x0600055F RID: 1375 RVA: 0x00025F56 File Offset: 0x00024156
	// (set) Token: 0x06000560 RID: 1376 RVA: 0x00025F5E File Offset: 0x0002415E
	public List<ShapesFromXml.ShapeCategory> ShapeCategories { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000561 RID: 1377 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool AllowBlockTriggers
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x00025F68 File Offset: 0x00024168
	public Block()
	{
		this.shape = new BlockShapeCube();
		this.shape.Init(this);
		this.Properties = new DynamicProperties();
		this.blockMaterial = MaterialBlock.air;
		this.MeshIndex = 0;
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00026144 File Offset: 0x00024344
	public static Vector3 StringToVector3(string _input)
	{
		Vector3 zero = Vector3.zero;
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 2, 0, -1);
		int num = 255;
		int num2 = 255;
		int num3 = 255;
		StringParsers.TryParseSInt32(_input, out num, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer);
		if (separatorPositions.TotalFound > 0)
		{
			StringParsers.TryParseSInt32(_input, out num2, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer);
		}
		if (separatorPositions.TotalFound > 1)
		{
			StringParsers.TryParseSInt32(_input, out num3, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Integer);
		}
		zero.x = (float)num / 255f;
		zero.y = (float)num2 / 255f;
		zero.z = (float)num3 / 255f;
		return zero;
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00026200 File Offset: 0x00024400
	public virtual void Init()
	{
		if (Block.nameToBlockCaseInsensitive.ContainsKey(this.blockName))
		{
			Log.Error("Block " + this.blockName + " is found multiple times, overriding with latest definition!");
		}
		Block.nameToBlock[this.blockName] = this;
		Block.nameToBlockCaseInsensitive[this.blockName] = this;
		if (this.Properties.Values.ContainsKey(Block.PropTag))
		{
			this.Tags = FastTags<TagGroup.Global>.Parse(this.Properties.Values[Block.PropTag]);
		}
		if (this.Properties.Values.ContainsKey(Block.PropLightOpacity))
		{
			int.TryParse(this.Properties.Values[Block.PropLightOpacity], out this.lightOpacity);
		}
		else
		{
			this.lightOpacity = Math.Max(this.blockMaterial.LightOpacity, (int)this.shape.LightOpacity);
		}
		this.Properties.ParseColorHex(Block.PropTintColor, ref this.tintColor);
		StringParsers.TryParseBool(this.Properties.Values[Block.PropCanPickup], out this.CanPickup, 0, -1, true);
		if (this.CanPickup && this.Properties.Params1.ContainsKey(Block.PropCanPickup))
		{
			this.PickedUpItemValue = this.Properties.Params1[Block.PropCanPickup];
		}
		if (this.Properties.Values.ContainsKey(Block.PropFuelValue))
		{
			int.TryParse(this.Properties.Values[Block.PropFuelValue], out this.FuelValue);
		}
		if (this.Properties.Values.ContainsKey(Block.PropWeight))
		{
			int startValue;
			int.TryParse(this.Properties.Values[Block.PropWeight], out startValue);
			this.Weight = new DataItem<int>(startValue);
		}
		this.CanMobsSpawnOn = false;
		this.Properties.ParseBool(Block.PropCanMobsSpawnOn, ref this.CanMobsSpawnOn);
		this.CanPlayersSpawnOn = true;
		this.Properties.ParseBool(Block.PropCanPlayersSpawnOn, ref this.CanPlayersSpawnOn);
		if (this.Properties.Values.ContainsKey(Block.PropPickupTarget))
		{
			this.PickupTarget = this.Properties.Values[Block.PropPickupTarget];
		}
		if (this.Properties.Values.ContainsKey(Block.PropPickupSource))
		{
			this.PickupSource = this.Properties.Values[Block.PropPickupSource];
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlaceAltBlockValue))
		{
			this.placeAltBlockNames = this.Properties.Values[Block.PropPlaceAltBlockValue].Split(',', StringSplitOptions.None);
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlaceShapeCategories))
		{
			string[] array = this.Properties.Values[Block.PropPlaceShapeCategories].Split(',', StringSplitOptions.None);
			this.ShapeCategories = new List<ShapesFromXml.ShapeCategory>();
			foreach (string text in array)
			{
				ShapesFromXml.ShapeCategory item;
				if (ShapesFromXml.shapeCategories.TryGetValue(text, out item))
				{
					this.ShapeCategories.Add(item);
				}
				else
				{
					Log.Error("Block " + this.blockName + " has unknown ShapeCategory " + text);
				}
			}
		}
		if (this.Properties.Values.ContainsKey(Block.PropIndexName))
		{
			this.IndexName = this.Properties.Values[Block.PropIndexName];
		}
		this.Properties.ParseBool(Block.PropCanBlocksReplace, ref this.CanBlocksReplace);
		this.Properties.ParseBool(Block.PropCanDecorateOnSlopes, ref this.CanDecorateOnSlopes);
		this.SlopeMaxCos = 90f;
		this.Properties.ParseFloat(Block.PropSlopeMax, ref this.SlopeMaxCos);
		this.SlopeMaxCos = Mathf.Cos(this.SlopeMaxCos * 0.017453292f);
		if (this.Properties.Values.ContainsKey(Block.PropIsTerrainDecoration))
		{
			this.IsTerrainDecoration = StringParsers.ParseBool(this.Properties.Values[Block.PropIsTerrainDecoration], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropIsDecoration))
		{
			this.IsDecoration = StringParsers.ParseBool(this.Properties.Values[Block.PropIsDecoration], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropDistantDecoration))
		{
			this.IsDistantDecoration = StringParsers.ParseBool(this.Properties.Values[Block.PropDistantDecoration], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropBigDecorationRadius))
		{
			this.BigDecorationRadius = int.Parse(this.Properties.Values[Block.PropBigDecorationRadius]);
		}
		if (this.Properties.Values.ContainsKey(Block.PropSmallDecorationRadius))
		{
			this.SmallDecorationRadius = int.Parse(this.Properties.Values[Block.PropSmallDecorationRadius]);
		}
		this.Properties.ParseFloat(Block.PropGndAlign, ref this.GroundAlignDistance);
		this.Properties.ParseBool(Block.PropIgnoreKeystoneOverlay, ref this.IgnoreKeystoneOverlay);
		this.LPHardnessScale = 1f;
		if (this.Properties.Values.ContainsKey(Block.PropLPScale))
		{
			this.LPHardnessScale = StringParsers.ParseFloat(this.Properties.Values[Block.PropLPScale], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropMapColor))
		{
			this.MapColor = StringParsers.ParseColor32(this.Properties.Values[Block.PropMapColor]);
			this.bMapColorSet = true;
		}
		if (this.Properties.Values.ContainsKey(Block.PropMapColor2))
		{
			this.MapColor2 = StringParsers.ParseColor32(this.Properties.Values[Block.PropMapColor2]);
			this.bMapColor2Set = true;
		}
		if (this.Properties.Values.ContainsKey(Block.PropMapElevMinMax))
		{
			this.MapElevMinMax = StringParsers.ParseVector2i(this.Properties.Values[Block.PropMapElevMinMax], ',');
		}
		else
		{
			this.MapElevMinMax = Vector2i.zero;
		}
		if (this.Properties.Values.ContainsKey(Block.PropMapSpecular))
		{
			this.MapSpecular = StringParsers.ParseFloat(this.Properties.Values[Block.PropMapSpecular], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropGroupName) && !Block.groupNameStringToGroupNames.TryGetValue(this.Properties.Values[Block.PropGroupName], out this.GroupNames))
		{
			string[] array3 = this.Properties.Values[Block.PropGroupName].Split(',', StringSplitOptions.None);
			if (array3.Length != 0)
			{
				this.GroupNames = new string[array3.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					this.GroupNames[j] = array3[j].Trim();
				}
			}
			Block.groupNameStringToGroupNames.Add(this.Properties.Values[Block.PropGroupName], this.GroupNames);
		}
		if (this.Properties.Values.ContainsKey(Block.PropCustomIcon))
		{
			this.CustomIcon = this.Properties.Values[Block.PropCustomIcon];
		}
		if (this.Properties.Values.ContainsKey(Block.PropCustomIconTint))
		{
			this.CustomIconTint = StringParsers.ParseHexColor(this.Properties.Values[Block.PropCustomIconTint]);
		}
		else
		{
			this.CustomIconTint = Color.white;
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlacementWireframe))
		{
			this.bHasPlacementWireframe = StringParsers.ParseBool(this.Properties.Values[Block.PropPlacementWireframe], 0, -1, true);
		}
		else
		{
			this.bHasPlacementWireframe = true;
		}
		this.isOversized = this.Properties.Values.ContainsKey(Block.PropOversizedBounds);
		if (this.isOversized)
		{
			this.oversizedBounds = StringParsers.ParseBounds(this.Properties.Values[Block.PropOversizedBounds]);
		}
		else
		{
			this.oversizedBounds = default(Bounds);
		}
		if (this.Properties.Values.ContainsKey(Block.PropMultiBlockDim))
		{
			this.isMultiBlock = true;
			Vector3i vector3i = StringParsers.ParseVector3i(this.Properties.Values[Block.PropMultiBlockDim], 0, -1, false);
			List<Vector3i> list = new List<Vector3i>();
			if (this.Properties.Values.ContainsKey(Block.PropMultiBlockLayer0))
			{
				int num = 0;
				while (this.Properties.Values.ContainsKey(Block.PropMultiBlockLayer + num.ToString()))
				{
					string[] array4 = this.Properties.Values[Block.PropMultiBlockLayer + num.ToString()].Split(',', StringSplitOptions.None);
					for (int k = 0; k < array4.Length; k++)
					{
						array4[k] = array4[k].Trim();
						if (array4[k].Length > vector3i.x)
						{
							throw new Exception("Multi block layer entry " + k.ToString() + " too long for block " + this.blockName);
						}
						for (int l = 0; l < array4[k].Length; l++)
						{
							if (array4[k][l] != ' ')
							{
								list.Add(new Vector3i(l, num, k));
							}
						}
					}
					num++;
				}
			}
			else
			{
				int num2 = vector3i.x / 2;
				int num3 = Mathf.RoundToInt((float)vector3i.x / 2f + 0.1f) - 1;
				int num4 = vector3i.z / 2;
				int num5 = Mathf.RoundToInt((float)vector3i.z / 2f + 0.1f) - 1;
				for (int m = -num2; m <= num3; m++)
				{
					for (int n = 0; n < vector3i.y; n++)
					{
						for (int num6 = -num4; num6 <= num5; num6++)
						{
							list.Add(new Vector3i(m, n, num6));
						}
					}
				}
			}
			this.multiBlockPos = new Block.MultiBlockArray(vector3i, list);
		}
		if (this.Properties.Values.ContainsKey(Block.PropTerrainAlignment))
		{
			this.terrainAlignmentMode = EnumUtils.Parse<TerrainAlignmentMode>(this.Properties.Values[Block.PropTerrainAlignment], false);
			if (this.terrainAlignmentMode != TerrainAlignmentMode.None)
			{
				bool flag = this.shape is BlockShapeModelEntity;
				bool flag2 = this.isOversized || this.isMultiBlock;
				if (!flag || !flag2)
				{
					Debug.LogWarning(string.Format("Failed to apply TerrainAlignmentMode \"{0}\" to {1}. ", this.terrainAlignmentMode, this.blockName) + "Terrain alignment is only supported for oversized- and multi-blocks with shape type \"ModelEntity\".\n" + string.Format("isModelEntity: {0}. isOversized: {1}. isMultiBlock: {2}. ", flag, this.isOversized, this.isMultiBlock));
					this.terrainAlignmentMode = TerrainAlignmentMode.None;
				}
			}
		}
		else
		{
			this.terrainAlignmentMode = TerrainAlignmentMode.None;
		}
		this.Properties.ParseFloat(Block.PropHeatMapStrength, ref this.HeatMapStrength);
		this.FallDamage = 1f;
		if (this.Properties.Values.ContainsKey(Block.PropFallDamage))
		{
			this.FallDamage = StringParsers.ParseFloat(this.Properties.Values[Block.PropFallDamage], 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropCount))
		{
			this.Count = int.Parse(this.Properties.Values[Block.PropCount]);
		}
		if (this.Properties.Values.ContainsKey("ImposterExclude"))
		{
			this.bImposterExclude = StringParsers.ParseBool(this.Properties.Values["ImposterExclude"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey("ImposterExcludeAndStop"))
		{
			this.bImposterExcludeAndStop = StringParsers.ParseBool(this.Properties.Values["ImposterExcludeAndStop"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey("ImposterDontBlock"))
		{
			this.bImposterDontBlock = StringParsers.ParseBool(this.Properties.Values["ImposterDontBlock"], 0, -1, true);
		}
		this.AllowedRotations = EBlockRotationClasses.No45;
		if (this.shape is BlockShapeModelEntity)
		{
			this.AllowedRotations |= EBlockRotationClasses.Basic45;
		}
		if (this.Properties.Values.ContainsKey(Block.PropAllowAllRotations) && StringParsers.ParseBool(this.Properties.Values[Block.PropAllowAllRotations], 0, -1, true))
		{
			this.AllowedRotations |= EBlockRotationClasses.Basic45;
		}
		if (this.Properties.Values.ContainsKey("OnlySimpleRotations") && StringParsers.ParseBool(this.Properties.Values["OnlySimpleRotations"], 0, -1, true))
		{
			this.AllowedRotations &= ~(EBlockRotationClasses.Headfirst | EBlockRotationClasses.Sideways);
		}
		if (this.Properties.Values.ContainsKey("AllowedRotations"))
		{
			this.AllowedRotations = EBlockRotationClasses.None;
			foreach (string text2 in this.Properties.Values["AllowedRotations"].Split(',', StringSplitOptions.None))
			{
				EBlockRotationClasses eblockRotationClasses;
				if (EnumUtils.TryParse<EBlockRotationClasses>(text2, out eblockRotationClasses, true))
				{
					this.AllowedRotations |= eblockRotationClasses;
				}
				else
				{
					Log.Error(string.Concat(new string[]
					{
						"Rotation class '",
						text2,
						"' not found for block '",
						this.blockName,
						"'"
					}));
				}
			}
		}
		if (this.Properties.Values.ContainsKey("PlaceAsRandomRotation"))
		{
			this.PlaceRandomRotation = StringParsers.ParseBool(this.Properties.Values["PlaceAsRandomRotation"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropIsPlant))
		{
			this.bIsPlant = StringParsers.ParseBool(this.Properties.Values[Block.PropIsPlant], 0, -1, true);
		}
		this.Properties.ParseString("CustomPlaceSound", ref this.CustomPlaceSound);
		this.Properties.ParseString("UpgradeSound", ref this.UpgradeSound);
		this.Properties.ParseString("DowngradeFX", ref this.DowngradeFX);
		this.Properties.ParseString("DestroyFX", ref this.DestroyFX);
		if (this.Properties.Values.ContainsKey(Block.PropBuffsWhenWalkedOn))
		{
			this.BuffsWhenWalkedOn = this.Properties.Values[Block.PropBuffsWhenWalkedOn].Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (this.BuffsWhenWalkedOn.Length < 1)
			{
				this.BuffsWhenWalkedOn = null;
			}
		}
		this.Properties.ParseBool(Block.PropIsReplaceRandom, ref this.IsReplaceRandom);
		if (this.Properties.Values.ContainsKey(Block.PropCraftExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropCraftExpValue], out this.CraftComponentExp, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropCraftTimeValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropCraftTimeValue], out this.CraftComponentTime, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropLootExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropLootExpValue], out this.LootExp, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropDestroyExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropDestroyExpValue], out this.DestroyExp, 0, -1, NumberStyles.Any);
		}
		this.Properties.ParseString(Block.PropParticleOnDeath, ref this.deathParticleName);
		if (this.Properties.Values.ContainsKey(Block.PropPlaceExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropPlaceExpValue], out this.PlaceExp, 0, -1, NumberStyles.Any);
		}
		if (this.Properties.Values.ContainsKey(Block.PropUpgradeExpValue))
		{
			StringParsers.TryParseFloat(this.Properties.Values[Block.PropUpgradeExpValue], out this.UpgradeExp, 0, -1, NumberStyles.Any);
		}
		this.Properties.ParseFloat(Block.PropEconomicValue, ref this.EconomicValue);
		this.Properties.ParseFloat(Block.PropEconomicSellScale, ref this.EconomicSellScale);
		this.Properties.ParseInt(Block.PropEconomicBundleSize, ref this.EconomicBundleSize);
		if (this.Properties.Values.ContainsKey(Block.PropSellableToTrader))
		{
			StringParsers.TryParseBool(this.Properties.Values[Block.PropSellableToTrader], out this.SellableToTrader, 0, -1, true);
		}
		this.Properties.ParseString(Block.PropTraderStageTemplate, ref this.TraderStageTemplate);
		if (this.Properties.Values.ContainsKey(Block.PropCreativeMode))
		{
			this.CreativeMode = EnumUtils.Parse<EnumCreativeMode>(this.Properties.Values[Block.PropCreativeMode], false);
		}
		if (this.Properties.Values.ContainsKey(Block.PropFilterTag))
		{
			this.FilterTags = this.Properties.Values[Block.PropFilterTag].Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (this.FilterTags.Length < 1)
			{
				this.FilterTags = null;
			}
		}
		this.SortOrder = this.Properties.GetString(Block.PropCreativeSort1);
		this.SortOrder += this.Properties.GetString(Block.PropCreativeSort2);
		if (this.Properties.Values.ContainsKey(Block.PropDisplayType))
		{
			this.DisplayType = this.Properties.Values[Block.PropDisplayType];
		}
		if (this.Properties.Values.ContainsKey(Block.PropItemTypeIcon))
		{
			this.ItemTypeIcon = this.Properties.Values[Block.PropItemTypeIcon];
		}
		if (this.Properties.Values.ContainsKey(Block.PropAutoShape))
		{
			this.AutoShapeType = EnumUtils.Parse<EAutoShapeType>(this.Properties.Values[Block.PropAutoShape], false);
			if (this.AutoShapeType != EAutoShapeType.None)
			{
				string[] array5 = this.blockName.Split(':', StringSplitOptions.None);
				this.autoShapeBaseName = array5[0];
				this.autoShapeShapeName = array5[1];
				Block.autoShapeMaterials.Add(this.autoShapeBaseName);
			}
		}
		this.MaxDamage = this.blockMaterial.MaxDamage;
		this.Properties.ParseInt(Block.PropMaxDamage, ref this.MaxDamage);
		this.Properties.ParseInt(Block.PropStartDamage, ref this.StartDamage);
		this.Properties.ParseInt(Block.PropStage2Health, ref this.Stage2Health);
		this.Properties.ParseFloat(Block.PropDamage, ref this.Damage);
		if (this.Properties.Values.ContainsKey(Block.PropActivationDistance))
		{
			int.TryParse(this.Properties.Values[Block.PropActivationDistance], out this.activationDistance);
		}
		if (this.Properties.Values.ContainsKey(Block.PropPlacementDistance))
		{
			int.TryParse(this.Properties.Values[Block.PropPlacementDistance], out this.placementDistance);
		}
		if (this.Properties.Values.ContainsKey("PassThroughDamage"))
		{
			this.EnablePassThroughDamage = StringParsers.ParseBool(this.Properties.Values["PassThroughDamage"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey("CopyPaintOnDowngrade"))
		{
			string[] array6 = this.Properties.Values["CopyPaintOnDowngrade"].Split(',', StringSplitOptions.None);
			HashSet<BlockFace> hashSet = new HashSet<BlockFace>();
			for (int num7 = 0; num7 < array6.Length; num7++)
			{
				char c = array6[num7][0];
				if (c <= 'E')
				{
					if (c != 'B')
					{
						if (c == 'E')
						{
							hashSet.Add(BlockFace.East);
						}
					}
					else
					{
						hashSet.Add(BlockFace.Bottom);
					}
				}
				else if (c != 'N')
				{
					switch (c)
					{
					case 'S':
						hashSet.Add(BlockFace.South);
						break;
					case 'T':
						hashSet.Add(BlockFace.Top);
						break;
					case 'W':
						hashSet.Add(BlockFace.West);
						break;
					}
				}
				else
				{
					hashSet.Add(BlockFace.North);
				}
			}
			this.RemovePaintOnDowngrade = new List<BlockFace>();
			for (int num8 = 0; num8 < 6; num8++)
			{
				if (!hashSet.Contains((BlockFace)num8))
				{
					this.RemovePaintOnDowngrade.Add((BlockFace)num8);
				}
			}
		}
		for (int num9 = 0; num9 < 1; num9++)
		{
			string useGlobalUV = ShapesFromXml.TextureLabelsByChannel[num9].UseGlobalUV;
			string @string = this.Properties.GetString(useGlobalUV);
			if (@string.Length > 0)
			{
				this.UVModesPerSide[num9] = 0U;
				if (!@string.Contains(","))
				{
					char c2 = @string[0];
					Block.UVMode uvmode = (c2 == 'G') ? Block.UVMode.Global : ((c2 == 'L') ? Block.UVMode.Local : Block.UVMode.Default);
					for (int num10 = 0; num10 < this.cUVModeSides; num10++)
					{
						this.UVModesPerSide[num9] |= (uint)((uint)uvmode << (num10 * this.cUVModeBits & 31));
					}
				}
				else
				{
					int num11 = 0;
					foreach (char c3 in @string)
					{
						if (c3 != ',')
						{
							Block.UVMode uvmode2 = (c3 == 'G') ? Block.UVMode.Global : ((c3 == 'L') ? Block.UVMode.Local : Block.UVMode.Default);
							this.UVModesPerSide[num9] |= (uint)((uint)uvmode2 << (num11 & 31));
							num11 += this.cUVModeBits;
						}
					}
				}
			}
		}
		if (this.Properties.Values.ContainsKey(Block.PropRadiusEffect))
		{
			string[] array7 = this.Properties.Values[Block.PropRadiusEffect].Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries);
			List<BlockRadiusEffect> list2 = new List<BlockRadiusEffect>();
			foreach (string text3 in array7)
			{
				int num13 = text3.IndexOf('(');
				int num14 = text3.IndexOf(')');
				BlockRadiusEffect item2 = default(BlockRadiusEffect);
				if (num13 != -1 && num14 != -1 && num14 > num13 + 1)
				{
					item2.radius = StringParsers.ParseFloat(text3.Substring(num13 + 1, num14 - num13 - 1), 0, -1, NumberStyles.Any);
					item2.variable = text3.Substring(0, num13);
				}
				else
				{
					item2.radius = 1f;
					item2.variable = text3;
				}
				list2.Add(item2);
			}
			this.RadiusEffects = list2.ToArray();
		}
		else
		{
			this.RadiusEffects = null;
		}
		if (this.Properties.Values.ContainsKey(Block.PropDescriptionKey))
		{
			this.DescriptionKey = this.Properties.Values[Block.PropDescriptionKey];
		}
		else
		{
			this.DescriptionKey = string.Format("{0}Desc", this.blockName);
			if (!Localization.Exists(this.DescriptionKey, false))
			{
				this.DescriptionKey = Block.defaultBlockDescriptionKey;
			}
		}
		if (this.Properties.Values.ContainsKey(Block.PropCraftingSkillGroup))
		{
			this.CraftingSkillGroup = this.Properties.Values[Block.PropCraftingSkillGroup];
		}
		else
		{
			this.CraftingSkillGroup = "";
		}
		if (this.Properties.Values.ContainsKey(Block.PropHarvestOverdamage))
		{
			this.HarvestOverdamage = StringParsers.ParseBool(this.Properties.Values[Block.PropHarvestOverdamage], 0, -1, true);
		}
		this.bShowModelOnFall = (!this.Properties.Values.ContainsKey(Block.PropShowModelOnFall) || StringParsers.ParseBool(this.Properties.Values[Block.PropShowModelOnFall], 0, -1, true));
		if (this.Properties.Values.ContainsKey("HandleFace"))
		{
			this.HandleFace = EnumUtils.Parse<BlockFace>(this.Properties.Values["HandleFace"], false);
		}
		if (this.Properties.Values.ContainsKey("DisplayInfo"))
		{
			this.DisplayInfo = EnumUtils.Parse<Block.EnumDisplayInfo>(this.Properties.Values["DisplayInfo"], false);
		}
		if (this.Properties.Values.ContainsKey("SelectAlternates"))
		{
			this.SelectAlternates = StringParsers.ParseBool(this.Properties.Values["SelectAlternates"], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(Block.PropNoScrapping))
		{
			this.NoScrapping = StringParsers.ParseBool(this.Properties.Values[Block.PropNoScrapping], 0, -1, true);
		}
		this.VehicleHitScale = 1f;
		this.Properties.ParseFloat(Block.PropVehicleHitScale, ref this.VehicleHitScale);
		if (this.Properties.Values.ContainsKey("UiBackgroundTexture") && !StringParsers.TryParseSInt32(this.Properties.Values["UiBackgroundTexture"], out this.uiBackgroundTextureId, 0, -1, NumberStyles.Integer))
		{
			this.uiBackgroundTextureId = -1;
		}
		this.Properties.ParseString(Block.PropBlockAddedEvent, ref this.blockAddedEvent);
		this.Properties.ParseString(Block.PropBlockDestroyedEvent, ref this.blockDestroyedEvent);
		this.Properties.ParseString(Block.PropBlockDowngradeEvent, ref this.blockDowngradeEvent);
		this.Properties.ParseString(Block.PropBlockDowngradedToEvent, ref this.blockDowngradedToEvent);
		this.Properties.ParseBool(Block.PropIsTemporaryBlock, ref this.IsTemporaryBlock);
		this.Properties.ParseBool(Block.PropRefundOnUnload, ref this.RefundOnUnload);
		this.Properties.ParseString(Block.PropSoundPickup, ref this.SoundPickup);
		this.Properties.ParseString(Block.PropSoundPlace, ref this.SoundPlace);
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x00027BE0 File Offset: 0x00025DE0
	public virtual void LateInit()
	{
		this.shape.LateInit();
		if (this.AutoShapeType == EAutoShapeType.Shape)
		{
			this.autoShapeHelper = Block.GetBlockByName(this.autoShapeBaseName + ":" + ShapesFromXml.VariantHelperName, false);
		}
		if (this.Properties.Values.ContainsKey(Block.PropSiblingBlock))
		{
			this.SiblingBlock = ItemClass.GetItem(this.Properties.Values[Block.PropSiblingBlock], false).ToBlockValue();
			if (this.SiblingBlock.Equals(BlockValue.Air))
			{
				throw new Exception("Block with name '" + this.Properties.Values[Block.PropSiblingBlock] + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.SiblingBlock = BlockValue.Air;
		}
		if (this.Properties.Values.ContainsKey("MirrorSibling"))
		{
			string text = this.Properties.Values["MirrorSibling"];
			this.MirrorSibling = ItemClass.GetItem(text, false).ToBlockValue().type;
			if (this.MirrorSibling == 0)
			{
				throw new Exception("Block with name '" + text + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.MirrorSibling = 0;
		}
		if (this.Properties.Values.ContainsKey(Block.PropUpgradeBlockClassToBlock))
		{
			this.UpgradeBlock = Block.GetBlockValue(this.Properties.Values[Block.PropUpgradeBlockClassToBlock], false);
			if (this.UpgradeBlock.isair)
			{
				throw new Exception("Block with name '" + this.Properties.Values[Block.PropUpgradeBlockClassToBlock] + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.UpgradeBlock = BlockValue.Air;
		}
		if (this.Properties.Values.ContainsKey(Block.PropDowngradeBlock))
		{
			this.DowngradeBlock = Block.GetBlockValue(this.Properties.Values[Block.PropDowngradeBlock], false);
			if (this.DowngradeBlock.isair)
			{
				throw new Exception("Block with name '" + this.Properties.Values[Block.PropDowngradeBlock] + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.DowngradeBlock = BlockValue.Air;
		}
		if (this.Properties.Values.ContainsKey(Block.PropLockpickDowngradeBlock))
		{
			this.LockpickDowngradeBlock = Block.GetBlockValue(this.Properties.Values[Block.PropLockpickDowngradeBlock], false);
			if (this.LockpickDowngradeBlock.isair)
			{
				throw new Exception("Block with name '" + this.Properties.Values[Block.PropLockpickDowngradeBlock] + "' not found in block " + this.blockName);
			}
		}
		else
		{
			this.LockpickDowngradeBlock = this.DowngradeBlock;
		}
		if (this.Properties.Values.ContainsKey("ImposterExchange"))
		{
			this.ImposterExchange = Block.GetBlockValue(this.Properties.Values["ImposterExchange"], false).type;
			if (this.Properties.Params1.ContainsKey("ImposterExchange"))
			{
				this.ImposterExchangeTexIdx = (byte)int.Parse(this.Properties.Params1["ImposterExchange"]);
			}
		}
		if (this.Properties.Values.ContainsKey("MergeInto"))
		{
			this.MergeIntoId = Block.GetBlockValue(this.Properties.Values["MergeInto"], false).type;
			if (this.MergeIntoId == 0)
			{
				Log.Warning("Warning: MergeInto block with name '{0}' not found!", new object[]
				{
					this.Properties.Values["MergeInto"]
				});
			}
			if (this.Properties.Params1.ContainsKey("MergeInto"))
			{
				string[] array = this.Properties.Params1["MergeInto"].Split(',', StringSplitOptions.None);
				if (array.Length == 6)
				{
					this.MergeIntoTexIds = new int[6];
					for (int i = 0; i < this.MergeIntoTexIds.Length; i++)
					{
						this.MergeIntoTexIds[i] = int.Parse(array[i].Trim());
					}
				}
			}
		}
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			Block.PropertiesCache.Store(this.blockID, this.dynamicProperties);
			this.dynamicProperties = null;
		}
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00028020 File Offset: 0x00026220
	public static void InitStatic()
	{
		Block.nameToBlock = new Dictionary<string, Block>();
		Block.nameToBlockCaseInsensitive = new CaseInsensitiveStringDictionary<Block>();
		Block.list = new Block[Block.MAX_BLOCKS];
		Block.autoShapeMaterials = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		Block.groupNameStringToGroupNames = new Dictionary<string, string[]>();
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			Block.PropertiesCache = new DynamicPropertiesCache();
		}
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0002807C File Offset: 0x0002627C
	public static void LateInitAll()
	{
		for (int i = 0; i < Block.MAX_BLOCKS; i++)
		{
			if (Block.list[i] != null)
			{
				Block.list[i].LateInit();
			}
		}
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			GC.Collect();
		}
		int type = BlockValue.Air.type;
		for (int j = 0; j < Block.MAX_BLOCKS; j++)
		{
			Block block = Block.list[j];
			if (block != null)
			{
				int num = block.MaxDamage;
				int num2 = 0;
				int type2 = block.DowngradeBlock.type;
				while (type2 != type)
				{
					Block block2 = Block.list[type2];
					num += block2.MaxDamage;
					type2 = block2.DowngradeBlock.type;
					if (++num2 > 10)
					{
						Log.Warning("Block '{0}' over downgrade limit", new object[]
						{
							block.blockName
						});
						break;
					}
				}
				block.MaxDamagePlusDowngrades = num;
			}
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x00028157 File Offset: 0x00026357
	public static void OnWorldUnloaded()
	{
		if (PlatformOptimizations.FileBackedBlockProperties)
		{
			DynamicPropertiesCache propertiesCache = Block.PropertiesCache;
			if (propertiesCache != null)
			{
				propertiesCache.Cleanup();
			}
			Block.PropertiesCache = null;
		}
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool FilterIndexType(BlockValue bv)
	{
		return true;
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x00028176 File Offset: 0x00026376
	public Vector2 GetPathOffset(int _rotation)
	{
		if (this.PathType != -1)
		{
			return Vector2.zero;
		}
		return this.shape.GetPathOffset(_rotation);
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x00028193 File Offset: 0x00026393
	public static void Cleanup()
	{
		Block.nameToBlock = null;
		Block.nameToBlockCaseInsensitive = null;
		Block.groupNameStringToGroupNames = null;
		Block.list = null;
		Block.autoShapeMaterials = null;
		Block.fullMappingDataForClients = null;
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x000281BC File Offset: 0x000263BC
	public void CopyDroppedFrom(Block _other)
	{
		foreach (KeyValuePair<EnumDropEvent, List<Block.SItemDropProb>> keyValuePair in _other.itemsToDrop)
		{
			EnumDropEvent key = keyValuePair.Key;
			List<Block.SItemDropProb> value = keyValuePair.Value;
			List<Block.SItemDropProb> list = this.itemsToDrop.ContainsKey(key) ? this.itemsToDrop[key] : null;
			if (list == null)
			{
				list = new List<Block.SItemDropProb>();
				this.itemsToDrop[key] = list;
			}
			for (int i = 0; i < value.Count; i++)
			{
				bool flag = true;
				int num = 0;
				while (flag && num < list.Count)
				{
					if (list[num].name == value[i].name)
					{
						flag = false;
					}
					num++;
				}
				if (flag)
				{
					list.Add(value[i]);
				}
			}
		}
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x000282C0 File Offset: 0x000264C0
	public virtual BlockFace getInventoryFace()
	{
		return BlockFace.North;
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x000282C3 File Offset: 0x000264C3
	public virtual byte GetLightValue(BlockValue _blockValue)
	{
		return this.lightValue;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x000282CB File Offset: 0x000264CB
	public virtual Block SetLightValue(float _lightValueInPercent)
	{
		this.lightValue = (byte)(15f * _lightValueInPercent);
		return this;
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool UseBuffsWhenWalkedOn(World world, Vector3i _blockPos, BlockValue _blockValue)
	{
		return true;
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x000282DC File Offset: 0x000264DC
	public virtual bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlocked {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlocked(_world, parentPos, block, _face);
		}
		else
		{
			if (!this.IsCollideMovement)
			{
				return false;
			}
			if (this.BlocksMovement == 0)
			{
				return this.shape.IsMovementBlocked(_blockValue, _face);
			}
			return this.BlocksMovement == 1;
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00028384 File Offset: 0x00026584
	public virtual bool IsSeeThrough(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.isMultiBlock || !_blockValue.ischild)
		{
			return !this.IsCollideSight && !_world.IsWater(_blockPos);
		}
		Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
		BlockValue block = _world.GetBlock(parentPos);
		if (block.ischild)
		{
			Log.Error("IsSeeThrough {0} at {1} has child parent, {2} at {3} ", new object[]
			{
				this,
				_blockPos,
				block.Block,
				parentPos
			});
			return true;
		}
		return this.IsSeeThrough(_world, _clrIdx, parentPos, block);
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00028414 File Offset: 0x00026614
	public virtual bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlocked {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlocked(_world, parentPos, block, _sides);
		}
		else
		{
			if (_sides == BlockFaceFlag.None)
			{
				return this.IsMovementBlocked(_world, _blockPos, _blockValue, BlockFace.None);
			}
			for (int i = 0; i <= 5; i++)
			{
				if ((1 << i & (int)_sides) != 0 && !this.IsMovementBlocked(_world, _blockPos, _blockValue, (BlockFace)i))
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x000284C9 File Offset: 0x000266C9
	public virtual bool IsWaterBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		return this.IsMovementBlocked(_world, _blockPos, _blockValue, _sides);
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x000284D8 File Offset: 0x000266D8
	public bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, Vector3 _entityPos)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlocked {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlocked(_world, parentPos, block, _entityPos);
		}
		else
		{
			BlockFaceFlag blockFaceFlag = BlockFaceFlags.FrontSidesFromPosition(_blockPos, _entityPos);
			if (blockFaceFlag == BlockFaceFlag.None)
			{
				return this.IsMovementBlocked(_world, _blockPos, _blockValue, BlockFace.None);
			}
			for (int i = 2; i <= 5; i++)
			{
				if ((1 << i & (int)blockFaceFlag) != 0 && !this.IsMovementBlocked(_world, _blockPos, _blockValue, (BlockFace)i))
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x00028594 File Offset: 0x00026794
	public bool IsMovementBlockedAny(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, Vector3 _entityPos)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("IsMovementBlockedAny {0} at {1} has child parent, {2} at {3} ", new object[]
				{
					this,
					_blockPos,
					block.Block,
					parentPos
				});
				return true;
			}
			return this.IsMovementBlockedAny(_world, parentPos, block, _entityPos);
		}
		else
		{
			BlockFaceFlag blockFaceFlag = BlockFaceFlags.FrontSidesFromPosition(_blockPos, _entityPos);
			if (blockFaceFlag == BlockFaceFlag.None)
			{
				return this.IsMovementBlocked(_world, _blockPos, _blockValue, BlockFace.None);
			}
			for (int i = 2; i <= 5; i++)
			{
				if ((1 << i & (int)blockFaceFlag) != 0 && this.IsMovementBlocked(_world, _blockPos, _blockValue, (BlockFace)i))
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00028650 File Offset: 0x00026850
	public virtual float GetStepHeight(IBlockAccess world, Vector3i blockPos, BlockValue blockDef, BlockFace stepFace)
	{
		if (!this.IsCollideMovement)
		{
			return 0f;
		}
		return this.shape.GetStepHeight(blockDef, stepFace);
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00028670 File Offset: 0x00026870
	public float MinStepHeight(BlockValue blockDef, BlockFaceFlag stepSides)
	{
		float num = -1f;
		for (int i = 2; i <= 5; i++)
		{
			if ((1 << i & (int)stepSides) != 0)
			{
				if (num < 0f)
				{
					num = this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i);
				}
				else
				{
					num = Math.Min(num, this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i));
				}
			}
		}
		return Math.Max(num, 0f);
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x000286D4 File Offset: 0x000268D4
	public float MaxStepHeight(BlockValue blockDef, BlockFaceFlag stepSides)
	{
		float num = -1f;
		for (int i = 2; i <= 5; i++)
		{
			if ((1 << i & (int)stepSides) != 0)
			{
				if (num < 0f)
				{
					num = this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i);
				}
				else
				{
					num = Math.Max(num, this.GetStepHeight(null, Vector3i.zero, blockDef, (BlockFace)i));
				}
			}
		}
		return Math.Max(num, 0f);
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00028738 File Offset: 0x00026938
	public float MinStepHeight(Vector3i blockPos, BlockValue blockDef, Vector3 entityPos)
	{
		BlockFaceFlag stepSides = BlockFaceFlags.FrontSidesFromPosition(blockPos, entityPos);
		return this.MinStepHeight(blockDef, stepSides);
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00028758 File Offset: 0x00026958
	public float MaxStepHeight(Vector3i blockPos, BlockValue blockDef, Vector3 entityPos)
	{
		BlockFaceFlag stepSides = BlockFaceFlags.FrontSidesFromPosition(blockPos, entityPos);
		return this.MaxStepHeight(blockDef, stepSides);
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x00028775 File Offset: 0x00026975
	public virtual float GetHardness()
	{
		return this.blockMaterial.Hardness.Value;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00028788 File Offset: 0x00026988
	public virtual int GetWeight()
	{
		int result = 0;
		if (this.Weight != null)
		{
			result = this.Weight.Value;
		}
		return result;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x000287AC File Offset: 0x000269AC
	public Block.UVMode GetUVMode(int side, int channel)
	{
		return (Block.UVMode)((ulong)(this.UVModesPerSide[channel] >> side * this.cUVModeBits) & (ulong)((long)this.cUVModeMask));
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x000287CC File Offset: 0x000269CC
	public virtual Rect getUVRectFromSideAndMetadata(int _meshIndex, BlockFace _side, Vector3[] _vertices, BlockValue _blockValue)
	{
		return this.getUVRectFromSideAndMetadata(_meshIndex, _side, (_vertices != null) ? _vertices[0] : Vector3.zero, _blockValue);
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x000287EC File Offset: 0x000269EC
	public virtual Rect getUVRectFromSideAndMetadata(int _meshIndex, BlockFace _side, Vector3 _worldPos, BlockValue _blockValue)
	{
		int sideTextureId = this.GetSideTextureId(_blockValue, _side, 0);
		if (sideTextureId < 0)
		{
			return UVRectTiling.Empty.uv;
		}
		UVRectTiling[] uvMapping = MeshDescription.meshes[_meshIndex].textureAtlas.uvMapping;
		if (sideTextureId >= uvMapping.Length)
		{
			return UVRectTiling.Empty.uv;
		}
		UVRectTiling uvrectTiling = uvMapping[sideTextureId];
		if (uvrectTiling.blockW == 1 && uvrectTiling.blockH == 1)
		{
			return uvrectTiling.uv;
		}
		float x = _worldPos.x;
		float y = _worldPos.y;
		float z = _worldPos.z;
		switch (_side)
		{
		case BlockFace.Top:
			return new Rect(uvrectTiling.uv.x + (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.Bottom:
			return new Rect(uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(uvrectTiling.blockW - 1) - (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.North:
			return new Rect(uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(uvrectTiling.blockW - 1) - (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.West:
			return new Rect(uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(uvrectTiling.blockW - 1) - (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.South:
			return new Rect(uvrectTiling.uv.x + (float)Utils.FastRoundToIntAndMod(x, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		case BlockFace.East:
			return new Rect(uvrectTiling.uv.x + (float)Utils.FastRoundToIntAndMod(z, uvrectTiling.blockW) * uvrectTiling.uv.width, uvrectTiling.uv.y + (float)Utils.FastRoundToIntAndMod(y, uvrectTiling.blockH) * uvrectTiling.uv.height, uvrectTiling.uv.width, uvrectTiling.uv.height);
		default:
			return new Rect(0f, 0f, 0f, 0f);
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x00028B80 File Offset: 0x00026D80
	public virtual void GetCollidingAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedAddY, Bounds _aabb, List<Bounds> _aabbList)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedAddY, Block.staticList_IntersectRayWithBlockList);
		for (int i = 0; i < Block.staticList_IntersectRayWithBlockList.Count; i++)
		{
			Bounds bounds = Block.staticList_IntersectRayWithBlockList[i];
			if (_aabb.Intersects(bounds))
			{
				_aabbList.Add(bounds);
			}
		}
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00028BDC File Offset: 0x00026DDC
	public virtual bool HasCollidingAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedAddY, Bounds _aabb)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedAddY, Block.staticList_IntersectRayWithBlockList);
		for (int i = 0; i < Block.staticList_IntersectRayWithBlockList.Count; i++)
		{
			Bounds bounds = Block.staticList_IntersectRayWithBlockList[i];
			if (_aabb.Intersects(bounds))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x00028C34 File Offset: 0x00026E34
	public virtual void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedAddY, List<Bounds> _result)
	{
		Vector3 b = new Vector3(0f, _distortedAddY, 0f);
		foreach (Bounds item in this.shape.GetBounds(_blockValue))
		{
			item.center += new Vector3((float)_x, (float)_y, (float)_z);
			item.max += b;
			_result.Add(item);
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00028CB0 File Offset: 0x00026EB0
	public virtual IList<Bounds> GetClipBoundsList(BlockValue _blockValue, Vector3 _blockPos)
	{
		return this.shape.GetBounds(_blockValue);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		return false;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void DoExchangeAction(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, string _action, int _itemCount)
	{
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00028CBE File Offset: 0x00026EBE
	public virtual void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!_blockValue.ischild)
		{
			this.shape.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		}
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00028CD9 File Offset: 0x00026ED9
	public virtual void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!_blockValue.ischild)
		{
			this.shape.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
		}
		if (this.RefundOnUnload)
		{
			GameEventManager.Current.RefundSpawnedBlock(_blockPos);
		}
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00028D08 File Offset: 0x00026F08
	public static bool CanFallBelow(WorldBase _world, int _x, int _y, int _z)
	{
		BlockValue block = _world.GetBlock(_x, _y - 1, _z);
		Block block2 = block.Block;
		return block.isair || !block2.StabilitySupport;
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00028D3D File Offset: 0x00026F3D
	public virtual ulong GetTickRate()
	{
		return 10UL;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00028D44 File Offset: 0x00026F44
	public virtual void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		this.shape.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
		MultiBlockManager.TrackedBlockData trackedBlockData;
		if (this.isMultiBlock && !MultiBlockManager.Instance.TryGetPOIMultiBlock(_blockPos, out trackedBlockData))
		{
			this.multiBlockPos.AddChilds(_world, _chunk, _blockPos, _blockValue);
		}
		if (this.IsTemporaryBlock)
		{
			ChunkCustomData chunkCustomData;
			if (!_chunk.ChunkCustomData.dict.TryGetValue("temporaryblocks", out chunkCustomData))
			{
				chunkCustomData = new ChunkBlockClearData("temporaryblocks", 0UL, false, _world as World);
				_chunk.ChunkCustomData.Add("temporaryblocks", chunkCustomData);
			}
			(chunkCustomData as ChunkBlockClearData).BlockList.Add(World.toBlock(_blockPos));
		}
		if (!string.IsNullOrEmpty(this.blockAddedEvent))
		{
			GameEventManager.Current.HandleAction(this.blockAddedEvent, null, null, false, _blockPos, "", "", false, true, "", null);
		}
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnBlockReset(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00028E28 File Offset: 0x00027028
	public virtual void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!_blockValue.ischild)
		{
			this.shape.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
			if (this.isMultiBlock)
			{
				this.multiBlockPos.RemoveChilds(_world, _blockPos, _blockValue);
			}
			ChunkCustomData chunkCustomData;
			if (this.IsTemporaryBlock && _chunk.ChunkCustomData.dict.TryGetValue("temporaryblocks", out chunkCustomData))
			{
				(chunkCustomData as ChunkBlockClearData).BlockList.Remove(World.toBlock(_blockPos));
				return;
			}
		}
		else if (this.isMultiBlock)
		{
			this.multiBlockPos.RemoveParentBlock(_world, _blockPos, _blockValue);
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00028EB8 File Offset: 0x000270B8
	public virtual void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (_oldBlockValue.ischild)
		{
			return;
		}
		this.shape.OnBlockValueChanged(_world, _blockPos, _clrIdx, _oldBlockValue, _newBlockValue);
		if (this.isMultiBlock && _oldBlockValue.rotation != _newBlockValue.rotation)
		{
			this.multiBlockPos.RemoveChilds(_world, _blockPos, _oldBlockValue);
			this.multiBlockPos.AddChilds(_world, _chunk, _blockPos, _newBlockValue);
		}
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00028F1B File Offset: 0x0002711B
	public virtual void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _blockValue, _ebcd);
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x00028F30 File Offset: 0x00027130
	public virtual void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
		_ebcd.UpdateTemperature();
		this.ForceAnimationState(_blockValue, _ebcd);
		if (this.GroundAlignDistance != 0f)
		{
			((World)_world).m_ChunkManager.AddGroundAlignBlock(_ebcd);
		}
		if (_world.TryRetrieveAndRemovePendingDowngradeBlock(_blockPos) && !string.IsNullOrEmpty(this.blockDowngradedToEvent))
		{
			GameEventManager.Current.HandleAction(this.blockDowngradedToEvent, null, null, false, _blockPos, "", "", false, true, "", null);
		}
		if (this.terrainAlignmentMode != TerrainAlignmentMode.None)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				MultiBlockManager.Instance.TryRegisterTerrainAlignedBlock(_blockPos, _blockValue);
			}
			MultiBlockManager.Instance.SetTerrainAlignmentDirty(_blockPos);
		}
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00028FF0 File Offset: 0x000271F0
	public virtual int DamageBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo = null, bool _bUseHarvestTool = false, bool _bBypassMaxDamage = false)
	{
		return this.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, 0);
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00029014 File Offset: 0x00027214
	public virtual int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return 0;
		}
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = chunkCache.GetBlock(parentPos);
			if (block.ischild)
			{
				Log.Error("Block on position {0} with name '{1}' should be a parent but is not! (6)", new object[]
				{
					parentPos,
					block.Block.blockName
				});
				return 0;
			}
			return block.Block.OnBlockDamaged(_world, _clrIdx, parentPos, block, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth + 1);
		}
		else
		{
			Block block2 = _blockValue.Block;
			int damage = _blockValue.damage;
			bool flag = damage >= block2.MaxDamage;
			int num = damage + _damagePoints;
			chunkCache.InvokeOnBlockDamagedDelegates(_blockPos, _blockValue, _damagePoints, _entityIdThatDamaged);
			if (num < 0)
			{
				if (!this.UpgradeBlock.isair)
				{
					BlockValue blockValue = this.UpgradeBlock;
					blockValue = BlockPlaceholderMap.Instance.Replace(blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
					blockValue.rotation = this.convertRotation(_blockValue, blockValue);
					blockValue.meta = _blockValue.meta;
					blockValue.damage = 0;
					Block block3 = blockValue.Block;
					if (!block3.shape.IsTerrain())
					{
						_world.SetBlockRPC(_clrIdx, _blockPos, blockValue);
						if (chunkCache.GetTextureFull(_blockPos) != 0L)
						{
							GameManager.Instance.SetBlockTextureServer(_blockPos, BlockFace.None, 0, _entityIdThatDamaged, byte.MaxValue);
						}
					}
					else
					{
						_world.SetBlockRPC(_clrIdx, _blockPos, blockValue, block3.Density);
					}
					DynamicMeshManager.ChunkChanged(_blockPos, _entityIdThatDamaged, _blockValue.type);
					return blockValue.damage;
				}
				if (_blockValue.damage != 0)
				{
					_blockValue.damage = 0;
					_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
				}
				return 0;
			}
			else
			{
				if (this.Stage2Health > 0)
				{
					int num2 = block2.MaxDamage - this.Stage2Health;
					if (damage < num2 && num >= num2)
					{
						num = num2;
					}
				}
				if (!flag && num >= block2.MaxDamage)
				{
					int num3 = num - block2.MaxDamage;
					DynamicMeshManager.ChunkChanged(_blockPos, _entityIdThatDamaged, _blockValue.type);
					Block.DestroyedResult destroyedResult = this.OnBlockDestroyedBy(_world, _clrIdx, _blockPos, _blockValue, _entityIdThatDamaged, _bUseHarvestTool);
					if (destroyedResult != Block.DestroyedResult.Keep)
					{
						if (!this.DowngradeBlock.isair && destroyedResult == Block.DestroyedResult.Downgrade)
						{
							if (_recDepth == 0)
							{
								this.SpawnDowngradeFX(_world, _blockValue, _blockPos, block2.tintColor, _entityIdThatDamaged);
							}
							BlockValue blockValue2 = this.DowngradeBlock;
							blockValue2 = BlockPlaceholderMap.Instance.Replace(blockValue2, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false);
							blockValue2.rotation = _blockValue.rotation;
							blockValue2.meta = _blockValue.meta;
							Block block4 = blockValue2.Block;
							if (!block4.shape.IsTerrain())
							{
								_world.SetBlockRPC(_clrIdx, _blockPos, blockValue2);
								if (chunkCache.GetTextureFull(_blockPos) != 0L)
								{
									if (this.RemovePaintOnDowngrade == null)
									{
										GameManager.Instance.SetBlockTextureServer(_blockPos, BlockFace.None, 0, _entityIdThatDamaged, byte.MaxValue);
									}
									else
									{
										for (int i = 0; i < this.RemovePaintOnDowngrade.Count; i++)
										{
											GameManager.Instance.SetBlockTextureServer(_blockPos, this.RemovePaintOnDowngrade[i], 0, _entityIdThatDamaged, byte.MaxValue);
										}
									}
								}
								_world.AddPendingDowngradeBlock(_blockPos);
								if (!string.IsNullOrEmpty(this.blockDowngradeEvent))
								{
									Entity entity = _world.GetEntity(_entityIdThatDamaged);
									EntityVehicle entityVehicle = entity as EntityVehicle;
									if (entityVehicle != null)
									{
										entity = entityVehicle.GetFirstAttached();
									}
									GameEventManager.Current.HandleAction(this.blockDowngradeEvent, null, entity as EntityPlayer, false, _blockPos, "", "", false, true, "", null);
								}
							}
							else
							{
								_world.SetBlockRPC(_clrIdx, _blockPos, blockValue2, block4.Density);
							}
							if ((num3 > 0 && this.EnablePassThroughDamage) || _bBypassMaxDamage)
							{
								block4.OnBlockDamaged(_world, _clrIdx, _blockPos, blockValue2, num3, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth + 1);
							}
						}
						else
						{
							Entity entity2 = _world.GetEntity(_entityIdThatDamaged);
							QuestEventManager.Current.BlockDestroyed(block2, _blockPos, entity2);
							this.SpawnDestroyFX(_world, _blockValue, _blockPos, this.GetColorForSide(_blockValue, BlockFace.Top), _entityIdThatDamaged);
							_world.SetBlockRPC(_clrIdx, _blockPos, BlockValue.Air);
							TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_blockPos) as TileEntityLootContainer;
							if (tileEntityLootContainer != null)
							{
								tileEntityLootContainer.OnDestroy();
								if (!GameManager.IsDedicatedServer)
								{
									XUiC_LootWindowGroup.CloseIfOpenAtPos(_blockPos, null);
								}
								Chunk chunk = _world.GetChunkFromWorldPos(_blockPos) as Chunk;
								if (chunk != null)
								{
									chunk.RemoveTileEntityAt<TileEntityLootContainer>((World)_world, World.toBlock(_blockPos));
								}
							}
							if (!string.IsNullOrEmpty(this.blockDestroyedEvent))
							{
								Entity entity3 = _world.GetEntity(_entityIdThatDamaged);
								EntityVehicle entityVehicle2 = entity3 as EntityVehicle;
								if (entityVehicle2 != null)
								{
									entity3 = entityVehicle2.GetFirstAttached();
								}
								GameEventManager.Current.HandleAction(this.blockDestroyedEvent, null, entity3 as EntityPlayer, false, _blockPos, "", "", false, true, "", null);
							}
						}
					}
					return block2.MaxDamage;
				}
				if (_blockValue.damage != num)
				{
					_blockValue.damage = num;
					if (!block2.shape.IsTerrain())
					{
						_world.SetBlocksRPC(new List<BlockChangeInfo>
						{
							new BlockChangeInfo(_blockPos, _blockValue, false, true)
						});
					}
					else
					{
						sbyte density = _world.GetDensity(_clrIdx, _blockPos);
						sbyte b = (sbyte)Utils.FastMin(-1f, (float)MarchingCubes.DensityTerrain * (1f - (float)num / (float)block2.MaxDamage));
						if ((_damagePoints > 0 && b > density) || (_damagePoints < 0 && b < density))
						{
							_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue, b);
						}
						else
						{
							_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
						}
					}
					if (this.terrainAlignmentMode != TerrainAlignmentMode.None)
					{
						MultiBlockManager.Instance.SetTerrainAlignmentDirty(_blockPos);
					}
				}
				return _blockValue.damage;
			}
		}
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00029589 File Offset: 0x00027789
	public virtual bool IsHealthShownInUI(BlockValue _bv)
	{
		if (this.Stage2Health > 0)
		{
			return _bv.Block.MaxDamage - _bv.damage > this.Stage2Health;
		}
		return _bv.damage > 0;
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x000295B9 File Offset: 0x000277B9
	[PublicizedFrom(EAccessModifier.Private)]
	public byte convertRotation(BlockValue _oldBV, BlockValue _newBV)
	{
		return _oldBV.rotation;
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x000295C4 File Offset: 0x000277C4
	public void AddDroppedId(EnumDropEvent _eEvent, string _name, int _minCount, int _maxCount, float _prob, float _resourceScale, float _stickChance, string _toolCategory, string _tag)
	{
		List<Block.SItemDropProb> list;
		this.itemsToDrop.TryGetValue(_eEvent, out list);
		if (list == null)
		{
			list = new List<Block.SItemDropProb>();
			this.itemsToDrop[_eEvent] = list;
		}
		list.Add(new Block.SItemDropProb(_name, _minCount, _maxCount, _prob, _resourceScale, _stickChance, _toolCategory, _tag));
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0002960F File Offset: 0x0002780F
	public bool HasItemsToDropForEvent(EnumDropEvent _eEvent)
	{
		return this.itemsToDrop.ContainsKey(_eEvent);
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00029620 File Offset: 0x00027820
	public void DropItemsOnEvent(WorldBase _world, BlockValue _blockValue, EnumDropEvent _eEvent, float _overallProb, Vector3 _dropPos, Vector3 _randomPosAdd, float _lifetime, int _entityId, bool _bGetSameItemIfNoneFound)
	{
		GameRandom gameRandom = _world.GetGameRandom();
		this.itemsDropped.Clear();
		List<Block.SItemDropProb> list;
		if (!this.itemsToDrop.TryGetValue(_eEvent, out list))
		{
			if (_bGetSameItemIfNoneFound)
			{
				ItemValue itemValue = _blockValue.ToItemValue();
				this.itemsDropped.Add(new ItemStack(itemValue, 1));
			}
		}
		else
		{
			for (int i = 0; i < list.Count; i++)
			{
				Block.SItemDropProb sitemDropProb = list[i];
				int num = gameRandom.RandomRange(sitemDropProb.minCount, sitemDropProb.maxCount + 1);
				if (num > 0)
				{
					if (sitemDropProb.stickChance < 0.001f || gameRandom.RandomFloat > sitemDropProb.stickChance)
					{
						if (sitemDropProb.name.Equals("[recipe]"))
						{
							List<Recipe> recipes = CraftingManager.GetRecipes(_blockValue.Block.GetBlockName());
							if (recipes.Count > 0)
							{
								for (int j = 0; j < recipes[0].ingredients.Count; j++)
								{
									if (recipes[0].ingredients[j].count / 2 > 0)
									{
										ItemStack item = new ItemStack(recipes[0].ingredients[j].itemValue, recipes[0].ingredients[j].count / 2);
										this.itemsDropped.Add(item);
									}
								}
							}
						}
						else
						{
							ItemValue itemValue2 = sitemDropProb.name.Equals("*") ? _blockValue.ToItemValue() : new ItemValue(ItemClass.GetItem(sitemDropProb.name, false).type, false);
							if (!itemValue2.IsEmpty() && sitemDropProb.prob > gameRandom.RandomFloat)
							{
								this.itemsDropped.Add(new ItemStack(itemValue2, num));
							}
						}
					}
					else
					{
						Vector3i vector3i = World.worldToBlockPos(_dropPos);
						if (!GameManager.Instance.World.IsWithinTraderArea(vector3i) && (_overallProb >= 0.999f || gameRandom.RandomFloat < _overallProb))
						{
							BlockValue blockValue = Block.GetBlockValue(sitemDropProb.name, false);
							if (!blockValue.isair && _world.GetBlock(vector3i).isair)
							{
								_world.SetBlockRPC(vector3i, blockValue);
							}
						}
					}
				}
			}
		}
		for (int k = 0; k < this.itemsDropped.Count; k++)
		{
			if (_overallProb >= 0.999f || gameRandom.RandomFloat < _overallProb)
			{
				ItemClass itemClass = this.itemsDropped[k].itemValue.ItemClass;
				_lifetime = ((_lifetime > 0.001f) ? _lifetime : ((itemClass != null) ? itemClass.GetLifetimeOnDrop() : 0f));
				if (_lifetime > 0.001f)
				{
					_world.GetGameManager().ItemDropServer(this.itemsDropped[k], _dropPos, _randomPosAdd, _entityId, _lifetime, false);
				}
			}
		}
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x000298F9 File Offset: 0x00027AF9
	public float GetExplosionResistance()
	{
		return this.blockMaterial.ExplosionResistance;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00029908 File Offset: 0x00027B08
	public bool intersectRayWithBlock(BlockValue _blockValue, int _x, int _y, int _z, Ray _ray, out Vector3 _hitPoint, World _world)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, _x, _y, _z, 0f, Block.staticList_IntersectRayWithBlockList);
		for (int i = 0; i < Block.staticList_IntersectRayWithBlockList.Count; i++)
		{
			if (Block.staticList_IntersectRayWithBlockList[i].IntersectRay(_ray))
			{
				_hitPoint = new Vector3((float)_x, (float)_y, (float)_z);
				return true;
			}
		}
		_hitPoint = Vector3.zero;
		return false;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00029984 File Offset: 0x00027B84
	public virtual Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerThatStartedExpl)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache != null)
		{
			chunkCache.InvokeOnBlockDamagedDelegates(_blockPos, _blockValue, _blockValue.Block.MaxDamage, _playerThatStartedExpl);
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x000299B3 File Offset: 0x00027BB3
	public virtual void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		_world.SetBlockRPC(_blockPos, BlockValue.Air);
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x000299C4 File Offset: 0x00027BC4
	public virtual bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (_blockPos.y > 253)
		{
			return false;
		}
		Block block = _blockValue.Block;
		if (!GameManager.Instance.IsEditMode())
		{
			if (!block.isMultiBlock)
			{
				if (((World)_world).IsWithinTraderPlacingProtection(_blockPos))
				{
					return false;
				}
			}
			else
			{
				Bounds bounds = block.multiBlockPos.CalcBounds(_blockValue.type, (int)_blockValue.rotation);
				bounds.center += _blockPos.ToVector3();
				if (((World)_world).IsWithinTraderPlacingProtection(bounds))
				{
					return false;
				}
			}
		}
		return (!block.isMultiBlock || _blockPos.y + block.multiBlockPos.dim.y < 254) && (GameManager.Instance.IsEditMode() || !block.bRestrictSubmergedPlacement || !this.IsUnderwater(_world, _blockPos, _blockValue)) && (GameManager.Instance.IsEditMode() || _bOmitCollideCheck || !this.overlapsWithOtherBlock(_world, _clrIdx, _blockPos, _blockValue));
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00029AB8 File Offset: 0x00027CB8
	public Vector3i GetFreePlacementPosition(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityPlacing)
	{
		Vector3i vector3i = _blockPos;
		int num = 15;
		while (_blockValue.Block.overlapsWithOtherBlock(_world, _clrIdx, vector3i, _blockValue))
		{
			Vector3 direction = _entityPlacing.getHeadPosition() - (vector3i.ToVector3() + Vector3.one * 0.5f);
			Vector3 vector;
			BlockFace blockFace;
			vector3i = Voxel.OneVoxelStep(vector3i, vector3i.ToVector3() + Vector3.one * 0.5f, direction, out vector, out blockFace);
			if (--num <= 0)
			{
				break;
			}
		}
		if (num <= 0)
		{
			vector3i = _blockPos;
		}
		return vector3i;
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x00029B40 File Offset: 0x00027D40
	[PublicizedFrom(EAccessModifier.Private)]
	public bool overlapsWithOtherBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.isMultiBlock)
		{
			int type = _world.GetBlock(_clrIdx, _blockPos).type;
			return type != 0 && !Block.list[type].CanBlocksReplaceOrGroundCover();
		}
		byte rotation = _blockValue.rotation;
		for (int i = this.multiBlockPos.Length - 1; i >= 0; i--)
		{
			Vector3i pos = _blockPos + this.multiBlockPos.Get(i, _blockValue.type, (int)rotation);
			int type2 = _world.GetBlock(_clrIdx, pos).type;
			if (type2 != 0 && !Block.list[type2].CanBlocksReplaceOrGroundCover())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00029BE0 File Offset: 0x00027DE0
	public bool CanBlocksReplaceOrGroundCover()
	{
		return this.CanBlocksReplace || this.blockMaterial.IsGroundCover;
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00029BF8 File Offset: 0x00027DF8
	public bool IsUnderwater(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (this.isMultiBlock)
		{
			int num = _blockPos.y + this.multiBlockPos.dim.y - 1;
			for (int i = 0; i < this.multiBlockPos.Length; i++)
			{
				Vector3i vector3i = _blockPos + this.multiBlockPos.Get(i, _blockValue.type, (int)_blockValue.rotation);
				if (vector3i.y == num && _world.IsWater(vector3i))
				{
					return true;
				}
			}
		}
		else if (_world.IsWater(_blockPos))
		{
			return true;
		}
		return false;
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00029C80 File Offset: 0x00027E80
	public virtual BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		return _blockValue;
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00029C84 File Offset: 0x00027E84
	public virtual void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		DynamicMeshManager.ChunkChanged(_bpResult.blockPos, (_ea != null) ? _ea.entityId : -2, _bpResult.blockValue.type);
		if (this.SelectAlternates)
		{
			byte rotation = _bpResult.blockValue.rotation;
			_bpResult.blockValue = _bpResult.blockValue.Block.GetAltBlockValue(_ea.inventory.holdingItemItemValue.Meta);
			_bpResult.blockValue.rotation = rotation;
		}
		else
		{
			string placeAltBlockValue = this.GetPlaceAltBlockValue(_world);
			_bpResult.blockValue = ((placeAltBlockValue.Length == 0) ? _bpResult.blockValue : Block.GetBlockValue(placeAltBlockValue, false));
		}
		Block block = _bpResult.blockValue.Block;
		if (block.PlaceRandomRotation)
		{
			int num;
			bool flag;
			do
			{
				num = _rnd.RandomRange(28);
				if (num < 4)
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Basic90) > EBlockRotationClasses.None);
				}
				else if (num < 8)
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Headfirst) > EBlockRotationClasses.None);
				}
				else if (num < 24)
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Sideways) > EBlockRotationClasses.None);
				}
				else
				{
					flag = ((block.AllowedRotations & EBlockRotationClasses.Basic45) > EBlockRotationClasses.None);
				}
			}
			while (!flag);
			_bpResult.blockValue.rotation = (byte)num;
		}
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00029D98 File Offset: 0x00027F98
	public virtual void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		Block block = _result.blockValue.Block;
		int changingEntityId = (_ea == null) ? -1 : _ea.entityId;
		if (block.shape.IsTerrain())
		{
			_world.SetBlockRPC(_result.clrIdx, _result.blockPos, _result.blockValue, this.Density, changingEntityId);
		}
		else if (!block.IsTerrainDecoration)
		{
			_world.SetBlockRPC(_result.clrIdx, _result.blockPos, _result.blockValue, MarchingCubes.DensityAir, changingEntityId);
		}
		else
		{
			_world.SetBlockRPC(_result.clrIdx, _result.blockPos, _result.blockValue, changingEntityId);
		}
		if (this.blockName.Equals("keystoneBlock") && _ea is EntityPlayerLocal)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager == null)
			{
				return;
			}
			achievementManager.SetAchievementStat(EnumAchievementDataStat.LandClaimPlaced, 1);
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x00029E64 File Offset: 0x00028064
	public virtual ItemStack OnBlockPickedUp(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		ItemStack itemStack = new ItemStack((this.PickedUpItemValue == null) ? _blockValue.ToItemValue() : ItemClass.GetItem(this.PickedUpItemValue, false), 1);
		return (this.PickupTarget == null) ? itemStack : new ItemStack(new ItemValue(ItemClass.GetItem(this.PickupTarget, false).type, false), 1);
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00029EC0 File Offset: 0x000280C0
	public virtual bool OnBlockActivated(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer != null)
		{
			_player.AimingGun = false;
			Vector3i blockPos = tileEntityLootContainer.ToWorldPos();
			tileEntityLootContainer.bWasTouched = tileEntityLootContainer.bTouched;
			_world.GetGameManager().TELockServer(_clrIdx, blockPos, tileEntityLootContainer.entityId, _player.entityId, null);
			return true;
		}
		bool flag = this.CanPickup;
		Block block = _blockValue.Block;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _player, null, block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		if (!_world.CanPickupBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer()))
		{
			_player.PlayOneShot("keystone_impact_overlay", false, false, false, null);
			return false;
		}
		if (_blockValue.damage > 0)
		{
			GameManager.ShowTooltip(_player, Localization.Get("ttRepairBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return false;
		}
		ItemStack itemStack = block.OnBlockPickedUp(_world, _clrIdx, _blockPos, _blockValue, _player.entityId);
		if (!_player.inventory.CanTakeItem(itemStack) && !_player.bag.CanTakeItem(itemStack))
		{
			GameManager.ShowTooltip(_player, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return false;
		}
		QuestEventManager.Current.BlockPickedUp(block.GetBlockName(), _blockPos);
		QuestEventManager.Current.ItemAdded(itemStack);
		_world.GetGameManager().PickupBlockServer(_clrIdx, _blockPos, _blockValue, _player.entityId, null);
		return false;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _entity)
	{
		return false;
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
	{
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanPlantStay(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		return true;
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0002A038 File Offset: 0x00028238
	public void SetBlockName(string _blockName)
	{
		this.blockName = _blockName;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0002A041 File Offset: 0x00028241
	public string GetBlockName()
	{
		return this.blockName;
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0002A049 File Offset: 0x00028249
	public static HashSet<string> GetAutoShapeMaterials()
	{
		return Block.autoShapeMaterials;
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0002A050 File Offset: 0x00028250
	public EAutoShapeType GetAutoShapeType()
	{
		return this.AutoShapeType;
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0002A058 File Offset: 0x00028258
	public string GetAutoShapeBlockName()
	{
		return this.autoShapeBaseName;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0002A060 File Offset: 0x00028260
	public string GetAutoShapeShapeName()
	{
		return this.autoShapeShapeName;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0002A068 File Offset: 0x00028268
	public Block GetAutoShapeHelperBlock()
	{
		return this.autoShapeHelper;
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0002A070 File Offset: 0x00028270
	public string GetLocalizedAutoShapeShapeName()
	{
		return Localization.Get("shape" + this.GetAutoShapeShapeName(), false);
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x0002A088 File Offset: 0x00028288
	public bool AutoShapeSupportsShapeName(string _shapeName)
	{
		return this.AutoShapeType == EAutoShapeType.Helper && this.ContainsAlternateBlock(this.autoShapeBaseName + ":" + _shapeName);
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0002A0AC File Offset: 0x000282AC
	public int AutoShapeAlternateShapeNameIndex(string _shapeName)
	{
		if (this.AutoShapeType == EAutoShapeType.Helper)
		{
			return this.GetAlternateBlockIndex(this.autoShapeBaseName + ":" + _shapeName);
		}
		return -1;
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x0002A0D0 File Offset: 0x000282D0
	public virtual string GetLocalizedBlockName()
	{
		if (this.localizedBlockName != null)
		{
			return this.localizedBlockName;
		}
		if (this.AutoShapeType != EAutoShapeType.None)
		{
			return this.localizedBlockName = this.blockMaterial.GetLocalizedMaterialName() + " - " + this.GetLocalizedAutoShapeShapeName();
		}
		return this.localizedBlockName = Localization.Get(this.GetBlockName(), false);
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x0002A130 File Offset: 0x00028330
	public virtual string GetLocalizedBlockName(ItemValue _itemValueRef)
	{
		if (this.AutoShapeType != EAutoShapeType.Helper || _itemValueRef.ToBlockValue().Equals(BlockValue.Air))
		{
			return this.GetLocalizedBlockName();
		}
		this.GetAltBlocks();
		return this.placeAltBlockClasses[_itemValueRef.Meta].GetLocalizedBlockName();
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0002A17B File Offset: 0x0002837B
	public string GetIconName()
	{
		return this.CustomIcon ?? this.GetBlockName();
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0002A18D File Offset: 0x0002838D
	public void SetSideTextureId(int _textureId, int channel)
	{
		this.textureInfos[channel].singleTextureId = _textureId;
		this.textureInfos[channel].bTextureForEachSide = false;
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x0002A1B4 File Offset: 0x000283B4
	public void SetSideTextureId(string[] _texIds, int channel)
	{
		this.textureInfos[channel].sideTextureIds = new int[_texIds.Length];
		for (int i = 0; i < _texIds.Length; i++)
		{
			this.textureInfos[channel].sideTextureIds[i] = int.Parse(_texIds[i]);
		}
		this.textureInfos[channel].bTextureForEachSide = true;
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x0002A218 File Offset: 0x00028418
	public int GetSideTextureId(BlockValue _blockValue, BlockFace _side, int channel)
	{
		if (this.textureInfos[channel].bTextureForEachSide)
		{
			int num = this.shape.MapSideAndRotationToTextureIdx(_blockValue, _side);
			if (num >= this.textureInfos[channel].sideTextureIds.Length)
			{
				num = 0;
			}
			return this.textureInfos[channel].sideTextureIds[num];
		}
		return this.textureInfos[channel].singleTextureId;
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0002A284 File Offset: 0x00028484
	public MaterialBlock GetMaterialForSide(BlockValue _blockValue, BlockFace _side)
	{
		MaterialBlock materialBlock = null;
		int sideTextureId = this.GetSideTextureId(_blockValue, _side, 0);
		Block block = _blockValue.Block;
		if (sideTextureId != -1 && MeshDescription.meshes[(int)block.MeshIndex].textureAtlas.uvMapping.Length > sideTextureId)
		{
			materialBlock = MeshDescription.meshes[(int)block.MeshIndex].textureAtlas.uvMapping[sideTextureId].material;
		}
		if (materialBlock == null)
		{
			materialBlock = block.blockMaterial;
		}
		return materialBlock;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0002A2F1 File Offset: 0x000284F1
	public int GetUiBackgroundTextureId(BlockValue _blockValue, BlockFace _side, int channel = 0)
	{
		if (this.uiBackgroundTextureId < 0)
		{
			return this.GetSideTextureId(_blockValue, _side, channel);
		}
		return this.uiBackgroundTextureId;
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x0002A30C File Offset: 0x0002850C
	public string GetParticleForSide(BlockValue _blockValue, BlockFace _side)
	{
		MaterialBlock materialForSide = this.GetMaterialForSide(_blockValue, _side);
		if (materialForSide != null && materialForSide.ParticleCategory != null)
		{
			return materialForSide.ParticleCategory;
		}
		if (materialForSide != null && materialForSide.SurfaceCategory != null)
		{
			return materialForSide.SurfaceCategory;
		}
		return null;
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0002A348 File Offset: 0x00028548
	public string GetDestroyParticle(BlockValue _blockValue)
	{
		if (this.blockMaterial.ParticleDestroyCategory != null)
		{
			return this.blockMaterial.ParticleDestroyCategory;
		}
		if (this.blockMaterial.ParticleCategory != null)
		{
			return this.blockMaterial.ParticleCategory;
		}
		if (this.blockMaterial.SurfaceCategory != null)
		{
			return this.blockMaterial.SurfaceCategory;
		}
		return null;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0002A3A4 File Offset: 0x000285A4
	public Color GetColorForSide(BlockValue _blockValue, BlockFace _side)
	{
		TextureAtlas textureAtlas = MeshDescription.meshes[(int)_blockValue.Block.MeshIndex].textureAtlas;
		int sideTextureId = this.GetSideTextureId(_blockValue, _side, 0);
		if (sideTextureId != -1 && textureAtlas.uvMapping.Length > sideTextureId)
		{
			return textureAtlas.uvMapping[sideTextureId].color;
		}
		return Color.gray;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0002A3FC File Offset: 0x000285FC
	public Color GetMapColor(BlockValue _blockValue, Vector3 _normal, int _yPos)
	{
		Color color;
		if (!this.bMapColorSet)
		{
			if (_normal.x > 0.5f || _normal.z > 0.5f || _normal.x < -0.5f || _normal.z < -0.5f)
			{
				color = this.GetColorForSide(_blockValue, BlockFace.South);
			}
			else
			{
				color = this.GetColorForSide(_blockValue, BlockFace.Top);
			}
		}
		else
		{
			color = this.MapColor;
		}
		float num = this.MapSpecular;
		if (this.bMapColor2Set && this.MapElevMinMax.y != this.MapElevMinMax.x)
		{
			float num2 = (float)Utils.FastMax(_yPos - this.MapElevMinMax.x, 0) / (float)(this.MapElevMinMax.y - this.MapElevMinMax.x);
			color = Color.Lerp(this.MapColor, this.MapColor2, num2);
			num = Utils.FastMax(num - num2 * 0.5f, 0f);
		}
		float num3 = (_normal.z + 1f) / 2f * (_normal.x + 1f) / 2f;
		num3 *= 2f;
		color = Utils.Saturate(color * 0.5f + color * num3);
		color.a = num;
		return color;
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0002A532 File Offset: 0x00028732
	public static bool CanDrop(BlockValue _blockValue)
	{
		return !_blockValue.Equals(BlockValue.Air);
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsElevator()
	{
		return false;
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsElevator(int rotation)
	{
		return false;
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x0002A543 File Offset: 0x00028743
	public virtual bool IsPlant()
	{
		return this.blockMaterial.IsPlant || this.bIsPlant;
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0002A55A File Offset: 0x0002875A
	public bool HasTag(BlockTags _tag)
	{
		return this.BlockTag == _tag;
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0002A565 File Offset: 0x00028765
	public bool HasAnyFastTags(FastTags<TagGroup.Global> _tags)
	{
		return this.Tags.Test_AnySet(_tags);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x0002A573 File Offset: 0x00028773
	public bool HasAllFastTags(FastTags<TagGroup.Global> _tags)
	{
		return this.Tags.Test_AllSet(_tags);
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x0002A581 File Offset: 0x00028781
	public virtual bool CanRepair(BlockValue _blockValue)
	{
		return _blockValue.damage > 0;
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x0002A58C File Offset: 0x0002878C
	public virtual string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		Block block = _blockValue.Block;
		TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer != null)
		{
			string arg = block.GetLocalizedBlockName();
			PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
			string arg2 = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
			if (!tileEntityLootContainer.bTouched)
			{
				return string.Format(Localization.Get("lootTooltipNew", false), arg2, arg);
			}
			if (tileEntityLootContainer.IsEmpty())
			{
				return string.Format(Localization.Get("lootTooltipEmpty", false), arg2, arg);
			}
			return string.Format(Localization.Get("lootTooltipTouched", false), arg2, arg);
		}
		else
		{
			if (!this.CanPickup && EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _entityFocusing, null, _blockValue.Block.Tags, true, true, true, true, true, 1, true, false) <= 0f)
			{
				return null;
			}
			if (!_world.CanPickupBlockAt(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer()))
			{
				return null;
			}
			string key = block.GetBlockName();
			if (!string.IsNullOrEmpty(block.PickedUpItemValue))
			{
				key = block.PickedUpItemValue;
			}
			else if (!string.IsNullOrEmpty(block.PickupTarget))
			{
				key = block.PickupTarget;
			}
			return string.Format(Localization.Get("pickupPrompt", false), Localization.Get(key, false));
		}
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0002A6D8 File Offset: 0x000288D8
	public void SpawnDowngradeFX(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, Color _color, int _entityIdThatCaused)
	{
		Block block = _blockValue.Block;
		if (block.DowngradeFX != null)
		{
			this.SpawnFX(_world, _blockPos, 1f, _color, _entityIdThatCaused, block.DowngradeFX);
			return;
		}
		this.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, 1f, _color, _entityIdThatCaused);
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x0002A720 File Offset: 0x00028920
	public void SpawnDestroyFX(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, Color _color, int _entityIdThatCaused)
	{
		Block block = _blockValue.Block;
		if (block.DestroyFX != null)
		{
			this.SpawnFX(_world, _blockPos, 1f, _color, _entityIdThatCaused, block.DestroyFX);
			return;
		}
		this.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, 1f, _color, _entityIdThatCaused);
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x0002A768 File Offset: 0x00028968
	public virtual void SpawnDestroyParticleEffect(WorldBase _world, BlockValue _blockValue, Vector3i _blockPos, float _lightValue, Color _color, int _entityIdThatCaused)
	{
		if (this.deathParticleName != null)
		{
			_world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(this.deathParticleName, World.blockToTransformPos(_blockPos) + new Vector3(0f, 0.5f, 0f), _lightValue, _color, this.blockMaterial.SurfaceCategory + "destroy", null, true), _entityIdThatCaused, false, true);
			return;
		}
		MaterialBlock materialForSide = this.GetMaterialForSide(_blockValue, BlockFace.Top);
		string destroyParticle = this.GetDestroyParticle(_blockValue);
		if (destroyParticle != null && materialForSide.SurfaceCategory != null)
		{
			_world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("blockdestroy_" + destroyParticle, World.blockToTransformPos(_blockPos) + new Vector3(0f, 0.5f, 0f), _lightValue, _color, this.blockMaterial.SurfaceCategory + "destroy", null, true), _entityIdThatCaused, false, true);
		}
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x0002A848 File Offset: 0x00028A48
	public void SpawnFX(WorldBase _world, Vector3i _blockPos, float _lightValue, Color _color, int _entityIdThatCaused, string _fxName)
	{
		string[] array = _fxName.Split(',', StringSplitOptions.None);
		_world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(array[0], World.blockToTransformPos(_blockPos) + new Vector3(0f, 0.5f, 0f), _lightValue, _color, array[1], null, true), _entityIdThatCaused, false, true);
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x0002A8A0 File Offset: 0x00028AA0
	public static BlockValue GetBlockValue(string _blockName, bool _caseInsensitive = false)
	{
		Block blockByName = Block.GetBlockByName(_blockName, _caseInsensitive);
		if (blockByName != null)
		{
			return blockByName.ToBlockValue();
		}
		return BlockValue.Air;
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x0002A8C4 File Offset: 0x00028AC4
	public static Block GetBlockByName(string _blockname, bool _caseInsensitive = false)
	{
		if (Block.nameToBlock == null)
		{
			return null;
		}
		Block result;
		if (_caseInsensitive)
		{
			Block.nameToBlockCaseInsensitive.TryGetValue(_blockname, out result);
		}
		else
		{
			Block.nameToBlock.TryGetValue(_blockname, out result);
		}
		return result;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x0002A8FC File Offset: 0x00028AFC
	public BlockValue ToBlockValue()
	{
		return new BlockValue
		{
			type = this.blockID
		};
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0002A920 File Offset: 0x00028B20
	public static BlockValue GetBlockValue(int _blockType)
	{
		if (Block.list[_blockType] == null)
		{
			return BlockValue.Air;
		}
		return new BlockValue
		{
			type = _blockType
		};
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x0002A950 File Offset: 0x00028B50
	public BlockValue GetBlockValueFromProperty(string _propValue)
	{
		BlockValue result = BlockValue.Air;
		if (!this.Properties.Values.ContainsKey(_propValue))
		{
			throw new Exception("You need to specify a property with name '" + _propValue + "' for the block " + this.blockName);
		}
		result = Block.GetBlockValue(this.Properties.Values[_propValue], false);
		if (result.Equals(BlockValue.Air))
		{
			throw new Exception("Block with name '" + this.Properties.Values[_propValue] + "' not found!");
		}
		return result;
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0002A9DF File Offset: 0x00028BDF
	public virtual bool ShowModelOnFall()
	{
		return this.bShowModelOnFall;
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060005D5 RID: 1493 RVA: 0x0002A9E8 File Offset: 0x00028BE8
	public BlockActivationCommand[] CustomCmds
	{
		get
		{
			if (this.customCmds == null)
			{
				int num = 0;
				int num2 = 1;
				while (num2 <= 10 && this.Properties.Values.ContainsKey(string.Format("{0}{1}", Block.PropCustomCommandName, num2)))
				{
					num++;
					num2++;
				}
				this.customCmds = new BlockActivationCommand[num];
				if (num > 0)
				{
					for (int i = 1; i <= num; i++)
					{
						if (this.Properties.Values.ContainsKey(string.Format("{0}{1}", Block.PropCustomCommandName, i)))
						{
							BlockActivationCommand blockActivationCommand = default(BlockActivationCommand);
							blockActivationCommand.text = this.Properties.Values[string.Format("{0}{1}", Block.PropCustomCommandName, i)];
							blockActivationCommand.icon = this.Properties.Values[string.Format("{0}{1}", Block.PropCustomCommandIcon, i)];
							blockActivationCommand.eventName = this.Properties.Values[string.Format("{0}{1}", Block.PropCustomCommandEvent, i)];
							string text = string.Format("{0}{1}", Block.PropCustomCommandIconColor, i);
							if (this.Properties.Values.ContainsKey(text))
							{
								blockActivationCommand.iconColor = StringParsers.ParseHexColor(this.Properties.Values[text]);
							}
							else
							{
								blockActivationCommand.iconColor = Color.white;
							}
							blockActivationCommand.enabled = true;
							this.customCmds[i - 1] = blockActivationCommand;
						}
					}
				}
			}
			return this.customCmds;
		}
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x0002AB8C File Offset: 0x00028D8C
	public virtual bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.GetTileEntity(_blockPos) is TileEntityLootContainer;
		bool flag2 = this.CanPickup;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _entityFocusing, null, _blockValue.Block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag2 = true;
		}
		return flag2 || flag || this.CustomCmds.Length != 0;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0002ABF4 File Offset: 0x00028DF4
	public virtual BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.GetTileEntity(_blockPos) is TileEntityLootContainer;
		bool flag2 = false;
		bool flag3 = this.CanPickup;
		if (EffectManager.GetValue(PassiveEffects.BlockPickup, null, 0f, _entityFocusing, null, _blockValue.Block.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			flag3 = true;
		}
		if (flag3)
		{
			this.cmds[0].enabled = true;
			flag2 = true;
		}
		if (flag)
		{
			this.cmds[1].enabled = true;
			flag2 = true;
		}
		if (!flag2)
		{
			return BlockActivationCommand.Empty;
		}
		return this.cmds;
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0002AC84 File Offset: 0x00028E84
	public virtual bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == this.cmds[0].text || _commandName == this.cmds[1].text)
		{
			this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
			return true;
		}
		return false;
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0002ACD8 File Offset: 0x00028ED8
	public virtual void RenderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		this.shape.renderDecorations(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, _nBlocks);
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsExplosionAffected()
	{
		return true;
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0002AD00 File Offset: 0x00028F00
	public int GetActivationDistanceSq()
	{
		int num = this.activationDistance;
		if (num == 0)
		{
			return (int)(Constants.cCollectItemDistance * Constants.cCollectItemDistance);
		}
		return num * num;
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0002AD28 File Offset: 0x00028F28
	public int GetPlacementDistanceSq()
	{
		int num = this.placementDistance;
		if (num == 0)
		{
			num = this.activationDistance;
		}
		if (num == 0)
		{
			return (int)(Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance);
		}
		return num * num;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0002AD5C File Offset: 0x00028F5C
	public virtual void CheckUpdate(BlockValue _oldBV, BlockValue _newBV, out bool bUpdateMesh, out bool bUpdateNotify, out bool bUpdateLight)
	{
		bUpdateMesh = (bUpdateNotify = (bUpdateLight = true));
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool RotateVerticesOnCollisionCheck(BlockValue _blockValue)
	{
		return true;
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		return false;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ActivateBlockOnce(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		return false;
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnTriggerAddedFromPrefab(BlockTrigger _trigger, Vector3i _blockPos, BlockValue _blockValue, FastTags<TagGroup.Global> _questTags)
	{
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnTriggerRefresh(BlockTrigger _trigger, BlockValue _bv, FastTags<TagGroup.Global> questTag)
	{
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnTriggerChanged(BlockTrigger _trigger, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnTriggerChanged(BlockTrigger _trigger, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges)
	{
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnTriggered(EntityPlayer _player, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Refresh(WorldBase _world, Chunk _chunk, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x0002AD78 File Offset: 0x00028F78
	public void HandleTrigger(EntityPlayer _player, World _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageBlockTrigger>().Setup(_cIdx, _blockPos, _blockValue), false);
			return;
		}
		BlockTrigger blockTrigger = ((Chunk)_world.ChunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		if (blockTrigger != null)
		{
			_world.triggerManager.TriggerBlocks(_player, _player.prefab, blockTrigger);
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0002ADFE File Offset: 0x00028FFE
	public override string ToString()
	{
		return this.blockName + " " + this.blockID.ToString();
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0002AE1C File Offset: 0x0002901C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsLinear()
	{
		bool[] usedIds = new bool[Block.MAX_BLOCKS];
		List<Block> list = new List<Block>(Block.nameToBlock.Count);
		Block.nameToBlock.CopyValuesTo(list);
		Block.assignLeftOverBlocks(usedIds, list);
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0002AE54 File Offset: 0x00029054
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignId(Block _b, int _id, bool[] _usedIds)
	{
		Block.list[_id] = _b;
		_b.blockID = _id;
		_usedIds[_id] = true;
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0002AE6C File Offset: 0x0002906C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignLeftOverBlocks(bool[] _usedIds, List<Block> _unassignedBlocks)
	{
		foreach (KeyValuePair<string, int> keyValuePair in Block.fixedBlockIds)
		{
			if (Block.nameToBlock.ContainsKey(keyValuePair.Key))
			{
				Block block = Block.nameToBlock[keyValuePair.Key];
				if (_unassignedBlocks.Contains(block))
				{
					_unassignedBlocks.Remove(block);
					Block.assignId(block, keyValuePair.Value, _usedIds);
				}
			}
		}
		int num = 0;
		int num2 = 255;
		foreach (Block block2 in _unassignedBlocks)
		{
			if (block2.shape.IsTerrain())
			{
				while (_usedIds[++num])
				{
				}
				Block.assignId(block2, num, _usedIds);
			}
			else
			{
				while (_usedIds[++num2])
				{
				}
				Block.assignId(block2, num2, _usedIds);
			}
		}
		Log.Out("Block IDs total {0}, terr {1}, last {2}", new object[]
		{
			Block.nameToBlock.Count,
			num,
			num2
		});
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0002AFA8 File Offset: 0x000291A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void assignIdsFromMapping()
	{
		List<Block> list = new List<Block>();
		bool[] usedIds = new bool[Block.MAX_BLOCKS];
		foreach (KeyValuePair<string, Block> keyValuePair in Block.nameToBlock)
		{
			int idForName = Block.nameIdMapping.GetIdForName(keyValuePair.Key);
			if (idForName >= 0)
			{
				Block.assignId(keyValuePair.Value, idForName, usedIds);
			}
			else
			{
				list.Add(keyValuePair.Value);
			}
		}
		Block.assignLeftOverBlocks(usedIds, list);
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0002B044 File Offset: 0x00029244
	[PublicizedFrom(EAccessModifier.Private)]
	public static void createFullMappingForClients()
	{
		NameIdMapping nameIdMapping = new NameIdMapping(null, Block.MAX_BLOCKS);
		foreach (KeyValuePair<string, Block> keyValuePair in Block.nameToBlock)
		{
			nameIdMapping.AddMapping(keyValuePair.Value.blockID, keyValuePair.Key, false);
		}
		Block.fullMappingDataForClients = nameIdMapping.SaveToArray();
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0002B0C0 File Offset: 0x000292C0
	public static void AssignIds()
	{
		if (Block.nameToBlock.Count > Block.MAX_BLOCKS)
		{
			throw new ArgumentOutOfRangeException(string.Format("Too many blocks defined ({0}, allowed {1}", Block.nameToBlock.Count, Block.MAX_BLOCKS));
		}
		if (Block.nameIdMapping != null)
		{
			Log.Out("Block IDs with mapping");
			Block.assignIdsFromMapping();
		}
		else
		{
			Log.Out("Block IDs withOUT mapping");
			Block.assignIdsLinear();
		}
		Block.createFullMappingForClients();
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsTileEntitySavedInPrefab()
	{
		return false;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string GetCustomDescription(Vector3i _blockPos, BlockValue _bv)
	{
		return "";
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x0002B13A File Offset: 0x0002933A
	public string GetPlaceAltBlockValue(WorldBase _world)
	{
		if (this.placeAltBlockNames != null && this.placeAltBlockNames.Length != 0)
		{
			return this.placeAltBlockNames[_world.GetGameRandom().RandomRange(0, this.placeAltBlockNames.Length)];
		}
		return string.Empty;
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x0002B16E File Offset: 0x0002936E
	public Block GetAltBlock(int _typeId)
	{
		this.GetAltBlocks();
		if (this.placeAltBlockClasses != null && this.placeAltBlockClasses.Length != 0)
		{
			return this.placeAltBlockClasses[_typeId];
		}
		return Block.list[0];
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0002B198 File Offset: 0x00029398
	public BlockValue GetAltBlockValue(int typeID)
	{
		return this.GetAltBlock(typeID).ToBlockValue();
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x0002B1A6 File Offset: 0x000293A6
	public string[] GetAltBlockNames()
	{
		return this.placeAltBlockNames;
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0002B1B0 File Offset: 0x000293B0
	public Block[] GetAltBlocks()
	{
		if (this.placeAltBlockClasses == null && this.placeAltBlockNames != null)
		{
			this.placeAltBlockClasses = new Block[this.placeAltBlockNames.Length];
			for (int i = 0; i < this.placeAltBlockNames.Length; i++)
			{
				this.placeAltBlockClasses[i] = Block.GetBlockByName(this.placeAltBlockNames[i], false);
			}
		}
		return this.placeAltBlockClasses;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0002B20F File Offset: 0x0002940F
	public int AlternateBlockCount()
	{
		return this.placeAltBlockNames.Length;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x0002B21C File Offset: 0x0002941C
	public bool ContainsAlternateBlock(string block)
	{
		for (int i = 0; i < this.placeAltBlockNames.Length; i++)
		{
			if (this.placeAltBlockNames[i] == block)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0002B250 File Offset: 0x00029450
	public int GetAlternateBlockIndex(string block)
	{
		for (int i = 0; i < this.placeAltBlockNames.Length; i++)
		{
			if (this.placeAltBlockNames[i] == block)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x0002B284 File Offset: 0x00029484
	public static void GetShapeCategories(IEnumerable<Block> _altBlocks, List<ShapesFromXml.ShapeCategory> _targetList)
	{
		_targetList.Clear();
		foreach (Block block in _altBlocks)
		{
			if (block.ShapeCategories != null)
			{
				foreach (ShapesFromXml.ShapeCategory item in block.ShapeCategories)
				{
					if (!_targetList.Contains(item))
					{
						_targetList.Add(item);
					}
				}
			}
		}
		_targetList.Sort();
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0002B324 File Offset: 0x00029524
	public int GetShownMaxDamage()
	{
		if (this is BlockDoor)
		{
			return this.MaxDamagePlusDowngrades;
		}
		return this.MaxDamage;
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0002B33B File Offset: 0x0002953B
	public bool SupportsRotation(byte _rotation)
	{
		if (_rotation < 4)
		{
			return (this.AllowedRotations & EBlockRotationClasses.Basic90) > EBlockRotationClasses.None;
		}
		if (_rotation < 8)
		{
			return (this.AllowedRotations & EBlockRotationClasses.Headfirst) > EBlockRotationClasses.None;
		}
		if (_rotation < 24)
		{
			return (this.AllowedRotations & EBlockRotationClasses.Sideways) > EBlockRotationClasses.None;
		}
		return (this.AllowedRotations & EBlockRotationClasses.Basic45) > EBlockRotationClasses.None;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0002B37C File Offset: 0x0002957C
	public void RotateHoldingBlock(ItemClassBlock.ItemBlockInventoryData _blockInventoryData, bool _increaseRotation, bool _playSoundOnRotation = true)
	{
		if (_blockInventoryData.mode == BlockPlacement.EnumRotationMode.Auto)
		{
			_blockInventoryData.mode = BlockPlacement.EnumRotationMode.Simple;
		}
		BlockValue bv = _blockInventoryData.itemValue.ToBlockValue();
		bv.rotation = _blockInventoryData.rotation;
		bv = this.BlockPlacementHelper.OnPlaceBlock(_blockInventoryData.mode, _blockInventoryData.localRot, _blockInventoryData.world, bv, _blockInventoryData.hitInfo.hit, _blockInventoryData.holdingEntity.position).blockValue;
		int rotation = (int)_blockInventoryData.rotation;
		_blockInventoryData.rotation = this.BlockPlacementHelper.LimitRotation(_blockInventoryData.mode, ref _blockInventoryData.localRot, _blockInventoryData.hitInfo.hit, _increaseRotation, bv, bv.rotation);
		if (_playSoundOnRotation && rotation != (int)_blockInventoryData.rotation)
		{
			_blockInventoryData.holdingEntity.PlayOneShot("rotateblock", false, false, false, null);
		}
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0002B444 File Offset: 0x00029644
	public void GroundAlign(BlockEntityData _data)
	{
		if (!_data.bHasTransform)
		{
			return;
		}
		BlockValue blockValue = _data.blockValue;
		int type = blockValue.type;
		Transform transform = _data.transform;
		GameObject gameObject = transform.gameObject;
		gameObject.SetActive(false);
		Vector3 vector = Vector3.zero;
		int num = 0;
		Ray ray = new Ray(Vector3.zero, Vector3.down);
		Vector3 b = new Vector3(0.5f, 0.75f, 0.5f) - Origin.position;
		float num2 = this.GroundAlignDistance + 0.5f;
		Vector3i pos = _data.pos;
		Vector3 vector2 = transform.position;
		Vector3 vector3;
		if (!this.isMultiBlock)
		{
			vector3 = new Vector3(0f, float.MinValue, 0f);
			ray.origin = pos.ToVector3() + b;
			RaycastHit raycastHit;
			bool flag = Physics.SphereCast(ray, 0.22f, out raycastHit, num2 - 0.22f + 0.25f, 1082195968);
			if (!flag)
			{
				flag = Physics.SphereCast(ray, 0.48f, out raycastHit, num2 - 0.48f + 0.25f, 1082195968);
			}
			if (flag)
			{
				vector3 = raycastHit.point;
				vector = raycastHit.normal;
				num = 1;
			}
		}
		else
		{
			if (blockValue.ischild)
			{
				pos = new Vector3i(blockValue.parentx, blockValue.parenty, blockValue.parentz);
			}
			vector3 = vector2;
			vector3.y = float.MinValue;
			byte rotation = blockValue.rotation;
			for (int i = this.multiBlockPos.Length - 1; i >= 0; i--)
			{
				Vector3i vector3i = this.multiBlockPos.Get(i, type, (int)rotation);
				if (vector3i.y == 0)
				{
					ray.origin = (pos + vector3i).ToVector3() + b;
					RaycastHit raycastHit;
					if (Physics.SphereCast(ray, 0.22f, out raycastHit, num2 - 0.22f + 0.25f, 1082195968))
					{
						if (vector3.y < raycastHit.point.y)
						{
							vector3.y = raycastHit.point.y;
						}
						vector += raycastHit.normal;
						num++;
					}
				}
			}
			if (num > 0)
			{
				vector *= 1f / (float)num;
				vector.Normalize();
			}
		}
		if (num > 0)
		{
			vector2 = vector3;
			Quaternion quaternion = transform.rotation;
			quaternion = Quaternion.FromToRotation(Vector3.up, vector) * quaternion;
			transform.SetPositionAndRotation(vector2, quaternion);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0002B6CC File Offset: 0x000298CC
	public static void CacheStats()
	{
		DynamicPropertiesCache propertiesCache = Block.PropertiesCache;
		if (propertiesCache == null)
		{
			return;
		}
		propertiesCache.Stats();
	}

	// Token: 0x04000622 RID: 1570
	public const int cAirId = 0;

	// Token: 0x04000623 RID: 1571
	public const int cTerrainStartId = 1;

	// Token: 0x04000624 RID: 1572
	public const int cWaterId = 240;

	// Token: 0x04000625 RID: 1573
	public const int cWaterPOIId = 241;

	// Token: 0x04000626 RID: 1574
	public const int cWaterDataId = 242;

	// Token: 0x04000627 RID: 1575
	public const int cGeneralStartId = 256;

	// Token: 0x04000628 RID: 1576
	public static int MAX_BLOCKS = 65536;

	// Token: 0x04000629 RID: 1577
	public static int ItemsStartHere = Block.MAX_BLOCKS;

	// Token: 0x0400062A RID: 1578
	public static bool FallInstantly = false;

	// Token: 0x0400062B RID: 1579
	public const int BlockFaceDrawn_Top = 1;

	// Token: 0x0400062C RID: 1580
	public const int BlockFaceDrawn_Bottom = 2;

	// Token: 0x0400062D RID: 1581
	public const int BlockFaceDrawn_North = 4;

	// Token: 0x0400062E RID: 1582
	public const int BlockFaceDrawn_West = 8;

	// Token: 0x0400062F RID: 1583
	public const int BlockFaceDrawn_South = 16;

	// Token: 0x04000630 RID: 1584
	public const int BlockFaceDrawn_East = 32;

	// Token: 0x04000631 RID: 1585
	public const int BlockFaceDrawn_AllORD = 63;

	// Token: 0x04000632 RID: 1586
	public const int BlockFaceDrawn_All = 255;

	// Token: 0x04000633 RID: 1587
	public static float cWaterLevel = 62.88f;

	// Token: 0x04000634 RID: 1588
	public static string PropCanPickup = "CanPickup";

	// Token: 0x04000635 RID: 1589
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPickupTarget = "PickupTarget";

	// Token: 0x04000636 RID: 1590
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPickupSource = "PickupSource";

	// Token: 0x04000637 RID: 1591
	public static string PropPlaceAltBlockValue = "PlaceAltBlockValue";

	// Token: 0x04000638 RID: 1592
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlaceShapeCategories = "ShapeCategories";

	// Token: 0x04000639 RID: 1593
	public static string PropSiblingBlock = "SiblingBlock";

	// Token: 0x0400063A RID: 1594
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFuelValue = "FuelValue";

	// Token: 0x0400063B RID: 1595
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropWeight = "Weight";

	// Token: 0x0400063C RID: 1596
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanMobsSpawnOn = "CanMobsSpawnOn";

	// Token: 0x0400063D RID: 1597
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanPlayersSpawnOn = "CanPlayersSpawnOn";

	// Token: 0x0400063E RID: 1598
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIndexName = "IndexName";

	// Token: 0x0400063F RID: 1599
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanBlocksReplace = "CanBlocksReplace";

	// Token: 0x04000640 RID: 1600
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCanDecorateOnSlopes = "CanDecorateOnSlopes";

	// Token: 0x04000641 RID: 1601
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSlopeMax = "SlopeMax";

	// Token: 0x04000642 RID: 1602
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsTerrainDecoration = "IsTerrainDecoration";

	// Token: 0x04000643 RID: 1603
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsDecoration = "IsDecoration";

	// Token: 0x04000644 RID: 1604
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDistantDecoration = "IsDistantDecoration";

	// Token: 0x04000645 RID: 1605
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBigDecorationRadius = "BigDecorationRadius";

	// Token: 0x04000646 RID: 1606
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSmallDecorationRadius = "SmallDecorationRadius";

	// Token: 0x04000647 RID: 1607
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGndAlign = "GndAlign";

	// Token: 0x04000648 RID: 1608
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIgnoreKeystoneOverlay = "IgnoreKeystoneOverlay";

	// Token: 0x04000649 RID: 1609
	public static string PropUpgradeBlockClass = "UpgradeBlock";

	// Token: 0x0400064A RID: 1610
	public static string PropUpgradeBlockClassToBlock = Block.PropUpgradeBlockClass + ".ToBlock";

	// Token: 0x0400064B RID: 1611
	public static string PropUpgradeBlockItemCount = "ItemCount";

	// Token: 0x0400064C RID: 1612
	public static string PropUpgradeBlockClassItemCount = Block.PropUpgradeBlockClass + ".ItemCount";

	// Token: 0x0400064D RID: 1613
	public static string PropDowngradeBlock = "DowngradeBlock";

	// Token: 0x0400064E RID: 1614
	public static string PropLockpickDowngradeBlock = "LockPickDowngradeBlock";

	// Token: 0x0400064F RID: 1615
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLPScale = "LPHardnessScale";

	// Token: 0x04000650 RID: 1616
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMapColor = "Map.Color";

	// Token: 0x04000651 RID: 1617
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMapColor2 = "Map.Color2";

	// Token: 0x04000652 RID: 1618
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMapSpecular = "Map.Specular";

	// Token: 0x04000653 RID: 1619
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMapElevMinMax = "Map.ElevMinMax";

	// Token: 0x04000654 RID: 1620
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGroupName = "Group";

	// Token: 0x04000655 RID: 1621
	public static string PropCustomIcon = "CustomIcon";

	// Token: 0x04000656 RID: 1622
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCustomIconTint = "CustomIconTint";

	// Token: 0x04000657 RID: 1623
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlacementWireframe = "PlacementWireframe";

	// Token: 0x04000658 RID: 1624
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMultiBlockDim = "MultiBlockDim";

	// Token: 0x04000659 RID: 1625
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOversizedBounds = "OversizedBounds";

	// Token: 0x0400065A RID: 1626
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTerrainAlignment = "TerrainAlignment";

	// Token: 0x0400065B RID: 1627
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMultiBlockLayer = "MultiBlockLayer";

	// Token: 0x0400065C RID: 1628
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropMultiBlockLayer0 = "MultiBlockLayer0";

	// Token: 0x0400065D RID: 1629
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsPlant = "IsPlant";

	// Token: 0x0400065E RID: 1630
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropHeatMapStrength = "HeatMapStrength";

	// Token: 0x0400065F RID: 1631
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropFallDamage = "FallDamage";

	// Token: 0x04000660 RID: 1632
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBuffsWhenWalkedOn = "BuffsWhenWalkedOn";

	// Token: 0x04000661 RID: 1633
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropRadiusEffect = "ActiveRadiusEffects";

	// Token: 0x04000662 RID: 1634
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCount = "Count";

	// Token: 0x04000663 RID: 1635
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropAllowAllRotations = "AllowAllRotations";

	// Token: 0x04000664 RID: 1636
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivationDistance = "ActivationDistance";

	// Token: 0x04000665 RID: 1637
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlacementDistance = "PlacementDistance";

	// Token: 0x04000666 RID: 1638
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropIsReplaceRandom = "IsReplaceRandom";

	// Token: 0x04000667 RID: 1639
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCraftExpValue = "CraftComponentExpValue";

	// Token: 0x04000668 RID: 1640
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCraftTimeValue = "CraftComponentTimeValue";

	// Token: 0x04000669 RID: 1641
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLootExpValue = "LootExpValue";

	// Token: 0x0400066A RID: 1642
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDestroyExpValue = "DestroyExpValue";

	// Token: 0x0400066B RID: 1643
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropParticleOnDeath = "ParticleOnDeath";

	// Token: 0x0400066C RID: 1644
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlaceExpValue = "PlaceExpValue";

	// Token: 0x0400066D RID: 1645
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropUpgradeExpValue = "UpgradeExpValue";

	// Token: 0x0400066E RID: 1646
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEconomicValue = "EconomicValue";

	// Token: 0x0400066F RID: 1647
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEconomicSellScale = "EconomicSellScale";

	// Token: 0x04000670 RID: 1648
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEconomicBundleSize = "EconomicBundleSize";

	// Token: 0x04000671 RID: 1649
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSellableToTrader = "SellableToTrader";

	// Token: 0x04000672 RID: 1650
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTraderStageTemplate = "TraderStageTemplate";

	// Token: 0x04000673 RID: 1651
	public static string PropResourceScale = "ResourceScale";

	// Token: 0x04000674 RID: 1652
	public static string PropMaxDamage = "MaxDamage";

	// Token: 0x04000675 RID: 1653
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStartDamage = "StartDamage";

	// Token: 0x04000676 RID: 1654
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStage2Health = "Stage2Health";

	// Token: 0x04000677 RID: 1655
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamage = "Damage";

	// Token: 0x04000678 RID: 1656
	public static string PropDescriptionKey = "DescriptionKey";

	// Token: 0x04000679 RID: 1657
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActionSkillGroup = "ActionSkillGroup";

	// Token: 0x0400067A RID: 1658
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropCraftingSkillGroup = "CraftingSkillGroup";

	// Token: 0x0400067B RID: 1659
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropShowModelOnFall = "ShowModelOnFall";

	// Token: 0x0400067C RID: 1660
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLightOpacity = "LightOpacity";

	// Token: 0x0400067D RID: 1661
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropHarvestOverdamage = "HarvestOverdamage";

	// Token: 0x0400067E RID: 1662
	public static string PropTintColor = "TintColor";

	// Token: 0x0400067F RID: 1663
	public static string PropCreativeMode = "CreativeMode";

	// Token: 0x04000680 RID: 1664
	public static string PropFilterTag = "FilterTags";

	// Token: 0x04000681 RID: 1665
	public static string PropTag = "Tags";

	// Token: 0x04000682 RID: 1666
	public static string PropCreativeSort1 = "SortOrder1";

	// Token: 0x04000683 RID: 1667
	public static string PropCreativeSort2 = "SortOrder2";

	// Token: 0x04000684 RID: 1668
	public static string PropDisplayType = "DisplayType";

	// Token: 0x04000685 RID: 1669
	public static string PropUnlockedBy = "UnlockedBy";

	// Token: 0x04000686 RID: 1670
	public static string PropNoScrapping = "NoScrapping";

	// Token: 0x04000687 RID: 1671
	public static string PropVehicleHitScale = "VehicleHitScale";

	// Token: 0x04000688 RID: 1672
	public static string PropItemTypeIcon = "ItemTypeIcon";

	// Token: 0x04000689 RID: 1673
	public static string PropAutoShape = "AutoShape";

	// Token: 0x0400068A RID: 1674
	public static string PropBlockAddedEvent = "AddedEvent";

	// Token: 0x0400068B RID: 1675
	public static string PropBlockDestroyedEvent = "DestroyedEvent";

	// Token: 0x0400068C RID: 1676
	public static string PropBlockDowngradeEvent = "DowngradeEvent";

	// Token: 0x0400068D RID: 1677
	public static string PropBlockDowngradedToEvent = "DowngradedToEvent";

	// Token: 0x0400068E RID: 1678
	public static string PropIsTemporaryBlock = "IsTemporaryBlock";

	// Token: 0x0400068F RID: 1679
	public static string PropRefundOnUnload = "RefundOnUnload";

	// Token: 0x04000690 RID: 1680
	public static string PropSoundPickup = "SoundPickup";

	// Token: 0x04000691 RID: 1681
	public static string PropSoundPlace = "SoundPlace";

	// Token: 0x04000692 RID: 1682
	public static string PropCustomCommandName = "CustomCommandName";

	// Token: 0x04000693 RID: 1683
	public static string PropCustomCommandIcon = "CustomCommandIcon";

	// Token: 0x04000694 RID: 1684
	public static string PropCustomCommandIconColor = "CustomCommandIconColor";

	// Token: 0x04000695 RID: 1685
	public static string PropCustomCommandEvent = "CustomCommandEvent";

	// Token: 0x04000696 RID: 1686
	public static NameIdMapping nameIdMapping;

	// Token: 0x04000697 RID: 1687
	public static byte[] fullMappingDataForClients;

	// Token: 0x04000698 RID: 1688
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Block> nameToBlock;

	// Token: 0x04000699 RID: 1689
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Block> nameToBlockCaseInsensitive;

	// Token: 0x0400069A RID: 1690
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string[]> groupNameStringToGroupNames;

	// Token: 0x0400069B RID: 1691
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<string> autoShapeMaterials;

	// Token: 0x0400069C RID: 1692
	public static Block[] list;

	// Token: 0x0400069D RID: 1693
	public static DynamicPropertiesCache PropertiesCache;

	// Token: 0x0400069E RID: 1694
	public static string defaultBlockDescriptionKey = "";

	// Token: 0x0400069F RID: 1695
	public int blockID;

	// Token: 0x040006A0 RID: 1696
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicProperties dynamicProperties;

	// Token: 0x040006A1 RID: 1697
	public BlockShape shape;

	// Token: 0x040006A2 RID: 1698
	public int BlockingType;

	// Token: 0x040006A3 RID: 1699
	public BlockValue SiblingBlock;

	// Token: 0x040006A4 RID: 1700
	public BlockTags BlockTag;

	// Token: 0x040006A5 RID: 1701
	public BlockPlacement BlockPlacementHelper;

	// Token: 0x040006A6 RID: 1702
	public bool CanBlocksReplace;

	// Token: 0x040006A7 RID: 1703
	public float LPHardnessScale;

	// Token: 0x040006A8 RID: 1704
	public float MovementFactor;

	// Token: 0x040006A9 RID: 1705
	public bool CanPickup;

	// Token: 0x040006AA RID: 1706
	public string PickedUpItemValue;

	// Token: 0x040006AB RID: 1707
	public string PickupTarget;

	// Token: 0x040006AC RID: 1708
	public string PickupSource;

	// Token: 0x040006AD RID: 1709
	public byte BlocksMovement;

	// Token: 0x040006AE RID: 1710
	public int FuelValue;

	// Token: 0x040006AF RID: 1711
	public DataItem<int> Weight;

	// Token: 0x040006B0 RID: 1712
	public bool CanMobsSpawnOn;

	// Token: 0x040006B1 RID: 1713
	public bool CanPlayersSpawnOn;

	// Token: 0x040006B2 RID: 1714
	public string IndexName;

	// Token: 0x040006B3 RID: 1715
	public bool CanDecorateOnSlopes;

	// Token: 0x040006B4 RID: 1716
	public float SlopeMaxCos;

	// Token: 0x040006B5 RID: 1717
	public bool IsTerrainDecoration;

	// Token: 0x040006B6 RID: 1718
	public bool IsDecoration;

	// Token: 0x040006B7 RID: 1719
	public bool IsDistantDecoration;

	// Token: 0x040006B8 RID: 1720
	public int BigDecorationRadius;

	// Token: 0x040006B9 RID: 1721
	public int SmallDecorationRadius;

	// Token: 0x040006BA RID: 1722
	public float GroundAlignDistance;

	// Token: 0x040006BB RID: 1723
	public bool IgnoreKeystoneOverlay;

	// Token: 0x040006BC RID: 1724
	public const int cPathScan = -1;

	// Token: 0x040006BD RID: 1725
	public const int cPathSolid = 1;

	// Token: 0x040006BE RID: 1726
	public int PathType;

	// Token: 0x040006BF RID: 1727
	public float PathOffsetX;

	// Token: 0x040006C0 RID: 1728
	public float PathOffsetZ;

	// Token: 0x040006C1 RID: 1729
	public BlockFaceFlag WaterFlowMask = BlockFaceFlag.All;

	// Token: 0x040006C2 RID: 1730
	public bool WaterClipEnabled;

	// Token: 0x040006C3 RID: 1731
	public Plane WaterClipPlane;

	// Token: 0x040006C4 RID: 1732
	public BlockValue DowngradeBlock;

	// Token: 0x040006C5 RID: 1733
	public BlockValue LockpickDowngradeBlock;

	// Token: 0x040006C6 RID: 1734
	public BlockValue UpgradeBlock;

	// Token: 0x040006C7 RID: 1735
	public string[] GroupNames = new string[]
	{
		"Decor/Miscellaneous"
	};

	// Token: 0x040006C8 RID: 1736
	public string CustomIcon;

	// Token: 0x040006C9 RID: 1737
	public Color CustomIconTint;

	// Token: 0x040006CA RID: 1738
	public bool bHasPlacementWireframe;

	// Token: 0x040006CB RID: 1739
	public bool bUserHidden;

	// Token: 0x040006CC RID: 1740
	public float FallDamage;

	// Token: 0x040006CD RID: 1741
	public float HeatMapStrength;

	// Token: 0x040006CE RID: 1742
	public string[] BuffsWhenWalkedOn;

	// Token: 0x040006CF RID: 1743
	public BlockRadiusEffect[] RadiusEffects;

	// Token: 0x040006D0 RID: 1744
	public string DescriptionKey;

	// Token: 0x040006D1 RID: 1745
	public string CraftingSkillGroup = "";

	// Token: 0x040006D2 RID: 1746
	public string ActionSkillGroup = "";

	// Token: 0x040006D3 RID: 1747
	public bool IsReplaceRandom = true;

	// Token: 0x040006D4 RID: 1748
	public float CraftComponentExp = 1f;

	// Token: 0x040006D5 RID: 1749
	public float CraftComponentTime = 1f;

	// Token: 0x040006D6 RID: 1750
	public float LootExp = 1f;

	// Token: 0x040006D7 RID: 1751
	public float DestroyExp = 1f;

	// Token: 0x040006D8 RID: 1752
	[PublicizedFrom(EAccessModifier.Protected)]
	public string deathParticleName;

	// Token: 0x040006D9 RID: 1753
	public float EconomicValue;

	// Token: 0x040006DA RID: 1754
	public float EconomicSellScale = 1f;

	// Token: 0x040006DB RID: 1755
	public int EconomicBundleSize = 1;

	// Token: 0x040006DC RID: 1756
	public bool SellableToTrader = true;

	// Token: 0x040006DD RID: 1757
	public string TraderStageTemplate;

	// Token: 0x040006DE RID: 1758
	public float PlaceExp = 1f;

	// Token: 0x040006DF RID: 1759
	public float UpgradeExp = 1f;

	// Token: 0x040006E0 RID: 1760
	public int Count = 1;

	// Token: 0x040006E1 RID: 1761
	public int Stacknumber = 500;

	// Token: 0x040006E2 RID: 1762
	public bool HarvestOverdamage;

	// Token: 0x040006E3 RID: 1763
	public bool SelectAlternates;

	// Token: 0x040006E4 RID: 1764
	public EnumCreativeMode CreativeMode;

	// Token: 0x040006E5 RID: 1765
	public string[] FilterTags;

	// Token: 0x040006E6 RID: 1766
	public bool NoScrapping;

	// Token: 0x040006E7 RID: 1767
	public string SortOrder;

	// Token: 0x040006E8 RID: 1768
	public string DisplayType = "defaultBlock";

	// Token: 0x040006E9 RID: 1769
	[PublicizedFrom(EAccessModifier.Private)]
	public RecipeUnlockData[] unlockedBy;

	// Token: 0x040006EA RID: 1770
	public string ItemTypeIcon = "";

	// Token: 0x040006EB RID: 1771
	[PublicizedFrom(EAccessModifier.Private)]
	public EAutoShapeType AutoShapeType;

	// Token: 0x040006EC RID: 1772
	[PublicizedFrom(EAccessModifier.Private)]
	public string autoShapeBaseName;

	// Token: 0x040006ED RID: 1773
	[PublicizedFrom(EAccessModifier.Private)]
	public string autoShapeShapeName;

	// Token: 0x040006EE RID: 1774
	[PublicizedFrom(EAccessModifier.Private)]
	public Block autoShapeHelper;

	// Token: 0x040006EF RID: 1775
	public float VehicleHitScale;

	// Token: 0x040006F0 RID: 1776
	[PublicizedFrom(EAccessModifier.Private)]
	public Color MapColor;

	// Token: 0x040006F1 RID: 1777
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bMapColorSet;

	// Token: 0x040006F2 RID: 1778
	[PublicizedFrom(EAccessModifier.Private)]
	public Color MapColor2;

	// Token: 0x040006F3 RID: 1779
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bMapColor2Set;

	// Token: 0x040006F4 RID: 1780
	[PublicizedFrom(EAccessModifier.Private)]
	public float MapSpecular;

	// Token: 0x040006F5 RID: 1781
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i MapElevMinMax;

	// Token: 0x040006F6 RID: 1782
	[PublicizedFrom(EAccessModifier.Private)]
	public byte lightValue;

	// Token: 0x040006F7 RID: 1783
	public int lightOpacity;

	// Token: 0x040006F8 RID: 1784
	public Color tintColor = Color.clear;

	// Token: 0x040006F9 RID: 1785
	public Color defaultTintColor = Color.clear;

	// Token: 0x040006FA RID: 1786
	public Vector3 tintColorV = Vector3.one;

	// Token: 0x040006FB RID: 1787
	public byte MeshIndex;

	// Token: 0x040006FC RID: 1788
	public List<Block.SItemNameCount> RepairItems;

	// Token: 0x040006FD RID: 1789
	public List<Block.SItemNameCount> RepairItemsMeshDamage;

	// Token: 0x040006FE RID: 1790
	public bool bRestrictSubmergedPlacement;

	// Token: 0x040006FF RID: 1791
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockAddedEvent;

	// Token: 0x04000700 RID: 1792
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockDestroyedEvent;

	// Token: 0x04000701 RID: 1793
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockDowngradeEvent;

	// Token: 0x04000702 RID: 1794
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockDowngradedToEvent;

	// Token: 0x04000703 RID: 1795
	public bool IsTemporaryBlock;

	// Token: 0x04000704 RID: 1796
	public bool RefundOnUnload;

	// Token: 0x04000705 RID: 1797
	public string SoundPickup = "craft_take_item";

	// Token: 0x04000706 RID: 1798
	public string SoundPlace = "craft_place_item";

	// Token: 0x04000707 RID: 1799
	public bool isMultiBlock;

	// Token: 0x04000708 RID: 1800
	public Block.MultiBlockArray multiBlockPos;

	// Token: 0x04000709 RID: 1801
	public bool isOversized;

	// Token: 0x0400070A RID: 1802
	public Bounds oversizedBounds;

	// Token: 0x0400070B RID: 1803
	public TerrainAlignmentMode terrainAlignmentMode;

	// Token: 0x0400070C RID: 1804
	public const int BT_All = 255;

	// Token: 0x0400070D RID: 1805
	public const int BT_None = 0;

	// Token: 0x0400070E RID: 1806
	public const int BT_Sight = 1;

	// Token: 0x0400070F RID: 1807
	public const int BT_Movement = 2;

	// Token: 0x04000710 RID: 1808
	public const int BT_Bullets = 4;

	// Token: 0x04000711 RID: 1809
	public const int BT_Rockets = 8;

	// Token: 0x04000712 RID: 1810
	public const int BT_Melee = 16;

	// Token: 0x04000713 RID: 1811
	public const int BT_Arrows = 32;

	// Token: 0x04000714 RID: 1812
	public bool IsCheckCollideWithEntity;

	// Token: 0x04000715 RID: 1813
	[PublicizedFrom(EAccessModifier.Private)]
	public Block.TextureInfo[] textureInfos = new Block.TextureInfo[1];

	// Token: 0x04000716 RID: 1814
	[PublicizedFrom(EAccessModifier.Private)]
	public int uiBackgroundTextureId = -1;

	// Token: 0x04000717 RID: 1815
	public int TerrainTAIndex = 1;

	// Token: 0x04000718 RID: 1816
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bNotifyOnLoadUnload;

	// Token: 0x04000719 RID: 1817
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bIsPlant;

	// Token: 0x0400071A RID: 1818
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShowModelOnFall;

	// Token: 0x0400071B RID: 1819
	public Dictionary<EnumDropEvent, List<Block.SItemDropProb>> itemsToDrop = new EnumDictionary<EnumDropEvent, List<Block.SItemDropProb>>();

	// Token: 0x0400071C RID: 1820
	public bool IsSleeperBlock;

	// Token: 0x0400071D RID: 1821
	public bool IsRandomlyTick;

	// Token: 0x0400071E RID: 1822
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] placeAltBlockNames;

	// Token: 0x0400071F RID: 1823
	[PublicizedFrom(EAccessModifier.Private)]
	public Block[] placeAltBlockClasses;

	// Token: 0x04000721 RID: 1825
	public MaterialBlock blockMaterial;

	// Token: 0x04000722 RID: 1826
	public bool StabilitySupport = true;

	// Token: 0x04000723 RID: 1827
	public bool StabilityIgnore;

	// Token: 0x04000724 RID: 1828
	public bool StabilityFull;

	// Token: 0x04000725 RID: 1829
	public sbyte Density;

	// Token: 0x04000726 RID: 1830
	[PublicizedFrom(EAccessModifier.Private)]
	public string blockName;

	// Token: 0x04000727 RID: 1831
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedBlockName;

	// Token: 0x04000728 RID: 1832
	public float ResourceScale;

	// Token: 0x04000729 RID: 1833
	public int MaxDamage;

	// Token: 0x0400072A RID: 1834
	public int MaxDamagePlusDowngrades;

	// Token: 0x0400072B RID: 1835
	public int StartDamage;

	// Token: 0x0400072C RID: 1836
	[PublicizedFrom(EAccessModifier.Private)]
	public int Stage2Health;

	// Token: 0x0400072D RID: 1837
	public float Damage;

	// Token: 0x0400072E RID: 1838
	public EBlockRotationClasses AllowedRotations;

	// Token: 0x0400072F RID: 1839
	public bool PlaceRandomRotation;

	// Token: 0x04000730 RID: 1840
	public string CustomPlaceSound;

	// Token: 0x04000731 RID: 1841
	public string UpgradeSound;

	// Token: 0x04000732 RID: 1842
	public string DowngradeFX;

	// Token: 0x04000733 RID: 1843
	public string DestroyFX;

	// Token: 0x04000734 RID: 1844
	[PublicizedFrom(EAccessModifier.Private)]
	public int activationDistance;

	// Token: 0x04000735 RID: 1845
	[PublicizedFrom(EAccessModifier.Private)]
	public int placementDistance;

	// Token: 0x04000736 RID: 1846
	public int cUVModeBits = 2;

	// Token: 0x04000737 RID: 1847
	public int cUVModeMask = 3;

	// Token: 0x04000738 RID: 1848
	public int cUVModeSides = 7;

	// Token: 0x04000739 RID: 1849
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly uint[] UVModesPerSide = new uint[1];

	// Token: 0x0400073A RID: 1850
	public bool bImposterExclude;

	// Token: 0x0400073B RID: 1851
	public bool bImposterExcludeAndStop;

	// Token: 0x0400073C RID: 1852
	public int ImposterExchange;

	// Token: 0x0400073D RID: 1853
	public byte ImposterExchangeTexIdx;

	// Token: 0x0400073E RID: 1854
	public bool bImposterDontBlock;

	// Token: 0x0400073F RID: 1855
	public int MergeIntoId;

	// Token: 0x04000740 RID: 1856
	public int[] MergeIntoTexIds;

	// Token: 0x04000741 RID: 1857
	public int MirrorSibling;

	// Token: 0x04000742 RID: 1858
	[PublicizedFrom(EAccessModifier.Protected)]
	public static List<Bounds> staticList_IntersectRayWithBlockList = new List<Bounds>();

	// Token: 0x04000743 RID: 1859
	public BlockFace HandleFace = BlockFace.None;

	// Token: 0x04000744 RID: 1860
	public bool EnablePassThroughDamage;

	// Token: 0x04000745 RID: 1861
	public List<BlockFace> RemovePaintOnDowngrade;

	// Token: 0x04000746 RID: 1862
	public FastTags<TagGroup.Global> Tags;

	// Token: 0x04000747 RID: 1863
	public bool HasTileEntity;

	// Token: 0x04000748 RID: 1864
	public Block.EnumDisplayInfo DisplayInfo;

	// Token: 0x04000749 RID: 1865
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("take", "hand", false, false, null),
		new BlockActivationCommand("Search", "search", false, false, null)
	};

	// Token: 0x0400074A RID: 1866
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ItemStack> itemsDropped = new List<ItemStack>();

	// Token: 0x0400074B RID: 1867
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockActivationCommand[] customCmds;

	// Token: 0x0400074C RID: 1868
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, int> fixedBlockIds = new Dictionary<string, int>
	{
		{
			"air",
			0
		},
		{
			"water",
			240
		},
		{
			"terrWaterPOI",
			241
		},
		{
			"waterdata",
			242
		}
	};

	// Token: 0x020000E7 RID: 231
	public struct SItemDropProb
	{
		// Token: 0x06000600 RID: 1536 RVA: 0x0002BB53 File Offset: 0x00029D53
		public SItemDropProb(string _name, int _minCount, int _maxCount, float _prob, float _resourceScale, float _stickChance, string _toolCategory, string _tag)
		{
			this.name = _name;
			this.minCount = _minCount;
			this.maxCount = _maxCount;
			this.prob = _prob;
			this.resourceScale = _resourceScale;
			this.stickChance = _stickChance;
			this.toolCategory = _toolCategory;
			this.tag = _tag;
		}

		// Token: 0x0400074D RID: 1869
		public string name;

		// Token: 0x0400074E RID: 1870
		public int minCount;

		// Token: 0x0400074F RID: 1871
		public int maxCount;

		// Token: 0x04000750 RID: 1872
		public float prob;

		// Token: 0x04000751 RID: 1873
		public float resourceScale;

		// Token: 0x04000752 RID: 1874
		public float stickChance;

		// Token: 0x04000753 RID: 1875
		public string toolCategory;

		// Token: 0x04000754 RID: 1876
		public string tag;
	}

	// Token: 0x020000E8 RID: 232
	public struct SItemNameCount
	{
		// Token: 0x04000755 RID: 1877
		public string ItemName;

		// Token: 0x04000756 RID: 1878
		public int Count;
	}

	// Token: 0x020000E9 RID: 233
	public class MultiBlockArray
	{
		// Token: 0x06000601 RID: 1537 RVA: 0x0002BB92 File Offset: 0x00029D92
		public MultiBlockArray(Vector3i _dim, List<Vector3i> _pos)
		{
			this.dim = _dim;
			this.pos = _pos.ToArray();
			this.Length = _pos.Count;
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0002BBBC File Offset: 0x00029DBC
		public Vector3i Get(int _idx, int _blockId, int _rotation)
		{
			Vector3 vector = Block.list[_blockId].shape.GetRotation(new BlockValue
			{
				type = _blockId,
				rotation = (byte)_rotation
			}) * this.pos[_idx].ToVector3();
			return new Vector3i(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0002BC31 File Offset: 0x00029E31
		public Vector3i GetParentPos(Vector3i _childPos, BlockValue _blockValue)
		{
			return new Vector3i(_childPos.x + _blockValue.parentx, _childPos.y + _blockValue.parenty, _childPos.z + _blockValue.parentz);
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x0002BC64 File Offset: 0x00029E64
		public void AddChilds(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			byte rotation = _blockValue.rotation;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				Vector3i vector3i = this.Get(i, _blockValue.type, (int)rotation);
				if (!(vector3i == Vector3i.zero))
				{
					Vector3i vector3i2 = _blockPos + vector3i;
					int x = World.toBlockXZ(vector3i2.x);
					int z = World.toBlockXZ(vector3i2.z);
					int y = vector3i2.y;
					if (y >= 0 && y < 254)
					{
						Chunk chunk = (Chunk)chunkCache.GetChunkFromWorldPos(vector3i2);
						if (chunk == null)
						{
							long num = WorldChunkCache.MakeChunkKey(World.toChunkXZ(vector3i2.x), World.toChunkXZ(vector3i2.z));
							if (_chunk.Key == num)
							{
								chunk = _chunk;
							}
						}
						if (chunk != null)
						{
							BlockValue block = chunk.GetBlock(x, y, z);
							if (block.isair || !block.Block.shape.IsTerrain())
							{
								BlockValue blockValue = _blockValue;
								blockValue.ischild = true;
								blockValue.parentx = -vector3i.x;
								blockValue.parenty = -vector3i.y;
								blockValue.parentz = -vector3i.z;
								chunk.SetBlock(_world, x, y, z, blockValue, false, true, false, false, -1);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0002BDBC File Offset: 0x00029FBC
		public void RemoveChilds(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			byte rotation = _blockValue.rotation;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				Vector3i vector3i = this.Get(i, _blockValue.type, (int)rotation);
				if ((vector3i.x != 0 || vector3i.y != 0 || vector3i.z != 0) && chunkCache.GetBlock(_blockPos + vector3i).type == _blockValue.type)
				{
					chunkCache.SetBlock(_blockPos + vector3i, true, BlockValue.Air, true, MarchingCubes.DensityAir, false, false, false, true, -1);
				}
			}
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0002BE58 File Offset: 0x0002A058
		public void RemoveParentBlock(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
		{
			ChunkCluster chunkCache = _world.ChunkCache;
			if (chunkCache == null)
			{
				return;
			}
			Vector3i parentPos = this.GetParentPos(_blockPos, _blockValue);
			BlockValue block = chunkCache.GetBlock(parentPos);
			if (!block.ischild && block.type == _blockValue.type)
			{
				chunkCache.SetBlock(parentPos, BlockValue.Air, true, true);
			}
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0002BEAC File Offset: 0x0002A0AC
		public bool ContainsPos(WorldBase _world, Vector3i _parentPos, BlockValue _blockValue, Vector3i _posToCheck)
		{
			if (_world.ChunkCache == null)
			{
				return false;
			}
			byte rotation = _blockValue.rotation;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				if (_parentPos + this.Get(i, _blockValue.type, (int)rotation) == _posToCheck)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0002BF00 File Offset: 0x0002A100
		public Bounds CalcBounds(int _blockId, int _rotation)
		{
			Quaternion rotation = Block.list[_blockId].shape.GetRotation(new BlockValue
			{
				type = _blockId,
				rotation = (byte)_rotation
			});
			Vector3 vector = Vector3.positiveInfinity;
			Vector3 vector2 = Vector3.negativeInfinity;
			for (int i = this.Length - 1; i >= 0; i--)
			{
				Vector3 rhs = rotation * this.pos[i].ToVector3();
				vector = Vector3.Min(vector, rhs);
				vector2 = Vector3.Max(vector2, rhs);
			}
			Bounds result = default(Bounds);
			result.SetMinMax(vector, vector2);
			return result;
		}

		// Token: 0x04000757 RID: 1879
		public int Length;

		// Token: 0x04000758 RID: 1880
		public Vector3i dim;

		// Token: 0x04000759 RID: 1881
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i[] pos;
	}

	// Token: 0x020000EA RID: 234
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TextureInfo
	{
		// Token: 0x0400075A RID: 1882
		public bool bTextureForEachSide;

		// Token: 0x0400075B RID: 1883
		public int singleTextureId;

		// Token: 0x0400075C RID: 1884
		public int[] sideTextureIds;
	}

	// Token: 0x020000EB RID: 235
	public enum UVMode : byte
	{
		// Token: 0x0400075E RID: 1886
		Default,
		// Token: 0x0400075F RID: 1887
		Global,
		// Token: 0x04000760 RID: 1888
		Local
	}

	// Token: 0x020000EC RID: 236
	public enum EnumDisplayInfo
	{
		// Token: 0x04000762 RID: 1890
		None,
		// Token: 0x04000763 RID: 1891
		Name,
		// Token: 0x04000764 RID: 1892
		Description,
		// Token: 0x04000765 RID: 1893
		Custom
	}

	// Token: 0x020000ED RID: 237
	public enum DestroyedResult
	{
		// Token: 0x04000767 RID: 1895
		Keep,
		// Token: 0x04000768 RID: 1896
		Downgrade,
		// Token: 0x04000769 RID: 1897
		Remove
	}
}
