using System;
using UnityEngine.Scripting;

// Token: 0x0200076D RID: 1901
[Preserve]
public class NetPackagePickupBlock : NetPackage
{
	// Token: 0x0600374D RID: 14157 RVA: 0x0016A8C2 File Offset: 0x00168AC2
	public NetPackagePickupBlock Setup(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerId, PersistentPlayerData _persistentPlayerData)
	{
		this.clrIdx = _clrIdx;
		this.blockPos = _blockPos;
		this.blockValue = _blockValue;
		this.playerId = _playerId;
		this.persistentPlayerId = ((_persistentPlayerData != null) ? _persistentPlayerData.PrimaryId : null);
		return this;
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x0016A8F8 File Offset: 0x00168AF8
	public override void read(PooledBinaryReader _br)
	{
		this.clrIdx = _br.ReadInt32();
		this.blockPos = StreamUtils.ReadVector3i(_br);
		this.blockValue = new BlockValue(_br.ReadUInt32());
		this.playerId = _br.ReadInt32();
		this.persistentPlayerId = PlatformUserIdentifierAbs.FromStream(_br, false, false);
	}

	// Token: 0x0600374F RID: 14159 RVA: 0x0016A948 File Offset: 0x00168B48
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.clrIdx);
		StreamUtils.Write(_bw, this.blockPos);
		_bw.Write(this.blockValue.rawData);
		_bw.Write(this.playerId);
		this.persistentPlayerId.ToStream(_bw, false);
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x0016A9A0 File Offset: 0x00168BA0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.playerId, false))
		{
			return;
		}
		if (!base.ValidUserIdForSender(this.persistentPlayerId))
		{
			return;
		}
		if (!_world.IsRemote())
		{
			_world.GetGameManager().PickupBlockServer(this.clrIdx, this.blockPos, this.blockValue, this.playerId, this.persistentPlayerId);
			return;
		}
		_world.GetGameManager().PickupBlockClient(this.clrIdx, this.blockPos, this.blockValue, this.playerId);
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x0016AA25 File Offset: 0x00168C25
	public override int GetLength()
	{
		return 36;
	}

	// Token: 0x04002CE3 RID: 11491
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x04002CE4 RID: 11492
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04002CE5 RID: 11493
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockValue;

	// Token: 0x04002CE6 RID: 11494
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerId;

	// Token: 0x04002CE7 RID: 11495
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs persistentPlayerId;
}
