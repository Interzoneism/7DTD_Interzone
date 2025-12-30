using System;
using UnityEngine;

// Token: 0x0200048A RID: 1162
public class CollisionCallForward : MonoBehaviour
{
	// Token: 0x060025E5 RID: 9701 RVA: 0x000F3EA4 File Offset: 0x000F20A4
	public void OnCollisionEnter(Collision collision)
	{
		if (this.Entity != null)
		{
			this.Entity.OnCollisionForward(base.transform, collision, false);
		}
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x000F3EC7 File Offset: 0x000F20C7
	public void OnCollisionStay(Collision collision)
	{
		if (this.Entity != null)
		{
			this.Entity.OnCollisionForward(base.transform, collision, true);
		}
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000F3EEC File Offset: 0x000F20EC
	public static Entity FindEntity(Transform _t)
	{
		Entity component = _t.GetComponent<Entity>();
		if (component)
		{
			return component;
		}
		CollisionCallForward componentInParent = _t.GetComponentInParent<CollisionCallForward>();
		if (componentInParent)
		{
			return componentInParent.Entity;
		}
		return null;
	}

	// Token: 0x04001CB6 RID: 7350
	public Entity Entity;
}
