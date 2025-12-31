using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x0200013C RID: 316
[Preserve]
public class BlockSpeaker : BlockPowered
{
	// Token: 0x060008D6 RID: 2262 RVA: 0x0003DEAC File Offset: 0x0003C0AC
	public BlockSpeaker()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0003DEC6 File Offset: 0x0003C0C6
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("PlaySound"))
		{
			this.playSound = base.Properties.Values["PlaySound"];
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0003DF00 File Offset: 0x0003C100
	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		byte b = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
		if (_blockValue.meta != b)
		{
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
			try
			{
				if (isOn)
				{
					Manager.BroadcastPlay(_blockPos.ToVector3(), this.playSound, 0f);
				}
				else
				{
					Manager.BroadcastStop(_blockPos.ToVector3(), this.playSound);
				}
			}
			catch (Exception)
			{
			}
		}
		return true;
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0003DF98 File Offset: 0x0003C198
	public override void OnBlockUnloaded(WorldBase world, int clrIdx, Vector3i blockPos, BlockValue blockValue)
	{
		base.OnBlockUnloaded(world, clrIdx, blockPos, blockValue);
		Manager.Stop(blockPos.ToVector3(), this.playSound);
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x0003DFB7 File Offset: 0x0003C1B7
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		Manager.Stop(_blockPos.ToVector3(), this.playSound);
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0003DFD8 File Offset: 0x0003C1D8
	public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		if ((TileEntityPoweredBlock)_world.GetTileEntity(_clrIdx, _blockPos) != null)
		{
			if ((_blockValue.meta & 2) > 0)
			{
				Manager.Play(_blockPos.ToVector3(), this.playSound, -1, false);
				return;
			}
			Manager.Stop(_blockPos.ToVector3(), this.playSound);
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0003E035 File Offset: 0x0003C235
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.Consumer
		};
	}

	// Token: 0x040008AC RID: 2220
	[PublicizedFrom(EAccessModifier.Private)]
	public string playSound;

	// Token: 0x040008AD RID: 2221
	[PublicizedFrom(EAccessModifier.Private)]
	public float soundDelay = 1f;
}
