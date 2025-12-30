using System;
using UnityEngine.Scripting;

// Token: 0x0200075E RID: 1886
[Preserve]
public class NetPackageMinEventFire : NetPackage
{
	// Token: 0x06003700 RID: 14080 RVA: 0x00168EBB File Offset: 0x001670BB
	public NetPackageMinEventFire Setup(int _selfEntityID, int _otherEntityID, MinEventTypes _eventType, ItemValue _itemValue)
	{
		this.selfEntityID = _selfEntityID;
		this.otherEntityID = _otherEntityID;
		this.eventType = _eventType;
		this.itemValue = _itemValue;
		this.eventPackageType = NetPackageMinEventFire.EventPackageTypes.ItemEvent;
		return this;
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x00168EE2 File Offset: 0x001670E2
	public NetPackageMinEventFire Setup(int _selfEntityID, int _otherEntityID, MinEventTypes _eventType, BlockValue _blockValue)
	{
		this.selfEntityID = _selfEntityID;
		this.otherEntityID = _otherEntityID;
		this.eventType = _eventType;
		this.blockValue = _blockValue;
		this.eventPackageType = NetPackageMinEventFire.EventPackageTypes.BlockEvent;
		return this;
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x00168F0C File Offset: 0x0016710C
	public override void read(PooledBinaryReader _br)
	{
		this.selfEntityID = _br.ReadInt32();
		this.otherEntityID = _br.ReadInt32();
		this.eventType = (MinEventTypes)_br.ReadByte();
		this.eventPackageType = (NetPackageMinEventFire.EventPackageTypes)_br.ReadByte();
		if (this.eventPackageType == NetPackageMinEventFire.EventPackageTypes.ItemEvent)
		{
			this.itemValue = new ItemValue();
			this.itemValue.Read(_br);
			this.blockValue = BlockValue.Air;
			return;
		}
		this.blockValue = new BlockValue(_br.ReadUInt32());
		this.itemValue = ItemValue.None;
	}

	// Token: 0x06003703 RID: 14083 RVA: 0x00168F90 File Offset: 0x00167190
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.selfEntityID);
		_bw.Write(this.otherEntityID);
		_bw.Write((byte)this.eventType);
		_bw.Write((byte)this.eventPackageType);
		if (this.eventPackageType == NetPackageMinEventFire.EventPackageTypes.ItemEvent)
		{
			this.itemValue.Write(_bw);
			return;
		}
		_bw.Write(this.blockValue.rawData);
	}

	// Token: 0x06003704 RID: 14084 RVA: 0x00168FFC File Offset: 0x001671FC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.selfEntityID) as EntityAlive;
		if (entityAlive != null)
		{
			entityAlive.MinEventContext.Self = entityAlive;
			entityAlive.MinEventContext.Other = ((this.otherEntityID == -1) ? null : (_world.GetEntity(this.otherEntityID) as EntityAlive));
			entityAlive.MinEventContext.ItemValue = this.itemValue;
			entityAlive.MinEventContext.BlockValue = this.blockValue;
			entityAlive.FireEvent(this.eventType, true);
		}
	}

	// Token: 0x17000596 RID: 1430
	// (get) Token: 0x06003705 RID: 14085 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x00169085 File Offset: 0x00167285
	public override int GetLength()
	{
		return 32;
	}

	// Token: 0x04002C8B RID: 11403
	[PublicizedFrom(EAccessModifier.Private)]
	public int selfEntityID;

	// Token: 0x04002C8C RID: 11404
	[PublicizedFrom(EAccessModifier.Private)]
	public int otherEntityID;

	// Token: 0x04002C8D RID: 11405
	[PublicizedFrom(EAccessModifier.Private)]
	public MinEventTypes eventType;

	// Token: 0x04002C8E RID: 11406
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue itemValue;

	// Token: 0x04002C8F RID: 11407
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValue;

	// Token: 0x04002C90 RID: 11408
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageMinEventFire.EventPackageTypes eventPackageType;

	// Token: 0x0200075F RID: 1887
	public enum EventPackageTypes
	{
		// Token: 0x04002C92 RID: 11410
		ItemEvent,
		// Token: 0x04002C93 RID: 11411
		BlockEvent
	}
}
