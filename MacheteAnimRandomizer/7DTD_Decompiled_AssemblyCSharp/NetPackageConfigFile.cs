using System;
using UnityEngine.Scripting;

// Token: 0x02000711 RID: 1809
[Preserve]
public class NetPackageConfigFile : NetPackage
{
	// Token: 0x17000563 RID: 1379
	// (get) Token: 0x06003533 RID: 13619 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x00162D12 File Offset: 0x00160F12
	public NetPackageConfigFile Setup(string _name, byte[] _data)
	{
		this.name = _name;
		this.data = _data;
		return this;
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x00162D24 File Offset: 0x00160F24
	public override void read(PooledBinaryReader _reader)
	{
		this.name = _reader.ReadString();
		int num = _reader.ReadInt32();
		this.data = ((num >= 0) ? _reader.ReadBytes(num) : null);
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x00162D58 File Offset: 0x00160F58
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.name);
		if (this.data != null)
		{
			_writer.Write(this.data.Length);
			_writer.Write(this.data);
			return;
		}
		_writer.Write(-1);
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x00162D97 File Offset: 0x00160F97
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		WorldStaticData.ReceivedConfigFile(this.name, this.data);
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x00162DAA File Offset: 0x00160FAA
	public override int GetLength()
	{
		int num = this.name.Length * 2;
		byte[] array = this.data;
		return num + ((array != null) ? array.Length : 0);
	}

	// Token: 0x17000564 RID: 1380
	// (get) Token: 0x06003539 RID: 13625 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002B57 RID: 11095
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04002B58 RID: 11096
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}
