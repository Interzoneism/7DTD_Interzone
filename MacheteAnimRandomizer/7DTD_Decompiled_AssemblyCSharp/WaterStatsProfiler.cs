using System;
using System.Diagnostics;
using Unity.Profiling;

// Token: 0x02000B7C RID: 2940
public static class WaterStatsProfiler
{
	// Token: 0x06005B27 RID: 23335 RVA: 0x00002914 File Offset: 0x00000B14
	public static void SampleTick(WaterStats stats)
	{
	}

	// Token: 0x040045B8 RID: 17848
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounter<int> NumChunksProcessed = new ProfilerCounter<int>(ProfilerCategory.Scripts, "Num Chunks Processed", ProfilerMarkerDataUnit.Count);

	// Token: 0x040045B9 RID: 17849
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounter<int> NumChunksActive = new ProfilerCounter<int>(ProfilerCategory.Scripts, "Num Chunks Active", ProfilerMarkerDataUnit.Count);

	// Token: 0x040045BA RID: 17850
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounter<int> NumFlowEvents = new ProfilerCounter<int>(ProfilerCategory.Scripts, "Num Flow Events", ProfilerMarkerDataUnit.Count);

	// Token: 0x040045BB RID: 17851
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounter<int> NumVoxelProcessed = new ProfilerCounter<int>(ProfilerCategory.Scripts, "Num Voxels Processed", ProfilerMarkerDataUnit.Count);

	// Token: 0x040045BC RID: 17852
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounter<int> NumVoxelsPutToSleep = new ProfilerCounter<int>(ProfilerCategory.Scripts, "Num Voxels Put To Sleep", ProfilerMarkerDataUnit.Count);

	// Token: 0x040045BD RID: 17853
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerCounter<int> NumVoxelsWokeUp = new ProfilerCounter<int>(ProfilerCategory.Scripts, "Num Voxels Woke Up", ProfilerMarkerDataUnit.Count);

	// Token: 0x02000B7D RID: 2941
	public struct Timer
	{
		// Token: 0x06005B29 RID: 23337 RVA: 0x00248723 File Offset: 0x00246923
		public Timer(string name)
		{
			this.stopwatch = new Stopwatch();
			this.counterValue = new ProfilerCounter<double>(ProfilerCategory.Scripts, name, ProfilerMarkerDataUnit.TimeNanoseconds);
		}

		// Token: 0x06005B2A RID: 23338 RVA: 0x00248742 File Offset: 0x00246942
		public void Start()
		{
			this.stopwatch.Start();
		}

		// Token: 0x06005B2B RID: 23339 RVA: 0x0024874F File Offset: 0x0024694F
		public void Stop()
		{
			this.stopwatch.Stop();
		}

		// Token: 0x06005B2C RID: 23340 RVA: 0x0024875C File Offset: 0x0024695C
		public void Sample()
		{
			this.stopwatch.Reset();
		}

		// Token: 0x040045BE RID: 17854
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly double tickToNanos = 1000000000.0 / (double)Stopwatch.Frequency;

		// Token: 0x040045BF RID: 17855
		[PublicizedFrom(EAccessModifier.Private)]
		public Stopwatch stopwatch;

		// Token: 0x040045C0 RID: 17856
		public ProfilerCounter<double> counterValue;
	}
}
