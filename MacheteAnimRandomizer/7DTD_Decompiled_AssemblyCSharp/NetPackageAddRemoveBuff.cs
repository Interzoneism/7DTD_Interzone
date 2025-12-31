using System;
using UnityEngine.Scripting;

// Token: 0x020001A2 RID: 418
[Preserve]
public class NetPackageAddRemoveBuff : NetPackage
{
	// Token: 0x06000CD9 RID: 3289 RVA: 0x00057918 File Offset: 0x00055B18
	public NetPackageAddRemoveBuff Setup(int _entityId, string _buffName, float _duration, bool _adding, int _instigatorId, Vector3i _instigatorPos)
	{
		this.entityId = _entityId;
		this.buffName = _buffName;
		this.adding = _adding;
		this.instigatorId = _instigatorId;
		this.duration = _duration;
		this.instigatorPos = _instigatorPos;
		return this;
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00057948 File Offset: 0x00055B48
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.buffName = _reader.ReadString();
		this.duration = _reader.ReadSingle();
		this.adding = _reader.ReadBoolean();
		this.instigatorId = _reader.ReadInt32();
		this.instigatorPos = StreamUtils.ReadVector3i(_reader);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x000579A0 File Offset: 0x00055BA0
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.buffName);
		_writer.Write(this.duration);
		_writer.Write(this.adding);
		_writer.Write(this.instigatorId);
		StreamUtils.Write(_writer, this.instigatorPos);
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x000579FC File Offset: 0x00055BFC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			ConnectionManager instance = SingletonMonoBehaviour<ConnectionManager>.Instance;
			NetPackage package = NetPackageManager.GetPackage<NetPackageAddRemoveBuff>().Setup(this.entityId, this.buffName, this.duration, this.adding, this.instigatorId, this.instigatorPos);
			bool onlyClientsAttachedToAnEntity = false;
			int attachedToEntityId = -1;
			int entitiesInRangeOfEntity = this.entityId;
			instance.SendPackage(package, onlyClientsAttachedToAnEntity, attachedToEntityId, this.instigatorId, entitiesInRangeOfEntity, null, 192, false);
		}
		EntityAlive entityAlive = _world.GetEntity(this.entityId) as EntityAlive;
		if (entityAlive != null)
		{
			if (this.adding)
			{
				entityAlive.Buffs.AddBuff(this.buffName, this.instigatorPos, this.instigatorId, false, false, this.duration);
				return;
			}
			entityAlive.Buffs.RemoveBuff(this.buffName, false);
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04000AA3 RID: 2723
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04000AA4 RID: 2724
	[PublicizedFrom(EAccessModifier.Private)]
	public int instigatorId;

	// Token: 0x04000AA5 RID: 2725
	[PublicizedFrom(EAccessModifier.Private)]
	public string buffName;

	// Token: 0x04000AA6 RID: 2726
	[PublicizedFrom(EAccessModifier.Private)]
	public float duration;

	// Token: 0x04000AA7 RID: 2727
	[PublicizedFrom(EAccessModifier.Private)]
	public bool adding;

	// Token: 0x04000AA8 RID: 2728
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i instigatorPos;
}
