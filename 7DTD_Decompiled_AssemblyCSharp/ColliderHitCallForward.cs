using System;
using UnityEngine;

// Token: 0x02000489 RID: 1161
public class ColliderHitCallForward : MonoBehaviour
{
	// Token: 0x060025E3 RID: 9699 RVA: 0x000F3E88 File Offset: 0x000F2088
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (this.Entity != null)
		{
			this.Entity.OnControllerColliderHit(hit);
		}
	}

	// Token: 0x04001CB5 RID: 7349
	public Entity Entity;
}
