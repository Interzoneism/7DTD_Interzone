using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000464 RID: 1124
[Preserve]
public class EntitySwarm : EntityVulture
{
	// Token: 0x06002465 RID: 9317 RVA: 0x000E7B34 File Offset: 0x000E5D34
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Init()
	{
		base.Init();
		this.targetAttackHealthPercent = 1f;
		this.ignoreTargetAttached = true;
		this.wanderHeightRange.x = 1f;
		this.wanderHeightRange.y = 8f;
		this.dissipateDelay = 24f;
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000E7B84 File Offset: 0x000E5D84
	public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (!this.IsDead())
		{
			this.dissipateDelay -= 0.05f;
			if (this.dissipateDelay <= 2f && this.state != EntityVulture.State.Home)
			{
				base.StartHome(this.position + new Vector3(0f, 50f, 0f));
			}
			if (this.dissipateDelay <= 0f)
			{
				this.Kill(DamageResponse.New(true));
			}
		}
	}

	// Token: 0x04001B50 RID: 6992
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dissipateDelay;
}
