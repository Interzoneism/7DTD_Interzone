using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000403 RID: 1027
[Preserve]
public class EAIWander : EAIBase
{
	// Token: 0x06001EE4 RID: 7908 RVA: 0x000BAB17 File Offset: 0x000B8D17
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000C0570 File Offset: 0x000BE770
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "exePer", ref this.executePercent);
		base.GetData(data, "fade", ref this.fade);
		base.GetData(data, "lookMin", ref this.lookMin);
		base.GetData(data, "lookMax", ref this.lookMax);
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000C05CC File Offset: 0x000BE7CC
	public override bool CanExecute()
	{
		if (this.theEntity.sleepingOrWakingUp)
		{
			return false;
		}
		if (this.manager.lookTime > 0f)
		{
			return false;
		}
		if (this.fade == 1f && this.theEntity.GetTicksNoPlayerAdjacent() >= 120)
		{
			return false;
		}
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		bool isAlert = this.theEntity.IsAlert;
		if (!isAlert && this.executePercent * this.executeWaitTime <= base.RandomFloat)
		{
			return false;
		}
		int minXZ = 1;
		int num = (int)this.manager.interestDistance;
		if (isAlert)
		{
			minXZ = 2;
			num *= 2;
		}
		Vector3 dirV = (base.RandomFloat < 0.6f) ? this.theEntity.GetForwardVector() : base.Random.RandomOnUnitCircleXZ;
		Vector3 vector = RandomPositionGenerator.CalcInDir(this.theEntity, minXZ, num, num, dirV, 90f);
		if (vector.y == 0f)
		{
			return false;
		}
		this.position = vector;
		return true;
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000C06C1 File Offset: 0x000BE8C1
	public override void Start()
	{
		this.time = 0f;
		this.theEntity.FindPath(this.position, this.theEntity.GetMoveSpeed(), false, this);
		this.theEntity.renderFadeMax = this.fade;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000C0700 File Offset: 0x000BE900
	public override bool Continue()
	{
		return this.theEntity.bodyDamage.CurrentStun == EnumEntityStunType.None && this.theEntity.moveHelper.BlockedTime <= 0.3f && this.time <= 30f && !this.theEntity.navigator.noPathAndNotPlanningOne();
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000C075C File Offset: 0x000BE95C
	public override void Update()
	{
		this.time += 0.05f;
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000C0770 File Offset: 0x000BE970
	public override void Reset()
	{
		this.manager.lookTime = base.Random.RandomRange(this.lookMin, this.lookMax);
		this.theEntity.moveHelper.Stop();
		this.theEntity.renderFadeMax = 1f;
	}

	// Token: 0x04001558 RID: 5464
	[PublicizedFrom(EAccessModifier.Private)]
	public float fade = 1f;

	// Token: 0x04001559 RID: 5465
	[PublicizedFrom(EAccessModifier.Private)]
	public float lookMin = 0.5f;

	// Token: 0x0400155A RID: 5466
	[PublicizedFrom(EAccessModifier.Private)]
	public float lookMax = 5f;

	// Token: 0x0400155B RID: 5467
	[PublicizedFrom(EAccessModifier.Private)]
	public float executePercent = 0.2f;

	// Token: 0x0400155C RID: 5468
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x0400155D RID: 5469
	[PublicizedFrom(EAccessModifier.Private)]
	public float time;
}
