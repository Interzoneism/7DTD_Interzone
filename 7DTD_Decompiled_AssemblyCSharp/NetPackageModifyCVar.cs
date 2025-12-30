using System;
using UnityEngine.Scripting;

// Token: 0x020001A3 RID: 419
[Preserve]
public class NetPackageModifyCVar : NetPackage
{
	// Token: 0x06000CDF RID: 3295 RVA: 0x00057ACC File Offset: 0x00055CCC
	public NetPackageModifyCVar Setup(EntityAlive entity, string _cvarName, float _value, CVarOperation _operation)
	{
		this.m_entityId = entity.entityId;
		this.cvarName = _cvarName;
		this.value = _value;
		this.operation = _operation;
		return this;
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00057AF1 File Offset: 0x00055CF1
	public override void read(PooledBinaryReader _reader)
	{
		this.m_entityId = _reader.ReadInt32();
		this.cvarName = _reader.ReadString();
		this.value = _reader.ReadSingle();
		this.operation = (CVarOperation)_reader.ReadInt16();
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00057B23 File Offset: 0x00055D23
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_entityId);
		_writer.Write(this.cvarName);
		_writer.Write(this.value);
		_writer.Write((short)this.operation);
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00057B60 File Offset: 0x00055D60
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.m_entityId) as EntityAlive;
		if (entityAlive != null)
		{
			entityAlive.Buffs.SetCustomVar(this.cvarName, this.value, SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer, this.operation);
		}
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04000AA9 RID: 2729
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_entityId;

	// Token: 0x04000AAA RID: 2730
	[PublicizedFrom(EAccessModifier.Private)]
	public string cvarName;

	// Token: 0x04000AAB RID: 2731
	[PublicizedFrom(EAccessModifier.Private)]
	public float value;

	// Token: 0x04000AAC RID: 2732
	[PublicizedFrom(EAccessModifier.Private)]
	public CVarOperation operation;
}
