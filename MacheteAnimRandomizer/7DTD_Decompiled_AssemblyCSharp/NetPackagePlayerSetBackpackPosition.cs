using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000778 RID: 1912
[Preserve]
public class NetPackagePlayerSetBackpackPosition : NetPackage
{
	// Token: 0x0600379D RID: 14237 RVA: 0x0016B74E File Offset: 0x0016994E
	public NetPackagePlayerSetBackpackPosition Setup(int _playerId, List<Vector3i> _positions)
	{
		this.playerId = _playerId;
		this.positions = _positions;
		return this;
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x0016B760 File Offset: 0x00169960
	public override void read(PooledBinaryReader _br)
	{
		this.playerId = _br.ReadInt32();
		int num = (int)_br.ReadByte();
		this.positions = new List<Vector3i>();
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.positions.Add(StreamUtils.ReadVector3i(_br));
			}
		}
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x0016B7AC File Offset: 0x001699AC
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.playerId);
		if (this.positions == null)
		{
			_bw.Write(0);
			return;
		}
		_bw.Write((byte)this.positions.Count);
		for (int i = 0; i < this.positions.Count; i++)
		{
			StreamUtils.Write(_bw, this.positions[i]);
		}
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x0016B818 File Offset: 0x00169A18
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (GameManager.Instance.World == null)
		{
			return;
		}
		EntityPlayerLocal entityPlayerLocal = GameManager.Instance.World.GetEntity(this.playerId) as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.SetDroppedBackpackPositions(this.positions);
		}
	}

	// Token: 0x170005A9 RID: 1449
	// (get) Token: 0x060037A1 RID: 14241 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002D06 RID: 11526
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerId;

	// Token: 0x04002D07 RID: 11527
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3i> positions;
}
