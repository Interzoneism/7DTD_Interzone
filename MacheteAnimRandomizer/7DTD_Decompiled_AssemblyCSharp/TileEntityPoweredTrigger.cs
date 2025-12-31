using System;

// Token: 0x02000B05 RID: 2821
public class TileEntityPoweredTrigger : TileEntityPowered
{
	// Token: 0x170008B3 RID: 2227
	// (get) Token: 0x0600574A RID: 22346 RVA: 0x00237694 File Offset: 0x00235894
	// (set) Token: 0x0600574B RID: 22347 RVA: 0x002376A8 File Offset: 0x002358A8
	public bool IsTriggered
	{
		get
		{
			return ((PowerTrigger)this.PowerItem).IsTriggered;
		}
		set
		{
			PowerTrigger powerTrigger = this.PowerItem as PowerTrigger;
			powerTrigger.IsTriggered = value;
			if (powerTrigger.TriggerType == PowerTrigger.TriggerTypes.PressurePlate)
			{
				(powerTrigger as PowerPressurePlate).Pressed = true;
			}
		}
	}

	// Token: 0x0600574C RID: 22348 RVA: 0x002376DD File Offset: 0x002358DD
	public TileEntityPoweredTrigger(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x0600574D RID: 22349 RVA: 0x002376F1 File Offset: 0x002358F1
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x0600574E RID: 22350 RVA: 0x00237704 File Offset: 0x00235904
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x0600574F RID: 22351 RVA: 0x0023770C File Offset: 0x0023590C
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
	}

	// Token: 0x06005750 RID: 22352 RVA: 0x00237718 File Offset: 0x00235918
	public bool Activate(bool activated, bool isOn)
	{
		World world = GameManager.Instance.World;
		BlockValue block = this.chunk.GetBlock(base.localChunkPos);
		return block.Block.ActivateBlock(world, base.GetClrIdx(), base.ToWorldPos(), block, isOn, activated);
	}

	// Token: 0x06005751 RID: 22353 RVA: 0x00237760 File Offset: 0x00235960
	[PublicizedFrom(EAccessModifier.Protected)]
	public override PowerItem CreatePowerItem()
	{
		BlockPowered blockPowered = (BlockPowered)this.chunk.GetBlock(base.localChunkPos).Block;
		if (blockPowered is BlockPressurePlate)
		{
			this.TriggerType = PowerTrigger.TriggerTypes.PressurePlate;
		}
		else if (blockPowered is BlockMotionSensor)
		{
			this.TriggerType = PowerTrigger.TriggerTypes.Motion;
		}
		else if (blockPowered is BlockTripWire)
		{
			this.TriggerType = PowerTrigger.TriggerTypes.TripWire;
		}
		else if (blockPowered is BlockTimerRelay)
		{
			this.TriggerType = PowerTrigger.TriggerTypes.TimerRelay;
		}
		else if (blockPowered is BlockSwitch)
		{
			this.TriggerType = PowerTrigger.TriggerTypes.Switch;
		}
		if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
		{
			return new PowerTimerRelay
			{
				TriggerType = this.TriggerType
			};
		}
		if (this.TriggerType == PowerTrigger.TriggerTypes.TripWire)
		{
			return new PowerTripWireRelay
			{
				TriggerType = PowerTrigger.TriggerTypes.TripWire
			};
		}
		if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
		{
			return new PowerTrigger
			{
				TriggerType = this.TriggerType,
				TriggerPowerDuration = PowerTrigger.TriggerPowerDurationTypes.Triggered,
				TriggerPowerDelay = PowerTrigger.TriggerPowerDelayTypes.Instant
			};
		}
		if (this.TriggerType == PowerTrigger.TriggerTypes.PressurePlate)
		{
			return new PowerPressurePlate
			{
				TriggerType = this.TriggerType
			};
		}
		return new PowerTrigger
		{
			TriggerType = this.TriggerType
		};
	}

	// Token: 0x06005752 RID: 22354 RVA: 0x00237868 File Offset: 0x00235A68
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.TriggerType = (PowerTrigger.TriggerTypes)_br.ReadByte();
		if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
		{
			this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		}
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeRead.FromClient)
			{
				if (this.PowerItem == null)
				{
					this.PowerItem = base.CreatePowerItemForTileEntity((ushort)this.chunk.GetBlock(base.localChunkPos).type);
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
				{
					(this.PowerItem as PowerTimerRelay).StartTime = _br.ReadByte();
					(this.PowerItem as PowerTimerRelay).EndTime = _br.ReadByte();
				}
				else if (this.TriggerType != PowerTrigger.TriggerTypes.Switch)
				{
					(this.PowerItem as PowerTrigger).TriggerPowerDelay = (PowerTrigger.TriggerPowerDelayTypes)_br.ReadByte();
					(this.PowerItem as PowerTrigger).TriggerPowerDuration = (PowerTrigger.TriggerPowerDurationTypes)_br.ReadByte();
					if (_br.ReadBoolean())
					{
						(this.PowerItem as PowerTrigger).ResetTrigger();
					}
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
				{
					this.TargetType = _br.ReadInt32();
					return;
				}
			}
			else
			{
				if (this.TriggerType == PowerTrigger.TriggerTypes.TripWire)
				{
					this.ClientData.ShowTriggerOptions = _br.ReadBoolean();
					this.ClientData.HasChanges = true;
				}
				if (this.TriggerType != PowerTrigger.TriggerTypes.Switch)
				{
					this.ClientData.Property1 = _br.ReadByte();
					this.ClientData.Property2 = _br.ReadByte();
					this.ClientData.HasChanges = true;
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
				{
					int targetType = _br.ReadInt32();
					if (!this.bUserAccessing)
					{
						this.TargetType = targetType;
					}
				}
			}
		}
	}

	// Token: 0x06005753 RID: 22355 RVA: 0x002379F2 File Offset: 0x00235BF2
	public void ResetTrigger()
	{
		if (this.TriggerType != PowerTrigger.TriggerTypes.TimerRelay)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerTrigger).ResetTrigger();
				return;
			}
			this.ClientData.ResetTrigger = true;
			base.SetModified();
		}
	}

	// Token: 0x06005754 RID: 22356 RVA: 0x00237A2C File Offset: 0x00235C2C
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write((byte)this.TriggerType);
		if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
		{
			this.ownerID.ToStream(_bw, false);
		}
		if (_eStreamMode != TileEntity.StreamModeWrite.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeWrite.ToServer)
			{
				if (this.TriggerType != PowerTrigger.TriggerTypes.Switch)
				{
					_bw.Write(this.ClientData.Property1);
					_bw.Write(this.ClientData.Property2);
					_bw.Write(this.ClientData.ResetTrigger);
					this.ClientData.ResetTrigger = false;
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
				{
					_bw.Write(this.TargetType);
					return;
				}
			}
			else
			{
				if (this.PowerItem == null)
				{
					this.PowerItem = base.CreatePowerItemForTileEntity((ushort)this.chunk.GetBlock(base.localChunkPos).type);
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.TripWire)
				{
					PowerTripWireRelay powerTripWireRelay = this.PowerItem as PowerTripWireRelay;
					_bw.Write(powerTripWireRelay.Parent != null && powerTripWireRelay.Parent is PowerTripWireRelay);
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
				{
					_bw.Write((this.PowerItem as PowerTimerRelay).StartTime);
					_bw.Write((this.PowerItem as PowerTimerRelay).EndTime);
				}
				else if (this.TriggerType != PowerTrigger.TriggerTypes.Switch)
				{
					_bw.Write((byte)(this.PowerItem as PowerTrigger).TriggerPowerDelay);
					_bw.Write((byte)(this.PowerItem as PowerTrigger).TriggerPowerDuration);
				}
				if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
				{
					_bw.Write(this.TargetType);
				}
			}
		}
	}

	// Token: 0x170008B4 RID: 2228
	// (get) Token: 0x06005755 RID: 22357 RVA: 0x00237BB4 File Offset: 0x00235DB4
	// (set) Token: 0x06005756 RID: 22358 RVA: 0x00237C04 File Offset: 0x00235E04
	public byte Property1
	{
		get
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return this.ClientData.Property1;
			}
			if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
			{
				return (this.PowerItem as PowerTimerRelay).StartTime;
			}
			return (byte)(this.PowerItem as PowerTrigger).TriggerPowerDelay;
		}
		set
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.ClientData.Property1 = value;
				return;
			}
			if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
			{
				(this.PowerItem as PowerTimerRelay).StartTime = value;
				return;
			}
			(this.PowerItem as PowerTrigger).TriggerPowerDelay = (PowerTrigger.TriggerPowerDelayTypes)value;
		}
	}

	// Token: 0x170008B5 RID: 2229
	// (get) Token: 0x06005757 RID: 22359 RVA: 0x00237C58 File Offset: 0x00235E58
	// (set) Token: 0x06005758 RID: 22360 RVA: 0x00237CA8 File Offset: 0x00235EA8
	public byte Property2
	{
		get
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return this.ClientData.Property2;
			}
			if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
			{
				return (this.PowerItem as PowerTimerRelay).EndTime;
			}
			return (byte)(this.PowerItem as PowerTrigger).TriggerPowerDuration;
		}
		set
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.ClientData.Property2 = value;
				return;
			}
			if (this.TriggerType == PowerTrigger.TriggerTypes.TimerRelay)
			{
				(this.PowerItem as PowerTimerRelay).EndTime = value;
				return;
			}
			(this.PowerItem as PowerTrigger).TriggerPowerDuration = (PowerTrigger.TriggerPowerDurationTypes)value;
		}
	}

	// Token: 0x170008B6 RID: 2230
	// (get) Token: 0x06005759 RID: 22361 RVA: 0x00237CFA File Offset: 0x00235EFA
	// (set) Token: 0x0600575A RID: 22362 RVA: 0x00237D24 File Offset: 0x00235F24
	public int TargetType
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (int)(this.PowerItem as PowerTrigger).TargetType;
			}
			return this.ClientData.TargetType;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerTrigger).TargetType = (PowerTrigger.TargetTypes)value;
				return;
			}
			this.ClientData.TargetType = value;
		}
	}

	// Token: 0x170008B7 RID: 2231
	// (get) Token: 0x0600575B RID: 22363 RVA: 0x00237D50 File Offset: 0x00235F50
	public bool ShowTriggerOptions
	{
		get
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return this.ClientData.ShowTriggerOptions;
			}
			if (this.TriggerType == PowerTrigger.TriggerTypes.TripWire)
			{
				PowerTripWireRelay powerTripWireRelay = this.PowerItem as PowerTripWireRelay;
				return powerTripWireRelay.Parent != null && powerTripWireRelay.Parent is PowerTripWireRelay;
			}
			return true;
		}
	}

	// Token: 0x170008B8 RID: 2232
	// (get) Token: 0x0600575C RID: 22364 RVA: 0x00237DA4 File Offset: 0x00235FA4
	public bool TargetSelf
	{
		get
		{
			return (this.TargetType & 1) == 1;
		}
	}

	// Token: 0x170008B9 RID: 2233
	// (get) Token: 0x0600575D RID: 22365 RVA: 0x00237DB1 File Offset: 0x00235FB1
	public bool TargetAllies
	{
		get
		{
			return (this.TargetType & 2) == 2;
		}
	}

	// Token: 0x170008BA RID: 2234
	// (get) Token: 0x0600575E RID: 22366 RVA: 0x00237DBE File Offset: 0x00235FBE
	public bool TargetStrangers
	{
		get
		{
			return (this.TargetType & 4) == 4;
		}
	}

	// Token: 0x170008BB RID: 2235
	// (get) Token: 0x0600575F RID: 22367 RVA: 0x00237DCB File Offset: 0x00235FCB
	public bool TargetZombies
	{
		get
		{
			return (this.TargetType & 8) == 8;
		}
	}

	// Token: 0x06005760 RID: 22368 RVA: 0x000DCA80 File Offset: 0x000DAC80
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Trigger;
	}

	// Token: 0x04004336 RID: 17206
	public PowerTrigger.TriggerTypes TriggerType;

	// Token: 0x04004337 RID: 17207
	public TileEntityPoweredTrigger.ClientTriggerData ClientData = new TileEntityPoweredTrigger.ClientTriggerData();

	// Token: 0x04004338 RID: 17208
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x02000B06 RID: 2822
	public class ClientTriggerData
	{
		// Token: 0x04004339 RID: 17209
		public byte Property1;

		// Token: 0x0400433A RID: 17210
		public byte Property2;

		// Token: 0x0400433B RID: 17211
		public int TargetType = 3;

		// Token: 0x0400433C RID: 17212
		public bool ShowTriggerOptions;

		// Token: 0x0400433D RID: 17213
		public bool ResetTrigger;

		// Token: 0x0400433E RID: 17214
		public bool HasChanges;
	}
}
