using System;
using System.IO;
using UnityEngine;

// Token: 0x02000847 RID: 2119
public class PowerSolarPanel : PowerSource
{
	// Token: 0x17000649 RID: 1609
	// (get) Token: 0x06003CE6 RID: 15590 RVA: 0x0018723B File Offset: 0x0018543B
	// (set) Token: 0x06003CE7 RID: 15591 RVA: 0x00187243 File Offset: 0x00185443
	public bool HasLight { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700064A RID: 1610
	// (get) Token: 0x06003CE8 RID: 15592 RVA: 0x00076E19 File Offset: 0x00075019
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.SolarPanel;
		}
	}

	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x06003CE9 RID: 15593 RVA: 0x0018724C File Offset: 0x0018544C
	public override string OnSound
	{
		get
		{
			return "solarpanel_on";
		}
	}

	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x06003CEA RID: 15594 RVA: 0x00187253 File Offset: 0x00185453
	public override string OffSound
	{
		get
		{
			return "solarpanel_off";
		}
	}

	// Token: 0x06003CEB RID: 15595 RVA: 0x0018725C File Offset: 0x0018545C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckLightLevel()
	{
		if (this.TileEntity != null)
		{
			Chunk chunk = this.TileEntity.GetChunk();
			Vector3i localChunkPos = this.TileEntity.localChunkPos;
			this.sunLight = chunk.GetLight(localChunkPos.x, localChunkPos.y, localChunkPos.z, Chunk.LIGHT_TYPE.SUN);
		}
		this.lastHasLight = this.HasLight;
		this.HasLight = (this.sunLight == 15 && GameManager.Instance.World.IsDaytime());
		if (this.lastHasLight != this.HasLight)
		{
			this.HandleOnOffSound();
			if (!this.HasLight)
			{
				this.CurrentPower = 0;
				this.HandleDisconnect();
				return;
			}
			base.SendHasLocalChangesToRoot();
		}
	}

	// Token: 0x06003CEC RID: 15596 RVA: 0x00187306 File Offset: 0x00185506
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void TickPowerGeneration()
	{
		if (this.HasLight)
		{
			this.CurrentPower = this.MaxOutput;
		}
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x0018731C File Offset: 0x0018551C
	public override void HandleSendPower()
	{
		if (base.IsOn)
		{
			if (Time.time > this.lightUpdateTime)
			{
				this.lightUpdateTime = Time.time + 2f;
				this.CheckLightLevel();
			}
			if (this.HasLight)
			{
				if (this.CurrentPower < this.MaxPower)
				{
					this.TickPowerGeneration();
				}
				else if (this.CurrentPower > this.MaxPower)
				{
					this.CurrentPower = this.MaxPower;
				}
				if (this.ShouldAutoTurnOff())
				{
					this.CurrentPower = 0;
					base.IsOn = false;
				}
				if (this.hasChangesLocal)
				{
					this.LastPowerUsed = 0;
					ushort num = (ushort)Mathf.Min((int)this.MaxOutput, (int)this.CurrentPower);
					ushort num2 = num;
					World world = GameManager.Instance.World;
					for (int i = 0; i < this.Children.Count; i++)
					{
						num = num2;
						this.Children[i].HandlePowerReceived(ref num2);
						this.LastPowerUsed += num - num2;
					}
				}
				if (this.LastPowerUsed >= this.CurrentPower)
				{
					base.SendHasLocalChangesToRoot();
					this.CurrentPower = 0;
					return;
				}
				this.CurrentPower -= this.LastPowerUsed;
			}
		}
	}

	// Token: 0x06003CEE RID: 15598 RVA: 0x00187445 File Offset: 0x00185645
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ShouldClearPower()
	{
		return this.sunLight != 15 || !GameManager.Instance.World.IsDaytime();
	}

	// Token: 0x06003CEF RID: 15599 RVA: 0x00187465 File Offset: 0x00185665
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void RefreshPowerStats()
	{
		base.RefreshPowerStats();
		this.MaxPower = this.MaxOutput;
	}

	// Token: 0x06003CF0 RID: 15600 RVA: 0x00187479 File Offset: 0x00185679
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		if (PowerManager.Instance.CurrentFileVersion >= 2)
		{
			this.sunLight = _br.ReadByte();
		}
	}

	// Token: 0x06003CF1 RID: 15601 RVA: 0x0018749C File Offset: 0x0018569C
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.sunLight);
	}

	// Token: 0x04003137 RID: 12599
	public ushort InputFromSun;

	// Token: 0x04003138 RID: 12600
	[PublicizedFrom(EAccessModifier.Private)]
	public byte sunLight;

	// Token: 0x04003139 RID: 12601
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastHasLight;

	// Token: 0x0400313A RID: 12602
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSound = "solarpanel_idle";

	// Token: 0x0400313C RID: 12604
	[PublicizedFrom(EAccessModifier.Private)]
	public float lightUpdateTime;
}
