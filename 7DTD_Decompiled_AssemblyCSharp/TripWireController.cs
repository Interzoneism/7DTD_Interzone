using System;
using UnityEngine;

// Token: 0x02000390 RID: 912
public class TripWireController : MonoBehaviour
{
	// Token: 0x06001B3C RID: 6972 RVA: 0x00002914 File Offset: 0x00000B14
	public void Init(DynamicProperties _properties)
	{
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x000AAD5D File Offset: 0x000A8F5D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		this.checkIfTriggered(other);
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x000AAD5D File Offset: 0x000A8F5D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerStay(Collider other)
	{
		this.checkIfTriggered(other);
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x000AAD68 File Offset: 0x000A8F68
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkIfTriggered(Collider other)
	{
		if (this.TileEntityParent == null || this.WireNode == null)
		{
			return;
		}
		EntityAlive entityAlive = other.transform.GetComponent<EntityAlive>();
		if (entityAlive == null)
		{
			entityAlive = other.transform.GetComponentInParent<EntityAlive>();
		}
		if (entityAlive == null)
		{
			entityAlive = other.transform.parent.GetComponentInChildren<EntityAlive>();
		}
		if (entityAlive == null)
		{
			entityAlive = other.transform.GetComponentInChildren<EntityAlive>();
		}
		if (entityAlive != null && entityAlive as EntityVehicle != null && !(entityAlive as EntityVehicle).HasDriver)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.TileEntityParent.IsPowered)
		{
			this.TileEntityChild.IsTriggered = true;
		}
	}

	// Token: 0x04001223 RID: 4643
	public TileEntityPoweredTrigger TileEntityParent;

	// Token: 0x04001224 RID: 4644
	public TileEntityPoweredTrigger TileEntityChild;

	// Token: 0x04001225 RID: 4645
	public IWireNode WireNode;
}
