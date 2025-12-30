using System;

// Token: 0x02000679 RID: 1657
public enum MinEventTypes
{
	// Token: 0x04002815 RID: 10261
	onSelfBuffStart,
	// Token: 0x04002816 RID: 10262
	onSelfBuffUpdate,
	// Token: 0x04002817 RID: 10263
	onSelfBuffFinish,
	// Token: 0x04002818 RID: 10264
	onSelfBuffRemove,
	// Token: 0x04002819 RID: 10265
	onSelfBuffStack,
	// Token: 0x0400281A RID: 10266
	onSelfProgressionUpdate,
	// Token: 0x0400281B RID: 10267
	onSelfChallengeCompleteUpdate,
	// Token: 0x0400281C RID: 10268
	onOtherDamagedSelf,
	// Token: 0x0400281D RID: 10269
	onOtherAttackedSelf,
	// Token: 0x0400281E RID: 10270
	onOtherHealedSelf,
	// Token: 0x0400281F RID: 10271
	onSelfDamagedOther,
	// Token: 0x04002820 RID: 10272
	onSelfAttackedOther,
	// Token: 0x04002821 RID: 10273
	onSelfVehicleAttackedOther,
	// Token: 0x04002822 RID: 10274
	onSelfHealedOther,
	// Token: 0x04002823 RID: 10275
	onSelfExplosionAttackedOther,
	// Token: 0x04002824 RID: 10276
	onSelfExplosionDamagedOther,
	// Token: 0x04002825 RID: 10277
	onSelfExplosionMissEntity,
	// Token: 0x04002826 RID: 10278
	onSelfDamagedSelf,
	// Token: 0x04002827 RID: 10279
	onSelfHealedSelf,
	// Token: 0x04002828 RID: 10280
	onSelfKilledOther,
	// Token: 0x04002829 RID: 10281
	onOtherKilledSelf,
	// Token: 0x0400282A RID: 10282
	onBlockKilledSelf,
	// Token: 0x0400282B RID: 10283
	onSelfKilledSelf,
	// Token: 0x0400282C RID: 10284
	onSelfDied,
	// Token: 0x0400282D RID: 10285
	onSelfPrimaryActionStart,
	// Token: 0x0400282E RID: 10286
	onSelfPrimaryActionRayHit,
	// Token: 0x0400282F RID: 10287
	onSelfPrimaryActionRayMiss,
	// Token: 0x04002830 RID: 10288
	onSelfPrimaryActionGrazeHit,
	// Token: 0x04002831 RID: 10289
	onSelfPrimaryActionGrazeMiss,
	// Token: 0x04002832 RID: 10290
	onSelfPrimaryActionEnd,
	// Token: 0x04002833 RID: 10291
	onSelfPrimaryActionUpdate,
	// Token: 0x04002834 RID: 10292
	onSelfPrimaryActionMissEntity,
	// Token: 0x04002835 RID: 10293
	onSelfSecondaryActionStart,
	// Token: 0x04002836 RID: 10294
	onSelfSecondaryActionRayHit,
	// Token: 0x04002837 RID: 10295
	onSelfSecondaryActionRayMiss,
	// Token: 0x04002838 RID: 10296
	onSelfSecondaryActionGrazeHit,
	// Token: 0x04002839 RID: 10297
	onSelfSecondaryActionGrazeMiss,
	// Token: 0x0400283A RID: 10298
	onSelfSecondaryActionEnd,
	// Token: 0x0400283B RID: 10299
	onSelfSecondaryActionUpdate,
	// Token: 0x0400283C RID: 10300
	onSelfSecondaryActionMissEntity,
	// Token: 0x0400283D RID: 10301
	onSelfAction2Start,
	// Token: 0x0400283E RID: 10302
	onSelfAction2Update,
	// Token: 0x0400283F RID: 10303
	onSelfAction2End,
	// Token: 0x04002840 RID: 10304
	onSelfRepairBlock,
	// Token: 0x04002841 RID: 10305
	onSelfPlaceBlock,
	// Token: 0x04002842 RID: 10306
	onSelfUpgradedBlock,
	// Token: 0x04002843 RID: 10307
	onSelfDamagedBlock,
	// Token: 0x04002844 RID: 10308
	onSelfDestroyedBlock,
	// Token: 0x04002845 RID: 10309
	onSelfHarvestBlock,
	// Token: 0x04002846 RID: 10310
	onSelfHarvestOther,
	// Token: 0x04002847 RID: 10311
	onOtherEnteredRange,
	// Token: 0x04002848 RID: 10312
	onOtherLeftRange,
	// Token: 0x04002849 RID: 10313
	onSelfRangedBurstShotStart,
	// Token: 0x0400284A RID: 10314
	onSelfRangedBurstShotEnd,
	// Token: 0x0400284B RID: 10315
	onSelfEquipStart,
	// Token: 0x0400284C RID: 10316
	onSelfEquipChanged,
	// Token: 0x0400284D RID: 10317
	onSelfEquipUpdate,
	// Token: 0x0400284E RID: 10318
	onSelfEquipStop,
	// Token: 0x0400284F RID: 10319
	onReloadStart,
	// Token: 0x04002850 RID: 10320
	onReloadUpdate,
	// Token: 0x04002851 RID: 10321
	onReloadStop,
	// Token: 0x04002852 RID: 10322
	onSelfFirstSpawn,
	// Token: 0x04002853 RID: 10323
	onSelfRespawn,
	// Token: 0x04002854 RID: 10324
	onSelfLeaveGame,
	// Token: 0x04002855 RID: 10325
	onSelfEnteredGame,
	// Token: 0x04002856 RID: 10326
	onSelfTeleported,
	// Token: 0x04002857 RID: 10327
	onSelfJump,
	// Token: 0x04002858 RID: 10328
	onSelfLandJump,
	// Token: 0x04002859 RID: 10329
	onSelfRun,
	// Token: 0x0400285A RID: 10330
	onSelfWalk,
	// Token: 0x0400285B RID: 10331
	onSelfCrouch,
	// Token: 0x0400285C RID: 10332
	onSelfStand,
	// Token: 0x0400285D RID: 10333
	onSelfAimingGunStart,
	// Token: 0x0400285E RID: 10334
	onSelfAimingGunStop,
	// Token: 0x0400285F RID: 10335
	onSelfCrouchRun,
	// Token: 0x04002860 RID: 10336
	onSelfCrouchWalk,
	// Token: 0x04002861 RID: 10337
	onSelfSwimStart,
	// Token: 0x04002862 RID: 10338
	onSelfSwimStop,
	// Token: 0x04002863 RID: 10339
	onSelfSwimRun,
	// Token: 0x04002864 RID: 10340
	onSelfSwimIdle,
	// Token: 0x04002865 RID: 10341
	onSelfWaterSurface,
	// Token: 0x04002866 RID: 10342
	onSelfWaterSubmerge,
	// Token: 0x04002867 RID: 10343
	onSelfHoldingItemThrown,
	// Token: 0x04002868 RID: 10344
	onSelfHoldingItemCreated,
	// Token: 0x04002869 RID: 10345
	onSelfItemCrafted,
	// Token: 0x0400286A RID: 10346
	onSelfItemRepaired,
	// Token: 0x0400286B RID: 10347
	onSelfItemLooted,
	// Token: 0x0400286C RID: 10348
	onSelfItemLost,
	// Token: 0x0400286D RID: 10349
	onSelfItemGained,
	// Token: 0x0400286E RID: 10350
	onSelfItemSold,
	// Token: 0x0400286F RID: 10351
	onSelfItemBought,
	// Token: 0x04002870 RID: 10352
	onSelfItemActivate,
	// Token: 0x04002871 RID: 10353
	onSelfItemDeactivate,
	// Token: 0x04002872 RID: 10354
	onSelfChangedView,
	// Token: 0x04002873 RID: 10355
	onSelfFallImpact,
	// Token: 0x04002874 RID: 10356
	onProjectilePreImpact,
	// Token: 0x04002875 RID: 10357
	onProjectileImpact,
	// Token: 0x04002876 RID: 10358
	onPerkLevelChanged,
	// Token: 0x04002877 RID: 10359
	onSelfEnteredBiome,
	// Token: 0x04002878 RID: 10360
	onSelfLootContainer,
	// Token: 0x04002879 RID: 10361
	onSelfOpenLootContainer,
	// Token: 0x0400287A RID: 10362
	onSelfCloseLootContainer,
	// Token: 0x0400287B RID: 10363
	onSelfBiomeLootStageMaxEntered,
	// Token: 0x0400287C RID: 10364
	onSelfBiomeLootStageMaxExited,
	// Token: 0x0400287D RID: 10365
	onDismember,
	// Token: 0x0400287E RID: 10366
	onCombatEntered,
	// Token: 0x0400287F RID: 10367
	onTrackedEntityLost,
	// Token: 0x04002880 RID: 10368
	onTreasureRadiusEntered,
	// Token: 0x04002881 RID: 10369
	onTreasureRadiusExited,
	// Token: 0x04002882 RID: 10370
	onTreasureRadiusCompleted,
	// Token: 0x04002883 RID: 10371
	COUNT
}
