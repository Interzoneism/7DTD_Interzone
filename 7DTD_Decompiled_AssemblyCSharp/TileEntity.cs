using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AF4 RID: 2804
public abstract class TileEntity : ITileEntity
{
	// Token: 0x17000888 RID: 2184
	// (get) Token: 0x06005624 RID: 22052 RVA: 0x002322E1 File Offset: 0x002304E1
	public int EntityId
	{
		get
		{
			return this.entityId;
		}
	}

	// Token: 0x17000889 RID: 2185
	// (get) Token: 0x06005625 RID: 22053 RVA: 0x002322E9 File Offset: 0x002304E9
	// (set) Token: 0x06005626 RID: 22054 RVA: 0x002322F1 File Offset: 0x002304F1
	public Vector3i localChunkPos
	{
		get
		{
			return this.chunkPos;
		}
		set
		{
			this.chunkPos = value;
			this.OnSetLocalChunkPosition();
		}
	}

	// Token: 0x1700088A RID: 2186
	// (get) Token: 0x06005627 RID: 22055 RVA: 0x00232300 File Offset: 0x00230500
	public BlockValue blockValue
	{
		get
		{
			return this.chunk.GetBlock(this.localChunkPos);
		}
	}

	// Token: 0x1700088B RID: 2187
	// (get) Token: 0x06005628 RID: 22056 RVA: 0x00232313 File Offset: 0x00230513
	// (set) Token: 0x06005629 RID: 22057 RVA: 0x0023231B File Offset: 0x0023051B
	public bool IsRemoving { get; set; }

	// Token: 0x14000083 RID: 131
	// (add) Token: 0x0600562A RID: 22058 RVA: 0x00232324 File Offset: 0x00230524
	// (remove) Token: 0x0600562B RID: 22059 RVA: 0x0023235C File Offset: 0x0023055C
	public event XUiEvent_TileEntityDestroyed Destroyed;

	// Token: 0x1700088C RID: 2188
	// (get) Token: 0x0600562C RID: 22060 RVA: 0x00232391 File Offset: 0x00230591
	public List<ITileEntityChangedListener> listeners
	{
		get
		{
			return this._listeners;
		}
	}

	// Token: 0x1700088D RID: 2189
	// (get) Token: 0x0600562D RID: 22061 RVA: 0x00232399 File Offset: 0x00230599
	public bool bWaitingForServerResponse
	{
		get
		{
			return this.lockHandleWaitingFor != byte.MaxValue;
		}
	}

	// Token: 0x0600562E RID: 22062 RVA: 0x002323AB File Offset: 0x002305AB
	public TileEntity(Chunk _chunk)
	{
		this.chunk = _chunk;
	}

	// Token: 0x0600562F RID: 22063 RVA: 0x002323D7 File Offset: 0x002305D7
	public virtual TileEntity Clone()
	{
		throw new NotImplementedException("Clone() not implemented yet");
	}

	// Token: 0x06005630 RID: 22064 RVA: 0x002323E3 File Offset: 0x002305E3
	public virtual void CopyFrom(TileEntity _other)
	{
		throw new NotImplementedException("CopyFrom() not implemented yet");
	}

	// Token: 0x06005631 RID: 22065 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UpdateTick(World world)
	{
	}

	// Token: 0x06005632 RID: 22066
	public abstract TileEntityType GetTileEntityType();

	// Token: 0x06005633 RID: 22067 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnSetLocalChunkPosition()
	{
	}

	// Token: 0x06005634 RID: 22068 RVA: 0x002323F0 File Offset: 0x002305F0
	public Vector3i ToWorldPos()
	{
		if (this.chunk != null)
		{
			return new Vector3i(this.chunk.X * 16, this.chunk.Y * 256, this.chunk.Z * 16) + this.localChunkPos;
		}
		return Vector3i.zero;
	}

	// Token: 0x06005635 RID: 22069 RVA: 0x00232448 File Offset: 0x00230648
	public Vector3 ToWorldCenterPos()
	{
		if (this.entityId != -1)
		{
			Entity entity = GameManager.Instance.World.GetEntity(this.entityId);
			if (entity)
			{
				return entity.position;
			}
		}
		if (this.chunk != null)
		{
			BlockValue blockNoDamage = this.chunk.GetBlockNoDamage(this.chunkPos.x, this.chunkPos.y, this.chunkPos.z);
			Block block = blockNoDamage.Block;
			Vector3 vector;
			vector.x = (float)(this.chunk.X * 16 + this.chunkPos.x);
			vector.y = (float)(this.chunk.Y * 256 + this.chunkPos.y);
			vector.z = (float)(this.chunk.Z * 16 + this.chunkPos.z);
			if (!block.isMultiBlock)
			{
				vector.x += 0.5f;
				vector.y += 0.5f;
				vector.z += 0.5f;
			}
			else
			{
				BlockShapeModelEntity blockShapeModelEntity = block.shape as BlockShapeModelEntity;
				if (blockShapeModelEntity != null)
				{
					Quaternion rotation = blockShapeModelEntity.GetRotation(blockNoDamage);
					vector += blockShapeModelEntity.GetRotatedOffset(block, rotation);
					vector.x += 0.5f;
					vector.z += 0.5f;
				}
			}
			return vector;
		}
		return Vector3.zero;
	}

	// Token: 0x06005636 RID: 22070 RVA: 0x002325B9 File Offset: 0x002307B9
	public int GetClrIdx()
	{
		if (this.chunk == null)
		{
			return 0;
		}
		return this.chunk.ClrIdx;
	}

	// Token: 0x06005637 RID: 22071 RVA: 0x002325D0 File Offset: 0x002307D0
	public Chunk GetChunk()
	{
		return this.chunk;
	}

	// Token: 0x06005638 RID: 22072 RVA: 0x002325D8 File Offset: 0x002307D8
	public void SetChunk(Chunk _chunk)
	{
		this.chunk = _chunk;
	}

	// Token: 0x06005639 RID: 22073 RVA: 0x002325E4 File Offset: 0x002307E4
	public static TileEntity Instantiate(TileEntityType type, Chunk _chunk)
	{
		switch (type)
		{
		case TileEntityType.DewCollector:
			return new TileEntityDewCollector(_chunk);
		case TileEntityType.LandClaim:
			return new TileEntityLandClaim(_chunk);
		case TileEntityType.Loot:
			return new TileEntityLootContainer(_chunk);
		case TileEntityType.Trader:
			return new TileEntityTrader(_chunk);
		case TileEntityType.VendingMachine:
			return new TileEntityVendingMachine(_chunk);
		case TileEntityType.Forge:
			return new TileEntityForge(_chunk);
		case TileEntityType.SecureLoot:
			return new TileEntitySecureLootContainer(_chunk);
		case TileEntityType.SecureDoor:
			return new TileEntitySecureDoor(_chunk);
		case TileEntityType.Workstation:
			return new TileEntityWorkstation(_chunk);
		case TileEntityType.Sign:
			return new TileEntitySign(_chunk);
		case TileEntityType.GoreBlock:
			return new TileEntityGoreBlock(_chunk);
		case TileEntityType.Powered:
			return new TileEntityPoweredBlock(_chunk);
		case TileEntityType.PowerSource:
			return new TileEntityPowerSource(_chunk);
		case TileEntityType.PowerRangeTrap:
			return new TileEntityPoweredRangedTrap(_chunk);
		case TileEntityType.Light:
			return new TileEntityLight(_chunk);
		case TileEntityType.Trigger:
			return new TileEntityPoweredTrigger(_chunk);
		case TileEntityType.Sleeper:
			return new TileEntitySleeper(_chunk);
		case TileEntityType.PowerMeleeTrap:
			return new TileEntityPoweredMeleeTrap(_chunk);
		case TileEntityType.SecureLootSigned:
			return new TileEntitySecureLootContainerSigned(_chunk);
		case TileEntityType.Composite:
			return new TileEntityComposite(_chunk);
		}
		Log.Warning("Dropping TE with unknown type: " + type.ToStringCached<TileEntityType>());
		return null;
	}

	// Token: 0x0600563A RID: 22074 RVA: 0x002326FC File Offset: 0x002308FC
	public virtual void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		if (_eStreamMode == TileEntity.StreamModeRead.Persistency)
		{
			this.readVersion = (int)_br.ReadUInt16();
			this.chunkPos = StreamUtils.ReadVector3i(_br);
			this.entityId = _br.ReadInt32();
			if (this.readVersion > 1)
			{
				this.heapMapUpdateTime = _br.ReadUInt64();
				this.heapMapLastTime = this.heapMapUpdateTime - AIDirector.GetActivityWorldTimeDelay();
				return;
			}
		}
		else
		{
			this.chunkPos = StreamUtils.ReadVector3i(_br);
			this.entityId = _br.ReadInt32();
		}
	}

	// Token: 0x0600563B RID: 22075 RVA: 0x00232770 File Offset: 0x00230970
	public virtual void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		if (_eStreamMode == TileEntity.StreamModeWrite.Persistency)
		{
			_bw.Write(12);
			StreamUtils.Write(_bw, this.chunkPos);
			_bw.Write(this.entityId);
			_bw.Write(this.heapMapUpdateTime);
			return;
		}
		StreamUtils.Write(_bw, this.chunkPos);
		_bw.Write(this.entityId);
	}

	// Token: 0x0600563C RID: 22076 RVA: 0x002327C8 File Offset: 0x002309C8
	public override string ToString()
	{
		return string.Format(string.Concat(new string[]
		{
			"[TE] ",
			this.GetTileEntityType().ToStringCached<TileEntityType>(),
			"/",
			this.ToWorldPos().ToString(),
			"/",
			this.entityId.ToString()
		}), Array.Empty<object>());
	}

	// Token: 0x0600563D RID: 22077 RVA: 0x00232835 File Offset: 0x00230A35
	public virtual void OnRemove(World world)
	{
		this.OnDestroy();
	}

	// Token: 0x0600563E RID: 22078 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnUnload(World world)
	{
	}

	// Token: 0x0600563F RID: 22079 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnReadComplete()
	{
	}

	// Token: 0x06005640 RID: 22080 RVA: 0x0023283D File Offset: 0x00230A3D
	public void SetDisableModifiedCheck(bool _b)
	{
		this.bDisableModifiedCheck = _b;
	}

	// Token: 0x06005641 RID: 22081 RVA: 0x00232846 File Offset: 0x00230A46
	public void SetModified()
	{
		this.setModified();
	}

	// Token: 0x06005642 RID: 22082 RVA: 0x0023284E File Offset: 0x00230A4E
	public void SetChunkModified()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.chunk != null)
		{
			this.chunk.isModified = true;
		}
	}

	// Token: 0x06005643 RID: 22083 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsActive(World world)
	{
		return false;
	}

	// Token: 0x06005644 RID: 22084 RVA: 0x00232870 File Offset: 0x00230A70
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool IsByWater(World _world, Vector3i _blockPos)
	{
		return _world.IsWater(_blockPos.x, _blockPos.y + 1, _blockPos.z) | _world.IsWater(_blockPos.x + 1, _blockPos.y, _blockPos.z) | _world.IsWater(_blockPos.x - 1, _blockPos.y, _blockPos.z) | _world.IsWater(_blockPos.x, _blockPos.y, _blockPos.z + 1) | _world.IsWater(_blockPos.x, _blockPos.y, _blockPos.z - 1);
	}

	// Token: 0x06005645 RID: 22085 RVA: 0x00232904 File Offset: 0x00230B04
	[PublicizedFrom(EAccessModifier.Protected)]
	public void emitHeatMapEvent(World world, EnumAIDirectorChunkEvent eventType)
	{
		if (world.worldTime < this.heapMapLastTime)
		{
			this.heapMapUpdateTime = 0UL;
		}
		if (world.worldTime >= this.heapMapUpdateTime && world.aiDirector != null)
		{
			Vector3i vector3i = this.ToWorldPos();
			Block block = world.GetBlock(vector3i).Block;
			if (block != null)
			{
				world.aiDirector.NotifyActivity(eventType, vector3i, block.HeatMapStrength, 720f);
				this.heapMapLastTime = world.worldTime;
				this.heapMapUpdateTime = world.worldTime + AIDirector.GetActivityWorldTimeDelay();
			}
		}
	}

	// Token: 0x06005646 RID: 22086 RVA: 0x00232990 File Offset: 0x00230B90
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void setModified()
	{
		if (this.bDisableModifiedCheck)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SetChunkModified();
			Vector3? entitiesInRangeOfWorldPos = new Vector3?(this.ToWorldCenterPos());
			if (entitiesInRangeOfWorldPos.Value == Vector3.zero)
			{
				entitiesInRangeOfWorldPos = null;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTileEntity>().Setup(this, TileEntity.StreamModeWrite.ToClient, byte.MaxValue), true, -1, -1, -1, entitiesInRangeOfWorldPos, 192, false);
		}
		else
		{
			byte b = this.handleCounter + 1;
			this.handleCounter = b;
			if (b == 255)
			{
				this.handleCounter = 0;
			}
			this.lockHandleWaitingFor = this.handleCounter;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageTileEntity>().Setup(this, TileEntity.StreamModeWrite.ToServer, this.lockHandleWaitingFor), false);
		}
		this.NotifyListeners();
	}

	// Token: 0x06005647 RID: 22087 RVA: 0x00232A58 File Offset: 0x00230C58
	public override int GetHashCode()
	{
		if (this.entityId != -1)
		{
			return this.entityId | 134217728;
		}
		return this.ToWorldPos().GetHashCode() & int.MaxValue;
	}

	// Token: 0x06005648 RID: 22088 RVA: 0x001D9CD6 File Offset: 0x001D7ED6
	public override bool Equals(object obj)
	{
		return base.Equals(obj) && obj.GetHashCode() == this.GetHashCode();
	}

	// Token: 0x06005649 RID: 22089 RVA: 0x00232A98 File Offset: 0x00230C98
	public void NotifyListeners()
	{
		for (int i = 0; i < this.listeners.Count; i++)
		{
			this.listeners[i].OnTileEntityChanged(this);
		}
	}

	// Token: 0x0600564A RID: 22090 RVA: 0x00232ACD File Offset: 0x00230CCD
	public virtual void UpgradeDowngradeFrom(TileEntity _other)
	{
		_other.OnDestroy();
	}

	// Token: 0x0600564B RID: 22091 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
	}

	// Token: 0x0600564C RID: 22092 RVA: 0x00232AD5 File Offset: 0x00230CD5
	public virtual void SetUserAccessing(bool _bUserAccessing)
	{
		this.bUserAccessing = _bUserAccessing;
	}

	// Token: 0x0600564D RID: 22093 RVA: 0x00232ADE File Offset: 0x00230CDE
	public bool IsUserAccessing()
	{
		return this.bUserAccessing;
	}

	// Token: 0x0600564E RID: 22094 RVA: 0x00232AE6 File Offset: 0x00230CE6
	public virtual void SetHandle(byte _handle)
	{
		if (this.lockHandleWaitingFor != 255 && this.lockHandleWaitingFor == _handle)
		{
			this.lockHandleWaitingFor = byte.MaxValue;
		}
	}

	// Token: 0x0600564F RID: 22095 RVA: 0x00232B09 File Offset: 0x00230D09
	public virtual void OnDestroy()
	{
		if (this.Destroyed != null)
		{
			this.Destroyed(this);
		}
	}

	// Token: 0x06005650 RID: 22096 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Reset(FastTags<TagGroup.Global> questTags)
	{
	}

	// Token: 0x040042BD RID: 17085
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int CurrentSaveVersion = 12;

	// Token: 0x040042BE RID: 17086
	public int entityId = -1;

	// Token: 0x040042BF RID: 17087
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i chunkPos;

	// Token: 0x040042C0 RID: 17088
	[PublicizedFrom(EAccessModifier.Protected)]
	public int readVersion;

	// Token: 0x040042C1 RID: 17089
	[PublicizedFrom(EAccessModifier.Protected)]
	public Chunk chunk;

	// Token: 0x040042C2 RID: 17090
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong heapMapLastTime;

	// Token: 0x040042C3 RID: 17091
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong heapMapUpdateTime;

	// Token: 0x040042C4 RID: 17092
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bDisableModifiedCheck;

	// Token: 0x040042C5 RID: 17093
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bUserAccessing;

	// Token: 0x040042C8 RID: 17096
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ITileEntityChangedListener> _listeners = new List<ITileEntityChangedListener>();

	// Token: 0x040042C9 RID: 17097
	[PublicizedFrom(EAccessModifier.Private)]
	public byte handleCounter;

	// Token: 0x040042CA RID: 17098
	[PublicizedFrom(EAccessModifier.Private)]
	public byte lockHandleWaitingFor = byte.MaxValue;

	// Token: 0x02000AF5 RID: 2805
	public enum StreamModeRead
	{
		// Token: 0x040042CC RID: 17100
		Persistency,
		// Token: 0x040042CD RID: 17101
		FromServer,
		// Token: 0x040042CE RID: 17102
		FromClient
	}

	// Token: 0x02000AF6 RID: 2806
	public enum StreamModeWrite
	{
		// Token: 0x040042D0 RID: 17104
		Persistency,
		// Token: 0x040042D1 RID: 17105
		ToServer,
		// Token: 0x040042D2 RID: 17106
		ToClient
	}
}
