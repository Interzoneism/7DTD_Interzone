using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000131 RID: 305
[Preserve]
public class BlockSecure : Block
{
	// Token: 0x06000891 RID: 2193 RVA: 0x0003B730 File Offset: 0x00039930
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0003B738 File Offset: 0x00039938
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntitySecure tileEntitySecure = (TileEntitySecure)_world.GetTileEntity(_result.clrIdx, _result.blockPos);
		if (tileEntitySecure == null)
		{
			return;
		}
		tileEntitySecure.SetOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0003B778 File Offset: 0x00039978
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.GetActivationText(_world, block, _clrIdx, parentPos, _entityFocusing);
		}
		TileEntitySecureDoor tileEntitySecureDoor = (TileEntitySecureDoor)_world.GetTileEntity(_clrIdx, _blockPos);
		if (tileEntitySecureDoor == null)
		{
			return "";
		}
		if (_blockValue.Block.HasTag(BlockTags.Window))
		{
			return null;
		}
		if (tileEntitySecureDoor.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
		{
			return Localization.Get("useSecureDoor", false);
		}
		return Localization.Get("noSecureDoorAccess", false);
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsWaterBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		return true;
	}
}
