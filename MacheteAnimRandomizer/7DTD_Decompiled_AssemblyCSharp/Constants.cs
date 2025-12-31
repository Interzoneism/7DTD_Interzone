using System;
using UnityEngine;

// Token: 0x02000F33 RID: 3891
public class Constants
{
	// Token: 0x04005DDD RID: 24029
	public static bool IsWebPlayer = false;

	// Token: 0x04005DDE RID: 24030
	public static bool Is32BitOs = IntPtr.Size == 4;

	// Token: 0x04005DDF RID: 24031
	public const int cMinShadowDistance = 20;

	// Token: 0x04005DE0 RID: 24032
	public const string cOptionsIni = "UserOptions.ini";

	// Token: 0x04005DE1 RID: 24033
	public const string cProduct = "7 Days To Die";

	// Token: 0x04005DE2 RID: 24034
	public const string cProductAbbrev = "7DTD";

	// Token: 0x04005DE3 RID: 24035
	public const VersionInformation.EGameReleaseType cReleaseType = VersionInformation.EGameReleaseType.V;

	// Token: 0x04005DE4 RID: 24036
	public const int cVersionMajor = 2;

	// Token: 0x04005DE5 RID: 24037
	public const int cVersionMinor = 4;

	// Token: 0x04005DE6 RID: 24038
	public const int cVersionBuild = 7;

	// Token: 0x04005DE7 RID: 24039
	public static readonly VersionInformation cVersionInformation = new VersionInformation(VersionInformation.EGameReleaseType.V, 2, 4, 7);

	// Token: 0x04005DE8 RID: 24040
	public const int cGameResetRevision = 13;

	// Token: 0x04005DE9 RID: 24041
	public const int cGraphicsResetRevision = 4;

	// Token: 0x04005DEA RID: 24042
	public const int cControlsResetRevision = 7;

	// Token: 0x04005DEB RID: 24043
	public const string cCopyright = "Copyright (c) 2014-2025 The Fun Pimps LLC All Rights Reserved.";

	// Token: 0x04005DEC RID: 24044
	public const int cMaxMPPlayers = 8;

	// Token: 0x04005DED RID: 24045
	public const int cMaxCrossplayMPPlayers = 8;

	// Token: 0x04005DEE RID: 24046
	public const int cDefaultUserPermissionLevel = 1000;

	// Token: 0x04005DEF RID: 24047
	public const string cDefaultPlayerName = "Player";

	// Token: 0x04005DF0 RID: 24048
	public const string cDirWorlds = "Data/Worlds";

	// Token: 0x04005DF1 RID: 24049
	public const string cDirPrefabs = "Data/Prefabs";

	// Token: 0x04005DF2 RID: 24050
	public const string cDirBluff = "Data/Bluffs";

	// Token: 0x04005DF3 RID: 24051
	public const string cDirBlocks = "Data/Config";

	// Token: 0x04005DF4 RID: 24052
	public const string cDirGroups = "Data/Config/Groups";

	// Token: 0x04005DF5 RID: 24053
	public const string cDirItems = "Data/Config";

	// Token: 0x04005DF6 RID: 24054
	public const string cDirWorldCreation = "Data/Config";

	// Token: 0x04005DF7 RID: 24055
	public const string cDirConfig = "Data/Config";

	// Token: 0x04005DF8 RID: 24056
	public const string cDirHeightmaps = "Data/Heightmaps";

	// Token: 0x04005DF9 RID: 24057
	public const string cDirBackgroundImage = "Data/Textures/misc/background.jpg";

	// Token: 0x04005DFA RID: 24058
	public const string cDirAssetBundles = "Data/Bundles/Standalone";

	// Token: 0x04005DFB RID: 24059
	public const string cDirPrefabParts = "Data/Prefabs/Parts";

	// Token: 0x04005DFC RID: 24060
	public const string cFolderResourcesSounds = "Sounds";

	// Token: 0x04005DFD RID: 24061
	public const string cFolderResourcesConfig = "Data/Config";

	// Token: 0x04005DFE RID: 24062
	public const string cFolderResourcesTextures = "Textures";

	// Token: 0x04005DFF RID: 24063
	public const string cFolderResourcesEnvironment = "Textures/Environment";

	// Token: 0x04005E00 RID: 24064
	public const string cFolderResourcesTerrainTextures = "Textures/Terrain";

	// Token: 0x04005E01 RID: 24065
	public const string cFolderResourcesLocalizationEnglish = "GUI/Localization/English";

	// Token: 0x04005E02 RID: 24066
	public const string cFolderSaveGame = "Saves";

	// Token: 0x04005E03 RID: 24067
	public const string cFolderSaveGameLocal = "SavesLocal";

	// Token: 0x04005E04 RID: 24068
	public const string cFolderSaveRegion = "Region";

	// Token: 0x04005E05 RID: 24069
	public const string cFolderSavePlayer = "Player";

	// Token: 0x04005E06 RID: 24070
	public const string cFolderGeneratedWorlds = "GeneratedWorlds";

	// Token: 0x04005E07 RID: 24071
	public const string cFolderLocalPrefabs = "LocalPrefabs";

	// Token: 0x04005E08 RID: 24072
	public const string cFolderSaveTwitch = "Twitch";

	// Token: 0x04005E09 RID: 24073
	public const string cDirConfigInternal = "DataInternal/Config";

	// Token: 0x04005E0A RID: 24074
	public const string cExtLevels = ".ttw";

	// Token: 0x04005E0B RID: 24075
	public const string cExtPrefabs = ".tts";

	// Token: 0x04005E0C RID: 24076
	public const string cExtPrefabImposters = ".mesh";

	// Token: 0x04005E0D RID: 24077
	public const string cExtIdNameMappings = ".nim";

	// Token: 0x04005E0E RID: 24078
	public const string cExtPlayedLevel = ".played";

	// Token: 0x04005E0F RID: 24079
	public const string cExtSdf = ".sdf";

	// Token: 0x04005E10 RID: 24080
	public const string cExtFlag = ".flag";

	// Token: 0x04005E11 RID: 24081
	public const string cFileMainTTW = "main.ttw";

	// Token: 0x04005E12 RID: 24082
	public const string cFileWorldChecksums = "checksums.txt";

	// Token: 0x04005E13 RID: 24083
	public static readonly string cFileBlockMappings = "blockmappings.nim";

	// Token: 0x04005E14 RID: 24084
	public static readonly string cFileItemMappings = "itemmappings.nim";

	// Token: 0x04005E15 RID: 24085
	public const string cFileDecos = "decoration.7dt";

	// Token: 0x04005E16 RID: 24086
	public const string cFileMultiBlocks = "multiblocks.7dt";

	// Token: 0x04005E17 RID: 24087
	public const string cGameNameDefault = "Region";

	// Token: 0x04005E18 RID: 24088
	public const string cSdfFileName = "gameOptions.sdf";

	// Token: 0x04005E19 RID: 24089
	public const string cPersistentPlayersFileName = "players.xml";

	// Token: 0x04005E1A RID: 24090
	public const string cAchievementFilename = "achievements.bin";

	// Token: 0x04005E1B RID: 24091
	public const string cNewGameSdfFileName = "newGameOptions.sdf";

	// Token: 0x04005E1C RID: 24092
	public const string cFileArchivedFlag = "archived.flag";

	// Token: 0x04005E1D RID: 24093
	public static string cPrefixAtlas = "ta_";

	// Token: 0x04005E1E RID: 24094
	public const string cLevelPrefab = "prefabs";

	// Token: 0x04005E1F RID: 24095
	public const string cSpawnPoints = "spawnpoints";

	// Token: 0x04005E20 RID: 24096
	public const string cExtTexturePack = ".xml";

	// Token: 0x04005E21 RID: 24097
	public static string cArgDedicatedServer = "-dedicated";

	// Token: 0x04005E22 RID: 24098
	public static string cArgSubmissionBuild = "-submission";

	// Token: 0x04005E23 RID: 24099
	public const int cMaxBiomes = 50;

	// Token: 0x04005E24 RID: 24100
	public const string cTagLargeEntityBlocker = "LargeEntityBlocker";

	// Token: 0x04005E25 RID: 24101
	public const string cTagPhysics = "Physics";

	// Token: 0x04005E26 RID: 24102
	public const int cLayerDefault = 0;

	// Token: 0x04005E27 RID: 24103
	public const int cLayerTransparentFx = 1;

	// Token: 0x04005E28 RID: 24104
	public const int cLayerIgnoreRaycast = 2;

	// Token: 0x04005E29 RID: 24105
	public const int cLayerWater = 4;

	// Token: 0x04005E2A RID: 24106
	public const int cLayerNoShadow = 8;

	// Token: 0x04005E2B RID: 24107
	public const int cLayerBackgroundImage = 9;

	// Token: 0x04005E2C RID: 24108
	public const int cLayerHoldingItem = 10;

	// Token: 0x04005E2D RID: 24109
	public const int cLayerRenderInTexture = 11;

	// Token: 0x04005E2E RID: 24110
	public const int cLayerNGUI = 12;

	// Token: 0x04005E2F RID: 24111
	public const int cLayerItems = 13;

	// Token: 0x04005E30 RID: 24112
	public const int cLayerNoCollision = 14;

	// Token: 0x04005E31 RID: 24113
	public const int cLayerCCPhysics = 15;

	// Token: 0x04005E32 RID: 24114
	public const int cLayerTerrainCollision = 16;

	// Token: 0x04005E33 RID: 24115
	public const int cLayerPhysicsDead = 17;

	// Token: 0x04005E34 RID: 24116
	public const int cLayerGrass = 18;

	// Token: 0x04005E35 RID: 24117
	public const int cLayerLargeEntityBlocker = 19;

	// Token: 0x04005E36 RID: 24118
	public const int cLayerLocalCCPhysics = 20;

	// Token: 0x04005E37 RID: 24119
	public const int cLayerPhysics = 21;

	// Token: 0x04005E38 RID: 24120
	public const int cLayerUnderwaterEffects = 22;

	// Token: 0x04005E39 RID: 24121
	public const int cLayerTrees = 23;

	// Token: 0x04005E3A RID: 24122
	public const int cLayerLocalPlayer = 24;

	// Token: 0x04005E3B RID: 24123
	public const int cLayerPlayerRagdollsOLD = 27;

	// Token: 0x04005E3C RID: 24124
	public const int cLayerTerrain = 28;

	// Token: 0x04005E3D RID: 24125
	public const int cLayerWires = 29;

	// Token: 0x04005E3E RID: 24126
	public const int cLayerGlass = 30;

	// Token: 0x04005E3F RID: 24127
	public const int cLayerVolumes = 31;

	// Token: 0x04005E40 RID: 24128
	public const int cLayerMaskItems = 8192;

	// Token: 0x04005E41 RID: 24129
	public const int cLayerMaskIgnoreRayCast = 538480644;

	// Token: 0x04005E42 RID: 24130
	public const int cLayerMaskGrass = 262144;

	// Token: 0x04005E43 RID: 24131
	public const int cLayerMaskWater = 16;

	// Token: 0x04005E44 RID: 24132
	public const int cLayerMaskLocalPlayer = 17825792;

	// Token: 0x04005E45 RID: 24133
	public const int cLayerMaskAllLayers = -538480645;

	// Token: 0x04005E46 RID: 24134
	public const int cLayerMaskNoItems = -538488837;

	// Token: 0x04005E47 RID: 24135
	public const int cLayerMaskOnlyItemsAndCollision = 73728;

	// Token: 0x04005E48 RID: 24136
	public const int cLayerMaskNoItemsNoGrass = -538750981;

	// Token: 0x04005E49 RID: 24137
	public const int cLayerMaskNoItemsNoGrassNoWater = -538750997;

	// Token: 0x04005E4A RID: 24138
	public const int cLayerMaskNoItemsNoLocalPlayer = -555266053;

	// Token: 0x04005E4B RID: 24139
	public const int cLayerMaskAttackingBlocksMask = 1073807360;

	// Token: 0x04005E4C RID: 24140
	public const int cLayerMaskSight = -1612492821;

	// Token: 0x04005E4D RID: 24141
	public static int cDistanceRandomDisplayUpdates = 30;

	// Token: 0x04005E4E RID: 24142
	public static float cRunningFOVMultiplier = 1.05f;

	// Token: 0x04005E4F RID: 24143
	public static float cRunningFOVSpeedDown = 3f;

	// Token: 0x04005E50 RID: 24144
	public static float cRunningFOVSpeedUp = 1f;

	// Token: 0x04005E51 RID: 24145
	public static int cDefaultCameraFieldOfView = 65;

	// Token: 0x04005E52 RID: 24146
	public static int cMinCameraFieldOfView = 50;

	// Token: 0x04005E53 RID: 24147
	public static int cMaxCameraFieldOfView = 85;

	// Token: 0x04005E54 RID: 24148
	public static readonly Vector3 cDefaultCameraPlayerOffset = new Vector3(0f, 1.6f, 0f);

	// Token: 0x04005E55 RID: 24149
	public const int cMaxVertices = 786432;

	// Token: 0x04005E56 RID: 24150
	public static float cMinGlobalBackgroundOpacity = 0.55f;

	// Token: 0x04005E57 RID: 24151
	public static float cMinGlobalForegroundOpacity = 0.75f;

	// Token: 0x04005E58 RID: 24152
	public const int cTicksPerSecond = 20;

	// Token: 0x04005E59 RID: 24153
	public const float cTickDuration = 0.05f;

	// Token: 0x04005E5A RID: 24154
	public const float cPhysicsTicksPerSecond = 50f;

	// Token: 0x04005E5B RID: 24155
	public const float cPhysicsTickDuration = 0.02f;

	// Token: 0x04005E5C RID: 24156
	public static float cDefaultDistortionFactor = 1f;

	// Token: 0x04005E5D RID: 24157
	public static int cMaxEntitiesPerMobSpawner = 8;

	// Token: 0x04005E5E RID: 24158
	public const byte cMaxLightValue = 15;

	// Token: 0x04005E5F RID: 24159
	public static float cSizePlanesAround = 250f;

	// Token: 0x04005E60 RID: 24160
	public static int cDefaultPort = 26900;

	// Token: 0x04005E61 RID: 24161
	public static int cLevelServerPort = 6789;

	// Token: 0x04005E62 RID: 24162
	public static int cRandomSpawnPointsToPlace = 4;

	// Token: 0x04005E63 RID: 24163
	public static int cStartTeamTickets = 0;

	// Token: 0x04005E64 RID: 24164
	public static int cTeamTicketsAlarm = 10;

	// Token: 0x04005E65 RID: 24165
	public static float cDecreaseOneTicketTime = 5f;

	// Token: 0x04005E66 RID: 24166
	public static float cTimeGameOverButWaitSeconds = 5f;

	// Token: 0x04005E67 RID: 24167
	public static float cDigAndBuildDistance = 4f;

	// Token: 0x04005E68 RID: 24168
	public static float cCollectItemDistance = 2f;

	// Token: 0x04005E69 RID: 24169
	public static float cRespawnAfterDeathTime = 3f;

	// Token: 0x04005E6A RID: 24170
	public static float cRespawnEnterGameTime = 0f;

	// Token: 0x04005E6B RID: 24171
	public static float cRespawnAfterFallenDown = 3f;

	// Token: 0x04005E6C RID: 24172
	public static float cHitColorDuration = 0.15f;

	// Token: 0x04005E6D RID: 24173
	public static float cItemDroppedOnDeathLifetime = 300f;

	// Token: 0x04005E6E RID: 24174
	public const float cItemDroppedLifetime = 60f;

	// Token: 0x04005E6F RID: 24175
	public static float cItemExplosionLifetime = 30f;

	// Token: 0x04005E70 RID: 24176
	public static float cItemHealthDroppedLifetime = 60f;

	// Token: 0x04005E71 RID: 24177
	public static float cItemSpawnPointLifetime = float.MaxValue;

	// Token: 0x04005E72 RID: 24178
	public static float cItemPortalLifetime = float.MaxValue;

	// Token: 0x04005E73 RID: 24179
	public static float cItemItemSpawnerLifetime = float.MaxValue;

	// Token: 0x04005E74 RID: 24180
	public static int cHardenBrickTime = 20;

	// Token: 0x04005E75 RID: 24181
	public const int cItemQualityTierVariations = 1;

	// Token: 0x04005E76 RID: 24182
	public const ushort cItemMaxQuality = 6;

	// Token: 0x04005E77 RID: 24183
	public static float cMinHolsterTime = 0.1f;

	// Token: 0x04005E78 RID: 24184
	public static float cMinUnHolsterTime = 0.1f;

	// Token: 0x04005E79 RID: 24185
	public static float cEnergyJetpackPerBlock = 10f;

	// Token: 0x04005E7A RID: 24186
	public static int cHealthPotionAdd = 30;

	// Token: 0x04005E7B RID: 24187
	public static int cMaxPlayerFood = 100;

	// Token: 0x04005E7C RID: 24188
	public static int cFoodOversaturate = 100;

	// Token: 0x04005E7D RID: 24189
	public static int cMaxPlayerDrink = 100;

	// Token: 0x04005E7E RID: 24190
	public static int cDrinkOversaturate = 100;

	// Token: 0x04005E7F RID: 24191
	public static int cItemDropCountWhenDead = 3;

	// Token: 0x04005E80 RID: 24192
	public static float cBuildIntervall = 0.5f;

	// Token: 0x04005E81 RID: 24193
	public static float cSendWorldTickTimeToClients = 1.5f;

	// Token: 0x04005E82 RID: 24194
	public static float cCheckGameState = 0.5f;

	// Token: 0x04005E83 RID: 24195
	public static float cSneakDamageMultiplier = 2f;

	// Token: 0x04005E84 RID: 24196
	public static int cNumberOfTeams = 2;

	// Token: 0x04005E85 RID: 24197
	public static Color[] cTeamColors = new Color[]
	{
		Color.white,
		new Color(0f, 0.8f, 1f),
		Color.red
	};

	// Token: 0x04005E86 RID: 24198
	public static string[] cTeamName = new string[]
	{
		"No",
		"BLUE",
		"RED"
	};

	// Token: 0x04005E87 RID: 24199
	public static string[] cTeamSkinName = new string[]
	{
		"Soldier Blue",
		"Soldier Blue",
		"Soldier Red"
	};

	// Token: 0x04005E88 RID: 24200
	public static float cDarkAtNightSubtraction = 12f;

	// Token: 0x04005E89 RID: 24201
	public static float cDefaultMonsterSeeDistance = 48f;

	// Token: 0x04005E8A RID: 24202
	public static float cPlayerSpeedModifierRunning = 1.6f;

	// Token: 0x04005E8B RID: 24203
	public static float cPlayerSpeedModifierWalking = 0.8f;

	// Token: 0x04005E8C RID: 24204
	public static float cPlayerSpeedModifierCrouching = 0.4f;

	// Token: 0x04005E8D RID: 24205
	public static int cPosInventoryYSub = 70;

	// Token: 0x04005E8E RID: 24206
	public static int cPosMinimapY = 10;

	// Token: 0x04005E8F RID: 24207
	public static int cPosMinimapSubRight = 20;

	// Token: 0x04005E90 RID: 24208
	public static Color cColorBlood = new Color(0.8f, 0f, 0.08f);

	// Token: 0x04005E91 RID: 24209
	public static Color cColorBorderBox = new Color(0.8f, 0f, 0f, 0.5f);

	// Token: 0x04005E92 RID: 24210
	public static Vector3 cStartPositionPlayerInLevel = new Vector3(0f, 200f, 0f);

	// Token: 0x04005E93 RID: 24211
	public static Vector3 cStartRotationPlayerInLevel = new Vector3(0f, 0f, 0f);

	// Token: 0x04005E94 RID: 24212
	public static BlockValue cTerrainBlockValue = new BlockValue(1U);

	// Token: 0x04005E95 RID: 24213
	public static string cTerrainFillerBlockName = "terrainFiller";

	// Token: 0x04005E96 RID: 24214
	public static string cTerrainFiller2BlockName = "terrainFillerAdaptive";

	// Token: 0x04005E97 RID: 24215
	public static string cPOIFillerBlock = "poiFillerBlock";

	// Token: 0x04005E98 RID: 24216
	public static string cQuestLootFetchContainerIndexName = "FetchContainer";

	// Token: 0x04005E99 RID: 24217
	public static string cQuestRestorePowerIndexName = "QuestRestorePower";

	// Token: 0x04005E9A RID: 24218
	public static Color[] TrackedFriendColors = new Color[]
	{
		Color.green,
		Color.blue,
		Color.yellow,
		new Color(1f, 0f, 1f),
		new Color(0.5f, 0.25f, 0f),
		new Color(1f, 0.5f, 0f),
		new Color32(56, 35, 16, byte.MaxValue),
		new Color32(42, 59, 0, byte.MaxValue)
	};

	// Token: 0x04005E9B RID: 24219
	public const int cMaxViewDistanceOptions = 7;

	// Token: 0x04005E9C RID: 24220
	public const int cMinViewDistance = 4;

	// Token: 0x04005E9D RID: 24221
	public const int cMaxViewDistance = 12;

	// Token: 0x04005E9E RID: 24222
	public const float cBlockDamageLosesPaint = 1f;

	// Token: 0x04005E9F RID: 24223
	public static int cEnemySenseMemory = 60;

	// Token: 0x04005EA0 RID: 24224
	public const int ChunkCompressionLevel = 3;

	// Token: 0x04005EA1 RID: 24225
	public const int NetworkCompressionLevel = 3;

	// Token: 0x04005EA2 RID: 24226
	public const string cSpecialWorldName_Empty = "Empty";

	// Token: 0x04005EA3 RID: 24227
	public const string cSpecialWorldName_Playtesting = "Playtesting";

	// Token: 0x04005EA4 RID: 24228
	public const string cSpecialWorldName_Navezgane = "Navezgane";

	// Token: 0x04005EA5 RID: 24229
	public const float cMouseSensitivityMin = 0.05f;

	// Token: 0x04005EA6 RID: 24230
	public const float cMouseSensitivityRange = 1.45f;

	// Token: 0x04005EA7 RID: 24231
	public const float cMouseSensitivityMax = 1.5f;

	// Token: 0x04005EA8 RID: 24232
	public const float cControllerSensitivityMin = 0.05f;

	// Token: 0x04005EA9 RID: 24233
	public const float cControllerSensitivityMax = 1f;

	// Token: 0x04005EAA RID: 24234
	public const float cControllerModifierSensitivityMax = 2f;

	// Token: 0x04005EAB RID: 24235
	public const int cPlayTestingSpawnOffset = 10;

	// Token: 0x04005EAC RID: 24236
	public const float cAimAssistMaxDistance = 50f;

	// Token: 0x04005EAD RID: 24237
	public const float cAimAssistSlowDownEntity = 0.5f;

	// Token: 0x04005EAE RID: 24238
	public const float cAimAssistSlowDownItem = 0.6f;

	// Token: 0x04005EAF RID: 24239
	public const float cAimAssistSlowDownItemDistance = 10f;

	// Token: 0x04005EB0 RID: 24240
	public const float cAimAssistSlowThreatLevelThreshold = 0.75f;

	// Token: 0x04005EB1 RID: 24241
	public const float cAimAssistSnapScreenDistance = 0.15f;

	// Token: 0x04005EB2 RID: 24242
	public const float cCameraSnapTime = 0.3f;

	// Token: 0x04005EB3 RID: 24243
	public const float cAimAssistSnapMaximumAngle = 15f;

	// Token: 0x04005EB4 RID: 24244
	public const float cAimAssistZoomSnapSpeed = 1f;

	// Token: 0x04005EB5 RID: 24245
	public const float cAimAssistMeleeSnapAngle = 20f;

	// Token: 0x04005EB6 RID: 24246
	public const float cAimAssistMeleeSnapSpeed = 1.5f;

	// Token: 0x04005EB7 RID: 24247
	public const float cAimAssistMeleeHitSnapAngle = 30f;

	// Token: 0x04005EB8 RID: 24248
	public const float cRapidTriggerFireDelay = 0.25f;

	// Token: 0x04005EB9 RID: 24249
	public const float cMapViewControllerSpeed = 500f;

	// Token: 0x04005EBA RID: 24250
	public const float cRunToggleHoldTime = 0.2f;

	// Token: 0x04005EBB RID: 24251
	public const float cRecoveryPositionAttemptTime = 30f;

	// Token: 0x04005EBC RID: 24252
	public const float cRecoveryPositionMinSqrMagnitude = 10000f;

	// Token: 0x04005EBD RID: 24253
	public const int cMaxRecoveryPositions = 5;

	// Token: 0x04005EBE RID: 24254
	public const float cCursorSensitivityMin = 0.1f;

	// Token: 0x04005EBF RID: 24255
	public const float cCursorSensitivityMax = 1f;

	// Token: 0x04005EC0 RID: 24256
	public const int cNavWorldSizeX = 6144;

	// Token: 0x04005EC1 RID: 24257
	public const int cNavWorldSizeZ = 6144;

	// Token: 0x04005EC2 RID: 24258
	public const float cPartyActivationRange = 15f;

	// Token: 0x04005EC3 RID: 24259
	public const int cMaxPartySize = 8;

	// Token: 0x04005EC4 RID: 24260
	public const int cDummyWaterTexId = 5000;

	// Token: 0x04005EC5 RID: 24261
	public static int cMaxLoadTimePixelsPerTest = 4096;

	// Token: 0x04005EC6 RID: 24262
	public static int cMaxLoadTimePerFrameMillis = 50;

	// Token: 0x04005EC7 RID: 24263
	public const float cInputRepeatDelay = 0.1f;

	// Token: 0x04005EC8 RID: 24264
	public const float cInputInitialRepeatDelay = 0.35f;

	// Token: 0x04005EC9 RID: 24265
	public const int cConsoleMaxPersistentPlayerDataEntries = 100;

	// Token: 0x04005ECA RID: 24266
	public const int cVirtualKeyboardDefaultCharacterLimit = 200;

	// Token: 0x02000F34 RID: 3892
	public enum EBiomePoiMap : byte
	{
		// Token: 0x04005ECC RID: 24268
		CityAsphalt = 1,
		// Token: 0x04005ECD RID: 24269
		CountryRoadAsphalt,
		// Token: 0x04005ECE RID: 24270
		RoadGravel,
		// Token: 0x04005ECF RID: 24271
		Sand,
		// Token: 0x04005ED0 RID: 24272
		Free
	}
}
