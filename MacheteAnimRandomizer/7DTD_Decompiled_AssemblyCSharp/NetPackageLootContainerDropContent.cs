using System;
using UnityEngine.Scripting;

// Token: 0x0200075B RID: 1883
[Preserve]
public class NetPackageLootContainerDropContent : NetPackage
{
	// Token: 0x060036E9 RID: 14057 RVA: 0x00168B47 File Offset: 0x00166D47
	public NetPackageLootContainerDropContent Setup(Vector3i _worldPos, int _lootEntityId)
	{
		this.worldPos = _worldPos;
		this.lootEntityId = _lootEntityId;
		return this;
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x00168B58 File Offset: 0x00166D58
	public override void read(PooledBinaryReader _br)
	{
		this.worldPos = StreamUtils.ReadVector3i(_br);
		this.lootEntityId = _br.ReadInt32();
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x00168B72 File Offset: 0x00166D72
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		StreamUtils.Write(_bw, this.worldPos);
		_bw.Write(this.lootEntityId);
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x00168B93 File Offset: 0x00166D93
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().DropContentOfLootContainerServer(_world.GetBlock(this.worldPos), this.worldPos, this.lootEntityId, null);
	}

	// Token: 0x17000591 RID: 1425
	// (get) Token: 0x060036ED RID: 14061 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002C82 RID: 11394
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i worldPos;

	// Token: 0x04002C83 RID: 11395
	[PublicizedFrom(EAccessModifier.Private)]
	public int lootEntityId;

	// Token: 0x04002C84 RID: 11396
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] items;

	// Token: 0x04002C85 RID: 11397
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldBlockType = -1;
}
