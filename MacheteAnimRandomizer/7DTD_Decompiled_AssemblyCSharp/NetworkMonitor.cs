using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

// Token: 0x020011CF RID: 4559
public class NetworkMonitor
{
	// Token: 0x06008E75 RID: 36469 RVA: 0x00390E3C File Offset: 0x0038F03C
	public NetworkMonitor(int _channel, Transform _parent)
	{
		this.channel = _channel;
		this.parent = _parent;
		this.uiLblOverview = _parent.Find("lblOverview").GetComponent<UILabel>();
		Transform transform;
		this.uiLblRecvPkgCnt = (((transform = _parent.Find("lblRecvPkgCnt")) != null) ? transform.GetComponent<UILabel>() : null);
		this.uiLblSentPkgCnt = (((transform = _parent.Find("lblSentPkgCnt")) != null) ? transform.GetComponent<UILabel>() : null);
		this.uiLblRecvPkgSeq = (((transform = _parent.Find("lblRecvPkgSeq")) != null) ? transform.GetComponent<UILabel>() : null);
		this.uiLblSentPkgSeq = (((transform = _parent.Find("lblSentPkgSeq")) != null) ? transform.GetComponent<UILabel>() : null);
		this.uiTxtRecv = (((transform = _parent.Find("texRecv")) != null) ? transform.GetComponent<UITexture>() : null);
		this.uiTxtSent = (((transform = _parent.Find("texSent")) != null) ? transform.GetComponent<UITexture>() : null);
	}

	// Token: 0x06008E76 RID: 36470 RVA: 0x00390FA4 File Offset: 0x0038F1A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		this.initialized = true;
		if (this.uiTxtRecv != null)
		{
			this.graphReceived = new SimpleGraph();
			this.graphReceived.Init(1024, 128, 1f, new float[]
			{
				0.01f,
				0.5f,
				1f
			});
			this.uiTxtRecv.mainTexture = this.graphReceived.texture;
		}
		if (this.uiTxtSent != null)
		{
			this.graphSent = new SimpleGraph();
			this.graphSent.Init(1024, 128, 1f, new float[]
			{
				0.01f,
				0.5f,
				1f
			});
			this.uiTxtSent.mainTexture = this.graphSent.texture;
		}
	}

	// Token: 0x06008E77 RID: 36471 RVA: 0x0039106C File Offset: 0x0038F26C
	public void Cleanup()
	{
		SimpleGraph simpleGraph = this.graphReceived;
		if (simpleGraph != null)
		{
			simpleGraph.Cleanup();
		}
		SimpleGraph simpleGraph2 = this.graphSent;
		if (simpleGraph2 == null)
		{
			return;
		}
		simpleGraph2.Cleanup();
	}

	// Token: 0x06008E78 RID: 36472 RVA: 0x00391090 File Offset: 0x0038F290
	public void ResetAllNumbers()
	{
		this.bResetNext = true;
		this.sumBRecv = 0;
		this.totalBRecv = 0;
		this.sumBSent = 0;
		this.totalBSent = 0;
		this.sumPRecv = 0;
		this.totalPRecv = 0;
		this.sumPSent = 0;
		this.totalPSent = 0;
	}

	// Token: 0x06008E79 RID: 36473 RVA: 0x003910DC File Offset: 0x0038F2DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDataForConnection(INetConnection[] _nc)
	{
		if (this.channel >= _nc.Length)
		{
			return;
		}
		if (_nc[this.channel] != null)
		{
			int num;
			int num2;
			int num3;
			int num4;
			_nc[this.channel].GetStats().GetStats(0f, out num, out num2, out num3, out num4);
			this.totalBytesSent += num;
			this.totalBytesReceived += num3;
			this.totalPackagesSent += num2;
			this.totalPackagesReceived += num4;
			_nc[this.channel].GetStats().GetPackageTypes(this.packagesPerTypeReceived, this.bytesPerTypeReceived, this.packagesPerTypeSent, this.bytesPerTypeSent, this.bResetNext);
			this.recSequence = _nc[this.channel].GetStats().GetLastPackagesReceived();
			this.sentSequence = _nc[this.channel].GetStats().GetLastPackagesSent();
		}
		this.bResetNext = false;
	}

	// Token: 0x06008E7A RID: 36474 RVA: 0x003911C0 File Offset: 0x0038F3C0
	public void Update()
	{
		if (this.Enabled != this.parent.gameObject.activeSelf)
		{
			this.parent.gameObject.SetActive(this.Enabled);
		}
		if (!this.Enabled)
		{
			this.bpsRecv = (this.bpsSent = 0f);
			this.sumPRecv = (this.sumPSent = 0);
			return;
		}
		this.timePassed += Time.deltaTime;
		if (!this.initialized)
		{
			this.Init();
		}
		this.totalBytesSent = 0;
		this.totalBytesReceived = 0;
		this.totalPackagesSent = 0;
		this.totalPackagesReceived = 0;
		Array.Clear(this.packagesPerTypeReceived, 0, this.packagesPerTypeReceived.Length);
		Array.Clear(this.bytesPerTypeReceived, 0, this.bytesPerTypeReceived.Length);
		Array.Clear(this.packagesPerTypeSent, 0, this.packagesPerTypeSent.Length);
		Array.Clear(this.bytesPerTypeSent, 0, this.bytesPerTypeSent.Length);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			ReadOnlyCollection<ClientInfo> list = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ClientInfo clientInfo = list[i];
					this.updateDataForConnection(clientInfo.netConnection);
				}
			}
		}
		else
		{
			this.updateDataForConnection(SingletonMonoBehaviour<ConnectionManager>.Instance.GetConnectionToServer());
		}
		SimpleGraph simpleGraph = this.graphReceived;
		if (simpleGraph != null)
		{
			simpleGraph.Update((float)this.totalBytesReceived / 1024f, Color.green);
		}
		SimpleGraph simpleGraph2 = this.graphSent;
		if (simpleGraph2 != null)
		{
			simpleGraph2.Update((float)this.totalBytesSent / 1024f, Color.green);
		}
		this.sumBRecv += this.totalBytesReceived;
		this.totalBRecv += this.totalBytesReceived;
		this.sumBSent += this.totalBytesSent;
		this.totalBSent += this.totalBytesSent;
		this.sumPRecv += this.totalPackagesReceived;
		this.totalPRecv += this.totalPackagesReceived;
		this.sumPSent += this.totalPackagesSent;
		this.totalPSent += this.totalPackagesSent;
		if (this.timePassed > 1f)
		{
			this.bpsRecv = (float)this.sumBRecv / this.timePassed;
			this.bpsSent = (float)this.sumBSent / this.timePassed;
			this.sumBSent = (this.sumBRecv = 0);
			this.ppsRecv = (float)this.sumPRecv / this.timePassed;
			this.ppsSent = (float)this.sumPSent / this.timePassed;
			this.sumPRecv = (this.sumPSent = 0);
			this.timePassed = 0f;
		}
		this.uiLblOverview.text = string.Format("Overview Channel {0}:\n Recv: {1:0.00} kB/s {2:0} kB\n Sent: {3:0.00} kB/s {4:0} kB\n Recv: {5:0.0} p/s {6:0} p\n Sent: {7:0.0} p/s {8:0} p\n", new object[]
		{
			this.channel,
			this.bpsRecv / 1024f,
			(float)this.totalBRecv / 1024f,
			this.bpsSent / 1024f,
			(float)this.totalBSent / 1024f,
			this.ppsRecv,
			this.totalPRecv,
			this.ppsSent,
			this.totalPSent
		});
		StringBuilder sb = new StringBuilder();
		this.updatePackageListText(sb, this.uiLblRecvPkgCnt, this.packagesPerTypeReceived, this.bytesPerTypeReceived, "Rec");
		this.updatePackageListText(sb, this.uiLblSentPkgCnt, this.packagesPerTypeSent, this.bytesPerTypeSent, "Sent");
		if (this.uiLblRecvPkgSeq != null)
		{
			this.uiLblRecvPkgSeq.text = this.printPackageSequence(this.recSequence, "Rec last 30:");
		}
		if (this.uiLblSentPkgSeq != null)
		{
			this.uiLblSentPkgSeq.text = this.printPackageSequence(this.sentSequence, "Sent last 30:");
		}
	}

	// Token: 0x06008E7B RID: 36475 RVA: 0x003915C4 File Offset: 0x0038F7C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePackageListText(StringBuilder _sb, UILabel _label, int[] _packagesPerType, int[] _bytesPerType, string _caption)
	{
		if (_label != null)
		{
			this.sortList.Clear();
			for (int i = 0; i < _packagesPerType.Length; i++)
			{
				NetworkMonitor.SIdCnt item = new NetworkMonitor.SIdCnt
				{
					Id = i,
					Cnt = _packagesPerType[i],
					Bytes = _bytesPerType[i]
				};
				this.sortList.Add(item);
			}
			this.sortList.Sort(this.sorter);
			_sb.Length = 0;
			_sb.Append(_caption);
			_sb.Append(":\n");
			int num = 0;
			while (num < 16 && num < this.sortList.Count && this.sortList[num].Cnt != 0)
			{
				_sb.Append(string.Format("{0}. {1}: {2} - {3} B\n", new object[]
				{
					num + 1,
					NetPackageManager.GetPackageName(this.sortList[num].Id),
					this.sortList[num].Cnt,
					this.sortList[num].Bytes
				}));
				num++;
			}
			_label.text = _sb.ToString();
		}
	}

	// Token: 0x06008E7C RID: 36476 RVA: 0x00391708 File Offset: 0x0038F908
	[PublicizedFrom(EAccessModifier.Private)]
	public string printPackageSequence(RingBuffer<SNetPackageInfo> _sequence, string _addInfo)
	{
		if (_sequence == null)
		{
			return _addInfo;
		}
		StringBuilder stringBuilder = new StringBuilder();
		_sequence.SetToLast();
		ulong tick = _sequence.Peek().Tick;
		stringBuilder.Append(_addInfo);
		stringBuilder.Append(" ");
		stringBuilder.Append(tick.ToString());
		stringBuilder.Append("\n");
		SNetPackageInfo? snetPackageInfo = null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		while (num3 < 30 && num4 < _sequence.Count - 1)
		{
			SNetPackageInfo prev = _sequence.GetPrev();
			if (snetPackageInfo != null)
			{
				if (snetPackageInfo.Value.Tick == prev.Tick && snetPackageInfo.Value.Id == prev.Id)
				{
					num++;
				}
				else
				{
					stringBuilder.Append(this.formatPackage(tick, snetPackageInfo.Value, num, num2));
					num = 0;
					num2 = 0;
					num3++;
				}
			}
			snetPackageInfo = new SNetPackageInfo?(prev);
			num2 += prev.Size;
			num4++;
		}
		if (snetPackageInfo != null)
		{
			stringBuilder.Append(this.formatPackage(tick, snetPackageInfo.Value, num, num2));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06008E7D RID: 36477 RVA: 0x00391830 File Offset: 0x0038FA30
	[PublicizedFrom(EAccessModifier.Private)]
	public string formatPackage(ulong _baseTick, SNetPackageInfo _lastPackage, int _lastPackageCnt, int _size)
	{
		return string.Format(" {0:000} {1}{2} {3} B\n", new object[]
		{
			_baseTick - _lastPackage.Tick,
			(_lastPackageCnt > 0) ? ((_lastPackageCnt + 1).ToString() + "x ") : string.Empty,
			NetPackageManager.GetPackageName(_lastPackage.Id),
			_size
		});
	}

	// Token: 0x04006E3B RID: 28219
	public bool Enabled;

	// Token: 0x04006E3C RID: 28220
	[PublicizedFrom(EAccessModifier.Private)]
	public bool initialized;

	// Token: 0x04006E3D RID: 28221
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UILabel uiLblOverview;

	// Token: 0x04006E3E RID: 28222
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UILabel uiLblRecvPkgCnt;

	// Token: 0x04006E3F RID: 28223
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UILabel uiLblSentPkgCnt;

	// Token: 0x04006E40 RID: 28224
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UILabel uiLblRecvPkgSeq;

	// Token: 0x04006E41 RID: 28225
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UILabel uiLblSentPkgSeq;

	// Token: 0x04006E42 RID: 28226
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UITexture uiTxtRecv;

	// Token: 0x04006E43 RID: 28227
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly UITexture uiTxtSent;

	// Token: 0x04006E44 RID: 28228
	[PublicizedFrom(EAccessModifier.Private)]
	public SimpleGraph graphReceived;

	// Token: 0x04006E45 RID: 28229
	[PublicizedFrom(EAccessModifier.Private)]
	public SimpleGraph graphSent;

	// Token: 0x04006E46 RID: 28230
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] packagesPerTypeReceived = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04006E47 RID: 28231
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] packagesPerTypeSent = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04006E48 RID: 28232
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] bytesPerTypeReceived = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04006E49 RID: 28233
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] bytesPerTypeSent = new int[NetPackageManager.KnownPackageCount];

	// Token: 0x04006E4A RID: 28234
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bResetNext;

	// Token: 0x04006E4B RID: 28235
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalBytesSent;

	// Token: 0x04006E4C RID: 28236
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalBytesReceived;

	// Token: 0x04006E4D RID: 28237
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalPackagesSent;

	// Token: 0x04006E4E RID: 28238
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalPackagesReceived;

	// Token: 0x04006E4F RID: 28239
	[PublicizedFrom(EAccessModifier.Private)]
	public RingBuffer<SNetPackageInfo> recSequence;

	// Token: 0x04006E50 RID: 28240
	[PublicizedFrom(EAccessModifier.Private)]
	public RingBuffer<SNetPackageInfo> sentSequence;

	// Token: 0x04006E51 RID: 28241
	[PublicizedFrom(EAccessModifier.Private)]
	public float timePassed;

	// Token: 0x04006E52 RID: 28242
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<NetworkMonitor.SIdCnt> sortList = new List<NetworkMonitor.SIdCnt>();

	// Token: 0x04006E53 RID: 28243
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly NetworkMonitor.SIdCntSorter sorter = new NetworkMonitor.SIdCntSorter();

	// Token: 0x04006E54 RID: 28244
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Transform parent;

	// Token: 0x04006E55 RID: 28245
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int channel;

	// Token: 0x04006E56 RID: 28246
	[PublicizedFrom(EAccessModifier.Private)]
	public int sumBRecv;

	// Token: 0x04006E57 RID: 28247
	[PublicizedFrom(EAccessModifier.Private)]
	public int sumBSent;

	// Token: 0x04006E58 RID: 28248
	[PublicizedFrom(EAccessModifier.Private)]
	public int sumPRecv;

	// Token: 0x04006E59 RID: 28249
	[PublicizedFrom(EAccessModifier.Private)]
	public int sumPSent;

	// Token: 0x04006E5A RID: 28250
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalBRecv;

	// Token: 0x04006E5B RID: 28251
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalBSent;

	// Token: 0x04006E5C RID: 28252
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalPRecv;

	// Token: 0x04006E5D RID: 28253
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalPSent;

	// Token: 0x04006E5E RID: 28254
	[PublicizedFrom(EAccessModifier.Private)]
	public float bpsRecv;

	// Token: 0x04006E5F RID: 28255
	[PublicizedFrom(EAccessModifier.Private)]
	public float bpsSent;

	// Token: 0x04006E60 RID: 28256
	[PublicizedFrom(EAccessModifier.Private)]
	public float ppsRecv;

	// Token: 0x04006E61 RID: 28257
	[PublicizedFrom(EAccessModifier.Private)]
	public float ppsSent;

	// Token: 0x04006E62 RID: 28258
	[PublicizedFrom(EAccessModifier.Private)]
	public const int maxDisplayedPackageTypes = 16;

	// Token: 0x020011D0 RID: 4560
	[PublicizedFrom(EAccessModifier.Private)]
	public struct SIdCnt
	{
		// Token: 0x04006E63 RID: 28259
		public int Id;

		// Token: 0x04006E64 RID: 28260
		public int Cnt;

		// Token: 0x04006E65 RID: 28261
		public int Bytes;
	}

	// Token: 0x020011D1 RID: 4561
	[PublicizedFrom(EAccessModifier.Private)]
	public class SIdCntSorter : IComparer<NetworkMonitor.SIdCnt>
	{
		// Token: 0x06008E7E RID: 36478 RVA: 0x00391898 File Offset: 0x0038FA98
		public int Compare(NetworkMonitor.SIdCnt _obj1, NetworkMonitor.SIdCnt _obj2)
		{
			return _obj2.Cnt - _obj1.Cnt;
		}
	}
}
