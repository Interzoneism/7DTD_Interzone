using System;
using UnityEngine.Scripting;

// Token: 0x0200073E RID: 1854
[Preserve]
public class NetPackageEntityRemove : NetPackage
{
	// Token: 0x1700057D RID: 1405
	// (get) Token: 0x06003634 RID: 13876 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003635 RID: 13877 RVA: 0x00166482 File Offset: 0x00164682
	public NetPackageEntityRemove Setup(int _entityId, EnumRemoveEntityReason _reason)
	{
		this.entityId = _entityId;
		this.reason = (byte)_reason;
		return this;
	}

	// Token: 0x06003636 RID: 13878 RVA: 0x00166494 File Offset: 0x00164694
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.reason = _reader.ReadByte();
	}

	// Token: 0x06003637 RID: 13879 RVA: 0x001664AE File Offset: 0x001646AE
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.reason);
	}

	// Token: 0x06003638 RID: 13880 RVA: 0x001664CF File Offset: 0x001646CF
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.RemoveEntity(this.entityId, (EnumRemoveEntityReason)this.reason);
	}

	// Token: 0x06003639 RID: 13881 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002C0C RID: 11276
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C0D RID: 11277
	[PublicizedFrom(EAccessModifier.Private)]
	public byte reason;
}
