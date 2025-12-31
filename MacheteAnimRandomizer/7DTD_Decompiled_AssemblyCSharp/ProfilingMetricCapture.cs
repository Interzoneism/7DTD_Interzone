using System;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;

// Token: 0x02001282 RID: 4738
public class ProfilingMetricCapture
{
	// Token: 0x06009430 RID: 37936 RVA: 0x003B1930 File Offset: 0x003AFB30
	public void AddDummy(string header)
	{
		this.metrics.Add(new ConstantValueMetric
		{
			Header = header,
			value = 0
		});
	}

	// Token: 0x06009431 RID: 37937 RVA: 0x003B1950 File Offset: 0x003AFB50
	public void Add(string header, ProfilerRecorder recorder)
	{
		if (!recorder.IsRunning)
		{
			recorder.Start();
		}
		this.metrics.Add(new ProfilerRecorderMetric
		{
			recorder = recorder,
			Header = header
		});
	}

	// Token: 0x06009432 RID: 37938 RVA: 0x003B1980 File Offset: 0x003AFB80
	public void Add(string header, CallbackMetric.GetLastValue callback)
	{
		this.metrics.Add(new CallbackMetric
		{
			callback = callback,
			Header = header
		});
	}

	// Token: 0x06009433 RID: 37939 RVA: 0x003B19A0 File Offset: 0x003AFBA0
	public void Add(IMetric metric)
	{
		this.metrics.Add(metric);
	}

	// Token: 0x06009434 RID: 37940 RVA: 0x003B19B0 File Offset: 0x003AFBB0
	public void Cleanup()
	{
		foreach (IMetric metric in this.metrics)
		{
			metric.Cleanup();
		}
		this.metrics.Clear();
	}

	// Token: 0x06009435 RID: 37941 RVA: 0x003B1A0C File Offset: 0x003AFC0C
	public string GetCsvHeader()
	{
		for (int i = 0; i < this.metrics.Count; i++)
		{
			this.outputBuilder.Append(this.metrics[i].Header);
			if (i < this.metrics.Count - 1)
			{
				this.outputBuilder.Append(",");
			}
		}
		string result = this.outputBuilder.ToString();
		this.outputBuilder.Clear();
		return result;
	}

	// Token: 0x06009436 RID: 37942 RVA: 0x003B1A84 File Offset: 0x003AFC84
	public string GetLastValueCsv()
	{
		for (int i = 0; i < this.metrics.Count; i++)
		{
			this.metrics[i].AppendLastValue(this.outputBuilder);
			if (i < this.metrics.Count - 1)
			{
				this.outputBuilder.Append(",");
			}
		}
		string result = this.outputBuilder.ToString();
		this.outputBuilder.Clear();
		return result;
	}

	// Token: 0x06009437 RID: 37943 RVA: 0x003B1AF8 File Offset: 0x003AFCF8
	public string PrettyPrint()
	{
		for (int i = 0; i < this.metrics.Count; i++)
		{
			this.outputBuilder.AppendFormat("{0}: ", this.metrics[i].Header);
			this.metrics[i].AppendLastValue(this.outputBuilder);
			this.outputBuilder.AppendLine();
		}
		string result = this.outputBuilder.ToString();
		this.outputBuilder.Clear();
		return result;
	}

	// Token: 0x04007152 RID: 29010
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IMetric> metrics = new List<IMetric>();

	// Token: 0x04007153 RID: 29011
	[PublicizedFrom(EAccessModifier.Private)]
	public StringBuilder outputBuilder = new StringBuilder();
}
