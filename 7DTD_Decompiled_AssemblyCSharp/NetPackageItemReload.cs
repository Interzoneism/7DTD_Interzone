using System;
using UnityEngine.Scripting;

// Token: 0x02000756 RID: 1878
[Preserve]
public class NetPackageItemReload : NetPackage
{
	// Token: 0x060036CA RID: 14026 RVA: 0x00168774 File Offset: 0x00166974
	public NetPackageItemReload Setup(int _entityId)
	{
		this.entityId = _entityId;
		return this;
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x0016877E File Offset: 0x0016697E
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x0016878C File Offset: 0x0016698C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x001687A1 File Offset: 0x001669A1
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			_world.GetGameManager().ItemReloadServer(this.entityId);
			return;
		}
		_world.GetGameManager().ItemReloadClient(this.entityId);
	}

	// Token: 0x060036CE RID: 14030 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002C7B RID: 11387
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;
}
