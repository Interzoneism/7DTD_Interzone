using System;
using System.Text;
using Unity.Profiling;

// Token: 0x02001283 RID: 4739
public static class ProfilerPlatformCorrections
{
	// Token: 0x06009439 RID: 37945 RVA: 0x003B1B95 File Offset: 0x003AFD95
	public static IMetric Graphics(string header, string usedOrReserved)
	{
		return new ProfilerRecorderMetric
		{
			Header = header,
			recorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default)
		};
	}

	// Token: 0x0600943A RID: 37946 RVA: 0x003B1BC6 File Offset: 0x003AFDC6
	public static IMetric Native(string header, string usedOrReserved)
	{
		return new ProfilerPlatformCorrections.NativeDefault(header, usedOrReserved);
	}

	// Token: 0x0600943B RID: 37947 RVA: 0x003B1BCF File Offset: 0x003AFDCF
	public static IMetric TotalTracked(string header, string usedOrReserved)
	{
		return new ProfilerRecorderMetric
		{
			Header = header,
			recorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default)
		};
	}

	// Token: 0x02001284 RID: 4740
	public class NativeDefault : IMetric
	{
		// Token: 0x17000F34 RID: 3892
		// (get) Token: 0x0600943C RID: 37948 RVA: 0x003B1C00 File Offset: 0x003AFE00
		// (set) Token: 0x0600943D RID: 37949 RVA: 0x003B1C08 File Offset: 0x003AFE08
		public string Header { get; set; }

		// Token: 0x0600943E RID: 37950 RVA: 0x003B1C14 File Offset: 0x003AFE14
		public NativeDefault(string header, string usedOrReserved)
		{
			this.Header = header;
			this.graphics = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default);
			this.total = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default);
			this.managed = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default);
		}

		// Token: 0x0600943F RID: 37951 RVA: 0x003B1C98 File Offset: 0x003AFE98
		public void AppendLastValue(StringBuilder builder)
		{
			double num = this.total.LastValueAsDouble - this.graphics.LastValueAsDouble - this.managed.LastValueAsDouble;
			builder.AppendFormat("{0:F2}", num * 9.5367431640625E-07);
		}

		// Token: 0x06009440 RID: 37952 RVA: 0x003B1CE5 File Offset: 0x003AFEE5
		public void Cleanup()
		{
			this.graphics.Dispose();
			this.total.Dispose();
			this.managed.Dispose();
		}

		// Token: 0x04007155 RID: 29013
		[PublicizedFrom(EAccessModifier.Private)]
		public ProfilerRecorder graphics;

		// Token: 0x04007156 RID: 29014
		[PublicizedFrom(EAccessModifier.Private)]
		public ProfilerRecorder total;

		// Token: 0x04007157 RID: 29015
		[PublicizedFrom(EAccessModifier.Private)]
		public ProfilerRecorder managed;
	}

	// Token: 0x02001285 RID: 4741
	public class TotalTrackedPS5 : IMetric
	{
		// Token: 0x17000F35 RID: 3893
		// (get) Token: 0x06009441 RID: 37953 RVA: 0x003B1D08 File Offset: 0x003AFF08
		// (set) Token: 0x06009442 RID: 37954 RVA: 0x003B1D10 File Offset: 0x003AFF10
		public string Header { get; set; }

		// Token: 0x06009443 RID: 37955 RVA: 0x003B1D1C File Offset: 0x003AFF1C
		public TotalTrackedPS5(string header, string usedOrReserved)
		{
			this.Header = header;
			this.graphics = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default);
			this.total = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total " + usedOrReserved + " Memory", 1, ProfilerRecorderOptions.Default);
		}

		// Token: 0x06009444 RID: 37956 RVA: 0x003B1D7C File Offset: 0x003AFF7C
		public void AppendLastValue(StringBuilder builder)
		{
			double num = this.total.LastValueAsDouble - this.graphics.LastValueAsDouble;
			builder.AppendFormat("{0:F2}", num * 9.5367431640625E-07);
		}

		// Token: 0x06009445 RID: 37957 RVA: 0x003B1DBD File Offset: 0x003AFFBD
		public void Cleanup()
		{
			this.graphics.Dispose();
			this.total.Dispose();
		}

		// Token: 0x04007159 RID: 29017
		[PublicizedFrom(EAccessModifier.Private)]
		public ProfilerRecorder graphics;

		// Token: 0x0400715A RID: 29018
		[PublicizedFrom(EAccessModifier.Private)]
		public ProfilerRecorder total;
	}
}
