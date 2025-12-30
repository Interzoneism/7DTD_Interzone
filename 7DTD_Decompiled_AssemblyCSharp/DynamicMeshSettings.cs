using System;

// Token: 0x0200035A RID: 858
public class DynamicMeshSettings
{
	// Token: 0x170002CE RID: 718
	// (get) Token: 0x0600191D RID: 6429 RVA: 0x0009A086 File Offset: 0x00098286
	// (set) Token: 0x0600191E RID: 6430 RVA: 0x0009A08D File Offset: 0x0009828D
	public static bool UseImposterValues { get; set; } = true;

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x0600191F RID: 6431 RVA: 0x0009A095 File Offset: 0x00098295
	// (set) Token: 0x06001920 RID: 6432 RVA: 0x0009A09C File Offset: 0x0009829C
	public static bool OnlyPlayerAreas { get; set; } = false;

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001921 RID: 6433 RVA: 0x0009A0A4 File Offset: 0x000982A4
	// (set) Token: 0x06001922 RID: 6434 RVA: 0x0009A0AB File Offset: 0x000982AB
	public static int PlayerAreaChunkBuffer { get; set; } = 3;

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001923 RID: 6435 RVA: 0x0009A0B3 File Offset: 0x000982B3
	// (set) Token: 0x06001924 RID: 6436 RVA: 0x0009A0BA File Offset: 0x000982BA
	public static int MaxViewDistance
	{
		get
		{
			return DynamicMeshSettings._maxViewDistance;
		}
		set
		{
			DynamicMeshSettings._maxViewDistance = Math.Min(3000, value);
			PrefabLODManager.lodPoiDistance = DynamicMeshSettings._maxViewDistance;
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001925 RID: 6437 RVA: 0x0009A0D6 File Offset: 0x000982D6
	// (set) Token: 0x06001926 RID: 6438 RVA: 0x0009A0DD File Offset: 0x000982DD
	public static bool NewWorldFullRegen { get; set; } = false;

	// Token: 0x06001927 RID: 6439 RVA: 0x0009A0E8 File Offset: 0x000982E8
	public static void LogSettings()
	{
		Log.Out("Dynamic Mesh Settings");
		Log.Out("Use Imposter Values: " + DynamicMeshSettings.UseImposterValues.ToString());
		Log.Out("Only Player Areas: " + DynamicMeshSettings.OnlyPlayerAreas.ToString());
		Log.Out("Player Area Buffer: " + DynamicMeshSettings.PlayerAreaChunkBuffer.ToString());
		Log.Out("Max View Distance: " + DynamicMeshSettings.MaxViewDistance.ToString());
		Log.Out("Regen all on new world: " + DynamicMeshSettings.NewWorldFullRegen.ToString());
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x00002914 File Offset: 0x00000B14
	public static void Validate()
	{
	}

	// Token: 0x04001014 RID: 4116
	public static int MaxRegionMeshData = 1;

	// Token: 0x04001015 RID: 4117
	public static int MaxRegionLoadMsPerFrame = 2;

	// Token: 0x04001016 RID: 4118
	public static int MaxDyMeshData = 3;

	// Token: 0x0400101A RID: 4122
	[PublicizedFrom(EAccessModifier.Private)]
	public static int _maxViewDistance = 1000;
}
