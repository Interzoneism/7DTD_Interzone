using System;

// Token: 0x02000A2A RID: 2602
[Flags]
public enum ChunkProtectionLevel
{
	// Token: 0x04003CC0 RID: 15552
	None = 0,
	// Token: 0x04003CC1 RID: 15553
	NearOfflinePlayer = 1,
	// Token: 0x04003CC2 RID: 15554
	NearBedroll = 2,
	// Token: 0x04003CC3 RID: 15555
	NearSupplyCrate = 4,
	// Token: 0x04003CC4 RID: 15556
	NearQuestObjective = 8,
	// Token: 0x04003CC5 RID: 15557
	NearDroppedBackpack = 16,
	// Token: 0x04003CC6 RID: 15558
	NearVehicle = 32,
	// Token: 0x04003CC7 RID: 15559
	NearLandClaim = 64,
	// Token: 0x04003CC8 RID: 15560
	OfflinePlayer = 128,
	// Token: 0x04003CC9 RID: 15561
	Bedroll = 256,
	// Token: 0x04003CCA RID: 15562
	SupplyCrate = 512,
	// Token: 0x04003CCB RID: 15563
	QuestObjective = 1024,
	// Token: 0x04003CCC RID: 15564
	DroppedBackpack = 2048,
	// Token: 0x04003CCD RID: 15565
	Trader = 4096,
	// Token: 0x04003CCE RID: 15566
	Drone = 8192,
	// Token: 0x04003CCF RID: 15567
	Vehicle = 16384,
	// Token: 0x04003CD0 RID: 15568
	LandClaim = 32768,
	// Token: 0x04003CD1 RID: 15569
	CurrentlySynced = 65536,
	// Token: 0x04003CD2 RID: 15570
	All = -1
}
