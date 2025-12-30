using System;
using UnityEngine.Scripting;

// Token: 0x02000751 RID: 1873
[Preserve]
public class NetPackageHoldingItem : NetPackage
{
	// Token: 0x060036A9 RID: 13993 RVA: 0x00168110 File Offset: 0x00166310
	public NetPackageHoldingItem Setup(EntityAlive _entity)
	{
		this.entityId = _entity.entityId;
		this.holdingItemStack = _entity.inventory.holdingItemStack;
		this.holdingItemIndex = (byte)_entity.inventory.holdingItemIdx;
		return this;
	}

	// Token: 0x060036AA RID: 13994 RVA: 0x00168142 File Offset: 0x00166342
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.holdingItemStack = new ItemStack();
		this.holdingItemStack.Read(_reader);
		this.holdingItemIndex = _reader.ReadByte();
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x00168174 File Offset: 0x00166374
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		this.holdingItemStack.Write(_writer);
		_writer.Write(this.holdingItemIndex);
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x001681A4 File Offset: 0x001663A4
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
			entityAlive.EnqueueNetworkHoldingData(this.holdingItemStack, this.holdingItemIndex);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageHoldingItem>().Setup(entityAlive), false, -1, base.Sender.entityId, -1, null, 192, false);
		}
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002C64 RID: 11364
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C65 RID: 11365
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack holdingItemStack;

	// Token: 0x04002C66 RID: 11366
	[PublicizedFrom(EAccessModifier.Private)]
	public byte holdingItemIndex;
}
