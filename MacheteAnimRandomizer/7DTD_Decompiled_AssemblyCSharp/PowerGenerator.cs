using System;
using System.Globalization;
using System.IO;

// Token: 0x02000841 RID: 2113
public class PowerGenerator : PowerSource
{
	// Token: 0x1700063E RID: 1598
	// (get) Token: 0x06003CA6 RID: 15526 RVA: 0x00075E2B File Offset: 0x0007402B
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.Generator;
		}
	}

	// Token: 0x1700063F RID: 1599
	// (get) Token: 0x06003CA7 RID: 15527 RVA: 0x001860BD File Offset: 0x001842BD
	public override string OnSound
	{
		get
		{
			return "generator_start";
		}
	}

	// Token: 0x17000640 RID: 1600
	// (get) Token: 0x06003CA8 RID: 15528 RVA: 0x001860C4 File Offset: 0x001842C4
	public override string OffSound
	{
		get
		{
			return "generator_stop";
		}
	}

	// Token: 0x06003CA9 RID: 15529 RVA: 0x001860CB File Offset: 0x001842CB
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		this.CurrentFuel = _br.ReadUInt16();
	}

	// Token: 0x06003CAA RID: 15530 RVA: 0x001860E1 File Offset: 0x001842E1
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.CurrentFuel);
	}

	// Token: 0x06003CAB RID: 15531 RVA: 0x001860F6 File Offset: 0x001842F6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool ShouldAutoTurnOff()
	{
		return this.CurrentFuel <= 0;
	}

	// Token: 0x06003CAC RID: 15532 RVA: 0x00186104 File Offset: 0x00184304
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void TickPowerGeneration()
	{
		if ((float)(this.MaxPower - this.CurrentPower) >= this.OutputPerFuel && this.CurrentFuel > 0)
		{
			this.CurrentFuel -= 1;
			this.CurrentPower += (ushort)this.OutputPerFuel;
		}
	}

	// Token: 0x06003CAD RID: 15533 RVA: 0x00186154 File Offset: 0x00184354
	public override void SetValuesFromBlock()
	{
		base.SetValuesFromBlock();
		Block block = Block.list[(int)this.BlockID];
		if (block.Properties.Values.ContainsKey("MaxPower"))
		{
			this.MaxPower = ushort.Parse(block.Properties.Values["MaxPower"]);
		}
		if (block.Properties.Values.ContainsKey("MaxFuel"))
		{
			this.MaxFuel = ushort.Parse(block.Properties.Values["MaxFuel"]);
		}
		else
		{
			this.MaxFuel = 1000;
		}
		if (block.Properties.Values.ContainsKey("OutputPerFuel"))
		{
			this.OutputPerFuel = StringParsers.ParseFloat(block.Properties.Values["OutputPerFuel"], 0, -1, NumberStyles.Any);
			return;
		}
		this.OutputPerFuel = 100f;
	}

	// Token: 0x04003107 RID: 12551
	public ushort CurrentFuel;

	// Token: 0x04003108 RID: 12552
	public ushort MaxFuel;

	// Token: 0x04003109 RID: 12553
	public float OutputPerFuel;
}
