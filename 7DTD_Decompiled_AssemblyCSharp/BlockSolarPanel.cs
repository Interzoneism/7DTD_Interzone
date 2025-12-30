using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000129 RID: 297
[Preserve]
public class BlockSolarPanel : BlockPowerSource
{
	// Token: 0x06000848 RID: 2120 RVA: 0x0003A035 File Offset: 0x00038235
	public override TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		if (this.slotItem == null)
		{
			this.slotItem = ItemClass.GetItemClass(this.SlotItemName, false);
		}
		return new TileEntityPowerSource(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.SolarPanel,
			SlotItem = this.slotItem
		};
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0003A06C File Offset: 0x0003826C
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (!base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		Vector3i blockPos = _blockPos + Vector3i.up;
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		return chunkCluster == null || chunkCluster.GetLight(blockPos, Chunk.LIGHT_TYPE.SUN) >= 15;
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0003A0B5 File Offset: 0x000382B5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetPowerSourceIcon()
	{
		return "electric_solar";
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0003A0BC File Offset: 0x000382BC
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSound);
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0003A0DB File Offset: 0x000382DB
	public override void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
		Manager.Stop(_blockPos.ToVector3(), this.runningSound);
	}

	// Token: 0x0400086D RID: 2157
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSound = "solarpanel_idle";
}
