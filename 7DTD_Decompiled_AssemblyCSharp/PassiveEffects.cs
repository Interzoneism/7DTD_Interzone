using System;

// Token: 0x020005BB RID: 1467
public enum PassiveEffects : byte
{
	// Token: 0x04002557 RID: 9559
	None,
	// Token: 0x04002558 RID: 9560
	EntityDamage,
	// Token: 0x04002559 RID: 9561
	EntityHeal,
	// Token: 0x0400255A RID: 9562
	BlockDamage,
	// Token: 0x0400255B RID: 9563
	BlockRepairAmount,
	// Token: 0x0400255C RID: 9564
	DamageModifier,
	// Token: 0x0400255D RID: 9565
	LandClaimDamageModifier,
	// Token: 0x0400255E RID: 9566
	DegradationPerUse,
	// Token: 0x0400255F RID: 9567
	DegradationMax,
	// Token: 0x04002560 RID: 9568
	MagazineSize,
	// Token: 0x04002561 RID: 9569
	DamageFalloffRange,
	// Token: 0x04002562 RID: 9570
	MaxRange,
	// Token: 0x04002563 RID: 9571
	BlockRange,
	// Token: 0x04002564 RID: 9572
	WeaponHandling,
	// Token: 0x04002565 RID: 9573
	IncrementalSpreadMultiplier,
	// Token: 0x04002566 RID: 9574
	BurstRoundCount,
	// Token: 0x04002567 RID: 9575
	RoundRayCount,
	// Token: 0x04002568 RID: 9576
	RoundsPerMinute,
	// Token: 0x04002569 RID: 9577
	AttacksPerMinute,
	// Token: 0x0400256A RID: 9578
	ExplosionBlockDamage,
	// Token: 0x0400256B RID: 9579
	ExplosionEntityDamage,
	// Token: 0x0400256C RID: 9580
	ExplosionRadius,
	// Token: 0x0400256D RID: 9581
	ExplosionIncomingDamage,
	// Token: 0x0400256E RID: 9582
	ModSlots,
	// Token: 0x0400256F RID: 9583
	ModPowerBonus,
	// Token: 0x04002570 RID: 9584
	SpreadMultiplierHip,
	// Token: 0x04002571 RID: 9585
	SpreadMultiplierAiming,
	// Token: 0x04002572 RID: 9586
	SpreadMultiplierRunning,
	// Token: 0x04002573 RID: 9587
	SpreadMultiplierWalking,
	// Token: 0x04002574 RID: 9588
	SpreadMultiplierCrouching,
	// Token: 0x04002575 RID: 9589
	SpreadMultiplierIdle,
	// Token: 0x04002576 RID: 9590
	SpreadDegreesVertical,
	// Token: 0x04002577 RID: 9591
	SpreadDegreesHorizontal,
	// Token: 0x04002578 RID: 9592
	KickDegreesVerticalMin,
	// Token: 0x04002579 RID: 9593
	KickDegreesHorizontalMin,
	// Token: 0x0400257A RID: 9594
	KickDegreesVerticalMax,
	// Token: 0x0400257B RID: 9595
	KickDegreesHorizontalMax,
	// Token: 0x0400257C RID: 9596
	SphereCastRadius,
	// Token: 0x0400257D RID: 9597
	GrazeDamageMultiplier,
	// Token: 0x0400257E RID: 9598
	GrazeStaminaMultiplier,
	// Token: 0x0400257F RID: 9599
	GeneralDamageResist,
	// Token: 0x04002580 RID: 9600
	PhysicalDamageResist,
	// Token: 0x04002581 RID: 9601
	PhysicalDamageResistMax,
	// Token: 0x04002582 RID: 9602
	ElementalDamageResist,
	// Token: 0x04002583 RID: 9603
	ElementalDamageResistMax,
	// Token: 0x04002584 RID: 9604
	HypothermalResist,
	// Token: 0x04002585 RID: 9605
	HyperthermalResist,
	// Token: 0x04002586 RID: 9606
	FallDamageReduction,
	// Token: 0x04002587 RID: 9607
	Tier,
	// Token: 0x04002588 RID: 9608
	LightIntensity,
	// Token: 0x04002589 RID: 9609
	VehicleFuelMaxPer,
	// Token: 0x0400258A RID: 9610
	VehicleFuelUsePer,
	// Token: 0x0400258B RID: 9611
	VehicleVelocityMaxPer,
	// Token: 0x0400258C RID: 9612
	VehicleMotorTorquePer,
	// Token: 0x0400258D RID: 9613
	VehicleCarryCapacity,
	// Token: 0x0400258E RID: 9614
	VehicleEntityDamage,
	// Token: 0x0400258F RID: 9615
	VehicleBlockDamage,
	// Token: 0x04002590 RID: 9616
	VehicleSelfDamage,
	// Token: 0x04002591 RID: 9617
	VehicleStrongSelfDamage,
	// Token: 0x04002592 RID: 9618
	VehicleSeats,
	// Token: 0x04002593 RID: 9619
	VehicleTankSize,
	// Token: 0x04002594 RID: 9620
	VehicleHopStrength,
	// Token: 0x04002595 RID: 9621
	VehiclePlayerStaminaDrainRate,
	// Token: 0x04002596 RID: 9622
	BatteryMaxLoadInVolts,
	// Token: 0x04002597 RID: 9623
	BatteryDischargeTimeInMinutes,
	// Token: 0x04002598 RID: 9624
	DistractionResistance,
	// Token: 0x04002599 RID: 9625
	DistractionRadius,
	// Token: 0x0400259A RID: 9626
	DistractionLifetime,
	// Token: 0x0400259B RID: 9627
	DistractionStrength,
	// Token: 0x0400259C RID: 9628
	DistractionEatTicks,
	// Token: 0x0400259D RID: 9629
	ProjectileGravity,
	// Token: 0x0400259E RID: 9630
	ProjectileVelocity,
	// Token: 0x0400259F RID: 9631
	ProjectileStickChance,
	// Token: 0x040025A0 RID: 9632
	RecipeTagUnlocked,
	// Token: 0x040025A1 RID: 9633
	JunkTurretActiveRange,
	// Token: 0x040025A2 RID: 9634
	JunkTurretActiveCount,
	// Token: 0x040025A3 RID: 9635
	EconomicValue,
	// Token: 0x040025A4 RID: 9636
	LockPickTime,
	// Token: 0x040025A5 RID: 9637
	LockPickBreakChance,
	// Token: 0x040025A6 RID: 9638
	LootProb,
	// Token: 0x040025A7 RID: 9639
	LootDropProb,
	// Token: 0x040025A8 RID: 9640
	LootQuantity,
	// Token: 0x040025A9 RID: 9641
	Tracking,
	// Token: 0x040025AA RID: 9642
	AttributeLevel,
	// Token: 0x040025AB RID: 9643
	SkillLevel,
	// Token: 0x040025AC RID: 9644
	PerkLevel,
	// Token: 0x040025AD RID: 9645
	SkillExpGain,
	// Token: 0x040025AE RID: 9646
	PlayerExpGain,
	// Token: 0x040025AF RID: 9647
	NoiseMultiplier,
	// Token: 0x040025B0 RID: 9648
	LightMultiplier,
	// Token: 0x040025B1 RID: 9649
	CraftingTime,
	// Token: 0x040025B2 RID: 9650
	CraftingTier,
	// Token: 0x040025B3 RID: 9651
	CraftingOutputCount,
	// Token: 0x040025B4 RID: 9652
	ActiveCraftingSlots,
	// Token: 0x040025B5 RID: 9653
	CraftingSlots,
	// Token: 0x040025B6 RID: 9654
	CraftingSmeltTime,
	// Token: 0x040025B7 RID: 9655
	ExpDeficitMaxPercentage,
	// Token: 0x040025B8 RID: 9656
	ExpDeficitPerDeathPercentage,
	// Token: 0x040025B9 RID: 9657
	CarryCapacity,
	// Token: 0x040025BA RID: 9658
	BagSize,
	// Token: 0x040025BB RID: 9659
	RepairTime,
	// Token: 0x040025BC RID: 9660
	RepairAmount,
	// Token: 0x040025BD RID: 9661
	ReloadSpeedMultiplier,
	// Token: 0x040025BE RID: 9662
	WaterRegenRate,
	// Token: 0x040025BF RID: 9663
	HealthMax,
	// Token: 0x040025C0 RID: 9664
	HealthChangeOT,
	// Token: 0x040025C1 RID: 9665
	HealthGain,
	// Token: 0x040025C2 RID: 9666
	HealthLoss,
	// Token: 0x040025C3 RID: 9667
	HealthMaxBlockage,
	// Token: 0x040025C4 RID: 9668
	StaminaMax,
	// Token: 0x040025C5 RID: 9669
	StaminaChangeOT,
	// Token: 0x040025C6 RID: 9670
	StaminaGain,
	// Token: 0x040025C7 RID: 9671
	StaminaLoss,
	// Token: 0x040025C8 RID: 9672
	StaminaMaxBlockage,
	// Token: 0x040025C9 RID: 9673
	FoodMax,
	// Token: 0x040025CA RID: 9674
	FoodChangeOT,
	// Token: 0x040025CB RID: 9675
	FoodGain,
	// Token: 0x040025CC RID: 9676
	FoodLoss,
	// Token: 0x040025CD RID: 9677
	FoodLossPerHealthPointLost,
	// Token: 0x040025CE RID: 9678
	FoodLossPerStaminaPointGained,
	// Token: 0x040025CF RID: 9679
	FoodLossPerHealthPointGained,
	// Token: 0x040025D0 RID: 9680
	FoodMaxBlockage,
	// Token: 0x040025D1 RID: 9681
	WaterMax,
	// Token: 0x040025D2 RID: 9682
	WaterChangeOT,
	// Token: 0x040025D3 RID: 9683
	WaterGain,
	// Token: 0x040025D4 RID: 9684
	WaterLoss,
	// Token: 0x040025D5 RID: 9685
	WaterLossPerHealthPointGained,
	// Token: 0x040025D6 RID: 9686
	WaterLossPerStaminaPointGained,
	// Token: 0x040025D7 RID: 9687
	WaterMaxBlockage,
	// Token: 0x040025D8 RID: 9688
	CoreTempChangeOT,
	// Token: 0x040025D9 RID: 9689
	CoreTempGain,
	// Token: 0x040025DA RID: 9690
	CoreTempLoss,
	// Token: 0x040025DB RID: 9691
	JumpStrength,
	// Token: 0x040025DC RID: 9692
	WalkSpeed,
	// Token: 0x040025DD RID: 9693
	RunSpeed,
	// Token: 0x040025DE RID: 9694
	CrouchSpeed,
	// Token: 0x040025DF RID: 9695
	Mobility,
	// Token: 0x040025E0 RID: 9696
	LandMineImmunity,
	// Token: 0x040025E1 RID: 9697
	ScavengingTime,
	// Token: 0x040025E2 RID: 9698
	ScavengingTier,
	// Token: 0x040025E3 RID: 9699
	ScavengingItemCount,
	// Token: 0x040025E4 RID: 9700
	HarvestCount,
	// Token: 0x040025E5 RID: 9701
	ScrappingTime,
	// Token: 0x040025E6 RID: 9702
	DismemberSelfChance,
	// Token: 0x040025E7 RID: 9703
	DismemberChance,
	// Token: 0x040025E8 RID: 9704
	TreasureRadius,
	// Token: 0x040025E9 RID: 9705
	TreasureBlocksPerReduction,
	// Token: 0x040025EA RID: 9706
	BreathHoldDuration,
	// Token: 0x040025EB RID: 9707
	BarteringBuying,
	// Token: 0x040025EC RID: 9708
	BarteringSelling,
	// Token: 0x040025ED RID: 9709
	SecretStash,
	// Token: 0x040025EE RID: 9710
	LootTier,
	// Token: 0x040025EF RID: 9711
	HeatGain,
	// Token: 0x040025F0 RID: 9712
	MovementFactorMultiplier,
	// Token: 0x040025F1 RID: 9713
	QuestBonusItemReward,
	// Token: 0x040025F2 RID: 9714
	QuestRewardOptionCount,
	// Token: 0x040025F3 RID: 9715
	QuestRewardChoiceCount,
	// Token: 0x040025F4 RID: 9716
	GameStage,
	// Token: 0x040025F5 RID: 9717
	TraderStage,
	// Token: 0x040025F6 RID: 9718
	LootStage,
	// Token: 0x040025F7 RID: 9719
	LootStageMax,
	// Token: 0x040025F8 RID: 9720
	DamageBonus,
	// Token: 0x040025F9 RID: 9721
	InternalDamageModifier,
	// Token: 0x040025FA RID: 9722
	TargetArmor,
	// Token: 0x040025FB RID: 9723
	FallingBlockDamage,
	// Token: 0x040025FC RID: 9724
	SilenceBlockSteps,
	// Token: 0x040025FD RID: 9725
	TrackDistance,
	// Token: 0x040025FE RID: 9726
	HealthSteal,
	// Token: 0x040025FF RID: 9727
	TurretWakeUp,
	// Token: 0x04002600 RID: 9728
	ElectricalTrapXP,
	// Token: 0x04002601 RID: 9729
	TrapDoorTriggerDelay,
	// Token: 0x04002602 RID: 9730
	LandMineTriggerDelay,
	// Token: 0x04002603 RID: 9731
	TrapIncomingDamage,
	// Token: 0x04002604 RID: 9732
	BlockPickup,
	// Token: 0x04002605 RID: 9733
	HeadShotOnly,
	// Token: 0x04002606 RID: 9734
	NegateDamageSelf,
	// Token: 0x04002607 RID: 9735
	NegateDamageOther,
	// Token: 0x04002608 RID: 9736
	DisableItem,
	// Token: 0x04002609 RID: 9737
	DisableLoot,
	// Token: 0x0400260A RID: 9738
	NoVehicle,
	// Token: 0x0400260B RID: 9739
	FlipControls,
	// Token: 0x0400260C RID: 9740
	CelebrationKill,
	// Token: 0x0400260D RID: 9741
	ShuffledBackpack,
	// Token: 0x0400260E RID: 9742
	EnemySearchDuration,
	// Token: 0x0400260F RID: 9743
	TwitchViewerPointRate,
	// Token: 0x04002610 RID: 9744
	TwitchSpawnMultiplier,
	// Token: 0x04002611 RID: 9745
	TwitchAddCooldown,
	// Token: 0x04002612 RID: 9746
	TwitchAddPimpPot,
	// Token: 0x04002613 RID: 9747
	InfiniteAmmo,
	// Token: 0x04002614 RID: 9748
	NoTimeDisplay,
	// Token: 0x04002615 RID: 9749
	DisableGameEventNotify,
	// Token: 0x04002616 RID: 9750
	NoTrader,
	// Token: 0x04002617 RID: 9751
	DisableMovement,
	// Token: 0x04002618 RID: 9752
	ExperienceGain,
	// Token: 0x04002619 RID: 9753
	CriticalChance,
	// Token: 0x0400261A RID: 9754
	BuffProcChance,
	// Token: 0x0400261B RID: 9755
	BuffBlink,
	// Token: 0x0400261C RID: 9756
	BuffResistance,
	// Token: 0x0400261D RID: 9757
	CraftingIngredientCount,
	// Token: 0x0400261E RID: 9758
	EntityPenetrationCount,
	// Token: 0x0400261F RID: 9759
	BlockPenetrationFactor,
	// Token: 0x04002620 RID: 9760
	Count
}
