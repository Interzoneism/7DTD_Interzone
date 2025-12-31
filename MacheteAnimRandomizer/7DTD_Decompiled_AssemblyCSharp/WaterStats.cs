using System;
using Unity.Collections;

// Token: 0x02000B7B RID: 2939
public struct WaterStats
{
	// Token: 0x06005B24 RID: 23332 RVA: 0x002485A4 File Offset: 0x002467A4
	public static WaterStats Sum(NativeArray<WaterStats> array)
	{
		WaterStats waterStats = default(WaterStats);
		for (int i = 0; i < array.Length; i++)
		{
			waterStats += array[i];
		}
		return waterStats;
	}

	// Token: 0x06005B25 RID: 23333 RVA: 0x002485DC File Offset: 0x002467DC
	public static WaterStats operator +(WaterStats a, WaterStats b)
	{
		return new WaterStats
		{
			NumChunksProcessed = a.NumChunksProcessed + b.NumChunksProcessed,
			NumChunksActive = a.NumChunksActive + b.NumChunksActive,
			NumFlowEvents = a.NumFlowEvents + b.NumFlowEvents,
			NumVoxelsProcessed = a.NumVoxelsProcessed + b.NumVoxelsProcessed,
			NumVoxelsPutToSleep = a.NumVoxelsPutToSleep + b.NumVoxelsPutToSleep,
			NumVoxelsWokeUp = a.NumVoxelsWokeUp + b.NumVoxelsWokeUp
		};
	}

	// Token: 0x06005B26 RID: 23334 RVA: 0x0024866A File Offset: 0x0024686A
	public void ResetFrame()
	{
		this.NumChunksProcessed = 0;
		this.NumChunksActive = 0;
		this.NumFlowEvents = 0;
		this.NumVoxelsProcessed = 0;
		this.NumVoxelsPutToSleep = 0;
		this.NumVoxelsWokeUp = 0;
	}

	// Token: 0x040045B2 RID: 17842
	public int NumChunksProcessed;

	// Token: 0x040045B3 RID: 17843
	public int NumChunksActive;

	// Token: 0x040045B4 RID: 17844
	public int NumFlowEvents;

	// Token: 0x040045B5 RID: 17845
	public int NumVoxelsProcessed;

	// Token: 0x040045B6 RID: 17846
	public int NumVoxelsPutToSleep;

	// Token: 0x040045B7 RID: 17847
	public int NumVoxelsWokeUp;
}
