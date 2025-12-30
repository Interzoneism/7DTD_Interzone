using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
public class ExplosionDamageArea : MonoBehaviour
{
	// Token: 0x060027F7 RID: 10231 RVA: 0x00103711 File Offset: 0x00101911
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x00103728 File Offset: 0x00101928
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity getEntityFromCollider(Collider col)
	{
		Transform transform = col.transform;
		if (!transform.tag.StartsWith("E_") && !transform.CompareTag("Item"))
		{
			return null;
		}
		if (transform.CompareTag("Item"))
		{
			return null;
		}
		Transform transform2 = null;
		if (transform.tag.StartsWith("E_BP_"))
		{
			transform2 = GameUtils.GetHitRootTransform(transform.tag, transform);
		}
		EntityAlive entityAlive = (transform2 != null) ? transform2.GetComponent<EntityAlive>() : null;
		if (entityAlive == null || entityAlive.IsDead())
		{
			return null;
		}
		return entityAlive;
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x001037B4 File Offset: 0x001019B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		EntityAlive entityAlive = this.getEntityFromCollider(other) as EntityAlive;
		if (entityAlive == null)
		{
			return;
		}
		if (this.BuffActions != null)
		{
			for (int i = 0; i < this.BuffActions.Count; i++)
			{
				entityAlive.Buffs.AddBuff(this.BuffActions[i], this.InitiatorEntityId, entityAlive.isEntityRemote, false, -1f);
			}
		}
	}

	// Token: 0x04001EAA RID: 7850
	public List<string> BuffActions;

	// Token: 0x04001EAB RID: 7851
	public int InitiatorEntityId = -1;
}
