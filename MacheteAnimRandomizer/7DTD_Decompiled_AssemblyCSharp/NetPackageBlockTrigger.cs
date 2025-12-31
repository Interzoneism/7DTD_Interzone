using System;
using UnityEngine.Scripting;

// Token: 0x02000704 RID: 1796
[Preserve]
public class NetPackageBlockTrigger : NetPackage
{
	// Token: 0x060034E7 RID: 13543 RVA: 0x00161E43 File Offset: 0x00160043
	public NetPackageBlockTrigger Setup(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		this.clrIdx = _clrIdx;
		this.blockPos = _blockPos;
		this.blockValue = _blockValue;
		return this;
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x00161E5B File Offset: 0x0016005B
	public override void read(PooledBinaryReader _br)
	{
		this.clrIdx = _br.ReadInt32();
		this.blockPos = StreamUtils.ReadVector3i(_br);
		this.blockValue = new BlockValue(_br.ReadUInt32());
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x00161E86 File Offset: 0x00160086
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.clrIdx);
		StreamUtils.Write(_bw, this.blockPos);
		_bw.Write(this.blockValue.rawData);
	}

	// Token: 0x060034EA RID: 13546 RVA: 0x00161EB8 File Offset: 0x001600B8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (base.Sender.bAttachedToEntity)
		{
			EntityPlayer player = _world.GetEntity(base.Sender.entityId) as EntityPlayer;
			this.blockValue.Block.HandleTrigger(player, _world, this.clrIdx, this.blockPos, this.blockValue);
		}
	}

	// Token: 0x17000557 RID: 1367
	// (get) Token: 0x060034EB RID: 13547 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060034EC RID: 13548 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}

	// Token: 0x04002B23 RID: 11043
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x04002B24 RID: 11044
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04002B25 RID: 11045
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValue;
}
