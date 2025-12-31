using System;

// Token: 0x02000F83 RID: 3971
public enum EnumGamePrefs
{
	// Token: 0x04006095 RID: 24725
	CreateLevelName,
	// Token: 0x04006096 RID: 24726
	CreateLevelDim,
	// Token: 0x04006097 RID: 24727
	OptionsAmbientVolumeLevel,
	// Token: 0x04006098 RID: 24728
	OptionsMusicVolumeLevel,
	// Token: 0x04006099 RID: 24729
	OptionsMenuMusicVolumeLevel,
	// Token: 0x0400609A RID: 24730
	OptionsOverallAudioVolumeLevel,
	// Token: 0x0400609B RID: 24731
	OptionsGfxAASharpness,
	// Token: 0x0400609C RID: 24732
	OptionsGfxWaterQuality,
	// Token: 0x0400609D RID: 24733
	OptionsGfxViewDistance,
	// Token: 0x0400609E RID: 24734
	OptionsGfxShadowDistance,
	// Token: 0x0400609F RID: 24735
	OptionsPlayerModel,
	// Token: 0x040060A0 RID: 24736
	OptionsPlayerModelTexture,
	// Token: 0x040060A1 RID: 24737
	OptionsGfxAA,
	// Token: 0x040060A2 RID: 24738
	OptionsLookSensitivity,
	// Token: 0x040060A3 RID: 24739
	OptionsZoomSensitivity,
	// Token: 0x040060A4 RID: 24740
	OptionsInvertMouse,
	// Token: 0x040060A5 RID: 24741
	OptionsGfxFOV,
	// Token: 0x040060A6 RID: 24742
	UNUSED_OptionsFieldOfViewNew,
	// Token: 0x040060A7 RID: 24743
	ServerPort,
	// Token: 0x040060A8 RID: 24744
	ServerIP,
	// Token: 0x040060A9 RID: 24745
	ServerPassword,
	// Token: 0x040060AA RID: 24746
	ServerName,
	// Token: 0x040060AB RID: 24747
	ServerDescription,
	// Token: 0x040060AC RID: 24748
	ServerWebsiteURL,
	// Token: 0x040060AD RID: 24749
	ServerPasswordCache,
	// Token: 0x040060AE RID: 24750
	ServerIsPublic,
	// Token: 0x040060AF RID: 24751
	ServerMaxPlayerCount,
	// Token: 0x040060B0 RID: 24752
	ServerAllowCrossplay,
	// Token: 0x040060B1 RID: 24753
	ServerEACPeerToPeer,
	// Token: 0x040060B2 RID: 24754
	GameMode,
	// Token: 0x040060B3 RID: 24755
	GameDifficulty,
	// Token: 0x040060B4 RID: 24756
	GameName,
	// Token: 0x040060B5 RID: 24757
	GameNameClient,
	// Token: 0x040060B6 RID: 24758
	GameWorld,
	// Token: 0x040060B7 RID: 24759
	GameVersion,
	// Token: 0x040060B8 RID: 24760
	ConnectToServerIP,
	// Token: 0x040060B9 RID: 24761
	ConnectToServerPort,
	// Token: 0x040060BA RID: 24762
	PlayerName,
	// Token: 0x040060BB RID: 24763
	UNUSED_PlayerId,
	// Token: 0x040060BC RID: 24764
	PlayerPassword,
	// Token: 0x040060BD RID: 24765
	PlayerAutologin,
	// Token: 0x040060BE RID: 24766
	PlayerToken,
	// Token: 0x040060BF RID: 24767
	PlayerSafeZoneHours,
	// Token: 0x040060C0 RID: 24768
	PlayerSafeZoneLevel,
	// Token: 0x040060C1 RID: 24769
	DebugMenuShowTasks,
	// Token: 0x040060C2 RID: 24770
	DebugMenuEnabled,
	// Token: 0x040060C3 RID: 24771
	DebugStopEnemiesMoving,
	// Token: 0x040060C4 RID: 24772
	CreativeMenuEnabled,
	// Token: 0x040060C5 RID: 24773
	FavoriteServersList,
	// Token: 0x040060C6 RID: 24774
	UNUSED_ControlPanelPort,
	// Token: 0x040060C7 RID: 24775
	UNUSED_ControlPanelPassword,
	// Token: 0x040060C8 RID: 24776
	DynamicSpawner,
	// Token: 0x040060C9 RID: 24777
	PlayerKillingMode,
	// Token: 0x040060CA RID: 24778
	MatchLength,
	// Token: 0x040060CB RID: 24779
	FragLimit,
	// Token: 0x040060CC RID: 24780
	RebuildMap,
	// Token: 0x040060CD RID: 24781
	JoiningOptions,
	// Token: 0x040060CE RID: 24782
	ZombiePlayers,
	// Token: 0x040060CF RID: 24783
	BuildCreate,
	// Token: 0x040060D0 RID: 24784
	DayCount,
	// Token: 0x040060D1 RID: 24785
	DayNightLength,
	// Token: 0x040060D2 RID: 24786
	DayLightLength,
	// Token: 0x040060D3 RID: 24787
	BloodMoonFrequency,
	// Token: 0x040060D4 RID: 24788
	BloodMoonRange,
	// Token: 0x040060D5 RID: 24789
	BloodMoonWarning,
	// Token: 0x040060D6 RID: 24790
	ShowFriendPlayerOnMap,
	// Token: 0x040060D7 RID: 24791
	AdminFileName,
	// Token: 0x040060D8 RID: 24792
	UNUSED_ControlPanelEnabled,
	// Token: 0x040060D9 RID: 24793
	TelnetEnabled,
	// Token: 0x040060DA RID: 24794
	TelnetPort,
	// Token: 0x040060DB RID: 24795
	ZombieFeralSense,
	// Token: 0x040060DC RID: 24796
	UNUSED_OptionsSSAO,
	// Token: 0x040060DD RID: 24797
	ZombieMove,
	// Token: 0x040060DE RID: 24798
	ZombieMoveNight,
	// Token: 0x040060DF RID: 24799
	ZombieFeralMove,
	// Token: 0x040060E0 RID: 24800
	ZombieBMMove,
	// Token: 0x040060E1 RID: 24801
	OptionsGfxLODDistance,
	// Token: 0x040060E2 RID: 24802
	DropOnDeath,
	// Token: 0x040060E3 RID: 24803
	DropOnQuit,
	// Token: 0x040060E4 RID: 24804
	DeathPenalty,
	// Token: 0x040060E5 RID: 24805
	LootTimer,
	// Token: 0x040060E6 RID: 24806
	BloodMoonEnemyCount,
	// Token: 0x040060E7 RID: 24807
	EnemySpawnMode,
	// Token: 0x040060E8 RID: 24808
	EnemyDifficulty,
	// Token: 0x040060E9 RID: 24809
	BlockDamagePlayer,
	// Token: 0x040060EA RID: 24810
	BlockDamageAI,
	// Token: 0x040060EB RID: 24811
	BlockDamageAIBM,
	// Token: 0x040060EC RID: 24812
	LootAbundance,
	// Token: 0x040060ED RID: 24813
	LootRespawnDays,
	// Token: 0x040060EE RID: 24814
	TelnetPassword,
	// Token: 0x040060EF RID: 24815
	LandClaimCount,
	// Token: 0x040060F0 RID: 24816
	LandClaimSize,
	// Token: 0x040060F1 RID: 24817
	LandClaimDeadZone,
	// Token: 0x040060F2 RID: 24818
	LandClaimExpiryTime,
	// Token: 0x040060F3 RID: 24819
	LandClaimDecayMode,
	// Token: 0x040060F4 RID: 24820
	LandClaimOnlineDurabilityModifier,
	// Token: 0x040060F5 RID: 24821
	LandClaimOfflineDurabilityModifier,
	// Token: 0x040060F6 RID: 24822
	LandClaimOfflineDelay,
	// Token: 0x040060F7 RID: 24823
	AirDropFrequency,
	// Token: 0x040060F8 RID: 24824
	MaxSpawnedZombies,
	// Token: 0x040060F9 RID: 24825
	PartySharedKillRange,
	// Token: 0x040060FA RID: 24826
	UNUSED_SaveGameFolder,
	// Token: 0x040060FB RID: 24827
	OptionsMicVolumeLevel,
	// Token: 0x040060FC RID: 24828
	OptionsVoiceVolumeLevel,
	// Token: 0x040060FD RID: 24829
	OptionsVoiceChatEnabled,
	// Token: 0x040060FE RID: 24830
	OptionsGfxTexQuality,
	// Token: 0x040060FF RID: 24831
	AutopilotMode,
	// Token: 0x04006100 RID: 24832
	SelectionOperationMode,
	// Token: 0x04006101 RID: 24833
	SelectionContextMode,
	// Token: 0x04006102 RID: 24834
	EACEnabled,
	// Token: 0x04006103 RID: 24835
	PersistentPlayerProfiles,
	// Token: 0x04006104 RID: 24836
	XPMultiplier,
	// Token: 0x04006105 RID: 24837
	OptionsAudioOcclusion,
	// Token: 0x04006106 RID: 24838
	LastGameResetRevision,
	// Token: 0x04006107 RID: 24839
	OptionsGfxResolution,
	// Token: 0x04006108 RID: 24840
	OptionsGfxVsync,
	// Token: 0x04006109 RID: 24841
	OptionsGfxReflectQuality,
	// Token: 0x0400610A RID: 24842
	OptionsGfxResetRevision,
	// Token: 0x0400610B RID: 24843
	UNUSED_OptionsReflectionCullList,
	// Token: 0x0400610C RID: 24844
	UNUSED_OptionsReflectionFarClip,
	// Token: 0x0400610D RID: 24845
	UNUSED_OptionsReflectionShadowDistance,
	// Token: 0x0400610E RID: 24846
	UNUSED_OptionsReflectionBounces,
	// Token: 0x0400610F RID: 24847
	UNUSED_OptionsReflectionTimeSlicingMode,
	// Token: 0x04006110 RID: 24848
	UNUSED_OptionsReflectionRefreshMode,
	// Token: 0x04006111 RID: 24849
	OptionsGfxObjQuality,
	// Token: 0x04006112 RID: 24850
	OptionsGfxGrassDistance,
	// Token: 0x04006113 RID: 24851
	UNUSED_OptionsMotionBlur,
	// Token: 0x04006114 RID: 24852
	UNUSED_OptionsObjectBlur,
	// Token: 0x04006115 RID: 24853
	MaxSpawnedAnimals,
	// Token: 0x04006116 RID: 24854
	UNUSED_OptionsBloom,
	// Token: 0x04006117 RID: 24855
	UNUSED_OptionsSunShafts,
	// Token: 0x04006118 RID: 24856
	UNUSED_OptionsDOF,
	// Token: 0x04006119 RID: 24857
	OptionsGfxReflectShadows,
	// Token: 0x0400611A RID: 24858
	OptionsAllowController,
	// Token: 0x0400611B RID: 24859
	OptionsGfxQualityPreset,
	// Token: 0x0400611C RID: 24860
	OptionsScreenBoundsValue,
	// Token: 0x0400611D RID: 24861
	OptionsInterfaceSensitivity,
	// Token: 0x0400611E RID: 24862
	OptionsControllerVibration,
	// Token: 0x0400611F RID: 24863
	NoGraphicsMode,
	// Token: 0x04006120 RID: 24864
	OptionsHudSize,
	// Token: 0x04006121 RID: 24865
	OptionsHudOpacity,
	// Token: 0x04006122 RID: 24866
	OptionsShowCrosshair,
	// Token: 0x04006123 RID: 24867
	OptionsShowCompass,
	// Token: 0x04006124 RID: 24868
	ServerDisabledNetworkProtocols,
	// Token: 0x04006125 RID: 24869
	OptionsBackgroundGlobalOpacity,
	// Token: 0x04006126 RID: 24870
	OptionsForegroundGlobalOpacity,
	// Token: 0x04006127 RID: 24871
	UNUSED_OptionsGamma,
	// Token: 0x04006128 RID: 24872
	OptionsStabSpawnBlocksOnGround,
	// Token: 0x04006129 RID: 24873
	OptionsTempCelsius,
	// Token: 0x0400612A RID: 24874
	AirDropMarker,
	// Token: 0x0400612B RID: 24875
	OptionsGfxWaterPtlLimiter,
	// Token: 0x0400612C RID: 24876
	UNUSED_OptionsGfxUMATexQuality,
	// Token: 0x0400612D RID: 24877
	HideCommandExecutionLog,
	// Token: 0x0400612E RID: 24878
	MaxUncoveredMapChunksPerPlayer,
	// Token: 0x0400612F RID: 24879
	ServerReservedSlots,
	// Token: 0x04006130 RID: 24880
	ServerReservedSlotsPermission,
	// Token: 0x04006131 RID: 24881
	ServerAdminSlots,
	// Token: 0x04006132 RID: 24882
	ServerAdminSlotsPermission,
	// Token: 0x04006133 RID: 24883
	GameGuidClient,
	// Token: 0x04006134 RID: 24884
	BedrollDeadZoneSize,
	// Token: 0x04006135 RID: 24885
	LastLoadedPrefab,
	// Token: 0x04006136 RID: 24886
	UNUSED_LastLoadedPrefabSize,
	// Token: 0x04006137 RID: 24887
	OptionsJournalPopup,
	// Token: 0x04006138 RID: 24888
	OptionsFilterProfanity,
	// Token: 0x04006139 RID: 24889
	TelnetFailedLoginLimit,
	// Token: 0x0400613A RID: 24890
	TelnetFailedLoginsBlocktime,
	// Token: 0x0400613B RID: 24891
	TerminalWindowEnabled,
	// Token: 0x0400613C RID: 24892
	ServerEnabled,
	// Token: 0x0400613D RID: 24893
	ServerVisibility,
	// Token: 0x0400613E RID: 24894
	ServerLoginConfirmationText,
	// Token: 0x0400613F RID: 24895
	WorldGenSeed,
	// Token: 0x04006140 RID: 24896
	WorldGenSize,
	// Token: 0x04006141 RID: 24897
	OptionsGfxTreeDistance,
	// Token: 0x04006142 RID: 24898
	OptionsPOICulling,
	// Token: 0x04006143 RID: 24899
	OptionsDynamicMusicEnabled,
	// Token: 0x04006144 RID: 24900
	OptionsDynamicMusicDailyTime,
	// Token: 0x04006145 RID: 24901
	OptionsPlayChanceFrequency,
	// Token: 0x04006146 RID: 24902
	OptionsPlayChanceProbability,
	// Token: 0x04006147 RID: 24903
	UNUSED_UserDataFolder,
	// Token: 0x04006148 RID: 24904
	OptionsGfxStreamMipmaps,
	// Token: 0x04006149 RID: 24905
	UNUSED_OptionsStreamingMipmapsBudget,
	// Token: 0x0400614A RID: 24906
	OptionsGfxBloom,
	// Token: 0x0400614B RID: 24907
	OptionsGfxDOF,
	// Token: 0x0400614C RID: 24908
	OptionsGfxMotionBlur,
	// Token: 0x0400614D RID: 24909
	OptionsGfxSSAO,
	// Token: 0x0400614E RID: 24910
	OptionsGfxSSReflections,
	// Token: 0x0400614F RID: 24911
	OptionsGfxSunShafts,
	// Token: 0x04006150 RID: 24912
	OptionsDisableChunkLODs,
	// Token: 0x04006151 RID: 24913
	ServerMaxWorldTransferSpeedKiBs,
	// Token: 0x04006152 RID: 24914
	ServerMaxAllowedViewDistance,
	// Token: 0x04006153 RID: 24915
	OptionsGfxOcclusion,
	// Token: 0x04006154 RID: 24916
	BedrollExpiryTime,
	// Token: 0x04006155 RID: 24917
	OptionsGfxTexFilter,
	// Token: 0x04006156 RID: 24918
	OptionsGfxTerrainQuality,
	// Token: 0x04006157 RID: 24919
	OptionsGfxBrightness,
	// Token: 0x04006158 RID: 24920
	LastLoadingTipRead,
	// Token: 0x04006159 RID: 24921
	OptionsGfxDynamicMode,
	// Token: 0x0400615A RID: 24922
	OptionsGfxDynamicMinFPS,
	// Token: 0x0400615B RID: 24923
	OptionsGfxDynamicScale,
	// Token: 0x0400615C RID: 24924
	OptionsUiFpsScaling,
	// Token: 0x0400615D RID: 24925
	OptionsControlsResetRevision,
	// Token: 0x0400615E RID: 24926
	OptionsWeaponAiming,
	// Token: 0x0400615F RID: 24927
	DynamicMeshEnabled,
	// Token: 0x04006160 RID: 24928
	DynamicMeshDistance,
	// Token: 0x04006161 RID: 24929
	DynamicMeshLandClaimOnly,
	// Token: 0x04006162 RID: 24930
	DynamicMeshLandClaimBuffer,
	// Token: 0x04006163 RID: 24931
	DynamicMeshUseImposters,
	// Token: 0x04006164 RID: 24932
	DynamicMeshMaxRegionCache,
	// Token: 0x04006165 RID: 24933
	DynamicMeshMaxItemCache,
	// Token: 0x04006166 RID: 24934
	TwitchBloodMoonAllowed,
	// Token: 0x04006167 RID: 24935
	TwitchServerPermission,
	// Token: 0x04006168 RID: 24936
	OptionsVehicleLookSensitivity,
	// Token: 0x04006169 RID: 24937
	OptionsSelectionBoxAlphaMultiplier,
	// Token: 0x0400616A RID: 24938
	PlaytestBiome,
	// Token: 0x0400616B RID: 24939
	Language,
	// Token: 0x0400616C RID: 24940
	LanguageBrowser,
	// Token: 0x0400616D RID: 24941
	Region,
	// Token: 0x0400616E RID: 24942
	ServerHistoryCache,
	// Token: 0x0400616F RID: 24943
	OptionsVoiceInputDevice,
	// Token: 0x04006170 RID: 24944
	OptionsVoiceOutputDevice,
	// Token: 0x04006171 RID: 24945
	MaxChunkAge,
	// Token: 0x04006172 RID: 24946
	SaveDataLimit,
	// Token: 0x04006173 RID: 24947
	OptionsSubtitlesEnabled,
	// Token: 0x04006174 RID: 24948
	OptionsIntroMovieEnabled,
	// Token: 0x04006175 RID: 24949
	AllowSpawnNearBackpack,
	// Token: 0x04006176 RID: 24950
	OptionsZoomAccel,
	// Token: 0x04006177 RID: 24951
	UNUSED_NewGameSetDefaults,
	// Token: 0x04006178 RID: 24952
	UNUSED_OptionsGfxGameplayResolutionWidth,
	// Token: 0x04006179 RID: 24953
	UNUSED_OptionsGfxGameplayResolutionHeight,
	// Token: 0x0400617A RID: 24954
	OptionsMumblePositionalAudioSupport,
	// Token: 0x0400617B RID: 24955
	OptionsLiteNetLibMtuOverride,
	// Token: 0x0400617C RID: 24956
	WebDashboardEnabled,
	// Token: 0x0400617D RID: 24957
	WebDashboardPort,
	// Token: 0x0400617E RID: 24958
	WebDashboardUrl,
	// Token: 0x0400617F RID: 24959
	EnableMapRendering,
	// Token: 0x04006180 RID: 24960
	OptionsAutoPartyWithFriends,
	// Token: 0x04006181 RID: 24961
	OptionsQuestsAutoShare,
	// Token: 0x04006182 RID: 24962
	OptionsQuestsAutoAccept,
	// Token: 0x04006183 RID: 24963
	OptionsControllerTriggerEffects,
	// Token: 0x04006184 RID: 24964
	MaxQueuedMeshLayers,
	// Token: 0x04006185 RID: 24965
	OptionsControllerSensitivityX,
	// Token: 0x04006186 RID: 24966
	OptionsControllerSensitivityY,
	// Token: 0x04006187 RID: 24967
	OptionsControllerLookInvert,
	// Token: 0x04006188 RID: 24968
	OptionsControllerJoystickLayout,
	// Token: 0x04006189 RID: 24969
	OptionsControllerLookAcceleration,
	// Token: 0x0400618A RID: 24970
	OptionsControllerZoomSensitivity,
	// Token: 0x0400618B RID: 24971
	OptionsControllerLookAxisDeadzone,
	// Token: 0x0400618C RID: 24972
	OptionsControllerMoveAxisDeadzone,
	// Token: 0x0400618D RID: 24973
	OptionsControllerCursorSnap,
	// Token: 0x0400618E RID: 24974
	OptionsControllerCursorHoverSensitivity,
	// Token: 0x0400618F RID: 24975
	OptionsControllerVehicleSensitivity,
	// Token: 0x04006190 RID: 24976
	OptionsControllerWeaponAiming,
	// Token: 0x04006191 RID: 24977
	OptionsControllerAimAssists,
	// Token: 0x04006192 RID: 24978
	OptionsChatCommunication,
	// Token: 0x04006193 RID: 24979
	OptionsControlsSprintLock,
	// Token: 0x04006194 RID: 24980
	OptionsDisableXmlEvents,
	// Token: 0x04006195 RID: 24981
	DebugPanelsEnabled,
	// Token: 0x04006196 RID: 24982
	OptionsControllerVibrationStrength,
	// Token: 0x04006197 RID: 24983
	EulaVersionAccepted,
	// Token: 0x04006198 RID: 24984
	EulaLatestVersion,
	// Token: 0x04006199 RID: 24985
	OptionsGfxMotionBlurEnabled,
	// Token: 0x0400619A RID: 24986
	IgnoreEOSSanctions,
	// Token: 0x0400619B RID: 24987
	SkipSpawnButton,
	// Token: 0x0400619C RID: 24988
	OptionsUiCompassUseEnglishCardinalDirections,
	// Token: 0x0400619D RID: 24989
	OptionsGfxShadowQuality,
	// Token: 0x0400619E RID: 24990
	QuestProgressionDailyLimit,
	// Token: 0x0400619F RID: 24991
	OptionsControllerIconStyle,
	// Token: 0x040061A0 RID: 24992
	OptionsShowConsoleButton,
	// Token: 0x040061A1 RID: 24993
	SaveDataLimitType,
	// Token: 0x040061A2 RID: 24994
	OptionsCrossplay,
	// Token: 0x040061A3 RID: 24995
	AllowSpawnNearFriend,
	// Token: 0x040061A4 RID: 24996
	BiomeProgression,
	// Token: 0x040061A5 RID: 24997
	ServerMatchmakingGroup,
	// Token: 0x040061A6 RID: 24998
	OptionsGfxUpscalerMode,
	// Token: 0x040061A7 RID: 24999
	OptionsGfxFSRPreset,
	// Token: 0x040061A8 RID: 25000
	StormFreq,
	// Token: 0x040061A9 RID: 25001
	Last
}
