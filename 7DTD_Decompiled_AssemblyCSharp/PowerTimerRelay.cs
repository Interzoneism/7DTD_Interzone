using System;
using System.IO;
using Audio;
using UnityEngine;

// Token: 0x02000849 RID: 2121
public class PowerTimerRelay : PowerTrigger
{
	// Token: 0x17000651 RID: 1617
	// (get) Token: 0x06003D06 RID: 15622 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.Timer;
		}
	}

	// Token: 0x17000652 RID: 1618
	// (get) Token: 0x06003D07 RID: 15623 RVA: 0x0018793B File Offset: 0x00185B3B
	// (set) Token: 0x06003D08 RID: 15624 RVA: 0x00187944 File Offset: 0x00185B44
	public byte StartTime
	{
		get
		{
			return this.startTime;
		}
		set
		{
			this.startTime = value;
			int hours = (int)(this.startTime / 2);
			bool flag = this.startTime % 2 == 1;
			this.startTimeInTicks = GameUtils.DayTimeToWorldTime(1, hours, flag ? 30 : 0);
		}
	}

	// Token: 0x17000653 RID: 1619
	// (get) Token: 0x06003D09 RID: 15625 RVA: 0x00187982 File Offset: 0x00185B82
	// (set) Token: 0x06003D0A RID: 15626 RVA: 0x0018798C File Offset: 0x00185B8C
	public byte EndTime
	{
		get
		{
			return this.endTime;
		}
		set
		{
			this.endTime = value;
			int hours = (int)(this.endTime / 2);
			bool flag = this.endTime % 2 == 1;
			this.endTimeInTicks = GameUtils.DayTimeToWorldTime(1, hours, flag ? 30 : 0);
		}
	}

	// Token: 0x06003D0B RID: 15627 RVA: 0x001879CA File Offset: 0x00185BCA
	public PowerTimerRelay()
	{
		this.StartTime = 0;
		this.EndTime = 24;
	}

	// Token: 0x17000654 RID: 1620
	// (get) Token: 0x06003D0C RID: 15628 RVA: 0x001879E9 File Offset: 0x00185BE9
	// (set) Token: 0x06003D0D RID: 15629 RVA: 0x001879F4 File Offset: 0x00185BF4
	public override bool IsTriggered
	{
		get
		{
			return this.isTriggered;
		}
		set
		{
			if (this.lastTriggered != value)
			{
				this.lastTriggered = this.isTriggered;
				this.isTriggered = value;
				if (!this.isTriggered && this.lastTriggered)
				{
					if (this.isPowered)
					{
						Manager.BroadcastPlay(this.Position.ToVector3(), "timer_start", 0f);
					}
					this.HandleDisconnect();
					return;
				}
				if (this.isPowered)
				{
					Manager.BroadcastPlay(this.Position.ToVector3(), "timer_stop", 0f);
				}
				this.isActive = true;
				base.SendHasLocalChangesToRoot();
			}
		}
	}

	// Token: 0x06003D0E RID: 15630 RVA: 0x00187A85 File Offset: 0x00185C85
	public override bool PowerChildren()
	{
		return this.IsTriggered;
	}

	// Token: 0x06003D0F RID: 15631 RVA: 0x00187A90 File Offset: 0x00185C90
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CheckForActiveChange()
	{
		if (GameManager.Instance.World != null)
		{
			ulong num = GameManager.Instance.World.worldTime % 24000UL;
			if (this.StartTime < this.EndTime)
			{
				this.IsTriggered = (this.startTimeInTicks < num && num < this.endTimeInTicks);
				return;
			}
			if (this.EndTime < this.StartTime)
			{
				this.IsTriggered = (num > this.startTimeInTicks || num < this.endTimeInTicks);
				return;
			}
			this.IsTriggered = false;
		}
	}

	// Token: 0x06003D10 RID: 15632 RVA: 0x00187B1B File Offset: 0x00185D1B
	public override void CachedUpdateCall()
	{
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 1f;
			this.CheckForActiveChange();
		}
	}

	// Token: 0x06003D11 RID: 15633 RVA: 0x00187B44 File Offset: 0x00185D44
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
				this.Children[i].HandlePowerReceived(ref power);
				if (power <= 0)
				{
					return;
				}
			}
		}
	}

	// Token: 0x06003D12 RID: 15634 RVA: 0x00187BC8 File Offset: 0x00185DC8
	public override void HandlePowerUpdate(bool parentIsOn)
	{
		if (this.TileEntity != null)
		{
			((TileEntityPoweredTrigger)this.TileEntity).Activate(this.isPowered && parentIsOn, this.isTriggered);
			this.TileEntity.SetModified();
		}
		for (int i = 0; i < this.Children.Count; i++)
		{
			this.Children[i].HandlePowerUpdate(this.isPowered && parentIsOn);
		}
		this.hasChangesLocal = true;
	}

	// Token: 0x06003D13 RID: 15635 RVA: 0x00187C3D File Offset: 0x00185E3D
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		this.StartTime = _br.ReadByte();
		this.EndTime = _br.ReadByte();
	}

	// Token: 0x06003D14 RID: 15636 RVA: 0x00187C5F File Offset: 0x00185E5F
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.StartTime);
		_bw.Write(this.EndTime);
	}

	// Token: 0x04003146 RID: 12614
	[PublicizedFrom(EAccessModifier.Private)]
	public byte startTime;

	// Token: 0x04003147 RID: 12615
	[PublicizedFrom(EAccessModifier.Private)]
	public byte endTime = 12;

	// Token: 0x04003148 RID: 12616
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong startTimeInTicks;

	// Token: 0x04003149 RID: 12617
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong endTimeInTicks;

	// Token: 0x0400314A RID: 12618
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;
}
