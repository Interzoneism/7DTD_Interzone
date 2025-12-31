using System;
using System.Collections.Generic;
using System.Diagnostics;

// Token: 0x02000B69 RID: 2921
public class NetPackageMeasure
{
	// Token: 0x06005ACE RID: 23246 RVA: 0x00246F05 File Offset: 0x00245105
	public NetPackageMeasure(double _windowSizeSeconds)
	{
		this.timeWindowTicks = (long)(_windowSizeSeconds * (double)Stopwatch.Frequency);
		this.timer.Start();
	}

	// Token: 0x06005ACF RID: 23247 RVA: 0x00246F40 File Offset: 0x00245140
	public void SamplePackages(List<NetPackage> _packages)
	{
		long num = 0L;
		foreach (NetPackage netPackage in _packages)
		{
			num += (long)netPackage.GetLength();
		}
		this.AddSample(num);
	}

	// Token: 0x06005AD0 RID: 23248 RVA: 0x00246F9C File Offset: 0x0024519C
	public void AddSample(long _totalBytes)
	{
		this.samples.AddLast(new NetPackageMeasure.Sample(this.timer.ElapsedTicks, _totalBytes));
		this.totalSent += _totalBytes;
	}

	// Token: 0x06005AD1 RID: 23249 RVA: 0x00246FCC File Offset: 0x002451CC
	public void RecalculateTotals()
	{
		this.timer.Stop();
		while (this.samples.First != null && Math.Abs(this.timer.ElapsedTicks - this.samples.First.Value.timestamp) > this.timeWindowTicks)
		{
			NetPackageMeasure.Sample value = this.samples.First.Value;
			this.totalSent -= value.totalBytesSent;
			this.samples.RemoveFirst();
		}
		this.timer.Start();
	}

	// Token: 0x0400456E RID: 17774
	[PublicizedFrom(EAccessModifier.Private)]
	public long timeWindowTicks;

	// Token: 0x0400456F RID: 17775
	[PublicizedFrom(EAccessModifier.Private)]
	public LinkedList<NetPackageMeasure.Sample> samples = new LinkedList<NetPackageMeasure.Sample>();

	// Token: 0x04004570 RID: 17776
	[PublicizedFrom(EAccessModifier.Private)]
	public Stopwatch timer = new Stopwatch();

	// Token: 0x04004571 RID: 17777
	public long totalSent;

	// Token: 0x02000B6A RID: 2922
	public struct Sample
	{
		// Token: 0x06005AD2 RID: 23250 RVA: 0x0024705B File Offset: 0x0024525B
		public Sample(long _timestamp, long _totalBytesSent)
		{
			this.timestamp = _timestamp;
			this.totalBytesSent = _totalBytesSent;
		}

		// Token: 0x04004572 RID: 17778
		public long totalBytesSent;

		// Token: 0x04004573 RID: 17779
		public long timestamp;
	}
}
