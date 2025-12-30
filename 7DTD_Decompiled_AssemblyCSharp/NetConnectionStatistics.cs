using System;
using UnityEngine;

// Token: 0x020006CB RID: 1739
public class NetConnectionStatistics
{
	// Token: 0x06003349 RID: 13129 RVA: 0x0015B848 File Offset: 0x00159A48
	public void RegisterReceivedPackage(int _packageType, int _length)
	{
		this.statsPackagePerTypeReceived[_packageType]++;
		this.statsBytesPerTypeReceived[_packageType] += _length;
		this.statsLastPackagesRec.Add(new SNetPackageInfo(_packageType, _length));
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x0015B87E File Offset: 0x00159A7E
	public void RegisterReceivedData(int _packageCount, int _netDataSize)
	{
		this.statsBytesReceived += _netDataSize;
		this.statsPackagesReceived += _packageCount;
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x0015B8A4 File Offset: 0x00159AA4
	public void RegisterSentData(int _packageCount, int _netDataSize)
	{
		this.statsBytesSent += _netDataSize;
		this.statsPackagesSent += _packageCount;
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x0015B8CA File Offset: 0x00159ACA
	public void RegisterSentPackage(int _packageType, int _length)
	{
		this.statsPackagePerTypeSent[_packageType]++;
		this.statsBytesPerTypeSent[_packageType] += _length;
		this.statsLastPackagesSent.Add(new SNetPackageInfo(_packageType, _length));
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x0015B900 File Offset: 0x00159B00
	public void GetPackageTypes(int[] _packagesPerTypeReceived, int[] _bytesPerTypeReceived, int[] _packagesPerTypeSent, int[] _bytesPerTypeSent, bool _reset)
	{
		for (int i = 0; i < _packagesPerTypeReceived.Length; i++)
		{
			_packagesPerTypeReceived[i] += this.statsPackagePerTypeReceived[i];
			_packagesPerTypeSent[i] += this.statsPackagePerTypeSent[i];
			_bytesPerTypeReceived[i] += this.statsBytesPerTypeReceived[i];
			_bytesPerTypeSent[i] += this.statsBytesPerTypeSent[i];
		}
		if (_reset)
		{
			Array.Clear(this.statsPackagePerTypeReceived, 0, this.statsPackagePerTypeReceived.Length);
			Array.Clear(this.statsPackagePerTypeSent, 0, this.statsPackagePerTypeSent.Length);
			Array.Clear(this.statsBytesPerTypeReceived, 0, this.statsBytesPerTypeReceived.Length);
			Array.Clear(this.statsBytesPerTypeSent, 0, this.statsBytesPerTypeSent.Length);
		}
	}

	// Token: 0x0600334E RID: 13134 RVA: 0x0015B9BC File Offset: 0x00159BBC
	public RingBuffer<SNetPackageInfo> GetLastPackagesSent()
	{
		return this.statsLastPackagesSent;
	}

	// Token: 0x0600334F RID: 13135 RVA: 0x0015B9C4 File Offset: 0x00159BC4
	public RingBuffer<SNetPackageInfo> GetLastPackagesReceived()
	{
		return this.statsLastPackagesRec;
	}

	// Token: 0x06003350 RID: 13136 RVA: 0x0015B9CC File Offset: 0x00159BCC
	public void GetStats(float _interval, out int _bytesPerSecondSent, out int _packagesPerSecondSent, out int _bytesPerSecondReceived, out int _packagesPerSecondReceived)
	{
		_bytesPerSecondSent = this.statsBytesSent;
		_packagesPerSecondSent = this.statsPackagesSent;
		_bytesPerSecondReceived = this.statsBytesReceived;
		_packagesPerSecondReceived = this.statsPackagesReceived;
		this.resetStats();
	}

	// Token: 0x06003351 RID: 13137 RVA: 0x0015B9FE File Offset: 0x00159BFE
	[PublicizedFrom(EAccessModifier.Private)]
	public void resetStats()
	{
		this.statsBytesReceived = 0;
		this.statsPackagesReceived = 0;
		this.statsBytesSent = 0;
		this.statsPackagesSent = 0;
		this.lastTimeStatsRequested = Time.time;
	}

	// Token: 0x04002A0C RID: 10764
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastTimeStatsRequested;

	// Token: 0x04002A0D RID: 10765
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile int statsBytesSent;

	// Token: 0x04002A0E RID: 10766
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile int statsPackagesSent;

	// Token: 0x04002A0F RID: 10767
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] statsPackagePerTypeSent = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04002A10 RID: 10768
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] statsBytesPerTypeSent = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04002A11 RID: 10769
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly RingBuffer<SNetPackageInfo> statsLastPackagesSent = new RingBuffer<SNetPackageInfo>(30);

	// Token: 0x04002A12 RID: 10770
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile int statsBytesReceived;

	// Token: 0x04002A13 RID: 10771
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile int statsPackagesReceived;

	// Token: 0x04002A14 RID: 10772
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] statsPackagePerTypeReceived = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04002A15 RID: 10773
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] statsBytesPerTypeReceived = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04002A16 RID: 10774
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly RingBuffer<SNetPackageInfo> statsLastPackagesRec = new RingBuffer<SNetPackageInfo>(30);

	// Token: 0x04002A17 RID: 10775
	[PublicizedFrom(EAccessModifier.Private)]
	public int bytesPerSecondSent;

	// Token: 0x04002A18 RID: 10776
	[PublicizedFrom(EAccessModifier.Private)]
	public int packagesPerSecondSent;

	// Token: 0x04002A19 RID: 10777
	[PublicizedFrom(EAccessModifier.Private)]
	public int bytesPerSecondReceived;

	// Token: 0x04002A1A RID: 10778
	[PublicizedFrom(EAccessModifier.Private)]
	public int packagesPerSecondReceived;
}
