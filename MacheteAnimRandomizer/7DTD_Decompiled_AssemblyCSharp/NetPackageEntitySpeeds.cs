using System;
using UnityEngine.Scripting;

// Token: 0x02000743 RID: 1859
[Preserve]
public class NetPackageEntitySpeeds : NetPackage
{
	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x06003655 RID: 13909 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ReliableDelivery
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06003656 RID: 13910 RVA: 0x00166ADB File Offset: 0x00164CDB
	public NetPackageEntitySpeeds Setup(Entity _entity)
	{
		this.entityId = _entity.entityId;
		this.movementState = _entity.MovementState;
		this.speedForward = _entity.speedForward;
		this.speedStrafe = _entity.speedStrafe;
		return this;
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x00166B0E File Offset: 0x00164D0E
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.movementState = (int)_reader.ReadByte();
		this.speedForward = _reader.ReadSingle();
		this.speedStrafe = _reader.ReadSingle();
	}

	// Token: 0x06003658 RID: 13912 RVA: 0x00166B40 File Offset: 0x00164D40
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write((byte)this.movementState);
		_writer.Write(this.speedForward);
		_writer.Write(this.speedStrafe);
	}

	// Token: 0x06003659 RID: 13913 RVA: 0x00166B7C File Offset: 0x00164D7C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Entity entity = _world.GetEntity(this.entityId);
		if (entity != null)
		{
			entity.MovementState = this.movementState;
			entity.speedForward = this.speedForward;
			entity.speedStrafe = this.speedStrafe;
			if (!_world.IsRemote())
			{
				_world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, this, true);
			}
		}
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C18 RID: 11288
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C19 RID: 11289
	[PublicizedFrom(EAccessModifier.Private)]
	public int movementState;

	// Token: 0x04002C1A RID: 11290
	[PublicizedFrom(EAccessModifier.Private)]
	public float speedForward;

	// Token: 0x04002C1B RID: 11291
	[PublicizedFrom(EAccessModifier.Private)]
	public float speedStrafe;
}
