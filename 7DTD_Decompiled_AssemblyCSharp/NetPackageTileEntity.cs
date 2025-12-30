using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200079E RID: 1950
[Preserve]
public class NetPackageTileEntity : NetPackage
{
	// Token: 0x06003879 RID: 14457 RVA: 0x0016FBFB File Offset: 0x0016DDFB
	public NetPackageTileEntity Setup(TileEntity _te, TileEntity.StreamModeWrite _eStreamMode)
	{
		return this.Setup(_te, _eStreamMode, byte.MaxValue);
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x0016FC0C File Offset: 0x0016DE0C
	public NetPackageTileEntity Setup(TileEntity _te, TileEntity.StreamModeWrite _eStreamMode, byte _handle)
	{
		this.handle = _handle;
		this.teEntityId = _te.entityId;
		this.teWorldPos = _te.ToWorldPos();
		this.bValidEntityId = (this.teEntityId != -1);
		this.clrIdx = _te.GetClrIdx();
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.ms);
			_te.write(pooledBinaryWriter, _eStreamMode);
		}
		return this;
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x0016FC94 File Offset: 0x0016DE94
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageTileEntity()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x0016FCCC File Offset: 0x0016DECC
	public override void read(PooledBinaryReader _br)
	{
		this.handle = _br.ReadByte();
		this.bValidEntityId = _br.ReadBoolean();
		if (this.bValidEntityId)
		{
			this.teEntityId = _br.ReadInt32();
		}
		else
		{
			this.clrIdx = (int)_br.ReadUInt16();
			this.teWorldPos = StreamUtils.ReadVector3i(_br);
			this.teEntityId = -1;
		}
		int length = (int)_br.ReadUInt16();
		StreamUtils.StreamCopy(_br.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x0016FD44 File Offset: 0x0016DF44
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.handle);
		_bw.Write(this.bValidEntityId);
		if (this.bValidEntityId)
		{
			_bw.Write(this.teEntityId);
		}
		else
		{
			_bw.Write((ushort)this.clrIdx);
			StreamUtils.Write(_bw, this.teWorldPos);
		}
		_bw.Write((ushort)this.ms.Length);
		this.ms.WriteTo(_bw.BaseStream);
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x0016FDC4 File Offset: 0x0016DFC4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		TileEntity tileEntity = this.bValidEntityId ? _world.GetTileEntity(this.teEntityId) : _world.GetTileEntity(this.clrIdx, this.teWorldPos);
		if (tileEntity == null)
		{
			return;
		}
		tileEntity.SetHandle(this.handle);
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			PooledExpandableMemoryStream obj = this.ms;
			lock (obj)
			{
				pooledBinaryReader.SetBaseStream(this.ms);
				this.ms.Position = 0L;
				tileEntity.read(pooledBinaryReader, _world.IsRemote() ? TileEntity.StreamModeRead.FromServer : TileEntity.StreamModeRead.FromClient);
			}
		}
		tileEntity.NotifyListeners();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			tileEntity.SetChunkModified();
			Vector3? entitiesInRangeOfWorldPos = new Vector3?(tileEntity.ToWorldCenterPos());
			if (entitiesInRangeOfWorldPos.Value == Vector3.zero)
			{
				entitiesInRangeOfWorldPos = null;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTileEntity>().Setup(tileEntity, TileEntity.StreamModeWrite.ToClient, this.handle), true, -1, -1, -1, entitiesInRangeOfWorldPos, 192, false);
		}
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x0016FEF4 File Offset: 0x0016E0F4
	public override int GetLength()
	{
		return (int)(22L + this.ms.Length);
	}

	// Token: 0x04002DC6 RID: 11718
	[PublicizedFrom(EAccessModifier.Private)]
	public byte handle;

	// Token: 0x04002DC7 RID: 11719
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bValidEntityId;

	// Token: 0x04002DC8 RID: 11720
	[PublicizedFrom(EAccessModifier.Private)]
	public int teEntityId;

	// Token: 0x04002DC9 RID: 11721
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x04002DCA RID: 11722
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i teWorldPos;

	// Token: 0x04002DCB RID: 11723
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms = MemoryPools.poolMemoryStream.AllocSync(true);
}
