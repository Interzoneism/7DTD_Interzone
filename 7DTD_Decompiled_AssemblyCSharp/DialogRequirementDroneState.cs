using System;
using UnityEngine.Scripting;

// Token: 0x020002B0 RID: 688
[Preserve]
public class DialogRequirementDroneState : BaseDialogRequirement
{
	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06001359 RID: 4953 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.DroneState;
		}
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x000768E4 File Offset: 0x00074AE4
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		EntityDrone entityDrone = talkingTo as EntityDrone;
		if (entityDrone)
		{
			EntityDrone.Orders orders;
			if (Enum.TryParse<EntityDrone.Orders>(base.Value, out orders))
			{
				return entityDrone.OrderState == orders;
			}
			EntityDrone.AllyHealMode allyHealMode;
			if (Enum.TryParse<EntityDrone.AllyHealMode>(base.Value, out allyHealMode) && entityDrone.IsHealModAttached)
			{
				return entityDrone.HealAllyMode == allyHealMode;
			}
			bool flag = entityDrone.TargetCanBeHealed(player);
			if (flag)
			{
				return flag;
			}
		}
		return false;
	}
}
