using System;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class ParticleLifetimeSwitch : MonoBehaviour
{
	// Token: 0x0600261F RID: 9759 RVA: 0x000F6B8B File Offset: 0x000F4D8B
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		this.entityRoot = RootTransformRefEntity.FindEntityUpwards(base.transform);
		this.particles = base.transform.GetComponent<ParticleSystem>();
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x000F6BB0 File Offset: 0x000F4DB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.bFirstUpdate)
		{
			this.bFirstUpdate = false;
			this.entity = ((this.entityRoot != null) ? this.entityRoot.GetComponent<Entity>() : null);
			if (this.entity && this.entity.IsDead())
			{
				if (this.particles != null)
				{
					this.particles.emission.enabled = false;
					this.particles = null;
				}
				this.entity = null;
			}
		}
		if (this.particles && !this.entity)
		{
			this.delay -= Time.deltaTime;
			if (this.delay <= 0f)
			{
				this.particles.emission.enabled = false;
				this.particles = null;
			}
		}
		if (this.entity && this.entity.IsDead())
		{
			this.delay = this.TurnOffDelayAfterEntityDies;
			this.entity = null;
		}
	}

	// Token: 0x04001D0B RID: 7435
	public float TurnOffDelayAfterEntityDies = 5f;

	// Token: 0x04001D0C RID: 7436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform entityRoot;

	// Token: 0x04001D0D RID: 7437
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity entity;

	// Token: 0x04001D0E RID: 7438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ParticleSystem particles;

	// Token: 0x04001D0F RID: 7439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float delay;

	// Token: 0x04001D10 RID: 7440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bFirstUpdate = true;
}
