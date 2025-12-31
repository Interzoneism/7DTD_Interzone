using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000736 RID: 1846
[Preserve]
public class NetPackageEntityLookAt : NetPackage
{
	// Token: 0x17000578 RID: 1400
	// (get) Token: 0x06003602 RID: 13826 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003603 RID: 13827 RVA: 0x001657FC File Offset: 0x001639FC
	public NetPackageEntityLookAt Setup(int _entityId, Vector3 _lookAtPosition)
	{
		this.entityId = _entityId;
		this.lookAtPosition = _lookAtPosition;
		return this;
	}

	// Token: 0x06003604 RID: 13828 RVA: 0x0016580D File Offset: 0x00163A0D
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.lookAtPosition = new Vector3((float)_reader.ReadInt32(), (float)_reader.ReadInt32(), (float)_reader.ReadInt32());
	}

	// Token: 0x06003605 RID: 13829 RVA: 0x0016583C File Offset: 0x00163A3C
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write((int)this.lookAtPosition.x);
		_writer.Write((int)this.lookAtPosition.y);
		_writer.Write((int)this.lookAtPosition.z);
	}

	// Token: 0x06003606 RID: 13830 RVA: 0x00165894 File Offset: 0x00163A94
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)_world.GetEntity(this.entityId);
		if (entityAlive != null && entityAlive.emodel != null && entityAlive.emodel.avatarController != null)
		{
			entityAlive.emodel.avatarController.SetLookPosition(this.lookAtPosition);
		}
	}

	// Token: 0x06003607 RID: 13831 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002BE9 RID: 11241
	public int entityId;

	// Token: 0x04002BEA RID: 11242
	public Vector3 lookAtPosition;
}
