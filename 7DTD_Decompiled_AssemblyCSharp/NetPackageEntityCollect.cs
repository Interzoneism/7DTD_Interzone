using System;
using UnityEngine.Scripting;

// Token: 0x02000735 RID: 1845
[Preserve]
public class NetPackageEntityCollect : NetPackage
{
	// Token: 0x060035FC RID: 13820 RVA: 0x00165757 File Offset: 0x00163957
	public NetPackageEntityCollect Setup(int _entityId, int _playerId)
	{
		this.entityId = _entityId;
		this.playerId = _playerId;
		return this;
	}

	// Token: 0x060035FD RID: 13821 RVA: 0x00165768 File Offset: 0x00163968
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.playerId = _br.ReadInt32();
	}

	// Token: 0x060035FE RID: 13822 RVA: 0x00165782 File Offset: 0x00163982
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
		_bw.Write(this.playerId);
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x001657A4 File Offset: 0x001639A4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.playerId, false))
		{
			return;
		}
		if (!_world.IsRemote())
		{
			_world.GetGameManager().CollectEntityServer(this.entityId, this.playerId);
			return;
		}
		_world.GetGameManager().CollectEntityClient(this.entityId, this.playerId);
	}

	// Token: 0x06003600 RID: 13824 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002BE7 RID: 11239
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002BE8 RID: 11240
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerId;
}
