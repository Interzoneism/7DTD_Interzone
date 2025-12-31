using System;
using UnityEngine.Scripting;

// Token: 0x02000637 RID: 1591
[Preserve]
public class MinEventActionAddPartFPV : MinEventActionAddPart
{
	// Token: 0x060030C3 RID: 12483 RVA: 0x0014D254 File Offset: 0x0014B454
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		EntityPlayerLocal entityPlayerLocal = _params.Self as EntityPlayerLocal;
		return entityPlayerLocal != null && entityPlayerLocal.emodel.IsFPV && base.CanExecute(_eventType, _params);
	}
}
