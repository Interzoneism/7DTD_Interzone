using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class HazardDamageController : MonoBehaviour
{
	// Token: 0x0600052C RID: 1324 RVA: 0x00024D18 File Offset: 0x00022F18
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!this.IsActive)
		{
			if (this.CollidersThisFrame != null && this.CollidersThisFrame.Count > 0)
			{
				this.CollidersThisFrame.Clear();
			}
			return;
		}
		if (this.CollidersThisFrame == null || this.CollidersThisFrame.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.CollidersThisFrame.Count; i++)
		{
			this.touched(this.CollidersThisFrame[i]);
		}
		this.CollidersThisFrame.Clear();
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x00024D98 File Offset: 0x00022F98
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00024D98 File Offset: 0x00022F98
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerStay(Collider other)
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x00024D98 File Offset: 0x00022F98
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerExit(Collider other)
	{
		if (!this.IsActive)
		{
			return;
		}
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x00024DD0 File Offset: 0x00022FD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void touched(Collider collider)
	{
		if (!this.IsActive || collider == null)
		{
			return;
		}
		Transform transform = collider.transform;
		if (transform != null)
		{
			EntityAlive entityAlive = transform.GetComponent<EntityAlive>();
			if (entityAlive == null)
			{
				entityAlive = transform.GetComponentInParent<EntityAlive>();
			}
			if (entityAlive == null && transform.parent != null)
			{
				entityAlive = transform.parent.GetComponentInChildren<EntityAlive>();
			}
			if (entityAlive == null)
			{
				entityAlive = transform.GetComponentInChildren<EntityAlive>();
			}
			if (entityAlive != null && entityAlive.IsAlive() && this.buffActions != null)
			{
				for (int i = 0; i < this.buffActions.Count; i++)
				{
					if (entityAlive.emodel != null && entityAlive.emodel.transform != null && !entityAlive.Buffs.HasBuff(this.buffActions[i]))
					{
						entityAlive.Buffs.AddBuff(this.buffActions[i], -1, true, false, -1f);
					}
				}
			}
		}
	}

	// Token: 0x040005E1 RID: 1505
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Collider> CollidersThisFrame;

	// Token: 0x040005E2 RID: 1506
	public bool IsActive;

	// Token: 0x040005E3 RID: 1507
	public List<string> buffActions;
}
