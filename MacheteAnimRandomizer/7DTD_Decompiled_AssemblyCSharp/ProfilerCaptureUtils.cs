using System;
using Unity.Profiling;

// Token: 0x02001272 RID: 4722
public static class ProfilerCaptureUtils
{
	// Token: 0x060093F0 RID: 37872 RVA: 0x003B106C File Offset: 0x003AF26C
	public static ProfilingMetricCapture CreateMemoryProfiler()
	{
		ProfilingMetricCapture profilingMetricCapture = new ProfilingMetricCapture();
		profilingMetricCapture.Add("Managed Heap Used", ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory", 1, ProfilerRecorderOptions.Default));
		profilingMetricCapture.Add("Managed Heap Reserved", ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory", 1, ProfilerRecorderOptions.Default));
		profilingMetricCapture.AddDummy("Graphics Used");
		profilingMetricCapture.AddDummy("Graphics Reserved");
		profilingMetricCapture.Add("Audio", ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Audio Used Memory", 1, ProfilerRecorderOptions.Default));
		profilingMetricCapture.Add(ProfilerPlatformCorrections.Native("Native Used", "Used"));
		profilingMetricCapture.Add(ProfilerPlatformCorrections.Native("Native Reserved", "Reserved"));
		profilingMetricCapture.Add(ProfilerPlatformCorrections.TotalTracked("Total Tracked Used", "Used"));
		profilingMetricCapture.Add(ProfilerPlatformCorrections.TotalTracked("Total Tracked Reserved", "Reserved"));
		profilingMetricCapture.Add("Profiler Used", ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Profiler Used Memory", 1, ProfilerRecorderOptions.Default));
		profilingMetricCapture.Add("Profiler Reserved", ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Profiler Reserved Memory", 1, ProfilerRecorderOptions.Default));
		profilingMetricCapture.AddDummy("Texture");
		profilingMetricCapture.AddDummy("Mesh");
		profilingMetricCapture.AddDummy("Material");
		profilingMetricCapture.AddDummy("AnimationClip");
		profilingMetricCapture.AddDummy("AudioClip");
		profilingMetricCapture.Add(MetricHelpers.TextureStreamingCurrent);
		profilingMetricCapture.Add(MetricHelpers.TextureStreamingTarget);
		profilingMetricCapture.Add(MetricHelpers.TextureStreamingDesired);
		profilingMetricCapture.Add(MetricHelpers.TextureStreamingNonStreamed);
		profilingMetricCapture.Add(MetricHelpers.TextureStreamingBudget);
		profilingMetricCapture.AddDummy("SaveDataManager");
		return profilingMetricCapture;
	}
}
