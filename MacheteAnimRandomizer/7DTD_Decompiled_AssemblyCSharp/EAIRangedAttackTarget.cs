using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003E8 RID: 1000
[Preserve]
public class EAIRangedAttackTarget : EAIBase
{
	// Token: 0x06001E49 RID: 7753 RVA: 0x000BCFA8 File Offset: 0x000BB1A8
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 11;
		this.cooldown = 3f;
		this.attackDuration = 20f;
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x000BCFD0 File Offset: 0x000BB1D0
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "itemType", ref this.itemActionType);
		base.GetData(data, "startAnimType", ref this.startAnimType);
		base.GetData(data, "releaseDelay", ref this.releaseDelay);
		base.GetData(data, "cooldown", ref this.baseCooldown);
		base.GetData(data, "duration", ref this.attackDuration);
		base.GetData(data, "minRange", ref this.minRange);
		base.GetData(data, "maxRange", ref this.maxRange);
		base.GetData(data, "unreachableRange", ref this.unreachableRange);
		data.TryGetValue("sndStart", out this.soundStartName);
		data.TryGetValue("sndRelease", out this.soundReleaseName);
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x000BD098 File Offset: 0x000BB298
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
		if (!this.theEntity.IsAttackValid())
		{
			return false;
		}
		this.entityTarget = this.theEntity.GetAttackTarget();
		return this.entityTarget && !this.entityTarget.IsDead() && !this.theEntity.bodyDamage.IsAnyLegMissing && (this.startAnimType < 0 || !this.theEntity.bodyDamage.IsAnyArmOrLegMissing) && this.InRange() && this.theEntity.CanSee(this.entityTarget);
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000BD160 File Offset: 0x000BB360
	public override void Start()
	{
		this.theEntity.emodel.avatarController.hitWeightMax = 0.5f;
		this.painHitsFelt = this.theEntity.painHitsFelt;
		this.elapsedTime = 0f;
		this.state = EAIRangedAttackTarget.State.Attack;
		this.stateTime = 0f;
		if (this.startAnimType >= 0)
		{
			this.state = EAIRangedAttackTarget.State.StartAnim;
			this.theEntity.StartAnimAction(this.startAnimType + 3000);
		}
		if (!string.IsNullOrEmpty(this.soundStartName))
		{
			Manager.BroadcastPlay(this.theEntity, this.soundStartName, false);
		}
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x000BD1FC File Offset: 0x000BB3FC
	public override bool Continue()
	{
		return this.entityTarget && !this.entityTarget.IsDead() && this.elapsedTime < this.attackDuration && this.theEntity.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.theEntity.Electrocuted && (this.startAnimType < 0 || !this.theEntity.bodyDamage.IsAnyArmOrLegMissing) && this.theEntity.painHitsFelt - this.painHitsFelt < 1f;
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x000BD28C File Offset: 0x000BB48C
	public override void Update()
	{
		this.elapsedTime += 0.05f;
		if (this.elapsedTime < this.attackDuration * 0.5f)
		{
			Vector3 headPosition = this.entityTarget.getHeadPosition();
			if (this.theEntity.IsInFrontOfMe(headPosition))
			{
				this.theEntity.SetLookPosition(headPosition);
			}
			this.theEntity.SeekYawToPos(this.entityTarget.position, 30f);
		}
		this.stateTime += 0.05f;
		if (this.state == EAIRangedAttackTarget.State.StartAnim)
		{
			if (this.theEntity.GetAnimActionState() != AvatarController.ActionState.Ready)
			{
				return;
			}
			this.theEntity.ContinueAnimAction(this.startAnimType + 1 + 3000);
			this.state = EAIRangedAttackTarget.State.ReleaseAnim;
			this.stateTime = 0f;
			if (!string.IsNullOrEmpty(this.soundReleaseName))
			{
				Manager.BroadcastPlay(this.theEntity, this.soundReleaseName, false);
			}
		}
		if (this.state == EAIRangedAttackTarget.State.ReleaseAnim)
		{
			if (this.stateTime < this.releaseDelay)
			{
				return;
			}
			this.state = EAIRangedAttackTarget.State.Attack;
			this.stateTime = 0f;
		}
		this.theEntity.UseHoldingItem(this.itemActionType, false);
		if (!this.theEntity.IsHoldingItemInUse(this.itemActionType))
		{
			this.elapsedTime = float.MaxValue;
		}
	}

	// Token: 0x06001E4F RID: 7759 RVA: 0x000BD3D0 File Offset: 0x000BB5D0
	public override void Reset()
	{
		this.theEntity.ShowHoldingItem(false);
		this.theEntity.UseHoldingItem(this.itemActionType, true);
		this.theEntity.StartAnimAction(9999);
		this.theEntity.SetLookPosition(Vector3.zero);
		this.theEntity.emodel.avatarController.hitWeightMax = 1f;
		this.entityTarget = null;
		this.cooldown = this.baseCooldown + this.baseCooldown * 0.5f * base.RandomFloat;
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x000BD460 File Offset: 0x000BB660
	[PublicizedFrom(EAccessModifier.Private)]
	public bool InRange()
	{
		float distanceSq = this.entityTarget.GetDistanceSq(this.theEntity);
		if (this.unreachableRange > 0f)
		{
			EntityMoveHelper moveHelper = this.theEntity.moveHelper;
			if (moveHelper.IsUnreachableAbove || moveHelper.IsUnreachableSide)
			{
				return distanceSq <= this.unreachableRange * this.unreachableRange;
			}
		}
		return distanceSq >= this.minRange * this.minRange && distanceSq <= this.maxRange * this.maxRange;
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x000BD4E4 File Offset: 0x000BB6E4
	public override string ToString()
	{
		bool flag = this.entityTarget && this.InRange();
		return string.Format("{0} {1}, inRange{2}, Time {3}", new object[]
		{
			base.ToString(),
			this.entityTarget ? this.entityTarget.EntityName : "",
			flag,
			this.elapsedTime.ToCultureInvariantString("0.00")
		});
	}

	// Token: 0x040014DB RID: 5339
	[PublicizedFrom(EAccessModifier.Private)]
	public int itemActionType;

	// Token: 0x040014DC RID: 5340
	[PublicizedFrom(EAccessModifier.Private)]
	public int startAnimType = -1;

	// Token: 0x040014DD RID: 5341
	[PublicizedFrom(EAccessModifier.Private)]
	public float releaseDelay = 0.5f;

	// Token: 0x040014DE RID: 5342
	[PublicizedFrom(EAccessModifier.Private)]
	public float baseCooldown;

	// Token: 0x040014DF RID: 5343
	[PublicizedFrom(EAccessModifier.Private)]
	public float cooldown;

	// Token: 0x040014E0 RID: 5344
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x040014E1 RID: 5345
	[PublicizedFrom(EAccessModifier.Private)]
	public float attackDuration;

	// Token: 0x040014E2 RID: 5346
	[PublicizedFrom(EAccessModifier.Private)]
	public float elapsedTime;

	// Token: 0x040014E3 RID: 5347
	[PublicizedFrom(EAccessModifier.Private)]
	public float minRange = 4f;

	// Token: 0x040014E4 RID: 5348
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxRange = 25f;

	// Token: 0x040014E5 RID: 5349
	[PublicizedFrom(EAccessModifier.Private)]
	public float unreachableRange;

	// Token: 0x040014E6 RID: 5350
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundStartName;

	// Token: 0x040014E7 RID: 5351
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundReleaseName;

	// Token: 0x040014E8 RID: 5352
	[PublicizedFrom(EAccessModifier.Private)]
	public EAIRangedAttackTarget.State state;

	// Token: 0x040014E9 RID: 5353
	[PublicizedFrom(EAccessModifier.Private)]
	public float stateTime;

	// Token: 0x040014EA RID: 5354
	[PublicizedFrom(EAccessModifier.Private)]
	public float painHitsFelt;

	// Token: 0x020003E9 RID: 1001
	[PublicizedFrom(EAccessModifier.Private)]
	public enum State
	{
		// Token: 0x040014EC RID: 5356
		StartAnim,
		// Token: 0x040014ED RID: 5357
		ReleaseAnim,
		// Token: 0x040014EE RID: 5358
		Attack,
		// Token: 0x040014EF RID: 5359
		Release
	}
}
