using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000AFA RID: 2810
public class TileEntityGoreBlock : TileEntityLootContainer
{
	// Token: 0x06005692 RID: 22162 RVA: 0x00234AB1 File Offset: 0x00232CB1
	public TileEntityGoreBlock(Chunk _chunk) : base(_chunk)
	{
		this.tickTimeToRemove = GameTimer.Instance.ticks + 60000UL;
	}

	// Token: 0x06005693 RID: 22163 RVA: 0x00234AD1 File Offset: 0x00232CD1
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.GoreBlock;
	}

	// Token: 0x06005694 RID: 22164 RVA: 0x00234AD5 File Offset: 0x00232CD5
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		if (GameTimer.Instance.ticks > this.tickTimeToRemove)
		{
			ThreadManager.StartCoroutine(this.destroyBlockLater(world));
		}
	}

	// Token: 0x06005695 RID: 22165 RVA: 0x00234AFD File Offset: 0x00232CFD
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator destroyBlockLater(World world)
	{
		yield return new WaitForEndOfFrame();
		world.SetBlockRPC(base.ToWorldPos(), BlockValue.Air);
		yield break;
	}

	// Token: 0x06005696 RID: 22166 RVA: 0x00234B13 File Offset: 0x00232D13
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.tickTimeToRemove = _br.ReadUInt64();
		if (this.readVersion < 4)
		{
			this.tickTimeToRemove += 60000UL;
		}
	}

	// Token: 0x06005697 RID: 22167 RVA: 0x00234B45 File Offset: 0x00232D45
	public override void write(PooledBinaryWriter stream, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(stream, _eStreamMode);
		stream.Write(this.tickTimeToRemove);
	}

	// Token: 0x040042FA RID: 17146
	public ulong tickTimeToRemove;
}
