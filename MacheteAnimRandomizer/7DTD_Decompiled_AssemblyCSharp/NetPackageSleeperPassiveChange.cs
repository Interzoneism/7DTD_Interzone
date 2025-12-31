using System;
using UnityEngine.Scripting;

// Token: 0x02000797 RID: 1943
[Preserve]
public class NetPackageSleeperPassiveChange : NetPackage
{
	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x06003853 RID: 14419 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x0016F532 File Offset: 0x0016D732
	public NetPackageSleeperPassiveChange Setup(int targetId)
	{
		this.m_targetId = targetId;
		return this;
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x0016F53C File Offset: 0x0016D73C
	public override void read(PooledBinaryReader _reader)
	{
		this.m_targetId = _reader.ReadInt32();
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x0016F54A File Offset: 0x0016D74A
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_targetId);
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x0016F560 File Offset: 0x0016D760
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null || !_world.IsRemote())
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.m_targetId) as EntityAlive;
		if (entityAlive == null)
		{
			return;
		}
		entityAlive.IsSleeperPassive = false;
	}

	// Token: 0x06003858 RID: 14424 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002DAC RID: 11692
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_targetId;
}
