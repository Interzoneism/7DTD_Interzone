using System;
using UnityEngine.Scripting;

// Token: 0x0200075D RID: 1885
[Preserve]
public class NetPackageMapPosition : NetPackage
{
	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x060036F9 RID: 14073 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x00168E0E File Offset: 0x0016700E
	public NetPackageMapPosition Setup(int _entityId, Vector2i _mapMiddlePosition)
	{
		this.entityId = _entityId;
		this.mapMiddlePosition = _mapMiddlePosition;
		return this;
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x00168E1F File Offset: 0x0016701F
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.mapMiddlePosition = StreamUtils.ReadVector2i(_br);
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x00168E39 File Offset: 0x00167039
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
		StreamUtils.Write(_bw, this.mapMiddlePosition);
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x00168E5C File Offset: 0x0016705C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.entityId, false))
		{
			return;
		}
		EntityPlayer entityPlayer = _world.GetEntity(this.entityId) as EntityPlayer;
		if (entityPlayer != null && entityPlayer.ChunkObserver.mapDatabase != null)
		{
			entityPlayer.ChunkObserver.mapDatabase.SetClientMapMiddlePosition(this.mapMiddlePosition);
		}
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002C89 RID: 11401
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C8A RID: 11402
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i mapMiddlePosition;
}
