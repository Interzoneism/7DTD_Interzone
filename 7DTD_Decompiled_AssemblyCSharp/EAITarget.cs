using System;
using UnityEngine.Scripting;

// Token: 0x020003FF RID: 1023
[Preserve]
public abstract class EAITarget : EAIBase
{
	// Token: 0x06001ED0 RID: 7888 RVA: 0x000C007B File Offset: 0x000BE27B
	public void Init(EntityAlive _theEntity, float _maxXZDistance, bool _bNeedToSee)
	{
		base.Init(_theEntity);
		this.seeCounter = 0;
		this.maxXZDistance = _maxXZDistance;
		this.bNeedToSee = _bNeedToSee;
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000C0099 File Offset: 0x000BE299
	public override void Start()
	{
		this.seeCounter = 0;
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000C00A4 File Offset: 0x000BE2A4
	public override bool Continue()
	{
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (attackTarget == null)
		{
			return false;
		}
		if (!attackTarget.IsAlive())
		{
			return false;
		}
		if (this.maxXZDistance > 0f && this.theEntity.GetDistanceSq(attackTarget) > this.maxXZDistance * this.maxXZDistance)
		{
			return false;
		}
		if (this.bNeedToSee)
		{
			if (!this.theEntity.CanSee(attackTarget))
			{
				int num = this.seeCounter + 1;
				this.seeCounter = num;
				if (num > 600)
				{
					return false;
				}
			}
			else
			{
				this.seeCounter = 0;
			}
		}
		return true;
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000C0134 File Offset: 0x000BE334
	public override void Reset()
	{
		this.theEntity.SetAttackTarget(null, 0);
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x000C0144 File Offset: 0x000BE344
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool check(EntityAlive _e)
	{
		if (_e == null)
		{
			return false;
		}
		if (_e == this.theEntity)
		{
			return false;
		}
		if (!_e.IsAlive())
		{
			return false;
		}
		if (_e.IsIgnoredByAI())
		{
			return false;
		}
		Vector3i vector3i = World.worldToBlockPos(_e.position);
		if (!this.theEntity.isWithinHomeDistance(vector3i.x, vector3i.y, vector3i.z))
		{
			return false;
		}
		if (this.bNeedToSee && !this.theEntity.CanSee(_e))
		{
			return false;
		}
		EntityPlayer entityPlayer = _e as EntityPlayer;
		return !(entityPlayer != null) || this.theEntity.CanSeeStealth(this.manager.GetSeeDistance(entityPlayer), entityPlayer.Stealth.lightLevel);
	}

	// Token: 0x06001ED5 RID: 7893 RVA: 0x000BA39C File Offset: 0x000B859C
	[PublicizedFrom(EAccessModifier.Protected)]
	public EAITarget()
	{
	}

	// Token: 0x0400154C RID: 5452
	[PublicizedFrom(EAccessModifier.Protected)]
	public float maxXZDistance;

	// Token: 0x0400154D RID: 5453
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bNeedToSee;

	// Token: 0x0400154E RID: 5454
	[PublicizedFrom(EAccessModifier.Private)]
	public int seeCounter;
}
