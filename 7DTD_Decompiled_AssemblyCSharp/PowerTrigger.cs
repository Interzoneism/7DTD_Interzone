using System;
using System.IO;
using Audio;
using UnityEngine;

// Token: 0x0200084A RID: 2122
public class PowerTrigger : PowerConsumer
{
	// Token: 0x17000655 RID: 1621
	// (get) Token: 0x06003D15 RID: 15637 RVA: 0x00075C39 File Offset: 0x00073E39
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.Trigger;
		}
	}

	// Token: 0x17000656 RID: 1622
	// (get) Token: 0x06003D16 RID: 15638 RVA: 0x00187C80 File Offset: 0x00185E80
	// (set) Token: 0x06003D17 RID: 15639 RVA: 0x00187C88 File Offset: 0x00185E88
	public PowerTrigger.TriggerPowerDelayTypes TriggerPowerDelay
	{
		get
		{
			return this.triggerPowerDelay;
		}
		set
		{
			this.triggerPowerDelay = value;
		}
	}

	// Token: 0x17000657 RID: 1623
	// (get) Token: 0x06003D18 RID: 15640 RVA: 0x00187C91 File Offset: 0x00185E91
	// (set) Token: 0x06003D19 RID: 15641 RVA: 0x00187C99 File Offset: 0x00185E99
	public PowerTrigger.TriggerPowerDurationTypes TriggerPowerDuration
	{
		get
		{
			return this.triggerPowerDuration;
		}
		set
		{
			this.triggerPowerDuration = value;
		}
	}

	// Token: 0x17000658 RID: 1624
	// (get) Token: 0x06003D1A RID: 15642 RVA: 0x00187CA2 File Offset: 0x00185EA2
	public virtual bool IsActive
	{
		get
		{
			if (this.TriggerType == PowerTrigger.TriggerTypes.Switch)
			{
				return this.isTriggered;
			}
			return this.isActive || this.parentTriggered;
		}
	}

	// Token: 0x17000659 RID: 1625
	// (get) Token: 0x06003D1B RID: 15643 RVA: 0x001879E9 File Offset: 0x00185BE9
	// (set) Token: 0x06003D1C RID: 15644 RVA: 0x00187CC4 File Offset: 0x00185EC4
	public virtual bool IsTriggered
	{
		get
		{
			return this.isTriggered;
		}
		set
		{
			if (this.TriggerType == PowerTrigger.TriggerTypes.Switch)
			{
				this.lastTriggered = this.isTriggered;
				this.isTriggered = value;
				if (this.isTriggered && !this.lastTriggered)
				{
					this.isActive = true;
				}
				base.SendHasLocalChangesToRoot();
				if (!this.isTriggered && this.lastTriggered)
				{
					this.HandleDisconnectChildren();
					this.isActive = false;
					return;
				}
			}
			else
			{
				this.isTriggered = value;
				if (this.isTriggered && !this.lastTriggered)
				{
					PowerTrigger.TriggerTypes triggerType = this.TriggerType;
					if (triggerType != PowerTrigger.TriggerTypes.Motion)
					{
						if (triggerType == PowerTrigger.TriggerTypes.TripWire)
						{
							Manager.BroadcastPlay(this.Position.ToVector3(), "trip_wire_trigger", 0f);
						}
					}
					else
					{
						Manager.BroadcastPlay(this.Position.ToVector3(), "motion_sensor_trigger", 0f);
					}
					base.SendHasLocalChangesToRoot();
				}
				this.lastTriggered = this.isTriggered;
				if (this.IsPowered && !this.isActive && this.delayStartTime == -1f)
				{
					this.lastPowerTime = Time.time;
					this.delayStartTime = -1f;
					switch (this.TriggerPowerDelay)
					{
					case PowerTrigger.TriggerPowerDelayTypes.OneSecond:
						this.delayStartTime = 1f;
						break;
					case PowerTrigger.TriggerPowerDelayTypes.TwoSecond:
						this.delayStartTime = 2f;
						break;
					case PowerTrigger.TriggerPowerDelayTypes.ThreeSecond:
						this.delayStartTime = 3f;
						break;
					case PowerTrigger.TriggerPowerDelayTypes.FourSecond:
						this.delayStartTime = 4f;
						break;
					case PowerTrigger.TriggerPowerDelayTypes.FiveSecond:
						this.delayStartTime = 5f;
						break;
					}
					if (this.delayStartTime == -1f)
					{
						this.isActive = true;
						this.SetupDurationTime();
					}
				}
				this.parentTriggered = false;
			}
		}
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x00187E60 File Offset: 0x00186060
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetupDurationTime()
	{
		this.lastPowerTime = Time.time;
		switch (this.TriggerPowerDuration)
		{
		case PowerTrigger.TriggerPowerDurationTypes.Always:
			this.powerTime = -1f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.Triggered:
			this.powerTime = 0f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.OneSecond:
			this.powerTime = 1f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.TwoSecond:
			this.powerTime = 2f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.ThreeSecond:
			this.powerTime = 3f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.FourSecond:
			this.powerTime = 4f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.FiveSecond:
			this.powerTime = 5f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.SixSecond:
			this.powerTime = 6f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.SevenSecond:
			this.powerTime = 7f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.EightSecond:
			this.powerTime = 8f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.NineSecond:
			this.powerTime = 9f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.TenSecond:
			this.powerTime = 10f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.FifteenSecond:
			this.powerTime = 15f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.ThirtySecond:
			this.powerTime = 30f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.FourtyFiveSecond:
			this.powerTime = 45f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.OneMinute:
			this.powerTime = 60f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.FiveMinute:
			this.powerTime = 300f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.TenMinute:
			this.powerTime = 600f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.ThirtyMinute:
			this.powerTime = 1800f;
			return;
		case PowerTrigger.TriggerPowerDurationTypes.SixtyMinute:
			this.powerTime = 3600f;
			return;
		default:
			return;
		}
	}

	// Token: 0x06003D1F RID: 15647 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool PowerChildren()
	{
		return true;
	}

	// Token: 0x06003D20 RID: 15648 RVA: 0x00187FF4 File Offset: 0x001861F4
	public override void HandlePowerReceived(ref ushort power)
	{
		ushort num = (ushort)Mathf.Min((int)this.RequiredPower, (int)power);
		num = (ushort)Mathf.Min((int)num, (int)this.RequiredPower);
		this.isPowered = (num == this.RequiredPower);
		power -= num;
		if (power <= 0)
		{
			return;
		}
		this.CheckForActiveChange();
		if (this.PowerChildren())
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				if (this.Children[i] is PowerTrigger)
				{
					PowerTrigger powerTrigger = this.Children[i] as PowerTrigger;
					this.HandleParentTriggering(powerTrigger);
					if ((this.TriggerType == PowerTrigger.TriggerTypes.Motion || this.TriggerType == PowerTrigger.TriggerTypes.PressurePlate || this.TriggerType == PowerTrigger.TriggerTypes.TripWire) && (powerTrigger.TriggerType == PowerTrigger.TriggerTypes.Motion || powerTrigger.TriggerType == PowerTrigger.TriggerTypes.PressurePlate || powerTrigger.TriggerType == PowerTrigger.TriggerTypes.TripWire))
					{
						powerTrigger.HandlePowerReceived(ref power);
					}
					else if (this.IsActive)
					{
						powerTrigger.HandlePowerReceived(ref power);
					}
				}
				else if (this.IsActive)
				{
					this.Children[i].HandlePowerReceived(ref power);
				}
				if (power <= 0)
				{
					return;
				}
			}
		}
	}

	// Token: 0x06003D21 RID: 15649 RVA: 0x00188103 File Offset: 0x00186303
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void CheckForActiveChange()
	{
		if (this.powerTime == 0f && this.lastTriggered && !this.isTriggered)
		{
			this.isActive = false;
			this.HandleDisconnectChildren();
			base.SendHasLocalChangesToRoot();
			this.powerTime = -1f;
		}
	}

	// Token: 0x06003D22 RID: 15650 RVA: 0x00188140 File Offset: 0x00186340
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleSingleUseDisable()
	{
		PowerTrigger.TriggerTypes triggerType = this.TriggerType;
		if (triggerType == PowerTrigger.TriggerTypes.PressurePlate || triggerType - PowerTrigger.TriggerTypes.Motion <= 1)
		{
			this.lastTriggered = this.isTriggered;
			this.isTriggered = false;
		}
	}

	// Token: 0x06003D23 RID: 15651 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleSoundDisable()
	{
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x00188174 File Offset: 0x00186374
	public override void HandlePowerUpdate(bool parentIsOn)
	{
		if (this.TileEntity != null)
		{
			((TileEntityPoweredTrigger)this.TileEntity).Activate(this.isPowered && parentIsOn, this.isTriggered);
			this.TileEntity.SetModified();
		}
		for (int i = 0; i < this.Children.Count; i++)
		{
			if (this.Children[i] is PowerTrigger)
			{
				PowerTrigger child = this.Children[i] as PowerTrigger;
				this.HandleParentTriggering(child);
				this.Children[i].HandlePowerUpdate(this.isPowered && parentIsOn);
			}
			else if (this.IsActive)
			{
				this.Children[i].HandlePowerUpdate(this.isPowered && parentIsOn);
			}
		}
		this.hasChangesLocal = true;
		this.HandleSingleUseDisable();
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x00188240 File Offset: 0x00186440
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleParentTriggering(PowerTrigger child)
	{
		if (!this.IsActive)
		{
			child.SetTriggeredByParent(false);
			return;
		}
		if ((this.TriggerType == PowerTrigger.TriggerTypes.Motion || this.TriggerType == PowerTrigger.TriggerTypes.PressurePlate || this.TriggerType == PowerTrigger.TriggerTypes.TripWire) && (child.TriggerType == PowerTrigger.TriggerTypes.Motion || child.TriggerType == PowerTrigger.TriggerTypes.PressurePlate || child.TriggerType == PowerTrigger.TriggerTypes.TripWire))
		{
			child.SetTriggeredByParent(true);
			return;
		}
		child.SetTriggeredByParent(false);
	}

	// Token: 0x06003D26 RID: 15654 RVA: 0x001882A2 File Offset: 0x001864A2
	public void SetTriggeredByParent(bool triggered)
	{
		this.parentTriggered = triggered;
	}

	// Token: 0x06003D27 RID: 15655 RVA: 0x001882AC File Offset: 0x001864AC
	public virtual void CachedUpdateCall()
	{
		PowerTrigger.TriggerTypes triggerType = this.TriggerType;
		if (triggerType == PowerTrigger.TriggerTypes.PressurePlate || triggerType - PowerTrigger.TriggerTypes.Motion <= 1)
		{
			if (!this.hasChangesLocal)
			{
				if (this.isTriggered != this.lastTriggered)
				{
					base.SendHasLocalChangesToRoot();
				}
				this.CheckForActiveChange();
				this.HandleSingleUseDisable();
			}
			if (this.delayStartTime >= 0f)
			{
				if (Time.time - this.lastPowerTime >= this.delayStartTime)
				{
					base.SendHasLocalChangesToRoot();
					this.delayStartTime = -1f;
					this.isActive = true;
					this.SetupDurationTime();
				}
			}
			else if (this.powerTime > 0f && !this.parentTriggered && Time.time - this.lastPowerTime >= this.powerTime)
			{
				this.isActive = false;
				this.HandleDisconnectChildren();
				base.SendHasLocalChangesToRoot();
				this.powerTime = -1f;
			}
			this.hasChangesLocal = false;
			this.HandleSoundDisable();
		}
	}

	// Token: 0x06003D28 RID: 15656 RVA: 0x0018838C File Offset: 0x0018658C
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		this.TriggerType = (PowerTrigger.TriggerTypes)_br.ReadByte();
		if (this.TriggerType == PowerTrigger.TriggerTypes.Switch)
		{
			this.isTriggered = _br.ReadBoolean();
		}
		else
		{
			this.isActive = _br.ReadBoolean();
		}
		if (this.TriggerType != PowerTrigger.TriggerTypes.Switch)
		{
			this.TriggerPowerDelay = (PowerTrigger.TriggerPowerDelayTypes)_br.ReadByte();
			this.TriggerPowerDuration = (PowerTrigger.TriggerPowerDurationTypes)_br.ReadByte();
			this.delayStartTime = _br.ReadSingle();
			this.powerTime = _br.ReadSingle();
		}
		if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
		{
			this.TargetType = (PowerTrigger.TargetTypes)_br.ReadInt32();
		}
	}

	// Token: 0x06003D29 RID: 15657 RVA: 0x0018841C File Offset: 0x0018661C
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.TriggerType);
		if (this.TriggerType == PowerTrigger.TriggerTypes.Switch)
		{
			_bw.Write(this.isTriggered);
		}
		else
		{
			_bw.Write(this.isActive);
		}
		if (this.TriggerType != PowerTrigger.TriggerTypes.Switch)
		{
			_bw.Write((byte)this.TriggerPowerDelay);
			_bw.Write((byte)this.TriggerPowerDuration);
			_bw.Write(this.delayStartTime);
			_bw.Write(this.powerTime);
		}
		if (this.TriggerType == PowerTrigger.TriggerTypes.Motion)
		{
			_bw.Write((int)this.TargetType);
		}
	}

	// Token: 0x06003D2A RID: 15658 RVA: 0x001884B0 File Offset: 0x001866B0
	public virtual void HandleDisconnectChildren()
	{
		this.HandlePowerUpdate(false);
		for (int i = 0; i < this.Children.Count; i++)
		{
			this.Children[i].HandleDisconnect();
		}
	}

	// Token: 0x06003D2B RID: 15659 RVA: 0x001884EC File Offset: 0x001866EC
	public override void HandleDisconnect()
	{
		this.parentTriggered = (this.isActive = false);
		base.HandleDisconnect();
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x0018850F File Offset: 0x0018670F
	public void ResetTrigger()
	{
		this.delayStartTime = -1f;
		this.powerTime = -1f;
		this.isActive = false;
		this.HandleDisconnectChildren();
		base.SendHasLocalChangesToRoot();
	}

	// Token: 0x0400314B RID: 12619
	public PowerTrigger.TriggerTypes TriggerType;

	// Token: 0x0400314C RID: 12620
	public byte Parameter;

	// Token: 0x0400314D RID: 12621
	[PublicizedFrom(EAccessModifier.Protected)]
	public PowerTrigger.TriggerPowerDelayTypes triggerPowerDelay;

	// Token: 0x0400314E RID: 12622
	[PublicizedFrom(EAccessModifier.Protected)]
	public PowerTrigger.TriggerPowerDurationTypes triggerPowerDuration = PowerTrigger.TriggerPowerDurationTypes.Triggered;

	// Token: 0x0400314F RID: 12623
	public PowerTrigger.TargetTypes TargetType = PowerTrigger.TargetTypes.Self | PowerTrigger.TargetTypes.Allies;

	// Token: 0x04003150 RID: 12624
	[PublicizedFrom(EAccessModifier.Protected)]
	public float delayStartTime = -1f;

	// Token: 0x04003151 RID: 12625
	[PublicizedFrom(EAccessModifier.Protected)]
	public float powerTime;

	// Token: 0x04003152 RID: 12626
	[PublicizedFrom(EAccessModifier.Protected)]
	public float lastPowerTime = -1f;

	// Token: 0x04003153 RID: 12627
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lastTriggered;

	// Token: 0x04003154 RID: 12628
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isTriggered;

	// Token: 0x04003155 RID: 12629
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool parentTriggered;

	// Token: 0x04003156 RID: 12630
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isActive;

	// Token: 0x0200084B RID: 2123
	public enum TriggerTypes
	{
		// Token: 0x04003158 RID: 12632
		Switch,
		// Token: 0x04003159 RID: 12633
		PressurePlate,
		// Token: 0x0400315A RID: 12634
		TimerRelay,
		// Token: 0x0400315B RID: 12635
		Motion,
		// Token: 0x0400315C RID: 12636
		TripWire
	}

	// Token: 0x0200084C RID: 2124
	public enum TriggerPowerDelayTypes
	{
		// Token: 0x0400315E RID: 12638
		Instant,
		// Token: 0x0400315F RID: 12639
		OneSecond,
		// Token: 0x04003160 RID: 12640
		TwoSecond,
		// Token: 0x04003161 RID: 12641
		ThreeSecond,
		// Token: 0x04003162 RID: 12642
		FourSecond,
		// Token: 0x04003163 RID: 12643
		FiveSecond
	}

	// Token: 0x0200084D RID: 2125
	public enum TriggerPowerDurationTypes
	{
		// Token: 0x04003165 RID: 12645
		Always,
		// Token: 0x04003166 RID: 12646
		Triggered,
		// Token: 0x04003167 RID: 12647
		OneSecond,
		// Token: 0x04003168 RID: 12648
		TwoSecond,
		// Token: 0x04003169 RID: 12649
		ThreeSecond,
		// Token: 0x0400316A RID: 12650
		FourSecond,
		// Token: 0x0400316B RID: 12651
		FiveSecond,
		// Token: 0x0400316C RID: 12652
		SixSecond,
		// Token: 0x0400316D RID: 12653
		SevenSecond,
		// Token: 0x0400316E RID: 12654
		EightSecond,
		// Token: 0x0400316F RID: 12655
		NineSecond,
		// Token: 0x04003170 RID: 12656
		TenSecond,
		// Token: 0x04003171 RID: 12657
		FifteenSecond,
		// Token: 0x04003172 RID: 12658
		ThirtySecond,
		// Token: 0x04003173 RID: 12659
		FourtyFiveSecond,
		// Token: 0x04003174 RID: 12660
		OneMinute,
		// Token: 0x04003175 RID: 12661
		FiveMinute,
		// Token: 0x04003176 RID: 12662
		TenMinute,
		// Token: 0x04003177 RID: 12663
		ThirtyMinute,
		// Token: 0x04003178 RID: 12664
		SixtyMinute
	}

	// Token: 0x0200084E RID: 2126
	[Flags]
	public enum TargetTypes
	{
		// Token: 0x0400317A RID: 12666
		None = 0,
		// Token: 0x0400317B RID: 12667
		Self = 1,
		// Token: 0x0400317C RID: 12668
		Allies = 2,
		// Token: 0x0400317D RID: 12669
		Strangers = 4,
		// Token: 0x0400317E RID: 12670
		Zombies = 8
	}
}
