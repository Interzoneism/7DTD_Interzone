using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003E4 RID: 996
[Preserve]
public class EAIDodge : EAIBase
{
	// Token: 0x06001E1C RID: 7708 RVA: 0x000BB593 File Offset: 0x000B9793
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.executeDelay = 0.1f;
		this.cooldown = 3f;
		this.actionkDuration = 1f;
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000BB5C0 File Offset: 0x000B97C0
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		string str;
		if (data.TryGetValue("tags", out str))
		{
			this.tags = FastTags<TagGroup.Global>.Parse(str);
		}
		base.GetData(data, "maxXZDistance", ref this.maxXZDistance);
		base.GetData(data, "cooldown", ref this.baseCooldown);
		base.GetData(data, "duration", ref this.actionkDuration);
		base.GetData(data, "minRange", ref this.minRange);
		base.GetData(data, "maxRange", ref this.maxRange);
		base.GetData(data, "unreachableRange", ref this.unreachableRange);
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000BB65C File Offset: 0x000B985C
	public override bool CanExecute()
	{
		if (this.theEntity.IsDancing)
		{
			return false;
		}
		if (this.cooldown > 0f)
		{
			this.cooldown -= this.executeWaitTime;
			return false;
		}
		this.theEntity.world.GetEntitiesInBounds(this.tags, BoundsUtils.ExpandBounds(this.theEntity.boundingBox, this.maxXZDistance, 8f, this.maxXZDistance), EAIDodge.entityList);
		this.entityTarget = null;
		for (int i = 0; i < EAIDodge.entityList.Count; i++)
		{
			EntityAlive entityAlive = EAIDodge.entityList[i] as EntityAlive;
			if (entityAlive && !entityAlive.IsDead() && entityAlive.emodel.avatarController.IsAnimationToDodge())
			{
				this.entityTarget = entityAlive;
				break;
			}
		}
		EAIDodge.entityList.Clear();
		return !(this.entityTarget == null) && this.InRange() && this.theEntity.CanSee(this.entityTarget);
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x000BB763 File Offset: 0x000B9963
	public override void Start()
	{
		this.actionTime = 0f;
		this.theEntity.emodel.avatarController.StartAnimationDodge(base.Random.RandomFloat);
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x000BB790 File Offset: 0x000B9990
	public override bool Continue()
	{
		return this.entityTarget && this.entityTarget.IsAlive() && this.actionTime < this.actionkDuration && this.theEntity.hasBeenAttackedTime <= 0;
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x000BB7D0 File Offset: 0x000B99D0
	public override void Update()
	{
		this.actionTime += 0.05f;
		if (this.actionTime < this.actionkDuration * 0.5f)
		{
			Vector3 headPosition = this.entityTarget.getHeadPosition();
			if (this.theEntity.IsInFrontOfMe(headPosition))
			{
				this.theEntity.SetLookPosition(headPosition);
			}
		}
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Reset()
	{
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x000BB82C File Offset: 0x000B9A2C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool InRange()
	{
		float distanceSq = this.entityTarget.GetDistanceSq(this.theEntity);
		return distanceSq >= this.minRange * this.minRange && distanceSq <= this.maxRange * this.maxRange;
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x000BB870 File Offset: 0x000B9A70
	public override string ToString()
	{
		bool flag = this.entityTarget && this.InRange();
		return string.Format("{0} {1}, inRange{2}, Time {3}", new object[]
		{
			base.ToString(),
			this.entityTarget ? this.entityTarget.EntityName : "",
			flag,
			this.actionTime.ToCultureInvariantString("0.00")
		});
	}

	// Token: 0x040014B7 RID: 5303
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tags;

	// Token: 0x040014B8 RID: 5304
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxXZDistance = 100f;

	// Token: 0x040014B9 RID: 5305
	[PublicizedFrom(EAccessModifier.Private)]
	public float baseCooldown;

	// Token: 0x040014BA RID: 5306
	[PublicizedFrom(EAccessModifier.Private)]
	public float cooldown;

	// Token: 0x040014BB RID: 5307
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x040014BC RID: 5308
	[PublicizedFrom(EAccessModifier.Private)]
	public float actionTime;

	// Token: 0x040014BD RID: 5309
	[PublicizedFrom(EAccessModifier.Private)]
	public float actionkDuration;

	// Token: 0x040014BE RID: 5310
	[PublicizedFrom(EAccessModifier.Private)]
	public float minRange = 4f;

	// Token: 0x040014BF RID: 5311
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxRange = 25f;

	// Token: 0x040014C0 RID: 5312
	[PublicizedFrom(EAccessModifier.Private)]
	public float unreachableRange;

	// Token: 0x040014C1 RID: 5313
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> entityList = new List<Entity>();
}
