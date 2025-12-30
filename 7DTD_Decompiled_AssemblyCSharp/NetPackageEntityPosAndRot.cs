using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200073A RID: 1850
[Preserve]
public class NetPackageEntityPosAndRot : NetPackage
{
	// Token: 0x1700057A RID: 1402
	// (get) Token: 0x06003618 RID: 13848 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ReliableDelivery
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06003619 RID: 13849 RVA: 0x00165DB8 File Offset: 0x00163FB8
	public NetPackageEntityPosAndRot Setup(Entity _entity)
	{
		this.entityId = _entity.entityId;
		this.pos = _entity.position;
		this.rot = _entity.rotation;
		this.onGround = _entity.onGround;
		this.qrot = _entity.qrotation;
		this.bUseQRotation = _entity.IsQRotationUsed();
		return this;
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x00165E10 File Offset: 0x00164010
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.pos = new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
		this.bUseQRotation = _br.ReadBoolean();
		if (!this.bUseQRotation)
		{
			this.rot = new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
		}
		else
		{
			this.qrot = new Quaternion(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
		}
		this.onGround = _br.ReadBoolean();
	}

	// Token: 0x0600361B RID: 13851 RVA: 0x00165EA8 File Offset: 0x001640A8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
		_bw.Write(this.pos.x);
		_bw.Write(this.pos.y);
		_bw.Write(this.pos.z);
		_bw.Write(this.bUseQRotation);
		if (!this.bUseQRotation)
		{
			_bw.Write(this.rot.x);
			_bw.Write(this.rot.y);
			_bw.Write(this.rot.z);
		}
		else
		{
			_bw.Write(this.qrot.x);
			_bw.Write(this.qrot.y);
			_bw.Write(this.qrot.z);
			_bw.Write(this.qrot.w);
		}
		_bw.Write(this.onGround);
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x00165FA0 File Offset: 0x001641A0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.entityId, true))
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
		entity.serverPos = NetEntityDistributionEntry.EncodePos(this.pos);
		if (this.bUseQRotation)
		{
			entity.SetPosAndQRotFromNetwork(this.pos, this.qrot, 3);
		}
		else
		{
			entity.SetPosAndRotFromNetwork(this.pos, this.rot, 3);
		}
		entity.onGround = this.onGround;
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x000445B9 File Offset: 0x000427B9
	public override int GetLength()
	{
		return 25;
	}

	// Token: 0x04002BFB RID: 11259
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityId;

	// Token: 0x04002BFC RID: 11260
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 pos;

	// Token: 0x04002BFD RID: 11261
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 rot;

	// Token: 0x04002BFE RID: 11262
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool onGround;

	// Token: 0x04002BFF RID: 11263
	[PublicizedFrom(EAccessModifier.Protected)]
	public Quaternion qrot;

	// Token: 0x04002C00 RID: 11264
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bUseQRotation;
}
