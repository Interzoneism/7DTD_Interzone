using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200073D RID: 1853
[Preserve]
public class NetPackageEntityRelPosAndRot : NetPackageEntityRotation
{
	// Token: 0x1700057C RID: 1404
	// (get) Token: 0x0600362D RID: 13869 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ReliableDelivery
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600362E RID: 13870 RVA: 0x001662A6 File Offset: 0x001644A6
	public NetPackageEntityRelPosAndRot Setup(int _entityId, Vector3i _deltaPos, Vector3i _absRot, Quaternion _qrot, bool _onGround, bool _bUseQRot, int _updateSteps)
	{
		base.Setup(_entityId, _absRot, _qrot, _bUseQRot);
		this.dPos = _deltaPos;
		this.onGround = _onGround;
		this.updateSteps = (short)_updateSteps;
		return this;
	}

	// Token: 0x0600362F RID: 13871 RVA: 0x001662CE File Offset: 0x001644CE
	public override void read(PooledBinaryReader _reader)
	{
		base.read(_reader);
		this.dPos = new Vector3i((int)_reader.ReadInt16(), (int)_reader.ReadInt16(), (int)_reader.ReadInt16());
		this.onGround = _reader.ReadBoolean();
		this.updateSteps = _reader.ReadInt16();
	}

	// Token: 0x06003630 RID: 13872 RVA: 0x0016630C File Offset: 0x0016450C
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((short)this.dPos.x);
		_writer.Write((short)this.dPos.y);
		_writer.Write((short)this.dPos.z);
		_writer.Write(this.onGround);
		_writer.Write(this.updateSteps);
	}

	// Token: 0x06003631 RID: 13873 RVA: 0x00166370 File Offset: 0x00164570
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
		entity.serverPos += this.dPos;
		Vector3 pos = entity.serverPos.ToVector3() / 32f;
		Vector3 rot = new Vector3((float)(this.rot.x * 360) / 256f, (float)(this.rot.y * 360) / 256f, (float)(this.rot.z * 360) / 256f);
		if (this.bUseQRotation)
		{
			entity.SetPosAndQRotFromNetwork(pos, this.qrot, (int)this.updateSteps);
		}
		else
		{
			entity.SetPosAndRotFromNetwork(pos, rot, (int)this.updateSteps);
		}
		entity.onGround = this.onGround;
	}

	// Token: 0x06003632 RID: 13874 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C09 RID: 11273
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i dPos;

	// Token: 0x04002C0A RID: 11274
	[PublicizedFrom(EAccessModifier.Private)]
	public bool onGround;

	// Token: 0x04002C0B RID: 11275
	[PublicizedFrom(EAccessModifier.Private)]
	public short updateSteps;
}
