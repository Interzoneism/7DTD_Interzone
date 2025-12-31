using System;
using UnityEngine;

// Token: 0x0200130F RID: 4879
[Serializable]
public class vp_UnitBankType : vp_ItemType
{
	// Token: 0x04007472 RID: 29810
	[SerializeField]
	public vp_UnitType Unit;

	// Token: 0x04007473 RID: 29811
	[SerializeField]
	public int Capacity = 10;

	// Token: 0x04007474 RID: 29812
	[SerializeField]
	public bool Reloadable = true;

	// Token: 0x04007475 RID: 29813
	[SerializeField]
	public bool RemoveWhenDepleted;
}
