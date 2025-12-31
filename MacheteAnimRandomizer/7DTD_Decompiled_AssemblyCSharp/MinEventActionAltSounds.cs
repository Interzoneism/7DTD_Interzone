using System;
using System.Xml.Linq;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000654 RID: 1620
[Preserve]
public class MinEventActionAltSounds : MinEventActionTargetedBase
{
	// Token: 0x06003134 RID: 12596 RVA: 0x0014F5AC File Offset: 0x0014D7AC
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i] is EntityPlayerLocal)
			{
				EntityVehicle entityVehicle = this.targets[i].AttachedToEntity as EntityVehicle;
				if (entityVehicle != null)
				{
					entityVehicle.vehicle.FireEvent(Vehicle.Event.Stop);
					Manager.Instance.bUseAltSounds = this.enabled;
					entityVehicle.vehicle.FireEvent(Vehicle.Event.Start);
				}
				else
				{
					Manager.Instance.bUseAltSounds = this.enabled;
				}
			}
		}
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x0014F63C File Offset: 0x0014D83C
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "enabled")
		{
			this.enabled = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
		}
		return flag;
	}

	// Token: 0x04002789 RID: 10121
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool enabled;
}
