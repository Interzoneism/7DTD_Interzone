using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200073F RID: 1855
[Preserve]
public class NetPackageEntityRotation : NetPackage
{
	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x0600363B RID: 13883 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ReliableDelivery
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x001664E8 File Offset: 0x001646E8
	public NetPackageEntityRotation Setup(int _entityId, Vector3i _rot, Quaternion _qrot, bool _bUseQRot)
	{
		this.entityId = _entityId;
		this.rot = _rot;
		this.qrot = _qrot;
		this.bUseQRotation = _bUseQRot;
		return this;
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x00166508 File Offset: 0x00164708
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.bUseQRotation = _br.ReadBoolean();
		if (!this.bUseQRotation)
		{
			this.rot = new Vector3i((int)_br.ReadInt16(), (int)_br.ReadInt16(), (int)_br.ReadInt16());
			return;
		}
		this.qrot = new Quaternion(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
	}

	// Token: 0x0600363E RID: 13886 RVA: 0x00166578 File Offset: 0x00164778
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
		_bw.Write(this.bUseQRotation);
		if (!this.bUseQRotation)
		{
			_bw.Write((short)this.rot.x);
			_bw.Write((short)this.rot.y);
			_bw.Write((short)this.rot.z);
			return;
		}
		_bw.Write(this.qrot.x);
		_bw.Write(this.qrot.y);
		_bw.Write(this.qrot.z);
		_bw.Write(this.qrot.w);
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x0016662C File Offset: 0x0016482C
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
		Entity entity = _world.GetEntity(this.entityId);
		if (entity == null)
		{
			return;
		}
		Entity attachedMainEntity = entity.AttachedMainEntity;
		if (attachedMainEntity != null && _world.GetPrimaryPlayerId() == attachedMainEntity.entityId)
		{
			return;
		}
		if (this.bUseQRotation)
		{
			entity.SetQRotFromNetwork(this.qrot, 3);
			return;
		}
		Vector3 vector = new Vector3((float)(this.rot.x * 360) / 256f, (float)(this.rot.y * 360) / 256f, (float)(this.rot.z * 360) / 256f);
		entity.SetRotFromNetwork(vector, 3);
	}

	// Token: 0x06003640 RID: 13888 RVA: 0x001666F0 File Offset: 0x001648F0
	public override int GetLength()
	{
		return 12;
	}

	// Token: 0x04002C0E RID: 11278
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityId;

	// Token: 0x04002C0F RID: 11279
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3i rot;

	// Token: 0x04002C10 RID: 11280
	[PublicizedFrom(EAccessModifier.Protected)]
	public Quaternion qrot;

	// Token: 0x04002C11 RID: 11281
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bUseQRotation;
}
