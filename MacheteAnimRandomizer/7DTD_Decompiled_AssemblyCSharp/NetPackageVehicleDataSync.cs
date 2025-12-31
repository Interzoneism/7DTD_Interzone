using System;
using UnityEngine.Scripting;

// Token: 0x020007A5 RID: 1957
[Preserve]
public class NetPackageVehicleDataSync : NetPackage
{
	// Token: 0x060038A7 RID: 14503 RVA: 0x00170660 File Offset: 0x0016E860
	public NetPackageVehicleDataSync Setup(EntityVehicle _ev, int _senderId, ushort _syncFlags)
	{
		this.senderId = _senderId;
		this.vehicleId = _ev.entityId;
		this.syncFlags = _syncFlags;
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.entityData);
			_ev.WriteSyncData(pooledBinaryWriter, _syncFlags);
		}
		return this;
	}

	// Token: 0x060038A8 RID: 14504 RVA: 0x001706C4 File Offset: 0x0016E8C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackageVehicleDataSync()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.entityData);
	}

	// Token: 0x060038A9 RID: 14505 RVA: 0x001706FC File Offset: 0x0016E8FC
	public override void read(PooledBinaryReader _br)
	{
		this.senderId = _br.ReadInt32();
		this.vehicleId = _br.ReadInt32();
		this.syncFlags = _br.ReadUInt16();
		int length = (int)_br.ReadUInt16();
		StreamUtils.StreamCopy(_br.BaseStream, this.entityData, length, null, true);
	}

	// Token: 0x060038AA RID: 14506 RVA: 0x00170748 File Offset: 0x0016E948
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.senderId);
		_bw.Write(this.vehicleId);
		_bw.Write(this.syncFlags);
		_bw.Write((ushort)this.entityData.Length);
		this.entityData.WriteTo(_bw.BaseStream);
	}

	// Token: 0x060038AB RID: 14507 RVA: 0x001707A4 File Offset: 0x0016E9A4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.senderId, false))
		{
			return;
		}
		EntityVehicle entityVehicle = GameManager.Instance.World.GetEntity(this.vehicleId) as EntityVehicle;
		if (entityVehicle == null)
		{
			return;
		}
		if (this.entityData.Length > 0L)
		{
			PooledExpandableMemoryStream obj = this.entityData;
			lock (obj)
			{
				this.entityData.Position = 0L;
				try
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(this.entityData);
						entityVehicle.ReadSyncData(pooledBinaryReader, this.syncFlags, this.senderId);
					}
				}
				catch (Exception e)
				{
					Log.Exception(e);
					string str = "Error syncing data for entity ";
					EntityVehicle entityVehicle2 = entityVehicle;
					Log.Error(str + ((entityVehicle2 != null) ? entityVehicle2.ToString() : null) + "; Sender id = " + this.senderId.ToString());
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			ushort syncFlagsReplicated = entityVehicle.GetSyncFlagsReplicated(this.syncFlags);
			if (syncFlagsReplicated != 0)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageVehicleDataSync>().Setup(entityVehicle, this.senderId, syncFlagsReplicated), false, -1, this.senderId, -1, null, 192, false);
			}
		}
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x0017090C File Offset: 0x0016EB0C
	public override int GetLength()
	{
		return (int)(12L + this.entityData.Length);
	}

	// Token: 0x04002DDB RID: 11739
	[PublicizedFrom(EAccessModifier.Private)]
	public int senderId;

	// Token: 0x04002DDC RID: 11740
	[PublicizedFrom(EAccessModifier.Private)]
	public int vehicleId;

	// Token: 0x04002DDD RID: 11741
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort syncFlags;

	// Token: 0x04002DDE RID: 11742
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream entityData = MemoryPools.poolMemoryStream.AllocSync(true);
}
