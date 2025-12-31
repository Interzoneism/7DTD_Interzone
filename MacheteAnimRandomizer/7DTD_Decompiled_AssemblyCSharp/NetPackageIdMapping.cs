using System;
using UnityEngine.Scripting;

// Token: 0x02000753 RID: 1875
[Preserve]
public class NetPackageIdMapping : NetPackage
{
	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x060036B5 RID: 14005 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x00168362 File Offset: 0x00166562
	public NetPackageIdMapping Setup(string _name, byte[] _data)
	{
		this.name = _name;
		this.data = _data;
		return this;
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x00168374 File Offset: 0x00166574
	public override void read(PooledBinaryReader _reader)
	{
		this.name = _reader.ReadString();
		int count = _reader.ReadInt32();
		this.data = _reader.ReadBytes(count);
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x001683A1 File Offset: 0x001665A1
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.name);
		_writer.Write(this.data.Length);
		_writer.Write(this.data);
	}

	// Token: 0x060036B9 RID: 14009 RVA: 0x001683D0 File Offset: 0x001665D0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		GameManager.Instance.IdMappingReceived(this.name, this.data);
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x001683E8 File Offset: 0x001665E8
	public override int GetLength()
	{
		return this.name.Length * 2 + this.data.Length;
	}

	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x060036BB RID: 14011 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002C6A RID: 11370
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04002C6B RID: 11371
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}
