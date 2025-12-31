using System;
using UnityEngine.Scripting;

// Token: 0x02000638 RID: 1592
[Preserve]
public class MinEventActionAddPartTPV : MinEventActionAddPart
{
	// Token: 0x060030C5 RID: 12485 RVA: 0x0014D290 File Offset: 0x0014B490
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		EntityPlayerLocal entityPlayerLocal = _params.Self as EntityPlayerLocal;
		if (entityPlayerLocal == null || entityPlayerLocal.emodel.IsFPV)
		{
			EntityPlayer entityPlayer = _params.Self as EntityPlayer;
			if (entityPlayer == null || !entityPlayer.isEntityRemote)
			{
				return false;
			}
		}
		return base.CanExecute(_eventType, _params);
	}
}
