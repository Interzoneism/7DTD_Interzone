using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020007B3 RID: 1971
[Preserve]
public class NetPackageWorldAreas : NetPackage
{
	// Token: 0x060038F4 RID: 14580 RVA: 0x00171BE7 File Offset: 0x0016FDE7
	public NetPackageWorldAreas Setup(List<TraderArea> _list)
	{
		this.traders = _list;
		return this;
	}

	// Token: 0x060038F5 RID: 14581 RVA: 0x00171BF4 File Offset: 0x0016FDF4
	public override void read(PooledBinaryReader _reader)
	{
		_reader.ReadByte();
		int num = (int)_reader.ReadInt16();
		this.traders = new List<TraderArea>();
		for (int i = 0; i < num; i++)
		{
			TraderArea item = TraderArea.Read(_reader);
			this.traders.Add(item);
		}
	}

	// Token: 0x060038F6 RID: 14582 RVA: 0x00171C3C File Offset: 0x0016FE3C
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(1);
		_writer.Write((short)this.traders.Count);
		for (int i = 0; i < this.traders.Count; i++)
		{
			this.traders[i].Write(_writer);
		}
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x00171C91 File Offset: 0x0016FE91
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		World.SetWorldAreas(this.traders);
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x00171CA0 File Offset: 0x0016FEA0
	public override int GetLength()
	{
		int num = 2;
		for (int i = 0; i < this.traders.Count; i++)
		{
			num += this.traders[i].GetReadWriteSize();
		}
		return num;
	}

	// Token: 0x170005B9 RID: 1465
	// (get) Token: 0x060038F9 RID: 14585 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002E0A RID: 11786
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte cVersion = 1;

	// Token: 0x04002E0B RID: 11787
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TraderArea> traders;
}
