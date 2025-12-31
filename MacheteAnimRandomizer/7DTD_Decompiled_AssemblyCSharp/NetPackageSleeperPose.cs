using System;
using UnityEngine.Scripting;

// Token: 0x02000798 RID: 1944
[Preserve]
public class NetPackageSleeperPose : NetPackage
{
	// Token: 0x0600385A RID: 14426 RVA: 0x0016F59C File Offset: 0x0016D79C
	public NetPackageSleeperPose Setup(int targetId, byte pose)
	{
		this.m_targetId = targetId;
		this.m_pose = pose;
		return this;
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x0016F5AD File Offset: 0x0016D7AD
	public override void read(PooledBinaryReader _reader)
	{
		this.m_targetId = _reader.ReadInt32();
		this.m_pose = _reader.ReadByte();
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x0016F5C7 File Offset: 0x0016D7C7
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_targetId);
		_writer.Write(this.m_pose);
	}

	// Token: 0x0600385D RID: 14429 RVA: 0x0016F5E8 File Offset: 0x0016D7E8
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
		entityAlive.TriggerSleeperPose((int)this.m_pose, false);
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002DAD RID: 11693
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_targetId;

	// Token: 0x04002DAE RID: 11694
	[PublicizedFrom(EAccessModifier.Private)]
	public byte m_pose;
}
