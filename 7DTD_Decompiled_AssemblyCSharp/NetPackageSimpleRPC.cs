using System;
using UnityEngine.Scripting;

// Token: 0x02000796 RID: 1942
[Preserve]
public class NetPackageSimpleRPC : NetPackage
{
	// Token: 0x0600384D RID: 14413 RVA: 0x0016F4BB File Offset: 0x0016D6BB
	public NetPackageSimpleRPC Setup(int _entityId, SimpleRPCType _type)
	{
		this.entityId = _entityId;
		this.type = _type;
		return this;
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x0016F4CC File Offset: 0x0016D6CC
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.type = (SimpleRPCType)_reader.ReadByte();
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x0016F4E6 File Offset: 0x0016D6E6
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write((byte)this.type);
	}

	// Token: 0x06003850 RID: 14416 RVA: 0x0016F507 File Offset: 0x0016D707
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!base.ValidEntityIdForSender(this.entityId, false))
		{
			return;
		}
		_callbacks.SimpleRPC(this.entityId, this.type, true, _world.IsRemote());
	}

	// Token: 0x06003851 RID: 14417 RVA: 0x000768A9 File Offset: 0x00074AA9
	public override int GetLength()
	{
		return 10;
	}

	// Token: 0x04002DAA RID: 11690
	public int entityId;

	// Token: 0x04002DAB RID: 11691
	public SimpleRPCType type;
}
