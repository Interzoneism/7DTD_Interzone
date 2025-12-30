using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000B36 RID: 2870
[Preserve]
public class VPFuelTank : VehiclePart
{
	// Token: 0x0600592B RID: 22827 RVA: 0x0023F2CC File Offset: 0x0023D4CC
	public VPFuelTank()
	{
		this.fuelLevel = 0f;
	}

	// Token: 0x0600592C RID: 22828 RVA: 0x0023F2E0 File Offset: 0x0023D4E0
	public override void SetProperties(DynamicProperties _properties)
	{
		base.SetProperties(_properties);
		StringParsers.TryParseFloat(base.GetProperty("capacity"), out this.fuelCapacity, 0, -1, NumberStyles.Any);
		if (this.fuelCapacity < 1f)
		{
			this.fuelCapacity = 1f;
		}
		this.fuelLevel = this.fuelCapacity;
	}

	// Token: 0x0600592D RID: 22829 RVA: 0x0023F336 File Offset: 0x0023D536
	public override void HandleEvent(VehiclePart.Event _event, VehiclePart _part, float _amount)
	{
		if (_event == VehiclePart.Event.FuelRemove)
		{
			this.AddFuel(-_amount);
		}
	}

	// Token: 0x0600592E RID: 22830 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsBroken()
	{
		return false;
	}

	// Token: 0x0600592F RID: 22831 RVA: 0x0023F344 File Offset: 0x0023D544
	public float GetFuelLevel()
	{
		if (this.IsBroken())
		{
			return 0f;
		}
		return this.fuelLevel;
	}

	// Token: 0x06005930 RID: 22832 RVA: 0x0023F35A File Offset: 0x0023D55A
	public float GetMaxFuelLevel()
	{
		if (this.IsBroken())
		{
			return 0f;
		}
		return this.fuelCapacity * this.vehicle.EffectFuelMaxPer;
	}

	// Token: 0x06005931 RID: 22833 RVA: 0x0023F37C File Offset: 0x0023D57C
	public float GetFuelLevelPercent()
	{
		if (this.IsBroken())
		{
			return 0f;
		}
		float num = this.fuelLevel / (this.fuelCapacity * this.vehicle.EffectFuelMaxPer);
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	// Token: 0x06005932 RID: 22834 RVA: 0x0023F3C0 File Offset: 0x0023D5C0
	public void SetFuelLevel(float _fuelLevel)
	{
		if (_fuelLevel <= 0f)
		{
			this.fuelLevel = 0f;
			this.vehicle.FireEvent(VehiclePart.Event.FuelEmpty, this, 0f);
			return;
		}
		float num = this.fuelCapacity * this.vehicle.EffectFuelMaxPer;
		if (_fuelLevel > num)
		{
			_fuelLevel = num;
		}
		this.fuelLevel = _fuelLevel;
	}

	// Token: 0x06005933 RID: 22835 RVA: 0x0023F414 File Offset: 0x0023D614
	public void AddFuel(float _amount)
	{
		this.SetFuelLevel(this.fuelLevel + _amount);
	}

	// Token: 0x04004427 RID: 17447
	[PublicizedFrom(EAccessModifier.Private)]
	public float fuelCapacity;

	// Token: 0x04004428 RID: 17448
	[PublicizedFrom(EAccessModifier.Private)]
	public float fuelLevel;
}
