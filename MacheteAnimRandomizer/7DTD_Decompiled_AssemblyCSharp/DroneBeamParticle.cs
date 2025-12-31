using System;
using UnityEngine;

// Token: 0x02000392 RID: 914
public class DroneBeamParticle : MonoBehaviour
{
	// Token: 0x06001B4A RID: 6986 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x000AB060 File Offset: 0x000A9260
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!this.drone)
		{
			this.drone = base.GetComponentInParent<EntityDrone>();
			Transform transform = base.transform.parent.FindInChilds("WristLeft", false);
			if (transform)
			{
				base.transform.SetParent(transform, false);
			}
			return;
		}
		this.displayTime -= Time.deltaTime;
		EntityAlive attackTargetLocal = this.drone.GetAttackTargetLocal();
		if (attackTargetLocal)
		{
			Vector3 chestPosition = attackTargetLocal.getChestPosition();
			this.root.transform.rotation = Quaternion.LookRotation(chestPosition - this.drone.HealArmPosition);
			if (this.displayTime <= 0f || (attackTargetLocal && attackTargetLocal.IsDead()))
			{
				UnityEngine.Object.Destroy(this.root);
			}
		}
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x000AB12E File Offset: 0x000A932E
	public void SetDisplayTime(float time)
	{
		this.displayTime = time;
	}

	// Token: 0x0400122E RID: 4654
	public GameObject root;

	// Token: 0x0400122F RID: 4655
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone drone;

	// Token: 0x04001230 RID: 4656
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float displayTime;
}
