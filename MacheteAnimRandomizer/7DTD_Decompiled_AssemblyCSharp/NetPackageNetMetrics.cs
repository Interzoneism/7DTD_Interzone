using System;
using UnityEngine.Scripting;

// Token: 0x02000761 RID: 1889
[Preserve]
public class NetPackageNetMetrics : NetPackage
{
	// Token: 0x06003711 RID: 14097 RVA: 0x001692FC File Offset: 0x001674FC
	public static NetPackageNetMetrics SetupClient(string content, string csv)
	{
		NetPackageNetMetrics package = NetPackageManager.GetPackage<NetPackageNetMetrics>();
		package.content = content;
		package.csv = csv;
		return package;
	}

	// Token: 0x06003712 RID: 14098 RVA: 0x00169311 File Offset: 0x00167511
	public static NetPackageNetMetrics SetupServer(bool enable, float duration, bool loop)
	{
		NetPackageNetMetrics package = NetPackageManager.GetPackage<NetPackageNetMetrics>();
		package.enable = enable;
		package.duration = duration;
		package.loop = loop;
		return package;
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x00169330 File Offset: 0x00167530
	public override void read(PooledBinaryReader _reader)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.content = _reader.ReadString();
			this.csv = _reader.ReadString();
			return;
		}
		this.enable = _reader.ReadBoolean();
		this.duration = _reader.ReadSingle();
		this.loop = _reader.ReadBoolean();
	}

	// Token: 0x06003714 RID: 14100 RVA: 0x00169388 File Offset: 0x00167588
	public override void write(PooledBinaryWriter _writer)
	{
		_writer.Write((byte)base.PackageId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			_writer.Write(this.enable);
			_writer.Write(this.duration);
			_writer.Write(this.loop);
			return;
		}
		_writer.Write(this.content);
		_writer.Write(this.csv);
	}

	// Token: 0x06003715 RID: 14101 RVA: 0x001693EC File Offset: 0x001675EC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Log.Out("RECEIVED STATS BACK");
			Log.Out(this.content);
			Log.Out(this.csv);
			GameManager.Instance.netpackageMetrics.AppendClientCSV(this.csv);
			return;
		}
		Log.Out("REQUESTED TO RECORD STATS");
		if (this.enable)
		{
			NetPackageMetrics.Instance.RecordForPeriod(this.duration);
		}
	}

	// Token: 0x06003716 RID: 14102 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002C9C RID: 11420
	public bool enable;

	// Token: 0x04002C9D RID: 11421
	public float duration;

	// Token: 0x04002C9E RID: 11422
	public bool loop;

	// Token: 0x04002C9F RID: 11423
	public string content;

	// Token: 0x04002CA0 RID: 11424
	public string csv;
}
