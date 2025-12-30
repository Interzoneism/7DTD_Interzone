using System;
using UnityEngine.Scripting;

// Token: 0x02000EA1 RID: 3745
[Preserve]
public class XUiC_VehicleStats : XUiController
{
	// Token: 0x17000C10 RID: 3088
	// (get) Token: 0x06007640 RID: 30272 RVA: 0x00302374 File Offset: 0x00300574
	// (set) Token: 0x06007641 RID: 30273 RVA: 0x0030237C File Offset: 0x0030057C
	public EntityVehicle Vehicle
	{
		get
		{
			return this.vehicle;
		}
		set
		{
			this.vehicle = value;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06007642 RID: 30274 RVA: 0x0030238C File Offset: 0x0030058C
	public override void Init()
	{
		base.Init();
		this.sprFuelFill = (XUiV_Sprite)base.GetChildById("sprFuelFill").ViewComponent;
		this.sprFillPotential = (XUiV_Sprite)base.GetChildById("sprFillPotential").ViewComponent;
		this.sprFillPotential.Fill = 0f;
		this.btnRefuel = base.GetChildById("btnRefuel");
		this.btnRefuel_Background = (XUiV_Button)this.btnRefuel.GetChildById("clickable").ViewComponent;
		this.btnRefuel_Background.Controller.OnPress += this.BtnRefuel_OnPress;
		this.btnRefuel_Background.Controller.OnHover += this.btnRefuel_OnHover;
		this.isDirty = true;
	}

	// Token: 0x06007643 RID: 30275 RVA: 0x00302455 File Offset: 0x00300655
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnRefuel_OnHover(XUiController _sender, bool _isOver)
	{
		this.RefuelButtonHovered = _isOver;
		base.RefreshBindings(false);
	}

	// Token: 0x06007644 RID: 30276 RVA: 0x00302465 File Offset: 0x00300665
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRefuel_OnPress(XUiController _sender, int _mouseButton)
	{
		if (base.xui.vehicle.AddFuelFromInventory(base.xui.playerUI.entityPlayer))
		{
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06007645 RID: 30277 RVA: 0x00302490 File Offset: 0x00300690
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1246567232U)
		{
			if (num <= 603484716U)
			{
				if (num <= 90731405U)
				{
					if (num != 30055014U)
					{
						if (num == 90731405U)
						{
							if (bindingName == "fuel")
							{
								value = this.fuelFormatter.Format((int)XUiM_Vehicle.GetFuelLevel(base.xui));
								return true;
							}
						}
					}
					else if (bindingName == "storage")
					{
						value = "BASKET";
						return true;
					}
				}
				else if (num != 461899258U)
				{
					if (num == 603484716U)
					{
						if (bindingName == "vehiclename")
						{
							value = Localization.Get(XUiM_Vehicle.GetEntityName(base.xui), false);
							return true;
						}
					}
				}
				else if (bindingName == "protection")
				{
					value = this.protectionFormatter.Format((int)XUiM_Vehicle.GetProtection(base.xui));
					return true;
				}
			}
			else if (num <= 659620374U)
			{
				if (num != 609690980U)
				{
					if (num == 659620374U)
					{
						if (bindingName == "passengerstitle")
						{
							value = Localization.Get("xuiSeats", false);
							return true;
						}
					}
				}
				else if (bindingName == "vehiclestatstitle")
				{
					value = Localization.Get("xuiStats", false);
					return true;
				}
			}
			else if (num != 804398480U)
			{
				if (num == 1246567232U)
				{
					if (bindingName == "fuelfill")
					{
						value = this.fuelFillFormatter.Format(XUiM_Vehicle.GetFuelFill(base.xui));
						return true;
					}
				}
			}
			else if (bindingName == "potentialfuelfill")
			{
				if (!this.RefuelButtonHovered)
				{
					value = "0";
				}
				else
				{
					Vehicle vehicle = base.xui.vehicle.GetVehicle();
					value = this.potentialFuelFillFormatter.Format(vehicle.GetFuelPercent());
				}
				return true;
			}
		}
		else if (num <= 2420381393U)
		{
			if (num <= 2072037248U)
			{
				if (num != 2015621088U)
				{
					if (num == 2072037248U)
					{
						if (bindingName == "speed")
						{
							value = this.speedFormatter.Format((int)XUiM_Vehicle.GetSpeed(base.xui));
							return true;
						}
					}
				}
				else if (bindingName == "passengers")
				{
					value = this.passengersFormatter.Format(XUiM_Vehicle.GetPassengers(base.xui));
					return true;
				}
			}
			else if (num != 2224025142U)
			{
				if (num == 2420381393U)
				{
					if (bindingName == "noise")
					{
						value = XUiM_Vehicle.GetNoise(base.xui);
						return true;
					}
				}
			}
			else if (bindingName == "speedtitle")
			{
				value = Localization.Get("xuiSpeed", false);
				return true;
			}
		}
		else if (num <= 2673357751U)
		{
			if (num != 2662421117U)
			{
				if (num == 2673357751U)
				{
					if (bindingName == "speedtext")
					{
						value = XUiM_Vehicle.GetSpeedText(base.xui);
						return true;
					}
				}
			}
			else if (bindingName == "noisetitle")
			{
				value = Localization.Get("xuiNoise", false);
				return true;
			}
		}
		else if (num != 2956837708U)
		{
			if (num != 3776886369U)
			{
				if (num == 4165243794U)
				{
					if (bindingName == "locktype")
					{
						value = Localization.Get("none", false);
						return true;
					}
				}
			}
			else if (bindingName == "fueltitle")
			{
				value = Localization.Get("xuiGas", false);
				return true;
			}
		}
		else if (bindingName == "protectiontitle")
		{
			value = Localization.Get("xuiDefense", false);
			return true;
		}
		return false;
	}

	// Token: 0x06007646 RID: 30278 RVA: 0x00302886 File Offset: 0x00300A86
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.Vehicle == null)
		{
			return;
		}
		base.Update(_dt);
	}

	// Token: 0x06007647 RID: 30279 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x04005A29 RID: 23081
	[PublicizedFrom(EAccessModifier.Private)]
	public bool RefuelButtonHovered;

	// Token: 0x04005A2A RID: 23082
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite sprFuelFill;

	// Token: 0x04005A2B RID: 23083
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite sprFillPotential;

	// Token: 0x04005A2C RID: 23084
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnRefuel;

	// Token: 0x04005A2D RID: 23085
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnRefuel_Background;

	// Token: 0x04005A2E RID: 23086
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityVehicle vehicle;

	// Token: 0x04005A2F RID: 23087
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04005A30 RID: 23088
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt speedFormatter = new CachedStringFormatterInt();

	// Token: 0x04005A31 RID: 23089
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt protectionFormatter = new CachedStringFormatterInt();

	// Token: 0x04005A32 RID: 23090
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt fuelFormatter = new CachedStringFormatterInt();

	// Token: 0x04005A33 RID: 23091
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt passengersFormatter = new CachedStringFormatterInt();

	// Token: 0x04005A34 RID: 23092
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat potentialFuelFillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04005A35 RID: 23093
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat fuelFillFormatter = new CachedStringFormatterFloat(null);
}
