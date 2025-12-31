using System;
using GamePath;
using UnityEngine.Scripting;

// Token: 0x020003FE RID: 1022
[Preserve]
public class EAIInCover : EAIBase
{
	// Token: 0x06001ECA RID: 7882 RVA: 0x000BFF0F File Offset: 0x000BE10F
	public EAIInCover()
	{
		this.MutexBits = 1;
		this.ecm = EntityCoverManager.Instance;
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000BFF29 File Offset: 0x000BE129
	public override void Start()
	{
		this.coverTicks = 60f;
		PathFinderThread.Instance.RemovePathsFor(this.theEntity.entityId);
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000BFF4C File Offset: 0x000BE14C
	public override bool CanExecute()
	{
		return !this.theEntity.sleepingOrWakingUp && this.theEntity.bodyDamage.CurrentStun == EnumEntityStunType.None && (!this.theEntity.Jumping || this.theEntity.isSwimming) && this.ecm.HasCover(this.theEntity.entityId);
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x000BFFAF File Offset: 0x000BE1AF
	public override bool Continue()
	{
		return this.ecm.HasCover(this.theEntity.entityId);
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x000BFFCC File Offset: 0x000BE1CC
	public override void Update()
	{
		if (!this.ecm.HasCover(this.theEntity.entityId))
		{
			return;
		}
		if (this.ecm.GetCoverPos(this.theEntity.entityId) == null)
		{
			return;
		}
		if (this.coverTicks > 0f)
		{
			this.coverTicks -= 1f;
			if (this.coverTicks <= 0f)
			{
				if (base.Random.RandomRange(2) < 1)
				{
					this.freeCover();
					return;
				}
				this.coverTicks = 60f;
			}
		}
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x000C0058 File Offset: 0x000BE258
	[PublicizedFrom(EAccessModifier.Private)]
	public void freeCover()
	{
		this.ecm.FreeCover(this.theEntity.entityId);
		this.coverTicks = 60f;
	}

	// Token: 0x04001549 RID: 5449
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x0400154A RID: 5450
	[PublicizedFrom(EAccessModifier.Private)]
	public float coverTicks;

	// Token: 0x0400154B RID: 5451
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityCoverManager ecm;
}
