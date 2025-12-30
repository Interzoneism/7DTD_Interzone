using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000731 RID: 1841
[Preserve]
public class NetPackageEntityAddVelocity : NetPackage
{
	// Token: 0x060035DF RID: 13791 RVA: 0x0016523F File Offset: 0x0016343F
	public NetPackageEntityAddVelocity Setup(int _entityId, Vector3 _addVelocity)
	{
		this.entityId = _entityId;
		this.addVelocity = _addVelocity;
		return this;
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x00165250 File Offset: 0x00163450
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.addVelocity = StreamUtils.ReadVector3(_br);
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x0016526A File Offset: 0x0016346A
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
		StreamUtils.Write(_bw, this.addVelocity);
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x0016528B File Offset: 0x0016348B
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().AddVelocityToEntityServer(this.entityId, this.addVelocity);
	}

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x060035E3 RID: 13795 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002BD4 RID: 11220
	public int entityId;

	// Token: 0x04002BD5 RID: 11221
	public Vector3 addVelocity;
}
