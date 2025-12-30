using System;
using UnityEngine.Scripting;

// Token: 0x0200078B RID: 1931
[Preserve]
public class NetPackageSetAttackTarget : NetPackage
{
	// Token: 0x06003815 RID: 14357 RVA: 0x0016E53C File Offset: 0x0016C73C
	public NetPackageSetAttackTarget Setup(int entityId, int targetId)
	{
		this.m_entityId = entityId;
		this.m_targetId = targetId;
		return this;
	}

	// Token: 0x06003816 RID: 14358 RVA: 0x0016E54D File Offset: 0x0016C74D
	public override void read(PooledBinaryReader _reader)
	{
		this.m_entityId = _reader.ReadInt32();
		this.m_targetId = _reader.ReadInt32();
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x0016E567 File Offset: 0x0016C767
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_entityId);
		_writer.Write(this.m_targetId);
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x0016E588 File Offset: 0x0016C788
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.m_entityId) as EntityAlive;
		if (entityAlive == null)
		{
			return;
		}
		EntityAlive attackTargetClient = _world.GetEntity(this.m_targetId) as EntityAlive;
		entityAlive.SetAttackTargetClient(attackTargetClient);
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002D81 RID: 11649
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_targetId;

	// Token: 0x04002D82 RID: 11650
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_entityId;
}
