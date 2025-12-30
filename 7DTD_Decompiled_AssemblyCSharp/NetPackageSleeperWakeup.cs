using System;
using UnityEngine.Scripting;

// Token: 0x02000799 RID: 1945
[Preserve]
public class NetPackageSleeperWakeup : NetPackage
{
	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x06003860 RID: 14432 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x0016F62A File Offset: 0x0016D82A
	public NetPackageSleeperWakeup Setup(int targetId)
	{
		this.m_targetId = targetId;
		return this;
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x0016F634 File Offset: 0x0016D834
	public override void read(PooledBinaryReader _reader)
	{
		this.m_targetId = _reader.ReadInt32();
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x0016F642 File Offset: 0x0016D842
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_targetId);
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x0016F658 File Offset: 0x0016D858
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
		entityAlive.ConditionalTriggerSleeperWakeUp();
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002DAF RID: 11695
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_targetId;
}
