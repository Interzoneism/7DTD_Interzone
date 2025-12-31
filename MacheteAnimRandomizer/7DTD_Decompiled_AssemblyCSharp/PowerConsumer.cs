using System;
using Audio;

// Token: 0x0200083E RID: 2110
public class PowerConsumer : PowerItem
{
	// Token: 0x06003C96 RID: 15510 RVA: 0x00185CF8 File Offset: 0x00183EF8
	public override void HandlePowerUpdate(bool isOn)
	{
		bool flag = this.isPowered && isOn;
		if (this.TileEntity != null)
		{
			this.TileEntity.Activate(flag);
			if (flag && this.lastActivate != flag)
			{
				this.TileEntity.ActivateOnce();
			}
			this.TileEntity.SetModified();
		}
		this.lastActivate = flag;
		if (this.PowerChildren())
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandlePowerUpdate(isOn);
			}
		}
	}

	// Token: 0x06003C97 RID: 15511 RVA: 0x00185D80 File Offset: 0x00183F80
	public override void SetValuesFromBlock()
	{
		base.SetValuesFromBlock();
		Block block = Block.list[(int)this.BlockID];
		if (block.Properties.Values.ContainsKey("RequiredPower"))
		{
			this.RequiredPower = ushort.Parse(block.Properties.Values["RequiredPower"]);
		}
		if (block.Properties.Values.ContainsKey("StartSound"))
		{
			this.StartSound = block.Properties.Values["StartSound"];
		}
		if (block.Properties.Values.ContainsKey("EndSound"))
		{
			this.EndSound = block.Properties.Values["EndSound"];
		}
	}

	// Token: 0x06003C98 RID: 15512 RVA: 0x00185E3B File Offset: 0x0018403B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void IsPoweredChanged(bool newPowered)
	{
		Manager.BroadcastPlay(this.Position.ToVector3(), newPowered ? this.StartSound : this.EndSound, 0f);
	}

	// Token: 0x04003102 RID: 12546
	[PublicizedFrom(EAccessModifier.Protected)]
	public string StartSound = "";

	// Token: 0x04003103 RID: 12547
	[PublicizedFrom(EAccessModifier.Protected)]
	public string EndSound = "";

	// Token: 0x04003104 RID: 12548
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lastActivate;
}
