using System;
using UnityEngine.Scripting;

// Token: 0x020003BB RID: 955
[Preserve]
public class AIDirectorConstants
{
	// Token: 0x040013DA RID: 5082
	public static bool DebugOutput = true;

	// Token: 0x040013DB RID: 5083
	public const int kFileVersion = 10;

	// Token: 0x040013DC RID: 5084
	public const int kMaxSupplyCrates = 12;

	// Token: 0x040013DD RID: 5085
	public const float kStealthSightDistanceMultiplier = 0.8f;

	// Token: 0x040013DE RID: 5086
	public const float kStealthNighttimeSightDistanceMultiplier = 0.5f;

	// Token: 0x040013DF RID: 5087
	public const float kHordeMeterWarn1Threshold = 0.5f;

	// Token: 0x040013E0 RID: 5088
	public const float kHordeMeterWarn2Threshold = 0.8f;

	// Token: 0x040013E1 RID: 5089
	public const float kHordeMeterWarnResetThreshold = 0.2f;

	// Token: 0x040013E2 RID: 5090
	public const int kHordeDaySpawnRangeMin = 45;

	// Token: 0x040013E3 RID: 5091
	public const int kHordeDaySpawnRangeMax = 55;

	// Token: 0x040013E4 RID: 5092
	public const int kHordeNightSpawnRangeMin = 55;

	// Token: 0x040013E5 RID: 5093
	public const int kHordeNightSpawnRangeMax = 70;

	// Token: 0x040013E6 RID: 5094
	public const float kHordeMeterDecayDelay = 8f;

	// Token: 0x040013E7 RID: 5095
	public const float kHordeMeterDecayRate = 4f;

	// Token: 0x040013E8 RID: 5096
	public const int kWanderingHordeGlobalStartTime = 28000;

	// Token: 0x040013E9 RID: 5097
	public const int kSpawnWanderingHordeMin = 12000;

	// Token: 0x040013EA RID: 5098
	public const int kSpawnWanderingHordeMax = 24000;

	// Token: 0x040013EB RID: 5099
	public const int kWanderingHordeGroupSize = 6;

	// Token: 0x040013EC RID: 5100
	public const float kWanderingHordeSpawnDistance = 92f;

	// Token: 0x040013ED RID: 5101
	public const float kWanderingHordeSpawnMinDistance = 50f;

	// Token: 0x040013EE RID: 5102
	public const float kWanderingHordePlayerClusterSize = 30f;

	// Token: 0x040013EF RID: 5103
	public const int kSoundPriorityStart = 10;

	// Token: 0x040013F0 RID: 5104
	public const int kSoundPriorityRange = 100;

	// Token: 0x040013F1 RID: 5105
	public const int kScoutSpawnDistance = 80;

	// Token: 0x040013F2 RID: 5106
	public const float kScoutScreamGraceTime = 2f;

	// Token: 0x040013F3 RID: 5107
	public const float kScoutScreamAgainTime = 18f;

	// Token: 0x040013F4 RID: 5108
	public const float kScoutSpawnAnotherScoutChance = 0.12f;

	// Token: 0x040013F5 RID: 5109
	public const int kScoutSummonedPerScream = 5;

	// Token: 0x040013F6 RID: 5110
	public const int kScoutSummonedTotal = 25;
}
