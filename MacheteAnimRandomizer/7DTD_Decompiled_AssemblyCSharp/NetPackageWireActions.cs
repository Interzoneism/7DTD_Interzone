using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020007AF RID: 1967
[Preserve]
public class NetPackageWireActions : NetPackage
{
	// Token: 0x060038E7 RID: 14567 RVA: 0x00171493 File Offset: 0x0016F693
	public NetPackageWireActions Setup(NetPackageWireActions.WireActions _operation, Vector3i _tileEntityPosition, List<Vector3i> _wireChildren, int wiringEntity = -1)
	{
		this.currentOperation = _operation;
		this.tileEntityPosition = _tileEntityPosition;
		this.wireChildren = _wireChildren;
		return this;
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x001714AC File Offset: 0x0016F6AC
	public override void read(PooledBinaryReader _br)
	{
		this.currentOperation = (NetPackageWireActions.WireActions)_br.ReadByte();
		this.tileEntityPosition = StreamUtils.ReadVector3i(_br);
		int num = (int)_br.ReadByte();
		this.wireChildren.Clear();
		for (int i = 0; i < num; i++)
		{
			this.wireChildren.Add(StreamUtils.ReadVector3i(_br));
		}
		if (this.currentOperation != NetPackageWireActions.WireActions.SendWires)
		{
			this.wiringEntityID = _br.ReadInt32();
		}
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x00171518 File Offset: 0x0016F718
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.currentOperation);
		StreamUtils.Write(_bw, this.tileEntityPosition);
		_bw.Write((byte)this.wireChildren.Count);
		for (int i = 0; i < this.wireChildren.Count; i++)
		{
			StreamUtils.Write(_bw, this.wireChildren[i]);
		}
		if (this.currentOperation != NetPackageWireActions.WireActions.SendWires)
		{
			_bw.Write(this.wiringEntityID);
		}
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x00171594 File Offset: 0x0016F794
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			switch (this.currentOperation)
			{
			case NetPackageWireActions.WireActions.SetParent:
			{
				TileEntityPowered poweredTileEntity = this.GetPoweredTileEntity(_world, this.tileEntityPosition);
				ushort blockID = 0;
				PowerItem powerItem = PowerManager.Instance.GetPowerItemByWorldPos(poweredTileEntity.ToWorldPos());
				if (powerItem == null)
				{
					powerItem = poweredTileEntity.CreatePowerItemForTileEntity(blockID);
					poweredTileEntity.SetModified();
					powerItem.AddTileEntity(poweredTileEntity);
				}
				TileEntityPowered poweredTileEntity2 = this.GetPoweredTileEntity(_world, this.wireChildren[0]);
				PowerItem powerItem2 = PowerManager.Instance.GetPowerItemByWorldPos(poweredTileEntity2.ToWorldPos());
				if (powerItem2 == null)
				{
					powerItem2 = poweredTileEntity2.CreatePowerItemForTileEntity(blockID);
					poweredTileEntity2.SetModified();
					powerItem2.AddTileEntity(poweredTileEntity2);
				}
				PowerItem parent = powerItem.Parent;
				PowerManager.Instance.SetParent(powerItem, powerItem2);
				if (parent != null && parent.TileEntity != null)
				{
					parent.TileEntity.CreateWireDataFromPowerItem();
					parent.TileEntity.SendWireData();
					parent.TileEntity.RemoveWires();
					parent.TileEntity.DrawWires();
				}
				if (powerItem2.TileEntity != null)
				{
					powerItem2.TileEntity.CreateWireDataFromPowerItem();
					powerItem2.TileEntity.SendWireData();
					powerItem2.TileEntity.RemoveWires();
					powerItem2.TileEntity.DrawWires();
					return;
				}
				break;
			}
			case NetPackageWireActions.WireActions.RemoveParent:
			{
				PowerItem powerItem3 = this.GetPoweredTileEntity(_world, this.tileEntityPosition).GetPowerItem();
				if (powerItem3.Parent != null)
				{
					PowerItem parent2 = powerItem3.Parent;
					powerItem3.RemoveSelfFromParent();
					if (parent2.TileEntity != null)
					{
						parent2.TileEntity.CreateWireDataFromPowerItem();
						parent2.TileEntity.SendWireData();
						parent2.TileEntity.RemoveWires();
						parent2.TileEntity.DrawWires();
						return;
					}
				}
				break;
			}
			case NetPackageWireActions.WireActions.SendWires:
				break;
			default:
				return;
			}
		}
		else
		{
			Chunk chunk = _world.GetChunkFromWorldPos(this.tileEntityPosition.x, this.tileEntityPosition.y, this.tileEntityPosition.z) as Chunk;
			if (chunk != null)
			{
				IPowered powered = _world.GetTileEntity(chunk.ClrIdx, this.tileEntityPosition) as IPowered;
				if (this.currentOperation == NetPackageWireActions.WireActions.SendWires && powered != null)
				{
					powered.SetWireData(this.wireChildren);
				}
			}
		}
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x001717B4 File Offset: 0x0016F9B4
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowered GetPoweredTileEntity(World _world, Vector3i tileEntityPosition)
	{
		Chunk chunk = _world.GetChunkFromWorldPos(tileEntityPosition.x, tileEntityPosition.y, tileEntityPosition.z) as Chunk;
		TileEntityPowered tileEntityPowered = _world.GetTileEntity(chunk.ClrIdx, tileEntityPosition) as TileEntityPowered;
		if (tileEntityPowered == null)
		{
			Block block = _world.GetBlock(tileEntityPosition).Block;
			if (block is BlockPowered)
			{
				tileEntityPowered = (block as BlockPowered).CreateTileEntity(chunk);
			}
			tileEntityPowered.localChunkPos = World.toBlock(tileEntityPosition);
			BlockEntityData blockEntity = chunk.GetBlockEntity(tileEntityPosition);
			if (blockEntity != null)
			{
				tileEntityPowered.BlockTransform = blockEntity.transform;
			}
			tileEntityPowered.InitializePowerData();
			chunk.AddTileEntity(tileEntityPowered);
		}
		return tileEntityPowered;
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x001666F0 File Offset: 0x001648F0
	public override int GetLength()
	{
		return 12;
	}

	// Token: 0x04002DFC RID: 11772
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i tileEntityPosition;

	// Token: 0x04002DFD RID: 11773
	[PublicizedFrom(EAccessModifier.Private)]
	public int wiringEntityID;

	// Token: 0x04002DFE RID: 11774
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageWireActions.WireActions currentOperation;

	// Token: 0x04002DFF RID: 11775
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3i> wireChildren = new List<Vector3i>();

	// Token: 0x020007B0 RID: 1968
	public enum WireActions
	{
		// Token: 0x04002E01 RID: 11777
		SetParent,
		// Token: 0x04002E02 RID: 11778
		RemoveParent,
		// Token: 0x04002E03 RID: 11779
		SendWires
	}
}
