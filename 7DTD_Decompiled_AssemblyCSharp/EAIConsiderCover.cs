using System;
using UnityEngine.Scripting;

// Token: 0x020003FD RID: 1021
[Preserve]
public class EAIConsiderCover : EAIBase
{
	// Token: 0x06001EC6 RID: 7878 RVA: 0x000BFE5A File Offset: 0x000BE05A
	public EAIConsiderCover()
	{
		this.MutexBits = 1;
		this.ecm = EntityCoverManager.Instance;
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000BFE74 File Offset: 0x000BE074
	public override bool CanExecute()
	{
		if (this.theEntity.sleepingOrWakingUp || this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None || (this.theEntity.Jumping && !this.theEntity.isSwimming))
		{
			return false;
		}
		this.entityTarget = this.theEntity.GetAttackTarget();
		return !(this.entityTarget == null) && !this.ecm.HasCover(this.theEntity.entityId);
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x000BFEF8 File Offset: 0x000BE0F8
	public override bool Continue()
	{
		return base.Continue();
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000BFF00 File Offset: 0x000BE100
	public override void Update()
	{
		this.entityTarget == null;
	}

	// Token: 0x04001547 RID: 5447
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x04001548 RID: 5448
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityCoverManager ecm;
}
