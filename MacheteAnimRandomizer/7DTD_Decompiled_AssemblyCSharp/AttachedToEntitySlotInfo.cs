using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003A4 RID: 932
[Preserve]
public class AttachedToEntitySlotInfo
{
	// Token: 0x0400128D RID: 4749
	public int slotIdx;

	// Token: 0x0400128E RID: 4750
	public Transform enterParentTransform;

	// Token: 0x0400128F RID: 4751
	public Vector3 enterPosition;

	// Token: 0x04001290 RID: 4752
	public Vector3 enterRotation;

	// Token: 0x04001291 RID: 4753
	public Vector2 pitchRestriction;

	// Token: 0x04001292 RID: 4754
	public Vector2 yawRestriction;

	// Token: 0x04001293 RID: 4755
	public bool bKeep3rdPersonModelVisible;

	// Token: 0x04001294 RID: 4756
	public bool bAllow3rdPerson;

	// Token: 0x04001295 RID: 4757
	public bool bReplaceLocalInventory;

	// Token: 0x04001296 RID: 4758
	public List<AttachedToEntitySlotExit> exits = new List<AttachedToEntitySlotExit>();
}
