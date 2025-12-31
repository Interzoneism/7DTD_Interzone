using System;
using UnityEngine.Scripting;

// Token: 0x020007A6 RID: 1958
[Preserve]
public class NetPackageEntityAttach : NetPackage
{
	// Token: 0x060038AE RID: 14510 RVA: 0x00170937 File Offset: 0x0016EB37
	public NetPackageEntityAttach Setup(NetPackageEntityAttach.AttachType _attachType, int _riderId, int _vehicleId, int _slot)
	{
		this.attachType = _attachType;
		this.riderId = _riderId;
		this.vehicleId = _vehicleId;
		this.slot = _slot;
		return this;
	}

	// Token: 0x060038AF RID: 14511 RVA: 0x00170957 File Offset: 0x0016EB57
	public override void read(PooledBinaryReader _br)
	{
		this.attachType = (NetPackageEntityAttach.AttachType)_br.ReadByte();
		this.riderId = _br.ReadInt32();
		this.vehicleId = _br.ReadInt32();
		this.slot = (int)_br.ReadInt16();
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x00170989 File Offset: 0x0016EB89
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.attachType);
		_bw.Write(this.riderId);
		_bw.Write(this.vehicleId);
		_bw.Write((short)this.slot);
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x001709C4 File Offset: 0x0016EBC4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Entity entity = GameManager.Instance.World.GetEntity(this.riderId);
		if (entity == null)
		{
			return;
		}
		Entity entity2 = GameManager.Instance.World.GetEntity(this.vehicleId);
		switch (this.attachType)
		{
		case NetPackageEntityAttach.AttachType.AttachServer:
		{
			if (entity2 == null)
			{
				return;
			}
			int num = entity2.FindAttachSlot(entity);
			if (num < 0)
			{
				num = entity.AttachToEntity(entity2, this.slot);
			}
			if (num >= 0)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.AttachClient, this.riderId, this.vehicleId, num), false, -1, -1, -1, null, 192, false);
				return;
			}
			break;
		}
		case NetPackageEntityAttach.AttachType.AttachClient:
			if (entity2 == null)
			{
				return;
			}
			entity.AttachToEntity(entity2, this.slot);
			return;
		case NetPackageEntityAttach.AttachType.DetachServer:
			entity.Detach();
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.DetachClient, this.riderId, -1, -1), false, -1, this.riderId, -1, null, 192, false);
			return;
		case NetPackageEntityAttach.AttachType.DetachClient:
			entity.Detach();
			break;
		default:
			return;
		}
	}

	// Token: 0x060038B2 RID: 14514 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002DDF RID: 11743
	[PublicizedFrom(EAccessModifier.Private)]
	public int riderId;

	// Token: 0x04002DE0 RID: 11744
	[PublicizedFrom(EAccessModifier.Private)]
	public int vehicleId;

	// Token: 0x04002DE1 RID: 11745
	[PublicizedFrom(EAccessModifier.Private)]
	public int slot;

	// Token: 0x04002DE2 RID: 11746
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEntityAttach.AttachType attachType;

	// Token: 0x020007A7 RID: 1959
	public enum AttachType : byte
	{
		// Token: 0x04002DE4 RID: 11748
		AttachServer,
		// Token: 0x04002DE5 RID: 11749
		AttachClient,
		// Token: 0x04002DE6 RID: 11750
		DetachServer,
		// Token: 0x04002DE7 RID: 11751
		DetachClient
	}
}
