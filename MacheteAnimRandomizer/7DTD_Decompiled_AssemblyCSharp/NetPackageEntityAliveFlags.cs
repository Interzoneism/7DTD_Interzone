using System;
using UnityEngine.Scripting;

// Token: 0x02000732 RID: 1842
[Preserve]
public class NetPackageEntityAliveFlags : NetPackage
{
	// Token: 0x060035E6 RID: 13798 RVA: 0x001652A8 File Offset: 0x001634A8
	public NetPackageEntityAliveFlags Setup(EntityAlive _entity)
	{
		this.entityId = _entity.entityId;
		this.flags = 0;
		if (_entity.AimingGun)
		{
			this.flags |= 4;
		}
		if (_entity.Spawned)
		{
			this.flags |= 8;
		}
		if (_entity.Jumping)
		{
			this.flags |= 16;
		}
		if (_entity.IsBreakingBlocks)
		{
			this.flags |= 32;
		}
		if (_entity.IsAlert)
		{
			this.flags |= 64;
		}
		if (_entity.inventory.IsFlashlightOn)
		{
			this.flags |= 128;
		}
		if (_entity.IsGodMode.Value)
		{
			this.flags |= 256;
		}
		if (_entity.IsCrouching)
		{
			this.flags |= 512;
		}
		return this;
	}

	// Token: 0x060035E7 RID: 13799 RVA: 0x0016539A File Offset: 0x0016359A
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.flags = _reader.ReadUInt16();
	}

	// Token: 0x060035E8 RID: 13800 RVA: 0x001653B4 File Offset: 0x001635B4
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.flags);
	}

	// Token: 0x060035E9 RID: 13801 RVA: 0x001653D8 File Offset: 0x001635D8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.entityId, false))
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.entityId) as EntityAlive;
		if (entityAlive)
		{
			entityAlive.AimingGun = ((this.flags & 4) > 0);
			entityAlive.Spawned = ((this.flags & 8) > 0);
			entityAlive.Jumping = ((this.flags & 16) > 0);
			entityAlive.IsBreakingBlocks = ((this.flags & 32) > 0);
			entityAlive.IsGodMode.Value = ((this.flags & 256) > 0);
			entityAlive.Crouching = ((this.flags & 512) > 0);
			if (entityAlive.isEntityRemote)
			{
				entityAlive.bReplicatedAlertFlag = ((this.flags & 64) > 0);
			}
			entityAlive.inventory.SetFlashlight((this.flags & 128) > 0);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(entityAlive), false, -1, base.Sender.entityId, -1, null, 192, false);
			}
		}
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x0015DD3D File Offset: 0x0015BF3D
	public override int GetLength()
	{
		return 60;
	}

	// Token: 0x04002BD6 RID: 11222
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002BD7 RID: 11223
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFApproachingEnemy = 1;

	// Token: 0x04002BD8 RID: 11224
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFApproachingPlayer = 2;

	// Token: 0x04002BD9 RID: 11225
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFAimingGun = 4;

	// Token: 0x04002BDA RID: 11226
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFSpawned = 8;

	// Token: 0x04002BDB RID: 11227
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFJumping = 16;

	// Token: 0x04002BDC RID: 11228
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFIsBreakingBlocks = 32;

	// Token: 0x04002BDD RID: 11229
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFIsAlert = 64;

	// Token: 0x04002BDE RID: 11230
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFIsFlashlightOn = 128;

	// Token: 0x04002BDF RID: 11231
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFIsGodMode = 256;

	// Token: 0x04002BE0 RID: 11232
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cFCrouching = 512;

	// Token: 0x04002BE1 RID: 11233
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort flags;
}
